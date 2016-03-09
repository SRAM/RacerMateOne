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
	/// Interaction logic for ReverseBorder.xaml
	/// </summary>
	public partial class ReverseBorder : UserControl
	{
		//=====================================================================================================
		public static DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ReverseBorder),
				new FrameworkPropertyMetadata(new CornerRadius(15), new PropertyChangedCallback(OnCornerRadiusChanged)));
		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)this.GetValue(CornerRadiusProperty); }
			set { this.SetValue(CornerRadiusProperty, value); }
		}
		private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ReverseBorder rd = (ReverseBorder)d;
			rd.Border.CornerRadius = rd.CornerRadius;
			rd.Redo();
		}
		//======================================================================================================
		public static new DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(ReverseBorder),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBorderThicknessChanged)));
		public new Thickness BorderThickness 
		{
			get { return (Thickness)this.GetValue(BorderThicknessProperty); }
			set { this.SetValue(BorderThicknessProperty, value); }
		}
		private static void OnBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ReverseBorder rd = (ReverseBorder)d;
			rd.Border.BorderThickness = rd.BorderThickness;
			rd.Redo();
		}
		//======================================================================================================
		public static new DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ReverseBorder),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBorderBrushChanged)));
		public new Brush BorderBrush 
		{
			get { return (Brush)this.GetValue(BorderBrushProperty); }
			set { this.SetValue(BorderBrushProperty, value); }
		}
		private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ReverseBorder rd = (ReverseBorder)d;
			rd.Border.BorderBrush = rd.BorderBrush;
		}
		//======================================================================================================
		public static new DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(ReverseBorder),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBackgroundChanged)));
		public new Brush Background
		{
			get { return (Brush)this.GetValue(BackgroundProperty); }
			set { this.SetValue(BackgroundProperty, value); }
		}
		private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ReverseBorder rd = (ReverseBorder)d;
			rd.UpperLeft.Fill = rd.UpperRight.Fill = rd.LowerLeft.Fill = rd.LowerRight.Fill = rd.Background;
		}
		//======================================================================================================
		void Redo()
		{
			double w = ActualWidth;
			double h = ActualHeight;
			if (w == 0 || h == 0)
				return;
			CornerRadius c = CornerRadius;
			StreamGeometry geometry;
			double x, y;

			x = y = c.TopLeft;// Data="M 0,50 A 50,50 90 0 1 50,0 L 0,0"
			geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			using (StreamGeometryContext ctx = geometry.Open())
			{
				ctx.BeginFigure(new Point(0, y), true /* is filled */, true /* is closed */);
				ctx.ArcTo(new Point(x, 0), new Size(x, y), 90, false, SweepDirection.Clockwise, true, false);
				ctx.LineTo(new Point(0, 0), true /* is stroked */, false /* is smooth join */);
			}
			geometry.Freeze();
			UpperLeft.Data = geometry;

			x = y = c.TopRight; //            Data="M 200,50 A 50,50 90 0 0 150,0 L 200,0"
			geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			using (StreamGeometryContext ctx = geometry.Open())
			{
				ctx.BeginFigure(new Point(w, y), true /* is filled */, true /* is closed */);
				ctx.ArcTo(new Point(w-x, 0), new Size(x, y), 90, false, SweepDirection.Counterclockwise, true, false);
				ctx.LineTo(new Point(w, 0), true /* is stroked */, false /* is smooth join */);
			}
			geometry.Freeze();
			UpperRight.Data = geometry;

			x = y = c.BottomLeft; // Data="M 0,150 A 50,50 90 0 0 50,200 L 0,200"
			geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			using (StreamGeometryContext ctx = geometry.Open())
			{
				ctx.BeginFigure(new Point(0, h - y), true /* is filled */, true /* is closed */);
				ctx.ArcTo(new Point(x, h ), new Size(x, y), 90, false, SweepDirection.Counterclockwise, true, false);
				ctx.LineTo(new Point(0, h), true /* is stroked */, false /* is smooth join */);
			}
			geometry.Freeze();
			LowerLeft.Data = geometry;


			x = y = c.BottomRight;//             Data="M 200,150 A 50,50 90 0 1 150,200 L 200,200"
			geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			using (StreamGeometryContext ctx = geometry.Open())
			{
				ctx.BeginFigure(new Point(w, h - y ), true /* is filled */, true /* is closed */);
				ctx.ArcTo(new Point(w - x, h ), new Size(x, y), 90, false, SweepDirection.Clockwise, true, false);
				ctx.LineTo(new Point(w, h), true /* is stroked */, false /* is smooth join */);
			}
			geometry.Freeze();
			LowerRight.Data = geometry;

		}




		public ReverseBorder()
		{
			InitializeComponent();
		}

		private void bdr_Loaded(object sender, RoutedEventArgs e)
		{
			Redo();
		}

		private void bdr_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Redo();
		}
	}
}
