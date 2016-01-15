using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;


namespace RacerMateOne {

	/// <summary>
    /// Trainers connected to the Computer. Defined as static so there can be only one and it's initialized at program start.
    /// </summary>
	public static class TrainerDevices  {
	
        // made public to let users know the enum definitions
		public const int NumOfPossibleCOMPorts = 256;    //Single point to change up to 256 HERE.EVER!!!
			
        /// <summary>
		/// Initializes the Trainer[]array to default UNKNOWN before info is collected 
		/// </summary>
        static  TrainerDevices() //class constructor
        {
            _lightScanFoundTrainerList.Clear();
            _fullScanFoundTrainerList.Clear();
            _verboseListOfAllPortsInfo.Clear();
		}

        private static List<Trainer> _lightScanFoundTrainerList = new List<Trainer>();   
        public static List<Trainer> LightScanFoundTrainerList
        {
            get { return _lightScanFoundTrainerList; }
        }

        private static List<Trainer> _fullScanFoundTrainerList = new List<Trainer>();
        public static List<Trainer> FullScanFoundTrainerList
        {
            get { return _fullScanFoundTrainerList; }
        }
        
        private static List<Trainer> _verboseListOfAllPortsInfo = new List<Trainer>();
        public static List<Trainer> VerboseListOfAllPortsInfo
        {
            get { return _verboseListOfAllPortsInfo; }
        }
    
        private static List<String> GetCommPortsListed
        {
            get
            {
                List<String> CommPortsListed = new List<String>();
                CommPortsListed.Clear();
                string[] Portnames = System.IO.Ports.SerialPort.GetPortNames(); // nice limited list of active coms, not 256
                foreach (String aaa in Portnames)
                    CommPortsListed.Add(aaa);
                return CommPortsListed;
            }
        }
            
	    /// <summary>
        /// Scans all ports (that exist in OS) up to NumOfCOMPorts. Update and return FullScanFoundTrainerList 
        /// COM ports not visible to OS are not scanned, for efficiency.
        /// This fills trainers and assigns them currently position index 0 since it is a full reset scan to find all trainers
        /// Also updates the view of all comm ports visible in this.CommPortsListed
        /// </summary>
        public static List<Trainer> ScanVerbose()  
        {
			// reset the entire array of trainers with the default UNKNOWN
            _verboseListOfAllPortsInfo.Clear();
            List<String> Ports = GetCommPortsListed;

            for (int i = 0; i < NumOfPossibleCOMPorts; i++) 
            {
                Trainer aaa = new Trainer(i+1);
                aaa.SerialPortNum = i + 1;
                _verboseListOfAllPortsInfo.Add(aaa);
            }
            foreach(String bbb in Ports)
            {

				int portCommIndex_m1 = Convert.ToInt32(bbb.Remove(0, 3)) - 1;						// just gives the port number and -1 gives corresponding index into CompleteTrainerList
				//Scan the device in the list of trainers at index portint-1 since
                if (portCommIndex_m1 >= 0 && portCommIndex_m1 <= NumOfPossibleCOMPorts)  
                {			//range check, probably easier way??
                    _verboseListOfAllPortsInfo[portCommIndex_m1].DiscoverRacerMateDevice(portCommIndex_m1);
  				}
			}
            return VerboseListOfAllPortsInfo;
		}

        /// <summary>
        /// Scans all ports (completely) up to NumOfCOMPorts. Update and return FullScanFoundTrainerList 
        /// COM ports not visible to OS are not scanned, for efficiency.
        /// This fills trainers and assigns them currently position index 0 since it is a full reset scan to find all trainers
        /// Also updates the view of all comm ports visible in this.CommPortsListed
        /// </summary>
        public static List<Trainer> ScanForNew()
        {
            // reset the entire array of trainers with the default UNKNOWN
            _fullScanFoundTrainerList.Clear();
           List<String> Ports = GetCommPortsListed;

           if (AppWin.SimTestNumTrainers > 0 && AppWin.SimTrainersConnected == true)
           {
               Ports.Clear();
               for (int i = 1; i <= AppWin.SimTestNumTrainers; i++)
                   Ports.Add("COM" + i.ToString());
           }
           for (int i = 0; i < Ports.Count; i++)
            {
                int portCommIndex_m1 = Convert.ToInt32(Ports[i].Remove(0, 3)) - 1;						// just gives the port number and -1 gives corresponding index into CompleteTrainerList
                //Scan the device in the list of trainers at index portint-1 since
                Trainer aaa = new Trainer(portCommIndex_m1+1);
                    aaa.DiscoverRacerMateDevice(portCommIndex_m1);
           //         Debug.WriteLine("found device on com " + Ports[i] + " type is " + aaa.ScannedDeviceType.ToString());
                       
                    if (aaa.ScannedDeviceType == Trainer.EnumDeviceType.DEVICE_COMPUTRAINER || aaa.ScannedDeviceType == Trainer.EnumDeviceType.DEVICE_VELOTRON)
                    {
                       // Debug.WriteLine("found device on com " + Ports[i]);
                        aaa.ModifyRememberedDeviceTypebasedOnScan();
                        _fullScanFoundTrainerList.Add(aaa); //don't need a copy, they are not from SavedTrainersList
                    }
            }
            return _fullScanFoundTrainerList;
        }

       

		//	Some devices have been configured; for those, rescan and update status. 
		//	This prevents triggering off behaviors in other serial devices.
        //	The devices that are known are defined in the RM1_Settings.Trainer List with PreviouslyDiscovered=1. 
        //	If a previously-thought-to-be-known device is found to be DOES_NOT_EXIST, the PreviouslyDiscovered=1 will be cleared
		//	Note carefully this does NOT scan for any newly-added devices as in ScanForNew(), it just verifies old knowledge and deletes
		//	devices that may have become missing.
        // /// Rescan devices known to exist in past. This is quickupdate scan only, according to 
        //// those Trainers in SavedTrainersList with PreviouslyDiscovered=1. 
        //// If a Trainer of either CompuTrainer or Velotron is found on the given com port, update its properties (even if the type has changed)
     
       /// <summary>
        /// Rescan devices known to exist in past. This is quickupdate scan only, according to 
        /// those Trainers in SavedTrainersList with PreviouslyDiscovered=1. 
       /// If a Trainer of either CompuTrainer or Velotron is found on the given com port, update its properties (even if the type has changed)
       /// </summary>
       /// <returns>True if all previouslydiscovered trainers are found; LightScanFoundTrainerList updated</returns>
       public static bool RescanOnlyConfigured()  
        {
            _lightScanFoundTrainerList.Clear();
            bool retbool = true;
            
           foreach (TrainerUserConfigurable aaa in RM1_Settings.SavedTrainersList)
            {
              //  Debug.WriteLine("Scanning..");
               // showtrainer(aaa);
             
               if (aaa.PreviouslyDiscovered == 1)
                {
                    if (CheckCOMStillAvailable(aaa.SavedSerialPortNum) == true)  //if still a valid COM port then do the actual query for device
                    {
                        Trainer scannedTrainer = new Trainer(aaa.SavedSerialPortNum);  
                        scannedTrainer.DiscoverRacerMateDevice(aaa.SavedSerialPortNum - 1);
                        if (scannedTrainer.ScannedDeviceType == Trainer.EnumDeviceType.DEVICE_COMPUTRAINER || scannedTrainer.ScannedDeviceType == Trainer.EnumDeviceType.DEVICE_VELOTRON)
                        {
                            scannedTrainer.TrainerUserConfigs = aaa.CopyMe(); //merge in the user configurable information into the Trainer, but use a copy
                            scannedTrainer.ModifyRememberedDeviceTypebasedOnScan(); //in case the device type changed but still a trainer existing
                            _lightScanFoundTrainerList.Add(scannedTrainer);
                        }
                        else
                        {
                            retbool = false;
                        }

                    }
                    else
                    {
                        retbool = false;
                        aaa.RESETtoDefault(aaa.PositionIndex);           // the com port is now gone, I get to reset this trainer to not found in the saved trainers list
                    }
                }
            }
            return retbool;
             
		}
       private static void showtrainer(TrainerUserConfigurable aaa)
       {
           
               Debug.WriteLine("Trainer " + aaa.PositionIndex + " is on port COM"  + " Saved Serial Port num = " + aaa.SavedSerialPortNum);

       }
   
          
	    /// <summary>
        /// Compares the input parameter as a COM port number (1...NumOfPossibleCOMPorts)
		///	to the returned list of active com ports in OS.
		/// </summary>
        /// <param name="COMnumber">Com port number from 1...</param>
        /// <returns>True when port available</returns>
		private static bool CheckCOMStillAvailable(int COMnumber)  
        {
		//range check
            if (AppWin.SimTestNumTrainers > 0 && AppWin.SimTrainersConnected == true)
            {
                return true;
            }
            else
            {
                if (COMnumber <= 0 || COMnumber > NumOfPossibleCOMPorts)
                    return false;

                // get active COM ports from system

                string[] Portnames = System.IO.Ports.SerialPort.GetPortNames(); // nice limited list of active coms, not 256

                //look for the COMnumber in the list

                for (int i = 0; i < Portnames.Length; i++)
                {
                    int portindex = Convert.ToInt32(Portnames[i].Remove(0, 3)); //compare integers
                    if (portindex == COMnumber)
                    {
                        return true;
                    }
                }

                //didn't find a match so...          
                return false;
            }
		}

		

	}							// public static class TrainerDevices
}								// namespace RacerMate
