﻿using System;
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
	/// Interaction logic for RTab.xaml
	/// </summary>
	public partial class RTab : UserControl
	{
		//============================================================================
		public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(Object), typeof(RTab));
		public Object Text
		{
			get { return (Object)this.GetValue(TextProperty); }
			set { this.SetValue(TextProperty, value); }
		}
		//===============================================================
		public static DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(RTab),
			new FrameworkPropertyMetadata(false,new PropertyChangedCallback(OnSelectedChanged)));
		public bool Selected
		{
			get { return (bool)this.GetValue(SelectedProperty); }
			set { this.SetValue(SelectedProperty, value); }
		}
		private static void OnSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RTab)d).SelectedChanged();
		}
		//============================================================================
		//static Brush std_Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6C9ABA"));
		//static Brush std_Outline = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF266388"));
		//============================================================================
		public static DependencyProperty SelectedBrushProperty = DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(RTab),
			new FrameworkPropertyMetadata(AppWin.StdBrush_BackgroundLight));
		public Brush SelectedBrush
		{
			get { return (Brush)this.GetValue(SelectedBrushProperty); }
			set { this.SetValue(SelectedBrushProperty, value); }
		}
		//============================================================================
		public static DependencyProperty NotSelectedBrushProperty = DependencyProperty.Register("NotSelectedBrush", typeof(Brush), typeof(RTab),
			new FrameworkPropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6C9ABA"))));
		public Brush NotSelectedBrush
		{
			get { return (Brush)this.GetValue(NotSelectedBrushProperty); }
			set { this.SetValue(NotSelectedBrushProperty, value); }
		}
		//============================================================================
		public static DependencyProperty SelectedTextColorProperty = DependencyProperty.Register("SelectedTextColor", typeof(Color), typeof(RTab),
			new FrameworkPropertyMetadata((Color)ColorConverter.ConvertFromString("#FFFFFFFF")));
		public Color SelectedTextColor
		{
			get { return (Color)this.GetValue(SelectedTextColorProperty); }
			set { this.SetValue(SelectedTextColorProperty, value); }
		}
		//============================================================================

		public static DependencyProperty NotSelectedTextColorProperty = DependencyProperty.Register("NotSelectedTextColor", typeof(Color), typeof(RTab),
			new FrameworkPropertyMetadata((Color)ColorConverter.ConvertFromString("#FF266388")));
		public Color NotSelectedTextColor
		{
			get { return (Color)this.GetValue(NotSelectedTextColorProperty); }
			set { this.SetValue(NotSelectedTextColorProperty, value); }
		}
		//============================================================================
		void SelectedChanged()
		{
			if (Selected == true)
			{
				OffEnter.Stop();
				OffLeave.Stop();
				TabBorder.BorderThickness = new Thickness(1,1,1,0);
				TabBorderColor.Color = SelectedTextColor;
				TabBorder.Background = SelectedBrush;
				TextColor.Color = SelectedTextColor;
			}
			else
			{
				OffLeave.Begin();
				TabBorder.Background = NotSelectedBrush;
				TabBorderColor.Color = NotSelectedTextColor;
			}
		}
		//============================================================================
		public RTab()
		{
			InitializeComponent();
		}
		//=============================================================================
		public static readonly RoutedEvent ClickEvent =
			EventManager.RegisterRoutedEvent(
			"Click", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(RTab));

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
		}

		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			m_bIn = true;
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

		private void btn_Loaded(object sender, RoutedEventArgs e)
		{
			SelectedChanged();
		}
		//=============================================================================

	}
}
