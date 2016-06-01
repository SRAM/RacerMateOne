using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RacerMateOne
{
	/// <summary>
	/// Encapsulates only the user configurable (and saved) settings of a trainer, all those not discoverable on scan
	/// </summary>
	public class TrainerUserConfigurable //: INotifyPropertyChanged
	{
		// Velotron specific properties
		private int velotronChainring = 62;      //initialize the default values for Velotron Chainring
		public int VelotronChainring
		{
			get { return velotronChainring; }
			set
			{
				if (value < 10) velotronChainring = 62;
				else velotronChainring = Math.Max(10, value); //smallest crankset is 0 to indicate unavailable
			}
		}

		private int shiftingInverted = 0;  //default value for shifting inverted
		public int ShiftingInverted
		{
			get { return shiftingInverted; }
			set
			{ //range check to  pin to binary 
				if (value == 0) shiftingInverted = value;
				else shiftingInverted = 1;
			}

		}

		private bool m_Active = false;
		public bool Active
		{
			get { return m_Active; }
			set
			{
				if (m_Active != value)
				{
					m_Active = value;
				}
			}
		}

		private string rememberedDeviceType = "Unknown";
		public string RememberedDeviceType
		{
			get { return rememberedDeviceType; }
			set
			{
				if (string.Compare(value.ToUpper(), "COMPUTRAINER", true) == 0 || string.Compare(value.ToUpper(), "VELOTRON", true) == 0) rememberedDeviceType = value;
				else rememberedDeviceType = "Unknown";
			}
		}

		public RM1.DeviceType DeviceType
		{
			get
			{
				if (rememberedDeviceType.ToUpper() == "COMPUTRAINER")
					return RM1.DeviceType.COMPUTRAINER;
				else if (rememberedDeviceType.ToUpper() == "VELOTRON")
					return RM1.DeviceType.VELOTRON;
				return PreviouslyDiscovered == 0 ? RM1.DeviceType.DOES_NOT_EXIST : RM1.DeviceType.EXISTS;
			}
			set
			{
				if (value == RM1.DeviceType.COMPUTRAINER)
					RememberedDeviceType = "Computrainer";
				else if (value == RM1.DeviceType.VELOTRON)
					RememberedDeviceType = "Velotron";
				else
					RememberedDeviceType = "Unknown";
			}
		}


		private int positionIndex = 0;
		public int PositionIndex
		{
			get { return positionIndex; }
			set
			{ //range check to 1-8, set to 0 if outside range
				if (value >= 1 && value <= RM1Constants.maxtrainersGeneral) positionIndex = value;
				else positionIndex = 0;
				// OnPropertyChanged("PositionIndex");
			}
		}
		private int previouslyDiscovered = 0;
		public int PreviouslyDiscovered
		{
			get { return previouslyDiscovered; }
			set  //range check and pin to a binary
			{
				if (value == 0) previouslyDiscovered = value;
				else previouslyDiscovered = 1;
			}

		}

		private string previousRiderKey = RM1Constants.NoRiderKey;
		public string PreviousRiderKey
		{
			get { return previousRiderKey; }
			set
			{
				if (value.Length != 36) previousRiderKey = RM1Constants.NoRiderKey;
				else previousRiderKey = value;
			}
		}

        private string savedPortName = string.Empty;
        /// <summary>
        /// SavedPortName is now string-based to support serial and wifi devices from RM1_Settings
        /// </summary>
        public string SavedPortName
        {
            get { return savedPortName; }
            set
            {
                savedPortName = value;
            }
        }

        /// <summary>
        /// Trainer Constructor
        /// </summary>
        /// <param name="PositionIndex">Position index 1-8</param>
        public TrainerUserConfigurable(int PositionInd)
		{
			RESETtoDefault(PositionInd);
			// all other properties will be set to defaults
		}
		public void RESETtoDefault(int _PositionIndex)
		{
			this.PositionIndex = _PositionIndex;
			this.RememberedDeviceType = "Unknown";
			this.VelotronChainring = 62;
			this.ShiftingInverted = 0;
			this.PreviouslyDiscovered = 0;
			this.PreviousRiderKey = RM1Constants.NoRiderKey;
            this.savedPortName = string.Empty;


		}
		/// <summary>
		/// Provide a copy of this in a new object
		/// </summary>
		/// <returns></returns>
		public TrainerUserConfigurable CopyMe()
		{
			TrainerUserConfigurable newObject = new TrainerUserConfigurable(0);
			newObject.PositionIndex = this.PositionIndex;
			newObject.RememberedDeviceType = this.RememberedDeviceType;
			newObject.VelotronChainring = this.VelotronChainring;
			newObject.ShiftingInverted = this.ShiftingInverted;
			newObject.PreviouslyDiscovered = this.PreviouslyDiscovered;
			newObject.PreviousRiderKey = this.PreviousRiderKey;
			newObject.savedPortName = this.savedPortName;
			return newObject;
		}

		public RM1.Trainer ActiveTrainer
		{
			get
			{
				RM1.Trainer trainer;
				if ((trainer = RM1.Trainer.Find(SavedPortName)) != null)
				{
					if (trainer.IsConnected)
						return trainer;
				}
				return null;
			}
		}

        public RM1.Trainer CurrentTrainer
		{
			get
			{
				RM1.Trainer trainer;
				if ((trainer = RM1.Trainer.Find(SavedPortName)) != null)
				{
					return trainer;
				}
				return null;
			}
		}


		String m_BotKey = "";
		public String BotKey
		{
			get
			{
				return m_BotKey;
			}
			set
			{
				String v = value == null ? "" : value;
				if (m_BotKey != v)
					m_BotKey = v;
			}
		}
	}

}
