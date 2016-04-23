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
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Threading;
using System.Threading;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace RacerMateOne.Pages
{
	/// <summary>
	/// Interaction logic for RideOptionsAll.xaml
	/// </summary>
	public partial class RideOptionsAll : Page
	{
		static private String[] ms_Empty = new String[] {};
		static private String[] ms_AllTag = new String[] { "@All" };
		private String[] m_Tags = new String[] { };
		private String[] m_NextTags = null;
		private Dictionary<String,List<OptionGroup>> m_TagMap = new Dictionary<String,List<OptionGroup>>();
		private List<OptionGroup> m_All;
		private Controls.HardwareLine[] m_HardwareLines;
        private bool m_bLoaded=false;

		int TagVersion = 0;

		public bool TagOn( String tag )
		{
			foreach(String s in m_Tags)
			{
				if (String.Compare( s, tag, true ) == 0)
					return true;
			}
			return false;
		}
		
		public class OptionGroup 
		{
			public int Version;
			public Grid MainElement;
			public OptionGroup( Grid g )
			{
				MainElement = g;
			}
		}

		//===========================================================================
		

        //void OnPadKey(RM1.Trainer trainer, RM1.PadKeys key, double pressed)
        //{
        //    // Determine if this trainer is OK to actually start the race...  First trainer or First active trainer.
        //    //UnitsList.SetActiveUnits();
        //    //Debug.WriteLine("onpadkey pressed");
        //    //if (m_ActivePads.Contains(trainer) && m_KeyControls != null && pressed == 0)
        //    //{
        //    //    if (m_ActivePads.IndexOf(trainer) == 0 || Unit.RiderUnits.IndexOf(Unit.GetUnit(trainer)) == 0) // First unit always has control
        //    //        m_KeyControls(trainer, key, pressed);
        //    //}
        //}

        //RM1.TrainerPadKey m_KeyControls = null;
        //public RM1.TrainerPadKey KeyControls
        //{
        //    get { return m_KeyControls; }
        //    set
        //    {
        //        if (value == m_KeyControls)
        //            return;
        //        m_KeyControls = value;
        //        UpdateKeyControls();
        //    }
        //}

        //List<RM1.Trainer> m_ActivePads = new List<RM1.Trainer>();
        //void UpdateKeyControls()
        //{
        //    //Debug.WriteLine("in update keycontrols");
        //    //if (!m_bLoaded)
        //    //    return;
        //    //foreach (RM1.Trainer trainer in m_ActivePads)
        //    //    trainer.OnPadKey -= new RM1.TrainerPadKey(OnPadKey);
        //    //m_ActivePads.Clear();
        //    //if (m_KeyControls == null)
        //    //    return;
        //    ////UnitsList.SetActiveUnits();
        //    //foreach (Unit unit in Unit.Units)
        //    //{
        //    //    if (unit.Trainer != null && unit.Trainer.IsConnected)
        //    //        m_ActivePads.Add(unit.Trainer);
        //    //}
        //    //foreach(RM1.Trainer trainer in m_ActivePads)
        //    //    trainer.OnPadKey += new RM1.TrainerPadKey(OnPadKey);
        //}
		//===========================================================================


        //String m_CourseSave;
		public String[] Tags
		{
			get { return m_Tags; }
			set
			{
				m_Tags = value;
				RedoTags();
				FadeOut.Stop();
				riderOptionsAll.Opacity = 1;
                //m_CourseSave = m_Tags.Length > 0 ? m_Tags[0] : "Unknown";
                //if (m_CourseSave == "@PowerTraining" || m_CourseSave == "@MultiRider" || m_CourseSave == "@SpinScan")
                //{
                //    if ((CourseTypeFilter & CourseType.Watts) == CourseType.Watts)
                //        m_CourseSave += "_Watts";
                //    if ((CourseTypeFilter & CourseType.Performance) == CourseType.Performance)
                //        m_CourseSave += "_Performance";
                //}
                //if (m_CourseSave == "@PowerTraining")
                //{
                //    //ss_CourseGrade.Visibility = Visibility.Visible;
                //    //ss_CourseWatts.Visibility = Visibility.Visible;
                //    //ss_PerformanceGrade.Visibility = Visibility.Visible;
                //    //ss_PerformanceWatts.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    //ss_CourseGrade.Visibility = Visibility.Visible;
                //    //ss_CourseWatts.Visibility = Visibility.Collapsed;
                //    //ss_PerformanceGrade.Visibility = Visibility.Visible;
                //    //ss_PerformanceWatts.Visibility = Visibility.Collapsed;
                //}
			}
		}
		public void ChangeTags(String[] tags)
		{
			m_NextTags = tags == null || tags.Length == 0 ? ms_AllTag : tags;
			FadeOut.Begin();
			if (DemoBike.IsVisible)
				DemoBike.ViewOffFade();
		}
		public void ChangeTags(List<String> tags )
		{
			String[] arr = new String[tags.Count];
			int n = 0;
			foreach(String i in tags)
				arr[n++] = i;
			ChangeTags(arr);
		}

		private void riderOptionsAll_Loaded(object sender, RoutedEventArgs e)  {
			m_bLoaded = true;
			if (m_bLoaded) { };

#if !DEBUG
            c_SpinScanValuesBox.Visibility = System.Windows.Visibility.Collapsed;
#endif

            MetricChanged();
            //UpdateKeyControls();
		}

        private void riderOptionsAll_Unloaded(object sender, RoutedEventArgs e)  {
            //KeyControls = null;
            m_bLoaded = false;
            Commit();
        }

		public void Commit()  {

#if DEBUG
			Debug.WriteLine("\nRideOptionsAll::Commit()");
#endif

            t_dummy.Focus();
        //    if (CourseViewer.IsVisible && CourseViewer.CurrentCourse != null)
        //        RM1_Settings.General.SelectedCourse[m_CourseSave] = new CourseInfo(CourseViewer.CurrentCourse);

            if (m_bHardwareInit)
            {
                foreach (Controls.HardwareLine h in m_HardwareLines)
                {
                    Unit unit = Unit.Units[h.Number - 1];
                    unit.Trainer = h.Trainer;
                }
                Unit.UpdateTrainerData(true);
            }

            //UnitsList.SaveToSettings();
            //UnitsList.SetActiveUnits();
            Riders.SaveToFile();
            RM1_Settings.SaveToFile();
		}




		void AddTag(String tag, OptionGroup item)
		{
			tag = tag.ToLower();
			List<OptionGroup> list;
			if (m_TagMap.TryGetValue(tag, out list) == false)
			{
				list = new List<OptionGroup>();
				m_TagMap[tag] = list;
			}
			list.Add(item);
		}

		public RideOptionsAll()
		{

#if DEBUG
			Debug.WriteLine("RideOptionsAll.xaml.cs, RideOptionsAll() constructor");
#endif
			InitializeComponent();
			foreach(Object o in ControlPanels.Children)
			{
				Grid g = o as Grid;
				if (g != null)
				{
					OptionGroup item = new OptionGroup(g);
					String s = g.Tag as String;
					g.Visibility = Visibility.Visible;
					if (s == null) s = "";
					String[] taglist = s.Split(',');
					foreach (String t in taglist)
						AddTag(t, item);
					AddTag("@All", item);
					//AddTag("All", item);
				}
			}
			m_HardwareLines = new Controls.HardwareLine[8] { bike_1, bike_2, bike_3, bike_4, bike_5, bike_6, bike_7, bike_8 };
			m_All = m_TagMap["@all"];
		}

		private void FadeOut_Completed(object sender, EventArgs e)
		{
			if (m_NextTags != null)
			{
				m_Tags = m_NextTags;
				m_NextTags = null;
				RedoTags();
			}
			FadeIn.Begin();
		}

		public void RedoTags()
		{
			if (m_Tags == null)
				m_Tags = ms_AllTag;
			int lastv = TagVersion++;
			List<OptionGroup> list;
			foreach (String t in m_Tags)
			{
				if (m_TagMap.TryGetValue(t.ToLower(), out list))
				{
					foreach (OptionGroup g in list)
						g.Version = TagVersion;
				}
			}
			foreach (OptionGroup g in m_All)
			{
				if (g.Version == lastv)
					g.MainElement.Visibility = Visibility.Collapsed;
				else if (g.Version == TagVersion)
				{
					g.MainElement.Visibility = Visibility.Visible;
					DeepTags(g.MainElement);  
				}
			}
		}

		/// <summary>
		/// Show or hide elements based on tags. (if they are in the elemnt.
		/// </summary>
		/// <param name="obj"></param>
		public void DeepTags(Object obj)
		{
			Panel panel = obj as Panel;
			if (panel == null)
				return;

			foreach (Object o in panel.Children)
			{
				FrameworkElement f = o as FrameworkElement;
				if (f.Tag != null)
				{
					String s = f.Tag as String;
					if (s != null)
					{
						int show = 0;
						s = s.ToLower();
						String[] taglist = s.Split(',');
						bool collapse = false;
						bool n = false;
						foreach (String t in taglist)
						{
							if (t.Length == 0)
								continue;
							if (t == "^")
							{
								collapse = true;
								continue;
							}
							if (t[0] == '!' || t[0] == '~')
							{
								// If the tag is in the string hide it
								show = 1;
								String nt = t.Substring(1);
								foreach (String tt in m_Tags)
								{
									if (String.Compare(tt,nt,true) == 0)
									{
										show = -1;
										break;
									}
								}
							}
							else
							{
								n = true;
								foreach (String tt in m_Tags)
								{
									if (String.Compare(tt, t, true) == 0)
									{
										show = 1;
										break;
									}
								}
							}
							if (show != 0)
								break;
						}
						if (n && show == 0)
							show = -1;
						f.Visibility = show >= 0 ? Visibility.Visible : (collapse ? Visibility.Collapsed:Visibility.Hidden);
					}
				}
				DeepTags(f); // Keep going down tree.
			}
		}


		//===========================================================================================
		/*
		private bool m_C_Riders_Init = false;
		private void C_Riders_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Grid g = sender as Grid;
			if (g.Visibility == Visibility.Visible && !m_C_Riders_Init)
			{
				m_C_Riders_Init = true;
				Binding binding = new Binding();//set binding parameters if necessary
				binding.Source = Riders.RidersList;
				ChooseRiderList.SetBinding(ItemsControl.ItemsSourceProperty, binding);

				RedoRiderList();
			}
		}

		public void RedoRiderList()
		{
			RiderPicker.Items.Clear();
			int cnt = 1;
			int first = -1;
			foreach (TrainerUserConfigurable tc in RM1_Settings.SavedTrainersList)
			{
				ListBoxItem item = new ListBoxItem();
				String num = "" + cnt;
				Controls.RiderPickerLine cline = new Controls.RiderPickerLine();
				cline.Number.Text = "" + cnt;
				cline.Trainer = tc;
				item.Content = cline;
				item.Tag = cline;
				int idx = RiderPicker.Items.Add(item);
				if (tc.ActiveTrainer != null && first < 0)
					first = idx;					
				cnt++;
			}
			if (first >= 0)
				RiderPicker.SelectedIndex = first;
		}
		private void RiderPicker_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Controls.RiderPickerLine cline = e.Source as Controls.RiderPickerLine;
			if (cline != null)
			{
				Log.WriteLine(cline.Number.Text);
				ChooseRider.PlacementTarget = cline;
				ChooseRiderList.SelectedIndex = -1;
				ChoosePacerList.SelectedIndex = -1;
				ChooseRider.StaysOpen = true;
				ChooseRider.IsOpen = true;
			}
		}
		private void RiderPicker_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (ChooseRider.IsOpen == true)
				ChooseRider.StaysOpen = false;
		}

		private void ChooseRider_LostFocus(object sender, RoutedEventArgs e)
		{
		}

		private void ChooseRiderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Rider rider = ChooseRiderList.SelectedItem as Rider;
			ListBoxItem item = RiderPicker.SelectedItem as ListBoxItem;
			Controls.RiderPickerLine cline = RiderPicker.SelectedItem as Controls.RiderPickerLine;
			if (item != null && item.Tag != null && rider != null && (item.Tag as Controls.RiderPickerLine) != null)
			{
				ChooseRider.IsOpen = false;
				Controls.RiderPickerLine cl = item.Tag as Controls.RiderPickerLine;
				TrainerUserConfigurable tc = cl.Trainer;
				tc.PreviousRiderKey = rider.DatabaseKey;
				cl.Trainer = null;
				cl.Trainer = tc;
			}
		}

		private void ChoosePacerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ChooseRider.IsOpen = false;
		}
		 */

		//==============================================================================================================
        //private bool m_C_SpinScanOptions_Init = false;
        //private void C_SpinScanOptions_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    Grid g = sender as Grid;
        //    if (g.Visibility == Visibility.Visible && !m_C_SpinScanOptions_Init)
        //    {
        //        m_C_SpinScanOptions_Init = true;
        //        SS_UpdateManualControl();
        //    }
        //}
		//==============================================================================================================
        //public List<Controls.CoursePickerLine> m_PickerList = new List<RacerMateOne.Controls.CoursePickerLine>();
        //bool m_C_CourseInfo_Init = false;
        //private void C_CourseInfo_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    //Binding binding;
        //    Grid g = sender as Grid;
        //    if (g.Visibility == Visibility.Visible && !m_C_CourseInfo_Init)
        //    {
        //        // Set up bindings
        //        bool ss = TagOn("@SpinScan");

        //        //CoursePicker.ItemsSource = m_PickerList;

        //        //binding = new Binding("WattsStep");
        //        //binding.Source = RM1_Settings.General;
        //        //WattsStep.SetBinding(TextBox.TextProperty,binding);
        //        //WattsStepSlider.SetBinding(Slider.ValueProperty,binding);

        //        //binding = new Binding(ss ? "SS_GradeStep":"GradeStep");
        //        //binding.Source = RM1_Settings.General;
        //        //GradeStep.SetBinding(TextBox.TextProperty,binding);
        //        //GradeStepSlider.SetBinding(Slider.ValueProperty,binding);

        //        //binding = new Binding(ss ? "SS_ManualControl":"ManualControl");
        //        //binding.Source = RM1_Settings.General;
        //        //ManualControl.SetBinding(CheckBox.IsCheckedProperty, binding);


        //        //SceneryDropDown.SelectedItem = RM1_Settings.General.Scenery;
        //        //SceneryDropDown.IsSynchronizedWithCurrentItem = true;  //I cannot stress more the importance of this little gem.
        //        //binding = new Binding();//set binding parameters if necessary
        //        //binding.Source = Controls.Render3D.SceneryInfo.SceneryList;
        //        //SceneryDropDown.SetBinding(ItemsControl.ItemsSourceProperty, binding);

        //        //binding = new Binding("AllowDrafting");
        //        //binding.Source = RM1_Settings.General;
        //        //AllowDrafting.SetBinding(CheckBox.IsCheckedProperty, binding);
				
        //        //binding = new Binding("Thumbnail");
        //        //binding.Source = Controls.Render3D.SceneryInfo.SceneryList;
        //        //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //        //SceneryImage.SetBinding(Image.SourceProperty, binding);
        //    }
        //    //SS_UpdateManualControl();
        //    //RedoCourseType();
        //}

        //private void SceneryDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //RM1_Settings.General.Scenery = SceneryDropDown.SelectedItem as Controls.Render3D.SceneryInfo;
        //}

        //private static String ms_SavePath;

        //private void SaveAs_Click(object sender, RoutedEventArgs e)
        //{
        //    //Debug.WriteLine("Saving from rideoptionsAll");
        //    //Commit();
        //    //Course course = CourseViewer.CurrentCourse;
        //    ////Options.CourseViewer.Course
        //    //if (ms_SavePath == null)
        //    //    ms_SavePath = RacerMatePaths.CoursesFullPath;
        //    //SaveFileDialog f = new SaveFileDialog();
        //    //String full = course.FileName;
        //    //full = System.IO.Path.Combine(ms_SavePath, System.IO.Path.GetFileName(full));
        //    //String ext = System.IO.Path.GetExtension(full);
        //    //if (String.Compare(ext, ".rmc", true) != 0)
        //    //{
        //    //    full = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(full),
        //    //        System.IO.Path.GetFileNameWithoutExtension(full)+".rmc");
        //    //}
        //    //f.FileName = full;
        //    //f.CheckFileExists = false;
        //    //f.CheckPathExists = false;
        //    //f.Title = "Save this course";
        //    //f.AddExtension = true;
        //    //f.DefaultExt = ".rmc";
        //    //f.Filter = "Course files (.rmc)|*.rmc";
        //    //if (f.ShowDialog() == true)
        //    //{
        //    //    ms_SavePath = System.IO.Path.GetDirectoryName(f.FileName);
        //    //    course.Save(f.FileName, course.Description);
        //    //    Course c = new Course();
        //    //    if (c.Load(f.FileName)) 
        //    //    {
        //    //        Courses.Replace(c);
        //    //        RedoDisplayList();
        //    //    }
        //    //}	
        //}



        //private void SS_ManualControl_Click(object sender, RoutedEventArgs e)
        //{
        //    SS_UpdateManualControl();
        //    RedoCourseType();
        //}

        //public CourseType CourseFilter = CourseType.Distance;

        //ReportColumns m_RC_Template;

        //private void RedoDisplayList()
        //{
        //    if (m_Tags.Length > 0)
        //    {
        //        switch (m_Tags[0])
        //        {
        //            case "@3D":
        //                m_RC_Template = ReportColumns.Display_3DRoadRacing;
        //                break;
        //            case "@RCV":
        //                m_RC_Template = ReportColumns.Display_RCV;
        //                break;
        //            case "@SpinScan":
        //                m_RC_Template = ReportColumns.Display_SpinScan;
        //                break;
        //            case "@PowerTraining":
        //                m_RC_Template = (CourseFilter & CourseType.Watts) == CourseType.Watts ? ReportColumns.Display_WattTestingERG : ReportColumns.Display_WattTestingAT;
        //                break;
        //            case "@MultiRider":
        //                m_RC_Template = ReportColumns.Display_3DRoadRacing;
        //                break;
        //        };
        //        //DisplayType.Visibility = m_RC_Template.SubItemList.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        //        //if (m_RC_Template.SubItemList.Count > 1)
        //        //{
        //        //    ReportColumns r;
        //        //    if (!ReportColumns.m_DB.TryGetValue(m_RC_Template.Selected, out r))
        //        //        r = null;
        //        //    Binding binding = new Binding("SubItemList");
        //        //    binding.Source = m_RC_Template;
        //        //    DisplayType.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        //        //    if (r != null)
        //        //        DisplayType.SelectedItem = r;
        //        //}
        //    }
        //}
        //private void DisplayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //ReportColumns r = DisplayType.SelectedItem as ReportColumns;
        //    //if (r != null && m_RC_Template != null)
        //    //    m_RC_Template.Selected = r.Key;
        //}

        //private void RedoCourseType()
        //{
        //    if (TagOn("@3D"))
        //    {
        //        //CourseTypeFilter = CourseType.ThreeD | CourseType.Distance;
        //        //StepWattsGroup.Visibility = Visibility.Collapsed;
        //        //StepGradeGroup.Visibility = Visibility.Collapsed;
        //        //WattsManualText.Visibility = Visibility.Collapsed;
        //        //GradeManualText.Visibility = Visibility.Collapsed;
        //        RedoDisplayList();
        //        return;
        //    }
        //    if (TagOn("@RCV"))
        //    {
        //        //CourseTypeFilter = CourseType.Video;
        //        //StepWattsGroup.Visibility = Visibility.Collapsed;
        //        //StepGradeGroup.Visibility = Visibility.Collapsed;
        //        //WattsManualText.Visibility = Visibility.Collapsed;
        //        //GradeManualText.Visibility = Visibility.Collapsed;
        //        RedoDisplayList();
        //        return;
        //    }
        //    // Based on the drop down type
        //    CourseType ct = CourseFilter;
        //    // Turn off all the types we will be adjusting.
        //    ct &= ~(CourseType.Distance | CourseType.Watts | CourseType.Performance);
        //    //ComboBoxItem citem = s_FilterType.SelectedItem as ComboBoxItem;
        //    //if (citem == null)
        //    //{
        //    //    s_FilterType.SelectedIndex = 0;
        //    //    citem = s_FilterType.SelectedItem as ComboBoxItem;
        //    //}
        //    //if (citem.Name == "ss_CourseGrade")
        //    //    ct |= CourseType.ThreeD | CourseType.Distance;
        //    //else if (citem.Name == "ss_CourseWatts")
        //    //    ct |= CourseType.Watts;
        //    //else if (citem.Name == "ss_PerformanceGrade")
        //    //    ct |= CourseType.Distance | CourseType.Performance;
        //    //else if (citem.Name == "ss_PerformanceWatts")
        //    //    ct |= CourseType.Watts | CourseType.Performance;

        //    //StepWattsGroup.Visibility = (ct & CourseType.Watts) == CourseType.Watts ? Visibility.Visible:Visibility.Collapsed;
        //    //StepGradeGroup.Visibility = (ct & CourseType.Distance) == CourseType.Distance ? Visibility.Visible:Visibility.Collapsed;

        //    //WattsManualText.Visibility = (ct & (CourseType.Watts | CourseType.Performance)) == CourseType.Watts ? Visibility.Visible : Visibility.Collapsed;
        //    //GradeManualText.Visibility = (ct & (CourseType.Distance | CourseType.Performance)) == CourseType.Distance ? Visibility.Visible : Visibility.Collapsed;
        //    CourseFilter = CourseTypeFilter = ct;

        //    RedoDisplayList();

        //}

        //private void SS_UpdateManualControl()
        //{
        //    //if (ManualControl.IsChecked == true)
        //    //{
        //    //    //CoursePicker_Disable.Visibility = Visibility.Visible;
        //    //    CourseViewer.Disable = true;
        //    //    StepGroup.Opacity = 1.0;
        //    //}
        //    //else
        //    //{
        //    //    //CoursePicker_Disable.Visibility = Visibility.Collapsed;
        //    //    CourseViewer.Disable = false;
        //    //    StepGroup.Opacity = 0.4;
        //    //}
        //}

        //private void PickCourseBrowse_Click(object sender, RoutedEventArgs e)
        //{
        //    // ToDos 
        //    // - Change MessageBox prompts to a better prompt.
        //    // - Save Initial Directory to Settings 

        //    Controls.CoursePickerLine lastAdded = m_LastAdded; // This will be use to compare a successful load
        //    //OK... Display the course Browser.
        //    OpenFileDialog f = new OpenFileDialog();
        //    f.InitialDirectory = RacerMatePaths.SettingsFullPath; // this needs to use last folder visited
        //    f.Multiselect = false;
        //    f.Title = "Select program to convert";
        //    f.ValidateNames = true;
        //    f.AddExtension = true;
        //    f.CheckFileExists = true;
        //    f.DefaultExt = ".3dc;.crs;.dnw;.erg;.tng;.mrc;.rmp;.rmc";
        //    f.Filter = "Racermate Files(*.3dc,*.crs,*.dnw,*.erg,*.tng,*.mrc,*.rmp,*.rmc)|*.3dc;*.crs;*.dnw;*.erg;*.tng;*.mrc;*.rmp;*.rmc|All Files|*.*";
        //    if (f.ShowDialog() == true)
        //    {
        //        string srcfilename = System.IO.Path.GetFileName(f.FileName);
        //        string ext = System.IO.Path.GetExtension(srcfilename);
        //        string newext = ext.Replace(".", "_");
        //        string fname = srcfilename.Replace(ext, newext + ".rmc");
        //        Course cOld = new Course();
        //        if (cOld.Load(f.FileName))
        //        {
        //            MessageBoxResult result = MessageBox.Show("Do you want a permanent copy?","Copy Course",MessageBoxButton.YesNo,MessageBoxImage.Question);
        //            if (result == MessageBoxResult.Yes)
        //            {
        //                string description = cOld.Description;
        //                if (description.Length == 0)
        //                    description = "Converted from " + f.FileName;
        //                if (cOld.Save(RacerMatePaths.CoursesFullPath + "\\" + fname, description))
        //                {
        //                    Course c = new Course();
        //                    if (c.Load(RacerMatePaths.CoursesFullPath + "\\" + fname))
        //                    {
        //                        Courses.Add(c);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Courses.Add(cOld);
        //            }
        //            if (m_LastAdded != lastAdded)
        //            {
        //                //CoursePicker.SelectedItem = m_LastAdded;
        //                //int idx = m_LastAdded != null ? m_PickerList.IndexOf(m_LastAdded) : 0;
        //                //if (idx < 0)
        //                //    idx = 0;
        //                //CoursePicker.SelectedIndex = idx;
        //                //if (CoursePicker.SelectedItem != null)
        //                //    CoursePicker.ScrollIntoView(CoursePicker.SelectedItem);
        //            }
        //        }
        //        if (m_LastAdded != lastAdded && m_LastAdded != null)
        //        {
        //            // Based on the drop down type
        //            CourseType ct = CourseTypeFilter;
        //            if ((ct & m_LastAdded.Course.Type) == CourseType.Zero)
        //            {
        //                MessageBox.Show("Course just added will not work on current mode", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("File selected failed to load", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }

        //        /*
        //        string[] files = f.FileNames;
        //        foreach(string file in files)
        //        {

        //        }
        //        */
        //    }

        //}

        //private void CourseViewer_StartChanged(object sender, RoutedEventArgs e)
        //{
        ////    if (TagOn("@RCV") && CourseViewer.CurrentCourse != null)
        ////    {
        ////        RCVPreview.Location = CourseViewer.CurrentCourse.StartAt;
        ////    }

        //}




		//===========================================================================================
		//private bool m_C_CoursePicker_Init = false;

        //private CourseType m_CourseTypeFilter = CourseType.Distance;
        //public CourseType CourseTypeFilter
        //{
        //    get { return m_CourseTypeFilter; }
        //    set
        //    {
        //        if (value != m_CourseTypeFilter)
        //        {
        //            m_CourseTypeFilter = value;
        //            RedoCoursePicker();
        //            RedoColumnHeaders();
        //        }
        //    }
        //}

        //void RedoColumnHeaders()
        //{
        //    //bool watts = (m_CourseTypeFilter & CourseType.Watts) == CourseType.Watts;
        //    //PC_Label_Column1.Content = watts ? "Interval" : "Laps";
        //    //PC_Label_Column2.Content = watts ? "Minutes" : ConvertConst.TextDistanceLabel;
        //    //PC_Label_Column3.Content = watts ? "Max Watts" : "Alt";
        //}


        //private static int CTest(Controls.CoursePickerLine a, Controls.CoursePickerLine b)
        //{
        //    if (a.Course.TotalX < b.Course.TotalX)
        //        return -1;
        //    else if (a.Course.TotalX > b.Course.TotalX)
        //        return 1;
        //    return 0;
        //}

        //bool m_NoSortPicker = false;
        //Controls.CoursePickerLine m_SelectedLine = null;
        //Controls.CoursePickerLine m_LastAdded = null;
        //public void RedoCoursePicker()
        //{
        //    CoursePicker.SelectedItem = null;
        //    m_PickerList.Clear();
        //    m_NoSortPicker = true;
        //    try
        //    {
        //        foreach (Course c in Courses.AllCourses)
        //            OnCourseAddedSimple(c);
        //    }
        //    catch { }
        //    m_NoSortPicker = false;
        //    //SortPicker();
        //    if (CoursePicker.SelectedItem != null)
        //        CoursePicker.ScrollIntoView(CoursePicker.SelectedItem);
        //}

        //void OnCourseAdded(Course c)
        //{
        //    if ((c.Type & m_CourseTypeFilter) != 0)
        //    {
        //        foreach (Controls.CoursePickerLine cline in m_PickerList)
        //        {
        //            if (cline.Course == c)
        //                return;
        //        }
        //        OnCourseAddedSimple(c);
        //    }
        //}

        //void OnCourseAddedSimple(Course c)
        //{
        //    if ((c.Type & m_CourseTypeFilter) != 0)
        //    {
        //        Controls.CoursePickerLine cline = new Controls.CoursePickerLine(c);
        //        m_PickerList.Add(cline);
        //        m_LastAdded = cline; // ECT - Just a helper to find the last course added
        //        try
        //        {
					
        //            CourseInfo cinfo;
        //            if (RM1_Settings.General.SelectedCourse.TryGetValue(m_CourseSave, out cinfo))
        //            {
        //                if (cinfo != null && String.Compare(cinfo.FileName, c.FileName, true) == 0)
        //                {
        //                    c.Set(cinfo);
        //                    m_SelectedLine = cline;
        //                }
        //            }
        //        }
        //        catch { }
        //        //SortPicker();
        //    }
        //}



        //private static int cmpColumn_0(Controls.CoursePickerLine a, Controls.CoursePickerLine b)
        //{
        //    return String.Compare(a.Course.Name.ToString(), b.Course.Name.ToString(), true);
        //}
        //private static int cmpColumn_1(Controls.CoursePickerLine a, Controls.CoursePickerLine b)
        //{
        //    if (a.Course.Laps == b.Course.Laps)
        //        return String.Compare(a.Course.Name.ToString(), b.Course.Name.ToString(), true);
        //    return a.Course.Laps < b.Course.Laps ? -1 : 1;
        //}
        //private static int cmpColumn_2(Controls.CoursePickerLine a, Controls.CoursePickerLine b)
        //{
        //    if (Math.Round(a.Course.TotalX,2) == Math.Round(b.Course.TotalX,2))
        //        return String.Compare(a.Course.Name.ToString(), b.Course.Name.ToString(), true);
        //    return a.Course.TotalX < b.Course.TotalX ? -1 : 1;
        //}
        //private static int cmpColumn_3(Controls.CoursePickerLine a, Controls.CoursePickerLine b)
        //{
        //    if (Math.Round(a.Course.Alt, 2) == Math.Round(b.Course.Alt, 2))
        //        return String.Compare(a.Course.Name.ToString(), b.Course.Name.ToString(), true);
        //    return a.Course.Alt < b.Course.Alt ? -1 : 1;
        //}

        //void SortPicker()
        //{
        //    if (m_NoSortPicker)
        //        return;
        //    Controls.CoursePickerLine sitem = CoursePicker.SelectedItem as Controls.CoursePickerLine;
        //    if (m_SelectedLine != null)
        //    {
        //        if (m_PickerList.IndexOf(m_SelectedLine) >= 0)
        //        {
        //            sitem = m_SelectedLine;
        //        }
        //        m_SelectedLine = null;
        //    }
        //    CoursePicker.ItemsSource = null;
        //    switch (ms_PickerSortColumn)
        //    {
        //        case 0: m_PickerList.Sort(cmpColumn_0);	break;
        //        case 1: m_PickerList.Sort(cmpColumn_1); break;
        //        case 2: m_PickerList.Sort(cmpColumn_2); break;
        //        case 3: m_PickerList.Sort(cmpColumn_3); break;
        //    }
        //    if (ms_PickerSortUp)
        //        m_PickerList.Reverse();
        //    CoursePicker.ItemsSource = m_PickerList;
        //    CoursePicker.SelectedItem = sitem;
        //    int idx = sitem != null ? m_PickerList.IndexOf(sitem):0;
        //    if (idx < 0)
        //        idx = 0;
        //    CoursePicker.SelectedIndex = idx;

        //    //PC_Label_Column1.FontWeight = ms_PickerSortColumn == 1 ? FontWeights.Bold : FontWeights.Normal;
        //    //PC_Label_Column2.FontWeight = ms_PickerSortColumn == 2 ? FontWeights.Bold : FontWeights.Normal;
        //    //PC_Label_Column3.FontWeight = ms_PickerSortColumn == 3 ? FontWeights.Bold : FontWeights.Normal;
        //}

        //static int ms_PickerSortColumn = 0;
        //static bool ms_PickerSortUp = false;
		//=============
        //private void PC_Label_Column1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (ms_PickerSortColumn != 1)
        //        ms_PickerSortColumn = 1;
        //    else
        //        ms_PickerSortColumn = 0;
        //    SortPicker();
        //}
        //private void PC_Label_Column2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (ms_PickerSortColumn != 2)
        //        ms_PickerSortColumn = 2;
        //    else
        //        ms_PickerSortColumn = 0;
        //    SortPicker();

        //}

        //private void PC_Label_Column3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (ms_PickerSortColumn != 3)
        //        ms_PickerSortColumn = 3;
        //    else
        //        ms_PickerSortColumn = 0;
        //    SortPicker();
        //}
		//===========================================================================================
        //private void C_CoursePicker_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    Grid g = sender as Grid;
        //    if (g.Visibility == Visibility.Visible)
        //    {
        //        //s_FilterType.SelectedItem = RM1_Settings.General.SelectWatts ? ss_CourseWatts : ss_CourseGrade;
        //        CoursePicker.SelectedItem = null;
        //        RedoCoursePicker();
        //        Courses.OnCourseAdded += new Courses.CourseAdded(OnCourseAdded);
        //    }
        //}

        //private void s_FilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //if (m_bLoaded)
        //    //    Commit();

        //    //RM1_Settings.General.SelectWatts = s_FilterType.SelectedIndex == 0 || s_FilterType.SelectedIndex == 2;
        //    //m_CourseSave = m_Tags.Length > 0 ? m_Tags[0] : "Unknown";
        //    //if (m_CourseSave == "@PowerTraining" || m_CourseSave == "@MultiRider" || m_CourseSave == "@SpinScan")
        //    //{
        //    //    if ((CourseTypeFilter & CourseType.Watts) == CourseType.Watts)
        //    //        m_CourseSave += "_Watts";
        //    //    if ((CourseTypeFilter & CourseType.Performance) == CourseType.Performance)
        //    //        m_CourseSave += "_Performance";
        //    //}
        //    //if (m_bLoaded && C_CourseInfo.IsVisible)
        //    //    RedoCourseType();


        //}


		//===========================================================================================
        //private void C_Course_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    Grid g = sender as Grid;
        //    /*
        //    if (m_PickerList.Count > 0)
        //    {
        //        CoursePicker.SelectedIndex = 0;
        //    }
        //     */
        //}

        //private void CoursePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Debug.WriteLine(" CoursePicker Selection changed");
			
        //    Controls.CoursePickerLine cline = CoursePicker.SelectedItem as Controls.CoursePickerLine;
        //    if (cline != null)
        //    {
        //        Course c = cline.Course;
        //        CourseInfo cinfo = new CourseInfo(c);
        //        Debug.WriteLine("Selection changed");
        //        CourseViewer.Save();
        //        CourseViewer.SetCourse(c, c.Reverse, c.Mirror, false , c.StartAt, c.EndAt);
        //        Debug.WriteLine("Set course with rev= " + c.Reverse + " mirror = " + c.Mirror + " start = " + c.StartAt + " end= " + c.EndAt ); 
        //        Laps.Text = c.Laps.ToString();
        //        Course_Zoom.Selected = false;
        //        Debug.WriteLine("possibly changing the direction");

        //        CourseDirection.SelectedIndex = c.Mirror ? 2 : c.Reverse ? 1 : 0;
				
        //        SaveAs.Visibility = CourseViewer.NeedSave ? Visibility.Visible : Visibility.Hidden;
        //        //I uncommented the below
        //        //UpdateCourseDirection();
        //        Laps.Text = c.StringLaps;
        //        if (TagOn("@RCV"))
        //        {
        //            Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
        //            {
        //                RCVPreview.Course = null;
        //                RCVPreview.Course = c;
        //            });

        //        }
        //    }
        //}

		//===========================================================================================
		bool m_bHardwareInit;
		void RedoHardwareLines()
		{
			foreach (Controls.HardwareLine h in m_HardwareLines)
			{
				Unit unit = Unit.Units[h.Number - 1];
				RM1.Trainer trainer = unit.Trainer;
				if (trainer == null)
					trainer = unit.TC.CurrentTrainer;

				h.Trainer = Unit.Units[h.Number - 1].Trainer;
			}
		}


		private void C_Hardware_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Grid g = sender as Grid;
			bool active = g.Visibility == Visibility.Visible;
			foreach(Controls.HardwareLine h in m_HardwareLines)
				h.Active = active;
			if (active && !m_bHardwareInit)
			{
				m_bHardwareInit = true;
				RedoHardwareLines();
			}
		}

		private void OnBikeChanged(object sender, RoutedEventArgs e)
		{
			Controls.HardwareLine ht = sender as Controls.HardwareLine;
            //Log.WriteLine("got onbikechanged");
			RM1.Trainer last = ht.LastTrainer;
			if (ht.Trainer == null)
				return;
			foreach (Controls.HardwareLine h in m_HardwareLines)
			{
				if (h != ht && h.Trainer == ht.Trainer)
				{
					h.LastTrainer = last;
					h.Trainer = last;

					last = null;
				}
			}
		}

		private void bike_CalibrateClick(object sender, RoutedEventArgs e)
		{
			Controls.HardwareLine hw = sender as Controls.HardwareLine;
            //Log.WriteLine("got bikecalibrateclicked");
			if (hw == null)
				return;

			// Choose between acuwatt and calibrate.
			if (hw.AccuwattButton)
			{
				// Accuwatt
				Pages.Modes.Accuwatt acc = new Pages.Modes.Accuwatt();
				//acc.UnitNumber = hw.Number - 1;
				AppWin.Instance.MainFrame.Navigate(acc);
			}
			else
			{
				Pages.Modes.Calibrate cpage = new Pages.Modes.Calibrate();
				cpage.UnitNumber = hw.Number - 1;
                //Debug.WriteLine("selected hw is index " + (hw.Number - 1).ToString());
				Course course = new Course();
				if (course.Load(RacerMatePaths.EXEPath + @"\Courses\Distance and Grade\Warmup_3dc.rmc"))
				{
					Controls.Render3D.Course = Unit.Course = course;
                    //Debug.WriteLine("about to nav to calibrate");
					AppWin.Instance.MainFrame.Navigate(cpage);
				}
			}
		}


		bool m_bFullScan;
		Page m_Scanning = null;

		private void HardwareRefresh_Click(object sender, RoutedEventArgs e)  {
			if (m_Scanning != null)	// Make sure we have only one.
				return;

			m_bFullScan = false;
			RM1.OnTrainerInitialized += new RM1.TrainerInitialized(ScanDone);

			List<int> tlist = new List<int>();
			foreach (TrainerUserConfigurable tc in RM1_Settings.ActiveTrainerList)
				tlist.Add(tc.SavedSerialPortNum);

			if (tlist.Count() == 0)  {
				m_bFullScan = true;
				if (RM1.StartFullScan() == 0)
					return;
			}
			else if (RM1.StartSpecificScan(tlist) == 0)
				return;

			m_Scanning = new Pages.Modes.Scanning();
			AppWin.Instance.MainFrame.Navigate(m_Scanning);
		}

		/**********************************************************************************************************

		**********************************************************************************************************/

		private void HardwareRescan_Click(object sender, RoutedEventArgs e)  {					// 20141113
			if (m_Scanning != null)	// Make sure we have only one.
				return;
			m_bFullScan = true;
			RM1.OnTrainerInitialized += new RM1.TrainerInitialized(ScanDone);
			if (RM1.StartFullScan() == 0)
				return;
			m_Scanning = new Pages.Modes.Scanning();
			AppWin.Instance.MainFrame.Navigate(m_Scanning);
		}

		/**********************************************************************************************************

		**********************************************************************************************************/

		private void ScanDone(RM1.Trainer trainer, int left) {					// 20141113
			// 20150109
			if (left > 0)
				return;
			if (AppWin.Instance.MainFrame.NavigationService.Content == m_Scanning)  {
				if (DemoBike.IsVisible)
					DemoBike.ViewOffFade();

				AppWin.Instance.MainFrame.NavigationService.GoBack();

				if (m_bFullScan)  {
					// OK... We are done. Lets allocate any trainers that are not allocated to empty slots.
					//Unit.LoadFromSettings();
					Unit.AllocateHardware(false);
					Unit.SaveToSettings();
				}
				m_Scanning = null;
			}

			RM1.Trainer.WaitToScan(10);
			RM1.OnTrainerInitialized -= new RM1.TrainerInitialized(ScanDone);
			Unit.UpdateTrainerData(m_bFullScan);
			RedoHardwareLines();
		}

		//===========================================================================================

        //private RadioButton[,] m_ZoneRB = new RadioButton[5,2];

		private String m_Gender;
		public String Gender { get { return m_Gender; } set { m_Gender = value; } }

		private bool m_bRiderInit = false;
		private void C_Rider_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Grid grid = sender as Grid;
			bool active = grid.Visibility == Visibility.Visible;
			if (active && !m_bRiderInit)
			{
				m_bRiderInit = true;
				// First time through set the initial rider to the SelectedRider
				Rider rider = Riders.FindRiderByKey(RM1_Settings.General.SelectedRiderKey);

				// Bind the rider 
				RiderEditSelect.IsSynchronizedWithCurrentItem = true;  //I cannot stress more the importance of this little gem.
				Binding binding = new Binding();//set binding parameters if necessary
				binding.Source = Riders.RidersList;
				RiderEditSelect.SetBinding(ItemsControl.ItemsSourceProperty, binding);

				binding = new Binding("FirstName");
				binding.Source = Riders.RidersList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Rider_FirstName.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("LastName");
				binding.Source = Riders.RidersList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Rider_LastName.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("NickName");
				binding.Source = Riders.RidersList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Rider_UserName.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("AgeString");
				binding.Source = Riders.RidersList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Rider_Age.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("WeightRiderDisplay");//set binding parameters if necessary
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				//binding.Converter = new MetricWeightValueConverter();
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Rider_Weight.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("WeightBikeDisplay");//set binding parameters if necessary
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				//binding.Converter = new MetricWeightValueConverter();
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Rider_BikeWeight.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("WeightTotalDisplay");//set binding parameters if necessary
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				//binding.Converter = new MetricWeightValueConverter();
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Rider_TotalWeight.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("HeightMetersDisplay");
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Rider_HeightMeters.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("HeightFeetDisplay");
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.TwoWay;
				Rider_HeightFeet.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("HeightInchesDisplay");
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.TwoWay;
				Rider_HeightInches.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("DragFactor");
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.TwoWay;
				Rider_DragSlider.SetBinding(Slider.ValueProperty, binding);

				binding = new Binding("DragFactor");
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.OneWay;
				Rider_DragLabel.SetBinding(Label.ContentProperty, binding);


				binding = new Binding("HrAnT");//set binding parameters if necessary
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
				Rider_HRAnT.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("HrMin");//set binding parameters if necessary
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				Rider_HRmin.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("HrMax");//set binding parameters if necessary
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				Rider_HRmax.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("PowerAnT");//set binding parameters if necessary
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Rider_PowerAnT.SetBinding(TextBox.TextProperty, binding);

				binding = new Binding("PowerFTP");//set binding parameters if necessary
				binding.ValidatesOnExceptions = true;
				binding.Source = Riders.RidersList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Rider_PowerFTP.SetBinding(TextBox.TextProperty, binding);

                binding = new Binding("SkinBrush"); 
				binding.Source = Riders.RidersList;
				Rider_Skin.SetBinding(ComboBox.SelectedIndexProperty, binding);

				binding = new Binding("RiderTypeIndex");
				binding.Source = Riders.RidersList;
				binding.ValidatesOnExceptions = true;
				Rider_Type.SetBinding(ComboBox.SelectedIndexProperty, binding);

				binding = new Binding("SkinBrush");
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.TwoWay;
				binding.ValidatesOnExceptions = true;
				Rider_Skin.SetBinding(Controls.ColorPickerControlView.CurrentColorProperty, binding);

				binding = new Binding("HelmetBrush");
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.TwoWay;
				binding.ValidatesOnExceptions = true;
				Rider_Helmet.SetBinding(Controls.ColorPickerControlView.CurrentColorProperty, binding);

				binding = new Binding("HairBrush");
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.TwoWay;
				binding.ValidatesOnExceptions = true;
				Rider_Hair.SetBinding(Controls.ColorPickerControlView.CurrentColorProperty, binding);

				binding = new Binding("ShoesBrush");
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.TwoWay;
				binding.ValidatesOnExceptions = true;
				Rider_Shoes.SetBinding(Controls.ColorPickerControlView.CurrentColorProperty, binding);

				binding = new Binding("BikeColor1Brush");
				binding.Source = Riders.RidersList;
				binding.ValidatesOnExceptions = true;
				binding.Mode = BindingMode.TwoWay;
				Rider_BikeColor1.SetBinding(Controls.ColorPickerControlView.CurrentColorProperty, binding);
				
				binding = new Binding("BikeColor2Brush");
				binding.Source = Riders.RidersList;
				binding.ValidatesOnExceptions = true;
				binding.Mode = BindingMode.TwoWay;
				Rider_BikeColor2.SetBinding(Controls.ColorPickerControlView.CurrentColorProperty, binding);

				binding = new Binding("Clothing1Brush");
				binding.Source = Riders.RidersList;
				binding.ValidatesOnExceptions = true;
				binding.Mode = BindingMode.TwoWay;
				Rider_Clothing1.SetBinding(Controls.ColorPickerControlView.CurrentColorProperty, binding);

				binding = new Binding("Clothing2Brush");
				binding.Source = Riders.RidersList;
				binding.ValidatesOnExceptions = true;
				binding.Mode = BindingMode.TwoWay;
				Rider_Clothing2.SetBinding(Controls.ColorPickerControlView.CurrentColorProperty, binding);

				binding = new Binding("Zone1");
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.OneWay;
				Rider_Zone1.SetBinding(TextBox.TextProperty, binding);
				binding = new Binding("Zone2");
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.OneWay;
				Rider_Zone2.SetBinding(TextBox.TextProperty, binding);
				binding = new Binding("Zone3");
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.OneWay;
				Rider_Zone3.SetBinding(TextBox.TextProperty, binding);
				binding = new Binding("Zone4");
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.OneWay;
				Rider_Zone4.SetBinding(TextBox.TextProperty, binding);
				binding = new Binding("Zone5");
				binding.Source = Riders.RidersList;
				binding.Mode = BindingMode.OneWay;
				Rider_Zone5.SetBinding(TextBox.TextProperty, binding);

				if (rider != null)
					RiderEditSelect.SelectedItem = rider;
			}
			if (active)
			{
				//GearTeathButton.Enabled = Unit.AnyVelotron;
			}

			//UpdateRiderList();
		}

		private void RB_Rider_English_Checked(object sender, RoutedEventArgs e)
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			if (rider != null)
				rider.Metric = false;
			//RMLab1.Content = RMLab2.Content = 
			RMLab3.Content = "lbs";
			Height_Metric.Visibility = Visibility.Collapsed;
			Height_English.Visibility = Visibility.Visible;
		}
		private void RB_Rider_Metric_Checked(object sender, RoutedEventArgs e)
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			if (rider != null)
				rider.Metric = true;
			//RMLab1.Content = RMLab2.Content = 
			RMLab3.Content = "kgs";
			Height_Metric.Visibility = Visibility.Visible;
			Height_English.Visibility = Visibility.Collapsed;
		}


		public void FillAlarms()
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			int minzone = rider.AlarmMinZone;
			int maxzone = rider.AlarmMaxZone;

			z10.IsChecked = (minzone == 1);
			z20.IsChecked = (minzone == 2);
			z30.IsChecked = (minzone == 3);
			z40.IsChecked = (minzone == 4);
			z50.IsChecked = (minzone == 5);

			z11.IsChecked = (maxzone == 1);
			z21.IsChecked = (maxzone == 2);
			z31.IsChecked = (maxzone == 3);
			z41.IsChecked = (maxzone == 4);
			z51.IsChecked = (maxzone == 5);
		}


		// Deal programicly with the stuff that I can't deal with through bindings.
		private void RiderEditSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			if (rider != null)
			{
				Rider_Male.IsChecked = rider.Gender == "M";
				Rider_Female.IsChecked = rider.Gender == "F";
				RB_Rider_English.IsChecked = !rider.Metric;
				RB_Rider_Metric.IsChecked = rider.Metric;
				DemoBike.RiderModel = rider;
				FillAlarms();
				RM1_Settings.General.SelectedRiderKey = rider.DatabaseKey;
			}
		}
		private void s_CurrentColorChanged(object sender, RoutedEventArgs e)
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			if (rider != null)
			{
				DemoBike.RiderModel = rider;
			}
		}
		private void Rider_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			if (rider != null)
			{
				DemoBike.RiderModel = rider;
				ColorEnable();
			}
		}

		private void ColorEnable()
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			bool enable = rider != null && rider.Model < RiderModels.BotBike;
			Rider_BikeColor1.IsEnabled = Rider_BikeColor2.IsEnabled = Rider_Hair.IsEnabled = Rider_Skin.IsEnabled =
				Rider_Shoes.IsEnabled = Rider_Clothing1.IsEnabled = Rider_Clothing2.IsEnabled = Rider_Helmet.IsEnabled = enable;
		}

		private void Rider_Male_Checked(object sender, RoutedEventArgs e)
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			if (rider != null)
			{
				rider.Gender = "M";
				DemoBike.RiderModel = rider;
			}
		}
		private void Rider_Female_Checked(object sender, RoutedEventArgs e)
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			if (rider != null)
			{
				rider.Gender = "F";
				DemoBike.RiderModel = rider;
			}
		}

		private void Rider_Zone_Click(object sender, RoutedEventArgs e)
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			RadioButton rb = sender as RadioButton;
			if (rider != null)
			{
				int zone = (int)rb.Name[1] - (int)'0';
				if (rb.GroupName == "Lower")
				{
					if (rider.AlarmMinZone == zone)
					{
						rider.AlarmMinZone = 0;
						rb.IsChecked = false;
					}
					else
						rider.AlarmMinZone = zone;
				}
				else
				{
					if (rider.AlarmMaxZone == zone)
					{
						rider.AlarmMaxZone = 6;
						rb.IsChecked = false;
					}
					else
						rider.AlarmMaxZone = zone;
				}
			}
			FillAlarms();
		}
		private void Rider_Unit_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			/*
			RM1_Settings.General.Metric = Rider_Unit.SelectedIndex == 1;
			RM1_Settings.gsettingUSOrMetric.UserSetting.ID = Rider_Unit.SelectedIndex;
			int idx = RiderEditSelect.SelectedIndex;
			RiderEditSelect.SelectedIndex = -1;
			RiderEditSelect.SelectedIndex = idx; // Force reload of the box.
			 */
		}


		private TextBox m_RiderEditText;
		private Rider m_CurRider;

		private void SelectRider(Rider rider)
		{
			if (rider == null)
			{
				m_RiderEditText.Text = "";
			}
			if (RiderEditSelect.SelectedItem == null || ((ComboBoxItem)(RiderEditSelect.SelectedItem)).Tag != rider)
			{
				foreach (ComboBoxItem citem in RiderEditSelect.Items)
				{
					Rider r = citem.Tag as Rider;
					if (r == rider)
						RiderEditSelect.SelectedItem = citem;
				}
			}
			if (m_CurRider == rider)
				return;

			m_CurRider = rider;
		}

		private void RiderEditSelect_Loaded(object sender, RoutedEventArgs e)
		{
			ComboBox combobox = sender as ComboBox;
			m_RiderEditText = combobox.Template.FindName("PART_EditableTextBox", combobox) as TextBox;
			if (m_RiderEditText != null)
			{
				m_RiderEditText.TextChanged += new TextChangedEventHandler(RiderEditSelect_TextChanged);
			}
		}

		private void RiderEditSelect_TextChanged(object sender, TextChangedEventArgs e)
		{
			Rider r;
			r = Riders.FindRiderByKey(m_RiderEditText.Text);
			if (r != null)
			{
				SelectRider(r);
			}
			RiderEditSelect.IsDropDownOpen = true;
		}


		private void UpdateRiderList()
		{
			RiderEditSelect.Items.Clear();
			ComboBoxItem citem = new ComboBoxItem();
			citem.Content = "-- New rider --";
			citem.Tag = null;
			foreach (Rider rider in Riders.RidersList)
			{
				if (rider.DatabaseKey == "11111111-1111-1111-1111-111111111111") // Don't add the default rider to the list of riders
					continue;
				citem = new ComboBoxItem();
				citem.Content = rider.IDName;
				citem.Tag = rider;
				RiderEditSelect.Items.Add(citem);
			}
		}

		private void AddRider_Click(object sender, RoutedEventArgs e)
		{
            //adds a new rider to the list by instantiating the default rider with "FirstName", "LastName", "NickName" showing in the three textboxes.
            //generates a unique GUID too.
            Rider AddedRider = new Rider();
			RacerMateOne.Dialogs.TextLine tline = new RacerMateOne.Dialogs.TextLine();
			tline.TopText.Text = "Rider nickname";
			tline.Owner = AppWin.Instance;
			tline.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			tline.ShowDialog();
			String t;
			if (tline.OutText == null || (t = tline.OutText.Trim()) == "")
				return;

            AddedRider.FirstName = "";
            AddedRider.LastName = "";
            AddedRider.NickName = t;
            Riders.AddNewRider(AddedRider);
            // now the above operations have propagated changes to the UI via Change Notification.
            // this entry will be stuck at the last entry so update the selectedItem in the drop down.
            this.RiderEditSelect.SelectedItem = AddedRider;
		}

		private void DeleteRider_Click(object sender, RoutedEventArgs e)
		{
			if (Riders.RidersList.Count > 0 && RiderEditSelect.SelectedIndex > -1)
			{
				Rider SelectedRider = Riders.RidersList[RiderEditSelect.SelectedIndex];
				Riders.RemoveRider(SelectedRider);
			}
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

        private void ANTSensors_Click(object sender, RoutedEventArgs e) {
            Rider rider = RiderEditSelect.SelectedItem as Rider;

            RacerMateOne.Dialogs.ANTSensorsDialog dialog = new RacerMateOne.Dialogs.ANTSensorsDialog();
            dialog.Owner = AppWin.Instance;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.HrSensorIdText.Text = rider.HrSensorId.ToString();
            dialog.CadenceSensorIdText.Text = rider.CadenceSensorId.ToString();
            dialog.ShowDialog();

            // HeartRate sensor
            int hrSensorId = rider.HrSensorId;
            if (dialog.HrSensorId != null && Int32.TryParse(dialog.HrSensorId, out hrSensorId))
            {
                rider.HrSensorId = hrSensorId;
            }

            // Cadence sensor
            int cadenceSensorId = rider.CadenceSensorId;
            if (dialog.CadenceSensorId != null && Int32.TryParse(dialog.CadenceSensorId, out cadenceSensorId))
            {
                rider.CadenceSensorId = cadenceSensorId;
            }
        }

        private void SetGearTeeth_Click(object sender, RoutedEventArgs e)  {
			Rider rider = RiderEditSelect.SelectedItem as Rider;

			/*
			bool old = true;
			if (old) {
				//RacerMateOne.CourseEditorDev.Options.SetGearTeeth1 p = new RacerMateOne.CourseEditorDev.Options.SetGearTeeth1(rider);
				//RacerMateOne.Pages.Dialogs.SetGearTeeth1 p = new RacerMateOne.Pages.Dialogs.SetGearTeeth1(rider);
				RacerMateOne.Pages.Dialogs.SetGearTeeth1 p = null;
			}
			else {
				RacerMateOne.CourseEditorDev.Options.SetGearTeeth2 p = null;
			}
			*/

			if (rider != null)  {
					// tlm20150506: change between old velotron gear code and the new stuff here

				bool old = true;

				if (old) {
					//RacerMateOne.CourseEditorDev.Options.SetGearTeeth1 p = new RacerMateOne.CourseEditorDev.Options.SetGearTeeth1(rider);
					//RacerMateOne.Pages.Dialogs.SetGearTeeth1 p = new RacerMateOne.Pages.Dialogs.SetGearTeeth1(rider);
#if DEBUG
					Debug.WriteLine("RideOptionsAll.xaml.cs, creating SetGearTeeth1 dialog");
#endif

					RacerMateOne.Pages.Dialogs.SetGearTeeth1 p = new RacerMateOne.Pages.Dialogs.SetGearTeeth1(rider);

#if DEBUG
					Debug.WriteLine("RideOptionsAll.xaml.cs, back from creating SetGearTeeth1 dialog");
					Debug.WriteLine("RideOptionsAll.xaml.cs, before Navigate()");
#endif
					AppWin.Instance.MainFrame.Navigate(p);
#if DEBUG
					Debug.WriteLine("RideOptionsAll.xaml.cs, after Navigate()");
#endif
				}
				else {
					RacerMateOne.CourseEditorDev.Options.SetGearTeeth2 p = new RacerMateOne.CourseEditorDev.Options.SetGearTeeth2(rider);
					AppWin.Instance.MainFrame.Navigate(p);
				}

             //Pages.Dialogs.SetGearTeeth p = new Pages.Dialogs.SetGearTeeth();
             //p.CurRider = rider;
             //if (DemoBike.IsVisible)
             //    DemoBike.ViewOffFade();

				//AppWin.Instance.MainFrame.Navigate(p);

				return;
			}

		}


		private void Rider_HRAnT_LostFocus(object sender, RoutedEventArgs e)
		{
			Rider rider = RiderEditSelect.SelectedItem as Rider;
			if (rider != null)
			{
				rider.HrAnT = Convert.ToInt32(Rider_HRAnT.Text);
			}
		}



		//===============================================================================================
		private bool m_bDisplayInit = false;
		private void C_PerformanceDisplay_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Binding binding;
			Grid grid = sender as Grid;
			bool active = grid.Visibility == Visibility.Visible;
			if (active && !m_bDisplayInit)
			{

				m_bDisplayInit = true;

				// 
				DisplayCatagories.IsSynchronizedWithCurrentItem = true;
				binding = new Binding();
				binding.Source = ReportColumns.DisplayTemplateList;
				DisplayCatagories.SetBinding(ItemsControl.ItemsSourceProperty, binding);

				// Bind the rider 
				DisplaySub.IsSynchronizedWithCurrentItem = true;  //I cannot stress more the importance of this little gem.
				
				binding = new Binding("SubItemList");//set binding parameters if necessary
				binding.Source = DisplayCatagories.ItemsSource;
				DisplaySub.SetBinding(ItemsControl.ItemsSourceProperty, binding);

				binding = new Binding("ShowKeypadPresses");
				binding.Source = RM1_Settings.General;
				ShowKeypadPresses.SetBinding(CheckBox.IsCheckedProperty, binding);


				// = 
				PerfModeDefaults.IsSynchronizedWithCurrentItem = true;
				binding = new Binding();
				binding.Source = ReportColumns.DisplayList;
				PerfModeDefaults.SetBinding(ItemsControl.ItemsSourceProperty, binding);

				binding = new Binding("SelectedItem");
				binding.Source = DisplaySub;
				PerfModeDefaults.SetBinding(ComboBox.SelectedItemProperty, binding);
				// = 

				binding = new Binding("SaveAs_Show");
				binding.Source = ReportColumns.DisplayList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				PerfModeSaveAs.SetBinding(Controls.PlainButton.VisibilityProperty, binding);

				binding = new Binding("Reset_Show");
				binding.Source = ReportColumns.DisplayList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				PerfModeResetToDefault.SetBinding(Controls.PlainButton.VisibilityProperty, binding);

				binding = new Binding("Delete_Show");
				binding.Source = ReportColumns.DisplayList;
				PerfModeDelete.SetBinding(Controls.PlainButton.VisibilityProperty, binding);

				ckBindEx( "Speed", ReportColumns.DisplayList, D_Speed);
				ckBindEx( "Speed_Avg", ReportColumns.DisplayList, D_Speed_Avg );
				ckBindEx( "Speed_Max", ReportColumns.DisplayList, D_Speed_Max );
				ckBindEx( "Watts", ReportColumns.DisplayList, D_Watts );
				ckBindEx( "Watts_Avg", ReportColumns.DisplayList, D_Watts_Avg );
				ckBindEx( "Watts_Max", ReportColumns.DisplayList, D_Watts_Max );
				ckBindEx( "Watts_Wkg", ReportColumns.DisplayList, D_Watts_Wkg);
				ckBindEx( "HeartRate", ReportColumns.DisplayList, D_HeartRate );
				ckBindEx( "HeartRate_Avg", ReportColumns.DisplayList, D_HeartRate_Avg );
				ckBindEx( "HeartRate_Max", ReportColumns.DisplayList, D_HeartRate_Max );
				ckBindEx( "Cadence", ReportColumns.DisplayList, D_Cadence );
				ckBindEx( "Cadence_Avg", ReportColumns.DisplayList, D_Cadence_Avg );
				ckBindEx( "Cadence_Max", ReportColumns.DisplayList, D_Cadence_Max );
				ckBindEx( "Distance", ReportColumns.DisplayList, D_Distance );
				ckBindEx( "Lead", ReportColumns.DisplayList, D_Lead );
				ckBindEx( "Grade", ReportColumns.DisplayList, D_Grade );
				ckBindEx( "Wind", ReportColumns.DisplayList, D_Wind );
				//ckBindEx( "Load", ReportColumns.DisplayList, D_Load);
				//ckBindEx( "PercentAT", ReportColumns.DisplayList, D_PercentAT);
				ckBindEx( "Calories", ReportColumns.DisplayList, D_Calories);
				ckBindEx( "PulsePower", ReportColumns.DisplayList, D_PulsePower );
				ckBindEx( "DragFactor", ReportColumns.DisplayList, D_DragFactor );
				ckBindEx( "TSS_IF_NP", ReportColumns.DisplayList, D_TSS_IF_NP );
				ckBindEx( "Gear", ReportColumns.DisplayList, D_Gear );
				ckBindEx( "Gearing", ReportColumns.DisplayList, D_Gearing);
				ckBindEx( "Load", ReportColumns.DisplayList, D_Load);
				ckBindEx( "PercentAT", ReportColumns.DisplayList, D_PercentAT);
				
				RB_English.IsChecked = !RM1_Settings.General.Metric;
				RB_Metric.IsChecked = RM1_Settings.General.Metric;
			}
			// Any Velotron
			if (!Unit.AnyVelotron)
			{
				StackPanel p;
				p = VisualTreeHelper.GetParent(D_Gear) as StackPanel;
				p.Opacity = 0.5;
				p = VisualTreeHelper.GetParent(D_Gearing) as StackPanel;
				p.Opacity = 0.5;
			}

		}

		private void PerfModeDefaults_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DisplayNameLine.Visibility = DisplaySub.Items.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
		}



		private void FixDisplaySub()
		{
			ReportColumns rc = DisplayCatagories.SelectedItem as ReportColumns;
			if (rc != null)
				DisplaySub.SelectedItem = rc.SubItemList[0];
			DisplayNameLine.Visibility = DisplaySub.Items.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
		}
		private void DisplayCatagories_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			FixDisplaySub();
		}

		private void PerfModeResetToDefault_Click(object sender, RoutedEventArgs e)
		{
			ReportColumns rc = PerfModeDefaults.SelectedItem as ReportColumns;
			rc.Reset();
		}

		private void PerfModeSaveAs_Click(object sender, RoutedEventArgs e)
		{
			ReportColumns nbase = PerfModeDefaults.SelectedItem as ReportColumns;
			if (nbase == null)
				return;
			RacerMateOne.Dialogs.TextLine tline = new RacerMateOne.Dialogs.TextLine();
			tline.TopText.Text = "Name for this";
			tline.Owner = AppWin.Instance;
			tline.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			tline.ShowDialog();
			if (tline.OutText == null)
				return;
			String title = tline.OutText.Trim();
			if (title == "")
				return;

			ReportColumns nc = null;
			foreach (ReportColumns rc in ReportColumns.DisplayList)
			{
				if (rc.Title == title)
				{
					RacerMateOne.Dialogs.Ask ask = new RacerMateOne.Dialogs.Ask("Do you wish to overwrite this exisiting profile?", "Yes", "No");
					ask.Owner = AppWin.Instance;
					ask.ShowDialog();
					if (!ask.IsOK)
						return;

					nc = rc;
					break;
				}
			}
			if (nc == null)
			{
				nc = new ReportColumns();
				nc.Title = title;
				nc.SetTo(nbase);
				nc.Original = null;
				ReportColumns.DisplayList.Add(nc);
			}
			else
				nc.SetTo(nbase);
			PerfModeDefaults.SelectedItem = nc;
		}

		protected class ShowOpacity: IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return value == null ? null : (object)((Visibility)value == Visibility.Visible ? 1.0:0.5);
			}
			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}
		static ShowOpacity ms_OpacityConvert = new ShowOpacity();

		protected class ShowEnable : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return value == null ? null : (object)((Visibility)value == Visibility.Visible ? true : false);
			}
			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}
		static ShowEnable ms_EnableConvert = new ShowEnable();

		void ckBind(String name, object source, FrameworkElement f)
		{
			Binding b = new Binding(name);
			b.Source = source;
			f.SetBinding(CheckBox.IsCheckedProperty, b);

			b = new Binding(name + "_Show");
			b.Source = source;
			f.SetBinding(CheckBox.VisibilityProperty, b);
		}

		void ckBindEx(String name, object source, FrameworkElement f)
		{
			Binding b = new Binding(name);
			b.Source = source;
			f.SetBinding(CheckBox.IsCheckedProperty, b);

			StackPanel p = VisualTreeHelper.GetParent(f) as StackPanel;
			if (p != null)
			{
				b = new Binding(name + "_Show");
				b.Source = source;
				b.Converter = ms_OpacityConvert;
				p.SetBinding(StackPanel.OpacityProperty, b);
			}

			b = new Binding(name + "_Show");
			b.Source = source;
			b.Converter = ms_EnableConvert;
			f.SetBinding(CheckBox.IsEnabledProperty, b);

		}

		private void PerfModeDelete_Click(object sender, RoutedEventArgs e)
		{
			ReportColumns nbase = PerfModeDefaults.SelectedItem as ReportColumns;
			if (nbase != null)
			{
				nbase.Remove();
				nbase.ShowTemplate = null;
				FixDisplaySub();
			}
		}

		private void RB_English_Checked(object sender, RoutedEventArgs e)
		{
			RM1_Settings.General.Metric = false;
			MetricChanged();
		}
		private void RB_Metric_Checked(object sender, RoutedEventArgs e)
		{
			RM1_Settings.General.Metric = true;
			MetricChanged();
		}
        void MetricChanged()
        {
            m1.Content = RM1_Settings.General.Metric ? "KPH" : "MPH";
            r1.Content = RM1_Settings.General.Metric ? "KPH" : "MPH";
            //RedoColumnHeaders();
        }
		//===============================================================================================
		private bool m_bExportOptionsInit = false;
		private void C_ExportOptions_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Binding binding;
			Grid grid = sender as Grid;
			bool active = grid.Visibility == Visibility.Visible;
			if (active && !m_bExportOptionsInit)
			{

				m_bExportOptionsInit = true;

				ReportCatagories.IsSynchronizedWithCurrentItem = true;
				binding = new Binding();
				binding.Source = ReportColumns.ReportTemplateList;
				ReportCatagories.SetBinding(ItemsControl.ItemsSourceProperty, binding);

				// Bind the rider 
				ReportDefaults.IsSynchronizedWithCurrentItem = true;  //I cannot stress more the importance of this little gem.

				binding = new Binding("SubItemList");//set binding parameters if necessary
				binding.Source = ReportCatagories.ItemsSource; // ReportColumns.ReportList;
				ReportSub.SetBinding(ItemsControl.ItemsSourceProperty, binding);

				// = 
				ReportDefaults.IsSynchronizedWithCurrentItem = true;
				binding = new Binding();
				binding.Source = ReportColumns.ReportList;
				ReportDefaults.SetBinding(ItemsControl.ItemsSourceProperty, binding);

				binding = new Binding("SelectedItem");
				binding.Source = ReportSub;
				ReportDefaults.SetBinding(ComboBox.SelectedItemProperty, binding);				
				// = 

				binding = new Binding("SaveAs_Show");
				binding.Source = ReportColumns.ReportList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				ReportSaveAs.SetBinding(Controls.PlainButton.VisibilityProperty, binding);


				binding = new Binding("Reset_Show");
				binding.Source = ReportColumns.ReportList;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				ReportResetToDefault.SetBinding(Controls.PlainButton.VisibilityProperty, binding);

				binding = new Binding("Delete_Show");
				binding.Source = ReportColumns.ReportList;
				ReportDelete.SetBinding(Controls.PlainButton.VisibilityProperty, binding);

				EnableOpacityConverter ec = new EnableOpacityConverter();
				binding = new Binding("Launch");
				binding.Source = RM1_Settings.General;
				Launch.SetBinding(CheckBox.IsCheckedProperty, binding);
				LaunchPrompt.SetBinding(CheckBox.IsEnabledProperty, binding);

				binding = new Binding("Launch");
				binding.Source = RM1_Settings.General;
				binding.Mode = BindingMode.OneWay;
				binding.Converter = ec;
				LaunchPrompt.SetBinding(CheckBox.OpacityProperty, binding);

				LaunchProgramName.Text = RM1_Settings.General.LaunchProgramName;

				binding = new Binding("LaunchPrompt");
				binding.Source = RM1_Settings.General;
				LaunchPrompt.SetBinding(CheckBox.IsCheckedProperty, binding);

				binding = new Binding("SavePrompt");
				binding.Source = RM1_Settings.General;
				SavePrompt.SetBinding(CheckBox.IsCheckedProperty, binding);


				binding = new Binding("ExportSave");
				binding.Source = RM1_Settings.General;
				ExportSave.SetBinding(CheckBox.IsCheckedProperty, binding);
				ExportPrompt.SetBinding(CheckBox.IsEnabledProperty, binding);

				binding = new Binding("ExportPrompt");
				binding.Source = RM1_Settings.General;
				ExportPrompt.SetBinding(CheckBox.IsCheckedProperty, binding);

				binding = new Binding("ExportSave");
				binding.Source = RM1_Settings.General;
				binding.Mode = BindingMode.OneWay;
				binding.Converter = ec;
				ExportPrompt.SetBinding(CheckBox.OpacityProperty, binding);

				//================================
				binding = new Binding("PWXSave");
				binding.Source = RM1_Settings.General;
				PWXSave.SetBinding(CheckBox.IsCheckedProperty, binding);
				PWXPrompt.SetBinding(CheckBox.IsEnabledProperty, binding);

				binding = new Binding("PWXPrompt");
				binding.Source = RM1_Settings.General;
				PWXPrompt.SetBinding(CheckBox.IsCheckedProperty, binding);

				binding = new Binding("PWXSave");
				binding.Source = RM1_Settings.General;
				binding.Mode = BindingMode.OneWay;
				binding.Converter = ec;
				PWXPrompt.SetBinding(CheckBox.OpacityProperty, binding);

				PWXSaveTo.IsSynchronizedWithCurrentItem = true;  //I cannot stress more the importance of this little gem.
				binding = new Binding("PWXSaveToNumber");
				binding.Source = RM1_Settings.General;
				PWXSaveTo.SetBinding(ComboBox.SelectedIndexProperty, binding);

				binding = new Binding("PWXSaveTo");
				binding.Source = RM1_Settings.General;
				PWXSaveToT.SetBinding(ComboBox.ToolTipProperty, binding);
				binding = new Binding("PWXSaveToVisibility");
				binding.Source = RM1_Settings.General;
				PWXSaveToT.SetBinding(ComboBox.VisibilityProperty, binding);

				binding = new Binding("PWXSaveToTitle");
				binding.Source = RM1_Settings.General;
				PWXSaveToTitle.SetBinding(TextBlock.TextProperty, binding);

				//================================
				binding = new Binding("ReportSave");
				binding.Source = RM1_Settings.General;
				ReportSave.SetBinding(CheckBox.IsCheckedProperty, binding);
				ReportPrompt.SetBinding(CheckBox.IsEnabledProperty, binding);

				binding = new Binding("ReportPrompt");
				binding.Source = RM1_Settings.General;
				ReportPrompt.SetBinding(CheckBox.IsCheckedProperty, binding);

				binding = new Binding("ReportSave");
				binding.Source = RM1_Settings.General;
				binding.Mode = BindingMode.OneWay;
				binding.Converter = ec;
				ReportPrompt.SetBinding(CheckBox.OpacityProperty, binding);

				

				binding = new Binding("DelimiterIndex");
				binding.Source = RM1_Settings.General;
				DelimiterIndex.SetBinding(ComboBox.SelectedIndexProperty, binding);

				binding = new Binding("RateIndex");
				binding.Source = RM1_Settings.General;
				RateIndex.SetBinding(ComboBox.SelectedIndexProperty, binding);



				ckBindEx("Speed", ReportColumns.ReportList, R_Speed );
				ckBindEx("Speed_Avg", ReportColumns.ReportList, R_Speed_Avg);
				ckBindEx("Speed_Max", ReportColumns.ReportList, R_Speed_Max);
				ckBindEx("Watts", ReportColumns.ReportList, R_Watts);
				ckBindEx("Watts_Avg", ReportColumns.ReportList, R_Watts_Avg);
				ckBindEx("Watts_Max", ReportColumns.ReportList, R_Watts_Max);
				ckBindEx("Watts_Wkg", ReportColumns.ReportList, R_Watts_Wkg);
				ckBindEx("Watts_Load", ReportColumns.ReportList, R_Watts_Load);
				ckBindEx("HeartRate", ReportColumns.ReportList, R_HeartRate);
				ckBindEx("HeartRate_Avg", ReportColumns.ReportList, R_HeartRate_Avg);
				ckBindEx("HeartRate_Max", ReportColumns.ReportList, R_HeartRate_Max);
				ckBindEx("Cadence", ReportColumns.ReportList, R_Cadence);
				ckBindEx("Cadence_Avg", ReportColumns.ReportList, R_Cadence_Avg);
				ckBindEx("Cadence_Max", ReportColumns.ReportList, R_Cadence_Max);
				ckBindEx("Distance", ReportColumns.ReportList, R_Distance);
				//ckBindEx("Lead", ReportColumns.ReportList, R_Lead);
				ckBindEx("Grade", ReportColumns.ReportList, R_Grade);
				ckBindEx("Wind", ReportColumns.ReportList, R_Wind);
				ckBindEx("Watts_Load", ReportColumns.ReportList, R_Watts_Load);
				ckBindEx("PercentAT", ReportColumns.ReportList, R_PercentAT);
				ckBindEx("Calories", ReportColumns.ReportList, R_Calories);
				ckBindEx("PulsePower", ReportColumns.ReportList, R_PulsePower);
				ckBindEx("DragFactor", ReportColumns.ReportList, R_DragFactor);
				ckBindEx("TSS_IF_NP", ReportColumns.ReportList, R_TSS_IF_NP);
				ckBindEx("SpinScan", ReportColumns.ReportList, R_SpinScan);
				ckBindEx("LeftSS", ReportColumns.ReportList, R_LeftSS);
				ckBindEx("RightSS", ReportColumns.ReportList, R_RightSS);
				ckBindEx("LeftATA", ReportColumns.ReportList, R_LeftATA);
				ckBindEx("RightATA", ReportColumns.ReportList, R_RightATA);
				ckBindEx("LeftPower", ReportColumns.ReportList, R_LeftPower);
				ckBindEx("RightPower", ReportColumns.ReportList, R_RightPower);
				ckBindEx("Gear", ReportColumns.ReportList, R_Gear);
				ckBindEx("Gearing", ReportColumns.ReportList, R_Gearing);
				ckBindEx("RawSpinScan", ReportColumns.ReportList, R_RawSpinScan);
				ckBindEx("CadenceTiming", ReportColumns.ReportList, R_CadenceTiming);
			}
			if (active)
			{
				d_VelotronOnly.Opacity = Unit.AnyVelotron ? 1.0:0.5;
			}


		}

		private void FixReportSub()
		{
			ReportColumns rc = ReportCatagories.SelectedItem as ReportColumns;
			if (rc != null)
				ReportSub.SelectedItem = rc.SubItemList[0];
			ReportNameLine.Visibility = ReportSub.Items.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
		}

		private void ReportDefaults_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ReportNameLine.Visibility = ReportSub.Items.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
		}

		private void ReportCatagories_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			FixReportSub();
		}

		private void ReportResetToDefault_Click(object sender, RoutedEventArgs e)
		{
			ReportColumns rc = ReportDefaults.SelectedItem as ReportColumns;
			rc.Reset();
		}

		private void ReportSaveAs_Click(object sender, RoutedEventArgs e)
		{
			ReportColumns nbase = ReportDefaults.SelectedItem as ReportColumns;
			if (nbase == null)
				return;
			RacerMateOne.Dialogs.TextLine tline = new RacerMateOne.Dialogs.TextLine();
			tline.TopText.Text = "Name for this report";
			tline.Owner = AppWin.Instance;
			tline.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			tline.ShowDialog();
			if (tline.OutText == null)
				return;
			String title = tline.OutText.Trim();
			if (title == "")
				return;

			ReportColumns nc = null;
			foreach (ReportColumns rc in ReportColumns.ReportList)
			{
				if (rc.Title == title)
				{
					RacerMateOne.Dialogs.Ask ask = new RacerMateOne.Dialogs.Ask("Do you wish to overwrite this exisiting profile?", "Yes", "No");
					ask.Owner = AppWin.Instance;
					ask.ShowDialog();
					if (!ask.IsOK)
						return;


					nc = rc;
					break;
				}
			}
			if (nc == null)
			{
				nc = new ReportColumns();
				nc.Title = title;
				nc.SetTo(nbase);
				nc.Original = null;
				ReportColumns.ReportList.Add(nc);
			}
			else
				nc.SetTo( nbase );
			ReportDefaults.SelectedItem = nc;
			ReportSub.SelectedItem = nc;
		}
		private void ReportDelete_Click(object sender, RoutedEventArgs e)
		{
			ReportColumns nbase = ReportDefaults.SelectedItem as ReportColumns;
			if (nbase != null && nbase.Original == null)
			{
				ReportColumns.ReportList.Remove(nbase);
				nbase.ShowTemplate = null;
				FixReportSub();
			}
		}

		private void ReportExportBrowse_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog();
			dialog.SelectedPath = RM1_Settings.General.PWXSaveToNumber == 0 ? 
				RacerMatePaths.ReportsFullPath:RM1_Settings.General.PWXSaveTo;
			System.Windows.Forms.DialogResult result = dialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				RM1_Settings.General.PWXSaveTo = dialog.SelectedPath;
				RM1_Settings.General.PWXSaveToNumber = 1;
			}
		}

		//==============================================================================================================
		private void C_ReportOptions_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{

		}
		//==============================================================================================================
		private bool m_bDevelopmentInit = false;
		private void C_Develpment_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			//Binding binding;
			Grid grid = sender as Grid;
			bool active = grid.Visibility == Visibility.Visible;
			if (active && !m_bDevelopmentInit)
			{
				m_bDevelopmentInit = true;
				ckBind("DemoDevice", RM1_Settings.General, dev_DemoUnit);
			}
		}

		private void Edit_Click(object sender, RoutedEventArgs e)
		{
         //   CourseFilter cf = t_Filter.SelectedItem as CourseFilter;
            //CourseFilter cf = null;
            MessageBox.Show("Find cource type /RacerMateOne_Source/RacerMateOne/Pages/RideOptionsAll.xaml.cs");
			if (DemoBike.IsVisible)
				DemoBike.ViewOffFade();
            //#if DEBUG
            //    RacerMateOne.Pages.Modes.CourseEditor page = new Pages.Modes.CourseEditor(cf);
            //#else
            //    RacerMateOne.Pages.ComingSoon page = new ComingSoon("Course Editor", "Course_Creator.htm", null);
            //#endif
                RacerMateOne.Pages.ComingSoon page = new ComingSoon("Course Editor", "Course_Creator.htm", null);
			AppWin.Instance.MainFrame.Navigate(page);
		}

        //bool m_bShowAll = true;

        //private void Course_ShowAll_Click(object sender, RoutedEventArgs e)
        //{
        //    //if (!m_bShowAll)
        //    //{
        //    //    m_bShowAll = true;
        //    //    Course_ShowAll.Selected = true;
        //    //    Course_Zoom.Selected = false;
        //    //    CourseViewer.Zoom = false;
        //    //}

        //}

        //private void Course_Zoom_Click(object sender, RoutedEventArgs e)
        //{
        //    //if (m_bShowAll)
        //    //{
        //    //    m_bShowAll = false;
        //    //    Course_ShowAll.Selected = false;
        //    //    Course_Zoom.Selected = true;
        //    //    CourseViewer.Zoom = true;
        //    //}

        //}

        //private void CourseDirection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Debug.WriteLine("direction selection is changed");
        //    UpdateCourseDirection();
        //}

        //private void UpdateCourseDirection()
        //{
        //    //Debug.WriteLine("updating course direction to " + CourseDirection.SelectedIndex);
        // //if (CourseViewer == null)
        // //       return;
        // //   int n = CourseDirection.SelectedIndex;
        // //   switch (n)
        // //   {
        // //       case 0: CourseViewer.SetFlags(false, false); break; // CourseViewer.Mirror = false; CourseViewer.Reverse = false; break;
        // //       case 1: CourseViewer.SetFlags(false, true); break; // CourseViewer.Mirror = false; CourseViewer.Reverse = true; break;
        // //       case 2: CourseViewer.SetFlags(true, false); break; // CourseViewer.Mirror = true; CourseViewer.Reverse = false; break;
        // //   }
        //}

        //private void Laps_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //try
        //    //{
        //    //    int laps = Convert.ToInt32(Laps.Text);
        //    //    CourseViewer.Laps = laps;
        //    //    SaveAs.Visibility = CourseViewer.NeedSave ? Visibility.Visible : Visibility.Hidden;
        //    //}
        //    //catch { }
        //}


        //private void CourseViewer_NeedSaveChanged(object sender, RoutedEventArgs e)
        //{
        //    //bool ns = CourseViewer.NeedSave;
        //    //SaveAs.Visibility = ns ? Visibility.Visible : Visibility.Hidden;
        //}

		//==================================================
		private bool m_C_ChartSettings_Init;
		private void C_ChartSettings_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Grid grid = sender as Grid;
			if (grid.IsVisible == true && !m_C_ChartSettings_Init)
			{
				m_C_ChartSettings_Init = true;
				BindTo("GraphSpeed_Min", RM1_Settings.General, c_GraphSpeed_Min, TextBox.TextProperty, null);
				BindTo("GraphSpeed_Max", RM1_Settings.General, c_GraphSpeed_Max, TextBox.TextProperty, null);
				BindTo("GraphPower_Min", RM1_Settings.General, c_GraphPower_Min, TextBox.TextProperty, null);
				BindTo("GraphPower_Max", RM1_Settings.General, c_GraphPower_Max, TextBox.TextProperty, null);
				BindTo("GraphRPM_Min", RM1_Settings.General, c_GraphRPM_Min, TextBox.TextProperty, null);
				BindTo("GraphRPM_Max", RM1_Settings.General, c_GraphRPM_Max, TextBox.TextProperty, null);
                BindTo("GraphRPM_Target", RM1_Settings.General, c_GraphRPM_Target, TextBox.TextProperty, null);
                BindTo("GraphHR_Min", RM1_Settings.General, c_GraphHR_Min, TextBox.TextProperty, null);
				BindTo("GraphHR_Max", RM1_Settings.General, c_GraphRPM_Max, TextBox.TextProperty, null);
				BindTo("GraphPulsePower_Min", RM1_Settings.General, c_GraphPulsePower_Min, TextBox.TextProperty, null);
				BindTo("GraphPulsePower_Max", RM1_Settings.General, c_GraphRPM_Max, TextBox.TextProperty, null);
				BindTo("GraphSpinScanPercent_Min", RM1_Settings.General, c_GraphSpinScanPercent_Min, TextBox.TextProperty, null);
				BindTo("GraphSpinScanPercent_Max", RM1_Settings.General, c_GraphRPM_Max, TextBox.TextProperty, null);
				BindTo("GraphPowerSplit_Min", RM1_Settings.General, c_GraphPowerSplit_Min, TextBox.TextProperty, null);
				BindTo("GraphPowerSplit_Max", RM1_Settings.General, c_GraphRPM_Max, TextBox.TextProperty, null);
				BindTo("GraphATA_Min", RM1_Settings.General, c_GraphATA_Min, TextBox.TextProperty, null);
				BindTo("GraphATA_Max", RM1_Settings.General, c_GraphRPM_Max, TextBox.TextProperty, null);
				Binding binding = new Binding("CalibrationCheck");
				binding.Source = RM1_Settings.General;
				c_CalibrationCheck.SetBinding(CheckBox.IsCheckedProperty, binding);

				BindTo("XPath_RCV", RM1_Settings.General, c_RCVPath, TextBox.TextProperty, null);
			}
		}
		private void btn_Browse_RCV(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog();
			dialog.SelectedPath = RM1_Settings.General.XPath_RCV;
			System.Windows.Forms.DialogResult result = dialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				String v = RM1_Settings.General.Path_RCV;

				RM1_Settings.General.XPath_RCV = dialog.SelectedPath;
				if (String.Compare(RM1_Settings.General.Path_RCV,v,true)!=0)
				{
					Courses.RemoveByDirectory(v);
					Courses.Scan(RM1_Settings.General.Path_RCV);
				}
                //RedoCoursePicker();
			}
			/*
			OpenFileDialog f = new OpenFileDialog();
			f.InitialDirectory = RacerMatePaths.SettingsFullPath;
			f.Multiselect = false;
			f.Title = "Select a Riders.csv to import";
			f.ValidateNames = true;
			f.AddExtension = true;
			f.CheckFileExists = true;
			f.DefaultExt = ".csv";
			f.Filter = "CSV Files (.csv)|Riders*.csv";
			if (f.ShowDialog() == true)
			{
				Log.WriteLine("Importing \"" + f.FileName + "\"");
			}
			*/
		}

		//==================================================
		static DpConverter m_iconv = new DpConverter(0);
		private void BindTo(String name, Object source, FrameworkElement f, DependencyProperty dp, IValueConverter conv)
		{
			Binding binding = new Binding(name);
			binding.Source = source;
			if (conv != null)
				binding.Converter = conv;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			f.SetBinding(dp, binding);
		}

		//===========================================================================================
		//private bool m_C_Units_Init = false;
        //private void C_Units_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    Grid g = sender as Grid;
        //    if (g.Visibility == Visibility.Visible)
        //    {
        //        //m_C_Units_Init = true;
        //        Unit.LoadFromSettings();
        //        //UnitsList.LoadFromUnits();
        //    }
        //}

        //private void btn_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DemoBike.IsVisible)
        //        DemoBike.ViewOffFade();
        //    Commit();
        //    AppWin.Instance.MainFrame.Navigate(new Pages.SelectRider());
        //}

        private void PickSavedPerformance_Click(object sender, RoutedEventArgs e)
        {
            // Planning to add this functionality
            OpenFileDialog f = new OpenFileDialog();
            f.InitialDirectory = RacerMatePaths.PerformancesFullPath; // this needs to use last folder visited
            f.Multiselect = false;
            f.Title = "Select performance file to export";
            f.ValidateNames = true;
            f.AddExtension = true;
            f.CheckFileExists = true;
            f.DefaultExt = ".rmp";
            f.Filter = "Racermate Performance Files(*.rmp)|*.rmp|All Files|*.*";
            if (f.ShowDialog() == true)
            {
				RacerMateOne.Dialogs.ReportQuestions rq = new RacerMateOne.Dialogs.ReportQuestions();
				rq.Owner = AppWin.Instance;
				rq.ShowDialog();
				if (rq.IsOK)
				{
					RacerMateOne.Dialogs.ExportPerformance ep = new RacerMateOne.Dialogs.ExportPerformance(f.FileName);
					ep.ExportSave = rq.ExportSave;
					ep.PWXSave = rq.PWXSave;
					ep.ReportSave = rq.ReportSave;

					ep.Owner = AppWin.Instance;
					ep.ShowDialog();
				}
				
				/*
                if (RM1_Settings.General.ExportSave || RM1_Settings.General.ExportPrompt || RM1_Settings.General.ReportPrompt)
                {
                    RacerMateOne.Dialogs.SavePerformance sp = new RacerMateOne.Dialogs.SavePerformance(f.FileName);
                    //m_bBlockKeys = true;
                    sp.ShowDialog();
                    //m_bBlockKeys = false;
                }
				 */
            }
        }

		private void LearnMore_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help("Customize_Report.htm");
		}

		bool m_bLimit;
		private void Limit_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (tb == null || m_bLimit)
				return;
			m_bLimit = true;
			try
			{
				String s = tb.Text;
				int sz = Convert.ToInt32(tb.Tag);
				if (s.Length > sz)
				{
					s = s.Substring(s.Length - sz);
				}
				s = ms_RemoveLeadingZeros.Replace(s, "");
				int start = tb.SelectionStart;
				tb.Text = s;
				tb.SelectionStart = start;
			}
			catch { }
			m_bLimit = false;
		}

		static Regex ms_NotDigits = new Regex("[^0-9]+");
		static Regex ms_RemoveLeadingZeros = new Regex("^0*");
		private void Limit_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = ms_NotDigits.IsMatch(e.Text);
		}

		private void t_Credits_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Instance.MainFrame.Navigate(new Pages.Credits());
		}

		private void t_About_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Instance.About();
		}

        private void btn_RestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you wish to clear the settings?  This will also exit the program." + Environment.NewLine + "Upon restart, the default courses will be copied into the local Courses folder.", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                RM1_Settings.DeleteSettingsFile();
                //Riders.DeleteRidersFile();
                AppWin.Exit();
            }
        }

        private void btn_RestoreCourses_Click(object sender, RoutedEventArgs e)
        {
            RacerMateOne.Dialogs.JustInfo diag = new RacerMateOne.Dialogs.JustInfo("Are you sure you wish to restore the default Courses? It will not affect custom courses created.  This will also exit the program.", "OK", "Cancel");
        //    MessageBoxResult result = MessageBox.Show("Are you sure you wish to restore the default Courses? It will not affect custom courses created.  This will also exit the program.", "", MessageBoxButton.YesNo);
            bool clickedOkay = (bool)diag.ShowDialog();
            if (diag.IsOK == true)
            {
                RM1_Settings.CopyDefaultCoursesToLocal();
                AppWin.Exit();
            }
        }

        private void btn_DefaultSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you wish to clear the settings?  This will also exit the program." + Environment.NewLine + "Upon restart, the default courses will be copied into the local Courses folder.", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                RM1_Settings.DeleteSettingsFile();
                //Riders.DeleteRidersFile();
                AppWin.Exit();
            }
        }

        private void t_Updates_Click(object sender, RoutedEventArgs e)
        {
            AppWin.Help("updates.htm");
        }

       

	}
}
