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
using System.Windows.Interop;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ComponentModel;
using System.Windows.Media.Effects;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace RacerMateOne.Pages.Modes
{
    /// <summary>
    /// Interaction logic for Staging.xaml
    /// </summary>
    public partial class Staging : Page
    {
        public static Perf CachedPerformance; // For quick reloading of the system.
        public static String CachedPerformanceName;
        public StagingInfoViewModel stagingInfoViewModel;



        //==========================================================================================
        public partial class StagingInfo
        {
            public Object Title;
            public bool IsSingleRider;
            public ObservableCollection<CourseFilter> Filter = new ObservableCollection<CourseFilter>();
            public bool ShowVideo, ShowScenery, ShowDrafting, ShowManual, HideDirection;
            public bool NoSave;
            public ReportColumns DisplayList;


            public Course SavedCourse;
            public String Key;

            public String HelpFile;



            public bool WillCourseWork(Course c, out CourseFilter matched)
            {
                foreach (CourseFilter f in Filter)
                {
                    if (f.InFilter(c))
                    {
                        matched = f;
                        return true;
                    }
                }
                matched = null;
                return false;
            }
        }
        class StagingInfo_Ride3D : StagingInfo
        {
            public StagingInfo_Ride3D()
            {
                Title = "3D Cycling";
                ShowScenery = true;
                ShowDrafting = true;
                Filter.Add(CourseFilter.F_DistanceGrade);
                Key = "@3D";
                DisplayList = ReportColumns.Display_3DRoadRacing;
                HelpFile = "3D_Cycling.htm";
            }
        }
        class StagingInfo_RCV : StagingInfo
        {
            public StagingInfo_RCV()
            {
                Title = "Real Course Video";
                ShowVideo = true;
                Filter.Add(CourseFilter.F_RCV);
                Key = "@RCV";
                DisplayList = ReportColumns.Display_RCV;
                NoSave = true;
                HelpFile = "Real_Course_Video.htm";
            }
        }
        class StagingInfo_MultiRider : StagingInfo
        {
            public StagingInfo_MultiRider()
            {
                Title = "Classic MultiRider";
                IsSingleRider = true;
                ShowManual = true;
                Filter.Add(CourseFilter.F_DistanceGrade);
                Filter.Add(CourseFilter.F_TimeGrade);
                Filter.Add(CourseFilter.F_TimePercentAT);
                Key = "@MultiRider";
                DisplayList = ReportColumns.Display_3DRoadRacing;
                HelpFile = "Classic_MultiRider.htm";
            }
        }

        class StagingInfo_PowerTraining : StagingInfo
        {
            public StagingInfo_PowerTraining()
            {
                Title = "Power Training";
                IsSingleRider = true;
                ShowManual = true;
                Filter.Add(CourseFilter.F_DistanceGrade);
                Filter.Add(CourseFilter.F_TimeGrade);
                Filter.Add(CourseFilter.F_TimeWatts);
                Filter.Add(CourseFilter.F_DistanceWatts);
                Filter.Add(CourseFilter.F_TimePercentAT);
                Filter.Add(CourseFilter.F_DistancePercentAT);
                Filter.Add(CourseFilter.F_PerformanceGrade);
                Filter.Add(CourseFilter.F_PerformanceWatts);
                Key = "@PowerTraining";
                DisplayList = ReportColumns.Display_WattTestingAT;
                HelpFile = "Power_Training.htm";



                //taContext = new
            }
        }
        class StagingInfo_SpinScan : StagingInfo
        {
            public StagingInfo_SpinScan()
            {
                Title = "SpinScan";
                IsSingleRider = true;
                ShowManual = true;
                Filter.Add(CourseFilter.F_DistanceGrade);
                Filter.Add(CourseFilter.F_TimeGrade);
                Filter.Add(CourseFilter.F_PerformanceGrade);
                Key = "@SpinScan";
                DisplayList = ReportColumns.Display_SpinScan;
                HelpFile = "SpinScan.htm";
            }
        }


        public static StagingInfo Staging_Ride3D = new StagingInfo_Ride3D();
        public static StagingInfo Staging_RCV = new StagingInfo_RCV();
        public static StagingInfo Staging_MultiRider = new StagingInfo_MultiRider();
        public static StagingInfo Staging_PowerTraining = new StagingInfo_PowerTraining();
        public static StagingInfo Staging_SpinScan = new StagingInfo_SpinScan();
        //==========================================================================================
        public static UnitSave ms_DemoRestore;
        public static UnitSave ms_DemoNext;

        public static int RCVDemo = 0;
        public static double RCVDemo_StartAt;

        bool m_bLoaded;
        StagingInfo m_StagingInfo;

        bool m_bSaved = false;
        bool m_SelectedFileCanBeEdited = false;
        public bool SelectedFileCanBeEdited
        {
            get { return m_SelectedFileCanBeEdited; }
            set
            {
                if (value == true)
                { t_Edit.Visibility = Visibility.Visible; }
                else { t_Edit.Visibility = Visibility.Hidden; }
                m_SelectedFileCanBeEdited = value;
            }
        }

        public Staging(StagingInfo sinfo)
        {
            m_StagingInfo = sinfo;
            InitializeComponent();
            Loaded += Staging_Loaded;
        }

        void Staging_Loaded(object sender, RoutedEventArgs e)
        {
           // Debug.WriteLine("Staging loaded handling edit visibility");
            Type t = m_StagingInfo.GetType();
            if (SelectedFileCanBeEdited && t.Name == "StagingInfo_PowerTraining" || t.Name == "StagingInfo_Ride3D" || t.Name == "StagingInfo_SpinScan")
            {
                t_Edit.Visibility = Visibility.Visible;
            }
            else
            {
                t_Edit.Visibility = Visibility.Hidden;
            }

        }


        private void RiderOptions_Loaded(object sender, RoutedEventArgs e)
        {
            t_CoursePicker.BlockRedo = true;
            t_CourseViewer.CurrentCourse = null;
            t_CourseViewer.Zoom = false;
            m_bSaved = false;
            m_bLoaded = true;

            Binding binding;
            t_Title.Content = m_StagingInfo.Title;
            t_UnitsList.OnlyOne = m_StagingInfo.IsSingleRider;

            t_Filter.IsSynchronizedWithCurrentItem = true;  //I cannot stress more the importance of this little gem.
            binding = new Binding();//set binding parameters if necessary
            binding.Source = m_StagingInfo.Filter;
            t_Filter.SetBinding(ItemsControl.ItemsSourceProperty, binding);


            if (m_StagingInfo.Filter.Count > 1)
                l_Course.Visibility = Visibility.Hidden;
            else
                t_Filter.Visibility = Visibility.Hidden;

            t_UnitsList.OnlyPerformanceBot = m_StagingInfo == Staging_RCV;


            if (m_StagingInfo.ShowDrafting)
            {
                t_AllowDrafting.Visibility = Visibility.Visible;
                binding = new Binding("AllowDrafting");
                binding.Source = RM1_Settings.General;
                t_AllowDrafting.SetBinding(CheckBox.IsCheckedProperty, binding);
            }
            if (m_StagingInfo.HideDirection)
                t_CourseDirection.Visibility = Visibility.Collapsed;
            if (m_StagingInfo.ShowManual)
            {
                b_ManualGroup.Visibility = Visibility.Visible;
                t_StepGroup.Visibility = Visibility.Visible;

                binding = new Binding("WattsStep");
                binding.Source = RM1_Settings.General;
                t_WattsStep.SetBinding(TextBox.TextProperty, binding);
                t_WattsStepSlider.SetBinding(Slider.ValueProperty, binding);


                binding = new Binding("PercentATStep");
                binding.Source = RM1_Settings.General;
                t_PercentATStep.SetBinding(TextBox.TextProperty, binding);
                t_PercentATStepSlider.SetBinding(Slider.ValueProperty, binding);


                t_StepGroup.Visibility = Visibility.Visible;
                binding = new Binding(m_StagingInfo == Staging_PowerTraining ? "GradeStep" : "SS_GradeStep");
                binding.Source = RM1_Settings.General;
                binding.StringFormat = "0.#";
                t_GradeStep.SetBinding(TextBox.TextProperty, binding);
                t_GradeStepSlider.SetBinding(Slider.ValueProperty, binding);


                binding = new Binding(m_StagingInfo == Staging_PowerTraining ? "ManualControl" : "SS_ManualControl");
                binding.Source = RM1_Settings.General;
                t_ManualControl.SetBinding(CheckBox.IsCheckedProperty, binding);


                //t_WattsInitial.Text = RM1_Settings.General.WattsInitial.ToString();
                binding = new Binding("WattsInitial");
                binding.Source = RM1_Settings.General;
                t_WattsInitial.SetBinding(TextBox.TextProperty, binding);
                //t_PercentATInitial.Text = RM1_Settings.General.PercentATInitial.ToString();
                binding = new Binding("PercentATInitial");
                binding.Source = RM1_Settings.General;
                t_PercentATInitial.SetBinding(TextBox.TextProperty, binding);
                //t_GradeInitial.Text = m_StagingInfo == Staging_PowerTraining ? RM1_Settings.General.GradeInitial.ToString() : RM1_Settings.General.SS_GradeInitial.ToString();
                binding = new Binding(m_StagingInfo == Staging_PowerTraining ? "GradeInitial" : "SS_GradeInitial");
                binding.Source = RM1_Settings.General;
                t_GradeInitial.SetBinding(TextBox.TextProperty, binding);


                UpdateManualControl();
            }
            if (m_StagingInfo.ShowVideo)
            {
                t_Video.Visibility = Visibility.Visible;

            }
            if (m_StagingInfo.ShowScenery)
            {
                t_3DScenery.Visibility = Visibility.Visible;
                t_SceneryDropDown.SelectedItem = RM1_Settings.General.Scenery;
                t_SceneryDropDown.IsSynchronizedWithCurrentItem = true;  //I cannot stress more the importance of this little gem.
                binding = new Binding();//set binding parameters if necessary
                binding.Source = Controls.Render3D.SceneryInfo.SceneryList;
                t_SceneryDropDown.SetBinding(ItemsControl.ItemsSourceProperty, binding);

                binding = new Binding("Thumbnail");
                binding.Source = Controls.Render3D.SceneryInfo.SceneryList;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                t_SceneryImage.SetBinding(Image.SourceProperty, binding);
            }
            if (m_StagingInfo.NoSave)
            {
                t_SaveAs.Visibility = Visibility.Collapsed;
                t_Edit.Visibility = Visibility.Collapsed;
            }

            if (m_StagingInfo.DisplayList != null)
            {
                ReportColumns r;
                if (!ReportColumns.m_DB.TryGetValue(m_StagingInfo.DisplayList.Selected, out r))
                    r = null;
                binding = new Binding("SubItemList");
                binding.Source = m_StagingInfo.DisplayList;
                t_DisplayType.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                if (r != null)
                    t_DisplayType.SelectedItem = r;
                t_DisplayTypePanel.Visibility = m_StagingInfo.DisplayList.SubItemList.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            Controls.CoursePickerLine.UseRegistration = m_StagingInfo == Staging_RCV;


            ActiveUnits au;
            if (RM1_Settings.General.ActiveUnits.TryGetValue(m_StagingInfo.Key, out au))
                Unit.ActiveUnits = au;
            else
            {
                Debug.WriteLine("about to call atleastoneactive from staging");
                Unit.AtLeastOneActive();
                Debug.WriteLine("returned");
            }
            t_UnitsList.ActiveUnits = Unit.ActiveUnits;

            CourseFilter cf;
            if (RM1_Settings.General.CourseFilters.TryGetValue(m_StagingInfo.Key, out cf))
                t_Filter.SelectedItem = cf;
            UpdateFilter();

            //UpdateFilter();
            UpdateKeyControls();

            // Pick a course to display in the course displayer...thingy
            Course best = null;
            if ((best = Unit.PerformanceCourse(m_StagingInfo == Staging_RCV)) == null)
            {
                if (m_StagingInfo.SavedCourse == null)
                {
                    CourseInfo cinfo;
                    // First time through find a course to load... if we can.
                    if (RM1_Settings.General.SelectedCourse.TryGetValue(m_StagingInfo.Key, out cinfo))
                    {
                        if (cinfo != null)
                        {
                            foreach (Course course in Courses.AllCourses)
                            {
                                if (String.Compare(cinfo.FileName, course.FileName, true) == 0)
                                {
                                    best = course;
                                    if (t_CoursePicker.Shown(course))
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                    best = m_StagingInfo.SavedCourse;
            }

            string bestfilename;
            if (best == null && t_CoursePicker.List.Count > 0)
            {
                bestfilename = t_CoursePicker.List[0].FileName;
                t_CoursePicker.SelectedCourse = t_CoursePicker.List[0];
            }
            else if (best == null)
            {
                t_CoursePicker.SelectedCourse = null;
                bestfilename = null;

            }
            else
            {
                t_CoursePicker.SelectedCourse = best;
                bestfilename = best.FileName;
            }
            // disable/enable the edit button kludge
            string fnameextension = null;
            if (bestfilename != null)
            {
                fnameextension = System.IO.Path.GetExtension(bestfilename).ToUpper();
            }
           // Debug.WriteLine("Rideroptions_loaded handling edit button visibility" + fnameextension);
            if (fnameextension == ".RMC")
            {
               SelectedFileCanBeEdited = true;
                
            }
            else
            {
                SelectedFileCanBeEdited = false;
            }


            //if (best != null)
            //    Debug.WriteLine("best is not null, it is " + best.FileName);
            //else
            //    Debug.WriteLine("best is null");

            //if (best == null && t_CoursePicker.List.Count > 0)
            //{
            //    Debug.WriteLine("Should be picking first entry");
            //    best = t_CoursePicker.List[0];
            //}
            //StagingCurrentCourse = best;


            AppWin.Note(m_StagingInfo.Title.ToString());

            if (RCVDemo > 0)
            {
                Start_Click(null, null);
                RCVDemo = 0;
            }
            // by the time we get to here, best is either null or it is a full course or at least a course with correct filename but 
            // possibly wrong segments, let's load it fronm the file to get all segments.

            Course bestWithAllSegs = new Course();
            if (bestfilename != null && bestfilename != "")
            {
                bestWithAllSegs.Load(bestfilename);
            }
            else
            {
                bestWithAllSegs = null;
            }


            //best = StagingCurrentCourse;

            DispatcherTimer dt = new DispatcherTimer(DispatcherPriority.Normal);
            dt.Tick += (s, ev) =>
            {
                dt.Stop();
                t_CoursePicker.BlockRedo = false;
                m_AsCoursePickerSelection = true;
                StagingCurrentCourse = bestWithAllSegs;
                m_AsCoursePickerSelection = false;
            };
            dt.Interval = TimeSpan.FromMilliseconds(1000);
            dt.Start();

        }

        void Commit() { Commit(true); }
        void Commit(bool save)
        {
            t_dummy.Focus();
            /*
            t_GradeInitial.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            t_WattsInitial.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            t_PercentATInitial.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            t_GradeStep.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            t_WattsStep.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            t_PercentATStep.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            t_Laps.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            */

            CourseFilter cf = t_Filter.SelectedItem as CourseFilter;
            if (cf != null)
            {
                RM1_Settings.General.CourseFilters[m_StagingInfo.Key] = cf;
            }

            Unit.ActiveUnits = t_UnitsList.ActiveUnits;
            RM1_Settings.General.ActiveUnits[m_StagingInfo.Key] = Unit.ActiveUnits;

            if (save)
                RM1_Settings.SaveToFile();
        }
        private void RiderOptions_Unloaded(object sender, RoutedEventArgs e)
        {
            Controls.CoursePickerLine.UseRegistration = false;
            m_bLoaded = false;
            KeyControls = null;
            UpdateKeyControls();
            Courses.CancelLoadPerformance();
            if (!m_bSaved)
            {
                Commit(false);
                m_StagingInfo.SavedCourse = Unit.Manual ? Unit.Course : t_CourseViewer.CurrentCourse;
                if (m_StagingInfo.SavedCourse != null)
                    RM1_Settings.General.SelectedCourse[m_StagingInfo.Key] = new CourseInfo(m_StagingInfo.SavedCourse);
                RM1_Settings.SaveToFile();
            }
        }


        CourseFilter m_Filter = CourseFilter.F_DistanceGrade;
        CourseFilter Filter
        {
            get { return m_Filter; }
            set
            {
                if (m_Filter == value)
                    return;
                m_Filter = value;
                UpdateFilter();
            }
        }

       void UpdateFilter()  {
            CourseFilter f = m_Filter;
            if (f == null)
                f = CourseFilter.F_DistanceGrade;

            bool isperf = f == CourseFilter.F_PerformanceGrade || f == CourseFilter.F_PerformanceWatts;

            t_CourseDirection.Visibility = isperf || f == CourseFilter.F_RCV ? Visibility.Collapsed : Visibility.Visible;
            t_Laps.IsReadOnly = isperf;
            if (m_StagingInfo.ShowManual)
            {
                t_StepGradeGroup.Visibility = f.YUnit == CourseYUnits.Grade ? Visibility.Visible : Visibility.Collapsed;
                t_StepWattsGroup.Visibility = f.YUnit == CourseYUnits.Watts ? Visibility.Visible : Visibility.Collapsed;
                t_StepPercentATGroup.Visibility = f.YUnit == CourseYUnits.PercentAT ? Visibility.Visible : Visibility.Collapsed;
                l_ManualControl.Content = f.YUnit == CourseYUnits.Grade ? "Use handlebar controller to change grade" :
                    f.YUnit == CourseYUnits.PercentAT ? "Use handlebar controller to change %AT" :
                    "Use handlebar controller to change watts";
            }
            t_PC_Label_Column1.Content = f.Column1;
            t_PC_Label_Column2.Content = f.Column2;
            t_PC_Label_Column3.Content = f.Column3;

            t_RiderDisable.Visibility = isperf ? Visibility.Visible : Visibility.Collapsed;
            t_UnitsList.Opacity = t_RiderEdit.Opacity = isperf ? 0.3 : 1.0;

            bool perf = Filter == CourseFilter.F_PerformanceGrade || Filter == CourseFilter.F_PerformanceWatts;


            b_ManualGroup.Opacity = b_StepGroup.Opacity = f.ManualAvailable && !perf ? 1.0 : 0.3;
            r_ManualGroup.Visibility = r_StepGroup.Visibility = f.ManualAvailable && !perf ? Visibility.Collapsed : Visibility.Visible;

            String find = t_Find.Text.Trim();
            if (find != "")
            {
                Regex reg = new Regex(Regex.Escape(find), RegexOptions.IgnoreCase);
                f = (CourseFilter)f.Clone();
                f.Add(CourseFilter.FilterBy.Search, reg);
            }
            t_CoursePicker.Filter = f;

            ms_PickerSortUp = true;
            ms_PickerSortColumn = 0;
            SortPicker();

            UpdateManualControl();
        }

        void UpdateManualControl()
        {
            if (t_ManualControl.IsChecked == true && Filter != CourseFilter.F_PerformanceGrade && Filter != CourseFilter.F_PerformanceWatts)
            {
                t_CoursePicker_Disable.Visibility = Visibility.Visible;
                t_CourseViewer.Disable = true;
                t_StepGroup.Opacity = 1.0;
            }
            else
            {
                t_CoursePicker_Disable.Visibility = Visibility.Collapsed;
                t_CourseViewer.Disable = false;
                t_StepGroup.Opacity = 0.4;
            }
        }

        //=============================================================

        void OnPadKey(RM1.Trainer trainer, RM1.PadKeys key, double pressed)
        {
            // Determine if this trainer is OK to actually start the race...  First trainer or First active trainer.
            t_UnitsList.SetActiveUnits();
            if (m_ActivePads.Contains(trainer) && pressed == 0)
            {
                if (m_ActivePads.IndexOf(trainer) == 0 || Unit.RiderUnits.IndexOf(Unit.GetUnit(trainer)) == 0) // First unit always has control
                {
                    if (key == RM1.PadKeys.F1)
                        Start_Click(null, null);
                    if (m_KeyControls != null)
                        m_KeyControls(trainer, key, pressed);
                }
            }
        }

        RM1.TrainerPadKey m_KeyControls = null;
        public RM1.TrainerPadKey KeyControls
        {
            get { return m_KeyControls; }
            set
            {
                if (value == m_KeyControls)
                    return;
                m_KeyControls = value;
                UpdateKeyControls();
            }
        }

        List<RM1.Trainer> m_ActivePads = new List<RM1.Trainer>();
        void UpdateKeyControls()
        {
            foreach (RM1.Trainer trainer in m_ActivePads)
                trainer.OnPadKey -= new RM1.TrainerPadKey(OnPadKey);
            m_ActivePads.Clear();
            if (!m_bLoaded)
                return;
            t_UnitsList.SetActiveUnits();
            foreach (Unit unit in Unit.Units)
            {
                if (unit.Trainer != null && unit.Trainer.IsConnected)
                    m_ActivePads.Add(unit.Trainer);
            }
            foreach (RM1.Trainer trainer in m_ActivePads)
                trainer.OnPadKey += new RM1.TrainerPadKey(OnPadKey);
        }
        //=============================================================


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //I want to wipe out the list of courses

            // Give it time to sync the variables.
            // Debug.WriteLine("starting the click sequence");
            //Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
            //{
            Commit(false); // Make sure everthing is ready.
            Course cc = t_CourseViewer.CurrentCourse;
            if (cc == null && t_ManualControl.IsChecked == false)
            {
                Debug.WriteLine("there is no course and no manual control, so I should be exiting");
                return;
            }

            CourseFilter filter = m_StagingCurrentCourseFilter == null ? Filter : m_StagingCurrentCourseFilter;
            if (cc == null && (m_StagingInfo == Staging_PowerTraining || m_StagingInfo == Staging_SpinScan) && t_ManualControl.IsChecked == true)
                filter = Filter;
            else if (!filter.InFilter(cc, true))
            {
                // OK figure out the correct filter.
                filter = null;
                foreach (CourseFilter f in m_StagingInfo.Filter)
                {
                    if (f.InFilter(cc))
                    {
                        filter = f;
                        break;
                    }
                }
                if (filter == null)
                    return;
            }

            if (m_StagingInfo == Staging_PowerTraining)
            {
                if (filter == CourseFilter.F_PerformanceGrade || filter == CourseFilter.F_PerformanceWatts)
                {
                    Pages.Modes.PowerTraining_Review ride = new Pages.Modes.PowerTraining_Review();
                    ride.Course = t_CourseViewer.CurrentCourse;
                    //ride.Course = t_CourseViewer.ModifiedCourse;

                    if (Staging.CachedPerformance != null && Staging.CachedPerformanceName == t_CourseViewer.CurrentCourse.FileName)
                    {
                        //Debug.WriteLine("cached performance");
                        ride.Performance = Staging.CachedPerformance;
                        NavigationService.Navigate(ride);
                    }
                    else
                    {
                        // Debug.WriteLine("not cached performance");
                        Pages.Modes.Loader loader = new Pages.Modes.Loader("Loading performance", ride, ride,
                            new DoWorkEventHandler(ride.LoadPerformance), null, null);
                        NavigationService.Navigate(loader);
                    }
                    CachedPerformance = null;
                }
                else
                {
                    Unit.ActivateOneActive();
                    Pages.Modes.PowerTraining ride = new Pages.Modes.PowerTraining();
                    bool manual = t_ManualControl.IsChecked == true;
                    Unit.Manual = manual;
                    Course course = manual ?
                        Course.CreateAsYouGo(Filter.XUnit, Filter.YUnit, Unit.RiderUnits[0]) :
                        t_CourseViewer.ModifiedCourse;
                    if (course != null)
                    {
                        Log.WriteLine("PowerTraining launched with manual = " + manual.ToString() + " course = " + course.FileName);
                    }
                    else
                    {
                        Log.WriteLine("PowerTraining launched with manual = " + manual.ToString() + " course = null");
                    }
                    Unit.Course = course;
                    NavigationService.Navigate(ride);
                    CachedPerformance = null;
                }
                //Debug.WriteLine("powertraining is setup");
            }
            else if (m_StagingInfo == Staging_SpinScan)
            {
                Unit.ActivateOneActive();
                if (filter == CourseFilter.F_PerformanceGrade)
                {
                    Pages.Modes.SpinScan_Review ride = new Pages.Modes.SpinScan_Review();
                    ride.Course = t_CourseViewer.CurrentCourse;

                    if (Staging.CachedPerformance != null)
                    {
                        ride.Performance = Staging.CachedPerformance;
                        NavigationService.Navigate(ride);
                    }
                    else
                    {
                        Pages.Modes.Loader loader = new Pages.Modes.Loader("Loading performance", ride, ride,
                            new DoWorkEventHandler(ride.LoadPerformance), null, null);

                        NavigationService.Navigate(loader);
                    }
                    CachedPerformance = null;
                }
                else
                {
                    Pages.Modes.SpinScan ride = new Pages.Modes.SpinScan();
                    bool manual = t_ManualControl.IsChecked == true;
                    Unit.Manual = manual;
                    Course course = manual ?
                        Course.CreateAsYouGo(Filter.XUnit, Filter.YUnit, Unit.RiderUnits[0]) :
                        t_CourseViewer.ModifiedCourse;
                    Unit.Course = course;
                    NavigationService.Navigate(ride);
                }
                CachedPerformance = null;
            }
            else if (m_StagingInfo == Staging_RCV)
            {
                Unit.FixRiderList();
                Unit.Course = t_CourseViewer.ModifiedCourse; // Start that loading right away...
                if (t_CourseViewer.Zoom)
                    t_CourseViewer.Save();

                if ((Unit.RaceUnit.Count > 1 && RCVDemo == 0) || RCVDemo > 1)
                {
                    Pages.Modes.RCV_Split ride2 = new Pages.Modes.RCV_Split();
                    if (RCVDemo > 0)
                        ride2.StartDemo = true;
                    ride2.OriginalCourse = t_CourseViewer.CurrentCourse;
                    NavigationService.Navigate(ride2);
                }
                else
                {
                    Pages.Modes.RCV ride = new Pages.Modes.RCV();
                    if (RCVDemo > 0)
                        ride.StartDemo = true;
                    ride.OriginalCourse = t_CourseViewer.CurrentCourse;
                    NavigationService.Navigate(ride);
                }
                CachedPerformance = null;

            }
            else if (m_StagingInfo == Staging_MultiRider)
            {
                CachedPerformance = null;
            }
            else if (m_StagingInfo == Staging_Ride3D)
            {
                Commit(false); // Make sure everthing is ready.
                Unit.FixRiderList();

                //Can I wipe out all the courses?
                //int i = 0;
                //int maxcourses = Courses.AllCourses.Count;
                //Course testagainst = StagingCurrentCourse;
                //Debug.WriteLine("I think I am wiping out " + maxcourses + " courses." + Environment.NewLine);
                //Courses.AllCourses.RemoveAll(item => item != testagainst);
                //var itemsToRemove = Courses.AllList.Where(item => item != testagainst).ToList();

                //foreach (var itemToRemove in itemsToRemove)
                //{
                //    Courses.AllList.Remove(itemToRemove);
                //}

                //Debug.WriteLine("the Courses.AllCourses list is " + Courses.AllCourses.Count +" long");
                //maxcourses = Courses.AllCourses.Count;
                //for (i = 0; i < maxcourses; i++)
                //{
                //    if (Courses.AllCourses.ElementAt(i) != null)
                //    {
                //        Course ccc = Courses.AllCourses.ElementAt(i);
                //        Debug.WriteLine("THis one is not null : " + System.IO.Path.GetFileName(ccc.FileName));

                //    }
                //}
                //maxcourses = Courses.AllList.Count;
                //for (i = 0; i < maxcourses; i++)
                //{
                //    if (Courses.AllList.ElementAt(i) != null)
                //    {
                //        Course ccc = Courses.AllCourses.ElementAt(i);
                //        Debug.WriteLine("THis one is not null : " + System.IO.Path.GetFileName(ccc.FileName));

                //    }
                //}
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();


                Pages.Modes.Ride3D ride = new Pages.Modes.Ride3D();
                Controls.Render3D.Course = Unit.Course = t_CourseViewer.ModifiedCourse; // Start that loading right away...
                PerformanceBot.StartDistance = t_CourseViewer.CurrentCourse.StartAt;

                // See if there is any performance bots in the list.		
                NavigationService.Navigate(ride.LoadScreen());
                // Debug.WriteLine("the loadscreen has logically returned");
                CachedPerformance = null;
            }
            // Debug.WriteLine("done deciding what to show. Unit.Manual = " + Unit.Manual.ToString());
            m_StagingInfo.SavedCourse = Unit.Manual ? Unit.Course : t_CourseViewer.CurrentCourse;
            RM1_Settings.General.SelectedCourse[m_StagingInfo.Key] = new CourseInfo(m_StagingInfo.SavedCourse);

            RM1_Settings.SaveToFile();
            m_bSaved = true;
            //});
            //Debug.WriteLine("beyond the thread delegate");
        }

        //=============================================================
        private void Options_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.RideOptions());
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Commit();
            NavigationService.GoBack();
        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            AppWin.Help(m_StagingInfo.HelpFile);
        }
        //==============================================================
        private void RidersEdit_Click(object sender, RoutedEventArgs e)
        {
            Commit();
            Pages.SelectRider page = new Pages.SelectRider();
            if (m_StagingInfo == Staging_RCV)
            {
                page.VideoMode = true;
            }

            page.Mode = m_StagingInfo.Key;
            AppWin.Instance.MainFrame.Navigate(page);
        }

        private void CourseViewer_NeedSaveChanged(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("-----------NeedSaveChanged fired  state = " + t_CourseViewer.NeedSave.ToString());
            bool ns = t_CourseViewer.NeedSave;
            t_SaveAs.Visibility = ns ? Visibility.Visible : Visibility.Hidden;
        }
        private void CourseViewer_StartChanged(object sender, RoutedEventArgs e)
        {
            if (m_StagingInfo.ShowVideo && t_CourseViewer.CurrentCourse != null)
            {
                t_RCVPreview.Location = t_CourseViewer.CurrentCourse.StartAt;
            }

        }

        bool m_bShowAll = true;
        private void Course_ShowAll_Click(object sender, RoutedEventArgs e)
        {
            if (!m_bShowAll)
            {
                m_bShowAll = true;
                t_Course_ShowAll.Selected = true;
                t_Course_Zoom.Selected = false;
                t_CourseViewer.Zoom = false;
            }
        }

        private void Course_Zoom_Click(object sender, RoutedEventArgs e)
        {
            if (m_bShowAll)
            {
                m_bShowAll = false;
                t_Course_ShowAll.Selected = false;
                t_Course_Zoom.Selected = true;
                t_CourseViewer.Zoom = true;
            }

        }
        private void SS_ManualControl_Click(object sender, RoutedEventArgs e) { UpdateManualControl(); }


        private void t_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // t_Edit.Visibility = Visibility.Visible;
            CourseFilter f = t_Filter.SelectedItem as CourseFilter;
            //Debug.WriteLine("the course filter has changed");
            if (f == CourseFilter.F_PerformanceWatts || f == CourseFilter.F_PerformanceGrade)
            {
                Courses.LoadPerformances();
                t_Edit.Visibility = Visibility.Hidden;
            }
            else
            {
                Courses.CancelLoadPerformance();
            }
            if (f != null)
                Filter = f;
            // add this to alpha sort ascending
            //ms_PickerSortUp = true;
            //ms_PickerSortColumn = 0;
            //SortPicker();

        }

        CourseFilter m_StagingCurrentCourseFilter;
        CourseInfo m_StagingCurrentCourseInfo;
        public Course StagingCurrentCourse
        {
            get { t_CourseViewer.Save(); return t_CourseViewer.CurrentCourse; }
            set
            {
                //Debug.WriteLine("setting the StagingCurrentCourse");
                //t_CourseViewer.Save();

                Course cvalue = value;

                if (m_StagingInfo == Staging_RCV)
                {
                    // We cannot have a null course... very bad in video land.
                    if (cvalue != null)
                        cvalue = cvalue.VideoCourse;
                    if (cvalue == null)
                        cvalue = Courses.FirstVideoCourse;
                }
                if (cvalue != null)
                {
                    //Debug.WriteLine("Course is not null");
                    m_bShowAll = true;
                    t_Course_ShowAll.Selected = true;
                    t_Course_Zoom.Selected = false;

                    m_StagingCurrentCourseFilter = Filter;
                    //Debug.WriteLine("calling setcourse : " + Environment.NewLine +
                    //                "Original Reverse = " + cvalue.OriginalReverse.ToString() + Environment.NewLine +
                    //                "Original Mirror = " + cvalue.OriginalMirror.ToString() + Environment.NewLine +
                    //                "Original StartAt = " + cvalue.OriginalStartAt.ToString() + Environment.NewLine +
                    //                "Original Endat = " + cvalue.OriginalEndAt.ToString() + Environment.NewLine );
                    ////t_CourseViewer.SetCourse(cvalue, cvalue.OriginalReverse, cvalue.OriginalMirror, false, cvalue.OriginalStartAt, cvalue.OriginalEndAt);

                    t_CourseViewer.SetCourse(cvalue, cvalue.Reverse, cvalue.Mirror, false, cvalue.StartAt, cvalue.EndAt);
                    m_StagingCurrentCourseInfo = new CourseInfo(cvalue);

                    //Debug.WriteLine("   now changing direction selector");
                    m_bInUpdateCourseDirection = true; //do this to prevent the service for the event on Selectedindex cahnged.
                    if (m_AsCoursePickerSelection)
                        t_CourseDirection.SelectedIndex = cvalue.Mirror ? 2 : cvalue.Reverse ? 1 : 0;
                    //if the course does not change the direction, the event may not fire, so lets force it.
                    m_bInUpdateCourseDirection = false; // re-enable
                    UpdateCourseDirection(false); //because this is the result of a change in selected course, I don't want a re-compute of teh mirroring.

                    t_Laps.Text = cvalue.StringLaps;
                    if (m_StagingInfo.ShowVideo)
                    {
                        Course c = cvalue;
                        Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
                        {
                            t_RCVPreview.Course = null;
                            t_RCVPreview.Course = c == null ? null : c.VideoCourse;
                        });
                    }
                    //Debug.WriteLine("ABOUT TO MAKE change to course t_CoursePicker.selectedcourse");
                    // PAS: removed this because the t_CoursePicker is no longer a proper collection of real courses.
                    // we're loading the list with everything-but-segments version of courses and loading segments only on selection
                    // leaving in this line means the selector gets confused. It's been updated just fine thanks.
                    //t_CoursePicker.SelectedCourse = cvalue;
                }
                else
                {
                    //Debug.WriteLine(" a null course was selected");
                    t_CourseViewer.SetCourse(null, false, false, false, 0.0, 0.0);
                    m_StagingCurrentCourseInfo = null;

                    t_Laps.Text = "";
                    t_Course_Zoom.Selected = false;
                    t_CourseDirection.SelectedIndex = 0;
                    if (m_StagingInfo.ShowVideo)
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
                        {
                            t_RCVPreview.Course = null;
                        });
                    }
                    // PAS: removed this because the t_CoursePicker is no longer a proper collection of real courses.
                    // we're loading the list with everything-but-segments version of courses and loading segments only on selection
                    // leaving in this line means the selector gets confused. It's been updated just fine thanks.

                    //t_CoursePicker.SelectedCourse = cvalue;
                }
                //t_UnitsList.AdjustCourse(CurrentCourse);
                Unit.ClearPerformanceBots(StagingCurrentCourse, m_StagingInfo == Staging_RCV);
                t_UnitsList.Refresh();

            }
        }
        private Boolean m_AsCoursePickerSelection = false;
        private void t_CoursePicker_CourseSelected(object sender, RoutedEventArgs e)
        {
           // Debug.WriteLine("a course has been selected by a selection event");
            m_AsCoursePickerSelection = true;
            //now I should actually read the file and use it for the StagingCurrent course

            string fname = t_CoursePicker.SelectedCourse.FileName;
            string fnameextension = System.IO.Path.GetExtension(fname);
           // Debug.WriteLine("the extension in toupper is " + fnameextension.ToUpper());
            if (fnameextension.ToUpper() == ".RMC")
            {
                SelectedFileCanBeEdited = true;
            }
            else
            {
                SelectedFileCanBeEdited = false;
            }


            //Debug.WriteLine("selected Course filename is " + t_CoursePicker.SelectedCourse.FileName);
            Course ccNoSegs = new Course();

            if (ccNoSegs.Load(fname))
            {
                //Courses.Replace(ccNoSegs);
                StagingCurrentCourse = ccNoSegs;
            }
            else
            {
                StagingCurrentCourse = null;
            }

            //StagingCurrentCourse = t_CoursePicker.SelectedCourse;

            m_AsCoursePickerSelection = false;

            if (StagingCurrentCourse == null) Debug.WriteLine("the course selected is null");

        }

        void UpdateSave()
        {
            //Debug.WriteLine("UpdateSaveCalled");
            t_SaveAs.Visibility = t_CourseViewer.NeedSave ? Visibility.Visible : Visibility.Hidden;
        }
        public void Edit_Click(object sender, RoutedEventArgs e)
        {
            //Commit();
            //#if DEBUG
            //    RacerMateOne.Pages.Modes.CourseEditor page = new Pages.Modes.CourseEditor();
            //    page.Course = StagingCurrentCourse;
            //#else
            //    RacerMateOne.Pages.ComingSoon page = new ComingSoon("Course Editor", "Course_Creator.htm", null);
            //#endif
            // Course ee = StagingCurrentCourse;
            if (StagingCurrentCourse == null)
                return;

            RacerMateOne.CourseEditorDev.CourseEditor page = new RacerMateOne.CourseEditorDev.CourseEditor(StagingCurrentCourse.FileName);
            AppWin.Instance.MainFrame.Navigate(page);
        }

        private void Laps_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int laps = 1;
                if (t_Laps.Text != null || t_Laps.Text != "")
                {
                    laps = Convert.ToInt32(t_Laps.Text);
                }
                t_CourseViewer.Laps = laps;
                UpdateSave();
            }
            catch
            {
                t_CourseViewer.Laps = 1;
                UpdateSave();
            }
        }

        Course m_MirroredOriginalCourse;
        Course m_MirroredNewCourse;

        bool m_bInUpdateCourseDirection;
        void UpdateCourseDirection(Boolean ReCalcMirror)
        {
            //Set RecalcMirror true only on Change of direction forced by user click-change of the selector box. This will give a new course for the range between start and stop
            // if called from code, set to false so that the course with previous mirroring is shown and limits are as saved.
            //Yes this is a kludge.
            if (t_CourseViewer == null)
                return;
            if (m_bInUpdateCourseDirection)
                return;
            m_bInUpdateCourseDirection = true;
            //Debug.WriteLine("In update courswe direction, recalc mirror= " + ReCalcMirror.ToString());
            try
            {
                int n = t_CourseDirection.SelectedIndex;
                switch (n)
                {
                    case 0:
                        if (m_MirroredNewCourse != null && m_MirroredNewCourse == t_CourseViewer.CurrentCourse)
                        {
                            //Debug.WriteLine("reverting to the MirroredOriginal course from 0");
                            StagingCurrentCourse = m_MirroredOriginalCourse;
                            m_MirroredNewCourse = null;
                        }
                        //Debug.WriteLine("..and setting false mirror false reverse from 0");
                        t_CourseViewer.SetFlags(false, false);

                        break; // CourseViewer.Mirror = false; CourseViewer.Reverse = false; break;
                    case 1:
                        if (m_MirroredNewCourse != null && m_MirroredNewCourse == t_CourseViewer.CurrentCourse)
                        {
                            // Debug.WriteLine("reverting to the MirroredOriginal course from 1");
                            StagingCurrentCourse = m_MirroredOriginalCourse;
                            m_MirroredNewCourse = null;
                            //t_CourseViewer.SetFlags(false, true);
                            //t_CourseDirection.SelectedIndex = 1;
                        }
                        //else
                        //Debug.WriteLine("..and setting false mirror, true reverse from 1");
                        t_CourseViewer.SetFlags(false, true);
                        break; // CourseViewer.Mirror = false; CourseViewer.Reverse = true; break;
                    case 2: //Fwd+Mirror
                        // 
                        if (ReCalcMirror && (t_CourseViewer.StartFlag.Value > 0 || t_CourseViewer.EndFlag.Value < 1))
                        {
                            // Special case... create a new course.
                            // Debug.WriteLine("*************** this case creates a whole new course from 2");
                            t_CourseViewer.Save();
                            Course c = t_CourseViewer.CurrentCourse;
                            Course nc = new Course(c, c.Reverse, false, true, c.StartAt, c.EndAt);
                            // Debug.WriteLine(" the new climb is : " + nc.ClimbY);
                            m_MirroredOriginalCourse = c;
                            m_MirroredNewCourse = nc;
                            nc.Mirror = true;
                            StagingCurrentCourse = nc;
                        }
                        else
                            //Debug.WriteLine("We do not get a whole new course, since the end points are 0 and 1, or ReCalcMirror is false from 2");

                            t_CourseViewer.SetFlags(true, false);
                        break; // CourseViewer.Mirror = true; CourseViewer.Reverse = false; break;
                }
            }
            catch { }
            m_bInUpdateCourseDirection = false;
        }


        private void CourseDirection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Debug.WriteLine(" 242424242424242  the course direction has changed to " + t_CourseDirection.SelectedIndex);

            UpdateCourseDirection(true);
        }

        private void DisplayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReportColumns r = t_DisplayType.SelectedItem as ReportColumns;
            if (r != null && m_StagingInfo.DisplayList != null)
                m_StagingInfo.DisplayList.Selected = r.Key;
        }

        private void SceneryDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RM1_Settings.General.Scenery = t_SceneryDropDown.SelectedItem as Controls.Render3D.SceneryInfo;
        }

        private static String ms_SavePath;


        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            // Debug.WriteLine("saving from Staging");
            Commit();
            Course course = t_CourseViewer.CurrentCourse;
            // Course course = t_CourseViewer.ModifiedCourse;
            // note the above saves the course in a format that never shows reverse true nor mirror true, it is full-on linear.
            // Options.CourseViewer.Course
            if (ms_SavePath == null)
                ms_SavePath = RacerMatePaths.CoursesFullPath;
            SaveFileDialog f = new SaveFileDialog();
            //String full = course.FileName;
            String full = System.IO.Path.Combine(ms_SavePath, course.FileName);
            String ext = System.IO.Path.GetExtension(full).ToUpper();
            if (String.Compare(ext, ".RMC", true) != 0)
            {
                full = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(full),
                    System.IO.Path.GetFileNameWithoutExtension(full) + ".rmc");
            }
            f.FileName = full;
            f.CheckFileExists = false;
            f.CheckPathExists = false;
            f.Title = "Save this course";
            f.AddExtension = true;
            f.DefaultExt = ".rmc";
            f.Filter = "Course Files (.rmc)|*.rmc";
            if (f.ShowDialog() == true)
            {
                ms_SavePath = System.IO.Path.GetDirectoryName(f.FileName);
                course.Save(f.FileName, course.Description);
                Course c = new Course();
                if (c.Load(f.FileName))
                {
                    Courses.Replace(c);
                    StagingCurrentCourse = c;
                }
            }
        }


        void SortPicker()
        {
            if (ms_PickerSortColumn == 0)
                ms_PickerSortUp = false;
            t_CoursePicker.Set(Filter, ms_PickerSortColumn, ms_PickerSortUp);
            t_PC_Label_Column1.FontWeight = ms_PickerSortColumn == 1 ? FontWeights.Bold : FontWeights.Normal;
            t_PC_Label_Column2.FontWeight = ms_PickerSortColumn == 2 ? FontWeights.Bold : FontWeights.Normal;
            t_PC_Label_Column3.FontWeight = ms_PickerSortColumn == 3 ? FontWeights.Bold : FontWeights.Normal;
        }

        //=========================================================================================================


        static int ms_PickerSortColumn = 0;
        static bool ms_PickerSortUp = false;
        //=============
        private void PC_Label_Column1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ms_PickerSortColumn != 1)
            {
                ms_PickerSortColumn = 1;
                ms_PickerSortUp = false;
            }
            else if (ms_PickerSortUp == false)
                ms_PickerSortUp = true;
            else
                ms_PickerSortColumn = 0;
            SortPicker();
        }
        private void PC_Label_Column2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ms_PickerSortColumn != 2)
            {
                ms_PickerSortColumn = 2;
                ms_PickerSortUp = false;
            }
            else if (ms_PickerSortUp == false)
                ms_PickerSortUp = true;
            else
                ms_PickerSortColumn = 0;
            SortPicker();

        }

        private void PC_Label_Column3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ms_PickerSortColumn != 3)
            {
                ms_PickerSortColumn = 3;
                ms_PickerSortUp = false;
            }
            else if (ms_PickerSortUp == false)
                ms_PickerSortUp = true;
            else
                ms_PickerSortColumn = 0;
            SortPicker();
        }

        static OpenFileDialog ms_BrowseDialog;

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            //Controls.CoursePickerLine lastAdded = m_LastAdded; // This will be use to compare a successful load
            //OK... Display the course Browser.
            OpenFileDialog f;
            if (ms_BrowseDialog == null)
            {
                f = ms_BrowseDialog = new OpenFileDialog();

                f.Multiselect = false;
                f.Title = "Select program to convert";
                f.ValidateNames = true;
                f.AddExtension = true;
                f.CheckFileExists = true;
                f.DefaultExt = ".3dc;.crs;.dnw;.erg;.tng;.mrc;.rmp;.rmc";
                f.Filter = "Racermate Files(*.3dc,*.crs,*.dnw,*.erg,*.tng,*.mrc,*.rmp,*.rmc)|*.3dc;*.crs;*.dnw;*.erg;*.tng;*.mrc;*.rmp;*.rmc|All Files|*.*";
            }
            else
                f = ms_BrowseDialog;

            if (m_StagingInfo == Staging_RCV)
                f.Filter = "Real Course Videos(*.avi)|*.avi|All Files|*.*";
            else if (m_StagingInfo == Staging_Ride3D)
                f.Filter = "Racermate Files(*.3dc,*.crs,*.dnw,*.erg,*.tng,*.mrc,*.rmc)|*.3dc;*.crs;*.dnw;*.erg;*.tng;*.mrc;*.rmc|All Files|*.*";
            else
                f.Filter = "Racermate Files(*.3dc,*.crs,*.dnw,*.erg,*.tng,*.mrc,*.rmp,*.rmc)|*.3dc;*.crs;*.dnw;*.erg;*.tng;*.mrc;*.rmp;*.rmc|All Files|*.*";

            f.InitialDirectory = RacerMatePaths.BrowsePath;
            if (f.ShowDialog() == true)
            {
                RacerMatePaths.BrowsePath = f.InitialDirectory;

                string srcfilename = System.IO.Path.GetFileName(f.FileName);
                string ext = System.IO.Path.GetExtension(srcfilename);
                string newext = ext.Replace(".", "_");
                string fname = srcfilename.Replace(ext, newext + ".rmc");
                Course cOld = new Course();
                Course c = null;
                if (cOld.Load(f.FileName))
                {
                    bool isperf = false;
                    RacerMateOne.Dialogs.Ask ask = new RacerMateOne.Dialogs.Ask("Do you want a permanent copy of this course?", "Yes", "No");
                    if (m_StagingInfo == Staging_RCV)
                        ask.IsOK = false;
                    else
                    {
                        if (cOld.PerformanceInfo != null)
                        {
                            ask = new RacerMateOne.Dialogs.Ask("Do you want to copy this file into your performance directory?", "Yes", "No");
                            isperf = true;
                        }
                        ask.ShowDialog();
                    }

                    if (ask.IsOK)
                    {
                        if (isperf)
                        {
                            String destname = System.IO.Path.GetFileName(f.FileName);
                            destname = System.IO.Path.Combine(RacerMatePaths.PerformancesFullPath, destname);
                            File.Copy(f.FileName, destname);
                            Course cc = new Course();
                            if (cc.Load(destname))
                            {
                                Courses.Add(cc);
                                c = cc;
                            }
                        }
                        else
                        {
                            string description = cOld.Description;
                            if (description.Length == 0)
                                description = "Converted from " + f.FileName;
                            if (cOld.Save(RacerMatePaths.CoursesFullPath + "\\" + fname, description))
                            {
                                Course cc = new Course();
                                if (cc.Load(RacerMatePaths.CoursesFullPath + "\\" + fname))
                                {
                                    Courses.Add(cc);
                                    c = cc;
                                }
                            }
                        }
                    }
                    else
                    {
                        Courses.Add(cOld);
                        c = cOld;
                        if (m_StagingInfo == Staging_RCV)
                        {
                            Commit();
                            RM1_Settings.SaveToFile();
                        }
                    }
                    if (c == null)
                        c = cOld;

                }
                else
                {
                    // We failed.
                    RacerMateOne.Dialogs.Ask ask = new RacerMateOne.Dialogs.Ask("Could not load this course", null, "OK");
                    ask.ShowDialog();
                    return;
                }

                // Go through the filter list and see if this course matches it.
                if (c != null && !m_Filter.InFilter(c))
                {
                    CourseFilter ft = null;
                    // Go backwards through the list... since this way performances will use the performance filter.
                    for (int i = m_StagingInfo.Filter.Count - 1; i >= 0; i--)
                    {
                        CourseFilter filter = m_StagingInfo.Filter[i];
                        if (filter.InFilter(c))
                        {
                            ft = filter;
                            break;
                        }
                    }
                    if (ft == null)
                    {
                        RacerMateOne.Dialogs.Ask ask = new RacerMateOne.Dialogs.Ask("This mode doesn't support this course type.  You can access this course in PowerTraining mode.", null, "OK");
                        ask.ShowDialog();
                        t_CoursePicker.RedoList();
                    }
                    else
                    {
                        Filter = ft; // Select this filter
                        t_Filter.SelectedItem = ft;
                        t_CoursePicker.RedoList();
                        t_CoursePicker.SelectedCourse = c;
                        StagingCurrentCourse = c;
                    }
                }
                else
                {
                    t_CoursePicker.RedoList();
                    t_CoursePicker.SelectedCourse = c;
                    StagingCurrentCourse = c;
                }
            }
        }

        private void t_PC_Label_Column_MouseEnter(object sender, MouseEventArgs e)
        {
            Label label = (Label)sender;
            label.Foreground = Brushes.White;

        }

        private void t_PC_Label_Column_MouseLeave(object sender, MouseEventArgs e)
        {
            Label label = (Label)sender;
            label.Foreground = AppWin.StdBrush_Dark;
        }

        private void t_PC_Label_Box_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            border.Background = AppWin.StdBrush_ButtonBackground;
        }

        private void t_PC_Label_Box_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            border.Background = Brushes.Transparent;
        }

        private void t_Find_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFilter();
        }

    }

    public class StagingInfoViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        Visibility powerTraining = Visibility.Hidden;
        public Visibility PowerTraining
        {
            get { return powerTraining; }
            set
            {
                PowerTraining = value;
                OnPropertyChanged("PowerTraining");
            }
        }
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
