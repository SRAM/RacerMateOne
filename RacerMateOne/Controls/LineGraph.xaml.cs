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
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for LineGraph.xaml
	/// </summary>
	public partial class LineGraph : UserControl
	{
        // ECT - just test values to see if there more than one LineGraph running
        //static Random _r = new Random(); // **** test *****
        //public Int16 _testval = (Int16)_r.Next(1000);

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
		//===========================================================================
		public static DependencyProperty SecondSizeProperty = DependencyProperty.Register("SecondSize", typeof(double), typeof(LineGraph),
			new FrameworkPropertyMetadata(10.0,new PropertyChangedCallback(_SecondSizeChanged)));
		private double m_SecondSize = 10.0;
		public double SecondSize
		{
			get { return (double)this.GetValue(SecondSizeProperty); }
			set 
			{ 
				this.SetValue(SecondSizeProperty, value);
			}
		}
		private static void _SecondSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LineGraph)d).m_SecondSize = ((LineGraph)d).SecondSize;
		}
		//===========================================================================
		public static DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(String), typeof(LineGraph),
			new FrameworkPropertyMetadata(null,new PropertyChangedCallback(OnItemsChanged)));
		public String Items
		{
			get { return (String)this.GetValue(ItemsProperty); }
			set { this.SetValue(ItemsProperty, value); }
		}
		private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LineGraph)d).ItemsChanged();
		}

		//===========================================================================
		private Dictionary<String, BaseItem> m_Items = new Dictionary<string, BaseItem>();
		//===========================================================================
		private delegate void BaseEvent();
		private Mutex m_Mux = new Mutex();
		//===============================================
		public RM1.IStatsEx m_Statistics;
		public RM1.IStatsEx Statistics
		{
			get { return m_Statistics; }
			set
			{
				if (m_Statistics != null)
				{
					m_Statistics.OnUpdate -= new RM1.IStatsEvent(OnStatsUpdate);
				}

				m_Statistics = value;

                // ECT - added a method to disable update when page is unloaded
                // set d_Graph.Statistics to null when page is unloaded
                if (value != null)
                    m_Statistics.OnUpdate += new RM1.IStatsEvent(OnStatsUpdate);
                else
                    m_bLoaded = false;
			}
		}
		//===============================================
		private bool m_bLoaded;
		private double m_CurTime;

		public LineGraph()
		{
			InitializeComponent();
			m_UpdateEvent = new BaseEvent(Update);
		}
		private void graph_Loaded(object sender, RoutedEventArgs e)
		{
			m_bLoaded = true;
			ItemsChanged();
		}

		private String m_CurItems;
		private List<BaseItem> m_FieldItems = new List<BaseItem>();
		private void ItemsChanged()
		{

            //Debug.WriteLine("Items changed has been called");
            if (!m_bLoaded)
				return;
			String f = Items;
			if (m_CurItems == f)
				return;
			foreach(BaseItem b in m_FieldItems)
				RemoveItem( b );
			m_FieldItems.Clear();

			if (f != null)
			{
				String[] arr = f.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries);
				int i = 0;
				while(i<arr.Length)
				{
					String name = arr[i++].Trim();
					double bottom = Convert.ToDouble( arr[i++].Trim() );
					double top = Convert.ToDouble( arr[i++].Trim() );
					Brush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(arr[i++].Trim());
					BaseItem bitem = (BaseItem)AddItem( name, bottom, top, brush, 2, true );
					m_FieldItems.Add(bitem);
				}
			}
			m_CurItems = f;
			Update();
		}
		public void RemoveItem( Object obj )
		{
			BaseItem item = obj as BaseItem;
			if (item != null)
			{
				Area.Children.Remove(item.Path);
				m_Items.Remove(item.Name);
			}
		}

		public Object AddItem( String name, double bottom, double top, Brush brush, double thickness, bool show )
		{
		   BaseItem bitem = new BaseItem( name, bottom,top, brush );
			
            bitem.Path.StrokeThickness = thickness;
			if (m_Items.ContainsKey(name))
				RemoveItem( m_Items[name] );
			m_Items[name] = bitem;
			Area.Children.Add( bitem.Path );
			if (!show)
				bitem.Path.Visibility = Visibility.Collapsed;
			return bitem;
		}

		public void ShowItem(Object item, bool show)
		{
			BaseItem bitem = item as BaseItem;
			if (bitem == null)
				return;

			bitem.Path.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
		}



		//var p = GetType().GetField("person").GetValue(this);
		//p.GetType().GetProperty("Name").SetValue(p, "new name", null);


		//================================================
		private bool m_Updating;
		private BaseEvent m_UpdateEvent;

		private int m_Errors;
		private double m_TX;

		private void OnStatsUpdate(RM1.IStats istats, object arguments)
		{
			double time = m_Statistics.Time;
			if (time == m_CurTime)
				return;
			m_Mux.WaitOne();
			try
			{
				double adv = time - m_CurTime;

				m_CurTime = time;

                // ECT - This removes previous runs values 
                // Probably need a better checks than time == 0 to detect when to clear.
                // This works right now
                if (time == 0)
                {
                    foreach (KeyValuePair<String, BaseItem> pair in m_Items)
                    {
                        BaseItem bitem = pair.Value;
                        bitem.PointList.Clear();
                    }
                }

				Point p = new Point();

				// How much do we move the entire thing - left by?
				double w = ActualWidth;
				double basex = time * ActualWidth / m_SecondSize;
				double lasttx = m_TX;
				m_TX = w - basex;
				bool samex = Math.Round(lasttx) == Math.Round(m_TX);

				double h = ActualHeight;
				double v;

				// Update all the stats here.
				foreach (KeyValuePair<String, BaseItem> pair in m_Items)
				{
                     BaseItem bitem = pair.Value;

					v = Convert.ToDouble(bitem.PropertyInfo.GetValue((RM1.IStatsEx)m_Statistics, null));
                      p.Y = (1.0 - (v - bitem.Bottom) * bitem.ScaleToOne) * h;
                      if (pair.Key == "Speed")
                      {
                          Debug.WriteLine("updating " + pair.Key + " it is " + v + " and p.y = " + p.Y  + " and speed is " + m_Statistics.Speed );
                      }
                
                    if (bitem.PointList.Count == 0)
					{
						p.X = 0;
						bitem.PointList.AddLast(p);
                        // ECT - added a second point so it renders and clears previous runs
                        bitem.PointList.AddLast(p);
                    }
					else if (!samex)
					{
						//bitem.TX -= move;
						//p.X = w - bitem.TX;
						p.X = basex;
						if (bitem.PointList.Last.Value.Y == p.Y)
						{
							if (!bitem.Saved)
							{
								bitem.PointList.AddLast(p);
								bitem.Saved = true;
							}
							else
								bitem.PointList.Last.Value = p;
						}
						else
						{
							bitem.PointList.Last.Value = new Point(p.X, bitem.PointList.Last.Value.Y);
							bitem.PointList.AddLast(p);
							bitem.Saved = false;
						}
					}
					if (bitem.PointList.Count > 1)
					{
						LinkedListNode<Point> ck = bitem.PointList.First;
						LinkedListNode<Point> n = ck.Next;
						while (n != null && n.Value.X +m_TX < 0)
						{
							bitem.PointList.Remove(ck);
							ck = n;
							n = ck.Next;
						}
					}
				}
				// Call the event to update line... we only one one of these so check the variable... update line will NULL out the variable.
				if (!m_Updating)
				{
					m_Updating = true;
					Dispatcher.BeginInvoke(DispatcherPriority.Normal, m_UpdateEvent);
				}
			}
			catch 
			{
				m_Errors++;
			}
			m_Mux.ReleaseMutex();
		}


		private void Update()
		{
			if (!m_bLoaded)
				return;
			if (IsInDesignMode)
			{
				double per = 0.8 / m_Items.Count;
				double acc = 0.1;
				double w = ActualWidth;
				double h = ActualHeight;

				foreach (KeyValuePair<String, BaseItem> p in m_Items)
				{
					p.Value.RenderPreview(acc, w, h);
					acc += per;
				}
			}
			else
			{
				m_Mux.WaitOne();
				try
				{
					foreach (KeyValuePair<String, BaseItem> p in m_Items)
						p.Value.Render(m_TX);
				}
				catch 
				{
					m_Errors++;
				}
				m_Updating = false;
				m_Mux.ReleaseMutex();
			}
		}

		protected class BaseItem
		{
			public String Name;
			public double ScaleToOne;
			public double Bottom;
			public double Top;
			public PropertyInfo PropertyInfo;
			public Path Path = new Path();
			public bool Saved;
			
			public LinkedList<Point> PointList = new LinkedList<Point>();

			public BaseItem(String fieldname, double bottom, double top, Brush brush)
			{
				Name = fieldname;
				System.Type istats = typeof(RM1.IStatsEx);

				PropertyInfo = istats.GetProperty(fieldname);

				if (PropertyInfo == null)
				{
					if ((PropertyInfo = typeof(RM1.IStats).GetProperty(fieldname)) == null)
						throw new FieldAccessException();
				}
				Bottom = bottom;
				Top = top;
				Path.Stroke = brush;
				Path.ClipToBounds = true;
				Path.StrokeThickness = 1.0;
				ScaleToOne = 1.0 / (Top - Bottom);
			}
			public virtual void RenderPreview( double td, double w, double h )
			{
				StreamGeometry geometry = new StreamGeometry();
				using (StreamGeometryContext ctx = geometry.Open())
				{
					ctx.BeginFigure(new Point(0, h * td), false, false);
					ctx.LineTo(new Point(w, h * td), true, false);
				}
				geometry.Freeze();
				Path.Data = geometry;
			}

			public virtual void Render(double offset)
			{
				if (PointList.Count > 1)
				{
					StreamGeometry geometry = new StreamGeometry();
					using (StreamGeometryContext ctx = geometry.Open())
					{
						LinkedListNode<Point> pp = PointList.First;
						ctx.BeginFigure(new Point(pp.Value.X + offset,pp.Value.Y), false, false);
						for (; pp != null; pp = pp.Next)
							ctx.LineTo(new Point(pp.Value.X + offset,pp.Value.Y), true, false);
					}
					geometry.Freeze();
					Path.Data = geometry;
				}
			}
		}


	}
}