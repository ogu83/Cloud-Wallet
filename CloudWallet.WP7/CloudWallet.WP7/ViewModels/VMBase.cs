using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
//using System.Threading.Tasks;

namespace CloudWallet.ViewModels
{
    [XmlInclude(typeof(ItemVM)), XmlInclude(typeof(WalletVM))]
    public abstract class VMBase : INotifyPropertyChanged
    {
        //[field: NonSerialized]
        //[XmlIgnore]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        //[field: NonSerialized]
        //[XmlIgnore]
        public event EventHandler Changed;
        protected void RaiseChanged()
        {
            if (Changed != null)
                Changed(this, null);
        }

        protected int _id;
        [XmlAttribute]
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        //[NonSerialized]        
        private bool _isChanged;
        [XmlIgnore]
        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                _isChanged = value;
                NotifyPropertyChanged("IsChanged");
                RaiseChanged();
            }
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

        protected static T DeSerializeFromXML<T>(byte[] bytes) where T : VMBase
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(bytes);
            return (T)serializer.Deserialize(ms);
        }

        protected byte[] SerializeToXML()
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, this);
            return ms.ToArray();
        }
    }
}