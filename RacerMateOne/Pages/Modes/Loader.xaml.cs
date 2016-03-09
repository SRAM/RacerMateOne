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
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel; // CancelEventArgs
using System.Diagnostics;

namespace RacerMateOne.Pages.Modes
{
	/// <summary>
	/// Interaction logic for Loader.xaml
	/// </summary>
	public partial class Loader : Page
	{
		//public RideOptionsAll Options = new RideOptionsAll();
        //protected class TabInfo
        //{
        //    public String[] Tags;
        //}

		DoWorkEventHandler m_baseWork;
		RunWorkerCompletedEventHandler m_baseComplete;
		ProgressChangedEventHandler m_baseProgress;
		object m_baseArgs;
		Page m_baseNextPage;
		BackgroundWorker m_bw;
		bool m_bCanceled;

		public Loader(String title, Page nextpage, Object args,
			DoWorkEventHandler work, RunWorkerCompletedEventHandler complete, ProgressChangedEventHandler progress )
		{
			InitializeComponent();
			m_baseNextPage = nextpage;
			m_baseWork = work;
			m_baseComplete = complete;
			m_baseProgress = progress;
			m_baseArgs = args;
			Dlg_TopLabel.Content = title;
            Dlg_f1.Visibility = Visibility.Hidden;
           
		}
		void bw_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
           // Dlg_f1.Visibility = Visibility.Visible;
            if (m_baseComplete != null)
				m_baseComplete(this, e);
           // Debug.WriteLine("Completed lader BW and now will pull a navigation");
            if (m_bCanceled || m_baseNextPage == null)
            { 
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(m_baseNextPage);
            }
		}
		public double OverridePercent;
		void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
            //Debug.WriteLine("prog changed");
            OverridePercent = e.ProgressPercentage;
			if (m_baseProgress != null)
				m_baseProgress(this, e);
			double progress = OverridePercent / 100.0;
			double w = Dlg_ProgressBar.ActualWidth;
			PerfProgressBar.Width = w * progress;
            //Debug.WriteLine("Width = " + PerfProgressBar.Width);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{

			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += new DoWorkEventHandler(m_baseWork);
			bw.WorkerReportsProgress = true;
			bw.WorkerSupportsCancellation = true;
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_Completed);
			bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
			bw.RunWorkerAsync(m_baseArgs);
			m_bw = bw;
           
		}

		//=============================================================
        //private void Options_Click(object sender, RoutedEventArgs e)
        //{
        //    NavigationService.Navigate(new Pages.RideOptions());
        //}
        //private void Back_Click(object sender, RoutedEventArgs e)
        //{
        //    NavigationService.GoBack();
        //}

		private void Dlg_f1_Click(object sender, RoutedEventArgs e)
		{
            //if (m_bCanceled || m_baseNextPage == null)
            //{
            //    NavigationService.GoBack();
            //}
            //else
            //{
            //    NavigationService.Navigate(m_baseNextPage);
            //}
		}

		private void Dlg_f2_Click(object sender, RoutedEventArgs e)
		{
			m_bCanceled = true;
			m_bw.CancelAsync();
		}

	}
}

