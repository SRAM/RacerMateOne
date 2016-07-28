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
        public String SSID = string.Empty;
        public String Password = string.Empty;
        private string m_selectedRacerMateAccessPointSSID = string.Empty;
        private WlanClient client = new WlanClient();
        Wlan.WlanAvailableNetwork m_connectedNetwork;
        List<Wlan.WlanAvailableNetwork> m_racermateNetworks = new List<Wlan.WlanAvailableNetwork>();
        List<Wlan.WlanAvailableNetwork> m_otherNetworks = new List<Wlan.WlanAvailableNetwork>();
        int m_numConfiguredTrainers = 0;

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

            HomeNetworkNameDropDrop.IsEnabled = hasWifiSupport;
            PasswordText.IsEnabled = hasWifiSupport;

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

                    if (availableSSID.StartsWith("RacerMateCT") && network.flags == 0)
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

            // Auto-select the network that we think is their home network
            if (HomeNetworkNameDropDrop.Items.Contains(CurrentSSIDText.Text))
            {
                HomeNetworkNameDropDrop.SelectedItem = CurrentSSIDText.Text;
            }
            else
            {
                if (HomeNetworkNameDropDrop.Items.Count > 0)
                {
                    HomeNetworkNameDropDrop.SelectedIndex = 0;
                }
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
            // return true if at least one trainer was configured
            this.DialogResult = (m_numConfiguredTrainers > 0);
            Close();
        }

        private string ConvertToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        private void SendConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Mouse.SetCursor(Cursors.Wait);
            // Attempt to connect to the wifi Access Point
            WlanClient.WlanInterface wlanIface = client.Interfaces[0];
            try
            {
                string profileName = m_selectedRacerMateAccessPointSSID;
                string mac = ConvertToHex(profileName).ToUpper();

                // find these profiles by running this command at an elevated cmd prompt: "netsh wlan export profile <Network Name>"
                string profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>manual</connectionMode><MSM><security><authEncryption><authentication>open</authentication><encryption>none</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>", profileName, mac);

                // This is one approach to connect to the networks: Set (create) a profile for the network, then connect using the profile name
                //wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                //if (wlanIface.ConnectSynchronously(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName, 10 * 1000) == false) // timeout is in milliseconds

                if (wlanIface.ConnectSynchronously(Wlan.WlanConnectionMode.TemporaryProfile, Wlan.Dot11BssType.Any, profileXml, 10 * 1000) == false)
                {
                    StatusText.Text = "Failed to connect.";
                    Mouse.SetCursor(Cursors.Arrow);
                    return;
                }
                else
                {
                    StatusText.Text = "Connected!";

                    // Run external exe
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"console.exe";
                    startInfo.Arguments = "\"" + SSID + "\" \"" + PasswordText.Password + "\" false";
                    startInfo.CreateNoWindow = true;
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
                            StatusText.Text = "Configured!";
                            m_numConfiguredTrainers++;
                        }
                        else
                        {
                            StatusText.Foreground = Brushes.Red;
                            StatusText.Text = error;
                            Mouse.SetCursor(Cursors.Arrow);
                            return;
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                StatusText.Text = exception.Message;
                Mouse.SetCursor(Cursors.Arrow);
                return;
            }

            // If the code got this far, then we connected to a RacerMate Access Point above, and now need to reconnect to the user's home network.
            string homeNetworkSSID = HomeNetworkNameDropDrop.SelectedItem.ToString();

            // first attempt to connect using the stored profile name, which is likely going to work for a user's home network
            bool bReconnected = false;
            try
            {
                bReconnected = wlanIface.ConnectSynchronously(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, homeNetworkSSID, 10 * 1000);
            }
            catch
            {
                bReconnected = false;
            }

            if (bReconnected == false)
            {
                string authentication = "open";
                string encryption = "none";
                string sharedKeyXml = "";
                bool bSupported = true;

                // if it didn't work, then attempt to generate a temporary profile xml so that we can connect.
                // NOTE: These profiles may not be correct for all configurations, so if any of these don't work, we simply prompt the user to reconnect to their network manually.
                switch(m_connectedNetwork.dot11DefaultAuthAlgorithm)
                {
                    case Wlan.Dot11AuthAlgorithm.IEEE80211_Open:
                        authentication = "open";
                        encryption = "none";
                        sharedKeyXml = "";
                        break;
                    case Wlan.Dot11AuthAlgorithm.IEEE80211_SharedKey:
                        authentication = "shared";
                        encryption = "WEP";
                        sharedKeyXml = string.Format("<sharedKey><keyType>networkKey</keyType><protected>false</protected><keyMaterial>{0}</keyMaterial></sharedKey>", PasswordText.Password);
                        break;
                    case Wlan.Dot11AuthAlgorithm.RSNA_PSK:
                        authentication = "WPA2PSK";
                        encryption = "AES";
                        sharedKeyXml = string.Format("<sharedKey><keyType>networkKey</keyType><protected>false</protected><keyMaterial>{0}</keyMaterial></sharedKey>", PasswordText.Password);
                        break;
                    case Wlan.Dot11AuthAlgorithm.WPA_PSK:
                        authentication = "WPAPSK";
                        encryption = "AES";
                        sharedKeyXml = string.Format("<sharedKey><keyType>networkKey</keyType><protected>false</protected><keyMaterial>{0}</keyMaterial></sharedKey>", PasswordText.Password);
                        break;
                    case Wlan.Dot11AuthAlgorithm.RSNA:
                    case Wlan.Dot11AuthAlgorithm.WPA:
                    default:
                        bSupported = false;
                        break;
                }

                if (bSupported)
                {
                    string homeProfileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><MSM><security><authEncryption><authentication>{2}</authentication><encryption>{3}</encryption><useOneX>false</useOneX></authEncryption>{4}<keyIndex>0</keyIndex></security></MSM></WLANProfile>", homeNetworkSSID, ConvertToHex(homeNetworkSSID), authentication, encryption, sharedKeyXml);
                    try {
                        bReconnected = wlanIface.ConnectSynchronously(Wlan.WlanConnectionMode.TemporaryProfile, Wlan.Dot11BssType.Any, homeProfileXml, 10 * 1000);
                    }
                    catch
                    {
                        bReconnected = false;
                    }
                }
            }

            // see if either the first or second attempt to connect was successful
            if (bReconnected == false)
            {
                // there was an error, so display a message to the user telling them they'll have to reconnect to their own home network.
                Ask msgbox = new Ask("RacerMateOne is unable to reconnect to your home wireless network, please manually reconnect to '" + homeNetworkSSID + "'.", "OK", "Cancel");
                msgbox.ShowDialog();
            }
            StatusText.Text = "Complete!";

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SSID = null;
            Password = null;
            Close();
        }

        private void RMAccessPoint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RacerMateAccessPointsDropDown.SelectedItem != null)
            {
                m_selectedRacerMateAccessPointSSID = RacerMateAccessPointsDropDown.SelectedItem.ToString();
            }
            else
            {
                m_selectedRacerMateAccessPointSSID = string.Empty;
            }
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
