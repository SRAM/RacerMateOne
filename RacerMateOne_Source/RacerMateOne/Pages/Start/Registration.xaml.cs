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
using System.Diagnostics;
namespace RacerMateOne.Pages.Start
{
	/// <summary>
	/// Interaction logic for Registration.xaml
	/// </summary>
	public partial class Registration : Page
	{
        static bool ms_Start = true;

		const String c_ExampleSerialNumber = "CDKEY-ENTER-HEREX-XXXXX-XXXXX";
		const String c_ExampleEmail = "your@RM1EmailAccount.com";
		const String c_FirstName = "FirstName";
		const String c_LastName = "LastName";
        const String c_TitleNote = "(Commonly for computers not connected to the Internet)";
        const String c_ManualRegistration = "Manual Registration";
        const String c_Registration = "Registration";

        static String ms_FirstName = c_FirstName;
		static String ms_LastName = c_LastName;
        static String ms_SerialNumber = c_ExampleSerialNumber;
		static String ms_Email = c_ExampleEmail;
        static bool ms_NewAccount = true;
        static public String ms_Passw1 = "";
        static public String ms_Passw2 = "";

        public bool CheckingConnection = false;
        public bool ManualRegistration = false;
        public String m_Passw1 = "";
        public String m_Passw2 = "";
        public String m_errormsg = "";

		public Registration()
		{
			InitializeComponent();
            t_TitleMsg.Text = ManualRegistration ? c_ManualRegistration : c_Registration;
            t_TitleNote.Text = ManualRegistration ? c_TitleNote : "";
        }

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
            if (CheckingConnection)
            {
                Registration_Send reg = new Registration_Send();
                NavigationService.Navigate(reg);
            }
            else
            {
                //Random rand = new Random();
                t_HardwareID.Text = App.GetHardwareID();
                t_FirstName.Text = ms_FirstName;
                t_LastName.Text = ms_LastName;
                t_SerialNumberManual.Text = ms_SerialNumber;
                t_SerialNumber.Text = ms_SerialNumber;
                t_EmailManual.Text = ms_Email;
                t_Email.Text = ms_Email;
                t_CheckEmailManual.Text = "";
                t_CheckEmail.Text = "";
                t_ErrorMsg.Text = m_errormsg;

                m_Passw1 = ms_Passw1 = "";
                m_Passw2 = ms_Passw2 = "";

                //t_Password.Password = ms_Passw1;
                //t_CheckPassword.Password = ms_Passw2;

                t_NewAccount.IsChecked = ms_NewAccount;
                t_ExistingAccount.IsChecked = !ms_NewAccount;

                t_Auto.IsChecked = !ManualRegistration;

                b_StartReg.Visibility = ms_Start ? Visibility.Visible : Visibility.Collapsed;
                b_ManualReg.Visibility = (!ms_Start && ManualRegistration) ? Visibility.Visible : Visibility.Collapsed;
                b_AutoReg.Visibility = (!ms_Start && !ManualRegistration) ? Visibility.Visible : Visibility.Collapsed;
                t_TitleMsg.Text = ManualRegistration ? c_ManualRegistration : c_Registration;
                t_TitleNote.Text = ManualRegistration ? c_TitleNote : "";

                //t_Auto.Visibility = (!ms_Start && ManualRegistration) ? Visibility.Visible : Visibility.Hidden;

                b_CheckEmail.Visibility = b_FirstName.Visibility = b_LastName.Visibility = b_ConfirmPassword.Visibility = (t_NewAccount.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
                b_ErrorMsg.Visibility = (0 >= t_ErrorMsg.Text.Length) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
            if (!CheckingConnection)
            {
                ms_FirstName = t_FirstName.Text;
                ms_LastName = t_LastName.Text;
                if (ManualRegistration)
                {
                    ms_SerialNumber = t_SerialNumberManual.Text;
                    ms_Email = t_EmailManual.Text;
                    t_CheckEmail.Text = t_CheckEmailManual.Text;
                }
                else
                {
                    ms_SerialNumber = t_SerialNumber.Text;
                    ms_Email = t_Email.Text;
                    t_CheckEmailManual.Text = t_CheckEmail.Text;
                }
                t_SerialNumberManual.Text = ms_SerialNumber;
                t_SerialNumber.Text = ms_SerialNumber;
                t_EmailManual.Text = ms_Email;
                t_Email.Text = ms_Email;
                m_Passw1 = ms_Passw1;
                m_Passw2 = ms_Passw2;

                //ms_Passw1 = t_Password.Password;
                //ms_Passw2 = t_CheckPassword.Password;

                ms_NewAccount = t_NewAccount.IsChecked == true;
            }
		}


		private void Help_Click(object sender, RoutedEventArgs e)
		{
            AppWin.Help("Register_RM1.htm");
		}

		private void t_Register_Click(object sender, RoutedEventArgs e)
		{
            if (ms_Start)
            {
                ms_Start = false;
                CheckingConnection = true;
            }
            ms_Passw1 = m_Passw1;
            ms_Passw2 = m_Passw2;
            t_ErrorMsg.Text = m_errormsg = "";
            Registration_Send reg = new Registration_Send();
            NavigationService.Navigate(reg);
		}

		private void t_RegisterOffline_Click(object sender, RoutedEventArgs e)
		{
			AppWin.SetPreviewMode();
			NavigationService.Navigate(new Pages.Start.PreviewWarning());
            //MessageBox.Show(string.Format("TO DO - Normally this will exit the application here, or put RM1 in Demo mode. But for now, not being registered will let testers test still."));

            // demo or trial mode here
            //NavigationService.Navigate(AppWin.Instance.Page_Splash);

            // Exits for now until Demo or Trial mode is in place
            // AppWin.Exit();
        }

        protected void Password1_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            m_Passw1 = t_Password.Password;
            //Debug.WriteLine(t_Password.Password);
        }

        protected void Password2_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            m_Passw2 = t_CheckPassword.Password;
            //Debug.WriteLine(t_CheckPassword.Password);
        }

        private void NewAccount_Unchecked(object sender, RoutedEventArgs e)
        {
            t_ErrorMsg.Text = m_errormsg = "";
            b_ErrorMsg.Visibility = (0 >= t_ErrorMsg.Text.Length) ? Visibility.Collapsed : Visibility.Visible;
            b_CheckEmail.Visibility = b_FirstName.Visibility = b_LastName.Visibility = b_ConfirmPassword.Visibility = (t_NewAccount.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ExistingAccount_Unchecked(object sender, RoutedEventArgs e)
        {
            t_ErrorMsg.Text = m_errormsg = "";
            b_ErrorMsg.Visibility = (0 >= t_ErrorMsg.Text.Length) ? Visibility.Collapsed : Visibility.Visible;
            b_CheckEmail.Visibility = b_FirstName.Visibility = b_LastName.Visibility = b_ConfirmPassword.Visibility = (t_NewAccount.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Auto_OnClick(object sender, RoutedEventArgs e)
        {
            if (ManualRegistration)
            {
                ms_SerialNumber = t_SerialNumberManual.Text;
                ms_Email = t_EmailManual.Text;
            }
            else
            {
                ms_SerialNumber = t_SerialNumber.Text;
                ms_Email = t_Email.Text;
            }

            ManualRegistration = !(t_Auto.IsChecked == true);

            t_SerialNumberManual.Text = ms_SerialNumber;
            t_SerialNumber.Text = ms_SerialNumber;
            t_EmailManual.Text = ms_Email;
            t_Email.Text = ms_Email;
            b_StartReg.Visibility = ms_Start ? Visibility.Visible : Visibility.Collapsed;
            b_ManualReg.Visibility = (!ms_Start && ManualRegistration) ? Visibility.Visible : Visibility.Collapsed;
            b_AutoReg.Visibility = (!ms_Start && !ManualRegistration) ? Visibility.Visible : Visibility.Collapsed;
            t_TitleMsg.Text = ManualRegistration ? c_ManualRegistration : c_Registration;
            t_TitleNote.Text = ManualRegistration ? c_TitleNote : "";
            //t_Auto.Visibility = (!ms_Start && ManualRegistration) ? Visibility.Visible : Visibility.Hidden;

            t_ErrorMsg.Text = m_errormsg = "";
            b_ErrorMsg.Visibility = (0 >= t_ErrorMsg.Text.Length) ? Visibility.Collapsed : Visibility.Visible;
            b_CheckEmail.Visibility = b_FirstName.Visibility = b_LastName.Visibility = b_ConfirmPassword.Visibility = (t_NewAccount.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LogoAbout_Loaded(object sender, RoutedEventArgs e)
        {

        }

		// This hides the text and shows it again.
		// =======================================
		private void t_SerialNumber_GotFocus(object sender, RoutedEventArgs e)
		{
			if (t_SerialNumber.Text == c_ExampleSerialNumber)
				t_SerialNumber.Text = "";
		}

		private void t_SerialNumber_LostFocus(object sender, RoutedEventArgs e)
		{
			if (t_SerialNumber.Text.Trim() == "")
				t_SerialNumber.Text = c_ExampleSerialNumber;
		}

		private void t_SerialNumberManual_GotFocus(object sender, RoutedEventArgs e)
		{
			if (t_SerialNumberManual.Text == c_ExampleSerialNumber)
				t_SerialNumberManual.Text = "";

		}
		private void t_SerialNumberManual_LostFocus(object sender, RoutedEventArgs e)
		{
			if (t_SerialNumberManual.Text.Trim() == "")
				t_SerialNumberManual.Text = c_ExampleSerialNumber;

		}

		private void t_Email_GotFocus(object sender, RoutedEventArgs e)
		{
			if (t_Email.Text == c_ExampleEmail)
				t_Email.Text = "";

		}

		private void t_Email_LostFocus(object sender, RoutedEventArgs e)
		{
			if (t_Email.Text.Trim() == "")
				t_Email.Text = c_ExampleEmail;

		}

		private void t_FirstName_GotFocus(object sender, RoutedEventArgs e)
		{
			if (t_FirstName.Text == c_FirstName)
				t_FirstName.Text = "";

		}

		private void t_FirstName_LostFocus(object sender, RoutedEventArgs e)
		{
			if (t_FirstName.Text.Trim() == "")
				t_FirstName.Text = c_FirstName;

		}

		private void t_LastName_GotFocus(object sender, RoutedEventArgs e)
		{
			if (t_LastName.Text == c_LastName)
				t_LastName.Text = "";

		}

		private void t_LastName_LostFocus(object sender, RoutedEventArgs e)
		{
			if (t_LastName.Text.Trim() == "")
				t_LastName.Text = c_LastName;

		}

        private void t_EmailManual_GotFocus(object sender, RoutedEventArgs e)
        {
            if (t_EmailManual.Text == c_ExampleEmail)
                t_EmailManual.Text = "";
        }

        private void t_EmailManual_LostFocus(object sender, RoutedEventArgs e)
        {
            if (t_EmailManual.Text.Trim() == "")
                t_EmailManual.Text = c_ExampleEmail;
        }





    }
}
