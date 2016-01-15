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
	/// Interaction logic for PowerTraining.xaml
	/// </summary>
	public partial class PowerTraining_Review : ReviewBase
	{
		AppWin m_App;
		public PowerTraining_Review()
		{
			InitializeComponent();
			m_App = AppWin.Instance;
		}

		Statistics m_Statistics;

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			MainGrid.Children.Remove(DlgGrid);
			Dlg.Content = DlgGrid;
			Dlg.FadeTo = 0.0;

			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				m_Unit = Unit.RiderUnits[0];
				m_Statistics = m_Unit.Statistics;
				RideBase_Loaded(TopGrid);

				m_bTimeBased = m_Course.XUnits == CourseXUnits.Time;
				m_bLoadBased = m_Course.YUnits != CourseYUnits.Grade;
				m_bPercentBased = m_Course.YUnits == CourseYUnits.PercentAT;


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

				if (m_bLoadBased)
				{
					d_Grade.Opacity = 0.3;
					d_Grade.Content = "Grade";
					d_Wind.Opacity = 0.3;
					d_Wind.Content = "Wind";
					m_StatFlags &= ~(StatFlags.Grade | StatFlags.Wind | StatFlags.SS_Stats);
					d_SpinScanSection.Opacity = 0.3;
					d_Disable_SpinScan.Visibility = Visibility.Visible;
					if (m_bPercentBased)
						d_Load.Margin = new Thickness(-25, 0, 0, 0);
				}
				else
				{
					d_Load.Opacity = 0.3;
					d_Load.Content = "Load";
					m_StatFlags &= ~(StatFlags.Watts_Load);
				}


				d_Graph.Perf = Performance;
				d_Graph.Course = Course;
				d_Graph.StartSeconds = m_StartSeconds;
				d_Graph.EndSeconds = m_EndSeconds;

				ck_Speed.Tag = d_Graph.AddItem(StatFlags.Speed,"Speed", RM1_Settings.General.GraphSpeed_Min, RM1_Settings.General.GraphSpeed_Max, ck_Speed.Fill, 2, ck_Speed.IsOn);
				ck_Watts.Tag = d_Graph.AddItem(StatFlags.Watts, "Watts", RM1_Settings.General.GraphPower_Min, RM1_Settings.General.GraphPower_Max, ck_Watts.Fill, 2, ck_Watts.IsOn);
				ck_HeartRate.Tag = d_Graph.AddItem(StatFlags.HeartRate,"HeartRate", RM1_Settings.General.GraphHR_Min, RM1_Settings.General.GraphHR_Max, ck_HeartRate.Fill, 2, ck_HeartRate.IsOn);
				ck_Cadence.Tag = d_Graph.AddItem(StatFlags.Cadence, "Cadence", RM1_Settings.General.GraphRPM_Min, RM1_Settings.General.GraphRPM_Max, ck_Cadence.Fill, 2, ck_Cadence.IsOn);

				ck_Speed.IsOn = ReviewBase.GetSavedState(ck_Speed.Name);
				ck_Watts.IsOn = ReviewBase.GetSavedState(ck_Watts.Name);
				ck_HeartRate.IsOn = ReviewBase.GetSavedState(ck_HeartRate.Name);
				ck_Cadence.IsOn = ReviewBase.GetSavedState(ck_Cadence.Name);


				List<object> t;
				if (!m_bLoadBased)
				{
					t = new List<object>();
					t.Add(d_Graph.AddItem(StatFlags.SSLeft,"SSLeft", 180, 0, ck_SS.Fill, 2, ck_SS.IsOn));
					t.Add(d_Graph.AddItem(StatFlags.SSRight,"SSRight", 180, 0, ck_SS.Fill, 2, ck_SS.IsOn));
					t.Add(d_Graph.AddItem(StatFlags.SS,"SS", 180, 0, ck_SS.Fill, 2, ck_SS.IsOn));
					ck_SS.Tag = t;

					t = new List<object>();
					t.Add(d_Graph.AddItem(StatFlags.SSLeft_Avg,"SSLeft_Avg", 180, 0, ck_SS_Avg.Fill, 2, ck_SS_Avg.IsOn));
					t.Add(d_Graph.AddItem(StatFlags.SSRight_Avg,"SSRight_Avg", 180, 0, ck_SS_Avg.Fill, 2, ck_SS_Avg.IsOn));
					t.Add(d_Graph.AddItem(StatFlags.SS_Avg,"SS_Avg", 180, 0, ck_SS_Avg.Fill, 2, ck_SS_Avg.IsOn));
					ck_SS_Avg.Tag = t;

					t = new List<object>();
					t.Add(d_Graph.AddItem(StatFlags.SSLeftATA,"SSLeftATA", 180, 0, ck_SS_ATA.Fill, 2, ck_SS_ATA.IsOn));
					t.Add(d_Graph.AddItem(StatFlags.SSRightATA,"SSRightATA", 180, 0, ck_SS_ATA.Fill, 2, ck_SS_ATA.IsOn));
					ck_SS_ATA.Tag = t;

					t = new List<object>();
					t.Add(d_Graph.AddItem(StatFlags.SSLeftSplit,"SSLeftSplit", 180, 0, ck_SS_Watts.Fill, 2, ck_SS_Watts.IsOn));
					t.Add(d_Graph.AddItem(StatFlags.SSRightSplit,"SSRightSplit", 180, 0, ck_SS_Watts.Fill, 2, ck_SS_Watts.IsOn));
					ck_SS_Watts.Tag = t;

					ck_SS.IsOn = ReviewBase.GetSavedState(ck_SS.Name);
					ck_SS_Avg.IsOn = ReviewBase.GetSavedState(ck_SS_Avg.Name);
					ck_SS_ATA.IsOn = ReviewBase.GetSavedState(ck_SS_ATA.Name);
					ck_SS_Watts.IsOn = ReviewBase.GetSavedState(ck_SS_Watts.Name);
				}

				l_Speed.Content = ConvertConst.TextMPHorKPH_C;

				if (m_Unit.IsVelotron)
				{
					m_StatFlags |= StatFlags.Gearing | StatFlags.GearInches;
					d_RRC.Opacity = 0.3;
					d_NoRRC.Visibility = Visibility.Collapsed;
					d_RRCText.Text = "";
				}
				else
				{
					d_GearInches.Opacity = d_Gearing.Opacity = 0.3;
				}
				UpdateCalibrationString();


				// One time things.
				d_Weight.Content = m_Unit.Rider.WeightTotalDisplay;
				d_DragFactor.Content = m_Unit.Rider.DragFactor;

				AppWin.AddRenderUpdate(new AppWin.RenderUpdate(RenderUpdate), 0);

				Unit.AddNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitUpdate));
				OnUnitUpdate(m_Unit, m_StatFlags);
				d_Graph.UpdateGraphs();
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
			StatFlags.RiderName | StatFlags.Time | StatFlags.Lap | StatFlags.Distance |
			StatFlags.Grade | StatFlags.Wind | StatFlags.Watts_Load |
			StatFlags.PulsePower | StatFlags.Calories | StatFlags.Watts_Wkg | StatFlags.TSS_IF_NP |
			StatFlags.Speed | StatFlags.Speed_Avg | StatFlags.Speed_Max |
			StatFlags.Watts | StatFlags.Watts_Avg | StatFlags.Watts_Max |
			StatFlags.HeartRate | StatFlags.HeartRate_Avg | StatFlags.HeartRate_Max | StatFlags.HeartRateAlarm |
			StatFlags.Cadence | StatFlags.Cadence_Avg | StatFlags.Cadence_Max |
			StatFlags.SS_Stats | StatFlags.Calibration | StatFlags.Finished;

		bool m_bTimeBased;
		bool m_bLoadBased;
		bool m_bPercentBased;

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
				if (!m_bTimeBased)
					d_Remaining.Text = ConvertConst.MetersToDistanceString(m_Course.TotalX * Unit.Laps - s.Distance);
			}
			if ((changed & StatFlags.Grade) != StatFlags.Zero)
				d_Grade.Content = String.Format("Grade {0}%", s.Grade_String);
			if ((changed & StatFlags.Watts_Load) != StatFlags.Zero)
			{
				if (m_bPercentBased)
					d_Load.Content = String.Format("Load {0:F0} Watts {1:F0}%", s.Watts_Load, s.PercentAT);
				else
					d_Load.Content = String.Format("Load {0:F0} Watts", s.Watts_Load);

			}
			if ((changed & StatFlags.Wind) != StatFlags.Zero)
				d_Wind.Content = String.Format("Wind {0} {1}", s.Wind_String,ConvertConst.TextMPHorKPH_C);

			if ((changed & StatFlags.GearInches) != StatFlags.Zero)
				d_GearInches.Content = String.Format("Gear {0}", s.GearInches);
			if ((changed & StatFlags.Gearing) != StatFlags.Zero)
				d_Gearing.Content = String.Format("Gearing {0}", s.Gearing_String);

			if ((changed & StatFlags.PulsePower) != StatFlags.Zero)
				d_PulsePower.Content = s.PulsePower_String;
			if ((changed & StatFlags.Calories) != StatFlags.Zero)
				d_Calories.Content = s.Calories_String;
			if ((changed & StatFlags.Watts_Wkg) != StatFlags.Zero)
				d_Wkg.Content = s.Watts_Wkg_String;
			if ((changed & StatFlags.TSS) != StatFlags.Zero)
				d_TSS.Content = String.Format("{0:F1}",s.TSS);
			if ((changed & StatFlags.IF) != StatFlags.Zero)
				d_IF.Content = String.Format("{0:F1}", s.IF);
			if ((changed & StatFlags.NP) != StatFlags.Zero)
				d_NP.Content = String.Format("{0:F1}", s.NP);

			if ((changed & StatFlags.Speed) != StatFlags.Zero)
				d_Speed.Content = s.Speed_String;
			if ((changed & StatFlags.Speed_Avg) != StatFlags.Zero)
				d_Speed_Avg.Content = s.Speed_Avg_String;
			if ((changed & StatFlags.Speed_Max) != StatFlags.Zero)
				d_Speed_Max.Content = s.Speed_Max_String;

			if ((changed & StatFlags.Watts) != StatFlags.Zero)
				d_Watts.Content = s.Watts_String;
			if ((changed & StatFlags.Watts_Avg) != StatFlags.Zero)
				d_Watts_Avg.Content = s.Watts_Avg_String;
			if ((changed & StatFlags.Watts_Max) != StatFlags.Zero)
				d_Watts_Max.Content = s.Watts_Max_String;

			if ((changed & StatFlags.HeartRate) != StatFlags.Zero)
			{
				d_HRZone.Content = m_Unit.Rider.ZoneHR((int)s.HeartRate);
				d_HeartRate.Content = s.HeartRate_String;
			}
			//if ((changed & StatFlags.HeartRateAlarm) != StatFlags.Zero)
			//	d_HeartRate.Foreground = unit.Statistics.HeartRateFlash ? Brushes.Red : Brushes.White;
			if ((changed & StatFlags.HeartRate_Avg) != StatFlags.Zero)
				d_HeartRate_Avg.Content = s.HeartRate_Avg_String;
			if ((changed & StatFlags.HeartRate_Max) != StatFlags.Zero)
				d_HeartRate_Max.Content = s.HeartRate_Max_String;

			if ((changed & StatFlags.Cadence) != StatFlags.Zero)
			{
				RedoRPMLine();
				d_Cadence.Content = s.Cadence_String;
			}
			if ((changed & StatFlags.Cadence_Avg) != StatFlags.Zero)
				d_Cadence_Avg.Content = s.Cadence_Avg_String;
			if ((changed & StatFlags.Cadence_Max) != StatFlags.Zero)
				d_Cadence_Max.Content = s.Cadence_Max_String;

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
			/*
			if ((changed & StatFlags.Finished) != StatFlags.Zero)
			{
				if (AllFinished())
				{
					m_bReset = true;
					SavePerformance();
				}
			}
			 **/

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
			else if (m_Course.YUnits == CourseYUnits.Watts)
			{
				dir *= (float)(RM1_Settings.General.WattsStep);
				y += dir;
				y = y < 0 ? 0: y > 2000 ? 2000:y;
			}
			else
			{
				dir *= (float)(RM1_Settings.General.PercentATStep);
				y += dir;
				y = y < 0 ? 0 : y > 200 ? 200 : y;
			}

			m_Course.ManualY = y;
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
			KeyBox.Focus();
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


		//=============================================================
		int m_TargetRPM = 90;
		double m_MinRPM = 80.0f;
		double m_MaxRPM = 100.0f;
		public int TargetRPM
		{
			get { return m_TargetRPM; }
			set
			{
				if (value < 10)
					return;
				m_TargetRPM = value;
				m_MinRPM = value - 11.0f;
				m_MaxRPM = value + 11.0f;
				plus10.Content = (m_TargetRPM + 10).ToString();
				plus5.Content = (m_TargetRPM + 5).ToString();
				at0.Content = m_TargetRPM.ToString();
				minus5.Content = (m_TargetRPM - 5).ToString();
				minus10.Content = (m_TargetRPM - 10).ToString();
				RedoRPMLine();
			}
		}
		void RedoRPMLine()
		{
			if (!m_bInit || m_Statistics == null)
				return;
			double lineheight = RPMLine.ActualHeight;
			double rpm = m_Statistics.Cadence;
			if (rpm < m_MinRPM)
				rpm = m_MinRPM;
			else if (rpm > m_MaxRPM)
				rpm = m_MaxRPM;
			double n = 1.0 - (rpm - (m_TargetRPM - 10.0)) / 20.0;
			double h = RPMArea.ActualHeight - 21;
			double y = n * h + (10.5 - RPMLine.ActualHeight * 0.5);
			RPMLine.Margin = new Thickness(RPMLine.Margin.Left, y, RPMLine.Margin.Right, 0);
		}


		private void ck_IsOnChanged(object sender, RoutedEventArgs e)
		{
			Controls.ColorBox c = sender as Controls.ColorBox;

			if (c != null && c.Tag != null)
			{
				bool ison = c.IsOn;
				ReviewBase.SetSavedState(c.Name, ison);

				object tag = c.Tag;
				List<object> lst = tag as List<object>;
				if (lst != null)
				{
					foreach (object o in lst)
						d_Graph.ShowItem(o, ison);
				}
				else
					d_Graph.ShowItem(tag, ison);
			}
		}


		int m_TimeRPMCount = 0;
		DispatcherTimer m_TimeRPMFocus;
		private void TargetRPM_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				if (m_TimeRPMCount < 3)
					m_TimeRPMCount = 3;
				TargetRPM = Convert.ToInt32(TargetRPMInput.Text);
			}
			catch { }
		}
		private void TargetRPMInput_GotFocus(object sender, RoutedEventArgs e)
		{
			if (m_TimeRPMFocus == null)
			{
				m_TimeRPMFocus = new DispatcherTimer();
				m_TimeRPMFocus.Tick += new EventHandler(timeRPM_Tick);
				m_TimeRPMFocus.Interval = new TimeSpan(0, 0, 1);
			}
			m_TimeRPMCount = 5;
			m_TimeRPMFocus.Start();
		}
		private void TargetRPMInput_LostFocus(object sender, RoutedEventArgs e)
		{
			m_TimeRPMFocus.Stop();
		}
		void timeRPM_Tick(object sender, EventArgs e)
		{
			if (m_TimeRPMCount-- <= 0)
				KeyBox.Focus();
		}



		void UpdateCalibrationString()
		{
			if (m_Unit.Trainer == null || m_Unit.IsVelotron)
			{
				d_NoRRC.Visibility = Visibility.Collapsed;
				d_RRCText.Text = "";
				d_RRC.Opacity = 0.3;
			}
			else
			{
				d_NoRRC.Visibility = m_Unit.Statistics.IsCalibrated ? Visibility.Collapsed : Visibility.Visible;
				d_RRCText.Text = m_Unit.Statistics.CalibrationString;
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
		bool m_bNoCourseUpdate;
		private void d_Course_DistanceChanged(object sender, RoutedEventArgs e)
		{
			if (m_Unit == null)
				return;
			m_bNoCourseUpdate = true;
			try
			{
				double dist = (d_Course.Distance + Course.StartAt) + Course.TotalX * (m_Unit.Statistics.Lap - 1);
				XLocClip = dist;
			}
			catch { }
			m_bNoCourseUpdate = false;
		}


		bool m_bNoGraphUpdate;
		private void d_Graph_SecondsChanged(object sender, RoutedEventArgs e)
		{
			if (m_Unit == null)
				return;
			m_bNoGraphUpdate = true;
			try
			{
				Seconds = d_Graph.Seconds;
			}
			catch { }
			m_bNoGraphUpdate = false;
		}

		protected override void UpdateSeconds()
		{
			if (!m_bNoGraphUpdate)
			{
				d_Graph.Seconds = Seconds;
			}
			if (!m_bNoCourseUpdate)
			{
				double lapdistance = m_Unit.Statistics.LapDistance - Course.StartAt;
				lapdistance = lapdistance < 0 ? 0 : lapdistance > Course.TotalX ? Course.TotalX : lapdistance;
				d_Course.Distance = lapdistance;
				d_Course.OnDistance();
			}

			base.UpdateSeconds();
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
					//m_Polar = !m_Polar;
					//UpdateSpinScanMode();
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

		private void RiderOptions_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (m_bInit)
				d_Graph.UpdateGraphs();
		}




	}
}
