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


namespace RacerMateOne.Dialogs
{
    /// <summary>
    /// Interaction logic for GPXImportTrackSelector.xaml
    /// </summary>
    public partial class GpxImportTrackSelector : Window
    {
        public GpxImportTrackSelector(GPXLoader gpxLoader)
        {
            InitializeComponent();
            SelectedTrackName = string.Empty;
            SelectedTrackIndex = -1;
            foreach (GPXTrack gpxTrack in gpxLoader.GPXTrackList)
            {
                ListGpxTracks.Items.Add(gpxTrack.TrackName);
            }
        }

        public string SelectedTrackName { get; set; }
        public int SelectedTrackIndex { get; set; }


        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            SelectedTrackName = ListGpxTracks.SelectedItem.ToString();
            SelectedTrackIndex = ListGpxTracks.SelectedIndex;
            DialogResult = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }



    }
}
