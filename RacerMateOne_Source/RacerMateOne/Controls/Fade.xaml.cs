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

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for OverlayMessage.xaml
	/// </summary>
	public partial class Fade : UserControl
	{
		//=============================================================
		public static DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(Duration), typeof(Fade),
			new FrameworkPropertyMetadata(new Duration(new TimeSpan(0,0,0,0,100)), new PropertyChangedCallback(_OnDuratonChanged)));
		public Duration Duration
		{
			get { return (Duration)this.GetValue(DurationProperty); }
			set
			{
				this.SetValue(DurationProperty, value);
			}
		}
		private static void _OnDuratonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Fade)d).OnDurationChanged();
		}
		private void OnDurationChanged()
		{
			a_FadeAnim.Duration = Duration;
		}

		//=============================================================
		public static DependencyProperty FadeToProperty = DependencyProperty.Register("FadeTo", typeof(double), typeof(Fade),
			new FrameworkPropertyMetadata(0.0,new PropertyChangedCallback(_OnFadeToChanged)));
		public double FadeTo
		{
			get { return (double)this.GetValue(FadeToProperty); }
			set 
			{
				double v = value < 0 ? 0 : value > 1 ? 1 : value;
				this.SetValue(FadeToProperty, value); 
			}
		}
		private static void _OnFadeToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Fade)d).OnFadeToChanged();
		}
		bool m_bInit;
		double m_CurFade = -1.0f;
		private void OnFadeToChanged()
		{
			if (!m_bInit)
				return;
			double fade = FadeTo;
			if (fade == m_CurFade)
				return;
			m_CurFade = fade;
			if (m_CurFade > 0)
				Visibility = Visibility.Visible;
			a_Fade.Stop();
			a_FadeAnim.To = m_CurFade;
			a_Fade.Begin();
		}

		public void Reset()
		{
			m_CurFade = -1.0f;
			Opacity = 0.0;
			Visibility = Visibility.Hidden;
			a_Fade.Stop();
			OnFadeToChanged();
		}

		//=============================================================

		public Fade()
		{
			InitializeComponent();
		}

		private void mainBox_Loaded(object sender, RoutedEventArgs e)
		{
			if (AppWin.IsInDesignMode)
				return;
			m_CurFade = FadeTo;
			Opacity = m_CurFade;
			Visibility = m_CurFade == 0.0 ? Visibility.Hidden : Visibility.Visible;
			m_bInit = true; // Now we can fade to whatever.
		}

		private void a_Fade_Completed(object sender, EventArgs e)
		{
			if (a_FadeAnim.To == 0)
				Visibility = Visibility.Hidden;
		}


	}
}
