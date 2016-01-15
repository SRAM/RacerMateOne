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
	public partial class Edit_Bot : Window
	{


		public Edit_Bot()
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

		int m_Min = 0;
		int m_Max = 15;

		public void Set(int min, int max, String label, float val, bool bfloat)
		{
			RoundValue = bfloat ? 1 : 0;
			m_Min = min;
			m_Max = max;
			Slider.Minimum = min;
			Slider.Maximum = max;
			Slider.Value = val;
			SliderText.Text = String.Format(bfloat ? "{0:0.#}":"{0}",val);
			TextBoxMaskBehavior.SetMask(SliderText, bfloat ? MaskType.Decimal:MaskType.Integer);
			//TextBoxMaskBehavior.SetMinimumValue(SliderText, min);
			TextBoxMaskBehavior.SetMaximumValue(SliderText, max);
			SliderLabel.Content = label;
		}

		public void Set(int min, int max, String label, float val)
		{
			Set(min, max, label, val, true);
		}

		private void Edit_Loaded(object sender, RoutedEventArgs e)
		{

		}

		bool m_InChanged;
		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try
			{
				if (!m_InChanged)
				{
					double v = Math.Round(Convert.ToDouble(Slider.Value), 1);
					v = v < m_Min ? m_Min : v > m_Max ? m_Max : v;
					SliderText.Text = String.Format(RoundValue == 1 ? "{0:0.#}":"{0:0}", v);
				}
			}
			catch { }

		}

		private void Slider_TextChanged(object sender, TextChangedEventArgs e)
		{
			m_InChanged = true;
			try
			{
				double v = Math.Round(Convert.ToDouble(SliderText.Text), 1);
				v = v < m_Min ? m_Min : v > m_Max ? m_Max : v;
				Slider.Value = v;
			}
			catch { }
			m_InChanged = false;
		}
		public float Value
		{
			get { return (float)Slider.Value; }
			set
			{
				Slider.Value = (float)Math.Round(value,RoundValue);
				SliderText.Text = value.ToString();
			}
		}
		public int RoundValue = 1;

	}
}
