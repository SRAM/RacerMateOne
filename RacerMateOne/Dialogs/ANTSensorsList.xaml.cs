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
		/// <summary>
		/// This maps an unassigned sensor ID to the UI line so that we can reset the assigned rider if the rider is
		/// moved to a different sensor.
		/// </summary>
		private Dictionary<int, Controls.AntSensorLine> m_unassignedSensorToLineDict = new Dictionary<int, Controls.AntSensorLine>();

		/// <summary>
		/// This is the list of detected sensors with all available information.
		/// </summary>
		private List<RM1.SENSOR> m_detectedSensors = new List<RM1.SENSOR>();

		/// <summary>
		/// This is a list of the HR sensor IDs have been detected.
		/// </summary>
		private List<int> m_detectedHRSensors = new List<int>();

		/// <summary>
		/// This stores the temporary assignments that have been made so that the user can either
		/// Cancel the changes if they've made a mistake,
		/// or Save the changes when they are done.
		/// </summary>
		private Dictionary<Rider, int> m_tmpRiderSensors = new Dictionary<Rider, int>();

		/// <summary>
		///  This is the list of all known riders.
		/// </summary>
		public System.Collections.ObjectModel.ObservableCollection<Rider> RiderList;

		/// <summary>
		/// Constuctor
		/// </summary>
        public ANTSensorsList()
        {
            InitializeComponent();
        }

		/// <summary>
		/// When the window is loaded, extract the known HR sensors from the list of all riders.
		/// Then refresh the UI.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
			foreach (Rider rider in RiderList)
			{
				m_tmpRiderSensors.Add(rider, rider.HrSensorId);
			}

			while (RM1.StartedANT == false)
			{
				RacerMateOne.Dialogs.Ask info = new RacerMateOne.Dialogs.Ask("ANT+ stick not detected.\nRacerMate will not be able to utilize ANT+ sensors.\nPlease insert an ANT+ stick.", "Rescan", "Skip");
				bool? result = info.ShowDialog();

				bool attemptToDetectAntStick = (!result.HasValue) ? false : result.Value;

				if (attemptToDetectAntStick == false)
				{
					break;
				}

				RM1.StartANT(0);
			}

			Refresh_Click(sender, e);
        }
        
		/// <summary>
		/// Save the changes to the rider settings file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
			foreach (Rider rider in m_tmpRiderSensors.Keys)
			{
				rider.HrSensorId = m_tmpRiderSensors[rider];
			}

			RM1_Settings.SaveToFile();

			Close();
        }

		/// <summary>
		/// Cancel any changes that were made, do not save them, close the dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		
		/// <summary>
		/// Refreshes the list of detected sensors and resets the UI to the most recently saved state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			NewSensorListPanel.Children.Clear();
			NewHRCountLabel.Content = "0";
			FoundSensorListPanel.Children.Clear();
			FoundHRCountLabel.Content = "0";
			SavedSensorListPanel.Children.Clear();
			SavedHRCountLabel.Content = "0";

			m_detectedSensors = RM1.GetAntSensorList();

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

		/// <summary>
		/// Called when the user changes the rider assigned to a sensor.
		/// Updates other sensor lines to reflect the newly assigned rider & sensor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRiderChanged(object sender, Controls.AntSensorLine.RiderChangedEventArgs e)
		{
			// Since riders can only have 1 HR monitor, we can:
			// 1) If the new rider was previously assigned to a different sensor, remove it.
			// 2) Assign the rider to the new sensor.
			// 3) If there was a previous rider on this sensor, clear that persons sensor.
			// Note: Order matters in the above, so that we don't accidentally clear the rider that was just moved.
			if (e.newRider != null)
			{
				// Get the previous sensor if the newly assigned rider already had a sensor.
				int previousSensor = m_tmpRiderSensors[e.newRider];
				if (previousSensor != 0 && m_unassignedSensorToLineDict.ContainsKey(previousSensor))
				{
					// in this case, we have to remove this rider from the previous sensor
					m_unassignedSensorToLineDict[previousSensor].UnassignRider();
				}

				m_tmpRiderSensors[e.newRider] = e.sensorId;
			}

			if (e.previousRider != null)
			{
				m_tmpRiderSensors[e.previousRider] = 0;
			}
		}
	}
}
