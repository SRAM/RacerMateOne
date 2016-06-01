using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;
using System.IO;
using System.Data;
using System.Xml.Linq;
// Dan added these:
using System.Diagnostics; // Needed for process invocation
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Runtime.InteropServices; //for dllimport
using System.Text.RegularExpressions;


namespace RacerMateOne
{

    public static class RM1_Settings  {
		static RM1_Settings()  {
#if DEBUG
			Log.WriteLine("Settings.cs, RM1_Settings() constructor");
#endif

			RM1.OnTrainerInitialized += new RM1.TrainerInitialized( OnTrainerInitialized );
#if DEBUG
			Log.WriteLine("Settings.cs, RM1_Settings() constructor done");
#endif
		 }

		//=====================================

		private static void OnTrainerInitialized(RM1.Trainer trainer, int left)  {
			if (trainer == null || !trainer.IsConnected) {
				return;
			}

			foreach (TrainerUserConfigurable t in ActiveTrainerList)  {
				if (t.SavedPortName == trainer.PortName)  {
					if (trainer.Type == RM1.DeviceType.VELOTRON)  {
						if (trainer.VelotronData != null)  {
							trainer.VelotronData.ActualChainring = t.VelotronChainring;
							//trainer.SetVelotronParameters();
							trainer.SetVelotron_trnr_Parameters();
						}
					}
					break;
				}
			}
		}

		//================================================================================
		public class GeneralSettings : INotifyPropertyChanged
		{
			public bool NeedSave;

			public event PropertyChangedEventHandler PropertyChanged;
			public void OnPropertyChanged(string name) { if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name)); }

			protected internal GeneralSettings() { }

			private bool m_Commercial = false;
			public bool Commercial { get { return m_Commercial; } set { if (m_Commercial != value) { m_Commercial = value; OnPropertyChanged("Commercial"); } } }

			private bool m_DemoDevice = false;
			public bool DemoDevice { get { return m_DemoDevice; } set { m_DemoDevice = value; OnPropertyChanged("DemoDevice"); } }

			private float m_IdleGrade = 0;
			public float IdleGrade
			{
				get { return m_IdleGrade; }
				set
				{
					m_IdleGrade = value < -15 ? -15 : value > 15 ? 15 : (float)Math.Round(value, 1);
					OnPropertyChanged("IdleGrade");
				}
			}

			private int m_IdleWatts = 100;
			public int IdleWatts
			{
				get { return m_IdleWatts; }
				set
				{
					m_IdleWatts = value < 0 ? 0 : value > 2000 ? 2000 : value;
					OnPropertyChanged("IdleWatts");
				}
			}
			private int m_IdlePercentAT = 50;
			public int IdlePercentAT
			{
				get { return m_IdlePercentAT; }
				set
				{
					m_IdlePercentAT = value < 0 ? 0 : value > 200 ? 200 : value;
					OnPropertyChanged("IdlePercentAT");
				}
			}


			private double m_GradeStep = 1;
			public double GradeStep 
			{
				get { return m_GradeStep; }
				set 
				{
					m_GradeStep = value < 0.1 ? 0.1:value > 1 ? 1:Math.Round(value,1);
					OnPropertyChanged( "GradeStep" );
				}
			}
			private double m_SS_GradeStep = 1;
			public double SS_GradeStep
			{
				get { return m_SS_GradeStep; }
				set
				{
					m_SS_GradeStep = value < 0.1 ? 0.1 : value > 1 ? 1 : Math.Round(value, 1);
					OnPropertyChanged("SS_GradeStep");
				}
			}
			private double m_GradeInitial = 0;
			public double GradeInitial
			{
				get { return m_GradeInitial; }
				set
				{
					m_GradeInitial = value < -15 ? -15 : value > 15 ? 15 : Math.Round(value, 1);
					OnPropertyChanged("GradeInitial");
				}
			}
			private double m_SS_GradeInitial = 0;
			public double SS_GradeInitial 
			{
				get { return m_SS_GradeInitial; }
				set 
				{
					m_SS_GradeInitial = value < -15 ? -15 : value > 15 ? 15 : Math.Round(value, 1);
					OnPropertyChanged( "SS_GradeInitial" );
				}
			}


			private int m_WattsStep = 50;
			public int WattsStep
			{
				get { return m_WattsStep; }
				set
				{
					m_WattsStep = value < 1 ? 1 : value > 100 ? 100 : value;
					OnPropertyChanged("WattsStep");
				}
			}
			private int m_WattsInitial = 100;
			public int WattsInitial
			{
				get { return m_WattsInitial; }
				set
				{
					m_WattsInitial = value < 0 ? 0 : value > 2000 ? 2000 : value;
					OnPropertyChanged("WattsInitial");
				}
			}
			private int m_PercentATStep = 1;
			public int PercentATStep
			{
				get { return m_PercentATStep; }
				set
				{
					m_PercentATStep = value < 1 ? 1 : value > 10 ? 10 : value;
					OnPropertyChanged("PercentATStep");
				}
			}
			private int m_PercentATInitial = 50;
			public int PercentATInitial
			{
				get { return m_PercentATInitial; }
				set
				{
					m_PercentATInitial = value < 0 ? 0 : value > 200 ? 200 : value;
					OnPropertyChanged("PercentATInitial");
				}
			}



			private bool m_SelectWatts = false;
			public bool SelectWatts { get { return m_SelectWatts; } set { m_SelectWatts = value; OnPropertyChanged("SelectWatts"); } }

			private bool m_ManualControl = false;
			public bool ManualControl { get { return m_ManualControl; } set { m_ManualControl = value; OnPropertyChanged("ManualControl"); } }

			private bool m_SS_ManualControl = false;
			public bool SS_ManualControl { get { return m_SS_ManualControl; } set { m_SS_ManualControl = value; OnPropertyChanged("SS_ManualControl"); } }


			private int m_GraphSpeed_Min = 0;
			public int GraphSpeed_Min { get { return m_GraphSpeed_Min; } set { m_GraphSpeed_Min = value; OnPropertyChanged("GraphSpeed_Min"); } }
			
			private int m_GraphSpeed_Max = 40;
			public int GraphSpeed_Max { get { return m_GraphSpeed_Max; } set { m_GraphSpeed_Max = value; OnPropertyChanged("GraphSpeed_Max"); } }
			
			private int m_GraphPower_Min = 0;
			public int GraphPower_Min { get { return m_GraphPower_Min; } set { m_GraphPower_Min = value; OnPropertyChanged("GraphPower_Min"); } }
			
			private int m_GraphPower_Max = 500;
			public int GraphPower_Max { get { return m_GraphPower_Max; } set { m_GraphPower_Max = value; OnPropertyChanged("GraphPower_Max"); } }
			
			private int m_GraphRPM_Min = 0;
			public int GraphRPM_Min { get { return m_GraphRPM_Min; } set { m_GraphRPM_Min = value; OnPropertyChanged("GraphRPM_Min"); } }
			
			private int m_GraphRPM_Max = 200;
			public int GraphRPM_Max { get { return m_GraphRPM_Max; } set { m_GraphRPM_Max = value; OnPropertyChanged("GraphRPM_Max"); } }

            private int m_GraphRPM_Target = 90;
            public int GraphRPM_Target { get { return m_GraphRPM_Target; } set { m_GraphRPM_Target = value; OnPropertyChanged("GraphRPM_Target"); } }
		
			private int m_GraphHR_Min = 0;
			public int GraphHR_Min { get { return m_GraphHR_Min; } set { m_GraphHR_Min = value; OnPropertyChanged("GraphHR_Min"); } }
			
			private int m_GraphHR_Max = 220;
			public int GraphHR_Max { get { return m_GraphHR_Max; } set { m_GraphHR_Max = value; OnPropertyChanged("GraphHR_Max"); } }
			
			private int m_GraphPulsePower_Min = 0;
			public int GraphPulsePower_Min { get { return m_GraphPulsePower_Min; } set { m_GraphPulsePower_Min = value; OnPropertyChanged("GraphPulsePower_Min"); } }
			
			private int m_GraphPulsePower_Max = 2000;
			public int GraphPulsePower_Max { get { return m_GraphPulsePower_Max; } set { m_GraphPulsePower_Max = value; OnPropertyChanged("GraphPulsePower_Max"); } }

			private int m_GraphSpinScanPercent_Min = 0;
			public int GraphSpinScanPercent_Min { get { return m_GraphSpinScanPercent_Min; } set { m_GraphSpinScanPercent_Min = value; OnPropertyChanged("GraphSpinScanPercent_Min"); } }
			
			private int m_GraphSpinScanPercent_Max = 100;
			public int GraphSpinScanPercent_Max { get { return m_GraphSpinScanPercent_Max; } set { m_GraphSpinScanPercent_Max = value; OnPropertyChanged("GraphSpinScanPercent_Max"); } }
			
			private int m_GraphPowerSplit_Min = 0;
			public int GraphPowerSplit_Min { get { return m_GraphPowerSplit_Min; } set { m_GraphPowerSplit_Min = value; OnPropertyChanged("GraphPowerSplit_Min"); } }
			
			private int m_GraphPowerSplit_Max = 1000;
			public int GraphPowerSplit_Max { get { return m_GraphPowerSplit_Max; } set { m_GraphPowerSplit_Max = value; OnPropertyChanged("GraphPowerSplit_Max"); } }
			
			private int m_GraphATA_Min = 0;
			public int GraphATA_Min { get { return m_GraphATA_Min; } set { m_GraphATA_Min = value; OnPropertyChanged("GraphATA_Min"); } }
			
			private int m_GraphATA_Max = 180;
			public int GraphATA_Max { get { return m_GraphATA_Max; } set { m_GraphATA_Max = value; OnPropertyChanged("GraphATA_Max"); } }

			public bool AllowDrafting { get { return Unit.AllowDrafting; } set { Unit.AllowDrafting = value; OnPropertyChanged("AllowDrafting"); } }

			private bool m_ShowKeypadPresses = false;
			public bool ShowKeypadPresses { get { return m_ShowKeypadPresses; } set { m_ShowKeypadPresses = value; OnPropertyChanged("ShowKeypadPresses"); } }


			private String m_SelectedRiderKey = "";
			public String SelectedRiderKey
			{
				get { return m_SelectedRiderKey; }
				set { if (m_SelectedRiderKey != value) { m_SelectedRiderKey = value; OnPropertyChanged("SelectedRiderKey"); } }
			}


			private bool m_CalibrationCheck = true;
			public bool CalibrationCheck { get { return m_CalibrationCheck; } set { m_CalibrationCheck = value; OnPropertyChanged("CalibrationCheck"); OnPropertyChanged("DontAskCalibration"); } }
			public bool DontAskCalibration { get { return !m_CalibrationCheck; } set { CalibrationCheck = value; } }

			private bool m_Metric = false;
			public bool Metric 
			{ 
				get { return m_Metric; } 
				set 
				{
					if (m_Metric != value)
					{
						m_Metric = value;
						OnPropertyChanged("Metric");
					}
				} 
			}

			private bool m_Launch = false;
			public bool Launch
			{
				get { return m_Launch; }
				set { if (m_Launch != value) { m_Launch = value; OnPropertyChanged("Launch"); OnPropertyChanged("LaunchPrompt"); } }
			}

			private String m_LaunchProgram = "";
			public String LaunchProgram
			{
				get { return m_LaunchProgram; }
				set
				{
					if (String.Compare(m_LaunchProgram, value, true) != 0)
					{
						m_LaunchProgram = value; OnPropertyChanged("LaunchProgram"); OnPropertyChanged("LaunchProgramName");
					}
				}
			}
			public String LaunchProgramName
			{
				get 
				{
					String s;
					try { s = System.IO.Path.GetFileName(m_LaunchProgram); }
					catch { s = ""; }
					return s;
				}
			}
			private bool m_LaunchPrompt = false;
			public bool LaunchPrompt
			{
				get { return m_Launch ? m_LaunchPrompt:false; }
				set { if (m_LaunchPrompt != value) { m_LaunchPrompt = value; OnPropertyChanged("LaunchPrompt"); } }
			}
			private bool m_SavePrompt = false;
			public bool SavePrompt
			{
				get { return m_SavePrompt; }
				set { if (m_SavePrompt != value) { m_SavePrompt = value; OnPropertyChanged("SavePrompt"); } }
			}


			private bool m_ExportSave = false;
			public bool ExportSave
			{
				get { return m_ExportSave; }
				set { if (m_ExportSave != value) { m_ExportSave = value; OnPropertyChanged("ExportSave"); OnPropertyChanged("ExportPrompt"); } }
			}
			private bool m_ExportPrompt = true;
			public bool ExportPrompt
			{
				get { return m_ExportSave ? m_ExportPrompt:false; }
				set { if (m_ExportPrompt != value) { m_ExportPrompt = value; OnPropertyChanged("ExportPrompt"); } }
			}
			private char m_Delimiter = ',';
			public char Delimiter
			{
				get { return m_Delimiter; }
				set { if (m_Delimiter != value) { m_Delimiter = value; OnPropertyChanged("Delimiter"); OnPropertyChanged("DelimiterIndex"); } }
			}
			public int DelimiterIndex
			{
				get
				{
					switch (m_Delimiter)
					{
						case ',': return 0;
						case '\t': return 1;
						case ';': return 2;
						case '.': return 3;
						case '"': return 4;
					}
					return 0;
				}
				set
				{
					switch (value)
					{
						case 0: Delimiter = ','; break;
						case 1: Delimiter = '\t'; break;
						case 2: Delimiter = ';'; break;
						case 3: Delimiter = ','; break;
						case 4: Delimiter = '"'; break;
					}
				}
			}

			private bool m_PWXSave = false;
			public bool PWXSave
			{
				get { return m_PWXSave; }
				set { if (m_PWXSave != value) { m_PWXSave = value; OnPropertyChanged("PWXSave"); OnPropertyChanged("PWXPrompt"); } }
			}
			private bool m_PWXPrompt = true;
			public bool PWXPrompt
			{
				get { return m_PWXSave ? m_PWXPrompt : false; }
				set { if (m_PWXPrompt != value) { m_PWXPrompt = value; OnPropertyChanged("PWXPrompt"); } }
			}
			private bool m_bPWXSaveToCustom;
			public int PWXSaveToNumber
			{
				get { return m_bPWXSaveToCustom ? 1 : 0; }
				set
				{
					bool v = value > 0;
					if (v != m_bPWXSaveToCustom) { m_bPWXSaveToCustom = v; OnPropertyChanged("PWXSaveToNumber"); }
				}
			}
			private string m_PWXSaveTo = "";
			public string PWXSaveTo
			{
				get { return m_PWXSaveTo; }
				set
				{
					if (value != m_PWXSaveTo) { m_PWXSaveTo = value; OnPropertyChanged("PWXSaveTo"); OnPropertyChanged("PWXSaveToNumber"); OnPropertyChanged("PWXSaveToTitle"); OnPropertyChanged("PWXSaveToVisibility"); }
				}
			}
			public Visibility PWXSaveToVisibility
			{
				get { return m_PWXSaveTo == "" || m_PWXSaveTo == null ? Visibility.Collapsed:Visibility.Visible; }
				set
				{
				}
			}
			public string PWXSaveToTitle
			{
				get
				{
					return System.IO.Path.GetFileName(PWXSaveTo);
				}
				set
				{
				}
			}



			private int m_Rate = 1;
			public int Rate
			{
				get { return m_Rate; }
				set
				{
					int v = value < 0 ? 0 : value > 60 ? 60 : value;
					if (v != m_Rate)
					{
						m_Rate = v;
						OnPropertyChanged("Rate");
						OnPropertyChanged("RateIndex");
					}
				}
			}
			public int RateIndex
			{
				get
				{
					if (m_Rate < 1)
						return 0;
					if (m_Rate < 2)
						return 1;
					if (m_Rate < 5)
						return 2;
					return 3;
				}
				set
				{
					switch (value)
					{
						case 0: Rate = 0; break;
						case 1: Rate = 1; break;
						case 2: Rate = 2; break;
						case 5: Rate = 5; break;
					}
				}
			}

			private bool m_ReportPrompt = true;
			public bool ReportPrompt
			{
				get { return m_ReportSave ? m_ReportPrompt : false; }
				set { if (value != m_ReportPrompt) { m_ReportPrompt = value; OnPropertyChanged("ReportPrompt"); } }
			}
			private bool m_ReportSave = true;
			public bool ReportSave
			{
				get { return m_ReportSave; }
				set { if (value != m_ReportSave) { m_ReportSave = value; OnPropertyChanged("ReportSave"); OnPropertyChanged("ReportPrompt");  } }
			}


			public Dictionary<String, CourseInfo> m_SelectedCourse = new Dictionary<string, CourseInfo>();
			public Dictionary<String, CourseInfo> SelectedCourse { get { return m_SelectedCourse; } }


			public Dictionary<String, CourseFilter> m_CourseFilters = new Dictionary<string, CourseFilter>();
			public Dictionary<String, CourseFilter> CourseFilters { get { return m_CourseFilters; } }

			Dictionary<String, ActiveUnits> m_ActiveUnits = new Dictionary<string, ActiveUnits>();
			public Dictionary<String, ActiveUnits> ActiveUnits { get { return m_ActiveUnits; } }

			class VideoHash
			{
				public String CourseHash;
				public String HeaderHash;
				public int FileSize;
				public DateTime FileDate;
				public int AVISize;
				public DateTime AVIDate;

				public bool Load(XElement elem)
				{
					bool ans = true;
					try
					{
						CourseHash = elem.Attribute("CourseHash").Value;
						HeaderHash = elem.Attribute("HeaderHash").Value;
						FileSize = Convert.ToInt32(elem.Attribute("FileSize").Value);
						FileDate = Convert.ToDateTime(elem.Attribute("FileDate").Value);
						AVISize = Convert.ToInt32(elem.Attribute("AVISize").Value);
						AVIDate = Convert.ToDateTime(elem.Attribute("AVIDate").Value);
					}
					catch { ans = false; }
					return ans;
				}
				public XElement Save(string filename)
				{
					XElement elem = new XElement("File",
						new XAttribute("Name", filename),
						new XAttribute("CourseHash", CourseHash),
						new XAttribute("HeaderHash", HeaderHash),
						new XAttribute("FileSize", FileSize),
						new XAttribute("FileDate", FileDate),
						new XAttribute("AVISize", AVISize),
						new XAttribute("AVIDate", AVIDate));
					return elem;
				}


			}

			List<String> m_KnownRCV = null;
			public List<String> KnownRCV
			{
				get
				{
					if (m_KnownRCV == null)
						m_KnownRCV = new List<string>();
					m_KnownRCV.Clear();
					foreach (KeyValuePair<String, VideoHash> vp in m_VideoCourseHash)
						m_KnownRCV.Add( vp.Key );
					return m_KnownRCV;
				}
			}

			Dictionary<String, VideoHash> m_VideoCourseHash = new Dictionary<string, VideoHash>();
			public void SetVideoCourseHash(Course course)
			{
				try
				{
					VideoHash vh;
					if (course == null || ((course.Type & CourseType.Video) == CourseType.Zero) || course.AVIFilename == null || course.AVIFilename == "")
						return;
					FileInfo afi = new FileInfo(course.AVIFilename);
					FileInfo fi = new FileInfo(course.FileName);

					if (m_VideoCourseHash.TryGetValue(course.FileName, out vh))
					{
						if (afi.LastWriteTime == vh.AVIDate && (int)afi.Length == vh.AVISize &&
							fi.LastWriteTime == vh.FileDate && (int)fi.Length == vh.FileSize)
						{
							course.CourseHash = vh.CourseHash;
							course.HeaderHash = vh.HeaderHash;
							return;
						}
					}
					vh = new VideoHash();
					vh.AVISize = (int)afi.Length;
					vh.AVIDate = afi.LastWriteTime;
					vh.FileSize = (int)fi.Length;
					vh.FileDate = fi.LastWriteTime;
					vh.CourseHash = course.CourseHash;
					vh.HeaderHash = course.HeaderHash;
					m_VideoCourseHash[course.FileName] = vh;
				}
				catch { }
			}



			private int m_TrainingRiderNumber = 0;
			public int TrainingRiderNumber
			{
				get { return m_TrainingRiderNumber; }
				set
				{
					int v = value;
					if (v >= 0 && v < 8)
					{
						if (v != m_TrainingRiderNumber)
						{
							m_TrainingRiderNumber = v;
							OnPropertyChanged("TrainingRiderNumber");
						}
					}
				}
			}

			private String m_SceneryString;
			public String SceneryString
			{
				get { return m_SceneryString; }
				set
				{
					m_SceneryString = value;
					m_Scenery = null;
					OnPropertyChanged("Scenery");
					OnPropertyChanged("SceneryString");
				}
			}
			private Controls.Render3D.SceneryInfo m_Scenery;
			public Controls.Render3D.SceneryInfo Scenery
			{
				get 
				{
					if (m_Scenery == null)
						m_Scenery = Controls.Render3D.SceneryInfo.Find(m_SceneryString);
					if (m_Scenery == null)
					{
						m_Scenery = Controls.Render3D.SceneryInfo.SceneryList.First();
						m_SceneryString = m_Scenery.ID;
						OnPropertyChanged("Scenery");
						OnPropertyChanged("SceneryString");
					}
					return m_Scenery; 
				}
				set
				{
					if (m_Scenery == value || value == null)
						return;
					m_Scenery = value;
					if (value != null)
					{
						m_SceneryString = value.ID;
						OnPropertyChanged("SceneryString");
					}
					OnPropertyChanged("Scenery");
				}
			}



			const String c_Path_RCV = @"C:\Real Course Video";
			public String Path_BaseRCV { get { return c_Path_RCV; } }

			String m_Path_RCV = c_Path_RCV;
			public String Path_RCV
			{
				get 
				{ 
					if (Directory.Exists( m_Path_RCV ))
						return m_Path_RCV;
					if (Directory.Exists(c_Path_RCV))
						return c_Path_RCV;
					return null;
				}
				set
				{
					if (Directory.Exists( value ))
						XPath_RCV = value;
				}
			}
			public String XPath_RCV
			{
				get { return m_Path_RCV; }
				set 
				{
					m_Path_RCV = value;
					OnPropertyChanged( "Path_RCV" );
					OnPropertyChanged( "XPath_RCV" );
				}
			}

			public void Load(XDocument indoc)
			{
				XElement root = indoc.Root;
				XElement elem;
				XAttribute att;
				if ((elem = root.Element("Commercial")) != null)
					Commercial = Convert.ToBoolean(elem.Value);

				if ((elem = root.Element("Paths")) != null)
				{
					XElement sub;
					if ((sub = elem.Element("RCV")) != null)
						XPath_RCV = sub.Value;
				}

				if ((elem = root.Element("AllowDrafting")) != null)
					AllowDrafting = Convert.ToBoolean(elem.Value);
				if ((elem = root.Element("ShowKeypadPresses")) != null)
					ShowKeypadPresses = Convert.ToBoolean(elem.Value);

                if ((elem = root.Element("DemoDevice")) != null)
                { // I'm killing this function PAS
                    //DemoDevice = Convert.ToBoolean(elem.Value);
                    DemoDevice = false;
                }

				try { Metric = Convert.ToBoolean(root.Element("Metric").Value); }
				catch { }

				if ((elem = root.Element("Idle")) != null)
				{
					try { IdleGrade = (float)Convert.ToDouble(elem.Attribute("Grade").Value); }
					catch { }
					try { IdleWatts = Convert.ToInt32(elem.Attribute("Watts").Value); }
					catch { }
					try { IdlePercentAT = Convert.ToInt32(elem.Attribute("PercentAT").Value); }
					catch { }
				}
				if ((elem = root.Element("GradeStep")) != null)
				{
					try { GradeStep = Convert.ToDouble(elem.Attribute("PowerTraining").Value); }
					catch { }
					try { GradeInitial = Convert.ToDouble(elem.Attribute("PowerTrainingInitial").Value); }
					catch { }
					try { SS_GradeStep = Convert.ToDouble(elem.Attribute("SpinScan").Value); }
					catch { }
					try { SS_GradeInitial = Convert.ToDouble(elem.Attribute("SpinScanInitial").Value); }
					catch { }
				}
				if ((elem = root.Element("WattsStep")) != null)
				{
					try { WattsStep = Convert.ToInt32(elem.Value); }
					catch { }
					try { WattsInitial = Convert.ToInt32(elem.Attribute("Initial").Value); }
					catch { }
				}
				if ((elem = root.Element("PercentATStep")) != null)
				{
					try { PercentATStep = Convert.ToInt32(elem.Value); }
					catch { }
					try { PercentATInitial = Convert.ToInt32(elem.Attribute("Initial").Value); }
					catch { }
				}

				// Asking requesters
				if ((elem = root.Element("Ask")) != null)
				{
					try { CalibrationCheck = Convert.ToBoolean(elem.Attribute("Calibration").Value); }
					catch { }
				}


				if ((elem = root.Element("ManualControl")) != null)
				{
					try { ManualControl = Convert.ToBoolean(elem.Attribute("PowerTraining").Value); }
					catch { }
					try { SS_ManualControl = Convert.ToBoolean(elem.Attribute("SpinScan").Value); }
					catch { }
				}

				if ((elem = root.Element("SelectWatts")) != null)
				{
					try { SelectWatts = Convert.ToBoolean(root.Element("SelectWatts").Value); }
					catch { }
				}

				try { TrainingRiderNumber = Convert.ToInt32(root.Element("TrainingRider").Value) - 1; }
				catch { }

				if ((elem = root.Element("Scenery")) != null)
				{
					SceneryString = elem.Value.ToString();
				}

				elem = root.Element("SelectedRiderKey");
				if (elem != null)
					SelectedRiderKey = elem.Value;

				elem = root.Element("Graph");
				if (elem != null)
				{
					XElement graphelem;
					if ((graphelem = elem.Element("Speed")) != null)
					{
						try { GraphSpeed_Min = Convert.ToInt32(graphelem.Attribute("Min").Value); }
						catch { }
						try { GraphSpeed_Max = Convert.ToInt32(graphelem.Attribute("Max").Value); }
						catch { }
					}
					if ((graphelem = elem.Element("Power")) != null)
					{
						try { GraphPower_Min = Convert.ToInt32(graphelem.Attribute("Min").Value); }
						catch { }
						try { GraphPower_Max = Convert.ToInt32(graphelem.Attribute("Max").Value); }
						catch { }
					}
					if ((graphelem = elem.Element("RPM")) != null)
					{
						try { GraphRPM_Min = Convert.ToInt32(graphelem.Attribute("Min").Value); }
						catch { }
						try { GraphRPM_Max = Convert.ToInt32(graphelem.Attribute("Max").Value); }
						catch { }
					}
                    if ((graphelem = elem.Element("RPMTarget")) != null)
                    {
                        try { GraphRPM_Target = Convert.ToInt32(graphelem.Value); }
                        catch { }
                    }
					if ((graphelem = elem.Element("HR")) != null)
					{
						try { GraphHR_Min = Convert.ToInt32(graphelem.Attribute("Min").Value); }
						catch { }
						try { GraphHR_Max = Convert.ToInt32(graphelem.Attribute("Max").Value); }
						catch { }
					}
					if ((graphelem = elem.Element("PulsePower")) != null)
					{
						try { GraphPulsePower_Min = Convert.ToInt32(graphelem.Attribute("Min").Value); }
						catch { }
						try { GraphPulsePower_Max = Convert.ToInt32(graphelem.Attribute("Max").Value); }
						catch { }
					}
					if ((graphelem = elem.Element("SpinScanPercent")) != null)
					{
						try { GraphSpinScanPercent_Min = Convert.ToInt32(graphelem.Attribute("Min").Value); }
						catch { }
						try { GraphSpinScanPercent_Max = Convert.ToInt32(graphelem.Attribute("Max").Value); }
						catch { }
					}
					if ((graphelem = elem.Element("PowerSplit")) != null)
					{
						try { GraphPowerSplit_Min = Convert.ToInt32(graphelem.Attribute("Min").Value); }
						catch { }
						try { GraphPowerSplit_Max = Convert.ToInt32(graphelem.Attribute("Max").Value); }
						catch { }
					}
					if ((graphelem = elem.Element("ATA")) != null)
					{
						try { GraphATA_Min = Convert.ToInt32(graphelem.Attribute("Min").Value); }
						catch { }
						try { GraphATA_Max = Convert.ToInt32(graphelem.Attribute("Max").Value); }
						catch { }
					}
				}

				elem = root.Element("SelectedCourse");
				if (elem != null)
				{
					IEnumerable<XElement> nodelist = elem.Elements("CourseInfo");
					foreach (XElement ele in nodelist)
					{
						CourseInfo cinfo = new CourseInfo();
						if (cinfo.Load(ele))
						{
							XAttribute key = ele.Attribute("Key");
							if (key != null)
								m_SelectedCourse[key.Value] = cinfo;
						}
					}
				}
				elem = root.Element("CourseFilters");
				if (elem != null)
				{
					IEnumerable<XElement> nodelist = elem.Elements("Filter");
					foreach (XElement ele in nodelist)
					{
						XAttribute key = ele.Attribute("Key");
						XAttribute fname = ele.Attribute("Selected");
						CourseFilter cf;
						if (key != null && fname != null && (cf = CourseFilter.Find(fname.Value.ToString())) != null)
						{
							m_CourseFilters[key.Value.ToString()] = cf;
						}
					}
				}

				elem = root.Element("ActiveUnits");
				if (elem != null)
				{
					IEnumerable<XElement> nodelist = elem.Elements("Mode");
					foreach (XElement ele in nodelist)
					{
						XAttribute key = ele.Attribute("Key");
						XAttribute units = ele.Attribute("Units");
						if (key != null)
						{
							try { m_ActiveUnits[key.Value.ToString()] = (ActiveUnits)Enum.Parse(typeof(ActiveUnits), units.Value); }
							catch { }
						}
					}
				}

				elem = root.Element("VideoHashes");
				if (elem != null)
				{
					IEnumerable<XElement> nodelist = elem.Elements("File");
					foreach (XElement ele in nodelist)
					{
						XAttribute key = ele.Attribute("Name");
						if (key != null)
						{
							VideoHash vh = new VideoHash();
							if (vh.Load(ele))
								m_VideoCourseHash[key.Value.ToString()] = vh;
						}
					}
				}

				elem = root.Element("PerformanceSavePrompt");
				if (elem != null)
				{
					try { SavePrompt = Convert.ToBoolean(elem.Value); }
					catch { }
				}

				elem = root.Element("Export");
				if (elem != null)
				{
					m_LaunchProgram = elem.Value.ToString();
					if ((att = elem.Attribute("Launch")) != null)
						try { Launch = Convert.ToBoolean(att.Value); }
						catch { }
					if ((att = elem.Attribute("LaunchPrompt")) != null)
						try { LaunchPrompt = Convert.ToBoolean(att.Value); }
						catch { }
					if ((att = elem.Attribute("Save")) != null)
						try { ExportSave = Convert.ToBoolean(att.Value); }
						catch { }
					if ((att = elem.Attribute("Prompt")) != null)
						try { ExportPrompt = Convert.ToBoolean(att.Value); }
						catch { }
					if ((att = elem.Attribute("Delimiter")) != null)
						try { Delimiter = Convert.ToChar(att.Value); }
						catch { }
					if ((att = elem.Attribute("Rate")) != null)
						try { Rate = Convert.ToInt32(att.Value); }
						catch { }
				}
				elem = root.Element("PWXExport");
				if (elem != null)
				{
					try { m_PWXSaveTo = elem.Value.ToString(); }
					catch { }
					if ((att = elem.Attribute("Save")) != null)
						try { PWXSave = Convert.ToBoolean(att.Value); }
						catch { }
					if ((att = elem.Attribute("Prompt")) != null)
						try { PWXPrompt = Convert.ToBoolean(att.Value); }
						catch { }
					if ((att = elem.Attribute("DefaultLocation")) != null)
						try { PWXSaveToNumber = Convert.ToBoolean(att.Value) ? 0:1; }
						catch { }
				}


				if ((elem = root.Element("Reports")) != null)
				{
					if ((att = elem.Attribute("Save")) != null)
						try { ReportSave = Convert.ToBoolean(att.Value); }
						catch { }
					if ((att = elem.Attribute("Prompt")) != null)
						try { ReportPrompt = Convert.ToBoolean(att.Value); }
						catch { }
				}

				if ((elem = root.Element("BrowsePath")) != null)
				{
					try { RacerMatePaths.BrowsePath = elem.Value.ToString(); }
					catch { }
				}
			}
			public void Save(XElement root)
			{
				NeedSave = false;
				root.Add(new XElement("Commercial", m_Commercial));
				root.Add(new XElement("Metric", Metric));
				root.Add(new XElement("Paths",
					new XElement("RCV",Path_RCV)));
				root.Add(new XElement("Ask",
					new XAttribute("Calibration",m_CalibrationCheck)));
				root.Add(new XElement("ManualControl",
					new XAttribute("SpinScan",m_SS_ManualControl),
					new XAttribute("PowerTraining",m_ManualControl)));
				root.Add(new XElement("WattsStep", m_WattsStep,
					new XAttribute("Initial",m_WattsInitial)));
				root.Add(new XElement("PercentATStep", m_PercentATStep,
					new XAttribute("Initial",m_PercentATInitial)));
				root.Add(new XElement("GradeStep", 
					new XAttribute("SpinScan",m_SS_GradeStep),
					new XAttribute("SpinScanInitial",m_SS_GradeInitial),
					new XAttribute("PowerTraining",m_GradeStep),
					new XAttribute("PowerTrainingInitial",m_GradeInitial)));		
				root.Add(new XElement("Idle",
					new XAttribute("Grade",m_IdleGrade),
					new XAttribute("Watts",m_IdleWatts),
					new XAttribute("PercentAT",m_IdlePercentAT)));
				root.Add(new XElement("SelectWatts", m_SelectWatts));

				root.Add(new XElement("DemoDevice", m_DemoDevice));
				root.Add(new XElement("SelectedRiderKey", m_SelectedRiderKey ));

				root.Add(new XElement("AllowDrafting", AllowDrafting));
				root.Add(new XElement("ShowKeypadPresses", ShowKeypadPresses));
				

				root.Add(new XElement("TrainingRider",m_TrainingRiderNumber+1));
				root.Add(new XElement("Scenery",m_SceneryString));

				if (RacerMatePaths.BrowsePath != RacerMatePaths.RacerMateFullPath)
				{
					root.Add(new XElement("BrowsePath",RacerMatePaths.BrowsePath));
				}

				XElement graph = new XElement("Graph",
					new XElement("Speed", new XAttribute("Min", m_GraphSpeed_Min), new XAttribute("Max", m_GraphSpeed_Max)),
					new XElement("Power", new XAttribute("Min", m_GraphPower_Min), new XAttribute("Max", m_GraphPower_Max)),
					new XElement("RPM", new XAttribute("Min", m_GraphRPM_Min), new XAttribute("Max", m_GraphRPM_Max)),
                    new XElement("RPMTarget", m_GraphRPM_Target),				
                    new XElement("HR", new XAttribute("Min", m_GraphHR_Min), new XAttribute("Max", m_GraphHR_Max)),
					new XElement("PulsePower", new XAttribute("Min", m_GraphPulsePower_Min), new XAttribute("Max", m_GraphPulsePower_Max)),
					new XElement("SpinScanPercent", new XAttribute("Min", m_GraphSpinScanPercent_Min), new XAttribute("Max", m_GraphSpinScanPercent_Max)),
					new XElement("PowerSplit", new XAttribute("Min", m_GraphPowerSplit_Min), new XAttribute("Max", m_GraphPowerSplit_Max)),
					new XElement("ATA", new XAttribute("Min", m_GraphATA_Min), new XAttribute("Max", m_GraphATA_Max))
					);
				root.Add(graph);

				XElement savep = new XElement("PerformanceSavePrompt", m_SavePrompt);
				root.Add(savep);

				XElement export = new XElement("Export", m_LaunchProgram,
					new XAttribute("Launch", m_Launch),
					new XAttribute("LaunchPrompt", m_LaunchPrompt),
					new XAttribute("Save", m_ExportSave),
					new XAttribute("Prompt", m_ExportPrompt),
					new XAttribute("Delimiter", m_Delimiter),
					new XAttribute("Rate", m_Rate));
				root.Add(export);

				XElement pwx = new XElement("PWXExport",
					m_PWXSaveTo,
					new XAttribute("Save", m_PWXSave),
					new XAttribute("Prompt", m_PWXPrompt),
					new XAttribute("DefaultLocation",PWXSaveToNumber > 0 ? false:true));
				root.Add(pwx);
				
				root.Add(new XElement("Reports", 
					new XAttribute("Save", m_ReportSave),
					new XAttribute("Prompt", m_ReportPrompt)));

				XElement sc = new XElement("SelectedCourse");
				foreach (KeyValuePair<String, CourseInfo> kv in m_SelectedCourse)
				{
					XElement e = kv.Value.ToXElement( kv.Key );
					sc.Add(e);
				}
				root.Add(sc);

				XElement cfx = new XElement("CourseFilters");
				foreach (KeyValuePair<String, CourseFilter> kc in m_CourseFilters )
				{
					XElement e = new XElement("Filter");
					e.Add(new XAttribute("Key", kc.Key));
					e.Add(new XAttribute("Selected", kc.Value.Key ));
					cfx.Add(e);
				}
				root.Add(cfx);

				XElement au = new XElement("ActiveUnits");
				foreach (KeyValuePair<String, ActiveUnits> acc in m_ActiveUnits)
				{
					XElement e = new XElement("Mode", new XAttribute("Key", acc.Key), new XAttribute("Units", acc.Value));
					au.Add(e);
				}
				root.Add(au);

				XElement vu = new XElement("VideoHashes");
				foreach (KeyValuePair<String, VideoHash> vp in m_VideoCourseHash)
				{
					XElement e = vp.Value.Save(vp.Key);
					vu.Add(e);
				}
				root.Add(vu);
			}
		}
		public static GeneralSettings General = new GeneralSettings();
		//================================================================================


    
        private const string DefaultSettingsFilename = @"\RM1_DefaultSettings.xml";
 
        private const string SettingsFilename = @"\RM1_Settings.xml";
        private const string SettingsXSLFilename = @"\RM1_Settings.xsl"; // private
        // private since I'm not sure why other classes would need to see this at compile time.
        // only this class should do operations with the Settings where the Versions would be relevant

        private const string defaultSettingsMajorVersion = "1";
        private const string defaultSettingsMinorVersion = "0";  //private since I'm not sure why other classes need to see this at compile time??
        // These paths used ONLY when there is no entry in the application's CoursePathRoot entry, made privat
        private const string _defaultCoursesRCV = @"\RCV";
        private const string _defaultCoursesCrs3D = @"\Crs3D";
        private const string _defaultCoursesErgMrc = @"\ErgMrc";
        private const string _defaultCoursesErgVideo = @"\ErgVideo";
 


        private static string _majorversion = defaultSettingsMajorVersion;  //initialize to default then takes from file
        private static string _minorversion = defaultSettingsMinorVersion;  //initialize to default then takes from file

        //make the version readonly, not sure why other classes would be built outside this class to change the Version, so till then, readonly.
        
        //define some GlobalSettings
        
        //settings objects of the form GeneralSetting

    

        /** <summary>True if first run or flag is set in xml file.</summary> */
        public static bool gFirstRun = true;


        public static string MajorVersion
        { get { return _majorversion; } }
        public static string MinorVersion
        { get { return _minorversion; } }

        private static string applicationdir = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).ToString()).LocalPath;

        public static List<TrainerUserConfigurable> SavedTrainersList = new List<TrainerUserConfigurable>();
		
		public static List<TrainerUserConfigurable> ActiveTrainerList = new List<TrainerUserConfigurable>();

        //public static List<Trainer> VerboseSavedTrainersList = new List<Trainer>();

        /// <summary>
        /// Static constructor for the RM1_Settings class, loaded from a file
        /// </summary>
        /// <returns>false if failure and termination of program required</returns>
        public static bool LoadSettings()
        {
             bool fromsaved = LoadFromFile(RacerMatePaths.SettingsFullPath + SettingsFilename);
             return fromsaved; //caller will terminate app if sees false
        }

		public static void DeleteSettingsFile()
		{
			string realfilename = RacerMatePaths.SettingsFullPath + SettingsFilename;
			File.Delete(realfilename);
		}
   
        /// <summary>
        /// Read in a list of Settings from the Settings file, full path required.  
        /// If a load fails, prompts to load from backups and will reset to default
        /// DefaultRM1_Settings.xml content only when backup reload refused by user.
        /// </summary>
        /// <param name="infilename">Filename to read</param>
        /// <returns>True on success, false when reverted to default DefaultRM1_Settings.xml content</returns>
        private static bool LoadFromFile(string infilename)
        {
            bool retval = false;
            string realfilename = infilename;
            XDocument FullDoc = null;
            bool IsFullDocgoodXML = true;
            bool IsSettingsValid = false;
            bool? FileOpenDialogResult = false;
            bool IsResolved = false;
            
            // ensure that the Settings directory exists by making an access to a static member in RacerMate Paths.
            string xxx = RacerMatePaths.SettingsFullPath;

            if (!File.Exists(realfilename))
            {
                // this is the first run, there is no RM1_Settings file at all. Initialize as first time run.
                CopyGoldenDefaultSettingsFile(realfilename);

            }
            // Open the settings file.
            // reset the validation settings
            do
            {
                IsFullDocgoodXML = true;
                IsSettingsValid = false;
                try
                {
                    FullDoc = XDocument.Load(realfilename);
                }
                catch
                {
                    IsFullDocgoodXML = false;  //when there is a XML style error, caught as exception
                }
                finally { }

                if (IsFullDocgoodXML == true)
                {
                    IsSettingsValid = IsValidSettings(FullDoc);  //test for validity of data and loads the useable classes and properties             
                }
                if (IsSettingsValid == false || IsFullDocgoodXML ==false)
                {
                    // here we have invalid input file for either bad xml or invalid data in good xml.
                    // either way, prompt user to read from a backup file
                    Window_CustomLoadBackupFile diagWin = new Window_CustomLoadBackupFile();
                    diagWin.Title = "Settings file Error";
                    diagWin.CaptionText = "The Settings file is corrupt. Please choose from the options below:";
                    diagWin.ShowDialog(); //shows as modal
                    int returnedvalue = diagWin.UserChoice;
                    switch (returnedvalue)
                    {
                        case 0: //Exit the application Load from backup
                            {
                                retval = false;
                                IsResolved = true; //this pair will terminate the app.
                                break;
                            }
                        case 1: //Load from Backup
                            {
                                OpenFileDialog openFile1 = new OpenFileDialog();
                                openFile1.InitialDirectory = RacerMatePaths.BackupFullPath;
                                openFile1.Multiselect = false;
                                openFile1.Title = "Select a backup Settings file";
                                openFile1.ValidateNames = true;
                                openFile1.AddExtension = true;
                                openFile1.CheckFileExists = true;
                                openFile1.DefaultExt = ".xml";
                                openFile1.Filter = "Settings Files (.xml)|RM1_Settings*.xml";
                                FileOpenDialogResult = openFile1.ShowDialog();
                                if (FileOpenDialogResult == true)
                                {
                                    realfilename = openFile1.FileName;  //new file to test
                                    retval = false;
                                    IsResolved = false;  //this pair will loop and try again
                                }
                                else  //if user does not select a file or hits cancel, there is no second chance on this.
                                {
                                    retval=false;
                                    IsResolved = true; //this pair will terminate the app
                                }
                                break;
                            }
                        case 2: //Load factory default
                            {
                                CopyGoldenDefaultSettingsFile(realfilename);
                                retval = false;
                                IsResolved = false; //need to read the new file
                                break;
                            }
                    }
                }
                else
                {
                    retval = true;
                    IsResolved = true;
                }
            }
            while (IsResolved == false);
            GC.Collect();
            return retval;
         }


        /// <summary>
        /// A File RM1_DefaultSettings.xml exists in the Application Direcory. A copy is made and written Settings Folder as RM1_Settings.xml
        /// The Directory exists because of earlier calls to create the Directories.
        /// </summary>
        private static void CopyGoldenDefaultSettingsFile(string infilename)
        {
            Debug.WriteLine("---------------CopyGoldenSettings");
            string DefaultFilename = RacerMatePaths.DefaultSettingsFullPath + DefaultSettingsFilename;
            string DestinationFilename = RacerMatePaths.SettingsFullPath + SettingsFilename;
            //always creat a backup
            if (File.Exists(infilename)) BackupTodays(infilename);
            File.Copy(DefaultFilename, DestinationFilename, true); //', False, FileIO.UICancelOption.DoNothing)
            // Adding copy of Program files course directory to the Users course directory as well.
            RM1_Settings.CopyDefaultCoursesToLocal();
            Debug.WriteLine("---------------CopyGoldenSettings  Finish");
           

        }
        
        /// <summary>
        /// Save the loaded Settings.
        /// Before writing, a backup of the existing RM1_Settings.xml file is created
        /// with date added to filename as RM1_Settings_yyyy_mm_dd.xml, so that sorting on name results
        /// in newest to oldest or vice versa.
        /// Only 1 backup per day of use is made, and only most recent 10 days activity are recorded.
        /// </summary>
        public static bool SaveToFile()
        {
            // Take existing riders and flush to the file.
            // If any errors during write, let user know
            bool retval = false;
            //before saving check the backup situation
            
            // Using a Temp settings file since I don't want to blow away the working RM1_Settings.xml as I work toward completion
            // view the TEMP file as output save.
            string realfilename = RacerMatePaths.SettingsFullPath + SettingsFilename;
         
            BackupTodays(SettingsFilename); //if fails, ignore.
            //TrainerUserConfigurable[] RationalizedTrainers = CreateRTList(SavedTrainersList, AppWin.TrainersAvailableThisSession);             
            //SavedTrainersList.Clear();
            //VerboseSavedTrainersList.Clear();
            //foreach (TrainerUserConfigurable aaa in RationalizedTrainers)
            //    SavedTrainersList.Add(aaa);  //now the rationalized trainerslist will be scanned on LWScans
         
            XDocument doc = CreateXDocFromSettings();
            if (doc != null)
            {
                try
                {
                    doc.Save(realfilename);
                    retval = true;
                }
                catch
                {
                    MessageBoxResult result = MessageBox.Show("Could not write the Settings file." + Environment.NewLine + "No changes to Settings.",
                        "File Write error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Could not create valid XML from the Settings List." + Environment.NewLine + "No changes to Settings.",
                    "XML error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLine("Could not create valid XML");
            }
            return retval;
        }

         /// <summary>
        /// Takes all relevant settings to be stored in RM1_Settings.xml and creates a valid XDoc object
        /// </summary>
        /// <returns></returns>
		public static XDocument CreateXDocFromSettings()
        {
            XDocument retdoc;
            try
            {
                XElement rootSettings = new XElement("Settings", new XAttribute("VersionMajor", Riders.MajorVersion), new XAttribute("VersionMinor", Riders.MinorVersion));

				XElement FirstRun = new XElement("FirstRun",gFirstRun ? "1":"0");
				rootSettings.Add(FirstRun);


                //Add the Trainers to the Root

                //First build each Trainer as its own XElement,
				Unit.UpdateTrainerData();
                XElement Trainers = new XElement("Trainers");
                foreach (TrainerUserConfigurable aaa in SavedTrainersList)
                {
                    XElement thisXTrainer = new XElement("Trainer", new XAttribute("Index", aaa.PositionIndex),
                        new XAttribute("PreviouslyDiscovered", aaa.PreviouslyDiscovered),
                        new XAttribute("DeviceType", aaa.RememberedDeviceType),
						new XAttribute("Active",aaa.Active),
                        new XElement("VelotronChainring", aaa.VelotronChainring),
                        new XElement("PreviousRiderKey", aaa.PreviousRiderKey));
					if (aaa.SavedPortName != string.Empty)
						thisXTrainer.Add(new XElement("PortName", aaa.SavedPortName));

					if (aaa.BotKey != "")
					{
						thisXTrainer.Add(new XElement("Pacer", aaa.BotKey));
					}
                    Trainers.Add(thisXTrainer);

                }
                rootSettings.Add(Trainers); //complete Trainers

                //Add System
                XElement SystemEle = new XElement("System");
                //Add Global 

                OpMode opmodeRCV = new OpMode(SupportedModes.ModeRCV);
                XElement ModeRCVEle = opmodeRCV.GetAsXElement();
                
                OpMode opmodeErgVideo = new OpMode(SupportedModes.ModeErgVideo);
                XElement ModeErgVideoEle = opmodeErgVideo.GetAsXElement();


                OpMode opmodeGroupRacing = new OpMode(SupportedModes.ModeGroupRacing);
                XElement ModeGroupRacingEle = opmodeGroupRacing.GetAsXElement();


                OpMode opmodeWattTesting = new OpMode(SupportedModes.ModeWattTesting);
                XElement ModeWattTestingEle = opmodeWattTesting.GetAsXElement();

 
                OpMode opmodeSpinScanTraining = new OpMode(SupportedModes.ModeSpinScanTraining);
                XElement ModeSpinScanEle = opmodeSpinScanTraining.GetAsXElement(); //currently no settings

                OpMode opmodePreviousPerformance = new OpMode(SupportedModes.ModePreviousPerformance);
                XElement ModePreviousPerformanceEle = opmodePreviousPerformance.GetAsXElement(); //currently no settings
    
                SystemEle.Add(ModeRCVEle);
                SystemEle.Add(ModeErgVideoEle);
                SystemEle.Add(ModeGroupRacingEle);

                SystemEle.Add(ModeWattTestingEle);
                SystemEle.Add(ModeSpinScanEle);
                SystemEle.Add(ModePreviousPerformanceEle);


                //and finally with a flourish

                rootSettings.Add(SystemEle);

                //string relativepath = @".." + (RacerMatePaths.ReportTemplatesRelativePath);
                
                string SettingsPath = RacerMatePaths.SettingsFullPath;
                string ReportTemplatesPath = RacerMatePaths.ReportTemplatesFullPath;
                string relativepath = RacerMatePaths.RelativePathTo(SettingsPath, ReportTemplatesPath);
            
                retdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                    new XProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + relativepath + SettingsXSLFilename + "'"),
                    new XComment("RacerMate One Settings"),rootSettings);

				rootSettings.Add(ReportColumns.SaveDB());
				rootSettings.Add(RiderGroup.SaveDB());
				General.Save(rootSettings);
				rootSettings.Add( Courses.SaveFavorites());


				//================================================================================================
            }


            catch
            {
                retdoc = null;
            }
            GC.Collect();  // tidy up a bunch of objects never to be used again.
            return retdoc;
        }
      

        /// <summary>
        /// Makes a backup of RM1_Settings.xml with dated suffix for easy sort.
        /// Only happens once per day of use. Maintains only most recent 10 backups.
        /// </summary>
        /// <param name="filename">filename and it will go in the Backup directory</param>

        private static void BackupTodays(string infilename)
        {
            DateTime today = System.DateTime.Today;
            string suffix = today.Year + "_" + today.Month + "_" + today.Day;
            string realfilename = RacerMatePaths.SettingsFullPath + infilename;
            string FileBackupname = RacerMatePaths.BackupFullPath + @"\" + Path.GetFileNameWithoutExtension(infilename) + suffix + Path.GetExtension(infilename);
            //Debug.WriteLine("backing up " + FileBackupname);
            try
            {
                if (!File.Exists(FileBackupname) && File.Exists(realfilename))
                {
                    File.Copy(realfilename, FileBackupname, false); //', False, FileIO.UICancelOption.DoNothing)
                }
                else
                {
                    // the file exists, we already have a backup for today, and we can make a fast exit
                    return;
                }
            }
            catch
            { //do nothing, not important, in fact this happens on initial load, as load defaults tries to write a db file.
            }


            // getting here means there has been a write and we need to maintain only 10 copies
            // now have a look at this folder and maintain only 10 files
            DirectoryInfo Di = new DirectoryInfo(RacerMatePaths.BackupFullPath);
            FileInfo[] ALLfilesinDi;
            ALLfilesinDi = Di.GetFiles();
            List<DateTime> TheseFilesinDi = new List<DateTime>();
            TheseFilesinDi.Clear();

            foreach (FileInfo aaa in ALLfilesinDi)
            {
                if (aaa.Name.StartsWith(Path.GetFileNameWithoutExtension(infilename)))
                {
                    TheseFilesinDi.Add(aaa.CreationTime);
                }
            }
            TheseFilesinDi.Sort(); //this will make them in increasing order

            DateTime cutdate = System.DateTime.Today;
            if (TheseFilesinDi.Count >= 10)
            {
                // pick the date of the index 9 entry
                cutdate = TheseFilesinDi[TheseFilesinDi.Count - 10];
                foreach (FileInfo aaa in ALLfilesinDi)
                {
                    if (aaa.Name.StartsWith(Path.GetFileNameWithoutExtension(infilename)) &&
                    aaa.LastWriteTime <= cutdate)
                    {
                        try
                        {
                            //Debug.WriteLine("cleaning up " + appfolder + @"\" + aaa.Name);
                            File.Delete(RacerMatePaths.BackupFullPath + @"\" + aaa.Name);
                        }
                        catch { } //do nothing not important

                    }
                }
            }
        }



        /// <summary>
        /// This will parse the RM1_Settings.xml for structure and will load the Settings objects.
        /// Settings objects do range and validity check and reset to default values when unacceptable.
        /// This will fail should the XML document be valid in formed-ness but invalid per
        /// the expected Settings specification.
        /// </summary>
        /// <param name="indoc">An XDocument object representing the Settings</param>
        /// <returns>True on no unmanagable-errors, False when not recoverable from error </returns>
        private static bool IsValidSettings(XDocument indoc)
        {
            bool retval = false;
            try
            {
                XElement rootNode = indoc.Root;  // the rootnode is Settings //
                _majorversion = rootNode.Attribute("VersionMajor").Value;
                _minorversion = rootNode.Attribute("VersionMinor").Value;
                // process the trainer settings
                ParseTrainerSettings(indoc);                
                ParseFirstRun(indoc);

				ReportColumns.LoadDB(indoc);
				RiderGroup.LoadDB(indoc);

				General.Load(indoc);

				try
				{
					Courses.LoadFavorites(rootNode.Element("Favorites"));
				}
				catch { }

                // process the pacers
                //ParsePacerSettings(indoc);
                // completed parsing of the trainer properties.
                retval = true;
           }
            catch
            {//default return value is false but set it anyway for readability
                retval = false;
            }
            return retval;
        }
        private static void ParseFirstRun(XDocument indoc)
        {
            XElement rootNode = indoc.Root;
            XElement firstRunNode = rootNode.Element("FirstRun");
			if (firstRunNode != null)
			{
				String v = firstRunNode.Value.Trim();
				gFirstRun = (firstRunNode != null ? v != "0" && v != "" : true);
			}
        }
        private static void ParseTrainerSettings(XDocument indoc)
        {
            XElement rootNode = indoc.Root;
            XElement TrainersNode = rootNode.Element("Trainers");
            IEnumerable<XElement> Trainernodes = TrainersNode.Elements("Trainer");

            //note that all trainers have gears and serial port entries in the XML, which relieves conditional writes to the settings files.
            foreach (XElement ele in Trainernodes)
            {
                int PositionIndex = Convert.ToInt32(ele.Attribute("Index").Value);
                // need to prevent two trainers from having the same index
                bool duplicated = false;
                foreach (TrainerUserConfigurable ccc in SavedTrainersList)
                {
                    if (ccc.PositionIndex == PositionIndex)
                        duplicated = true;
                }
				if (duplicated == false) {
					TrainerUserConfigurable thisTrainer = new TrainerUserConfigurable(PositionIndex); //this merely establishes a trainer on the port in the trainer object

					thisTrainer.PreviouslyDiscovered = Convert.ToInt32(ele.Attribute("PreviouslyDiscovered").Value); //range check will make correstions if insane value
					XAttribute att;
					if ((att = ele.Attribute("Active")) != null)
						thisTrainer.Active = Convert.ToBoolean(att.Value);

					thisTrainer.RememberedDeviceType = ele.Attribute("DeviceType").Value;
                    XElement portNameElement;
                    portNameElement = ele.Element("PortName");
                    if (portNameElement == null) {
                        // no PortName, so the setting needs to be converted.
                        // look for "Port" or "SerialPort"
                        int savedSerialPortNum = -1;
                        XElement tmpElement;
                        tmpElement = ele.Element("Port");
                        if (tmpElement == null) {
                            tmpElement = ele.Element("SerialPort");
                            if (tmpElement != null) {
                                savedSerialPortNum = Convert.ToInt32(tmpElement.Value);
                                if (savedSerialPortNum == 0)
                                    savedSerialPortNum = -1;
                            }
                        }
                        else {
                            savedSerialPortNum = Convert.ToInt32(tmpElement.Value);
                        }

                        thisTrainer.SavedPortName = "COM" + savedSerialPortNum;
                    }
                    else {
                        thisTrainer.SavedPortName = portNameElement.Value;
                    }

                    thisTrainer.PreviousRiderKey = (string)ele.Element("PreviousRiderKey").Value;
					thisTrainer.VelotronChainring = Convert.ToInt32(ele.Element("VelotronChainring").Value);

					XElement elem = ele.Element("Pacer");
					if (elem != null)
						thisTrainer.BotKey = elem.Value.ToString();
					else
						thisTrainer.BotKey = "";

					SavedTrainersList.Add(thisTrainer);

					// If it is a velotron or a ct
					if (thisTrainer.SavedPortName != string.Empty)  {
						ActiveTrainerList.Add(thisTrainer);
					}

                }//get out before more than 8 trainers read
            }
            
        }
   
        [DllImport("shell32.dll")]
        static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken,
           uint dwFlags, [Out] StringBuilder pszPath);
        /// <summary>
        /// Gets the Public Videos directory path.
        /// </summary>
        /// <returns>The fonts directory path</returns>
        public static String GetPublicVideosDirectory()
        {
            StringBuilder sb = new StringBuilder();
            int CSIDL_COMMON_VIDEO = 0x0037;
            SHGetFolderPath(IntPtr.Zero, CSIDL_COMMON_VIDEO, IntPtr.Zero, 0x0000, sb);
            return sb.ToString();
        }

        /// <summary>
        /// changes Settings Filename to _BeforeRestore.xml
        /// </summary>
        public static void RenamePreserveSettingsFile()
        {
            if(File.Exists(RacerMatePaths.SettingsFullPath + SettingsFilename))
                File.Copy(RacerMatePaths.SettingsFullPath + SettingsFilename, RacerMatePaths.SettingsFullPath + "\\" + Path.GetFileNameWithoutExtension(SettingsFilename) + "_beforeRestore.xml", true);
            File.Delete(RacerMatePaths.SettingsFullPath + SettingsFilename);
        }




        /// <summary>
        /// Rationalizes teh Saved Trainers list and Trainers availble for this session to preserve all Previously discovered trainers
        /// EVEN if they were left assigned in this session. Thus the LW scan will always find PD trainers as long as they are not
        /// obliterated by conflicting re-assignment 
        /// </summary>
        /// <param name="insavedlist">The SavedTrainerList read on program launch</param>
        /// <param name="inAvailable">The available trainers configured for the sessionn</param>
        /// <returns> an array of TrainerUserconfigurable in position order</returns>
		/*
        private static TrainerUserConfigurable[] CreateRTList(List<TrainerUserConfigurable> insavedlist, List<Trainer> inAvailable)
        { 
            //this seems like overkil complexity, and it took a stupid amount of time to crack this issue.
            // The problem solved is one of not simpley storing previous configuration, but alos storing PD trainers on com ports even if
            // the user MAY have not enabled them in the last session. If they are still on non-copnflicting com ports, they should still be 
            // discoverd on LW scans when program restarts.

           TrainerUserConfigurable[] retarray = new TrainerUserConfigurable[RM1Constants.maxtrainersGeneral];
           for (int i = 0; i < retarray.Count(); i++)
           {
               retarray[i] = null;
           }
           foreach (Trainer aaa in inAvailable)
           {
               if (aaa.TrainerUserConfigs.PositionIndex >= 1 && aaa.TrainerUserConfigs.PositionIndex <= 8)
                   retarray[aaa.TrainerUserConfigs.PositionIndex - 1] = aaa.TrainerUserConfigs.CopyMe();   //use a copy
           }
           foreach (TrainerUserConfigurable bbb in insavedlist) //this list has sequential position indices, or at least 1-8 as read in inputfile 
           {
               TrainerUserConfigurable copyofbbb = bbb.CopyMe(); //again use a copy
               if (IsSameCommAssigned(copyofbbb, retarray) == true)
               {
                   copyofbbb.RESETtoDefault(copyofbbb.PositionIndex); //can wipe it out finally since it has been found in com port conflict
               }
               if (retarray[copyofbbb.PositionIndex - 1] == null) //I can replace this now knowing no COM conflicts will occur
                   retarray[copyofbbb.PositionIndex - 1] = copyofbbb;
           }
            return retarray;
        }
		 */

		//private static bool IsSameCommAssigned(TrainerUserConfigurable inTrainertoTest, TrainerUserConfigurable[] ArraytoCheckforComConflicts)
  //      {
  //          int CommPortToTest = inTrainertoTest.SavedSerialPortNum;
  //          if (CommPortToTest < 0) return true; // send back on 0 so this case is benign
  //          for (int i = 0; i < RM1Constants.maxtrainersGeneral; i++)
  //          {
  //              if (ArraytoCheckforComConflicts[i] != null && ArraytoCheckforComConflicts[i].SavedSerialPortNum == CommPortToTest)
  //                  return true;
  //          }
  //          return false;
            
  //      }

		/// <summary>
		/// Resets all the trainers to unknown, Makes sure there is at least 8 entries.
		/// </summary>
		public static void ResetSavedTrainers()
		{
			foreach (TrainerUserConfigurable tc in SavedTrainersList)
			{
				tc.PreviouslyDiscovered = 0;
				tc.RememberedDeviceType = "Unknown";
				tc.SavedPortName = string.Empty;
			}
			while( SavedTrainersList.Count < RM1Constants.maxtrainersGeneral)
				SavedTrainersList.Add( new TrainerUserConfigurable(SavedTrainersList.Count) );
		}
		public static bool NoTrainersAssigned()
		{
			foreach (TrainerUserConfigurable tc in SavedTrainersList)
			{
				if (tc.PreviouslyDiscovered != 0 && tc.RememberedDeviceType != "Unknown")
					return false;
			}
			return true;
		}

        //
        // uses the fixed course directory installed relative to application and copies to the default  courses folder in Documents.
        //
        public static void CopyDefaultCoursesToLocal()
        {
            String SourceDirectory = RacerMatePaths.CommonCoursesFullPath;
            String DestinationDirectory = RacerMatePaths.CoursesFullPath;
            
            foreach (string dirPath in Directory.GetDirectories(SourceDirectory, "*", SearchOption.AllDirectories))
                 if (!Directory.Exists(dirPath.Replace(SourceDirectory, DestinationDirectory)))
                    Directory.CreateDirectory(dirPath.Replace(SourceDirectory, DestinationDirectory));
                
            foreach (string newPath in Directory.GetFiles(SourceDirectory, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourceDirectory, DestinationDirectory), true);
        }

    }

    
    public static class RM1Constants
    {
        public const int maxtrainersGeneral = 8;
        public static Color RiderClothingColor0Blue = Color.FromRgb(0, 0, 255);
        public static Color RiderClothingColor1Green = Color.FromRgb(0, 255, 0);
        public static Color RiderClothingColor2Pink = Color.FromRgb(255, 0, 255);
        public static Color RiderClothingColor3Red = Color.FromRgb(255, 0, 0);
        public static Color RiderClothingColor4Yellow = Color.FromRgb(255, 255, 0);

        public static Color RiderSkinColor0 = Color.FromRgb(245, 217, 190);
        public static Color RiderSkinColor1 = Color.FromRgb(244, 188, 165);
        public static Color RiderSkinColor2 = Color.FromRgb(200, 156, 127);
        public static Color RiderSkinColor3 = Color.FromRgb(159, 144, 81);
        public static Color RiderSkinColor4 = Color.FromRgb(100, 64, 56);
        public static Color RiderSkinColor5 = Color.FromRgb(230, 196, 127);
        public const string NoRiderKey = "00000000-0000-0000-0000-000000000000";
        public const string DefaultStudioLogoFilename = @"/RacerMateOne;component/Resources/ZRes_LogoStudio.png";
    }

    public class GeneralSetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<KeyStringPair> selections;
        private KeyStringPair _usersetting;
        public KeyStringPair UserSetting
        {
            get
            {
                return _usersetting;
            }
            set
            {
                _usersetting = value;
                OnPropertyChanged("UserSetting");
            }
        }
        public string MyXmlTag
        { get; set; }
        public int Default
        { get; set; }
        //     public int FeatureEnabled
        //     { get; set; }
        public string DisplayString
        { get; set; }
        public List<KeyStringPair> Selections  //Use Sorted dictionary because nothing says XML will appear in order
        //{get; set;}
        {
            get { return selections; }
            set
            {
                selections = value;
                //because we cannot rely on the XML to be present in order of setting#, we must sort the list on the keys
                selections.Sort(CompareSelections);
            }
        }
        private int CompareSelections(KeyStringPair x, KeyStringPair y)
        {
            if (x.ID < y.ID)
            { return -1; }
            else if (x.ID == y.ID)
            { return 0; }
            else
            { return 1; }

        }


        public virtual XElement GetAsXElement()
        {
            XElement Selections = new XElement("Selections");

            foreach (KeyStringPair aaa in this.Selections)
            {
                XElement thisselection = new XElement("Selection", new XAttribute("Index", aaa.ID), new XAttribute("DisplayString", aaa.Name));
                Selections.Add(thisselection);
            }

            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("UserSetting", this.UserSetting.ID), new XAttribute("Default", this.Default),
                 new XAttribute("DisplayString", this.DisplayString), Selections);

            return retElement;
        }
        /// <summary>
        /// returns the short Xelement without the selections list, setting 
        /// </summary>
        /// <returns></returns>
        public virtual XElement GetAsShortXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("UserSetting", this.UserSetting.ID), new XAttribute("Default", this.Default),
              new XAttribute("DisplayString", this.DisplayString));

            return retElement;
        }

        public virtual void LoadFromXElement(XElement inElement)
        {

            XElement SelectionsNode = inElement.Element("Selections");
            List<KeyStringPair> SelectionStrings = new List<KeyStringPair>();
            SelectionStrings.Clear();
            IEnumerable<XElement> ASelectionsList = SelectionsNode.Elements("Selection");
            this.Default = Convert.ToInt32(inElement.Attribute("Default").Value);
            //     this.FeatureEnabled = Convert.ToInt32(inElement.Attribute("FeatureEnabled").Value);
            this.DisplayString = inElement.Attribute("DisplayString").Value;
            this.MyXmlTag = inElement.Name.ToString();
            // Debug.WriteLine("the MyXMLTag is ---" + inElement.Name.ToString());
            //we cannot guarantee that the list will be presented in index order, so must store the indices with the string
            // and these indices, rather than strings, will likely be used by the engines
            foreach (XElement aaa in ASelectionsList)
            {
                KeyStringPair ele = new KeyStringPair(Convert.ToInt32(aaa.Attribute("Index").Value), (aaa.Attribute("DisplayString").Value));
                //KeyValuePair<Int32,String> ele = new KeyValuePair<Int32,String>(Convert.ToInt32(aaa.Attribute("Index").Value), (aaa.Attribute("DisplayString").Value));
                SelectionStrings.Add(ele);
            }
            this.Selections = SelectionStrings;
            this.UserSetting = FindSettingFromIndex(Convert.ToInt32(inElement.Attribute("UserSetting").Value));
    
        }
        //returns first element if not found
        private KeyStringPair FindSettingFromIndex(int inputindex)
        {
            foreach (KeyStringPair aa in this.Selections)
            {
                if (aa.ID == inputindex)
                {
                    return aa;
                }
            }
            return this.Selections[0];
        }
        
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }
  
    
    public class GeneralPacerSetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<KeyStringPair> selections;
        private KeyStringPair _usersetting;
        public KeyStringPair UserSetting
        {
            get
            {
                //return _usersetting;
                return _usersetting;
            }

            set
            {
                _usersetting = value;
                OnPropertyChanged("UserSetting");
            }
        }
        private int CompareSelections(KeyStringPair x, KeyStringPair y)
        {
            if (x.ID < y.ID)
            { return -1; }
            else if (x.ID == y.ID)
            { return 0; }
            else
            { return 1; }

        }
        public string MyXmlTag
        { get; set; }
        public int Default
        { get; set; }
        public int FeatureEnabled
        { get; set; }
        public string DisplayString
        { get; set; }
        public List<KeyStringPair> Selections  //Use Sorted dictionary because nothing says XML will appear in order
        //{get; set;}
        {
            get { return selections; }
            set
            {
                selections = value;
                selections.Sort(CompareSelections);
            }
        }
        private KeyStringPair FindSettingFromIndex(int inputindex)
        {
            foreach (KeyStringPair aa in this.Selections)
            {
                if (aa.ID == inputindex)
                {
                    return aa;
                }
            }
            return this.Selections[0];
        }

        public virtual XElement GetAsXElement()
        {
            XElement Selections = new XElement("Selections");

            foreach (KeyStringPair aaa in this.Selections)
            {
                XElement thisselection = new XElement("Selection", new XAttribute("Index", aaa.ID), new XAttribute("DisplayString", aaa.Name));
                Selections.Add(thisselection);
            }

            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("UserSetting", this.UserSetting.ID), new XAttribute("Default", this.Default),
                new XAttribute("FeatureEnabled", this.FeatureEnabled), new XAttribute("DisplayString", this.DisplayString), Selections);

            return retElement;
        }
        /// <summary>
        /// returns the short Xelement without the selections list, setting 
        /// </summary>
        /// <returns></returns>
        public virtual XElement GetAsShortXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("UserSetting", this.UserSetting.ID), new XAttribute("Default", this.Default),
              new XAttribute("FeatureEnabled", this.FeatureEnabled), new XAttribute("DisplayString", this.DisplayString));

            return retElement;
        }

        public virtual void LoadFromXElement(XElement inElement)
        {

            XElement SelectionsNode = inElement.Element("Selections");
            List<KeyStringPair> SelectionStrings = new List<KeyStringPair>();
            SelectionStrings.Clear();
            IEnumerable<XElement> ASelectionsList = SelectionsNode.Elements("Selection");
            this.Default = Convert.ToInt32(inElement.Attribute("Default").Value);
            this.FeatureEnabled = Convert.ToInt32(inElement.Attribute("FeatureEnabled").Value);
            this.DisplayString = inElement.Attribute("DisplayString").Value;
            this.MyXmlTag = inElement.Name.ToString();
            foreach (XElement aaa in ASelectionsList)
            {
                KeyStringPair ele = new KeyStringPair(Convert.ToInt32(aaa.Attribute("Index").Value), (aaa.Attribute("DisplayString").Value));
                //KeyValuePair<Int32,String> ele = new KeyValuePair<Int32,String>(Convert.ToInt32(aaa.Attribute("Index").Value), (aaa.Attribute("DisplayString").Value));
                SelectionStrings.Add(ele);
            }
            this.Selections = SelectionStrings;
            this.UserSetting = FindSettingFromIndex(Convert.ToInt32(inElement.Attribute("UserSetting").Value));
            Debug.WriteLine(this.MyXmlTag + " usersetting is " + this.UserSetting.Name);
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }
    
    public class GeneralSettingNoSelections : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _usersetting;
        public int UserSetting
        {
            get
            {
                return _usersetting;
            }

            set
            {
                _usersetting = Math.Max(0, value);
                OnPropertyChanged("UserSetting");
            }
        }
        public string MyXmlTag
        { get; set; }
        public int Default
        { get; set; }
        //     public int FeatureEnabled
        //     { get; set; }
        public string DisplayString
        { get; set; }
 
        /// <summary>
        /// Ensures the Selections are sorted on the KeyValue in ascending order
        /// </summary>
   
        public virtual XElement GetAsXElement()
        {
           // XElement Selections = new XElement("Selections");

         
            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("UserSetting", this.UserSetting), new XAttribute("Default", this.Default),
                 new XAttribute("DisplayString", this.DisplayString));

            return retElement;
        }
        /// <summary>
        /// returns the short Xelement without the selections list, setting 
        /// </summary>
        /// <returns></returns>
        public virtual XElement GetAsShortXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("UserSetting", this.UserSetting), new XAttribute("Default", this.Default),
              new XAttribute("DisplayString", this.DisplayString));

            return retElement;
        }

        public virtual void LoadFromXElement(XElement inElement)
        {

            this.UserSetting = Convert.ToInt32(inElement.Attribute("UserSetting").Value);
            this.Default = Convert.ToInt32(inElement.Attribute("Default").Value);
            //     this.FeatureEnabled = Convert.ToInt32(inElement.Attribute("FeatureEnabled").Value);
            this.DisplayString = inElement.Attribute("DisplayString").Value;
            this.MyXmlTag = inElement.Name.ToString();
            // Debug.WriteLine("the MyXMLTag is ---" + inElement.Name.ToString());
            //we cannot guarantee that the list will be presented in index order, so must store the indices with the string
            // and these indices, rather than strings, will likely be used by the engines
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }
    
     
    public class GeneralFloatSetting
    {
        private float _usersetting;
        public float UserSetting
        {
            get
            {
                return _usersetting;
            }

            set
            {
                _usersetting = Math.Max(0, value);
                _usersetting = (float)Math.Round(_usersetting, 1); //we will carry only 1 decimal place

            }
        }
        public string LabelToShow
        { get; set; }
        public string MyXmlTag
        { get; set; }
        public float Default
        { get; set; }
  //      public int FeatureEnabled
  //      { get; set; }
        public string DisplayString
        { get; set; }

        public XElement GetAsXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("UserSetting", this.UserSetting), new XAttribute("Default", this.Default),
                  new XAttribute("DisplayString", this.DisplayString), new XElement("LabelToShow", this.LabelToShow));

            return retElement;
        }
        public void LoadFromXElement(XElement inElement)
        {
            this.UserSetting = Convert.ToSingle(inElement.Attribute("UserSetting").Value);
            this.Default = Convert.ToSingle(inElement.Attribute("Default").Value);
        //    this.FeatureEnabled = Convert.ToInt32(inElement.Attribute("FeatureEnabled").Value);
            this.DisplayString = inElement.Attribute("DisplayString").Value;
            this.LabelToShow = (string)inElement.Element("LabelToShow").Value;
            this.MyXmlTag = inElement.Name.ToString();
        }
    }
   
    public class GeneralPacerFloatSetting
    {
        private float _usersetting;
        public float UserSetting
        {
            get
            {
                return _usersetting;
            }

            set
            {
                _usersetting = Math.Max(0, value);
                _usersetting = (float)Math.Round(_usersetting, 1); //we will carry only 1 decimal place

            }
        }
        public string LabelToShow
        { get; set; }
        public string MyXmlTag
        { get; set; }
        public float Default
        { get; set; }
        public int FeatureEnabled
        { get; set; }
        public string DisplayString
        { get; set; }

        public XElement GetAsXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("UserSetting", this.UserSetting), new XAttribute("Default", this.Default),
                  new XAttribute("FeatureEnabled", this.FeatureEnabled), new XAttribute("DisplayString", this.DisplayString), new XElement("LabelToShow", this.LabelToShow));

            return retElement;
        }
        public void LoadFromXElement(XElement inElement)
        {
            this.UserSetting = Convert.ToSingle(inElement.Attribute("UserSetting").Value);
            this.Default = Convert.ToSingle(inElement.Attribute("Default").Value);
            this.FeatureEnabled = Convert.ToInt32(inElement.Attribute("FeatureEnabled").Value);
            this.DisplayString = inElement.Attribute("DisplayString").Value;
            this.LabelToShow = (string)inElement.Element("LabelToShow").Value;
            this.MyXmlTag = inElement.Name.ToString();
        }
    }

    public class GeneralIntegerSetting
    {
        private int _usersetting;
        public int UserSetting
        {
            get
            {
                return _usersetting;
            }

            set
            {
                _usersetting = Math.Max(0, value);
            }
        }
        public string LabelToShow
        { get; set; }
        public string MyXmlTag
        { get; set; }
        public int Default
        { get; set; }
 //       public int FeatureEnabled
  //      { get; set; }
        public string DisplayString
        { get; set; }
        public XElement GetAsXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("UserSetting", this.UserSetting), new XAttribute("Default", this.Default),
              new XAttribute("DisplayString", this.DisplayString),new XElement("LabelToShow",this.LabelToShow));

            return retElement;
        }
        public void LoadFromXElement(XElement inElement)
        {
            this.UserSetting = Convert.ToInt32(inElement.Attribute("UserSetting").Value);
            this.Default = Convert.ToInt32(inElement.Attribute("Default").Value);
        //    this.FeatureEnabled = Convert.ToInt32(inElement.Attribute("FeatureEnabled").Value);
            this.DisplayString = inElement.Attribute("DisplayString").Value;
            this.LabelToShow = inElement.Element("LabelToShow").Value;
            this.MyXmlTag = inElement.Name.ToString();
        }
    }
    
    public class ScaleSetting
    {
        private int _usersettingMin;
        public int UserSettingMin
        {
            get
            {
                return _usersettingMin;
            }

            set
            {
                _usersettingMin = Math.Max(0, value);
                _usersettingMin = Math.Min(_usersettingMin, _usersettingMax); //ensure Min is never > Max  
            }
        }
        private int _usersettingMax;
        public int UserSettingMax
        {
            get
            {
                return _usersettingMax;
            }

            set
            {
                _usersettingMax = Math.Max(0, value);
                _usersettingMax = Math.Max(_usersettingMin, _usersettingMax); //ensure Max is never < Min.
            }
        }
        
        public string MyXmlTag
        { get; set; }
        public int DefaultMin
        { get; set; }
        public int DefaultMax
        { get; set; }
     
       public XElement GetAsXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("UserSettingMin", this.UserSettingMin), new XAttribute("DefaultMin", this.DefaultMin),
                new XAttribute("UserSettingMax", this.UserSettingMax), new XAttribute("DefaultMax", this.DefaultMax));

            return retElement;
        }

        public void LoadFromXElement(XElement inElement)
        {
            this.UserSettingMin = Convert.ToInt32(inElement.Attribute("UserSettingMin").Value);
            this.DefaultMin = Convert.ToInt32(inElement.Attribute("DefaultMin").Value);
            this.UserSettingMax = Convert.ToInt32(inElement.Attribute("UserSettingMax").Value);
            this.DefaultMax = Convert.ToInt32(inElement.Attribute("DefaultMax").Value);
            this.MyXmlTag = inElement.Name.ToString();
        }

    }

    public class KeyStringPair
    {
        public KeyStringPair(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
        public int ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// ToString from base class is overridden so that the rendering of the bound object is not a string describing Typeof(object)
        /// which is the default characteristic.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name.ToString();
        }
    }

    public class GeneralFilePath : INotifyPropertyChanged
    {
        private string _settingsdirstring = ""; //init to no specific app
        public event PropertyChangedEventHandler PropertyChanged;

        public string MyXmlTag
        { get; set; }

        public string FullPath
        {
            get { return _settingsdirstring; }

            set
            {
                if (value != null || value != "")
                {
                    if (!File.Exists(value)) _settingsdirstring = ""; //yes we will allow the null string
                    else _settingsdirstring = value;
                }
                else
                { _settingsdirstring = value; }
                OnPropertyChanged("FullPath");
            }
        }
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public XElement GetAsXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, this.FullPath);

            return retElement;
        }
        public void LoadFromXElement(XElement inElement)
        {
            this.FullPath = inElement.Value;
            this.MyXmlTag = inElement.Name.ToString();
        }
    }

    public class GeneralFolderPath : INotifyPropertyChanged
    {
        private string _settingsdirstring = ""; //init to no specific path
        public event PropertyChangedEventHandler PropertyChanged;
        public string MyXmlTag
        { get; set; }

        public string FullPath
        {
            get { return _settingsdirstring; }

            set
            {
                if (!Directory.Exists(value)) _settingsdirstring = ""; //yes we will allow the null string

                else _settingsdirstring = value;
                OnPropertyChanged("FullPath");
            }
        }
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public XElement GetAsXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, this.FullPath);

            return retElement;
        }
        public void LoadFromXElement(XElement inElement)
        {
            this.FullPath = inElement.Value;
            this.MyXmlTag = inElement.Name.ToString();
        }

    }

    /// <summary>
    /// A graphic File path. this is separate from general path class since there may need to be file format checking in this class
    /// </summary>
    public class GraphicFilePath : INotifyPropertyChanged
    {
        private string _source = RM1Constants.DefaultStudioLogoFilename; //this is the default and note it is not necesarily in file system
        public event PropertyChangedEventHandler PropertyChanged;

        public string MyXmlTag
        { get; set; }

        public string RelativePath
        {
            get { return _source; }
            //TODO: some sort of checking on the input format to be either a pointer into resources or a plain vanilla file. Do a check on existance??
            set
            {
                //check this can be a valid image
                try
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri("file:///" + value.Replace("\\", "/"));
                    bmp.EndInit();

                    _source = value;
                }
                catch
                {
                    _source = RM1Constants.DefaultStudioLogoFilename;
                }
                OnPropertyChanged("RelativePath");
            }
        }
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public XElement GetAsXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, this.RelativePath);

            return retElement;
        }
        public void LoadFromXElement(XElement inElement)
        {
            this.RelativePath = inElement.Value;
            this.MyXmlTag = inElement.Name.ToString();
        }

    }
    public class PacersDisp
    {
        private List<KeyStringPair> selections;
        private Object _usersetting;
        public string MyXmlTag
        { get; set; }
        public string LabelToShow
        { get; set; }
        public Object UserSetting
        {
            get
            {
                    return _usersetting;
            }

            set
            {
                if (value.GetType() == typeof(KeyStringPair))
                    _usersetting = value;
                else
                    _usersetting = Convert.ToDecimal(value);

            }
        }
        public float Default
        { get; set; }
      //  public int FeatureEnabled
     //   { get; set; }
        public string DisplayString
        { get; set; }
        public List<KeyStringPair> Selections  //Use Sorted dictionary because nothing says XML will appear in order
        //{get; set;}
        {
            get { return selections; }
            set
            {
                selections = value; //presume sorted by the original object
             }
        }


        public override string ToString()
        {
            return DisplayString;
        }
        public PacersDisp MakeCopy()  {
            PacersDisp copyofme = (PacersDisp) this.MemberwiseClone(); // new PacersDisp();
            return copyofme;
        }

    }
    /// <summary>
    /// Encapsulates an assigned pacer and is useful for spinning out XML to teh Ride_Input.xml
    /// </summary>
    public class AssignedPacer
    {
        public int PositionIndex
        { get; set; }
        public PacersDisp PacerInfo
        { get; set; }
        public AssignedPacer(int index, PacersDisp inpacer)
        {
            PositionIndex = index;
            PacerInfo = inpacer;
        }
        public XElement GetAsXElement()
        {


            if (this.PacerInfo.UserSetting.GetType() == typeof(KeyStringPair))
            {

                KeyStringPair aa = (KeyStringPair)this.PacerInfo.UserSetting;
                return new XElement("Pacer", new XAttribute("Index", this.PositionIndex), new XAttribute("Type", this.PacerInfo.MyXmlTag), new XAttribute("RiderRefIndex", this.PositionIndex - 4),
                 aa.ID);
                   }
            else
            {
                 Decimal  bb = (Decimal)this.PacerInfo.UserSetting;
                return new XElement("Pacer", new XAttribute("Index", this.PositionIndex), new XAttribute("Type", this.PacerInfo.MyXmlTag), new XAttribute("RiderRefIndex", this.PositionIndex - 4),
                               bb.ToString("####.0"));
              }
        }

    }
    public class AssignedRider
    {
        public int PositionIndex
        { get; set; }
        public Rider RiderInfo
        { get; set; }
        public AssignedRider(int index, Rider inrider)
        {
            PositionIndex = index;
            RiderInfo = inrider;
        }
    }
    
    
    /// <summary>
    /// THe Opmode contains all Properties Unique to an Operating Mode.
    /// The CurrentOpMode property set will update several readonly properties
    /// </summary>
    public class OpMode
    {
        private string pvsubtitle;
        private string pvconstraints;
        private BitmapImage pvicon;
        private int pvcoursetypemask;
        private int pvmaxnumtrainers;
        private int pvmaxnumpacers;
        private bool pvsharedridertrainerslot;
        private bool pvsupportssavedsperformancepacer;
        private SupportedModes pvcurrentopmode;
        private string pvdisplaystring;
        private string pvmyxmltag;
        private bool pvIsTeamEvent;

        public string Subtitle
        { get { return pvsubtitle; } }
        public string Constraints
        { get { return pvconstraints; } }
        public BitmapImage Icon
        { get { return pvicon; } }
        public int CourseTypeMask
        { get { return pvcoursetypemask; } }
        public int MaxNumTrainers
        { get { return pvmaxnumtrainers; } }
        public int MaxNumPacers
        { get { return pvmaxnumpacers; } }
        public bool SharedRiderTrainerSlot
        { get { return pvsharedridertrainerslot; } }
        public bool SupportsSavedPerformancePacer
        { get { return pvsupportssavedsperformancepacer; } }
        public string DisplayString
        {
            get { return pvdisplaystring; }
            set { pvdisplaystring = value; }
        }
        public string MyXmlTag
        {
            get { return pvmyxmltag; }
            set { pvmyxmltag = value; }
        }
        public bool IsTeamEvent
        {
            get { return pvIsTeamEvent; }
            set { pvIsTeamEvent = value; }
        }


        public SupportedModes CurrentOpMode
        {
            get { return pvcurrentopmode; }
            set
            {
                pvcurrentopmode = value;
                switch ((int)value)
                {
                case (int)SupportedModes.ModeRCV: // Interactive Real Course Videos
                    {
                        pvsubtitle = "Train on actual race courses enhanced with the realism of race-day video!";
                        pvconstraints = "Riders: 1-8 riders";
                        pvicon = new BitmapImage(new Uri("/RacerMateOne;component/Resources/ZRes_ModeRCV.png", UriKind.Relative));
                        pvcoursetypemask = (int)CourseFileType.CourseIRCV | (int)CourseFileType.CourseRM1Performance;
                        pvmaxnumtrainers = 8;
                        pvmaxnumpacers = 4;
                        pvsharedridertrainerslot = false;
                        pvsupportssavedsperformancepacer = true;
                        pvdisplaystring = "Real Course Video Ride";
                        pvmyxmltag = "ModeRCV";
                        break;
                    }
                case (int)SupportedModes.ModeErgVideo : // ErgVideo™ Courses
                    {
                        pvsubtitle = "RacerMate, partnering with ErgVideo, brings you ergometer-mode group rides";
                        pvconstraints = "Riders: 1-8 riders";
                        pvicon = new BitmapImage(new Uri("/RacerMateOne;component/Resources/ZRes_ModeErgVideo.png", UriKind.Relative));
                        pvcoursetypemask = (int)CourseFileType.CourseErgVideo;
                        pvmaxnumtrainers = 8;
                        pvmaxnumpacers = 0;
                        pvsharedridertrainerslot = false;
                        pvsupportssavedsperformancepacer = false;
                        pvdisplaystring = "ErgVideo Ride";
                        pvmyxmltag = "ModeErgVideo"; 
                        break;
                    }
                case (int)SupportedModes.ModeErgVideoTest: // ErgVideo™ Courses
                    {
                        pvsubtitle = "RacerMate, partnering with ErgVideo, brings you ergometer-mode group rides";
                        pvconstraints = "Riders: 1-8 riders";
                        pvicon = new BitmapImage(new Uri("/RacerMateOne;component/Resources/ZRes_ModeErgVideo.png", UriKind.Relative));
                        pvcoursetypemask = (int)CourseFileType.CourseErgVideo;
                        pvmaxnumtrainers = 8;
                        pvmaxnumpacers = 0;
                        pvsharedridertrainerslot = false;
                        pvsupportssavedsperformancepacer = false;
                        pvdisplaystring = "ErgVideo Performance Test";
                        pvmyxmltag = "ModeErgVideoTest";
                        break;
                    }
                case (int)SupportedModes.Mode3DRoadRacing: // Limitless 3D Courses
                    {
                        pvsubtitle = "Train on actual race courses created on-the-fly using unique 3D worlds from different locals";
                        pvconstraints = "Riders: Solo, Dual; or Solo + a pacer";
                        pvicon = new BitmapImage(new Uri("/RacerMateOne;component/Resources/ZRes_Mode3D.png", UriKind.Relative));
                        pvcoursetypemask = (int)CourseFileType.Course3d | (int)CourseFileType.CourseCrs | (int)CourseFileType.CourseRM1Performance;
                        pvmaxnumtrainers = 2;
                        pvmaxnumpacers = 1;
                        pvsharedridertrainerslot = true;
                        pvsupportssavedsperformancepacer = true;
                        pvdisplaystring = "3D Road Racing";
                        pvmyxmltag = "Mode3DRoadRacing";
                        break;
                    }
                case (int)SupportedModes.ModeGroupRacing : // MultiRider™ -- Individual Time Trial 
                    {
                        pvsubtitle = "Pit yourself against the clock, or other riders (individual scoring)";
                        pvconstraints = "Riders: 1-8 riders";
                        pvicon = new BitmapImage(new Uri("/RacerMateOne;component/Resources/ZRes_ModeITT.png", UriKind.Relative));
                        pvcoursetypemask = (int)CourseFileType.Course3d | (int)CourseFileType.CourseCrs | (int)CourseFileType.CourseRM1Performance;
                        pvmaxnumtrainers = 8;
                        pvmaxnumpacers = 4;
                        pvsharedridertrainerslot = false;
                        pvsupportssavedsperformancepacer = true;
                        pvdisplaystring = "Group Racing";
                        pvmyxmltag = "ModeGroupRacing";
                        break;
                    }
                case (int)SupportedModes.ModeTeamTT: // MultiRider™ -- Team Time Trial 
                    {
                        pvsubtitle = "Teams working together against another team";
                        pvconstraints = "Riders: 2-8 riders, select red team vs green team";
                        pvicon = new BitmapImage(new Uri("/RacerMateOne;component/Resources/ZRes_ModeITT.png", UriKind.Relative));
                        pvcoursetypemask = (int)CourseFileType.Course3d | (int)CourseFileType.CourseCrs;
                        pvmaxnumtrainers = 8;
                        pvmaxnumpacers = 0;
                        pvsharedridertrainerslot = false;
                        pvsupportssavedsperformancepacer = false;
                        pvdisplaystring = "Team Time Trial";
                        pvmyxmltag = "ModeTeamTT";
                        break;
                    }
                case (int)SupportedModes.ModeWattTesting: // Coaching/Ergometer Training
                    {
                        pvsubtitle = "Train on specialized Time/Watt; Time/Grade; Distance/Watt; Distance/Grade courses";
                        pvconstraints = "Riders: 1-8";
                        pvicon = new BitmapImage(new Uri("/RacerMateOne;component/Resources/ZRes_ModeCoach.png", UriKind.Relative));
                        pvcoursetypemask = (int)CourseFileType.CourseErg | (int)CourseFileType.CourseMrc;
                        pvmaxnumtrainers = 8;
                        pvmaxnumpacers = 0;
                        pvsharedridertrainerslot = false;
                        pvsupportssavedsperformancepacer = false;
                        pvdisplaystring = "Watt Testing";
                        pvmyxmltag = "ModeWattTesting";
                        break;
                    }
                case (int)SupportedModes.ModeSpinScanTraining: // SpinScan™
                    {
                        pvsubtitle = "Full screen, full on Spinscan with all the data you want";
                        pvconstraints = "Riders: Solo";
                        pvicon = new BitmapImage(new Uri("/RacerMateOne;component/Resources/ZRes_ModeSpinscan.png", UriKind.Relative));
                        pvcoursetypemask = (int)CourseFileType.None;
                        pvmaxnumtrainers = 1;
                        pvmaxnumpacers = 0;
                        pvsharedridertrainerslot = false;
                        pvsupportssavedsperformancepacer = false;
                        pvdisplaystring = "SpinScan Training";
                        pvmyxmltag = "ModeSpinScanTraining";
                        break;
                     }
                case (int)SupportedModes.ModePreviousPerformance: // Recall a previous performance
                    {
                        pvsubtitle = "Open a past performance and review the data";
                        pvconstraints = "Riders: None";
                        pvicon = new BitmapImage(new Uri("/RacerMateOne;component/Resources/ZRes_ModeRecall.png", UriKind.Relative));
                        pvcoursetypemask = (int)CourseFileType.CourseRM1Performance;
                        pvmaxnumtrainers = 0;
                        pvmaxnumpacers = 0;
                        pvsharedridertrainerslot = false;
                        pvsupportssavedsperformancepacer = false;
                        pvdisplaystring = "Previous Performance";
                        pvmyxmltag = "ModePreviousPerformance";
                        break;
                    }
                }
            }           
        }
        
      
        public OpMode(SupportedModes inmode)
        {
            CurrentOpMode = inmode; //this should launch the whole set
        }
        
        public virtual XElement GetAsXElement()
        {
            XElement retElement = new XElement(this.MyXmlTag, new XAttribute("DisplayString", this.DisplayString));

            return retElement;
        }
        public virtual void LoadFromXElement(XElement inElement)
        {
            this.DisplayString = inElement.Attribute("DisplayString").Value;
            this.MyXmlTag = inElement.Name.ToString();
        }

        /// <summary>
        /// Forms the filter string for file dialog lookup
        /// </summary>
        /// <param name="coursemask"></param>
        /// <returns></returns>
        public string GetFileFilterString(int coursemask)
        {

            string FiltString1 = "Courses (";
            string FiltString2 = "|";
            if (((int)coursemask & (int)CourseFileType.CourseRM1Performance) != 0)
            {
                FiltString1 = @"Courses & Performances (";
            }

            if (((int)coursemask & (int)CourseFileType.Course3d) != 0)
            {
                FiltString1 = FiltString1 + "*.3dc,";
                FiltString2 = FiltString2 + "*.3dc;";
            }
            if (((int)coursemask & (int)CourseFileType.CourseCrs) != 0)
            {
                FiltString1 = FiltString1 + "*.crs,";
                FiltString2 = FiltString2 + "*.crs;";
            }
            if (((int)coursemask & (int)CourseFileType.CourseErg) != 0)
            {
                FiltString1 = FiltString1 + "*.erg,";
                FiltString2 = FiltString2 + "*.erg;";
            }

            if (((int)coursemask & (int)CourseFileType.CourseIRCV) != 0)
            {
                FiltString1 = FiltString1 + "*.ircv,";
                FiltString2 = FiltString2 + "*.ircv;";
            }
            if (((int)coursemask & (int)CourseFileType.CourseMrc) != 0)
            {
                FiltString1 = FiltString1 + "*.mrc,";
                FiltString2 = FiltString2 + "*.mrc;";
            }
            if (((int)coursemask & (int)CourseFileType.CourseErgVideo) != 0)
            {
                FiltString1 = FiltString1 + "*.evr,";
                FiltString2 = FiltString2 + "*.evr;";
            }
            if (((int)coursemask & (int)CourseFileType.CourseRM1Performance) != 0)
            {
                FiltString1 = FiltString1 + "*.xml,";
                FiltString2 = FiltString2 + "*.xml;";
            }
            
            FiltString1 = FiltString1.Remove(FiltString1.LastIndexOf(","), 1);
            FiltString1 = FiltString1 + ")";
            FiltString2 = FiltString2.Remove(FiltString2.LastIndexOf(";"), 1);

            string FiltString = FiltString1 + FiltString2;
            return FiltString;
        }
       
    }
    public enum CourseFileType : int
    {
        None = 0,
        CourseIRCV = 1,
        Course3d = 2,
        CourseCrs = 4,
        CourseErg = 8,
        CourseMrc = 16,
        CourseErgVideo = 32,
        CourseRM1Performance = 64,
    };

  
    public enum SupportedModes
    {
        ModeRCV,
        ModeErgVideo,
        ModeErgVideoTest,
        Mode3DRoadRacing,
        ModeGroupRacing,
        ModeTeamTT,
        ModeWattTesting,
        ModeSpinScanTraining,
        ModePreviousPerformance,
    };




}
