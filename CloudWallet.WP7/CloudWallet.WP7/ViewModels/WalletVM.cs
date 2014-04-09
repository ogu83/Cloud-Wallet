using CloudWallet.Crypto;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using Microsoft.Live;

namespace CloudWallet.ViewModels
{
    public class WalletVM : VMBase
    {
        public event EventHandler OnWalletDeleted;
        public event EventHandler OnSaveCompleted;

        public WalletVM()
        {
            Items = new ObservableCollection<ItemVM>();
        }

        #region Properties
        private ObservableCollection<ItemVM> _items;
        [XmlArray]
        public ObservableCollection<ItemVM> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
                NotifyPropertyChanged("FilteredItems");
            }
        }
        [XmlIgnore]
        public IEnumerable<ItemVM> FilteredItems
        {
            get
            {
                IEnumerable<ItemVM> filteredItems;
                if (!string.IsNullOrEmpty(_searchText))
                    filteredItems = from x in _items
                                    where x.Title.ToLower().Contains(_searchText.ToLower())
                                    orderby x.Title
                                    select x;
                else
                    filteredItems = from x in _items
                                    orderby x.Title
                                    select x;
                return filteredItems;
            }
        }

        //[NonSerialized]
        private string _searchText;
        [XmlIgnore]
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                NotifyPropertyChanged("SearchText");
                NotifyPropertyChanged("FilteredItems");
            }
        }


        private ItemVM _selectedItem;
        [XmlIgnore]
        public ItemVM SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
                NotifyPropertyChanged("IsAnyItemSelected");
            }
        }

        public bool IsAnyItemSelected
        {
            get { return _selectedItem != null; }
        }

        private const string _defaultFileName = "NewWallet";
        private string _fileName = _defaultFileName;
        [XmlAttribute]
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                NotifyPropertyChanged("FileName");
                NotifyPropertyChanged("Title");
            }
        }

        //private string _fullPath;
        [XmlAttribute]
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyPropertyChanged("Password");
            }
        }

        #endregion

        #region Methods
        public static WalletVM FromFile(string fileName, string password)
        {
            try
            {
                List<byte> myBytes = new List<byte>();
                using (IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream ifs = myFile.OpenFile(fileName, FileMode.Open))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ifs.CopyTo(ms);
                            WalletVM retVal = DeSerializeFromXML<WalletVM>(AES.Decrypt(ms.ToArray(), password));
                            retVal.BindItemEvents();
                            retVal.ResetChanges();
                            return retVal;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Corrupt file or wrong password", "Loading File Failed", MessageBoxButton.OK);
                return null;
            }
        }

        public void SaveFile()
        {
            byte[] myBytes = AES.Encrypt(this.SerializeToXML(), _password);

            MemoryStream ms = new MemoryStream(myBytes);
            var clientFolder = new LiveConnectClient(App.Session);
            clientFolder.UploadCompleted += (object sender, LiveOperationCompletedEventArgs e) =>
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message, "Upload Failed", MessageBoxButton.OK);

                string name = e.Result["name"].ToString();
                string id = e.Result["id"].ToString();

                if (App.MyWalletFile == null)
                {
                    App.MyWalletFile = new WalletFileWM { FileId = id, FileName = name };
                    App.MyWallet = this;
                }

                ms.Dispose();
            };
            if (App.MyWalletFile == null)
                clientFolder.UploadAsync("me/skydrive", FileName, ms, OverwriteOption.Rename);
            else
                clientFolder.UploadAsync(App.MyWalletFile.FileId, FileName, ms, OverwriteOption.Overwrite);

            if (OnSaveCompleted != null)
                OnSaveCompleted(this, null);
        }

        private void removeItem(ItemVM item)
        {
            item.Changed -= item_Changed;
            item.OnDeleted -= item_OnDeleted;
            this.Items.Remove(item);
            NotifyPropertyChanged("FilteredItems");
            IsChanged = true;
        }
        private void addItem(ItemVM item)
        {
            item.Changed += item_Changed;
            item.OnDeleted += item_OnDeleted;
            this.Items.Add(item);
            NotifyPropertyChanged("FilteredItems");
            SelectedItem = item;
            IsChanged = true;
        }

        private void item_OnDeleted(object sender, EventArgs e)
        {
            var myItem = sender as ItemVM;
            if (myItem != null)
                removeItem(myItem);
        }
        private void item_Changed(object sender, EventArgs e)
        {
            this.IsChanged = this.IsChanged || (sender as ItemVM).IsChanged;
        }

        internal void ResetChanges()
        {
            foreach (ItemVM item in Items)
                item.IsChanged = false;

            this.IsChanged = false;
        }

        private void BindItemEvents()
        {
            foreach (ItemVM item in _items)
            {
                item.Changed += item_Changed;
                item.OnDeleted += item_OnDeleted;
            }
        }
        #endregion

        #region Commands
        internal void AddCommand()
        {
            addItem(new ItemVM(Items.Count, "New Item", ""));
        }
        internal void DeleteCommand()
        {
            //if (SelectedItem != null)
            //    if (MessageBox.Show("Do you want to delete selected item?", "Delete Item", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //        removeItem(SelectedItem);
        }

        internal void DeleteWalletCommand()
        {
            var clientFolder = new LiveConnectClient(App.Session);
            clientFolder.DeleteCompleted += (object sender, LiveOperationCompletedEventArgs e) =>
                {
                    if (e.Error != null)
                        MessageBox.Show(e.Error.Message, "Delete Wallet Failed", MessageBoxButton.OK);

                    if (OnWalletDeleted != null)
                        OnWalletDeleted(this, null);
                };
            clientFolder.DeleteAsync(App.MyWalletFile.FileId);
        }
        #endregion
    }
}