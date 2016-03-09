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
	public class BaseClick: UserControl
	{
		public BaseClick()
		{
			MouseDown += new MouseButtonEventHandler(btn_MouseDown);
			MouseLeave += new MouseEventHandler(btn_MouseLeave);
			MouseEnter += new MouseEventHandler(btn_MouseEnter);
			MouseUp += new MouseButtonEventHandler(btn_MouseUp);
		}
		//=============================================================================
		public static readonly RoutedEvent ClickEvent =
			EventManager.RegisterRoutedEvent(
			"Click", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(BaseClick));
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
			AnimDown();
		}

		protected virtual void AnimUp() { }
		protected virtual void AnimDown() { }

		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			m_bIn = false;
			if (m_bClickCheck)
			{
				AnimUp();
			}
		}

		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			m_bIn = true;
			if (m_bClickCheck)
			{
				AnimDown();
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
