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

namespace RacerMateOne.Dialogs
{
	/// <summary>
	/// Interaction logic for Edit_DelayBot.xaml
	/// </summary>
    public partial class SavePerformance : Window
	{

        static private bool CloseWhenDone = false;
        static private int ProgressBarMax = 100;
        static private int ProgressBarMin = 0;
        static private int ProgressUnits = 1;
        static private int ProgressPercent = 0;
        BackgroundWorker bw = null;
        string srcFilename = "";

        bool m_bClosing;

        public SavePerformance(string filename)
		{
			InitializeComponent();

            srcFilename = filename;

            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoSaveWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            if ((RM1_Settings.General.ExportSave && !RM1_Settings.General.ExportPrompt) && !RM1_Settings.General.ReportPrompt)
            {
                // Only works if it background worker is not busy
                if (!bw.IsBusy)
                {
                    // autoclose after saving
                    CloseWhenDone = true;
                    Save();
                }
            }
        }

        private void SavePerformance_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Save_Click(object sender, RoutedEventArgs e)
		{
            // Only works if it background worker is not busy
            if (!bw.IsBusy)
            {
                // autoclose
                CloseWhenDone = true;
                Save();
            }
		}

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy)
                bw.CancelAsync();
            else
                CloseThis();
        }

        private void CloseThis()
        {
            m_bClosing = true;
            Fade.Stop();
            FadeAnim.To = 0.0f;
            Fade.Begin();
        }

        private void FadeAnim_Completed(object sender, EventArgs e)
		{
			if (m_bClosing)
				Close();
		}

        public void Save()
		{
            ProgressPercent = ProgressBarMin;
            UpdateProgressBar(ProgressPercent);

            bw.RunWorkerAsync(srcFilename);
        }

        // Todo - enable progress bar stuff
        // This delegate enables asynchronous calls
        delegate void UpdateProgressBarCallback(int ProgressPercentage);
        private void UpdateProgressBar(int ProgressPercentage)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.PerfProgressBar.Dispatcher.CheckAccess())
            {
                double maxVal = this.PerfProgressBarRect.ActualWidth;
                this.PerfProgressBar.Width = maxVal * ProgressPercentage / 100;

                //this.ProgressBar.Minimum = ProgressBarMin;
                //this.ProgressBar.Maximum = ProgressBarMax;
                //this.ProgressBar.Value = ProgressPercentage;
            }
            else
            {
                UpdateProgressBarCallback d = new UpdateProgressBarCallback(UpdateProgressBar);
                this.Dispatcher.Invoke(d, new object[] { ProgressPercentage });
            }
        }
        // This event handler Shows progress
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressBar(e.ProgressPercentage);
        }

        // This event handler handles end of worker thread
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // The user canceled the operation.
                CloseThis();
            }
            else if (e.Error != null)
            {
                // There was an error during the operation.
                string msg = String.Format("An error occurred: {0}", e.Error.Message);
                // For testing
                //MessageBox.Show(msg);
            }
            else
            {
                // Set the progress bar to complete
                UpdateProgressBar(ProgressBarMax);

                // The operation completed normally.
                string msg = String.Format("Result = {0}", e.Result);
                // For testing
                //MessageBox.Show(msg);

                // Called with the flag to autoclose
                if (CloseWhenDone)
                {
                    CloseWhenDone = false; // reset back to off
                    CloseThis();
                }
            }
        }

		  /**********************************************************************************************

		  **********************************************************************************************/

		  private void bw_DoSaveWork(object sender, DoWorkEventArgs e) {
            App.SetDefaultCulture();

            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.
            BackgroundWorker bw = sender as BackgroundWorker;

            // Extract the argument.
            string filename = (string)e.Argument;

            // Start the time-consuming operation.
            ProcessSave(bw, filename);

            e.Result = true;

            // If the operation was canceled by the user, 
            // set the DoWorkEventArgs.Cancel property to true.
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }

		  /**********************************************************************************************

		  **********************************************************************************************/

		  private void ProcessSave(BackgroundWorker bw, string filename) {

            if (0 < filename.Length)  {
                int numFiles = 1;
                if (RM1_Settings.General.ExportSave)
                    numFiles += 2;
                if (RM1_Settings.General.ReportPrompt)
                    numFiles++;
                if (numFiles > 0)
                    ProgressUnits = 100 / numFiles;

                Perf perf = new Perf();
                perf.LoadRawTemps(filename);

                ProgressPercent += ProgressUnits;
                UpdateProgressBar(ProgressPercent);

                // periodically checks if the work is cancelled
                if (bw != null && bw.CancellationPending)
                    return;

                ReportColumns reportCols;
                reportCols = ReportColumns.Report_3DRoadRacing.Selected_ReportColumns;
                StatFlags ExportFlags = reportCols.StatFlags;

                // Export here 

                if (RM1_Settings.General.ExportSave)  {
                    perf.ExportPWXFromLoadedFile(bw, ExportFlags);
                    ProgressPercent += ProgressUnits;
                    UpdateProgressBar(ProgressPercent);

                    perf.ExportCSVFromLoadedFile(bw, ExportFlags);
                    ProgressPercent += ProgressUnits;
                    UpdateProgressBar(ProgressPercent);
                }

                // periodically checks if the work is cancelled
                if (bw != null && bw.CancellationPending)
                    return;

                // Save report here if prompted
                if (RM1_Settings.General.ReportPrompt)
                {
                    perf.SaveReportFromLoadedFile(bw, ExportFlags);
                    ProgressPercent += ProgressUnits;
                    UpdateProgressBar(ProgressPercent);
                }
            }
            else  {
                List<Unit> saveUnits = Unit.Active;
                if (saveUnits.Count > 0)
                {
                    int numUnits = 0;
                    foreach (Unit unit in saveUnits)
                    {
                        if (unit.IsActive && (unit.IsDemoUnit || unit.IsPerson))
                        {
                            numUnits++;
                        }
                    }

                    int numFiles = 0;
                    if (RM1_Settings.General.ExportSave)
                        numFiles += 2;
                    if (RM1_Settings.General.ReportPrompt)
                        numFiles += 1;
                    numFiles = numUnits * numFiles;
                    if (numFiles > 0)
                        ProgressUnits = 100 / numFiles;

                    foreach (Unit unit in saveUnits)
                    {
                        if (unit.IsActive && (unit.IsDemoUnit || unit.IsPerson))
                        {
                            // periodically checks if the work is cancelled
                            if (bw.CancellationPending)
                                return;

                            Unit thisUnit = unit;
                            Statistics stats = thisUnit.Statistics;
                            Perf PerfContainer = stats.PerfContainer;

                            Rider rider = unit.Rider;
                            int iRider = unit.iRider;

                            ReportColumns reportCols;
                            reportCols = ReportColumns.Report_3DRoadRacing.Selected_ReportColumns;
                            StatFlags ExportFlags = reportCols.StatFlags;

                            // Export here 
#if SAVE_IN_RAM
									if (RM1_Settings.General.ExportSave)  {
                                PerfContainer.ExportPWX(bw, stats, unit, ExportFlags);
                                ProgressPercent += ProgressUnits;
                                UpdateProgressBar(ProgressPercent);

                                PerfContainer.ExportCSV(bw, stats, unit, ExportFlags);
                                ProgressPercent += ProgressUnits;
                                UpdateProgressBar(ProgressPercent);
                            }
#endif

                            // periodically checks if the work is cancelled
                            if (bw.CancellationPending)
                                return;

                            // Save report here if prompted
                            if (RM1_Settings.General.ReportPrompt)
                            {
                                PerfContainer.SaveReport(bw, stats, unit, ExportFlags);
                                ProgressPercent += ProgressUnits;
                                UpdateProgressBar(ProgressPercent);
                            }

                            // periodically checks if the work is cancelled
                            if (bw.CancellationPending)
                                return;
                        }
                    }
                }
            }
        }

	}
}
