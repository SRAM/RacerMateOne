
#pragma warning disable 414

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
using System.Text.RegularExpressions;

namespace RacerMateOne.Pages  {
	/// <summary>
	/// Interaction logic for SelectRider.xaml
	/// </summary>
	public partial class SelectRider : Page
	{
		class Node
		{
			public Unit			Unit;
			public Label		RiderName;
			public Label		Type;
			public ListBoxItem  Item;
			public Brush TextColor;

			Bot m_Bot;


			public Node( Unit unit, Label ridername, Label type, ListBoxItem item )
			{
				Unit = unit;
				RiderName = ridername;
				Type = type;
				Item = item;
				Item.Tag = this;
				TextColor = Type.Foreground;
			}
			~Node()
			{
				if (m_Bot != null)
					m_Bot.KeyChanged -= new Bot.KeyChangedEvent(cb_BotKeyChanged);
			}

			void cb_BotKeyChanged(Bot bot)
			{
				Update();
			}


			public void Update()
			{
				if (m_Bot != Unit.Bot)
				{
					if (m_Bot != null)
						m_Bot.KeyChanged -= new Bot.KeyChangedEvent(cb_BotKeyChanged);
					m_Bot = Unit.Bot;
					if (m_Bot != null)
						m_Bot.KeyChanged += new Bot.KeyChangedEvent(cb_BotKeyChanged);
				}
				if (Unit.Bot != null)
				{
					RiderName.Content = Unit.Bot.UnitContent;
					Type.Content = Unit.Bot.EditArea();
					Type.Foreground = TextColor;
				}
				else
				{
					RiderName.Content = Unit.Statistics.RiderName;
					String t = Unit.Trainer == null ? "-" :
						Unit.TC.DeviceType == RM1.DeviceType.COMPUTRAINER ? "CompuTrainer" :
						Unit.Trainer.Type == RM1.DeviceType.VELOTRON ? "Velotron" :
						Unit.TC == null ? "-" :
						Unit.TC.DeviceType == RM1.DeviceType.COMPUTRAINER ? "CompuTrainer" :
						Unit.TC.DeviceType == RM1.DeviceType.VELOTRON ? "Velotron" : "-";

					Brush b = t == "-" || (Unit.Trainer != null && Unit.Trainer.IsConnected) ? TextColor : Brushes.Red;
					if (t == "-" && Unit.Statistics.RiderName == "")
					{
						t = "Add";
						b = TextColor;
					}

					Type.Content = t;
					Type.Foreground = b; 
				}
			}
		}
		Node[] m_Nodes = new Node[8];
		public bool VideoMode;

		private RiderGroup m_RestoreGroup = new RiderGroup();
		public SelectRider()
		{
			m_RestoreGroup.Remove();
			InitializeComponent();
			m_Nodes[0] = new Node(Unit.Units[0], RiderName_0, Type_0, (ListBoxItem)RiderBox.Items[0]);
			m_Nodes[1] = new Node(Unit.Units[1], RiderName_1, Type_1, (ListBoxItem)RiderBox.Items[1]);
			m_Nodes[2] = new Node(Unit.Units[2], RiderName_2, Type_2, (ListBoxItem)RiderBox.Items[2]);
			m_Nodes[3] = new Node(Unit.Units[3], RiderName_3, Type_3, (ListBoxItem)RiderBox.Items[3]);
			m_Nodes[4] = new Node(Unit.Units[4], RiderName_4, Type_4, (ListBoxItem)RiderBox.Items[4]);
			m_Nodes[5] = new Node(Unit.Units[5], RiderName_5, Type_5, (ListBoxItem)RiderBox.Items[5]);
			m_Nodes[6] = new Node(Unit.Units[6], RiderName_6, Type_6, (ListBoxItem)RiderBox.Items[6]);
			m_Nodes[7] = new Node(Unit.Units[7], RiderName_7, Type_7, (ListBoxItem)RiderBox.Items[7]);

			Binding binding;

			GroupSelect.IsSynchronizedWithCurrentItem = true;
			binding = new Binding();
			binding.Source = RiderGroup.GroupList;
			GroupSelect.SetBinding(ItemsControl.ItemsSourceProperty, binding);
			GroupSelect.SelectedItem = RiderGroup.Selected;
			m_LBStyle = (Style)FindResource("LBLabel");
		}
		ActiveUnitSave m_ActiveSave = new ActiveUnitSave();

		bool m_bInit;

		private void RiderOptions_Loaded(object sender, RoutedEventArgs e)  {
			if (SelectedRider == null)
				SelectedRider = m_Nodes[0];

			Binding binding;
			binding = new Binding();
			d_CourseFilter.IsSynchronizedWithCurrentItem = true;
			binding.Source = Courses.PerformanceCourses;
			d_CourseFilter.SetBinding(ItemsControl.ItemsSourceProperty, binding);

			m_ActiveSave.Save();
			UpdateNodes();
			t_Rider.Selected = true;

			m_RestoreGroup.SaveFromUnits();

			if (VideoMode)
			{
				t_Pacer.Opacity = 0.3;
			}

			m_bInit = true;
			UpdateList();
			UpdateFilter();
		}

		void UpdateNodes()
		{
			foreach (Node n in m_Nodes)
			{
				n.Update();
			}
		}

		public String Mode;
		//=============================================================
		private void Options_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Pages.RideOptions());
		}
		private void Back_Click(object sender, RoutedEventArgs e)
		{
			m_ActiveSave.Restore();
			Unit.SaveToSettings();
			if (Mode != null)
				RM1_Settings.General.ActiveUnits[Mode] = Unit.ActiveUnits;
			NavigationService.GoBack();
		}

		private void Start_Click(object sender, RoutedEventArgs e)
		{
			m_ActiveSave.Restore();
			Unit.SaveToSettings();
			if (Mode != null) {
				RM1_Settings.General.ActiveUnits[Mode] = Unit.ActiveUnits;
			}

			// tlm2014
			System.Type t = sender.GetType();							// RacerMateOne.Controlls.Pillbox
			if (t.FullName == "RacerMateOne.Controls.Pillbox") {
				RacerMateOne.Controls.Pillbox pb = (RacerMateOne.Controls.Pillbox)sender;
				String s = pb.Text;
				if (s == "Save") {
					RM1_Settings.SaveToFile();
					//Riders.SaveToFile();									// any need to do this?
				}
			}


			NavigationService.GoBack();
		}

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help("Pick_Rider.htm");
		}
		//=============================================================
		Dictionary<object, ListBoxItem> m_RiderCache = new Dictionary<object, ListBoxItem>();


		Style m_LBStyle = null;
		Label StdLabel(string text, double width)
		{
			Label lb = new Label();
			lb.Style = m_LBStyle;
			if (width > 0.0)
			{
				TextBlock tb = new TextBlock();
				tb.Text = text;
				tb.Width = width;
				tb.TextTrimming = TextTrimming.CharacterEllipsis;
				lb.Content = tb;
			}
			else
				lb.Content = text;
			return lb;
		}


		String mp_Filter = "";
		String Filter
		{
			get { return mp_Filter; }
			set
			{
				String v = value == null ? "":value.Trim();
				if (v != mp_Filter)
				{
					mp_Filter = v;
					UpdateList();
				}
			}
		}

		bool m_InListUpdate;

		void UpdateList()  {
			m_InListUpdate = true;

			try  {
				Style lbstyle = (Style)FindResource("ListBoxItemSquare");

				Node n = SelectedRider;
				ListBoxItem e;
				bool check = mp_Filter != "";

				//zzz

				if (m_CurTab == t_Rider)  {
					d_CoursePicker.Visibility = Visibility.Hidden;
					RList.Visibility = Visibility.Visible;
				   RList.Items.Clear();
					d_CoursePicker.SelectedCourse = null;

					foreach (Rider r in Riders.RidersList)  {
						if (check && !Regex.IsMatch(r.FullName, mp_Filter, RegexOptions.IgnoreCase) && !Regex.IsMatch(r.NickName, mp_Filter, RegexOptions.IgnoreCase))  {
							if (m_RiderCache.TryGetValue(r, out e)) {
								RList.Items.Remove(e);
							}
							continue;
						}

						if (!m_RiderCache.TryGetValue(r, out e))  {
							e = new ListBoxItem();
							e.Height = 28.0;
							e.Style = lbstyle;
							StackPanel sp = new StackPanel();
							sp.Orientation = Orientation.Horizontal;
							sp.Children.Add(StdLabel(r.NickName, 150));
							sp.Children.Add(StdLabel(r.FullName, 225));
							e.Content = sp;
							m_RiderCache[r] = e;
							e.Tag = r;
						}

						RList.Items.Add(e);

						if (n.Unit.Bot == null && n.Unit.Rider == r) {
							RList.SelectedItem = e;
						}

					}
				}

				else if (m_CurTab == t_Performance)  {
					RList.Visibility = Visibility.Hidden;
					d_CoursePicker.Visibility = Visibility.Visible;

					RList.Items.Clear();

					if (!m_bLoading)  {
						m_bLoading = true;
						d_LoadingMessage.Visibility = Visibility.Visible;
						d_Loading.Active = true;
						Courses.OnLoadPerformanceDone += new Courses.LoadPerformanceDone(Courses_OnLoadPerformanceDone);
						Courses.LoadPerformances();
					}
					UpdateFilter();
            }

				else if (m_CurTab == t_Pacer)  {
					d_CoursePicker.Visibility = Visibility.Hidden;
               d_CoursePicker.SelectedCourse = null;
               RList.Visibility = Visibility.Visible;
					RList.Items.Clear();

					foreach (IBotInfo binfo in Bot.Bots)  {
						e = new ListBoxItem();
						e.Height = 28.0;
						e.Style = lbstyle;
						e.Content = binfo.Info();
						e.Tag = binfo;
						RList.Items.Add(e);
						if (n.Unit.Bot != null && n.Unit.Bot.Info == binfo)
							RList.SelectedItem = e;
					}
				}
			}
			catch {

			}
			m_InListUpdate = false;
		}


		bool m_bLoading;


		void Courses_OnLoadPerformanceDone()  {
			d_LoadingMessage.Visibility = Visibility.Hidden;
			d_Loading.Active = false;
			Courses.OnLoadPerformanceDone -= new Courses.LoadPerformanceDone(Courses_OnLoadPerformanceDone);
		}


		Node m_SelectedRider;

		Node SelectedRider  {
			get {
				return m_SelectedRider;
			}

			set  {
				if (m_SelectedRider == value)
					return;
				m_SelectedRider = value;
				Node n = value;
				ListBoxItem item = value == null ? null:value.Item;
				RiderBox.SelectedItem = item;
				PerformanceBot pb = null;

				if (n.Unit.Bot != null && (pb = n.Unit.Bot as PerformanceBot) != null)  {
					d_CoursePicker.SelectedCourse = pb.Course;
				}

				Controls.RTab pc = VideoMode ? t_Performance : t_Pacer;
				Controls.RTab rtab = n.Unit.Bot == null ? (n.Unit.Trainer != null || n.Unit.Rider != null ? t_Rider : pc) :
					(pb != null ? t_Performance : pc);
				m_CurTab = null; // Force update.
				CurTab = rtab;
				UpdateList();
			}
		}



		//=============================================================

		Controls.RTab m_CurTab;
#if DEBUG
		int bp = 0;
#endif

		Controls.RTab CurTab   {
			get {
				return m_CurTab;
			}

			set  {
				if (value == m_CurTab) {
					return;
				}

#if DEBUG										// tlm
				if (m_CurTab == null) {
					bp = 1;
				}
				else if (m_CurTab == t_Rider) {
					bp = 1;
				}
				else if (m_CurTab == t_Performance) {
					bp = 1;
				}
				else if (m_CurTab == t_Pacer) {
					bp = 1;
				}
				else  {
					bp = 2;
				}
#else
				//bp = 3;
#endif
				m_CurTab = value;
				t_Rider.Selected = t_Performance.Selected = t_Pacer.Selected = false;
				m_CurTab.Selected = true;
				FindArea.Children.Remove(SBox);
				FindArea.Children.Remove(m_CurTab);
				FindArea.Children.Insert(2, SBox);
				FindArea.Children.Insert(3, m_CurTab);
				CourseLine.Visibility = m_CurTab != t_Performance ? Visibility.Collapsed : Visibility.Visible;
				FindLine.Visibility = m_CurTab == t_Pacer ?  Visibility.Collapsed : Visibility.Visible;
				UpdateList();
			}
		}


		private void btn_Click(object sender, RoutedEventArgs e)  {
			Controls.RTab rtab = (Controls.RTab)sender;

#if DEBUG										// tlm
			if (rtab == null) {
				bp = 1;
			}
			else if (rtab == t_Rider) {
				bp = 1;
			}
			else if (rtab == t_Performance) {
				bp = 1;
			}
			else if (rtab == t_Pacer) {
				bp = 1;
			}
			else {
				bp = 2;
			}
#else
				//bp = 3;
#endif

			if (rtab == t_Pacer && VideoMode) {
				return;
			}

			CurTab = rtab;						// invokes CurTab.set(), which sets SelectRider.m_CurTab

#if DEBUG
			bp = 4;
#endif
		}

		private void GroupSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RiderGroup.Selected = GroupSelect.SelectedItem as RiderGroup;
		}


		private void RList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListBoxItem item = (ListBoxItem)RList.SelectedItem;
			Rider r;

			if (item == null || m_InListUpdate)
				return;
			if ((r = item.Tag as Rider) != null)
			{
				m_ActiveSave.SetActive(SelectedRider.Unit.Number, true);
				SelectedRider.Unit.Rider = r;
				SelectedRider.Unit.Bot = null;
				foreach (Node n in m_Nodes)
					n.Update();
			}
			else if ((item.Tag as IBotInfo) != null)
			{
				IBotInfo binfo = item.Tag as IBotInfo;
				Bot bot = binfo.Create( binfo.DefaultKey );
				m_ActiveSave.SetActive(SelectedRider.Unit.Number, true);
				SelectedRider.Unit.Bot = bot;
				foreach (Node n in m_Nodes)
					n.Update();
                if (binfo == PerformanceBot.gInfo)
                {
                    CurTab = t_Performance;
                }
			}
            else if ((item.Tag as PerformanceFileEntry) != null)
            {
                PerformanceBot bot = SelectedRider.Unit.Bot as PerformanceBot;
                if (bot != null)
                {
                    PerformanceFileEntry entry = item.Tag as PerformanceFileEntry;
                    bot.LoadedPerfFile = entry.FileName;
                    foreach (Node n in m_Nodes)
                        n.Update();
                }
            }
        }

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			SelectedRider.Unit.Rider = null;
			SelectedRider.Unit.Bot = null;
			UpdateNodes();
		}

		private void RiderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Node n = ((Node)((ListBoxItem)RiderBox.SelectedItem).Tag);
			SelectedRider = n;
			m_ActiveSave.SetActive(SelectedRider.Unit.Number, true);
		}

		private void d_CoursePicker_CourseSelected(object sender, RoutedEventArgs e)
		{
			Course c = d_CoursePicker.SelectedCourse;
			int percent = 0;
			PerformanceBot pb;
			if (SelectedRider.Unit.Bot != null && (pb = SelectedRider.Unit.Bot as PerformanceBot) != null)
				percent = pb.Percent;
			SelectedRider.Unit.Bot = new PerformanceBot(c.FileName, percent,c);
			m_Hash = c.HeaderHash;
			SelectedRider.Unit.Rider = null;
			Unit.ClearPerformanceBots(c.HeaderHash);
			UpdateNodes();
			d_CourseFilter.SelectedItem = c.Name;
            m_Hash = null; //added this so there is no cull post selection
			UpdateFilter();
		}

		private void RiderOptions_Unloaded(object sender, RoutedEventArgs e)
		{
			Courses.OnLoadPerformanceDone -= new Courses.LoadPerformanceDone(Courses_OnLoadPerformanceDone);
			Courses.CancelLoadPerformance();
		}

		/***********************************************************************************************************************

		***********************************************************************************************************************/

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)  {
			m_Hash = null;
			UpdateFilter();
		}

		private void d_CourseFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)  {
			m_Hash = null;
			UpdateFilter();
		}

		String m_Hash = null;
		String m_CurHash = null;

		String m_CurName = "xxxx";
		String m_CurRider = "";

		/****************************************************************************************************************

		****************************************************************************************************************/

		void UpdateFilter()  {

			try {
			// tlm20140923:
			// made changes here so that the realtime rider search would work

			//if (!m_bInit || m_CurTab != t_Performance)  {
			if (!m_bInit)  {
				return;
			}

			if (m_CurTab == t_Performance) {
				String cname = d_CourseFilter.SelectedItem == null ? "" : d_CourseFilter.SelectedItem.ToString();
				String crider = d_Find.Text;

				if (String.Compare(cname, m_CurName, true) != 0 || String.Compare(crider, m_CurRider, true) != 0 || m_Hash != m_CurHash) {
					m_CurName = cname;
					m_CurRider = crider;
					m_CurHash = m_Hash;
					CourseFilter cf = CourseFilter.F_PerformanceGrade.Clone() as CourseFilter;
					cf.Add(CourseFilter.FilterBy.XUnits, CourseXUnits.Distance);

					if (VideoMode)
						cf.Add(CourseFilter.FilterBy.Video, null);

					if (m_CurHash != null) {
						cf.Add(CourseFilter.FilterBy.HeaderHash, m_CurHash);
					}
					else if (m_CurName != "")
						cf.Add(CourseFilter.FilterBy.CourseName, m_CurName);

					if (m_CurRider != "") {
						Regex reg = new Regex(Regex.Escape(m_CurRider), RegexOptions.IgnoreCase);			// this filters the performances in real time
						cf.Add(CourseFilter.FilterBy.Search, reg);
					}
					d_CoursePicker.Filter = cf;
				}
			}										// if (m_CurTab == t_Performance) {

			else if (m_CurTab == t_Rider)  {

				if (d_CoursePicker.Visibility != Visibility.Hidden)  {
					throw new Exception("error 120");
				}

				if (RList.Visibility != Visibility.Visible) {
					throw new Exception("error 121");
				}

				if (d_CoursePicker.SelectedCourse != null) {
					throw new Exception("error 119");
				}

				String cname = d_CourseFilter.SelectedItem == null ? "" : d_CourseFilter.SelectedItem.ToString();
				String crider = d_Find.Text;

				//zzz

				if (
					String.Compare(cname, m_CurName, true) != 0 || 
					String.Compare(crider, m_CurRider, true) != 0 ||
					m_Hash != m_CurHash)  {

					m_CurName = cname;					// ""
					m_CurRider = crider;					// "b"
					m_CurHash = m_Hash;					// null

					ListBoxItem lbi;

					if (m_CurRider == "")  {					// repopulate the list with everything
						RList.Items.Clear();

						foreach (Rider r in Riders.RidersList) {
							if (m_RiderCache.TryGetValue(r, out lbi)) {
								RList.Items.Add(lbi);
							}								// if (!m_RiderCache.TryGetValue(r, out lbi))
							else {
								throw new Exception("error 122");
							}

							Node n = SelectedRider;
							if (n.Unit.Bot == null && n.Unit.Rider == r) {
								RList.SelectedItem = lbi;
							}
						}
					}						// if (m_CurRider == "")

					else  {
						Regex reg = new Regex(Regex.Escape(m_CurRider), RegexOptions.IgnoreCase);			// this filters the performances in real time
						RList.Items.Clear();
						foreach (Rider r in Riders.RidersList) {
							if (
									Regex.IsMatch(r.FullName, m_CurRider, RegexOptions.IgnoreCase) ||
									Regex.IsMatch(r.NickName, m_CurRider, RegexOptions.IgnoreCase)) {

									if (m_RiderCache.TryGetValue(r, out lbi)) {
										RList.Items.Add(lbi);
									}
							}
						}								// foreach (Rider r in Riders.RidersList)
					}									// if (m_CurRider != "")
				}										// if (String.Compare(cname, m_CurName, true) != 0 || String.Compare(crider, m_CurRider, true) != 0 || m_Hash != m_CurHash)
				else  {
					throw new Exception("error 123");
				}
			}
			else if (m_CurTab == t_Pacer) {
#if DEBUG
				bp = 1;
#endif
			}
			else {
#if DEBUG
				bp = 3;
#endif
			}
			}
			catch (Exception e) {
				// tlm20160218
				//#if DEBUG
				System.Console.WriteLine("SelectRider.xaml.cs.UpdateFilter error: '{0}'\n", e.ToString());
				//#endif
				Log.WriteLine(e.ToString());
			}


		}							// UpdateFilter()

		/****************************************************************************************************************

		****************************************************************************************************************/

		private void d_CourseFilter_DropDownOpened(object sender, EventArgs e)  {
			String s = d_CourseFilter.SelectedItem.ToString();
			Courses.SortPerformanceCourses();
			d_CourseFilter.SelectedItem = s;
		}

	}								// public partial class SelectRider : Page
}									// namespace RacerMateOne.Pages
