using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RacerMateOne.CourseEditorDev.Options  {

	/************************************************************************************************
		RiderExtended2 class
	************************************************************************************************/

	public class RiderExtended2 : Rider {
		private int ncranks = 3;
		private int ncogs = 10;

		private BitmapImage crank_bitmap_image = new BitmapImage(new Uri("../Images/L3.png", UriKind.Relative));
		private BitmapImage cog_bitmap_image = new BitmapImage(new Uri("../Images/S10.png", UriKind.Relative));
#if DEBUG
		private int bp = 0;
#endif

		public ObservableCollection<GearData> _CogGear;
		public ObservableCollection<GearData> _CrankGear;

		public ObservableCollection<GearData> CogGear {
			get {
				return _CogGear;
			}
		}

		/************************************************************************************************

		************************************************************************************************/

		public ObservableCollection<GearData> CrankGear {
			get {
				return _CrankGear;
			}
		}

		/************************************************************************************************

		************************************************************************************************/

		public int CurrentCogset {
			get {
				return ncogs;
		  	}

			set {
				ncogs = value;
				OnPropertyChanged("CurrentCogset");
#if DEBUG
				String s = string.Format("../Images/S{0}.png", ncogs);				// 
				bp = 1;
#endif
				CogGearSet = new BitmapImage(new Uri(string.Format("../Images/S{0}.png", ncogs), UriKind.Relative));
			}
		}

		/************************************************************************************************

		************************************************************************************************/

		public int nCranks  {
			get {
				return ncranks;
			}

			set {
				ncranks = value;
				//OnPropertyChanged("CurrentCrank");
				OnPropertyChanged("nCranks");
#if DEBUG
				String s = string.Format("../Images/L{0}.png", ncranks);				// ../Images/L2.png, etc
				bp = 1;
#endif
				Crankset = new BitmapImage(new Uri(string.Format("../Images/L{0}.png", ncranks), UriKind.Relative));
			}
		}

		/************************************************************************************************

		************************************************************************************************/

		public RiderExtended2() {
			_CrankGear = new ObservableCollection<GearData>();
			_CogGear = new ObservableCollection<GearData>();
		}

		/************************************************************************************************
			Change the start picture
		************************************************************************************************/

		public BitmapImage Crankset {
			get {
				return crank_bitmap_image;
			}
			set {
				crank_bitmap_image = value;
				OnPropertyChanged("Crankset");
			}
		}

		/************************************************************************************************

		************************************************************************************************/

		public BitmapImage CogGearSet {
			get {
				return cog_bitmap_image;
			}
			set {
				cog_bitmap_image = value;
				OnPropertyChanged("CogGearSet");
			}
		}


		/************************************************************************************************
			constructor
		************************************************************************************************/

		public RiderExtended2(Rider rider)
			: base(rider.DatabaseKey, rider.LastName, rider.FirstName, rider.NickName, rider.Gender, rider.AgeString, rider.HrAeT, rider.HrMax, rider.HrAlarmMin,
					rider.HrAlarmMax, rider.PowerAeT, rider.PowerFTP, rider.WeightRider, rider.WeightBike, rider.DragFactor, rider.GearingCrankset, rider.GearingCogset, rider.WheelDiameter)
		{

			_CrankGear = new ObservableCollection<GearData>();

#if DEBUG
			int n = rider.GearingCrankset.Length;									// 3
			bp = 1;
#endif
			for (int i=0; i<rider.GearingCrankset.Length; i++)  {
#if DEBUG
				n = GearingCrankset[i];													// 53, 39, 25
				bp = 1;
#endif
				_CrankGear.Add(new GearData(GearingCrankset[i], true));
			}

			//_CrankGear.Add(new GearData(25, true));				// Agha added, tlm20150406 commented out

			_CogGear = new ObservableCollection<GearData>();

#if DEBUG
			n = rider.GearingCogset.Length;									// 10
			bp = 3;
#endif

			for (int i = 0; i < rider.GearingCogset.Length; i++)   {
#if DEBUG
				n = GearingCogset[i];													// 26, ...
				bp = i;
#endif
				_CogGear.Add(new GearData(GearingCogset[i], true));
			}

			return;
		}						// constructor

	}							// public class RiderExtended2 : Rider



	/************************************************************************************************
				GearData class
	************************************************************************************************/

	public class GearData : INotifyPropertyChanged  {
		private int selectedCogset = 10;
		private Brush backBrush = Brushes.White;
		private int selectedCrankset = 1;
		private bool show;
		private int teeth;
#if DEBUG
		private int bp = 0;
#endif

#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
#endregion


		/************************************************************************************************

		************************************************************************************************/

		public int Teeth {
			get{
				return teeth;
			}

			set {
				teeth = value;
				OnPropertyChanged("Teeth");
			}
		}

		/************************************************************************************************

		************************************************************************************************/

		public bool Show {
			get {
				return show;
			}

			set {
				show = value;
				OnPropertyChanged("Show");
			}
		}

		/************************************************************************************************

		************************************************************************************************/

		public int SelectedCrankset {
			get {
				return selectedCrankset;
			}

			set {
				selectedCrankset = value;
				OnPropertyChanged("SelectedCrankset");
			}
		}


		/************************************************************************************************

		************************************************************************************************/

		public int SelectedCogset {
			get {
				return selectedCogset;
			}

			set {
				selectedCogset = value;
				OnPropertyChanged("SelectedCogset");
			}
		}

		/************************************************************************************************

		************************************************************************************************/

		public GearData(int Teeth, bool Show) {
			this.Teeth = Teeth;
			this.Show = Show;
		}

		/************************************************************************************************

		************************************************************************************************/

		public Brush BackBrush {
			get {
				return backBrush;
			}

			set {
				backBrush = value;
				OnPropertyChanged("BackBrush");
			}
		}
	}										// public class GearData : INotifyPropertyChanged

}											// namespace RacerMateOne.CourseEditorDev.Options

