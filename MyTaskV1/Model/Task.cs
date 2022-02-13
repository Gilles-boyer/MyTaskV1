using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTaskV1
{
    class Task : INotifyPropertyChanged
    {
        private int _id;
        private string _label;
        private string _statut;
        private string _date;
        private int _ordre;
        private int _last;

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
                OnPropertyChanged("Label");
            }
        }

        public string Statut
        {
            get
            {
                return _statut;
            }
            set
            {
                _statut = value;
                OnPropertyChanged("Statut");
            }
        }

        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        public int Ordre
        {
            get
            {
                return _ordre;
            }
            set
            {
                _ordre = value;
                OnPropertyChanged("Ordre");
            }
        }

        public int Last
        {
            get
            {
                return _last;
            }
            set
            {
                _last = value;
                OnPropertyChanged("Last");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
