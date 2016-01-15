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

    public class RacerMateInfo : INotifyPropertyChanged
    {
        string name;
        public string Name {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
        string fileName;
        public string FileName 
        {
            get { return fileName; }
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
            }
        }
        string type;
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }
        bool looped;
        public bool Looped
        {
            get { return looped; }
            set
            {
                looped = value;
                OnPropertyChanged("Looped");
            }
        }

        string length;
        public string Length
        {
            get { return length; }
            set
            {
                length = value;
                OnPropertyChanged("Length");
            }
        }
       
        double startAt;
        public double StartAt
        {
            get { return startAt; }
            set
            {
                startAt = value;
                OnPropertyChanged("StartAt");
            }
        }

        double endAt;
        public double EndAt
        {
            get { return endAt; }
            set
            {
                endAt = value;
                OnPropertyChanged("EndAt");
            }
        }

        int laps;
        public int Laps
        {
            get { return laps; }
            set
            {
                laps = value;
                OnPropertyChanged("Laps");
            }
        }

        bool mirror;
        public bool Mirror
        {
            get { return mirror; }
            set
            {
                mirror = value;
                OnPropertyChanged("Mirror");
            }
        }

        bool reverse;
        public bool Reverse
        {
            get { return reverse; }
            set
            {
                reverse = value;
                OnPropertyChanged("Reverse");
            }
        }
        bool modified;
        public bool Modified
        {
            get { return modified; }
            set
            {
                modified = value;
                OnPropertyChanged("Modified");
            }
        }
        string xUnits;
        public string XUnits
        {
            get { return xUnits; }
            set
            {
                xUnits = value;
                OnPropertyChanged("XUnits");
            }
        }

        string yUnits;
        public string YUnits
        {
            get { return yUnits; }
            set
            {
                yUnits = value;
                OnPropertyChanged("YUnits");
            }
        }

        string originalHash;
        public string OriginalHash
        {
            get { return originalHash; }
            set
            {
                originalHash = value;
                OnPropertyChanged("OriginalHash");
            }
        }

        string courseHash;
        public string CourseHash
        {
            get { return courseHash; }
            set
            {
                courseHash = value;
                OnPropertyChanged("CourseHash");
            }
        }

        string headerHash;
        public string HeaderHash
        {
            get { return headerHash; }
            set
            {
                headerHash = value;
                OnPropertyChanged("HeaderHash");
            }
        }

        bool hasRotation = false;
        public bool HasRotation
        {
            get { return hasRotation; }
            set { hasRotation = value; }
        }

        public void Copy(RacerMateInfo Info)
        {
            this.Name           = Info.Name;
            this.Description    = Info.Description;
            this.FileName       = Info.FileName;
            this.Type           = Info.Type;
            this.Looped         = Info.Looped;
            this.Length         = Info.Length;
            this.Laps           = Info.Laps;
            this.StartAt        = Info.StartAt;
            this.EndAt          = Info.EndAt;
            this.Mirror         = Info.Mirror;
            this.Reverse        = Info.Reverse;
            this.Modified       = Info.Modified;
            this.XUnits         = Info.XUnits;
            this.YUnits         = Info.YUnits;
            this.OriginalHash   = Info.OriginalHash;
            this.CourseHash     = Info.CourseHash;
            this.HeaderHash     = Info.HeaderHash;

            OnPropertyChanged("Name");
            OnPropertyChanged("Description");
            OnPropertyChanged("FileName");
            OnPropertyChanged("Type");
            OnPropertyChanged("Looped");
            OnPropertyChanged("Length");
            OnPropertyChanged("Laps");
            OnPropertyChanged("StartAt");
            OnPropertyChanged("EndAt");
            OnPropertyChanged("Mirror");
            OnPropertyChanged("Reverse");
            OnPropertyChanged("Modified");
            OnPropertyChanged("XUnits");
            OnPropertyChanged("YUnits");
            OnPropertyChanged("OriginalHash");
            OnPropertyChanged("CourseHash");
            OnPropertyChanged("HeaderHash");

        }

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
