
// d:\_fs\rm1\RacerMateOne_Source\RacerMateOne\Pages\Dialogs\SetGearTeeth1.xaml.cs

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
//using RacerMateOne.CourseEditorDev.Dialogs;
using System.Diagnostics;


namespace RacerMateOne.Pages.Dialogs {
	/// <summary>
	/// Interaction logic for SetGearTeeth1.xaml
	/// </summary>
	public partial class SetGearTeeth1 : Page  {
		//public Rider CurRider;
		public Rider rider;
		public int[] front_gears = new int[RM1.MAX_FRONT_GEARS];
		public int[] rear_gears = new int[RM1.MAX_REAR_GEARS];

		public Controls.Gear[] ChainGear_controls = new Controls.Gear[RM1.MAX_FRONT_GEARS];
		public Controls.Gear[] CogGear_controls = new Controls.Gear[RM1.MAX_REAR_GEARS];

		public int MaxChainring;
		public int MaxCogset;
		public int MaxTeeth = -1;

#if DEBUG
		private int bp = 0;
#endif
		private bool dosave = false;



		/*****************************************************************************************************************************
			constructor
		*****************************************************************************************************************************/

		/*
		public SetGearTeeth1() {
			InitializeComponent();
		}
		*/

		/*
		public SetGearTeeth2(Rider rider) {
			CurRider = new RiderExtended2(rider);
			CurRider.RiderType = rider.RiderType;					// Sad: previous programmer did not include in Contactor 
			InitializeComponent();
			this.DataContext = CurRider;
		}
		*/

		public SetGearTeeth1(Rider _rider) {
			//CurRider = new RiderExtended1(rider);
			//CurRider = new Rider(rider);
			rider = _rider;
			//CurRider.RiderType = rider.RiderType;					// Sad: previous programmer did not include in Contactor 
			InitializeComponent();
			//this.DataContext = CurRider;
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		void RedoMaxTeeth() {
#if DEBUG
			bp = 2;
#endif

			int maxTeeth = 0;
			for (int i = 0; i < MaxCogset; i++) {
				if (rear_gears[i] > maxTeeth)
					maxTeeth = rear_gears[i];
			}
			for (int i = 0; i < MaxChainring; i++) {
				if (front_gears[i] > maxTeeth)
					maxTeeth = front_gears[i];
			}
			if (maxTeeth == MaxTeeth)
				return;

			MaxTeeth = maxTeeth;

			foreach (Controls.Gear gear in CogGear_controls) {
				gear.MaxTeeth = maxTeeth;
			}

			foreach (Controls.Gear gear in ChainGear_controls) {
				gear.MaxTeeth = maxTeeth;
			}

			return;
		}										// RedoMaxTeeth()

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		void RedoTeeth() {

#if DEBUG
			dump("SetGearTeeth1.xaml.cs, RedoTeeth");
#endif
			
			foreach (Controls.Gear gear in ChainGear_controls) {
				gear.Teeth = gear.Number >= MaxChainring ? 0 : front_gears[gear.Number];
			}

			foreach (Controls.Gear gear in CogGear_controls) {
				gear.Teeth = gear.Number >= MaxCogset ? 0 : rear_gears[gear.Number];
			}

			Cog_Gear_Count.Content = "" + MaxCogset;
			Crank_Gear_Count.Content = "" + MaxChainring;

			return;
		}									// RedoTeeth()

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void RiderOptions_Loaded(object sender, RoutedEventArgs e)  {
			MaxCogset = rider.GearingCogset.Length;
			MaxChainring = rider.GearingCrankset.Length;
			Cog_Gear_Count.Content = "" + MaxCogset;
			Crank_Gear_Count.Content = "" + MaxChainring;

			Controls.Gear.NoUpdate = true;

			for (int i = 0; i < rear_gears.Length; i++) {
				rear_gears[i] = i < MaxCogset ? rider.GearingCogset[i] : 0;
				((TextBox)FindName("Cogset_" + i)).Text = rear_gears[i].ToString();
				TextBox t = (TextBox)FindName("Cogset_" + i);
				t.Text = rear_gears[i].ToString();

				if (i >= MaxCogset) {
					t.Visibility = Visibility.Collapsed;
				}

				Controls.Gear gear = (Controls.Gear)FindName("Cog_Teeth_" + i);
				CogGear_controls[i] = gear;
				gear.Number = i;
				gear.Teeth = rear_gears[i];
				t.Tag = gear;
			}

			for (int i = 0; i < front_gears.Length; i++) {
				front_gears[i] = i < MaxChainring ? rider.GearingCrankset[i] : 0;
				TextBox t = (TextBox)FindName("Chainring_" + i);
				t.Text = front_gears[i].ToString();
				if (i >= MaxChainring) {
					t.Visibility = Visibility.Collapsed;
				}

				Controls.Gear gear = (Controls.Gear)FindName("Crank_Teeth_" + i);
				ChainGear_controls[i] = gear;
				gear.Number = i;
				gear.Teeth = front_gears[i];
				gear.MaxTeeth = MaxTeeth;
				t.Tag = gear;
			}

			Controls.Gear.NoUpdate = false;
			RedoMaxTeeth();
		}												// RiderOptions_Loaded()


		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Cogset_Minus_Click(object sender, RoutedEventArgs e) {
			if (MaxCogset >= 2) {
				MaxCogset--;
				((TextBox)FindName("Cogset_" + MaxCogset)).Visibility = Visibility.Collapsed;
				RedoTeeth();
			}
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Cogset_Plus_Click(object sender, RoutedEventArgs e) {
			if (MaxCogset < rear_gears.Length) {
				((TextBox)FindName("Cogset_" + MaxCogset)).Visibility = Visibility.Visible;
				MaxCogset++;
				RedoTeeth();
			}
		}

		/*****************************************************************************************************************************
				front_gears is sorted in reverse order
		*****************************************************************************************************************************/

		private void Chainring_Minus_Click(object sender, RoutedEventArgs e) {
			if (MaxChainring >= 2) {
				MaxChainring--;				// eliminate the lowest gear
#if DEBUG
				String s = "SetGearTeeth1.xaml.cs, making invisible: Chainring_" + MaxChainring;
				Debug.WriteLine(s);

#endif
				((TextBox)FindName("Chainring_" + MaxChainring)).Visibility = Visibility.Collapsed;
				RedoTeeth();
			}

		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Chainring_Plus_Click(object sender, RoutedEventArgs e) {
			if (MaxChainring < front_gears.Length) {
				((TextBox)FindName("Chainring_" + MaxChainring)).Visibility = Visibility.Visible;
				MaxChainring++;
				RedoTeeth();
			}
		}



		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Back_Click(object sender, RoutedEventArgs e) {
			//save()?
#if DEBUG
			dump("Back_click()");
#endif

			AppWin.Instance.MainFrame.NavigationService.GoBack();
			return;

		}

		/*****************************************************************************************************************************
			foreach (Controls.Gear gear in ChainGear_controls) {
				gear.Teeth = gear.Number >= MaxChainring ? 0 : front_gears[gear.Number];
				s = "   " + gear.Number.ToString() + "   " + gear.Teeth.ToString() + "   " + front_gears[i++].ToString();
				Debug.WriteLine(s);
			}

			Debug.WriteLine("\nSetGearTeeth1.xaml.cs, rear gears, len = " + rear_gears.Length.ToString() + ":");
			i = 0;

			foreach (Controls.Gear gear in CogGear_controls) {
				gear.Teeth = gear.Number >= MaxCogset ? 0 : rear_gears[gear.Number];
				s = "   " + gear.Number.ToString() + "   " + gear.Teeth.ToString() + "   " + rear_gears[i++].ToString();
				Debug.WriteLine(s);
			}


		*****************************************************************************************************************************/

		private void RiderOptions_Unloaded(object sender, RoutedEventArgs e) {

#if DEBUG
			Debug.WriteLine("SetGearTeeth1.xaml.cs, RiderOptions_Unloaded()");
#endif
			if (!dosave) {
				return;
			}

			int i;

			i = 0;
			foreach (Controls.Gear gear in ChainGear_controls) {
				rider.GearingCrankset[i] = front_gears[i] = gear.Number >= MaxChainring ? 0 : front_gears[gear.Number];
				i++;
			}

			i = 0;
			foreach (Controls.Gear gear in CogGear_controls) {
				rider.GearingCogset[i] = rear_gears[i] = gear.Number >= MaxCogset ? 0 : rear_gears[gear.Number];
				i++;
			}

			//rider.GearingCrankset = front_gears;
			//rider.GearingCogset = rear_gears;

#if DEBUG
			dump("RiderOptions_Unloaded");
#endif

			return;
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Cogset_TextChanged(object sender, TextChangedEventArgs e) {
			TextBox tb = sender as TextBox;

			if (tb == null) {
				return;
			}

			Controls.Gear gear = tb.Tag as Controls.Gear;

			if (gear == null) {
				return;
			}

			int num = 0;

			try {
				num = Convert.ToInt32(tb.Text);
			}
			catch {
			}

			if (num < 100) {
				rear_gears[gear.Number] = num;
				RedoMaxTeeth();
				gear.Teeth = num;
			}

		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Chainset_TextChanged(object sender, TextChangedEventArgs e) {
			TextBox tb = sender as TextBox;

			if (tb == null) {
				return;
			}

			Controls.Gear gear = tb.Tag as Controls.Gear;

			if (gear == null) {
				return;
			}

			int num = 0;

			try {
				num = Convert.ToInt32(tb.Text);
			}
			catch {
			}

			if (num < 100) {
				front_gears[gear.Number] = num;
				RedoMaxTeeth();
				gear.Teeth = num;
			}

#if DEBUG
			if (num == 55) {
				bp = 4;
			}
#endif
		}										// Chainset_TextChanged()

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Help_Click(object sender, RoutedEventArgs e)  {
			AppWin.Help("Velotron_Gearing.htm");
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void save(object sender, RoutedEventArgs e) {

#if DEBUG
			dump("save()");
#endif
			dosave = true;

			//AppWin.Instance.MainFrame.NavigationService.GoBack();

			bool b = true;

			if (b) {
				// Documents and Settings\user\My Documents\RacerMate\Settings\RM1_RiderDB.xml
				string strDir = @"\RacerMate\Settings\RM1_RiderDB.xml";
				string DocumentDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				string FilePath = DocumentDir + strDir;

				/*
				XDocument rdoc = XDocument.Load(FilePath);
				string xmlDeclaration = rdoc.Declaration.ToString();
				string stylesheetDeclaration = rdoc.FirstNode.ToString();
				XNode Root = rdoc.Root;
				*/

#if DEBUG
				//Debug.WriteLine("\n\nSAVE:");
				//dump("SAVE");
#endif

				string FileName = System.IO.Path.GetFileName(FilePath);
				string strMessage = string.Format("{0} has been saved", FileName);

				/*
				//InfoDialog infoDialog = new InfoDialog(strMessage, ShowIcon.OK, FilePath);
				InfoDialog infoDialog = new InfoDialog(strMessage, 2, FilePath);
				infoDialog.Owner = Application.Current.MainWindow;
				infoDialog.ShowDialog();
				*/

				//AppWin.Instance.MainFrame.NavigationService.GoBack();
			}						// if (b)

			return;
		}							// save()

#if DEBUG

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		void dump(String title) {
			Debug.WriteLine("\n\n" + title );
			Debug.WriteLine("SetGearTeeth1.xaml.cs, front gears, len = " + front_gears.Length.ToString() + ":");
			String s;
			int i;
			i = 0;

			foreach (Controls.Gear gear in ChainGear_controls) {
				gear.Teeth = gear.Number >= MaxChainring ? 0 : front_gears[gear.Number];
				s = "   " + gear.Number.ToString() + "   " + gear.Teeth.ToString() + "   " + front_gears[i++].ToString();
				Debug.WriteLine(s);
			}

			Debug.WriteLine("\nSetGearTeeth1.xaml.cs, rear gears, len = " + rear_gears.Length.ToString() + ":");
			i = 0;

			foreach (Controls.Gear gear in CogGear_controls) {
				gear.Teeth = gear.Number >= MaxCogset ? 0 : rear_gears[gear.Number];
				s = "   " + gear.Number.ToString() + "   " + gear.Teeth.ToString() + "   " + rear_gears[i++].ToString();
				Debug.WriteLine(s);
			}

			return;
		}									// dump()
#endif

	}								// public partial class SetGearTeeth1 : Page
}									// namespace RacerMateOne.Pages.Dialogs

