﻿using System;
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
	/// Interaction logic for Window_ImportCSV.xaml
	/// </summary>
	public partial class Window_ImportCSV : Window
	{
		public Window_ImportCSV()
		{
			InitializeComponent();
		}

		private bool bDone = false;
		private string pvUserChoice = "Yes";
		public string UserChoice { get { return pvUserChoice; } }


		private void FormFadeIn_Completed(object sender, EventArgs e)
		{

		}
		private void FormFadeOut_Completed(object sender, EventArgs e)
		{
			this.Close();
		}

		private void Yes_Click(object sender, RoutedEventArgs e)
		{
			if (bDone)
				return;
			bDone = true;
			FormFadeOut.Begin();
			pvUserChoice = "Yes";
		}

		private void No_Click(object sender, RoutedEventArgs e)
		{
			if (bDone)
				return;
			bDone = true;
			FormFadeOut.Begin();
			pvUserChoice = "No";
		}
	}
}