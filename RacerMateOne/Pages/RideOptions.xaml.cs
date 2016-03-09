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
using System.Reflection;
//using System.Diagnostics;


namespace RacerMateOne.Pages
{
	/// <summary>
	/// Interaction logic for RideOptions.xaml
	/// </summary>
	public partial class RideOptions : Page
	{

		bool m_bNow = false;
		public Dictionary<String, Controls.RTab> AllTabs = new Dictionary<string, RacerMateOne.Controls.RTab>();
		public RideOptionsAll Options = new RideOptionsAll();
		bool m_bInit = false;

		public bool bAddBack;

		protected class TabInfo
		{
			public String[] Tags;
			public String HelpPage;
		}
		protected Controls.RTab SelectedTab = null;

		public static String ms_SelectedTab = "Riders";

		public RideOptions()
		{
			InitializeComponent();
		}

		private void RiderOptions_Loaded(object sender, RoutedEventArgs e)
		{
			if (!m_bInit)
			{
				Frame.Navigate(Options);
				Options.Background = Brushes.Transparent;
				AddTab("Riders", "@Rider", "Options_Riders.htm");
				AddTab("Hardware setup", "@Hardware", "Options_Hardware.htm");
				AddTab("Display", "@Display", "Options_Display.htm");
				AddTab("File Saving", "@Reports", "Options_File_Saving.htm");
                AddTab("Advanced", "@Advanced", "Options_Advanced.htm");

				//AddTab("All", "@All");
				m_bNow = true;
				if (!SelectTab(ms_SelectedTab))
				{
					ms_SelectedTab = "Riders";
					SelectTab(ms_SelectedTab);
				}
				//AddTab("All", new RideOptionsAll(), true );
				m_bInit = true;
			}
			else
			{
				SelectedTab.Selected = false;
				SelectedTab.Selected = true;
			}
		}
		public void AddTab(String text, String tag, String help)
		{
			AddTab(text, new String[] { tag }, help);
		}
		public void AddTab(String text, String[] tags, String help )
		{
			TabInfo info = new TabInfo();
			info.Tags = tags;
			Controls.RTab rtab = new Controls.RTab();
			rtab.Text = text;
			rtab.Tag = info;
			rtab.Padding = new Thickness(0,0,20,0);
			rtab.Click += new RoutedEventHandler(rtab_Click);
			info.HelpPage = help;
			Tabs.Children.Add(rtab);
			AllTabs[text] = rtab;
		}
		public bool SelectTab(String text)
		{
			Controls.RTab rtab;
			if (AllTabs.TryGetValue(text, out rtab))
			{
				SelectTab(rtab);
				return true;
			}
			return false;
		}

		public void SelectTab(Controls.RTab rtab)
		{
			if (SelectedTab == rtab)
				return;

			if (SelectedTab != null)
				SelectedTab.Selected = false;

			String tag = "";
			if (rtab != null)
			{
				TabInfo info = rtab.Tag as TabInfo;
				rtab.Selected = true;
				if (m_bNow)
				{
					Options.Tags = info.Tags;
					m_bNow = false;
				}
				else
					Options.ChangeTags(info.Tags); // Will fade out the current options and fade in new stuff (if the tags don't match)
				tag = info.Tags[0];
				String txt = ((Controls.RTab)rtab).Text.ToString();
				if (txt == "Hardware setup")
					txt = "Hardware";
				TopLine.Text = txt;

			}
			foreach (Object o in InfoBox.Children)
			{
				FrameworkElement elem = o as FrameworkElement;
				if (elem != null)
				{
					elem.Visibility = (elem.Tag.ToString() == tag ? Visibility.Visible : Visibility.Collapsed);
				}
			}
			SelectedTab = rtab;
			ms_SelectedTab = rtab.Text.ToString();
		}

		private void rtab_Click(Object sender, RoutedEventArgs e)
		{
			Controls.RTab rtab = sender as Controls.RTab;
			if (rtab.Selected)
				return;
			SelectTab(rtab);
		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			if (Options.DemoBike.IsVisible)
				Options.DemoBike.ViewOffFade();
			Save();
			NavigationService.GoBack();
		}

		public void Save()
		{
			RM1_Settings.SaveToFile();
			Riders.SaveToFile();
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			Save();
			if (Options.DemoBike.IsVisible)
				Options.DemoBike.ViewOffFade();
			NavigationService.GoBack();
		}

		private void FindTag_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!m_bInit)
				return;
			String f = FindTag.Text.Trim();
			
			if (SelectTab(f))
				return;
			SelectTab("All");
			List<String> tags = new List<String>();
			String[] arr = f.Split(',');
			String t;
			foreach (String sub in arr)
			{
				String[] warr = sub.Split(' ');
				t = sub.Trim();
				if (t != "" && !tags.Contains(t))
					tags.Add(t);
				foreach (String s in warr)
				{
					t = s.Trim();
					if (t != "" && !tags.Contains(t))
						tags.Add(t);
				}
			}
			//Options.ChangeTags(tags); // Will fade out the current options and fade in new stuff (if the tags don't match)
		}

        private void Start_Click(object sender, RoutedEventArgs e)
        {
			if (Options.DemoBike.IsVisible)
				Options.DemoBike.ViewOffFade();
			Save();
			NavigationService.GoBack();
        }
        private void Options_Click(object sender, RoutedEventArgs e)
        {
			if (Options.DemoBike.IsVisible)
				Options.DemoBike.ViewOffFade();
			NavigationService.Navigate(new Pages.RideOptions());
        }
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedTab != null)
				AppWin.Help(((TabInfo)SelectedTab.Tag).HelpPage);
			else
				AppWin.Help();
		}
	}
}
