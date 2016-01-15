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
	/// Interaction logic for PlainButton.xaml
	/// </summary>
	public partial class PlainButton : UserControl
	{
		public PlainButton()
		{
			InitializeComponent();
		}

		public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(Object), typeof(PlainButton));
		public Object Text
		{
			get { return (Object)this.GetValue(TextProperty); }
			set { this.SetValue(TextProperty, value); }
		}

		//=============================================================================
		public static DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(PlainButton),
			new FrameworkPropertyMetadata(true, new PropertyChangedCallback(_EnabledChanged)));
		public bool Enabled
		{
			get { return (bool)this.GetValue(EnabledProperty); }
			set { this.SetValue(EnabledProperty, value); }
		}
		private static void _EnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { ((PlainButton)d).EnabledChanged(); }
		private void EnabledChanged()
		{
			m_bEnabled = Enabled;
			Opacity = m_bEnabled ? 1 : 0.5;
		}
		bool m_bEnabled = true;
		//=============================================================================
		public static readonly RoutedEvent ClickEvent =
			EventManager.RegisterRoutedEvent(
			"Click", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(PlainButton));

		public event RoutedEventHandler Click
		{
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}

		bool m_bClickCheck = false;
		bool m_bIn = false;
		private void btn_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (m_bEnabled)
				m_bClickCheck = true;
		}

		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			m_bIn = false;
			if (m_bClickCheck)
			{
				Anim_Down.Stop();
				Anim_Up.Begin();
			}
		}

		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			m_bIn = true;
			if (m_bClickCheck && m_bEnabled)
			{
				Anim_Up.Stop();
				Anim_Down.Begin();
			}
		}

		private void btn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (m_bIn && m_bClickCheck)
			{
				RoutedEventArgs args = new RoutedEventArgs(ClickEvent);
				RaiseEvent(args);
			}
			m_bClickCheck = false;
		}
		//=============================================================================

	}
}
