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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;


namespace RacerMateOne.Pages.Modes
{
	public class SavePerformance
	{
		//private int ProgressBarMax = 100;
		private int ProgressBarMin = 0;
		//private int ProgressUnits = 1;
		private int ProgressPercent = 0;
		BackgroundWorker bw = null;

		bool m_bStarted = false;
		bool m_bExportSave = false;
		bool m_bSaveReport = false;
		bool m_bSavePWX = false;



		bool m_bLaunchProgram = false;

		public delegate void ProgressEvent(double progress,bool done);
		public event ProgressEvent Progress;

		public bool ExportSave
		{
			get { return m_bExportSave; }
			set
			{
				if (m_bStarted)
					return;
				m_bExportSave = value;
			}
		}
		public bool SaveReport
		{
			get { return m_bSaveReport; }
			set
			{
				if (m_bStarted)
					return;
				m_bSaveReport = value;
			}
		}
		public bool SavePWX
		{
			get { return m_bSavePWX; }
			set
			{
				if (m_bStarted)
					return;
				m_bSavePWX = value;
			}
		}
		public bool LaunchProgram
		{
			get { return m_bLaunchProgram; }
			set
			{
				if (m_bStarted)
					return;
				m_bLaunchProgram = value;
			}
		}

		ReportColumns m_ReportColumns;
		public SavePerformance(ReportColumns rc)
		{
			m_ReportColumns = rc;
			bw = new BackgroundWorker();
			bw.WorkerSupportsCancellation = true;
			bw.WorkerReportsProgress = true;
			bw.DoWork += new DoWorkEventHandler(bw_DoSaveWork);
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
			bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
		}

		public void Save()
		{
			if (m_bStarted)
				return;
			m_bStarted = true;

			ProgressPercent = ProgressBarMin;
			bw.RunWorkerAsync();
		}

		// This event handler Shows progress
		private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (Progress != null)
				Progress(e.ProgressPercentage / 100.0,false);
		}

		// This event handler handles end of worker thread
		private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Progress(1.0, true);
		}

		public void Cancel()
		{
			bw.CancelAsync();
		}

		public Perf SavePerf;

		private void bw_DoSaveWork(object sender, DoWorkEventArgs e)
		{
            App.SetDefaultCulture();

            // Do not access the form's BackgroundWorker reference directly.
			// Instead, use the reference provided by the sender parameter.
			BackgroundWorker bw = sender as BackgroundWorker;

			// Extract the argument.
			//int arg = (int)e.Argument;

			// Start the time-consuming operation.
			if (SavePerf != null)
				ProcessSaveFromPerf(bw, SavePerf);
			else
				ProcessSave(bw);

			e.Result = true;

			// If the operation was canceled by the user, 
			// set the DoWorkEventArgs.Cancel property to true.
			if (bw.CancellationPending)
			{
				e.Cancel = true;
			}
		}

		public class PerfData
		{
			public Unit Unit;
			public Perf Perf;
			public Rider Rider;
			public int iRider;
			public PerfData(Unit unit)
			{
				Unit = unit;
				Perf = unit.Statistics.PerfContainer;
				Rider = unit.Rider;
				iRider = unit.iRider;
			}
		}

		private void ProcessSave(BackgroundWorker bw)
		{
			List<PerfData> perfdata = new List<PerfData>();
			List<Unit> saveUnits = Unit.Active;
			int progresscount;
			if (saveUnits.Count > 0)
			{
				foreach (Unit unit in saveUnits)
				{
					if (unit.IsActive && (unit.IsDemoUnit || unit.IsPerson))
						perfdata.Add(new PerfData(unit));
				}

				int numFiles = 0;
				if (SavePWX)
					numFiles++;
				if (ExportSave)
					numFiles++;
				if (SaveReport)
					numFiles++;

				progresscount = perfdata.Count * numFiles;
				if (progresscount == 0)
					progresscount = 1;
				int cnt = 0;
				double per;

				foreach (PerfData pd in perfdata)
				{

					if (bw.CancellationPending)
						return;

					if (SavePWX)
					{
                        pd.Perf.ExportPWX(bw, pd.Unit.Statistics, pd.Unit, m_ReportColumns.StatFlags);
						cnt++;
						per = cnt * 100.0 / progresscount;
						bw.ReportProgress((int)per, per);
					}

					if (bw.CancellationPending)
						return;

					if (ExportSave)
					{
                        pd.Perf.ExportCSV(bw, pd.Unit.Statistics, pd.Unit, m_ReportColumns.StatFlags);
						cnt++;
						per = cnt * 100.0 / progresscount;
						bw.ReportProgress((int)per, per);
					}

					if (bw.CancellationPending)
						return;


					// Save report here if prompted
					if (SaveReport)
					{
                        pd.Perf.SaveReport(bw, pd.Unit.Statistics, pd.Unit, m_ReportColumns.StatFlags);
						cnt++;
						per = cnt * 100.0 / progresscount;
						bw.ReportProgress((int)per, per);
					}
					
					if (bw.CancellationPending)
						return;
				}
			}
		}

		private void ProcessSaveFromPerf(BackgroundWorker bw, Perf perf)
		{
			List<PerfData> perfdata = new List<PerfData>();
			List<Unit> saveUnits = Unit.Active;
			int progresscount;
			int numFiles = 0;
			if (SavePWX)
				numFiles++;
			if (ExportSave)
				numFiles++;
			if (SaveReport)
				numFiles++;

			progresscount = numFiles;
			if (progresscount == 0)
				progresscount = 1;
			int cnt = 0;
			double per;
			if (bw.CancellationPending)
				return;

			if (SavePWX)
			{
				//perf.ExportPWXFromLoadedFile(bw, m_ReportColumns.StatFlags); //somehow there were 2 entries here.
				perf.ExportPWXFromLoadedFile(bw, m_ReportColumns.StatFlags);
				cnt++;
				per = cnt * 100.0 / progresscount;
				bw.ReportProgress((int)per, per);
			}

			if (bw.CancellationPending)
				return;

			if (ExportSave)
			{
				perf.ExportCSVFromLoadedFile(bw, m_ReportColumns.StatFlags);
				cnt++;
				per = cnt * 100.0 / progresscount;
				bw.ReportProgress((int)per, per);
			}
			if (bw.CancellationPending)
				return;


			// Save report here if prompted
			if (SaveReport)
			{
				perf.SaveReportFromLoadedFile(bw, m_ReportColumns.StatFlags);
				cnt++;
				per = cnt * 100.0 / progresscount;
				bw.ReportProgress((int)per, per);
			}

			if (bw.CancellationPending)
				return;
		}
	}
}
