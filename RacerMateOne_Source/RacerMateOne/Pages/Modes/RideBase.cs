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
using System.ComponentModel; // CancelEventArgs


namespace RacerMateOne.Pages.Modes
{
	public partial class RideBase: Page
	{
		//============================================================
		public enum Mode: int
		{
			None,
			OnePlayer,
			TwoPlayer,
			TwoPlayerSplit,	// Special two player mode with 2 screens instead of single.
			MultiPlayer,	// 2 to 8 players
			MultiDisplay2,	// Like split but with one bar.
			MultiDisplay3,
			MultiDisplay4,
			MultiDisplay6,
			MultiDisplay8,
		};
		//============================================================
		protected String[] ms_BotNames = new String[] { "Jamie", "Baily", "Ashton", "Jessie", "Shannon", "Taylor", "Dakota", "Ariel" };

		public Page NextPage = null;
		protected bool m_bCloseOnExit = false;

		protected bool m_bInit = false;
		protected Grid m_Grid;
		protected TextBox KeyBox;
		public RideBase()
		{
		}

		List<RM1.Trainer> m_OnPadList = new List<RM1.Trainer>();
		protected void TurnOnKeypads()
		{
			TurnOffKeypads();
			foreach (Unit unit in Unit.RiderUnits)
			{
				if (unit.Trainer != null)
				{
					m_OnPadList.Add( unit.Trainer );
					unit.Trainer.OnPadKey += new RM1.TrainerPadKey(OnPadKey);
				}
			}
		}
		protected void TurnOffKeypads()
		{
			foreach (RM1.Trainer t in m_OnPadList)
				t.OnPadKey -= new RM1.TrainerPadKey(OnPadKey);
			m_OnPadList.Clear();
		}



		protected void RideBase_Loaded(Grid maingrid)
		{
			Statistics.Unregistered = false; // Return this to false on any exit.

			Unit.Reset();	// Reset everything... back to its original state.


			foreach (Unit unit in Unit.Active)
			{
				if (unit.Bot != null)
					unit.Bot.Ready = true;
			}

			m_Grid = maingrid;
			KeyBox = new TextBox();
            KeyBox.Cursor = Cursors.Arrow;
			KeyBox.PreviewKeyDown += new KeyEventHandler(Keyboard_KeyDown);
			KeyBox.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(KeyBox_PreviewMouseLeftButtonDown);
			KeyBox.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(KeyBox_PreviewMouseLeftButtonUp);
			KeyBox.PreviewMouseRightButtonDown += new MouseButtonEventHandler(KeyBox_PreviewMouseRightButtonDown);
			KeyBox.PreviewMouseRightButtonUp += new MouseButtonEventHandler(KeyBox_PreviewMouseRightButtonUp);
			KeyBox.Opacity = 0.0;

			m_Grid.Children.Add(KeyBox);
			TurnOnKeypads();
			KeyBox.Focus();

			m_bInit = true;
			AppWin.Note(string.Format("Course: {0}",Unit.Course.FileName));

			AppWin.Instance.OnClose += new AppWin.OnCloseEvent(OnClose);
		}

		protected virtual void OnClose(CancelEventArgs e)
		{
		}


		void KeyBox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			RightButton(false);
		}

		void KeyBox_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			RightButton(true);
		}
		void KeyBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			LeftButton(false);
		}
		void KeyBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			LeftButton(true);
		}

		protected virtual void LeftButton(bool down)
		{
		}
		protected virtual void RightButton(bool down)
		{
		}


		protected void RideBase_Unloaded()
		{
			Statistics.Unregistered = false; // Return this to false on any exit.
			TurnOffKeypads();
			AppWin.Instance.OnClose -= new AppWin.OnCloseEvent(OnClose);
			if (m_bCloseOnExit)
				AppWin.Instance.DeferExit();

		}

		protected void SelectMasterPad()
		{
			foreach (Unit unit in Unit.Units)
				unit.MasterPad = false;
			if (Unit.RiderUnits.Count > 0)
				Unit.RiderUnits[0].MasterPad = true;
		}

		protected bool m_bBlockKeys = false;
		protected virtual void OnPadKey(RM1.Trainer trainer, RM1.PadKeys key, double pressed)
		{
			if (!m_bInit || m_bBlockKeys || pressed != 0)
				return;
			OnDirectKey(Unit.GetUnit(trainer), key);
		}
		protected virtual void OnDirectKey(Unit unit, RM1.PadKeys key)
		{
			switch (key)
			{
				case RM1.PadKeys.F1: 
					if (Unit.FirstMasterUnit == unit)
						StartOrPause(); 
					break;
				case RM1.PadKeys.F2:
					Scroll(unit,1); 
					break;
				case RM1.PadKeys.F3:
					if (OverrideF3(unit))
						break;
					if (Unit.State == Statistics.State.Paused && unit.Trainer != null)
					{
						unit.Trainer.CalibrateMode = !unit.Trainer.CalibrateMode;
						break;
					}
					Scroll(unit,-1); break;
				case RM1.PadKeys.F4:
					F4Key(unit);
					break;
				case RM1.PadKeys.F5:
					if (Unit.FirstMasterUnit == unit)
						Reset(); 
					break;
				case RM1.PadKeys.FN_UP:
					FNArrow(unit,1);
					break;
				case RM1.PadKeys.FN_DOWN:
					FNArrow(unit,-1);
					break;

				//case RM1.PadKeys.UP: AddSpeed(0, 1); break;
				//case RM1.PadKeys.DOWN: AddSpeed(0, -1); break;
				//case RM1.PadKeys.FN_UP: SetReal(0, true); break;
				//case RM1.PadKeys.FN_DOWN: SetReal(0, false); break;
			}
		}
		protected virtual void F4Key(Unit unit)
		{
		}
		protected virtual void FNArrow(Unit unit,int dir)
		{
		}
		protected virtual bool ScreenChange(Unit unit)
		{
			return false;
		}
		protected virtual bool OverrideF3(Unit unit)
		{
			return false;
		}

		protected virtual void Keyboard_KeyDown(object sender, KeyEventArgs e)
		{
			if (!m_bInit || m_bBlockKeys)
				return;

			switch (e.Key)
			{
				//case Key.Space: break;
				//case Key.A: AddRider(); break;
				//case Key.T: ChangeModel(); break;
				//case Key.V: AddView(); break;
				//case Key.C: break; // Change camera
				//case Key.I: ShowInfo(); break;
                //case Key.F: NextCameraRider(); break;
				//case Key.H: Overlays.Visibility = Overlays.Visibility != Visibility.Visible ? Visibility.Visible : Visibility.Collapsed; break;

				//case Key.D1: if (Keyboard.Modifiers == ModifierKeys.Control) SetNumViews(1); else SetRiderViews(1); break;
				//case Key.D2:
				//	if (m_Statistics2 != null)
				//		SetNumViews(2);
				//	else if (Keyboard.Modifiers == ModifierKeys.Control) SetNumViews(2);
				//	else SetRiderViews(2);
				//	break;
				//case Key.D3: if (Keyboard.Modifiers == ModifierKeys.Control) SetNumViews(3); else SetRiderViews(3); break;
				//case Key.D4: if (Keyboard.Modifiers == ModifierKeys.Control) SetNumViews(4); else SetRiderViews(4); break;
				//case Key.D5: if (Keyboard.Modifiers == ModifierKeys.Control) SetNumViews(5); else SetRiderViews(5); break;
				//case Key.D6: if (Keyboard.Modifiers == ModifierKeys.Control) SetNumViews(6); else SetRiderViews(6); break;
				//case Key.D7: if (Keyboard.Modifiers == ModifierKeys.Control) SetNumViews(7); else SetRiderViews(7); break;
				//case Key.D8: if (Keyboard.Modifiers == ModifierKeys.Control) SetNumViews(8); else SetRiderViews(8); break;
				case Key.D1: SelectUnit(0); break;
				case Key.D2: SelectUnit(1); break;
				case Key.D3: SelectUnit(2); break;
				case Key.D4: SelectUnit(3); break;
				case Key.D5: SelectUnit(4); break;
				case Key.D6: SelectUnit(5); break;
				case Key.D7: SelectUnit(6); break;
				case Key.D8: SelectUnit(7); break;

				case Key.F7: Unit.AllowDrafting = !Unit.AllowDrafting; break;

                case Key.V: OnDirectKey(Unit.FirstMasterUnit, RM1.PadKeys.F2_Long); break; //ChangeView(); break;
				case Key.G: OnDirectKey(Unit.FirstMasterUnit, RM1.PadKeys.F1); break;
				case Key.S: OnDirectKey(Unit.FirstMasterUnit, RM1.PadKeys.F2); break;
				case Key.R: OnDirectKey(Unit.FirstMasterUnit, RM1.PadKeys.F5); break;
				case Key.Up: 
					OnDirectKey(SelectedUnit, Keyboard.Modifiers == ModifierKeys.Control ? RM1.PadKeys.FN_UP : RM1.PadKeys.UP);
					break;
				case Key.Down: 
					OnDirectKey(SelectedUnit, Keyboard.Modifiers == ModifierKeys.Control ? RM1.PadKeys.FN_DOWN : RM1.PadKeys.DOWN);
					break;
				case Key.A:
					if (IsDemo)
						AddDemoRider();
					else
						OnDirectKey(Unit.FirstMasterUnit, RM1.PadKeys.F3); 
					break;

				// DEMO MODE STUFF
				case Key.D: ToggleDemo(); break;
				case Key.OemComma: DecreaseSomething(); break;
				case Key.OemPeriod: IncreaseSomething(); break;
				case Key.Oem3:
					if (Keyboard.Modifiers != ModifierKeys.Shift)
						break;
					if (Window_Log.Instance.IsVisible)
						Window_Log.Instance.Hide();
					else
						Window_Log.Instance.Show();
					break;
			}
			e.Handled = true;
		}
		protected virtual void ToggleDemo()
		{
		}
		protected virtual bool IsDemo { get { return false; } }
		protected virtual void AddDemoRider() { } 

		protected int m_Selection = 0;
		protected virtual void SelectUnit(int num)
		{
			m_Selection = num;
		}
		protected virtual Unit SelectedUnit
		{
			get { return m_Selection >= Unit.RaceUnit.Count ? Unit.FirstMasterUnit : Unit.RaceUnit[m_Selection]; }
		}





		protected virtual void StartOrPause()  {
			switch (Unit.State)  {
				case Statistics.State.Paused:
					Log.WriteLine("RideBase UnPause");
					UnPause();
                    break;
				case Statistics.State.Stopped:
					Log.WriteLine("/Pages/Modes/RideBase.cs, RideBase:StartOrPause()");
					Start();
                    break;
				case Statistics.State.Running:
					Log.WriteLine("Ride Base Pause");
					Pause();
                    break;
			}
		}
		protected virtual void ChangeView()
		{
		}
		protected virtual void Scroll(Unit unit, int dir)
		{
		}
		protected virtual void Start()
		{
			Unit.Start();
		}

		bool m_bBackCalled = false;
		protected void Back()
		{
			if (!m_bBackCalled)
			{
				NavigationService.GoBack();
				m_bBackCalled = true;
			}
		}

		protected virtual void Reset()
		{
			if (!Unit.HasStarted)
			{
				if (NextPage != null)
					NavigationService.Navigate(NextPage);
				Back();
			}
			else
			{
				Unit.Stop();
				if (AllFinished())
					SavePerformance();
				Unit.Reset();
			}
		}
		protected virtual void Pause()
		{
            //Debug.WriteLine("calling in the native Pause");
            Unit.Pause();
		}
		protected virtual void UnPause()
		{
           // Debug.WriteLine("calling in the native UnPause");
            Unit.ClearCalibration();
            Unit.UnPause();
		}
		protected virtual void IncreaseSomething()
		{
		}
		protected virtual void DecreaseSomething()
		{
		}

        protected virtual void NextCameraRider()
        {
        }


		protected virtual bool AllFinished()
		{
			foreach (Unit unit in Unit.RiderUnits)
			{
				PerformanceBot pb = unit.Bot as PerformanceBot;
				if (pb == null && !unit.Statistics.Finished)
					return false;
			}
			return true;
		}

		protected virtual void SavePerformance()
		{
			/*
			Unit.Stop(); // Stop everything if we haven't already
            if (RM1_Settings.General.ExportSave || RM1_Settings.General.ExportPrompt || RM1_Settings.General.ReportPrompt)
            {
                RacerMateOne.Dialogs.SavePerformance sp = new RacerMateOne.Dialogs.SavePerformance("");
                //Pause();
				m_bBlockKeys = true;
                sp.ShowDialog();
				m_bBlockKeys = false;
                //UnPause();
            }
            UnPause();
			*/
        }

	}
}


/*
	public static class PerfSave
	{
		class SaveInfo
		{
			public Perf Performance;
			public bool Export;
			public bool Report;
			public bool App;
		}
		static LinkedList<SaveInfo> ms_List = new LinkedList<SaveInfo>();
		static Mutex ms_Mux = new Mutex();
		static Thread ms_Thread;


		public static void Save(Perf perf, bool export, bool report, bool app)
		{
			SaveInfo sinfo = new SaveInfo();
			sinfo.Performance = perf;
			sinfo.Export = export;
			sinfo.Report = report;
			sinfo.App = app;

			ms_Mux.WaitOne();
			try
			{
				ms_List.AddLast(sinfo);
				if (ms_Thread == null)
				{
					ms_Thread = new Thread(new ThreadStart(ThreadProc));
					ms_Thread.Start();
				}
			}
			catch { }
			ms_Mux.ReleaseMutex();

		}

		static void ThreadProc()
		{
			bool keepgoing = true;
			SaveInfo sinfo;
			while (true)
			{
				sinfo = null;
				ms_Mux.WaitOne();
				try
				{
					sinfo = ms_List.Last();
					if (sinfo != null)
						ms_List.RemoveLast();
					else
						ms_Thread = null;
				}
				catch { sinfo = null; }
				ms_Mux.ReleaseMutex();
				if (sinfo == null)
					break;



				PerfContainer.SnapShotEnd(this, ref thisUnit, reportCols, bSavePerf);
				// Export here
				bool bExport = true;
				PerfContainer.Export(this, ref thisUnit, reportCols, bExport);
				// Save report here
				bool bSaveReport = true;
				PerfContainer.SaveReport(this, ref thisUnit, reportCols, bSaveReport);
				// Disable saving until started again next time 
				PerfContainer.started = false;

			} 
		}
*/