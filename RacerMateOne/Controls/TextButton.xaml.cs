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
	/// Interaction logic for TextLink.xaml
	/// </summary>
	public partial class TextButton : UserControl
	{
		public TextButton()
		{
			InitializeComponent();
		}
		public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(Object), typeof(TextButton));
		public Object Text
		{
			get { return (Object)this.GetValue(TextProperty); }
			set { this.SetValue(TextProperty, value); }
		}

		//=============================================================================
		public static readonly RoutedEvent ClickEvent =
			EventManager.RegisterRoutedEvent(
			"Click", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(TextButton));

		public event RoutedEventHandler Click
		{
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}

		bool m_bClickCheck = false;
		bool m_bIn = false;
		private void btn_MouseDown(object sender, MouseButtonEventArgs e)
		{
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
			if (m_bClickCheck)
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
