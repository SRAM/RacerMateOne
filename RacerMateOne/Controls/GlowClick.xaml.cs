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

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for GlowClick.xaml
	/// </summary>
	public partial class GlowClick : BaseClick
	{
		public GlowClick()
		{
			InitializeComponent();
			if (AppWin.IsInDesignMode)
				PartnerOverlay.Opacity = 0.5;
		}
	}
}
