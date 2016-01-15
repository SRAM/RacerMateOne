using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using System.Diagnostics;       // Needed for process invocation
using Microsoft.Win32;
using System.ComponentModel; // CancelEventArgs
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Security.Permissions;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RacerMateOne.Dialogs;

namespace RacerMateOne
{
	#region GPSDATA
	public struct GPSData
	{
		public uint frame;			// real frame number
		public int real;						// 1 if real data, 0 if interpolated data
		public int seconds;
		public double lat;						// degrees
		public double lon;						// degrees
		public double unfiltered_elev;		// raw garmin elevation
		public double filtered_elev;		// filtered garmin elevation
		public double manelev;				// manually entered elevation

		public double accum_meters1;		// from the garmin.txt file
		public double accum_meters2;		// computed by me

		public double section_meters1;		// garmin's version
		public double section_meters2;		// computed by me

		public double pg;						// percent grade
		public double mps1;					// based on garmin distances
		public double mph1;					// based on garmin distances
		public double mps2;					// based on computed distances
		public double mph2;					// based on computed distances
		public double faz;						// forward azimuth
		public double seconds_offset;
		public double x;						// in meters, used to create the csv file
		public double y;						// in meters, used to create the csv file
		public double z;						// in meters, used to create the csv file same as manelev but in meters
	};
	#endregion

	[Flags]
	public enum CourseType
	{
		Zero = 0,
		Distance = 0x01,
		Watts = 0x02,
		Video = 0x04,
		ThreeD = 0x08,
		GPS = 0x10,
		Performance = 0x20
	};
	[Flags]
	public enum CourseAttributes
	{
		Zero = 0,
		Mirror = 0x01,
		Reverse = 0x02,
		Looped = 0x04,
		Modified = 0x08,
		OutAndBack = 0x10,
		Performance = 0x20
	};

	public enum CourseXUnits: byte
	{
		Distance,
		Time
	};
	public enum CourseYUnits: byte
	{
		Grade,
		Watts,
		PercentAT
	};

	public class CourseFilter : ICloneable
	{
		String m_Name;
		String m_Key;
		public String Key { get { return m_Key; } }
		CourseType m_Type;
		CourseXUnits m_XUnit;
		CourseYUnits m_YUnit;
		public CourseYUnits YUnit { get { return m_UseYUnit ? m_YUnit : CourseYUnits.Grade; } }
		public CourseXUnits XUnit
		{
			get
			{
				return m_UseXUnit ? m_XUnit : CourseXUnits.Distance; 
			}
		}
		bool m_UseType;
		bool m_UseXUnit;
		bool m_UseYUnit;
		bool m_NoPerformance = false;

		public String Column1 
		{ 
			get 
			{
				return m_UseYUnit && m_YUnit != CourseYUnits.Grade ? "Interval" : "Laps"; 
			} 
		}
		public String Column2 
		{ 
			get 
			{ 
				return !m_UseXUnit || m_XUnit == CourseXUnits.Distance ? ConvertConst.TextDistanceLabel : "Minutes"; 
			} 
		}
		public String Column3 
		{ 
			get 
			{ 
				return m_UseYUnit && m_YUnit != CourseYUnits.Grade ? "Watts" : "Elev"; 
			} 
		}

		public bool ManualAvailable { get { return !(m_UseType && ((m_Type & CourseType.Performance) != CourseType.Zero)); }}

		static Dictionary<String, CourseFilter> ms_Filters = new Dictionary<string,CourseFilter>();
		public static CourseFilter Find( String name )
		{
			CourseFilter cf;
			if (ms_Filters.TryGetValue(name.ToLower(),out cf))
				return cf;
			return null;
		}


		public override string ToString()
		{
			return m_Name;
		}
		CourseFilter() { }
		CourseFilter(String key, String name, CourseXUnits xunit, CourseYUnits yunits, bool noperformance )
		{
			m_Key = key.ToLower();
			m_Name = name;
			m_XUnit = xunit;
			m_YUnit = yunits;
			m_UseXUnit = m_UseYUnit = true;
			m_NoPerformance = noperformance;
			ms_Filters[m_Key] = this;
		}
		CourseFilter(String key, String name, CourseType type, bool noperformance)
		{
			m_Key = key.ToLower();
			m_Name = name;
			m_Type = type;
			m_UseType = true;
			m_NoPerformance = noperformance;
			if (!m_NoPerformance && type == CourseType.Performance)
			{
				m_UseType = false;
				Add(FilterBy.Performance, null);
			}
			ms_Filters[m_Key] = this;
		}
		CourseFilter(String key, String name, CourseType type, CourseYUnits yunits, bool noperformance)
		{
			m_Key = key.ToLower();
			m_Name = name;
			m_Type = type;
			m_YUnit = yunits;
			m_UseType = m_UseYUnit = true;
			m_NoPerformance = noperformance;
			if (!m_NoPerformance && type == CourseType.Performance)
			{
				m_UseType = false;
				Add(FilterBy.Performance, null);
			}
			ms_Filters[m_Key] = this;
		}
		CourseFilter(String key, String name, CourseType type, CourseXUnits xunit, CourseYUnits yunits, bool noperformance)
		{
			m_Key = key.ToLower();
			m_Name = name;
			m_Type = type;
			m_XUnit = xunit;
			m_YUnit = yunits;
			m_UseType = m_UseXUnit = m_UseYUnit = true;
			m_NoPerformance = noperformance;
			if (!m_NoPerformance && type == CourseType.Performance)
			{
				m_UseType = false;
				Add(FilterBy.Performance, null);
			}
			ms_Filters[m_Key] = this;
		}
		public bool InFilter(Course c) { return InFilter(c, false); }
		public bool InFilter(Course c, bool perf_ok)
		{
			if (c == null)
				return false;
			bool ans = (!m_UseType || ((c.Type & m_Type) != CourseType.Zero)) &&
				(!m_UseXUnit || (c.XUnits == m_XUnit)) &&
				(!m_UseYUnit || (c.YUnits == m_YUnit)) &&
				(!m_NoPerformance || ((c.Type & CourseType.Performance) == CourseType.Zero) || perf_ok);
			if (ans && m_ExFilter != null)
			{
				String t,r;
				foreach (ExFilter f in m_ExFilter)
				{
					switch (f.Type)
					{
						case FilterBy.CourseHash:
							ans = String.Compare(c.CourseHash, f.Data.ToString()) == 0;
							break;
						case FilterBy.HeaderHash:
							ans = String.Compare(c.HeaderHash, f.Data.ToString()) == 0;
							break;
						case FilterBy.Favorite:
							ans = c.Favorite;
							break;
						case FilterBy.CourseName:
							ans = String.Compare(c.Name, f.Data.ToString(), true) == 0;
							break;
						case FilterBy.RiderName:
							if (c.PerformanceHeader != null)
							{
								t = f.Data.ToString();
								r = c.PerformanceInfo.RiderName.Substring(0, t.Length);
								ans = String.Compare(t, r, true) == 0;
							}
							else
								ans = false;
							break;
						case FilterBy.Performance:
							ans = c.PerformanceHeader != null;
							break;
						case FilterBy.XUnits:
							ans = (CourseXUnits)f.Data == c.XUnits;
							break;
						case FilterBy.YUnits:
							ans = (CourseYUnits)f.Data == c.YUnits;
							break;
						case FilterBy.Search:
							{
								ans = true;
								Regex reg = f.Data as Regex;
								if (reg == null) { ans = false; break; }

								Match m = reg.Match(c.Name);
								if (m.Success) break;

								if (c.PerformanceHeader != null)
								{
									m = reg.Match(c.PerformanceInfo.RiderName);
									if (m.Success) break;

									m = reg.Match(c.PerformanceHeader.Date.ToString());
									if (m.Success) break;
								}
								ans = false;
							}
							break;
						case FilterBy.Video:
							{
								Course cc = c.OriginalCourse;
								cc = cc == null ? c : cc;
								ans = (cc.Type & CourseType.Video) != CourseType.Zero;
							}
							break;
					}
					if (!ans)
						break;
				}
			}
			return ans;
		}
		
		public bool InFilter(CourseInfo c)
		{
			bool ans = (!m_UseType || ((c.Type & m_Type) != CourseType.Zero)) &&
				(!m_UseXUnit || (c.XUnits == m_XUnit)) &&
				(!m_UseYUnit || (c.YUnits == m_YUnit)) &&
				(!m_NoPerformance || ((c.Type & CourseType.Performance) == CourseType.Zero));
			return ans;
		}

		public Object Clone()
		{
			CourseFilter f = new CourseFilter();
			return CopyTo(f);
		}

		public CourseFilter CopyTo( CourseFilter f )
		{
			f.m_Name = m_Name;
			f.m_Key = m_Key+".m";
			f.m_Type = m_Type;
			f.m_XUnit = m_XUnit;
			f.m_YUnit = m_YUnit;
			f.m_UseXUnit = m_UseXUnit;
			f.m_UseYUnit = m_UseYUnit;
			f.m_UseType = m_UseType;
			f.m_NoPerformance = m_NoPerformance;
			if (m_ExFilter != null)
				f.m_ExFilter = new List<ExFilter>(m_ExFilter);
			return f;
		}
		//=============================================
		// Special data
		public enum FilterBy
		{
			CourseHash,
			HeaderHash,
			Favorite,
			CourseName,
			RiderName,
			Performance,
			XUnits,
			YUnits,
			Search,
			Video
		};
		struct ExFilter
		{
			public FilterBy Type;
			public Object Data;
			public ExFilter(FilterBy type, Object data)
			{
				Type = type;
				Data = data;
			}
		};
		List<ExFilter> m_ExFilter;
		//=============================================
		public static CourseFilter F_DistanceGrade = new CourseFilter( "Dist.Grade", "Course (Dist / Grade)", CourseXUnits.Distance, CourseYUnits.Grade, true );
		public static CourseFilter F_TimeGrade = new CourseFilter("Time.Grade", "Course (Time / Grade)", CourseXUnits.Time, CourseYUnits.Grade, true);
		public static CourseFilter F_TimeWatts = new CourseFilter("Time.Watts", "Erg-mode (Time / Watts)", CourseXUnits.Time, CourseYUnits.Watts, true);
		public static CourseFilter F_DistanceWatts = new CourseFilter( "Dist.Watts", "Erg-mode (Dist / Watts)", CourseXUnits.Distance, CourseYUnits.Watts, true );
		public static CourseFilter F_TimePercentAT = new CourseFilter( "Time.Percent","Erg-Mode (Time / %AT)", CourseXUnits.Time, CourseYUnits.PercentAT, true );
		public static CourseFilter F_DistancePercentAT = new CourseFilter( "Dist.Percent","Erg-Mode (Dist / %AT)", CourseXUnits.Distance, CourseYUnits.PercentAT, true );
		public static CourseFilter F_PerformanceGrade = new CourseFilter( "Performance.Grade", "Performance (Grade)", CourseType.Performance, CourseYUnits.Grade, false);
		public static CourseFilter F_PerformanceWatts = new CourseFilter( "Performance.Watts", "Performance (Watts)", CourseType.Performance, CourseYUnits.Watts, false );

		public static CourseFilter F_RCV = new CourseFilter("RCV", "RCV", CourseType.Video, true );
		//=============================================
		public void Add( FilterBy f, Object data )
		{
			if (m_ExFilter == null)
				m_ExFilter = new List<ExFilter>();
			m_ExFilter.Add( new ExFilter( f, data ) );
		}
	}

	public class CourseInfo
	{
		public CourseType Type;
		public String FileName;
		public double StartAt;		// In Meters	
		public double EndAt;		// In Meters
		public bool Mirror;
		public bool Reverse;
		public CourseXUnits XUnits;
		public CourseYUnits YUnits;
        public String HeaderHash;
        public String CourseHash;


		private int m_Laps;
		public int Laps { get { return m_Laps; } set { m_Laps = value < 1 ? 1 : value; } }

		public CourseInfo() { }
		public CourseInfo(Course c)
		{
			Type = c.Type;
			FileName = c.FileName;
			StartAt = c.StartAt;
			EndAt = c.EndAt;
			Mirror = c.Mirror;
			Reverse = c.Reverse;
			Laps = c.Laps;
			XUnits = c.XUnits;
			YUnits = c.YUnits;
			CourseHash = c.CourseHash;
			HeaderHash = c.HeaderHash;
        }

		public bool Load(XElement elem)
		{
			if (elem.Name != "CourseInfo")
				return false;
			XAttribute att;
			if ((att = elem.Attribute("FileName")) == null)
				return false;
			FileName = att.Value;

			if ((att = elem.Attribute("Type")) == null)
				return false;
			try { Type = (CourseType)Enum.Parse(typeof(CourseType), att.Value); }
			catch { return false;  }

			if ((att = elem.Attribute("StartAt")) != null)
				try { StartAt = Convert.ToDouble(att.Value); }
				catch { StartAt = 0.0; }

			if ((att = elem.Attribute("EndAt")) != null)
				try { EndAt = Convert.ToDouble(att.Value); }
				catch { EndAt = double.NaN; }

			if ((att = elem.Attribute("Mirror")) != null)
				try { Mirror = Convert.ToBoolean(att.Value); }
				catch { Mirror = false; }

			if ((att = elem.Attribute("Reverse")) != null)
				try { Reverse = Convert.ToBoolean(att.Value); }
				catch { Reverse = false; }

			if ((att = elem.Attribute("Laps")) != null)
				try { Laps = Convert.ToInt32(att.Value); }
				catch { Laps = 1; }
			
            if ((att = elem.Attribute("CourseHash")) == null)
                return false;
            CourseHash = att.Value;

            if ((att = elem.Attribute("HeaderHash")) == null)
                return false;
            HeaderHash = att.Value;

            return true;
		}

		public XElement ToXElement( String key )
		{
			XElement e = new XElement("CourseInfo");
			if (key != null)
				e.Add(new XAttribute("Key", key ));
			e.Add( 
				new XAttribute("FileName", FileName),
				new XAttribute("Type", Type.ToString()),
				new XAttribute("StartAt", StartAt),
				new XAttribute("EndAt", EndAt),
				new XAttribute("Mirror", Mirror),
				new XAttribute("Reverse", Reverse),
				new XAttribute("Laps", Laps ),
                new XAttribute("CourseHash", CourseHash),
                new XAttribute("HeaderHash", HeaderHash)
				);
			return e;
		}

		public XElement ToXElement()
		{
			return ToXElement(null);
		}


		public Course Course
		{
			get
			{
				Course c;
				if (!Courses.TrackDB.TryGetValue( CourseHash, out c ))
				{
					c = new Course();
					if (!c.Load(FileName))
						return null;
				}
				c.Set(this);
				return c;
			}
		}


	}


	#region ThreeDCCourse
	public class ThreeDCCourse
	{
		public struct Segment
		{
			public double Length;		// In Meters
			public double StartGrade;		// ratio over time 0.01 = 1% grade
			public double EndGrade;		// ratio over time 0.01 = 1% grade
			public double Wind;			// In Meters per second.
            public double Rot;
			public double StartY;
			public double EndY;
			public int Divisions;
        };


		[DllImport("RM1_Ext.dll")]
		private static extern IntPtr Load3DCCourse(IntPtr filename);
		[DllImport("RM1_Ext.dll")]
		private static extern void Free3DCCourse(IntPtr ptr);
		[DllImport("RM1_Ext.dll")]
		private static extern int Get3DCCourseSegments(IntPtr ptr);
		[DllImport("RM1_Ext.dll")]
		private static extern Segment Get3DCCourseSegment(IntPtr ptr, int num);
        [DllImport("RM1_Ext.dll")]
        private static extern int IsCourseClosedLoop(IntPtr ptr);
        [DllImport("RM1_Ext.dll")]
        private static extern int GetLaps(IntPtr ptr);

		private IntPtr m_Ptr = IntPtr.Zero;
		public bool Open(String filename)
		{
			IntPtr p_filename = Marshal.StringToHGlobalAnsi(filename);

			m_Ptr = Load3DCCourse(p_filename);
			Marshal.FreeHGlobal(p_filename);
			if (m_Ptr == null)
				return false;
			return true;
		}
		~ThreeDCCourse()
		{
			Close();
		}
		public void Close()
		{
			if (m_Ptr != IntPtr.Zero)
			{
				Free3DCCourse(m_Ptr);
				m_Ptr = IntPtr.Zero;
			}
		}

        public bool IsClosedLooped()
        {
            if (m_Ptr != IntPtr.Zero)
            {
                return IsCourseClosedLoop(m_Ptr) > 0 ? true : false;
            }
            return false;
        }

		public int Segments { get { return Get3DCCourseSegments(m_Ptr); } }

        public int Laps { get { return GetLaps(m_Ptr); } }

		public Segment this[int index]
		{
			get { return Get3DCCourseSegment(m_Ptr, index); }
		}


	}
	#endregion ThreeDCourse

	#region RCVCourseData
	public class RCVCourseData
	{
		[DllImport("RM1_Ext.dll")] private static extern Int64 GetChallengeID();
		[DllImport("RM1_Ext.dll")] private static extern IntPtr LoadCourseData(IntPtr filename, Int64 verify);
		[DllImport("RM1_Ext.dll")] private static extern void FreeCourseData(IntPtr ptr );
		[DllImport("RM1_Ext.dll")] private static extern int GetCourseSegments(IntPtr ptr);
		[DllImport("RM1_Ext.dll")] private static extern GPSData GetCourseSegment(IntPtr ptr, int num);
		[DllImport("RM1_Ext.dll")] private static extern int IsRCVRegistered(IntPtr name, IntPtr inifilename);

		private IntPtr m_Ptr = IntPtr.Zero;

		public bool Open(String filename)
		{
			IntPtr p_filename = Marshal.StringToHGlobalAnsi(filename);

			Int64 cid = GetChallengeID();
			Int64 verify = ((4294967291 % cid) + 17283884);
			m_Ptr = LoadCourseData(p_filename, verify);
			Marshal.FreeHGlobal(p_filename);
			if (m_Ptr == null)
				return false;
			return true;
		}
		~RCVCourseData()
		{
			Close();
		}
		public void Close()
		{
			if (m_Ptr == IntPtr.Zero)
			{
				FreeCourseData(m_Ptr);
				m_Ptr = IntPtr.Zero;
			}
		}
		public int Segments { get { return GetCourseSegments(m_Ptr); } }

		public GPSData this[int index]
		{
			get { return GetCourseSegment(m_Ptr, index); }
		}

		public String RegisteredError;
		public bool Registered(Course course)
		{
			// Find the INI file
			bool reg = false;
			RegisteredError = null;
			try
			{
				String inipath = System.IO.Path.Combine( RM1_Settings.General.Path_BaseRCV, "vid.ini");
				if (!File.Exists(inipath))
				{
					inipath = System.IO.Path.Combine( RM1_Settings.General.Path_RCV, "vid.ini");
					if (!File.Exists(inipath))
					{
						inipath = System.IO.Path.GetDirectoryName( course.FileName );
						inipath = System.IO.Path.GetDirectoryName( inipath );
						inipath = System.IO.Path.Combine(RM1_Settings.General.Path_RCV, "vid.ini");
						if(!File.Exists(inipath))
						{
							RegisteredError = "Couldn't find vid.ini file";
							return false;
						}
					}
				}
				IntPtr p_name = Marshal.StringToHGlobalAnsi(course.Name);
				IntPtr p_inipath = Marshal.StringToHGlobalAnsi(inipath);
				reg = IsRCVRegistered(p_name, p_inipath) != 0;
				Marshal.FreeHGlobal(p_inipath);
				Marshal.FreeHGlobal(p_name);

			}
			catch (Exception ex)
			{
				RegisteredError = ex.ToString();
			}
			return reg;
		}

	}
	#endregion

	#region enum Measurement
	/// <summary>
	/// Types of mesurements the courses can accept.
	/// </summary>
	public enum Measurement
	{
		Meters,
		Kilometers,
		Feet,
		Miles
	};
	#endregion

	#region Courses
	/// <summary>
	/// All the courses
	/// </summary>
	public class Courses
	{
		public static Dictionary<String, Course> TrackDB = new Dictionary<string, Course>();
		public static Dictionary<String, Course> FileDB = new Dictionary<string, Course>();
		public static Dictionary<String, Course> HeaderDB = new Dictionary<string, Course>();

		#region Lists
		public static ObservableCollection<Course> AllList = new ObservableCollection<Course>();

		public static Course FirstVideoCourse
		{
			get
			{
				foreach (Course c in AllCourses)
				{
					Course cc = c.VideoCourse;
					if (cc != null)
						return cc;
				}
				return null;
			}
		}

		//public static ObservableCollection<Course> ThreeDList = new ObservableCollection<Course>();
		//public static ObservableCollection<Course> DistanceList = new ObservableCollection<Course>();
		//public static ObservableCollection<Course> WattsList = new ObservableCollection<Course>();
		//public static ObservableCollection<Course> VideoList = new ObservableCollection<Course>();
		//public static ObservableCollection<Course> GPSList = new ObservableCollection<Course>();

		#endregion

        [DllImport("RM1_Ext.dll")]
        private static extern void InitRM1ExtDLL();

        static Thread ms_OtherThread;
        public static bool ms_WaitForOtherThread = true;
        public static void InitOtherThread()
        {
            //try
            //{
                if (ms_OtherThread != null)
                    ms_OtherThread.Join();		// We will have to wait.
                OtherThread(); //Paul says no threads for safety please.
                //ms_OtherThread = new Thread(new ThreadStart(OtherThread));
                //ms_OtherThread.Start();
                
            //}
            //catch
            //{
            //    Log.WriteLine("Starting InitRM1_ExtDLL threw an exception handled by user code");
            //}
        }

        public static void OtherThread()
        {
            App.SetDefaultCulture();

            Log.WriteLine("Initializing 3D Start");
            InitRM1ExtDLL();
            ms_WaitForOtherThread = false;
            Log.WriteLine("Initializing 3D Done");
            /*
            //Debug.WriteLine(string.Format("{0} - Worker Scanning start", DateTime.Now));
            Log.WriteLine("Starting Course Scan");

            foreach (KeyValuePair<String, CourseInfo> kv in RM1_Settings.General.SelectedCourse)
            {
                AddScanFile(kv.Value.FileName);
            }

            Scan(RacerMatePaths.RCVFullPath);
            Scan(RacerMatePaths.CommonCoursesFullPath);
            foreach (string dir in Directory.GetDirectories(RacerMatePaths.CommonCoursesFullPath))
            {
                Scan(System.IO.Path.Combine(RacerMatePaths.CommonCoursesFullPath, dir));
            }
            Scan(RacerMatePaths.CoursesFullPath);
            //Scan(RacerMatePaths.CoursesFullPath + "\\3DC");
            foreach (string dir in Directory.GetDirectories(RacerMatePaths.CoursesFullPath))
            {
                Scan(System.IO.Path.Combine(RacerMatePaths.CoursesFullPath, dir));
            }
            ScanVideos();
            Log.WriteLine("Initializing Button Boxes");
            */
            ms_OtherThread = null;
            //Debug.WriteLine(string.Format("{0} - Worker Scanning running in thread", DateTime.Now));
        }

        
        private static List<String> ms_Scanned = new List<String>();
		private static List<Course> m_Courses = new List<Course>();
		private static bool m_sorted = false;

		private static bool ms_bFirstScan;

		public static Course Find(CourseInfo cinfo, ObservableCollection<Course> list)
		{
			if (list == null)
				list = AllList;
			foreach (Course c in list)
			{
				if (String.Compare(cinfo.FileName, c.FileName, true) == 0)
					return c;
			}
			return null;
		}



		public static int cmp_CourseNames( Course c1, Course c2 )
		{
			return String.Compare(c1.Name,c2.Name,true);
		}
		public static List<Course> AllCourses
		{
			get
			{
				if (!m_sorted)
				{
                    //Debug.WriteLine("AllCourses sort");
                    m_Courses.Sort(cmp_CourseNames);
					m_sorted = true;
				}
				return m_Courses;
			}
		}

		public delegate void LoadPerformanceDone();
		public static event LoadPerformanceDone OnLoadPerformanceDone;

		static BackgroundWorker ms_PerformanceBackground;
		public static void CancelLoadPerformance()
		{
			if (ms_PerformanceBackground != null)
			{
				ms_PerformanceBackground.CancelAsync();
				ms_PerformanceBackground = null;
				if (OnLoadPerformanceDone != null)
					OnLoadPerformanceDone();
			}
		}
		public static void LoadPerformances()
		{
			if (ms_PerformanceBackground != null)
				return;
			BackgroundWorker bw = new BackgroundWorker();
			ms_PerformanceBackground = bw;
			bw.DoWork += new DoWorkEventHandler(bw_Performance_ScanWork);
			bw.WorkerSupportsCancellation = true;
			bw.WorkerReportsProgress = true;
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_Performance_RunWorkerCompleted);
			bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
			bw.RunWorkerAsync(RacerMatePaths.PerformancesFullPath);
		}
		public static Dictionary<String, Course> ms_CheckedPerformances = new Dictionary<string, Course>();

		public static bool NeedSortPerformanceCourses;
		public static ObservableCollection<String> PerformanceCourses = new ObservableCollection<String>();

		public static void SortPerformanceCourses()
		{
			if (NeedSortPerformanceCourses)
			{
				NeedSortPerformanceCourses = false;
				List<String> list = new List<String>(PerformanceCourses);
				list.Sort();
				PerformanceCourses.Clear();
				foreach(String s in list)
					PerformanceCourses.Add( s );

			}
		}


		private static void bw_Performance_ScanWork(object sender, DoWorkEventArgs e)
		{
            App.SetDefaultCulture();

            BackgroundWorker worker = sender as BackgroundWorker;
			String dir = (String)e.Argument;

			int cnt = 0;
			string[] files = Directory.GetFiles(dir, "*.RMP");
			int total = files.Length;

			// Lets go through and sort these in Date order.
			List<String> sfiles = new List<string>(files);
			sfiles.Sort();
			sfiles.Reverse();

			foreach (string filename in sfiles)
			{
				String lkey = filename.ToLower();
				Course c;
				if (!ms_CheckedPerformances.TryGetValue(lkey, out c))
				{
					// OK we don't have it yet... let try loading up the performance.
					c = new Course();
					if (c.LoadWithReducedSegments(filename, true))
					{
						c.fix();
						worker.ReportProgress(cnt * 100 / total, c);
						ms_CheckedPerformances[lkey] = c;
					}
					else
						ms_CheckedPerformances[lkey] = null;
					Thread.Sleep(5);
				}
				if (worker.CancellationPending)
				{
					return;
				}
			}
		}
		private static void bw_Performance_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (sender == ms_PerformanceBackground)
				ms_PerformanceBackground = null;
			if (OnLoadPerformanceDone != null)
				OnLoadPerformanceDone();
		}

		static LinkedList<String> ms_ScanList = new LinkedList<String>();
		static int ms_ScanTotal = 0;
		static int ms_ScanCount = 0;
		static BackgroundWorker ms_ScanBW;

		static HashSet<string> ms_ScanListSave = new HashSet<string>();

		static Regex regexp_avi = new Regex(@"(\.avi$)",RegexOptions.IgnoreCase);
		static void addScan( String dir )
		{
			if (!ms_Scanned.Contains(dir.ToLower()))
			{
				ms_Scanned.Add(dir);

				string[] dirs = Directory.GetDirectories(dir);
				foreach( string d in dirs )
				{
					addScan( d );
				}

				string[] avifiles = Directory.GetFiles(dir, "*.avi");
				HashSet<string> hs = new HashSet<string>();
				foreach( string af in avifiles )
				{
					string s = regexp_avi.Replace(af,".3dc");
					hs.Add( s.ToLower() );
				}
				string[] files = Directory.GetFiles(dir, "*.*"); 
				foreach( string f in files )
				{
					String lower = f.ToLower();
					if (!hs.Contains(lower) && !FileDB.ContainsKey(lower) && !ms_ScanListSave.Contains(lower))
					{
						ms_ScanList.AddLast(f);
						ms_ScanListSave.Add(lower);
						FileDB[lower] = null;
					}
				}
			}
		}

		public static void AddScanFile( String file )
		{
			lock (ms_ScanList)
			{
				int bnum = ms_ScanList.Count;
				String lower = file.ToLower();
				if (!FileDB.ContainsKey(lower) && !ms_ScanList.Contains(file) && !ms_ScanListSave.Contains(lower))
				{
					ms_ScanList.AddLast(file);
					ms_ScanListSave.Add(lower);
					FileDB[lower] = null;
				}
				ms_ScanTotal += ms_ScanList.Count - bnum; // Adjust the total by number added.
			}
		}


		public static void ScanVideos()
		{
			List<String> krcv = RM1_Settings.General.KnownRCV;
			lock (ms_ScanList)
			{
				int bnum = ms_ScanList.Count;
				foreach (String f in krcv)
				{
					String lower = f.ToLower();
					if (!FileDB.ContainsKey(lower) && !ms_ScanList.Contains(f))
						ms_ScanList.AddLast(f);
				}
				ms_ScanTotal += ms_ScanList.Count - bnum; // Adjust the total by number added.
			}
		}

		
		public static void Scan(String dir)
		{
			if (!ms_bFirstScan)
			{
				// This makes sure we don't scan the performance directory... that is handled else where.
				ms_Scanned.Add(RacerMatePaths.PerformancesFullPath);
				ms_bFirstScan = true;
			}
			if (dir == null)
				return;

			lock (ms_ScanList)
			{
				int bnum = ms_ScanList.Count;
				addScan( dir );	// Add all the sub directories in as well.
				ms_ScanTotal += ms_ScanList.Count - bnum; // Adjust the total by number added.

				// Now start the scannier if not alreay started.
				if (ms_ScanBW == null)
				{
                    Log.Debug("Scanning courses started");
                   // Debug.WriteLine("Scanning courses xyx");
                    BackgroundWorker bw = new BackgroundWorker();
					bw.DoWork += new DoWorkEventHandler(bw_ScanWork);

					bw.WorkerReportsProgress = true;
					bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
					bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
					bw.RunWorkerAsync(null);
					ms_ScanBW = bw;
				}
			}
		}

		static Regex regexp_lastdir = new Regex(@"[\\]+([^\\]+$)");
		private static void bw_ScanWork(object sender, DoWorkEventArgs e)
		{
            App.SetDefaultCulture();
            BackgroundWorker worker = sender as BackgroundWorker;

			while(true)
			{
				String file = null;
				int percent = 0;
                lock (ms_ScanList)
                {
                    try
                    {
                        // This is were we leave if we there are no more things to scan.
                        if (ms_ScanList.Count == 0)
                        {
                            ms_ScanBW = null;
                            if (ms_OtherThread != null)
                                ms_OtherThread = null;
                            Log.WriteLine("Courses.cs, Scanning courses finished");
                            return;
                        }
                        // Get a file and romove it from the list.
                        file = ms_ScanList.First();
                        ms_ScanList.RemoveFirst();
                    }
                    catch
                    {
                        Log.Debug(string.Format(@"Scanning course ""{0}"" failed", file));
                    }
                    ms_ScanCount++;
                    percent = ms_ScanCount * 100 / ms_ScanTotal;
                }
                if (0 < file.Length)
                {
                    Course c = new Course();
                    if (c.LoadWithReducedSegments (file, true))
                    {
						c.fix();
                        worker.ReportProgress(percent, c);
                        Thread.Sleep(5);
                    }
                }
            }
		}

		private static void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
            m_sorted = false;
            Course.ms_lowPri = false;
            Log.WriteLine("Course.cs, bw_RunWorkerCompleted() called");
        }
		private static void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Course c = (Course)e.UserState;
			Courses.Add(c, false);
			m_sorted = true;
			//m_sorted = false;
			if (OnCourseAdded != null)
			{
				OnCourseAdded(c);
			}
            //Log.WriteLine(c.Description + " " + c.TotalX + "(" + c.MinY + "," + c.MaxY + ")");

            // Debug.WriteLine(string.Format("bw_ProgressChanged Course {0} added", c.FileName));
        }
		public static void RemoveCourse(Course course)
		{
			m_Courses.Remove(course);
			AllList.Remove(course);
			//ThreeDList.Remove(course);
			//DistanceList.Remove(course);
			//WattsList.Remove(course);
			//VideoList.Remove(course);
			//GPSList.Remove(course);
		}

		public static void Replace(Course c)
		{
			if (c == null)
				return;
			foreach (Course course in m_Courses)
			{
				if (String.Compare(course.FileName, c.FileName, true) == 0)
				{
					RemoveCourse(course);
					break;
				}
			}
			Add(c);
		}

        public static void Add(Course c) { Add(c, true); }
		public static void Add( Course c, bool markSort )
		{
			RM1_Settings.General.SetVideoCourseHash(c);
			c.CheckDemo();
			
			m_Courses.Add(c);
			
            if(markSort)
                m_sorted = false;

			if (OnCourseAdded != null)
			{
				OnCourseAdded(c);
			}


			Course cc;
			if (c.PerformanceHeader == null && ((!TrackDB.TryGetValue( c.CourseHash, out cc ) || c.AVIFilename != null )))
				TrackDB[c.CourseHash ] = c;
			if (c.FileName != "")
				FileDB[c.FileName.ToLower()] = c;
			AllList.Add(c);



			/*
			if ((c.Type & CourseType.ThreeD) == CourseType.ThreeD)
				ThreeDList.Add(c);
            if ((c.Type & CourseType.Distance) == CourseType.Distance)
                DistanceList.Add(c);
            if ((c.Type & CourseType.Watts) == CourseType.Watts)
				WattsList.Add(c);
			if ((c.Type & CourseType.Video) == CourseType.Video)
				VideoList.Add(c);
			if ((c.Type & CourseType.GPS) == CourseType.GPS)
				GPSList.Add(c);
			*/
			if (c.PerformanceHeader != null)
			{
				// This is a performance... See if the track name is in the track collection.
				if (!PerformanceCourses.Contains(c.Name))
				{
					PerformanceCourses.Add(c.Name);
					if (PerformanceCourses.Count > 1)
						NeedSortPerformanceCourses = true;
				}
			}

			// Log.WriteLine(c.Description + " " + c.TotalX + "(" + c.MinY + "," + c.MaxY + ")");
		}



		public delegate void CourseAdded(Course course);
		public static event CourseAdded OnCourseAdded;

		public static void RemoveByDirectory( String path )
		{
			List<Course> m = new List<Course>();
			foreach( Course c in Courses.AllCourses )
			{
				if (String.Compare(c.Directory,path, true ) == 0)
					m.Add( c );
			}
			foreach( Course c in m )
			{
				Courses.Remove(c);
			}
		}
		public static void Remove( Course c )
		{
			m_Courses.Remove( c );
		}

		public static HashSet<String> FavoriteFiles = new HashSet<String>();
		public static bool NeedToSaveFavorites;
		public static XElement SaveFavorites()
		{
			XElement pe = new XElement("Favorites");
			foreach (String s in FavoriteFiles)
				pe.Add(new XElement("Favorite", new XAttribute("File", s)));
			return pe;
		}
		public static void LoadFavorites(XElement favoriteselem)
		{
			if (favoriteselem != null)
			{
				IEnumerable<XElement> nodelist = favoriteselem.Elements("Favorite");
				foreach (XElement ele in nodelist)
				{
					XAttribute att = ele.Attribute("File");
					if (att != null)
					{
						try
						{
							String s = ele.Value.ToString().ToLower();
							if (!FavoriteFiles.Contains(s))
								FavoriteFiles.Add(s);
						}
						catch { }
					}
				}
			}
		}

		static Courses()
		{
			PerformanceCourses.Add("");
		}


	}
	#endregion


	#region Course
	public class Course
	{
		#region Constructors

		public Course()
		{
			Version = 1;
			Name = "";
			FileName = "";
			Description = "";
			XUnits = CourseXUnits.Distance;
			YUnits = CourseYUnits.Grade;
            CommitOnly();
		}

		public Course(CourseXUnits xunits,CourseYUnits yunits, double initlength)
		{
			Version = 1;
			Name = "";
			FileName = "";
			Description = "";
			XUnits = xunits;
			YUnits = yunits;

			CommitOnly();
			if (yunits == CourseYUnits.Grade)
				AddLast(new PysicalSegment(initlength, 90, 0, 0));
			else
				AddLast(new WattsSegment(initlength, (int)100, (int)100));
		}

		public Course(Course c, bool reverse, bool mirror, bool zoom, double startat, double endat)
		{
			LinkedListNode<Segment> node;
			Segment ns;

			m_AVIFileName = c.m_AVIFileName;
			OriginalCourse = c;
			Version = 1;
			Name = c.Name;
			FileName = c.FileName;
			Directory = c.Directory;
			Description = c.Description;
			Laps = c.Laps;
            m_FixAt = null;
			StartAt = startat;
			EndAt = endat;
            // added to make sure Type is set
            Type = c.Type;
			XUnits = c.XUnits;
			YUnits = c.YUnits;
            Looped = c.Looped;
            Modified = c.Modified;

			/** I don't think if the course is modified we should keep track of the hash
            // To keep track if looped and mirrored
            if (Looped && c.Mirror)
            {
                CourseHash = c.CourseHash;

                Modified = true;
            }
            else
            {
                // Recalculate
                DataHash = null;
                Hash = null; 
            }
			 */

			// I'm going to do this the Bad but easy way... one at a time.
			if (zoom)
			{
				//Debug.WriteLine("Called in Zoom");
                if (reverse || mirror)
                {
                    //Debug.WriteLine("----------------------IN new course(parms) zoom=true, rev mirror true");
					c = new Course(c, reverse, mirror, false,startat,endat);
                    //Debug.WriteLine ("-----------------------called recursive on zoom and continuing");
                }
				c.fix();
				double s = startat;
				double e = endat;
				for (node = c.Segments.First; node != null && s >= node.Value.EndX; node = node.Next)
					;
				if (node != null && e <= node.Value.EndX)
				{
					// Single node...
					ns = (Segment)node.Value.Clone();
					ns.Length = e - s;
					AddLast(ns);
					m_FixAt = ns.Node;
				}
				else if (node != null)
				{
					ns = (Segment)node.Value.Clone();
					ns.Length = node.Value.EndX - s;
					AddLast(ns);
					m_FixAt = ns.Node;
                    node = node.Next;
					for (; node != null && e >= node.Value.EndX; node = node.Next)
					{
						ns = (Segment)node.Value.Clone();
						AddLast(ns);
					}
					if (node != null)
					{
						ns = (Segment)node.Value.Clone();
						ns.Length = e - node.Value.StartX;
						AddLast(ns);
					}
				}
				StartAt = 0.0;
				EndAt = TotalX;
			}
			else if (mirror)
			{
				//Debug.WriteLine("          Called in Mirror");
                if (reverse)
                    c = new Course(c, reverse, false, false,startat,endat);
				for (node = c.Segments.First; node != null; node = node.Next)
				{
					ns = (Segment)node.Value.Clone();
					AddLast(ns);
					if (m_FixAt == null)
						m_FixAt = ns.Node;
				}
				for (node = c.Segments.Last; node != null; node = node.Previous)
				{
					ns = (Segment)node.Value.Reverse();
					AddLast(ns);
					if (m_FixAt == null)
						m_FixAt = ns.Node;
				}
				Mirror = false;
			}
			else if (reverse)
			{
				//Debug.WriteLine("                    Called in Reverse");
                for (node = c.Segments.Last; node != null; node = node.Previous)
				{
					ns = (Segment)node.Value.Reverse();
					AddLast(ns);
					if (m_FixAt == null)
						m_FixAt = ns.Node;
				}
                //Debug.WriteLine("aaaaa" + Environment.NewLine + " I have done a reversal " + Environment.NewLine + " aaaaaaaaaa");
                //Debug.WriteLine("Original StartX: " + c.Segments.First.Value.StartX + " endX : " + c.Segments.First.Value.EndX + " StartY : " + c.Segments.First.Value.StartY + " Endy" + c.Segments.First.Value.EndY);
                //Debug.WriteLine("New StartX: " + this.Segments.First.Value.StartX + " endX : " + this.Segments.First.Value.EndX + " StartY : " + this.Segments.First.Value.StartY + " Endy" + this.Segments.First.Value.EndY);
                //Debug.WriteLine("aaaaa" + Environment.NewLine + " I have done a reversal " + Environment.NewLine + " aaaaaaaaaa");
                //Reverse = false; //Paul added this as experiment
            }
			else
			{
				Debug.WriteLine("                           Called in None");
                for (node = c.Segments.First; node != null; node = node.Next)
				{
					ns = (Segment)node.Value.Clone();
					AddLast(ns);
					if (m_FixAt == null)
						m_FixAt = ns.Node;
				}
			}
			Commit();
		}


		public Course(Course c)
		{
			OriginalCourse = c;
			Version = 1;
			Name = c.Name;
			FileName = c.FileName;
			Directory = c.Directory;
			Description = c.Description;
			Laps = c.Laps;

            m_FixAt = null;
            // added to make sure Type is set
            Type = c.Type;
            Looped = c.Looped;
            Modified = c.Modified;

			Segment ns;
			foreach (Segment s in c.Segments)
			{
				AddLast( (ns = (Segment)s.Clone()) );
				if (m_FixAt == null)
					m_FixAt = ns.Node;
			}
			CourseHash = c.CourseHash;
			Commit();
		}
		#endregion

		#region Basic Variables
		static int ms_cnt = 1;
		public readonly int ID = ms_cnt++;

		public CourseType Type { get; protected set; }

		public virtual CourseXUnits XUnits { get; protected set; }
		public virtual CourseYUnits YUnits { get; protected set; }

		Course m_OriginalCourse;
		String m_OriginalHash = null;

		public Course OriginalCourse
		{
			get
			{
				if (m_OriginalCourse == null && m_OriginalHash != null)
				{
					// OK Try to find the original course.
					Course cc;
					if (Courses.TrackDB.TryGetValue( m_OriginalHash, out cc ))
					{
						m_OriginalCourse = cc;
					}
				}
				return m_OriginalCourse;
			}
			set
			{
				m_OriginalCourse = value;
			}
		}


		public String OriginalHash
		{
			get
			{
				return OriginalCourse == null ? CourseHash : OriginalCourse.OriginalHash;
			}
		}



		public LinkedList<Segment> Segments = new LinkedList<Segment>();
		private LinkedListNode<Segment> pvm_FixAt;
        private static Object LockObject = new Object();
        public LinkedListNode<Segment> m_FixAt
        {
            get { return pvm_FixAt; }
            set
            {
                //if (Thread.CurrentThread.Name == "MainThread")
                //                   Debug.WriteLine("writing m_Fixatfrom thread : " + Thread.CurrentThread.Name);
               
                     pvm_FixAt = value;
                                 

            }
        }

		String m_Name = null;
		public String Name
		{
			get
			{
				return m_Name == null ? Description : m_Name;
			}
			protected set { m_Name = value; }
		}

		public CourseAttributes Attributes
		{
			get
			{
				return
					(Mirror ? CourseAttributes.Mirror : CourseAttributes.Zero) |
					(Reverse ? CourseAttributes.Reverse : CourseAttributes.Zero) |
					(Looped ? CourseAttributes.Looped : CourseAttributes.Zero) |
					(Modified ? CourseAttributes.Modified : CourseAttributes.Zero);
				//(OutAndBack ? CourseAttributes.OutAndBack:CourseAttributes.Zero) |
				//(Performance ? CourseAttributes.Modified:CourseAttributes.Zero) |
			}
			set
			{
				Mirror = (value & CourseAttributes.Mirror) != CourseAttributes.Zero;
				Reverse = (value & CourseAttributes.Reverse) != CourseAttributes.Zero;
				Looped = (value & CourseAttributes.Looped) != CourseAttributes.Zero;
				Modified = (value & CourseAttributes.Modified) != CourseAttributes.Zero;
			}
		}

		public bool Mirror; // Just flags.  Has nothing to do with this course.
		public bool Reverse; // Just flags.  Has nothing to do with this course.
		public double StartAt;
		public double EndAt;
		public bool Looped; // Just for 3D saving
		public bool Modified; // Just for 3D saving


		public bool OriginalMirror;
		public bool OriginalReverse;
		public double OriginalStartAt;
		public double OriginalEndAt;
		public int OriginalLaps;


		public float hdrVersion; // Just for checking in loading

		public bool Changed
		{
			get
			{
                bool chng = OriginalMirror != Mirror || OriginalReverse != Reverse || Math.Round(OriginalStartAt, 0) != Math.Round(StartAt, 0) ||
                    Math.Round(OriginalEndAt, 0) != Math.Round(EndAt, 0) || OriginalLaps != Laps;
                //Debug.WriteLine("the Chnaged flag is : " + chng.ToString());
                return chng;
 			}
		}
		public void Reset()
		{
			Mirror = OriginalMirror;
			Reverse = OriginalReverse;
			StartAt = OriginalStartAt;
			EndAt = OriginalEndAt;
			Laps = OriginalLaps;
		}
		// Calculates hash on commit 
		public void CommitOnly()
		{
			OriginalMirror = Mirror;
			OriginalReverse = Reverse;
			OriginalStartAt = StartAt;
			OriginalEndAt = EndAt;
			OriginalLaps = Laps;
		}
		public void Commit()
		{
			CommitOnly();
		}

		public void Set(CourseInfo cinfo)
		{
			Mirror = cinfo.Mirror;
			Reverse = cinfo.Reverse;
			StartAt = cinfo.StartAt;
			EndAt = cinfo.EndAt;
			CourseHash = cinfo.CourseHash;
			if (HeaderHash != cinfo.HeaderHash)
				throw new Exception("On setting the course info the hash didn't match. Logic error!!!");
			Commit();
		}

		public String FileName;
		public String Description;

		public int Laps = 1;
		public String StringLaps { get { return "" + (int)Laps; } }

		public String StringLength 
		{ 
			get 
			{ 
				if (XUnits == CourseXUnits.Distance)
					return ConvertConst.DisplayDistancekkk(TotalX);
				return String.Format("{0:F1}",TotalX / 60);
			} 
		}

		public String StringAltitude
		{
			get
			{
				if ((Type & (CourseType.Distance | CourseType.ThreeD)) != CourseType.Zero)
				{
					return String.Format("{0:F0}", ClimbY * ConvertConst.MetersToMetersOrFeet);
				}
				return String.Format("{0:F0}", MaxY);
			}
		}

/// <summary>
/// New Version that creates a course with zoom true and reports resulting ClimbY
/// </summary>
        public String StringAscentBounded()
        {
            
                if ((Type & (CourseType.Distance | CourseType.ThreeD)) != CourseType.Zero)
                {
                    //Course tempcourse = new Course(this, this.Reverse, this.Mirror, true, this.StartAt, this.EndAt);
                    //int segs = tempcourse.Segments.Count;
                    //string returnvalue = String.Format("{0:F0}", CalcClimbBounded(this.StartAt, this.EndAt) * ConvertConst.MetersToMetersOrFeet);
                    string returnvalue = String.Format("{0:F0}", AscentMetricOrMaxPowerWithLaps * ConvertConst.MetersToMetersOrFeet);
                    return returnvalue;
                }
                return String.Format("{0:F0}", AscentMetricOrMaxPowerWithLaps);
           
        }
        public String StringAscentBounded(double StartDistance, double EndDistance)
        {
            
                if ((Type & (CourseType.Distance | CourseType.ThreeD)) != CourseType.Zero)
                {
                    //Course tempcourse = new Course(this, this.Reverse, this.Mirror, true, this.StartAt, this.EndAt);
                    //int segs = tempcourse.Segments.Count;
                    string returnvalue = String.Format("{0:F0}", CalcClimbBounded(StartDistance, EndDistance) * ConvertConst.MetersToMetersOrFeet);
                    return returnvalue;
                }
                return String.Format("{0:F0}", MaxY);
            
        }
        public double GetAccomplishedClimbingInMeters(double TotalDistanceRiddenInMeters)
        {
            double returnvalue = 0;
            if ((this.EndAt-this.StartAt) == 0) return 0;
            //make sure all this is positive!
            double NumLapsCompleted = TotalDistanceRiddenInMeters / Math.Abs(this.EndAt - this.StartAt);
            int CompleteLaps = (int)NumLapsCompleted;
            double FractionalLaps = NumLapsCompleted - CompleteLaps;
            
            Course tempcoursefulllap = new Course(this, this.Reverse, this.Mirror, true, this.Mirror ? this.StartAt*2:this.StartAt , this.Mirror ? this.EndAt * 2 - 0.1 : this.EndAt - 0.1);
            double ClimbingperCompletelap = CompleteLaps * tempcoursefulllap.ClimbY;
            double ProgressinPartialLap = (this.EndAt - this.StartAt) * FractionalLaps;
            double endpointpartial = this.StartAt + ProgressinPartialLap;
            Course tempcoursePartiallap = new Course(this, this.Reverse, this.Mirror, true, this.Mirror ? this.StartAt * 2 : this.StartAt, this.Mirror ? endpointpartial * 2 - 0.1 : endpointpartial - 0.1);
            returnvalue = ClimbingperCompletelap + tempcoursePartiallap.ClimbY;
            tempcoursePartiallap = tempcoursefulllap = null;

            return returnvalue;
        }

        public String StringAscentBoundedWithUnits(double StartDistance, double EndDistance)
        {

            if ((Type & (CourseType.Distance | CourseType.ThreeD)) != CourseType.Zero)
            {
                //Course tempcourse = new Course(this, this.Reverse, this.Mirror, true, this.StartAt, this.EndAt);
                //int segs = tempcourse.Segments.Count;
                string returnvalue = ConvertConst.TextHeight(CalcClimbBounded(StartDistance, EndDistance));
                return returnvalue;
            }
            return String.Format("{0:F0}", MaxY);

        }
        public double CalcClimbBounded(double StartDistance, double EndDistance)
        {
            double returnedvalue = 0;
            //take off some distance so that the whole course is computed if mirrored.
            //Debug.WriteLine(" file in = " + System.IO.Path.GetFileName(this.FileName) + " in  segs = " + this.Segments.Count + " laps = " + this.Laps);
            //Debug.WriteLine(" climb = " + this.ClimbY);
         
            Course tempcourse = new Course(this, this.Reverse, this.Mirror, true, this.Mirror ? StartDistance*2:StartDistance , this.Mirror ? EndDistance * 2 - 0.1 : EndDistance - 0.1);
            //Debug.WriteLine(" Calc Climb Bounded in = " + System.IO.Path.GetFileName(this.FileName) + " in  segs = " + this.Segments.Count);
            //Debug.WriteLine("start distance = " + StartDistance + " End ddistance = " + EndDistance);
            //Debug.WriteLine(" Calc Climb Bounded in = " + System.IO.Path.GetFileName(this.FileName) + " in  segs = " + this.Segments.Count);
            //Debug.WriteLine(" Calc Climb Bounded out segs = " + tempcourse.Segments.Count);
            returnedvalue = tempcourse.ClimbY * this.Laps;
            tempcourse  = null; //no sense having something hang about.

            return returnedvalue;
        }


		public double Alt
		{
			get
			{
				return (Type & (CourseType.Distance | CourseType.ThreeD)) != CourseType.Zero ? ClimbY : MaxY;
			}
		}
		public int Version { get; protected set; }

        private double m_MetricDistanceOrTimeWithLaps = 0;
        private double m_AscentMetricOrMaxPowerWithLaps = 0;

        public double MetricDistanceOrTimeWithLaps { get { return m_MetricDistanceOrTimeWithLaps; } }
        public double AscentMetricOrMaxPowerWithLaps { get { return m_AscentMetricOrMaxPowerWithLaps; } }
		
        
        public enum InfoLabelTypes
		{
			TotalDistance,
			FromStartDistance,
			Grade,
			Wind,
			Altitude,
			Watts,
			PercentAT,
			TotalTime,
			FromStartTime,
   //         segval,
            Ascent,
   //         StartEnd,
		}


		List<InfoLabelTypes> m_InfoLabelTypes = new List<InfoLabelTypes>();
		public List<InfoLabelTypes> InfoLabelTypeList
		{
			get
			{
				List<String> t = InfoLabels; // Make sure things are generated.
				return m_InfoLabelTypes;
			}
		}
		List<String> m_InfoLabels = new List<String>();
		CourseXUnits m_CurIL_X;
		CourseYUnits m_CurIL_Y;
		public List<String> InfoLabels
		{
			get
			{
				if (m_InfoLabels.Count == 0 || m_CurIL_X != XUnits || m_CurIL_Y != YUnits)
				{
					m_CurIL_X = XUnits;
					m_CurIL_Y = YUnits;
					m_InfoLabels.Clear();
					m_InfoLabelTypes.Clear();
					// Redo the list based on the x and y untis.
					switch (XUnits)
					{
						case CourseXUnits.Distance:
							m_InfoLabels.Add("Distance");
							m_InfoLabels.Add("From start");
                           // m_InfoLabels.Add("segval");
                            m_InfoLabels.Add("Ascent");
                          //  m_InfoLabels.Add("Start/End");
                            m_InfoLabelTypes.Add(InfoLabelTypes.TotalDistance);
							m_InfoLabelTypes.Add(InfoLabelTypes.FromStartDistance);
                         //   m_InfoLabelTypes.Add(InfoLabelTypes.segval);
                            m_InfoLabelTypes.Add(InfoLabelTypes.Ascent);
                         //   m_InfoLabelTypes.Add(InfoLabelTypes.StartEnd);
                            break;
						case CourseXUnits.Time:
							m_InfoLabels.Add("Total time");
							m_InfoLabels.Add("From start");
							m_InfoLabelTypes.Add(InfoLabelTypes.TotalTime);
							m_InfoLabelTypes.Add(InfoLabelTypes.FromStartTime);
							break;
					}
					switch (YUnits)
					{
						case CourseYUnits.Grade:
							m_InfoLabels.Add("Grade");
							m_InfoLabels.Add("Wind");
							m_InfoLabels.Add("Altitude");
							m_InfoLabelTypes.Add(InfoLabelTypes.Grade);
							m_InfoLabelTypes.Add(InfoLabelTypes.Wind);
							m_InfoLabelTypes.Add(InfoLabelTypes.Altitude);
							break;
						case CourseYUnits.Watts:
							m_InfoLabels.Add("Watts");
							m_InfoLabelTypes.Add(InfoLabelTypes.Watts);
							break;
						case CourseYUnits.PercentAT:
							m_InfoLabels.Add("%AT");
							m_InfoLabelTypes.Add(InfoLabelTypes.PercentAT);
							break;
					}
				}
				return m_InfoLabels;
			}
		}

		protected internal bool m_bCalcMinMax = true;

		public override String ToString()
		{
			return FileName;
		}

		//=============================================
		public bool Manual { get; protected set; }
		protected float m_ManualY;
		int m_ManualGrowCount;
       // static int depthcount = 0;

		protected int m_ManualY_Version;
		protected int m_ManualY_Segment_Version;
		protected Segment m_ManualY_Segment;
		public float ManualY
		{
			get
			{
				return m_ManualY;
			}
			set
			{
              
                if (value == m_ManualY)
					return;
				m_ManualY = value;
				if (!Manual)
					return;
               
                //Debug.WriteLine(depthcount + " Adding a segment from user");

				bool bgrade = YUnits == CourseYUnits.Grade;
                if (m_ManualGrowCount > 0)
                {
                    // We need to add a new segment.
                    Segment last = Segments.Last.Value;
                    Segment s = bgrade ? new PysicalSegment(0.1, (float)(m_ManualY), 0, 0) as Segment :
                        new WattsSegment(0.1, (int)m_ManualY, (int)m_ManualY) as Segment;
                    lock (LockObject)
                    {
                        try
                        {
                            s.Course = this;
                            s.Num = Segments.Last == null ? 0 : Segments.Last.Value.Num + 1;
                            s.m_StartX = last.m_EndX;
                            if (bgrade)
                                s.m_StartY = last.m_EndY;
                            s.Node = Segments.AddLast(s);
                            s.fix();
                            m_ManualY_Segment = s;
                        }
                        catch { }
                        finally { }
                    }
                }
                else
                {
                    // Didn't grow... so we can just use this segment.
                    lock (LockObject)
                    {
                        try
                        {
                            Segment s = Segments.Last.Value;
                            if (bgrade)
                            {
                                s.m_Change = value;
                                s.m_EndY = s.m_StartY + value * s.Length;
                            }
                            else
                            {
                                s.m_StartY = value;
                                s.m_EndY = value;
                            }
                            s.fix();
                        }
                        catch { }
                        finally { }
                    }
                }

                //Debug.WriteLine(depthcount + "   Ending teh add segment");
            }
		}
		public void ManualAdvance(double dist) // Advance the track to this distance... if not already
		{
			if (dist <= TotalX || !Manual)
				return;
			Segment s = Segments.Last.Value;
            //Debug.WriteLine("manual Advance called");
            // How much do we need to grow?
			double grow = dist - TotalX;
            lock (LockObject)
            {
                try
                {
                    s.Length += grow;
                    s.m_EndY = s.m_StartY + s.Length * s.m_Change;
                    if (s.m_EndY < m_MinY)
                        m_MinY = s.m_EndY;
                    if (s.m_EndY > m_MaxY)
                        m_MaxY = s.m_EndY;
                    s.fix();
                }
                catch { }
                finally { }

            }
            m_ManualGrowCount++;
		}
		//=============================================
		public static Course CreateAsYouGo( CourseXUnits xunits, CourseYUnits yunits, Unit unit )
		{
			Course c = new Course();

			c.Name = String.Format("Manual {0} - {1}",
				xunits == CourseXUnits.Distance ? "Dist" : "Time",
				yunits == CourseYUnits.Watts ? "Watts" : yunits == CourseYUnits.Grade ? "Grade" : "%AT");

			c.XUnits = xunits;
			c.YUnits = yunits;
			// Make a small segment.
			double y = yunits == CourseYUnits.Grade ? RM1_Settings.General.GradeInitial / 100 :
				yunits == CourseYUnits.Watts ? RM1_Settings.General.WattsInitial :
				unit == null || unit.Rider == null ? 100:unit.Rider.PowerAeT * (RM1_Settings.General.PercentATInitial / 100) ;
			Segment seg = yunits == CourseYUnits.Grade ? new PysicalSegment(1, (float)y, 0, 0) as Segment : (new WattsSegment(1, (float)y, (float)y)) as Segment;

			c.AddLast( seg );
			c.Manual = true;
			c.m_ManualY = (float)y;
			c.m_ManualY_Version = 1;
			c.m_ManualY_Segment = seg;
			c.m_ManualY_Segment_Version = 0;
			c.EndAt = c.TotalX;
			c.CommitOnly();
            //Debug.WriteLine("Create as you go course");
			return c;
		}



		String m_Directory;
		public virtual String Directory
		{
			get { return m_Directory; }
			set
			{
				m_Directory = value;
			}
		}


		public double TotalX
		{
			get { fix(); return (Segments.Last == null ? 0 : Segments.Last.Value.EndX); }
		}

		double m_MinY;
		double m_MaxY;

		public double MinY 
		{
			get
            {
                lock (LockObject)
                {
                    try { calcMinMaxY(); }
                    catch { }
                    finally { }
                } 
                return m_MinY;
            }
		}
		public double MaxY
		{
			get
			{
                lock (LockObject)
                {
                    try { calcMinMaxY(); }
                    catch { }
                    finally { }

                }
                return m_MaxY;
			}
		}
		#endregion

		#region Advanced Variables
		//===============================
		// Extended stats 
		//===============================
		protected int m_ExStatsVersion=-1;
		public void CalcExStats()
		{
			if (m_ExStatsVersion == Version)
				return;
			m_ExStatsVersion = Version;
			fix();
			m_ClimbY = 0.0;
			foreach (Segment s in Segments)
			{
				double t = s.EndY - s.StartY;
				if (t > 0)
					m_ClimbY += t;
			}
		}

		protected double m_ClimbY;
		public double ClimbY
		{
			get { CalcExStats(); return m_ClimbY; }
		}
        /// <summary>
        /// Computes vertcial climbing between a start and end.
        /// </summary>
        /// <param name="StartDistance"></param>
        /// <param name="EndDistance"></param>
        /// <returns></returns>
        
        protected double m_AtHereY;
        public double GetAltHere(double distance)
        {
            double startalt = Segments.First.Value.StartY;
            if (this.YUnits == CourseYUnits.Grade)
            {
                double cumdist = 0;
                int indextoseg = 0;
                do
                {
                    cumdist = Segments.ElementAt(indextoseg).EndX;
                    indextoseg += 1;
                } while (cumdist <= distance || indextoseg >= Segments.Count - 1);
                //coming out we are either dead on distance, past it, or at the end of the course
                if (cumdist == distance)
                    m_AtHereY = Segments.ElementAt(indextoseg - 1).EndY;
                else if (indextoseg >= Segments.Count - 1)
                    m_AtHereY = Segments.Last.Value.EndY;
                else
                { //I went over must interpolate
                    try
                    {
                        double Slope = (Segments.ElementAt(indextoseg - 1).EndY - Segments.ElementAt(indextoseg - 1).StartY) / (Segments.ElementAt(indextoseg - 1).EndX - Segments.ElementAt(indextoseg - 1).StartX);
                        double proprun = distance - Segments.ElementAt(indextoseg - 1).StartX;
                        m_AtHereY = Segments.ElementAt(indextoseg - 1).StartY + Slope * proprun;

                    }
                    catch //in case of divide by zero, take the nearest y altitude
                    {
                        m_AtHereY = Segments.ElementAt(indextoseg - 1).StartY;
                    }
                }
                return m_AtHereY;
            }
            else
                return 0;
        }




		public double MinCourseX = 10.0;

		public double MinCourseXNormalized
		{
			get
			{
				fix();
				double ans = MinCourseX / TotalX;
				return ans > 1 ? 1:ans;
			}
		}

		public Run LengthFillTextBlock(TextBlock tb, Location start, Location end, int laps)
		{
			if (laps < 1)
				laps = 1;
			Run r;
			InlineCollection ic = tb.Inlines;
			ic.Clear();
			Run laprun = null;

			bool erg = YUnits != CourseYUnits.Grade;
			bool time = XUnits == CourseXUnits.Time;


			if (laps > 1)
			{
				r = new Run("Laps ");
				ic.Add(r);
				r = new Run(laps.ToString());
				laprun = r;
				r.FontWeight = FontWeights.Bold;
				ic.Add(r);
				r = new Run(time ? " Course time ":" Course length ");
				ic.Add(r);
			}
			else
			{
				r = new Run(time ? "Course time ":"Course length ");
				ic.Add(r);
			}
			r = new Run(SectionLengthText(start, end));
			r.FontWeight = FontWeights.Bold;
			ic.Add(r);
			
			if (laps > 1)
			{
				r = new Run(time ? " Total time":" Total length ");
				ic.Add(r);
				r = new Run(TotalLengthText(start,end,laps));
				r.FontWeight = FontWeights.Bold;
				ic.Add(r);
			}
			return laprun;
		}

		public Run LengthFillTextBlock(TextBlock tb, int laps)
		{
			Location start = new Course.Location(this, StartAt);
			Location end = new Course.Location(this, EndAt);
			return LengthFillTextBlock(tb, start, end, laps);
		}


		public String TotalLengthText(Location start, Location end, int laps)
		{
			double n = end.Normalized - start.Normalized;
			double len = n * this.TotalX * laps;
			if (XUnits == CourseXUnits.Time)
			{
				TimeSpan ts = new TimeSpan((long)(ConvertConst.SecondToHundredNanosecond * len));
				return String.Format("{0}h {1}m", ts.Days * 24 + ts.Hours, ts.Minutes);
			}
			return ConvertConst.MetersToDistanceString(len);
		}

        public String DisplayedLengthText()
        {
            //this is used to report to the coursepicker the saved length of the ride.
            // 
            double number = MetricDistanceOrTimeWithLaps;
            //double n = (EndAt-StartAt)/TotalX ;
            //double len = n * this.TotalX * this.Laps;
            //if (this.Mirror) len *= 2;
            if (XUnits == CourseXUnits.Time)
            {
                TimeSpan ts = TimeSpan.FromSeconds(number); //((long)(ConvertConst.SecondToHundredNanosecond * len));
                return String.Format("{0}h {1}m", ts.Days * 24 + ts.Hours, ts.Minutes);
            }
            return ConvertConst.MetersToDistanceStringWithoutUnits(number);
       
        }
        public double CourseLengthOrDurationMetersorSeconds()
        {
            //this is used to report to the coursepicker the saved length of the ride.

            double n = (EndAt - StartAt) / TotalX;
            double len = n * this.TotalX * this.Laps;
            if (this.Mirror) len *= 2;
            if (XUnits == CourseXUnits.Time)
            {
                TimeSpan ts = new TimeSpan((long)(ConvertConst.SecondToHundredNanosecond * len));
                return ts.TotalSeconds;
            }
            return len;
        }

		public String SectionLengthText(Location start, Location end)
		{
			return TotalLengthText(start, end, 1);
		}

        
		void calcMinMaxY()
		{
           // Debug.WriteLine("computeminmaxy from thread " + Thread.CurrentThread.Name );
             if (m_bCalcMinMax)
            {
                double CurrentMinY = m_MinY;
                double CurrentMaxY = m_MaxY;
                double temp_MinY = 0;
                double temp_MaxY = 0;

                //because this is shown to fail if Segments modified during processing
                // save the previous MinY maxY and have it handy for the exception processing, this will kill the 
                // realtime display "blipping"
               
                //if (Thread.CurrentThread.Name == "MainThread") Debug.WriteLine("doing compute of minMaxY " + Segments.Count + " " + Thread.CurrentThread.Name);
				m_bCalcMinMax = false;
				//fix(); //I don't know why fix??
                if (Segments.Count == 0)
                {
                    m_MinY = -0.1;
                    m_MaxY = 0.1;
                    return;
                }
                temp_MinY = Segments.First.Value.MinY;
				temp_MaxY = Segments.First.Value.MaxY;
                try
                {
                        foreach (Segment s in Segments)
                        {
                            if (s.MinY < temp_MinY)
                                temp_MinY = s.MinY;
                            if (s.MaxY > temp_MaxY)
                                temp_MaxY = s.MaxY;
                        }
                }
                  catch
                {
                    Debug.WriteLine("exception on Segments, m_MinY= " + m_MinY + " m_MaxY = " + m_MaxY + " diff = " + (m_MaxY-m_MinY).ToString() );
                    temp_MinY = CurrentMinY;
                    temp_MaxY = CurrentMaxY;
                }
                         
               
				if (temp_MinY == temp_MaxY)
				{
					temp_MinY -= 0.1;
					temp_MaxY += 0.1;
				}
             // update the object private here, 
                 m_MinY = temp_MinY;
                m_MaxY = temp_MaxY;
                            
			}
		}
		#endregion

		#region Location
		public class Location
		{
			protected internal double m_Loc;
			protected internal Course m_Course;
			protected internal LinkedListNode<Segment> m_Segment = null;
			protected internal int m_Version = -1;
			protected internal double m_SegVal = 0.0;

			protected int m_StartEndVersion = -1;

			public Location(Course c, double val)
			{
				m_Course = c;
				Normalized = val;
			}
			public Course Course
			{
				get { return m_Course; }
			}
			public Segment Segment
			{
				get { return m_Segment != null ? m_Segment.Value:null; }
			}
			public double NormalizedY 
			{
				get { return m_Segment != null ? m_Segment.Value.GetNormalizedY( m_SegVal ):0; }
			}
			public double Normalized
			{
				get { return m_Loc; }
				set
				{
					if (m_Version != m_Course.Version)
						m_Segment = null;
					if (m_Segment == null)
						m_Segment = m_Course.Segments.First;
					double t = value < 0 ? 0 : value > 1 ? 1 : value;
					if (m_Segment != null)
					{
						Point sp = m_Segment.Value.StartPoint;
						Point ep = m_Segment.Value.EndPoint;
						if (t < sp.X)
						{
							while (t < sp.X && m_Segment.Previous != null)
							{
								m_Segment = m_Segment.Previous;
								sp = m_Segment.Value.StartPoint;
								ep = m_Segment.Value.EndPoint;
							}
						}
						else if (t > ep.X)
						{
							while (t > ep.X && m_Segment.Next != null)
							{
								m_Segment = m_Segment.Next;
								sp = m_Segment.Value.StartPoint;
								ep = m_Segment.Value.EndPoint;
							}
						}
						m_SegVal = ep.X == sp.X ? 0.0:(t - sp.X) / (ep.X - sp.X);
					}
					else
						m_SegVal = 0;
					m_Loc = t;
				}
			}
			Location m_Start;
			Location m_End;
			/// <summary>
			/// Set this to have a start postion other than 0.  Affects the X
			/// </summary>
			public Location Start
			{
				get { return m_Start; }
				set 
				{
					if (value.m_Course != m_Course)
						throw new Exception("Start's course doesn't match parent's course");
					m_Start = value;
					m_StartEndVersion = -1;
				}
			}
			/// <summary>
			/// Set this to have a end position other that the end of the race Affects the X variable.
			/// </summary>
			public Location End
			{
				get { return m_End; }
				set
				{
					if (value.m_Course != m_Course)
						throw new Exception("End's course doesn't match parent's course");
					m_End = value;
					m_StartEndVersion = -1;
				}
			}

			public double X
			{
				get { return m_Loc * m_Course.TotalX; }
				set
				{
					Normalized = value / m_Course.TotalX;
				}
			}
			public double Y
			{
				get { return m_Segment != null ? m_Segment.Value.GetY( m_SegVal ):0; }
			}

			public double SegmentX
			{
				get { return m_SegVal; }
			}

			public float Grade
			{
				get
				{
					return m_Segment.Value.GradeAt(m_SegVal);
				}
			}

			public float Change { get { return m_Segment == null ? 0.0f : m_Segment.Value.Change; } }
			public String XText { get { return m_Segment == null ? "" : m_Segment.Value.GetXText(m_SegVal); } }

			List<String> m_InfoData;
			public List<String> InfoData
			{
				get
				{
					if (m_InfoData == null)
						m_InfoData = new List<String>();

					m_InfoData.Clear();
					List<InfoLabelTypes> tlist = m_Course.InfoLabelTypeList;
					double len = m_Segment.Value.StartX + m_Segment.Value.Length * m_SegVal;
					//double slen = len - m_Course.StartAt;
                    double slen = len - this.Start.X;
                    
					foreach (InfoLabelTypes t in tlist)
					{
						switch (t)
						{
							case InfoLabelTypes.TotalDistance: m_InfoData.Add(ConvertConst.MetersToDistanceString(len)); break;
							case InfoLabelTypes.FromStartDistance: m_InfoData.Add(slen < 0 ? "-" : ConvertConst.MetersToDistanceString(slen)); break;
							case InfoLabelTypes.Grade: m_InfoData.Add(String.Format("{0:F1}%", m_Segment.Value.GradeAt(m_SegVal)*100)); break;
							case InfoLabelTypes.Wind:
								{
									PysicalSegment ps = m_Segment.Value as PysicalSegment;
									m_InfoData.Add(ConvertConst.TextSpeed(ps != null ? ps.Wind : 0));
								}
								break;
							case InfoLabelTypes.Altitude: m_InfoData.Add(ConvertConst.TextHeight(m_Segment.Value.StartY + m_SegVal * (m_Segment.Value.EndY - m_Segment.Value.StartY))); break;
							case InfoLabelTypes.Watts: m_InfoData.Add(String.Format("{0:F0}", m_Segment.Value.StartY + m_SegVal * (m_Segment.Value.EndY - m_Segment.Value.StartY))); break;
							case InfoLabelTypes.PercentAT: m_InfoData.Add(String.Format("{0:F1}%", m_Segment.Value.StartY + m_SegVal * (m_Segment.Value.EndY - m_Segment.Value.StartY))); break;
							case InfoLabelTypes.TotalTime: m_InfoData.Add(String.Format("{0:F1} minutes", len / 60)); break;
							case InfoLabelTypes.FromStartTime: m_InfoData.Add(String.Format("{0:F1} minutes", slen / 60)); break;
                           // case InfoLabelTypes.segval: m_InfoData.Add((m_Segment.Value.StartX + m_Segment.Value.Length * m_SegVal).ToString()); break;
                            case InfoLabelTypes.Ascent:
                                {
                                    double mouseposition = m_Segment.Value.StartX + m_Segment.Value.Length * m_SegVal;
                                    double Ascending = 0;
                                    if (this.Start.X <= mouseposition)
                                    {
                                        Course tempcourse = new Course(m_Course, m_Course.Reverse, m_Course.Mirror, true, this.Start.X, m_Segment.Value.StartX + m_Segment.Value.Length * m_SegVal);
                                        Ascending = tempcourse.ClimbY;
                                        tempcourse = null;
                                    }
                                    m_InfoData.Add(ConvertConst.TextHeight(Ascending)); 
                                    break;
                                }
                           //case InfoLabelTypes.StartEnd:
                           //     {
                           //         m_InfoData.Add(String.Format("{0:F1}/{1:F1}", m_Course.StartAt, m_Course.EndAt));
                           //         break;
                           //     }
                        }
					}

					return m_InfoData;
				}
			}

		}
		#endregion

		#region Favorite
		public bool Favorite
		{
			get
			{
				return Courses.FavoriteFiles.Contains(FileName.ToLower());
			}
			set
			{
				String lf = FileName.ToLower();
				bool cur = Courses.FavoriteFiles.Contains(lf);
				if (value == cur)
					return;
				if (value)
					Courses.FavoriteFiles.Add(lf);
				else
					Courses.FavoriteFiles.Remove(lf);
				Courses.NeedToSaveFavorites = true;
			}
		}
		#endregion


		#region SEGMENTS

		#region Segment
		public class Segment: ICloneable
		{
			public Course Course { get; protected internal set; }
			public LinkedListNode<Segment>	Node { get; protected internal set; }
            
			public int Num { get; protected internal set; }

			protected internal double m_Length;
			protected internal float m_Change;

			protected internal double m_StartX;
			protected internal double m_EndX;

			protected internal double m_StartY;
			protected internal double m_EndY;

            protected Guid m_SegID;

			public virtual String HashString
			{
				get
				{
					string rstring = String.Format("{1:F3},{2:F3},{3:F3},{4:F3}", Length, StartX, EndX, StartY, EndY );
                    return rstring;
				}
			}

            public void SetY(double startY, double endY)
            {
                m_StartY = startY;
                m_EndY = endY;
            }

            public void SetX(double startX, double endX)
            {
                m_StartX = startX;
                m_EndX = endX;
            }
			public Segment()
			{
				m_Length = 1;
				m_Change = 0;
                m_SegID = Guid.NewGuid();
			}
			public Segment(double length, float change)
			{
				m_Length = length;
				m_Change = change;
                m_SegID = Guid.NewGuid();
			}

            public Guid GetSegID()
            {
                return m_SegID;
            }

			public virtual Object Clone()
			{
				Segment s = new Segment();
				s.m_Length = m_Length;
				s.m_Change = m_Change;
                s.m_SegID = m_SegID;
				return s;
			}

			public virtual Segment Reverse()
			{
				Segment s = (Segment)Clone();
				s.m_Change = -m_Change;
				return s;
			}

			public virtual float GradeAt(double val)
			{
				return m_Change;
			}


           
			protected internal void setFix()
			{

                lock (LockObject)
                {
                    //use a try so the lock is released
                    try
                    {
                        if (Course != null)
                        {
                            if (Course.m_FixAt == null || Num < Course.m_FixAt.Value.Num)
                            {
                                Course.m_FixAt = Node;
                                Course.m_bCalcMinMax = true;
                                Course.Version++;
                            }
                        }
                    }
                    catch { }
                    finally { }
                }
                
			}
			protected virtual internal void fix()
			{
                lock (LockObject)
                {
                    //use a try so the lock is always given up on exception
                    try
                    {
                        if (Course != null)
                        {
                            if (Node.Previous == null)
                            {
                                m_StartX = m_StartY = 0;
                                Num = 0;
                            }
                            else
                                Num = Node.Previous.Value.Num + 1;
                            m_EndX = m_StartX + m_Length;
                            m_EndY = m_StartY + m_Length * m_Change;
                            if (Node.Next != null)
                            {
                                Segment ns = Node.Next.Value;
                                ns.m_StartX = m_EndX;
                                ns.m_StartY = m_EndY;
                            }
                            if (Course.m_FixAt == Node)
                            {
                                Course.m_FixAt = Node.Next;
                            }
                        }
                    }
                    catch { }
                    finally { }
                }               
			}

			public double Length
			{
				get { return m_Length; }
				set
                {
                    lock (LockObject)
                    {
                        try
                        {
                            m_Length = value;
                            setFix();
                        }
                        catch { }
                        finally { }
                    }
				}
			}

			public float Change
			{
				get { return m_Change; }
				set
                {
                    lock (LockObject)
                    {
                        try
                        {
                            m_Change = value;
                            setFix();
                        }
                        catch { }
                        finally { }
                    }
				}
			}

			protected double dvalue( double val ) 
			{
                lock (LockObject)
                {
                    try
                    {
                        if (Course == null) return 0;
                        try
                        {
                            if (Course.m_FixAt != null && Course.m_FixAt.Value.Num <= Num)
                                Course.fix();
                        }
                        catch
                        {
                            Log.WriteLine("Thread-based exception on dValue");
                        }

                    }
                    catch { }
                    finally { }
                }
                return val;
			}
			public double StartX { get { return dvalue(m_StartX); } }
			public double StartY { get { return dvalue(m_StartY); } }

			public double EndX { get { return dvalue(m_EndX); } }
			public double EndY { get { return dvalue(m_EndY); } }

			public virtual double MinY { get { return StartY > EndY ? EndY:StartY;} }
			public virtual double MaxY { get { return StartY > EndY ? StartY:EndY;} }


			public int m_StartPointVersion = -1;
			public int m_EndPointVersion = -1;
			public Point m_StartPoint = new Point();
			public Point m_EndPoint = new Point();
			public Point StartPoint
			{
				get
				{
					if (m_StartPointVersion != Course.Version)
					{
						m_StartPoint.X = 1 * StartX / Course.TotalX;
						m_StartPoint.Y = 1 * (1.0 - ((StartY - Course.MinY) / (Course.MaxY - Course.MinY)));
						m_StartPointVersion = Course.Version;
					}

					return m_StartPoint;
				}
			}
			public Point EndPoint
			{
				get
				{
					if (m_EndPointVersion != Course.Version)
					{
						m_EndPoint.X = 1 * EndX / Course.TotalX;
						m_EndPoint.Y = 1 * (1.0 - ((EndY - Course.MinY) / (Course.MaxY - Course.MinY)));
						m_EndPointVersion = Course.Version;
					}
					return m_EndPoint;
				}
			}
            static double CurrentNormY = 1;
			public virtual double GetNormalizedY(double loc)
			{
                
                double ans = m_StartY + Length * loc * m_Change;
                double newNormY = 1.0 - (ans - Course.MinY) / (Course.MaxY - Course.MinY);
                if (newNormY != CurrentNormY) //Debug.WriteLine("A new NormY = " + newNormY);
                CurrentNormY=newNormY;
                return newNormY; //1.0 - (ans - Course.MinY) / (Course.MaxY - Course.MinY);

			}
			public virtual double GetY(double loc)
			{
				return m_StartY + Length * loc * m_Change;
			}

			//public virtual String GetXText(double normalized) { return ""; }
			public virtual String GetXText(double normalized)
			{
				if (Course.YUnits == CourseYUnits.PercentAT)
				{
					if (m_Change != 0)
						return String.Format("{0}% to {1}%", (int)m_StartY, (int)m_EndY);
					return String.Format("{0:F1}%", (int)m_StartY);
				}
				else if (Course.YUnits == CourseYUnits.Watts)
				{
					if (m_Change != 0)
						return String.Format("{0} to {1} watts", (int)m_StartY, (int)m_EndY);
					return String.Format("{0:F1} watts", (int)m_StartY);
				}
				return String.Format("{0:F1}% grade", Change * 100);
			}


			public virtual void FillInfoData(String[] infodata, double pos)
			{
			}
		}
		#endregion

		#region PysicalSegment
		public class PysicalSegment : Segment
		{
			public double Distance { get { return Length; }  set { Length = value; } }
			public virtual float Grade { get { return Change; } set { Change = value; } }
			public float Wind;
            public float Rotation; 

			public override Object Clone()
			{
				PysicalSegment s = new PysicalSegment(m_Length,m_Change,Wind,Rotation);
				return s;
			}


            public PysicalSegment(double length, float change, float wind, float rot)
                : base(length, change)
			{
				Wind = wind;
                Rotation = rot;
			}
			public override String GetXText( double normalized ) 
			{
				return String.Format("{0:F1}% grade", Grade * 100);
			}
			public override void FillInfoData(String[] infodata, double pos)
			{
				double len = m_StartX + m_Length * pos;
				double slen = len - Course.StartAt;
				infodata[0] = ConvertConst.MetersToDistanceString(len);
				infodata[1] = slen < 0 ? "-" : ConvertConst.MetersToDistanceString(slen);
				infodata[2] = String.Format("{0:F1}%", Grade * 100);
				infodata[3] = ConvertConst.TextSpeed(Wind);
				infodata[4] = ConvertConst.TextHeight(m_StartY + pos * (m_EndY - m_StartY));
			}
		}
		#endregion

		#region SmoothSegment
		public class SmoothSegment : PysicalSegment
		{
			public float m_StartGrade;
			public float m_EndGrade;
			public int m_Divisions;
			public float[] m_YArr;
			public float[] m_GArr;
			public float m_DivisionSize;

			public override String HashString
			{
				get
				{
					return "S," + base.HashString;
				}
			}

			//int calcdiv;

			public int Divisions { get { return m_Divisions; } }

			public double m_MinY, m_MaxY;

			public override Object Clone()
			{
				SmoothSegment s = new SmoothSegment(m_Length,m_Change,Wind,Rotation, m_StartGrade, m_EndGrade, m_Divisions);
				return s;
			}

			public override Segment Reverse()
			{
				SmoothSegment s = new SmoothSegment(m_Length,(float)(m_StartY - m_EndY), Wind, -Rotation, -m_EndGrade, -m_StartGrade, m_Divisions);
				return s;
			}

			public SmoothSegment( double length, float change, float wind, float rot, float startgrade, float endgrade, int divisions )
				: base( length, change, wind, rot )
			{				
				m_StartGrade = startgrade;
				m_EndGrade = endgrade;
				m_Divisions = divisions;
				m_YArr = new float[m_Divisions];
				m_GArr = new float[m_Divisions];
				m_DivisionSize = (float)(1.0 / m_Divisions);
			}

			protected internal override void  fix()
			{
                //Debug.WriteLine("called smooth segment fix");
               

                    if (Course != null)
                    {
                        if (Node.Previous == null)
                        {
                            m_StartX = m_StartY = 0;
                            Num = 0;
                        }
                        else
                            Num = Node.Previous.Value.Num + 1;
                        m_EndX = m_StartX + m_Length;

                        // EndY is not so easy to figure out... we will need to calculate it.
                        // At the same time we will be calculating the maxY and minY.
                        float grade = m_StartGrade;
                        float delta = (m_EndGrade - m_StartGrade) / m_Divisions;
                        float dlen = (float)(m_Length / (m_Divisions - 1));
                        double y = m_StartY;
                        m_MinY = m_MaxY = y;

                        int i;

                        //float endy = CalcEndHeight( m_Grade);
                        for (i = 0; i < m_Divisions; i++)
                        {			// There is probably some formula to do this but I'm too lazy to figure it out.
                            m_YArr[i] = (float)y;
                            m_GArr[i] = grade;
                            if (y < m_MinY)
                                m_MinY = y;
                            else if (y > m_MaxY)
                                m_MaxY = y;
                            y += grade * dlen;	// Grade * the division length.
                            grade += delta;
                        }
                        m_EndY = m_YArr[m_Divisions - 1];
                        if (Node.Next != null)
                        {
                            Segment ns = Node.Next.Value;
                            ns.m_StartX = m_EndX;
                            ns.m_StartY = m_EndY;
                        }
                        if (Course.m_FixAt == Node)
                        {
                            Course.m_FixAt = Node.Next;
                        }
                    }
                

			}

			public override double MinY { get { return m_MinY;} }
			public override double MaxY { get { return m_MaxY;} }

			public override double GetNormalizedY(double loc)
			{
				return 1.0 - (GetY(loc) - Course.MinY) / (Course.MaxY - Course.MinY);
			}
			public override double GetY(double loc)
			{
				if (loc >= 1.0)
					return m_EndY;
				if (loc <= 0.0)
					return m_StartY;
				double t = loc * (m_Divisions - 1);
				double n = Math.Floor(t);
				double r = t - n;
				int s = (int)n;
				float y1 = m_YArr[s];
				float y2 = m_YArr[s+1];
				return y1 + (y2 - y1) * r;
			}

			public override float Grade { get { return m_EndGrade; } set { Change = value; } }

			public override float GradeAt(double val)
			{
				return (float)((m_StartGrade + (m_EndGrade - m_StartGrade) * val));
			}

			public override String GetXText(double normalized)
			{
				return String.Format("{0:F1}% grade", GradeAt(normalized) * 100);
			}
			public override void FillInfoData(String[] infodata, double pos)
			{
				double len = m_StartX + m_Length * pos;
				double slen = len - Course.StartAt;
				infodata[0] = ConvertConst.MetersToDistanceString(len);
				infodata[1] = slen < 0 ? "-" : ConvertConst.MetersToDistanceString(slen);
				infodata[2] = String.Format("{0:F1}%", GradeAt(pos) * 100);
				infodata[3] = ConvertConst.TextSpeed(Wind);
				infodata[4] = ConvertConst.TextHeight(GetY(pos));
			}

			public static int CalcDivisions(float startgrade, float endgrade, float length, float rotation)
			{
				double deg = rotation * ConvertConst.RadiansToDegrees;
				int d = (int)Math.Abs(deg * (12.0f / 25.0f));

				// Figure the length divisions

				int ld = (int)(length / 10.0);

				// Figure the grade divisions

				float gdif = endgrade - startgrade;
				int g = 1 + (int)(Math.Abs((gdif * 100.0f) / 2.0f));
				if (g > (int)length)
					g = (int)length;

				// Use the larger of the two divisions.
				int divisions;
				divisions = d;
				if (ld > divisions)
					divisions = ld;
				if (g > divisions)
					divisions = g;

				if (divisions < 2)
					divisions = 2;

				if ((divisions & 1) == 1)
					divisions += 1;
				return divisions;
			}

		}
		#endregion

		#region WattsSegment
		public class WattsSegment : Segment
		{
			public WattsSegment(double seconds, float startwatts, float endwatts )
				: base(seconds, (float)(((float)endwatts - (float)startwatts) / seconds))
			{
				m_StartY = startwatts;
				m_EndY = endwatts;
			}

			public double Seconds { get { return Length; } set { Length = value; } } 
			public double Minutes { get { return Length / 60; } set { Length = value * 60; } }
			public float WattsChange { get { return Change; } set { Change = value; } }
			public int StartWatts { get { return (int)m_StartY; } }
			public int EndWatts { get { return (int)m_EndY; } }


			public override Object Clone()
			{
				WattsSegment s = new WattsSegment(m_Length, (int)m_StartY, (int)m_EndY);
				return s;
			}

			public override Segment Reverse()
			{
				return new WattsSegment(m_Length, (float)m_EndY, (float)m_StartY);
			}

			protected override internal void fix()
			{
                               
                    if (Course != null)
                    {
                        if (Node.Previous == null)
                        {
                            m_StartX = 0;
                            Num = 0;
                        }
                        else
                            Num = Node.Previous.Value.Num + 1;
                        m_EndX = m_StartX + m_Length;
                        m_EndY = m_StartY + m_Length * m_Change;
                        if (Node.Next != null)
                        {
                            Segment ns = Node.Next.Value;
                            ns.m_StartX = m_EndX;

                        }
                        if (Course.m_FixAt == Node)
                        {
                           // if (Thread.CurrentThread.Name == "MainThread") Debug.WriteLine("writing WattsSegment Course.m_FixAt = Node in thread " + Thread.CurrentThread.Name);
                            Course.m_FixAt = Node.Next;
                        }
                    }
                

			}
			public override void FillInfoData(String[] infodata, double pos)
			{
				double seconds = m_StartX + m_Length * pos;
				double slen = seconds - Course.StartAt;

				infodata[0] = String.Format("{0:F1} minutes", Minutes);
				infodata[1] = slen < 0 ? "-" : String.Format("{0:F1} minutes", slen / 60);
				infodata[2] = String.Format("{0:D}", StartWatts  );
				infodata[3] = Change == 0 ? String.Format("-"): String.Format("{0:F2} Watts/Minute", Change );
			}
		}
		#endregion

        #region AnySegment
        public class AnySegment : Segment
        {
            public AnySegment(double xunit, double startyunit, double endyunit)
                : base(xunit, (float)(endyunit - startyunit))
            {
                m_StartY = startyunit;
                m_EndY = endyunit;
            }

            public double XValue { get { return m_Length; } set { m_Length = value; } }
            public float YChange { get { return Change; } set { Change = value; } }
            public double StartYUnits { get { return m_StartY; } }
            public double EndYUnits { get { return m_EndY; } }


            public override Object Clone()
            {
                AnySegment s = new AnySegment(m_Length, m_StartY, m_EndY);
                return s;
            }

            protected override internal void fix()
            {
                //Debug.WriteLine("called anysegment fix");
                
                    if (Course != null)
                    { //    return;
                        if (Node.Previous == null)
                        {
                            m_StartX = 0;
                            Num = 0;
                        }
                        else
                            Num = Node.Previous.Value.Num + 1;
                        m_EndX = m_StartX + m_Length;
                        m_EndY = m_StartY + m_Length * m_Change;
                        if (Node.Next != null)
                        {
                            Segment ns = Node.Next.Value;
                            ns.m_StartX = m_EndX;
                        }
                        if (Course.m_FixAt == Node)
                        {
                            Course.m_FixAt = Node.Next;
                        }
                    }
                
            }
            public override void FillInfoData(String[] infodata, double pos)
            {
                double datalen = m_StartX + m_Length * pos;
                double slen = datalen - Course.StartAt;

                infodata[0] = String.Format("{0:F1} minutes", m_Length);
                infodata[1] = slen < 0 ? "-" : String.Format("{0:F1} minutes", slen);
                infodata[2] = String.Format("{0:D}", StartYUnits);
                infodata[3] = Change == 0 ? String.Format("-") : String.Format("{0:F2} Watts/Minute", Change);
            }
        }
        #endregion

		#region GPSSegment
		public class GPSSegment : PysicalSegment
		{
			public GPSData GPSData { get; protected set; }
			public GPSSegment( GPSData gpsdata, double length ):
				base(length, (float)(gpsdata.pg/100.0), 0.0f, 0.0f)
			{
				GPSData = gpsdata;
			}
			public override Object Clone()
			{
				GPSSegment s = new GPSSegment(GPSData,Length);
				s.m_Length = m_Length;
				s.m_Change = m_Change;
				return s;
			}
		}
		#endregion

		#endregion

		#region Functions
		public void Remove(Segment seg)
		{
            lock (LockObject)
            {
                try
                {
                    if (seg.Course != null)
                    {
                        seg.setFix();
                        if (seg.Node == m_FixAt)
                            m_FixAt = seg.Node.Next;

                        Segments.Remove(seg.Node);
                        seg.Course = null;
                        seg.Node = null;
                    }
                }
                catch { }
                finally { }
            }
		}

        public void AddLast(Segment seg)
        {
            lock (LockObject)
            {
                try
                {
                    Remove(seg);
                    seg.Course = this;
                    seg.Num = Segments.Last == null ? 0 : Segments.Last.Value.Num + 1;
                    seg.Node = Segments.AddLast(seg);
                    seg.setFix();
                }
                catch { }
                finally {}
            }
                // to let the UI be responsive
                if (m_lowPriLoad)
                    Thread.Sleep(0);
            
        }
        

        public void fix()
        {
            try
            {
                if (m_FixAt != null)
                {
                    lock (LockObject)
                    {
                        try
                        {
                            while (m_FixAt != null && m_FixAt.Value != null)
                            {
                                m_FixAt.Value.fix();
                            }
                        }
                        catch { }
                        finally { }
                    }
                }

            }
            catch
            {
                Log.WriteLine("Course.fix threw exception");
            }
                               
        }

        static Regex regexp_rmp = new Regex(@"\.rmp$",RegexOptions.IgnoreCase);
		static Regex regexp_rmx = new Regex(@"\.rmc$", RegexOptions.IgnoreCase);
		static Regex regexp_avi = new Regex(@"\.avi$", RegexOptions.IgnoreCase);
		static Regex regexp_crs = new Regex(@"\.crs$", RegexOptions.IgnoreCase);
		static Regex regexp_erg = new Regex(@"\.erg$", RegexOptions.IgnoreCase);
		static Regex regexp_mrc = new Regex(@"\.mrc$", RegexOptions.IgnoreCase);
		static Regex regexp_3dc = new Regex(@"\.3dc$", RegexOptions.IgnoreCase);
		static Regex regexp_adt = new Regex(@"\.adt$", RegexOptions.IgnoreCase);
		static Regex regexp_dnw = new Regex(@"\.dnw$", RegexOptions.IgnoreCase);
		static Regex regexp_tng = new Regex(@"\.dnw$", RegexOptions.IgnoreCase);
		static Regex regexp_diveq = new Regex(@"(^[^=]*)=(.*$)");
		static Regex regexp_nums = new Regex(@"-?(\d|,)*\.?\d+");
		static Regex regexp_words = new Regex(@"\S+");
		static Regex regexp_name_from_filename = new Regex(@"\\([^\\.]*)\.[^\\]*$");
		

		bool diveq( string s, out string a, out string b )
		{
			Match cc = regexp_diveq.Match(s);
			if (cc.Success)
			{
				a = cc.Groups[1].ToString().Trim();
				b = cc.Groups[2].ToString().Trim();
				return true;
			}
			else
				a = b = "";
			return false;
		}
		#endregion

		#region Course Loading

		public bool Save(String filename, string description)
		{
			String oldfname = FileName;
			FileName = filename;
			CourseType oldtype = Type;
			Type &= ~CourseType.Performance;
			bool ans = PerfFrame.SaveRMXCourse(filename, this, description);
			Type = oldtype;
			FileName = oldfname;
			return ans;
		}
		public bool Save_Special(String filename, string description)
		{
			CourseType type,oldtype;
			type = oldtype = Type;
			type &= ~CourseType.Video;
			if ((type & CourseType.ThreeD) == CourseType.Zero)
				type |= CourseType.Distance;
			Type = type;
			bool ans = Save(filename, description);
			Type = oldtype;
			return ans;
		}
        
        public static bool ms_lowPri = true;
        public bool m_lowPriLoad = true;

        public bool Load(String filename) { return Load(filename, false); }
		public bool Load(String filename, bool lowPri)
		{
            m_lowPriLoad = lowPri;
            bool returnval = false;
            try
            {
                FileName = filename;
                Directory = System.IO.Path.GetDirectoryName(filename);
                // Figure out some defaults from the file... like the name of the track.
                Match m = regexp_name_from_filename.Match(filename);
                if (m.Success)
                {
                    m_Name = m.Groups[1].ToString().Replace('_', ' ');
                }
                String ext = System.IO.Path.GetExtension(filename).ToLower();
                // experiment here
                //read the files and fill the length and ascent properties attached to the course.
                // then null out the segments to a single segment, and see how much memory is filled.
               switch (ext)
                {
                    case ".avi": returnval = LoadAVI(filename); break;
                    case ".crs": returnval = LoadCRS(filename); break;
                    case ".dnw": returnval = LoadERG(filename, true, CourseXUnits.Distance, CourseYUnits.Watts); break;
                    case ".atd": returnval = LoadERG(filename, true, CourseXUnits.Distance, CourseYUnits.PercentAT); break;
                    case ".tng": returnval = LoadERG(filename, true, CourseXUnits.Time, CourseYUnits.Grade); break;
                    case ".erg": returnval = LoadERG(filename, true, CourseXUnits.Time, CourseYUnits.Watts); break;
                    case ".mrc": returnval = LoadERG(filename, true, CourseXUnits.Time, CourseYUnits.PercentAT); break;
                    case ".3dc": returnval = Load3DC(filename); break;
                    case ".rmc": returnval = LoadRMX(filename); break;
                    case ".rmp": returnval = LoadRMP(filename, false); break;
                }

                if (returnval)
                {
                    m_MetricDistanceOrTimeWithLaps = CourseLengthOrDurationMetersorSeconds();
                    m_AscentMetricOrMaxPowerWithLaps = CalcClimbBounded(this.StartAt, this.EndAt) * this.Laps;
                }
            }
            catch
            {
                Log.WriteLine("Error loading course");
                returnval = false;
            }
			return returnval;
		}
        public bool LoadWithReducedSegments(String filename, bool lowPri)
        {
            m_lowPriLoad = lowPri;
            bool returnval = false;
            try
            {
                FileName = filename;
                Directory = System.IO.Path.GetDirectoryName(filename);
                // Figure out some defaults from the file... like the name of the track.
                Match m = regexp_name_from_filename.Match(filename);
                if (m.Success)
                {
                    m_Name = m.Groups[1].ToString().Replace('_', ' ');
                }
                String ext = System.IO.Path.GetExtension(filename).ToLower();
                // experiment here
                //read the files and fill the length and ascent properties attached to the course.
                // then null out the segments to a single segment, and see how much memory is filled.

                bool justperfheader = true;
                switch (ext)
                {
                    case ".avi": returnval = LoadAVI(filename); break;
                    case ".crs": returnval = LoadCRS(filename); break;
                    case ".dnw": returnval = LoadERG(filename, true, CourseXUnits.Distance, CourseYUnits.Watts); break;
                    case ".atd": returnval = LoadERG(filename, true, CourseXUnits.Distance, CourseYUnits.PercentAT); break;
                    case ".tng": returnval = LoadERG(filename, true, CourseXUnits.Time, CourseYUnits.Grade); break;
                    case ".erg": returnval = LoadERG(filename, true, CourseXUnits.Time, CourseYUnits.Watts); break;
                    case ".mrc": returnval = LoadERG(filename, true, CourseXUnits.Time, CourseYUnits.PercentAT); break;
                    case ".3dc": returnval = Load3DC(filename); break;
                    case ".rmc": returnval = LoadRMX(filename); break;
                    case ".rmp": returnval = LoadRMP(filename, justperfheader); break;
                }

                if (returnval)
                {
                    if (this.PerformanceHeader == null)
                    {
                        m_MetricDistanceOrTimeWithLaps = CourseLengthOrDurationMetersorSeconds();
                        m_AscentMetricOrMaxPowerWithLaps = CalcClimbBounded(this.StartAt, this.EndAt) * this.Laps;

                        Segment tempseg = Segments.First();
                        int countsegs = Segments.Count;

                        for (int i = 10; i < countsegs; i++)
                        {
                            //Segment killer = Segments.Last();
                            Segments.RemoveLast();
                        }
                        //this should leave only 10 segments.
                    }
                    else
                    {
                    }
                }
            }
            catch
            {
                Log.WriteLine("error loading course reduced");
                returnval = false;
            }
            return returnval;
        }


		Char[] white_space = new Char [] {' ', '\t' };
		private enum crsstates { none, header, data };
		public bool LoadCRS( String filename )
		{
            //Debug.WriteLine("Loading crs file " + System.IO.Path.GetFileNameWithoutExtension(filename));
        
            string[] lines;
			try { lines = File.ReadAllLines(filename); }
			catch { lines = null; }
			if (lines == null)
				return false;
			this.FileName = filename;
			crsstates state = crsstates.none;
			bool metric = false;

			string t,n,v;
			foreach (string line in lines)
			{
				t = line.Trim();
				t = Regex.Replace(t, @";.*$", ""); 
				switch (state)
				{
					case crsstates.none:
						if (t.Equals("[COURSE HEADER]", StringComparison.OrdinalIgnoreCase))
							state = crsstates.header;
						else if (t.Equals("[COURSE DATA]", StringComparison.OrdinalIgnoreCase))
							state = crsstates.data;
						break;
					case crsstates.header:
						if (t.Equals("[END COURSE HEADER]", StringComparison.OrdinalIgnoreCase))
							state = crsstates.none;
						else
						{
							if (!diveq(t, out n, out v))
								break;
							if (n.Equals("UNITS", StringComparison.OrdinalIgnoreCase))
							{
								metric = !v.Equals("English", StringComparison.OrdinalIgnoreCase);
							}
							else if (n.Equals("DESCRIPTION", StringComparison.OrdinalIgnoreCase))
							{
								Description = v;
							}
							else if (n.Equals("FILE NAME", StringComparison.OrdinalIgnoreCase))
							{
								//Name = v;
							}
						}
						break;
					case crsstates.data:
						if (t.Equals("[END COURSE DATA]", StringComparison.OrdinalIgnoreCase))
							state = crsstates.none;
						MatchCollection m = regexp_nums.Matches(t);
						if (m.Count >= 2)
						{
							try
							{
								// DISTANCE	GRADE		WIND
								double dist = Convert.ToDouble(m[0].ToString().Trim()) *
									(metric ? ConvertConst.KilometersToMeters : ConvertConst.MilesToMeters);
								float grade = (float)(Convert.ToDouble(m[1].ToString().Trim())/100.0);
								float wind;
								if (m.Count > 2)
									wind = (float)(Convert.ToDouble(m[2].ToString().Trim()) * 
                                        (metric ? ConvertConst.KPHToMetersPerSecond : ConvertConst.MPHToMetersPerSecond));
								else
									wind = 0;
								AddLast(new PysicalSegment(dist, grade, wind, 0.0f));
							}
							catch { }
						}
						break;
				}
			}
			Type = CourseType.Distance;
            Looped = false;
            Modified = false;

			EndAt = TotalX;
			Commit();
			return true;
		}
        

		static private String[] WattsLabels = new String[] { "Length", "Total time", "Watts", "Slope" };
		public bool LoadERG(String filename) { return LoadERG( filename, false, CourseXUnits.Time, CourseYUnits.Watts ); }
		public bool LoadERG(String filename, bool force, CourseXUnits xunits, CourseYUnits yunits)
		{
          //  Debug.WriteLine("Loading Erg file " + System.IO.Path.GetFileNameWithoutExtension(filename));
        
            string[] lines;
			try { lines = File.ReadAllLines(filename); }
			catch { lines = null; }
			if (lines == null)
				return false;
			FileName = filename;

			crsstates state = crsstates.none;
			bool metric = false;

			string t, n, v;
			//int curwatts = 0;
			//double curtime = 0.0;
			if (!force)
			{
				xunits = CourseXUnits.Time;
				yunits = CourseYUnits.Watts;
			}

			double curx = 0;
			double cury = 0;
			int segcnt = 0;

			foreach (string line in lines)
			{
				t = line.Trim();
				t = Regex.Replace(t, @";.*$", "");
				switch (state)
				{
					case crsstates.none:
						if (t.Equals("[COURSE HEADER]", StringComparison.OrdinalIgnoreCase))
							state = crsstates.header;
						else if (t.Equals("[COURSE DATA]", StringComparison.OrdinalIgnoreCase))
							state = crsstates.data;
						break;
					case crsstates.header:
						if (t.Equals("[END COURSE HEADER]", StringComparison.OrdinalIgnoreCase))
							state = crsstates.none;
						else
						{
							if (!diveq(t, out n, out v))
							{
								if (force)
									break;

								MatchCollection mm = regexp_words.Matches(t);
								if (mm.Count >= 2)
								{
									int found = 0;
									CourseXUnits _xunits = xunits;
									CourseYUnits _yunits = yunits;
									int i = 0;
									for(;i < mm.Count;i++)
									{
										String s = mm[i].ToString();
										if (String.Compare(s, "MINUTES", true) == 0 && found == 0)
										{
											found++;
											_xunits = CourseXUnits.Time;
										}
										else if (String.Compare(s, "DISTANCE", true) == 0 && found == 0)
										{
											found++;
											_xunits = CourseXUnits.Distance;
										}
										else if (String.Compare(s, "WATTS", true) == 0 && found == 1)
										{
											found++;
											_yunits = CourseYUnits.Watts;
										}
										else if (String.Compare(s, "%AT", true) == 0 && found == 1)
										{
											found++;
											_yunits = CourseYUnits.PercentAT;
										}
										else if (String.Compare(s, "GRADE", true) == 0 && found == 1)
										{
											found++;
											_yunits = CourseYUnits.Grade;
										}
									}
									if (found >= 2)
									{
										xunits = _xunits;
										yunits = _yunits;
									}
								}
								break;
							}
							if (n.Equals("UNITS", StringComparison.OrdinalIgnoreCase))
							{
								metric = !v.Equals("English", StringComparison.OrdinalIgnoreCase);
							}
							else if (n.Equals("DESCRIPTION", StringComparison.OrdinalIgnoreCase))
							{
								Description = v;
							}
							else if (n.Equals("FILE NAME", StringComparison.OrdinalIgnoreCase))
							{
								//Name = v;
							}
						}
						break;
					case crsstates.data:
						if (t.Equals("[END COURSE DATA]", StringComparison.OrdinalIgnoreCase))
							state = crsstates.none;
						MatchCollection m = regexp_nums.Matches(t);
						if (m.Count >= 2)
						{
							try
							{
								double x = (double)Convert.ToDouble(m[0].ToString().Trim());
								double y = (double)Convert.ToDouble(m[1].ToString().Trim());
								double w = m.Count >= 3 ? Convert.ToDouble(m[2].ToString().Trim()):0.0;
								if (segcnt == 0)
									cury = y;
								if (xunits == CourseXUnits.Time)
								{
									// Time is handled a little differently than distance.
									// Time is the minit marker of the specific time...
									if (x > curx)
									{
										double tm = (x - curx) * 60; // This is the time in seconds.
										if (yunits == CourseYUnits.Grade)
											AddLast( new PysicalSegment(tm,(float)(y/100.0),(float)w,0));
										else
											AddLast( new WattsSegment(tm, (int)cury, (int)y));
									}
								}
								else
								{
									if (x > 0)
									{
										double tm = x * (metric ? ConvertConst.KilometersToMeters : ConvertConst.MilesToMeters);
										if (yunits == CourseYUnits.Grade)
											AddLast( new PysicalSegment(tm,(float)(y/100.0),(float)w,0));
										else
											AddLast( new WattsSegment(tm, (int)cury, (int)y));
									}
								}
								curx = x;
								cury = y;
								segcnt++;
							}
							catch { }
						}
						break;
				}
			}
			Type = CourseType.Watts;
            Looped = false;
            Modified = false;

			XUnits = xunits;
			YUnits = yunits;

			if (XUnits == CourseXUnits.Distance && YUnits == CourseYUnits.Grade)
			{
				Type = CourseType.Distance;
			}
			else if (YUnits == CourseYUnits.Watts)
			{
				Type = CourseType.Watts;
			}

			EndAt = TotalX;
			Commit();
			return true;
		}

		String m_AVIFileName = null;
		public String AVIFilename
		{
			get { return m_AVIFileName; }
		}

		public Course VideoCourse
		{
			get
			{
				Course c = OriginalCourse;
				if (c == null)
					c = this;
				return (c.Type & CourseType.Video) != CourseType.Zero ? c : null;
			}
		}


		public bool Registered { get; protected set; }
		public void CheckDemo()
		{
			if (AVIFilename != null && m_CourseHash == "59EEB4F5FEB143A68C5E7F30622177EC")
			{
				double a = Math.Abs(MaxY - 0.17567814746672436);
				double b = Math.Abs(MinY - -1.8987048048937805);
				double c = Math.Abs(TotalX - 269.0978899005097);
				if (a < 0.1 && b < 0.1 && c < 0.1)
					Registered = true;
			}
		}


		public bool LoadAVI(String filename)
		{
           // Debug.WriteLine("Loading AVI file " + System.IO.Path.GetFileNameWithoutExtension(filename));
        
            
            String datfile = regexp_avi.Replace(filename,".dat");
			if (!File.Exists(datfile))
				return false;
			m_AVIFileName = filename;

			RCVCourseData cdata = new RCVCourseData();
			if (!cdata.Open(datfile))
			{
                Log.WriteLine(string.Format("Couldn't load course file - {0}", datfile));
				return false;
			}
			int segments = cdata.Segments;
			int n;
			double dist;
			for (int i = 0; i < segments; i++)
			{
				n = i+1 >= segments ? segments - 1 : i+1;
				dist = (double)(cdata[n].seconds - cdata[i].seconds) * cdata[i].mps2;
				AddLast(new GPSSegment(cdata[i], dist ));
			}

			Type = CourseType.Distance | CourseType.Video;
            Looped = false;
            Modified = false;

			Registered = cdata.Registered(this);


			EndAt = TotalX;
			Commit();
			return true;
		}

		public bool Load3DC(string filename)
		{
           // Debug.WriteLine("Loading 3dc file " + System.IO.Path.GetFileNameWithoutExtension(filename));
        
            if (!File.Exists(filename))
				return false;

			ThreeDCCourse cdata = new ThreeDCCourse();
			if (!cdata.Open(filename))
			{
                Log.WriteLine(string.Format("Couldn't load course file - {0}", filename));
				return false;
			}
			int segments = cdata.Segments;
			if (segments <= 0)
			{
				Log.WriteLine(String.Format("No segments in file \"{0}\"", filename));
				return false;
			}
			for (int i = 0; i < segments; i++)
			{
				ThreeDCCourse.Segment seg = cdata[i];
				//-------- temporary old value.
				float change = seg.Length > 0 ? (float)((seg.EndY - seg.StartY) / seg.Length):0.0f;
				//AddLast(new PysicalSegment(seg.Length, change, (float)(seg.Wind * ConvertConst.KPHToMetersPerSecond), (float)seg.Rot));
				//------------------------------
				if (seg.StartGrade == seg.EndGrade)
					AddLast( new PysicalSegment(seg.Length, (float)seg.EndGrade, (float)(seg.Wind * ConvertConst.KPHToMetersPerSecond), (float)seg.Rot));
				else
				{
					SmoothSegment s = new SmoothSegment(seg.Length, (float)(seg.EndY - seg.StartY), (float)(seg.Wind * ConvertConst.KPHToMetersPerSecond), (float)seg.Rot,
						(float)seg.StartGrade, (float)seg.EndGrade, seg.Divisions);
					AddLast(s);
				}
			}
			Type = CourseType.ThreeD;
            Looped = cdata.IsClosedLooped();
            Modified = false;
            Laps = cdata.Laps;

			EndAt = TotalX;
			Commit();
			return true;
		}

        // Imports a GPX File
        public bool LoadGPX(string filename)
        {
            // Debug.WriteLine("Loading GPX file " + System.IO.Path.GetFileNameWithoutExtension(filename));
            try
            {
                if (!File.Exists(filename))
                    return false;
                int gpxTrackIndex = 0;
                GPXLoader gpxLoader = new GPXLoader();
                gpxLoader.LoadGPXTracksList(filename);
                if (gpxLoader.GPXTrackList.Count < 1) return false;
                if (gpxLoader.GPXTrackList.Count > 1)
                {
                    GpxImportTrackSelector gpxTrackSelector = new GpxImportTrackSelector(gpxLoader);
                    if (gpxTrackSelector.ShowDialog().Value == true)
                    {
                        gpxTrackIndex = gpxTrackSelector.SelectedTrackIndex;
                    }
                    else
                    {
                        Log.WriteLine(string.Format("Couldn't load course file - {0}", filename));
                        return false;
                    }
                }
                //String tHeaderHash = null;
                //String tCourseHash = null;
                //bool result = false;
                hdrVersion = 1.3f;
                GPXTrack gpxTrack = gpxLoader.GPXTrackList[gpxTrackIndex];
                Name = gpxTrack.TrackName;
                Type = CourseType.Distance;
                Description = gpxTrack.TrackName;
                Looped = false;
                Laps = 1;
                EndAt = 1;
                StartAt = 0;
                Mirror = false;
                Reverse = false;
                Modified = false;
                //String tOriginalHash = null;
                XUnits = CourseXUnits.Distance;
                YUnits = CourseYUnits.Grade;
                int count = gpxTrack.SegList.Count;
                for (int i = 1; i < count; i++)
                {
                    GPXTrackSegment SegA = gpxTrack.SegList[i - 1];
                    GPXTrackSegment SegB = gpxTrack.SegList[i];


                    double len = GPXLoader.FindDistance(SegA.Latitude, SegA.Longitude, SegB.Latitude, SegB.Longitude);
                    float grade = (float)GPXLoader.FindGrade(SegA.Elevation, SegB.Elevation, len);
                    float wind = 0.0f;
                    float rot = 0.0f;
                    AddLast(new Course.PysicalSegment(len, grade, wind, rot));
                }
                this.fix();
                if (EndAt <= StartAt)
                    EndAt = TotalX;
                Commit();
            }

            catch
            {
                Log.WriteLine(string.Format("Course file failed to load - {0}", filename));
                return false;
            }
            return true;
        }

			public bool LoadRMX(Stream stream)  {
            using (XmlReader reader = XmlReader.Create(stream))  {
                XDocument xdoc = XDocument.Load(reader);

                if (xdoc == null)  {
                    Log.WriteLine(string.Format("Couldn't load course file"));
                    return false;
                }

                XElement rootNode = xdoc.Root; // "RMX"
                bool loaded = LoadXCourse(ref rootNode);

                if (!loaded)  {
                    Log.WriteLine(string.Format("Course file missing"));
                    return false;
                }

                if (EndAt <= StartAt)
                    EndAt = TotalX;

                Commit();
                return true;

            }									// using

            //return false;
        }

        // Loads the RMX type Course 
        public bool LoadRMX(string filename)
        {
            //int x = 0;
            // Debug.WriteLine("Loading RMX file " + System.IO.Path.GetFileNameWithoutExtension(filename));
            try
            {
                if (!File.Exists(filename))
                    return false;

                Stream fs = File.OpenRead(filename);

             //   StreamReader sr = new StreamReader(filename);

                LoadRMX(fs);

                //XDocument xdoc = XDocument.Load(filename);
                //if (xdoc == null)
                //{
                //    Log.WriteLine(string.Format("Couldn't load course file - {0}", filename));
                //    return false;
                //}

                //XElement rootNode = xdoc.Root; // "RMX"
                //bool loaded = LoadXCourse(ref rootNode);
                //if (!loaded)
                //{
                //    Log.WriteLine(string.Format("Course file missing - {0}", filename));
                //    return false;
                //}
                //if (EndAt <= StartAt)
                //    EndAt = TotalX;
                //Commit();
            }

            catch
            {
                Log.WriteLine(string.Format("Course file failed to load - {0}", filename));
                return false;
            }
            return true;
        }


        //public bool LoadRMX(string filename)
        //{
        //    int x = 0;
        //  // Debug.WriteLine("Loading RMX file " + System.IO.Path.GetFileNameWithoutExtension(filename));
        //    try
        //    {
        //        if (!File.Exists(filename))
        //            return false;

        //        XDocument xdoc = XDocument.Load(filename);
        //        if (xdoc == null)
        //        {
        //            Log.WriteLine(string.Format("Couldn't load course file - {0}",filename));
        //            return false;
        //        }

        //        XElement rootNode = xdoc.Root; // "RMX"
        //        bool loaded = LoadXCourse(ref rootNode);
        //        if (!loaded)
        //        {
        //            Log.WriteLine(string.Format("Course file missing - {0}", filename));
        //            return false;
        //        }
        //        if (EndAt <= StartAt)
        //            EndAt = TotalX;
        //        Commit();
        //    }

        //    catch
        //    {
        //        Log.WriteLine(string.Format("Course file failed to load - {0}",filename));
        //        return false;
        //    }
        //    return true;
        //}

        // Load Course element part of the XML Doc
        public bool LoadXCourse(ref XElement rootNode)
        {
			String tHeaderHash = null;
			String tCourseHash = null;
			bool result = false;
            hdrVersion = 0.0f;
            XElement eleC = rootNode.Element(xtag.Header);
            if (eleC != null)
            {
                XElement eleCI = eleC.Element(xtag.Version);
                try { hdrVersion = float.Parse(eleCI.Value); }
                catch { hdrVersion = 0.0f; }
            }
            if (hdrVersion < 1.0f)
                return false;

            eleC = rootNode.Element(xtag.Course);
            if (eleC != null)
            {
                XElement eleCI = eleC.Element(xtag.Info);
                try { Name = eleCI.Attribute(xtag.Name).Value; }
                catch { Name = "Unknown"; }
                try { Description = eleCI.Attribute(xtag.Description).Value; }
                catch { Description = ""; }
                //try { FileName = eleCI.Attribute(xtag.FileName).Value; }
                //catch { FileName = ""; }
                try { Type = (CourseType)Enum.Parse(typeof(CourseType), eleCI.Attribute(xtag.Type).Value); }
                catch { Type = CourseType.Distance; }
                try { Looped = bool.Parse(eleCI.Attribute(xtag.Looped).Value); }
                catch { Looped = false; }
                try { Laps = Int32.Parse(eleCI.Attribute(xtag.Laps).Value); }
                catch { Laps = 1; }
                try { EndAt = double.Parse(eleCI.Attribute(xtag.EndAt).Value); }
                catch { EndAt = 1; }
                try { StartAt = double.Parse(eleCI.Attribute(xtag.StartAt).Value); }
                catch { StartAt = 0; }
                try { Mirror = bool.Parse(eleCI.Attribute(xtag.Mirror).Value); }
                catch { Mirror = false; }
                try { Reverse = bool.Parse(eleCI.Attribute(xtag.Reverse).Value); }
                catch { Reverse = false; }
                try { Modified = bool.Parse(eleCI.Attribute(xtag.Modified).Value); }
                catch { Modified = false; }

				String tOriginalHash = null;

                if (hdrVersion > 1.00f)
                {
                    try { XUnits = (CourseXUnits)Enum.Parse(typeof(CourseXUnits), eleCI.Attribute(xtag.XUnits).Value); }
                    catch { XUnits = CourseXUnits.Distance; }
                    try { YUnits = (CourseYUnits)Enum.Parse(typeof(CourseYUnits), eleCI.Attribute(xtag.YUnits).Value); }
                    catch { YUnits = CourseYUnits.Grade; }
					try { tCourseHash = eleCI.Attribute(xtag.CourseHash).Value; }
					catch { }
					try { tHeaderHash = eleCI.Attribute(xtag.HeaderHash).Value; }
					catch { }
					try { tOriginalHash = eleCI.Attribute(xtag.OriginalHash).Value; }
					catch { }
                }

                CourseType ct = Type;
                if ((ct & CourseType.Video) == CourseType.Video)
                    ct = CourseType.Video;
                else if ((ct & CourseType.ThreeD) == CourseType.ThreeD)
                    ct = CourseType.ThreeD;
                switch (ct)
                {
                    case CourseType.Video:
                        {
                            XElement eleCD = eleC.Element(xtag.RCVType);
                            if (eleCD != null)
                            {
                                IEnumerable<XElement> eleArr = eleCD.Elements();
                                int count = int.Parse(eleCD.Attribute(xtag.Count).Value);
                                int i = 0;
                                foreach (XElement el in eleArr)
                                {
                                    double len = double.Parse(el.Element(xtag.Length).Value);
                                    float grade = float.Parse(el.Element(xtag.Grade).Value);
                                    float wind = float.Parse(el.Element(xtag.Wind).Value);

                                    XElement eleGPS = el.Element(xtag.GPSData);
                                    if (eleGPS != null)
                                    {
                                        GPSData gd = new GPSData();
                                        gd.frame = uint.Parse(eleGPS.Element("frame").Value);
                                        gd.real = Int32.Parse(eleGPS.Element("real").Value);
                                        gd.seconds = Int32.Parse(eleGPS.Element("seconds").Value);
                                        gd.lat = double.Parse(eleGPS.Element("lat").Value);
                                        gd.lon = double.Parse(eleGPS.Element("lon").Value);
                                        gd.unfiltered_elev = double.Parse(eleGPS.Element("unfiltered_elev").Value);
                                        gd.filtered_elev = double.Parse(eleGPS.Element("filtered_elev").Value);
                                        gd.manelev = double.Parse(eleGPS.Element("manelev").Value);
                                        gd.accum_meters1 = double.Parse(eleGPS.Element("accum_meters1").Value);
                                        gd.accum_meters2 = double.Parse(eleGPS.Element("accum_meters2").Value);
                                        gd.section_meters1 = double.Parse(eleGPS.Element("section_meters1").Value);
                                        gd.section_meters2 = double.Parse(eleGPS.Element("section_meters2").Value);
                                        gd.pg = double.Parse(eleGPS.Element("pg").Value);
                                        gd.mps1 = double.Parse(eleGPS.Element("mps1").Value);
                                        gd.mph1 = double.Parse(eleGPS.Element("mph1").Value);
                                        gd.mps2 = double.Parse(eleGPS.Element("mps2").Value);
                                        gd.mph2 = double.Parse(eleGPS.Element("mph2").Value);
                                        gd.faz = double.Parse(eleGPS.Element("faz").Value);
                                        gd.seconds_offset = double.Parse(eleGPS.Element("seconds_offset").Value);
                                        gd.x= double.Parse(eleGPS.Element("x").Value);				
                                        gd.y= double.Parse(eleGPS.Element("y").Value);				
                                        gd.z= double.Parse(eleGPS.Element("z").Value);
                                        AddLast(new Course.GPSSegment(gd, len));
                                    }
                                    else
                                        AddLast(new Course.PysicalSegment(len, grade, wind, 0.0f));
                                    if (++i >= count)
                                        break;
                                }
                                result = true;
                            }
                        }
                        break;
                    //default:
                    case CourseType.Distance:
                        {
                            XElement eleCD = eleC.Element(xtag.DistanceType);
                            if (eleCD != null)
                            {
                                IEnumerable<XElement> eleArr = eleCD.Elements();
                                int count = int.Parse(eleCD.Attribute(xtag.Count).Value);
                                int i = 0;
                                foreach (XElement el in eleArr)
                                {
                                    double len = double.Parse(el.Element(xtag.Length).Value);
                                    float grade = float.Parse(el.Element(xtag.Grade).Value);
                                    float wind = float.Parse(el.Element(xtag.Wind).Value);
                                    float rot = 0.0f;
                                    AddLast(new Course.PysicalSegment(len, grade, wind, rot));
                                    if (++i >= count)
                                        break;
                                }
                                result = true;
                            }
                        }
                        break;
                    case CourseType.ThreeD:
                        {
                            XElement xtemp;
                            XElement eleCD = eleC.Element(xtag.ThreeDType);
                            if (eleCD != null)
                            {
                                IEnumerable<XElement> eleArr = eleCD.Elements();
                                int count = int.Parse(eleCD.Attribute(xtag.Count).Value);
                                int i = 0;
								float lastgrade = 0.0f;
                                foreach (XElement el in eleArr)
                                {
                                    double len = double.Parse(el.Element(xtag.Length).Value);
                                    float grade = float.Parse(el.Element(xtag.Grade).Value);
                                    float wind = float.Parse(el.Element(xtag.Wind).Value);
                                    float rot = 0.0f;
                                    xtemp = el.Element(xtag.Rotation);
                                    if(xtemp != null)
                                        rot = float.Parse(xtemp.Value);
									XAttribute att = el.Attribute(xtag.Smooth);
									XElement etemp = el.Element(xtag.Divisions);
									if (att != null && bool.Parse(att.Value) && etemp != null)
									{
										int div = int.Parse(etemp.Value);
										AddLast(new SmoothSegment(len, grade, wind, rot, lastgrade, grade, div));
									}
									else
										AddLast(new Course.PysicalSegment(len, grade, wind, rot));
                                    if (++i >= count)
                                        break;
									lastgrade = grade;
                                }
                                result = true;
                            }
                        }
                        break;
					default:
						{
                            XElement eleCD = eleC.Element(xtag.Segments);
                            if (eleCD != null)
                            {
                                IEnumerable<XElement> eleArr = eleCD.Elements();
                                int count = int.Parse(eleCD.Attribute(xtag.Count).Value);
                                int i = 0;
                                foreach (XElement el in eleArr)
                                {
									double x,sy,ey,wind;
									x = sy = ey = wind = 0;
									if (XUnits == CourseXUnits.Distance)
										x = double.Parse(el.Element(xtag.Length).Value);
									else
										x = double.Parse(el.Element(xtag.Minutes).Value) * 60;

									if (YUnits == CourseYUnits.Grade)
									{
										ey = sy = double.Parse(el.Element(xtag.Grade).Value);
										XElement element = el.Element(xtag.Wind);
										wind = 0;
										if (element != null)
											try { wind = double.Parse(element.Value); }
											catch { }
									}
									else if (YUnits == CourseYUnits.Watts)
									{
										sy = double.Parse(el.Element(xtag.StartWatts).Value);
										ey = double.Parse(el.Element(xtag.EndWatts).Value);
									}
									else
									{
										sy = double.Parse(el.Element(xtag.StartAt).Value);
										ey = double.Parse(el.Element(xtag.EndAt).Value);
									}
									if (YUnits == CourseYUnits.Grade)
										AddLast( new PysicalSegment( x, (float)sy, (float)wind, 0 ) );
									else
										AddLast(new Course.WattsSegment(x, (int)sy, (int)ey));
                                    if (++i >= count)
                                        break;
                                }
                                result = true;
                            }
                        }
                        break;
                }
            }
			if (result)
			{
				HeaderHash = tHeaderHash;
				CourseHash = tCourseHash;
			}

            return result;
        }

        // Loads the Performance RMP type Course 
        public bool LoadRMP(string filename,bool processonlyperfheader)
        {
          //  Debug.WriteLine("Loading RMP file " + System.IO.Path.GetFileNameWithoutExtension(filename));
        
            try
            {
                if (!File.Exists(filename))
                {
                    Log.WriteLine(string.Format("Couldn't load course file {0}, does not exist", filename));
                    return false;
                }

                RawStream strIn = new RawStream();

                if (strIn == null || !strIn.OpenRawFileIn(filename) || !LoadRMPCourse(strIn, true, processonlyperfheader, 0.0f))
                {
                    if(strIn != null)
                        strIn.CloseRawFileIn();
                    Log.WriteLine("Couldn't load course file");
                    return false;
                }
                strIn.CloseRawFileIn();

                EndAt = TotalX;

                Commit();
            }

            catch
            {
                Log.WriteLine("Course file failed to load");
                return false;
            }
            return true;
        }

        public bool LoadRMPCourse(RawStream strIn, float version)
        {
            return LoadRMPCourse(strIn, false, false, version);
        }

		public PerfFile.CRMHeader PerformanceHeader;
		public PerfFile.CPerfInfo PerformanceInfo;




        // Load Course from Performance files
        public bool LoadRMPCourse(RawStream strIn, bool doHeader, bool onlyheader, float version)
        {
            object obj;
            bool result = false;
            try
            {
				String tHeaderHash = null;
				String tCourseHash = null;
                
                hdrVersion = version;

                if (doHeader)
                {
					strIn.SetCurRawFieldInPos(0);
                    PerfFile.CRMHeader rmh = new PerfFile.CRMHeader();
                    if (!rmh.Read(strIn))
                        return false;
                    hdrVersion = rmh.Version;

                    PerfFile.CPerfInfo pih = new PerfFile.CPerfInfo();
                    if (!pih.Read(strIn, hdrVersion))
                        return false;

                    strIn.SetCurRawFieldInPos(pih.CourseOffset);

					PerformanceHeader = rmh;
					PerformanceInfo = pih;
                }

                PFile.CourseHeader hdr = PFile.CourseHeaderCreate();

                obj = strIn.GetNextStructureValue(hdr.GetType());
                if (obj == null) return false;
                hdr = (PFile.CourseHeader)obj;

                if (hdr.Tag != xval.RMPCourseTag)
                    return false;

                Name = new UTF8Encoding().GetString(hdr.Name, 0, PerfFile.TrimEndZero(ref hdr.Name));
                Description = new UTF8Encoding().GetString(hdr.Description, 0, PerfFile.TrimEndZero(ref hdr.Description));
                //FileName = new UTF8Encoding().GetString(hdr.FileName, 0, PerfFile.TrimEndZero(ref hdr.FileName));
                Type = (CourseType)hdr.Type;
                StartAt = hdr.StartAt;
                EndAt = hdr.EndAt;
				Laps = hdr.Laps;

				Attributes = (CourseAttributes)hdr.Attributes;
				/*
                Looped = (((CourseAttributes)hdr.Attributes & CourseAttributes.Looped) != CourseAttributes.Zero);
                Mirror = (((CourseAttributes)hdr.Attributes & CourseAttributes.Mirror) != CourseAttributes.Zero);
                Reverse = (((CourseAttributes)hdr.Attributes & CourseAttributes.Reverse) != CourseAttributes.Zero);
                Modified = (((CourseAttributes)hdr.Attributes & CourseAttributes.Modified) != CourseAttributes.Zero);
                //OutAndBack = (((CourseAttributes)hdr.Attributes & CourseAttributes.OutAndBack) != CourseAttributes.Zero);
				*/

				Type |= CourseType.Performance;
                if (hdrVersion > 1.00f)
                {
                    XUnits = (CourseXUnits)hdr.XUnits;
                    YUnits = (CourseYUnits)hdr.YUnits;
					m_OriginalHash = new UTF8Encoding().GetString(hdr.OriginalHash, 0, PerfFile.TrimEndZero(ref hdr.OriginalHash));
					m_OriginalCourse = OriginalCourse;
                    tCourseHash = new UTF8Encoding().GetString(hdr.CourseHash, 0, PerfFile.TrimEndZero(ref hdr.OriginalHash));
                    tHeaderHash = new UTF8Encoding().GetString(hdr.HeaderHash, 0, PerfFile.TrimEndZero(ref hdr.HeaderHash));
					if (onlyheader)
					{
						CourseHash = tCourseHash;
						HeaderHash = tHeaderHash;
						return true;
					}
                }

				if (onlyheader)
					return false;

                CourseType ct = Type;
				if ((ct & CourseType.Video) != CourseType.Zero)
				{
					PFile.CourseDataRCV data = new PFile.CourseDataRCV();
					int count = hdr.Count;
					for (int i = 0; i < count; i++)
					{
						obj = strIn.GetNextStructureValue(data.GetType());
						if (obj == null) return false;
						data = (PFile.CourseDataRCV)obj;

                        //if (ms_lowPri && 0 == (count % 100)) Thread.Sleep(0);
						AddLast(new Course.GPSSegment(data.gd, data.Length));
					}
					result = true;
				}
				else if ((ct & CourseType.ThreeD) != CourseType.Zero)
				{
					if (hdrVersion > 1.00f)
					{
						PFile.CourseData3D_v2 data = new PFile.CourseData3D_v2();
						int count = hdr.Count;
						float lastgrade = 0;

						for (int i = 0; i < count; i++)
						{
							obj = strIn.GetNextStructureValue(data.GetType());
							if (obj == null) return false;
							data = (PFile.CourseData3D_v2)obj;
							if (data.Smooth)
								AddLast(new Course.SmoothSegment(data.Length, data.Grade, data.Wind, data.Rotation, lastgrade, data.Grade, data.Divisions));
							else
								AddLast(new Course.PysicalSegment(data.Length, data.Grade, data.Wind, data.Rotation));
							lastgrade = data.Grade;
						}
						result = true;
					}
					else
					{
						PFile.CourseData3D data = new PFile.CourseData3D();
						int count = hdr.Count;

						for (int i = 0; i < count; i++)
						{
							obj = strIn.GetNextStructureValue(data.GetType());
							if (obj == null) return false;
							data = (PFile.CourseData3D)obj;

							AddLast(new Course.PysicalSegment(data.Length, data.Grade, data.Wind, data.Rotation));
						}
						result = true;
					}
				}
				else
				{
					PFile.CourseDataAny data = new PFile.CourseDataAny();
					int count = hdr.Count;
					for (int i = 0; i < count; i++)
					{
						obj = strIn.GetNextStructureValue(data.GetType());
						if (obj == null) return false;
						data = (PFile.CourseDataAny)obj;
						if (YUnits == CourseYUnits.Grade)
							AddLast(new PysicalSegment(data.XUnits, (float)data.StartYUnits, (float)data.EndYUnits, 0));
						else
							AddLast(new Course.WattsSegment(data.XUnits, (float)data.StartYUnits, (float)data.EndYUnits));
					}
					result = true;
				}
				if (result)
				{
					CourseHash = tCourseHash;
					HeaderHash = tHeaderHash;
				}
            }
            catch
            {
                Log.WriteLine("Course file failed to load");
                return false;
            }
		
            return result;
		}
		#endregion

		#region HASH
		int m_HHVersion = -1;
		double m_HHStartAt;
		double m_HHEndAt;
		byte m_HHAttributes;
		int m_HHLaps;
		String m_HeaderHash;

		public String HeaderHash
		{
			get
			{
				if (m_HeaderHash == null || m_HHStartAt != StartAt || m_HHEndAt != EndAt ||
					m_HHAttributes != (byte)Attributes || m_HHLaps != Laps || m_HHVersion != Version)
				{
					HeaderHash = HashOutStream.ComputeHash(String.Format("{0},{1:F3},{2:F3},{3},{4}",
						CourseHash, StartAt, EndAt, (byte)Attributes, Laps));
				}
				return m_HeaderHash;
			}
			set
			{
				m_HHStartAt = StartAt;
				m_HHEndAt = EndAt;
				m_HHAttributes = (byte)Attributes;
				m_HHLaps = Laps;
				m_HeaderHash = value;
				m_HHVersion = Version;
			}

		}

		int m_CourseHashVersion = -1;
		String m_CourseHash = null;
		public String CourseHash
		{
			get
			{
				if (m_CourseHash == null || m_CourseHashVersion != Version)
				{
					HashOutStream inhash = new HashOutStream();
					LinkedListNode<Segment> node;
					int cnt;
					for (cnt = 0,node = Segments.First; node != null; node = node.Next,cnt++)
					{
						inhash.Insert(cnt.ToString());
						inhash.Insert(node.Value.HashString);
					}
					
					inhash.CloseHashOut();
					m_CourseHash = inhash.GetHashOut();
					m_CourseHashVersion = Version;
				}
				return m_CourseHash;
			}
			set
			{
				m_CourseHash = value;
				m_CourseHashVersion = Version;
			}
		}

		#endregion // HASH
	}
	#endregion

}
