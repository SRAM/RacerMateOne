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

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for ButtonBox.xaml
	/// </summary>
	public partial class ButtonBox : UserControl
	{
		protected RM1.Trainer m_Trainer;
		public RM1.Trainer Trainer
		{
			get { return m_Trainer; }
			set
			{
				AttachTrainer(value);
			}
		}

		public ButtonBox()
		{
			InitializeComponent();
			//m_EHandler = new EventHandler(Rendering);
			Opacity = 0.0;
		}
		public void DeatchTrainer()
		{
			if (m_Trainer != null)
			{
				m_Trainer.OnPadChanged -= new RM1.TrainerEvent( OnPadChanged );
				m_Trainer.OnClosed -= new RM1.TrainerEvent( OnTrainerClosed );
				m_Trainer = null;
			}
		}
		protected void AttachTrainer(RM1.Trainer trainer)
		{
			if (m_Trainer == trainer)
				return;
			if (m_Trainer != null)
				DeatchTrainer();

			m_Trainer = trainer;
			if (m_Trainer != null)
			{
				m_Trainer.OnPadChanged += new RM1.TrainerEvent( OnPadChanged );
				m_Trainer.OnClosed += new RM1.TrainerEvent( OnTrainerClosed );
			}
		}
	//	EventHandler m_EHandler;

	//	bool m_bStarted;
	//	long m_StartTime;
	//	double m_CurOpacity;
		const double OnTime = 0.3;
		const double OffTime = 1.3;
		const double OffLength = 0.3;
		void StartCycle()
		{
            pad.Visibility = Visibility.Collapsed;
            //if (RM1_Settings.General.ShowKeypadPresses)
            //{
            //    m_StartTime = DateTime.Now.Ticks;
            //    if (m_bStarted)
            //        return;
            //    m_bStarted = true;
            //    m_CurOpacity = pad.Opacity = 0.0;
            //    pad.Visibility = Visibility.Visible;
            //    CompositionTarget.Rendering += m_EHandler;
            //}
		}
		void StopCycle()
		{
            pad.Visibility = Visibility.Collapsed; 
            //if (m_bStarted)
            //{
            //    pad.Visibility = Visibility.Collapsed;
            //    CompositionTarget.Rendering -= m_EHandler;
            //    m_bStarted = false;
            //}
		}

		void Rendering(object sender, EventArgs e)
		{
            //double d = ConvertConst.HundredNanosecondToSecond * (DateTime.Now.Ticks - m_StartTime);
            //double opacity = 1.0;

            //if (d < OnTime)
            //{
            //    opacity = d / OnTime;
            //    if (opacity < m_CurOpacity)
            //        opacity = m_CurOpacity;
            //}
            //else if (d > OffTime)
            //{
            //    opacity = 1.0 - ((d - OffTime) / OffLength);
            //    if (opacity < 0)
            //    {
            //        StopCycle();
            //        return;
            //    }
            //}
            //if (opacity != m_CurOpacity)
            //{
            //    m_CurOpacity = pad.Opacity = opacity;
			//}
		}



		private void OnPadChanged(RM1.Trainer trainer, object obj )
		{
            //StartCycle();

            //Fn.Visibility = (trainer.RawButtons & 0x01) != 0 ? Visibility.Visible:Visibility.Hidden;
            //F1.Visibility = (trainer.RawButtons & 0x02) != 0 ? Visibility.Visible : Visibility.Hidden;
            //F2.Visibility = (trainer.RawButtons & 0x04) != 0 ? Visibility.Visible : Visibility.Hidden;
            //F3.Visibility = (trainer.RawButtons & 0x08) != 0 ? Visibility.Visible : Visibility.Hidden;
            //Up.Visibility = (trainer.RawButtons & 0x10) != 0 ? Visibility.Visible : Visibility.Hidden;
            //Down.Visibility = (trainer.RawButtons & 0x20) != 0 ? Visibility.Visible : Visibility.Hidden;
		}
		private void OnTrainerClosed(RM1.Trainer trainer,object arguments)
		{
			if (trainer == m_Trainer)
				DeatchTrainer();
		}
		private void pad_Unloaded(object sender, RoutedEventArgs e)
		{
			StopCycle();
		}
	}
}
