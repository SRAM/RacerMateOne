using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;

using System.ComponentModel;



namespace RacerMateOne
{
	public class ReportColumns:  INotifyPropertyChanged
	{

		#pragma warning disable 649
		public string Title;
		public string Key;
		public override string ToString() { return Title; }

		public enum Types
		{
			ShowTemplate,
			Display,
			Report
		};
		public Types Type;

		public bool UserDefined { get; protected set; }
		public ReportColumns Original;

		public ReportColumns _ShowTemplate;
		public ReportColumns ShowTemplate
		{
			get { return _ShowTemplate; }
			set
			{
				if (_ShowTemplate != null)
					_ShowTemplate.SubItemList.Remove(this);
				_ShowTemplate = value;
				if (_ShowTemplate != null && Title != null )
				{
					_ShowTemplate.SubItemList.Add(this);
				}
				OnPropertyChanged("ShowTemplate");
			}
		}
		private int ver = 1;

		// Speed
		public bool Speed { get { return _Speed; } set { ver++; _Speed = value; OnPropertyChanged("Speed"); } }
		private bool _Speed;
		public Visibility Speed_Show { get { return ShowTemplate._Speed ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Speed = value == Visibility.Visible; OnPropertyChanged("Speed_Show"); } }

		public bool Speed_Avg { get { return _Speed_Avg; } set { ver++; _Speed_Avg = value; OnPropertyChanged("Speed_Avg"); } }
		private bool _Speed_Avg;
		public Visibility Speed_Avg_Show { get { return ShowTemplate._Speed_Avg ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Speed_Avg = value == Visibility.Visible; OnPropertyChanged("Speed_Avg_Show"); } }

		public bool Speed_Max { get { return _Speed_Max; } set { ver++; _Speed_Max = value; OnPropertyChanged("Speed_Max"); } }
		private bool _Speed_Max;
		public Visibility Speed_Max_Show { get { return ShowTemplate._Speed_Max ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Speed_Max = value == Visibility.Visible; OnPropertyChanged("Speed_Max_Show"); } }


		public bool Watts { get { return _Watts; } set { ver++; _Watts = value; OnPropertyChanged("Watts"); } }
		private bool _Watts;
		public Visibility Watts_Show { get { return ShowTemplate._Watts ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Watts = value == Visibility.Visible; OnPropertyChanged("Watts_Show"); } }

		public bool Watts_Avg { get { return _Watts_Avg; } set { ver++; _Watts_Avg = value; OnPropertyChanged("Watts_Avg"); } }
		private bool _Watts_Avg;
		public Visibility Watts_Avg_Show { get { return ShowTemplate._Watts_Avg ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Watts_Avg = value == Visibility.Visible; OnPropertyChanged("Watts_Avg_Show"); } }

		public bool Watts_Max { get { return _Watts_Max; } set { ver++; _Watts_Max = value; OnPropertyChanged("Watts_Max"); } }
		private bool _Watts_Max;
		public Visibility Watts_Max_Show { get { return ShowTemplate._Watts_Max ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Watts_Max = value == Visibility.Visible; OnPropertyChanged("Watts_Max_Show"); } }

		public bool Watts_Wkg { get { return _Watts_Wkg; } set { ver++; _Watts_Wkg = value; OnPropertyChanged("Watts_Wkg"); } }
		private bool _Watts_Wkg;
		public Visibility Watts_Wkg_Show { get { return ShowTemplate._Watts_Wkg ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Watts_Wkg = value == Visibility.Visible; OnPropertyChanged("Watts_Wkg_Show"); } }

		public bool Watts_Load { get { return _Watts_Load; } set { ver++; _Watts_Load = value; OnPropertyChanged("Watts_Load"); } }
		private bool _Watts_Load;
		public Visibility Watts_Load_Show { get { return ShowTemplate._Watts_Load ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Watts_Load = value == Visibility.Visible; OnPropertyChanged("Watts_Load_Show"); } }

		public bool HeartRate { get { return _HeartRate; } set { ver++; _HeartRate = value; OnPropertyChanged("HeartRate"); } }
		private bool _HeartRate;
		public Visibility HeartRate_Show { get { return ShowTemplate._HeartRate ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._HeartRate = value == Visibility.Visible; OnPropertyChanged("HeartRate_Show"); } }

		public bool HeartRate_Avg { get { return _HeartRate_Avg; } set { ver++; _HeartRate_Avg = value; OnPropertyChanged("HeartRate_Avg"); } }
		private bool _HeartRate_Avg;
		public Visibility HeartRate_Avg_Show { get { return ShowTemplate._HeartRate_Avg ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._HeartRate_Avg = value == Visibility.Visible; OnPropertyChanged("HeartRate_Avg_Show"); } }

		public bool HeartRate_Max { get { return _HeartRate_Max; } set { ver++; _HeartRate_Max = value; OnPropertyChanged("HeartRate_Max"); } }
		private bool _HeartRate_Max;
		public Visibility HeartRate_Max_Show { get { return ShowTemplate._HeartRate_Max ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._HeartRate_Max = value == Visibility.Visible; OnPropertyChanged("HeartRate_Max_Show"); } }


		public bool Cadence { get { return _Cadence; } set { ver++; _Cadence = value; OnPropertyChanged("Cadence"); } }
		private bool _Cadence;
		public Visibility Cadence_Show { get { return ShowTemplate._Cadence ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Cadence = value == Visibility.Visible; OnPropertyChanged("Cadence_Show"); } }

		public bool Cadence_Avg { get { return _Cadence_Avg; } set { ver++; _Cadence_Avg = value; OnPropertyChanged("Cadence_Avg"); } }
		private bool _Cadence_Avg;
		public Visibility Cadence_Avg_Show { get { return ShowTemplate._Cadence_Avg ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Cadence_Avg = value == Visibility.Visible; OnPropertyChanged("Cadence_Avg_Show"); } }

		public bool Cadence_Max { get { return _Cadence_Max; } set { ver++; _Cadence_Max = value; OnPropertyChanged("Cadence_Max"); } }
		private bool _Cadence_Max;
		public Visibility Cadence_Max_Show { get { return ShowTemplate._Cadence_Max ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Cadence_Max = value == Visibility.Visible; OnPropertyChanged("Cadence_Max_Show"); } }


		public bool Distance { get { return _Distance; } set { ver++; _Distance = value; OnPropertyChanged("Distance"); } }
		private bool _Distance;
		public Visibility Distance_Show { get { return ShowTemplate._Distance ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Distance = value == Visibility.Visible; OnPropertyChanged("Distance_Show"); } }

		public bool Lead { get { return _Lead; } set { ver++; _Lead = value; OnPropertyChanged("Lead"); } }
		private bool _Lead;
		public Visibility Lead_Show { get { return ShowTemplate._Lead ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Lead = value == Visibility.Visible; OnPropertyChanged("Lead_Show"); } }

		public bool Grade { get { return _Grade; } set { ver++; _Grade = value; OnPropertyChanged("Grade"); } }
		private bool _Grade;
		public Visibility Grade_Show { get { return ShowTemplate._Grade ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Grade = value == Visibility.Visible; OnPropertyChanged("Grade_Show"); } }

		public bool Wind { get { return _Wind; } set { ver++; _Wind = value; OnPropertyChanged("Wind"); } }
		private bool _Wind;
		public Visibility Wind_Show { get { return ShowTemplate._Wind ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Wind = value == Visibility.Visible; OnPropertyChanged("Wind_Show"); } }

		public bool Load { get { return _Load; } set { ver++; _Load = value; OnPropertyChanged("Load"); } }
		private bool _Load;
		public Visibility Load_Show { get { return ShowTemplate._Load ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Load = value == Visibility.Visible; OnPropertyChanged("Load_Show"); } }

		public bool PercentAT { get { return _PercentAT; } set { ver++; _PercentAT = value; OnPropertyChanged("PercentAT"); } }
		private bool _PercentAT;
		public Visibility PercentAT_Show { get { return ShowTemplate._PercentAT ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._PercentAT = value == Visibility.Visible; OnPropertyChanged("PercentAT_Show"); } }


		public bool Calories { get { return _Calories; } set { ver++; _Calories = value; OnPropertyChanged("Calories"); } }
		private bool _Calories;
		public Visibility Calories_Show { get { return ShowTemplate._Calories ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Calories = value == Visibility.Visible; OnPropertyChanged("Calories_Show"); } }

		public bool PulsePower { get { return _PulsePower; } set { ver++; _PulsePower = value; OnPropertyChanged("PulsePower"); } }
		private bool _PulsePower;
		public Visibility PulsePower_Show { get { return ShowTemplate._PulsePower ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._PulsePower = value == Visibility.Visible; OnPropertyChanged("PulsePower_Show"); } }

		public bool DragFactor { get { return _DragFactor; } set { ver++; _DragFactor = value; OnPropertyChanged("DragFactor"); } }
		private bool _DragFactor;
		public Visibility DragFactor_Show { get { return ShowTemplate._DragFactor ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._DragFactor = value == Visibility.Visible; OnPropertyChanged("DragFactor_Show"); } }

		public bool TSS_IF_NP { get { return _TSS_IF_NP; } set { ver++; _TSS_IF_NP = value; OnPropertyChanged("TSS_IF_NP"); } }
		private bool _TSS_IF_NP;
		public Visibility TSS_IF_NP_Show { get { return ShowTemplate._TSS_IF_NP ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._TSS_IF_NP = value == Visibility.Visible; OnPropertyChanged("TSS_IF_NP_Show"); } }

		public bool SpinScan { get { return _SpinScan; } set { ver++; _SpinScan = value; OnPropertyChanged("SpinScan"); } }
		private bool _SpinScan;
		public Visibility SpinScan_Show { get { return ShowTemplate._SpinScan ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._SpinScan = value == Visibility.Visible; OnPropertyChanged("SpinScan_Show"); } }

		public bool LeftSS { get { return _LeftSS; } set { ver++; _LeftSS = value; OnPropertyChanged("LeftSS"); } }
		private bool _LeftSS;
		public Visibility LeftSS_Show { get { return ShowTemplate._LeftSS ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._LeftSS = value == Visibility.Visible; OnPropertyChanged("LeftSS_Show"); } }

		public bool RightSS { get { return _RightSS; } set { ver++; _RightSS = value; OnPropertyChanged("RightSS"); } }
		private bool _RightSS;
		public Visibility RightSS_Show { get { return ShowTemplate._RightSS ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._RightSS = value == Visibility.Visible; OnPropertyChanged("RightSS_Show"); } }

		public bool LeftATA { get { return _LeftATA; } set { ver++; _LeftATA = value; OnPropertyChanged("LeftATA"); } }
		private bool _LeftATA;
		public Visibility LeftATA_Show { get { return ShowTemplate._LeftATA ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._LeftATA = value == Visibility.Visible; OnPropertyChanged("LeftATA_Show"); } }

		public bool RightATA { get { return _RightATA; } set { ver++; _RightATA = value; OnPropertyChanged("RightATA"); } }
		private bool _RightATA;
		public Visibility RightATA_Show { get { return ShowTemplate._RightATA ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._RightATA = value == Visibility.Visible; OnPropertyChanged("RightATA_Show"); } }

		public bool LeftPower { get { return _LeftPower; } set { ver++; _LeftPower = value; OnPropertyChanged("LeftPower"); } }
		private bool _LeftPower;
		public Visibility LeftPower_Show { get { return ShowTemplate._LeftPower ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._LeftPower = value == Visibility.Visible; OnPropertyChanged("LeftPower_Show"); } }

		public bool RightPower { get { return _RightPower; } set { ver++; _RightPower = value; OnPropertyChanged("RightPower"); } }
		private bool _RightPower;
		public Visibility RightPower_Show { get { return ShowTemplate._RightPower ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._RightPower = value == Visibility.Visible; OnPropertyChanged("RightPower_Show"); } }

		public bool Gear { get { return _Gear; } set { ver++; _Gear = value; OnPropertyChanged("Gear"); } }
		private bool _Gear;
		public Visibility Gear_Show { get { return ShowTemplate._Gear ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Gear = value == Visibility.Visible; OnPropertyChanged("Gear_Show"); } }

		public bool Gearing { get { return _Gearing; } set { ver++; _Gearing = value; OnPropertyChanged("Gearing"); } }
		private bool _Gearing;
		public Visibility Gearing_Show { get { return ShowTemplate._Gearing ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._Gearing = value == Visibility.Visible; OnPropertyChanged("Gearing_Show"); } }

		public bool RawSpinScan { get { return _RawSpinScan; } set { ver++; _RawSpinScan = value; OnPropertyChanged("RawSpinScan"); } }
		private bool _RawSpinScan;
		public Visibility RawSpinScan_Show { get { return ShowTemplate._RawSpinScan ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._RawSpinScan = value == Visibility.Visible; OnPropertyChanged("RawSpinScan_Show"); } }

		public bool CadenceTiming { get { return _CadenceTiming; } set { ver++; _CadenceTiming = value; OnPropertyChanged("CadenceTiming"); } }
		private bool _CadenceTiming;
		public Visibility CadenceTiming_Show { get { return ShowTemplate._CadenceTiming ? Visibility.Visible : Visibility.Collapsed; } set { ShowTemplate.ver++; ShowTemplate._CadenceTiming = value == Visibility.Visible; OnPropertyChanged("CadenceTiming_Show"); } }

		String _Selected;
		public String Selected { get { return _Selected; } set { _Selected = value; OnPropertyChanged("Selected"); } }

		public ReportColumns Selected_ReportColumns  {
			get  {
				ReportColumns rc;
				if (!m_DB.TryGetValue(_Selected, out rc))
					rc = SubItemList != null ? SubItemList.First() : null;
				return rc;
			}
		}


		public static ObservableCollection<ReportColumns> DisplayList = new ObservableCollection<ReportColumns>();
		public static ObservableCollection<ReportColumns> ReportList = new ObservableCollection<ReportColumns>();

		public static ObservableCollection<ReportColumns> DisplayTemplateList = new ObservableCollection<ReportColumns>();
		public static ObservableCollection<ReportColumns> ReportTemplateList = new ObservableCollection<ReportColumns>();


		protected static ReportColumns ms_r_3DRoadRacing;
		protected static ReportColumns ms_r_RCV;
		protected static ReportColumns ms_r_WattTestingERG;
		protected static ReportColumns ms_r_WattTestingAT;
		protected static ReportColumns ms_r_SpinScan;

		public static ReportColumns Report_3DRoadRacing { get { return ms_r_3DRoadRacing; } }
		public static ReportColumns Report_RCV { get { return ms_r_RCV; } }
		public static ReportColumns Report_WattTestingERG { get { return ms_r_WattTestingERG; } }
		public static ReportColumns Report_WattTestingAT { get { return ms_r_WattTestingAT; } }
		public static ReportColumns Report_SpinScan { get { return ms_r_SpinScan; } }

		protected static ReportColumns ms_d_3DRoadRacing;
		protected static ReportColumns ms_d_RCV;
		protected static ReportColumns ms_d_WattTestingERG;
		protected static ReportColumns ms_d_WattTestingAT;
		protected static ReportColumns ms_d_SpinScan;

		public static ReportColumns Display_3DRoadRacing { get { return ms_d_3DRoadRacing; } }
		public static ReportColumns Display_RCV { get { return ms_d_RCV; } }
		public static ReportColumns Display_WattTestingERG { get { return ms_d_WattTestingERG; } }
		public static ReportColumns Display_WattTestingAT { get { return ms_d_WattTestingAT; } }
		public static ReportColumns Display_SpinScan { get { return ms_d_SpinScan; } }


		public ObservableCollection<ReportColumns> _SubItemList = new ObservableCollection<ReportColumns>();
		public ObservableCollection<ReportColumns> SubItemList { get { return _SubItemList; } set { ver++; _SubItemList = value; OnPropertyChanged("SubItemList"); } }


		public static Dictionary<String, ReportColumns> m_DB = new Dictionary<string, ReportColumns>();

		public void Remove()
		{
			if (Original == null)
			{
				DisplayList.Remove(this);
				ReportList.Remove(this);
				m_DB.Remove(Key);
			}
		}

		static ReportColumns()
		{
			ReportColumns r;

			ms_d_3DRoadRacing = r = new ReportColumns("Display3D");
			r.Title = "3D Cycling";
			r.Type = Types.ShowTemplate;
			r._Speed = r._Speed_Avg = r._Speed_Max = true;
			r._Watts = r._Watts_Avg = r._Watts_Max = r._Watts_Wkg = true;
			r._HeartRate = r._HeartRate_Avg = r._HeartRate_Max = true;
			r._Cadence = r._Cadence_Avg = r._Cadence_Max = true;
			r._Distance = r._Lead = r._Grade = r._Wind = true;
			r._Calories = r._PulsePower = r._DragFactor = r._TSS_IF_NP = true;
			r._Gear = r._Gearing = true;
			DisplayTemplateList.Add(r);

			ms_d_RCV = r = new ReportColumns("DisplayRCV");
			r.Type = Types.ShowTemplate;
			r.Title = "Real Course Video";
			r._Speed = r._Speed_Avg = r._Speed_Max = true;
			r._Watts = r._Watts_Avg = r._Watts_Max = r._Watts_Wkg = true;
			r._HeartRate = r._HeartRate_Avg = r._HeartRate_Max = true;
			r._Cadence = r._Cadence_Avg = r._Cadence_Max = true;
			r._Distance = r._Grade = true;
			r._Calories = r._PulsePower = r._DragFactor = r._TSS_IF_NP = true;
			r._Gear = r._Gearing = true;
			DisplayTemplateList.Add(r);

			ms_d_WattTestingERG = r = new ReportColumns("DisplayERG");
			r.Type = Types.ShowTemplate;
			r.Title = "Power Training (ERG-based)";
			r._Speed = r._Speed_Avg = r._Speed_Max = true;
			r._Watts = r._Watts_Avg = r._Watts_Max = r._Watts_Wkg = true;
			r._HeartRate = r._HeartRate_Avg = r._HeartRate_Max = true;
			r._Cadence = r._Cadence_Avg = r._Cadence_Max = true;
			r._Distance = r._Load = true;
			r._Calories = r._PulsePower = r._DragFactor = r._TSS_IF_NP = true;
			r._Gear = r._Gearing = true;
			//DisplayTemplateList.Add(r);

			ms_d_WattTestingAT = r = new ReportColumns("DisplayWatt");
			r.Type = Types.ShowTemplate;
			r.Title = "Power Training (Grade-based)";
			r._Speed = r._Speed_Avg = r._Speed_Max = true;
			r._Watts = r._Watts_Avg = r._Watts_Max = r._Watts_Wkg = true;
			r._HeartRate = r._HeartRate_Avg = r._HeartRate_Max = true;
			r._Cadence = r._Cadence_Avg = r._Cadence_Max = true;
			r._Distance = r._Load = r._PercentAT = true;
			r._Calories = r._PulsePower = r._DragFactor = r._TSS_IF_NP = true;
			r._Gear = r._Gearing = true;
			//DisplayTemplateList.Add(r);

			ms_d_SpinScan = r = new ReportColumns("DisplaySpinScan");
			r.Type = Types.ShowTemplate;
			r.Title = "SpinScan";
			r._Speed = r._Speed_Avg = r._Speed_Max = true;
			r._Watts = r._Watts_Avg = r._Watts_Max = r._Watts_Wkg = true;
			r._HeartRate = r._HeartRate_Avg = r._HeartRate_Max = true;
			r._Cadence = r._Cadence_Avg = r._Cadence_Max = true;
			r._Distance = r._Grade = r._Wind = true;
			r._Calories = r._PulsePower = r._DragFactor = r._TSS_IF_NP = true;
			r._Gear = r._Gearing = true;
			DisplayTemplateList.Add(r);

			r = new ReportColumns("334D280F-1114-4050-AC0D-47CC3513A654");
			r.Type = ReportColumns.Types.Display;
			r.Title = "Defaults";
			r.ShowTemplate = ms_d_3DRoadRacing;
			ms_d_3DRoadRacing._Selected = r.Key;
			r._Speed = r._Watts = r._HeartRate = r._Cadence = r._Distance = r._Lead = r._Grade = r._Gearing = true;
			DisplayList.Add(r.Dup());

			r = new ReportColumns("5B1FC59E-E9EC-425d-8FBF-456500D5FB17");
			r.Type = ReportColumns.Types.Display;
			r.Title = "Defaults";
			r.ShowTemplate = ms_d_RCV;
			ms_d_RCV._Selected = r.Key;
			r._Speed = r._Watts = r._Grade = r._HeartRate = r._Cadence = r._Distance = r._Gearing = true;
			DisplayList.Add(r.Dup());

			r = new ReportColumns("EC944EB3-4266-4cee-8243-1FC02EABFD61");
			r.Type = ReportColumns.Types.Display;
			r.Title = "Defaults";
			r.ShowTemplate = ms_d_WattTestingERG;
			ms_d_WattTestingERG._Selected = r.Key;
			r._Speed = r._Watts = r._HeartRate = r._Cadence = r._Distance = r._Gearing = true;
			DisplayList.Add(r.Dup());

			r = new ReportColumns("1D35D453-0DD5-483f-960D-55C8CF4B24F1");
			r.Type = ReportColumns.Types.Display;
			r.Title = "Defaults";
			r.ShowTemplate = ms_d_WattTestingAT;
			ms_d_WattTestingAT._Selected = r.Key;
			r._Speed = r._Watts = r._HeartRate = r._Cadence = r._Distance = r._Load = r._PercentAT = r._Gearing = true;
			DisplayList.Add(r.Dup());

			r = new ReportColumns("B1411A60-2C80-4ed6-B4D8-9BBED9A4A3E1");
			r.Type = ReportColumns.Types.Display;
			r.Title = "Defaults";
			r.ShowTemplate = ms_d_SpinScan;
			ms_d_SpinScan._Selected = r.Key;
			r._Speed = r._Watts = r._HeartRate = r._Cadence = r._Distance = r._Grade = r._Gearing = true;
			DisplayList.Add(r.Dup());
			//============================================================================================================
			ms_r_3DRoadRacing = r = new ReportColumns("Report3D");
			r.Type = Types.ShowTemplate;
			r.Title = "3D Cycling";
			r._Speed = r._Speed_Avg = r._Speed_Max = true;
			r._Watts = r._Watts_Avg = r._Watts_Max = r.Watts_Wkg = true;
			r._HeartRate = r._HeartRate_Avg = r._HeartRate_Max = true;
			r._Cadence = r._Cadence_Avg = r._Cadence_Max = true;
			r._Distance = r._Grade = r._Wind = true;
			r._Calories = r._PulsePower = r._DragFactor = r._TSS_IF_NP = true;
			r.SpinScan = r._LeftSS = r._RightSS = r._LeftATA = r._RightATA = r._LeftPower = r._RightPower = true;
			r._Gear = r._Gearing = true;
			r._RawSpinScan = r._CadenceTiming = true;
			ReportTemplateList.Add(r);

			ms_r_RCV = r = new ReportColumns("ReportRCV");
			r.Type = Types.ShowTemplate;
			r.Title = "Real Course Video";
			r._Speed = r._Speed_Avg = r._Speed_Max = true;
			r._Watts = r._Watts_Avg = r._Watts_Max = r._Watts_Wkg = true;
			r._HeartRate = r._HeartRate_Avg = r._HeartRate_Max = true;
			r._Cadence = r._Cadence_Avg = r._Cadence_Max = true;
			r._Distance = r._Grade = true;
			r._Calories = r._PulsePower = r._DragFactor = r._TSS_IF_NP = true;
			r._SpinScan = r._LeftSS = r._RightSS = r._LeftATA = r._RightATA = true;
			r._Gear = r._Gearing = true;
			r._RawSpinScan = r._CadenceTiming = true;
			ReportTemplateList.Add(r);

			ms_r_WattTestingERG = r = new ReportColumns("ReportERG");
			r.Type = Types.ShowTemplate;
			r.Title = "Power Training (ERG-based)";
			r._Speed = r._Speed_Avg = r._Speed_Max = true;
			r._Watts = r._Watts_Avg = r._Watts_Max = r._Watts_Wkg = r._Watts_Load = true;
			r._HeartRate = r._HeartRate_Avg = r._HeartRate_Max = true;
			r._Cadence = r._Cadence_Avg = r._Cadence_Max = true;
			r._Distance = true;
			r._Calories = r._PulsePower = r._DragFactor = r._TSS_IF_NP = true;
			r._Gear = r._Gearing = true;
			ReportTemplateList.Add(r);

			ms_r_WattTestingAT = r = new ReportColumns("ReportAT");
			r.Type = Types.ShowTemplate;
			r.Title = "Power Training (Grade-based)";
			r._Speed = r._Speed_Avg = r._Speed_Max = true;
			r._Watts = r._Watts_Avg = r._Watts_Max = r._Watts_Wkg = r._Watts_Load = true;
			r._HeartRate = r._HeartRate_Avg = r._HeartRate_Max = true;
			r._Cadence = r._Cadence_Avg = r._Cadence_Max = true;
			r._Distance = r._PercentAT = true;
			r._Calories = r._PulsePower = r._DragFactor = r._TSS_IF_NP = true;
			r._Gear = r._Gearing = true;
			ReportTemplateList.Add(r);

			ms_r_SpinScan = r = new ReportColumns("ReportSpinScan");
			r.Type = Types.ShowTemplate;
			r.Title = "SpinScan";
			r._Speed = r._Speed_Avg = r._Speed_Max = true;
			r._Watts = r._Watts_Avg = r._Watts_Max = r._Watts_Wkg = true;
			r._HeartRate = r._HeartRate_Avg = r._HeartRate_Max = true;
			r._Cadence = r._Cadence_Avg = r._Cadence_Max = true;
			r._Distance = r._Grade = r._Wind = true;
			r._Calories = r._PulsePower = r._DragFactor = r._TSS_IF_NP = true;
			r._SpinScan = r._LeftSS = r._RightSS = r._LeftATA = r._RightATA = r._LeftPower = r._RightPower = true;
			r._Gear = r._Gearing = true;
			ReportTemplateList.Add(r);

			r = new ReportColumns("F1AD0D8F-F6E3-4033-B272-9E2C0C661E6B");
			r.Type = ReportColumns.Types.Report;
			r.Title = "Defaults";
			r.ShowTemplate = ms_r_3DRoadRacing;
			ms_r_3DRoadRacing._Selected = r.Key;
			r._Speed = r._Watts = r._HeartRate = r._Cadence = r._Distance = r._Grade = r._SpinScan = r._Gearing = true;
			ReportList.Add(r.Dup());

			r = new ReportColumns("3044C019-14B1-4268-B9BC-8B0FC1C6A97D");
			r.Type = ReportColumns.Types.Report;
			r.Title = "Defaults";
			r.ShowTemplate = ms_r_RCV;
			ms_r_RCV._Selected = r.Key;
			r._Speed = r._Watts = r._HeartRate = r._Cadence = r._Distance = r._Grade = r._Gearing = true;
			ReportList.Add(r.Dup());

			r = new ReportColumns("D3F52F9F-6D4B-4820-AA48-1A2E1B97E3E8");
			r.Type = ReportColumns.Types.Report;
			r.Title = "Defaults";
			r.ShowTemplate = ms_r_WattTestingERG;
			ms_r_WattTestingERG._Selected = r.Key;
			r._Speed = r._Watts = r._Watts_Load = r._HeartRate = r._Cadence = r._Distance = r._Gearing = true;
			ReportList.Add(r.Dup());

			r = new ReportColumns("7E5E5EB2-57A9-4a3d-A299-E62C734E88C5");
			r.Type = ReportColumns.Types.Report;
			r.Title = "Defaults";
			r.ShowTemplate = ms_r_WattTestingAT;
			ms_r_WattTestingAT._Selected = r.Key;
			r._Speed = r._Watts = r._Watts_Load = r._HeartRate = r._Cadence = r._Distance = r._PercentAT = r._Gearing = true;
			ReportList.Add(r.Dup());

			r = new ReportColumns("349A2D42-5738-4240-955E-8F5E6AED66A6");
			r.Type = ReportColumns.Types.Report;
			r.Title = "Defaults";
			r.ShowTemplate = ms_r_SpinScan;
			ms_r_SpinScan._Selected = r.Key;
			r._Speed = r._Watts = r._HeartRate = r._Cadence = r._Distance = r._Grade = r._Gearing = true;
			ReportList.Add(r.Dup());
		}
		public ReportColumns()
		{
			Key = Guid.NewGuid().ToString(); // Build a GUID for this
			m_DB.Add(Key, this);
		}
		public ReportColumns(String key)
		{
			Key = key;
			m_DB.Add(Key, this);
		}

		StatFlags m_StatFlags;
		int m_StatFlagsVersion = 0;
		public StatFlags StatFlags
		{
			get
			{
				if (ver != m_StatFlagsVersion)
				{
					StatFlags f = StatFlags.Zero;
					if (_Speed) f |= RacerMateOne.StatFlags.Speed;
					if (_Speed_Avg) f |= RacerMateOne.StatFlags.Speed_Avg;
					if (_Speed_Max) f |= RacerMateOne.StatFlags.Speed_Max;
					if (_Watts) f |= RacerMateOne.StatFlags.Watts;
					if (_Watts_Avg) f |= RacerMateOne.StatFlags.Watts_Avg;
					if (_Watts_Max) f |= RacerMateOne.StatFlags.Watts_Max;
					if (_Watts_Wkg) f |= RacerMateOne.StatFlags.Watts_Wkg;
					if (_Watts_Load) f |= RacerMateOne.StatFlags.Watts_Load;
					if (_HeartRate) f |= RacerMateOne.StatFlags.HeartRate;
					if (_HeartRate_Avg) f |= RacerMateOne.StatFlags.HeartRate_Avg;
					if (_HeartRate_Max) f |= RacerMateOne.StatFlags.HeartRate_Max;
					if (_Cadence) f |= RacerMateOne.StatFlags.Cadence;
					if (_Cadence_Avg) f |= RacerMateOne.StatFlags.Cadence_Avg;
					if (_Cadence_Max) f |= RacerMateOne.StatFlags.Cadence_Max;
					if (_Distance) f |= RacerMateOne.StatFlags.Distance;
					if (_Lead) f |= RacerMateOne.StatFlags.Lead;
					if (_Grade) f |= RacerMateOne.StatFlags.Grade;
					if (_Wind) f |= RacerMateOne.StatFlags.Wind;
					//if (_Load) f |= RacerMateOne.StatFlags.Load;
					if (_PercentAT) f |= RacerMateOne.StatFlags.PercentAT;
					if (_Calories) f |= RacerMateOne.StatFlags.Calories;
					if (_PulsePower) f |= RacerMateOne.StatFlags.PulsePower;
					if (_DragFactor) f |= RacerMateOne.StatFlags.DragFactor;
					if (_TSS_IF_NP) f |= RacerMateOne.StatFlags.TSS_IF_NP;
					if (_SpinScan) f |= RacerMateOne.StatFlags.SS;
					if (_LeftSS) f |= RacerMateOne.StatFlags.SSLeft;
					if (_RightSS) f |= RacerMateOne.StatFlags.SSRight;
					if (_LeftATA) f |= RacerMateOne.StatFlags.SSLeftATA;
					if (_RightATA) f |= RacerMateOne.StatFlags.SSRightATA;
					if (_LeftPower) f |= RacerMateOne.StatFlags.SSLeftSplit;
					if (_RightPower) f |= RacerMateOne.StatFlags.SSRightSplit;
					if (_Gearing) f |= RacerMateOne.StatFlags.Gearing;
					if (_Gear) f |= RacerMateOne.StatFlags.GearInches;
					if (_RawSpinScan) f |= RacerMateOne.StatFlags.Bars;
					m_StatFlags = f;
					m_StatFlagsVersion = ver;
				}
				return m_StatFlags;
			}
		}
		public StatFlags StatFlags_Show
		{
			get { return ShowTemplate.StatFlags; }
		}


        public event PropertyChangedEventHandler PropertyChanged;
		bool m_incheck;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
				if (!_nodefCheck && name != "Reset_Show" && !m_incheck)
				{
					m_incheck = true;
					try { Reset_Show = Visibility.Visible; }
					catch { }
					m_incheck = false;
				}
            }
        }

		public ReportColumns Dup()
		{
			ReportColumns r = new ReportColumns();
			r.SetTo(this);
			Original = r;
			m_DB.Remove(r.Key);
			return this;
		}
		
		public void SetTo( ReportColumns r )
		{
			Type = r.Type;
			ShowTemplate = r.ShowTemplate;
			Original = r.Original;
			
			_Speed = r._Speed;
			_Speed_Avg = r._Speed_Avg;
			_Speed_Max = r._Speed_Max;
			_Watts = r._Watts;
			_Watts_Avg = r._Watts_Avg;
			_Watts_Max = r._Watts_Max;
			_Watts_Wkg = r._Watts_Wkg;
			_Watts_Load = r._Watts_Load;
			_HeartRate = r._HeartRate;
			_HeartRate_Avg = r._HeartRate_Avg;
			_HeartRate_Max = r._HeartRate_Max;
			_Cadence = r._Cadence;
			_Cadence_Avg = r._Cadence_Avg;
			_Cadence_Max = r._Cadence_Max;
			_Distance = r._Distance;
			_Lead = r._Lead;
			_Grade = r._Grade;
			_Wind = r._Wind;
			_Load = r._Load;
			_PercentAT = r._PercentAT;
			_Calories = r._Calories;
			_PulsePower = r._PulsePower;
			_DragFactor = r._DragFactor;
			_TSS_IF_NP = r._TSS_IF_NP;
			_SpinScan = r._SpinScan;
			_LeftSS = r._LeftSS;
			_RightSS = r._RightSS;
			_LeftATA = r._LeftATA;
			_RightATA = r._RightATA;
			_LeftPower = r._LeftPower;
			_RightPower = r._RightPower;
			_Gear = r._Gear;
			_Gearing = r._Gearing;
			_RawSpinScan = r._RawSpinScan;
			_CadenceTiming = r._CadenceTiming;
			ver++;
		}
		public void Reset()
		{
			if (Original == null)
				return;
			ReportColumns r = Original;
			_nodefCheck = true;
			Speed = r._Speed;
			Speed_Avg = r._Speed_Avg;
			Speed_Max = r._Speed_Max;
			Watts = r._Watts;
			Watts_Avg = r._Watts_Avg;
			Watts_Max = r._Watts_Max;
			Watts_Wkg = r._Watts_Wkg;
			Watts_Load = r._Watts_Load;
			HeartRate = r._HeartRate;
			HeartRate_Avg = r._HeartRate_Avg;
			HeartRate_Max = r._HeartRate_Max;
			Cadence = r._Cadence;
			Cadence_Avg = r._Cadence_Avg;
			Cadence_Max = r._Cadence_Max;
			Distance = r._Distance;
			Lead = r._Lead;
			Grade = r._Grade;
			Wind = r._Wind;
			Load = r._Load;
			PercentAT = r._PercentAT;
			Calories = r._Calories;
			PulsePower = r._PulsePower;
			DragFactor = r._DragFactor;
			TSS_IF_NP = r._TSS_IF_NP;
			SpinScan = r._SpinScan;
			LeftSS = r._LeftSS;
			RightSS = r._RightSS;
			LeftATA = r._LeftATA;
			RightATA = r._RightATA;
			LeftPower = r._LeftPower;
			RightPower = r._RightPower;
			Gear = r._Gear;
			Gearing = r._Gearing;
			RawSpinScan = r._RawSpinScan;
			CadenceTiming = r._CadenceTiming;
			_nodefCheck = false;
			Reset_Show = Visibility.Visible;
		}

		public static bool IsSame(ReportColumns r1, ReportColumns r2)
		{
			return (
				r1.Type == r2.Type &&
				r1._Speed == r2._Speed &&
				r1._Speed_Avg == r2._Speed_Avg &&
				r1._Speed_Max == r2._Speed_Max &&
				r1._Watts == r2._Watts &&
				r1._Watts_Avg == r2._Watts_Avg &&
				r1._Watts_Max == r2._Watts_Max &&
				r1._Watts_Wkg == r2._Watts_Wkg &&
				r1._Watts_Load == r2._Watts_Load &&
				r1._HeartRate == r2._HeartRate &&
				r1._HeartRate_Avg == r2._HeartRate_Avg &&
				r1._HeartRate_Max == r2._HeartRate_Max &&
				r1._Cadence == r2._Cadence &&
				r1._Cadence_Avg == r2._Cadence_Avg &&
				r1._Cadence_Max == r2._Cadence_Max &&
				r1._Distance == r2._Distance &&
				r1._Lead == r2._Lead &&
				r1._Grade == r2._Grade &&
				r1._Wind == r2._Wind &&
				r1._Load == r2._Load &&
				r1._PercentAT == r2._PercentAT &&
				r1._Calories == r2._Calories &&
				r1._PulsePower == r2._PulsePower &&
				r1._DragFactor == r2._DragFactor &&
				r1._TSS_IF_NP == r2._TSS_IF_NP &&
				r1._SpinScan == r2._SpinScan &&
				r1._LeftSS == r2._LeftSS &&
				r1._RightSS == r2._RightSS &&
				r1._LeftATA == r2._LeftATA &&
				r1._RightATA == r2._RightATA &&
				r1._LeftPower == r2._LeftPower &&
				r1._RightPower == r2._RightPower &&
				r1._Gear == r2._Gear &&
				r1._Gearing == r2._Gearing &&
				r1._RawSpinScan == r2._RawSpinScan &&
				r1._CadenceTiming == r2._CadenceTiming);
		}
		bool _nodefCheck;

		public Visibility SaveAs_Show
		{
			get
			{
				return Original == null || (Original != null && !IsSame(this, Original)) ? Visibility.Visible:Visibility.Collapsed;
			}
			set {  }
		}


		public Visibility Reset_Show 
		{ 
			get 
			{ 
				Visibility v = (Original == null || IsSame(this,Original) ? Visibility.Collapsed:Visibility.Visible);
				return v;
			}
			set { OnPropertyChanged("Reset_Show"); OnPropertyChanged("SaveAs_Show");  } 
		}

		public Visibility Delete_Show
		{
			get
			{
				return Original != null ? Visibility.Collapsed : Visibility.Visible;
			}
			set { OnPropertyChanged("Delete_Show"); OnPropertyChanged("SaveAs_Show"); }
		}

		public static void LoadDB(XDocument indoc)
		{
			XElement rootNode = indoc.Root;
			XElement dbnode = rootNode.Element("ReportColumns");
			if (dbnode != null)
				LoadDB(dbnode);
		}

		public static void LoadDB(XElement dbnode)
		{
			IEnumerable<XElement> nodelist = dbnode.Elements("Entry");
			foreach (XElement ele in nodelist)
			{
				String key = ele.Attribute("GUID").Value;
				ReportColumns r;
				XAttribute att;
				if (m_DB.ContainsKey(key))
					r = m_DB[key];
				else
				{
					r = new ReportColumns(key);
					att = ele.Attribute("Type");
					r.Type = (att == null ? ReportColumns.Types.Display:(att.Value == "Display" ? ReportColumns.Types.Display:ReportColumns.Types.Report));
					r.Title = ele.Attribute("Title").Value;

					r.ShowTemplate = m_DB[ele.Attribute("ShowTemplate").Value];
				}
				if (r.Original != null)
				{
					ReportColumns rsave = r.Original;
					r.SetTo(r.Original);
					r.Original = rsave;
				}

				if ((att = ele.Attribute("Speed")) != null) r._Speed = att.Value != "0";
				if ((att = ele.Attribute("Speed_Avg")) != null) r._Speed_Avg = att.Value != "0";
				if ((att = ele.Attribute("Speed_Max")) != null) r._Speed_Max = att.Value != "0";
				if ((att = ele.Attribute("Watts")) != null) r._Watts = att.Value != "0";
				if ((att = ele.Attribute("Watts_Avg")) != null) r._Watts_Avg = att.Value != "0";
				if ((att = ele.Attribute("Watts_Max")) != null) r._Watts_Max = att.Value != "0";
				if ((att = ele.Attribute("Watts_Wkg")) != null) r._Watts_Wkg = att.Value != "0";
				if ((att = ele.Attribute("Watts_Load")) != null) r._Watts_Load = att.Value != "0";
				if ((att = ele.Attribute("HeartRate")) != null) r._HeartRate = att.Value != "0";
				if ((att = ele.Attribute("HeartRate_Avg")) != null) r._HeartRate_Avg = att.Value != "0";
				if ((att = ele.Attribute("HeartRate_Max")) != null) r._HeartRate_Max = att.Value != "0";
				if ((att = ele.Attribute("Cadence")) != null) r._Cadence = att.Value != "0";
				if ((att = ele.Attribute("Cadence_Avg")) != null) r._Cadence_Avg = att.Value != "0";
				if ((att = ele.Attribute("Cadence_Max")) != null) r._Cadence_Max = att.Value != "0";
				if ((att = ele.Attribute("Distance")) != null) r._Distance = att.Value != "0";
				if ((att = ele.Attribute("Lead")) != null) r._Lead = att.Value != "0";
				if ((att = ele.Attribute("Grade")) != null) r._Grade = att.Value != "0";
				if ((att = ele.Attribute("Wind")) != null) r._Wind = att.Value != "0";
				if ((att = ele.Attribute("Load")) != null) r._Load = att.Value != "0";
				if ((att = ele.Attribute("PercentAT")) != null) r._PercentAT = att.Value != "0";
				if ((att = ele.Attribute("Calories")) != null) r._Calories = att.Value != "0";
				if ((att = ele.Attribute("PulsePower")) != null) r._PulsePower = att.Value != "0";
				if ((att = ele.Attribute("DragFactor")) != null) r._DragFactor = att.Value != "0";
				if ((att = ele.Attribute("TSS_IF_NP")) != null) r._TSS_IF_NP = att.Value != "0";
				if ((att = ele.Attribute("SpinScan")) != null) r._SpinScan = att.Value != "0";
				if ((att = ele.Attribute("LeftSS")) != null) r._LeftSS = att.Value != "0";
				if ((att = ele.Attribute("RightSS")) != null) r._RightSS = att.Value != "0";
				if ((att = ele.Attribute("LeftATA")) != null) r._LeftATA = att.Value != "0";
				if ((att = ele.Attribute("RightATA")) != null) r._RightATA = att.Value != "0";
				if ((att = ele.Attribute("LeftPower")) != null) r._LeftPower = att.Value != "0";
				if ((att = ele.Attribute("RightPower")) != null) r._RightPower = att.Value != "0";
				if ((att = ele.Attribute("Gear")) != null) r._Gear = att.Value != "0";
				if ((att = ele.Attribute("Gearing")) != null) r._Gearing = att.Value != "0";
				if ((att = ele.Attribute("RawSpinScan")) != null) r._RawSpinScan = att.Value != "0";
				if ((att = ele.Attribute("CadenceTiming")) != null) r._CadenceTiming = att.Value != "0";

				if (r.Type == Types.Display)
					DisplayList.Add(r);
				else if (r.Type == Types.Report)
					ReportList.Add(r);
			}
			nodelist = dbnode.Elements("Selected");
			foreach (XElement ele in nodelist)
			{
				XAttribute ga = ele.Attribute("GUID");
				XAttribute ka = ele.Attribute("Key");
				if (ga != null && ka != null)
				{
					ReportColumns ro, rs;
					if (m_DB.TryGetValue(ka.Value.ToString(), out ro) && m_DB.TryGetValue(ga.Value.ToString(), out rs))
					{
						ro.Selected = ga.Value.ToString();
					}
				}
			}
		}

		public XElement ToXElement()
		{
			XElement node = new XElement("Entry",
				new XAttribute("Title", Title),
				new XAttribute("GUID", Key),
				new XAttribute("Type", Type.ToString()),
				new XAttribute("ShowTemplate", ShowTemplate.Key),
				new XAttribute("Speed", _Speed ? "1" : "0"),
				new XAttribute("Speed_Avg", _Speed_Avg ? "1" : "0"),
				new XAttribute("Speed_Max", _Speed_Max ? "1" : "0"),
				new XAttribute("Watts", _Watts ? "1" : "0"),
				new XAttribute("Watts_Avg", _Watts_Avg ? "1" : "0"),
				new XAttribute("Watts_Max", _Watts_Max ? "1" : "0"),
				new XAttribute("Watts_Wkg", _Watts_Wkg ? "1" : "0"),
				new XAttribute("Watts_Load", _Watts_Load ? "1" : "0"),
				new XAttribute("HeartRate", _HeartRate ? "1" : "0"),
				new XAttribute("HeartRate_Avg", _HeartRate_Avg ? "1" : "0"),
				new XAttribute("HeartRate_Max", _HeartRate_Max ? "1" : "0"),
				new XAttribute("Cadence", _Cadence ? "1" : "0"),
				new XAttribute("Cadence_Avg", _Cadence_Avg ? "1" : "0"),
				new XAttribute("Cadence_Max", _Cadence_Max ? "1" : "0"),
				new XAttribute("Distance", _Distance ? "1" : "0"),
				new XAttribute("Lead", _Lead ? "1" : "0"),
				new XAttribute("Grade", _Grade ? "1" : "0"),
				new XAttribute("Wind", _Wind ? "1" : "0"),
				new XAttribute("Load", _Load ? "1" : "0"),
				new XAttribute("PercentAT", _PercentAT ? "1" : "0"),
				new XAttribute("Calories", _Calories ? "1" : "0"),
				new XAttribute("PulsePower", _PulsePower ? "1" : "0"),
				new XAttribute("DragFactor", _DragFactor ? "1" : "0"),
				new XAttribute("TSS_IF_NP", _TSS_IF_NP ? "1" : "0"),
				new XAttribute("SpinScan", _SpinScan ? "1" : "0"),
				new XAttribute("LeftSS", _LeftSS ? "1" : "0"),
				new XAttribute("RightSS", _RightSS ? "1" : "0"),
				new XAttribute("LeftATA", _LeftATA ? "1" : "0"),
				new XAttribute("RightATA", _RightATA ? "1" : "0"),
				new XAttribute("LeftPower", _LeftPower ? "1" : "0"),
				new XAttribute("RightPower", _RightPower ? "1" : "0"),
				new XAttribute("Gear", _Gear ? "1" : "0"),
				new XAttribute("Gearing", _Gearing ? "1" : "0"),
				new XAttribute("RawSpinScan", _RawSpinScan ? "1" : "0"),
				new XAttribute("CadenceTiming", _CadenceTiming ? "1" : "0")
				);
			return node;
		}

		// Dump out the DB to an xlement.
		public static XElement SaveDB()
		{
			XElement db = new XElement("ReportColumns");
			foreach (KeyValuePair<String, ReportColumns> k in m_DB)
			{
				ReportColumns r = k.Value;
				if ((r.Original == null || !IsSame(r, r.Original)) && r.Type != Types.ShowTemplate && r.ShowTemplate != null)
					db.Add(r.ToXElement());
			}
			foreach (ReportColumns r in DisplayTemplateList)
			{
				ReportColumns rr;
				if (r.Selected != null && m_DB.TryGetValue(r.Selected, out rr))
				{
					db.Add(new XElement("Selected", new XAttribute("Key", r.Key), new XAttribute("GUID", rr.Key)));
				}				
			}
			return db;
		}
	}
}
