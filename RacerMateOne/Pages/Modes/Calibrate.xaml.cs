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
using System.Diagnostics;
namespace RacerMateOne.Pages.Modes
{
    /// <summary>
    /// Interaction logic for Calibrate.xaml
    /// </summary>
    public partial class Calibrate : RideBase
    {
        static Unit ms_UseUnit;
        public static bool PreUse()
        {
            if (!RM1_Settings.General.CalibrationCheck && !RM1_Settings.General.Commercial)
                return false;

            ms_UseUnit = null;
            Unit.LoadFromSettings();
            int i;
            for (i = 0; i < 8; i++)
            {
                RM1.Trainer t = Unit.Units[i].Trainer;
                if (t != null && t.IsConnected && t.Type == RM1.DeviceType.COMPUTRAINER)
                {
                    if (ms_UseUnit != null)
                        return false;
                    ms_UseUnit = Unit.Units[i];
                }
            }
            return ms_UseUnit != null;
        }


        public bool ExitToSelection = false;


        const double ScaleIt = 60;
        double MaxTime = ScaleIt * 10; // In seconds;   //should be ScaleIt *10
        double AddedTime = 0.0;
        double AddTime = ScaleIt * 2; // 2 minutes.  //should be ScaleIt *2
        int m_UnitNumber;
        UnitSave m_Saved;
        public int UnitNumber
        {
            get { return m_UnitNumber; }
            set
            {
                //if (m_UnitNumber == value)
                //    return;
                m_UnitNumber = value;
                ms_UseUnit = Unit.Units[m_UnitNumber];
               SetUnitNumber();
            }
        }


        public Calibrate()
        {
            InitializeComponent();
            if (!AppWin.IsInDesignMode)
                Background = Brushes.Transparent;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RM1.OnCalibrationChanged += new RM1.TrainerEvent(RM1_OnCalibrationChanged);

            AppWin.Instance.MainRender3D.Init();
            AppWin.Instance.MainRender3D.Opacity = 0.0;
            AppWin.Instance.RenderCenter = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
            {
                App.AllowPerfSave = false;
                m_Saved = new UnitSave();
                AppWin.Instance.Render3DOn();
                RideBase_Loaded(MainGrid);
                SetUnitNumber();
                ShowMessage(Message_F1);
            });
        }

        void RM1_OnCalibrationChanged(RM1.Trainer trainer, object arguments)
        {
            // Thread.Sleep(500);

            if (m_Unit != null && m_Unit.Trainer != null)
            {
                CalibrationValue.Text = m_Unit.Statistics.CalibrationString;
            }
        }


        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_Unit != null)
                Unit.RemoveNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
            RideBase_Unloaded();
            AppWin.Instance.Render3DOff();
            if (m_Saved != null)
                m_Saved.Restore();
            AppWin.Instance.RenderCenter = false;
            RM1.OnCalibrationChanged -= new RM1.TrainerEvent(RM1_OnCalibrationChanged);
            App.AllowPerfSave = true;
        }

        Unit m_Unit;

        void SetUnitNumber()
        {
            if (!m_bInit)
                return;
            if (ms_UseUnit == null)
                ms_UseUnit = Unit.Units[m_UnitNumber];
            Unit.LoadFromSettings();
           // PreUse();
            Unit.ClearAll();
            ms_UseUnit.IsActive = true;
            
            Unit unit = Unit.RaceUnit[0];
            if (unit != m_Unit)
            {
                Unit.RemoveNotify(m_Unit, m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
                Unit.AddNotify(unit, m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
                m_Unit = unit;
            }
            if (unit.Rider == null)
            {
                unit.Rider = Riders.RidersList.First();
            }
            TurnOnKeypads();
            unit.Bot = null;

            // Load up the stuff.
            Stats.Unit = unit;
            Bar_1.AvgBarsOn = Polar_1.AvgBarsOn = true;
            Polar_1.Unit = unit;
            Bar_1.Unit = unit;
            Render_1.Unit = unit;
            SSBox_1.Unit = unit;
            Controls.Render3D.SetupRiders();
        }

        //=========================================
        StatFlags m_StatFlags = StatFlags.Time | StatFlags.Calibration;
        void OnUnitFlagsChanged(Unit unit, StatFlags changed)
        {
            if ((changed & StatFlags.Time) != StatFlags.Zero)
            {
                UpdateTime();
            }
            if ((changed & StatFlags.Calibration) != StatFlags.Zero)
            {
                CalibrationValue.Text = m_Unit.Statistics.CalibrationString;
            }
        }

        void UpdateTime()
        {

            double t = MaxTime - (m_Unit.Statistics.Time - AddedTime);

            if (t < 0.0 && m_CurMessage == null)
            {
                t = 0.0;
                Pause();	// Pause it and deal with time.
            }
            // Adjust bar
            //TimeBar.Width = w - w * t / MaxTime;

            if (t < 0)
                t = 0;
            d_Timer.Content = Statistics.SecondsToTimeString(t);

            double w = TimeGrid.ActualWidth;
            t = (m_Unit.Statistics.Time - AddedTime);
            if (t > MaxTime)
                t = MaxTime;
            TimeBar.Width = w * t / MaxTime;

        }

        protected override void Scroll(Unit unit, int dir)
        {
            if (dir < 0)
                return;

            if (unit == m_Unit)
            {
                if (Polar_1.Visibility == Visibility.Visible)
                {
                    Polar_1.Visibility = Visibility.Collapsed;
                    Bar_1.Visibility = Visibility.Visible;
                }
                else
                {
                    Polar_1.Visibility = Visibility.Visible;
                    Bar_1.Visibility = Visibility.Collapsed;
                }
            }
        }

        Controls.Fade m_CurMessage = null;
        void ShowMessage(Controls.Fade msg)
        {
            if (m_CurMessage == msg)
                return;
            if (m_CurMessage != null)
                m_CurMessage.FadeTo = 0.0;
            m_CurMessage = msg;
            if (m_CurMessage != null)
                m_CurMessage.FadeTo = 1.0;
        }


        void Exit()
        {
            if (ExitToSelection)
            {
                NavigationService.Navigate(new Pages.Selection());
            }
            else
                NavigationService.GoBack();

        }



        //===================================================
        protected override void Start()
        {
            ShowMessage(null);
            base.Start();
        }
        protected override void Reset()
        {

            base.Reset();
        }
        protected override void Pause()
        {
            ShowMessage(Message_Done);
            double t = MaxTime - (m_Unit.Statistics.Time - AddedTime);
            base.Pause();
            DispatcherTimer dt = new DispatcherTimer(DispatcherPriority.Normal);
            dt.Tick += (s, e) =>
            {
                dt.Stop();
                m_Unit.Trainer.CalibrateMode = true;
            };
            dt.Interval = TimeSpan.FromMilliseconds(500);
            dt.Start();
        }
        void base_Unpause()
        {
            base.UnPause();
        }

        protected override void UnPause()
        {
            bool exit = m_CurMessage == Message_NotCalibrated || m_CurMessage == Message_Calibrated;
            m_Unit.Trainer.CalibrateMode = false;
            DispatcherTimer dt = new DispatcherTimer(DispatcherPriority.Normal);
            dt.Tick += (s, e) =>
            {
                dt.Stop();
                base_Unpause();
                if (exit)
                {
                    Exit();
                    return;
                }
            };
            dt.Interval = TimeSpan.FromMilliseconds(500);
            dt.Start();
            double t = MaxTime - (m_Unit.Statistics.Time - AddedTime);
            if (t <= 0.0)
            {
                AddedTime += AddTime;
                // Adjust the Label's by two minutes.
                int c = (int)Math.Round(AddedTime / ScaleIt) + 1;
                foreach (object obj in TimeGrid.Children)
                {
                    Grid g = obj as Grid;
                    if (g != null)
                    {
                        Label l = g.Children[1] as Label;
                        if (l != null)
                        {
                            l.Content = String.Format("{0}:00", c);
                            c += 1;
                        }
                    }
                }
            }

            ShowMessage(null);
        }
        protected override bool OverrideF3(Unit unit)
        {
            if (m_CurMessage == Message_NotCalibrated || m_CurMessage == Message_Calibrated)
            {
                Pause();
                return true;
            }
            if (Unit.State == Statistics.State.Paused)
            {
                m_Unit.Trainer.CalibrateMode = false;
                //Thread.Sleep(1000);
                DispatcherTimer dt = new DispatcherTimer(DispatcherPriority.Normal);
                dt.Tick += (s, e) =>
                {
                    dt.Stop();
                    bool readcal = m_Unit.Statistics.IsCalibrated;  //going to force two reads
                    if (m_Unit.Statistics.IsCalibrated)
                    {
                        CalibrationValue.Text = m_Unit.Statistics.CalibrationString;
                        ShowMessage(Message_Calibrated);
                    }
                    else
                    {
                        ShowMessage(Message_NotCalibrated);
                    }
                    return;
                };
                dt.Interval = TimeSpan.FromMilliseconds(900);
                dt.Start();
            }
            return true;
        }

        TextBlock CalibrationValue;
        private void TextBlock_Loaded_2(object sender, RoutedEventArgs e)
        {
             CalibrationValue = sender as TextBlock;
        }

    }
}
