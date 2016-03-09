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

namespace RacerMateOne.Pages.Modes
{
	/// <summary>
	/// Interaction logic for SpinScan.xaml
	/// </summary>
	public partial class SpinScan_Review : ReviewBase
	{
		AppWin m_App;
		public SpinScan_Review()
		{
			InitializeComponent();
			m_App = AppWin.Instance;
		}

		Statistics m_Statistics;
		ReportColumns m_DisplayColumns;
		StatFlags m_DisplayStatFlags;

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			MainGrid.Children.Remove(DlgGrid);
			Dlg.Content = DlgGrid;
			Dlg.FadeTo = 0.0;
			m_DisplayColumns = ReportColumns.Display_SpinScan.Selected_ReportColumns;
			m_DisplayStatFlags = m_DisplayColumns.StatFlags & ~(StatFlags.RiderName | StatFlags.Lead);

			d_SpinScan.AvgBarsOn = true;
			d_SpinScan_Small.AvgBarsOn = true;
			d_BarSpinScan.AvgBarsOn = true;
			d_BarSpinScan_Small.AvgBarsOn = true;

			DpConverter iconv = new DpConverter(0);

			Binding binding;
			binding = new Binding("MaxForce");
			binding.Source = d_SpinScan;
			binding.Converter = iconv;
			d_MaxForce.SetBinding(Label.ContentProperty, binding);

			binding = new Binding("MaxForce");
			binding.Source = d_BarSpinScan;
			binding.Converter = iconv;
			d_BarMaxForce.SetBinding(Label.ContentProperty, binding);

			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				//SelectMasterPad();
				m_Unit = Unit.RiderUnits[0];
				m_Statistics = m_Unit.Statistics;
				RideBase_Loaded(TopGrid);

				d_Stats.ShowHeartRateAlarm = false;

				m_bTimeBased = m_Course.XUnits == CourseXUnits.Time;
				m_bLoadBased = m_Course.YUnits != CourseYUnits.Grade;
				if (m_bLoadBased)
					throw new Exception("Track cannot be load based!");

				d_Course.Course = m_Course;
				//d_Course.BikeIcon = BikeIcon;
				d_Course.Distance = 0;
				d_Course.RedrawCourse();
				d_Course.Interactive = true;
				m_MultiLap = Unit.Laps > 1;
				if (m_MultiLap)
				{
				}
				else
				{
					l_Current.Opacity = d_Current.Opacity = l_Best.Opacity = d_Best.Opacity = l_Last.Opacity = d_Last.Opacity = 0.3;
				}
				m_LapRun = m_Course.LengthFillTextBlock(d_TrackInfo,Unit.Laps);

				m_StatFlags &= ~(StatFlags.Watts_Load);


				if (m_Unit.IsVelotron)
				{
					m_DisplayStatFlags |= StatFlags.Gearing | StatFlags.GearInches;
					//d_RRC.Opacity = 0.3;
					//d_NoRRC.Visibility = Visibility.Collapsed;
					//d_RRCText.Text = "";
				}
				else
				{
					//d_GearInches.Opacity = d_Gearing.Opacity = 0.3;
				}
				d_Stats.StatFlags = m_DisplayStatFlags;
				d_Stats.Unit = m_Unit;		

				UpdateCalibrationString();

				UpdateSpinScanMode();


				// One time things.
				//d_Weight.Content = m_Unit.Rider.WeightTotalDisplay;
				//d_DragFactor.Content = m_Unit.Rider.DragFactor;
				if (Course.Laps > 1)
				{
					d_LapSliderBox.Visibility = Visibility.Visible;
					d_LapSlider.Maximum = Course.Laps;
				}

				AppWin.AddRenderUpdate(new AppWin.RenderUpdate(RenderUpdate), 0);

				Unit.AddNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitUpdate));
				OnUnitUpdate(m_Unit, StatFlags.Mask);
			});
		}

		double m_RedrawManual = 0;
		bool m_retime = false;
		bool RenderUpdate(double seconds, double split)
		{
			if (Course.Manual)
			{
				d_Course.RedrawCourse();
				// Right now unused.
				if (m_retime)
				{
					m_RedrawManual = seconds + 1;
					m_retime = false;
				}
				if (seconds >= m_RedrawManual)
				{
					m_retime = true;
					//d_Course.RedrawCourse();
				}
			}
			return false;
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			AppWin.RemoveRenderUpdate(new AppWin.RenderUpdate(RenderUpdate));
			Unit.Manual = false;
			Unit.RemoveNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitUpdate));
			RideBase_Unloaded();
		}

		Run m_LapRun;

		bool m_MultiLap;
		StatFlags m_StatFlags =
			StatFlags.RiderName | StatFlags.Time | StatFlags.Lap | StatFlags.Distance | StatFlags.SS_Stats | StatFlags.Finished;

		bool m_bTimeBased;
		bool m_bLoadBased;


		void OnUnitUpdate(Unit unit, StatFlags changed)
		{
			Statistics s = unit.Statistics;
			if ((changed & StatFlags.RiderName) != StatFlags.Zero)
				d_RiderName.Content = Course != null && Course.PerformanceInfo != null ? Course.PerformanceInfo.RiderName : "";
			if ((changed & StatFlags.Lap) != StatFlags.Zero)
			{
				changed |= StatFlags.LapTime | StatFlags.Time;
				LapInfo li = LapArr[s.Lap - 1];

				d_Last.Content = Statistics.SecondsToTimeString(li.Time);
				d_Best.Content = Statistics.SecondsToTimeString(li.Best);

				m_bLapSet = true;
				d_LapSlider.Value = s.Lap;
				m_bLapSet = false;
				if (s.Lap > 1)
				{
					l_Best.Content = String.Format("Best (lap {0})", li.BestLap);
					l_Last.Opacity = l_Best.Opacity = d_Best.Opacity = d_Last.Opacity = 1;
				}
				else
				{
					l_Best.Content = "Best";
					l_Last.Opacity = l_Best.Opacity = d_Best.Opacity = d_Last.Opacity = 0.3;
				}
				if (m_LapRun != null)
					m_LapRun.Text = String.Format("{0}/{1}", s.Lap > Unit.Laps ? Unit.Laps : s.Lap, Unit.Laps);

				l_LapSliderLabel.Content = String.Format("Lap {0}", s.Lap);

				double tlen = (s.Lap - 1) * Course.TotalX;
				double endx = tlen + Course.EndAt;
				double lenx = Course.EndAt - Course.StartAt;

				double r = (m_EndX - (endx - lenx)) / lenx;
				if (r >= 1.0)
				{
					d_GhostBox.Visibility = Visibility.Collapsed;
				}
				else
				{
					d_GhostBox.Visibility = Visibility.Visible;
					d_GhostBox.Margin = new Thickness(d_Course.ActualWidth * r + d_Course.Margin.Left, 0, 0, 0);
				}

			}
			if ((changed & StatFlags.Time) != StatFlags.Zero)
			{
				if (m_MultiLap)
					d_Current.Content = s.LapTimeString;
				d_Time.Content = s.TimerString;
				if (m_bTimeBased)
					d_Remaining.Text = Statistics.SecondsToTimeString(m_Course.TotalX * Unit.Laps - s.Time);
			}
			if ((changed & StatFlags.Distance) != StatFlags.Zero)
			{
				d_Distance.Text = s.Distance_String;
				d_Course.Distance = s.LapDistance;
				if (!m_bTimeBased)
					d_Remaining.Text = ConvertConst.MetersToDistanceString(m_Course.TotalX * Unit.Laps - s.Distance);
			}

			if ((changed & StatFlags.SSLeft) != StatFlags.Zero)
				d_SSLeft.Content = String.Format("{0:F0}", s.SSLeft);
			if ((changed & StatFlags.SSRight) != StatFlags.Zero)
				d_SSRight.Content = String.Format("{0:F0}", s.SSRight);
			if ((changed & StatFlags.SS) != StatFlags.Zero)
				d_SS.Content = String.Format("{0:F0}", s.SS);

			if ((changed & StatFlags.SSLeft_Avg) != StatFlags.Zero)
				d_SSLeft_Avg.Content = String.Format("{0:F0}", s.SSLeft_Avg);
			if ((changed & StatFlags.SSRight_Avg) != StatFlags.Zero)
				d_SSRight_Avg.Content = String.Format("{0:F0}", s.SSRight_Avg);
			if ((changed & StatFlags.SS_Avg) != StatFlags.Zero)
				d_SS_Avg.Content = String.Format("{0:F0}", s.SS_Avg);

			if ((changed & StatFlags.SSLeftATA) != StatFlags.Zero)
				d_SSLeftATA.Content = String.Format("{0:F0}", s.SSLeftATA);
			if ((changed & StatFlags.SSRightATA) != StatFlags.Zero)
				d_SSRightATA.Content = String.Format("{0:F0}", s.SSRightATA);

			if ((changed & StatFlags.SSLeftSplit) != StatFlags.Zero)
				d_SSLeftWatts.Content = String.Format("{0:F0}", s.SSLeftSplit);
			if ((changed & StatFlags.SSRightSplit) != StatFlags.Zero)
				d_SSRightWatts.Content = String.Format("{0:F0}", s.SSRightSplit);
			if ((changed & StatFlags.Calibration) != StatFlags.Zero)
				UpdateCalibrationString();

			if ((changed & StatFlags.Finished) != StatFlags.Zero)
			{
				/*
				if (AllFinished())
				{
					m_bReset = true;
					SavePerformance();
				}
				*/
			}

		}

		void ManualAdjust( float dir )
		{
			if (!m_Course.Manual)
				return;
			float y = m_Course.ManualY;
			if (m_Course.YUnits == CourseYUnits.Grade)
			{
				dir *= (float)(RM1_Settings.General.GradeStep);
				y = y * 100.0f;
				y += dir;
				y = m_Unit.IsVelotron ? (y < -24 ? -24:y > 24 ? 24:y):(y < -15 ? -15:y > 15 ? 15:y);
				y = y * 0.01f;
			}
			else
			{
				dir *= (float)(RM1_Settings.General.WattsStep);
				y += dir;
				y = y < 0 ? 0: y > 2000 ? 2000:y;
			}
			m_Course.ManualY = y;
		}

		bool m_Polar = true;
		void UpdateSpinScanMode()
		{
			Visibility polarv = m_Polar ? Visibility.Visible : Visibility.Collapsed;
			Visibility barv = m_Polar ? Visibility.Collapsed : Visibility.Visible;

			Unit polarunit = m_Polar ? m_Unit : null;
			Unit barunit = m_Polar ? null :m_Unit;

			d_SpinScan.Visibility = polarv;
			d_SpinScan_Small.Visibility = barv;
			d_SpinScan.Unit = polarunit;
			d_SpinScan_Small.Unit = barunit;
			d_MaxForceBox.Visibility = polarv;

			d_BarSpinScan.Visibility = barv;
			d_BarSpinScan_Small.Visibility = polarv;
			d_BarSpinScan.Unit = barunit;
			d_BarSpinScan_Small.Unit = polarunit;
			d_BarMaxForceBox.Visibility = barv;
		}


		//=============================================================
		delegate void DialogCB(RM1.PadKeys key);
		DialogCB m_CurDialogCB;
		void StartDialog(DialogCB cb, object toptext, object f1text, object f2text, bool progress)
		{
			if (m_CurDialogCB != null)
				m_CurDialogCB(RM1.PadKeys.MAX);

			Dlg_TopLabel.Content = toptext;
			Dlg_f1_Label.Content = f1text != null ? f1text : "";
			Dlg_f1.Visibility = f1text != null ? Visibility.Visible : Visibility.Collapsed;
			Dlg_f2_Label.Content = f2text != null ? f2text : "";
			Dlg_f2.Visibility = f2text != null ? Visibility.Visible : Visibility.Collapsed;
			Dlg_ProgressBar.Visibility = progress ? Visibility.Visible : Visibility.Collapsed;

			Dlg.Reset();
			Dlg.FadeTo = 1.0;

			m_CurDialogCB = cb;
		}


		void CloseDialog()
		{
			Dlg.FadeTo = 0.0;
			//KeyBox.Focus();
			m_CurDialogCB = null;
		}

		private void Dlg_f1_Click(object sender, RoutedEventArgs e)
		{
			if (m_CurDialogCB != null)
				m_CurDialogCB(RM1.PadKeys.F1);
		}

		private void Dlg_f2_Click(object sender, RoutedEventArgs e)
		{
			if (m_CurDialogCB != null)
				m_CurDialogCB(RM1.PadKeys.F2);
		}
		Label Dlg_f1_Label;
		Label Dlg_f2_Label;
		private void Dlg_f1_Loaded(object sender, RoutedEventArgs e) { Dlg_f1_Label = (Label)sender; }
		private void Dlg_f2_Loaded(object sender, RoutedEventArgs e) { Dlg_f2_Label = (Label)sender; }






		void UpdateCalibrationString()
		{
			/*
			if (m_Unit.Trainer == null || m_Unit.IsVelotron)
			{
				d_NoRRC.Visibility = Visibility.Collapsed;
				d_RRCText.Text = "";
				d_RRC.Opacity = 0.3;
			}
			else
			{
				d_NoRRC.Visibility = m_Unit.Trainer.IsCalibrated ? Visibility.Collapsed : Visibility.Visible;
				d_RRCText.Text = m_Unit.Trainer.CalibrationString;
			}
			*/
		}



		private void CountDown_Go(object sender, RoutedEventArgs e)
		{
		}

		protected override bool DoUpdate(double seconds, double split)
		{
			base.DoUpdate(seconds, split); // Adjust the frame to the proper location.


			return false;
		}

		bool m_bLapSet;
		private void d_LapSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (m_bLapSet)
				return;
			int lap = (int)d_LapSlider.Value;
			if (m_Unit != null && (m_Unit.Statistics.Lap != lap))
			{
				double dist = m_Unit.Statistics.LapDistance + Course.TotalX * (lap - 1);
				XLoc = dist;
			}
		}

		private void BikeIcon_MouseDown(object sender, MouseButtonEventArgs e)
		{
			d_Course.DMouseDown(e);
		}

		private void BikeIcon_MouseUp(object sender, MouseButtonEventArgs e)
		{
			d_Course.DMouseUp(e);
		}

		//=============================================================
		private void d_Course_DistanceChanged(object sender, RoutedEventArgs e)
		{
			if (m_Unit == null)
				return;
			double dist = (d_Course.Distance + Course.StartAt) + Course.TotalX * (m_Unit.Statistics.Lap - 1);
			XLocClip = dist;
		}
		//=============================================================
		protected override void OnDirectKey(Unit unit, RM1.PadKeys key)
		{
			if (m_CurDialogCB != null)
			{
				if (unit == null || !unit.MasterPad)
					return;
				if (key == RM1.PadKeys.F1 || key == RM1.PadKeys.F2)
					m_CurDialogCB(key);
				return;
			}
			switch (key)
			{
				case RM1.PadKeys.F1:
					m_bRunning = !m_bRunning;
					break;
				case RM1.PadKeys.F2:
					m_Polar = !m_Polar;
					UpdateSpinScanMode();
					break;
				case RM1.PadKeys.F3:
					if (Unit.State == Statistics.State.Paused && m_Unit.Trainer != null)
					{
						m_Unit.Trainer.CalibrateMode = !unit.Trainer.CalibrateMode;
						UpdateCalibrationString();
						break;
					}
					break;
				case RM1.PadKeys.F5:
					if (m_bRunning)
					{
						m_bRunning = false;
						Seconds = 0.0;
					}
					else
						NavigationService.GoBack();
					break;
				case RM1.PadKeys.FN_UP: break;
				case RM1.PadKeys.FN_UP_Short: ManualAdjust(1); break;
				case RM1.PadKeys.FN_UP_Long: ManualAdjust(1); break;
				case RM1.PadKeys.FN_UP_Repeat: ManualAdjust(1); break;
				case RM1.PadKeys.FN_DOWN: break;
				case RM1.PadKeys.FN_DOWN_Short: ManualAdjust(-1); break;
				case RM1.PadKeys.FN_DOWN_Long: ManualAdjust(-1); break;
				case RM1.PadKeys.FN_DOWN_Repeat: ManualAdjust(-1); break;



				/*
				case RM1.PadKeys.FN_UP: break;
				case RM1.PadKeys.FN_UP_Short: AdjustBot(unit, 1); break;
				case RM1.PadKeys.FN_UP_Long: AdjustBot(unit, 1); break;
				case RM1.PadKeys.FN_UP_Repeat: AdjustBot(unit, 1); break;
				case RM1.PadKeys.FN_DOWN: break;
				case RM1.PadKeys.FN_DOWN_Short: AdjustBot(unit, -1); break;
				case RM1.PadKeys.FN_DOWN_Long: AdjustBot(unit, -1); break;
				case RM1.PadKeys.FN_DOWN_Repeat: AdjustBot(unit, -1); break;
				 */
				default:
					base.OnDirectKey(unit, key);
					break;
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
