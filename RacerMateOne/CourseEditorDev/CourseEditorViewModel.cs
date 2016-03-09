using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace RacerMateOne.CourseEditorDev
{
    class CourseEditorViewModel : INotifyPropertyChanged
    {
        const double MeterToMile = 0.000621371192;
        public ObservableCollection<CourseUnits> courceUnitCollection;
        public ObservableCollection<CourseUnits> CourceUnitCollection
        {
            get { return courceUnitCollection; }
            set { courceUnitCollection = value; }
        }

        public CourseAttributes Attributes
        {
            get
            {
                return
                    (Info.Mirror ? CourseAttributes.Mirror : CourseAttributes.Zero) |
                    (Info.Reverse ? CourseAttributes.Reverse : CourseAttributes.Zero) |
                    (Info.Looped ? CourseAttributes.Looped : CourseAttributes.Zero) |
                    (Info.Modified ? CourseAttributes.Modified : CourseAttributes.Zero);
            }
            set
            {
                Info.Mirror = (value & CourseAttributes.Mirror) != CourseAttributes.Zero;
                Info.Reverse = (value & CourseAttributes.Reverse) != CourseAttributes.Zero;
                Info.Looped = (value & CourseAttributes.Looped) != CourseAttributes.Zero;
                Info.Modified = (value & CourseAttributes.Modified) != CourseAttributes.Zero;
            }
        }

        public CourseEditorViewModel()
        {
            courceUnitCollection = new ObservableCollection<CourseUnits>();
        }

        public Visibility undoVisibility = Visibility.Hidden;
        public Visibility UndoVisibility
        {
            get { return undoVisibility; }
            set
            {
                undoVisibility = value;
                OnPropertyChanged("UndoVisibility");
            }
        }

        public Visibility countVisibility = Visibility.Hidden;
        public Visibility CountVisibility
        {
            get { return countVisibility; }
            set
            {
                countVisibility = value;
                OnPropertyChanged("CountVisibility");
            }
        }

        public string legPath;
        public string LegPath
        {
            get { return legPath; }
            set
            {
                legPath = value;
                OnPropertyChanged("LegPath");
            }
        }

        public string courseFileName;
        public string CourseFileName
        {
            get { return courseFileName; }
            set
            {
                courseFileName = value;
                OnPropertyChanged("CourseFileName");
            }
        }

        public bool selectMeters;
        public bool SelectMeters
        {
            get { return selectMeters; }
            set
            {
                selectMeters = value;
                OnPropertyChanged("SelectMeters");
            }
        }

        Visibility memoryCanvas = Visibility.Hidden;
        public Visibility MemoryCanvas
        {
            get { return memoryCanvas; }
            set
            {
                memoryCanvas = value;
                OnPropertyChanged("MemoryCanvas");
            }
        }

       
        public RacerMateHeader Header;
        RacerMateInfo info;
        public RacerMateInfo Info
        {
            get { return info; }
            set
            {
                info = value;
                OnPropertyChanged("Info");
            }
        }

        string minMaxGrade;
        public string MinMaxGrade
        {
            get { return minMaxGrade; }
            set
            {
                minMaxGrade = value;
                OnPropertyChanged("MinMaxGrade");
            }
        }

        string avargeGrade;
        public string AvargeGrade
        {
            get { return avargeGrade; }
            set
            {
                avargeGrade = value;
                OnPropertyChanged("AvargeGrade");
            }
        }

        string minMaxWind;
        public string MinMaxWind
        {
            get { return minMaxWind; }
            set
            {
                minMaxWind = value;
                OnPropertyChanged("MinMaxWind");
            }
        }

        string avargeWind;
        public string AvargeWind
        {
            get { return avargeWind; }
            set
            {
                avargeWind = value;
                OnPropertyChanged("AvargeWind");
            }
        }


        string minMaxWatts;
        public string MinMaxWatts
        {
            get { return minMaxWatts; }
            set
            {
                minMaxWatts = value;
                OnPropertyChanged("MinMaxWatts");
            }
        }

        string avargeWatts;
        public string AvargeWatts
        {
            get { return avargeWatts; }
            set
            {
                avargeWatts = value;
                OnPropertyChanged("AvargeWatts");
            }
        }

        string minMaxTime;
        public string MinMaxTime
        {
            get { return minMaxTime; }
            set
            {
                minMaxTime = value;
                OnPropertyChanged("MinMaxTime");
            }
        }

        string minMaxDist;
        public string MinMaxDist
        {
            get { return minMaxDist; }
            set
            {
                minMaxDist = value;
                OnPropertyChanged("MinMaxDist");
            }
        }

        string avargeDist;
        public string AvargeDist
        {
            get { return avargeDist; }
            set
            {
                avargeDist = value;
                OnPropertyChanged("AvargeDist");
            }
        }

        string avargeTime;
        public string AvargeTime
        {
            get { return avargeTime; }
            set
            {
                avargeTime = value;
                OnPropertyChanged("AvargeTime");
            }
        }

        double totalDistance;
        public double TotalDistance
        {
            get { return totalDistance; }
            set
            {
                totalDistance = value;
                OnPropertyChanged("TotalDistance");
               // CourseDistance = string.Format("{0}", totalDistance);
            }
        }

        double saveTotalDistance;
        public double CourseEndAt
        {
            get { return saveTotalDistance; }
            set
            {
                saveTotalDistance = value;
                OnPropertyChanged("CourseEndAt");
            }
        }



        string courseDistance;
        public string CourseDistance
        {
            get { return courseDistance; }
            set
            {
                courseDistance = value;
                OnPropertyChanged("CourseDistance");
            }
        }

        public CourseType CurrCourseType { get; set; }

        public double OriginalMinimum {get; set;}
        public double OriginalMaximum { get; set; }
        bool saved = true;
        public bool Saved
        {
            get { return saved; }
            set { saved = value; }
        }

        public int SelectedIndex { get; set; } 


        public string FileName {get;set;}
    
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
    }
}
