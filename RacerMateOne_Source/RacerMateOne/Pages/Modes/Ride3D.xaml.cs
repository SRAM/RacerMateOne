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
using System.ComponentModel;
using System.Diagnostics;

namespace RacerMateOne.Pages.Modes
{
	/// <summary>
	/// Interaction logic for Ride3D.xaml
	/// </summary>
	public partial class Ride3D : RideBase
	{
		static Controls.StatsArea.Modes[] ms_ModeCycle = new RacerMateOne.Controls.StatsArea.Modes[] {
			Controls.StatsArea.Modes.Simple,
			Controls.StatsArea.Modes.Polar,
			Controls.StatsArea.Modes.Bar,
			Controls.StatsArea.Modes.Stats
			};
		static Controls.StatsArea.Modes[] ms_MultiModeCycle = new RacerMateOne.Controls.StatsArea.Modes[] {
			Controls.StatsArea.Modes.Stats,
			Controls.StatsArea.Modes.Polar,
			Controls.StatsArea.Modes.Bar,
			Controls.StatsArea.Modes.Simple,
			Controls.StatsArea.Modes.Time,
			};
		#pragma warning disable 649
		class Node
		{
			public Unit m_Unit;
			public int Num;
			public int ModeNum = 0;			// 
			public int MultiModeNum = 0;	// When in MultiDisplay3 - 8
			public Controls.StatsArea.Modes CurrentMode = Controls.StatsArea.Modes.Stats;
			public int CameraMode = 0;

			LinkedListNode<Node> m_Node;

			Unit m_BotUnit;
			public Node(Unit unit)
			{
				m_Node = new LinkedListNode<Node>(this);
				m_Unit = unit;
			}

			public bool NeedWindowed
			{
				get 
				{
					return (CurrentMode == Controls.StatsArea.Modes.Polar || CurrentMode == Controls.StatsArea.Modes.Bar);
				}
			}

			public void SelectBot(Ride3D r)
			{
				// Yes... Find the next botunit.
				if (m_Cur != null || m_BotUnit == null)
				{
					int num = m_BotUnit == null ? m_Unit.Number : m_BotUnit.Number;
					for (int i = m_BotUnit == null ? 0 : 1; i < 8; i++)
					{
						num++;
						if (num >= 8) num = 0;
						Unit u = Unit.Units[num];
						if (u.IsActive && u.IsBot && u.Bot.ControlUnit == m_Unit)
						{
							Off(r);
							m_BotUnit = u;
							break;
						}
					}
				}
				// 
				On(r);
			}

			Controls.RaceCourseDisplay m_Cur;
			public void Off(Ride3D r)
			{
				if (m_Cur != null)
				{
					m_Cur.SetMessage( m_BotUnit, null );
					m_Cur = null;
				}
			}
			double m_HideTime;
			public void On(Ride3D r)
			{
				if (m_BotUnit == null)
				{
					Off(r);
					return;
				}
				m_Cur = r.m_CurCourseDisplay;
				if (m_Cur != null && m_BotUnit != null && m_BotUnit.IsBot)
				{
					m_Cur.SetMessage(m_BotUnit, m_BotUnit.Bot.DisplayText);
					m_HideTime = r.m_LastRenderTime + 5.0;
				}
			}
			public void Update(Ride3D r)
			{
				if (m_Cur != null && r.m_LastRenderTime >= m_HideTime)
					Off(r);
			}
			public void Adjust(Ride3D r, int dir)
			{
				if (m_BotUnit == null)
					SelectBot(r);

				if (m_BotUnit != null && m_BotUnit.IsBot)
				{
					m_BotUnit.Bot.Adjust(dir);
					On(r);
					r.m_Touched = m_Unit;
				}
			}


		};													// class Node


		#pragma warning restore 649
		Node[] m_Nodes = new Node[8];

		/********************************************************************************************************

		********************************************************************************************************/

		void UpdateNodes()
		{
			foreach (Unit u in Unit.RiderUnits)
				m_Nodes[u.Number].Update(this);
		}


		//=================================================================================


		bool m_IndividualWindows;

		AppWin m_App;
		/********************************************************************************************************

		********************************************************************************************************/

		public Ride3D()  {
#if DEBUG
			Log.WriteLine("Ride3D.xaml.cs, Ride3D::Ride3D(), constructor");
#endif

			InitializeComponent();
			if (!AppWin.IsInDesignMode)
				Background = Brushes.Transparent;
			m_App = AppWin.Instance;
            
			//for(int i=0;i<8;i++)
			//	m_NameNodes[i] = new NameNode();
		}


		/********************************************************************************************************

		********************************************************************************************************/

		Controls.RaceCourseDisplay m_CurCourseDisplay;

		ReportColumns m_DisplayColumns;
		StatFlags m_DisplayStatFlags;
		private void Page_Loaded(object sender, RoutedEventArgs e)  {
#if DEBUG
			Log.WriteLine("Ride3D.xaml.cs, Ride3D::Page_Loaded");
#endif

            // Init3D
            m_App.MainRender3D.Init();
           // Debug.WriteLine("3d has been initialized, should be visible");
            if (m_LoaderScreen)
            {
                NavigationService.RemoveBackEntry();
                m_LoaderScreen = false;
            }
            this.Opacity = 1;
            MainGrid.Children.Remove(DlgGrid);
			Dlg.Content = DlgGrid;
			Dlg.FadeTo = 0.0;

			// Determin what mode we will be using
			int i;
			for(i=0;i<8;i++)
				m_Nodes[i] = new Node(Unit.Units[i]);
			//Course1.BikeIcon = BikeIcon1;

			m_DisplayColumns = ReportColumns.Display_3DRoadRacing.Selected_ReportColumns;
			m_DisplayStatFlags = m_DisplayColumns.StatFlags;

            Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
            {
				AppWin.AddRenderUpdate(new AppWin.RenderUpdate(RenderUpdate), 0);
				SelectMasterPad();
				Unit.AllocateBots();
				RideBase_Loaded(MainGrid);

				// Move the DlgLayer to the top.  This should be off now.
				MainGrid.Children.Remove(DlgLayer);
				MainGrid.Children.Add(DlgLayer);


				m_App.Render3DOn();	
				RedoNodes();
				Unit.AddNotify(null, m_StatFlags, new Unit.NotifyEvent(OnUpdateUnit));

				StatsUnit = Unit.RiderUnits[0];
            });
			
		}

		/********************************************************************************************************

		********************************************************************************************************/

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
            // TODO - Will check this, Call when unloading to revert all demobots back
            DemoOff();

			Unit.RemoveNotify(null, m_StatFlags, new Unit.NotifyEvent(OnUpdateUnit));
			AppWin.RemoveRenderUpdate(new AppWin.RenderUpdate(RenderUpdate));
			SetMode(Mode.None, true);
			m_App.Render3DOff();
			RideBase_Unloaded();
			Controls.Render3D.CloseRiders();
		}

		/********************************************************************************************************

		********************************************************************************************************/

		Label m_TimeUpdate;
		void OnUpdateUnit(Unit unit_n, StatFlags changed)  {
            if (!m_bInit)
				return;
            //Debug.WriteLine("in update unit " + unit_n.IsBot);
            if ((changed & (StatFlags.Order | StatFlags.RiderName)) != StatFlags.Zero)
			{
			}
			if ((changed & StatFlags.Finished) != StatFlags.Zero)
			{
				if (AllFinished())
					Finished();
			}
			if (m_LapRun != null && (changed & StatFlags.Lap) != StatFlags.Zero)
			{
				UpdateLap(m_StatsUnit);
			}

			if (m_TimeUpdate != null && (changed & StatFlags.Time) != StatFlags.Zero)
			{
				m_TimeUpdate.Content = Statistics.MasterTimerString;
			}
		}

		StatFlags m_StatFlags = StatFlags.Order | StatFlags.RiderName | StatFlags.Finished | StatFlags.Time | StatFlags.Lap;
		double m_StatsTimeout;		// 0 - never, Otherwise compare this to time.
		Unit m_StatsUnit;			// What unit is currently displayed in the multi rider line.
		Unit StatsUnit
		{
			get
			{
				return m_StatsUnit;
			}
			set
			{
				m_StatsUnit = value == null ? Unit.RiderUnits[0] : value;
				UpdateLap(m_StatsUnit);
				if (m_CurCourseDisplay != null)
					m_CurCourseDisplay.ActiveUnit = m_StatsUnit;
				Timer1.Unit = Timer_M.Unit = m_StatsUnit;
				if (m_CurView != null)
				{
					m_CurView.Unit = m_StatsUnit;
                    // make sure there is a valid rider
                    if (m_CurView.Unit != null && m_CurView.Unit.iRider > 0)
						Controls.Render3D.SetCameraRider(m_CurView.Unit, m_Nodes[m_CurView.Unit.Number].CameraMode);
				}
				if (m_CurStatsLine != null)
					m_CurStatsLine.Unit = m_StatsUnit;
					
				m_StatsTimeout = m_LastRenderTime + 10.0;
			}
		}

		Controls.Render3DView m_CurView;
		Controls.StatsLineSmall m_CurStatsLine;

		double m_LastRenderTime;

		Unit m_RiderUnit;			// What unit should be displayed in the rider line when the bot. If a pacer unit times out then goes back
		Unit m_Touched;
		/********************************************************************************************************

		********************************************************************************************************/

		bool RenderUpdate(double seconds, double split)
		{
			m_LastRenderTime = seconds;
			UpdateNodes();
			if (CurrentMode == Mode.MultiPlayer || CurrentMode == Mode.TwoPlayer)
			{
				if (m_StatsTimeout > 0 && seconds > m_StatsTimeout)
				{
					// Find the first rider unit after this one.
					int b = Unit.RaceUnit.IndexOf(m_StatsUnit);
					if (b < 0) {
						b = 0;
					}

					int i = b + 1;

					for (; i != b; i++)  {
						if (i >= Unit.RaceUnit.Count) {
							i = 0;
						}
						if (Unit.RaceUnit[i].IsRiderUnit) {
							break;
						}
					}
					StatsUnit = Unit.RaceUnit[i];
				}
			}
			if (m_BottomResize) {
				MultiBottomResize();
			}

			return false; // We want this to always keep going.
		}																// RenderUpdate()

		/*
		class NameNode
		{
			public Unit			unit;
			public Label		label;
			public TextBlock	name;
			public Label		pos;
			public bool			selected;
			public int			order;
			public NameNode()
			{
				unit = null;
				label = new Label();
				label.FontSize = 16;
				label.MaxWidth = 180;
				label.Padding = new Thickness(5,0,5,0);
				label.VerticalContentAlignment = VerticalAlignment.Center;

				TextBlock outter = new TextBlock();
				label.Content = outter;
				InlineCollection inlines = outter.Inlines;
				name = new TextBlock();
				name.TextTrimming = TextTrimming.CharacterEllipsis;
				inlines.Add(name);

				pos = new Label();
				pos.FontSize = 18;
				pos.MaxWidth = 180;
				pos.Padding = new Thickness(0,0,5,0);
				pos.VerticalContentAlignment = VerticalAlignment.Center;
				order = -1;
			}
		}
		 */
		//NameNode[] m_NameNodes = new NameNode[8];


		// These are used in decision only ... Since demo mode tree
		private int m_RiderCount;
		private int m_BotCount;
		private int m_TotalCount;
		Run m_LapRun = null;

		/********************************************************************************************************

		********************************************************************************************************/

		private void RedoNodes()
		{
			// How many live riders are there?
			m_RiderCount = Unit.RiderUnits.Count;
			m_BotCount = Unit.BotUnits.Count;
			m_TotalCount = Unit.RaceUnit.Count;
			if (m_RiderUnit != null && !Unit.RiderUnits.Contains(m_RiderUnit))
				m_RiderUnit = null;
			if (m_StatsUnit != null && !Unit.RaceUnit.Contains(m_StatsUnit))
				StatsUnit = null;
			if (m_RiderUnit == null)
				m_RiderUnit = Unit.RiderUnits.Count > 0 ? Unit.RiderUnits.First() : Unit.RaceUnit.First();
			if (m_StatsUnit == null)
			{
				StatsUnit = Unit.RiderUnits.First();
				m_StatsTimeout = 0;
			}

			// Figure out the mode
			Mode rmode;
			if (m_RiderCount <= 1 && m_TotalCount <= 2)
			{
				rmode = Mode.OnePlayer;
			}
			else
			{
				// Now we can decide what mode based on that information.
				rmode = Mode.MultiPlayer;
				if (m_RiderCount == 2) // && m_TotalCount == 2)
					rmode = m_IndividualWindows ? Mode.MultiDisplay2: Mode.TwoPlayer;
				else if (m_IndividualWindows)
				{
					switch(m_RiderCount)
					{
						case 2:
							rmode = Mode.MultiDisplay2; break;
						case 3:
							rmode = Mode.MultiDisplay3; break;
						case 4:
							rmode = Mode.MultiDisplay4; break;
						case 5:
						case 6:
							rmode = Mode.MultiDisplay6; break;
						case 7:
						case 8: 
						default: 
							rmode = Mode.MultiDisplay8; break;
					}
				}
			}
			SetMode( rmode, true );
		}													// RedoNodes


		/********************************************************************************************************

		********************************************************************************************************/

		Mode CurrentMode = Mode.None;
		private void SetMode(Mode mode) { SetMode(mode, false); }
		private void SetMode(Mode mode, bool force)
		{
			if (CurrentMode == mode && !force)
				return;

			switch (CurrentMode)
			{
				case Mode.None: break;
				case Mode.OnePlayer:
					OnePlayer.Visibility = Visibility.Hidden;
					SpinScan1.Unit = null;
					StatsBox1.Unit = null;
					SpinScanBox1.Unit = null;

					Timer1.Unit = null;
					//Course1.Unit = null;
					Stats1.Unit = null;
					if (Stats1b.Visibility == Visibility.Visible)
					{
						Stats1b.Visibility = Visibility.Collapsed;
						Stats1b.SetUnit(0, null);
						Stats1b.SetUnit(1, null);
						Course1b.Visibility = Visibility.Collapsed;
						Course1b.Course = null;
						Course1b.LoadUnits();
					}
					//InfoLine1.Unit = null;

					m_App.U_1.Visibility = Visibility.Hidden;
					m_App.R_1_0.Unit = null;
					break;
				case Mode.TwoPlayer:
				case Mode.TwoPlayerSplit:
					TwoPlayer.Visibility = Visibility.Hidden;
					//InfoLine2_1.Unit = 
					Timer2_1.Unit = SpinScan2_1.Unit = SpinScanBox2_1.Unit = null;
					//InfoLine2_2.Unit = 
					Timer2_2.Unit = SpinScan2_2.Unit = SpinScanBox2_2.Unit = null;
					StatsBox2_2.Unit = null;
					StatsBox2_1.Unit = null;

					Stats2.SetUnit(0, null);
					Stats2.SetUnit(1, null);

					if (CurrentMode == Mode.TwoPlayer)
					{
						m_App.U_1.Visibility = Visibility.Hidden;
						m_App.R_1_0.Unit = null;
						Controls.Render3D.CameraMode = Controls.CameraGameMode.Normal;
					}
					else
					{
						m_App.U_2.Visibility = Visibility.Hidden;
						m_App.R_2_0.Unit = null;
						m_App.R_2_1.Unit = null;
					}

					CourseDisplay2.Course = null;
					break;
				case Mode.MultiPlayer:
					MultiPlayer.Visibility = Visibility.Hidden;
					//InfoLine_M.DisplayFlags = m_DisplayStatFlags;
					//InfoLine_M.Unit = 
					Timer_M.Unit = Stats_M.Unit = SpinScan_M.Unit = SpinScanBox_M.Unit = null;
					StatsBox_M.Unit = null;
					m_App.U_1.Visibility = Visibility.Hidden;
					m_App.R_1_0.Unit = null;
					Controls.Render3D.CameraMode = Controls.CameraGameMode.Normal;
					break;
				case Mode.MultiDisplay2:
				case Mode.MultiDisplay3:
				case Mode.MultiDisplay4:
				case Mode.MultiDisplay6:
				case Mode.MultiDisplay8:
					ClearMultiDisplay();
					break;
			}										// switch(CurrentMode)

			m_TimeUpdate = null;
			m_CurCourseDisplay = null;

			m_CurView = null;	// The cycleing view.
			m_CurStatsLine = null;

			CurrentMode = mode;
			Unit unit;
			m_AdjustRows.Clear();
			int cnt = 0;
			foreach (Unit u in Unit.RiderUnits)
			{
				m_Nodes[u.Number].Num = cnt++;
			}

			m_LapRun = null;

			switch (mode)
			{
				case Mode.None: break;
				case Mode.OnePlayer:
					OnePlayer.Visibility = Visibility.Visible;
					m_TimeUpdate = OneTimer;

                    if (1 > Unit.RaceUnit.Count)
                    {
                        MessageBoxResult result = MessageBox.Show("Not enough unit available.",
                            "RaceUnit error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }

                    //unit = Unit.RaceUnit[0];
                    unit = Unit.RiderUnits[0];

                    // Prepair the mode
					SpinScan1.Unit = unit;
					StatsBox1.Unit = unit;
					SpinScanBox1.Unit = unit;
					m_LapRun = Unit.Course.LengthFillTextBlock(Length1, Unit.Laps);
					CourseName1.Text = Unit.Course.Name;
					StatsUnit = unit;
					UpdateLap(unit);

					Timer1.ShowFinal = false;
					Timer1.Unit = unit;
					if (m_TotalCount > 1)
					{
						Stats1.Visibility = Visibility.Collapsed;
						Stats1b.Visibility = Visibility.Visible;
						Stats1b.SetUnit(0, unit);
						Stats1b.SetUnit(1, Unit.BotUnits[0]);
						Stats1b.StatFlags = (StatFlags.RiderName | m_DisplayStatFlags | StatFlags.Drafting) & ~(StatFlags.Zero);

						Course1b.Visibility = Visibility.Visible;
						Course1b.Course = Unit.Course;
						m_CurCourseDisplay = Course1b;
					}
					else
					{
						Course1b.Course = Unit.Course;
						Course1b.Visibility = Visibility.Visible;
						Stats1.StatFlags = m_DisplayStatFlags & ~(StatFlags.Lead);
						Stats1.Unit = unit;
						Stats1.Visibility = Visibility.Visible;
					}
					//InfoLine1.DisplayFlags = m_DisplayStatFlags;
					//InfoLine1.Unit = unit;

					m_App.U_1.Visibility = Visibility.Visible;
					m_App.R_1_0.Unit = unit;
					break;
				case Mode.TwoPlayer:
				case Mode.TwoPlayerSplit:
					TwoPlayer.Visibility = Visibility.Visible;
					m_TimeUpdate = TwoTimer;


                    if (2 > Unit.RaceUnit.Count)
                    {
                        MessageBoxResult result = MessageBox.Show("Not enough unit available.",
                            "RaceUnit error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
					
					unit = Unit.RaceUnit[0];
					//InfoLine2_1.DisplayFlags = m_DisplayStatFlags;
					//InfoLine2_1.Unit = 
					Timer2_1.Unit = SpinScan2_1.Unit = SpinScanBox2_1.Unit = unit;
					StatsBox2_2.Unit = unit;

					unit = Unit.RaceUnit[1];

					//InfoLine2_2.DisplayFlags = m_DisplayStatFlags;
					//InfoLine2_2.Unit = 
					Timer2_2.Unit = SpinScan2_2.Unit = SpinScanBox2_2.Unit = unit;
					StatsBox2_1.Unit = unit;

					Stats2.StatFlags = (m_DisplayStatFlags | StatFlags.RiderName | StatFlags.Drafting) & ~(StatFlags.Zero);
					Stats2.SetUnit(0, Unit.RaceUnit[0]);
					Stats2.SetUnit(1, Unit.RaceUnit[1]);

					CourseDisplay2.Course = Unit.Course;
					CourseDisplay2.LoadUnits();
					m_CurCourseDisplay = CourseDisplay2;

					if (mode == Mode.TwoPlayer)
					{
						m_App.U_1.Visibility = Visibility.Visible;
						m_App.R_1_0.Unit = m_StatsUnit;
						m_CurView = m_App.R_1_0;
						Controls.Render3D.CameraMode = Controls.CameraGameMode.Normal;
						SpinScanDisplay2_1.Visibility = SpinScan2_1.Visibility = Visibility.Collapsed;
						SpinScanDisplay2_2.Visibility = SpinScan2_2.Visibility = Visibility.Collapsed;
					}
					else
					{

						m_App.U_2.Visibility = Visibility.Visible;
						m_App.R_2_0.Unit = Unit.RaceUnit[0];
						m_App.R_2_1.Unit = Unit.RaceUnit[1];
						SpinScanDisplay2_1.Visibility = SpinScan2_1.Visibility = Visibility.Visible;
						SpinScanDisplay2_2.Visibility = SpinScan2_2.Visibility = Visibility.Visible;
					}
					Unit.Course.LengthFillTextBlock(Length2, Unit.Laps);
					CourseName2.Text = Unit.Course.Name;
					break;
				case Mode.MultiPlayer:

                    if (1 > Unit.RaceUnit.Count)
                    {
                        MessageBoxResult result = MessageBox.Show("Not enough unit available.",
                            "RaceUnit error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
					m_TimeUpdate = MTimer;

					unit = m_StatsUnit;
					MultiPlayer.Visibility = Visibility.Visible;
					Stats_M.StatFlags = (m_DisplayStatFlags | StatFlags.RiderName | StatFlags.Drafting) & ~(StatFlags.Zero);
					//InfoLine_M.Unit =
					Timer_M.Unit = Stats_M.Unit = SpinScan_M.Unit = SpinScanBox_M.Unit = unit;
					CourseDisplay_M.Course = Unit.Course;
					CourseDisplay_M.LoadUnits();
					m_CurCourseDisplay = CourseDisplay_M;
					StatsBox_M.Unit = unit;

					m_App.U_1.Visibility = Visibility.Visible;
					m_App.R_1_0.Unit = unit;
					m_CurView = m_App.R_1_0;
					m_CurStatsLine = Stats_M;
					Unit.Course.LengthFillTextBlock(LengthM, Unit.Laps);
					CourseNameM.Text = Unit.Course.Name;
					Controls.Render3D.CameraMode = Controls.CameraGameMode.Normal;
					break;
				case Mode.MultiDisplay2:
					m_AdjustRows.Add(MultiDisplayRow);
					m_AdjustRows.Add(U_2a_r);
					m_AdjustRows.Add(AppWin.Instance.U_2a_r);
					SetupMultiDisplay(U_2a, AppWin.Instance.U_2a);
					break;
				case Mode.MultiDisplay3:
					m_AdjustRows.Add(MultiDisplayRow);
					m_AdjustRows.Add(U_3_r);
					m_AdjustRows.Add(AppWin.Instance.U_3_r);
					SetupMultiDisplay(U_3, AppWin.Instance.U_3);
					break;
				case Mode.MultiDisplay4:
					m_AdjustRows.Add(MultiDisplayRow);
					m_AdjustRows.Add(U_4_r);
					m_AdjustRows.Add(AppWin.Instance.U_4_r);
					SetupMultiDisplay(U_4, AppWin.Instance.U_4);
					break;
				case Mode.MultiDisplay6:
					m_AdjustRows.Add(MultiDisplayRow);
					m_AdjustRows.Add(U_8_r);
					m_AdjustRows.Add(AppWin.Instance.U_8_r);
					SetupMultiDisplay(U_6, AppWin.Instance.U_6);
					break;
				case Mode.MultiDisplay8:
					m_AdjustRows.Add(MultiDisplayRow);
					m_AdjustRows.Add(U_8_r);
					m_AdjustRows.Add(AppWin.Instance.U_8_r);
					SetupMultiDisplay(U_8, AppWin.Instance.U_8);
					break;
			}
			foreach (Unit u in Unit.RiderUnits)
				UpdateMode(m_Nodes[u.Number]);
			StatsUnit = m_StatsUnit; // Redo this one.
			Controls.Render3D.SetupRiders();
			if (m_CurCourseDisplay != null)
				m_CurCourseDisplay.RedoOn();
		}																// SetMode()

		List<RowDefinition> m_AdjustRows = new List<RowDefinition>();
		/*
		private void UnitCycle_OnUnitChanged(object sender, RoutedEventArgs e)
		{
			if (CurrentMode == Mode.MultiPlayer)
			{
				Unit unit = Cycle.Unit;
				Timer_M.Unit = Stats_M.Unit = InfoLine_M.Unit = SpinScan_M.Unit = SpinScanBox_M.Unit = unit;
				m_App.R_1_0.Unit = unit;
				CourseDisplay_M.ActiveUnit = unit;
				UpdateMode(m_Nodes[unit.RaceUnitNumber]);
			}

		}
		 */

		/********************************************************************************************************

		********************************************************************************************************/

		void CloseCurCtrl()
		{
			// Turn off the indivistual stats controls.
			if (m_CurCtrl != null)
			{
				foreach (Controls.StatsArea s in m_SAList)
					s.Unit = null;
				foreach (UIElement e in m_CurCtrl.Children)
				{
					Grid g = e as Grid;
					if (g != null)
						g.Children.Clear();
				}
				m_SAList.Clear();
				m_CurCtrl = null;
			}
		}				// CloseCurCtrl()

		/********************************************************************************************************

		********************************************************************************************************/

		List<Controls.StatsArea> m_SAList = new List<RacerMateOne.Controls.StatsArea>();
		Grid m_CurCtrl;
		Grid m_CurRender;
		int m_CurActiveUnits;

		void ClearMultiDisplay()
		{
			MultiDisplay.Visibility = Visibility.Collapsed;
			MultiDisplayCourse.Course = null;
			m_CurCtrl.Visibility = m_CurRender.Visibility = Visibility.Collapsed;
		}

		/********************************************************************************************************

		********************************************************************************************************/

		void SetupMultiDisplay(Grid ctrl, Grid render)
		{
			Unit.Course.LengthFillTextBlock(LengthMulti, Unit.Laps);
			CourseNameMulti.Text = Unit.Course.Name;
			m_TimeUpdate = TimerMulti;
			int max = m_RiderCount;
			int cnt = 0;
			if ((ctrl != m_CurCtrl || m_CurActiveUnits != max))
			{
				CloseCurCtrl();
				foreach (UIElement e in ctrl.Children)
				{
					Grid g = e as Grid;
					if (g != null)
					{
						Controls.StatsArea sa = new Controls.StatsArea();
						sa.StatsAreaFlags = m_DisplayStatFlags & ~(StatFlags.RiderName);
						sa.Unit = Unit.RiderUnits[cnt++];
						m_SAList.Add(sa);
						g.Children.Add(sa);
						if (cnt >= max)
							break;
					}
				}
				cnt = 0;
				foreach (UIElement e in render.Children)
				{
					Controls.Render3DView r = e as Controls.Render3DView;
					r.Visibility = cnt < max ? Visibility.Visible : Visibility.Hidden;
					r.Unit = cnt < max ? Unit.RiderUnits[cnt] : null;
					cnt++;
				}
				m_CurCtrl = ctrl;
			}
			MultiDisplay.Visibility = Visibility.Visible;
			m_CurRender = render;
			m_CurActiveUnits = max;
			m_CurCtrl.Visibility = m_CurRender.Visibility = Visibility.Visible;
			MultiDisplayCourse.Course = Unit.Course;
			MultiDisplayCourse.LoadUnits();
			m_CurCourseDisplay = MultiDisplayCourse;


			foreach (Unit u in Unit.RiderUnits)
			{
				Node n = m_Nodes[u.Number];
				UpdateMode(n);
			}
			MultiBottomResize();
		}


		/********************************************************************************************************

		********************************************************************************************************/


		bool[] m_Bar = new bool[8];

		void UpdateLap(Unit unit)  {
			if (unit != null && m_LapRun != null)
			{
				int lap = unit.Statistics.Lap;
				m_LapRun.Text = String.Format("{0}/{1}", lap > Unit.Laps ? Unit.Laps : lap, Unit.Laps);
			}
		}


		/********************************************************************************************************

		********************************************************************************************************/

		/// <summary>
		/// Keeps cycling through the stuff or sets the mode based on what we are in.
		/// </summary>
		/// <param name="n"></param>
		void UpdateMode(Node n)
		{
			if (n == null)
				return;


			int num;
			Controls.StatsArea.Modes mode;
			Controls.StatsArea.Modes[] modearr;
			bool multi = (int)CurrentMode >= (int)Mode.MultiDisplay2;

			if (multi)
			{
				modearr = ms_MultiModeCycle;
				num = n.MultiModeNum;
			}
			else if (m_RiderCount <= 2)
			{
				modearr = ms_ModeCycle;
				num = n.ModeNum;
			}
			else
			{
				modearr = ms_ModeCycle;
				num = 0;
			}

			while (num < 0)
				num += modearr.Length;
			while (num >= modearr.Length)
				num -= modearr.Length;

			if (multi)
				n.MultiModeNum = num;
			else
				n.ModeNum = num;

			mode = modearr[num];
			if (CurrentMode == Mode.MultiPlayer && m_RiderCount > 1)
				mode = Controls.StatsArea.Modes.Simple;
			switch (CurrentMode)
			{
				case Mode.OnePlayer:
					switch (mode)
					{
						case Controls.StatsArea.Modes.Polar:
						case Controls.StatsArea.Modes.Bar:
							SpinScan1.Polar = mode == Controls.StatsArea.Modes.Polar;
							SpinScanBox1.Visibility = SpinScan1.Visibility = Visibility.Visible;
							StatsBoxPanel1.Visibility = Visibility.Collapsed;
							break;
						case Controls.StatsArea.Modes.Stats:
							StatsBoxPanel1.Visibility = Visibility.Visible;
							SpinScanBox1.Visibility = SpinScan1.Visibility = Visibility.Collapsed;
							break;
						default:
							StatsBoxPanel1.Visibility =	SpinScanBox1.Visibility = SpinScan1.Visibility = Visibility.Collapsed;
							break;
					}
					break;
				case Mode.TwoPlayer:
				case Mode.TwoPlayerSplit:
					{
						Controls.SpinScan3D ss = n.Num == 0 ? SpinScan2_1 : SpinScan2_2;
						Controls.SpinScanBox sbox = n.Num == 0 ? SpinScanBox2_1 : SpinScanBox2_2;
						Grid stats = n.Num == 0 ? StatsBoxPanel2_1 : StatsBoxPanel2_2;
						switch (mode)
						{
							case Controls.StatsArea.Modes.Polar:
							case Controls.StatsArea.Modes.Bar:
								ss.Polar = mode == Controls.StatsArea.Modes.Polar;
								ss.Visibility = sbox.Visibility = Visibility.Visible;
								stats.Visibility = Visibility.Collapsed;
								break;
							case Controls.StatsArea.Modes.Stats:
								stats.Visibility = Visibility.Visible;
								ss.Visibility = sbox.Visibility = Visibility.Collapsed;
								break;
							default:
								stats.Visibility = ss.Visibility = sbox.Visibility = Visibility.Collapsed;
								break;
						}
					}
					break;
				case Mode.MultiPlayer:
					switch (mode)
					{
						case Controls.StatsArea.Modes.Polar:
						case Controls.StatsArea.Modes.Bar:
							SpinScan_M.Polar = mode == Controls.StatsArea.Modes.Polar;
							SpinScanBox_M.Visibility = SpinScan_M.Visibility = Visibility.Visible;
							StatsBoxPanel_M.Visibility = Visibility.Collapsed;
							break;
						case Controls.StatsArea.Modes.Stats:
							StatsBoxPanel_M.Visibility = Visibility.Visible;
							SpinScanBox_M.Visibility = SpinScan_M.Visibility = Visibility.Collapsed;
							break;
						default:
							StatsBoxPanel_M.Visibility = Visibility.Collapsed;
							SpinScanBox_M.Visibility = SpinScan_M.Visibility = Visibility.Collapsed;
							break;
					}
					break;
				case Mode.MultiDisplay2:
				case Mode.MultiDisplay3:
				case Mode.MultiDisplay4:
				case Mode.MultiDisplay6:
				case Mode.MultiDisplay8:
					m_SAList[n.Num].Mode = mode;
					break;
			}
		}
		/********************************************************************************************************

		********************************************************************************************************/

		protected override bool ScreenChange(Unit unit)
		{
			if (!unit.MasterPad)
				return false;
			if (CurrentMode == Mode.MultiPlayer && m_RiderCount > 1)
			{
				m_IndividualWindows = true;
				RedoNodes();
				return true;
			}
			else if (CurrentMode >= Mode.MultiDisplay2 && CurrentMode <= Mode.MultiDisplay8)
			{
				m_IndividualWindows = false;
				RedoNodes();
				return true;
			}
			else if (CurrentMode == Mode.TwoPlayer)
			{
				m_IndividualWindows = true;
				RedoNodes();
				return true;
			}
			m_IndividualWindows = false;
			RedoNodes();
			return true;
		}

		/********************************************************************************************************

		********************************************************************************************************/

		protected override void Start()  {
			Log.WriteLine("/Pages/Modes/Ride3D.xamal.cs, RideBase::Ride3D::Start()");
			Unit.ClearCalibration();
			m_Save = null;
			m_bReset = false;
			if (!m_bAllSaved)
				CountDown.Start();
			else
				Reset3D();
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
		}


		/********************************************************************************************************
			DEMO
		********************************************************************************************************/

		bool m_bDemo = false;
		UnitSave m_DemoSave;
		
		protected override void ToggleDemo()
		{
			if (!m_bDemo)
				DemoOn();
			else
				DemoOff();
		}

		/********************************************************************************************************

		********************************************************************************************************/

		void DemoOn()
		{
			if (m_bDemo)
				return;
			DemoDisplay.Visibility = Visibility.Visible;
			m_bDemo = true;
			m_DemoSave = new UnitSave();	// Save out the current state of the units.
			// Change all the bots to a active unit.
			foreach (Unit unit in Unit.RaceUnit)
			{
				SetUnitBot(unit);
			}
			RedoNodes();
		}

		/********************************************************************************************************

		********************************************************************************************************/

		void DemoOff()
		{
			if (!m_bDemo)
				return;
			DemoDisplay.Visibility = Visibility.Collapsed;
			m_bDemo = false;
			m_DemoSave.Restore();
		}

		/********************************************************************************************************

		********************************************************************************************************/

		protected override bool IsDemo
		{
			get
			{
				return m_bDemo;
			}
		}

		/********************************************************************************************************

		********************************************************************************************************/

		protected override void SelectUnit(int num)
		{
			base.SelectUnit(num);
			Unit unit = SelectedUnit;
			if (unit != null && unit.IsRiderUnit)
				StatsUnit = unit;
		}



		RandomModel[] m_BotModels = new RandomModel[8];

		/********************************************************************************************************

		********************************************************************************************************/

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
				if (m_BotModels[unit.Number] == null)
				{
					m_BotModels[unit.Number] = new RandomModel();
					if (unit.Rider != null)
						m_BotModels[unit.Number].Set(unit.Rider);
				}
				unit.Bot.OverrideModel = m_BotModels[unit.Number];
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

		/********************************************************************************************************

		********************************************************************************************************/

		protected override void AddDemoRider()
		{
			foreach (Unit unit in Unit.Units)
			{
				if (!unit.IsActive)
				{
					unit.IsActive = true;
					SetUnitBot(unit);
					if (unit.Statistics.CurrentState != Unit.State)
					{
						if (Unit.State == Statistics.State.Running)
							unit.Statistics.Start();
					}
					RedoNodes();
					return;
				}
			}
			// 8 riders.. so kill it back to zero.
			int i;
			for (i = 1; i < 8; i++)
			{
				Unit.Units[i].IsActive = false;
			}
			RedoNodes();
		}

		/********************************************************************************************************

		********************************************************************************************************/

		protected override void IncreaseSomething()
		{
			if (!m_bDemo)
				return;
			WattsBot bot = SelectedUnit.Bot as WattsBot;
			if (bot == null)
				return;
			bot.BotWatts += 25;
		}
		/********************************************************************************************************

		********************************************************************************************************/

		protected override void DecreaseSomething()
		{
			if (!m_bDemo)
				return;
			WattsBot bot = SelectedUnit.Bot as WattsBot;
			if (bot == null)
				return;

			if (bot.BotWatts > 50)
				bot.BotWatts -= 25;
		}
		/********************************************************************************************************

		********************************************************************************************************/

		protected override void NextCameraRider()
        {
            Unit unit = SelectedUnit;
            Controls.Render3D.NextCameraRider(unit);
        }

		/********************************************************************************************************

		********************************************************************************************************/

		private void Course1_Unloaded(object sender, RoutedEventArgs e)
		{
			DemoOff();	// Make sure everything is restored on exit.
		}

		/********************************************************************************************************

		********************************************************************************************************/

		protected override void LeftButton(bool down)
		{
			if (!down)
				return;
			Bot bot = SelectedUnit.Bot;
			if (bot != null)
			{
				bot.Adjust(-1);
				m_Nodes[SelectedUnit.Number].On(this);
			}
		}
		/********************************************************************************************************

		********************************************************************************************************/

		protected override void RightButton(bool down)
		{
			if (!down)
				return;
			Bot bot = SelectedUnit.Bot;
			if (bot != null)
			{
				bot.Adjust(1);
				m_Nodes[SelectedUnit.Number].On(this);
			}
		}

		/********************************************************************************************************

		********************************************************************************************************/

		protected Unit AdjustUnit
		{
			get { return m_BotCount == 1 && m_TotalCount == 2 ? Unit.BotUnits[0] : m_StatsUnit; }
		}

		/********************************************************************************************************

		********************************************************************************************************/

		protected override void Keyboard_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.F:
                    NextCameraRider();
					break;
                case Key.H:
                    Controls.Render3D.ToggleFOV();
                    break;
                case Key.I:
                    Controls.Render3D.ShowInfo();
                    break;
				case Key.F2:
					OnDirectKey(SelectedUnit, RM1.PadKeys.F2);
					OnDirectKey(SelectedUnit, RM1.PadKeys.F2_Short);
					break;
				case Key.F3:
					OnDirectKey(SelectedUnit, RM1.PadKeys.F3);
					OnDirectKey(SelectedUnit, RM1.PadKeys.F3_Short);
					break;
				case Key.F4:
					OnDirectKey(SelectedUnit, RM1.PadKeys.F4);
					OnDirectKey(SelectedUnit, RM1.PadKeys.F4_Short);
					break;
				default:
					base.Keyboard_KeyDown(sender, e);
					break;
			}
		}

		/********************************************************************************************************

		********************************************************************************************************/

		private void RideBase_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double w = e.NewSize.Width;
			MultiBottomResize();
		}

		bool m_BottomResize = false;
		const double c_Height = 640;
		const double c_TimerHeight = 30;
		void MultiBottomResize()
		{
			if (!MultiDisplayPanel.IsVisible)
			{
				m_BottomResize = false;
				return;
			}
			if (MultiDisplayPanel.ActualHeight <= 0)
			{
				m_BottomResize = true;
				return;
			}
			Point pt = MultiDisplayPanel.TranslatePoint(new Point(0, 0), AppWin.Instance); // Figure out the top of the panel on the main Screen.

			double y = ActualHeight - pt.Y;
			foreach (RowDefinition r in m_AdjustRows)
				r.Height = new GridLength(y, GridUnitType.Pixel);
			
			m_BottomResize = false;
			/*
			dlg.Left = AppWin.Instance.Left + pt.X;
			dlg.Top = AppWin.Instance.Top + pt.Y;




			double h = MultiDisplayCourse.CalcHeight;
			double c = h + c_TimerHeight;
			double y = c * sheight / c_Height; // How high the total bottom bar height should be in real coordinates.


			//double w = swidth  / sheight;
			//MultiDisplayCourse.Width = w;
			Debug.WriteLine( "{0} x {1},  {2}, {3}",swidth,sheight,h,y);
			*/
		}

		/********************************************************************************************************

		********************************************************************************************************/

		void Reset3D()
		{
			m_bReset = false;
			UnPause();
			base.Reset();
			m_bAllSaved = false;
			if (m_bBackReset)
			{
				m_bBackReset = false;
				Back();
			}
		}

		//======================================================
		bool m_bAllSaved = false;	// Set to true when all saved.... For quick reset.

		/********************************************************************************************************

		********************************************************************************************************/

		protected override void Reset()
		{
			if (!Unit.HasStarted || AllFinished())
				Reset3D();
			else
			{
				Pause();
				if (RM1_Settings.General.SavePrompt)
					StartDialog(new DialogCB(cb_ResetSave), "Save performance file?", "Yes", "No","Cancel", false);
				else if (m_bBackReset)
					StartDialog(new DialogCB(cb_Reset), "Exit and end my ride?", "Yes", "No", false);
				else
					StartDialog(new DialogCB(cb_Reset), "Reset my ride?", "Yes", "No", false);
			}
		}
		/********************************************************************************************************

		********************************************************************************************************/

		void Finished()
		{
			if (RM1_Settings.General.SavePrompt)
				StartDialog(new DialogCB(cb_ResetSave), "Save performance file?", "Yes", "No", null, false);
			else
				SavePerformance();
		}

		bool m_bBackReset = false;
		bool m_bReset = false;
		/********************************************************************************************************

		********************************************************************************************************/

		void cb_Reset(RM1.PadKeys key)
		{
			CloseDialog();
			if (key == RM1.PadKeys.F1)
			{
				SavePerformance();
			}
			else
			{
				m_bCloseOnExit = m_bBackReset = m_bReset = false;
				UnPause();
			}
		}
		/********************************************************************************************************

		********************************************************************************************************/

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


		SavePerformance m_Save;
		/********************************************************************************************************

		********************************************************************************************************/

		protected override void SavePerformance()
		{
			if (m_bAllSaved)
				return;
			if (m_Save != null)
				return;
			m_Save = new SavePerformance(ReportColumns.Report_3DRoadRacing.Selected_ReportColumns);
			Unit.Stop(); // Stop everything if we haven't already

			StartDialog(new DialogCB(cb_ExportFiles), "Save Export files?", "Yes", "No", false);
			if (!RM1_Settings.General.ExportPrompt)
				cb_ExportFiles( RM1_Settings.General.ExportSave ? RM1.PadKeys.F1 : RM1.PadKeys.F2 );
		}
		/********************************************************************************************************

		********************************************************************************************************/

		void cb_ExportFiles(RM1.PadKeys key)
		{
			m_Save.ExportSave = key == RM1.PadKeys.F1;
			CloseDialog();
			StartDialog(cb_SaveReport, "Save report?", "Yes", "No", false);
			if (!RM1_Settings.General.ReportPrompt)
				cb_SaveReport(RM1_Settings.General.ReportSave ? RM1.PadKeys.F1: RM1.PadKeys.F2);
		}
		/********************************************************************************************************

		********************************************************************************************************/

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
		/********************************************************************************************************

		********************************************************************************************************/

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

		/********************************************************************************************************

		********************************************************************************************************/

		void cb_Saving(RM1.PadKeys key)
		{
			if (key == RM1.PadKeys.F2)
			{
				m_Save.Cancel();
				CloseDialog();
				if (m_bReset)
				{
					m_bReset = false;
					Reset3D();
				}
			}
		}
		/********************************************************************************************************

		********************************************************************************************************/

		void cb_SaveProgress(double progress, bool done)
		{
			if (done)
			{
				m_bAllSaved = true;
				CloseDialog();
				if (m_bReset)
				{
					m_bReset = false;
					Reset3D();
				}
			}
			double w = Dlg_ProgressBar.ActualWidth;
            PerfProgressBar.Width = w * progress;
		}

		/********************************************************************************************************

		********************************************************************************************************/

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


		/********************************************************************************************************

		********************************************************************************************************/

		void CloseDialog()
		{
			Dlg.FadeTo = 0.0;
			KeyBox.Focus();
			m_CurDialogCB = null;
		}

		/********************************************************************************************************

		********************************************************************************************************/

		private void Dlg_f1_Click(object sender, RoutedEventArgs e)
		{
			if (m_CurDialogCB != null)
				m_CurDialogCB(RM1.PadKeys.F1);
		}

		/********************************************************************************************************

		********************************************************************************************************/

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

		int[] draft = new int[] { -30, -20, -10, 10, 20, 30 };

		/********************************************************************************************************

		********************************************************************************************************/

		void F2(Unit unit)
		{
			if (unit == null || !unit.IsActive)
				return;

			if (CurrentMode == Mode.MultiPlayer && m_RiderCount == 1)
			{
				Node n = m_Nodes[unit.Number];
				n.ModeNum += 1;
				UpdateMode(n);
			}
			else if (CurrentMode == Mode.MultiPlayer)
			{
				StatsUnit = unit;
			}
			else
			{
				if (CurrentMode == Mode.TwoPlayer)
					StatsUnit = unit;
				Node n = m_Nodes[unit.Number];
				if ((int)CurrentMode >= (int)Mode.MultiDisplay2)
					n.MultiModeNum += 1;
				else
					n.ModeNum += 1;
				UpdateMode(n);
			}
		}

		/********************************************************************************************************

		********************************************************************************************************/

		void SelectBot(Unit unit)
		{
			if (unit == null || !unit.IsActive)
				return;
			Node n = m_Nodes[unit.Number];
			n.SelectBot(this);
		}

		/********************************************************************************************************

		********************************************************************************************************/

		protected void AdjustBot(Unit unit, int dir)
		{
			if (unit == null)
				return;
			m_Nodes[unit.Number].Adjust(this, dir);
		}


		/********************************************************************************************************

		********************************************************************************************************/

		protected void F4(Unit unit)
		{
			Controls.Render3D.NextCameraRider(unit);
			m_Nodes[unit.Number].CameraMode = Controls.Render3D.GetCameraRider(unit);
			if (CurrentMode == Mode.MultiPlayer || CurrentMode == Mode.TwoPlayer)
			{
				StatsUnit = unit;
			}
		}


		/********************************************************************************************************

		********************************************************************************************************/

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
            //modification here so that Computrainer functions don't need the FN key 
            if (unit.IsVelotron )
            {
                switch (key)
                {
                    case RM1.PadKeys.F2: break;	// Make sure that this one doesn't happen at all
                    case RM1.PadKeys.F2_Short:
                        F2(unit);
                        break;
                    case RM1.PadKeys.F2_Long:
                        if (!ScreenChange(unit))
                            F2(unit);
                        break;
                    case RM1.PadKeys.F3:
                        if (Unit.State == Statistics.State.Paused && unit.Trainer != null)
                        {
                            unit.Trainer.CalibrateMode = !unit.Trainer.CalibrateMode;
                            break;
                        }
                        SelectBot(unit);
                        break;
                    case RM1.PadKeys.F4:
                        F4(unit);
                        break;
                    case RM1.PadKeys.F5:
                        if (Unit.FirstMasterUnit == unit)
                        {
                            m_bReset = true;
                            Reset();
                        }
                        break;
                    case RM1.PadKeys.FN_UP: break;
                    case RM1.PadKeys.FN_UP_Short: AdjustBot(unit, 1); break;
                    case RM1.PadKeys.FN_UP_Long: AdjustBot(unit, 1); break; 
                    case RM1.PadKeys.FN_UP_Repeat: AdjustBot(unit, 1); break;
                    case RM1.PadKeys.FN_DOWN: break;
                    case RM1.PadKeys.FN_DOWN_Short: AdjustBot(unit, -1); break;
                    case RM1.PadKeys.FN_DOWN_Long: AdjustBot(unit, -1); break;
                    case RM1.PadKeys.FN_DOWN_Repeat: AdjustBot(unit, -1); break;
                    default:
                        base.OnDirectKey(unit, key);
                        break;
                }
            }
            else
            {
                switch (key)
                {
                    case RM1.PadKeys.F2: break;	// Make sure that this one doesn't happen at all
                    case RM1.PadKeys.F2_Short:
                        F2(unit);
                        break;
                    case RM1.PadKeys.F2_Long:
                        if (!ScreenChange(unit))
                            F2(unit);
                        break;
                    case RM1.PadKeys.F3:
                        if (Unit.State == Statistics.State.Paused && unit.Trainer != null)
                        {
                            unit.Trainer.CalibrateMode = !unit.Trainer.CalibrateMode;
                            break;
                        }
                        SelectBot(unit);
                        break;
                    case RM1.PadKeys.F4:
                        F4(unit);
                        break;
                    case RM1.PadKeys.F5:
                        if (Unit.FirstMasterUnit == unit)
                        {
                            m_bReset = true;
                            Reset();
                        }
                        break;
                    case RM1.PadKeys.UP: break;
                    case RM1.PadKeys.UP_Short: AdjustBot(unit, 1); break;
                    case RM1.PadKeys.UP_Long: AdjustBot(unit, 1); break;
                    case RM1.PadKeys.UP_Repeat: AdjustBot(unit, 1); break;
                    case RM1.PadKeys.DOWN: break;
                    case RM1.PadKeys.DOWN_Short: AdjustBot(unit, -1); break;
                    case RM1.PadKeys.DOWN_Long: AdjustBot(unit, -1); break;
                    case RM1.PadKeys.DOWN_Repeat: AdjustBot(unit, -1); break;
                    default:
                        base.OnDirectKey(unit, key);
                        break;
                }
            }

		}

		/********************************************************************************************************

		********************************************************************************************************/

		public Page LoadScreen()
		{
			// Do we need the load screen at all
            //yes we do because when teh course is long and intricate, it takes a long time to load.
			int est = 100;
             
			foreach (Unit unit in Unit.Active)
			{
				PerformanceBot pb = unit.Bot as PerformanceBot;
				if (pb != null)
					est += pb.EstLoadPerformance();
			}
            if (est <= 0)
            {
                //Debug.WriteLine("The loader screen is not needed");
                return this; // Just naviagate to this screen not he load screen.
            }
           // Debug.WriteLine("I will show the loader screen with " + est);
			// Build the load screen.
            m_Est = est;
			Pages.Modes.Loader loader = new Pages.Modes.Loader("Loading course and performances", this, this,
				new DoWorkEventHandler(bw_Loader),new RunWorkerCompletedEventHandler (bw_done), null);
           //Debug.WriteLine("the loader has returned from launch");
            m_LoaderScreen = true;
			return loader;
		}


		/********************************************************************************************************

		********************************************************************************************************/

		void bw_done(object sender, RunWorkerCompletedEventArgs e)
        {
            //Debug.WriteLine("the bw_done has fired");
            
        }


		/********************************************************************************************************

		********************************************************************************************************/

		bool m_LoaderScreen;
        int m_Est;
		int m_EstBase;
		double m_EstCur;
		void bw_Loader(object sender, DoWorkEventArgs e)
        {
            App.SetDefaultCulture();

            BackgroundWorker bw = sender as BackgroundWorker;
            int et = 0;
            m_EstBase = et * 100 / m_Est;
            m_EstCur = 100.0 / (double)m_Est;

            while (true)
            {
                int loadprogress = (int)Controls.Render3D.ShowProgress();
                bw.ReportProgress(loadprogress);
                if (100 <= loadprogress)
                {
                    et += 100;
                    break;
                }
                Thread.Sleep(10);
            }
           // Debug.WriteLine("done the course");

            foreach (Unit unit in Unit.Active)
            {
                PerformanceBot pb = unit.Bot as PerformanceBot;
                if (pb != null)
                {
                    int size = pb.EstLoadPerformance();
                    if (size > 0)
                    {
                        m_EstBase = et * 100 / m_Est;
                        m_EstCur = (double)size / (double)m_Est;
                        pb.LoadPerformance(bw);
                        // here we need to examine if the perfomance is from an RCV and whether the start point is = 0 or not.
                        if(pb.Course.PerformanceInfo.Mode== AppModes.RCV && (pb.Course.StartAt != 0)) 
                        {
                            // breakpointmehere to simulate the lockup.
                            //double courseshiftX = pb.Course.StartAt;
                            //pb.Course.EndAt = pb.Course.EndAt - courseshiftX;
                            //pb.Course.StartAt = 0;
                        }

                        et += size;
                    }
                }
               
            }
            //break point here to mess with async.
           // Debug.WriteLine("Done the perfs");
            

        }
		/********************************************************************************************************

		********************************************************************************************************/

		void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Loader loader = sender as Loader;
			loader.OverridePercent = e.ProgressPercentage * m_EstCur;
		}

		/********************************************************************************************************

		********************************************************************************************************/

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
