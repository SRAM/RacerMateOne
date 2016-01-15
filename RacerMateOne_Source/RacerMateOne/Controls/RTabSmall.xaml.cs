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
	/// Interaction logic for RTabSmall.xaml
	/// </summary>
	public partial class RTabSmall : UserControl
	{
		//============================================================================
		public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(Object), typeof(RTabSmall));
		public Object Text
		{
			get { return (Object)this.GetValue(TextProperty); }
			set { this.SetValue(TextProperty, value); }
		}
		//===========================================
		public static DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(RTabSmall),
			new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnSelectedChanged)));
		public bool Selected
		{
			get { return (bool)this.GetValue(SelectedProperty); }
			set { this.SetValue(SelectedProperty, value); }
		}

		//=============================================================================
		public static readonly RoutedEvent ClickEvent =
			EventManager.RegisterRoutedEvent(
			"Click", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(RTabSmall));

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
			/*
			if (Selected == false)
			{
				OffEnter.Stop();
				OffLeave.Begin();
			}
			if (m_bClickCheck)
			{
				//Anim_Down.Stop();
				//Anim_Up.Begin();
			}
			 */
		}

		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			m_bIn = true;
			/*
			if (Selected == false)
			{
				OffEnter.Begin();
				OffLeave.Stop();
			}
			if (m_bClickCheck)
			{
				//Anim_Up.Stop();
				//Anim_Down.Begin();
			}
			 */
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


		//===========================================
		bool m_bLoaded;
		private static void OnSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RTabSmall)d).SelectedChanged();
		}

		public RTabSmall()
		{
			InitializeComponent();
		}

		private void btn_Loaded(object sender, RoutedEventArgs e)
		{
			m_bLoaded = true;
			SelectedChanged();
		}

		private void SelectedChanged()
		{
			if (!m_bLoaded)
				return;
			Brush othercolor = (Brush)FindResource("OtherColor");
			bool selected = Selected;
			if (selected)
			{
				Border.Background = Brushes.White;
				Label.Foreground = othercolor;
			}
			else
			{
				Border.Background = othercolor;
				Label.Foreground = Brushes.White;
			}
		}
	}
}
