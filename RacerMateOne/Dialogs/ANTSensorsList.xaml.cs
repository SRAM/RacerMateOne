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
using System.Windows.Shapes;

namespace RacerMateOne.Dialogs
{
    /// <summary>
    /// Interaction logic for ANTSensorsList.xaml
    /// </summary>
    public partial class ANTSensorsList : Window
    {
		private Dictionary<int, Controls.AntSensorLine> m_unassignedSensorToLineDict = new Dictionary<int, Controls.AntSensorLine>();

		private List<RM1.SENSOR> m_detectedSensors = new List<RM1.SENSOR>();
		private List<int> m_detectedHRSensors = new List<int>();

		private Dictionary<Rider, int> m_tmpRiderSensors = new Dictionary<Rider, int>();

		public System.Collections.ObjectModel.ObservableCollection<Rider> RiderList;

        public ANTSensorsList()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
			foreach (Rider rider in RiderList)
			{
				m_tmpRiderSensors.Add(rider, rider.HrSensorId);
			}

			Refresh_Click(sender, e);
        }
        
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

		/// <summary>
		/// Adds a sensor line to the 'New' list of sensors.
		/// These are sensors that were detected, but ARE NOT assigned to a rider yet.
		/// These sensors are inherently active, because they were detected nearby.
		/// </summary>
		/// <param name="sensorNumber"></param>
		private Controls.AntSensorLine AddToNewSensorLine(ushort sensorNumber, byte type)
		{
			Controls.AntSensorLine sensorLine = new Controls.AntSensorLine(sensorNumber, type);
			sensorLine.Active = true;
			sensorLine.RiderChanged += OnRiderChanged;
			NewSensorListPanel.Children.Add(sensorLine);
			NewHRCountLabel.Content = NewSensorListPanel.Children.Count;
			return sensorLine;
		}

		/// <summary>
		/// Adds a sensor line to the 'Found' list of sensors.
		/// These are sensors that were detected, and ARE assigned to a rider.
		/// These sensors are inherently active, because they were detected nearby.
		/// </summary>
		/// <param name="sensorNumber"></param>
		private Controls.AntSensorLine AddToFoundSensorLine(ushort sensorNumber, byte type)
		{
			Controls.AntSensorLine sensorLine = new Controls.AntSensorLine(sensorNumber, type);
			sensorLine.Active = true;
			sensorLine.RiderChanged += OnRiderChanged;
			FoundSensorListPanel.Children.Add(sensorLine);
			FoundHRCountLabel.Content = FoundSensorListPanel.Children.Count;
			return sensorLine;
		}

		/// <summary>
		/// Adds a sensor line to the 'Saved' list of sensors.
		/// These are sensors that were NOT detected, but are assigned to a rider.
		/// These sensors are inherently inactive, because they were not detected.
		/// </summary>
		/// <param name="sensorNumber"></param>
		private Controls.AntSensorLine AddToSavedSensorLine(ushort sensorNumber, byte type)
		{
			Controls.AntSensorLine sensorLine = new Controls.AntSensorLine(sensorNumber, type);
			sensorLine.Active = false;
			sensorLine.RiderChanged += OnRiderChanged;
			SavedSensorListPanel.Children.Add(sensorLine);
			SavedHRCountLabel.Content = SavedSensorListPanel.Children.Count;
			return sensorLine;
		}
		
		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			NewSensorListPanel.Children.Clear();
			NewHRCountLabel.Content = "0";
			FoundSensorListPanel.Children.Clear();
			FoundHRCountLabel.Content = "0";
			SavedSensorListPanel.Children.Clear();
			SavedHRCountLabel.Content = "0";

			m_detectedSensors = detect_sensors();

			// Extract just the HR sensor IDs
			m_detectedHRSensors.Clear();
			for (int i = 0; i < m_detectedSensors.Count; i++)
			{
				if (m_detectedSensors[i].type == 120)
				{
					m_detectedHRSensors.Add(m_detectedSensors[i].sensor_number);
				}
			}

			// Add the sensors that have been assigned to riders,
			// and filter out those that are still unassigned / newly discovered.
			List<int> unassignedHRSensors = new List<int>(m_detectedHRSensors);
			foreach (Rider rider in m_tmpRiderSensors.Keys)
			{
				if (rider.HrSensorId != 0)
				{
					if (m_detectedHRSensors.Contains(rider.HrSensorId))
					{
						Controls.AntSensorLine line = AddToFoundSensorLine((ushort)rider.HrSensorId, 120);
						line.AddRider(rider);
						unassignedHRSensors.Remove(rider.HrSensorId);
					}
					else
					{
						Controls.AntSensorLine line = AddToSavedSensorLine((ushort)rider.HrSensorId, 120);
						line.AddRider(rider);
					}
				}
			}

			// Add the newly discovered sensors
			foreach (int unassignedSensorID in unassignedHRSensors)
			{
				Controls.AntSensorLine line = AddToNewSensorLine((ushort)unassignedSensorID, 120);
				m_unassignedSensorToLineDict[unassignedSensorID] = line;
				foreach (Rider rider in m_tmpRiderSensors.Keys)
				{
					// only add riders to this line which have not yet had a HR sensor assigned to them.
					if (rider.HrSensorId == 0)
					{
						line.AddRider(rider);
					}
				}
			}
		}

		private void OnRiderChanged(object sender, Controls.AntSensorLine.RiderChangedEventArgs e)
		{
			// Since riders can only have 1 HR monitor, we can:
			// 1) clear the sensor ID from the previous rider if they existed
			// and 2) assign the sensor ID to the new rider if they exist
			if (e.previousRider != null)
			{
				m_tmpRiderSensors[e.previousRider] = 0;
			}

			if (e.newRider != null)
			{
				// Get the previous sensor if the newly assigned rider already had a sensor.
				int previousSensor = m_tmpRiderSensors[e.newRider];
				m_tmpRiderSensors[e.newRider] = e.sensorId;

				//if (previousSensor != 0)
				//{
				//	foreach (int sensor in m_unassignedSensorToLineDict.Keys)
				//	{
				//		if (m_unassignedSensorToLineDict[sensor].AssignedRider == e.newRider)
				//		{
				//			m_unassignedSensorToLineDict[sensor].UnassignRider();
				//		}
				//	}
				//}

				if (previousSensor != 0 && m_unassignedSensorToLineDict.ContainsKey(previousSensor))
				{
					// in this case, we have to remove this rider from the previous sensor
					m_unassignedSensorToLineDict[previousSensor].UnassignRider();
				}
			}
		}

		private List<RM1.SENSOR> detect_sensors()
		{
			return RM1.GetAntSensorList();
		}
	}
}
