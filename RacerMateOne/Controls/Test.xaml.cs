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

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for Test.xaml
	/// </summary>
	public partial class Test : BaseUnit
	{

		protected override void InitBindList()
		{
			AddBinding(SubType.Statistics,"TimerString", Timer, Label.ContentProperty, null);
		}

		public Test()
		{
			InitializeComponent();
		}

	}
}
