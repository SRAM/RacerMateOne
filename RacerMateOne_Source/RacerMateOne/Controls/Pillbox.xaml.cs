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
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for Pillbox.xaml
	/// </summary>
	public partial class Pillbox : UserControl
	{
		public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(Pillbox));
		public String Text
		{
			get { return (String)this.GetValue(TextProperty); }
			set { this.SetValue(TextProperty, value); }
		}


		public Pillbox()
		{
			InitializeComponent();
		}

		public static readonly RoutedEvent ClickEvent =
			EventManager.RegisterRoutedEvent(
			"Click", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(Pillbox));

		public event RoutedEventHandler Click
		{
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}

		bool m_bClickCheck = false;
		bool m_bIn = false;
		private void pillbox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			m_bClickCheck = true;
		}

		private void pillbox_MouseLeave(object sender, MouseEventArgs e)
		{
			m_bIn = false;
			if (m_bClickCheck)
			{
				Anim_Down.Stop();
				Anim_Up.Begin();
			}
		}

		private void pillbox_MouseEnter(object sender, MouseEventArgs e)
		{
			m_bIn = true;
			if (m_bClickCheck)
			{
				Anim_Up.Stop();
				Anim_Down.Begin();
			}
		}

		private void pillbox_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (m_bIn && m_bClickCheck)
			{
				RoutedEventArgs args = new RoutedEventArgs(ClickEvent);
				RaiseEvent(args);
			}
			m_bClickCheck = false;
		}
	}
}
