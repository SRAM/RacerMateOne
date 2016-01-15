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
using System.Runtime.InteropServices;
using WPFMediaKit.DirectShow.Controls;
using WPFMediaKit.DirectShow.MediaPlayers;
using DirectShowLib;


namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for RCV.xaml
	/// </summary>
	public partial class RCVPreview : UserControl
	{
		protected MediaUriElement m_VControl;
		//==================================
		Course m_Course;
		public Course Course 
		{
			get { return m_Course; }
			set 
			{
				Course v = value == null || value.AVIFilename == null ? null:value;
				if (m_Course == v)
					return;
				VControl.Stop();
				VControl.Visibility = v == null ? Visibility.Hidden:Visibility.Visible;
				m_Course = v;
				if (m_Course != null)
				{
					m_Location = new Course.Location(m_Course, m_Course.StartAt );
					VControl.Source = new Uri(v.AVIFilename);
					VControl.Play();
				}
			}
		}
		//==================================
		public bool FirstUnit = true;
		//==================================
		public RCVPreview()
		{
			InitializeComponent();
			if (AppWin.IsInDesignMode)
			{
				Image img = new Image();
				img.Source = new BitmapImage(new Uri("pack://application:,,,/RacerMateOne;component/Resources/RCV.jpg"));
				img.Stretch = Stretch.UniformToFill;
				img.HorizontalAlignment = HorizontalAlignment.Stretch;
				img.VerticalAlignment = VerticalAlignment.Stretch;
				img.StretchDirection = StretchDirection.Both;
				Content = img;
				return;
			}
			//m_VControl = new MediaUriElement();
			//VControl.Source = new Uri(@"file:///C:/Real Course Video/Kona/Kona.avi");
			VControl.VideoRenderer = VideoRendererType.VideoMixingRenderer9;
			VControl.AllowDrop = true;
			VControl.PreferedPositionFormat = MediaPositionFormat.Frame;  
			VControl.OverridesDefaultStyle = true;
			VControl.LoadedBehavior = WPFMediaKit.DirectShow.MediaPlayers.MediaState.Manual;
		}

		void BaseUnit_Loaded(object sender, RoutedEventArgs e)
		{
			if (AppWin.IsInDesignMode)
				return;

		}
		void BaseUnit_Unloaded(object sender, RoutedEventArgs e)
		{
			if (AppWin.IsInDesignMode)
				return;
			
		}


		long m_TargetFrame;
		Course.Location m_Location;
		public double Location
		{
			get { return m_Location.X; }
			set
			{
				if (m_Course == null || m_Location.X == value)
					return;

				//0.033366599999999998
				double spf = VControl.AverageTimePerFrame;
				if (spf == 0.0)
					spf = 0.033366599999999998;

				m_Location.X = value;
				Course.GPSSegment seg = m_Location.Segment as Course.GPSSegment;
				Course.GPSSegment nseg = seg.Node.Next == null ? seg:seg.Node.Next.Value as Course.GPSSegment;
				double t = (double)(nseg.GPSData.seconds - seg.GPSData.seconds) * m_Location.SegmentX + seg.GPSData.seconds;
				if (t < 0.0)
					t = 0.0;
				m_TargetFrame = (long)(.5 + t / spf);
				AdjustRate(0);
			}
		}
		//====================================================================================
		const double minrate = 0.0000000000000001;
		const double maxrate = 12.0;
		void AdjustRate(double splittime)
		{
			VControl.MediaPosition = m_TargetFrame;
		}
	}
}
