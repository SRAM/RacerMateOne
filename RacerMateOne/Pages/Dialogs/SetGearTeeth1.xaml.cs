
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
		public Rider rider;

        // These will store the number of teeth the user has entered for each gear
		public int[] front_gears = new int[RM1.MAX_FRONT_GEARS];
		public int[] rear_gears = new int[RM1.MAX_REAR_GEARS];

        // These are the actual UI elements that get displayed
		public Controls.Gear[] ChainGear_controls = new Controls.Gear[RM1.MAX_FRONT_GEARS];
		public Controls.Gear[] CogGear_controls = new Controls.Gear[RM1.MAX_REAR_GEARS];

        // These are the number of gears the user has selected
        private int m_numChainrings;
        private int m_numCogs;

		public int MaxTeeth = -1;
        
        /*****************************************************************************************************************************
			constructor
		*****************************************************************************************************************************/

        /*
		public SetGearTeeth1() {
			InitializeComponent();
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

            int maxTeeth = 0;
			for (int i = 0; i < m_numCogs; i++) {
				if (rear_gears[i] > maxTeeth)
					maxTeeth = rear_gears[i];
			}
			for (int i = 0; i < m_numChainrings; i++) {
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

#if DEBUG_LOG_ENABLED
			dump("SetGearTeeth1.xaml.cs, RedoTeeth");
#endif
			
			foreach (Controls.Gear gear in ChainGear_controls) {
				gear.Teeth = gear.Number >= m_numChainrings ? 0 : front_gears[gear.Number];
			}

			foreach (Controls.Gear gear in CogGear_controls) {
				gear.Teeth = gear.Number >= m_numCogs ? 0 : rear_gears[gear.Number];
			}

			Cog_Gear_Count.Content = "" + m_numCogs;
			Crank_Gear_Count.Content = "" + m_numChainrings;

			return;
		}									// RedoTeeth()

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void RiderOptions_Loaded(object sender, RoutedEventArgs e)  {
            m_numCogs = rider.GearingCogset.Length;
            m_numChainrings = rider.GearingCrankset.Length;
            
            Cog_Gear_Count.Content = "" + m_numCogs;
			Crank_Gear_Count.Content = "" + m_numChainrings;

			Controls.Gear.NoUpdate = true;

			for (int i = 0; i < rear_gears.Length; i++) {
				rear_gears[i] = i < m_numCogs ? rider.GearingCogset[i] : 0;
				((TextBox)FindName("Cogset_" + i)).Text = rear_gears[i].ToString();
				TextBox t = (TextBox)FindName("Cogset_" + i);
				t.Text = rear_gears[i].ToString();

				if (i >= m_numCogs) {
					t.Visibility = Visibility.Collapsed;
				}

				Controls.Gear gear = (Controls.Gear)FindName("Cog_Teeth_" + i);
				CogGear_controls[i] = gear;
				gear.Number = i;
				gear.Teeth = rear_gears[i];
				t.Tag = gear;
			}

			for (int i = 0; i < front_gears.Length; i++) {
				front_gears[i] = i < m_numChainrings ? rider.GearingCrankset[i] : 0;
				TextBox t = (TextBox)FindName("Chainring_" + i);
				t.Text = front_gears[i].ToString();
				if (i >= m_numChainrings) {
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
			if (m_numCogs > 1) {
                m_numCogs--;
				((TextBox)FindName("Cogset_" + m_numCogs)).Visibility = Visibility.Collapsed;
				RedoTeeth();
			}
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Cogset_Plus_Click(object sender, RoutedEventArgs e) {
			if (m_numCogs < RM1.MAX_REAR_GEARS) {
				((TextBox)FindName("Cogset_" + m_numCogs)).Visibility = Visibility.Visible;
                m_numCogs++;
				RedoTeeth();
			}
		}

		/*****************************************************************************************************************************
				front_gears is sorted in reverse order
		*****************************************************************************************************************************/

		private void Chainring_Minus_Click(object sender, RoutedEventArgs e) {
			if (m_numChainrings > 1) {
                m_numChainrings--;				// eliminate the lowest gear
#if DEBUG_LOG_ENABLED
				String s = "SetGearTeeth1.xaml.cs, making invisible: Chainring_" + m_numChainrings;
				Debug.WriteLine(s);
#endif

				((TextBox)FindName("Chainring_" + m_numChainrings)).Visibility = Visibility.Collapsed;
				RedoTeeth();
			}
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Chainring_Plus_Click(object sender, RoutedEventArgs e) {
			if (m_numChainrings < RM1.MAX_FRONT_GEARS) {
				((TextBox)FindName("Chainring_" + m_numChainrings)).Visibility = Visibility.Visible;
                m_numChainrings++;
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
				gear.Teeth = gear.Number >= m_numChainrings ? 0 : front_gears[gear.Number];
				s = "   " + gear.Number.ToString() + "   " + gear.Teeth.ToString() + "   " + front_gears[i++].ToString();
				Debug.WriteLine(s);
			}

			Debug.WriteLine("\nSetGearTeeth1.xaml.cs, rear gears, len = " + rear_gears.Length.ToString() + ":");
			i = 0;

			foreach (Controls.Gear gear in CogGear_controls) {
				gear.Teeth = gear.Number >= m_numCogs ? 0 : rear_gears[gear.Number];
				s = "   " + gear.Number.ToString() + "   " + gear.Teeth.ToString() + "   " + rear_gears[i++].ToString();
				Debug.WriteLine(s);
			}


		*****************************************************************************************************************************/

        private void RiderOptions_Unloaded(object sender, RoutedEventArgs e)
        {
#if DEBUG_LOG_ENABLED
			Debug.WriteLine("SetGearTeeth1.xaml.cs, RiderOptions_Unloaded()");
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
		}										// Chainset_TextChanged()

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void Help_Click(object sender, RoutedEventArgs e)  {
			AppWin.Help("Velotron_Gearing.htm");
		}

		/*****************************************************************************************************************************

		*****************************************************************************************************************************/

		private void save(object sender, RoutedEventArgs e)
        {
            // update the rider's chainrings
            int[] newCrankset = new int[m_numChainrings];
            for (int g = 0; g < m_numChainrings; g++)
            {
                newCrankset[g] = front_gears[g];
            }
            rider.GearingCrankset = newCrankset;

            // update the riders cogs
            int[] newCogset = new int[m_numCogs];
            for (int g = 0; g < m_numCogs; g++)
            {
                newCogset[g] = rear_gears[g];
            }
            rider.GearingCogset = newCogset;

#if DEBUG
            dump("save()");
#endif
            AppWin.Instance.MainFrame.NavigationService.GoBack();
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
				gear.Teeth = gear.Number >= m_numChainrings ? 0 : front_gears[gear.Number];
				s = "   " + gear.Number.ToString() + "   " + gear.Teeth.ToString() + "   " + front_gears[i++].ToString();
				Debug.WriteLine(s);
			}

			Debug.WriteLine("\nSetGearTeeth1.xaml.cs, rear gears, len = " + rear_gears.Length.ToString() + ":");
			i = 0;

			foreach (Controls.Gear gear in CogGear_controls) {
				gear.Teeth = gear.Number >= m_numCogs ? 0 : rear_gears[gear.Number];
				s = "   " + gear.Number.ToString() + "   " + gear.Teeth.ToString() + "   " + rear_gears[i++].ToString();
				Debug.WriteLine(s);
			}

			return;
		}									// dump()
#endif

	}								// public partial class SetGearTeeth1 : Page
}									// namespace RacerMateOne.Pages.Dialogs

