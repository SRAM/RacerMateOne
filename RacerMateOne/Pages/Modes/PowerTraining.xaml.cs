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
using System.ComponentModel; // CancelEventArgs
using System.Diagnostics;

namespace RacerMateOne.Pages.Modes
{
	/// <summary>
	/// Interaction logic for PowerTraining.xaml
	/// </summary>
	public partial class PowerTraining : RideBase
	{
		AppWin m_App;
		public PowerTraining()
		{
			InitializeComponent();
			m_App = AppWin.Instance;
		}

		bool m_bReset;
		bool m_bAllSaved;

		protected Course m_Course;
		protected Course.Location m_BikeLoc;
		public Course Course
		{
			get { return m_Course; }
			set
			{
				m_Course = value;
				d_Course.Course = m_Course;
				if (m_Course != null)
				{
					//ERGMode = (m_Course.Type & CourseType.Distance) != CourseType.Distance;
					m_BikeLoc = new Course.Location(m_Course, 0);
				}
				else
					m_BikeLoc = null;
			}
		}

		Unit m_Unit;
		Statistics m_Statistics;

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			MainGrid.Children.Remove(DlgGrid);
			Dlg.Content = DlgGrid;
			Dlg.FadeTo = 0.0;

			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				SelectMasterPad();
				m_Unit = Unit.RiderUnits[0];
				m_Statistics = m_Unit.Statistics;
				RideBase_Loaded(TopGrid);
				Course = Unit.Course;

				if (Course.Manual)
					lx_Remaining.Visibility = Visibility.Collapsed;

				m_bTimeBased = m_Course.XUnits == CourseXUnits.Time;
				m_bLoadBased = m_Course.YUnits != CourseYUnits.Grade;
				m_bPercentBased = m_Course.YUnits == CourseYUnits.PercentAT;


				//d_Course.BikeIcon = BikeIcon;
				d_Course.Distance = 0;
				d_Course.Unit = m_Unit;
				m_MultiLap = Unit.Laps > 1;
				if (m_MultiLap)
				{
				}
				else
				{
					l_Current.Opacity = d_Current.Opacity = l_Best.Opacity = d_Best.Opacity = l_Last.Opacity = d_Last.Opacity = 0.3;
				}
				m_LapRun = m_Course.LengthFillTextBlock(d_TrackInfo,Unit.Laps);
				if (Course.Manual)
				{
					d_TrackInfo.Visibility = Visibility.Hidden;
				}

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


				d_Graph.Statistics = m_Unit.Statistics;
				ck_Speed.Tag = d_Graph.AddItem("SpeedDisplay", RM1_Settings.General.GraphSpeed_Min, RM1_Settings.General.GraphSpeed_Max, ck_Speed.Fill, 2, ck_Speed.IsOn);
				ck_Watts.Tag = d_Graph.AddItem("Watts", RM1_Settings.General.GraphPower_Min, RM1_Settings.General.GraphPower_Max, ck_Watts.Fill, 2, ck_Watts.IsOn);
				ck_HeartRate.Tag = d_Graph.AddItem("HeartRate", RM1_Settings.General.GraphHR_Min, RM1_Settings.General.GraphHR_Max, ck_HeartRate.Fill, 2, ck_HeartRate.IsOn);
				ck_Cadence.Tag = d_Graph.AddItem("Cadence", RM1_Settings.General.GraphRPM_Min, RM1_Settings.General.GraphRPM_Max, ck_Cadence.Fill, 2, ck_Cadence.IsOn);
                
                TargetRPM = RM1_Settings.General.GraphRPM_Target;

                TargetRPMInput.Text = TargetRPM.ToString();
                //Debug.WriteLine("I just updated the RPM Pointer = " + TargetRPM);
				ck_Speed.IsOn = ReviewBase.GetSavedState(ck_Speed.Name);
				ck_Watts.IsOn = ReviewBase.GetSavedState(ck_Watts.Name);
				ck_HeartRate.IsOn = ReviewBase.GetSavedState(ck_HeartRate.Name);
				ck_Cadence.IsOn = ReviewBase.GetSavedState(ck_Cadence.Name);

				List<object> t;
				if (!m_bLoadBased)
				{
					t = new List<object>();
					t.Add(d_Graph.AddItem("SSLeft", 180, 0, ck_SS.Fill, 2, ck_SS.IsOn));
					t.Add(d_Graph.AddItem("SSRight", 180, 0, ck_SS.Fill, 2, ck_SS.IsOn));
					t.Add(d_Graph.AddItem("SS", 180, 0, ck_SS.Fill, 2, ck_SS.IsOn));
					ck_SS.Tag = t;

					t = new List<object>();
					t.Add(d_Graph.AddItem("SSLeft_Avg", 180, 0, ck_SS_Avg.Fill, 2, ck_SS_Avg.IsOn));
					t.Add(d_Graph.AddItem("SSRight_Avg", 180, 0, ck_SS_Avg.Fill, 2, ck_SS_Avg.IsOn));
					t.Add(d_Graph.AddItem("SS_Avg", 180, 0, ck_SS_Avg.Fill, 2, ck_SS_Avg.IsOn));
					ck_SS_Avg.Tag = t;

					t = new List<object>();
					t.Add(d_Graph.AddItem("SSLeftATA", 180, 0, ck_SS_ATA.Fill, 2, ck_SS_ATA.IsOn));
					t.Add(d_Graph.AddItem("SSRightATA", 180, 0, ck_SS_ATA.Fill, 2, ck_SS_ATA.IsOn));
					ck_SS_ATA.Tag = t;

					t = new List<object>();
					t.Add(d_Graph.AddItem("SSLeftSplit", 180, 0, ck_SS_Watts.Fill, 2, ck_SS_Watts.IsOn));
					t.Add(d_Graph.AddItem("SSRightSplit", 180, 0, ck_SS_Watts.Fill, 2, ck_SS_Watts.IsOn));
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

				if (Course.YUnits != CourseYUnits.Grade)
				{
					d_DragFactor.Opacity = 0.3;
					l_DragFactor.Opacity = 0.3;
				}

				AppWin.AddRenderUpdate(new AppWin.RenderUpdate(RenderUpdate), 0);

				Unit.AddNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitUpdate));
				OnUnitUpdate(m_Unit, m_StatFlags);
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
            // ECT - make sure it turns off update when page unloads
            d_Graph.Statistics = null;
			Unit.Manual = false;
			Unit.RemoveNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitUpdate));
			RideBase_Unloaded();
			if (m_Unit != null && m_Unit.Trainer != null)
				m_Unit.Trainer.SetGrade(0, true);
			if (m_Unit != null && m_Unit.Rider != null && m_AdjustDragFactor)
			{
				m_AdjustDragFactor = false;
				m_Unit.Rider.DragFactor = m_AdjustDragFactorSave;
			}
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
				d_RiderName.Content = s.RiderName;
			if ((changed & StatFlags.Lap) != StatFlags.Zero)
			{
				changed |= StatFlags.LapTime | StatFlags.Time;
				d_Last.Content = s.LastLapTimeString;
				d_Best.Content = s.BestLapTimeString;
				if (s.Lap > 1)
				{
					l_Best.Content = String.Format("Best (lap {0})", s.BestLap);
					l_Last.Opacity = l_Best.Opacity = d_Best.Opacity = d_Last.Opacity = 1;
				}
				else
				{
					l_Best.Content = "Best";
					l_Last.Opacity = l_Best.Opacity = d_Best.Opacity = d_Last.Opacity = 0.3;
				}
				if (m_LapRun != null)
					m_LapRun.Text = String.Format("{0}/{1}", s.Lap > Unit.Laps ? Unit.Laps : s.Lap, Unit.Laps);
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
			if ((changed & StatFlags.HeartRateAlarm) != StatFlags.Zero)
				d_HeartRate.Foreground = unit.Statistics.HeartRateFlash ? Brushes.Red : Brushes.White;

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
			if ((changed & StatFlags.Finished) != StatFlags.Zero)
			{
				if (AllFinished())
				{
					m_bReset = true;
					Finished();
				}
			}

		}
        // ECT -  Added IncreaseSomething() and DecreaseSomething() for testing without the keypad controller
        // We can remove it if not needed anymore
        protected override void IncreaseSomething()
        {
            ManualAdjust(1);
        }
        protected override void DecreaseSomething()
        {
            ManualAdjust(-1);
        }

		bool m_AdjustDragFactor = false;
		int m_AdjustDragFactorSave;

        void ManualAdjust(float dir)
		{
			if (Unit.State == Statistics.State.Stopped && m_Unit != null && m_Unit.Rider != null && Course.YUnits == CourseYUnits.Grade)
			{
				if (!m_AdjustDragFactor)
				{
					m_AdjustDragFactor = true;
					m_AdjustDragFactorSave = m_Unit.Rider.DragFactor;
				}
				
				m_Unit.Rider.DragFactor += (int)dir;

				//if (m_Unit.Trainer != null)
				//	m_Unit.Trainer.UpdateGradeOrLoad();
				d_DragFactor.Content = m_Unit.Rider.DragFactor;
				return;
			}


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
		void StartDialog(DialogCB cb, object toptext, object f1text, object f2text, bool progress) { StartDialog(cb, toptext, f1text, f2text, null, progress); }
		void StartDialog(DialogCB cb, object toptext, object f1text, object f2text, object f3text, bool progress)
		{
			if (m_CurDialogCB != null)
				m_CurDialogCB(RM1.PadKeys.MAX);

			Dlg_TopLabel.Content = toptext;
			Dlg_f1_Label.Content = f1text != null ? f1text : "";
			Dlg_f1.Visibility = f1text != null ? Visibility.Visible : Visibility.Collapsed;
			Dlg_f2_Label.Content = f2text != null ? f2text : "";
			Dlg_f2.Visibility = f2text != null ? Visibility.Visible : Visibility.Collapsed;
			Dlg_f3_Label.Content = f3text != null ? f3text : "";
			Dlg_f3.Visibility = f3text != null ? Visibility.Visible : Visibility.Collapsed;
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

		Label Dlg_f3_Label;
		private void Dlg_f3_Loaded(object sender, RoutedEventArgs e) { Dlg_f3_Label = (Label)sender; }
		private void Dlg_f3_Click(object sender, RoutedEventArgs e)
		{
			if (m_CurDialogCB != null && Dlg_f3.Visibility == Visibility.Visible)
				m_CurDialogCB(RM1.PadKeys.F3);
		}

		//=============================================================
		int m_TargetRPM = RM1_Settings.General.GraphRPM_Target;
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
                RM1_Settings.General.GraphRPM_Target = value;
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

		void cb_SaveDragFactor(RM1.PadKeys key)
		{
			bool save = key == RM1.PadKeys.F1;
			if (save)
				Riders.SaveToFile();
			else
				m_Unit.Rider.DragFactor = m_AdjustDragFactorSave;
			d_DragFactor.Content = m_Unit.Rider.DragFactor;
			m_AdjustDragFactor = false;
			CloseDialog();
			Start();
		}

		protected override void Start()
		{
			if (m_AdjustDragFactor)
			{
				StartDialog(new DialogCB(cb_SaveDragFactor), String.Format("Do you wish to set your DragFactor to {0}?",m_Unit.Rider.DragFactor), "Yes", "No", false);
				return;
			}

			Unit.ClearCalibration();
			UpdateCalibrationString();
			m_Save = null;
			m_bReset = false;
			if (!m_bAllSaved)
				base.Start();
			else
				RealReset();
		}
		private void CountDown_Go(object sender, RoutedEventArgs e)
		{
			base.Start();
		}

		protected override void Pause()
		{
			Paused.FadeTo = 1;
			base.Pause();
		}
		protected override void UnPause()
		{
			Paused.FadeTo = 0;
			base.UnPause();
			UpdateCalibrationString();
		}



		//=======================================================================================================
		SavePerformance m_Save;
		protected override void SavePerformance()
		{
			if (m_Save != null)
				return;
            ReportColumns rcolumn = null; 
            rcolumn = m_Course.YUnits == CourseYUnits.PercentAT ? ReportColumns.Report_WattTestingAT : ReportColumns.Report_WattTestingERG;
            m_Save = new SavePerformance(rcolumn.Selected_ReportColumns);
			Unit.Stop(); // Stop everything if we haven't already
			StartDialog(new DialogCB(cb_ExportFiles), "Save Export files?", "Yes", "No", false);
			if (!RM1_Settings.General.ExportPrompt)
				cb_ExportFiles(RM1_Settings.General.ExportSave ? RM1.PadKeys.F1 : RM1.PadKeys.F2);
		}
		void cb_ExportFiles(RM1.PadKeys key)
		{
			m_Save.ExportSave = key == RM1.PadKeys.F1;
			CloseDialog();
			StartDialog(cb_SaveReport, "Save report?", "Yes", "No", false);
			if (!RM1_Settings.General.ReportPrompt)
				cb_SaveReport(RM1.PadKeys.F1);
		}
		void cb_SaveReport(RM1.PadKeys key)
		{
			m_Save.SaveReport = key == RM1.PadKeys.F1;
			CloseDialog();

			StartDialog(new DialogCB(cb_PWXPrompt), "Save PWX file?", "Yes", "No", false);
			if (!RM1_Settings.General.PWXPrompt)
				cb_PWXPrompt(RM1_Settings.General.PWXSave ? RM1.PadKeys.F1 : RM1.PadKeys.F2);
			/*
			StartDialog(new DialogCB(cb_LaunchPrompt), "Launch Training Peaks?", "Yes", "No", false);
			if (!RM1_Settings.General.LaunchPrompt)
				cb_LaunchPrompt( RM1_Settings.General.Launch ? RM1.PadKeys.F1: RM1.PadKeys.F2 );
			*/
		}
		void cb_PWXPrompt(RM1.PadKeys key)
		{
			m_Save.SavePWX = key == RM1.PadKeys.F1;
			CloseDialog();
			StartDialog(new DialogCB(cb_Saving), "Saving performance files", null, "Cancel", true);
			m_Save.Progress += new SavePerformance.ProgressEvent(cb_SaveProgress);
			m_Save.LaunchProgram = key == RM1.PadKeys.F1;
			m_Save.Save();
		}
		/*
		void cb_LaunchPrompt( RM1.PadKeys key )
		{
			CloseDialog();
			StartDialog(new DialogCB(cb_Saving), "Saving performance files", null, "Cancel", true);
			m_Save.Progress += new SavePerformance.ProgressEvent(cb_SaveProgress);
			m_Save.LaunchProgram = key == RM1.PadKeys.F1;
			m_Save.Save();
		}
		*/
		void cb_Saving(RM1.PadKeys key)
		{
			if (key == RM1.PadKeys.F2)
			{
				m_Save.Cancel();
				CloseDialog();
				if (m_bReset)
				{
					m_bReset = false;
					RealReset();
				}
			}
		}
		void cb_SaveProgress(double progress, bool done)
		{
			if (done)
			{
				m_bAllSaved = true;
				CloseDialog();
				if (m_bReset)
				{
					m_bReset = false;
					RealReset();
				}
			}
			double w = Dlg_ProgressBar.ActualWidth;
			PerfProgressBar.Width = w * progress;
		}

		bool m_bBackReset;

		void RealReset()
		{
			m_bAllSaved = false;
			m_bReset = false;
			UnPause();
			base.Reset();
			if (m_bBackReset)
			{
				m_bBackReset = false;
				Back();
			}
		}

		//=======================================================================================================
		protected override void Reset()
		{
			if (!Unit.HasStarted || AllFinished())
				RealReset();
			else
			{
				Pause();
				if (RM1_Settings.General.SavePrompt)
					StartDialog(new DialogCB(cb_ResetSave), "Save performance file?", "Yes", "No", "Cancel", false);
				else if (m_bBackReset)
					StartDialog(new DialogCB(cb_Reset), "Exit and end my ride?", "Yes", "No", false);
				else
					StartDialog(new DialogCB(cb_Reset), "Reset my ride?", "Yes", "No", false);
			}
		}
		void Finished()
		{
			if (RM1_Settings.General.SavePrompt)
				StartDialog(new DialogCB(cb_ResetSave), "Save performance file?", "Yes", "No", null, false);
			else
				SavePerformance();
		}

		void cb_Reset(RM1.PadKeys key)
		{
			CloseDialog();
			if (key == RM1.PadKeys.F1)
				SavePerformance();
			else
			{
				m_bCloseOnExit = m_bBackReset = m_bReset = false;
				UnPause();
			}
		}
		void cb_ResetSave(RM1.PadKeys key)
		{
			CloseDialog();
			if (key == RM1.PadKeys.F1)
				SavePerformance();
			else if (key == RM1.PadKeys.F2)
			{
				UnPause();
				Unit.Stop();
				Unit.DeleteRecentPerformance();
				Unit.Reset();
				if (m_bBackReset)
					Back();
			}
			else
			{
				m_bCloseOnExit = m_bBackReset = m_bReset = false;
				UnPause();
			}
		}



		//=============================================================
		protected override void OnDirectKey(Unit unit, RM1.PadKeys key)
		{
			if (m_CurDialogCB != null)
			{
				if (unit == null || !unit.MasterPad)
					return;
				if (key == RM1.PadKeys.F1 || key == RM1.PadKeys.F2 || (Dlg_f3.Visibility == Visibility.Visible && key == RM1.PadKeys.F3))
					m_CurDialogCB(key);
				return;
			}
            if (unit.IsVelotron)
            {
                switch (key)
                {
                    /*
                    case RM1.PadKeys.F2: break;	// Make sure that this one doesn't happen at all
                    case RM1.PadKeys.F2_Short:
                        F2(unit);
                        break;
                    case RM1.PadKeys.F2_Long:
                        if (!ScreenChange(unit))
                            F2(unit);
                        break;
                    case RM1.PadKeys.F4:
                        F4(unit);
                        break;
                     */
                    case RM1.PadKeys.F3:
                        if (Unit.State == Statistics.State.Paused && m_Unit.Trainer != null)
                        {
                            m_Unit.Trainer.CalibrateMode = !unit.Trainer.CalibrateMode;
                            UpdateCalibrationString();
                            break;
                        }
                        break;
                    case RM1.PadKeys.F5:
                        if (Unit.FirstMasterUnit == unit)
                        {
                            m_bReset = true;
                            Reset();
                        }
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
            else
            {
                switch (key)
                {
                    /*
                    case RM1.PadKeys.F2: break;	// Make sure that this one doesn't happen at all
                    case RM1.PadKeys.F2_Short:
                        F2(unit);
                        break;
                    case RM1.PadKeys.F2_Long:
                        if (!ScreenChange(unit))
                            F2(unit);
                        break;
                    case RM1.PadKeys.F4:
                        F4(unit);
                        break;
                     */
                    case RM1.PadKeys.F3:
                        if (Unit.State == Statistics.State.Paused && m_Unit.Trainer != null)
                        {
                            m_Unit.Trainer.CalibrateMode = !unit.Trainer.CalibrateMode;
                            UpdateCalibrationString();
                            break;
                        }
                        break;
                    case RM1.PadKeys.F5:
                        if (Unit.FirstMasterUnit == unit)
                        {
                            m_bReset = true;
                            Reset();
                        }
                        break;
                    case RM1.PadKeys.UP: break;
                    case RM1.PadKeys.UP_Short: ManualAdjust(1); break;
                    case RM1.PadKeys.UP_Long: ManualAdjust(1); break;
                    case RM1.PadKeys.UP_Repeat: ManualAdjust(1); break;
                    case RM1.PadKeys.DOWN: break;
                    case RM1.PadKeys.DOWN_Short: ManualAdjust(-1); break;
                    case RM1.PadKeys.DOWN_Long: ManualAdjust(-1); break;
                    case RM1.PadKeys.DOWN_Repeat: ManualAdjust(-1); break;
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
		}


		//=============================================================
		private void Options_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Pages.RideOptions());
		}
		private void Back_Click(object sender, RoutedEventArgs e)
		{
			m_bReset = true;
			m_bBackReset = true;
			Reset();
		}
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help();
		}

		private void Dlg_Loaded(object sender, RoutedEventArgs e)
		{

		}

		protected override void OnClose(CancelEventArgs e)
		{
			e.Cancel = true;
			m_bCloseOnExit = true;
			m_bReset = true;
			m_bBackReset = true;
			Reset();
			
		}
        private void BindTo(String name, Object source, FrameworkElement f, DependencyProperty dp, IValueConverter conv)
        {
            Binding binding = new Binding(name);
            binding.Source = source;
            if (conv != null)
                binding.Converter = conv;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            f.SetBinding(dp, binding);
        }
	}
}
