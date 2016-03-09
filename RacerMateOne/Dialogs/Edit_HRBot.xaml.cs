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
using System.Windows.Shapes;

namespace RacerMateOne.Dialogs
{
	/// <summary>
	/// Interaction logic for Edit_DelayBot.xaml
	/// </summary>
	public partial class Edit_HRBot : Window
	{
		public Edit_HRBot()
		{
			InitializeComponent();
			foreach(UIElement e in Zones.Children)
				m_RB[((RadioButton)e).Tag.ToString()] = (RadioButton)e;
			foreach(UIElement e in Percentages.Children)
				m_RB[((RadioButton)e).Tag.ToString()] = (RadioButton)e;
		}
		bool m_bClosing;

		int m_Zone;
		int m_Lower;
		int m_Upper;

		Dictionary<String,RadioButton> m_RB = new Dictionary<string,RadioButton>();

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			m_bClosing = true;
			Fade.Stop();
			FadeAnim.To = 0.0f;
			Fade.Begin();
		}

		private void FadeAnim_Completed(object sender, EventArgs e)
		{
			if (m_bClosing)
				Close();
		}

		private void Edit_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			String t = rb.Tag as String;
			if (t[0] == 'z')
			{
				m_Zone = ((int)t[1]) - ((int)'0');
			}
			else
			{
				String[] ss = t.Split(',');
				m_Zone = 0;
				m_Lower = Convert.ToInt32(ss[0]);
				m_Upper = Convert.ToInt32(ss[1]);
			}
			if (m_Bot != null)
			{
				if (m_Zone == 0)
					m_Bot.SetPercentages(m_Lower, m_Upper);
				else
					m_Bot.SetZone(m_Zone);
			}
		}

		HRBot m_Bot;
		public HRBot Bot
		{
			get { return m_Bot; }
			set
			{
				m_Bot = null;
				m_Zone = value.Zone;
				m_Lower = value.LowerPercent;
				m_Upper = value.UpperPercent;
				if (m_Zone == 0)
				{
					m_Lower = (m_Lower / 5) * 5;
					m_Upper = (m_Upper / 5) * 5;
					if (m_Lower == m_Upper)
						m_Upper = m_Lower + 5;
					if (m_Upper > 100)
					{
						m_Upper = 100;
						m_Lower = 95;
					}
				}
				String t = m_Zone == 0 ? String.Format("{0},{1}", m_Lower, m_Upper) : String.Format("z{0}", m_Zone);
				RadioButton rb;
				if (m_RB.TryGetValue(t, out rb))
				{
					rb.IsChecked = true;
				}
				m_Bot = value;
			}
		}
	


	}
}
