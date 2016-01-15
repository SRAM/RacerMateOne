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
	public partial class Edit_PerformanceBot : Window
	{
		public Edit_PerformanceBot()
		{
			InitializeComponent();
			foreach(UIElement e in p50.Children)
				m_RB[Convert.ToInt32(((RadioButton)e).Tag.ToString())] = (RadioButton)e;
			foreach(UIElement e in p100.Children)
				m_RB[Convert.ToInt32(((RadioButton)e).Tag.ToString())] = (RadioButton)e;
		}
		bool m_bClosing;
		Dictionary<int, RadioButton> m_RB = new Dictionary<int, RadioButton>();


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
			int t = Convert.ToInt32(rb.Tag.ToString());
			if (m_Bot != null)
			{
				m_Bot.Percent = t;
				m_Bot.DoKeyChangeEvent();
			}
		}

		PerformanceBot m_Bot;
		public PerformanceBot Bot
		{
			get { return m_Bot; }
			set
			{
				m_Bot = null;
				bool found = false;
				if (value != null)
				{
					RadioButton rb;
					if (m_RB.TryGetValue(value.Percent, out rb))
					{
						rb.IsChecked = true;
						found = true;
					}
				}
				if (!found)
				{
					RadioButton best = null;
					int match = 500;
					int p = value.Percent;
					foreach (KeyValuePair<int, RadioButton> n in m_RB)
					{
						int t = (int)Math.Abs(n.Key - p);
						if (t < match)
							best = n.Value;
					}
					if (best != null)
						best.IsChecked = true;
				}
				m_Bot = value;
			}
		}

	}
}
