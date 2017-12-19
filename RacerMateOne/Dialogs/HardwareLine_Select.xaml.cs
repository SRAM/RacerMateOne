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

namespace RacerMateOne.Dialogs {

//--------------------------------------------------------------------------
// Interaction logic for HardwareLine_Select.xaml
//--------------------------------------------------------------------------

public partial class HardwareLine_Select : Window  {

	public RM1.Trainer Trainer;

	//--------------------------------------------------------------------------
	//
	//--------------------------------------------------------------------------

	public HardwareLine_Select() {
		InitializeComponent();
	}

	//--------------------------------------------------------------------------
	//
	//--------------------------------------------------------------------------

	private void Cancel_Click(object sender, RoutedEventArgs e) {
		Close();
	}

	//--------------------------------------------------------------------------
	//
	//--------------------------------------------------------------------------

	public void Window_Loaded(object sender, RoutedEventArgs e) {
		RM1.OnPadKey += new RM1.TrainerPadKey(RM1_OnPadKey);
	}










		//--------------------------------------------------------------------------
		//
		//--------------------------------------------------------------------------

	/*
	109 11/7/2017 8:18:30 AM,MainThread: HardwareLine::Identify_click()
	110 11/7/2017 8:18:44 AM,MainThread: F2 Down
	111 11/7/2017 8:18:44 AM,MainThread: HardwareLine_Select::RM1_OnPadKey(), pressed = 0, trainer is COM4: / v45.43 / CompuTrainer / RRC = 2.00
	112 11/7/2017 8:18:44 AM,MainThread: HardwareLine::OnPadChanged(), RawButtons = 4,  COM4: / v45.43 / CompuTrainer / RRC = 2.00
	113 11/7/2017 8:18:45 AM,MainThread: F2_Long Down
	114 11/7/2017 8:18:45 AM,MainThread: F2 Up 1.07 seconds
	115 11/7/2017 8:18:45 AM,MainThread: HardwareLine::OnPadChanged(), RawButtons = 0,  COM4: / v45.43 / CompuTrainer / RRC = 2.00
	116 11/7/2017 8:20:42 AM,MainThread:       Riders::CreateXDocFromRiders()
	*/

	void RM1_OnPadKey(RM1.Trainer trainer, RM1.PadKeys key, double pressed) {
		if (pressed == 0) {
#if DEBUG
			Log.WriteLine("HardwareLine_Select::RM1_OnPadKey(), pressed = 0, trainer is " + trainer.CBLine);
#endif
			Trainer = trainer;
			Close();
		}
		else {
#if DEBUG
			Log.WriteLine("HardwareLine_Select::RM1_OnPadKey(), pressed = " + pressed.ToString());
#endif
		}
			return;
	}

	//--------------------------------------------------------------------------
	//
	//--------------------------------------------------------------------------

	private void Window_Unloaded(object sender, RoutedEventArgs e) {
		RM1.OnPadKey -= new RM1.TrainerPadKey(RM1_OnPadKey);
	}
}

}

