﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace TreeSize
{
    public class ModelItem
    {
        protected ObservableCollection<ModelItem> children;
        public ObservableCollection<ModelItem> ModelItems
        {
            get { return children; }
            set { children = value;
                OnPropertyChanged("Children");
            }
        }

        protected string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("ParrentPath");
            }
        }

        protected string header;
        public string Header
        {
            get { return header; }

            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }

        protected string fullPath;
        public string FullPath
        {
            get { return fullPath; }
            set
            {
                fullPath = value;
                OnPropertyChanged("Tag");
            }
        }

        protected BitmapSource image;
        public BitmapSource Image
        {
            get {return image;}
            set
            {
                image = value;
                OnPropertyChanged("PathToImage");
            }        
        }

        protected double volumeMemory;
        public double VolumeMemory
        {
            get { return volumeMemory; }
            set
            {
                volumeMemory = value;
                OnPropertyChanged("VolumeMemoty");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
