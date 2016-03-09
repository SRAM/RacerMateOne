using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RacerMateOne.CourseEditorDev.Options
{
    public class BicycleRider
    {
        public string DatabaseKey { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string HRAeT { get; set; }
        public string HRMax { get; set; }
        public string HRMin { get; set; }
        public string AlarmMinZone { get; set; }
        public string AlarmMaxZone { get; set; }
        public string PowerAeT { get; set; }
        public string PowerFTP { get; set; }
        public string Metric { get; set; }
        public string WeightBike { get; set; }
        public string WeightRider { get; set; }
        public string Height { get; set; }
        public string DragFactor { get; set; }
        public string RiderType { get; set; }
        public string Skin { get; set; }
        public string Hair { get; set; }
        public string Helmet { get; set; }
        public string Shoes { get; set; }
        public string Clothing1 { get; set; }
        public string Clothing2 { get; set; }
        public string BikeColor1 { get; set; }
        public string BikeColor2 { get; set; }

        ObservableCollection<GearData> _CogGear;
        ObservableCollection<GearData> _CrankGear;

        public ObservableCollection<GearData> CogGear
        {
            get { return _CogGear; }
        }
        public ObservableCollection<GearData> CrankGear
        {
            get { return _CrankGear; }
        }

        public string WheelDiameter { get; set; }

        public BicycleRider()
        {
            _CogGear = new ObservableCollection<GearData>();
            _CrankGear = new ObservableCollection<GearData>();
        }
    }
}
