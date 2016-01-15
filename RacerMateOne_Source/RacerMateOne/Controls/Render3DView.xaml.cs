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


namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for Render3DView.xaml
	/// </summary>
	public partial class Render3DView : BaseUnit
	{
		//=================================================================
		public Render3DView()
		{
			InitializeComponent();
		}
		//Point relativePoint = myVisual.TransformToAncestor(rootVisual)
		//					  .Transform(new Point(0, 0));

		Render3D m_Render3D;
		protected override void BaseUnit_Loaded(object sender, RoutedEventArgs e)
		{
			base.BaseUnit_Loaded(sender, e);
			// Move up the tree until we find the parent control..
			m_Render3D = Render3D.FindAncestor(this, typeof(Render3D)) as Render3D;
			if (m_Render3D == null)
				throw new Exception("Must parent to a Render3D Control");
			if (Render3D.IsInDesignMode)
			{
				wrk.Source = new BitmapImage(new Uri("pack://application:,,,/RacerMateOne;component/Resources/Render3DView_Placeholder.jpg"));
				wrk.Visibility = Visibility.Visible;
				return;
			}
			m_Render3D.Add(this);
		}

		protected override void BaseUnit_Unloaded(object sender, RoutedEventArgs e)
		{
			base.BaseUnit_Unloaded(sender, e);
			if (m_Render3D != null)
				m_Render3D.Remove(this);
		}

		private void UserControl_LayoutUpdated(object sender, EventArgs e)
		{
			if (m_Render3D != null)
				m_Render3D.Changed(this);
		}

		private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (m_Render3D != null)
				m_Render3D.Changed(this);
		}


		protected override void OnUnitChanged()
		{
			if (m_Render3D != null)
				m_Render3D.ChangeUnit(this);
		}

		Brush m_FixBackground = null;
		public Brush FixBackground
		{
			get { return m_FixBackground; }
			set
			{
				if (m_FixBackground != value)
				{
					m_FixBackground = value;
					if (m_Render3D != null)
						m_Render3D.Changed(this);
				}
			}
		}

	}
}
