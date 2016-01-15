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

using System.Windows.Interop;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ComponentModel;
using System.Windows.Media.Effects;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;

namespace RacerMateOne.Pages
{
	/// <summary>
	/// Interaction logic for StyleSheet.xaml
	/// </summary>
	public partial class StyleSheet : Page
	{
		public StyleSheet()
		{
			InitializeComponent();
		}

		void Page_Loaded(object sender, RoutedEventArgs e)
		{
            //d_CoursePicker.Filter = CourseFilter.F_PerformanceWatts;
            //Courses.LoadPerformances();
		}

		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

	}
}
