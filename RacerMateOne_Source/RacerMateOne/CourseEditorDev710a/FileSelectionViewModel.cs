using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RacerMateOne.CourseEditorDev
{

    public class FileSelectionViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> gpxFileNames;
        public ObservableCollection<string> GpxFileNames
        {
            get { return gpxFileNames; }
            set
            {
                gpxFileNames = value;
                OnPropertyChanged("GpxFileNames");
            }
        }

        public FileSelectionViewModel()
        {
            gpxFileNames = new ObservableCollection<string>();
        }

        int selectd = 0;
        public int Selectd
        {
            get { return selectd; }
            set
            {
                selectd = value;
                OnPropertyChanged("Selectd");
            }
        }
        

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
