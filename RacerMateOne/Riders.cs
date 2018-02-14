using System.Collections.Generic;       // Needed for LISTs
using System.Xml;
using System.IO;
using System.Data;
using System;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;

using System.ComponentModel;
using System.Windows.Media;





namespace RacerMateOne {
	/// <summary>
	/// RM1RiderList inherits from Generic List, but when the Add method is called, this 
	/// method executes instead and ensure that the DataBaseKey is aways unique.
	/// The DataBaseKey may be changed to force uniqueness. Operating at this granularity ensures
	/// that a RiderDB is always stored with unique keys, and thus will always be read-back with unique keys.
	/// Only when a new rider is added manually by higher-level code is there a danger,or if the user has hand
	/// modified a Riders_db.xml file
	/// </summary>
	/// <typeparam name="Rider">the Rider object to add to the list</typeparam>

	public static class Riders {
		static Rider m_DefaultRider;

		public static Rider DefaultRider {
			get {
				if(m_DefaultRider == null)
					m_DefaultRider = new Rider();
				return m_DefaultRider;
			}
		}

		//private since I'm not sure why other classes need to see this at compile time.
		// only this class should do operations with the Database where the Versions would be relevant

		private const string defaultMajorVersion = "1";
		private const string defaultMinorVersion = "0";  //private since I'm not sure why other classes need to see this at compile time??

		// private since other code must not access the database file directly
		// and should only do so using these ReadFromFile and Write file functions

		private const string RiderInfoFilename = @"\RM1_RiderDB.xml";
		private const string RiderXSLFilename = @"\RM1_RiderDB.xsl"; // private
																						 // static contstructor

		// public static List Riderslist = new List<Riders>();

		public static ObservableCollection<Rider> RidersList = new ObservableCollection<Rider>();
		private static string _majorversion = defaultMajorVersion;  //initialize to default then takes from file
		private static string _minorversion = defaultMinorVersion;  //initialize to default then takes from file

		//make the version readonly, not sure why other classes would be built outside this class to change the Version, so till then, readonly.

		public static string MajorVersion { get { return _majorversion; } }
		public static string MinorVersion { get { return _minorversion; } }

		private static string applicationdir = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).ToString()).LocalPath;

		//private bool rewrite = false;

		/**************************************************************************************************************
			constructor
		**************************************************************************************************************/

		static Riders() {
			//load the special lists
			RidersList.Clear();
		}


		/**************************************************************************************************************
		
		**************************************************************************************************************/

		/// <summary>
		/// Read in a list of Riders from the AppData stored riders file, default file 
		/// RM1_RidersDB.xml. If a load fails, prompts to load from backups
		/// and will reset to default Riders_db.xml content only when backup reload refused by user.
		/// exits when user is ornary about it.
		/// </summary>
		/// <returns>false when failed to load a RidersDB</returns>
		public static bool LoadFromFile() {
#if DEBUG
			//tlm
			Log.WriteLine("   Riders::LoadFromFile()");
#endif

			bool fromsaved = LoadFromFile(RacerMatePaths.SettingsFullPath + RiderInfoFilename);
			return fromsaved; //caller will terminate app if sees false
		}

		/**************************************************************************************************************
		
		**************************************************************************************************************/

		public static void DeleteRidersFile() {
			string realfilename = RacerMatePaths.SettingsFullPath + RiderInfoFilename;
			File.Delete(realfilename);
		}



		/**************************************************************************************************************
			/// <summary>
			/// Read in a list of Riders from a riders file, full path required.  If a load fails, prompts to load from backups
			/// and will reset to default Ridersdb.xml content only when backup reload refused by user.
			/// </summary>
			/// <param name="infilename"> Filename to read, within LocalAppdata folder is assumed</param>
			/// <returns>False when default insisted, true when loaded OKAY </returns>
		**************************************************************************************************************/

		private static bool LoadFromFile(string infilename) {
			bool retval = false;
			bool IsResolved = false;

#if DEBUG
			//tlm
			Log.WriteLine("      Riders::LoadFromFile(" + infilename + ")");
#endif

			string realfilename = infilename;
			XDocument FullDoc = null;
			bool IsFullDocgoodXML = true;
			bool IsRDBValid = false;
			bool? FileOpenDialogResult = false;
			// ensure that the Settings directory exists by makinga useless
			// access to a static member in RacerMate Paths.
			string xxx = RacerMatePaths.SettingsFullPath;

			if(!File.Exists(infilename)) {
				// this is the first run, there is no RidersDb file at all. Initialize as first time run.
				ResetAsDefaults();
				SaveToFile();
			}

			// Open the rider file from within AppData.

			do {
				//reset the validation settings
				IsFullDocgoodXML = true;
				IsRDBValid = false;
				try {
					FullDoc = XDocument.Load(realfilename);
				}
				catch {
					IsFullDocgoodXML = false;  //when there is a XML style error, caught as exception
				}
				finally {
				}

				if(IsFullDocgoodXML == true) {
					IsRDBValid = IsValidRidersDB(FullDoc);  //test for validity of data and loads the useable classes and properties             
				}

				if(IsRDBValid == false || IsFullDocgoodXML == false) {
					// here we have invalid input file for either bad xml or invalid data in good xml.
					// either way, prompt user to read from a backup file
					Window_CustomLoadBackupFile diagWin = new Window_CustomLoadBackupFile();
					diagWin.Title = "Riders Database file Error";
					diagWin.CaptionText = "The Riders Database file is corrupt. Please choose from the options below:";
					diagWin.ShowDialog(); //shows as modal
					int returnedvalue = diagWin.UserChoice;

					switch(returnedvalue) {
						case 0: //Exit the application Load from backup
							{
								retval = false;
								IsResolved = true; //this pair will terminate the app.
								break;
							}
						case 1: //Load from Backup
							{
								OpenFileDialog openFile1 = new OpenFileDialog();
								openFile1.InitialDirectory = RacerMatePaths.BackupFullPath;
								openFile1.Multiselect = false;
								openFile1.Title = "Select a backup Riders database file";
								openFile1.ValidateNames = true;
								openFile1.AddExtension = true;
								openFile1.CheckFileExists = true;
								openFile1.DefaultExt = ".xml";
								openFile1.Filter = "Riders DB Files (.xml)|RM1_RiderDB*.xml";
								FileOpenDialogResult = openFile1.ShowDialog();
								if(FileOpenDialogResult == true) {
									realfilename = openFile1.FileName;  //new file to test
									retval = false;
									IsResolved = false;  //this pair will loop and try again
								}
								else  //if user does not select a file or hits cancel, there is no second chance on this.
								{
									retval = false;
									IsResolved = true; //this pair will terminate the app
								}
								break;
							}
						case 2: //Load factory default
							{
								ResetAsDefaults();
								SaveToFile();
								retval = false;
								IsResolved = false; //need to read the new file
								break;
							}
					}
				}                          // if (IsRDBValid == false || IsFullDocgoodXML == false)  {
				else {
					retval = true;
					IsResolved = true;
				}
			} while(IsResolved == false);

			GC.Collect();                    //this may be wise in case a bunch of fileopendialogs have been instantiated and are hanging about.

			// ok here 4		RidersList[11] is valid
			return retval;
		}                          // LoadFromFile(path)

		/**************************************************************************************************************

		**************************************************************************************************************/

		static bool TryGetColor(XAttribute att, out Color val) {
			val = Colors.Transparent;
			if(att == null)
				return false;
			Color v;
			bool ans = false;

			try {
				v = (Color)ColorConverter.ConvertFromString(att.Value);
				val = v;
				ans = true;
			}
			catch { }
			return ans;
		}


		/**************************************************************************************************************
			/// <summary>
			/// This will parse the Riders_db.xml for structure and will load each Rider object.
			/// Rider objects do range and validity check and reset to default values when unacceptable.
			/// This will fail should the XML document be valid in formed-ness but invalid per
			/// the expected Riders_db specification.
			/// </summary>
			/// <param name="indoc">An XDocument object representing the Riders_db</param>
			/// <returns>True on no unmanagable-errors, false when not recoverable from error </returns>
		**************************************************************************************************************/

		private static bool IsValidRidersDB(XDocument indoc) {
			bool retval = false;
			//bool fix = false;

			try {
#if DEBUG
				//tlm
				Log.WriteLine("         Riders::IsValidRidersDB()");
				bool result = false;
				//int bp = 0;
#endif

				XElement rootNode = indoc.Root;
				_majorversion = rootNode.Attribute("VersionMajor").Value;
				_minorversion = rootNode.Attribute("VersionMinor").Value;

				IEnumerable<XElement> ridernodes = rootNode.Elements("Rider");
				RidersList.Clear();

                //---just read in the riders list


				foreach (XElement ele in ridernodes) {
					// the above will all be present for a computrainer, Velotron has further properties

					XElement GearingCrankNode = ele.Element("GearingCrankset");
					IEnumerable<XElement> bigringnodes = GearingCrankNode.Elements();

					int[] Crankset = new int[RM1.MAX_FRONT_GEARS];
					for (int crankIndex = 0; crankIndex < RM1.MAX_FRONT_GEARS; ++crankIndex)
					{
						Crankset[crankIndex] = 0;
					}

					int[] Cogset = new int[RM1.MAX_REAR_GEARS];
					for (int cogIndex = 0; cogIndex < RM1.MAX_REAR_GEARS; ++cogIndex)
					{
						Cogset[cogIndex] = 0;
					}

					int gearcount = 0;

					foreach(XElement ringspec in bigringnodes) {
						Crankset[gearcount] = Convert.ToInt32(GearingCrankNode.Element("G" + (gearcount + 1).ToString()).Value);
						gearcount += 1;
					}

					XElement GearingCogNode = ele.Element("GearingCogset");
					IEnumerable<XElement> cognodes = GearingCogNode.Elements();
					gearcount = 0;
					foreach(XElement cogspec in cognodes) {
						Cogset[gearcount] = Convert.ToInt32(GearingCogNode.Element("G" + (gearcount + 1).ToString()).Value);
						gearcount += 1;
					}

                    // Newer rider databases will have AnT instead of AeT, but we need to support loading from older database.
                    XElement xHrAnT = ele.Element("HRAnT");
                    XElement xHrAeT = ele.Element("HRAeT");
                    int hrAnT = (xHrAnT != null) ? Convert.ToInt32(xHrAnT.Value) : (xHrAeT != null) ? Convert.ToInt32(xHrAeT.Value) : 0;

                    // Likewise for PowerAnT and PowerAeT
                    XElement xPowerAnT = ele.Element("PowerAnT");
                    XElement xPowerAeT = ele.Element("PowerAeT");
                    int powerAnT = (xPowerAnT != null) ? Convert.ToInt32(xPowerAnT.Value) : (xPowerAeT != null) ? Convert.ToInt32(xPowerAeT.Value) : 0;

					float WheelDiameter = Convert.ToSingle(ele.Element("WheelDiameter").Value);
					Rider thisrider = new Rider(
						ele.Attribute("DatabaseKey").Value,
						ele.Element("LastName").Value,
						ele.Element("FirstName").Value,
						ele.Element("NickName").Value,
						ele.Element("Gender").Value,
						ele.Element("Age").Value,
						hrAnT,
						Convert.ToInt32(ele.Element("HRMax").Value),
						0,
						0,
						powerAnT,
						Convert.ToInt32(ele.Element("PowerFTP").Value),
						Convert.ToDouble(ele.Element("WeightRider").Value),
						Convert.ToDouble(ele.Element("WeightBike").Value),
						Convert.ToInt32(ele.Element("DragFactor").Value),
						Crankset,
						Cogset,
						WheelDiameter);

                    // Support ANT+ Sensors
                    XElement xHrSensorId = ele.Element("HrSensorId");
                    XElement xCadenceSensorId = ele.Element("CadenceSensorId");

                    if (xHrSensorId != null)
                    {
                        thisrider.HrSensorId = Convert.ToInt32(xHrSensorId.Value);
                    }

                    if (xCadenceSensorId != null)
                    {
                        thisrider.CadenceSensorId = Convert.ToInt32(xCadenceSensorId.Value);
                    }

#if DEBUG
                    //int bbpp = 0;
                    //tlm

                    result = thisrider.NickName.Equals("barak", StringComparison.Ordinal);
					if(result) {
						int i, n = Crankset.Length;
						// okhere 2
						// thisrider.GearingCrankset, and Crankset

						for(i = 0; i < 3; i++) {
							Log.WriteLine("            read Crankset[" + i.ToString() + "] = " + Crankset[i].ToString());
						}

						Log.WriteLine("");

						n = Cogset.Length;

						for(i = 0; i < 10; i++) {
							Log.WriteLine("            read Cogset[" + i.ToString() + "] = " + Cogset[i].ToString());
						}
						//bbpp = 2;
					}
					else {
						//int bp = 2;
					}
					//bbpp = 3;
#endif
					// Optional newer stuff
					XElement e;
					if((e = ele.Element("HRMin")) != null)
						thisrider.HrMin = Convert.ToInt32(e.Value);

					if((e = ele.Element("Metric")) != null) {
						try { thisrider.Metric = Convert.ToBoolean(e.Value); }
						catch(Exception ex) {
							Debug.WriteLine(ex);
						}
					}
					if((e = ele.Element("Height")) != null) {
						try { thisrider.Height = (float)Convert.ToDouble(e.Value); }
						catch { }
					}
					if((e = ele.Element("Colors")) != null) {
						Color c;
						if(TryGetColor(e.Attribute("Skin"), out c))
							thisrider.Skin = c;
						if(TryGetColor(e.Attribute("Hair"), out c))
							thisrider.Hair = c;
						if(TryGetColor(e.Attribute("Helmet"), out c))
							thisrider.Helmet = c;
						if(TryGetColor(e.Attribute("Shoes"), out c))
							thisrider.Shoes = c;
						if(TryGetColor(e.Attribute("Clothing1"), out c))
							thisrider.Clothing1 = c;
						if(TryGetColor(e.Attribute("Clothing2"), out c))
							thisrider.Clothing2 = c;
						if(TryGetColor(e.Attribute("BikeColor1"), out c))
							thisrider.BikeColor1 = c;
						if(TryGetColor(e.Attribute("BikeColor2"), out c))
							thisrider.BikeColor2 = c;
					}


					if((e = ele.Element("RiderType")) != null)
						thisrider.RiderType = e.Value;

					try {
						if((e = ele.Element("AlarmMinZone")) != null)
							thisrider.AlarmMinZone = String.Compare("off", e.Value.ToString(), true) == 0 ? 0 : Convert.ToInt32(e.Value);
					}
					catch { thisrider.AlarmMinZone = 0; }
					try {
						if((e = ele.Element("AlarmMaxZone")) != null)
							thisrider.AlarmMaxZone = String.Compare("off", e.Value.ToString(), true) == 0 ? 6 : Convert.ToInt32(e.Value);
					}
					catch { thisrider.AlarmMaxZone = 6; }

					if((e = ele.Element("Created")) != null) {
						try {
							DateTime dt = Convert.ToDateTime(e.Value);
							thisrider.Created = dt;
						}
						catch { }
					}
					if((e = ele.Element("Modified")) != null) {
						try {
							DateTime dt = Convert.ToDateTime(e.Value);
							thisrider.Modified = dt;
						}
						catch { }
					}

                    if ((e = ele.Element("HrSensorId")) != null)
                    {
                        try { thisrider.HrSensorId = (int)Convert.ToInt32(e.Value); }
                        catch { }
                    }
                    if ((e = ele.Element("CadenceSensorId")) != null)
                    {
                        try { thisrider.CadenceSensorId = (int)Convert.ToInt32(e.Value); }
                        catch { }
                    }

#if DEBUG
                    if (result) {
						//bbpp = 8;
					}
#endif


					thisrider.UpdateModified();   // Snapshot things so we can check the modifications when saving out.

					// I wish I could override the .Add method but cannot seem to do so on an ObservableList
#if DEBUG
					if(result) {
						//bbpp = 8;								// ok here 3
					}
#endif
					AddNewRider(thisrider);                                        // adds thisrider to RidersList
				}                          // foreach (XElement ele in ridernodes)  {

				retval = true;

				/*
				if (fix) {
					retval = false;
				}
				else {
					retval = true;
				}
				*/

			}                             // try
			catch {                    //default return value is false but set it anyway for readability
				retval = false;
			}

			return retval;
		}                                // IsValidRidersDB()

		/**************************************************************************************************************
		
		**************************************************************************************************************/

		/// <summary>
		/// Looks at current list of riders and makes sure the Rider passed in will have a unique key before being added
		/// </summary>
		/// <param name="inrider"></param>
		private static string MakeDataBaseKeyUnique(string instring) {
			string outstring = instring;
			bool IsUnique = CheckUniqueString(outstring);
			while(IsUnique ==false) {
				outstring = Guid.NewGuid().ToString();
				IsUnique=CheckUniqueString(outstring);
			}
			return outstring;
		}

		/**************************************************************************************************************
		
		**************************************************************************************************************/

		/// <summary>
		/// Compare this string against all Databasekeys in riderslist
		/// </summary>
		/// <param name="instring"></param>
		/// <returns></returns>
		private static bool CheckUniqueString(string instring) {
			bool retval = true;
			foreach(Rider aaa in RidersList) {
				if(aaa.DatabaseKey.ToUpper() == instring.ToUpper()) {
					return false;
				}
			}
			return retval;
		}


		/**************************************************************************************************************
		
		**************************************************************************************************************/

		/// <summary>
		/// Given a unique string, will find the rider in the RidersList and rturn the rider
		/// </summary>
		/// <param name="instring"></param>
		/// <returns></returns>
		public static Rider FindRiderByKey(string instring) {
			Rider retval = null;
			foreach(Rider aaa in RidersList) {
				if(instring.ToUpper() == aaa.DatabaseKey.ToUpper()) {
					retval = aaa;
					break;
				}
			}
			return retval;

		}

#if DEBUG
		/**************************************************************************************************************
			tlm
		**************************************************************************************************************/

		public static Rider FindRiderByNick(string instring) {
			Rider retval = null;

			foreach(Rider aaa in RidersList) {
				if(instring.ToUpper() == aaa.NickName.ToUpper()) {
					retval = aaa;
					break;
				}
			}
			return retval;

		}
#endif

		/**************************************************************************************************************
		
		**************************************************************************************************************/

		/// <summary>
		/// Save the loaded list of Riders to the AppData stored riders file.
		/// Before writing, a backup of the existing LocalAppData Riders_db.xml file is created
		/// with date added to filename as Riders_db_yyyy_mm_dd.xml, so that sorting on name results
		/// in newest to oldest or vice versa.
		/// Only 1 backup per day of use is made, and only most recent 10 days activity are recorded.
		/// </summary>

		public static bool SaveToFile() {
			// Take existing riders and flush to the file. If any errors during write, let user know
			bool retval = false;

			string realfilename = RacerMatePaths.SettingsFullPath + RiderInfoFilename;          //before saving check the backup situation
			BackupTodays(RiderInfoFilename);                                                    //if fails, ignore.

#if DEBUG
			//tlm
			Log.WriteLine("   Riders::SaveToFile()");
#endif

			XDocument doc = CreateXDocFromRiders();
			if(doc != null) {
				try {
					doc.Save(realfilename);
					retval = true;
				}
				catch {
					MessageBoxResult result = MessageBox.Show("Could not write the RidersDB file." + Environment.NewLine + "No changes to Riders DB.",
						"File Write error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else {
				MessageBoxResult result = MessageBox.Show("Could not create valid XML from the Riders List." + Environment.NewLine + "No changes to Riders DB.",
					"XML error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			return retval;
		}

		/**************************************************************************************************************
		
		**************************************************************************************************************/

		/// <summary>
		/// Will add a new rider to the RidersDB by adding it to the RidersList and computing a
		/// unique DatabaseKey
		/// </summary>
		/// <returns>False when failed to add rider</returns>
		public static void AddNewRider(Rider inrider) {
			string UniqueKey = MakeDataBaseKeyUnique(inrider.DatabaseKey);
			inrider.DatabaseKey = UniqueKey;
			RidersList.Add(inrider);
			//now add the new rider to each of these special lists
		}

		/**************************************************************************************************************
		
		**************************************************************************************************************/

		public static void RemoveRider(Rider inrider) {
			int indexToRider = RidersList.IndexOf(inrider);
			if(indexToRider > -1) {
				RidersList.Remove(inrider);
			}

		}


		/**************************************************************************************************************
		
		**************************************************************************************************************/

		/// <summary>
		/// In the case of a corrupt Riders file, clean up the system using this function. This could make use of backups to restore to a recent known good state. 
		/// NOTE: This is kept as a private method as the only callers should be the above methods.
		/// </summary>

		private static void ResetAsDefaults() {
			// This function does not care if the Riders can't be saved. It's goal is 
			// to prevent meltdown due to a corrupt Riders file.

			// Clear UI list
			RidersList.Clear();
			_majorversion = defaultMajorVersion;
			_minorversion = defaultMinorVersion;
			// Poplulate the list with one sample rider, the default rider
			RidersList.Clear();

			Rider nrider = new Rider();
			AddNewRider(nrider);
			//there is no need to check for unique DataBaseKey element.    
		}

		/**************************************************************************************************************
		/// <summary>
		/// Just work here to put the Riders into XML XDocument object
		/// </summary>
		/// 
		**************************************************************************************************************/

		public static XDocument CreateXDocFromRiders() {
			XDocument retdoc;
#if DEBUG
			//tlm
			Log.WriteLine("      Riders::CreateXDocFromRiders()");
			dump("barak");                               // dump my test rider to the console
#endif

			try {
				XElement rootRiders = new XElement("Riders", new XAttribute("VersionMajor", Riders.MajorVersion), new XAttribute("VersionMinor", Riders.MinorVersion));
				foreach(Rider bbb in Riders.RidersList) {
					XElement arider = bbb.UpdateModified();
					rootRiders.Add(arider);
				}
				string relativepath = @".." + (RacerMatePaths.ReportTemplatesRelativePath);

				retdoc = new XDocument(
				  new XDeclaration("1.0", "utf-8", "yes"),
				  new XProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + relativepath + RiderXSLFilename+ "'"),
				  new XComment("RaceMate One Rider Database"),
				  rootRiders);
			}
			catch {
				retdoc = null;
			}

			return retdoc;
		}
		/// <summary>
		/// Makes a backup of Riders_db.xml with dated suffix for easy sort.
		/// Only happens once per day of use. Maintains only most recent 10 backups.
		/// </summary>
		/// <param name="filename">filename to backup</param>

		/**************************************************************************************************************
		
		**************************************************************************************************************/

		private static void BackupTodays(string infilename) {
			DateTime today = System.DateTime.Today;
			string suffix = today.Year + "_" + today.Month + "_" + today.Day;
			string realfilename = RacerMatePaths.SettingsFullPath  + infilename;
			string FileBackupname = RacerMatePaths.BackupFullPath + @"\"+ Path.GetFileNameWithoutExtension(infilename) + suffix + Path.GetExtension(infilename);
			//Log.WriteLine("backing up " + FileBackupname);
			try {
				if(!File.Exists(FileBackupname) && File.Exists(realfilename)) {
					File.Copy(realfilename, FileBackupname, false); //', False, FileIO.UICancelOption.DoNothing)
				}
				else {
					// the file exists, we already have a backup for today, and we can make a fast exit
					return;
				}
			}
			catch { //do nothing, not important, in fact this happens on initial load, as load defaults tries to write a db file.
			}


			// getting here means there has been a write and we need to maintain only 10 copies
			// now have a look at this folder and maintain only 10 files
			DirectoryInfo Di = new DirectoryInfo(RacerMatePaths.BackupFullPath);
			FileInfo[] ALLfilesinDi;
			ALLfilesinDi = Di.GetFiles();
			List<DateTime> TheseFilesinDi = new List<DateTime>();
			TheseFilesinDi.Clear();

			foreach(FileInfo aaa in ALLfilesinDi) {
				if(aaa.Name.StartsWith(Path.GetFileNameWithoutExtension(infilename))) {
					TheseFilesinDi.Add(aaa.CreationTime);
				}
			}
			TheseFilesinDi.Sort(); //this will make them in increasing order

			DateTime cutdate = System.DateTime.Today;
			if(TheseFilesinDi.Count >= 10) {
				// pick the date of the index 9 entry
				cutdate = TheseFilesinDi[TheseFilesinDi.Count - 10];
				foreach(FileInfo aaa in ALLfilesinDi) {
					if(aaa.Name.StartsWith(Path.GetFileNameWithoutExtension(infilename)) &&
					aaa.LastWriteTime <= cutdate) {
						try {
							//Log.WriteLine("cleaning up " + appfolder + @"\" + aaa.Name);
							File.Delete(RacerMatePaths.BackupFullPath  + @"\" + aaa.Name);
						}
						catch { } //do nothing not important

					}
				}
			}
		}

#if DEBUG
		/**************************************************************************************************************
			tlm
		**************************************************************************************************************/

		private static void dump(String name) {
			Rider r = FindRiderByNick(name);
			if(r==null) {
				return;
			}



			int i, n;

			//n = r._gearingCrankset.Length;
			n = r.GearingCrankset.Length;

			for(i = 0; i < n; i++) {
				Log.WriteLine("         barak's Crankset[" + i.ToString() + "] = " + r.GearingCrankset[i].ToString());
			}

			Log.WriteLine("");

			n = r.GearingCogset.Length;

			for(i = 0; i < n; i++) {
				Log.WriteLine("         barak's Cogset[" + i.ToString() + "] = " + r.GearingCogset[i].ToString());
			}

			return;
		}                                   // dump()

#endif

	}                       // class Riders



	/**************************************************************************************************************
		
	**************************************************************************************************************/

	public class RiderPacerSeparator {
		private String separatorstring;
		public String SeparatorString {
			get { return separatorstring; }
			set {
				separatorstring = value;
			}
		}

		public RiderPacerSeparator(String invalue) {
			SeparatorString = invalue;
		}
		public override string ToString() {
			return SeparatorString;
		}

	}


}
