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
	/// Interaction logic for RCV.xaml
	/// </summary>
	public partial class zzz_RCV : RideBase
	{
		static Controls.StatsArea.Modes[] ms_ModeCycle = new RacerMateOne.Controls.StatsArea.Modes[] {
			Controls.StatsArea.Modes.Polar,
			Controls.StatsArea.Modes.Bar,
			Controls.StatsArea.Modes.Simple
			};
		static Controls.StatsArea.Modes[] ms_MultiModeCycle = new RacerMateOne.Controls.StatsArea.Modes[] {
			Controls.StatsArea.Modes.Stats,
			Controls.StatsArea.Modes.Simple,
			Controls.StatsArea.Modes.Polar,
			Controls.StatsArea.Modes.Bar,
			};
		class Node
		{
			public Unit Unit;
			public int Num;
			public int ModeNum = 0;			// 
			public int MultiModeNum = 0;	// When in MultiDisplay3 - 8
		};

		public zzz_RCV()
		{
			InitializeComponent();
		}
		Node[] m_Nodes = new Node[8];


		public UnitSave StartDemoOnLoad = null;

		ReportColumns m_DisplayColumns;
		StatFlags m_DisplayStatFlags;
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			// Determin what mode we will be using
			if (StartDemoOnLoad != null)
				StartDemoOnLoad.Restore();

			Mode mode;
			int cnt = 0;
			m_Nodes = new Node[8];
			foreach (Unit unit in Unit.RaceUnit)
			{
				m_Nodes[cnt] = new Node();
				m_Nodes[cnt].Num = cnt;
				m_Nodes[cnt].Unit = unit;
				cnt++;
			}
			m_DisplayColumns = ReportColumns.Display_RCV.Selected_ReportColumns;
			m_DisplayStatFlags = m_DisplayColumns.StatFlags;

			RideBase_Loaded(MainGrid);
			cnt = Unit.RaceUnit.Count;
			if (cnt <= 1)
				mode = Mode.OnePlayer;
			else //if (cnt >= 2)
				mode = Mode.TwoPlayer;

			Course1.BikeIcon = BikeIcon1;

			SetMode(mode);
			if (StartDemoOnLoad != null)
			{
				DemoOn();
			}
				
		}


		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			RideBase_Unloaded();
		}

		Mode CurrentMode = Mode.None;

		private void SetMode(Mode mode)
		{
			if (CurrentMode == mode)
				return;

			switch (CurrentMode)
			{
				case Mode.None: break;
				case Mode.OnePlayer:
					OnePlayer.Visibility = Visibility.Collapsed;
					SpinScan1.Unit = null;
					SpinScanBox1.Unit = null;

					Timer1.Unit = null;
					Course1.Unit = null;
					Stats1.Unit = null;
					InfoLine1.Unit = null;
					break;
				case Mode.TwoPlayer:
					TwoPlayer.Visibility = Visibility.Collapsed;
					InfoLine2_1.Unit = Timer2_1.Unit = SpinScan2_1.Unit = SpinScanBox2_1.Unit = null;
					InfoLine2_2.Unit = Timer2_2.Unit = SpinScan2_2.Unit = SpinScanBox2_2.Unit = null;
					Stats2.SetUnit(0, null);
					Stats2.SetUnit(1, null);
					CourseDisplay2.Course = null;
					break;
				case Mode.MultiPlayer:
					break;
				case Mode.MultiDisplay3:
				case Mode.MultiDisplay4:
				case Mode.MultiDisplay6:
				case Mode.MultiDisplay8:
					ClearMultiDisplay();
					break;
			}

			CurrentMode = mode;
			Unit unit;

			switch (mode)
			{
				case Mode.None: break;
				case Mode.OnePlayer:
					OnePlayer.Visibility = Visibility.Visible;
					unit = Unit.RaceUnit[0];
					RCV_1.Course = Unit.Course;
					RCV_1.Location = 0;
					RCV_1.FirstUnit = false;
					RCV_1.Unit = unit;

					// Prepair the mode
					SpinScan1.Unit = unit;
					SpinScanBox1.Unit = unit;

					Timer1.Unit = unit;
					Course1.Unit = unit;
					Stats1.StatFlags = m_DisplayStatFlags;
					Stats1.Unit = unit;
					InfoLine1.Unit = unit;
					
					break;
				case Mode.TwoPlayer:
					TwoPlayer.Visibility = Visibility.Visible;

					unit = Unit.RaceUnit[0];
					InfoLine2_1.Unit = Timer2_1.Unit = SpinScan2_1.Unit = SpinScanBox2_1.Unit = unit;
					RCV_2_1.Course = Unit.Course;
					RCV_2_1.Location = 0;
					RCV_2_1.FirstUnit = false;
					RCV_2_1.Unit = unit;

					unit = Unit.RaceUnit[1];
					InfoLine2_2.Unit = Timer2_2.Unit = SpinScan2_2.Unit = SpinScanBox2_2.Unit = unit;
					RCV_2_2.Course = Unit.Course;
					RCV_2_2.Location = 0;
					RCV_2_2.FirstUnit = false;
					RCV_2_2.Unit = unit;

					Stats2.StatFlags = m_DisplayStatFlags | StatFlags.RiderName;
					Stats2.SetUnit(0, Unit.RaceUnit[0]);
					Stats2.SetUnit(1, Unit.RaceUnit[1]);

					CourseDisplay2.Course = Unit.Course;

					break;
				case Mode.MultiPlayer:
					break;
				case Mode.MultiDisplay3:
					break;
				case Mode.MultiDisplay4:
					break;
				case Mode.MultiDisplay6:
					break;
				case Mode.MultiDisplay8:
					break;
			}
			foreach (Node n in m_Nodes)
				UpdateMode(n);
		}

		private void UnitCycle_OnUnitChanged(object sender, RoutedEventArgs e)
		{
			if (CurrentMode == Mode.MultiPlayer)
			{
				//Unit unit = Cycle.Unit;
				//Timer_M.Unit = Stats_M.Unit = InfoLine_M.Unit = SpinScan_M.Unit = SpinScanBox_M.Unit = unit;
				//CourseDisplay_M.ActiveUnit = unit;
				//UpdateMode(m_Nodes[unit.RaceUnitNumber]);
			}

		}


		List<Controls.StatsArea> m_SAList = new List<RacerMateOne.Controls.StatsArea>();

		void ClearMultiDisplay()
		{
			//MultiDisplay.Visibility = Visibility.Collapsed;
			//m_CurCtrl.Visibility = m_CurRender.Visibility = Visibility.Collapsed;
		}
		void SetupMultiDisplay(Grid ctrl, Grid render)
		{
			/*
			int max = Unit.RaceUnit.Count;
			int cnt = 0;
			if ((ctrl != m_CurCtrl || m_CurActiveUnits != max))
			{
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
				}
				foreach (UIElement e in ctrl.Children)
				{
					Grid g = e as Grid;
					if (g != null)
					{
						Controls.StatsArea sa = new Controls.StatsArea();
						sa.Unit = Unit.RaceUnit[cnt++];
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
					r.Unit = cnt < max ? Unit.RaceUnit[cnt] : null;
					cnt++;
				}
			}
			MultiDisplay.Visibility = Visibility.Visible;
			m_CurCtrl = ctrl;
			m_CurRender = render;
			m_CurActiveUnits = max;
			m_CurCtrl.Visibility = m_CurRender.Visibility = Visibility.Visible;
			MultiDisplayCourse.Course = Unit.Course;
			 */
			foreach (Node n in m_Nodes)
			{
				if (n == null || n.Unit == null)
					break;
				UpdateMode(n);
			}
		}
		bool[] m_Bar = new bool[8];

		void UpdateMode(Node n)
		{
			if (n == null)
				return;

			int num;
			Controls.StatsArea.Modes mode;
			Controls.StatsArea.Modes[] modearr;
			bool multi = (int)CurrentMode >= (int)Mode.MultiDisplay3;

			if (multi)
			{
				modearr = ms_MultiModeCycle;
				num = n.MultiModeNum;
			}
			else
			{
				modearr = ms_ModeCycle;
				num = n.ModeNum;
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
			switch (CurrentMode)
			{
				case Mode.OnePlayer:
					switch (mode)
					{
						case Controls.StatsArea.Modes.Polar:
						case Controls.StatsArea.Modes.Bar:
							//SpinScan1.Polar = mode == Controls.StatsArea.Modes.Polar;
							//SpinScanBox1.Visibility = SpinScan1.Visibility = Visibility.Visible;
							break;
						default:
							//SpinScanBox1.Visibility = SpinScan1.Visibility = Visibility.Collapsed;
							break;
					}
					break;
				case Mode.TwoPlayer:
					{
						/*
						Controls.SpinScan3D ss = n.Num == 0 ? SpinScan2_1 : SpinScan2_2;
						Controls.SpinScanBox sbox = n.Num == 0 ? SpinScanBox2_1 : SpinScanBox2_2;
						switch (mode)
						{
							case Controls.StatsArea.Modes.Polar:
							case Controls.StatsArea.Modes.Bar:
								ss.Polar = mode == Controls.StatsArea.Modes.Polar;
								ss.Visibility = sbox.Visibility = Visibility.Visible;
								break;
							default:
								ss.Visibility = sbox.Visibility = Visibility.Collapsed;
								break;
						}
						 */
					}
					break;
				case Mode.MultiPlayer:
					/*
					if (Cycle.Unit != n.Unit)
						break;
					switch (mode)
					{
						case Controls.StatsArea.Modes.Polar:
						case Controls.StatsArea.Modes.Bar:
							SpinScan_M.Polar = mode == Controls.StatsArea.Modes.Polar;
							SpinScanBox_M.Visibility = SpinScan_M.Visibility = Visibility.Visible;
							break;
						default:
							SpinScanBox_M.Visibility = SpinScan_M.Visibility = Visibility.Collapsed;
							break;
					}
					 */
					break;
				case Mode.MultiDisplay3:
				case Mode.MultiDisplay4:
				case Mode.MultiDisplay6:
				case Mode.MultiDisplay8:
					//m_SAList[n.Num].Mode = mode;
					break;
			}
		}
		protected override void ChangeView()
		{
			if (CurrentMode == Mode.MultiPlayer)
			{
				switch (Unit.RaceUnit.Count)
				{
					case 1: SetMode(Mode.OnePlayer); break;
					case 2: SetMode(Mode.TwoPlayer); break;
					case 3: SetMode(Mode.MultiDisplay3); break;
					case 4: SetMode(Mode.MultiDisplay4); break;
					case 5: SetMode(Mode.MultiDisplay6); break;
					case 6: SetMode(Mode.MultiDisplay6); break;
					case 7: SetMode(Mode.MultiDisplay8); break;
					case 8: SetMode(Mode.MultiDisplay8); break;
				}
			}
			else if (CurrentMode != Mode.OnePlayer)
			{
				SetMode(Mode.MultiPlayer);
			}
		}
		protected override void Scroll(Unit unit, int dir)
		{
			if (unit == null || !unit.IsActive)
				return;

			Node n = m_Nodes[unit.RaceUnitNumber];
			if (CurrentMode == Mode.MultiPlayer && dir > 0)
			{
				/*
				if (Cycle.Unit != n.Unit)
				{
					Cycle.ForceUnit(n.Unit);
					return;
				}
				 */
			}
			if ((int)CurrentMode >= (int)Mode.MultiDisplay3)
				n.MultiModeNum += dir;
			else
				n.ModeNum += dir;
			UpdateMode(n);
		}

		protected override void Start()
		{
			CountDown.Start();
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
		//===========================================================
		bool m_bDemo = false;
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
			foreach (Unit unit in Unit.RaceUnit)
			{
				unit.Bot = new WattsBot(200 + unit.Number * 20);
				unit.Bot.Name = unit.Rider != null ? unit.Rider.ToString() : ms_BotNames[unit.Number];
			}
		}
		void DemoOff()
		{
			if (!m_bDemo)
				return;
			m_bDemo = false;
			m_DemoSave.Restore();
			if (StartDemoOnLoad != null)
			{
				NavigationService.GoBack();
			}
		}
		protected override bool IsDemo
		{
			get
			{
				return m_bDemo;
			}
		}

		protected override void AddDemoRider()
		{
			int active = Unit.RaceUnit.Count > 1 ? 1 : 2;
			if (active == 2)
			{
				foreach (Unit unit in Unit.Units)
				{
					if (!unit.IsActive)
					{
						unit.IsActive = true;
						unit.Bot = new WattsBot(200 + unit.Number * 20);
						unit.Bot.Name = unit.Rider != null ? unit.Rider.ToString() : ms_BotNames[unit.Number];
						break;
					}
				}
			}
			else
			{
				while (Unit.RaceUnit.Count > 1)
					Unit.RaceUnit[1].IsActive = false;
			}
			if (Staging.ms_DemoRestore == null)
				Staging.ms_DemoRestore = m_DemoSave;
			Staging.ms_DemoNext = new UnitSave();
			DemoOff();

			
			NavigationService.GoBack();
		}



	}
}
