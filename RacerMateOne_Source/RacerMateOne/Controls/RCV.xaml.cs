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
	public partial class RCV : BaseUnit
	{
		protected MediaUriElement m_VControl;
		//==================================
		Course m_Course;
		public Course Course 
		{
			get { return m_Course; }
			set 
			{
				Course v = value == null || value.VideoCourse == null ? null:value;
				if (m_Course == v)
					return;
				VControl.Stop();
				VControl.Visibility = v == null ? Visibility.Hidden:Visibility.Visible;
				m_Course = v;
				if (m_Course != null)
				{
					m_Location = new Course.Location(m_Course, 0.0f );
					VControl.Source = new Uri(v.VideoCourse.AVIFilename);
					m_ForceLoc = true;
					Location = 0;
					//VControl.SyncSource = m_RefClock;
					m_RefClock.PlayRate = 0.00000000000001;
					VControl.Play();
					m_ForceTo = m_TargetFrame + 1;	// Arbitrary number of frame forward.
					VControl.MediaPosition = m_TargetFrame;
					m_CatchupStopTime = DateTime.Now.Ticks + (Int64)(ConvertConst.SecondToHundredNanosecond * 10);
				}
			}
		}
		//==================================
		//===============================================================================


		//==================================
		public bool FirstUnit = true;
		//==================================
		ReferenceClock m_RefClock;
		public RCV()
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
			m_RefClock = new ReferenceClock();
			VControl.SyncSource = m_RefClock;
			m_RefClock.PlayRate = 0.000000000000001;
			//Content = m_VControl;
			/*
			 */
		}

		protected override void BaseUnit_Loaded(object sender, RoutedEventArgs e)
		{
			if (AppWin.IsInDesignMode)
				return;
			base.BaseUnit_Loaded(sender,e);

			BeginOnUpdate();
			//VControl.Play();
		}
		protected override void BaseUnit_Unloaded(object sender, RoutedEventArgs e)
		{
			if (AppWin.IsInDesignMode)
				return;
			
			base.BaseUnit_Loaded(sender, e);
			//VControl.Play();
		}


		long m_TargetFrame;
		double m_TargetRate;
		Course.Location m_Location;
		bool m_ForceLoc;

		long m_ForceTo; // When we need to prime the video - set this to a frame.  (Will get the rate going for a bit)

		public double Location
		{
			get { return m_Location.X; }
			set
			{
				if (m_Course == null || (m_Location.X == value && !m_ForceLoc))
					return;
				m_ForceLoc = false;

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
				m_TargetRate = RacerMateOne.Unit.State == Statistics.State.Running && m_Unit != null ?
					seg.GPSData.mps2 > 0 ? m_Unit.Statistics.Speed / seg.GPSData.mps2 : minrate : 0;

				if (m_Unit == null)
					AdjustRate(0.0);
				//AdjustRate();	// Adjust the rate when we adjust the frame.
			}
		}
		//====================================================================================
		const double minrate = 0.0000000000000001;
		const double maxrate = 12.0;
		public double Rate
		{
			get { return m_RefClock.PlayRate; }
			set 
			{
				double v = value < minrate ? minrate:value > maxrate ? maxrate:value;
				if (v == m_RefClock.PlayRate)
					return;
				m_RefClock.PlayRate = v;
			}
		}

		double m_AvgSplit = 0.0;
		double m_AvgCatchup = 0.0;

		Int64 m_CatchupStopTime = 0;

		double[] m_AvgC = new double[16];
		int m_AvgCLoc = 0;
		void resetAvgC()
		{
			for (int i = 0; i < 16; i++)
				m_AvgC[i] = 0;
			m_AvgCatchup = 0;
			m_AvgCLoc = 0;
		}
		void AvgCPush(double f)
		{
			m_AvgCatchup -= m_AvgC[m_AvgCLoc];
			f /= 16;
			m_AvgC[m_AvgCLoc++] = f;
			m_AvgCatchup += f;
			if (m_AvgCLoc >= 16)
				m_AvgCLoc = 0;
		}


		void AdjustRate(double splittime)
		{
			if (m_Unit == null)
			{
				VControl.MediaPosition = m_TargetFrame;
				m_CatchupStopTime = DateTime.Now.Ticks + (Int64)(ConvertConst.SecondToHundredNanosecond * 10);
				Rate = 0.0;
				return;
			}




			double spf = VControl.AverageTimePerFrame;
			if (spf == 0.0)
				spf = 0.033366599999999998;
			double fps = 1 / spf;
			long curframe = VControl.MediaPosition;
			long df = m_TargetFrame - curframe;
			double catchup = df * spf; // How many seconds (pos/neg to catch up to the target frame.
			if (catchup < -5 || catchup > 10)
			{
				if (DateTime.Now.Ticks > m_CatchupStopTime)
				{
					// We are way out of the loop...... We can only manage this if we do it every so often... no more
					// We are good to just nail this frame.
					curframe = VControl.MediaPosition = m_TargetFrame;
					//m_ForceTo = m_TargetFrame+10;
					df = 0;
					m_CatchupStopTime = DateTime.Now.Ticks + (Int64)(ConvertConst.SecondToHundredNanosecond * 10);
					m_AvgSplit = 0.0;
					resetAvgC();
				}
				else if (catchup < 0) // On vastly negetives or positives cap it out.
					catchup = 0;
				else if (catchup > 10)
					catchup = 10;
			}



			// OK  We want to catch up in one frame but try not to go over were we estimate we want to be.
			m_AvgSplit = 0.9 * m_AvgSplit + 0.1 * splittime;

			// Over the course of the next average frame... what rate do we need to be going to catchu p to this frame?
			// Over the course of 1 second what rate do we need to be traveling to catch up to this frame.
			AvgCPush(catchup);
			//m_AvgCatchup = 0.9 * m_AvgCatchup + 0.1 * catchup;

			double r = m_TargetRate + m_AvgCatchup;
			if (r < 0)
				r = 0;
			if (m_TargetFrame < m_ForceTo && VControl.MediaPosition < m_ForceTo)
				r = 0.25;
			Rate = r;
			/*
			l_TargetRate.Content = String.Format("Target Rate {0:F3}", m_TargetRate);
			l_Rate.Content = String.Format("SetRate {0:F3}", r);
			l_TargetFrame.Content = "Target frame "+m_TargetFrame.ToString();
			l_CurrentFrame.Content = "Current frame "+curframe.ToString();
			l_Difference.Content = "Diffence "+df.ToString();
			l_Split.Content = String.Format("Split {0:F3},Avg {1:F3}", splittime, m_AvgSplit);
			l_CatchupRate.Content = String.Format("Catchup {0:F3}, Avg {1:F3} ", catchup,m_AvgCatchup);
			l_Speed.Content = String.Format("MetersPS {0:F3}, Track {1:F3}",
					m_Unit.Statistics.Speed, ((Course.GPSSegment)m_Location.Segment).GPSData.mph2);
			l_TrackInfo.Content = String.Format("SegX {0:F3}, {1}", m_Location.SegmentX, m_Location.Segment.Num);
			*/
			/*
			String s = "";
			Course.GPSSegment gseg = m_Location.Segment as Course.GPSSegment;
			Course.GPSSegment ngseg;
			double len = 0.0;
			for (int i = 0; i < 50; i++,gseg = ngseg)
			{
				ngseg = gseg.Node.Next.Value as Course.GPSSegment;
				len = (ngseg.GPSData.seconds - gseg.GPSData.seconds) * gseg.GPSData.mps2;
				s += String.Format("{0:F5},{1:F5},{2:F5} ", gseg.Length, len, gseg.Length - len);
			}
			l_Text.Text = s;
			*/
		}

		private void BaseUnit_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsVisible)
				BeginOnUpdate();
			else
				EndOnUpdate();
		}
		// 
		int m_CurLap = -1;
		public double LastFrame;
		void OnUpdate( double splittime )
		{
			// While visible we will be updateing this every single time.
			if (FirstUnit && RacerMateOne.Unit.Active.Count > 0)
			{
				if (m_Unit != RacerMateOne.Unit.Active[0])
					m_Unit = RacerMateOne.Unit.Active[0];
			}
			if (m_Unit != null)
			{
				LastFrame = m_TargetFrame;
				Location = m_Unit.Statistics.LapDistance;
				if (m_CurLap != m_Unit.Statistics.Lap)
				{
					m_CurLap = m_Unit.Statistics.Lap;
					VControl.MediaPosition = m_TargetFrame;
					if (m_CurLap > 1)
						m_ForceTo = 0;
					m_AvgSplit = 0.0;
					resetAvgC();
					m_CatchupStopTime = DateTime.Now.Ticks + (Int64)(ConvertConst.SecondToHundredNanosecond * 10);
				}
				AdjustRate(splittime);
			}
		}

		bool m_bOnUpdate;
		void BeginOnUpdate()
		{
			if (m_bInit && IsVisible && !m_bOnUpdate)
			{
				m_bOnUpdate = true;
				RacerMateOne.Unit.OnUpdate += new RM1.UpdateEvent(OnUpdate);
			}
		}
		void EndOnUpdate()
		{
			if (m_bOnUpdate)
			{
				m_bOnUpdate = false;
				RacerMateOne.Unit.OnUpdate -= new RM1.UpdateEvent(OnUpdate);
				Rate = 0.0;
			}
		}
	}
}
