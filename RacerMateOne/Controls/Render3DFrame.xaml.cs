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
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for Render3DFrame.xaml
	/// </summary>
	public partial class Render3DFrame : UserControl
	{
		AppWin m_App;
		bool m_bInit;
		Unit m_Unit;
		public Unit Unit
		{
			get { return m_Unit; }
			set
			{
				if (m_Unit != value)
				{
					m_Unit = value;
					if (m_View != null)
						m_View.Unit = value;

					AdjustRect();
				}
			}

		}

		Render3DView m_View;
		public Render3DView View { get { return m_View; } }

		public Render3DFrame()
		{
			InitializeComponent();
			if (AppWin.IsInDesignMode)
			{
				Background = Brushes.Green;
				Opacity = 0.2;
				return;
			}
			m_App = AppWin.Instance;
			m_App.SizeChanged += new SizeChangedEventHandler(m_App_SizeChanged);

			//DependencyPropertyDescriptor desc = DependencyPropertyDescriptor.FromProperty(FrameworkElement.OpacityProperty, 
			//										typeof(FrameworkElement));
			//desc.AddValueChanged(m_App.MainFrame,new EventHandler(OpacityChanged) );

		}
		void OpacityChanged(object sender, EventArgs args)
		{
			//if (m_View != null)
			//	m_View.Opacity = m_App.MainFrame.Opacity;
		}

		void m_App_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			AdjustRect();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (AppWin.IsInDesignMode)
				return;
			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				m_bInit = true;
				AdjustRect();
			});
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			if (m_View != null)
			{
				m_View.Unit = null;
				m_App.Custom3D.Children.Remove(m_View);
				m_View = null;
			}

		}

		private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			AdjustRect();
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			AdjustRect();
		}

		//Rectangle m_Rect;
		public void AdjustRect()
		{
			if (!m_bInit || AppWin.IsInDesignMode || !IsVisible)
				return;
			if (m_View == null)
			{
				m_View = new Render3DView();
				m_View.HorizontalAlignment = HorizontalAlignment.Left;
				m_View.VerticalAlignment = VerticalAlignment.Top;
				m_App.Custom3D.Children.Add(m_View);
				m_View.Unit = m_Unit;
				m_View.Opacity = m_App.MainFrame.Opacity;
			}

            Point offset = this.TranslatePoint(new Point(0, 0), m_App.MainRender3D);
            m_View.Margin = new Thickness(offset.X, offset.Y, 0, 0);
            m_View.Width = ActualWidth;
            m_View.Height = ActualHeight;
            m_View.Visibility = m_View.Unit != null && m_View.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            m_View.FixBackground = Brushes.Black;

/* old - tobe removed
			Point loc2 = m_App.MainRender3D.TransformToAncestor(m_App).Transform(new Point(0, 0));
			Point w2 = m_App.MainRender3D.TransformToAncestor(m_App).Transform(new Point(m_App.MainRender3D.ActualWidth, m_App.MainRender3D.ActualHeight));
			double ax = (w2.X - loc2.X) / m_App.ActualWidth;
			double ay = (w2.Y - loc2.Y) / m_App.ActualHeight;

			Point loc = this.TransformToAncestor(m_App).Transform(new Point(0, 0));
			Point w = this.TransformToAncestor(m_App).Transform(new Point(ActualWidth, ActualHeight));
			Thickness margin = new Thickness(Math.Round(loc.X - loc2.X), Math.Round(loc.Y - loc2.Y), 0, 0);
			double ww = Math.Round((w.X - loc.X) / ax);
			double hh = Math.Round((w.Y - loc.Y) / ay);
			m_View.Margin = margin;
			m_View.Width = ww;
			m_View.Height = hh;
			m_View.Visibility = m_View.Unit != null && m_View.IsVisible ? Visibility.Visible:Visibility.Collapsed;
			m_View.FixBackground = Brushes.Black;
*/
		}
	}
}
