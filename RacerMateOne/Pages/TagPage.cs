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
using System.Windows.Media.Animation;


namespace RacerMateOne.Pages
{
	/// <summary>
	/// Interaction logic for RideOptionsAll.xaml
	/// </summary>
	public partial class TagPage : Page
	{
		protected Storyboard FadeOut; // Make sure to set these before starting the page
		protected Storyboard FadeIn;
		public TagPage()
		{
			Loaded += new RoutedEventHandler(page_Loaded);
			Unloaded += new RoutedEventHandler(page_Unloaded);
		}

		public void InitPage( Storyboard fadein, Storyboard fadeout, Panel panel )
		{
			FadeIn = fadein;
			FadeOut = fadeout;
			foreach (Object o in panel.Children)
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
					AddTag("All", item);
				}
			}
			m_All = m_TagMap["@all"];
		}

		static protected String[] ms_Empty = new String[] { };
		static protected String[] ms_AllTag = new String[] { "@All" };
		protected String[] m_Tags = new String[] { };
		protected String[] m_NextTags = null;
		protected Dictionary<String, List<OptionGroup>> m_TagMap = new Dictionary<String, List<OptionGroup>>();
		protected List<OptionGroup> m_All;
		protected Controls.HardwareLine[] m_HardwareLines;
		protected bool m_bLoaded;

		int TagVersion = 0;

		public bool TagOn(String tag)
		{
			foreach (String s in m_Tags)
			{
				if (String.Compare(s, tag, true) == 0)
					return true;
			}
			return false;
		}

		public class OptionGroup
		{
			public int Version;
			public Grid MainElement;
			public OptionGroup(Grid g)
			{
				MainElement = g;
			}
		}

		public String[] Tags
		{
			get { return m_Tags; }
			set
			{
				m_Tags = value;
				RedoTags();
				//FadeOut.Stop();
				//riderOptionsAll.Opacity = 1;
			}
		}
		public void ChangeTags(String[] tags)
		{
			m_NextTags = tags == null || tags.Length == 0 ? ms_AllTag : tags;
			//FadeOut.Begin();
		}
		public void ChangeTags(List<String> tags)
		{
			String[] arr = new String[tags.Count];
			int n = 0;
			foreach (String i in tags)
				arr[n++] = i;
			ChangeTags(arr);
		}

		protected virtual void page_Loaded(object sender, RoutedEventArgs e)
		{
			m_bLoaded = true;
		}

		protected virtual void page_Unloaded(object sender, RoutedEventArgs e)
		{
			m_bLoaded = false;
			Commit();
		}

		public virtual void Commit()
		{
			/*
			if (CourseViewer.IsVisible && CourseViewer.CurrentCourse != null)
				RM1_Settings.General.SelectedCourse[Tags[0]] = new CourseInfo(CourseViewer.CurrentCourse);

			UnitsList.SaveToSettings();
			UnitsList.SetActiveUnits();
			*/
			Riders.SaveToFile();
			RM1_Settings.SaveToFile();
		}

		protected void AddTag(String tag, OptionGroup item)
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

		protected void FadeOut_Completed(object sender, EventArgs e)
		{
			if (m_NextTags != null)
			{
				m_Tags = m_NextTags;
				m_NextTags = null;
				RedoTags();
			}
			//FadeIn.Begin();
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
									if (String.Compare(tt, nt, true) == 0)
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
						f.Visibility = show >= 0 ? Visibility.Visible : (collapse ? Visibility.Collapsed : Visibility.Hidden);
					}
				}
				DeepTags(f); // Keep going down tree.
			}
		}
	}
}
