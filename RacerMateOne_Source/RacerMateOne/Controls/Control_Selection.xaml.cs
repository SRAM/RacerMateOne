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

namespace RacerMateOne
{
	/// <summary>
	/// Interaction logic for Control_Selection.xaml
	/// </summary>
	public partial class Control_Selection : UserControl
	{
		protected static SolidColorBrush BkBorder_Off = new SolidColorBrush(Color.FromArgb(0xFF, 0x51, 0x86, 0xAE));
		protected static SolidColorBrush BkBorder_On = new SolidColorBrush(Color.FromArgb(0xFF, 0xB3, 0xCD, 0xE1));

		public Control_Selection()
		{
			InitializeComponent();
		}
		public String Title
		{
			get { return cName.Text; }
			set { cName.Text = value; }
		}
		public String Image
		{
			get { return cThumbnail.Source.ToString(); }
			set 
			{
				BitmapImage img = new BitmapImage();
				img.BeginInit();
				img.UriSource = new Uri("/RacerMateOne;component/Resources/"+value, UriKind.Relative);
				img.EndInit();
				cThumbnail.Source = img;
			}
		}
		public void AddReg()
		{
			TextBlock t = new TextBlock(new Run("®"));
			t.FontSize = 8;
			InlineUIContainer uc = new InlineUIContainer(t);
			uc.BaselineAlignment = BaselineAlignment.Top;
			cName.Inlines.Add(uc);

		}
		public Brush GridBackground
		{
			get { return (Brush)GetValue(GridBackgroundProperty); }
			set { SetValue(GridBackgroundProperty, value); }
		}
		public static readonly DependencyProperty GridBackgroundProperty =
			DependencyProperty.Register("GridBackground", typeof(Brush), typeof(Control_Selection), new FrameworkPropertyMetadata(Brushes.Blue));


		private void UserControl_MouseEnter(object sender, MouseEventArgs e)
		{
			BkBorder.Background = BkBorder_On;
		}

		private void UserControl_MouseLeave(object sender, MouseEventArgs e)
		{
			BkBorder.Background = BkBorder_Off;
		}

	}

}
