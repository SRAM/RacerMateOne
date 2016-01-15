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
	/// Interaction logic for HardwareLine.xaml
	/// </summary>
	public partial class HardwareLine : UserControl
	{
		//=========================================================================================================
		public static DependencyProperty NumberProperty = DependencyProperty.Register("Number", typeof(int), typeof(HardwareLine));
		public int Number
		{
			get { return (int)this.GetValue(NumberProperty); }
			set { this.SetValue(NumberProperty, value); }
		}
		//=========================================================================================================
		public static DependencyProperty ActiveProperty = DependencyProperty.Register("Active", typeof(bool), typeof(HardwareLine),
			new FrameworkPropertyMetadata(false,new PropertyChangedCallback(_ActiveChanged)));
		public bool Active
		{
			get { return (bool)this.GetValue(ActiveProperty); }
			set { this.SetValue(ActiveProperty, value); }
		}
		private static void _ActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((HardwareLine)d).OnActiveChanged();
		}
		//=========================================================================================================
		public static readonly RoutedEvent ChangedEvent =
			EventManager.RegisterRoutedEvent(
			"Changed", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(HardwareLine));


		public event RoutedEventHandler Changed
		{
			add { AddHandler(ChangedEvent, value); }
			remove { RemoveHandler(ChangedEvent, value); }
		}
		//=========================================================================================================
		public static readonly RoutedEvent CalibrateClickEvent =
			EventManager.RegisterRoutedEvent(
			"CalibrateClick", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(HardwareLine));
		public event RoutedEventHandler CalibrateClick
		{
			add { AddHandler(CalibrateClickEvent, value); }
			remove { RemoveHandler(CalibrateClickEvent, value); }
		}
		//=========================================================================================================



		public static bool StopChangeEvents = false;
		public RM1.Trainer ChangeTrainer;

		bool m_AccuwattButton = false;
		public bool AccuwattButton
		{
			get
			{
				return m_AccuwattButton;
			}
		}


		bool m_Attached = false;
		public TrainerUserConfigurable TrainerConfig { get; protected set; }
		RM1.Trainer m_Trainer;
		public RM1.Trainer LastTrainer { get; set; }
		public RM1.Trainer Trainer
		{
			get { return m_Trainer; }
			set
			{
                // Feb 6 2012 Paul BugRocket Ticket #12 We keep the return on same object, but modify visibility of the Extra StackPanel
                if (m_Trainer == value)
               {
                   if (m_Trainer == null || !m_Trainer.IsConnected)
                   {
                       Extra.Visibility = Visibility.Hidden;
                   }
                   else
                   {
                       Extra.Visibility = Visibility.Visible;
                       if (m_Trainer.Type == RM1.DeviceType.VELOTRON)
                       {
                           Teeth.Visibility = Visibility.Visible;
                           TeethValue.Text = "" + m_Trainer.VelotronData.ActualChainring;
                           Calibrate.Text = "Run Accuwatt";
                           m_AccuwattButton = true;
                       }
                       else
                       {
                           Teeth.Visibility = Visibility.Hidden;
                           Calibrate.Text = "Calibrate";
                           m_AccuwattButton = false;
                       }
                   }
                   return;
                }
                //end modifications
				if (m_Trainer != null)
					OnPadChanged(m_Trainer, null);
				if (m_Trainer != null)
					m_Trainer.OnPadChanged -= new RM1.TrainerEvent(OnPadChanged);

                LastTrainer = m_Trainer;
                m_Trainer = value;
                
				if (m_Trainer == null || !m_Trainer.IsConnected)
				{
					Extra.Visibility = Visibility.Hidden;
				}
				else
				{
					m_Trainer.OnPadChanged += new RM1.TrainerEvent(OnPadChanged);
					Extra.Visibility = Visibility.Visible;
					if (m_Trainer.Type == RM1.DeviceType.VELOTRON)
					{
						Teeth.Visibility = Visibility.Visible;
						TeethValue.Text = "" + m_Trainer.VelotronData.ActualChainring;
						Calibrate.Text = "Run Accuwatt";
						m_AccuwattButton = true;
					}
					else
					{
						Teeth.Visibility = Visibility.Hidden;
						Calibrate.Text = "Calibrate";
						m_AccuwattButton = false;
					}
				}
				if (!m_inchange)
				{
					foreach (ComboBoxItem item in Devices.Items)
					{
						if (item.Tag == m_Trainer)
						{
							Devices.SelectedItem = item;
							break;
						}
					}
				}
				if (m_Trainer != null)
					OnPadChanged(m_Trainer, null);
			}
		}




		public HardwareLine()
		{
			InitializeComponent();
		}

		~HardwareLine()
		{
			Detach();
		}

		private void btn_Loaded(object sender, RoutedEventArgs e)
		{
			BikeLabel.Content = "Bike " + Number;
			RM1.OnCalibrationChanged += new RM1.TrainerEvent(RM1_OnCalibrationChanged);
			RedoDropDown();
		}

		public void RedoDropDown()
		{
			if (TrainerConfig == null)
				return;
			Devices.Items.Clear();

			ComboBoxItem citem;
			citem = new ComboBoxItem();
			citem.Content = "Not yet configured";
			citem.Tag = null;
			Devices.Items.Add(citem);
			ComboBoxItem selitem = citem;
			foreach (RM1.Trainer trainer in RM1.HardwareList)
			{
				citem = new ComboBoxItem();
				citem.Content = trainer.CBLine;
				citem.Tag = trainer;
				if (trainer == m_Trainer)
					selitem = citem;
				Devices.Items.Add(citem);
			}
			if (selitem.Tag != null)
				Trainer = selitem.Tag as RM1.Trainer;
			Devices.SelectedItem = selitem;
		}

		/// <summary>
		/// 
		/// </summary>
		public void OnActiveChanged()
		{
			if (Active)
				Attach();
			if (!Active)
				Detach();
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Detach()
		{
			if (!m_Attached)
				return;
			RM1.OnTrainerInitialized -= new RM1.TrainerInitialized(RM1_OnTrainerInitialized);
		}

		/// <summary>
		///
		/// </summary>
		protected void Attach()
		{
			if (m_Attached)
				return;
			TrainerConfig = RM1_Settings.SavedTrainersList[Number-1];
			RM1.OnTrainerInitialized += new RM1.TrainerInitialized(RM1_OnTrainerInitialized);
			RedoDropDown();

		}

		void RM1_OnCalibrationChanged(RM1.Trainer trainer, object arguments)
		{
			RedoDropDown();
		}

		void RM1_OnTrainerInitialized(RM1.Trainer trainer, int left)
		{
			if (left == 0)
				RedoDropDown();
		}

		private bool m_inchange = false;
		private void Devices_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Devices.SelectedItem != null && !m_inchange)
			{
				m_inchange = true;
				ComboBoxItem item = Devices.SelectedItem as ComboBoxItem;
				Trainer = (item != null ? item.Tag as RM1.Trainer : null);
				if (LastTrainer != Trainer && !StopChangeEvents)
				{
					RoutedEventArgs args = new RoutedEventArgs(ChangedEvent);
					RaiseEvent(args);
				}
				m_inchange = false;
			}
		}
		static Color ms_Off1 = Color.FromArgb(255, 0, 0, 0);
		static Color ms_Off2 = Color.FromArgb(255, 0x77, 0x77, 0x77);
		static Color ms_On1 = Color.FromArgb(255, 0, 0xAA, 0x33);
		static Color ms_On2 = Color.FromArgb(255, 0, 0xFF, 0x88);

		private void OnPadChanged(RM1.Trainer trainer, object args)
		{
			if (Trainer.RawButtons == 0)
			{
				LED_c1.Color = ms_Off1;
				LED_c2.Color = ms_Off2;
			}
			else
			{
				LED_c1.Color = ms_On1;
				LED_c2.Color = ms_On2;
			}

		}


		private void TeethValue_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (m_Trainer == null || m_Trainer.VelotronData == null)
				return;
			int teeth = System.Convert.ToInt32(TeethValue.Text);
			if (teeth != m_Trainer.VelotronData.ActualChainring)
			{
				int save = m_Trainer.VelotronData.ActualChainring;
				m_Trainer.VelotronData.ActualChainring = teeth;
				if (m_Trainer.SetVelotron_trnr_Parameters() != RM1.DLLError.ALL_OK)
				{
					m_Trainer.VelotronData.ActualChainring = save;
					TeethValue.Text = save.ToString();
				}
				else
				{
					TrainerConfig.VelotronChainring = teeth;
					RM1_Settings.SaveToFile();
				}
			}
		}

		private delegate void _change(RM1.Trainer trainer);
		void change(RM1.Trainer trainer)
		{
			Trainer = trainer;
		}

		private void Identify_Click(object sender, RoutedEventArgs e)
		{
			Dialogs.HardwareLine_Select dlg = new Dialogs.HardwareLine_Select();
			dlg.Owner = AppWin.Instance;
			dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			dlg.ShowDialog(); //shows as modal
			if (dlg.Trainer != null)
				Dispatcher.BeginInvoke(DispatcherPriority.Normal, new _change(change), dlg.Trainer);
			/*
			if (dlg.Trainer != null)
			{
				m_inchange = true;
				Trainer = dlg.Trainer;
				foreach( ComboBoxItem item in Devices.Items)
				{
					if (dlg.Trainer == item.Tag as RM1.Trainer)
					{
						Devices.SelectedItem = item;
					}
				}
				if (!StopChangeEvents)
				{
					RoutedEventArgs args = new RoutedEventArgs(ChangedEvent);
					RaiseEvent(args);
				}
				m_inchange = false;
			}
			 */
		}

		private void Calibrate_Click(object sender, RoutedEventArgs e)
		{
			RaiseEvent(new RoutedEventArgs(CalibrateClickEvent));
		}

		private void btn_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsVisible)
				RedoDropDown();
		}

		private void btn_Unloaded(object sender, RoutedEventArgs e)
		{
			RM1.OnCalibrationChanged -= new RM1.TrainerEvent(RM1_OnCalibrationChanged);
		}
	}
}
