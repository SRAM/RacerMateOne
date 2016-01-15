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
    /// Interaction logic for Window_Welcome.xaml
    /// </summary>
    public partial class Window_Welcome : Window
    {
        public Window_Welcome()
        {
            InitializeComponent();
        }
		private bool bDone = false;
        private string pvUserChoice = "HomeUse"; //default is to quit app
        public string UserChoice { get { return pvUserChoice; } }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

		private void FormFadeIn_Completed(object sender, EventArgs e)
		{

		}
		private void FormFadeOut_Completed(object sender, EventArgs e)
		{
			this.Close();
		}

        private void HomeUse_Click(object sender, RoutedEventArgs e)
        {
			if (bDone)
				return;
            pvUserChoice = "HomeUse";
			bDone = true;
			FormFadeOut.Begin();
		}

        private void CommercialUse_Click(object sender, RoutedEventArgs e)
        {
			if (bDone)
				return;
            pvUserChoice = "CommercialUse";
			bDone = true;
			FormFadeOut.Begin();
		}
    }
}
