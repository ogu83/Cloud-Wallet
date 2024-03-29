﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CloudWallet.ViewModels
{
    [Serializable]
    [DataContract]
    public class ItemVM : VMBase
    {
        public ItemVM() { }
        public ItemVM(int id, string title, string content)
        {
            _id = id;
            _title = title;
            _content = content;
        }

        private string _title;
        [DataMember]
        [XmlAttribute]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
                IsChanged = true;
            }
        }

        private string _content;
        [DataMember]
        [XmlAttribute]
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                NotifyPropertyChanged("Content");
                IsChanged = true;
            }
        }
    }
}
