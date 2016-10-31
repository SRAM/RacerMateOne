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
using System.IO;
using System.Reflection;

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for AntSensorLine.xaml
	/// </summary>
	public partial class AntSensorLine : UserControl
	{
		//=========================================================================================================
		private int m_sensorID;
		public int SensorID
		{
			get { return m_sensorID; }
		}

		//=========================================================================================================
		public static DependencyProperty ActiveProperty = DependencyProperty.Register("Active", typeof(bool), typeof(AntSensorLine),
			new FrameworkPropertyMetadata(false, new PropertyChangedCallback(_ActiveChanged)));

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

		private List<Rider> m_availableRiders = new List<Rider>();

		public Rider AssignedRider
		{
			get; set;
		}

		private Rider LastAssignedRider { get; set; }

		//=========================================================================================================
		//public static readonly RoutedEvent ChangedEvent =
		//	EventManager.RegisterRoutedEvent(
		//	"Changed", RoutingStrategy.Bubble,
		//	typeof(RoutedEventHandler),
		//	typeof(AntSensorLine));

		public struct RiderChangedEventArgs
		{
			public Rider previousRider;
			public Rider newRider;
			public int sensorId;
		}

		public delegate void RiderChangedEventHandler(object sender, RiderChangedEventArgs args);

		public event RiderChangedEventHandler RiderChanged;

		//public event RoutedEventHandler Changed
		//{
		//	add { AddHandler(ChangedEvent, value); }
		//	remove { RemoveHandler(ChangedEvent, value); }
		//}

		//=========================================================================================================

		public static bool StopChangeEvents = false;

		public readonly ComboBoxItem Unassigned;

		public AntSensorLine(int sensorID, byte sensorType)
		{
			InitializeComponent();

			m_sensorID = sensorID;
			SensorId.Content = m_sensorID;

			// The only sensor type currently supported is 120 (heart rate monitors)

			Unassigned = new ComboBoxItem();
			Unassigned.Content = "Unassigned";
			Unassigned.Tag = null;

			AssignedRider = null;
		}

		~AntSensorLine()
		{
		}

		private void SensorLine_Loaded(object sender, RoutedEventArgs e)
		{
			LastAssignedRider = AssignedRider;
			RedoDropDown();
		}

		public void AddRider(Rider rider)
		{
			m_availableRiders.Add(rider);
		}

		public void RedoDropDown()
		{
			AssignedRiderDropDown.Items.Clear();
			AssignedRiderDropDown.Items.Add(Unassigned);

			ComboBoxItem selitem = Unassigned;

			foreach (Rider rider in m_availableRiders)
			{
				ComboBoxItem citem = new ComboBoxItem();
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

			//// I think this commented out bit are so that a rider can be assigned
			//// to this sensor elsewhere, and this will make sure the proper item is selected.
			//// They likely aren't needed for this dialog.
			//if (selitem.Tag != null)
			//	AssignedRider = selitem.Tag as Rider;

			AssignedRiderDropDown.SelectedItem = selitem;
		}

		public void UnassignRider()
		{
			m_inchange = true;
			AssignedRiderDropDown.SelectedItem = Unassigned;
			m_inchange = false;
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
				LED_c1.Color = ms_On1;
				LED_c2.Color = ms_On2;
			}
			else
			{
				LED_c1.Color = ms_Off1;
				LED_c2.Color = ms_Off2;
			}
		}

		private bool m_inchange = false;
		private void AssignedRider_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AssignedRiderDropDown.SelectedItem != null && !m_inchange)
			{
				RiderChangedEventArgs args = new RiderChangedEventArgs();
				args.previousRider = LastAssignedRider;
				ComboBoxItem item = AssignedRiderDropDown.SelectedItem as ComboBoxItem;
				args.newRider = (item != null ? item.Tag as Rider : null);
				args.sensorId = SensorID;

				if (RiderChanged != null &&
					args.previousRider != args.newRider)
				{
					RiderChanged(this, args);
				}
				LastAssignedRider = args.newRider;
			}
		}

		//private delegate void _change(Rider rider);
		//void change(Rider rider)
		//{
		//	AssignedRider = rider;
		//}
		
		private void SensorLine_Unloaded(object sender, RoutedEventArgs e)
		{
		}

		private void SensorLine_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{

		}
	}
}
