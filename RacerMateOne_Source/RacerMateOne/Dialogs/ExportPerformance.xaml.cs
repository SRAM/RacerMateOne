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
using System.Threading;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Media.Animation;

namespace RacerMateOne.Dialogs
{
	/// <summary>
	/// Interaction logic for TextLine.xaml
	/// </summary>
	public partial class ExportPerformance : Window
	{
		bool m_IsOK = false;
		public bool IsOK { get { return m_IsOK; } }

		String m_FileName;
		Perf m_Perf;

		BackgroundWorker m_bw;
		double m_StartAt;
		double m_EndAt;
		bool m_bIsCanceled;
		String m_ErrorString = null;

		public bool ExportSave;
		public bool PWXSave;
		public bool ReportSave;

		Pages.Modes.SavePerformance m_Save;


		public ExportPerformance(String filename)
		{
			InitializeComponent();
			m_FileName = filename;
		}

		public void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Step one ... is this performance cached.
			if (RacerMateOne.Pages.Modes.Staging.CachedPerformanceName == m_FileName &&
				RacerMateOne.Pages.Modes.Staging.CachedPerformance != null)
			{
				// Skip to the loading state.
				m_StartAt = 0;
				m_EndAt = 1;
				m_Perf = RacerMateOne.Pages.Modes.Staging.CachedPerformance;
				SavePerformance();
				return;
			}
			// Clear out the issue
			RacerMateOne.Pages.Modes.Staging.CachedPerformance = null;
			RacerMateOne.Pages.Modes.Staging.CachedPerformanceName = null;

			m_StartAt = 0;
			m_EndAt = 0.5;

			TopText.Text = "Loading performance file...";
			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += new DoWorkEventHandler(LoadPerformance);
			bw.WorkerReportsProgress = true;
			bw.WorkerSupportsCancellation = true;
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_LoadCompleted);
			bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
			bw.RunWorkerAsync();
			m_bw = bw;

		}

		void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			double progress = m_StartAt + (m_EndAt - m_StartAt) * (e.ProgressPercentage  / 100.0);
			double w = ProgressBar.ActualWidth;

			PerfProgressBar.Width = w * progress;
		}

		//==================================================================
		public void LoadPerformance(object sender, DoWorkEventArgs e)
		{
            App.SetDefaultCulture();

            BackgroundWorker bw = sender as BackgroundWorker;
			m_Perf = new Perf();
			if (!m_Perf.LoadRawTemps(bw, m_FileName))
				m_ErrorString = "Error in loading performance file";
			RacerMateOne.Pages.Modes.Staging.CachedPerformance = m_Perf;
			RacerMateOne.Pages.Modes.Staging.CachedPerformanceName = m_FileName;
		}
		

		//===================================================================
		void bw_LoadCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!m_bIsCanceled && m_ErrorString != null)
			{
				TopText.Text = m_ErrorString;
				ProgressBar.Visibility = Visibility.Hidden;
				Cancel.Text = "Close";
				return;
			}
			SavePerformance();
			m_StartAt = 0.500001;
			m_EndAt = 1.0;
		}
		public double OverridePercent;

		void SavePerformance()
		{
			if (m_bIsCanceled)
			{
				Close();
				return;
			}
			CourseType ct = m_Perf.CourseInfo.Type;

			ReportColumns rcolumn = null;
			if (ct == CourseType.ThreeD)
				rcolumn = ReportColumns.Report_3DRoadRacing.Selected_ReportColumns;
			else if (ct == CourseType.Video)
				rcolumn = ReportColumns.Report_RCV.Selected_ReportColumns;
			if (rcolumn == null)
			{
				rcolumn = m_Perf.CourseInfo.YUnits == CourseYUnits.Grade ? ReportColumns.Report_SpinScan :
					m_Perf.CourseInfo.YUnits == CourseYUnits.PercentAT ? ReportColumns.Report_WattTestingAT :
					ReportColumns.Report_WattTestingERG;
			}
			m_Save = new RacerMateOne.Pages.Modes.SavePerformance(rcolumn);
			m_Save.SaveReport = ReportSave;
			m_Save.ExportSave = ExportSave;
			m_Save.SavePWX = PWXSave;
			m_Save.Progress += new RacerMateOne.Pages.Modes.SavePerformance.ProgressEvent(cb_SaveProgress);
			m_Save.SavePerf = m_Perf; // Special save mode.
			m_Save.Save();

			TopText.Text = "Exporting reports...";
		}

		void cb_SaveProgress(double progress, bool done)
		{
			if (done)
			{
				Close();
			}
			double p = m_StartAt + (m_EndAt - m_StartAt) * (progress / 100.0);
			double w = ProgressBar.ActualWidth;
			PerfProgressBar.Width = w * p;
		}






		private void OK_Click(object sender, RoutedEventArgs e)
		{
			m_IsOK = true;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			m_bw.CancelAsync();
			m_bIsCanceled = true;
			TopText.Text = "Canceling request...";
		}
	}
}
