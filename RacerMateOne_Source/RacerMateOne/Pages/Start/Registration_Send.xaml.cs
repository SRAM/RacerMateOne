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
using System.Threading;
using System.Text.RegularExpressions;	

namespace RacerMateOne.Pages.Start
{
	/// <summary>
	/// Interaction logic for Scanning.xaml
	/// </summary>
	public partial class Registration_Send : Page
	{
        static string hardwareid = App.GetHardwareID();
        static string firstname = "";
        static string lastname = "";
        //static string company = "";
        static string cdkey = "";
        static string email = "";
        static string email2 = "";
        static string passwd1 = "";
        static string passwd2 = "";

        static string keycode = "";
        static bool checking = false;
        static bool manualreg = false;
        static bool newaccount = false;

        static string errormsg = "";

        static bool tryagain = false;

        public Registration_Send()
		{
			InitializeComponent();
            checking = AppWin.Instance.Page_Registration.CheckingConnection;
            if (checking)
                LoadingMessage.Content = "Checking internet connection";
            else
                LoadingMessage.Content = "Submitting registration information";

            errormsg = "";
        }

        static Thread ms_WebThread;
        public static void Register()
        {
            try
            {
                if (ms_WebThread != null)
                    ms_WebThread.Join();		// We will have to wait.
                ms_WebThread = new Thread(new ThreadStart(WebThread));
                ms_WebThread.Start();
            }
            catch { }
        }

        static void WebThread()
        {
            App.SetDefaultCulture();

            //string WebPage = "http://store.racermateinc.com/store/registration.php?";
            if (checking)
            {
                string results = BrowserClient.GetRM1("");
                if (results.Length <= 0)
                {
                    manualreg = true;
                    //MessageBox.Show(string.Format("Checking. Failed to connect to website {1}. Your HardwareID is {0}. Please go to website {1} to register and get your keycode.", hardwareid, WebPage));
                }
                else
                {
                    manualreg = false;
                    //MessageBox.Show(string.Format("Checking. Connected to website {1}. Your HardwareID is {0}.", hardwareid, WebPage));
                }
            }
            else
            {
                if (!manualreg)
                {
                    string inparse = @"\<strong id=\""racermatekeycoderesults\""\>([A-Z0-9]+-[A-Z0-9]+-[A-Z0-9]+-[A-Z0-9]+)\<\/strong\>";
                    string WebPage = "http://store.racermateinc.com/store/registration_noui_login.php";
                    string postdata = "";
                    if (!newaccount)
                    {
                        //inparse = @"\<strong\>([A-Z0-9]+-[A-Z0-9]+-[A-Z0-9]+-[A-Z0-9]+)\<\/strong\>";
                        postdata =
                            "usertype=C" + 
                            "&anonymous=" + 
                            "&password_is_modified=N" + 
                            "&pending_membershipid=0" + 

                            "&hardwareid=" + Uri.EscapeDataString(hardwareid) +
                            "&cdkey=" + Uri.EscapeDataString(cdkey) +
                            "&email=" + Uri.EscapeDataString(email) +
                            "&passwd1=" + Uri.EscapeDataString(passwd1);
                    }
                    else
                    {
                        //inparse = @"\<strong id=\""racermatekeycoderesults\""\>([A-Z0-9]+-[A-Z0-9]+-[A-Z0-9]+-[A-Z0-9]+)\<\/strong\>";
                        WebPage = "http://store.racermateinc.com/store/registration_noui.php";
                        postdata =
                            "usertype=C" + 
                            "&anonymous=" + 
                            "&password_is_modified=N" + 
                            "&pending_membershipid=C" + 

                            "&firstname=" + Uri.EscapeDataString(firstname) +
                            "&lastname=" + Uri.EscapeDataString(lastname) +
                            //"&company=" + Uri.EscapeDataString(company) +
                            "&hardwareid=" + Uri.EscapeDataString(hardwareid) +
                            "&cdkey=" + Uri.EscapeDataString(cdkey) +
                            "&email=" + Uri.EscapeDataString(email) +
                            "&passwd1=" + Uri.EscapeDataString(passwd1) +
                            "&passwd2=" + Uri.EscapeDataString(passwd2);
                    }

                    string results = BrowserClient.PostRM1(WebPage, postdata);
                    if (results.Length <= 0)
                    {
                        manualreg = true;
                        errormsg = "Please check internet connection and try again";
                        //MessageBox.Show(string.Format("Failed to register to website {1}. Your HardwareID is {0}. Please go to website {1} to register and get your keycode.", hardwareid, WebPage));
                    }
                    else
                    {
                        manualreg = false;
                        keycode = "BADXX-KEYXX-CODEX-XXXXX";

                        /*
                        string findstr = "Your keycode is <strong>";
                        string startstr = "      Thank you for registering your product.  Your keycode is <strong>";
                        //string endstr = "</strong><br/><br/>";

                        string[] blk = results.Split('\n');
                        foreach (string str in blk)
                        {
                            if (str.Contains(findstr))
                            {
                                keycode = str.Substring(startstr.Length, keycode.Length);
                                break;
                            }
                        }
                         * */

                        // * Saving this routine by Will for extracting keycode. 
                        //inparse = @"\<strong id=\""racermatekeycoderesults\""\>([A-Z0-9]+-[A-Z0-9]+-[A-Z0-9]+-[A-Z0-9]+)\<\/strong\>";
                        Regex regexp_words = new Regex(inparse);
                        Match cc = regexp_words.Match(results);
                        if (cc.Success)
                            keycode = cc.Groups[1].ToString().Trim();
                        else
                        {
                            //if (!newaccount)
                            //    inparse = @"\<strong\>([^\<]*)\<\/strong\>";
                            //else
                            inparse = @"\<strong id=\""racermatekeycoderesults\""\>([^\<]*)\<\/strong\>";
                            Regex regexp_words2 = new Regex(inparse);
                            Match cc2 = regexp_words2.Match(results);
                            if (cc2.Success)
                            {
                                errormsg = cc2.Groups[1].ToString().Trim();
                            }
                            else
                            {
                                errormsg = results;
                                if(errormsg.Contains("Email") && newaccount) 
                                {
                                    tryagain = true;
                                }
                            }
                            //else // todo 
                            //    errormsg = results;
                            /*
                            if (!newaccount)
                                MessageBox.Show(string.Format("Debug Msg. Register error - {0}", results));
                            else
                                MessageBox.Show("Debug Msg. Failed to register");
                             */
                        }

                        // * /


                        // Parse results for keycode
                        // keycode = "DDKSZ-4APHB-CI652-67KJW";
                        //MessageBox.Show(string.Format("Connected to website {1}. Your HardwareID is {0}.", hardwareid, WebPage));
                    }
                }
            }

            ms_WebThread = null;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}

        DispatcherTimer m_timer;
        private void Page_Loaded(object sender, RoutedEventArgs e)
		{
            // Just an example to wait for something.
            m_timer = new DispatcherTimer(); // Fake registration time
            m_timer.Tick += new EventHandler(timerTick);
            m_timer.Interval = new TimeSpan(0, 0, 1);
            m_timer.Start();

            checking = AppWin.Instance.Page_Registration.CheckingConnection;
            manualreg = AppWin.Instance.Page_Registration.ManualRegistration;

            bool validated = false;
            if (!checking)
            {
                keycode = AppWin.Instance.Page_Registration.t_Keycode.Text;
                //string company = AppWin.Instance.Page_Registration.t_Company.Text;
                cdkey = AppWin.Instance.Page_Registration.t_SerialNumber.Text;
                firstname = AppWin.Instance.Page_Registration.t_FirstName.Text;
                lastname = AppWin.Instance.Page_Registration.t_LastName.Text;
                email = AppWin.Instance.Page_Registration.t_Email.Text;
                email2 = AppWin.Instance.Page_Registration.t_CheckEmail.Text;
                passwd1 = AppWin.Instance.Page_Registration.m_Passw1;
                passwd2 = AppWin.Instance.Page_Registration.m_Passw2;
                newaccount = AppWin.Instance.Page_Registration.t_NewAccount.IsChecked == true;
                if (tryagain)
                {
                    tryagain = false;
                    if(newaccount)
                        newaccount = false;
                }

                errormsg = "";
                if (manualreg)
                {
                    if (0 >= cdkey.Trim().Length)
                        errormsg = "CD Key required";
                    else if (0 >= keycode.Trim().Length)
                        errormsg = "Keycode required";
                    else if (0 >= email.Trim().Length)
                        errormsg = "Email required";
                    else if (0 >= email2.Trim().Length)
                        errormsg = "Confirm email required";
                    else if (email != email2)
                        errormsg = "Emails did not match";

                    validated = (
                        0 < cdkey.Trim().Length &&
                        0 < keycode.Trim().Length &&
                        0 < email.Trim().Length &&
                        0 < email2.Trim().Length &&
                        email == email2
                        );
                }
                else
                {
                    if (newaccount)
                    {
                        if (0 >= firstname.Trim().Length)
                            errormsg = "First name required";
                        else if (0 >= lastname.Trim().Length)
                            errormsg = "Last name required";
                        else if (0 >= cdkey.Trim().Length)
                            errormsg = "CD Key required";
                        else if (0 >= email.Trim().Length)
                            errormsg = "Email required";
                        else if (0 >= email2.Trim().Length)
                            errormsg = "Confirm email required";
                        else if (email != email2)
                            errormsg = "Emails did not match";
                        else if (0 >= passwd1.Trim().Length)
                            errormsg = "Password required";
                        else if (0 >= passwd2.Trim().Length)
                            errormsg = "Confirm password required";
                        else if (passwd1 != passwd2)
                            errormsg = "Passwords did not match";

                        validated = (
                            0 < firstname.Trim().Length &&
                            0 < lastname.Trim().Length &&
                            0 < cdkey.Trim().Length &&
                            0 < email.Trim().Length &&
                            0 < email2.Trim().Length &&
                            0 < passwd1.Trim().Length &&
                            0 < passwd2.Trim().Length &&
                            passwd1 == passwd2 &&
                            email == email2
                            );
                    }
                    else
                    {
                        if (0 >= cdkey.Trim().Length)
                            errormsg = "CD Key required";
                        else if (0 >= email.Trim().Length)
                            errormsg = "Email required";
                        else if (0 >= passwd1.Trim().Length)
                            errormsg = "Password required";

                        validated = (
                            0 < cdkey.Trim().Length &&
                            0 < email.Trim().Length &&
                            0 < passwd1.Trim().Length
                            );
                    }
                }
            }
            if(checking || (validated && !manualreg))
                Register();
            //if(!checking && !validated)
            //    MessageBox.Show("Debug msg. Failed to register. Make sure all fields are filled out");
        }

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			// Better stop anything else that is not working.
			m_timer.Stop();
            if (ms_WebThread != null)
            {
                ms_WebThread.Abort();
                ms_WebThread = null;
            }
		}
		private void timerTick(object sender, EventArgs e)
		{
            m_timer.Stop();
            if (ms_WebThread != null)
            {
                m_timer.Start();
            }
            else
            {
                if (checking)
                {
                    // there is a connection
                    checking = false;
                    AppWin.Instance.Page_Registration.CheckingConnection = checking;
                    AppWin.Instance.Page_Registration.ManualRegistration = manualreg;

                    //AppWin.Instance.Page_Registration.t_ErrorMsg.Text = errormsg;

                    NavigationService.Navigate(AppWin.Instance.Page_Registration);
                }
                else
                {
                    AppWin.Instance.Page_Registration.ManualRegistration = manualreg;

                    //MessageBox.Show(string.Format("Your Keycode is {0}.", keycode));
                    if (0 < errormsg.Length || !AppWin.TryRegister(keycode, hardwareid, cdkey, email, false))
                    {
                        if (!tryagain)
                        {
                            if (0 >= errormsg.Length)
                            {
                                errormsg = "Failed to register, please check all fields";
                            }
                            //checking = true; // If failed to register, then do a internet connection again
                            //AppWin.Instance.Page_Registration.CheckingConnection = checking;
                            AppWin.Instance.Page_Registration.m_errormsg = errormsg;
                            NavigationService.Navigate(AppWin.Instance.Page_Registration);
                        }
                        else
                        {
                            errormsg = "";
                            Registration_Send reg = new Registration_Send();
                            NavigationService.Navigate(reg);
                        }
                    }
                    else
                    {
                        // We failed.
                        RacerMateOne.Dialogs.Ask ask = new RacerMateOne.Dialogs.Ask("Thank you for registering RacerMate One.", null, "OK");
                        ask.ShowDialog();

                        // Here is what we do if we are registered
                        NavigationService.Navigate(AppWin.Instance.Page_Splash);
                    }
                }
            }
		}
	}
}
