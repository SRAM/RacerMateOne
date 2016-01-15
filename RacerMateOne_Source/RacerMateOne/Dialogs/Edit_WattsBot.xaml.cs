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
	public partial class Edit_WattsBot : Window
	{
		public Edit_WattsBot()
		{
			InitializeComponent();
		}

		WattsBot m_Bot;
		public WattsBot Bot
		{
			get { return m_Bot; }
			set
			{
				m_Bot = value;
				if (m_Bot != null)
				{
					Watts = m_Bot.BotWatts;
				}
			}
		}
		int m_Watts;
		int Watts
		{
			get { return m_Watts; }
			set
			{
				int v = value < 1 ? 1:value > 1000 ? 1000:value;
				if (m_Watts != v)
				{
					m_Watts = v;
					if (m_Bot != null)
						m_Bot.BotWatts = v;
					if (WattsStepSlider != null)
						WattsStepSlider.Value = v;
					if (WattsStep != null)
						WattsStep.Text = v.ToString();
				}
			}
		}


		bool m_bClosing;

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

		private void WattsStepSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Watts = Convert.ToInt32(WattsStepSlider.Value);
		}

		private void WattsStep_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				int watts = Convert.ToInt32(WattsStep.Text);
				Watts = watts;
			}
			catch { }
		}
	}
}
