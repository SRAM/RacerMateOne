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


namespace RacerMateOne.Pages.Modes
{
	/// <summary>
	/// Interaction logic for SpinScan.xaml
	/// </summary>
	public partial class RCV : RideBase
	{
		AppWin m_App;
		public RCV()
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
				//d_Course.Course = m_Course;
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
		ReportColumns m_DisplayColumns;
		StatFlags m_DisplayStatFlags;
		Run m_Lap;
		public Course OriginalCourse;

		public bool StartDemo;

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			m_Unit = Unit.RiderUnits[0];
			Unit.AppMode = AppModes.RCV;
			if (StartDemo)
				DemoOn();
			d_OverlayGrid.Children.Remove(DlgGrid);
			Dlg.Content = DlgGrid;
			Dlg.FadeTo = 0.0;
			m_DisplayColumns = ReportColumns.Display_RCV.Selected_ReportColumns;
			m_DisplayStatFlags = m_DisplayColumns.StatFlags;

			DpConverter iconv = new DpConverter(0);

			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				Unit unit = m_Unit;
				SelectMasterPad();
				m_Statistics = m_Unit.Statistics;
				RideBase_Loaded(TopGrid);
				Course = Unit.Course;


				m_bTimeBased = m_Course.XUnits == CourseXUnits.Time;
				m_bLoadBased = m_Course.YUnits != CourseYUnits.Grade;
				if (m_bLoadBased)
					throw new Exception("Track cannot be load based!");

				
				//d_Course.Course = Unit.Course;
				//d_Course.LoadUnits();
				//d_Course.BikeIcon = BikeIcon;
				d_Course.Distance = 0;
				d_Course.Unit = unit;

				unit = Unit.RaceUnit[0];
				RCV_1.Course = Unit.Course;
				RCV_1.Location = OriginalCourse != null ? OriginalCourse.StartAt : 0;
				RCV_1.FirstUnit = false;
				RCV_1.Unit = unit;



				if (m_Unit.IsVelotron)
				{
					m_DisplayStatFlags |= StatFlags.Gearing | StatFlags.GearInches;
				}
				else
				{
				}
				d_Stats.StatFlags = m_DisplayStatFlags;
				d_Stats.Unit = m_Unit;
				d_Timer_r.Unit = m_Unit;

				m_Lap = Unit.Course.LengthFillTextBlock(d_Length, Unit.Laps);
				d_CourseName.Text = Unit.Course.Name;

				AppWin.AddRenderUpdate(new AppWin.RenderUpdate(RenderUpdate), 0);

				Unit.AddNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitUpdate));
				OnUnitUpdate(m_Unit, m_StatFlags);
				RCVScreen_SizeChanged(null, null);
				UpdateMode();

				if (StartDemo)
					CountDown_Go(null, null);
				Statistics.Unregistered = OriginalCourse.Registered == false;
			});
		}

		private void RCVScreen_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double h = ActualHeight;
			if (h != 0)
			{
				Point pt = d_BottomPanel.TranslatePoint(new Point(0, 0), AppWin.Instance); // Figure out the top of the panel on the main Screen.
				double y = h - pt.Y;
				if (y > 0)
				{
					g_Bottom.Height = new GridLength(y, GridUnitType.Pixel);
				}
			}
		}

		double m_RedrawManual = 0;
		bool m_retime = false;
		bool RenderUpdate(double seconds, double split)
		{
			if (Course.Manual)
			{
				//d_Course.RedrawCourse();
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
            // TODO - Will check this, Call when unloading to revert all demobots back
            DemoOff();

			AppWin.RemoveRenderUpdate(new AppWin.RenderUpdate(RenderUpdate));
			Unit.Manual = false;
			Unit.RemoveNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitUpdate));
			RideBase_Unloaded();
		}

		StatFlags m_StatFlags =
			//StatFlags.RiderName | StatFlags.Lap | StatFlags.Distance | StatFlags.SS_Stats | 
			StatFlags.Time | StatFlags.Lap | StatFlags.Finished;

		bool m_bTimeBased;
		bool m_bLoadBased;


		void OnUnitUpdate(Unit unit, StatFlags changed)
		{
			Statistics s = unit.Statistics;

			if ((changed & StatFlags.Lap) != StatFlags.Zero && m_Lap != null)
			{
				int lap = unit.Statistics.Lap;
				m_Lap.Text = String.Format("{0}/{1}", lap > Unit.Laps ? Unit.Laps : lap, Unit.Laps);
			}
			if ((changed & StatFlags.Finished) != StatFlags.Zero)
			{
				if (AllFinished())
				{
					m_bReset = true;
					Finished();
				}
			}
			if ((changed & StatFlags.Time) != StatFlags.Zero)
				d_MainTimer.Content = Statistics.MasterTimerString;
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
		//=======================================================================
		protected override void Start()
		{
			Unit.ClearCalibration();
			//UpdateCalibrationString();
			m_Save = null;
			m_bReset = false;
			if (!m_bAllSaved)
				CountDown_Go(null,null); // CountDown.Start();
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
			//UpdateCalibrationString();
		}

		int m_Mode = 0;
		int m_ModeMax = 4;

		void AdjustScreen(Unit unit)
		{
			RCV_1_v.Stretch = RCV_1_v.Stretch == Stretch.UniformToFill ? Stretch.Uniform : Stretch.UniformToFill;
		}


		void Cycle(Unit unit)
		{
			m_Mode++;
			if (m_Mode >= m_ModeMax)
				m_Mode = 0;
			UpdateMode();
		}
		void UpdateMode()
		{
			switch (m_Mode)
			{
				case 0:
					d_StatsBox_Panel.Visibility = Visibility.Collapsed;
					d_SS_Panel.Visibility = Visibility.Collapsed;
					d_SS_Box.Unit = d_SS.Unit = d_StatsBox.Unit = null;
					break;
				case 1:
					d_StatsBox_Panel.Visibility = Visibility.Collapsed;
					d_SS_Panel.Visibility = Visibility.Visible;
					d_SS_Box.Unit = d_SS.Unit = m_Unit;
					d_SS.Polar = true;
					d_StatsBox.Unit = null;
					break;
				case 2:
					d_StatsBox_Panel.Visibility = Visibility.Collapsed;
					d_SS_Panel.Visibility = Visibility.Visible;
					d_SS_Box.Unit = d_SS.Unit = m_Unit;
					d_SS.Polar = false;
					d_StatsBox.Unit = null;
					break;
				case 3:
					d_StatsBox_Panel.Visibility = Visibility.Visible;
					d_SS_Panel.Visibility = Visibility.Collapsed;
					d_SS_Box.Unit = d_SS.Unit = null;
					d_StatsBox.Unit = m_Unit;
					break;
			}
		}


		//=======================================================================================================
		SavePerformance m_Save;
		protected override void SavePerformance()
		{
			if (m_Save != null)
				return;
			m_Save = new SavePerformance(ReportColumns.Report_RCV.Selected_ReportColumns);
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
				case RM1.PadKeys.F2: break;	// Make sure that this one doesn't happen at all
				case RM1.PadKeys.F2_Short:
					Cycle(unit);
					break;
				case RM1.PadKeys.F2_Long:
					AdjustScreen(unit);
					break;
				case RM1.PadKeys.F3:
					if (Unit.State == Statistics.State.Paused && m_Unit.Trainer != null)
					{
						m_Unit.Trainer.CalibrateMode = !unit.Trainer.CalibrateMode;
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
				default:
					base.OnDirectKey(unit, key);
					break;
			}
		}

		protected override void Keyboard_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.V:
					OnDirectKey(SelectedUnit, RM1.PadKeys.F2_Long);
					break;
				default:
					base.Keyboard_KeyDown(sender, e);
					break;
			}
		}


		//=============================================================
		bool m_bDemo;
		UnitSave m_DemoSave;


		protected override void ToggleDemo()
		{
			if (!m_bDemo)
				DemoOn();
			else
				DemoOff();
		}
		void DemoOn()
		{
			if (m_bDemo)
				return;
			m_bDemo = true;
			m_DemoSave = new UnitSave();	// Save out the current state of the units.
			// Change all the bots to a active unit.
			SetUnitBot(m_Unit);
		}
		void DemoOff()
		{
			if (!m_bDemo)
				return;
			m_bDemo = false;
			m_DemoSave.Restore();
		}
		protected override bool IsDemo
		{
			get
			{
				return m_bDemo;
			}
		}
		void SetUnitBot(Unit unit)
		{
			String bkey = m_DemoSave.Nodes[unit.Number].BotKey;
			if (bkey == null || !m_DemoSave.Nodes[unit.Number].IsActive)
			{
				DemoBot dbot = new DemoBot();
				unit.Bot = dbot; //  WattsBot(200 + unit.Number * 20);

				dbot.DestSpeed = unit.Statistics.Speed;
				if (unit.Statistics.Finished)
					unit.Statistics.Reset();
				if (dbot.DestSpeed < ConvertConst.mps_mph_2)
					dbot.DestSpeedInt = ConvertConst.MPHToMetersPerSecond * (12 + unit.Number);
				unit.IsDemoUnit = true;


				if (unit.Statistics.CurrentState != Unit.State)
				{
					switch (Unit.State)
					{
						case Statistics.State.Running: unit.Statistics.Start(); break;
						case Statistics.State.Paused: unit.Statistics.Start(); unit.Statistics.Pause(); break;
						case Statistics.State.Stopped: unit.Statistics.Stop(); break;
					}
				}
				unit.Bot.Name = unit.Rider != null ? unit.Rider.ToString() : ms_BotNames[unit.Number];

			}
			else if (unit.Bot == null && bkey != null)
			{
				m_DemoSave.RestoreNode(unit.Number);
			}
		}
		protected override void LeftButton(bool down)
		{
			if (!down)
				return;
			Bot bot = SelectedUnit.Bot;
			if (bot != null)
			{
				bot.Adjust(-1);
			}
		}
		protected override void RightButton(bool down)
		{
			if (!down)
				return;
			Bot bot = SelectedUnit.Bot;
			if (bot != null)
			{
				bot.Adjust(1);
			}
		}

		protected override void AddDemoRider()
		{
			Staging.RCVDemo = 2;
			Back();
		}

		//=============================================================
		private void Options_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Pages.RideOptions());
		}
		private void Back_Click(object sender, RoutedEventArgs e)
		{
			Back();
		}
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help();
		}

		protected override void OnClose(CancelEventArgs e)
		{
			e.Cancel = true;
			m_bCloseOnExit = true;
			m_bReset = true;
			m_bBackReset = true;
			Reset();

		}

	}
}
