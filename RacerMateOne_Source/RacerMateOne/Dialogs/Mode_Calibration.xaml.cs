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

namespace RacerMateOne
{
	/// <summary>
	/// Interaction logic for Mode_Calibration.xaml
	/// </summary>
	public partial class Mode_Calibration : Window
	{
		bool bDone = false;
		public Mode_Calibration()
		{
			InitializeComponent();
		}
		private void FormFadeIn_Completed(object sender, EventArgs e)
		{

		}
		private void FormFadeOut_Completed(object sender, EventArgs e)
		{
			this.Close();
		}

		private void Done_Click(object sender, RoutedEventArgs e)
		{
			if (bDone)
				return;
			bDone = true;
			FormFadeOut.Begin();
			
		}
	}
}
