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
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.ComponentModel;
using RacerMateOne.CourseEditorDev.AttachedProperties;
using RacerMateOne.CourseEditorDev.Dialogs;
using System.Windows.Threading;



namespace RacerMateOne.CourseEditorDev
{
	/// <summary>
	/// Interaction logic for SpinScan.xaml
	/// </summary>
	public partial class CourseEditor : Page
	{
        Stack<CourseEditorViewModel> VMStack = new Stack<CourseEditorViewModel>();
        List<CourseUnits> SelectedColection;
        CourseEditorViewModel CourceEditorVM;
        List<GPXTrack> gpxTrackList;

        SplashWindow splashWindow;

        const uint MaximumLegsAllowed = 15000;
        string MessageNotSaved = "You have not saved your course!  Press Cancel to Save.  Press OK to discard changes and exit.";

        public String CourseHash { get; set; }
        public String HeaderHash { get; set; }

        const double KmToMile = 0.621371;
        const double MeterToFeet = 3.2808399;
        const double KiloMeter = 1000.0;
        const double MeterToMile = 0.000621371192;
        const double KilometerToMile = 0.62137;
        const double OneMileInMeter = 1609.344;
        const double SecondsInMinute = 60.0;
        const double MileToFeet = 5280;
        const double OneMile = 1.0;


        bool MetricSelected = true;

        string strFileName;
		public CourseEditor(string strFileName)
		{
            this.strFileName = strFileName;
            InitializeComponent();
            Loaded += CourseEditor_Loaded;
		}

        void PushVM()
        {
            CourseEditorViewModel CEditorVM = new CourseEditorViewModel();
            CEditorVM.Header = new RacerMateHeader(CourceEditorVM.Header.CreatorExe, CourceEditorVM.Header.Date, CourceEditorVM.Header.Version, CourceEditorVM.Header.Copyright, CourceEditorVM.Header.Comment, CourceEditorVM.Header.CompressType);
            CEditorVM.Info = new RacerMateInfo();
            CEditorVM.Info.Name = CourceEditorVM.Info.Name;
            CEditorVM.Info.Description = CourceEditorVM.Info.Description;
            CEditorVM.Info.Type = CourceEditorVM.Info.Type;
            CEditorVM.Info.FileName = CourceEditorVM.Info.FileName;
            CEditorVM.Info.Looped = CourceEditorVM.Info.Looped;
            CEditorVM.Info.Length = CourceEditorVM.Info.Length;
            CEditorVM.Info.Laps = CourceEditorVM.Info.Laps;
            CEditorVM.Info.StartAt = CourceEditorVM.Info.StartAt;
            CEditorVM.Info.EndAt = CourceEditorVM.Info.EndAt;
            CEditorVM.Info.Mirror = CourceEditorVM.Info.Mirror;
            CEditorVM.Info.Reverse = CourceEditorVM.Info.Reverse;
            CEditorVM.Info.Modified = CourceEditorVM.Info.Modified;
            CEditorVM.Info.XUnits = CourceEditorVM.Info.XUnits;
            CEditorVM.Info.YUnits = CourceEditorVM.Info.YUnits;
            CEditorVM.Info.OriginalHash = CourceEditorVM.Info.OriginalHash;
            CEditorVM.Info.CourseHash = CourceEditorVM.Info.CourseHash;
            CEditorVM.Info.HeaderHash = CourceEditorVM.Info.HeaderHash;
            CEditorVM.AvargeDist = CourceEditorVM.AvargeDist;
            CEditorVM.AvargeGrade = CourceEditorVM.AvargeGrade;
            CEditorVM.AvargeTime = CourceEditorVM.AvargeTime;
            CEditorVM.AvargeWatts = CourceEditorVM.AvargeWatts;
            CEditorVM.AvargeWind = CourceEditorVM.AvargeWind;
            CEditorVM.UndoVisibility = CourceEditorVM.UndoVisibility;
            CEditorVM.LegPath = CourceEditorVM.LegPath;
            CEditorVM.CourseFileName = CourceEditorVM.CourseFileName;
            CEditorVM.SelectMeters = CourceEditorVM.SelectMeters;
            CEditorVM.MemoryCanvas = CourceEditorVM.MemoryCanvas;
            CEditorVM.MinMaxGrade = CourceEditorVM.MinMaxGrade;
            CEditorVM.MinMaxWind = CourceEditorVM.MinMaxWind;
            CEditorVM.MinMaxWatts = CourceEditorVM.MinMaxWatts;
            CEditorVM.MinMaxTime = CourceEditorVM.MinMaxTime;
            CEditorVM.MinMaxDist = CourceEditorVM.MinMaxDist;
            CEditorVM.OriginalMinimum = CourceEditorVM.OriginalMinimum;
            CEditorVM.OriginalMaximum = CourceEditorVM.OriginalMaximum;
            CEditorVM.Saved = CourceEditorVM.Saved;
            CEditorVM.FileName = CourceEditorVM.FileName;
            CEditorVM.CourceUnitCollection = new ObservableCollection<CourseUnits>();
            CEditorVM.SelectedIndex = CourceEditorVM.SelectedIndex;
            CEditorVM.CurrCourseType = CourceEditorVM.CurrCourseType;
            CEditorVM.TotalDistance = CourceEditorVM.TotalDistance;


            foreach (CourseUnits cu in CourceEditorVM.CourceUnitCollection)
            {
                CEditorVM.CourceUnitCollection.Add(cu.Copy());
            }
            
            VMStack.Push(CEditorVM);
            CourceEditorVM.UndoVisibility = Visibility.Visible;
        }

        
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (VMStack.Count > 0)
            {
                CourseEditorViewModel UnDoCourceEditor = VMStack.Pop();
                CourceEditorVM.CourceUnitCollection.Clear();

                CourceEditorVM.Header = new RacerMateHeader(UnDoCourceEditor.Header.CreatorExe, UnDoCourceEditor.Header.Date, UnDoCourceEditor.Header.Version, UnDoCourceEditor.Header.Copyright, UnDoCourceEditor.Header.Comment, UnDoCourceEditor.Header.CompressType);

                CourceEditorVM.Info = new RacerMateInfo();
                CourceEditorVM.Info.Copy(UnDoCourceEditor.Info);


                int i = 1;
                foreach (CourseUnits cu in UnDoCourceEditor.CourceUnitCollection)
                {
                    CourseUnits newCopy = cu.Copy();
                    newCopy.Segment = i++;
                    CourceEditorVM.CourceUnitCollection.Add(newCopy);
                }

                CourceEditorVM.CurrCourseType = UnDoCourceEditor.CurrCourseType;
                switch (CourceEditorVM.CurrCourseType)
                {
                    case CourseType.DISTANCEGRADE:
                        DistanceGradeUpdate();
                        break;
                    case CourseType.DISTANCEPERAT:
                        DistanceAtUpdate();
                        break;
                    case CourseType.DISTANCEWATT:
                        DistanceWattUpdate();
                        break;
                    case CourseType.TIMEGRADE:
                        TimeGradeUpdate();
                        break;
                    case CourseType.TIMEPERAT:
                        TimeAtUpdate();
                        break;
                    case CourseType.TIMEWATT:
                        TimeWattsUpdate();
                        break;
                }

                if (ListViewSegments.Items.Count > 0)
                {
                    DisplySegments.Text = string.Format("Segments: {0}", CourceEditorVM.CourceUnitCollection.Count);
                    ListViewSegments.SelectedIndex = ListViewSegments.Items.Count > UnDoCourceEditor.SelectedIndex ? UnDoCourceEditor.SelectedIndex : 0;
                }

                

                CourceEditorVM.AvargeDist = UnDoCourceEditor.AvargeDist;
                CourceEditorVM.AvargeGrade = UnDoCourceEditor.AvargeGrade;
                CourceEditorVM.AvargeTime = UnDoCourceEditor.AvargeTime;
                CourceEditorVM.AvargeWatts = UnDoCourceEditor.AvargeWatts;
                CourceEditorVM.AvargeWind = UnDoCourceEditor.AvargeWind;
                CourceEditorVM.UndoVisibility = UnDoCourceEditor.UndoVisibility;
                CourceEditorVM.LegPath = UnDoCourceEditor.LegPath;
                CourceEditorVM.CourseFileName = UnDoCourceEditor.CourseFileName;
                CourceEditorVM.SelectMeters = UnDoCourceEditor.SelectMeters;
                CourceEditorVM.MemoryCanvas = UnDoCourceEditor.MemoryCanvas;
                CourceEditorVM.MinMaxGrade = UnDoCourceEditor.MinMaxGrade;
                CourceEditorVM.MinMaxWind = UnDoCourceEditor.MinMaxWind;
                CourceEditorVM.MinMaxWatts = UnDoCourceEditor.MinMaxWatts;
                CourceEditorVM.MinMaxTime = UnDoCourceEditor.MinMaxTime;
                CourceEditorVM.MinMaxDist = UnDoCourceEditor.MinMaxDist;
                CourceEditorVM.OriginalMinimum = UnDoCourceEditor.OriginalMinimum;
                CourceEditorVM.OriginalMaximum = UnDoCourceEditor.OriginalMaximum;
                CourceEditorVM.Saved = UnDoCourceEditor.Saved;
                CourceEditorVM.FileName = UnDoCourceEditor.FileName;
                CourceEditorVM.TotalDistance = UnDoCourceEditor.TotalDistance;

                if (ListViewSegments.HasItems && ListViewSegments.SelectedIndex == -1)
                {
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
                    {
                        Mouse.OverrideCursor = null;
                        Application.Current.MainWindow.Cursor = null;
                        ListViewSegments.SelectedIndex = ListViewSegments.Items.Count > UnDoCourceEditor.SelectedIndex ? UnDoCourceEditor.SelectedIndex : 0;
                    }));
                }
            }

            CourceEditorVM.UndoVisibility = VMStack.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        bool IsMetric()
        {
            string strDir = @"\RacerMate\Settings\RM1_Settings.xml";
            string DocumentDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string FilePath = DocumentDir + strDir;
            XDocument xSettingdoc = XDocument.Load(FilePath);
            string strSettingVal = xSettingdoc.Descendants("Metric").First().Value.ToString().ToLower();

            MetricSelected = strSettingVal == "true" ? true : false;
            return MetricSelected;
        }

        string SavePath()
        {
            string DocumentDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RacerMate\Courses";
            return DocumentDir;
        }


        void SetDefaults(CourseUnits cu)
        {
            // I think this is OK
            if (IsMetric() == true)
            {
                cu.MaxLength = 100;
                cu.MinLength = 0.005;
               
                cu.LengthUnit = "Meter";

                cu.MaxWind = 100;
                cu.MinWind  = -100;
                cu.WindUnit = "km/h";
               
              
                cu.MaxLength = 100;
                cu.MinLength = 1;
                cu.LengthUnit = "Km";
                cu.AccumulatorUnit = "Km";
  
            }
            else
            {
                cu.MaxLength = 62.1;
                cu.MinLength = 0.003;
                cu.LengthUnit = "Miles";

                cu.MaxWind = 62.1;
                cu.MinWind = -62.1;
                cu.WindUnit = "M/h";

                cu.MaxLength = 62.1;
                cu.MinLength = 1;
                cu.LengthUnit = "Miles";
                cu.AccumulatorUnit = "Miles";
            }

            cu.Length = 1;
            cu.MaxTime = 60;
            cu.MinTime = 1;
            cu.Time = 1;
            cu.TimeUnit = "Min";

            cu.MaxAT = 200;
            cu.MinAT = -100;
            cu.ATUnit = "%AT";
            cu.StartAT = 100;
            cu.EndAT = 100;

            cu.MaxGrade = 24;
            cu.MinGrade = -24;
            cu.GradeUnit = "%";

            cu.MaxWatt = 2000;
            cu.MinWatt = 1;
            cu.StartWatt = 100;
            cu.EndWatt = 100;
            cu.WattUnit = "Watts";

        }

        void CourseEditor_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBoxImage stop = MessageBoxImage.Stop;
            MessageBoxButton buttons = MessageBoxButton.OK;

            string strExtenstion = System.IO.Path.GetExtension(this.strFileName).ToLower();
            if (strExtenstion != ".rmc")
            {
                string strMessage = string.Format("{0} It is not a cource file", this.strFileName);
                InfoDialog infoDialog = new InfoDialog(strMessage, ShowIcon.OK);
                infoDialog.Owner = Application.Current.MainWindow;
                infoDialog.ShowDialog();
                return;
            }
            
            CourceEditorVM = new CourseEditorViewModel();
            Mouse.OverrideCursor = Cursors.Wait;

            XDocument xdoc = XDocument.Load(this.strFileName);

            CourceEditorVM.CourseFileName = this.strFileName;
            CourceEditorVM.SelectMeters = IsMetric();

            string strVal = xdoc.Descendants("Header").First().ToString();
            bool hasCreatorExe = strVal.Contains("CreatorExe");
            bool hasDate = strVal.Contains("Date");
            bool hasVersion = strVal.Contains("Version");
            bool hasCopyright = strVal.Contains("Copyright");
            bool hasComment = strVal.Contains("Comment");
            bool hasCompressType = strVal.Contains("CompressType");

            bool result = hasCreatorExe && hasDate && hasVersion && hasCopyright && hasComment && hasCompressType;

            if (result == false)
            {
                string strFileName = this.strFileName;
                MessageBox.Show(string.Format("Unable to read header of this {0}", strFileName), "Reading Error", buttons, stop);
                return;
            }

             var Heder = from h in xdoc.Descendants("Header")
                           select new
                           {
                               CreatorExe = h.Element("CreatorExe").Value,
                               Date = h.Element("Date").Value,
                               Version = h.Element("Version").Value,
                               Copyright = h.Element("Copyright").Value,
                               Comment = h.Element("Comment").Value,
                               CompressType = h.Element("CompressType").Value
                           };

                string strInfo = xdoc.Descendants("Info").First().ToString();

                bool hasName            = strInfo.Contains("Name");
                bool hasDescription     = strInfo.Contains("Description");
                bool hasType            = strInfo.Contains("Type");
                bool hasLooped          = strInfo.Contains("Looped");
                bool hasFileName        = strInfo.Contains("FileName");
                bool hasLength          = strInfo.Contains("Length");
                bool hasLaps            = strInfo.Contains("Laps");
                bool hasStartAt         = strInfo.Contains("StartAt");
                bool hasEndAt           = strInfo.Contains("EndAt");
                bool hasMirror          = strInfo.Contains("Mirror");
                bool hasReverse         = strInfo.Contains("Reverse");
                bool hasModified        = strInfo.Contains("Modified");
                bool hasXUnits          = strInfo.Contains("XUnits");
                bool hasYUnits          = strInfo.Contains("YUnits");
                bool hasOriginalHash    = strInfo.Contains("OriginalHash");
                bool hasCourseHash      = strInfo.Contains("CourseHash");
                bool hasHeaderHash      = strInfo.Contains("HeaderHash");


                result = hasName && hasDescription && hasType && hasLooped && hasFileName && hasLength && hasLaps && hasStartAt && hasEndAt && hasMirror && hasReverse && hasModified && hasXUnits && hasYUnits && hasOriginalHash && hasCourseHash && hasHeaderHash;

                if (result == false)
                {
                    string strFileName = this.strFileName;
                    MessageBox.Show(string.Format("Unable to read info of this {0}", strFileName), "Reading Error", buttons, stop);
                    return;
                }

                var info = from r in xdoc.Descendants("Info")
                            select new
                            {
                                Name = r.Attribute("Name").Value,
                                Description = r.Attribute("Description").Value,
                                Type = r.Attribute("Type").Value,
                                Looped = r.Attribute("Looped").Value,
                                FileName = r.Attribute("FileName").Value,
                                Length = r.Attribute("Length").Value,
                                Laps = r.Attribute("Laps").Value,
                                StartAt = r.Attribute("StartAt").Value,
                                EndAt = r.Attribute("EndAt").Value,
                                Mirror = r.Attribute("Mirror").Value,
                                Reverse = r.Attribute("Reverse").Value,
                                Modified = r.Attribute("Modified").Value,
                                XUnits = r.Attribute("XUnits").Value,
                                YUnits = r.Attribute("YUnits").Value,
                                OriginalHash = r.Attribute("OriginalHash").Value,
                                CourseHash = r.Attribute("CourseHash").Value,
                                HeaderHash = r.Attribute("HeaderHash").Value
                            };

                
                
                var HeaderRacerMate = Heder.First();
                var infoRacerMate = info.First();
                CourceEditorVM.Header = new RacerMateHeader(HeaderRacerMate.CreatorExe, HeaderRacerMate.Date, HeaderRacerMate.Version,HeaderRacerMate.Copyright,HeaderRacerMate.Comment, HeaderRacerMate.CompressType);

                CourceEditorVM.Info = new RacerMateInfo();
                CourceEditorVM.Info.Name = infoRacerMate.Name;
                CourceEditorVM.Info.Description = infoRacerMate.Description;
                CourceEditorVM.Info.Type = infoRacerMate.Type;
                CourceEditorVM.Info.FileName = infoRacerMate.FileName;

                string strLooped = infoRacerMate.Looped;
                strLooped = strLooped.ToLower();
                CourceEditorVM.Info.Looped = strLooped == "true" ? true : false;
                CourceEditorVM.Info.Length = infoRacerMate.Length;

                string strLaps = infoRacerMate.Laps;
                strLaps = strLaps.ToLower();
                CourceEditorVM.Info.Laps = int.Parse(strLaps);

                string strStartAt = infoRacerMate.StartAt;
                CourceEditorVM.Info.StartAt = double.Parse(strStartAt);

                string strEndAt = infoRacerMate.EndAt;
                CourceEditorVM.Info.EndAt = double.Parse(strEndAt);

                string strMirror = infoRacerMate.Mirror;
                strMirror = strMirror.ToLower();

                CourceEditorVM.Info.Mirror = strMirror == "True" ? true : false;

                string strReverse = infoRacerMate.Reverse;
                strReverse = strReverse.ToLower();

                CourceEditorVM.Info.Reverse = strReverse == "True" ? true : false;

                string strModified = infoRacerMate.Modified;
                strModified = strModified.ToLower();
                CourceEditorVM.Info.Modified = strModified == "True" ? true : false; 
                CourceEditorVM.Info.XUnits = infoRacerMate.XUnits;
                CourceEditorVM.Info.YUnits = infoRacerMate.YUnits;
                CourceEditorVM.Info.OriginalHash = infoRacerMate.OriginalHash;
                CourceEditorVM.Info.CourseHash = infoRacerMate.CourseHash;
                CourceEditorVM.Info.HeaderHash = infoRacerMate.HeaderHash;


                CourceEditorVM.CurrCourseType = CourseType.DISTANCEGRADE;
                if (CourceEditorVM.Info.XUnits == "Distance" && CourceEditorVM.Info.YUnits == "Grade")
                {
                    CourceEditorVM.CurrCourseType = CourseType.DISTANCEGRADE;
                }
                else if (CourceEditorVM.Info.XUnits == "Distance" && CourceEditorVM.Info.YUnits == "Watts")
                {
                    CourceEditorVM.CurrCourseType = CourseType.DISTANCEWATT;
                }else if (CourceEditorVM.Info.XUnits == "Distance" && CourceEditorVM.Info.YUnits == "PercentAT")
                {
                    CourceEditorVM.CurrCourseType = CourseType.DISTANCEPERAT;
                }else if (CourceEditorVM.Info.XUnits == "Time" && CourceEditorVM.Info.YUnits == "Grade")
                {
                    CourceEditorVM.CurrCourseType = CourseType.TIMEGRADE;
                }else if (CourceEditorVM.Info.XUnits == "Time" && CourceEditorVM.Info.YUnits == "Watts")
                {
                    CourceEditorVM.CurrCourseType = CourseType.TIMEWATT;
                }else if (CourceEditorVM.Info.XUnits == "Time" && CourceEditorVM.Info.YUnits == "PercentAT")
                {
                    CourceEditorVM.CurrCourseType = CourseType.TIMEPERAT;
                }
               
                BackgroundWorker worker = new BackgroundWorker();

                switch (CourceEditorVM.CurrCourseType)
                {
                    case CourseType.DISTANCEGRADE:
                        ListViewSegments.View = (GridView)FindResource("GV_DISTGRADE");
                        HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_DISTGRADE");
                        CourceTypeTB.Text = "Distance/Grade";
                        worker.DoWork += (s, args) => LoadDistanceGrade((XDocument) args.Argument);
                        break;

                    case CourseType.DISTANCEWATT:
                        ListViewSegments.View = (GridView)FindResource("GV_DISTWATT");
                        HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_DISTWATT");
                        CourceTypeTB.Text = "Distance/Watt";
                        worker.DoWork += (s, args) => LoadDistanceWatt((XDocument)args.Argument);
                        break;
                    
                    case CourseType.DISTANCEPERAT:
                        ListViewSegments.View = (GridView)FindResource("GV_DISTAT");
                        HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_DISTAT");
                        CourceTypeTB.Text = "Distance/%AT";
                        worker.DoWork += (s, args) => LoadDistanceAt((XDocument)args.Argument);
                        break;

                    case CourseType.TIMEGRADE:
                        ListViewSegments.View = (GridView)FindResource("GV_TIMEGRADE");
                        HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_TIMEGRADE");
                        CourceTypeTB.Text = "Time/Grade";
                        worker.DoWork += (s, args) => LoadTimeGrade((XDocument)args.Argument);
                        break;
                    
                    case CourseType.TIMEWATT:
                        ListViewSegments.View = (GridView)FindResource("GV_TIMEWATT");
                        HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_TIMEWATT");
                        CourceTypeTB.Text = "Time/Watt";
                        worker.DoWork += (s, args) => LoadTimeWatt((XDocument)args.Argument);
                        break;
                    
                    case CourseType.TIMEPERAT:
                        ListViewSegments.View = (GridView)FindResource("GV_TIMEAT");
                        HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_TIMEAT");
                        CourceTypeTB.Text = "Time/%AT";
                        worker.DoWork += (s, args) => LoadTimeAt((XDocument)args.Argument);
                        break;
                   
                }

                worker.RunWorkerCompleted += (s, args) =>
                {
                    Mouse.OverrideCursor = null;
                    uint total = (uint) CourceEditorVM.CourceUnitCollection.Count();
                    if (total > MaximumLegsAllowed)
                    {

                        string strMessage = string.Format("You are going to edit {0} segments and due to memory limitation this might not possible. Do you want to proceed?", total);
                        OkCancelDialog okCancelDialog = new OkCancelDialog(strMessage,"proceed");
                        okCancelDialog.Owner = Application.Current.MainWindow;
                        bool bResult = (bool)okCancelDialog.ShowDialog();
                        if (bResult == false)
                        {
                            return;
                        }
                    }

                    Mouse.OverrideCursor = Cursors.Wait;
                    splashWindow = new SplashWindow();
                    splashWindow.Owner = Application.Current.MainWindow;
                    splashWindow.Show();

                    DataContext = CourceEditorVM;
                    DisplySegments.Text = string.Format("Segments: {0}", CourceEditorVM.CourceUnitCollection.Count);

                    CurrentDistance();
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle,  new Action(() =>
                        {
                            Mouse.OverrideCursor = null;
                            Application.Current.MainWindow.Cursor = null;
                            splashWindow.Close();
                            
                            if (ListViewSegments.HasItems && ListViewSegments.SelectedIndex == -1)
                            {
                                ListViewSegments.SelectedIndex = 0;
                            }
                        }));
                   // Mouse.OverrideCursor = null;
                };
                worker.RunWorkerAsync(xdoc);
                if (ListViewSegments.Items.Count > 0)
                {
                   
                    DisplySegments.Text = string.Format("Segments: {0}", CourceEditorVM.CourceUnitCollection.Count);
                    ListViewSegments.SelectedIndex = 0;
                }
            }


        double SaveCurrentDistance()
        {
            double AccumulatorValue = 0.0;
            switch (CourceEditorVM.CurrCourseType)
            {
                case CourseType.DISTANCEGRADE:

                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumDistGrade;
                    if (CourceEditorVM.SelectMeters == false)
                    {
                        AccumulatorValue *= OneMileInMeter;
                    }
                    else
                    {
                        string LengthUnit = CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit.ToLower();
                        if (LengthUnit == "km")
                        {
                            AccumulatorValue *= 1000;
                        }
                        else
                        {
                        }
                    }

                    break;
                case CourseType.DISTANCEWATT:
                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumDistWatt;
                    if (CourceEditorVM.SelectMeters == false)
                    {
                        AccumulatorValue *= OneMileInMeter;
                    }
                    else
                    {
                        string LengthUnit = CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit.ToLower();

                        if (LengthUnit == "km")
                        {
                            AccumulatorValue *= 1000;
                        }
                        else
                        {
                        }
                    }

                    break;
                case CourseType.DISTANCEPERAT:
                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumDistAT;
                    if (CourceEditorVM.SelectMeters == false)
                    {
                        AccumulatorValue *= OneMileInMeter;
                    }
                    else
                    {
                        string LengthUnit = CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit.ToLower();
                        if (LengthUnit == "km")
                        {
                            AccumulatorValue *= 1000;
                        }
                        else
                        {
                        }
                    }

                    break;
                case CourseType.TIMEGRADE:
                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumTimeGrade;
                    AccumulatorValue *= 60;
                    break;
                case CourseType.TIMEWATT:
                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumTimeWatt;
                    AccumulatorValue *= 60;
                    break;
                case CourseType.TIMEPERAT:
                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumTimeAT;
                    AccumulatorValue *= 60;
                    break;
            }

            return Ulilities.Round(AccumulatorValue, 2);
        }
        double CurrentDistance()
        {
            double AccumulatorValue = 0.0;
            switch (CourceEditorVM.CurrCourseType)
            {
                case CourseType.DISTANCEGRADE:

                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumDistGrade;
                    if (CourceEditorVM.SelectMeters == false)
                    {
                       CourceEditorVM.CourseDistance = string.Format("Total distance : {0} {1} ",Ulilities.Round(AccumulatorValue,2), CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit);
                    }
                    else
                    {
                        string LengthUnit = CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit.ToLower();
                        CourceEditorVM.CourseDistance = string.Format("Total distance : {0} {1} ", Ulilities.Round(AccumulatorValue, 2), CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit);
                    }

                    break;
                case CourseType.DISTANCEWATT:
                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumDistWatt;
                    if (CourceEditorVM.SelectMeters == false)
                    {
                        CourceEditorVM.CourseDistance = string.Format("Total distance : {0} {1} ", Ulilities.Round(AccumulatorValue, 2), CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit);
                    }
                    else
                    {
                        string LengthUnit = CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit.ToLower();
                        CourceEditorVM.CourseDistance = string.Format("Total distance : {0} {1} ", Ulilities.Round(AccumulatorValue, 2), CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit);
                    }

                    break;
                case CourseType.DISTANCEPERAT:
                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumDistAT;
                    if (CourceEditorVM.SelectMeters == false)
                    {
                        AccumulatorValue *= OneMileInMeter;
                        CourceEditorVM.CourseDistance = string.Format("Total distance : {0} {1} ", Ulilities.Round(AccumulatorValue, 2), CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit);
                    }
                    else
                    {
                        string LengthUnit = CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit.ToLower();
                        CourceEditorVM.CourseDistance = string.Format("Total distance : {0} {1} ", Ulilities.Round(AccumulatorValue, 2), CourceEditorVM.CourceUnitCollection.Last().AccumulatorUnit);
                    }

                    break;
                case CourseType.TIMEGRADE:
                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumTimeGrade;
                    CourceEditorVM.CourseDistance = string.Format("Total Time : {0} {1} ", AccumulatorValue, CourceEditorVM.CourceUnitCollection.Last().TimeUnit);

                    break;
                case CourseType.TIMEWATT:
                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumTimeWatt;
                    CourceEditorVM.CourseDistance = string.Format("Total Time : {0} {1} ", AccumulatorValue, CourceEditorVM.CourceUnitCollection.Last().TimeUnit);

                    break;
                case CourseType.TIMEPERAT:
                    AccumulatorValue = CourceEditorVM.CourceUnitCollection.Last().AccumTimeAT;
                    CourceEditorVM.CourseDistance = string.Format("Total Time : {0} {1} ", AccumulatorValue, CourceEditorVM.CourceUnitCollection.Last().TimeUnit);
                    break;
            }
            return Ulilities.Round(AccumulatorValue, 2);
        }

              

        void ClearInfo()
        {
            if (CourceEditorVM.Info == null)
            {
                CourceEditorVM.Info = new RacerMateInfo();
                return;
            }
            
            CourceEditorVM.Info.Name = string.Empty;
            CourceEditorVM.Info.Description = string.Empty;
            CourceEditorVM.Info.Type = string.Empty;
            CourceEditorVM.Info.FileName = string.Empty;
            CourceEditorVM.Info.Looped = false;
            CourceEditorVM.Info.Length = string.Empty;
            CourceEditorVM.Info.Laps = 1;
            CourceEditorVM.Info.StartAt = 0.0;
            CourceEditorVM.Info.EndAt = 0.0;
            CourceEditorVM.Info.Mirror = false;
            CourceEditorVM.Info.Reverse = false;
            CourceEditorVM.Info.Modified = false;
            CourceEditorVM.Info.XUnits = string.Empty;
            CourceEditorVM.Info.YUnits = string.Empty;
            CourceEditorVM.Info.OriginalHash = string.Empty;
            CourceEditorVM.Info.CourseHash = string.Empty;
            CourceEditorVM.Info.HeaderHash = string.Empty;
            CourceEditorVM.Info.HasRotation = false;
        }

        
        void LoadDistanceGrade(XDocument xdoc)
        {
            var valFirst = xdoc.Descendants("Val").First();
            string strVal = valFirst.ToString();
            bool hasLength = strVal.Contains("Length");
            bool hasGrade = strVal.Contains("Grade");
            bool hasWind = strVal.Contains("Wind");
            bool hasRtation = strVal.Contains("Rotation");

            
            if (hasLength == false || hasGrade == false || hasWind == false)
            {
                MessageBox.Show("It is not Distance Grade File");
                return;
            }

            var AllValues = valFirst;
            strVal = valFirst.ToString();
            CourseUnits temp = new CourseUnits();
            temp.Grade = double.Parse(valFirst.Element("Grade").Value);
            double Wind = double.Parse(valFirst.Element("Wind").Value);
            double Length = double.Parse(valFirst.Element("Length").Value);

            temp.Wind = Ulilities.Round(Wind, 0);

            temp.Length = Length;

            temp.Smooth = strVal.Contains("Smooth") ? bool.Parse(valFirst.Attribute("Smooth").Value) : false;
            temp.Divisions = strVal.Contains("Divisions") ? int.Parse(valFirst.Element("Divisions").Value) : 0;
            temp.Rotation = strVal.Contains("Rotation") ? double.Parse(valFirst.Element("Rotation").Value) : 0;

            if (strVal.Contains("Rotation") == true && CourceEditorVM.Info.HasRotation == false)
                CourceEditorVM.Info.HasRotation = true;
            

            temp.CourseMode = CourseType.DISTANCEGRADE;
            CourceEditorVM.CourceUnitCollection.Add(temp);
               
            AllValues = (XElement)AllValues.NextNode;


            while (AllValues != null)
            {
                strVal = AllValues.ToString();
                if (strVal.Contains("Rotation") == true && CourceEditorVM.Info.HasRotation == false)
                    CourceEditorVM.Info.HasRotation = true;

                temp = new CourseUnits();
                temp.Grade = double.Parse(AllValues.Element("Grade").Value);
                Wind = double.Parse(AllValues.Element("Wind").Value);
                Length = double.Parse(AllValues.Element("Length").Value);
                temp.Wind = Ulilities.Round(Wind, 0);
                temp.Length = Length;

                temp.Smooth = strVal.Contains("Smooth") ? bool.Parse(AllValues.Attribute("Smooth").Value) : false;
                temp.Divisions = strVal.Contains("Divisions") ? int.Parse(AllValues.Element("Divisions").Value) : 0;
                temp.Rotation = strVal.Contains("Rotation") ? double.Parse(AllValues.Element("Rotation").Value) : 0;
                
                temp.CourseMode = CourseType.DISTANCEGRADE;
                CourceEditorVM.CourceUnitCollection.Add(temp);
                AllValues = (XElement)AllValues.NextNode;
            }
            

            DistanceGradeUpdate();
           
        }

        void DistanceGradeUpdate()
        {
            PolygonData.MaxValue = double.NegativeInfinity;
            PolygonData.MinValue = double.PositiveInfinity;

            int i = 1;
            double AccumDis = 0.0;
            double previousEndValue = 0.0;
            int count = CourceEditorVM.CourceUnitCollection.Count;

            double MaxGrade = double.NegativeInfinity;
            double MinGrade = double.PositiveInfinity;

            double MaxWind = double.NegativeInfinity;
            double MinWind = double.PositiveInfinity;

            double AvargeGrade = 0.0;
            double AvargeWind = 0.0;


            foreach (CourseUnits cu in CourceEditorVM.CourceUnitCollection)
            {
                cu.Segment = i;
                cu.DisplayGrade = cu.Grade * 100;

                cu.MinGrade = -24;
                cu.MaxGrade = 24;
                cu.GradeUnit = "%";
                AccumDis += cu.Length;
                cu.DisplayGrade = cu.Grade * 100.0;

                // I think this is OK
                if (MetricSelected == true)
                {
                    cu.MaxLength = 3218;
                    cu.MinLength = 1;
                    cu.MinMaxLengthUnit = "Meter";

                    cu.MinWind = -100;
                    cu.MaxWind = 100;
                    cu.WindUnit = "Km";

                    if (AccumDis < KiloMeter)
                    {
                        cu.AccumulatorUnit = "Meter";
                        cu.AccumDistGrade = AccumDis;

                    }
                    else
                    {
                        cu.AccumulatorUnit = "Km";
                        cu.AccumDistGrade = AccumDis / KiloMeter;
                    }

                    cu.LengthUnit = "Meter";
                    cu.DisplayLength = cu.Length;
                }
                else
                {
                    cu.MinWind = -62.1;
                    cu.MaxWind = 62.1;
                    cu.WindUnit = "Miles";

                    cu.MaxLength = 10560;
                    cu.MinLength = 3;
                    cu.LengthUnit = "Feet";
                    cu.MinMaxLengthUnit = "Feet";

                    double AllFeet = AccumDis * MeterToFeet;

                    if (AllFeet < (MileToFeet / 5.0))
                    {
                        cu.AccumulatorUnit = "Feet";
                        cu.AccumDistGrade = AccumDis * MeterToFeet;
                    }
                    else
                    {
                        cu.AccumulatorUnit = "Miles";
                        cu.AccumDistGrade = AccumDis * MeterToMile;
                    }

                    cu.LengthUnit = "Feet";
                    cu.DisplayLength = cu.Length * MeterToFeet;
                    
                }

                cu.StartGrade = previousEndValue;
                cu.EndGrade = previousEndValue + cu.Grade;

                previousEndValue = cu.EndGrade;

                if (MaxGrade < cu.Grade)
                    MaxGrade = cu.Grade;

                if (MinGrade > cu.Grade)
                    MinGrade = cu.Grade;

                if (MaxWind < cu.Wind)
                    MaxWind = cu.Wind;

                if (MinWind > cu.Wind)
                    MinWind = cu.Wind;

                AvargeGrade += cu.Grade;
                AvargeWind += cu.Wind;

                if (PolygonData.MaxValue < cu.StartGrade)
                    PolygonData.MaxValue = cu.StartGrade;
                if (PolygonData.MaxValue < cu.EndGrade)
                    PolygonData.MaxValue = cu.EndGrade;

                if (PolygonData.MinValue > cu.StartGrade)
                    PolygonData.MinValue = cu.StartGrade;
                if (PolygonData.MinValue > cu.EndGrade)
                    PolygonData.MinValue = cu.EndGrade;

                i++;
            }

            CourceEditorVM.OriginalMinimum = PolygonData.MinValue;
            CourceEditorVM.OriginalMaximum = PolygonData.MaxValue;

            if (PolygonData.MinValue < 0)
            {
                PolygonData.MaxValue += Math.Abs(PolygonData.MinValue);
                PolygonData.MinValue = 0;
            }

            CourceEditorVM.MinMaxGrade = string.Format("{0} / {1}", Ulilities.Round(MinGrade, 2), Ulilities.Round(MaxGrade, 2));
            CourceEditorVM.AvargeGrade = string.Format("{0}", Ulilities.Round(AvargeGrade / CourceEditorVM.CourceUnitCollection.Count(), 2));
            CourceEditorVM.MinMaxWind = string.Format("{0} / {1}", Ulilities.Round(MinWind, 2), Ulilities.Round(MaxWind, 2));
            CourceEditorVM.AvargeWind = string.Format("{0}", Ulilities.Round(AvargeWind, 2));


            CourseUnits lastCU = CourceEditorVM.CourceUnitCollection.Last();
            CourseUnits firstCU = CourceEditorVM.CourceUnitCollection[0];

            double MaximumHeight = Math.Abs(PolygonData.MinValue) + Math.Abs(PolygonData.MaxValue);


            foreach (CourseUnits cev in CourceEditorVM.CourceUnitCollection)
            {
                double SGrade = CourceEditorVM.OriginalMinimum < 0 ? cev.StartGrade + Math.Abs(CourceEditorVM.OriginalMinimum) : cev.StartGrade;
                double EGrade = CourceEditorVM.OriginalMinimum < 0 ? cev.EndGrade + Math.Abs(CourceEditorVM.OriginalMinimum) : cev.EndGrade;
                cev.PolygonDataValue = new PolygonData(SGrade, EGrade);
            }

        }


        void LoadTimeGrade(XDocument xdoc)
        {
            var valFirst = xdoc.Descendants("Val").First();
            string strVal = valFirst.ToString();
          
            bool hasMinutes = strVal.Contains("Minutes");
            bool hasLength = strVal.Contains("Length");
            bool hasGrade = strVal.Contains("Grade");
            bool hasWind = strVal.Contains("Wind");

            if (hasMinutes == false || hasLength == false || hasGrade == false)
            {
                MessageBox.Show("It is not Time Grade File");
                return;
            }

            if (hasWind == true)
            {
                var Vals = from r in xdoc.Descendants("Val")
                           select new
                           {
                               Minutes = r.Element("Minutes").Value,
                               Length = r.Element("Length").Value,
                               Grade = r.Element("Grade").Value,
                               Wind = r.Element("Wind").Value
                           };

                foreach (var r in Vals)
                {
                    CourseUnits temp = new CourseUnits();
                    temp.Grade = double.Parse(r.Grade);
                    temp.Time = double.Parse(r.Minutes);
                    temp.Wind = double.Parse(r.Wind);
                    temp.CourseMode = CourseType.TIMEGRADE;
                    temp.DisplayGrade = temp.Grade * 100.0;
                   
                    CourceEditorVM.CourceUnitCollection.Add(temp);

                }
            }
            else
            {
                var Vals = from r in xdoc.Descendants("Val")
                           select new
                           {
                               Minutes = r.Element("Minutes").Value,
                               Length = r.Element("Length").Value,
                               Grade = r.Element("Grade").Value,
                           };

                foreach (var r in Vals)
                {
                    CourseUnits temp = new CourseUnits();
                    temp.Grade = double.Parse(r.Grade);
                    temp.Time = double.Parse(r.Minutes); 
                    temp.Wind = 0;
                    temp.CourseMode = CourseType.TIMEGRADE;
                    temp.DisplayGrade = temp.Grade * 100.0;
                    
                    CourceEditorVM.CourceUnitCollection.Add(temp);
                }
            }
            TimeGradeUpdate();
        }

        void TimeGradeUpdate()
        {
            PolygonData.MaxValue = double.NegativeInfinity;
            PolygonData.MinValue = double.PositiveInfinity;

            double AccumTime = 0.0;
            double previousEndValue = 0.0;
            int i = 1;

            double MaxGrade = double.NegativeInfinity;
            double MinGrade = double.PositiveInfinity;

            double MaxTime = double.NegativeInfinity;
            double MinTime = double.PositiveInfinity;
            double AvgTime = 0;
            double AvargeGrade = 0.0;

            foreach (CourseUnits cu in CourceEditorVM.CourceUnitCollection)
            {
                
                cu.Segment = i;
                AccumTime += cu.Time;
                cu.AccumTimeGrade = AccumTime;
                cu.DisplayGrade = cu.Grade * 100;

                cu.StartGrade = previousEndValue;
                cu.EndGrade = previousEndValue + cu.Grade;

                previousEndValue = cu.EndGrade;

                cu.MinGrade = -24;
                cu.MaxGrade = 24;
                cu.GradeUnit = "%";

                cu.MaxTime = 10;
                cu.MinTime = 0.1;
                cu.TimeUnit = "Min";
                cu.MaxTimeUnit = "Min";

                if (MetricSelected == true)
                {
                    cu.WindUnit = "Km/h";
                    cu.MinWind = -100;
                    cu.MaxWind = 100;
                }
                else
                {
                    cu.WindUnit = "Mp/h";
                    cu.MinWind = -100.0 * KmToMile;
                    cu.MaxWind = 100.0 * KmToMile;
                }

                if (MaxGrade < cu.Grade)
                    MaxGrade = cu.Grade;

                if (MinGrade > cu.Grade)
                    MinGrade = cu.Grade;

                AvargeGrade += cu.Grade;
                AvgTime += cu.Time;

                if (MaxTime < cu.Time)
                    MaxTime = cu.Time;

                if (MinTime > cu.Time)
                    MinTime = cu.Time;


                if (PolygonData.MaxValue < cu.StartGrade)
                    PolygonData.MaxValue = cu.StartGrade;
                if (PolygonData.MaxValue < cu.EndGrade)
                    PolygonData.MaxValue = cu.EndGrade;

                if (PolygonData.MinValue > cu.StartGrade)
                    PolygonData.MinValue = cu.StartGrade;
                if (PolygonData.MinValue > cu.EndGrade)
                    PolygonData.MinValue = cu.EndGrade;

                i++;
            }

            CourceEditorVM.OriginalMinimum = PolygonData.MinValue;
            CourceEditorVM.OriginalMaximum = PolygonData.MaxValue;

            if (PolygonData.MinValue < 0)
            {
                PolygonData.MaxValue += Math.Abs(PolygonData.MinValue);
                PolygonData.MinValue = 0;
            }

            CourceEditorVM.MinMaxGrade = string.Format("{0} / {1}", Ulilities.Round(MinGrade, 2), Ulilities.Round(MaxGrade, 2));
            CourceEditorVM.AvargeGrade = string.Format("{0}", Ulilities.Round(AvargeGrade / CourceEditorVM.CourceUnitCollection.Count(), 2));
            CourceEditorVM.MinMaxTime = string.Format("{0} / {1}", Ulilities.Round(MinTime, 2), Ulilities.Round(MaxTime, 2));
            CourceEditorVM.AvargeTime = string.Format("{0}", Ulilities.Round(AvgTime / CourceEditorVM.CourceUnitCollection.Count(), 2));


            foreach (CourseUnits cev in CourceEditorVM.CourceUnitCollection)
            {
                double SGrade = cev.StartGrade + Math.Abs(CourceEditorVM.OriginalMinimum);
                double EGrade = cev.EndGrade + Math.Abs(CourceEditorVM.OriginalMinimum);
                cev.PolygonDataValue = new PolygonData(SGrade, EGrade);
            }

        }

        void LoadTimeWatt(XDocument xdoc)
        {

            var valFirst = xdoc.Descendants("Val").First();
            string strVal = valFirst.ToString();
            bool hasMinutes = strVal.Contains("Minutes");
            bool hasStartWatts = strVal.Contains("StartWatts");
            bool hasEndWatts = strVal.Contains("EndWatts");

            if (hasMinutes == false || hasStartWatts == false || hasEndWatts == false)
            {
                MessageBox.Show("It is not Time Watt File");
                return;
            }

            var Vals = from r in xdoc.Descendants("Val")
                       select new
                       {
                           Minutes = r.Element("Minutes").Value,
                           StartWatts = r.Element("StartWatts").Value,
                           EndWatts = r.Element("EndWatts").Value
                       };
           
            foreach (var r in Vals)
            {
                CourseUnits temp = new CourseUnits();
                temp.Time = double.Parse(r.Minutes);
                temp.StartWatt = double.Parse(r.StartWatts);
                temp.EndWatt = double.Parse(r.EndWatts);
                temp.CourseMode = CourseType.TIMEWATT;
                CourceEditorVM.CourceUnitCollection.Add(temp);
            }

            TimeWattsUpdate();
        }

        void TimeWattsUpdate()
        {
            PolygonData.MaxValue = double.NegativeInfinity;
            PolygonData.MinValue = double.PositiveInfinity;

            int i = 1;
            double AccumTime = 0.0;
            double MaxWatts = double.NegativeInfinity;
            double MinWatts = double.PositiveInfinity;

            double MaxTime = double.NegativeInfinity;
            double MinTime = double.PositiveInfinity;

            double AvgWatts = 0;
            double AvgTime = 0;


            foreach (CourseUnits cu in CourceEditorVM.CourceUnitCollection)
            {
                cu.Segment = i;
                AccumTime += cu.Time;
                cu.AccumTimeWatt = AccumTime;

                cu.MaxTime = 10;
                cu.MinTime = 0.1; 
                
                cu.TimeUnit = "Min";
                cu.MinWatt = 1;
                cu.MaxWatt = 2000;
                cu.WattUnit = "Watt";

                if (MaxWatts < cu.StartWatt)
                    MaxWatts = cu.StartWatt;
                if (MaxWatts < cu.EndWatt)
                    MaxWatts = cu.EndWatt;

                if (MinWatts > cu.StartWatt)
                    MinWatts = cu.StartWatt;

                if (MaxWatts > cu.EndWatt)
                    MaxWatts = cu.EndWatt;

                if (MaxTime < cu.Time)
                    MaxTime = cu.Time;

                if (MinTime > cu.Time)
                    MinTime = cu.Time;


                if (PolygonData.MaxValue < cu.StartWatt)
                    PolygonData.MaxValue = cu.StartWatt;
                if (PolygonData.MaxValue < cu.EndWatt)
                    PolygonData.MaxValue = cu.EndWatt;

                if (PolygonData.MinValue > cu.StartWatt)
                    PolygonData.MinValue = cu.StartWatt;
                if (PolygonData.MinValue > cu.EndWatt)
                    PolygonData.MinValue = cu.EndWatt;

                AvgWatts += (cu.StartWatt + cu.EndWatt) / 2;
                AvgTime += cu.Time;
               
                i++;
            }

            CourceEditorVM.OriginalMinimum = PolygonData.MinValue;
            CourceEditorVM.OriginalMaximum = PolygonData.MaxValue;

            if (PolygonData.MinValue < 0)
            {
                PolygonData.MaxValue += Math.Abs(PolygonData.MinValue);
                PolygonData.MinValue = 0;
            }
           

            CourceEditorVM.MinMaxWatts = string.Format("{0} / {1}", Ulilities.Round(MinWatts, 2), Ulilities.Round(MaxWatts, 2));
            CourceEditorVM.AvargeWatts = string.Format("{0}", Ulilities.Round(AvgWatts / CourceEditorVM.CourceUnitCollection.Count(), 2));
            CourceEditorVM.MinMaxTime = string.Format("{0} / {1}", Ulilities.Round(MinTime, 2), Ulilities.Round(MaxTime, 2));
            CourceEditorVM.AvargeTime = string.Format("{0}", Ulilities.Round(AvgTime / CourceEditorVM.CourceUnitCollection.Count(), 2));

            foreach (CourseUnits cev in CourceEditorVM.CourceUnitCollection)
            {
                double SGrade = CourceEditorVM.OriginalMinimum < 0 ? cev.StartWatt + Math.Abs(CourceEditorVM.OriginalMinimum) : cev.StartWatt;
                double EGrade = CourceEditorVM.OriginalMinimum < 0 ? cev.EndWatt + Math.Abs(CourceEditorVM.OriginalMinimum) : cev.EndWatt;
                cev.PolygonDataValue = new PolygonData(SGrade, EGrade);
            }


            CourseUnits lastCU = CourceEditorVM.CourceUnitCollection.Last();
            CourceEditorVM.Info.Length = lastCU.AccumTimeWatt.ToString();

        }


        void LoadDistanceWatt(XDocument xdoc)
        {
            var valFirst = xdoc.Descendants("Val").First();
            string strVal = valFirst.ToString();
            bool hasLength = strVal.Contains("Length");
            bool hasStartWatts = strVal.Contains("StartWatts");
            bool hasEndWatts = strVal.Contains("EndWatts");

            if (hasLength == false || hasStartWatts == false || hasEndWatts == false)
            {
                MessageBox.Show("It is not Distance Watt File");
                return;
            }

            var Vals = from r in xdoc.Descendants("Val")
                       select new
                       {
                           Length = r.Element("Length").Value,
                           StartWatts = r.Element("StartWatts").Value,
                           EndWatts = r.Element("EndWatts").Value
                       };

          
            foreach (var r in Vals)
            {
                CourseUnits temp = new CourseUnits();
                temp.Length = double.Parse(r.Length);
                temp.StartWatt = double.Parse(r.StartWatts);
                temp.EndWatt = double.Parse(r.EndWatts);
                temp.CourseMode = CourseType.DISTANCEWATT;
                CourceEditorVM.CourceUnitCollection.Add(temp);
            }

            DistanceWattUpdate();

        }

        void DistanceWattUpdate()
        {

            PolygonData.MaxValue = double.NegativeInfinity;
            PolygonData.MinValue = double.PositiveInfinity;
            double MaxDistance = double.NegativeInfinity;
            double MinDistance = double.PositiveInfinity;

            double MaxWatts = double.NegativeInfinity;
            double MinWatts = double.PositiveInfinity;

            int i = 1;
            double AccumDis = 0.0;
            double AvgWatt = 0.0;


            foreach (CourseUnits cu in CourceEditorVM.CourceUnitCollection)
            {
                cu.Segment = i;
                AccumDis += cu.Length;

                cu.MinWatt = 1;
                cu.MaxWatt = 2000;
                cu.WattUnit = "Watt";

                    // I think this is OK
                    if (MetricSelected == true)
                    {
                        cu.MaxLength = 3218;
                        cu.MinLength = 1;
                        cu.MinMaxLengthUnit = "Meter";

                        cu.MinWind = -100;
                        cu.MaxWind = 100;
                        cu.WindUnit = "Km";

                        if (AccumDis < KiloMeter)
                        {
                            cu.AccumulatorUnit = "Meter";
                            cu.AccumDistWatt = AccumDis;

                        }
                        else
                        {
                            cu.AccumulatorUnit = "Km";
                            cu.AccumDistWatt = AccumDis / KiloMeter;
                        }

                        cu.LengthUnit = "Meter";
                        cu.DisplayLength = cu.Length;

                    }
                    else
                    {
                        cu.MinWind = -62.1;
                        cu.MaxWind = 62.1;
                        cu.WindUnit = "Miles";

                        cu.MaxLength = 10560;
                        cu.MinLength = 3;

                        cu.LengthUnit = "Feet";
                        cu.MinMaxLengthUnit = "Feet";

                        if (AccumDis < (MileToFeet / 5.0))
                        {
                            cu.AccumulatorUnit = "Feet";
                            cu.AccumDistWatt = AccumDis * MeterToFeet;

                        }
                        else
                        {
                            cu.AccumulatorUnit = "Miles";
                            cu.AccumDistWatt = AccumDis * MeterToMile;
                        }

                        cu.LengthUnit = "Feet";
                        cu.DisplayLength = cu.Length * MeterToFeet;
                    }
                


                if (PolygonData.MaxValue < cu.StartWatt)
                    PolygonData.MaxValue = cu.StartWatt;
                if (PolygonData.MaxValue < cu.EndWatt)
                    PolygonData.MaxValue = cu.EndWatt;

                if (PolygonData.MinValue > cu.StartWatt)
                    PolygonData.MinValue = cu.StartWatt;
                if (PolygonData.MinValue > cu.EndWatt)
                    PolygonData.MinValue = cu.EndWatt;


                if (MaxDistance < cu.Length)
                    MaxDistance = cu.Length;

                if (MinDistance > cu.Length)
                    MinDistance = cu.Length;

                if (MaxWatts < cu.StartWatt)
                    MaxWatts = cu.StartWatt;
                if (MaxWatts < cu.EndWatt)
                    MaxWatts = cu.EndWatt;

                if (MinWatts > cu.StartWatt)
                    MinWatts = cu.StartWatt;
                if (MinWatts > cu.EndWatt)
                    MinWatts = cu.EndWatt;

                AvgWatt += (cu.StartWatt + cu.EndWatt) / 2;


                i++;
            }


            CourceEditorVM.OriginalMinimum = PolygonData.MinValue;
            CourceEditorVM.OriginalMaximum = PolygonData.MaxValue;

            if (PolygonData.MinValue < 0)
            {
                PolygonData.MaxValue += Math.Abs(PolygonData.MinValue);
                PolygonData.MinValue = 0;
            }

            CourceEditorVM.MinMaxWatts = string.Format("{0} / {1}", Ulilities.Round(MinWatts, 2), Ulilities.Round(MaxWatts, 2));
            CourceEditorVM.AvargeWatts = string.Format("{0}", Ulilities.Round(AvgWatt / CourceEditorVM.CourceUnitCollection.Count(), 2));


            if (MetricSelected == true)
            {
                if (MaxDistance <= KiloMeter)
                {
                    CourceEditorVM.MinMaxDist = string.Format("{0} / {1} M", Ulilities.Round(MinDistance, 2), Ulilities.Round(MaxDistance, 2));
                    CourceEditorVM.AvargeDist = string.Format("{0} M", Ulilities.Round(AccumDis / CourceEditorVM.CourceUnitCollection.Count(), 2));
                }
                else
                {
                    CourceEditorVM.MinMaxDist = string.Format("{0} / {1} KM", Ulilities.Round(MinDistance / KiloMeter, 2), Ulilities.Round(MaxDistance / KiloMeter, 2));
                    CourceEditorVM.AvargeDist = string.Format("{0} M", Ulilities.Round(AccumDis / KiloMeter / CourceEditorVM.CourceUnitCollection.Count(), 2));
                }

            }
            else
            {

                if (MaxDistance <= KiloMeter * MeterToFeet)
                {
                    CourceEditorVM.MinMaxDist = string.Format("{0} / {1} M", Ulilities.Round(MinDistance / MeterToFeet, 2), Ulilities.Round(MaxDistance / MeterToFeet, 2));
                    CourceEditorVM.AvargeDist = string.Format("{0} M", Ulilities.Round(AccumDis / MeterToFeet / CourceEditorVM.CourceUnitCollection.Count(), 2));
                }
                else
                {
                    CourceEditorVM.MinMaxDist = string.Format("{0} / {1} KM", Ulilities.Round(MinDistance / KiloMeter / MeterToFeet, 2), Ulilities.Round(MaxDistance / KiloMeter / MeterToFeet, 2));
                    CourceEditorVM.AvargeDist = string.Format("{0} M", Ulilities.Round(AccumDis / KiloMeter / MeterToFeet / CourceEditorVM.CourceUnitCollection.Count(), 2));
                }

            }



            foreach (CourseUnits cev in CourceEditorVM.CourceUnitCollection)
            {
                double SGrade = CourceEditorVM.OriginalMinimum < 0 ? cev.StartWatt + Math.Abs(CourceEditorVM.OriginalMinimum) : cev.StartWatt;
                double EGrade = CourceEditorVM.OriginalMinimum < 0 ? cev.EndWatt + Math.Abs(CourceEditorVM.OriginalMinimum) : cev.EndWatt;
                cev.PolygonDataValue = new PolygonData(SGrade, EGrade);
            }


            CourseUnits lastCU = CourceEditorVM.CourceUnitCollection.Last();
            CourceEditorVM.Info.Length = lastCU.AccumDistWatt.ToString();
        }


        void LoadDistanceAt(XDocument xdoc)
        {
            var valFirst = xdoc.Descendants("Val").First();
            string strVal = valFirst.ToString();
            bool hasLength = strVal.Contains("Length");
            bool hasStartAt = strVal.Contains("StartAt");
            bool hasEndAt = strVal.Contains("EndAt");

            if (hasLength == false || hasStartAt == false || hasEndAt == false)
            {
                MessageBox.Show("It is not Distance AT% File");
                return;
            }

            var Vals = from r in xdoc.Descendants("Val")
                       select new
                       {
                           Length = r.Element("Length").Value,
                           StartAt = r.Element("StartAt").Value,
                           EndAt = r.Element("EndAt").Value
                       };

            foreach (var r in Vals)
            {
                CourseUnits temp = new CourseUnits();
                temp.Length = double.Parse(r.Length);
                temp.StartAT = double.Parse(r.StartAt);
                temp.EndAT = double.Parse(r.EndAt);
                temp.CourseMode = CourseType.DISTANCEPERAT;

                CourceEditorVM.CourceUnitCollection.Add(temp);
            }

            DistanceAtUpdate();
     
        }

        void DistanceAtUpdate()
        {

            PolygonData.MaxValue = double.NegativeInfinity;
            PolygonData.MinValue = double.PositiveInfinity;

            double MaxWatts = double.NegativeInfinity;
            double MinWatts = double.PositiveInfinity;
            double AvargeWatt = 0.0;

            double MaxLength = double.NegativeInfinity;
            double MinLength = double.PositiveInfinity;
            double AvargeLength = 0.0;


            int i = 1;
            double AccumDis = 0.0;
            foreach (CourseUnits cu in CourceEditorVM.CourceUnitCollection)
            {
                cu.Segment = i;

                AccumDis += cu.Length;
                cu.AccumDistAT = AccumDis;

                cu.MaxAT = 200;
                cu.MinAT = -100;
                cu.ATUnit = "%";

                cu.MinLength = .005;
                cu.MaxLength = 100;
                cu.MinMaxLengthUnit = "km";

                // I think this is OK
                if (MetricSelected == true)
                {
                    cu.MaxLength = 3218;
                    cu.MinLength = 1;
                    cu.MinMaxLengthUnit = "Meter";

                    cu.MinWind = -100;
                    cu.MaxWind = 100;
                    cu.WindUnit = "Km";

                    if (AccumDis < KiloMeter)
                    {
                        cu.AccumulatorUnit = "Meter";
                        cu.AccumDistAT = AccumDis;

                    }
                    else
                    {
                        cu.AccumulatorUnit = "Km";
                        cu.AccumDistAT = AccumDis / KiloMeter;
                    }

                    cu.LengthUnit = "Meter";
                    cu.DisplayLength = cu.Length;
                }
                else
                {
                    cu.MinWind = -62.1;
                    cu.MaxWind = 62.1;
                    cu.WindUnit = "Miles";

                    cu.MaxLength = 10560;
                    cu.MinLength = 3;

                    cu.LengthUnit = "Feet";
                    cu.MinMaxLengthUnit = "Feet";

                    if (AccumDis < (MileToFeet / 5.0))
                    {
                        cu.AccumulatorUnit = "Feet";
                        cu.AccumDistAT = AccumDis * MeterToFeet;

                    }
                    else
                    {
                        cu.AccumulatorUnit = "Miles";
                        cu.AccumDistAT = AccumDis * MeterToMile;
                    }

                    cu.LengthUnit = "Feet";
                    cu.DisplayLength = cu.Length * MeterToFeet;
                    

                }

                AvargeLength += cu.Length;

                if (MaxLength < cu.Length)
                    MaxLength = cu.Length;

                if (MinLength > cu.Length)
                    MinLength = cu.Length;

                if (MaxWatts < cu.StartAT)
                    MaxWatts = cu.StartAT;
                if (MaxWatts < cu.EndAT)
                    MaxWatts = cu.EndAT;

                if (MinWatts > cu.StartAT)
                    MinWatts = cu.StartAT;
                if (MinWatts > cu.EndAT)
                    MinWatts = cu.EndAT;

                AvargeWatt += (cu.StartAT + cu.EndAT) / 2;

                if (PolygonData.MaxValue < cu.StartAT)
                    PolygonData.MaxValue = cu.StartAT;
                if (PolygonData.MaxValue < cu.EndAT)
                    PolygonData.MaxValue = cu.EndAT;

                if (PolygonData.MinValue > cu.StartAT)
                    PolygonData.MinValue = cu.StartAT;
                if (PolygonData.MinValue > cu.EndAT)
                    PolygonData.MinValue = cu.EndAT;

                i++;
            }

            CourceEditorVM.OriginalMinimum = PolygonData.MinValue;
            CourceEditorVM.OriginalMaximum = PolygonData.MaxValue;

            if (PolygonData.MinValue < 0)
            {
                PolygonData.MaxValue += Math.Abs(PolygonData.MinValue);
                PolygonData.MinValue = 0;
            }

            CourseUnits lastCU = CourceEditorVM.CourceUnitCollection.Last();

            CourceEditorVM.MinMaxWatts = string.Format("{0} / {1}", Ulilities.Round(MinWatts, 2), Ulilities.Round(MaxWatts, 2));
            CourceEditorVM.AvargeWatts = string.Format("{0}", Ulilities.Round(AvargeWatt / CourceEditorVM.CourceUnitCollection.Count(), 2));
            CourceEditorVM.MinMaxDist = string.Format("{0} / {1}", Ulilities.Round(MaxLength, 2), Ulilities.Round(MinLength, 2));
            CourceEditorVM.AvargeDist = string.Format("{0}", Ulilities.Round(AvargeLength / CourceEditorVM.CourceUnitCollection.Count(), 2));

            foreach (CourseUnits cev in CourceEditorVM.CourceUnitCollection)
            {
                double SGrade = CourceEditorVM.OriginalMinimum < 0 ? cev.StartAT + Math.Abs(CourceEditorVM.OriginalMinimum) : cev.StartAT;
                double EGrade = CourceEditorVM.OriginalMinimum < 0 ? cev.EndAT + Math.Abs(CourceEditorVM.OriginalMinimum) : cev.EndAT;
                cev.PolygonDataValue = new PolygonData(SGrade, EGrade);
            }

            CourceEditorVM.Info.Length = lastCU.AccumDistAT.ToString();
        }

        void LoadTimeAt(XDocument xdoc)
        {
            var valFirst = xdoc.Descendants("Val").First();
            string strVal = valFirst.ToString();
            bool hasMinutes = strVal.Contains("Minutes");
            bool hasStartAt = strVal.Contains("StartAt");
            bool hasEndAt = strVal.Contains("EndAt");

            if (hasMinutes == false || hasStartAt == false || hasEndAt == false)
            {
                MessageBox.Show("It is not Time AT% File");
                return;
            }

            var Vals = from r in xdoc.Descendants("Val")
                       select new
                       {
                           Minutes = r.Element("Minutes").Value,
                           StartAt = r.Element("StartAt").Value,
                           EndAt = r.Element("EndAt").Value
                       };

         
            foreach (var r in Vals)
            {
                CourseUnits temp = new CourseUnits();
                temp.Time = double.Parse(r.Minutes);
                temp.StartAT = double.Parse(r.StartAt);
                temp.EndAT = double.Parse(r.EndAt);
                temp.CourseMode = CourseType.TIMEPERAT;

                CourceEditorVM.CourceUnitCollection.Add(temp);
            }

            TimeAtUpdate();
         
        }

        void TimeAtUpdate()
        {
            PolygonData.MaxValue = double.NegativeInfinity;
            PolygonData.MinValue = double.PositiveInfinity;

            double MaxWatts = double.NegativeInfinity;
            double MinWatts = double.PositiveInfinity;

            double MaxTime = double.NegativeInfinity;
            double MinTime = double.PositiveInfinity;

            double AvargeWatt = 0.0;

            int i = 1;
            double AccumTime = 0.0;
            foreach (CourseUnits cu in CourceEditorVM.CourceUnitCollection)
            {
                cu.Segment = i;

                cu.MaxAT = 200;
                cu.MinAT = -100;
                cu.ATUnit = "%";
                AccumTime += cu.Time;
                cu.AccumTimeAT = AccumTime;

                cu.MaxTime = 10;
                cu.MinTime = 0.1;

                cu.TimeUnit = "Min";


                if (MaxTime < cu.Time)
                    MaxTime = cu.Time;


                if (MinTime > cu.Time)
                    MinTime = cu.Time;

                if (MaxWatts < cu.StartAT)
                    MaxWatts = cu.StartAT;
                if (MaxWatts < cu.EndAT)
                    MaxWatts = cu.EndAT;

                if (MinWatts > cu.StartAT)
                    MinWatts = cu.StartAT;
                if (MinWatts > cu.EndAT)
                    MinWatts = cu.EndAT;

                AvargeWatt += (cu.StartAT + cu.EndAT) / 2;

                if (PolygonData.MaxValue < cu.StartAT)
                    PolygonData.MaxValue = cu.StartAT;
                if (PolygonData.MaxValue < cu.EndAT)
                    PolygonData.MaxValue = cu.EndAT;

                if (PolygonData.MinValue > cu.StartAT)
                    PolygonData.MinValue = cu.StartAT;
                if (PolygonData.MinValue > cu.EndAT)
                    PolygonData.MinValue = cu.EndAT;

                i++;
            }

            CourceEditorVM.OriginalMinimum = PolygonData.MinValue;
            CourceEditorVM.OriginalMaximum = PolygonData.MaxValue;

            CourseUnits lastCU = CourceEditorVM.CourceUnitCollection.Last();

            CourceEditorVM.MinMaxWatts = string.Format("{0} / {1}", Ulilities.Round(MinWatts, 2), Ulilities.Round(MaxWatts, 2));
            CourceEditorVM.AvargeWatts = string.Format("{0}", Ulilities.Round(AvargeWatt / CourceEditorVM.CourceUnitCollection.Count(), 2));
            CourceEditorVM.MinMaxTime = string.Format("{0} / {1}", Ulilities.Round(MinTime, 2), Ulilities.Round(MaxTime, 2));
            CourceEditorVM.AvargeTime = string.Format("{0}", Ulilities.Round(lastCU.AccumTimeAT / CourceEditorVM.CourceUnitCollection.Count(), 2));

            foreach (CourseUnits cev in CourceEditorVM.CourceUnitCollection)
            {
                double SGrade = CourceEditorVM.OriginalMinimum < 0 ? cev.StartAT + Math.Abs(CourceEditorVM.OriginalMinimum) : cev.StartAT;
                double EGrade = CourceEditorVM.OriginalMinimum < 0 ? cev.EndAT + Math.Abs(CourceEditorVM.OriginalMinimum) : cev.EndAT;
                cev.PolygonDataValue = new PolygonData(SGrade, EGrade);
            }

            CourceEditorVM.Info.Length = lastCU.AccumTimeAT.ToString();

        }

        void CopyListItems()
        {
           
            int count = ListViewSegments.SelectedItems.Count;
            if (count > 0)
            {
                int index = ListViewSegments.SelectedIndex;

                SelectedColection = new List<CourseUnits>();
                foreach (var item in ListViewSegments.SelectedItems)
                {
                    SelectedColection.Add((CourseUnits)item);
                }

                CourseEditorViewModel CourceEditorVM = (CourseEditorViewModel)DataContext;
                if (CourceEditorVM != null)
                {
                    CourceEditorVM.MemoryCanvas = Visibility.Visible;
                }

                ListViewSegments.SelectedItems.Clear();
                ListViewSegments.SelectedIndex = index;
            }
        }



        private void InsertButton_Click(object sender, RoutedEventArgs e) 
        {
            if (CourceEditorVM == null)
                return;

            PushVM();
            CourceEditorVM.Saved = false;
            int index = HorizontalList.SelectedIndex;

            CourceEditorVM.CourceUnitCollection.Insert(index, CourceEditorVM.CourceUnitCollection[index].Copy());
            Update();

        }

        private void CopyButton_Click(object sender, RoutedEventArgs e) 
        {
            CopyListItems();
        }

        private void OnAddCopyListItemExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CopyListItems();
        }

        void PasteListItems()
        {
            if (CourceEditorVM == null)
                return;
            PushVM();

            int index = HorizontalList.SelectedIndex;
            bool bCopyBeforeChk = (bool)CopyBefore.IsChecked;
            if (bCopyBeforeChk == false && index != CourceEditorVM.CourceUnitCollection.Count)
                index++;
            int nextIndex = index;

            if (SelectedColection.Count != 0 && CourceEditorVM != null)
            {
                foreach (CourseUnits cev in SelectedColection)
                {
                    CourceEditorVM.CourceUnitCollection.Insert(nextIndex, cev.Copy());
                    nextIndex++;
                }
                Update();
            }

           CourceEditorVM.Saved = false;

        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            PasteListItems();
        }   

        private void OnAddPasteListItemExecute(object sender, ExecutedRoutedEventArgs e)
        {
            PasteListItems();
        }

        void DeleteListItems()
        {

            if (CourceEditorVM == null)
                return;
            int Index = HorizontalList.SelectedIndex;

            PushVM();

            int count = ListViewSegments.SelectedItems.Count;
            if (count != 0 && CourceEditorVM != null)
            {
                List<CourseUnits> temp = new List<CourseUnits>();
                foreach (CourseUnits cev in ListViewSegments.SelectedItems)
                {
                    temp.Add(cev);
                }

                foreach (CourseUnits cev in temp)
                {
                    CourceEditorVM.CourceUnitCollection.Remove(cev);
                }
                Update();
                if (CourceEditorVM.CourceUnitCollection.Count != 0)
                {
                    ListViewSegments.SelectedIndex = 0;
                }
                ListViewSegments.SelectedItems.Clear();
                temp = null;
            }

            int newCount = ListViewSegments.Items.Count;
            if (newCount != 0)
            {
                HorizontalList.SelectedIndex = (Index < newCount) ?  Index : 0;
            }
            CourceEditorVM.Saved = false;

        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteListItems();
        }

        private void OnAddDeleteListItemExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteListItems();
        }

        private void Update()
        {
            if (CourceEditorVM == null)
                return;

            switch (CourceEditorVM.CurrCourseType)
            {
                case CourseType.DISTANCEGRADE:
                    DistanceGradeUpdate();
                    break;
                case CourseType.TIMEGRADE:
                    TimeGradeUpdate();
                    break;
                case CourseType.TIMEWATT:
                    TimeWattsUpdate(); 
                    break;
                case CourseType.DISTANCEWATT:
                    DistanceWattUpdate();
                    break;
                case CourseType.TIMEPERAT:
                    TimeAtUpdate(); 
                    break;
                case CourseType.DISTANCEPERAT:
                    DistanceAtUpdate(); 
                    break;
            }

            DisplySegments.Text = string.Format("Segments: {0}", CourceEditorVM.CourceUnitCollection.Count);

            CurrentDistance();
        }

      
        private void MemoryButtonClick(object sender, RoutedEventArgs e)
        {
            ClearSegmentInMemory();
        }

        public string CalculateCourseHash()
        {
            HashOutStream inhash = new HashOutStream();
            int i = 0;
            foreach (CourseUnits cu in CourceEditorVM.CourceUnitCollection)
            {
                inhash.Insert(i.ToString());
                inhash.Insert(cu.getHashString());
            }
            inhash.CloseHashOut();
            return inhash.GetHashOut();
        }

        // please see 1442 of Course.cs
        public string CalculateHeaderHash()
        {
            HeaderHash = HashOutStream.ComputeHash(String.Format("{0},{1:F3},{2:F3},{3},{4}",
                        CourseHash, CourceEditorVM.Info.StartAt, CourceEditorVM.Info.EndAt, (byte)CourceEditorVM.Attributes, 1));
            return HeaderHash;
        }
        


        //=============================================================
        private void Options_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.RideOptions());
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (CourceEditorVM.Saved == false)
            {

                OkCancelDialog okCancelDialog = new OkCancelDialog(MessageNotSaved);
                okCancelDialog.Owner = Application.Current.MainWindow;
                bool result = (bool)okCancelDialog.ShowDialog();
                if (result == false)
                {
                    return;
                }
            }
            NavigationService.GoBack();
        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            AppWin.Help("Course_Creator.htm");
        }

        private void HorizontalList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewSegments.SelectedIndex != HorizontalList.SelectedIndex)
            {
                ListViewSegments.SelectedIndex = HorizontalList.SelectedIndex;
                ListBoxExtenders.OnAutoScrollToCurrentItem(ListViewSegments, ListViewSegments.SelectedIndex);
            }
            CourceEditorVM.SelectedIndex = HorizontalList.SelectedIndex;
        }


        private void ListViewSegments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewSegments.SelectedIndex != HorizontalList.SelectedIndex)
            {
                HorizontalList.SelectedIndex = ListViewSegments.SelectedIndex;
                ListBoxExtenders.OnAutoScrollToCurrentItem(HorizontalList, HorizontalList.SelectedIndex);
            }
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {

            //if (e.OriginalSource == null)// || ((GridViewColumnHeader)e.OriginalSource).Column == null)
            //    return;

            //if (e.OriginalSource is GridViewColumnHeader)
            //{
            //    return;
            //    GridViewColumn column = ((GridViewColumnHeader)e.OriginalSource).Column;
            //    GridView gridview = ListViewSegments.View as GridView;
            //    SortDescriptionCollection sorts = ListViewSegments.Items.SortDescriptions;
            //    RenderSort(sorts, column, GetSortDirection(sorts, column));
            //}

        }

        private void NewCourseBtnClick(object sender, RoutedEventArgs e)
        {
            if (CourceEditorVM.Saved == false)
            {
                OkCancelDialog okCancelDialog = new OkCancelDialog(MessageNotSaved);
                okCancelDialog.Owner = Application.Current.MainWindow;
                bool OkResult = (bool)okCancelDialog.ShowDialog();
                if (OkResult == false)
                {
                    return;
                }
            }
            CourseSelection courseSelection = new CourseSelection();
            courseSelection.Owner = Application.Current.MainWindow;
            bool result = (bool)courseSelection.ShowDialog();
            if (result == true)
            {
                ClearInfo();
                CourceEditorVM.CourceUnitCollection.Clear();
                //CourceEditorVM.Header = new RacerMateHeader("RacerMateOne", DateTime.Now.ToString(), "4.0.2", "(c) 2011, RacerMateOne, Inc.", string.Format("My own course"), "0");

                Mouse.OverrideCursor = Cursors.Wait;
                BackgroundWorker worker = new BackgroundWorker();

                switch (courseSelection.courseType)
                {
                    case CourseType.DISTANCEGRADE:
                        {

                            CourseUnits cu = new CourseUnits(1);
                            cu.CourseMode = CourseType.DISTANCEGRADE;
                            SetDefaults(cu);
                            cu.Grade = 0;
                            cu.Wind = 0;
                            CourceEditorVM.Info.XUnits = "Distance";
                            CourceEditorVM.Info.YUnits = "Grade";

                            cu.Length = CourceEditorVM.SelectMeters == true ? 1 : MeterToFeet;
                            
                            CourceEditorVM.Saved = false;
                            CourceEditorVM.CurrCourseType = CourseType.DISTANCEGRADE;

                            ListViewSegments.View = (GridView)FindResource("GV_DISTGRADE");
                            HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_DISTGRADE");

                            CourceEditorVM.CourceUnitCollection.Add(cu);
                            CourceEditorVM.CourseFileName = SavePath() + @"\Distance and Grade\NewFile";
                            Update();
                            worker.DoWork += (s, args) => DistanceGradeUpdate();
                        }
                        break;
                    case CourseType.TIMEGRADE:
                        {
                            CourseUnits cu = new CourseUnits(1);
                            cu.CourseMode = CourseType.TIMEGRADE;
                            SetDefaults(cu);
                            cu.Grade = 0;
                            cu.Time = 1;
                            cu.Wind = 0.0;
                            CourceEditorVM.Info.XUnits = "Time";
                            CourceEditorVM.Info.YUnits = "Grade";

                            
                            CourceEditorVM.Saved = false;
                            CourceEditorVM.CurrCourseType = CourseType.TIMEGRADE;

                            ListViewSegments.View = (GridView)FindResource("GV_TIMEGRADE");
                            HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_TIMEGRADE");

                            CourceEditorVM.CourceUnitCollection.Add(cu);
                            CourceEditorVM.CourseFileName = SavePath() + @"\Time and Grade\NewFile";

                            Update();
                            worker.DoWork += (s, args) => TimeGradeUpdate();
                        }
                        break;
                    case CourseType.TIMEWATT:
                        {
                            CourseUnits cu = new CourseUnits(1);
                            cu.CourseMode = CourseType.TIMEWATT;
                            SetDefaults(cu);
                            cu.Time = 1;
                            cu.StartWatt = 100;
                            cu.EndWatt = 100;
                            CourceEditorVM.Info.XUnits = "Time";
                            CourceEditorVM.Info.YUnits = "Watts";
                            
                            CourceEditorVM.Saved = false;
                            CourceEditorVM.CurrCourseType = CourseType.TIMEWATT;

                            ListViewSegments.View = (GridView)FindResource("GV_TIMEWATT");
                            HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_TIMEWATT");

                            CourceEditorVM.CourceUnitCollection.Add(cu);
                            CourceEditorVM.CourseFileName = SavePath() + @"\Time and Watts\NewFile";
                            Update();
                            worker.DoWork += (s, args) => TimeWattsUpdate();
                        }
                        break;
                    case CourseType.DISTANCEWATT:
                        {
                            CourseUnits cu = new CourseUnits(1);
                            cu.CourseMode = CourseType.DISTANCEWATT;
                            SetDefaults(cu);

                            cu.StartWatt = 100;
                            cu.EndWatt = 100;
                            cu.Length = CourceEditorVM.SelectMeters == true ? 1 : KiloMeter * MeterToMile;
                            CourceEditorVM.Info.XUnits = "Distance";
                            CourceEditorVM.Info.YUnits = "Watts";
                            
                            CourceEditorVM.Saved = false;
                            CourceEditorVM.CurrCourseType = CourseType.DISTANCEWATT;

                            ListViewSegments.View = (GridView)FindResource("GV_DISTWATT");
                            HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_DISTWATT");

                            CourceEditorVM.CourceUnitCollection.Add(cu);
                            CourceEditorVM.CourseFileName = SavePath() + @"\Distance and Watts\NewFile";
                            Update();
                            worker.DoWork += (s, args) => DistanceWattUpdate();
                        }
                        break;
                    case CourseType.TIMEPERAT:
                        {
                            CourseUnits cu = new CourseUnits(1);
                            cu.CourseMode = CourseType.TIMEPERAT;
                            SetDefaults(cu);
                            CourceEditorVM.Info.XUnits = "Time";
                            CourceEditorVM.Info.YUnits = "PercentAT";


                            cu.Time = 1;

                            CourceEditorVM.Saved = false;
                            CourceEditorVM.CurrCourseType = CourseType.TIMEPERAT;
                            ListViewSegments.View = (GridView)FindResource("GV_TIMEAT");
                            HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_TIMEAT");

                            CourceEditorVM.CourceUnitCollection.Add(cu);
                            CourceEditorVM.CourseFileName = SavePath() + @"\Time and %AT\NewFile";
                            Update();
                            worker.DoWork += (s, args) => TimeAtUpdate();
                        }
                        break;
                    case CourseType.DISTANCEPERAT:
                        {
                            CourseUnits cu = new CourseUnits(1);
                            cu.CourseMode = CourseType.DISTANCEPERAT;
                            SetDefaults(cu);

                            CourceEditorVM.Info.XUnits = "Distance";
                            CourceEditorVM.Info.YUnits = "PercentAT";


                            cu.Length = CourceEditorVM.SelectMeters == true ? 1 : KiloMeter * MeterToMile;

                            CourceEditorVM.Saved = false;
                            CourceEditorVM.CurrCourseType = CourseType.DISTANCEPERAT;
                            ListViewSegments.View = (GridView)FindResource("GV_DISTAT");
                            HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_DISTAT");

                            CourceEditorVM.CourceUnitCollection.Add(cu);
                            CourceEditorVM.CourseFileName = SavePath() + @"\Distance and %AT\NewFile";
                            Update();
                            worker.DoWork += (s, args) => DistanceAtUpdate();
                        }

                        break;
                }

                worker.RunWorkerCompleted += (s, args) =>
                {
                    switch (CourceEditorVM.CurrCourseType)
                    {
                        case CourseType.DISTANCEGRADE:
                            CourceTypeTB.Text = "Distance/Grade";
                            break;
                        case CourseType.DISTANCEPERAT:
                            CourceTypeTB.Text = "Distance/%AT";
                            break;
                        case CourseType.DISTANCEWATT:
                            CourceTypeTB.Text = "Distance/Watts";
                            break;
                        case CourseType.TIMEGRADE:
                            CourceTypeTB.Text = "Time/Grade";
                            break;
                        case CourseType.TIMEPERAT:
                            CourceTypeTB.Text = "Time/%AT";
                            break;
                        case CourseType.TIMEWATT:
                            CourceTypeTB.Text = "Time/Watts";
                            break;
                    }
                    
                    if (ListViewSegments.Items.Count > 0)
                    {
                        DisplySegments.Text = string.Format("Segments: {0}", CourceEditorVM.CourceUnitCollection.Count);
                        ListViewSegments.SelectedIndex = 0;
                    }
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
                    {

                        Mouse.OverrideCursor = null;
                        Application.Current.MainWindow.Cursor = null;
                        if (ListViewSegments.HasItems && ListViewSegments.SelectedIndex == -1)
                        {
                            ListViewSegments.SelectedIndex = 0;
                        }
                    }));
                    // Mouse.OverrideCursor = null;
                };

                worker.RunWorkerAsync();
            }

            //if (e.OriginalSource == null)// || ((GridViewColumnHeader)e.OriginalSource).Column == null)
            //    return;

            //if (e.OriginalSource is GridViewColumnHeader)
            //{
            //    return;
            //    GridViewColumn column = ((GridViewColumnHeader)e.OriginalSource).Column;
            //    GridView gridview = ListViewSegments.View as GridView;
            //    SortDescriptionCollection sorts = ListViewSegments.Items.SortDescriptions;
            //    RenderSort(sorts, column, GetSortDirection(sorts, column));
            //}

        }

      
        XElement AddLegs()
        {
            if (CourceEditorVM == null)
                return null;
            const int NoOfDecimal = 3;
           // double Length = 0.0;
            if (CourceEditorVM.CurrCourseType == CourseType.DISTANCEGRADE)
            {
                XElement root = new XElement("ThreeDType", new XAttribute("Count", CourceEditorVM.CourceUnitCollection.Count));
                for (int i = 0; i < CourceEditorVM.CourceUnitCollection.Count; i++)
                {
                    if (CourceEditorVM.Info.HasRotation)
                    {
                        int Divisions = CourceEditorVM.CourceUnitCollection[i].Divisions;
                        bool Smooth = CourceEditorVM.CourceUnitCollection[i].Smooth;
                        double Rotation = CourceEditorVM.CourceUnitCollection[i].Rotation;
                        XElement Val  = (Smooth == true) ? new XElement("Val", new XAttribute("Smooth", "true")) : new XElement("Val");
                        if (Divisions > 0)
                        {
                            Val.Add(new XElement("Divisions", Divisions));
                        }

                        Val.Add(new XElement("Length", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Length, NoOfDecimal)));
                        Val.Add(new XElement("Grade", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Grade, NoOfDecimal)));
                        Val.Add(new XElement("Wind", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Wind, NoOfDecimal)));
                        if (double.IsPositiveInfinity(Rotation) == false)
                        {
                            Val.Add(new XElement("Rotation", Rotation));
                        }

                        root.Add(Val);
                    }
                    else
                    {
                        root.Add(new XElement("Val", new XElement("Length", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Length, NoOfDecimal)), new XElement("Grade", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Grade, NoOfDecimal)), new XElement("Wind", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Wind, NoOfDecimal))));
                    }
                }
                return root;

            }
            else if (CourceEditorVM.CurrCourseType == CourseType.TIMEGRADE)
            {
                XElement root = new XElement("Segments", new XAttribute("Count", CourceEditorVM.CourceUnitCollection.Count));
                for (int i = 0; i < CourceEditorVM.CourceUnitCollection.Count; i++)
                {
                    root.Add(new XElement("Val", new XElement("Minutes", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Time, NoOfDecimal)), new XElement("Length", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Time * 60, NoOfDecimal)), new XElement("Grade", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Grade, NoOfDecimal)), new XElement("Wind", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Wind, NoOfDecimal))));
                }
                return root;
            }
            else if (CourceEditorVM.CurrCourseType == CourseType.TIMEWATT)
            {
                XElement root = new XElement("Segments", new XAttribute("Count", CourceEditorVM.CourceUnitCollection.Count));
                for (int i = 0; i < CourceEditorVM.CourceUnitCollection.Count; i++)
                {
                    root.Add(new XElement("Val", new XElement("Minutes", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Time, NoOfDecimal)), new XElement("StartWatts", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].StartWatt, NoOfDecimal)), new XElement("EndWatts", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].EndWatt, NoOfDecimal))));
                }
                return root;
            }
            else if (CourceEditorVM.CurrCourseType == CourseType.DISTANCEWATT)
            {
                XElement root = new XElement("Segments", new XAttribute("Count", CourceEditorVM.CourceUnitCollection.Count));
                for (int i = 0; i < CourceEditorVM.CourceUnitCollection.Count; i++)
                {
                    root.Add(new XElement("Val", new XElement("Length", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Length, NoOfDecimal)), new XElement("StartWatts", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].StartWatt, NoOfDecimal)), new XElement("EndWatts", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].EndWatt, NoOfDecimal))));
                }
                return root;
            }
            else if (CourceEditorVM.CurrCourseType == CourseType.TIMEPERAT)
            {
                XElement root = new XElement("Segments", new XAttribute("Count", CourceEditorVM.CourceUnitCollection.Count));
                for (int i = 0; i < CourceEditorVM.CourceUnitCollection.Count; i++)
                {
                    root.Add(new XElement("Val", new XElement("Minutes", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Time, NoOfDecimal)), new XElement("StartAt", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].StartAT, NoOfDecimal)), new XElement("EndAt", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].EndAT, NoOfDecimal))));
                }
                return root;

            }
            else if (CourceEditorVM.CurrCourseType == CourseType.DISTANCEPERAT)
            {
                XElement root = new XElement("Segments", new XAttribute("Count", CourceEditorVM.CourceUnitCollection.Count));
                for (int i = 0; i < CourceEditorVM.CourceUnitCollection.Count; i++)
                {
                   root.Add(new XElement("Val", new XElement("Length", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].Length, NoOfDecimal)), new XElement("StartAt", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].StartAT, NoOfDecimal)), new XElement("EndAt", Ulilities.Round(CourceEditorVM.CourceUnitCollection[i].EndAT, NoOfDecimal))));
                }
                return root;
            }


            return null;
        }

        private void SaveBtnClick(object sender, RoutedEventArgs e)
        {
            if (CourceEditorVM == null)
                return;

            string DirName = System.IO.Path.GetDirectoryName(CourceEditorVM.CourseFileName);

            string strCourceFileName = System.IO.Path.GetFileName(CourceEditorVM.CourseFileName);
            // if you would to remove spaces
            //while (strCourceFileName.Contains(' '))
            //{
            //    int Index = strCourceFileName.IndexOf(' ');
            //    strCourceFileName = strCourceFileName.Remove(Index, 1);
            //}

            SaveFileDialog dlSave = new SaveFileDialog();
            dlSave.FileName = strCourceFileName;
            dlSave.InitialDirectory = DirName;
            
            dlSave.DefaultExt = ".rmc";
            dlSave.Filter = "RacerMate Cource File (.rmc) | *.rmc";
            if (dlSave.ShowDialog().Value)
            {
               string SavingFile = dlSave.FileName;

               string strDirectoryName = System.IO.Path.GetDirectoryName(SavingFile);

               string strModifyName = System.IO.Path.GetFileNameWithoutExtension(SavingFile);

               SavingFile = strDirectoryName + "\\" + strModifyName + ".rmc";

                XElement Remaining   = null;
                if (CourceEditorVM.Header.Comment != string.Empty)
                    Remaining = new XElement("Comment", CourceEditorVM.Header.Comment);

               XElement xHeader = new XElement("Header",
               new XElement("CreatorExe", "RacerMateOne"),
               new XElement("Date", DateTime.Now.ToString()),
               new XElement("Version", "1.03"),
               new XElement("Copyright", "(c) 2013, RacerMateOne, Inc."),
               new XElement("Comment", CourceEditorVM.Header.Comment),
               new XElement("CompressType", "0"));
               CourseHash = CalculateCourseHash();
               HeaderHash = CalculateHeaderHash();

               XElement xLegs = AddLegs();
               String OriginalHash = HeaderHash;

                CourceEditorVM.Info.Description = TxtNotes.Text;
                CourceEditorVM.TotalDistance = Ulilities.Round(CurrentDistance(), 2);
                CourceEditorVM.SaveTotalDistance = Ulilities.Round(SaveCurrentDistance(), 2);

                switch (CourceEditorVM.CurrCourseType)
                {
                    case CourseType.DISTANCEGRADE:
                        CourceEditorVM.Info.Type = "ThreeD";
                        break;
                    case CourseType.DISTANCEPERAT:
                    case CourseType.DISTANCEWATT:
                    case CourseType.TIMEGRADE:
                    case CourseType.TIMEPERAT:
                    case CourseType.TIMEWATT:
                        CourceEditorVM.Info.Type = "Watts";
                        break;
                }

                switch (CourceEditorVM.CurrCourseType)
                {
                    case CourseType.DISTANCEPERAT:
                        {
                            double AccumDistAT = CourceEditorVM.CourceUnitCollection.Last().AccumDistAT;
                            if (CourceEditorVM.SelectMeters == true)
                            {
                                AccumDistAT /= OneMileInMeter;
                            }
                            AccumDistAT = Ulilities.Round(AccumDistAT, 2);
                            CourceEditorVM.Info.Length = AccumDistAT.ToString();
                        }
                        break;
                    case CourseType.DISTANCEWATT:
                        {
                            double AccumDistWatt = CourceEditorVM.CourceUnitCollection.Last().AccumDistWatt;
                            if (CourceEditorVM.SelectMeters == true)
                            {
                                AccumDistWatt /= OneMileInMeter;
                            }
                            AccumDistWatt = Ulilities.Round(AccumDistWatt, 2);
                            CourceEditorVM.Info.Length = AccumDistWatt.ToString();
                        }
                        break;
                    case CourseType.TIMEPERAT:
                    case CourseType.TIMEWATT:
                        break;

                    case CourseType.TIMEGRADE:
                      CourceEditorVM.Info.Length = Ulilities.Round(CourceEditorVM.CourceUnitCollection.Last().AccumTimeGrade,2).ToString();
                        break;
                    case CourseType.DISTANCEGRADE:
                        {
                            double AccumDistGrade = CourceEditorVM.CourceUnitCollection.Last().AccumDistGrade;
                            if (CourceEditorVM.SelectMeters == true)
                            {
                                AccumDistGrade *= KilometerToMile;
                            }
                            AccumDistGrade = Ulilities.Round(AccumDistGrade, 2);
                            CourceEditorVM.Info.Length = AccumDistGrade.ToString();
                        }
                        break;
                }
                


               XElement xCourse =
               new XElement("Info",
                   new XAttribute("Name", strModifyName),
                   new XAttribute("Description", CourceEditorVM.Info.Description),
                   new XAttribute("FileName", strModifyName  + ".rmc"),
                   new XAttribute("Type", CourceEditorVM.Info.Type),
                   new XAttribute("Looped", CourceEditorVM.Info.Looped ? "True" : "False"),
                   new XAttribute("Length", CourceEditorVM.Info.Length),
                   new XAttribute("Laps", CourceEditorVM.Info.Laps),
                   new XAttribute("StartAt", CourceEditorVM.Info.StartAt),
                   new XAttribute("EndAt", CourceEditorVM.SaveTotalDistance),
                   new XAttribute("Mirror", CourceEditorVM.Info.Mirror ? "True" : "False"),
                   new XAttribute("Reverse", CourceEditorVM.Info.Reverse ? "True" : "False"),
                   new XAttribute("Modified", CourceEditorVM.Info.Modified ? "True" : "False"),
                   new XAttribute("XUnits", CourceEditorVM.Info.XUnits),
                   new XAttribute("YUnits", CourceEditorVM.Info.YUnits),
                   new XAttribute("OriginalHash", OriginalHash),
                   new XAttribute("CourseHash", CourseHash),
                   new XAttribute("HeaderHash", HeaderHash) 
                
                   
               );// end of course

               XDocument xDoc = new XDocument(
               new XDeclaration("1.0", "utf-8", null),
               new XComment("This document is created by RacerMate"),
               new XElement("RMX", xHeader,new XElement("Course", xCourse, xLegs)));


               xDoc.Save(SavingFile);

               string FileName = System.IO.Path.GetFileName(SavingFile);
               string strMessage = string.Format("{0} has been saved", FileName);
                InfoDialog infoDialog = new InfoDialog(strMessage, ShowIcon.OK, SavingFile);
                infoDialog.Owner = Application.Current.MainWindow;
                infoDialog.ShowDialog();
                CourceEditorVM.Saved = true;
            }

        }

        double FindDistance(double latA, double lonA, double latB, double lotB)
        {

            double km = GPXLoader.FindDistance(latA, lonA, latB, lotB);
            return km;
        }

        double FindGrade(double A, double B)
        {
            double grade = B - A;
            return grade;
        }

        private void ImportBtnClick(object sender, RoutedEventArgs e)
        {
            if (CourceEditorVM.Saved == false)
            {
                OkCancelDialog okCancelDialog = new OkCancelDialog(MessageNotSaved);
                okCancelDialog.Owner = Application.Current.MainWindow;
                bool OkResult = (bool)okCancelDialog.ShowDialog();
                if (OkResult == false)
                {
                    return;
                }
            }
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.DefaultExt = ".gpx";
            openDialog.Filter = "GPS Data (.gpx)|*.gpx";
            openDialog.DefaultExt = ".gpx";

            if (openDialog.ShowDialog().Value)
            {
                GPXLoader gpxLoader = new GPXLoader();
                gpxTrackList = gpxLoader.LoadGPXTracksList(openDialog.FileName);
                string fileName = gpxTrackList[0].TrackName;
                FileSelectionViewModel fileSelectionViewModel = new FileSelectionViewModel();
                fileSelectionViewModel.Selectd = 0;

                if (gpxTrackList.Count > 1)
                {
                    for (int i = 0; i < gpxTrackList.Count; i++)
                    {
                        fileName = gpxTrackList[i].TrackName;
                        fileSelectionViewModel.GpxFileNames.Add(fileName);
                    }

                    FileSlection fileSlectionDlg = new FileSlection();
                    fileSlectionDlg.Owner = Application.Current.MainWindow;
                    fileSlectionDlg.DataContext = fileSelectionViewModel;
                    bool result = (bool)fileSlectionDlg.ShowDialog();
                    if (result == true)
                    {
                        fileName = gpxTrackList[fileSelectionViewModel.Selectd].TrackName;
                    }
                    else
                    {
                        return;
                    }
                }

                int count = gpxTrackList[fileSelectionViewModel.Selectd].SegList.Count;

                if (count == 0)
                {
                    InfoDialog infoDialog = new InfoDialog("There is a problem reading this file. It looks like it is not GPX file formate", ShowIcon.OK);
                    infoDialog.Owner = Application.Current.MainWindow;
                    infoDialog.ShowDialog();
                    return;
                }

                int length = gpxTrackList[fileSelectionViewModel.Selectd].SegList.Count;
                if (length <= 1)
                {
                    InfoDialog infoDialog = new InfoDialog("There is a problem reading this file. It looks like it is not GPX file formate", ShowIcon.OK);
                    infoDialog.Owner = Application.Current.MainWindow;
                    infoDialog.ShowDialog();
                    return;
                }

                CourceEditorVM.CurrCourseType = CourseType.DISTANCEGRADE;
                CourceEditorVM.CourceUnitCollection.Clear();
                CourceEditorVM.SelectMeters = IsMetric();
                CourceEditorVM.Header = new RacerMateHeader("RacerMateOne", DateTime.Now.ToString(), "4.0.2", "(c) 2011, RacerMateOne, Inc.", string.Format("Converted from {0}", strFileName), "0");
                CourceEditorVM.Info = new RacerMateInfo();
                CourceEditorVM.Info.Name = string.Format("{0}_3dc", strFileName);
                CourceEditorVM.Info.Description = TxtNotes.Text;
                CourceEditorVM.Info.Type = "ThreeD";
                CourceEditorVM.Info.FileName = string.Format("{0}_3dc.rmc", strFileName);
                CourceEditorVM.Info.Looped = false;
                CourceEditorVM.Info.Length = "";
                CourceEditorVM.Info.Laps = 1;
                CourceEditorVM.Info.StartAt = 0;
                CourceEditorVM.Info.EndAt = 0;
                CourceEditorVM.Info.Mirror = false;
                CourceEditorVM.Info.Reverse = false;
                CourceEditorVM.Info.Modified = false;
                CourceEditorVM.Info.XUnits = "Distance";
                CourceEditorVM.Info.YUnits = "Grade";
                CourceEditorVM.Info.OriginalHash = "";
                CourceEditorVM.Info.CourseHash = "";
                CourceEditorVM.Info.HeaderHash = "";
                CourceEditorVM.CourceUnitCollection.Clear();

                splashWindow = new SplashWindow();
                splashWindow.Owner = Application.Current.MainWindow;
                splashWindow.Show();


                for (int i = 0; i < gpxTrackList[0].SegList.Count - 1; i++)
                {
                    CourseUnits cu = new CourseUnits();
                    
                    double Length = FindDistance(gpxTrackList[fileSelectionViewModel.Selectd].SegList[i].Latitude, gpxTrackList[fileSelectionViewModel.Selectd].SegList[i].Longitude, gpxTrackList[fileSelectionViewModel.Selectd].SegList[i + 1].Latitude, gpxTrackList[fileSelectionViewModel.Selectd].SegList[i + 1].Longitude);
                    double Grade = FindGrade(gpxTrackList[fileSelectionViewModel.Selectd].SegList[i].Elevation, gpxTrackList[fileSelectionViewModel.Selectd].SegList[i + 1].Elevation);
                    double Course = gpxTrackList[fileSelectionViewModel.Selectd].SegList[i].Course;
                    double Speed = gpxTrackList[fileSelectionViewModel.Selectd].SegList[i].Speed;
                    string Time = gpxTrackList[fileSelectionViewModel.Selectd].SegList[i].Time;

                    cu.Wind = 0;
                    cu.Length = Length * 1000;
                    cu.Grade = Grade;
                    cu.DisplayGrade = Grade / 100.0;
                    CourceEditorVM.CourceUnitCollection.Add(cu);
                }

                BackgroundWorker worker = new BackgroundWorker();

                ListViewSegments.View = (GridView)FindResource("GV_DISTGRADE");
                HorizontalList.ItemContainerStyle = (Style)FindResource("LIST_DISTGRADE");
                CourceTypeTB.Text = "Distance/Grade";
                worker.DoWork += (s, args) => DistanceGradeUpdate();

                worker.RunWorkerCompleted += (s, args) =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    DisplySegments.Text = string.Format("Segments: {0}", CourceEditorVM.CourceUnitCollection.Count);

                    //DisplayTotalDistance();
                    //CurrentDistance();
                    CourceEditorVM.TotalDistance = CurrentDistance();
                    CourceEditorVM.CourseFileName = SavePath() + @"\Distance and Grade\NewFile";

                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
                    {
                        Mouse.OverrideCursor = null;
                        Application.Current.MainWindow.Cursor = null;
                        splashWindow.Close();

                        if (ListViewSegments.HasItems && ListViewSegments.SelectedIndex == -1)
                        {
                            ListViewSegments.SelectedIndex = 0;
                        }
                    }));
                    // Mouse.OverrideCursor = null;
                };

                worker.RunWorkerAsync();

                if (ListViewSegments.Items.Count > 0)
                {
                    DisplySegments.Text = string.Format("Segments: {0}", CourceEditorVM.CourceUnitCollection.Count);
                    ListViewSegments.SelectedIndex = 0;
                }
            }

        }

        private void OnList_MouseDoubleClik(object sender, MouseButtonEventArgs e)
        {
            PushVM();
            BackgroundWorker worker = new BackgroundWorker();
            if (CourceEditorVM.CurrCourseType == CourseType.DISTANCEGRADE)
            {
                int Index = HorizontalList.SelectedIndex;
                CourseUnits dlgEditorViewModel = new CourseUnits(Index + 1);

                CourceEditorVM.CourceUnitCollection[Index].DisplayLength = (CourceEditorVM.SelectMeters == true) ? CourceEditorVM.CourceUnitCollection[Index].Length : CourceEditorVM.CourceUnitCollection[Index].Length * MeterToFeet;

                dlgEditorViewModel.MinWind  = CourceEditorVM.CourceUnitCollection[Index].MinWind;
                dlgEditorViewModel.MaxWind  = CourceEditorVM.CourceUnitCollection[Index].MaxWind;
                dlgEditorViewModel.WindUnit = CourceEditorVM.CourceUnitCollection[Index].WindUnit;
                dlgEditorViewModel.Wind     = CourceEditorVM.CourceUnitCollection[Index].Wind;

                dlgEditorViewModel.MaxGrade = CourceEditorVM.CourceUnitCollection[Index].MaxGrade;
                dlgEditorViewModel.MinGrade = CourceEditorVM.CourceUnitCollection[Index].MinGrade;
                dlgEditorViewModel.GradeUnit = CourceEditorVM.CourceUnitCollection[Index].GradeUnit;

                dlgEditorViewModel.MinLength = CourceEditorVM.CourceUnitCollection[Index].MinLength;
                dlgEditorViewModel.MaxLength = CourceEditorVM.CourceUnitCollection[Index].MaxLength;
                dlgEditorViewModel.MinMaxLengthUnit = CourceEditorVM.CourceUnitCollection[Index].MinMaxLengthUnit;
                dlgEditorViewModel.DisplayLength = CourceEditorVM.CourceUnitCollection[Index].DisplayLength; 

                dlgEditorViewModel.DisplayGrade = CourceEditorVM.CourceUnitCollection[Index].Grade * 100.0; 
                
                dlgEditorViewModel.CourseMode = CourseType.DISTANCEGRADE;

                SegmentEditDialog legEditDlg = new SegmentEditDialog();
                legEditDlg.DataContext = dlgEditorViewModel;
                legEditDlg.Owner = Application.Current.MainWindow;
                bool result = (bool)legEditDlg.ShowDialog();
                if (result == true)
                {
                    CourceEditorVM.Saved = false;
                    if (dlgEditorViewModel.Wind < CourceEditorVM.CourceUnitCollection[Index].MinWind)
                    {
                        dlgEditorViewModel.Wind = CourceEditorVM.CourceUnitCollection[Index].MinWind;
                    }
                    if (dlgEditorViewModel.Wind > CourceEditorVM.CourceUnitCollection[Index].MaxWind)
                    {
                        dlgEditorViewModel.Wind = CourceEditorVM.CourceUnitCollection[Index].MaxWind;
                    }

                    if (dlgEditorViewModel.DisplayLength < CourceEditorVM.CourceUnitCollection[Index].MinLength)
                    {
                        dlgEditorViewModel.DisplayLength = CourceEditorVM.CourceUnitCollection[Index].MinLength;
                    }
                    if (dlgEditorViewModel.DisplayLength > CourceEditorVM.CourceUnitCollection[Index].MaxLength)
                    {
                        dlgEditorViewModel.DisplayLength = CourceEditorVM.CourceUnitCollection[Index].MaxLength;
                    }

                    if (dlgEditorViewModel.DisplayGrade < CourceEditorVM.CourceUnitCollection[Index].MinGrade)
                    {
                        dlgEditorViewModel.DisplayGrade = CourceEditorVM.CourceUnitCollection[Index].MinGrade;
                    }
                    if (dlgEditorViewModel.DisplayGrade > CourceEditorVM.CourceUnitCollection[Index].MaxGrade)
                    {
                        dlgEditorViewModel.DisplayGrade = CourceEditorVM.CourceUnitCollection[Index].MaxGrade;
                    }

                    CourceEditorVM.CourceUnitCollection[Index].Wind = dlgEditorViewModel.Wind;
                    CourceEditorVM.CourceUnitCollection[Index].DisplayLength = dlgEditorViewModel.DisplayLength;
                    CourceEditorVM.CourceUnitCollection[Index].DisplayGrade = dlgEditorViewModel.DisplayGrade;
                    
                    CourceEditorVM.CourceUnitCollection[Index].Grade = dlgEditorViewModel.DisplayGrade / 100.0;
                    CourceEditorVM.CourceUnitCollection[Index].Length = (CourceEditorVM.SelectMeters == true) ? dlgEditorViewModel.DisplayLength : dlgEditorViewModel.DisplayLength / MeterToFeet;

                    worker.DoWork += (s, args) => DistanceGradeUpdate();
                }

            }
            else if (CourceEditorVM.CurrCourseType == CourseType.TIMEGRADE)
            {
                int Index = HorizontalList.SelectedIndex;
                CourseUnits Current = CourceEditorVM.CourceUnitCollection[Index];
                CourseUnits dlgEditorViewModel = new CourseUnits(Index + 1);

                dlgEditorViewModel.MinWind = Current.MinWind;
                dlgEditorViewModel.MaxWind = Current.MaxWind;
                dlgEditorViewModel.WindUnit = Current.WindUnit;
                dlgEditorViewModel.Wind = Current.Wind;

                dlgEditorViewModel.MaxGrade = Current.MaxGrade;
                dlgEditorViewModel.MinGrade = Current.MinGrade;
                dlgEditorViewModel.GradeUnit = Current.GradeUnit;
                dlgEditorViewModel.DisplayGrade = Current.DisplayGrade; 

                dlgEditorViewModel.MinTime = Current.MinTime;
                dlgEditorViewModel.MaxTime = Current.MaxTime;
                dlgEditorViewModel.TimeUnit = Current.TimeUnit;
                dlgEditorViewModel.Time = Current.Time;

                dlgEditorViewModel.CourseMode = CourseType.TIMEGRADE;

                SegmentEditDialog legEditDlg = new SegmentEditDialog();
                legEditDlg.DataContext = dlgEditorViewModel;
                legEditDlg.Owner = Application.Current.MainWindow;
                bool result = (bool)legEditDlg.ShowDialog();
                if (result == true)
                {
                    CourceEditorVM.Saved = false;
                    if (dlgEditorViewModel.Wind < CourceEditorVM.CourceUnitCollection[Index].MinWind)
                    {
                        dlgEditorViewModel.Wind = CourceEditorVM.CourceUnitCollection[Index].MinWind;
                    }
                    if (dlgEditorViewModel.Wind > CourceEditorVM.CourceUnitCollection[Index].MaxWind)
                    {
                        dlgEditorViewModel.Wind = CourceEditorVM.CourceUnitCollection[Index].MaxWind;
                    }

                    if (dlgEditorViewModel.Time < CourceEditorVM.CourceUnitCollection[Index].MinTime)
                    {
                        dlgEditorViewModel.Time = CourceEditorVM.CourceUnitCollection[Index].MinTime;
                    }
                    if (dlgEditorViewModel.Time > CourceEditorVM.CourceUnitCollection[Index].MaxTime)
                    {
                        dlgEditorViewModel.Time = CourceEditorVM.CourceUnitCollection[Index].MaxTime;
                    }

                    if (dlgEditorViewModel.DisplayGrade < CourceEditorVM.CourceUnitCollection[Index].MinGrade)
                    {
                        dlgEditorViewModel.DisplayGrade = CourceEditorVM.CourceUnitCollection[Index].MinGrade;
                    }
                    if (dlgEditorViewModel.DisplayGrade > CourceEditorVM.CourceUnitCollection[Index].MaxGrade)
                    {
                        dlgEditorViewModel.DisplayGrade = CourceEditorVM.CourceUnitCollection[Index].MaxGrade;
                    }


                    CourceEditorVM.CourceUnitCollection[Index].Wind = dlgEditorViewModel.Wind;
                    CourceEditorVM.CourceUnitCollection[Index].Time = dlgEditorViewModel.Time;
                    CourceEditorVM.CourceUnitCollection[Index].Grade = dlgEditorViewModel.DisplayGrade / 100.0; 
                    worker.DoWork += (s, args) => TimeGradeUpdate();
                }
            }
            else if (CourceEditorVM.CurrCourseType == CourseType.TIMEWATT)
            {
                int Index = HorizontalList.SelectedIndex;
                CourseUnits Current = CourceEditorVM.CourceUnitCollection[Index];
                CourseUnits dlgEditorViewModel = new CourseUnits(Index + 1);

                PolygonData.MaxValue = double.NegativeInfinity;
                PolygonData.MinValue = double.PositiveInfinity;

                dlgEditorViewModel.StartWatt = Current.StartWatt;
                dlgEditorViewModel.MaxWatt = Current.MaxWatt;
                dlgEditorViewModel.MinWatt = Current.MinWatt;
                dlgEditorViewModel.WattUnit = Current.WattUnit;
                dlgEditorViewModel.EndWatt = Current.EndWatt;

                dlgEditorViewModel.MinTime = Current.MinTime;
                dlgEditorViewModel.MaxTime = Current.MaxTime;
                dlgEditorViewModel.TimeUnit = Current.TimeUnit;
                dlgEditorViewModel.Time = Current.Time;

                dlgEditorViewModel.CourseMode = CourseType.TIMEWATT;

                SegmentEditDialog legEditDlg = new SegmentEditDialog();
                legEditDlg.DataContext = dlgEditorViewModel;
                legEditDlg.Owner = Application.Current.MainWindow;
                bool result = (bool)legEditDlg.ShowDialog();
                if (result == true)
                {
                    CourceEditorVM.Saved = false;
                    if (dlgEditorViewModel.Time < CourceEditorVM.CourceUnitCollection[Index].MinTime)
                    {                                                                           
                        dlgEditorViewModel.Time = CourceEditorVM.CourceUnitCollection[Index].MinTime;
                    }                                                                           
                    if (dlgEditorViewModel.Time > CourceEditorVM.CourceUnitCollection[Index].MaxTime)
                    {                                                                           
                        dlgEditorViewModel.Time = CourceEditorVM.CourceUnitCollection[Index].MaxTime;
                    }

                    if (dlgEditorViewModel.StartWatt < CourceEditorVM.CourceUnitCollection[Index].MinWatt)
                    {                                                                               
                        dlgEditorViewModel.StartWatt = CourceEditorVM.CourceUnitCollection[Index].MinWatt;
                    }                                                                               
                    if (dlgEditorViewModel.StartWatt > CourceEditorVM.CourceUnitCollection[Index].MaxWatt)
                    {                                                                               
                        dlgEditorViewModel.StartWatt = CourceEditorVM.CourceUnitCollection[Index].MaxWatt;
                    }

                    if (dlgEditorViewModel.EndWatt < CourceEditorVM.CourceUnitCollection[Index].MinWatt)
                    {
                        dlgEditorViewModel.EndWatt = CourceEditorVM.CourceUnitCollection[Index].MinWatt;
                    }
                    if (dlgEditorViewModel.EndWatt > CourceEditorVM.CourceUnitCollection[Index].MaxWatt)
                    {
                        dlgEditorViewModel.EndWatt = CourceEditorVM.CourceUnitCollection[Index].MaxWatt;
                    }
                    CourceEditorVM.CourceUnitCollection[Index].Time = dlgEditorViewModel.Time;
                    CourceEditorVM.CourceUnitCollection[Index].StartWatt = dlgEditorViewModel.StartWatt;
                    CourceEditorVM.CourceUnitCollection[Index].EndWatt = dlgEditorViewModel.EndWatt;
                    worker.DoWork += (s, args) => TimeWattsUpdate();
                }
            }
            else if (CourceEditorVM.CurrCourseType == CourseType.DISTANCEWATT)
            {

                int Index = HorizontalList.SelectedIndex;
              //  CourseUnits Current = CourceEditorVM.CourceUnitCollection[Index];
                CourseUnits dlgEditorViewModel = new CourseUnits(Index + 1);
                CourceEditorVM.CourceUnitCollection[Index].DisplayLength = (CourceEditorVM.SelectMeters == true) ? CourceEditorVM.CourceUnitCollection[Index].Length : CourceEditorVM.CourceUnitCollection[Index].Length * MeterToFeet;


                PolygonData.MaxValue = double.NegativeInfinity;
                PolygonData.MinValue = double.PositiveInfinity;

                dlgEditorViewModel.StartWatt = CourceEditorVM.CourceUnitCollection[Index].StartWatt;
                dlgEditorViewModel.EndWatt = CourceEditorVM.CourceUnitCollection[Index].EndWatt;
                dlgEditorViewModel.MaxWatt = CourceEditorVM.CourceUnitCollection[Index].MaxWatt;
                dlgEditorViewModel.MinWatt = CourceEditorVM.CourceUnitCollection[Index].MinWatt;
                dlgEditorViewModel.WattUnit = CourceEditorVM.CourceUnitCollection[Index].WattUnit;

                dlgEditorViewModel.MinLength = CourceEditorVM.CourceUnitCollection[Index].MinLength;
                dlgEditorViewModel.MaxLength = CourceEditorVM.CourceUnitCollection[Index].MaxLength;
                dlgEditorViewModel.LengthUnit = CourceEditorVM.CourceUnitCollection[Index].LengthUnit;
                dlgEditorViewModel.MinMaxLengthUnit = CourceEditorVM.CourceUnitCollection[Index].MinMaxLengthUnit;
                dlgEditorViewModel.DisplayLength = CourceEditorVM.CourceUnitCollection[Index].DisplayLength;

                dlgEditorViewModel.CourseMode = CourseType.DISTANCEWATT;

                SegmentEditDialog legEditDlg = new SegmentEditDialog();
                legEditDlg.DataContext = dlgEditorViewModel;
                legEditDlg.Owner = Application.Current.MainWindow;
                bool result = (bool)legEditDlg.ShowDialog();
                if (result == true)
                {
                    CourceEditorVM.Saved = false;
                    if (dlgEditorViewModel.DisplayLength < CourceEditorVM.CourceUnitCollection[Index].MinLength)
                    {
                        dlgEditorViewModel.DisplayLength = CourceEditorVM.CourceUnitCollection[Index].MinLength;
                    }
                    if (dlgEditorViewModel.DisplayLength > CourceEditorVM.CourceUnitCollection[Index].MaxLength)
                    {
                        dlgEditorViewModel.DisplayLength = CourceEditorVM.CourceUnitCollection[Index].MaxLength;
                    }

                    if (dlgEditorViewModel.StartWatt < CourceEditorVM.CourceUnitCollection[Index].MinWatt)
                    {
                        dlgEditorViewModel.StartWatt = CourceEditorVM.CourceUnitCollection[Index].MinWatt;
                    }
                    if (dlgEditorViewModel.StartWatt > CourceEditorVM.CourceUnitCollection[Index].MaxWatt)
                    {
                        dlgEditorViewModel.StartWatt = CourceEditorVM.CourceUnitCollection[Index].MaxWatt;
                    }

                    if (dlgEditorViewModel.EndWatt < CourceEditorVM.CourceUnitCollection[Index].MinWatt)
                    {
                        dlgEditorViewModel.EndWatt = CourceEditorVM.CourceUnitCollection[Index].MinWatt;
                    }
                    if (dlgEditorViewModel.EndWatt > CourceEditorVM.CourceUnitCollection[Index].MaxWatt)
                    {
                        dlgEditorViewModel.EndWatt = CourceEditorVM.CourceUnitCollection[Index].MaxWatt;
                    }

                    CourceEditorVM.CourceUnitCollection[Index].StartWatt = dlgEditorViewModel.StartWatt;
                    CourceEditorVM.CourceUnitCollection[Index].EndWatt = dlgEditorViewModel.EndWatt;
                    CourceEditorVM.CourceUnitCollection[Index].Length = (CourceEditorVM.SelectMeters == true) ? dlgEditorViewModel.DisplayLength : dlgEditorViewModel.DisplayLength / MeterToFeet;

                    worker.DoWork += (s, args) => DistanceWattUpdate();
                }
            }
            else if (CourceEditorVM.CurrCourseType == CourseType.TIMEPERAT)
            {

                int Index = HorizontalList.SelectedIndex;
                CourseUnits Current = CourceEditorVM.CourceUnitCollection[Index];
                CourseUnits dlgEditorViewModel = new CourseUnits(Index + 1);

                PolygonData.MaxValue = double.NegativeInfinity;
                PolygonData.MinValue = double.PositiveInfinity;

                dlgEditorViewModel.StartAT = Current.StartAT;
                dlgEditorViewModel.EndAT = Current.EndAT;
                dlgEditorViewModel.MaxAT = Current.MaxAT;
                dlgEditorViewModel.MinAT = Current.MinAT;
                dlgEditorViewModel.ATUnit = Current.ATUnit;

                dlgEditorViewModel.MinTime = Current.MinTime;
                dlgEditorViewModel.MaxTime = Current.MaxTime;
                dlgEditorViewModel.TimeUnit = Current.TimeUnit;
                dlgEditorViewModel.Time = Current.Time;

                dlgEditorViewModel.CourseMode = CourseType.TIMEPERAT;

                SegmentEditDialog legEditDlg = new SegmentEditDialog();
                legEditDlg.DataContext = dlgEditorViewModel;
                legEditDlg.Owner = Application.Current.MainWindow;
                bool result = (bool)legEditDlg.ShowDialog();
                if (result == true)
                {
                    CourceEditorVM.Saved = false;
                    if (dlgEditorViewModel.Time < CourceEditorVM.CourceUnitCollection[Index].MinTime)
                    {                                                                           
                        dlgEditorViewModel.Time = CourceEditorVM.CourceUnitCollection[Index].MinTime;
                    }                                                                          
                    if (dlgEditorViewModel.Time > CourceEditorVM.CourceUnitCollection[Index].MaxTime)
                    {                                                                           
                        dlgEditorViewModel.Time = CourceEditorVM.CourceUnitCollection[Index].MaxTime;
                    }

                    if (dlgEditorViewModel.StartAT < CourceEditorVM.CourceUnitCollection[Index].MinAT)
                    {                                                                              
                        dlgEditorViewModel.StartAT = CourceEditorVM.CourceUnitCollection[Index].MinAT;
                    }                                                                              
                    if (dlgEditorViewModel.StartAT > CourceEditorVM.CourceUnitCollection[Index].MaxAT)
                    {
                        dlgEditorViewModel.StartAT = CourceEditorVM.CourceUnitCollection[Index].MaxAT;
                    }

                    if (dlgEditorViewModel.EndAT < CourceEditorVM.CourceUnitCollection[Index].MinAT)
                    {
                        dlgEditorViewModel.EndAT = CourceEditorVM.CourceUnitCollection[Index].MinAT;
                    }
                    if (dlgEditorViewModel.EndAT > CourceEditorVM.CourceUnitCollection[Index].MaxAT)
                    {
                        dlgEditorViewModel.EndAT = CourceEditorVM.CourceUnitCollection[Index].MaxAT;
                    }

                    CourceEditorVM.CourceUnitCollection[Index].Time = dlgEditorViewModel.Time;
                    CourceEditorVM.CourceUnitCollection[Index].StartAT = dlgEditorViewModel.StartAT;
                    CourceEditorVM.CourceUnitCollection[Index].EndAT = dlgEditorViewModel.EndAT;
                    worker.DoWork += (s, args) => TimeAtUpdate();
                }
            }
            else if (CourceEditorVM.CurrCourseType == CourseType.DISTANCEPERAT)
            {
                int Index = HorizontalList.SelectedIndex;
                CourseUnits dlgEditorViewModel = new CourseUnits(Index + 1);

                CourceEditorVM.CourceUnitCollection[Index].DisplayLength = (CourceEditorVM.SelectMeters == true) ? CourceEditorVM.CourceUnitCollection[Index].Length : CourceEditorVM.CourceUnitCollection[Index].Length * MeterToFeet;


                PolygonData.MaxValue = double.NegativeInfinity;
                PolygonData.MinValue = double.PositiveInfinity;

                dlgEditorViewModel.StartAT = CourceEditorVM.CourceUnitCollection[Index].StartAT;
                dlgEditorViewModel.EndAT = CourceEditorVM.CourceUnitCollection[Index].EndAT;

                dlgEditorViewModel.MaxAT = CourceEditorVM.CourceUnitCollection[Index].MaxAT;
                dlgEditorViewModel.MinAT = CourceEditorVM.CourceUnitCollection[Index].MinAT;
                dlgEditorViewModel.ATUnit = CourceEditorVM.CourceUnitCollection[Index].ATUnit;

                dlgEditorViewModel.MinLength = CourceEditorVM.CourceUnitCollection[Index].MinLength;
                dlgEditorViewModel.MaxLength = CourceEditorVM.CourceUnitCollection[Index].MaxLength;
                dlgEditorViewModel.LengthUnit = CourceEditorVM.CourceUnitCollection[Index].LengthUnit;
                dlgEditorViewModel.MinMaxLengthUnit = CourceEditorVM.CourceUnitCollection[Index].MinMaxLengthUnit;
                dlgEditorViewModel.DisplayLength = CourceEditorVM.CourceUnitCollection[Index].DisplayLength;
            
                dlgEditorViewModel.CourseMode = CourseType.DISTANCEPERAT;

                SegmentEditDialog legEditDlg = new SegmentEditDialog();
                legEditDlg.DataContext = dlgEditorViewModel;
                legEditDlg.Owner = Application.Current.MainWindow;
                bool result = (bool)legEditDlg.ShowDialog();
                if (result == true)
                {
                    CourceEditorVM.Saved = false;
                    if (dlgEditorViewModel.DisplayLength < CourceEditorVM.CourceUnitCollection[Index].MinLength)
                    {
                        dlgEditorViewModel.DisplayLength = CourceEditorVM.CourceUnitCollection[Index].MinLength;
                    }
                    if (dlgEditorViewModel.DisplayLength > CourceEditorVM.CourceUnitCollection[Index].MaxLength)
                    {
                        dlgEditorViewModel.DisplayLength = CourceEditorVM.CourceUnitCollection[Index].MaxLength;
                    }

                    if (dlgEditorViewModel.StartAT < CourceEditorVM.CourceUnitCollection[Index].MinAT)
                    {
                        dlgEditorViewModel.StartAT = CourceEditorVM.CourceUnitCollection[Index].MinAT;
                    }
                    if (dlgEditorViewModel.StartAT > CourceEditorVM.CourceUnitCollection[Index].MaxAT)
                    {
                        dlgEditorViewModel.StartAT = CourceEditorVM.CourceUnitCollection[Index].MaxAT;
                    }

                    if (dlgEditorViewModel.EndAT < CourceEditorVM.CourceUnitCollection[Index].MinAT)
                    {
                        dlgEditorViewModel.EndAT = CourceEditorVM.CourceUnitCollection[Index].MinAT;
                    }
                    if (dlgEditorViewModel.EndAT > CourceEditorVM.CourceUnitCollection[Index].MaxAT)
                    {
                        dlgEditorViewModel.EndAT = CourceEditorVM.CourceUnitCollection[Index].MaxAT;
                    }

                   
                    CourceEditorVM.CourceUnitCollection[Index].StartAT = dlgEditorViewModel.StartAT;
                    CourceEditorVM.CourceUnitCollection[Index].EndAT = dlgEditorViewModel.EndAT;
                    CourceEditorVM.CourceUnitCollection[Index].Length = (CourceEditorVM.SelectMeters == true) ? dlgEditorViewModel.DisplayLength : dlgEditorViewModel.DisplayLength / MeterToFeet;

                    worker.DoWork += (s, args) => DistanceAtUpdate();
                }

            }

            worker.RunWorkerCompleted += (s, args) =>
            {
             //   DataContext = CourceEditorVM;
             //   DisplySegments.Text = string.Format("Segments: {0}", CourceEditorVM.CourceUnitCollection.Count);

               // DisplayTotalDistance();
                CurrentDistance();


                System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
                {
                    Mouse.OverrideCursor = null;
                    Application.Current.MainWindow.Cursor = null;
                    if (ListViewSegments.HasItems && ListViewSegments.SelectedIndex == -1)
                    {
                        ListViewSegments.SelectedIndex = 0;
                    }
                }));
                // Mouse.OverrideCursor = null;
            };

            worker.RunWorkerAsync();
            
            if (ListViewSegments.Items.Count > 0 && ListViewSegments.SelectedIndex == -1)
            {
                DisplySegments.Text = string.Format("Segments: {0}", CourceEditorVM.CourceUnitCollection.Count);
                ListViewSegments.SelectedIndex = 0;
            }
        }
        
       
        void ClearSegmentInMemory()
        {
            SelectedColection = null;
            CourseEditorViewModel CourceEditorVM = (CourseEditorViewModel)DataContext;
            if (CourceEditorVM != null)
            {
                CourceEditorVM.MemoryCanvas = Visibility.Hidden;
            }
        }
	}
}

