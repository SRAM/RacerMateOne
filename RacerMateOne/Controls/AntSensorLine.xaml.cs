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

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for AntSensorLine.xaml
	/// </summary>
	public partial class AntSensorLine : UserControl
	{
		//=========================================================================================================
		public static DependencyProperty SensorIDProperty = DependencyProperty.Register("SensorID", typeof(int), typeof(AntSensorLine),
			new FrameworkPropertyMetadata(0, new PropertyChangedCallback(_SensorIDChanged)));

		public int SensorID
		{
			get { return (int)this.GetValue(SensorIDProperty); }
			set { this.SetValue(SensorIDProperty, value); }
		}

		private static void _SensorIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((AntSensorLine)d).OnSensorIDChanged();
		}

		//=========================================================================================================
		public static DependencyProperty ActiveProperty = DependencyProperty.Register("Active", typeof(bool), typeof(AntSensorLine),
			new FrameworkPropertyMetadata(false,new PropertyChangedCallback(_ActiveChanged)));

		public bool Active
		{
			get { return (bool)this.GetValue(ActiveProperty); }
			set { this.SetValue(ActiveProperty, value); }
		}

		//=========================================================================================================

		private static void _ActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((AntSensorLine)d).OnActiveChanged();
		}

		//public List<Rider> AllKnownRiders
		//{
		//	get
		//	{
		//		return allKnownRiders;
		//	}
		//
		//	set
		//	{
		//		allKnownRiders = value;
		//	}
		//}

		//		private List<Rider> allKnownRiders = new List<Rider>();
		public System.Collections.ObjectModel.ObservableCollection<Rider> allKnownRiders;

		public Rider AssignedRider
		{
			get; set;
		}

		private Rider LastAssignedRider { get; set; }

		//=========================================================================================================
		public static readonly RoutedEvent ChangedEvent =
			EventManager.RegisterRoutedEvent(
			"Changed", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(AntSensorLine));


		public event RoutedEventHandler Changed
		{
			add { AddHandler(ChangedEvent, value); }
			remove { RemoveHandler(ChangedEvent, value); }
		}

		//=========================================================================================================

		public static bool StopChangeEvents = false;

		public AntSensorLine()
		{
			InitializeComponent();
			AssignedRider = null;
		}

		~AntSensorLine()
		{
		}

		private void btn_Loaded(object sender, RoutedEventArgs e)
		{
			Icon.Content = "Heart";
			RedoDropDown();
		}

		public void RedoDropDown()
		{
			AssignedRiderDropDown.Items.Clear();

			ComboBoxItem citem;
			citem = new ComboBoxItem();
			citem.Content = "Unassigned";
			citem.Tag = null;
			AssignedRiderDropDown.Items.Add(citem);
			ComboBoxItem selitem = citem;

			if (allKnownRiders == null)
				return;

			foreach (Rider rider in allKnownRiders)
			{
				citem = new ComboBoxItem();
				citem.Content = rider.FirstName + " " + rider.LastName;
				citem.Tag = rider;
				if (rider.HrSensorId == SensorID)
				{
					AssignedRider = rider;
				}
				if (rider == AssignedRider)
				{
					selitem = citem;
				}
				AssignedRiderDropDown.Items.Add(citem);
			}
			if (selitem.Tag != null)
				AssignedRider = selitem.Tag as Rider;
			AssignedRiderDropDown.SelectedItem = selitem;
		}

		static Color ms_Off1 = Color.FromArgb(255, 0, 0, 0);
		static Color ms_Off2 = Color.FromArgb(255, 0x77, 0x77, 0x77);
		static Color ms_On1 = Color.FromArgb(255, 0, 0xAA, 0x33);
		static Color ms_On2 = Color.FromArgb(255, 0, 0xFF, 0x88);

		/// <summary>
		/// Changes the color of the LED to indicate whether or not the sensor is active.
		/// </summary>
		public void OnActiveChanged()
		{
			if (Active)
			{
				//Attach();
				LED_c1.Color = ms_On1;
				LED_c2.Color = ms_On2;
			}
			else
			{
				//Detach();
				LED_c1.Color = ms_Off1;
				LED_c2.Color = ms_Off2;
			}
		}

		/// <summary>
		/// Updates the UI to show the new value
		/// </summary>
		public void OnSensorIDChanged()
		{
			SensorId.Content = SensorID;
		}

		private bool m_inchange = false;
		private void AssignedRider_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AssignedRiderDropDown.SelectedItem != null && !m_inchange)
			{
				m_inchange = true;
				ComboBoxItem item = AssignedRiderDropDown.SelectedItem as ComboBoxItem;
				AssignedRider = (item != null ? item.Tag as Rider : null);
				if (AssignedRider != null)
				{
					AssignedRider.HrSensorId = SensorID;
					if (LastAssignedRider != AssignedRider && !StopChangeEvents)
					{
						RoutedEventArgs args = new RoutedEventArgs(ChangedEvent);
						RaiseEvent(args);
					}
				}
				m_inchange = false;
			}
		}

		private delegate void _change(Rider rider);
		void change(Rider rider)
		{
			AssignedRider = rider;
		}

		//private void Identify_Click(object sender, RoutedEventArgs e)
		//{
		//	Dialogs.HardwareLine_Select dlg = new Dialogs.HardwareLine_Select();
		//	dlg.Owner = AppWin.Instance;
		//	dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		//	dlg.ShowDialog(); //shows as modal
		//	if (dlg.Trainer != null)
		//		Dispatcher.BeginInvoke(DispatcherPriority.Normal, new _change(change), dlg.Trainer);
		//	/*
		//	if (dlg.Trainer != null)
		//	{
		//		m_inchange = true;
		//		Trainer = dlg.Trainer;
		//		foreach( ComboBoxItem item in Devices.Items)
		//		{
		//			if (dlg.Trainer == item.Tag as RM1.Trainer)
		//			{
		//				Devices.SelectedItem = item;
		//			}
		//		}
		//		if (!StopChangeEvents)
		//		{
		//			RoutedEventArgs args = new RoutedEventArgs(ChangedEvent);
		//			RaiseEvent(args);
		//		}
		//		m_inchange = false;
		//	}
		//	 */
		//}

		private void btn_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			//if (IsVisible)
			//	RedoDropDown();
		}

		private void btn_Unloaded(object sender, RoutedEventArgs e)
		{
//			RM1.OnCalibrationChanged -= new RM1.TrainerEvent(RM1_OnCalibrationChanged);
		}
	}
}
