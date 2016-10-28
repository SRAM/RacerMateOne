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
		private Controls.AntSensorLine[] m_sensorLines;

		private List<RM1.SENSOR> m_detectedSensors = new List<RM1.SENSOR>();

		//public List<Rider> RiderList;
		public System.Collections.ObjectModel.ObservableCollection<Rider> RiderList;

        public ANTSensorsList()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
			SensorListPanel.Children.Clear();
        }
        
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			int numSensors = detect_sensors();

			m_sensorLines = new Controls.AntSensorLine[numSensors];
			for (int i = 0; i < numSensors; i++)
			{
				m_sensorLines[i] = new Controls.AntSensorLine();
				m_sensorLines[i].SensorID = i;
				m_sensorLines[i].allKnownRiders = RiderList;
				//m_sensorLines[i].Active = ((i % 5) == 0);
				SensorListPanel.Children.Add(m_sensorLines[i]);
			}
		}

		private int detect_sensors()
		{
			m_detectedSensors = RM1.GetAntSensorList();
			int numSensors = 20;

			return numSensors;
		}
	}
}
