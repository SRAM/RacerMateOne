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


namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for RM1TitleTitle.xaml
	/// </summary>
	public partial class RM1PageTitle : UserControl
	{

		private static bool? _isInDesignMode;
		public static bool IsInDesignMode
		{
			get
			{
				if (!_isInDesignMode.HasValue)
				{
#if SILVERLIGHT
					_isInDesignMode = DesignerProperties.IsInDesignTool;
#else
					var prop = DesignerProperties.IsInDesignModeProperty;
					_isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
#endif
				}
				return _isInDesignMode.Value;
			}
		}

		public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(String), typeof(RM1PageTitle),
			new FrameworkPropertyMetadata("",new PropertyChangedCallback(_TitleChanged)));
		public String Title
		{
			get { return (String)this.GetValue(TitleProperty); }
			set { this.SetValue(TitleProperty, value); }
		}
		private static void _TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UpdateTitle();
		}

		private static LinkedList<RM1PageTitle> ms_List = new LinkedList<RM1PageTitle>();

		private LinkedListNode<RM1PageTitle> m_Node;
		public RM1PageTitle()
		{
			InitializeComponent();
		}
		public static void UpdateTitle()
		{
			if (!IsInDesignMode)
				AppWin.Instance.SetPageTitle(ms_List.Last == null ? "" : ms_List.Last.Value.Title);
		}

		private void rm1TitleBar_Loaded(object sender, RoutedEventArgs e)
		{
			if (!IsInDesignMode)
			{
				if (m_Node == null)
					m_Node = ms_List.AddLast(this);
				UpdateTitle();
			}
		}

		private void rm1TitleBar_Unloaded(object sender, RoutedEventArgs e)
		{
			if (m_Node != null)
			{
				ms_List.Remove(m_Node);
				m_Node = null;
			}
			UpdateTitle();
		}
	}
}
