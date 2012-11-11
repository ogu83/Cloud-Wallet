using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CloudWallet.ViewModels
{
    [Serializable]
    public abstract class VMBase : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        [field: NonSerialized]
        public event EventHandler Changed;
        protected void RaiseChanged()
        {
            if (Changed != null)
                Changed(this, null);
        }

        protected int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        [NonSerialized]
        private bool _isChanged;        
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

        protected byte[] SerializeToBinary()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            return ms.ToArray();
        }

        protected static T SerializeFromBinary<T>(byte[] bytes) where T : VMBase
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(bytes);
            return (T)bf.Deserialize(ms);
        }
    }
}