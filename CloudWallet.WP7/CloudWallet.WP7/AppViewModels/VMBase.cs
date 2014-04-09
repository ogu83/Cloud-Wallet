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
using System.ComponentModel;
using System.Reflection;

namespace CloudWallet
{
    public abstract class VMBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        public string AppTitle
        {
            get
            {
                var aAssembly = Assembly.GetExecutingAssembly();
                var title = (AssemblyTitleAttribute)
                                AssemblyTitleAttribute.GetCustomAttribute(
                                aAssembly, typeof(AssemblyTitleAttribute));
                return title.Title;
            }
        }

        public string AppDesc
        {
            get
            {
                var aAssembly = Assembly.GetExecutingAssembly();
                var desc = (AssemblyDescriptionAttribute)
                            AssemblyDescriptionAttribute.GetCustomAttribute(
                            aAssembly, typeof(AssemblyDescriptionAttribute));
                return desc.Description;
            }
        }

        public string AppVersion
        {
            get
            {
                var nameHelper = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
                return "Version: " + nameHelper.Version.ToString();
            }
        }

        protected bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged("IsBusy");
                NotifyPropertyChanged("IsNotBusy");
            }
        }
        public bool IsNotBusy { get { return !_isBusy; } }

        internal virtual void Initialize() { }
    }
}
