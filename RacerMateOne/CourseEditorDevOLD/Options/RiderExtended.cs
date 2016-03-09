using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RacerMateOne.CourseEditorDev.Options
{
    public class RiderExtended : Rider
    {
        public ObservableCollection<GearData> _CogGear;
        public ObservableCollection<GearData> _CrankGear;

        public ObservableCollection<GearData> CogGear
        {
            get { return _CogGear; }
        }
        public ObservableCollection<GearData> CrankGear
        {
            get { return _CrankGear; }
        }

        int currentCogset = 10;
        public int CurrentCogset
        {
            get { return currentCogset; }
            set
            {
                currentCogset = value;
                OnPropertyChanged("CurrentCogset");
                CogGearSet = new BitmapImage(new Uri(string.Format("../Images/S{0}.png", currentCogset), UriKind.Relative));
            }
        }

        int currentCrank = 3;
        public int CurrentCrank
        {
            get { return currentCrank; }
            set
            {
                currentCrank = value;
                OnPropertyChanged("CurrentCrank");
                Crankset = new BitmapImage(new Uri(string.Format("../Images/L{0}.png", currentCrank), UriKind.Relative));
            }
        }

        public RiderExtended()
        {
            _CrankGear = new ObservableCollection<GearData>();
            _CogGear = new ObservableCollection<GearData>();
        }

        // Change the start picture
        BitmapImage crankset = new BitmapImage(new Uri("../Images/L3.png", UriKind.Relative));
        public BitmapImage Crankset
        {
            get
            {
                return crankset;
            }
            set
            {
                crankset = value;
                OnPropertyChanged("Crankset");
            }
        }

        BitmapImage cogGearSet = new BitmapImage(new Uri("../Images/S10.png", UriKind.Relative));
        public BitmapImage CogGearSet
        {
            get { return cogGearSet; }
            set
            {
                cogGearSet = value;
                OnPropertyChanged("CogGearSet");
            }
        }

        public RiderExtended(Rider rider)
            : base(rider.DatabaseKey, rider.LastName, rider.FirstName, rider.NickName, rider.Gender, rider.AgeString, rider.HrAeT, rider.HrMax, rider.HrAlarmMin,
            rider.HrAlarmMax, rider.PowerAeT, rider.PowerFTP, rider.WeightRider, rider.WeightBike, rider.DragFactor, rider.GearingCrankset, rider.GearingCogset, rider.WheelDiameter)
        {
           
            _CrankGear = new ObservableCollection<GearData>();
            for (int i = 0; i < rider.GearingCrankset.Length; i++)
            {
                _CrankGear.Add(new GearData(GearingCrankset[i], true));
            }
            _CrankGear.Add(new GearData(25, true)); // Agha added

            _CogGear = new ObservableCollection<GearData>();

            for (int i = 0; i < rider.GearingCogset.Length; i++)
            {
                _CogGear.Add(new GearData(GearingCogset[i], true));
            }
        }
    }

    public class GearData : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        int teeth;
        public int Teeth
        {
            get{ return teeth;}
            set
            {
                teeth = value;
                OnPropertyChanged("Teeth");
            }
        }

        bool show;
        public bool Show
        {
            get { return show; }
            set
            {
                show = value;
                OnPropertyChanged("Show");
            }
        }

        int selectedCrankset = 1;
        public int SelectedCrankset
        {
            get { return selectedCrankset; }
            set
            {
                selectedCrankset = value;
                OnPropertyChanged("SelectedCrankset");
            }
        }

        int selectedCogset = 10;
        public int SelectedCogset
        {
            get { return selectedCogset; }
            set
            {
                selectedCogset = value;
                OnPropertyChanged("SelectedCogset");
            }
        }

        public GearData(int Teeth, bool Show)
        {
            this.Teeth = Teeth;
            this.Show = Show;
        }

        Brush backBrush = Brushes.White;
        public Brush BackBrush
        {
            get { return backBrush; }
            set
            {
                backBrush = value;
                OnPropertyChanged("BackBrush");
            }
        }
    }
}
