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
using NativeWifi;
using System.Diagnostics;

namespace RacerMateOne.Dialogs
{
    /// <summary>
    /// Interaction logic for HardwareWifiConfig.xaml
    /// </summary>
    public partial class HardwareWifiConfig : Window
    {
        public String SSID = null;
        public String Password = null;
        private WlanClient client = new WlanClient();
        Wlan.WlanAvailableNetwork m_connectedNetwork;
        List<Wlan.WlanAvailableNetwork> m_racermateNetworks = new List<Wlan.WlanAvailableNetwork>();
        List<Wlan.WlanAvailableNetwork> m_otherNetworks = new List<Wlan.WlanAvailableNetwork>();

        public HardwareWifiConfig()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VerifyWifiSupport();
            Refresh_Click(sender, e);
        }

        /// <summary>
        /// Converts a 802.11 SSID to a string.
        /// </summary>
        private static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        private void VerifyWifiSupport()
        {
            if (client.Interfaces.Length == 0)
            {
                Ask dialog = new Ask("Your computer MUST have wireless network access to use this setup wizard.", "OK", "Cancel");
                dialog.ShowDialog();
            }
        }

        private void UpdateDialogState()
        {
            bool hasWifiSupport = client.Interfaces.Length > 0;
            bool hasHomeSSID = HomeNetworkNameDropDrop.SelectedItem != null;
            bool hasHomePassword = PasswordText.Password != string.Empty;
            bool hasConfigured = false;

            HomeNetworkNameDropDrop.IsEnabled = hasWifiSupport;
            PasswordText.IsEnabled = hasWifiSupport;

            Send.IsEnabled = hasWifiSupport && hasHomeSSID && hasHomePassword;

            OK.IsEnabled = hasWifiSupport && hasHomeSSID && hasHomePassword && hasConfigured;

            if (hasWifiSupport)
            {
                HomeNetworkNameDropDrop.Focus();
            }
            else
            {
                RefreshButton.Focus();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RacerMateAccessPointsDropDown.Items.Clear();
            HomeNetworkNameDropDrop.Items.Clear();
            CurrentSSIDText.Foreground = Brushes.Red;
            CurrentSSIDText.Text = "Unavailable";

            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                // Lists all networks with WEP security
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);

                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    if (network.dot11Ssid.SSIDLength < 2)
                    {
                        // SSIDs that are really short are probably hidden and we don't want to attempt to get that working.
                        continue;
                    }

                    string availableSSID = GetStringForSSID(network.dot11Ssid);
                    if ((network.flags & Wlan.WlanAvailableNetworkFlags.Connected) == Wlan.WlanAvailableNetworkFlags.Connected)
                    {
                        m_connectedNetwork = network;
                        CurrentSSIDText.Foreground = Brushes.Green;
                        CurrentSSIDText.Text = availableSSID;
                    }

                    if (availableSSID.StartsWith("RacerMateCT"))
                    {
                        m_racermateNetworks.Add(network);
                        RacerMateAccessPointsDropDown.Items.Add(availableSSID);
                    }
                    else if (network.flags == 0)
                    {
                        // tells us that the network is available
                        m_otherNetworks.Add(network);
                        HomeNetworkNameDropDrop.Items.Add(availableSSID);
                    }
                }
            }

            // select the first one in the list
            if (HomeNetworkNameDropDrop.Items.Count > 0)
            {
                HomeNetworkNameDropDrop.SelectedIndex = 0;
            }

            // select the first one in the list
            if (RacerMateAccessPointsDropDown.Items.Count > 0)
            {
                RacerMateAccessPointsDropDown.SelectedIndex = 0;
            }

            UpdateDialogState();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SendConfiguration_Click(object sender, RoutedEventArgs e)
        {
            // Run external exe
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"console.exe";
            startInfo.Arguments = "\"" + SSID + "\" \"" + PasswordText.Password + "\" false";
            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            Process console = Process.Start(startInfo);

            // wait 10 seconds for it to complete, although it should be very quick
            if (console.WaitForExit(10 * 1000) == false)
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = "Timed out.";
                console.Kill();
            }
            else
            {
                int exitCode = console.ExitCode;
                System.IO.StreamReader stdOut = console.StandardOutput;
                System.IO.StreamReader stdErr = console.StandardError;

                // parse result
                string error = stdErr.ReadToEnd();
                string output = stdOut.ReadToEnd();

                // update status
                if (error.Length == 0 && output.StartsWith("OK"))
                {
                    StatusText.Foreground = Brushes.Green;
                    StatusText.Text = "Success!";
                    UpdateDialogState();
                }
                else
                {
                    StatusText.Foreground = Brushes.Red;
                    StatusText.Text = error;
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SSID = null;
            Password = null;
            Close();
        }

        private void RMAccessPoint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void HomeNetworkNameDropDrop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HomeNetworkNameDropDrop.SelectedItem != null)
            {
                SSID = HomeNetworkNameDropDrop.SelectedItem.ToString();
            }
            else
            {
                SSID = string.Empty;
            }
        }
    }
}
