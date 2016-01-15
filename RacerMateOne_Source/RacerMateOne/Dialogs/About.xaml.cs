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
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class About : Window
	{
		public RM1.Trainer Trainer;
		public About()
		{
			InitializeComponent();
		}

		bool m_bClosing;

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			m_bClosing = true;
			Fade.Stop();
			FadeAnim.To = 0.0f;
			Fade.Begin();
		}


		private void FadeAnim_Completed(object sender, EventArgs e)
		{
			if (m_bClosing)
				Close();
		}

		private void aboutBox_Loaded(object sender, RoutedEventArgs e)
		{
			string regCode = "", CDKey = "", Email = "";
			bool bRegistered = AppWin.GetRegistrationInfo(ref regCode, ref CDKey, ref Email);

			t_Version.Content = AppWin.Version;
			t_DLLVersion.Content = RM1.DLLVersion;

		
			if (bRegistered)
			{
				t_Registered.Visibility = Visibility.Visible;
				t_Email.Content = Email;
				t_CDKey.Content = CDKey;
			}
			else
				t_NotRegistered.Visibility = Visibility.Visible;
		}


	}
}
