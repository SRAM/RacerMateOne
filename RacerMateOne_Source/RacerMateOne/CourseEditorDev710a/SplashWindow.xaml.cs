using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace RacerMateOne.CourseEditorDev
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
      
      
        public bool bShowing = false;
        public SplashWindow()
        {
            InitializeComponent();
           
            bShowing = true;
            //Loaded += (s,a) =>
            //{
            //    splashAnimationTimer = new DispatcherTimer();
            //    splashAnimationTimer.Interval = TimeSpan.FromMilliseconds(10);
            //    int dotsCount = lblProgress.Content.ToString().Replace(Loading, string.Empty).Length;
            //    splashAnimationTimer.Tick += (sender, arg) =>
            //    {
            //        dotsCount = (dotsCount < 6) ? dotsCount++ : 0;
            //        lblProgress.Content = Loading.PadRight(Loading.Length + dotsCount, '.');
            //        percent = (percent < 10.0) ? percent++ : 0;
            //        OurProgressBar.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render , new DispatcherOperationCallback(delegate
            //       {
            //           OurProgressBar.Value = percent * 10.0;
            //           //do what you need to do on UI Thread
            //           return null;
            //       }), null);

            //    };
            //    splashAnimationTimer.Start();
            //};
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            bShowing = false;
          //  e.Cancel = true;
            //do my stuff before closing
        }

    }
}
