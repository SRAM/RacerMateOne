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
    /// Interaction logic for ANTSensorsDialog.xaml
    /// </summary>
    public partial class ANTSensorsDialog : Window
    {
        public String HrSensorId = null;
        public String CadenceSensorId = null;

        public ANTSensorsDialog()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HrSensorIdText.Focus();
        }
        
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // validate the input
            int hrId = 0;
            if (int.TryParse(HrSensorIdText.Text, out hrId) == false)
            {
                RacerMateOne.Dialogs.JustInfo msg = new RacerMateOne.Dialogs.JustInfo();
                msg.Owner = AppWin.Instance;
                msg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                msg.TopText.Text = "Invalid HR Sensor ID entered - it must be an integer.";
                msg.ShowDialog();
                HrSensorIdText.SelectAll();
                HrSensorIdText.Focus();
                return;
            }

            int cadId = 0;
            if (int.TryParse(CadenceSensorIdText.Text, out cadId) == false)
            {
                RacerMateOne.Dialogs.JustInfo msg = new RacerMateOne.Dialogs.JustInfo();
                msg.Owner = AppWin.Instance;
                msg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                msg.TopText.Text = "Invalid Cadence Sensor ID entered - it must be an integer.";
                msg.ShowDialog();
                CadenceSensorIdText.SelectAll();
                CadenceSensorIdText.Focus();
                return;
            }

            // All input is valid!
            HrSensorId = HrSensorIdText.Text;
            CadenceSensorId = CadenceSensorIdText.Text;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            HrSensorId = null;
            CadenceSensorId = null;
            Close();
        }
    }
}
