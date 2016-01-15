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

namespace RacerMateOne.Pages.Modes
{
	/// <summary>
	/// Interaction logic for SpinScan.xaml
	/// </summary>
	public partial class zzz_SpinScan : RideBase
	{
		public Statistics m_Statistics = new Statistics();

		Course.Location m_BikeLoc;
		Rider m_Rider;
		Unit m_Unit;

		public TrainerUserConfigurable TrainerUser;

		protected Course m_Course;
		public Course Course
		{
			get { return m_Course; }
			set
			{
				m_Course = value;
				d_Course.Course = m_Course;
				m_BikeLoc = m_Course != null ? new Course.Location(m_Course, 0):null;
			}
		}

		public zzz_SpinScan()
		{
			InitializeComponent();
		}

		ReportColumns m_DisplayColumns;
		StatFlags m_DisplayStatFlags;
		private void RiderOptions_Loaded(object sender, RoutedEventArgs e)
		{
			Binding binding;

			m_Unit = Unit.RiderUnits[0];
			Trainer = m_Unit.Trainer;
			m_Statistics = m_Unit.Statistics;
			Course = Unit.Course;
			m_Rider = m_Unit.Rider;

			m_DisplayColumns = ReportColumns.Display_SpinScan.Selected_ReportColumns;
			m_DisplayStatFlags = m_DisplayColumns.StatFlags;


			RideBase_Loaded(MainGrid);


			DpConverter tconv = new DpConverter(1);
			DpConverter iconv = new DpConverter(0);

			InfoLine.Unit = m_Unit;

			BindTo("TimerString", d_Timer, null);


			BindTo("SSLeft", d_SSLeft, iconv);
			BindTo("SSRight", d_SSRight, iconv);
			BindTo("SS", d_SSAvg, iconv);
			BindTo("SSLeftSplit", d_SSLeftWatts, iconv);
			BindTo("SSRightSplit", d_SSRightWatts, iconv);
			BindTo("SSLeftATA", d_SSLeftATA, iconv);
			BindTo("SSRightATA", d_SSRightATA, iconv);
			BindTo("SSLeft_Avg", d_SSLeft_Avg, iconv);
			BindTo("SSRight_Avg", d_SSRight_Avg, iconv);
			BindTo("SS_Avg", d_SSAvgATA, iconv);


			/*
			binding = new Binding("Distance");
			binding.Source = m_Statistics;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			d_Course.SetBinding(Controls.CourseDisplay.DistanceProperty, binding);
			*/

			binding = new Binding("Bars");
			binding.Source = m_Statistics;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			d_SpinScan.SetBinding(Controls.PolarSpinScan.BarsProperty, binding);
			d_BarSpinScan.SetBinding(Controls.BarSpinScan.BarsProperty, binding);

			binding = new Binding("AverageBars");
			binding.Source = m_Statistics;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			d_SpinScan.SetBinding(Controls.PolarSpinScan.AvgBarsProperty, binding);
			d_BarSpinScan.SetBinding(Controls.BarSpinScan.AvgBarsProperty, binding);

			binding = new Binding("SSLeftATA");
			binding.Source = m_Statistics;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			d_SpinScan.SetBinding(Controls.PolarSpinScan.SSLeftATAProperty, binding);
			d_BarSpinScan.SetBinding(Controls.BarSpinScan.SSLeftATAProperty, binding);
			
			binding = new Binding("SSRightATA");
			binding.Source = m_Statistics;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			d_SpinScan.SetBinding(Controls.PolarSpinScan.SSRightATAProperty, binding);
			d_BarSpinScan.SetBinding(Controls.BarSpinScan.SSRightATAProperty, binding);


			binding = new Binding("MaxForce");
			binding.Source = d_SpinScan;
			binding.Converter = iconv;
			MaxForce.SetBinding(Label.ContentProperty, binding);

			binding = new Binding("MaxForce");
			binding.Source = d_BarSpinScan;
			binding.Converter = iconv;
			BarMaxForce.SetBinding(Label.ContentProperty, binding);


			BindTo("GearString", d_VelotronGears, null);

			if (Trainer.Type == RM1.DeviceType.VELOTRON)
			{
				g_VelotronGears.Opacity = 1;
			}


			//FixDataLine();
			d_Course.BikeIcon = BikeIcon;
			d_Course.Unit = m_Unit;
			Stats.Unit = m_Unit;
			Stats.StatFlags = m_DisplayStatFlags;

			//VideoControl.Source = new Uri(@"C:\Real Course Video\IMCanada\IMCanada.avi");
			//VideoControl.Play();

			m_bInit = true;
			KeyBox.Margin = new Thickness(0, 60, 0, 0);

		}

		private void RiderOptions_Unloaded(object sender, RoutedEventArgs e)
		{
			RideBase_Unloaded();
		}


		private void BindTo(String name, FrameworkElement f, IValueConverter conv)
		{
			Binding binding = new Binding(name);
			binding.Source = m_Statistics;
			if (conv != null)
				binding.Converter = conv;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			f.SetBinding(Label.ContentProperty, binding);
		}
		private void BindTo(String name, FrameworkElement f, IValueConverter conv, bool targetupdated)
		{
			Binding binding = new Binding(name);
			binding.Source = m_Statistics;
			if (conv != null)
				binding.Converter = conv;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			binding.NotifyOnTargetUpdated = true;
			f.SetBinding(Label.ContentProperty, binding);
		}

		void adjFontSize(Panel elem, double adj)
		{
			foreach (Object child in elem.Children)
			{
				Label lab = child as Label;
				if (lab != null)
					lab.FontSize = adj;
				else
				{
					Panel p = child as Panel;
					if (p != null)
						adjFontSize(p, adj);
				}
			}
		}

		RM1.Trainer m_Trainer;
		public RM1.Trainer Trainer
		{
			get
			{
				return m_Trainer;
			}
			set
			{
				if (m_Trainer == value)
					return;
				m_Trainer = value;
			}
		}

		protected override void Scroll(Unit unit, int dir)
		{
			int state = (int)m_State;
			state += dir;
			if (state < 0)
				state = ((int)States.MAX)-1;
			if (state >= (int)States.MAX)
				state = 0;
			State = (States)state;
		}

		protected override void Reset()
		{
			if (m_Statistics.CurrentState == Statistics.State.Stopped && m_Statistics.Time == 0.0)
				NavigationService.GoBack();
			else
				Unit.Reset();
		}



		enum States
		{
			Polar,
			Bar,

			MAX
		}
		States m_State = States.Polar;
		States State
		{
			get { return m_State; }
			set
			{
				if (m_State == value)
					return;
				m_State = value;
				switch (m_State)
				{
					case States.Polar:
						d_SpinScan.Width = t_PolarSS_Large.Width;
						d_SpinScan.Height = t_PolarSS_Large.Height;
						d_SpinScan.Margin = t_PolarSS_Large.Margin;
						d_SpinScan.VerticalAlignment = t_PolarSS_Large.VerticalAlignment;
						d_SpinScan.HorizontalAlignment = t_PolarSS_Large.HorizontalAlignment;
						d_SpinScan.ShowLabels = true;

						d_BarSpinScan.Width = t_BarSS_Small.Width;
						d_BarSpinScan.Height = t_BarSS_Small.Height;
						d_BarSpinScan.Margin = t_BarSS_Small.Margin;
						d_BarSpinScan.VerticalAlignment = t_BarSS_Small.VerticalAlignment;
						d_BarSpinScan.HorizontalAlignment = t_BarSS_Small.HorizontalAlignment;
						d_BarSpinScan.ShowLabels = false;

						d_MaxForce.Visibility = Visibility.Visible;
						d_BarMaxForce.Visibility = Visibility.Hidden;
						break;
					case States.Bar:
						d_SpinScan.Width = t_PolarSS_Small.Width;
						d_SpinScan.Height = t_PolarSS_Small.Height;
						d_SpinScan.Margin = t_PolarSS_Small.Margin;
						d_SpinScan.VerticalAlignment = t_PolarSS_Small.VerticalAlignment;
						d_SpinScan.HorizontalAlignment = t_PolarSS_Small.HorizontalAlignment;
						d_SpinScan.ShowLabels = false;

						d_BarSpinScan.Width = t_BarSS_Large.Width;
						d_BarSpinScan.Height = t_BarSS_Large.Height;
						d_BarSpinScan.Margin = t_BarSS_Large.Margin;
						d_BarSpinScan.VerticalAlignment = t_BarSS_Large.VerticalAlignment;
						d_BarSpinScan.HorizontalAlignment = t_BarSS_Large.HorizontalAlignment;
						d_BarSpinScan.ShowLabels = true;

						d_MaxForce.Visibility = Visibility.Hidden;
						d_BarMaxForce.Visibility = Visibility.Visible;

						break;
				}
			}
		}
		//=============================================================
		private void Options_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Pages.RideOptions());
		}
		private void Back_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help();
		}
	}
}
