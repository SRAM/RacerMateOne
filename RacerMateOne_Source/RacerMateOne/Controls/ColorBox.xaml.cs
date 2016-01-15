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
	/// Interaction logic for ColorBox.xaml
	/// </summary>
	public partial class ColorBox : UserControl
	{

		//======================================
		public static DependencyProperty IsOnProperty = DependencyProperty.Register("IsOn", typeof(bool), typeof(ColorBox),
				new FrameworkPropertyMetadata(false,new PropertyChangedCallback(_IsOnChanged)));
		public bool IsOn
		{
			get { return (bool)this.GetValue(IsOnProperty); }
			set { this.SetValue(IsOnProperty, value); }
		}
		private static void _IsOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((ColorBox)d).IsOnN();
		}
		private void IsOnN()
		{
			if (m_bInit)
			{
				UpdateIsOn();
				RoutedEventArgs args = new RoutedEventArgs(IsOnChangedEvent);
				RaiseEvent(args);
			}
		}
		//======================================
		public static DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(ColorBox),
				new FrameworkPropertyMetadata(Brushes.Red,new PropertyChangedCallback(_IsOnChanged)));
		public Brush Fill
		{
			get { return (Brush)this.GetValue(FillProperty); }
			set { this.SetValue(FillProperty, value); }
		}
		//======================================
		public static readonly RoutedEvent IsOnChangedEvent =
			EventManager.RegisterRoutedEvent(
			"IsOnChanged", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(ColorBox));

		public event RoutedEventHandler IsOnChanged
		{
			add { AddHandler(IsOnChangedEvent, value); }
			remove { RemoveHandler(IsOnChangedEvent, value); }
		}


		public ColorBox()
		{
			InitializeComponent();
		}
		bool m_bInit;
		Brush m_OffBrush;
		private void btn_Loaded(object sender, RoutedEventArgs e)
		{
			m_bInit = true;
			m_OffBrush = Bdr.Background;
			UpdateIsOn();
		}

		private void UpdateIsOn()
		{
			if (!m_bInit)
				return;
			Bdr.Background = IsOn ? Fill : m_OffBrush;
		}

		private void Border_MouseDown(object sender, MouseButtonEventArgs e)
		{
			bool ison = IsOn;
			IsOn = ison ? false : true;
		}

	}
}
