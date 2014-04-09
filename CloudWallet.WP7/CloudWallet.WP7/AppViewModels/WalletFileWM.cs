using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CloudWallet
{
    public class WalletFileWM : VMBase
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                NotifyPropertyChanged("FileName");
            }
        }

        public bool IsParsed { get; set; }

        public string FileId { get; set; }
    }
}
