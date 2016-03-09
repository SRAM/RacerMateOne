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
using System.Windows.Media.Animation;

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for ModeDisplay.xaml
	/// </summary>
	public partial class ModeDisplayPartners : UserControl
	{
		//====================
		public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(String), typeof(ModeDisplayPartners),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTitleChanged)));
		public String Title
		{
			get { return (String)this.GetValue(TitleProperty); }
			set { this.SetValue(TitleProperty, value);	}
		}
		//====================
		public static DependencyProperty SubProperty = DependencyProperty.Register("Sub", typeof(String), typeof(ModeDisplayPartners));
		public String Sub
		{
			get { return (String)this.GetValue(SubProperty); }
			set { this.SetValue(SubProperty, value); }
		}
		//====================
		public static DependencyProperty NotAvailableProperty = DependencyProperty.Register("NotAvailable",typeof(bool),typeof(ModeDisplayPartners),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnNotAvailableChanged)));
		public bool NotAvailable
		{
			get { return (bool)this.GetValue(NotAvailableProperty); }
			set { this.SetValue(NotAvailableProperty,value); }
		}
		//====================
		public static DependencyProperty RegisteredTrademarkProperty = DependencyProperty.Register("RegisteredTrademark", typeof(bool), typeof(ModeDisplayPartners),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTitleChanged)));
		public bool RegisteredTrademark
		{
			get { return (bool)this.GetValue(RegisteredTrademarkProperty); }
			set { this.SetValue(RegisteredTrademarkProperty, value); }
		}
		//====================
		public static DependencyProperty ThumbnailProperty = DependencyProperty.Register("Thumbnail", typeof(ImageSource), typeof(ModeDisplayPartners));
		public ImageSource Thumbnail
		{
			get { return (ImageSource)this.GetValue(ThumbnailProperty); }
			set { this.SetValue(ThumbnailProperty, value); }
		}
		//====================
		private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((ModeDisplayPartners)d).TitleChanged();
		}

		private void TitleChanged()
		{
			cName.Text = Title;
			if (RegisteredTrademark == true)
			{
				TextBlock t = new TextBlock(new Run("®"));
				t.FontSize = 12;
				InlineUIContainer uc = new InlineUIContainer(t);
				uc.BaselineAlignment = BaselineAlignment.Top;
				cName.Inlines.Add(uc);
			}
		}

		Storyboard Anim_Up;
		Storyboard Anim_Down;
		Storyboard Anim_Highlight_On;
		Storyboard Anim_Highlight_Off;

	
		public ModeDisplayPartners()
		{
			InitializeComponent();
			Anim_Up = (Storyboard)FindResource("Anim_Up");
			Anim_Down = (Storyboard)FindResource("Anim_Down");
			Anim_Highlight_On = (Storyboard)FindResource("Anim_Highlight_On");
			Anim_Highlight_Off = (Storyboard)FindResource("Anim_Highlight_Off");
		}

		private static void OnNotAvailableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ModeDisplayPartners md = (ModeDisplayPartners)d;
			bool na = (bool)md.GetValue(NotAvailableProperty);
			md.DisabledBox.Visibility = (na ? Visibility.Visible : Visibility.Hidden);
			md.Opacity = (na ? 0.4:1.0);
		}

		//=============================================================================
		public static readonly RoutedEvent ClickEvent =
			EventManager.RegisterRoutedEvent(
			"Click", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(ModeDisplayPartners));


		public event RoutedEventHandler Click
		{
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}

		bool m_bClickCheck = false;
		bool m_bIn = false;
		private void btn_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!NotAvailable)
			{
				m_bClickCheck = true;
				Anim_Down.Begin();
				Anim_Up.Stop();
			}
		}

		private void btn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!NotAvailable)
			{
				Anim_Down.Stop();
				Anim_Up.Begin();
			}
			if (m_bIn && m_bClickCheck)
			{
				RoutedEventArgs args = new RoutedEventArgs(ClickEvent);
				RaiseEvent(args);
			}
			m_bClickCheck = false;
		}


		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			m_bIn = false;
			bool na = NotAvailable;
			if (m_bClickCheck && !NotAvailable)
			{
				Anim_Down.Stop();
				Anim_Up.Begin();
			}
			if (!na)
			{
				Anim_Highlight_Off.Begin();
				Anim_Highlight_On.Stop();
			}
		}

		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!NotAvailable)
			{
				m_bIn = true;
				Anim_Highlight_Off.Stop();
				Anim_Highlight_On.Begin();
			}
			if (m_bClickCheck && !NotAvailable)
			{
				Anim_Up.Stop();
				Anim_Down.Begin();
			}
		}

		private void btn_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void btn_ToolTipOpening(object sender, ToolTipEventArgs e)
		{
			if (!NotAvailable)
				e.Handled = true;
			//ToolTip.IsOpen = false;

		}

		//=============================================================================
	}
}
