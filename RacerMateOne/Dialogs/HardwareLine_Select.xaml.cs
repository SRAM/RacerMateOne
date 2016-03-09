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
	/// Interaction logic for HardwareLine_Select.xaml
	/// </summary>
	public partial class HardwareLine_Select : Window
	{
		public RM1.Trainer Trainer;
		public HardwareLine_Select()
		{
			InitializeComponent();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		public void Window_Loaded(object sender, RoutedEventArgs e)
		{
			RM1.OnPadKey += new RM1.TrainerPadKey(RM1_OnPadKey);
		}

		void RM1_OnPadKey(RM1.Trainer trainer, RM1.PadKeys key, double pressed)
		{
			if (pressed == 0)
			{
				Trainer = trainer;
				Close();
			}
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			RM1.OnPadKey -= new RM1.TrainerPadKey(RM1_OnPadKey);
		}
	}
}
