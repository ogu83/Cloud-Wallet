using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
//using System.Threading.Tasks;

namespace CloudWallet.ViewModels
{
    //[Serializable]    
    public class ItemVM : VMBase
    {
        public event EventHandler OnDeleted;

        public ItemVM() { }
        public ItemVM(int id, string title, string content)
        {
            _id = id;
            _title = title;
            _content = content;
        }

        private string _title;
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

        public void DeleteCommand()
        {
            if (OnDeleted != null)
                OnDeleted(this, null);
        }
    }
}
