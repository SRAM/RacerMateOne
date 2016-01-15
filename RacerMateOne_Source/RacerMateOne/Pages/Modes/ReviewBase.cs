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
using System.ComponentModel;
using System.Threading; // CancelEventArgs
using System.Diagnostics;


namespace RacerMateOne.Pages.Modes
{
	public partial class ReviewBase: Page
	{
		public ReviewBase()
		{
		}
		~ReviewBase()
		{
			//Debug.WriteLine("ReviewBase deleteing");
			Performance = null;	 // Just make sure this is released
		}


		//============================================================
		static Dictionary<String, bool> ms_SavedState = new Dictionary<String, bool>();
		public static bool GetSavedState(String str)
		{
			bool b;
			if (!ms_SavedState.TryGetValue(str, out b))
			{
				ms_SavedState[str] = true;
				b = true;
			}
			return b;
		}
		public static void SetSavedState(String str, bool state)
		{
			ms_SavedState[str] = state;
		}
		//============================================================

		protected Unit m_Unit;

		protected bool m_bBlockKeys;

		protected Course m_Course;
		protected Course.Location m_BikeLoc;

		public Course Course;
		public Perf Performance;

		protected bool m_bInit = false;
		protected Grid m_Grid;
		protected TextBox KeyBox;

		protected double m_StartSeconds;
		protected double m_EndSeconds;
		protected double m_MaxSeconds;
		protected double m_EndX;

        protected double courseshiftX =0;

        protected class LapInfo
		{
			public double Total;
			public double Time;
			public double Best;
			public int BestLap;
		}
		protected LapInfo[] LapArr;


		List<RM1.Trainer> m_OnPadList = new List<RM1.Trainer>();
		protected void TurnOnKeypads()
		{
			TurnOffKeypads();
			if (m_Unit.Trainer != null)
			{
				m_Unit.Trainer.OnPadKey += new RM1.TrainerPadKey(OnPadKey);
				m_OnPadList.Add(m_Unit.Trainer);
			}
		}
		protected void TurnOffKeypads()
		{
			foreach (RM1.Trainer t in m_OnPadList)
				t.OnPadKey -= new RM1.TrainerPadKey(OnPadKey);
			m_OnPadList.Clear();
		}

		// Special PRE-CALLBACK for loading screen.
		bool m_LoaderScreen;
		public void LoadPerformance(object sender, DoWorkEventArgs e)
		{
            App.SetDefaultCulture();

            BackgroundWorker bw = sender as BackgroundWorker;
			Performance = new Perf();
			Performance.LoadRawTemps(bw, Course.FileName);
			Staging.CachedPerformance = Performance;
			Staging.CachedPerformanceName = Course.FileName;
			m_LoaderScreen = true;
		}





		protected void RideBase_Loaded(Grid maingrid)
		{
			if (m_LoaderScreen)
				NavigationService.RemoveBackEntry();
			Unit.Reset();	// Reset everything... back to its original state.
			m_Unit.Statistics.Hold = true;
			foreach (Unit unit in Unit.Active)
			{
				if (unit.Bot != null)
					unit.Bot.Ready = true;
			}
			Unit.Laps = Course.Laps;

			m_Grid = maingrid;
			KeyBox = new TextBox();
			KeyBox.PreviewKeyDown += new KeyEventHandler(Keyboard_KeyDown);
			KeyBox.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(KeyBox_PreviewMouseLeftButtonDown);
			KeyBox.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(KeyBox_PreviewMouseLeftButtonUp);
			KeyBox.PreviewMouseRightButtonDown += new MouseButtonEventHandler(KeyBox_PreviewMouseRightButtonDown);
			KeyBox.PreviewMouseRightButtonUp += new MouseButtonEventHandler(KeyBox_PreviewMouseRightButtonUp);
			KeyBox.Opacity = 0.0;

			m_Grid.Children.Add(KeyBox);
			TurnOnKeypads();
			KeyBox.Focus();

			if (Performance == null)
			{
				Performance = new Perf();
				Performance.LoadRawTemps(Course.FileName);
			}
			Course.Laps = Performance.CourseInfo.Laps;
			if (Course.StartAt != 0 || Course.EndAt != Course.TotalX)
			{
				m_Course = new Course(Course, false, false, true, Course.StartAt, Course.EndAt);
				m_Course.Laps = Course.Laps;
               //added here to fix the presentation problem for RCV
                courseshiftX  = Course.StartAt - m_Course.StartAt;
                Course.EndAt = Course.EndAt - courseshiftX;
                Course.StartAt=0;
			}
			else
				m_Course = Course;
			m_bInit = true;
			AppWin.Note(string.Format("Course: {0}", Course.FileName));
			Seconds = 0;

			TurnOnKeypads();
			AppWin.AddRenderUpdate( new AppWin.RenderUpdate(DoUpdate), 0 );


			XLoc = Course.StartAt;
			m_StartSeconds = Seconds;
			int laps = Course.Laps < 1 ? 0 : Course.Laps - 1;
			XLoc = Course.EndAt + laps * m_Course.TotalX;
			m_EndSeconds = Seconds;

			m_MaxSeconds = Course.PerformanceInfo.TimeMS / 1000.0;
			Seconds = m_MaxSeconds;
			m_EndX = XLoc;
            //m_EndSeconds = Seconds;

			int maxlaps = Course.Laps < 1 ? 1 : Course.Laps;

			LapArr = new LapInfo[maxlaps + 1];
			int n, bestlap = 0;;
			for(n = 0;n < maxlaps+1;n++)
				LapArr[n] = new LapInfo();
			n = 0;
			double x = 0,last = 0,best = 0;
			foreach (LapInfo li in LapArr)
			{
				XLoc = x;
				li.Total = Seconds;
				li.Time = li.Total - last;
				if (n == 1 || li.Time < best)
				{
					bestlap = n;
					best = li.Time;
				}
				li.Best = best;
				li.BestLap = bestlap;
				last = li.Total;
				x += Course.TotalX;
				n++;
			}
			Seconds = 0;

			if (Course.EndAt == Course.TotalX && Course.StartAt == 0)
				Performance.ClearRecalc();
			else
				Performance.Recalc(m_StartSeconds, m_EndSeconds);
                
		}

		protected void RideBase_Unloaded()
		{
			foreach(Unit unit in Unit.Units)
				m_Unit.Statistics.Hold = false;
			AppWin.RemoveRenderUpdate( new AppWin.RenderUpdate(DoUpdate));
			TurnOffKeypads();

			// NULL out stuff to help with garbage collection.
			Performance = null;
            // if I shifted the course I want to restore it
            if (courseshiftX != 0)
            {
                Course.StartAt += courseshiftX;
                Course.EndAt += courseshiftX;
            }
            //terrible kludge

			Course = null;
			m_Course = null;
			GC.Collect();
			m_bInit = false;
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

		protected virtual void OnPadKey(RM1.Trainer trainer, RM1.PadKeys key, double pressed)
		{
			if (!m_bInit || pressed != 0 || m_bBlockKeys)
				return;
			OnDirectKey(Unit.GetUnit(trainer), key);
		}
		protected virtual void OnDirectKey(Unit unit, RM1.PadKeys key)
		{
		}

		protected virtual void Keyboard_KeyDown(object sender, KeyEventArgs e)
		{
			if (!m_bInit || m_bBlockKeys)
				return;

			switch (e.Key)
			{
				case Key.V: OnDirectKey(m_Unit, RM1.PadKeys.F2_Long); break; //ChangeView(); break;
				case Key.G: OnDirectKey(m_Unit, RM1.PadKeys.F1); break;
				case Key.S: OnDirectKey(m_Unit, RM1.PadKeys.F2); break;
				case Key.R: OnDirectKey(m_Unit, RM1.PadKeys.F5); break;
				case Key.Up:
					OnDirectKey(m_Unit, Keyboard.Modifiers == ModifierKeys.Control ? RM1.PadKeys.FN_UP : RM1.PadKeys.UP);
					break;
				case Key.Down:
					OnDirectKey(m_Unit, Keyboard.Modifiers == ModifierKeys.Control ? RM1.PadKeys.FN_DOWN : RM1.PadKeys.DOWN);
					break;
				case Key.A:
					OnDirectKey(m_Unit, RM1.PadKeys.F3);
					break;
				// DEMO MODE STUFF
				//case Key.D: ToggleDemo(); break;
				//case Key.OemComma: DecreaseSomething(); break;
				//case Key.OemPeriod: IncreaseSomething(); break;
				case Key.Oem3:
					if (Keyboard.Modifiers != ModifierKeys.Shift)
						break;
					if (Window_Log.Instance.IsVisible)
						Window_Log.Instance.Hide();
					else
						Window_Log.Instance.Show();
					break;
				case Key.F2:
					OnDirectKey(m_Unit, RM1.PadKeys.F2);
					OnDirectKey(m_Unit, RM1.PadKeys.F2_Short);
					break;
				case Key.F3:
					OnDirectKey(m_Unit, RM1.PadKeys.F3);
					OnDirectKey(m_Unit, RM1.PadKeys.F3_Short);
					break;
				case Key.F4:
					OnDirectKey(m_Unit, RM1.PadKeys.F4);
					OnDirectKey(m_Unit, RM1.PadKeys.F4_Short);
					break;
			}
			e.Handled = true;
		}

		//==============================================================================================
		protected bool m_bRunning;
		public bool Running
		{
			get { return m_bRunning; }
			set
			{
				m_bRunning = value;
			}
		}


		protected virtual bool DoUpdate(double seconds, double split)
		{
			if (m_bRunning)
			{
				Seconds += split;
			}			
			return false;
		}

		public Performance PFrame = new Performance();

		protected int m_FrameNum = 0;
		protected double m_Seconds;
		public double Seconds
		{
			get { return m_Seconds; }
			set
			{
				m_Seconds = value;
				if (Performance != null)
				{
					Performance.GetLoadedFrame(ref PFrame, m_Seconds, StatFlags.Mask);
					//Debug.WriteLine(String.Format("* {0:F2} {0:F3}", m_Seconds, PFrame.Speed * ConvertConst.MetersPerSecondToMPH));
					m_Unit.Statistics.LoadPerfFrame(ref PFrame.cur,Course);
					if (Course.XUnits == CourseXUnits.Distance)
					{
						// Is the distance BEFORE or after the visible area... jump to the next valid area
						if (m_Unit.Statistics.LapDistance > Course.EndAt)
						{
							m_Seconds = Performance.FindDistanceTime(Course.StartAt + Course.TotalX * (m_Unit.Statistics.Lap));
							Performance.GetLoadedFrame(ref PFrame, m_Seconds, StatFlags.Mask);
							m_Unit.Statistics.LoadPerfFrame(ref PFrame.cur,Course);
						}
						else if (m_Unit.Statistics.LapDistance < Course.StartAt)
						{
							m_Seconds = Performance.FindDistanceTime(Course.StartAt + Course.TotalX * (m_Unit.Statistics.Lap-1));
							Performance.GetLoadedFrame(ref PFrame, m_Seconds, StatFlags.Mask);
							m_Unit.Statistics.LoadPerfFrame(ref PFrame.cur,Course);
						}
					}
					m_FrameNum++;
				}
				UpdateSeconds();
			}
		}

		protected virtual void UpdateSeconds()
		{
		}

		public double XLoc
		{
			get
			{
				if (Performance == null)
					return 0;
				if (Course.XUnits == CourseXUnits.Time)
					return m_Seconds;
				return PFrame.cur.Distance;
			}
			set
			{
				if (Performance == null)
					return;
				if (Course.XUnits == CourseXUnits.Time)
					Seconds = value;
				else
					Seconds = Performance.FindDistanceTime(value);
			}
		}

		public double XLocClip
		{
			get { return XLoc; }
			set
			{
				if (Performance == null)
					return;
				int lap = m_Unit.Statistics.Lap;
				double v = value;
				double min = (lap - 1) * Course.TotalX;
				double max = lap * Course.TotalX;
				if (v < min) v = min;
				else if (v >= max) v = max - 0.1;
				if (Course.XUnits == CourseXUnits.Time)
					Seconds = v;
				else
					Seconds = Performance.FindDistanceTime(v);
				int nlap = m_Unit.Statistics.Lap;
				int cnt = 0;
				while (nlap < lap && cnt < 20)
				{
					Seconds += 1.0;
					nlap = m_Unit.Statistics.Lap;
					cnt++;
				}

			}
				
		}

	}
}
