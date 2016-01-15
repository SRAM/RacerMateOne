using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;


namespace RacerMateOne
{

	/// <summary>
    /// Represents a single trainer and exposes Readonly Properties associated with it.
	///	Public methods are used to call operations upon the Trainer via interop.
    /// </summary>
	public class Trainer  {

		// made public to let users know the enum definitions

		/// <summary>
		/// Enumeration meaning for DeviceType:
        ///   DEVICE_NOT_SCANNED,	    unknown, not scanned
        ///    DEVICE_DOES_NOT_EXIST,	serial port does not exist
        ///    DEVICE_EXISTS,			exists, openable, but no RM device on it
        ///    DEVICE_COMPUTRAINER,
        ///    DEVICE_VELOTRON,
        ///    DEVICE_ACCESS_DENIED,	port present but can't open it because something else has it open
        ///    DEVICE_OPEN_ERROR,		port present, unable to open port
        ///    DEVICE_OTHER_ERROR		prt present, error, none of the above
     	/// </summary>
        public enum EnumDeviceType {
            DEVICE_NOT_SCANNED,					// unknown, not scanned
            DEVICE_DOES_NOT_EXIST,				// serial port does not exist
            DEVICE_EXISTS,						// exists, openable, but no RM device on it
            DEVICE_COMPUTRAINER,
            DEVICE_VELOTRON,
            DEVICE_ACCESS_DENIED,				// port present but can't open it because something else has it open
            DEVICE_OPEN_ERROR,					// port present, unable to open port
            DEVICE_OTHER_ERROR					// prt present, error, none of the above
        };
        /// <summary>
        /// this enum is distinct from EnumDeviceType since it encodes only the nature of the device itself
        /// and not its status wrt to serial port function
        /// </summary>

       
        private int serialPortNum = 0;
        /// <summary>
        /// SerialPortNum is now aligned with actual port numbering as 1...256
        /// And is Read/Write due to loading  from RM1_Settings
        /// </summary>
        public int SerialPortNum   
        {
			get {return serialPortNum;}
            set {if (value>=0 && value<=256)serialPortNum=value;}               
		}

        private EnumDeviceType scannedDeviceType = EnumDeviceType.DEVICE_NOT_SCANNED;
        /// <summary>
        /// DeviceType describes the attached device or Serial Port status when there is no CompuTrainer or VeloTron
        /// Readonly property that can only be changed via the DiscoverDeviceType method
        /// </summary>
		public EnumDeviceType ScannedDeviceType  
        {
			get {return scannedDeviceType;}
		}

        private string firmwareVersion = "";
        /// <summary>
        /// FirmwareVersion can only be written by the DiscoverDeviceType Method
        /// </summary>
		public string FirmwareVersion  
        {
			get {return firmwareVersion;}
		}

        private int isCalibrated = 0;
        /// <summary>
        /// Iscalibrated can only be written by the DiscoverDeviceType Method
        /// </summary>
        public int IsCalibrated  //name as "isCalibrated" so that meaning of state is easily understood as boolean and how it applies  
        {			
			get {return isCalibrated;}
		}

        private int calibrationValue = 0; // RRC for CompuTrainer, AccuWatt for Velotron
        /// <summary>
        /// CalibrationValue can only be written by the DiscoverDeviceType Method
        /// </summary>
        public int CalibrationValue  
        {
			get {return calibrationValue;}
		}
        
    //    private TrainerUserConfigurable _trainerUserConfigs;
        public TrainerUserConfigurable TrainerUserConfigs
        { get; set; }

                 
        /// <summary>
        /// Trainer constructor with a portnumber
        /// </summary>
        /// <param name="SPortNum">Com port number starting 1..256, but 0 permitted=disabled</param>
        /// <param name="RealorFake">Use as flag to create a real trainer to be addressed or a dummy to be used as combobox title</param>
        public Trainer(int SPortNum)
        {
              serialPortNum = SPortNum;
            scannedDeviceType = EnumDeviceType.DEVICE_DOES_NOT_EXIST;
            // if the serial port does exist on the system, mark the trainer property at elast that the port
            // exists
            string[] Portnames = System.IO.Ports.SerialPort.GetPortNames(); // nice limited list of active coms, not 256
            //look for the COMnumber in the list
            for (int i = 0; i < Portnames.Length; i++)
            {
                int portindex = Convert.ToInt32(Portnames[i].Remove(0, 3)); //compare integers
                if (portindex == SPortNum)
                    scannedDeviceType = EnumDeviceType.DEVICE_EXISTS;
            }

            firmwareVersion = "";
            isCalibrated = 0;
            calibrationValue = 0;
            TrainerUserConfigs = new TrainerUserConfigurable(0);
       }
        /// <summary>
        /// Trainer constructor with no portnumber, sets it to 0
        /// </summary>
        /// <param name="SPortNum">Com port number starting 1..256, but 0 permitted=disabled</param>
        /// <param name="RealorFake">Use as flag to create a real trainer to be addressed or a dummy to be used as combobox title</param>
        public Trainer()
        {
            serialPortNum = 0;
            scannedDeviceType = EnumDeviceType.DEVICE_DOES_NOT_EXIST;
           
            firmwareVersion = "";
            isCalibrated = 0;
            calibrationValue = 0;
            TrainerUserConfigs = new TrainerUserConfigurable(0);
           
        }

		//	does scan on port number and updates only the private variables directly
		//	C# code sees these as ReadOnly Properties of the class. So far, 
		//	this DiscoverRacerMateDevice is the only place where
		//	deviceType, firmwareVersion, isCalibrated and calibrationValue can ever be written.
		//
        /// <summary>
        /// Calls out to Comm port discovery via interop to determine devicetype or status
        /// For Racermate devices, gathers firmware version and calibration info
        /// </summary>
        /// <param name="portnumber"></param>
		public void DiscoverRacerMateDevice(int portnumber) 
        {
            int dev = RacerMateOne.RM1ExtCOMM.GetRacerMateDeviceID(portnumber);					// this is actually comport number - 1
            //Debug.WriteLine("dev= " + dev + " port " + portnumber);
			switch (dev) 
            {
                case (int)EnumDeviceType.DEVICE_COMPUTRAINER: //2
					scannedDeviceType = EnumDeviceType.DEVICE_COMPUTRAINER;
                 //   RememberedDeviceType = "Computrainer";
                    firmwareVersion = RacerMateOne.RM1ExtCOMM.GetRacerMateFirmWareVersion(portnumber);
                    isCalibrated = RacerMateOne.RM1ExtCOMM.GetRacerMateIsCalibrated(portnumber);
                    if (isCalibrated == 1) calibrationValue = RacerMateOne.RM1ExtCOMM.GetRacerMateCTCalibration(portnumber);
                   // Debug.WriteLine("dev= " + dev + " calibation= " + calibrationValue);
                    break;

				case (int)EnumDeviceType.DEVICE_VELOTRON:  //3:
                    scannedDeviceType = EnumDeviceType.DEVICE_VELOTRON;
                //    RememberedDeviceType = "Velotron";
                    firmwareVersion = RacerMateOne.RM1ExtCOMM.GetRacerMateFirmWareVersion(portnumber);
                    isCalibrated = RacerMateOne.RM1ExtCOMM.GetRacerMateIsCalibrated(portnumber);
                    if (isCalibrated == 1) calibrationValue = RacerMateOne.RM1ExtCOMM.GetRacerMateVTAccuWatt(portnumber);
					break;
                //remainder of cases on returned GetRacerMateDeviceID()
                // require no follow up specific action, so set the device type simply to the returned ID 
				default:
                    scannedDeviceType = (EnumDeviceType)dev;
              //      RememberedDeviceType = "Unknown"; 
                    break;
			}
            return;
		}

        public void RESETtoDefault(int PositionIndex)
        {
            this.calibrationValue = 0;
            this.firmwareVersion = "";
            this.isCalibrated=0;
            this.scannedDeviceType=EnumDeviceType.DEVICE_DOES_NOT_EXIST;
            this.serialPortNum=0;
            this.TrainerUserConfigs.RESETtoDefault(this.TrainerUserConfigs.PositionIndex);
         }
        public void ModifyRememberedDeviceTypebasedOnScan()
        {
            if (this.ScannedDeviceType == Trainer.EnumDeviceType.DEVICE_COMPUTRAINER) this.TrainerUserConfigs.RememberedDeviceType = "Computrainer";
            else if (this.ScannedDeviceType == Trainer.EnumDeviceType.DEVICE_VELOTRON) this.TrainerUserConfigs.RememberedDeviceType = "Velotron";
            else this.TrainerUserConfigs.RememberedDeviceType = "Unknown";
        }
        
        public override String ToString()
        {

               if (this.ScannedDeviceType == Trainer.EnumDeviceType.DEVICE_VELOTRON)
                    return "COM" + this.SerialPortNum.ToString() + ": / v" + this.FirmwareVersion + " / Velotron / Accuwatt= " + this.CalibrationValue;
                else if (this.ScannedDeviceType == Trainer.EnumDeviceType.DEVICE_COMPUTRAINER)
                    return "COM" + this.SerialPortNum.ToString() + ": / v" + this.FirmwareVersion + " / CompuTrainer / RRC= " + this.CalibrationValue / 100.0;
                else return "Unknown";
                        
        }

        public Trainer CopyMe()
        {
            Trainer newObject = (Trainer)this.MemberwiseClone();
            newObject.TrainerUserConfigs = this.TrainerUserConfigs.CopyMe();  //replace the value type reference to a true copy

            return newObject;
        }

    
    }					// public class Trainer
    

}						// namespace RacerMate
