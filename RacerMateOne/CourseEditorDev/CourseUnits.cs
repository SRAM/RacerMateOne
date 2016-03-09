using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RacerMateOne.CourseEditorDev
{
    public class CourseUnits : INotifyPropertyChanged
    {
      //  public CourseType Type { get; set; }
        public double MaxWind { get; set; }
        public double MinWind { get; set; }
        public string WindUnit { get; set; }

        public double MaxGrade { get; set; }
        public double MinGrade { get; set; }
        public string GradeUnit { get; set; }

        public double MaxWatt { get; set; }
        public double MinWatt { get; set; }
        public string WattUnit { get; set; }

        public double MaxLength { get; set; }
        public double MinLength { get; set; }
       
        public string MinMaxLengthUnit { get; set; }
        

        public double MaxTime { get; set; }
        public double MinTime { get; set; }
        public string TimeUnit { get; set; }
        public string MaxTimeUnit { get; set; }

        public double MaxAT { get; set; }
        public double MinAT { get; set; }
        public string ATUnit { get; set; }

        public bool Smooth { get; set; }
        public int Divisions { get; set; }


        string lengthUnit;
        public string LengthUnit
        {
            get { return lengthUnit; }

            set
            {
                lengthUnit = value;
                OnPropertyChanged("LengthUnit");
            }
        }

        string accumulatorUnit;
        public string AccumulatorUnit
        {
            get { return accumulatorUnit; }

            set
            {
                accumulatorUnit = value;
                OnPropertyChanged("AccumulatorUnit");
            }
        }


        double rotation = double.PositiveInfinity;
        public double Rotation
        {
            get {return rotation;}
            set {rotation = value;}
        }

        double startAT= 0.0;
        public double StartAT
        {
            get { return startAT; }
            set
            {
                startAT = value;
                OnPropertyChanged("StartAT");
            }
        }

        double endAT = 0.0;
        public double EndAT
        {
            get { return endAT; }
            set
            {
                endAT = value;
                OnPropertyChanged("EndAT");
            }
        }


        int segment = 0;
        public int Segment{
            get { return segment; }
            set
            {
                segment = value;
                OnPropertyChanged("Segment");
            }
        }

        double length = 0.0;
        public double Length
        {
            get { return length; }
            set
            {
                length = value;
                OnPropertyChanged("Length");
            }
        }


        double displayLength = 0.0;
        public double DisplayLength
        {
            get { return displayLength; }
            set
            {
                displayLength = value;
                OnPropertyChanged("DisplayLength");
            }
        }

        double displayGrade = 0.0;
        public double DisplayGrade
        {
            get { return displayGrade; }
            set
            {
                displayGrade = value;
                OnPropertyChanged("DisplayGrade");
            }
        }


        double displayaccumDist = 0.0;
        public double DisplayaccumDist
        {
            get { return displayaccumDist; }
            set
            {
                displayaccumDist = value;
                OnPropertyChanged("DisplayaccumDist");
            }
        }


        double wind = 0.0;
        public double Wind
        {
            get { return wind; }
            set
            {
                wind = value;
                OnPropertyChanged("Wind");
            }
        }

        double actualwind = 0.0;
        public double Actualwind
        {
            get { return actualwind; }
            set
            {
                actualwind = value;
                OnPropertyChanged("Actualwind");
            }
        }

        double startWatt = 0.0;
        public double StartWatt
        {
            get { return startWatt; }
            set
            {
                startWatt = value;
                OnPropertyChanged("StartWatt");
            }
        }

        double endWatt = 0.0;
        public double EndWatt
        {
            get { return endWatt; }
            set
            {
                endWatt = value;
                OnPropertyChanged("EndWatt");
            }
        }

        double time = 0.0;
        public double Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged("Time");
            }
        }

        double accumTimeAT = 0.0;
        public double AccumTimeAT
        {
            get { return accumTimeAT; }
            set
            {
                accumTimeAT = value;
                OnPropertyChanged("AccumTimeAT");
            }
        }

        double accumTimeWatt = 0.0;
        public double AccumTimeWatt
        {
            get { return accumTimeWatt; }
            set
            {
                accumTimeWatt = value;
                OnPropertyChanged("AccumTimeWatt");
            }
        }

        double accumTimeGrade = 0.0;
        public double AccumTimeGrade
        {
            get { return accumTimeGrade; }
            set
            {
                accumTimeGrade = value;
                OnPropertyChanged("AccumTimeGrade");
            }
        }

        //string segmentTime = string.Empty;
        //public string SegmentTime
        //{
        //    get { return segmentTime; }
        //    set
        //    {
        //        segmentTime = value;
        //        OnPropertyChanged("SegmentTime");
        //    }
        //}

        //string accumTime = string.Empty;
        //public string AccumTime
        //{
        //    get { return accumTime; }
        //    set
        //    {
        //        accumTime = value;
        //        OnPropertyChanged("AccumTime");
        //    }
        //}

        double accumDistGrade = 0.0;
        public double AccumDistGrade
        {
            get { return accumDistGrade; }
            set
            {
                accumDistGrade = value;
                OnPropertyChanged("AccumDistGrade");
            }
        }

        double accumDistWatt = 0.0;
        public double AccumDistWatt
        {
            get { return accumDistWatt; }
            set
            {
                accumDistWatt = value;
                OnPropertyChanged("AccumDistWatt");
            }
        }


        double accumDistAT = 0.0;
        public double AccumDistAT
        {
            get { return accumDistAT; }
            set
            {
                accumDistAT = value;
                OnPropertyChanged("AccumDistAT");
            }
        }



        

        double grade = 0.0;
        public double Grade
        {
            get { return grade; }
            set
            {
                grade = value;
                OnPropertyChanged("Grade");
            }
        }

        double startGrade = 0.0;
        public double StartGrade
        {
            get { return startGrade; }
            set
            {
                startGrade = value;
                OnPropertyChanged("StartGrade");
            }
        }

        double endGrade = 0.0;
        public double EndGrade
        {
            get { return endGrade; }
            set
            {
                endGrade = value;
                OnPropertyChanged("EndGrade");
            }
        }

        double asecent = 0.0;
        public double Asecent
        {
            get { return asecent; }
            set
            {
                asecent = value;
                OnPropertyChanged("Asecent");
            }
        }




        double windValue = 0;
        public double WindValue
        {
            get { return WindValue; }
            set
            {
                windValue = value;
              //  windValue = Ulilities.Round(value, 2);
              //  OnPropertyChanged("WindValue");
            }
        }


        PolygonData polygonDataValue;

        public PolygonData PolygonDataValue
        {
            get { return polygonDataValue; }
            set
            {
                polygonDataValue = value;
                OnPropertyChanged("PolygonDataValue");
            }
        }

        bool meter = false;

        public bool Meter
        {
            get { return meter; }
            set
            {
                meter = value;
                OnPropertyChanged("Meter");
            }
        }

        public CourseUnits(int Segment)
        {
            this.Segment = Segment;
        }

        public CourseUnits()
        {
           
        }

        CourseType courseMode;
        public CourseType CourseMode
        {
            get { return courseMode; }
            set
            {
                courseMode = value;
                OnPropertyChanged("CourseMode");
            }
        }


        public string getHashString()
        {
            string rstring = string.Empty;
            switch (this.CourseMode)
            {
                case CourseType.DISTANCEGRADE:
                    rstring = String.Format("{0:F3},{1:F3},{2:F3},{3:F3}", AccumDistGrade - Length, AccumDistGrade, StartGrade, EndGrade);
                    break;
                case CourseType.DISTANCEWATT:
                    rstring = String.Format("{0:F3},{1:F3},{2:F3},{3:F3}", AccumDistWatt - Length, AccumDistWatt, StartWatt, EndWatt);
                    break;
                case CourseType.DISTANCEPERAT:
                    rstring = String.Format("{0:F3},{1:F3},{2:F3},{3:F3}", AccumDistAT - Length, AccumDistAT, startAT, EndAT);
                   break;
                case CourseType.TIMEGRADE:
                   rstring = String.Format("{0:F3},{1:F3},{2:F3},{3:F3}", AccumTimeGrade - Time, AccumTimeGrade, StartGrade, EndGrade);
                 break;
                case CourseType.TIMEWATT:
                 rstring = String.Format("{0:F3},{1:F3},{2:F3},{3:F3}", AccumTimeWatt - Time, AccumTimeWatt, StartWatt, EndWatt);
                    break;
                case CourseType.TIMEPERAT:
                    rstring = String.Format("{0:F3},{1:F3},{2:F3},{3:F3}", AccumTimeAT - Time, AccumTimeAT, startAT, EndAT);
                 break;
            }

            return rstring;
            }

        public CourseUnits Copy()
        {
            CourseUnits CUnit = new CourseUnits();
            CUnit.CourseMode = this.CourseMode;

            switch (this.CourseMode)
            {
                case CourseType.DISTANCEGRADE:
                    {
                        CUnit.MaxLength     = this.MaxLength;
                        CUnit.MinLength     = this.MinLength;
                        CUnit.LengthUnit    = this.LengthUnit;
                        CUnit.Length        = this.Length;

                        CUnit.MaxGrade      = this.MaxGrade;
                        CUnit.MinGrade      = this.MinGrade;
                        CUnit.GradeUnit     = this.GradeUnit;
                        CUnit.StartGrade    = this.StartGrade;
                        CUnit.EndGrade      = this.EndGrade;
                        CUnit.Grade         = this.Grade;
                        CUnit.Smooth        = this.Smooth;
                        CUnit.Divisions     = this.Divisions;
                        CUnit.Rotation      = this.Rotation;


                        CUnit.MaxWind = this.MaxWind;
                        CUnit.MinWind = this.MinWind;
                        CUnit.WindUnit = this.WindUnit;
                        CUnit.Wind = this.Wind;
                        CUnit.PolygonDataValue = new PolygonData(this.startGrade, this.EndGrade);

                    }

                    break;
                case CourseType.DISTANCEWATT:
                    {
                        CUnit.MaxLength     = this.MaxLength;
                        CUnit.MinLength     = this.MinLength;
                        CUnit.LengthUnit    = this.LengthUnit;
                        CUnit.Length        = this.Length;

                        CUnit.MaxWatt       = this.MaxWatt;
                        CUnit.MinWatt       = this.MinWatt;
                        CUnit.WattUnit      = this.WattUnit;
                        CUnit.StartWatt     = this.StartWatt;
                        CUnit.EndWatt       = this.EndWatt;
                        CUnit.PolygonDataValue = new PolygonData(this.StartWatt, this.EndWatt);

                    }

                    break;
                case CourseType.DISTANCEPERAT:
                    {
                        CUnit.MaxLength     = this.MaxLength;
                        CUnit.MinLength     = this.MinLength;
                        CUnit.LengthUnit    = this.LengthUnit;
                        CUnit.Length        = this.Length;

                        CUnit.MaxAT         = this.MaxAT;
                        CUnit.MinAT         = this.MinAT;
                        CUnit.ATUnit        = this.ATUnit;
                        CUnit.StartAT       = this.StartAT;
                        CUnit.EndAT         = this.EndAT;
                        CUnit.PolygonDataValue = new PolygonData(this.StartAT, this.EndAT);


                    }
                    break;
                case CourseType.TIMEGRADE:
                    {
                        CUnit.MaxTime       = this.MaxTime;
                        CUnit.MinTime       = this.MinTime;
                        CUnit.TimeUnit      = this.TimeUnit;
                        CUnit.Time          = this.Time;

                        CUnit.MaxGrade      = this.MaxGrade;
                        CUnit.MinGrade      = this.MinGrade;
                        CUnit.GradeUnit     = this.GradeUnit;
                        CUnit.StartGrade    = this.StartGrade;
                        CUnit.EndGrade      = this.EndGrade;
                        CUnit.Grade         = this.Grade;
                        CUnit.AccumTimeAT   = this.AccumTimeAT;
                        CUnit.Wind          = this.Wind;

                        CUnit.PolygonDataValue = new PolygonData(this.StartGrade, this.EndGrade);

                    }

                    break;
                case CourseType.TIMEWATT:
                    {
                        CUnit.MaxTime       = this.MaxTime;
                        CUnit.MinTime       = this.MinTime;
                        CUnit.TimeUnit      = this.TimeUnit;
                        CUnit.Time          = this.Time;

                        CUnit.MaxWatt       = this.MaxWatt;
                        CUnit.MinWatt       = this.MinWatt;
                        CUnit.WattUnit      = this.WattUnit;
                        CUnit.StartWatt     = this.StartWatt;
                        CUnit.EndWatt       = this.EndWatt;
                        CUnit.PolygonDataValue = new PolygonData(this.StartWatt, this.EndWatt);


                    }
                    break;
                case CourseType.TIMEPERAT:
                     {
                        CUnit.MaxTime       = this.MaxTime;
                        CUnit.MinTime       = this.MinTime;
                        CUnit.TimeUnit      = this.TimeUnit;
                        CUnit.Time          = this.Time;

                        CUnit.MaxAT         = this.MaxAT;
                        CUnit.MinAT         = this.MinAT;
                        CUnit.ATUnit        = this.ATUnit;
                        CUnit.StartAT       = this.StartAT;
                        CUnit.EndAT         = this.EndAT;
                        CUnit.PolygonDataValue = new PolygonData(this.StartAT, this.EndAT);


                    }
                   break;
            }

            return CUnit;
        }

        public void Invalidate()
        {
            CourseUnits CUnit = new CourseUnits();
            CUnit.CourseMode = this.CourseMode;

            switch (this.CourseMode)
            {
                case CourseType.DISTANCEGRADE:
                    {
                        OnPropertyChanged("MaxLength");
                        OnPropertyChanged("MinLength");
                        OnPropertyChanged("LengthUnit");
                        OnPropertyChanged("MaxGrade");
                        OnPropertyChanged("MinGrade");
                        OnPropertyChanged("GradeUnit");
                        OnPropertyChanged("StartGrade");
                        OnPropertyChanged("EndGrade");
                        OnPropertyChanged("Smooth");
                        OnPropertyChanged("Divisions");
                        OnPropertyChanged("Rotation");
                        OnPropertyChanged("MaxWind");
                        OnPropertyChanged("MinWind");
                        OnPropertyChanged("WindUnit");
                        OnPropertyChanged("PolygonDataValue");
                    }

                    break;
                case CourseType.DISTANCEWATT:
                    {
                        OnPropertyChanged("MaxLength");
                        OnPropertyChanged("MinLength");
                        OnPropertyChanged("LengthUnit");
                        OnPropertyChanged("MaxWatt");
                        OnPropertyChanged("MinGrade");
                        OnPropertyChanged("WattUnit");
                        OnPropertyChanged("StartWatt");
                        OnPropertyChanged("EndWatt");
                        OnPropertyChanged("PolygonDataValue");
                    }

                    break;
                case CourseType.DISTANCEPERAT:
                    {
                        OnPropertyChanged("MaxLength");
                        OnPropertyChanged("MinLength");
                        OnPropertyChanged("LengthUnit");
                        OnPropertyChanged("MaxAT");
                        OnPropertyChanged("MinAT");
                        OnPropertyChanged("ATUnit");
                        OnPropertyChanged("StartAT");
                        OnPropertyChanged("EndAT");
                    }
                    break;
                case CourseType.TIMEGRADE:
                    {
                        OnPropertyChanged("MaxTime");
                        OnPropertyChanged("MinTime");
                        OnPropertyChanged("TimeUnit");
                        OnPropertyChanged("MaxGrade");
                        OnPropertyChanged("MinGrade");
                        OnPropertyChanged("GradeUnit");
                        OnPropertyChanged("StartGrade");
                        OnPropertyChanged("EndGrade");
                    }

                    break;
                case CourseType.TIMEWATT:
                    {
                        OnPropertyChanged("MaxTime");
                        OnPropertyChanged("MinTime");
                        OnPropertyChanged("TimeUnit");
                        OnPropertyChanged("MaxWatt");
                        OnPropertyChanged("MinWatt");
                        OnPropertyChanged("WattUnit");
                        OnPropertyChanged("StartWatt");
                        OnPropertyChanged("EndWatt");
                    }
                    break;
                case CourseType.TIMEPERAT:
                    {
                        OnPropertyChanged("MaxTime");
                        OnPropertyChanged("MinTime");
                        OnPropertyChanged("TimeUnit");
                        OnPropertyChanged("MaxAT");
                        OnPropertyChanged("MinAT");
                        OnPropertyChanged("ATUnit");
                        OnPropertyChanged("StartAT");
                        OnPropertyChanged("EndAT");
                    }
                    break;
            }
            OnPropertyChanged("Segment");
            OnPropertyChanged("PolygonDataValue");
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
