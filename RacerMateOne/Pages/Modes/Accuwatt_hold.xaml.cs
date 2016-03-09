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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace RacerMateOne.Pages.Modes
{
	/// <summary>
	/// Interaction logic for Accuwatt.xaml
	/// </summary>
	public partial class Accuwatt_hold : RideBase
	{
		int m_UnitNumber;
		UnitSave m_Saved;
		Unit m_Unit;

		public int UnitNumber
		{
			get { return m_UnitNumber; }
			set
			{
				if (m_UnitNumber == value)
					return;
				m_UnitNumber = value;
				SetUnitNumber();
			}
		}

		public Accuwatt_hold()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				m_Saved = new UnitSave();
				RideBase_Loaded(MainGrid);
				SetUnitNumber();
				StartTest();
				if (m_Unit != null)
					OnUnitFlagsChanged(m_Unit, m_StatFlags);
				KeyBox.Margin = new Thickness(0, 0, 0, 50);
			});
		}


		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			if (m_Unit != null)
				Unit.RemoveNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
			RideBase_Unloaded();
			if (m_Saved != null)
				m_Saved.Restore();
		}

		void SetUnitNumber()
		{
			if (!m_bInit)
				return;

			Unit.LoadFromSettings();
			foreach (Unit u in Unit.Units)
			{
				bool active = u.Number == m_UnitNumber;
				if (u.IsActive != active)
				{
					u.IsActive = active;
					if (active)
						u.Trainer.OnPadKey += new RM1.TrainerPadKey(OnPadKey);
					else
						u.Trainer.OnPadKey -= new RM1.TrainerPadKey(OnPadKey);
				}
			}
			Unit unit = Unit.RaceUnit[0];
			if (unit != m_Unit)
			{
				Unit.RemoveNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
				Unit.AddNotify(unit, m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
				m_Unit = unit;
			}
			if (unit.Rider == null)
			{
				unit.Rider = Riders.RidersList.First();
			}
			unit.Bot = null;

			// Load up the stuff.
			/*
			Stats.Unit = unit;
			Polar_1.Unit = unit;
			Bar_1.Unit = unit;
			Render_1.Unit = unit;
			Controls.Render3D.SetupRiders();
			*/
		}
		enum Levels
		{
			Start,
			Keep,
			Halfway,
			Close,
			Stop
		};

		void StartTest()
		{
			m_bDone = false;
			Test.FadeTo = 1;
			Results.FadeTo = 0;
		}

		void ShowResults()
		{
			Test.FadeTo = 0;
			Results.FadeTo = 1;
		}

		bool m_bDone;
		Levels m_CurLevel;
		StatFlags m_StatFlags = StatFlags.Speed;
		void OnUnitFlagsChanged(Unit unit, StatFlags changed)
		{
			if (!m_bInit)
				return;
			if ((changed & StatFlags.Speed) != StatFlags.Zero)
			{
				double speed = unit.Statistics.SpeedDisplay;
				Speed.Content = String.Format("{0:F2}", speed);
				double mph = unit.Statistics.Speed * ConvertConst.MetersPerSecondToMPH;
				Levels level;
				if (mph <= 0.0001)
				{
					level = Levels.Start;
				}
				else if (mph < 11.5)
					level = Levels.Keep;
				else if (mph < 20)
					level = Levels.Close;
				else
					level = Levels.Stop;
				if (m_bDone)
				{
					if (mph < 10)
						ShowResults();
					level = Levels.Stop;
				}
				if (level != m_CurLevel)
				{
					m_CurLevel = level;
					bool flash = false;
					String c;
					switch (level)
					{
						case Levels.Start:
							c = "Start pedaling";
							break;
						case Levels.Keep:
							c = "Keep pedaling";
							break;
						case Levels.Halfway:
							c = "You are half way there";
							break;
						case Levels.Close:
							c = "Getting close";
							break;
						default:
							c = "Stop pedaling";
							flash = true;
							m_bDone = true;
							break;
					}
					Comment.Content = c;
					if (flash)
					{
						Comment.Foreground = Brushes.Red;
						Comment.BeginAnimation(Label.OpacityProperty, Flash_Anim);
					}
					else
					{
						Comment.BeginAnimation(Label.OpacityProperty, null);
						Comment.Foreground = Brushes.White;
					}
				}

			}
			/*
			if ((changed & StatFlags.Time) != StatFlags.Zero)
			{
				UpdateTime();
			}
			 */
		}


		//===================================================
		protected override void Start()
		{
			base.Start();
		}
		protected override void Reset()
		{
			base.Reset();
		}
		protected override void Pause()
		{
			base.Pause();
		}
		protected override void UnPause()
		{
			base.UnPause();
		}
		protected override bool OverrideF3(Unit unit)
		{
			return true;
		}

		//=========================================================

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}

		private void Retry_Click(object sender, RoutedEventArgs e)
		{
			StartTest();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}

		Label Speed;
		Label Comment;
		private void Speed_Loaded(object sender, RoutedEventArgs e)
		{
			Speed = sender as Label;
		}
		private void Comment_Loaded(object sender, RoutedEventArgs e)
		{
			Comment = sender as Label;
		}



	}

}
