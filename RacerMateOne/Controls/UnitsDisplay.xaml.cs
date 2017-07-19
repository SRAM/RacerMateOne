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
using System.Diagnostics;
namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for UnitsDisplay.xaml
	/// </summary>
	public partial class UnitsDisplay : UserControl
	{
		StatFlags m_StatFlags = StatFlags.HardwareStatus | StatFlags.RiderName;
		bool m_bInit = false;
		bool m_UpdateStarted = false;

		bool m_OnlyOne = false;
		public bool OnlyOne
		{
			get { return m_OnlyOne; }
			set
			{
				if (m_OnlyOne != value)
				{
					foreach (Node n in m_Nodes)
					{
						n.OneActive.Visibility = value ? Visibility.Visible : Visibility.Hidden;
						n.Active.Visibility = value ? Visibility.Hidden : Visibility.Visible;
					}
					m_OnlyOne = value;
				}
			}
		}

		public bool OnlyPerformanceBot = false;

		public ActiveUnits ActiveUnits
		{
			get
			{
				int i = 1;
				ActiveUnits au = ActiveUnits.Zero;
				foreach( Node n in m_Nodes )
				{
					if (m_OnlyOne && n.OneActive.IsEnabled && n.OneActive.IsChecked == true)
					{
						au = (ActiveUnits)i;
						break;
					}
					else if (n.Active.IsEnabled && n.Active.IsChecked == true)
						au |= (ActiveUnits)i;

					i += i;
				}
				return au;
			}
			set
			{
				LoadFromUnits();
				int i = 1;
				foreach (Node n in m_Nodes)
				{
					bool active = (value & (ActiveUnits)i) != ActiveUnits.Zero;
					if (m_OnlyOne)
						n.OneActive.IsChecked = n.OneActive.IsEnabled ? active : false;
					else
						n.Active.IsChecked = n.Active.IsEnabled ? active : false;
					i += i;
				}
			}
		}

		bool m_NoBots = false;
		public bool NoBots
		{
			get { return m_NoBots; }
			set
			{
				if (m_NoBots == value)
					return;
				m_NoBots = value;

			}
		}

		public void LoadFromUnits()
		{
			LoadFromUnits(false);
		}

		public void Refresh()
		{
			ActiveUnits au = ActiveUnits;
			LoadFromUnits(true);
			ActiveUnits = au;
		}

		void LoadFromUnits(bool again)
		{
			int cnt = 0;
			Node first = null;
			foreach(Node n in m_Nodes)
			{
				if (first == null && n.Unit.Rider != null)
				{
					first = n;
					break;
				}
			}
			if (first == null)
			{
				first = m_Nodes[0];
				first.Unit.Rider = Riders.RidersList[0];
			}
			bool onlyone = OnlyOne;
			foreach (Node n in m_Nodes)
			{
				bool enabled = n.Unit.Rider != null || (!onlyone  && n.Unit.Bot != null);
				bool active = enabled && n.Unit.TC != null && n.Unit.TC.Active;
				if (OnlyPerformanceBot && n.Unit.Bot != null && (n.Unit.Bot as PerformanceBot) == null)
				{
					enabled = false;
					active = false;
				}

				n.Active.IsEnabled = enabled;
				n.OneActive.IsEnabled = n.Unit.IsRiderUnit;
				n.Active.Opacity = n.OneActive.Opacity = enabled ? 1.0 : 0.5;
				if (enabled && active)
				{
					cnt++;
					n.Active.IsChecked = true;
				}
				else
				{
					n.Active.IsChecked = false;
				}
				n.OneActive.IsChecked = false;
			}
			int num = RM1_Settings.General.TrainingRiderNumber;
			if (cnt == 0 && !again)
			{
				foreach (Node n in m_Nodes)
					n.Unit.IsActive = false;
                //Debug.WriteLine("about to call atleastoneactive from units display");
				Unit.AtLeastOneActive();
                //Debug.WriteLine("returned");
				LoadFromUnits(true);
			}
			if (!again)
			{
				Unit ua = Unit.OneActive;
				foreach (Node n in m_Nodes)
				{
					if (n.Unit == ua)
					{
						n.OneActive.IsChecked = true;
						break;
					}
				}
			}
		}
		public void SaveToSettings()
		{
			foreach (Node n in m_Nodes)
			{
				if (n.Unit.TC != null)
					n.Unit.TC.Active = n.Active.IsChecked == true;
				if (n.OneActive.IsChecked == true)
				{
					Unit.OneActive = n.Unit;
					RM1_Settings.General.TrainingRiderNumber = n.Unit.Number;
				}
			}
			
		}



		public void SetActiveUnits()
		{
			foreach (Node n in m_Nodes)
			{
				if (m_OnlyOne)
				{
					n.Unit.IsActive = n.OneActive.IsChecked == true;
					if (n.OneActive.IsChecked == true)
						Unit.OneActive = n.Unit;
				}
				else
					n.Unit.IsActive = n.Active.IsChecked == true;
			}
		}

		class Node
		{
			public Unit Unit;
			public CheckBox Active;
			public Label RiderName;
			public Label Type;
			public RadioButton OneActive;
			public Brush TextColor;

			public Node(Unit unit, CheckBox active, Label ridername, Label type, RadioButton oneactive)
			{
				Unit = unit;
				Active = active;
				RiderName = ridername;
				Type = type;
				OneActive = oneactive;
				TextColor = Type.Foreground;
			}
			public void Update()
			{
				if (Unit.Bot != null)
				{
					RiderName.Content = Unit.Bot.UnitContent;
					Type.Content = "-";
				}
				else
				{
					RiderName.Content = Unit.Statistics.RiderName;
					String t = Unit.Trainer == null ? "-" :
						Unit.TC.DeviceType == RM1.DeviceType.COMPUTRAINER ? "CT" :
						Unit.Trainer.Type == RM1.DeviceType.VELOTRON ? "VT" :
						Unit.TC == null ? "-" :
						Unit.TC.DeviceType == RM1.DeviceType.COMPUTRAINER ? "CT" :
						Unit.TC.DeviceType == RM1.DeviceType.VELOTRON ? "VT" : "-";
					Type.Content = t;
					Type.Foreground = t == "-" || (Unit.Trainer != null && Unit.Trainer.IsConnected) ? TextColor : Brushes.Red;
                    //Added here Feb8 2012 Paul smeulders
                    if (Unit.Trainer != null && !Unit.Trainer.IsConnected)
                    {
                        Active.IsChecked = false;
                        OneActive.IsChecked = false;
                    }
				}
			}
		}
		Node[] m_Nodes = new Node[8];

		CheckBox[] m_Active = new CheckBox[8];
		Label[] m_RiderName = new Label[8];
		Label[] m_Type = new Label[8];


		public UnitsDisplay()
		{
			InitializeComponent();

			m_Nodes[0] = new Node(Unit.Units[0], Unit_Active_0, Unit_RiderName_0, Unit_Type_0, Unit_OneActive_0);
			m_Nodes[1] = new Node(Unit.Units[1], Unit_Active_1, Unit_RiderName_1, Unit_Type_1, Unit_OneActive_1);
			m_Nodes[2] = new Node(Unit.Units[2], Unit_Active_2, Unit_RiderName_2, Unit_Type_2, Unit_OneActive_2);
			m_Nodes[3] = new Node(Unit.Units[3], Unit_Active_3, Unit_RiderName_3, Unit_Type_3, Unit_OneActive_3);
			m_Nodes[4] = new Node(Unit.Units[4], Unit_Active_4, Unit_RiderName_4, Unit_Type_4, Unit_OneActive_4);
			m_Nodes[5] = new Node(Unit.Units[5], Unit_Active_5, Unit_RiderName_5, Unit_Type_5, Unit_OneActive_5);
			m_Nodes[6] = new Node(Unit.Units[6], Unit_Active_6, Unit_RiderName_6, Unit_Type_6, Unit_OneActive_6);
			m_Nodes[7] = new Node(Unit.Units[7], Unit_Active_7, Unit_RiderName_7, Unit_Type_7, Unit_OneActive_7);
		}

		void UpdateStart()
		{
            //Debug.WriteLine("I'm in updatestart");
            if (IsVisible && m_bInit && !m_UpdateStarted && !AppWin.IsInDesignMode)
			{
                m_UpdateStarted = true;
     			Unit.AddNotify(null, m_StatFlags, new Unit.NotifyEvent(OnUpdateUnit));
				OnUpdateUnit(null, m_StatFlags);

				foreach (Node n in m_Nodes)
				{
					n.Active.IsChecked = n.Unit.IsActive;
				}
			}
		}
		void UpdateStop()
		{
            //Debug.WriteLine("UpdateStop");
            if (m_UpdateStarted)
			{
				Unit.RemoveNotify(null, m_StatFlags, new Unit.NotifyEvent(OnUpdateUnit));
				m_UpdateStarted = false;
			}
		}


		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			m_bInit = true;
			UpdateStart();
			OnUpdateUnit(null, m_StatFlags);
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			UpdateStop();
		}

		void OnUpdateUnit(Unit unit_n, StatFlags changed)
		{
			if (!m_bInit)
				return;
			if ((changed & StatFlags.RiderName) != StatFlags.Zero)
			{
				foreach(Node n in m_Nodes)
					n.RiderName.Content = n.Unit.Statistics.RiderName;
			}
			if ((changed & StatFlags.HardwareStatus) != StatFlags.Zero)
			{
				foreach (Node n in m_Nodes)
				{
					n.Update();
				}
			}
		}

		private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsVisible)
				UpdateStart();
			else
				UpdateStop();
		}

		private void Unit_OneActive_Checked(object sender, RoutedEventArgs e)
		{
              //Debug.WriteLine("you just checked it Checked RadioButton");
		}

        private void Unit_Active_0_Clicked(object sender, RoutedEventArgs e)
        {
            //the last character must be a number
            CheckBox calledby = sender as CheckBox;
            if (calledby == null) return; //failed to cast

            string namestringprefix = "Unit_Active_";
            int lastletterindex = namestringprefix.Length;
            try
            {
                int index = Convert.ToInt32(calledby.Name.Substring(lastletterindex));
                //  Log.WriteLine("Selecting Checkbox Unit_Active_" + calledby.Name.Substring(lastletterindex) + " has been clicked" + " name is " + calledby.Name);
                if (calledby.IsChecked == true)
                {
                    //Debug.Write("The trainer type is :" + m_Nodes[index].Unit.Trainer.Type.ToString());
                    //Debug.Write("The trainer connected is :" + m_Nodes[index].Unit.Trainer.IsConnected.ToString());

                    if (m_Nodes[index].Unit.Trainer != null && m_Nodes[index].Unit.Trainer.IsConnected == false && m_Nodes[index].Unit.Bot == null)
                    {
                        RacerMateOne.Dialogs.JustInfo showinfo = new RacerMateOne.Dialogs.JustInfo("You are trying to assign a rider to a CT/VT that is not connected (red text)." + Environment.NewLine +
                            "Check connections and rescan using the options...hardware setup, or do not assign the position to a rider. If all riders are assigned to non-connected hardware, demo mode will play." + Environment.NewLine +
                            "Then use the PC keyboard to control the action.", "OK", null);
                        showinfo.ShowDialog();

                        calledby.IsChecked = false;
                    }
                }
            }
            catch
            { //in case of error do not allow the box to be checked at all
                Log.WriteLine("Selecting Checkbox Unit_Active_" + calledby.Name.Substring(lastletterindex) + " failed");
                calledby.IsChecked = false;
            }

        }

        private void Unit_OneActive_0_Click(object sender, RoutedEventArgs e)
        {
            RadioButton calledby = sender as RadioButton;
            if (calledby == null) return; //failed to cast

            string namestringprefix = "Unit_OneActive_";
            int lastletterindex = namestringprefix.Length;
            try
            {
             //   Log.WriteLine("Selecting Checkbox Unit_Active_" + calledby.Name.Substring(lastletterindex) + " has been clicked" + " name is " + calledby.Name );
  
                int index = Convert.ToInt32(calledby.Name.Substring(lastletterindex));
                if (calledby.IsChecked == true)
                {
                    if (m_Nodes[index].Unit.Trainer != null && m_Nodes[index].Unit.Trainer.IsConnected == false && m_Nodes[index].Unit.Bot == null)
                    {
                        RacerMateOne.Dialogs.JustInfo showinfo = new RacerMateOne.Dialogs.JustInfo("You are trying to assign a rider to a CT/VT that is not connected (red text)." + Environment.NewLine +
                            "Check connections and rescan using the options...hardware setup, or do not assign this position for the ride. If no riders are selected, the first communicating entry in the list will be assigned to the session." + Environment.NewLine
                            + "If no hardware is connected, demo mode will play.", "OK", null);
                        showinfo.ShowDialog();

                        calledby.IsChecked = false;
                    }
                }
            }
            catch
            { //in case of error do not allow the box to be checked at all
                Log.WriteLine("Selecting Checkbox Unit_Active_" + calledby.Name.Substring(lastletterindex) + " failed");
                calledby.IsChecked = false;
            }
        }
	}
}
