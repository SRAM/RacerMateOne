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

namespace RacerMateOne.CourseEditorDev.Options
{
    /// <summary>
    /// Interaction logic for SetGearTeeth.xaml
    /// </summary>
    public partial class SetGearTeeth : Page
    {
        public RiderExtended CurRider;
        public SetGearTeeth(Rider rider)
        {
            CurRider = new RiderExtended(rider);
            CurRider.RiderType = rider.RiderType; // Sad: previous programmer did not include in Contactor 
            InitializeComponent();
            this.DataContext = CurRider;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            AppWin.Help("Velotron_Gearing.htm");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AppWin.Instance.MainFrame.NavigationService.GoBack();
        }

        // 10 gears
        void UpdateCogset()
        {
            for (int i = 0; i < CurRider.CurrentCogset; i++)
            {
                CurRider.CogGear[i].Show = true;
            }

            for (int i = CurRider.CurrentCogset; i < 10; i++)
            {
                CurRider.CogGear[i].Show = false;
            }
        }

        // 3 gears
        void UpdateCrank()
        {
            for (int i = 0; i < CurRider.CurrentCrank; i++)
            {
                CurRider.CrankGear[i].Show = true;
            }

            for (int i = CurRider.CurrentCrank; i < 3; i++)
            {
                CurRider.CrankGear[i].Show = false;
            }
       
        }


        private void Cogset_Minus_Click(object sender, RoutedEventArgs e)
        {
            if (CurRider.CurrentCogset > 1)
            {
                CurRider.CurrentCogset--;
            }
            UpdateCogset();
        }

       

        private void Cogset_Plus_Click(object sender, RoutedEventArgs e)
        {
            if (CurRider.CurrentCogset >= 10)
                return;
            CurRider.CurrentCogset++;
            UpdateCogset();
        }

        private void Chainring_Minus_Click(object sender, RoutedEventArgs e)
        {
            if (CurRider.CurrentCrank > 1)
            {
                CurRider.CurrentCrank--;
            }
            UpdateCrank();
        }

        private void Chainring_Plus_Click(object sender, RoutedEventArgs e)
        {
            if (CurRider.CurrentCrank >= 3)
                return;
            CurRider.CurrentCrank++;
            UpdateCrank();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Documents and Settings\user\My Documents\RacerMate\Settings\RM1_RiderDB.xml
            string strDir = @"\RacerMate\Settings\RM1_RiderDB.xml";
            string DocumentDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string FilePath = DocumentDir + strDir;
            XDocument RM1_RiderDBdoc = XDocument.Load(FilePath);

            string xmlDeclaration = RM1_RiderDBdoc.Declaration.ToString();
            string stylesheetDeclaration = RM1_RiderDBdoc.FirstNode.ToString();
            XNode Root = RM1_RiderDBdoc.Root;

            List<BicycleRider> BicycleRiderList = new List<BicycleRider>();

            var BicycleRiders = from r in RM1_RiderDBdoc.Descendants("Rider")
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
                                    HRAeT = r.Element("HRAeT").Value,
                                    HRMax = r.Element("HRMax").Value,
                                    HRMin = r.Element("HRMin").Value,
                                    AlarmMinZone = r.Element("AlarmMinZone").Value,
                                    AlarmMaxZone = r.Element("AlarmMaxZone").Value,
                                    PowerAeT = r.Element("PowerAeT").Value,
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
                                                              G2 = Crank.Element("G2").Value

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

            foreach (var br in BicycleRiders)
            {
                BicycleRider BRider = new BicycleRider();
                BRider.DatabaseKey = br.DatabaseKey;
                BRider.Created = br.Created;
                BRider.Modified = br.Modified;
                BRider.FirstName = br.FirstName;
                BRider.LastName = br.LastName;
                BRider.NickName = br.NickName;
                BRider.Gender = br.Gender;
                BRider.Age = br.Age;
                BRider.HRAeT = br.HRAeT;
                BRider.HRMax = br.HRMax;
                BRider.HRMin = br.HRMin;
                BRider.AlarmMinZone = br.AlarmMinZone;
                BRider.AlarmMaxZone = br.AlarmMinZone;
                BRider.PowerAeT = br.PowerAeT;
                BRider.PowerFTP = br.PowerFTP;
                BRider.Metric = br.Metric;
                BRider.WeightBike = br.WeightBike;
                BRider.WeightRider = br.WeightRider;
                BRider.Height = br.Height;
                BRider.DragFactor = br.DragFactor;
                BRider.RiderType = br.RiderType;
                BRider.Skin = br.Skin;
                BRider.Hair = br.Hair;
                BRider.Helmet = br.Helmet;
                BRider.Shoes = br.Shoes;
                BRider.Clothing1 = br.Clothing1;
                BRider.Clothing2 = br.Clothing2;
                BRider.BikeColor1 = br.BikeColor1;
                BRider.BikeColor2 = br.BikeColor2;
                
                var GearingCrankset = br.GearingCrankset;
                foreach (var v in GearingCrankset)
                {
                    BRider.CrankGear.Add(new GearData(int.Parse(v.G1), true));
                    BRider.CrankGear.Add(new GearData(int.Parse(v.G2), true));
                }

                var GearingCogset = br.GearingCogset;
                foreach (var v in GearingCogset)
                {
                    BRider.CogGear.Add(new GearData(int.Parse(v.G1), true));
                    BRider.CogGear.Add(new GearData(int.Parse(v.G2), true));
                    BRider.CogGear.Add(new GearData(int.Parse(v.G3), true));
                    BRider.CogGear.Add(new GearData(int.Parse(v.G4), true));
                    BRider.CogGear.Add(new GearData(int.Parse(v.G5), true));
                    BRider.CogGear.Add(new GearData(int.Parse(v.G6), true));
                    BRider.CogGear.Add(new GearData(int.Parse(v.G7), true));
                    BRider.CogGear.Add(new GearData(int.Parse(v.G8), true));
                    BRider.CogGear.Add(new GearData(int.Parse(v.G9), true));
                    BRider.CogGear.Add(new GearData(int.Parse(v.G10), true));

                }
                BRider.WheelDiameter = br.WheelDiameter;

                BicycleRiderList.Add(BRider);
            }

            for (int i = 0; i < BicycleRiderList.Count; i++)
            {
                if (BicycleRiderList[i].DatabaseKey == CurRider.DatabaseKey)
                {
                    BicycleRiderList[i].Created = CurRider.Created.ToString("G");
                    BicycleRiderList[i].Modified = DateTime.Now.ToString("G");
                    BicycleRiderList[i].CrankGear[0] = CurRider.CrankGear[0];
                    BicycleRiderList[i].CrankGear[1] = CurRider.CrankGear[1];

                    for (int j = 0; j < 10; j++)
                    {
                        BicycleRiderList[i].CogGear[j] = CurRider.CogGear[j];
                    }
                    break;
                }
            }

            XElement Riders = new XElement("Riders", new XAttribute("VersionMajor", "1"), new XAttribute("VersionMinor", "0"));
            
            for (int i = 0; i < BicycleRiderList.Count; i++)
            {
                XElement Val = new XElement("Rider", new XAttribute("DatabaseKey", BicycleRiderList[i].DatabaseKey));
                Val.Add(new XElement("Created", BicycleRiderList[i].Created));
                Val.Add(new XElement("Modified", BicycleRiderList[i].Modified));
                Val.Add(new XElement("FirstName", BicycleRiderList[i].FirstName));
                Val.Add(new XElement("LastName", BicycleRiderList[i].LastName));
                Val.Add(new XElement("NickName", BicycleRiderList[i].NickName));
                Val.Add(new XElement("Gender", BicycleRiderList[i].Gender));
                Val.Add(new XElement("Age", BicycleRiderList[i].Age));
                Val.Add(new XElement("HRAeT", BicycleRiderList[i].HRAeT));
                Val.Add(new XElement("HRMax", BicycleRiderList[i].HRMax));
                Val.Add(new XElement("HRMin", BicycleRiderList[i].HRMin));

                Val.Add(new XElement("AlarmMinZone", BicycleRiderList[i].AlarmMinZone));
                Val.Add(new XElement("AlarmMaxZone", BicycleRiderList[i].AlarmMaxZone));
                Val.Add(new XElement("PowerAeT", BicycleRiderList[i].PowerAeT));
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

                Val.Add(new XElement("GearingCrankset", new XElement("G1", CrankGear0Teeth), new XElement("G2", CrankGear1Teeth)));

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
                            
        }
        private void ItemsControlCrankGearSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemsControl ICtrl = (ItemsControl)sender;
            ItemCollection ICol = ICtrl.Items;

            if (ICol != null && ICol.Count > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    GearData tb = (GearData)ICol[i];
                    tb.BackBrush = (i == CurRider.CurrentCrank - 1) ? Brushes.Red : Brushes.White;
                }
            }
        }

        private void ItemsControlCogGearSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemsControl ICtrl = (ItemsControl)sender;
            ItemCollection ICol = ICtrl.Items;

            if (ICol != null && ICol.Count > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    GearData tb = (GearData)ICol[i];
                    tb.BackBrush = (i == CurRider.CurrentCogset - 1) ? Brushes.Red : Brushes.White;
                }
            }
        }
    }
}
