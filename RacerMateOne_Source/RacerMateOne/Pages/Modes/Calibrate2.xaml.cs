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

namespace RacerMateOne.Pages.Modes
{
    /// <summary>
    /// Interaction logic for Calibrate.xaml
    /// </summary>
    public partial class Calibrate2 : RideBase
    {
        public bool ExitToSelection = false;

        static Unit[] ms_UseUnits = new Unit[2];

        public static bool OkToUse()
        {
            if (!RM1_Settings.General.CalibrationCheck && !RM1_Settings.General.Commercial)
                return false;

            ms_UseUnits[0] = null;
            ms_UseUnits[1] = null;
            Unit.LoadFromSettings();
            int i;
            int c = 0;
            for (i = 0; i < 8 && c < 2; i++)
            {
                RM1.Trainer t = Unit.Units[i].Trainer;
                if (t != null && t.IsConnected && t.Type == RM1.DeviceType.COMPUTRAINER)
                    ms_UseUnits[c++] = Unit.Units[i];
            }
            return c >= 2;
        }


        const double ScaleIt = 60;
        double MaxTime = ScaleIt * 10; // In seconds;
        double AddedTime = 0.0;
        double AddTime = ScaleIt * 2; // 2 minutes.
        UnitSave m_Saved;

        public Calibrate2()
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
                UIElement elem = Holder.Children[0] as UIElement;
                Holder.Children.Remove(elem);
                HolderGrid.Children.Add(elem);

                m_Saved = new UnitSave();
                AppWin.Instance.Render3DOn();

                if (!OkToUse())
                    throw new Exception("Should not have entered this screen without 2 Computrainers");

                App.AllowPerfSave = false;

                Unit.DeactivateAll();

                m_Units[0] = ms_UseUnits[0];
                m_Units[1] = ms_UseUnits[1];

                Unit unit;
                unit = m_Units[0];
                Unit.AddNotify(unit, m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
                unit.IsActive = true;
                unit.ForceRider();
                unit.Bot = null;
                Stats.SetUnit(0, unit);
                Bar_1.AvgBarsOn = Polar_1.AvgBarsOn = true;
                Polar_1.Unit = unit;
                Bar_1.Unit = unit;
                Render_1.Unit = unit;
                SSBox_1.Unit = unit;

                unit = m_Units[1];
                unit.IsActive = true;
                //Unit.AddNotify(unit, m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
                unit.ForceRider();
                unit.Bot = null;
                Stats.SetUnit(1, unit);
                Bar_2.AvgBarsOn = Polar_2.AvgBarsOn = true;
                Polar_2.Unit = unit;
                Bar_2.Unit = unit;
                Render_2.Unit = unit;
                SSBox_2.Unit = unit;

                RideBase_Loaded(MainGrid);
                Controls.Render3D.SetupRiders();
                ShowMessage(Message_F1);
            });
        }

        Unit[] m_Units = new Unit[2];

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_Units[0] != null)
                Unit.RemoveNotify(m_Units[0], m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
            //if (m_Units[1] != null)
            //	Unit.RemoveNotify(m_Units[1], m_StatFlags, new Unit.NotifyEvent(OnUnitFlagsChanged));
            RideBase_Unloaded();
            AppWin.Instance.Render3DOff();
            if (m_Saved != null)
                m_Saved.Restore();
            AppWin.Instance.RenderCenter = false;
            RM1.OnCalibrationChanged -= new RM1.TrainerEvent(RM1_OnCalibrationChanged);

            App.AllowPerfSave = true;
        }

        void RM1_OnCalibrationChanged(RM1.Trainer trainer, object arguments)
        {
            if (m_Units[0] != null && m_Units[0].Trainer != null)
                c1_Calibrated_value.Text = m_Units[0].Statistics.CalibrationString;
            if (m_Units[1] != null && m_Units[1].Trainer != null)
                c2_Calibrated_value.Text = m_Units[1].Statistics.CalibrationString;
        }


        //=========================================
        StatFlags m_StatFlags = StatFlags.Time;
        void OnUnitFlagsChanged(Unit unit, StatFlags changed)
        {
            if ((changed & StatFlags.Time) != StatFlags.Zero)
            {
                UpdateTime();
            }
        }

        void UpdateTime()
        {
            double t = MaxTime - (m_Units[0].Statistics.Time - AddedTime);
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
            t = (m_Units[0].Statistics.Time - AddedTime);
            if (t > MaxTime)
                t = MaxTime;
            TimeBar.Width = w * t / MaxTime;

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
        bool[] m_Calibrated = new bool[2];

        protected override void Pause()
        {
            ShowMessage(Message_Done);
            base.Pause();

            DispatcherTimer dt = new DispatcherTimer(DispatcherPriority.Normal);
            dt.Tick += (s, e) =>
            {
                dt.Stop();
                m_Units[0].Trainer.CalibrateMode = true;
                m_Units[1].Trainer.CalibrateMode = true;
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
            m_Units[0].Trainer.CalibrateMode = false;
            m_Units[1].Trainer.CalibrateMode = false;
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
            double t = MaxTime - (m_Units[0].Statistics.Time - AddedTime);
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
            if (m_CurMessage == Message_NotCalibrated && m_Units[0] == unit)
            {
                Pause();
                return true;
            }
            int c = 0;
            if (m_Units[0].Trainer.CalibrateMode) c++;
            if (m_Units[1].Trainer.CalibrateMode) c++;
            if (c <= 0)
            {
                Pause();
                return true;
            }

            if (Unit.State == Statistics.State.Paused && unit == m_Units[0] || unit == m_Units[1])
            {
                c--;
                unit.Trainer.CalibrateMode = false;
                DispatcherTimer dt = new DispatcherTimer(DispatcherPriority.Normal);
                dt.Tick += (s, e) =>
                {
                    
                    dt.Stop();
                    bool readcal = false;
                    ShowMessage(Message_Calibrated);

                    c1_Calibrated.Visibility = c1_Fail.Visibility = c1_Save.Visibility = Visibility.Hidden;
                    if (m_Units[0].Trainer.CalibrateMode)
                        c1_Save.Visibility = Visibility.Visible;
                    else
                    {
                        readcal = m_Units[0].Statistics.IsCalibrated;  //going to force two reads
                        if (m_Units[0].Statistics.IsCalibrated)
                        {
                            c1_Calibrated_value.Text = m_Units[0].Statistics.CalibrationString;
                            c1_Calibrated.Visibility = Visibility.Visible;
                        }
                        else
                            c1_Fail.Visibility = Visibility.Visible;
                    }

                    c2_Calibrated.Visibility = c2_Fail.Visibility = c2_Save.Visibility = Visibility.Hidden;
                    if (m_Units[1].Trainer.CalibrateMode)
                        c2_Save.Visibility = Visibility.Visible;
                    else
                    {
                        readcal = m_Units[1].Statistics.IsCalibrated;  //going to force two reads
                        if (m_Units[1].Statistics.IsCalibrated)
                        {
                            c2_Calibrated_value.Text = m_Units[1].Statistics.CalibrationString;
                            c2_Calibrated.Visibility = Visibility.Visible;
                        }
                        else
                            c2_Fail.Visibility = Visibility.Visible;
                    }

                    c_BottomLine.Visibility = c <= 0 ? Visibility.Visible : Visibility.Hidden;
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

        Grid HolderGrid;

        private void Holder_Loaded(object sender, RoutedEventArgs e)
        {
            // Move the prolematic message into the fader gid
            HolderGrid = sender as Grid;
        }

        protected override void Scroll(Unit unit, int dir)
        {
            if (dir < 0)
                return;
            if (unit == m_Units[0])
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
            else if (unit == m_Units[1])
            {
                if (Polar_2.Visibility == Visibility.Visible)
                {
                    Polar_2.Visibility = Visibility.Collapsed;
                    Bar_2.Visibility = Visibility.Visible;
                }
                else
                {
                    Polar_2.Visibility = Visibility.Visible;
                    Bar_2.Visibility = Visibility.Collapsed;
                }
            }
        }


    }
}
