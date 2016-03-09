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
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace RacerMateOne.Controls
{
	public partial class BaseUnit : UserControl, INotifyPropertyChanged
	{
		//==============================================================
		public static DependencyProperty UnitNumberProperty = DependencyProperty.Register("UnitNumber", typeof(int), typeof(BaseUnit),
			new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(_UnitNumberChanged)));
		public int UnitNumber
		{
			get { return (int)this.GetValue(UnitNumberProperty); }
			set { this.SetValue(UnitNumberProperty, value); }
		}
		private static void _UnitNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			int num = ((BaseUnit)d).UnitNumber;
			((BaseUnit)d).Unit = num < 0 || num >= Unit.MaxUnits ? null:Unit.Units[num];
		}
		//==============================================================
		public static DependencyProperty StatFlagsProperty = DependencyProperty.Register("StatFlags", typeof(StatFlags), typeof(BaseUnit),
			new FrameworkPropertyMetadata(StatFlags.Zero, new PropertyChangedCallback(_StatFlagsChanged)));
		public StatFlags StatFlags
		{
			get { return (StatFlags)this.GetValue(StatFlagsProperty); }
			set { this.SetValue(StatFlagsProperty, value); }
		}
		private static void _StatFlagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((BaseUnit)d).OnStatFlagsChanged();
		}
		
		public enum SubType
		{
			Unit,
			Statistics,
			Rider
		};

		protected struct BindData
		{
			public FrameworkElement		element;
			public DependencyProperty	prop;
			public Binding				binding;
			public SubType				subtype;
		}
		protected List<BindData> m_BindList = new List<BindData>();

		protected bool m_bInit;

		/// <summary>
		/// 
		/// </summary>
		public BaseUnit()
		{
			Loaded += new RoutedEventHandler(BaseUnit_Loaded);
			Unloaded += new RoutedEventHandler(BaseUnit_Unloaded);
			IsVisibleChanged += new DependencyPropertyChangedEventHandler(e_IsVisibleChanged);
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void InitBindList()
		{

		}

		protected object SourceType(SubType type)
		{
			switch (type)
			{
				case SubType.Statistics: return m_Unit != null ? m_Unit.Statistics : null;
				case SubType.Rider: return m_Unit != null ? m_Unit.Rider : null;
			}
			return m_Unit;
		}
		public void AddBinding(SubType type, String name, FrameworkElement element, DependencyProperty prop, IValueConverter conv)
		{
			BindData b;
			b.subtype = type;
			b.binding = new Binding(name);
			b.element = element;
			if (conv != null)
				b.binding.Converter = conv;
			b.prop = prop;
			m_BindList.Add(b);
			if (m_bindon)
			{
				try 
				{
					b.binding.Source = SourceType(b.subtype);
					b.element.SetBinding(b.prop,  b.binding); 
				}
				catch { ms_err++; }
			}
		}
			
		#region Events
		protected virtual void BaseUnit_Loaded(object sender, RoutedEventArgs e)
		{
			m_bInit = true;
			InitBindList();
			RedoStatFlags();
			OnUnitChanged();
			BindOn();
		}
		protected virtual void BaseUnit_Unloaded(object sender, RoutedEventArgs e)
		{
			BindOff();
		}
		private void e_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsVisible)
				BindOn();
			else
				BindOff();
		}
		#endregion Events

		// INotifyPropertyChanged
		// =============================================================
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}


		protected Unit m_Unit;
		public Unit Unit
		{
			get { return m_Unit; }
			set
			{
				if (m_Unit != value)
					BindOff();
				m_Unit = value;
				if (m_bInit)
					OnUnitChanged();
				BindOn();
			}
		}

		protected bool m_bindon;
		protected virtual void BindOn()
		{
			if (!IsVisible || !m_bInit || m_bindon || m_Unit == null)
				return;
			foreach(BindData b in m_BindList)
			{
				try 
				{
					b.binding.Source = SourceType(b.subtype);
					b.element.SetBinding(b.prop, b.binding); 
				}
				catch { ms_err++; }
			}
			m_bindon = true;
			if (m_Unit != null && m_StatFlags != StatFlags.Zero)
			{
				m_NotifyUnit = m_Unit;
				m_NotifyFlags = m_StatFlags;
				Unit.AddNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
				OnUnitFlagsChanged(m_Unit, m_StatFlags);
			}
		}
		Unit m_NotifyUnit = null;
		StatFlags m_NotifyFlags;

		public static int ms_err;
		protected virtual void BindOff()
		{
			if (!m_bindon)
				return;
			foreach (BindData b in m_BindList)
			{
				try 
				{
					b.binding.Source = null;
					b.element.SetBinding(b.prop, (BindingBase)null); 
				}
				catch { ms_err++; }
			}
			m_bindon = false;

			if (m_NotifyUnit != null)
			{
				Unit.RemoveNotify(m_NotifyUnit, m_NotifyFlags,new Unit.NotifyEvent(OnUnitFlagsChanged));
				m_NotifyUnit = null;
			}
		}

		protected virtual void OnUnitChanged()
		{
		}
		protected virtual void OnUnitFlagsChanged(Unit unit, StatFlags changed)
		{
		}
		protected virtual void RedoStatFlags()
		{
		}


		protected StatFlags m_StatFlags = StatFlags.Zero;
		protected virtual void OnStatFlagsChanged()
		{
			StatFlags s = StatFlags;
			if (s != m_StatFlags)
			{
				BindOff();
				RedoStatFlags();
				m_StatFlags = s;
				BindOn();
			}
		}

	}
}
