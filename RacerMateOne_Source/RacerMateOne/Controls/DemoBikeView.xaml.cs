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


namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for DemoBikeView.xaml
	/// </summary>
	public partial class DemoBikeView : UserControl
	{
		static DemoBikeView ms_Active;
		AppWin m_App;
		bool m_bInit;
		static Duration ms_FadeOutDuration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
		static Duration ms_FadeInDuration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
		DoubleAnimation m_FadeTo = new DoubleAnimation(1.0, ms_FadeOutDuration);

		public DemoBikeView()
		{
			InitializeComponent();
			m_App = AppWin.Instance;
			m_FadeTo.Completed += FadeToCompleted;
		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (AppWin.IsInDesignMode || ms_Active != null)
				return;
			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				m_bInit = true;
				Resize3D();
			});
		}


		private void FadeToCompleted(object sender, EventArgs e)
		{
			if (m_FadeTo.To == 0.0)
				ViewOff();
		}


		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			ViewOff();
		}

		UnitSave m_VisSave;
		bool m_Init3D;
		void ViewOn()
		{
			if (ms_Active != null || !m_bInit)
				return;
            // Init3D
            m_App.MainRender3D.Init();

            ms_Active = this;
			m_App.SizeChanged += new SizeChangedEventHandler(UserControl_SizeChanged);
			m_App.Render3DOn();
			if (!m_Init3D)
			{
				m_Init3D = true;
                /*
				Course c = new Course();
				if (c.Load(@"C:\Users\Will\Documents\RacerMate\Courses\3DC\__BasicTest.3dc"))
                    Controls.Render3D.Course = c;
                 */
			}
			m_App.RenderFront = true;
			m_VisSave = new UnitSave();
			Unit.Units[0].IsActive = true;
			m_Bot = Unit.Units[0].Bot = new WattsBot(200);
			m_Bot.OverrideModel = m_RiderModel;
			m_App.R_1_0.Unit = Unit.Units[0];
			Controls.Render3D.SetupRiders();
			Controls.Render3D.ShowDemoRider = true;
			//Unit.Start();
			m_App.U_1.Visibility = Visibility.Visible;
			m_App.FrontFrameOverlay.Visibility = Visibility.Visible;
			m_App.FrontFrameOverlay.Opacity = 0.0;
			m_App.MainRender3D.Opacity = 0.0;

			m_FadeTo.BeginTime = new TimeSpan(0, 0, 0, 1, 0);
			m_FadeTo.To = 1.0;
			m_FadeTo.Duration = ms_FadeInDuration;
			m_App.MainRender3D.BeginAnimation(Render3D.OpacityProperty, m_FadeTo);
			m_App.FrontFrameOverlay.BeginAnimation(Grid.OpacityProperty, m_FadeTo);
		}
		Bot m_Bot;

		void ViewOff()
		{
			if (ms_Active != this || !m_bInit)
				return;
			m_App.MainRender3D.BeginAnimation(Render3D.OpacityProperty, null);
			m_App.FrontFrameOverlay.BeginAnimation(Grid.OpacityProperty, null);

			ms_Active = null;
			Unit.Units[0].IsActive = true;
			Controls.Render3D.ShowDemoRider = false;
			Unit.Start();
			m_App.FrontFrameOverlay.Visibility = Visibility.Collapsed;
			m_App.U_1.Visibility = Visibility.Hidden;

			m_App.RenderFront = false;
			m_App.Render3DOff();
			m_App.SizeChanged -= new SizeChangedEventHandler(UserControl_SizeChanged);
			m_VisSave.Restore();
			m_VisSave = null;

			m_App.MainRender3D.Margin = new Thickness(0,0,0,0);
			m_App.MainRender3D.VerticalAlignment = VerticalAlignment.Stretch;
			m_App.MainRender3D.HorizontalAlignment = HorizontalAlignment.Stretch;
			m_App.MainRender3D.Width = Double.NaN;
			m_App.MainRender3D.Height = Double.NaN;
			m_App.MainRender3D.Opacity = 1.0;
			m_App.FrontFrameOverlay.Opacity = 1.0;
			m_App.RenderFront = false;

			Unit.Stop();
			Unit.Reset();

			m_Bot = null;
		}
		public void ViewOffFade()
		{
			m_FadeTo.To = 0.0;
			m_FadeTo.Duration = ms_FadeOutDuration;
			m_FadeTo.BeginTime = new TimeSpan(0, 0, 0, 0, 0);
			m_App.MainRender3D.BeginAnimation(Render3D.OpacityProperty, m_FadeTo);
			m_App.FrontFrameOverlay.BeginAnimation(Grid.OpacityProperty, m_FadeTo);
		}
		IRiderModel m_RiderModel;
		public IRiderModel RiderModel
		{
			get { return m_RiderModel; }
			set
			{
				m_RiderModel = value;
				if (m_Bot != null)
				{
					m_Bot.OverrideModel = m_RiderModel;
					Controls.Render3D.SetupRiders();
				}
			}
		}
			

		void Resize3D()
		{
			if (!m_bInit)
				return;
			bool v = IsVisible;
			if (!v)
			{
				ViewOff();
				return;
			}
			ViewOn();

			Point loc = this.TransformToAncestor(m_App).Transform(new Point(0, 0));
			Point w = this.TransformToAncestor(m_App).Transform(new Point(ActualWidth, ActualHeight));
			Thickness margin = new Thickness(Math.Round(loc.X), Math.Round(loc.Y), 0, 0);
			double ww = Math.Round(w.X - loc.X);
			double hh = Math.Round(w.Y - loc.Y);
			m_App.MainRender3D.Margin = margin;

			m_App.MainRender3D.VerticalAlignment = VerticalAlignment.Top;
			m_App.MainRender3D.HorizontalAlignment = HorizontalAlignment.Left;
			m_App.MainRender3D.Width = ww;
			m_App.MainRender3D.Height = hh;
			
			m_App.FrontFrameOverlay.Margin = margin;
			m_App.FrontFrameOverlay.Width = ww;
			m_App.FrontFrameOverlay.Height = hh;
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				Resize3D();
			});
		}

		private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				Resize3D();
			});
		}

	}
}
