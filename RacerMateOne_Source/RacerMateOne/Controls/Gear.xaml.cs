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
	/// Interaction logic for Gear.xaml
	/// </summary>
	public partial class Gear : UserControl
	{
		public static double GearDepth = 0.26;
		private double m_TopRadians = 1.05 * (Math.PI / 180.0);
		//=====================================================================================================
		public static DependencyProperty TeethProperty = DependencyProperty.Register("Teeth", typeof(int), typeof(Gear),
				new FrameworkPropertyMetadata(30,new PropertyChangedCallback(OnTeethChanged)));
		public int Teeth
		{
			get { return (int)this.GetValue(TeethProperty); }
			set { this.SetValue(TeethProperty, value); }
		}
		//=====================================================================================================
		public static DependencyProperty MaxTeethProperty = DependencyProperty.Register("MaxTeeth", typeof(int), typeof(Gear),
				new FrameworkPropertyMetadata(30, new PropertyChangedCallback(OnTeethChanged)));
		public int MaxTeeth
		{
			get { return (int)this.GetValue(MaxTeethProperty); }
			set { this.SetValue(MaxTeethProperty, value); }
		}
		//=====================================================================================================
		public static DependencyProperty RotationProperty = DependencyProperty.Register("Rotation", typeof(double), typeof(Gear),
				new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnTeethChanged)));
		public double Rotation
		{
			get { return (double)this.GetValue(RotationProperty); }
			set { this.SetValue(RotationProperty, value); }
		}
		//=====================================================================================================
		public static DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(Gear),
				new FrameworkPropertyMetadata(Brushes.White,new PropertyChangedCallback(OnTeethChanged)));
		public Brush Fill
		{
			get { return (Brush)this.GetValue(FillProperty); }
			set { this.SetValue(FillProperty, value); }
		}
		//=====================================================================================================
		public static DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(Gear),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnStrokeDashArrayChanged)));
		public DoubleCollection StrokeDashArray
		{
			get { return (DoubleCollection)this.GetValue(StrokeDashArrayProperty); }
			set { this.SetValue(StrokeDashArrayProperty, value); }
		}
		public static void OnStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Gear)d).GearPath.StrokeDashArray = ((Gear)d).StrokeDashArray;
		}

		//=====================================================================================================

		public static bool NoUpdate = false;

		public int Number;

		//private double m_DipSize = 0.1;

		private int m_Teeth;
		private int m_MaxTeeth;
		private double m_Rotation;


		private bool m_bLoaded = false;

		public Gear()
		{
			InitializeComponent();
		}

		private void Gear_Loaded(object sender, RoutedEventArgs e)
		{
			m_bLoaded = true;
			RedoGear(true);
		}

		private void Gear_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			RedoGear(true);
		}

		private static void OnTeethChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Gear)d).RedoGear();
		}



		private Point m_Center = new Point();

		private Point _temp = new Point();
		private Point _temp1 = new Point();
		private Point _temp2 = new Point();

		Point atR(double radians, double radius)
		{
			_temp.X = Math.Cos(radians) * radius + m_Center.X;
			_temp.Y = Math.Sin(radians) * radius + m_Center.Y;
			return _temp;
		}
		Point atR1(double radians, double radius)
		{
			_temp1.X = Math.Cos(radians) * radius + m_Center.X;
			_temp1.Y = Math.Sin(radians) * radius + m_Center.Y;
			return _temp;
		}
		Point atR2(double radians, double radius)
		{
			_temp2.X = Math.Cos(radians) * radius + m_Center.X;
			_temp2.Y = Math.Sin(radians) * radius + m_Center.Y;
			return _temp2;
		}


		public void RedoGear() { RedoGear(false); }
		public void RedoGear(bool force)
		{
			if (!m_bLoaded || NoUpdate)
				return;

			if (!force && Teeth == m_Teeth && MaxTeeth == m_MaxTeeth && Rotation == m_Rotation)
				return;

			m_Teeth = Teeth;
			m_MaxTeeth = MaxTeeth;
			m_Rotation = Rotation;

			if (m_Teeth < 3 || m_MaxTeeth < 3)
			{
				GearPath.Visibility = Visibility.Hidden;
				return;
			}
			GearPath.Visibility = Visibility.Visible;

			m_Center.X = ActualWidth * 0.5;
			m_Center.Y = ActualHeight * 0.5;
			double maxradius = Math.Min(m_Center.X,m_Center.Y);
			double maxstep = Math.PI * 2 / m_MaxTeeth;
			double rstep = Math.PI * 2 / m_Teeth;
			double radius = maxradius * maxstep / rstep;

			double subradius = radius - maxradius * m_TopRadians;

			double topr = m_TopRadians * maxradius / radius;

			double rr = m_Rotation * Math.PI / 180.0;



			StreamGeometry geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;

			Size topsize = new Size(m_TopRadians * maxradius, m_TopRadians * maxradius);
			double bottomradius = subradius - maxstep * GearDepth * maxradius;

			using (StreamGeometryContext ctx = geometry.Open())
			{
				ctx.BeginFigure(atR(rr,radius), true, true);
				for(int i = 0;i < m_Teeth;i++)
				{
					double r = rstep * i + rr;
					double er = rstep * (i + 1) + rr;

					ctx.ArcTo(atR(r + m_TopRadians, subradius), topsize, 90, false, SweepDirection.Clockwise, true, true);

					ctx.BezierTo(atR1(r + topr, bottomradius), atR2(r + topr, bottomradius), atR(r + rstep * 0.5, bottomradius), true, true);

					ctx.BezierTo(atR1(er - topr, bottomradius), atR2(er - topr, bottomradius), atR(er - topr, subradius), true, true);
					
					ctx.ArcTo(atR(er, radius), topsize, 90, false, SweepDirection.Clockwise, true, true);
				}
			}
			geometry.Freeze();
			GearPath.Data = geometry;
		}


	}
}
