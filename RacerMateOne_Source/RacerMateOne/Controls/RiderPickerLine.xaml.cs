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

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for RiderPickerLine.xaml
	/// </summary>
	public partial class RiderPickerLine : UserControl
	{
		public RiderPickerLine()
		{
			InitializeComponent();
		}

		private TrainerUserConfigurable m_Trainer;
		public TrainerUserConfigurable Trainer
		{
			get { return m_Trainer; }
			set
			{
				if (m_Trainer == value)
					return;
				m_Trainer = value;
				if (m_Trainer == null)
				{
					DeviceType.Text = "";
					RiderName.Text = "";
					return;
				}
				if (m_Trainer.RememberedDeviceType != "Unknown")
					DeviceType.Text = m_Trainer.RememberedDeviceType;
				Rider rider = Riders.FindRiderByKey(m_Trainer.PreviousRiderKey);
				if (rider == null)
				{
					RiderName.Text = "";
				}
				else
				{
					RiderName.Text = rider.IDName;
				}
			}
		}
	}
}
