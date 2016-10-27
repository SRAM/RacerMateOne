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
		public int numSensors = 0;
		private Controls.AntSensorLine[] sensorLines;
		//public List<Rider> RiderList;
		public System.Collections.ObjectModel.ObservableCollection<Rider> RiderList;

        public ANTSensorsList()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
			SensorListPanel.Children.Clear();

			numSensors = 20;
			sensorLines = new Controls.AntSensorLine[numSensors];
			for (int i = 0; i < numSensors; i++)
			{
				sensorLines[i] = new Controls.AntSensorLine();
				sensorLines[i].SensorID = i;
				sensorLines[i].allKnownRiders = RiderList;
				sensorLines[i].Active = ((i % 5) == 0);
				SensorListPanel.Children.Add(sensorLines[i]);
			}
        }
        
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
