using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Xml;
using System.IO;
using System.Data;
using System.Xml.Linq;




namespace RacerMateOne
{
	public enum RiderModels:int
	{
		Box = 0,
		Male_Triathlon,
		Female_Triathlon,
		Male_Road,
		Female_Road,
		Chrome_Male_Triathlon,
		Chrome_Female_Triathlon,
		Gold_Male_Triathlon,
		Gold_Female_Triathlon,

		BotBike = Chrome_Male_Triathlon
	};
	public enum RiderMaterials:int
	{
		Bike = 0,
		Tires,
		Skin,
		Hair,
		Helmet,
		Clothing_Top,
		Clothing_Bottom,
		Shoes,
		Max
	};


	public interface IRiderModel
	{
		RiderModels Model { get; }
		Color GetMaterialColor(RiderMaterials material);
	}

	public class RandomModel : IRiderModel
	{
		RiderModels m_Model = RiderModels.Male_Triathlon;
		static Color[] ms_SkinColors = new Color[] {			
			Color.FromRgb(255, 226, 187),
			Color.FromRgb(224, 187, 135),
			Color.FromRgb(210, 160, 134),
			Color.FromRgb(175, 104, 86),
			Color.FromRgb(118, 81, 58),
			Color.FromRgb(77, 47, 36) 
			};
		static Color[] ms_ClothingColor = new Color[] {
			Color.FromRgb(255,0,0),
			Color.FromRgb(0,255,0),
			Color.FromRgb(0,0,255),
			Color.FromRgb(255,255,0),
			Color.FromRgb(255,128,0),
			Color.FromRgb(128,128,0)
			};
		public Color[] Colors = new Color[(int)RiderMaterials.Max];

		public RandomModel()
		{
			Random r = new Random();
			if (r.Next(0, 1) == 1)
				m_Model = RiderModels.Female_Triathlon;
			for (int i = 0; i < (int)RiderMaterials.Max; i++)
			{
				Colors[i] = Color.FromRgb(255, 255, 255);
			}
			Colors[(int)RiderMaterials.Skin] = ms_SkinColors[r.Next(0, ms_SkinColors.Length - 1)];
			Colors[(int)RiderMaterials.Clothing_Top] = ms_ClothingColor[r.Next(0, ms_ClothingColor.Length - 1)];
			Colors[(int)RiderMaterials.Clothing_Bottom] = ms_ClothingColor[r.Next(0, ms_ClothingColor.Length - 1)];
		}
		public RiderModels Model { get { return m_Model; } set { m_Model = value; } }
		public Color GetMaterialColor(RiderMaterials material)
		{
			return Colors[(int)material];
		}
		public void Set(IRiderModel rm)
		{
			m_Model = rm.Model;
			for (int i = 0; i < (int)RiderMaterials.Max; i++)
				Colors[i] = rm.GetMaterialColor((RiderMaterials)i);
		}
	}


	public class Rider : INotifyPropertyChanged, IRiderModel
	{ //this is a class so that range checking is performed on property set
		public event PropertyChangedEventHandler PropertyChanged;

		private string _DatabaseKey;    // UNIQUE KEY -- enforced on new user creation/import
		private string _lastName;    // Duplicates permitted
		private string _firstName;   // Duplicates permitted
		private string _nickName; //duplicates permitted
		private string _gender;		// "M" or "F"
		private int _age;		// Age

		private int _hrAnT;          // HR @ anaerobic threshold
		private int _hrMin;		// HR @ resting threashold
		private int _hrMax;          // HR @ MAX Effort
		private int _alarmMinZone;
		private int _alarmMaxZone;
		private int _powerAnT;       // Power @ anaerobic threshold
		private int _powerFTP;       // Power @ Functional threshold
		private float _weightRider; // Weight in lbs
		private float _weightBike;  // Weight in lbs
		private int _dragFactor = 100;     // 3D drag -->  0-100
		private int _riderColor;
		private int _riderColor2;
		private float _wheelDiameter; //default wheel diameter 
		private string _riderType;
		private int[] _gearingCrankset = { 53, 39, 0 }; //initialize the default values for Velotron Crankset
		//     private int _velotronChainring;      //initialize the default values for Velotron Chainring
		private int[] _gearingCogset = { 26, 23, 21, 19, 17, 16, 14, 13, 12, 11 }; //initialize the default values for Velotron cogset

        private int _hrSensorId; // ANT+ HR Sensor ID
        private int _cadenceSensorId; // ANT+ Cadence Sensor ID

		// Not useing
		private string _birthday; //  as DD/MM/YYYY

		private bool _metric;


		private const string def_DatabaseKey = "11111111-1111-1111-1111-111111111111";    // UNIQUE KEY -- enforced on new user creation/import
		private const string def_lastName = "Adams";    // Duplicates permitted
		private const string def_firstName = "Sam";   // Duplicates permitted
		private const string def_nickName = "BarleyHops";
		private const string def_gender = "M";
		private const string def_birthday = "01/01/1960"; //note that Windows shows no dd/mm/yyyy format in slashes, only dashes dd-MMM-yyyy
		private const int def_age = 0;
		private const int def_hrAnT = 160;          // HR @ anaerobic threshold
		private const int def_hrMin = 60;			// HR @ minimum
		private const int def_hrMax = 170;          // HR @ MAX Effort
		private const int def_alarmMinZone = 0;		// Off
		private const int def_alarmMaxZone = 6;		// Off
		private const int def_powerAnT = 200;       // Power @ anaerobic threshold
		private const int def_powerFTP = 210;       // Power @ Functional threshold
		private const float def_weightRider = 160; // Weight in lbs
		private const float def_weightBike = 20;  // Weight in lbs
		private const int def_dragFactor = 100;     // 3D drag -->  0-100
		private const int def_riderColor = 0;
		private const int def_riderColor2 = 0;
		private const float def_wheelDiameter = 27; //default wheel diameter 
		private const string def_riderType = "Road";
		private int[] def_gearingCrankset = { 53, 39, 0 }; //initialize the default values for Velotron Crankset
		//     private const int def_velotronChainring = 62;      //initialize the default values for Velotron Chainring
		private int[] def_gearingCogset = { 26, 23, 21, 19, 17, 16, 14, 13, 12, 11 }; //initialize the default values for Velotron cogset

        private const int def_hrSensorId = 0; // ANT+ HR Sensor ID
        private const int def_cadenceSensorId = 0; // ANT+ Cadence Sensor ID

        private bool def_metric = false;

		private const float def_height = 1.778f; // 5' 10"
		// Properties are range checked when set


		/// <summary>
		/// DatabaseKey should be unique, but it entirely settable. Since this object does not necessarily have
		/// visibility into a fulldatabase, indeed it may exist before such list does
		/// it is not appropriate to do uniqueness check. Job Left to higher classes.
		/// </summary>
		public string DatabaseKey    // UNIQUE KEY -- enforced on new user creation/import
		{
				get { return _DatabaseKey; }
				set
				{
					// first deal with format problems
					try { _DatabaseKey = value; }

					catch { _DatabaseKey = def_DatabaseKey; }
					//Now enforce uniqueness of the key in the list
					//       _DatabaseKey = EnforceUniqueKeyInRiderList(value);
					OnPropertyChanged("DatabaseKey");
				}
		}

		/// <summary>
		/// Any string last name, set useful for later stripping characters
		/// </summary>
		public string LastName  // Duplicates permitted
		{
				get { return _lastName; }
				set
				{
					try { _lastName = value; }
					catch { _lastName = def_lastName; }
					OnPropertyChanged("LastName");
				OnPropertyChanged("");
				OnPropertyChanged(IDName);

				} //set is handy in case we want to pull out characters
		}

		/// <summary>
		/// Any string first name, set useful for later stripping characters
		/// </summary>
		public string FirstName   // Duplicates permitted
		{
				get { return _firstName; }
				set
				{
					try { _firstName = value; }
					catch { _firstName = def_firstName; }
					OnPropertyChanged("FirstName");
				OnPropertyChanged("FullName");
				OnPropertyChanged("IDName");

					//set is handy in case we want to pull out characters
				}
		}

		public string NickName   // Duplicates permitted
		{
				get { return _nickName; }
				set
				{
					try { _nickName = value; }
					catch { _nickName = def_nickName; }
				OnPropertyChanged("NickName");
				OnPropertyChanged("FullName");
				OnPropertyChanged("IDName");

					//set is handy in case we want to pull out characters
				}
		}
		public string FullName
		{
			get
			{
				String s = _firstName + " " + _lastName;
				s = s.Trim();
				return s == "" ? _nickName : s;
			}
		}


		public string IDName // Returns the Nickname or Firstname/Lastname
		{
			get
			{
				String s = _nickName == null ? "":_nickName.Trim();
				return (s == "" ? _firstName + " " + _lastName:s);
			}
		}


		// public string FirstAndLast   // Duplicates permitted
		// {
		//     get { return _firstName + " " + _lastName; }
		// }
		/// <summary>
		/// gender one of M or F
		/// </summary>
		public string Gender
		{
				get { return _gender; }
				set                     //range check and set to default
				{
				string g;
					try
					{
						if (value.ToUpper() == "M" || value.ToUpper() == "F")
								g = value.ToUpper();
						else
								g = def_gender;
					}
					catch { g = def_gender; }
				if (g != _gender)
				{
					_gender = g;
					OnPropertyChanged("Gender");
					redoModel();
				}
				}
		}

		/// <summary>
		///  birthday as DD/MM/YYYY, validate it is a valid form for Date
		///  this may have some culture issues, accepting ANY date time
		/// </summary>
		public string Birthday
		{
				get { return _birthday; }
				set
				{
					DateTime temp;
					try
					{
						bool time1 = DateTime.TryParse(value, out temp);
						if (time1 == true)
						{
								_birthday = value;
						}
						else
						{
								_birthday = def_birthday;
						}
					}
					catch (Exception)
					{
						_birthday = def_birthday;
					}
					OnPropertyChanged("Birthday");

				}
		}


		public int Age
		{
			get { return _age; }
			set
			{
				if (_age != value)
				{
					_age = value;
					if (_age < 0)
						Age = 0;
					OnPropertyChanged("Age");
				}
			}
		}
		public string AgeString
		{
			get { return (_age == 0 ? "" : _age.ToString()); }
			set
			{
				int na = (value.Trim() == "" ? 0:_age = Convert.ToInt32(value));
				if (na != _age)
				{
					_age = na;
					OnPropertyChanged("AgeString");
				}
			}
		}

		#region HeartRate

		/// <summary>
		/// hrAet non negative
		/// </summary>
		public int HrAnT
		{
				get { return _hrAnT; }
				set
				{
					try { _hrAnT = Math.Max(0, value); }
					catch { _hrAnT = def_hrAnT; }
					OnPropertyChanged("HrAnT");
				}//ensure not negative
		}

		/// <summary>
		/// hrMax non negative
		/// </summary>
		public int HrMax          // HR @ MAX Effort
		{
				get { return _hrMax; }
				set
				{
					try { _hrMax = Math.Max(0, value); }
					catch { _hrMax = def_hrMax; }
				calcZones();
					OnPropertyChanged("HrMax");
				}
				//ensure not negative
		}


		/// <summary>
		/// hrMin non negative
		/// </summary>
		public int HrMin          // HR @ MAX Effort
		{
			get { return _hrMin; }
			set
			{
				try { _hrMin = Math.Max(0, value); }
				catch { _hrMin = def_hrMin; }
				calcZones();
				OnPropertyChanged("HrMin");
			}
			//ensure not negative
		}

		private int[] m_hrZones = new int[7];
		public int HrZone1  { get {return m_hrZones[1]; } }
		public int HrZone2  { get {return m_hrZones[2]; } }
		public int HrZone3  { get {return m_hrZones[3]; } }
		public int HrZone4  { get {return m_hrZones[4]; } }
		public int HrZone5  { get {return m_hrZones[5]; } }

		private double m_hrToPercent;
		void calcZones()
		{
			double rhr = _hrMin;
			double mhr = _hrMax;
			m_hrZones[0] = _hrMin;
			m_hrZones[1] = (int)Math.Floor((mhr - rhr) * 0.5 + rhr);
			m_hrZones[2] = (int)Math.Floor((mhr - rhr) * 0.6 + rhr) + 1;
			m_hrZones[3] = (int)Math.Floor((mhr - rhr) * 0.7 + rhr) + 1;
			m_hrZones[4] = (int)Math.Floor((mhr - rhr) * 0.8 + rhr) + 1;
			m_hrZones[5] = (int)Math.Floor((mhr - rhr) * 0.9 + rhr) + 1;
			m_hrZones[6] = _hrMax;

			m_hrToPercent = 100.0 / (_hrMax - _hrMin);

			OnPropertyChanged("HrZone1");
			OnPropertyChanged("HrZone2");
			OnPropertyChanged("HrZone3");
			OnPropertyChanged("HrZone4");
			OnPropertyChanged("HrZone5");
			OnPropertyChanged("Zone1");
			OnPropertyChanged("Zone2");
			OnPropertyChanged("Zone3");
			OnPropertyChanged("Zone4");
			OnPropertyChanged("Zone5");
		}

		public double HeartRatePercentage(float heartrate)
		{
			return (heartrate - _hrMin) * m_hrToPercent;
		}
		public String HeartRateText(double minpercent, double maxpercent)
		{
			double rhr = _hrMin;
			double mhr = _hrMax;
			m_hrZones[0] = _hrMin;
			return String.Format("{0:F0}-{1:F1}", (mhr - rhr) * (minpercent / 100.0) + rhr, (mhr - rhr) * (maxpercent / 100.0) + rhr);
		}

		// Zones
		public String Zone1 { get { return String.Format("{0}-{1}", m_hrZones[1], m_hrZones[2] - 1); } }
		public String Zone2 { get { return String.Format("{0}-{1}", m_hrZones[2], m_hrZones[3] - 1); } }
		public String Zone3 { get { return String.Format("{0}-{1}", m_hrZones[3], m_hrZones[4] - 1); } }
		public String Zone4 { get { return String.Format("{0}-{1}", m_hrZones[4], m_hrZones[5] - 1); } }
		public String Zone5 { get { return String.Format("{0}-{1}", m_hrZones[5], _hrMax); } }


		public int AlarmMinZone
		{
			get { return _alarmMinZone; }
			set 
			{
				_alarmMinZone = value < 1 ? 0 : value > 5 ? 5 : value;
				if (_alarmMinZone > 0)
				{
					if (_alarmMaxZone < _alarmMinZone)
					{
						_alarmMaxZone = _alarmMinZone;
						OnPropertyChanged("AlarmMaxZone");
					}
				}
				OnPropertyChanged("AlarmMinZone");
			}
		}
		public int AlarmMaxZone
		{
			get { return _alarmMaxZone; }
			set
			{
				_alarmMaxZone = value < 1 ? 1 : value > 5 ? 6 : value;
				if (_alarmMaxZone < 6)
				{
					if (_alarmMaxZone < _alarmMinZone)
					{
						_alarmMinZone = _alarmMaxZone;
						OnPropertyChanged("AlarmMinZone");
					}
				}
			}
		}

		public int HrAlarmMin { get { return _alarmMinZone <= 0 ? 0 : m_hrZones[_alarmMinZone]; } }
		public int HrAlarmMax { get { return _alarmMaxZone <= 0 || _alarmMaxZone >= 6 ? 0 : m_hrZones[_alarmMaxZone+1]; } }



		/// <summary>
		/// powerAnT non negative
		/// Power @ anaerobic threshold
		/// </summary>
		public int PowerAnT
		{
				get { return _powerAnT; }
				set
				{
					try { _powerAnT = Math.Max(0, value); }
					catch { _powerAnT = def_powerAnT; }
					OnPropertyChanged("PowerAnT");
			}
		}

		#endregion


		/// <summary>
		/// Power @ Functional threshold is non-negative
		/// </summary>

		public int PowerFTP
		{
				get { return _powerFTP; }
				set
				{
					try { _powerFTP = Math.Max(0, value); }
					catch { _powerFTP = def_powerFTP; }
					OnPropertyChanged("PowerFTP");
				}
		}

		/// <summary>
		/// Rider Weight in lbs
		/// Handy to have this set function later for metric conversion to lbs
		/// non-negative
		/// </summary>
		public double WeightRider
		{
				get { return _weightRider; }
				set
				{
					try { _weightRider = (float)(Math.Max(0, value)); }
					catch { _weightRider = def_weightRider; }
					OnPropertyChanged("WeightRider");
				OnPropertyChanged("WeightTotal");
				OnPropertyChanged("WeightRiderDisplay");
				OnPropertyChanged("WeightTotalDisplay");
			}
		}


		/// <summary>
		/// Bike Weight in lbs
		/// Handy to have this set function later for metric conversion to lbs
		/// non-negative
		/// </summary>
		public double WeightBike
		{
				get { return _weightBike; }
				set
				{
					try { _weightBike = (float)Math.Max(0, value); }
					catch { _weightBike = def_weightBike; }
					OnPropertyChanged("WeightBike");
				OnPropertyChanged("WeightTotal");
				OnPropertyChanged("WeightBikeDisplay");
				OnPropertyChanged("WeightTotalDisplay");
			}
		}


		public double WeightTotal
		{
			get { return _weightRider + _weightBike; }
			set
			{
				double wt = _weightRider + _weightBike;
				double nwt;
				try { nwt = (float)Math.Max(0, value); }
				catch { return; }
				if (nwt == wt)
					return;

				_weightBike = 20f;
				_weightRider = (float)nwt - _weightBike;
				OnPropertyChanged("WeightRider");
				OnPropertyChanged("WeightBike");
				OnPropertyChanged("WeightTotal");
				OnPropertyChanged("WeightRiderDisplay");
				OnPropertyChanged("WeightBikeDisplay");
				OnPropertyChanged("WeightTotalDisplay");
			}
		}

		double toDisplay { get { return _metric ? ConvertConst.LBStoKGS : 1.0; } }
		double fromDisplay { get { return _metric ? ConvertConst.KGStoLBS : 1.0; } }
		public double WeightBikeDisplay
		{
			get { return Math.Round(_weightBike * toDisplay); }
			set { WeightBike = value * fromDisplay; }
		}
		public double WeightRiderDisplay
		{
			get { return Math.Round(_weightRider * toDisplay); }
			set { WeightRider = value * fromDisplay; }
		}
		public double WeightTotalDisplay
		{
			get { return Math.Round(WeightTotal * toDisplay); }
			set { WeightTotal = value * fromDisplay; }
		}
		public bool Metric
		{
			get { return _metric; }
			set { 
				if (_metric != value) 
				{ 
					_metric = value; 
					OnPropertyChanged("Metric");
					OnPropertyChanged("WeightRiderDisplay");
					OnPropertyChanged("WeightBikeDisplay");
					OnPropertyChanged("WeightTotalDisplay");
				} 
			}
		}


		/// <summary>
		/// 3d Drag 0-120
		/// </summary>
		public int DragFactor
		{
				get { return _dragFactor; }
				set
				{
					try
					{
						_dragFactor = Math.Max(0, value);
						_dragFactor = Math.Min(_dragFactor, 120);
					}
					catch { _dragFactor = def_dragFactor; }

				foreach (Unit unit in Unit.Units)
				{
					if (unit.Trainer != null && unit.Trainer.Rider == this)
						unit.Trainer.UpdateDragFactor();
				}

					OnPropertyChanged("DragFactor");
				}
		}


		public String RiderType
		{
			get {return _riderType; }
			set
			{
				string v = value == "Road" || value == "Triathlon" || value == "Chrome" || value == "Gold" ? value : def_riderType;
				if (v != _riderType)
				{
					_riderType = v;
					OnPropertyChanged("RiderType");
					OnPropertyChanged("RiderIndex");
					redoModel();
				}
			}
		}
		public int RiderTypeIndex
		{
			get { return _riderType == "Road" ? 0 : _riderType == "Triathlon" ? 1 : _riderType == "Chrome" ? 2 : 3; }
			set
			{
				RiderType = value == 0 ? "Road":value == 1 ? "Triathlon":value == 2 ? "Chrome":"Gold";
			}
		}



		/// <summary>
		/// // 3D rider preset ridercolor -->  index of UI control
		/// </summary>
		public int RiderColor
		{
				get { return _riderColor; }
				set
				{
					try
					{
						_riderColor = Math.Max(0, value); //should have a max index as well TBD
					}
					catch
					{
						_riderColor = def_riderColor;
					}
					OnPropertyChanged("RiderColor");
				}
		}
		/// <summary>
		/// // 3D rider preset ridercolor -->  index of UI control
		/// </summary>
	public int RiderColor2
		{
			get { return _riderColor2; }
			set
			{
				try
				{
					_riderColor2 = Math.Max(0, value); //should have a max index as well TBD
				}
				catch
				{
					_riderColor2 = def_riderColor2;
				}
				OnPropertyChanged("RiderColor2");
			}
		}

        public int HrSensorId
        {
            get { return _hrSensorId; }
            set
            {
                try
                {
                    _hrSensorId = Math.Max(0, value);
                }
                catch
                {
                    _hrSensorId = def_hrSensorId;
                }
                OnPropertyChanged("HrSensorId");
            }
        }

        public int CadenceSensorId
        {
            get { return _cadenceSensorId; }
            set
            {
                try
                {
                    _cadenceSensorId = Math.Max(0, value);
                }
                catch
                {
                    _cadenceSensorId = def_cadenceSensorId;
                }
                OnPropertyChanged("CadenceSensorId");
            }
        }

        // Velotron specific properties

        /********************************************************************************************************

		********************************************************************************************************/

        public int[] GearingCrankset {
				get {
					return _gearingCrankset;
				}

				set  {
					int i;
#if DEBUG
					bool result;
					result = NickName.Equals("barak", StringComparison.Ordinal);
					if (result)  {
						//int bp = 2;
						//bp = 0;
					}
#endif

					int max = value.Length > RM1.MAX_FRONT_GEARS ? RM1.MAX_FRONT_GEARS:value.Length;
					int[] v = new int[max];
					Array.Copy(value,v,max);
					Array.Sort(v, delegate(int a, int b) { return (a < b ? 1 : a > b ? -1 : 0); });

					for (i = 0; i < max; i++)  {
						if (v[i] <= 0)  {
							max = i;
							break;
						}
					}

					if (max == 0) {
						_gearingCrankset = new int[] { 1 };
					}
					else  {
						_gearingCrankset = new int[max];
						for (i = 0; i < max; i++) {
							_gearingCrankset[i] = v[i];
						}
					}

#if DEBUG
					if (result) {
						//int bp = 2;
						//bp = 0;
					}
#endif

					OnPropertyChanged("GearingCrankset");
				}
		}

		/********************************************************************************************************

		********************************************************************************************************/

		public int[] GearingCogset
		{
				get { return _gearingCogset; }
				set
				{
				int i;
				int max = value.Length > RM1.MAX_REAR_GEARS ? RM1.MAX_REAR_GEARS:value.Length;
				int[] v = new int[max];
				Array.Copy(value,v,max);
				for (i = 0; i < max; i++)
					if (v[i] <= 0)
					{
						max = i;
						break;
					}
				if (max == 0)
					_gearingCogset = new int[] { 1 };
				else
				{
					_gearingCogset = new int[max];
					for (i = 0; i < max; i++)
						_gearingCogset[i] = v[i];
				}
					OnPropertyChanged("GearingCogset");
				}
		}

		//      public int VelotronChainring
		//      {
		//          get { return _velotronChainring; }
		//          set
		//          {
		//              _velotronChainring = Math.Max(10, value); //smallest Chainring is 10
		//              OnPropertyChanged("VelotronChainring");
		//          }
		//      }


		/********************************************************************************************************

		********************************************************************************************************/

		public float WheelDiameter
		{
				get { return _wheelDiameter; }
				set
				{
					_wheelDiameter = Math.Max(10, value); //smallest wheel diameter is 10inches
					OnPropertyChanged("WheelDiameter");
				}
		}

		// constructor needed otherwise default values for each type are assigned
		/// <summary>
		/// Constructor for Rider fully writable parameters, range checked by this class.
		/// DataBaseKey will be changed if non-unique
		/// </summary>
		/// <param name="databaseKey"></param>
		/// <param name="lastName"></param>
		/// <param name="firstName"></param>
		/// <param name="gender"></param>
		/// <param name="birthday"></param>
		/// <param name="hrAnT"></param>
		/// <param name="hrMax"></param>
		/// <param name="hrAlarmMin"></param>
		/// <param name="hrAlarmMax"></param>
		/// <param name="powerAnT"></param>
		/// <param name="powerFTP"></param>
		/// <param name="weightRider"></param>
		/// <param name="weightBike"></param>
		/// <param name="dragFactor"></param>
		/// <param name="riderColor"></param>
		/// <param name="crankset"></param>
		/// <param name="cogset"></param>
		/// <param name="velotronChainring"></param>
		/// <param name="wheelDiameter"></param>

		//private int c_zoneAnT = -1;
		//private int c_zone = -1;


		public Rider(string databaseKey, string lastName, string firstName, string nickName, string gender, string age, int hrAnT,
					int hrMax, int hrAlarmMin, int hrAlarmMax, int powerAnT, int powerFTP, double weightRider, double weightBike, int dragFactor,
					int[] crankset, int[] cogset, float wheelDiameter)
		{
		
			this.DatabaseKey = databaseKey; //setting the DataBaseKey is okay from caller; this is usually called only from file load
			this.LastName = lastName;
			this.FirstName = firstName;
			this.NickName = nickName;
			this.Gender = gender;
			//this.Birthday = birthday;
			this.AgeString = age;
			this.HrAnT = hrAnT;
			this.HrMax = hrMax;
			this.PowerAnT = powerAnT;
			this.PowerFTP = powerFTP;
			this.WeightRider = weightRider;
			this.WeightBike = weightBike;
			this.DragFactor = dragFactor;
			this.GearingCrankset = crankset;
			this.GearingCogset = cogset;
			//this.VelotronChainring = velotronChainring;
			this.WheelDiameter = wheelDiameter;
			//FirstAndLast = this.FirstName + " " + this.LastName;
		}

		// okhere 1

		/// <summary>
		/// The null overload for new provides the default rider.
		/// </summary>

		public Rider()  {
		    DatabaseKey = def_DatabaseKey;  //setting the DataBaseKey in a New Rider() call will first adopt default and thencompute a distinct Key
		    LastName = def_lastName;
		    FirstName = def_firstName;
		    NickName = def_nickName;
		    Gender = def_gender;
		    Birthday = def_birthday;
		    HrAnT = def_hrAnT;
		    HrMin = def_hrMin;
		    HrMax = def_hrMax;
		    AlarmMinZone = def_alarmMinZone;
		    AlarmMaxZone = def_alarmMaxZone;
		    PowerAnT = def_powerAnT;
		    PowerFTP = def_powerFTP;
		    Metric = def_metric;
		    WeightRider = def_weightRider;
		    WeightBike = def_weightBike;
		    RiderType = def_riderType;
		    GearingCrankset = def_gearingCrankset;
		    GearingCogset = def_gearingCogset;
		    //VelotronChainring = def_velotronChainring;
		    WheelDiameter = def_wheelDiameter;
            HrSensorId = def_hrSensorId;
            CadenceSensorId = def_cadenceSensorId;
		}

		public override string ToString()  {
			return IDName;				
		}


		public void OnPropertyChanged(string name)  {
				if (PropertyChanged != null)  {
					PropertyChanged(this, new PropertyChangedEventArgs(name));
				}
		}

		public float WeightRiderKGS
		{
			get { return (float)(WeightRider * ConvertConst.LBStoKGS); }
		}
		public float WeightBikeKGS
		{
			get { return (float)(WeightBike * ConvertConst.LBStoKGS); }
		}
		public float WeightRiderLBS
		{
			get { return (float)WeightRider; }
		}
		public float WeightBikeLBS
		{
			get { return (float)WeightBike; }
		}

		public int ZoneHR(int hr)
		{
			if (hr < HrZone2)
				return HrZone1;
			else if (hr < HrZone2)
				return HrZone2;
			else if (hr < HrZone3)
				return HrZone3;
			else if (hr < HrZone4)
				return HrZone4;
			return HrZone5;
		}


		//==================================================================================================

		float m_Height = def_height;

		public float Height  {
			get { return m_Height; }
			set  {
				if (m_Height == value)
					return;

				m_Height = value;
				OnPropertyChanged("Height");
				OnPropertyChanged("HeightMetersDisplay");
				OnPropertyChanged("HeightInchesDisplay");
				OnPropertyChanged("HeightFeetDisplay");
			}
		}
		public double HeightMetersDisplay
		{
			get { return Math.Round(m_Height, 2); }
			set
			{
				Height = (float)Math.Round(value, 2);
			}
		}
		public int HeightFeetDisplay
		{
			get
			{
				return (int)Math.Floor(ConvertConst.MetersToFeet * m_Height);
			}
			set
			{
				int inches = HeightInchesDisplay;
				Height = (float)((value * 12 + inches) * ConvertConst.InchesToMeters);
			}
		}
		public int HeightInchesDisplay
		{
			get
			{
				double f = ConvertConst.MetersToFeet * m_Height * 12;
				return (int)Math.Round(f - (Math.Floor(f / 12) * 12));
			}
			set
			{
				int feet = HeightFeetDisplay;
				Height = (float)((feet * 12 + value) * ConvertConst.InchesToMeters);
			}
		}
		//==================================================================================================
		Color m_Skin = (Color)ColorConverter.ConvertFromString("#FFBD83");
		Color m_Hair = (Color)ColorConverter.ConvertFromString("#996600");
		Color m_Helmet = Colors.Black;
		Color m_Shoes = (Color)ColorConverter.ConvertFromString("#CCCCCC");
		Color m_BikeColor1 = Colors.White;
		Color m_BikeColor2 = (Color)ColorConverter.ConvertFromString("#343A52");
		Color m_Clothing1 = Colors.Red;
		Color m_Clothing2 = (Color)ColorConverter.ConvertFromString("#010147");

		public Color Skin
		{
			get { return m_Skin; }
			set { if (value != m_Skin) { m_Skin = value; OnPropertyChanged("Skin"); OnPropertyChanged("SkinBrush"); } }
		}
		public Color Hair
		{
			get { return m_Hair; }
			set { if (value != m_Hair) { m_Hair = value; OnPropertyChanged("Hair"); OnPropertyChanged("HairBrush"); } }
		}
		public Color Helmet
		{
			get { return m_Helmet; }
			set { if (value != m_Helmet) { m_Helmet = value; OnPropertyChanged("Helmet"); OnPropertyChanged("HelmetBrush"); } }
		}
		public Color Shoes
		{
			get { return m_Shoes; }
			set { if (value != m_Shoes) { m_Shoes = value; OnPropertyChanged("Shoes"); OnPropertyChanged("ShoesBrush"); } }
		}
		public Color BikeColor1
		{
			get { return m_BikeColor1; }
			set { if (value != m_BikeColor1) { m_BikeColor1 = value; OnPropertyChanged("BikeColor1"); OnPropertyChanged("BikeColor1Brush"); } }
		}
		public Color BikeColor2
		{
			get { return m_BikeColor2; }
			set { if (value != m_BikeColor2) { m_BikeColor2 = value; OnPropertyChanged("BikeColor2"); OnPropertyChanged("BikeColor2Brush"); } }
		}
		public Color Clothing1
		{
			get { return m_Clothing1; }
			set { if (value != m_Clothing1) { m_Clothing1 = value; OnPropertyChanged("Clothing1"); OnPropertyChanged("Clothing1Brush"); } }
		}
		public Color Clothing2
		{
			get { return m_Clothing2; }
			set { if (value != m_Clothing2) { m_Clothing2 = value; OnPropertyChanged("Clothing2"); OnPropertyChanged("Clothing2Brush"); } }
		}

		public SolidColorBrush SkinBrush
		{
			get { return new SolidColorBrush(m_Skin); }
			set { Skin = value.Color; } 
		}
		public SolidColorBrush HairBrush
		{
			get { return new SolidColorBrush(m_Hair); }
			set { Hair = value.Color; }
		}
		public SolidColorBrush HelmetBrush
		{
			get { return new SolidColorBrush(m_Helmet); }
			set { Helmet = value.Color; }
		}
		public SolidColorBrush ShoesBrush
		{
			get { return new SolidColorBrush(m_Shoes); }
			set { Shoes = value.Color; }
		}
		public SolidColorBrush BikeColor1Brush
		{
			get { return new SolidColorBrush(m_BikeColor1); }
			set { BikeColor1 = value.Color; }
		}
		public SolidColorBrush BikeColor2Brush
		{
			get { return new SolidColorBrush(m_BikeColor2); }
			set { BikeColor2 = value.Color; }
		}
		public SolidColorBrush Clothing1Brush
		{
			get { return new SolidColorBrush(m_Clothing1); }
			set { Clothing1 = value.Color; }
		}
		public SolidColorBrush Clothing2Brush
		{
			get { return new SolidColorBrush(m_Clothing2); }
			set { Clothing2 = value.Color; }
		}


		//==================================================================================================

		RiderModels m_RiderModel = RiderModels.Box;

		void redoModel()  {
			bool male = _gender == "M";
			RiderModels rm =
				RiderType == "Road" ? male ? RiderModels.Male_Road:RiderModels.Female_Road:
				RiderType == "Triathlon" ? male ? RiderModels.Male_Triathlon:RiderModels.Female_Triathlon:
				RiderType == "Chrome" ? male ? RiderModels.Chrome_Male_Triathlon: RiderModels.Chrome_Female_Triathlon:
				male ? RiderModels.Gold_Male_Triathlon:RiderModels.Gold_Female_Triathlon;
			if (rm != m_RiderModel)
			{
				m_RiderModel = rm;
				OnPropertyChanged("Model");
			}
		}
		public RiderModels Model { get { return m_RiderModel; } }
		public Color GetMaterialColor(RiderMaterials m)
		{
			switch (m)
			{
				case RiderMaterials.Bike: return BikeColor1;
				case RiderMaterials.Skin: return Skin;
				case RiderMaterials.Hair: return Hair;
				case RiderMaterials.Helmet: return Helmet;
				case RiderMaterials.Shoes: return Shoes;
				case RiderMaterials.Clothing_Top: return Clothing1;
				case RiderMaterials.Clothing_Bottom: return Clothing2;
			}
			return Colors.White;
		}


		XElement m_Snapshot;

		public void Snapshot()  {
			m_Snapshot = CreateXElement();
		}

		public DateTime Created = DateTime.Now;
		public DateTime Modified = DateTime.Now;

		public DateTime CreatedDate  {
				get { return Created; }
		}

		public DateTime ModifiedDate  {
				get { return Modified; }
		}

		public XElement UpdateModified()
		{
			XElement newss = CreateXElement();
			if (m_Snapshot == null)
			{
				m_Snapshot = newss;
			}
			else if (!XElement.DeepEquals(m_Snapshot, newss))
			{
				Modified = DateTime.Now;
				newss.Element("Modified").Value = Modified.ToString();
				m_Snapshot = newss;
			}
			return m_Snapshot;
		}

		public XElement CreateXElement()  {
			return CreateXElement(this);
		}


		public static XElement CreateXElement( Rider bbb )  {
			XElement xcrankset = new XElement("GearingCrankset");

			for (int i = 0; i < bbb.GearingCrankset.Length; i++)  {
				XElement e = new XElement("G" + (i + 1), bbb.GearingCrankset[i]);
				xcrankset.Add(e);
			}

			XElement xcogset = new XElement("GearingCogset");
			for (int i = 0; i < bbb.GearingCogset.Length; i++)
			{
				XElement e = new XElement("G" + (i + 1), bbb.GearingCogset[i]);
				xcogset.Add(e);
			}
			ColorConverter cc = new ColorConverter();
			XElement arider = new XElement("Rider",
				new XElement("Created",bbb.Created.ToString()),
				new XElement("Modified", bbb.Modified.ToString()),
				new XAttribute("DatabaseKey", bbb.DatabaseKey),
				new XElement("FirstName", bbb.FirstName),
				new XElement("LastName", bbb.LastName),
				new XElement("NickName", bbb.NickName),
				new XElement("Gender", bbb.Gender),
				//new XElement("Birthday", bbb.Birthday),
				new XElement("Age", bbb.Age),
                new XElement("HRAnT", bbb.HrAnT),
				new XElement("HRMax", bbb.HrMax),
				new XElement("HRMin", bbb.HrMin),
				new XElement("AlarmMinZone", bbb.AlarmMinZone == 0 ? "Off" : bbb.AlarmMinZone.ToString()),
				new XElement("AlarmMaxZone", bbb.AlarmMaxZone == 6 || bbb.AlarmMaxZone == 0 ? "Off" : bbb.AlarmMaxZone.ToString()),
                new XElement("PowerAnT", bbb.PowerAnT),
                new XElement("PowerFTP", bbb.PowerFTP),
				new XElement("Metric", bbb.Metric.ToString()),
				new XElement("WeightBike", bbb.WeightBike.ToString("####")),
				new XElement("WeightRider", bbb.WeightRider.ToString("####")),
				new XElement("Height", bbb.Height.ToString("####.####")),
				new XElement("DragFactor", bbb.DragFactor),
				new XElement("RiderType", bbb.RiderType),
				new XElement("Colors",
					new XAttribute("Skin", cc.ConvertToString(bbb.Skin)),
					new XAttribute("Hair", cc.ConvertToString(bbb.Hair)),
					new XAttribute("Helmet", cc.ConvertToString(bbb.Helmet)),
					new XAttribute("Shoes", cc.ConvertToString(bbb.Shoes)),
					new XAttribute("Clothing1", cc.ConvertToString(bbb.Clothing1)),
					new XAttribute("Clothing2", cc.ConvertToString(bbb.Clothing2)),
					new XAttribute("BikeColor1", cc.ConvertToString(bbb.BikeColor1)),
					new XAttribute("BikeColor2", cc.ConvertToString(bbb.BikeColor2))),
				xcrankset,
				xcogset,
				// new XElement("VelotronChainring",bbb.VelotronChainring),
				new XElement("WheelDiameter", bbb.WheelDiameter),
                new XElement("HrSensorId", bbb.HrSensorId),
                new XElement("CadenceSensorId", bbb.CadenceSensorId));

			return arider;
		}

	}
}

