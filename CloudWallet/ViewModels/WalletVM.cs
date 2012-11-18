using CloudWallet.Crypto;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;

namespace CloudWallet.ViewModels
{
    [Serializable]
    [DataContract]
    public class WalletVM : VMBase
    {
        public WalletVM()
        {
            Items = new ObservableCollection<ItemVM>();
        }

        #region Properties
        private ObservableCollection<ItemVM> _items;
        [DataMember]
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

        [NonSerialized]
        private string _searchText;
        [IgnoreDataMember, XmlIgnore]
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
        [IgnoreDataMember, XmlIgnore]
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
        [DataMember]
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
        public string Title
        {
            get
            {
                return "Cloud Wallet v:"
                    + Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                    " - " + _fileName;
            }
        }

        private string _fullPath;

        [DataMember]
        [XmlAttribute]
        public string Password { get; set; }

        #endregion

        #region Methods
        private bool passwordAndSave()
        {
            PasswordDialog dialog = new PasswordDialog(Password);
            if (dialog.ShowDialog().Value)
            {
                Password = dialog.Password;
                if (!string.IsNullOrWhiteSpace(Password))
                    return saveToFile(_fullPath, Password);
                else
                    return false;
            }
            else
                return false;
        }

        private bool saveToFile(string fileName, string password)
        {
            try
            {
                File.WriteAllBytes(fileName, AES.Encrypt(SerializeToXML(), password));
                ResetChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Saving File Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static WalletVM FromFile(string fileName, string password)
        {
            try
            {
                return DeSerializeFromXML<WalletVM>(AES.Decrypt(File.ReadAllBytes(fileName), password));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Corrupt file or wrong password", "Loading File Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void removeItem(ItemVM item)
        {
            item.Changed -= item_Changed;
            this.Items.Remove(item);
            NotifyPropertyChanged("FilteredItems");
            IsChanged = true;
        }
        private void addItem(ItemVM item)
        {
            item.Changed += item_Changed;
            this.Items.Add(item);
            NotifyPropertyChanged("FilteredItems");
            SelectedItem = item;
            IsChanged = true;
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
                item.Changed += item_Changed;
        }
        #endregion

        #region Commands
        internal bool CloseCommand()
        {
            if (IsChanged)
            {
                switch (MessageBox.Show("Dou you want to save changes before close", "Close File", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        return SaveCommand();
                    case MessageBoxResult.No:
                        return true;
                    case MessageBoxResult.Cancel:
                    default:
                        return false;
                }
            }
            else
                return true;
        }

        internal bool SaveCommand()
        {
            if (_fileName == _defaultFileName)
                return SaveAsCommand();
            else
                return passwordAndSave();
        }

        internal bool SaveAsCommand()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = Properties.Settings.Default.LastSaveFolder;
            dialog.DefaultExt = "wlt";
            dialog.Filter = "Wallet files (*.wlt)|*.wlt|All files (*.*)|*.*";
            if (dialog.ShowDialog().Value)
            {
                Properties.Settings.Default.LastSaveFolder = dialog.InitialDirectory;
                Properties.Settings.Default.Save();
                _fullPath = dialog.FileName;
                FileName = dialog.SafeFileName;
                return passwordAndSave();
            }
            else
                return false;
        }

        internal static WalletVM OpenCommand()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Properties.Settings.Default.LastSaveFolder;
            dialog.DefaultExt = "wlt";
            dialog.Filter = "Wallet files (*.wlt)|*.wlt|All files (*.*)|*.*";
            if (dialog.ShowDialog().Value)
            {
                if (dialog.CheckFileExists)
                {
                    PasswordDialog dialog1 = new PasswordDialog();
                    if (dialog1.ShowDialog().Value)
                    {
                        if (!string.IsNullOrWhiteSpace(dialog1.Password))
                        {
                            WalletVM wallet = FromFile(dialog.FileName, dialog1.Password);
                            if (wallet != null)
                            {
                                Properties.Settings.Default.LastSaveFolder = dialog.InitialDirectory;
                                Properties.Settings.Default.Save();

                                wallet._fullPath = dialog.FileName;
                                wallet.FileName = dialog.SafeFileName;
                                wallet.Password = dialog1.Password;
                                wallet.BindItemEvents();
                                wallet.ResetChanges();
                            }
                            return wallet;
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;
        }

        internal void AddCommand()
        {
            addItem(new ItemVM(Items.Count, "New Item", ""));
        }

        internal void DeleteCommand()
        {
            if (SelectedItem != null)
                if (MessageBox.Show("Do you want to delete selected item?", "Delete Item", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    removeItem(SelectedItem);
        }
        #endregion
    }
}