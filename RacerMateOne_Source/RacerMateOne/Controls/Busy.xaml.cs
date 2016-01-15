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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for Busy.xaml
	/// </summary>
	public partial class Busy : UserControl
	{
		//static RepeatBehavior r_one = new RepeatBehavior(1);
		public static DependencyProperty ActiveProperty = DependencyProperty.Register("Active", typeof(bool), typeof(Busy),
			new FrameworkPropertyMetadata(false,new PropertyChangedCallback(OnActiveChanged)));
		public bool Active
		{
			get { return (bool)this.GetValue(ActiveProperty); }
			set { 
				this.SetValue(ActiveProperty, value); 
			}
		}

		private static void OnActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Busy)d).SetAnim();
		}
		void SetAnim()
		{
			if (Active == true)
			{
				a1.Begin(this, true);
				a2.Begin(this, true);
				a3.Begin(this, true);
				a4.Begin(this, true);
				a5.Begin(this, true);
				a6.Begin(this, true);
				a7.Begin(this, true);
				a8.Begin(this, true);
			}
			else
			{
				a1.Stop(this);
				a2.Stop(this);
				a3.Stop(this);
				a4.Stop(this);
				a5.Stop(this);
				a6.Stop(this);
				a7.Stop(this);
				a8.Stop(this);
			}
		}

		public Busy()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			SetAnim();
		}
	}
}
