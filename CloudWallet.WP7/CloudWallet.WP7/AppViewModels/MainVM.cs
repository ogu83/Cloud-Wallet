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
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Live;
using System.Collections;
using System.Collections.Generic;
using CloudWallet.Crypto;

namespace CloudWallet
{
    public class MainVM : VMBase
    {
        internal event EventHandler OnWalletSelected;

        internal override void Initialize()
        {
            base.Initialize();
        }

        #region Functions
        private void _clientFolder_DownloadCompleted(object sender, LiveDownloadCompletedEventArgs e)
        {
            if (e.Result == null)
                return;

            LoadingText = "Decrypting...";

            using (IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myFile.FileExists(_selectedWalletFile.FileName))
                    myFile.DeleteFile(_selectedWalletFile.FileName);

                using (IsolatedStorageFileStream myFileStream = new IsolatedStorageFileStream(_selectedWalletFile.FileName, FileMode.CreateNew, myFile))
                {
                    e.Result.CopyTo(myFileStream);
                    myFileStream.Close();
                    App.MyWallet = CloudWallet.ViewModels.WalletVM.FromFile(_selectedWalletFile.FileName, _password);
                    if (App.MyWallet != null)
                    {
                        App.MyWalletFile = _selectedWalletFile;
                        App.MyWallet.Password = _password;
                        if (OnWalletSelected != null)
                            OnWalletSelected(this, null);
                    }
                }
            }
            IsProgressing = false;
        }

        private void _clientFolder_DownloadProgressChanged(object sender, LiveDownloadProgressChangedEventArgs e)
        {
            LoadingValue = e.ProgressPercentage;
            LoadingText = e.ProgressPercentage + "% Loading...";
        }

        private void _clientFolder_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (WalletFiles == null)
                WalletFiles = new ObservableCollection<WalletFileWM>();

            if (e.Result == null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() => { IsBusy = false; }));
                return;
            }

            List<object> data = (List<object>)e.Result["data"];
            foreach (IDictionary<string, object> item in data)
            {
                if (item["type"].ToString() == "folder")
                {
                    _clientFolder.GetAsync(item["id"].ToString() + "/files");
                    Deployment.Current.Dispatcher.BeginInvoke(new Action(() => { IsBusy = true; }));
                    _busyCount++;
                }
                else if (item["type"].ToString() == "file")
                {
                    string name = item["name"].ToString();
                    string id = item["id"].ToString();
                    if (name.Contains(".wlt"))
                        WalletFiles.Add(new WalletFileWM { FileName = name, FileId = id });
                }
            }

            _busyCount--;
            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                IsBusy = _busyCount > 0;
                if (!_isBusy)
                    NotifyPropertyChanged("ThereIsNoWalletVisibility");
            }));
        }

        void _clientFolder_UploadCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        private LiveConnectClient _clientFolder;
        private byte _busyCount = 0;

        private ObservableCollection<WalletFileWM> _walletFiles;
        public ObservableCollection<WalletFileWM> WalletFiles
        {
            get { return _walletFiles; }
            set
            {
                if (_walletFiles != value)
                {
                    _walletFiles = value;
                    NotifyPropertyChanged("WalletFiles");
                }
            }
        }

        private WalletFileWM _selectedWalletFile;
        public WalletFileWM SelectedWalletFile
        {
            get { return _selectedWalletFile; }
            set
            {
                _selectedWalletFile = value;
                NotifyPropertyChanged("SelectedWalletFile");
            }
        }

        private bool _signedIn;
        public bool SignedIn
        {
            get { return _signedIn; }
            set
            {
                _signedIn = value;
                NotifyPropertyChanged("SignedIn");
                NotifyPropertyChanged("NotSignedIn");
            }
        }
        public bool NotSignedIn { get { return !_signedIn; } }

        public Visibility ThereIsNoWalletVisibility
        {
            get { return (_signedIn && _walletFiles.Count == 0).ToVisibility(); }
        }

        private string _loadingText;
        public string LoadingText
        {
            get { return _loadingText; }
            set
            {
                _loadingText = value;
                NotifyPropertyChanged("LoadingText");
            }
        }

        private bool _isProgressing;
        public bool IsProgressing
        {
            get { return _isProgressing; }
            set
            {
                _isProgressing = value;
                NotifyPropertyChanged("IsProgressing");
            }
        }

        private int _loadingValue;
        public int LoadingValue
        {
            get { return _loadingValue; }
            set
            {
                _loadingValue = value;
                NotifyPropertyChanged("LoadingValue");
            }
        }

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

        #region Commands
        internal void SignInCommand()
        {
            WalletFiles = null;
            _clientFolder = new LiveConnectClient(App.Session);
            _clientFolder.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(_clientFolder_GetCompleted);
            _clientFolder.DownloadProgressChanged += new EventHandler<LiveDownloadProgressChangedEventArgs>(_clientFolder_DownloadProgressChanged);
            _clientFolder.DownloadCompleted += new EventHandler<LiveDownloadCompletedEventArgs>(_clientFolder_DownloadCompleted);
            _clientFolder.UploadCompleted += new EventHandler<LiveOperationCompletedEventArgs>(_clientFolder_UploadCompleted);
            _clientFolder.GetAsync("me/skydrive/files");
            _busyCount++;
            IsBusy = true;
            SignedIn = true;
        }

        internal void OpenSelectedFileCommand()
        {
            if (_selectedWalletFile == null)
                return;

            IsProgressing = true;
            _clientFolder.DownloadAsync(_selectedWalletFile.FileId + "/content");
        }
        #endregion

        internal void NewWalletCommand()
        {
            if (string.IsNullOrEmpty(_password))
            {
                MessageBox.Show("Please enter a password", "Password can't be empty", MessageBoxButton.OK);
                return;
            }

            CloudWallet.ViewModels.WalletVM myWallet = new ViewModels.WalletVM() { FileName = "MyCloudWallet" + _walletFiles.Count.ToString() + ".wlt", Password = _password };
            App.MyWalletFile = null;
            App.MyWallet = myWallet;
            myWallet.OnSaveCompleted += myWallet_OnSaveCompleted;
            myWallet.AddCommand();
            myWallet.SaveFile();
            this.IsBusy = true;
        }

        void myWallet_OnSaveCompleted(object sender, EventArgs e)
        {
            (sender as CloudWallet.ViewModels.WalletVM).OnSaveCompleted -= myWallet_OnSaveCompleted;

            this.IsBusy = false;

            if (OnWalletSelected != null)
                OnWalletSelected(this, null);
        }
    }
}
