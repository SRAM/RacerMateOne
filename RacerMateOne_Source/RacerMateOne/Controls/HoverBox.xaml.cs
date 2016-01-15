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
	/// Interaction logic for HoverBox.xaml
	/// </summary>
	public partial class HoverBox : UserControl
	{
		public enum Side
		{
			Left, Right, Top, Bottom
		};
		//=====================================================================================================
		public static DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(HoverBox),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCornerRadiusChanged)));
		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)this.GetValue(CornerRadiusProperty); }
			set { this.SetValue(CornerRadiusProperty, value); }
		}
		private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			HoverBox h = (HoverBox)d;
			h.BuildArrow();
		}
		//======================================================================================================
		public static new DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(HoverBox),
				new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
					new PropertyChangedCallback(OnBackgroundChanged)));
		public new Brush Background
		{
			get { return (Brush)this.GetValue(BackgroundProperty); }
			set { this.SetValue(BackgroundProperty, value); }
		}
		private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			HoverBox h = (HoverBox)d;
			h.BuildArrow();
		}
		//======================================================================================================
		public static new DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(HoverBox),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBorderThicknessChanged)));
		public new Thickness BorderThickness
		{
			get { return (Thickness)this.GetValue(BorderThicknessProperty); }
			set { this.SetValue(BorderThicknessProperty, value); }
		}
		private static void OnBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			HoverBox h = (HoverBox)d;
			h.BuildArrow();
		}
		//======================================================================================================
		public static new DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(HoverBox),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBorderBrushChanged)));
		public new Brush BorderBrush
		{
			get { return (Brush)this.GetValue(BorderBrushProperty); }
			set { this.SetValue(BorderBrushProperty, value); }
		}
		private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			HoverBox h = (HoverBox)d;
			h.BuildArrow();
		}
		//=====================================================================================================
		public static DependencyProperty ArrowWidthProperty = DependencyProperty.Register("ArrowWidth", typeof(double), typeof(HoverBox),
				new FrameworkPropertyMetadata(40.0, new PropertyChangedCallback(OnArrowWidthPropertyChanged)));
		public double ArrowWidth
		{
			get { return (double)this.GetValue(ArrowWidthProperty); }
			set { this.SetValue(ArrowWidthProperty, value); }
		}
		private static void OnArrowWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			HoverBox h = (HoverBox)d;
			h.BuildArrow();
		}
		//======================================================================================================
		public static DependencyProperty ArrowHeightProperty = DependencyProperty.Register("ArrowHeight", typeof(double), typeof(HoverBox),
				new FrameworkPropertyMetadata(50.0, new PropertyChangedCallback(OnArrowHeightPropertyChanged)));
		public double ArrowHeight
		{
			get { return (double)this.GetValue(ArrowHeightProperty); }
			set { this.SetValue(ArrowHeightProperty, value); }
		}
		private static void OnArrowHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			HoverBox h = (HoverBox)d;
			h.BuildArrow();
		}
		//======================================================================================================
		public static DependencyProperty ArrowSlantProperty = DependencyProperty.Register("ArrowSlant", typeof(double), typeof(HoverBox),
				new FrameworkPropertyMetadata(0.3, new PropertyChangedCallback(OnArrowSlantPropertyChanged)));
		public double ArrowSlant
		{
			get { return (double)this.GetValue(ArrowSlantProperty); }
			set { this.SetValue(ArrowSlantProperty, value); }
		}
		private static void OnArrowSlantPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			HoverBox h = (HoverBox)d;
			h.BuildArrow();
		}
		//======================================================================================================
		public static DependencyProperty ArrowSideProperty = DependencyProperty.Register("ArrowSide", typeof(Side), typeof(HoverBox),
				new FrameworkPropertyMetadata(Side.Bottom, new PropertyChangedCallback(OnArrowSidePropertyChanged)));
		public Side ArrowSide
		{
			get { return (Side)this.GetValue(ArrowSideProperty); }
			set { this.SetValue(ArrowSideProperty, value); }
		}
		private static void OnArrowSidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			HoverBox h = (HoverBox)d;
			h.BuildArrow();
		}
		//======================================================================================================
		public static DependencyProperty ArrowLocationProperty = DependencyProperty.Register("ArrowLocation", typeof(double), typeof(HoverBox),
				new FrameworkPropertyMetadata(0.5, new PropertyChangedCallback(OnArrowLocationPropertyChanged)));
		public double ArrowLocation
		{
			get { return (double)this.GetValue(ArrowLocationProperty); }
			set { this.SetValue(ArrowLocationProperty, value); }
		}
		private static void OnArrowLocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			HoverBox h = (HoverBox)d;
			h.BuildArrow();
		}
		//======================================================================================================
		private Path Arrow = null;
		private void Path_Loaded(object sender, RoutedEventArgs e)
		{
			Arrow = sender as Path;
			BuildArrow();
		}
		private Border OuterBorder = null;
		private void Border_Loaded(object sender, RoutedEventArgs e)
		{
			OuterBorder = sender as Border;
			BuildArrow();
		}
	
		public HoverBox()
		{
			InitializeComponent();
			VerticalAlignment = VerticalAlignment.Top;
			HorizontalAlignment = HorizontalAlignment.Left;

		}

		Point m_PointAdjust;

		private void hoverBox_Loaded(object sender, RoutedEventArgs e)
		{
			BuildArrow();
		}
		public void BuildArrow()
		{
			if (OuterBorder == null || Arrow == null || m_CustomArrow)
				return;
			double w = OuterBorder.ActualWidth;
			double h = OuterBorder.ActualHeight;
			if (w == 0 || h == 0)
				return;

			// Figure out where the arrow center is.
			Side side = ArrowSide;
			double cx, cy, px, py, x, y;

			StreamGeometry geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			using (StreamGeometryContext ctx = geometry.Open())
			{
				switch (side)
				{
					case Side.Bottom:
						cx = w * this.ArrowLocation;
						cy = h - 1;
						px = cx - ArrowHeight * ArrowSlant;
						py = h + ArrowHeight;
						m_PointAdjust = new Point(px, py);
						ctx.BeginFigure(m_PointAdjust, true, true);
						x = cx - ArrowWidth * 0.5;
						if (x < CornerRadius.BottomLeft)
							x = CornerRadius.BottomLeft;
						ctx.LineTo(new Point(x, cy), true, false);
						x += ArrowWidth;
						if (x >= w - CornerRadius.BottomRight)
							x = w - CornerRadius.BottomRight - 1;
						ctx.LineTo(new Point(x, cy), true, false);
						break;
					case Side.Top:
						cx = w * this.ArrowLocation;
						cy = 1;
						px = cx - ArrowHeight * ArrowSlant;
						py = -ArrowHeight;
						m_PointAdjust = new Point(px, py);
						ctx.BeginFigure(m_PointAdjust, true, true);
						x = cx - ArrowWidth * 0.5;
						if (x < CornerRadius.TopLeft)
							x = CornerRadius.TopLeft;
						ctx.LineTo(new Point(x, cy), true, false);
						x += ArrowWidth;
						if (x >= w - CornerRadius.TopRight)
							x = w - CornerRadius.TopRight - 1;
						ctx.LineTo(new Point(x, cy), true, false);
						break;
					case Side.Left:
						cy = h * this.ArrowLocation;
						cx = 1;
						py = cy + ArrowHeight * ArrowSlant;
						px = -ArrowHeight;
						m_PointAdjust = new Point(px, py);
						ctx.BeginFigure(m_PointAdjust, true, true);
						y = cy - ArrowWidth * 0.5;
						if (y < CornerRadius.TopLeft)
							y = CornerRadius.TopLeft;
						ctx.LineTo(new Point(cx, y), true, false);
						y += ArrowWidth;
						if (y >= h - CornerRadius.BottomLeft)
							y = h - CornerRadius.BottomLeft - 1;
						ctx.LineTo(new Point(cx, y), true, false);
						break;
					case Side.Right:
						cy = h * this.ArrowLocation;
						cx = w - 1;
						py = cy + ArrowHeight * ArrowSlant;
						px = w + ArrowHeight;
						m_PointAdjust = new Point(px, py);
						ctx.BeginFigure(m_PointAdjust, true, true);
						y = cy - ArrowWidth * 0.5;
						if (y < CornerRadius.TopRight)
							y = CornerRadius.TopRight;
						ctx.LineTo(new Point(cx, y), true, false);
						y += ArrowWidth;
						if (y >= h - CornerRadius.BottomRight)
							y = h - CornerRadius.BottomRight - 1;
						ctx.LineTo(new Point(cx, y), true, false);
						break;
				}
			}
			geometry.Freeze();
			Arrow.Data = geometry;
			m_CustomArrow = false;
		}

		bool m_CustomArrow = false;

		public void SetPoint(double x, double y)
		{
			if (m_PointAdjust == null)
				return;
			if (m_CustomArrow)
				BuildArrow();
			x -= m_PointAdjust.X;
			y -= m_PointAdjust.Y;

			VerticalAlignment = VerticalAlignment.Top;
			HorizontalAlignment = HorizontalAlignment.Left;

			Margin = new Thickness(x, y, 0, 0);
		}
		public void SetPoint(double x, double y, double maxx)
		{
			if (m_PointAdjust == null)
				return;
			double xx = x - m_PointAdjust.X;
			double yy = y - m_PointAdjust.Y;

			VerticalAlignment = VerticalAlignment.Top;
			HorizontalAlignment = HorizontalAlignment.Left;

			switch (ArrowSide)
			{
			case Side.Bottom:
				if (xx + ActualWidth > maxx && OuterBorder != null)
				{
					xx = maxx - ActualWidth;


					double w = OuterBorder.ActualWidth;
					double h = OuterBorder.ActualHeight;
					if (w == 0 || h == 0)
						return;

					// Figure out where the arrow center is.
					Side side = ArrowSide;
					double cx, px;
					double oldslant = ArrowSlant;
					px = x - xx;
					cx = w * this.ArrowLocation;
					ArrowSlant = (cx - px) / ArrowHeight;
				}
				break;
			}

			Margin = new Thickness(xx, yy, 0, 0);
		}


		private void hoverBox_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			BuildArrow();
		}
	}
}
