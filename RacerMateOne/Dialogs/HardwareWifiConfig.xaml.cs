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
using System.Net.NetworkInformation;

namespace RacerMateOne.Dialogs
{
    /// <summary>
    /// Interaction logic for HardwareWifiConfig.xaml
    /// </summary>
    public partial class HardwareWifiConfig : Window
    {
        private string m_selectedRacerMateAccessPointSSID = string.Empty;
        private WlanClient m_client = new WlanClient();
		bool m_hasInitialConnectedNetwork = false;
        Wlan.WlanAvailableNetwork m_connectedNetwork;
        string m_connectedSSID = string.Empty;
        List<Wlan.WlanAvailableNetwork> m_racermateNetworks = new List<Wlan.WlanAvailableNetwork>();
        List<Wlan.WlanAvailableNetwork> m_otherNetworks = new List<Wlan.WlanAvailableNetwork>();
        int m_numConfiguredTrainers = 0;
		List<string> m_detectedIPAddresses = new List<string>();

        public HardwareWifiConfig()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
			Refresh_Click(sender, e);
        }

        /// <summary>
        /// Converts a 802.11 SSID to a string.
        /// </summary>
        private static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        public static bool VerifyWifiSupport()
        {
			WlanClient client = new WlanClient();
			bool wifiSupported = (client.Interfaces.Length > 0);
            if (!wifiSupported)
            {
                Ask dialog = new Ask("Your computer MUST have wireless network access to use this setup wizard.", "OK", "Cancel");
                dialog.ShowDialog();
            }
			return wifiSupported;
		}

        private void UpdateDialogState()
        {
            bool hasWifiSupport = m_client.Interfaces.Length > 0;

			HomeNetworkNameDropDrop.IsEnabled = hasWifiSupport;
			PasswordTextPlain.IsEnabled = hasWifiSupport;
			PasswordText.IsEnabled = hasWifiSupport;
			ShowPasswordCheckBox.IsEnabled = hasWifiSupport;
			Send.IsEnabled = hasWifiSupport;
			OK.IsEnabled = hasWifiSupport;

            //if (CurrentIPAddressBox.Items.Count == 0)
            //{
            //    CurrentIPAddressBox.Items.Add("<None found>");
            //    CurrentIPAddressBox.SelectedIndex = 0;
            //    CurrentIPAddressBox.IsEnabled = false;
            //}
            //else
            //{
            //    CurrentIPAddressBox.IsEnabled = true;
            //}

            if (HomeNetworkNameDropDrop.Items.Count == 0)
			{
				HomeNetworkNameDropDrop.Items.Add("<None found>");
				HomeNetworkNameDropDrop.SelectedIndex = 0;
				HomeNetworkNameDropDrop.IsEnabled = false;
			}
            else
            {
                HomeNetworkNameDropDrop.IsEnabled = true;
            }

            if (RacerMateAccessPointsDropDown.Items.Count == 0)
			{
				RacerMateAccessPointsDropDown.Items.Add("<None found>");
				RacerMateAccessPointsDropDown.SelectedIndex = 0;
				RacerMateAccessPointsDropDown.IsEnabled = false;
			}
            else
            {
                RacerMateAccessPointsDropDown.IsEnabled = true;
            }

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
            Mouse.SetCursor(Cursors.Wait);
            foreach (WlanClient.WlanInterface wlanIface in m_client.Interfaces)
            {
                // returns immediately, but may take up to 4 seconds for the scan to complete in the background
                wlanIface.Scan();
            }

            // sleep for 4 seconds to let the scan complete
            System.Threading.Thread.Sleep(4000);

            RacerMateAccessPointsDropDown.Items.Clear();
            HomeNetworkNameDropDrop.Items.Clear();
			m_hasInitialConnectedNetwork = false;
            m_connectedSSID = string.Empty;
            m_connectedNetwork = new Wlan.WlanAvailableNetwork();

            m_detectedIPAddresses.Clear();
//			CurrentIPAddressBox.Items.Clear();
			if (RM1_Settings.General.WifiOverrideIPAddress != string.Empty)
			{
//				CurrentIPAddressBox.Items.Add(RM1_Settings.General.WifiOverrideIPAddress + " (WifiOverrideIPAddress)");
				m_detectedIPAddresses.Add(RM1_Settings.General.WifiOverrideIPAddress);
			}

            // if there wasn't an override IP address in the settings, 
            // then get the user's current IP address (which we assume 
            // is assigned by their home network and prefered network interface - wifi vs ethernet).
            // There are probably many better ways to do this, but this was the easiest solution I found
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            m_detectedIPAddresses.Add(ip.Address.ToString());
//                            CurrentIPAddressBox.Items.Add(ip.Address.ToString() + " (" + ni.Description + ")");
                        }
                    }
                }
            }

            //if (CurrentIPAddressBox.HasItems)
            //{
            //    CurrentIPAddressBox.SelectedIndex = 0;
            //}

            // Sort the available wireless networks into "home" networks and "RacerMate" networks.
            foreach (WlanClient.WlanInterface wlanIface in m_client.Interfaces)
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
                    if (m_connectedSSID == availableSSID)
                    {
                        // we're already connected to this network and have added it to our dropdown lists, so skip it
                        continue;
                    }

                    if ((network.flags & Wlan.WlanAvailableNetworkFlags.Connected) == Wlan.WlanAvailableNetworkFlags.Connected)
                    {
						m_hasInitialConnectedNetwork = true;
						m_connectedNetwork = network;
                        m_connectedSSID = availableSSID;
                    }

                    if (availableSSID.StartsWith("RacerMateCT"))
                    {
                        m_racermateNetworks.Add(network);
                        RacerMateAccessPointsDropDown.Items.Add(availableSSID);
                    }
                    else
                    {
                        // tells us that the network is available
                        m_otherNetworks.Add(network);
                        HomeNetworkNameDropDrop.Items.Add(availableSSID);
                    }
                }
            }

            // Auto-select the network that we think is their home network
            if (HomeNetworkNameDropDrop.Items.Contains(m_connectedSSID))
            {
                HomeNetworkNameDropDrop.SelectedItem = m_connectedSSID;
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
            Mouse.SetCursor(Cursors.Arrow);
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
            // validate state:
            //if (m_detectedIPAddresses.Count == 0 ||
            //    CurrentIPAddressBox.HasItems == false ||
            //    CurrentIPAddressBox.SelectedItem.ToString().Contains("Not found") ||
            //    CurrentIPAddressBox.IsEnabled == false)
            //{
            //    StatusText.Foreground = Brushes.Red;
            //    StatusText.Text = "Missing IP Address";
            //    return;
            //}

            if (HomeNetworkNameDropDrop.HasItems == false ||
				HomeNetworkNameDropDrop.SelectedItem.ToString().Contains("Not found") || 
				HomeNetworkNameDropDrop.IsEnabled == false)
			{
				StatusText.Foreground = Brushes.Red;
				StatusText.Text = "Missing Home SSID";
				return;
			}

			if ((PasswordText.Password.Length == 0 && PasswordTextPlain.Text.Length == 0))
			{
				StatusText.Foreground = Brushes.Red;
				StatusText.Text = "Missing SSID Password";
				return;
			}

			if (RacerMateAccessPointsDropDown.HasItems == false ||
				RacerMateAccessPointsDropDown.SelectedItem.ToString().Contains("Not found") ||
				RacerMateAccessPointsDropDown.IsEnabled == false)
			{
				StatusText.Foreground = Brushes.Red;
				StatusText.Text = "Missing Access Point";
				return;
			}

			Mouse.SetCursor(Cursors.Wait);

            // Attempt to connect to the wifi Access Point
            bool bConnectedToHandlebarAccessPoint = false;
            WlanClient.WlanInterface wlanIface = m_client.Interfaces[0];
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
                }
                else
                {
                    StatusText.Text = "Connected...";
                    bConnectedToHandlebarAccessPoint = true;
                }
            }
            catch (Exception exception)
            {
                StatusText.Text = exception.Message;
                Mouse.SetCursor(Cursors.Arrow);
            }

            // if we were able to connect to the handlebar access point,
            // then try to configure the wifi settings in the handlebar
            bool bConfiguredWifiSettings = false;
            if (bConnectedToHandlebarAccessPoint)
            {
                try
                {
					string SSID = HomeNetworkNameDropDrop.SelectedItem.ToString();
					string password = (ShowPasswordCheckBox.IsChecked == true) ? PasswordTextPlain.Text : PasswordText.Password;
//					string ipAddress = (m_detectedIPAddresses.Count == 0) ? "10.10.100.150" : m_detectedIPAddresses[CurrentIPAddressBox.SelectedIndex];
					string ipAddress = "10.10.100.150";

					Log.WriteLine("Launching: console.exe \"" + SSID + "\" \"<password>\" false \"" + RM1_Settings.General.WifiListenPort + "\" \"" + ipAddress + "\"");

					// Run external exe
					ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"console.exe";
                    startInfo.Arguments = "\"" + HomeNetworkNameDropDrop.SelectedItem.ToString() + "\" \"" + password + "\" false \"" + RM1_Settings.General.WifiListenPort + "\" \"" + ipAddress + "\"";
                    startInfo.CreateNoWindow = true;
                    startInfo.ErrorDialog = true;
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    Process console = Process.Start(startInfo);

                    // wait 60 seconds for it to complete, if successful it will be quick, but console.exe can take up to 45 seconds to timeout.
                    if (console.WaitForExit(60 * 1000) == false)
                    {
                        StatusText.Foreground = Brushes.Red;
                        StatusText.Text = "Timed out.";
                        console.Kill();
						Log.WriteLine("Console.exe timed out, so RM1 killed it.");
                    }
                    else
                    {
                        int exitCode = console.ExitCode;
                        System.IO.StreamReader stdOut = console.StandardOutput;
                        System.IO.StreamReader stdErr = console.StandardError;

                        // parse result
                        string error = stdErr.ReadToEnd().Trim();
                        string output = stdOut.ReadToEnd().Trim();
						Log.WriteLine("Console.exe stderr = '" + error + "'");
						Log.WriteLine("Console.exe stdout = '" + output + "'");

						bool isOutputOK = output.StartsWith("OK");

                        // update status
                        if (error.Length > 0 && !isOutputOK)
                        {
                            StatusText.Foreground = Brushes.Red;
                            StatusText.Text = error.Trim();
							Log.WriteLine("Console.exe considered an error.");
						}
						else if (!isOutputOK)
                        {
                            StatusText.Foreground = Brushes.Red;
                            StatusText.Text = output.Trim();
							Log.WriteLine("Console.exe considered not good.");
						}
						else if (isOutputOK)
                        {
                            StatusText.Foreground = Brushes.Green;
                            StatusText.Text = "Configured!";
                            bConfiguredWifiSettings = true;
                            m_numConfiguredTrainers++;
							Log.WriteLine("Console.exe considered Success!");
                        }
                        else
                        {
                            StatusText.Foreground = Brushes.Red;
                            StatusText.Text = "Failed to configure.";
							Log.WriteLine("Console.exe did something unexpected.");
						}
					}
                }
                catch (Exception exception)
                {
                    StatusText.Foreground = Brushes.Red;
                    StatusText.Text = exception.Message;
					Log.WriteLine("Caught Exception while trying to run console.exe.");
				}
			}

            // If the code got this far, then we connected to a RacerMate Access Point above, and now need to reconnect to the user's home network.
            string homeNetworkSSID = HomeNetworkNameDropDrop.SelectedItem.ToString();

            // first attempt to connect using the stored profile name, which is likely going to work for a user's home network
            bool bReconnected = false;
            if (bConnectedToHandlebarAccessPoint && m_hasInitialConnectedNetwork)
            {
				Log.WriteLine("Trying to reconnect to the user's home network.");
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
					Log.WriteLine("Reconnecting based on network profile didn't work, trying the complicated way.");
					string authentication = "open";
                    string encryption = "none";
                    string sharedKeyXml = "";
                    bool bSupported = true;

                    // if it didn't work, then attempt to generate a temporary profile xml so that we can connect.
                    // NOTE: These profiles may not be correct for all configurations, so if any of these don't work, we simply prompt the user to reconnect to their network manually.
                    switch (m_connectedNetwork.dot11DefaultAuthAlgorithm)
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
                        catch(Exception e2)
                        {
                            bReconnected = false;
							Log.WriteLine("Reconnecting failed using the complicated way: '" + e2.Message + "'.");
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
            }

			if (bConfiguredWifiSettings)
			{
				// racermate.dll will scan for new hardware every 5 seconds,
				// so wait 5 seconds so that it can update with the new trainer
				System.Threading.Thread.Sleep(5000);

				StatusText.Foreground = Brushes.Green;
				StatusText.Text = "Complete!";

				Log.WriteLine("The wifi settings were configured, so this was a success!");
			}
			else
			{
				Log.WriteLine("The wifi settings were NOT configured, this was a failure!");
			}

			Mouse.SetCursor(Cursors.Arrow);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
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

		private void ShowPasswordCheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (ShowPasswordCheckBox.IsChecked == true)
			{
				PasswordTextPlain.Text = PasswordText.Password;
				PasswordText.Visibility = Visibility.Collapsed;
				PasswordTextPlain.Visibility = Visibility.Visible;
			}
			else
			{
				PasswordText.Password = PasswordTextPlain.Text;
				PasswordTextPlain.Visibility = Visibility.Collapsed;
				PasswordText.Visibility = Visibility.Visible;
			}
		}
	}
}
