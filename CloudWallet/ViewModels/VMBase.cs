using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CloudWallet.ViewModels
{
    [Serializable]
    [DataContract]
    [KnownType(typeof(object))]
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
        [DataMember]
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
        [IgnoreDataMember]
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

        //protected byte[] SerializeToBinary()
        //{
        //    return SerializeToBinary<VMBase>(this);
        //}
        //protected static byte[] SerializeToBinary<T>(T obj)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(ms))
        //        {
        //            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
        //            dcs.WriteObject(writer, obj);
        //            writer.Flush();
        //            return ms.ToArray();
        //        }
        //    }
        //}
        protected byte[] SerializeToBinary()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            return ms.ToArray();
        }


        //protected static T SerializeFromBinary<T>(byte[] xml)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream(xml))
        //    {
        //        using (XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(
        //            memoryStream, XmlDictionaryReaderQuotas.Max))
        //        {
        //            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
        //            return (T)dcs.ReadObject(reader);
        //        }
        //    }
        //}
        protected static T SerializeFromBinary<T>(byte[] bytes) where T : VMBase
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(bytes);
            return (T)bf.Deserialize(ms);
        }
    }
}