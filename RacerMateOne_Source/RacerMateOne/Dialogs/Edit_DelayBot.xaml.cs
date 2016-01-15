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
	public class DialogEventArgs : EventArgs
	{
		public DialogResult DialogResult { get; set; }
		public SolidColorBrush SelectedColor { get; set; }
	}
	[Flags]
	public enum DialogResult
	{
		Ok = 1,
		Cancel = 2
	}

	/// <summary>
	/// Interaction logic for Edit_DelayBot.xaml
	/// </summary>
	public partial class Edit_DelayBot : Window
	{


		public Edit_DelayBot()
		{
			InitializeComponent();
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

		DelayBot m_Bot;
		public DelayBot Bot
		{
			get { return m_Bot; }
			set
			{
				m_Bot = value;
				if (m_Bot != null)
				{
					Delay = m_Bot.Delay;
				}
			}
		}
		int m_Delay;
		int Delay
		{
			get { return m_Delay; }
			set
			{
				int v = value < 1 ? 1:value > 15 ? 15:value;
				if (m_Delay != v)
				{
					m_Delay = v;
					if (m_Bot != null)
						m_Bot.Delay = v;
					if (SecondsStepSlider != null)
						SecondsStepSlider.Value = v;
					if (SecondsStep != null)
						SecondsStep.Text = v.ToString();
				}
			}
		}


		private void Edit_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void SecondsStepSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Delay = Convert.ToInt32(SecondsStepSlider.Value);
		}

		private void SecondsStep_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				int delay = Convert.ToInt32(SecondsStep.Text);
				Delay = delay;
			}
			catch { }
		}
	}
}
