
// d:\_fs\rm1\RacerMateOne_Source\RacerMateOne\CourseEditorDev\Options\SetGearTeeth2.xaml.cs

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
using System.Xml.Linq;
using RacerMateOne.CourseEditorDev.Dialogs;

namespace RacerMateOne.CourseEditorDev.Options {
	/// <summary>
	/// Interaction logic for SetGearTeeth2.xaml
	/// </summary>

	public partial class SetGearTeeth2 : Page  {
		public RiderExtended2 CurRider;

		/*****************************************************************************************************************************
			constructor
		*****************************************************************************************************************************/

		public SetGearTeeth2(Rider rider) {
			CurRider = new RiderExtended2(rider);
			CurRider.RiderType = rider.RiderType;					// Sad: previous programmer did not include in Contactor 
			InitializeComponent();
			this.DataContext = CurRider;
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Help_Click(object sender, RoutedEventArgs e)   {
			AppWin.Help("Velotron_Gearing.htm");
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Back_Click(object sender, RoutedEventArgs e)  {
			AppWin.Instance.MainFrame.NavigationService.GoBack();
		}

		/*****************************************************************************************************************************
			10 gears
		*****************************************************************************************************************************/

		void UpdateCogset()  {
			int i, n;
			n = CurRider.CurrentCogset;

			for (i=0; i<CurRider.CurrentCogset; i++)  {
				CurRider.CogGear[i].Show = true;
			}

			for (i = CurRider.CurrentCogset; i<10; i++)  {
				CurRider.CogGear[i].Show = false;
			}
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		void UpdateCrank()   {
			int i, n;
			n = CurRider.nCranks;

			for (i=0; i<CurRider.nCranks; i++)  {
				CurRider.CrankGear[i].Show = true;
			}

			for (i=CurRider.nCranks; i<3; i++)  {
				CurRider.CrankGear[i].Show = false;
			}

		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Cogset_Minus_Click(object sender, RoutedEventArgs e)   {
			if (CurRider.CurrentCogset > 1)  {
				CurRider.CurrentCogset--;
			}
			UpdateCogset();
		}


		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Cogset_Plus_Click(object sender, RoutedEventArgs e)  {
			if (CurRider.CurrentCogset >= 10)
				return;
			CurRider.CurrentCogset++;
			UpdateCogset();
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Chainring_Minus_Click(object sender, RoutedEventArgs e)  {
			if (CurRider.nCranks > 1)  {
				CurRider.nCranks--;
			}
			UpdateCrank();
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Chainring_Plus_Click(object sender, RoutedEventArgs e)  {
			if (CurRider.nCranks >= 3) {
				return;
			}
			CurRider.nCranks++;
			UpdateCrank();
		}

		/*****************************************************************************************************************************

		The Velotron Gear Shifting bug is:

		There is a gear table in RacerMate One Rider tab - only enabled with
		trainer=Velotron.  From it you can edit and change the gears as we've always
		allowed, like in Velotron 3D and Velotron CS.  Changes made to this screen are
			supposed read/write from:

			Users\My Documents\RacerMate\Settings\RM1_RiderDB.XML:

			<Rider DatabaseKey="11111111-1111-1111-1111-111111111111">

			<GearingCrankset>
			<G1>53</G1>
			<G2>39</G2>
			<G3>25</G3>
			</GearingCrankset>

			<GearingCogset>
			<G1>26</G1>
			<G2>23</G2>
			<G3>21</G3>
			<G4>19</G4>
			<G5>17</G5>
			<G6>16</G6>
			<G7>14</G7>
			<G8>13</G8>
			<G9>12</G9>
			<G10>11</G10>
			</GearingCogset>

			</Rider>

			As the above indicates every rider has an associated gear table and this is supposed to be edited within RacerMate One 'Riders' tab.

			Currently this data is not being saved when edited.

			Also, if you opt to change the data manually in the RiderDB file as an attempted work-around, the gears are still not following as the RiderDB shows
			but are coming from some other default gear table probably hard-coded to everyone.

			Note as well that the programmer Agha who did the Course Creator also started to work on this - on his own hook.  He changed the screen to accurately show
			sprockets and chains, etc.  But never went to the depth of fixing anything to read/write part of this.

		 *******************************************************************************************************************************/

		private void SaveBtn_Click(object sender, RoutedEventArgs e)   {
			// Documents and Settings\user\My Documents\RacerMate\Settings\RM1_RiderDB.xml
			string strDir = @"\RacerMate\Settings\RM1_RiderDB.xml";
			string DocumentDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string FilePath = DocumentDir + strDir;

			XDocument rdoc = XDocument.Load(FilePath);

			string xmlDeclaration = rdoc.Declaration.ToString();
			string stylesheetDeclaration = rdoc.FirstNode.ToString();
			XNode Root = rdoc.Root;

			List<BicycleRider> BicycleRiderList = new List<BicycleRider>();

			var docriders = from r in rdoc.Descendants("Rider")
				select new
				{
					DatabaseKey = r.Attribute("DatabaseKey").Value,
									Created = r.Element("Created").Value,
									Modified = r.Element("Modified").Value,
									FirstName = r.Element("FirstName").Value,
									LastName = r.Element("LastName").Value,
									NickName = r.Element("NickName").Value,
									Gender = r.Element("Gender").Value,
									Age = r.Element("Age").Value,
									HRAnT = r.Element("HRAeT").Value,
									HRMax = r.Element("HRMax").Value,
									HRMin = r.Element("HRMin").Value,
									AlarmMinZone = r.Element("AlarmMinZone").Value,
									AlarmMaxZone = r.Element("AlarmMaxZone").Value,
									PowerAnT = r.Element("PowerAeT").Value,
									PowerFTP = r.Element("PowerFTP").Value,
									Metric = r.Element("Metric").Value,
									WeightBike = r.Element("WeightBike").Value,
									WeightRider = r.Element("WeightRider").Value,
									Height = r.Element("Height").Value,
									DragFactor = r.Element("DragFactor").Value,
									RiderType = r.Element("RiderType").Value,
									Skin = r.Element("Colors").Attribute("Skin").Value,
									Hair = r.Element("Colors").Attribute("Hair").Value,
									Helmet = r.Element("Colors").Attribute("Helmet").Value,
									Shoes = r.Element("Colors").Attribute("Shoes").Value,
									Clothing1 = r.Element("Colors").Attribute("Clothing1").Value,
									Clothing2 = r.Element("Colors").Attribute("Clothing2").Value,
									BikeColor1 = r.Element("Colors").Attribute("BikeColor1").Value,
									BikeColor2 = r.Element("Colors").Attribute("BikeColor2").Value,

									GearingCrankset = from Crank in r.Descendants("GearingCrankset")
										select new
										{
											G1 = Crank.Element("G1").Value,
											G2 = Crank.Element("G2").Value,
											G3 = Crank.Element("G3").Value						// tlm20150406
										},

									GearingCogset = from Cog in r.Descendants("GearingCogset")
										select new
										{
											G1 = Cog.Element("G1").Value,
											G2 = Cog.Element("G2").Value,
											G3 = Cog.Element("G3").Value,
											G4 = Cog.Element("G4").Value,
											G5 = Cog.Element("G5").Value,
											G6 = Cog.Element("G6").Value,
											G7 = Cog.Element("G7").Value,
											G8 = Cog.Element("G8").Value,
											G9 = Cog.Element("G9").Value,
											G10 = Cog.Element("G10").Value

										},
									WheelDiameter = r.Element("WheelDiameter").Value

				};

			//-----------------------------------------------------------
			// docriders is a copy of the xml structure in memory
			//-----------------------------------------------------------

			BicycleRider rider;

			foreach (var br in docriders) {
				//BicycleRider BRider = new BicycleRider();
				rider = new BicycleRider();

				rider.DatabaseKey = br.DatabaseKey;
				rider.Created = br.Created;
				rider.Modified = br.Modified;
				rider.FirstName = br.FirstName;
				rider.LastName = br.LastName;
				rider.NickName = br.NickName;
				rider.Gender = br.Gender;
				rider.Age = br.Age;
				rider.HRAnT = br.HRAnT;
				rider.HRMax = br.HRMax;
				rider.HRMin = br.HRMin;
				rider.AlarmMinZone = br.AlarmMinZone;
				rider.AlarmMaxZone = br.AlarmMinZone;
				rider.PowerAnT = br.PowerAnT;
				rider.PowerFTP = br.PowerFTP;
				rider.Metric = br.Metric;
				rider.WeightBike = br.WeightBike;
				rider.WeightRider = br.WeightRider;
				rider.Height = br.Height;
				rider.DragFactor = br.DragFactor;
				rider.RiderType = br.RiderType;
				rider.Skin = br.Skin;
				rider.Hair = br.Hair;
				rider.Helmet = br.Helmet;
				rider.Shoes = br.Shoes;
				rider.Clothing1 = br.Clothing1;
				rider.Clothing2 = br.Clothing2;
				rider.BikeColor1 = br.BikeColor1;
				rider.BikeColor2 = br.BikeColor2;

				var GearingCrankset = br.GearingCrankset;

				foreach (var v in GearingCrankset)  {
					rider.CrankGear.Add(new GearData(int.Parse(v.G1), true));
					rider.CrankGear.Add(new GearData(int.Parse(v.G2), true));
					rider.CrankGear.Add(new GearData(int.Parse(v.G3), true));
				}

				var GearingCogset = br.GearingCogset;

				foreach (var v in GearingCogset)  {
					rider.CogGear.Add(new GearData(int.Parse(v.G1), true));
					rider.CogGear.Add(new GearData(int.Parse(v.G2), true));
					rider.CogGear.Add(new GearData(int.Parse(v.G3), true));
					rider.CogGear.Add(new GearData(int.Parse(v.G4), true));
					rider.CogGear.Add(new GearData(int.Parse(v.G5), true));
					rider.CogGear.Add(new GearData(int.Parse(v.G6), true));
					rider.CogGear.Add(new GearData(int.Parse(v.G7), true));
					rider.CogGear.Add(new GearData(int.Parse(v.G8), true));
					rider.CogGear.Add(new GearData(int.Parse(v.G9), true));
					rider.CogGear.Add(new GearData(int.Parse(v.G10), true));

				}
				rider.WheelDiameter = br.WheelDiameter;

				BicycleRiderList.Add(rider);
			}													// foreach (var br in BicycleRiders)

			for (int i = 0; i < BicycleRiderList.Count; i++)  {
				if (BicycleRiderList[i].DatabaseKey == CurRider.DatabaseKey)   {
					BicycleRiderList[i].Created = CurRider.Created.ToString("G");
					BicycleRiderList[i].Modified = DateTime.Now.ToString("G");

					BicycleRiderList[i].CrankGear[0] = CurRider.CrankGear[0];
					BicycleRiderList[i].CrankGear[1] = CurRider.CrankGear[1];
					BicycleRiderList[i].CrankGear[2] = CurRider.CrankGear[2];

					for (int j=0; j<10; j++)  {
						BicycleRiderList[i].CogGear[j] = CurRider.CogGear[j];
					}

					break;
				}
			}											// foreach (var br in BicycleRiders)  {

			XElement Riders = new XElement("Riders", new XAttribute("VersionMajor", "1"), new XAttribute("VersionMinor", "0"));

			for (int i=0; i<BicycleRiderList.Count; i++)   {
				XElement Val = new XElement("Rider", new XAttribute("DatabaseKey", BicycleRiderList[i].DatabaseKey));
				Val.Add(new XElement("Created", BicycleRiderList[i].Created));
				Val.Add(new XElement("Modified", BicycleRiderList[i].Modified));
				Val.Add(new XElement("FirstName", BicycleRiderList[i].FirstName));
				Val.Add(new XElement("LastName", BicycleRiderList[i].LastName));
				Val.Add(new XElement("NickName", BicycleRiderList[i].NickName));
				Val.Add(new XElement("Gender", BicycleRiderList[i].Gender));
				Val.Add(new XElement("Age", BicycleRiderList[i].Age));
				Val.Add(new XElement("HRAeT", BicycleRiderList[i].HRAnT));
				Val.Add(new XElement("HRMax", BicycleRiderList[i].HRMax));
				Val.Add(new XElement("HRMin", BicycleRiderList[i].HRMin));

				Val.Add(new XElement("AlarmMinZone", BicycleRiderList[i].AlarmMinZone));
				Val.Add(new XElement("AlarmMaxZone", BicycleRiderList[i].AlarmMaxZone));
				Val.Add(new XElement("PowerAeT", BicycleRiderList[i].PowerAnT));
				Val.Add(new XElement("PowerFTP", BicycleRiderList[i].PowerFTP));
				Val.Add(new XElement("Metric", BicycleRiderList[i].Metric));
				Val.Add(new XElement("WeightBike", BicycleRiderList[i].WeightBike));
				Val.Add(new XElement("WeightRider", BicycleRiderList[i].WeightRider));
				Val.Add(new XElement("Height", BicycleRiderList[i].Height));
				Val.Add(new XElement("DragFactor", BicycleRiderList[i].DragFactor));
				Val.Add(new XElement("RiderType", BicycleRiderList[i].RiderType));

				Val.Add(new XElement("Colors", new XAttribute("Skin", BicycleRiderList[i].Skin),
							new XAttribute("Hair", BicycleRiderList[i].Hair),
							new XAttribute("Helmet", BicycleRiderList[i].Helmet),
							new XAttribute("Shoes", BicycleRiderList[i].Shoes),
							new XAttribute("Clothing1", BicycleRiderList[i].Clothing1),
							new XAttribute("Clothing2", BicycleRiderList[i].Clothing2),
							new XAttribute("BikeColor1", BicycleRiderList[i].BikeColor1),
							new XAttribute("BikeColor2", BicycleRiderList[i].BikeColor2)));

				int CrankGear0Teeth = BicycleRiderList[i].CrankGear[0].Show == true ? BicycleRiderList[i].CrankGear[0].Teeth : 0;
				int CrankGear1Teeth = BicycleRiderList[i].CrankGear[1].Show == true ? BicycleRiderList[i].CrankGear[1].Teeth : 0;
				int CrankGear2Teeth = BicycleRiderList[i].CrankGear[2].Show == true ? BicycleRiderList[i].CrankGear[2].Teeth : 0;

				Val.Add(new XElement("GearingCrankset",
							new XElement("G1", CrankGear0Teeth),
							new XElement("G2", CrankGear1Teeth),
							new XElement("G3", CrankGear2Teeth)));			// tlm20150406

				int CogGear0Teeth = BicycleRiderList[i].CogGear[0].Show == true ? BicycleRiderList[i].CogGear[0].Teeth : 0;
				int CogGear1Teeth = BicycleRiderList[i].CogGear[1].Show == true ? BicycleRiderList[i].CogGear[1].Teeth : 0;
				int CogGear2Teeth = BicycleRiderList[i].CogGear[2].Show == true ? BicycleRiderList[i].CogGear[2].Teeth : 0;
				int CogGear3Teeth = BicycleRiderList[i].CogGear[3].Show == true ? BicycleRiderList[i].CogGear[3].Teeth : 0;
				int CogGear4Teeth = BicycleRiderList[i].CogGear[4].Show == true ? BicycleRiderList[i].CogGear[4].Teeth : 0;
				int CogGear5Teeth = BicycleRiderList[i].CogGear[5].Show == true ? BicycleRiderList[i].CogGear[5].Teeth : 0;
				int CogGear6Teeth = BicycleRiderList[i].CogGear[6].Show == true ? BicycleRiderList[i].CogGear[6].Teeth : 0;
				int CogGear7Teeth = BicycleRiderList[i].CogGear[7].Show == true ? BicycleRiderList[i].CogGear[7].Teeth : 0;
				int CogGear8Teeth = BicycleRiderList[i].CogGear[8].Show == true ? BicycleRiderList[i].CogGear[8].Teeth : 0;
				int CogGear9Teeth = BicycleRiderList[i].CogGear[9].Show == true ? BicycleRiderList[i].CogGear[9].Teeth : 0;

				Val.Add(new XElement("GearingCogset", new XElement("G1", CogGear0Teeth),
							new XElement("G2", CogGear1Teeth),
							new XElement("G3", CogGear2Teeth),
							new XElement("G4", CogGear3Teeth),
							new XElement("G5", CogGear4Teeth),
							new XElement("G6", CogGear5Teeth),
							new XElement("G7", CogGear6Teeth),
							new XElement("G8", CogGear7Teeth),
							new XElement("G9", CogGear8Teeth),
							new XElement("G10", CogGear9Teeth)));

				Val.Add(new XElement("WheelDiameter", BicycleRiderList[i].WheelDiameter));

				Riders.Add(Val);
			}


			XDocument xDoc = new XDocument(
					new XDeclaration("1.0", "utf-8", "yes"),
					new XProcessingInstruction("xml-stylesheet", "type='text/xsl' href='..\\RacerMate\\ReportTemplates\\RM1_RiderDB.xsl'"),
					new XComment("RaceMate One Rider Database"), Riders);

			xDoc.Save(FilePath);

			string FileName = System.IO.Path.GetFileName(FilePath);
			string strMessage = string.Format("{0} has been saved", FileName);
			InfoDialog infoDialog = new InfoDialog(strMessage, ShowIcon.OK, FilePath);
			infoDialog.Owner = Application.Current.MainWindow;
			infoDialog.ShowDialog();
			return;
		}							// SaveBtn_Click()


		/*****************************************************************************************************************************

		*******************************************************************************************************************************/ 

		private void ItemsControlCrankGearSizeChanged(object sender, SizeChangedEventArgs e)   {

			ItemsControl ICtrl = (ItemsControl)sender;
			ItemCollection ICol = ICtrl.Items;

			if (ICol != null && ICol.Count > 0)   {
				int n = ICol.Count;										// tlm20150406

				for (int i=0; i<n; i++)   {
					GearData tb = (GearData)ICol[i];
					// tlm20150406+++
					//tb.BackBrush = (i == CurRider.CurrentCrank - 1) ? Brushes.Red : Brushes.White;
					tb.BackBrush = Brushes.White;
					// tlm20150406---
				}
			}
		}

		/*****************************************************************************************************************************

		*******************************************************************************************************************************/

		private void ItemsControlCogGearSizeChanged(object sender, SizeChangedEventArgs e)  {


			ItemsControl ICtrl = (ItemsControl)sender;
			ItemCollection ICol = ICtrl.Items;

			if (ICol != null && ICol.Count > 0)  {
				int n = ICol.Count;								// tlm20150406
				for (int i = 0; i < n; i++)  {
					GearData tb = (GearData)ICol[i];
					// tlm20150406+++
					//tb.BackBrush = (i == CurRider.CurrentCogset - 1) ? Brushes.Red : Brushes.White;
					tb.BackBrush = Brushes.White;
					// tlm20150406---
				}
			}

		}
	}										// public partial class SetGearTeeth2 : Page

}											// namespace RacerMateOne.CourseEditorDev.Options {
