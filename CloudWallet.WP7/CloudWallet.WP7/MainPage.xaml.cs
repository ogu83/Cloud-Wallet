using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace CloudWallet
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MainVM _myVM;

        // Constructor
        public MainPage()
        {
            _myVM = new MainVM();
            _myVM.OnWalletSelected += new EventHandler(_myVM_OnWalletSelected);
            this.DataContext = _myVM;
            InitializeComponent();
            _myVM.Initialize();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (App.MyWalletFile != null && App.MyWallet != null)
            {
                App.MyWallet = null;
                App.MyWalletFile = null;
                _myVM.Initialize();
                
                if (App.Session != null)
                    _myVM.SignInCommand();
            }
        }

        private void _myVM_OnWalletSelected(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/WalletItemsPage.xaml", UriKind.Relative));
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void SignInButton_SessionChanged(object sender, Microsoft.Live.Controls.LiveConnectSessionChangedEventArgs e)
        {            
            if (e.Status == Microsoft.Live.LiveConnectSessionStatus.Connected)
            {
                //_myVM.IsBusy = true;
                App.Session = e.Session;
                _myVM.SignInCommand();
            }
        }

        private void SingInButton_Click(object sender, RoutedEventArgs e)
        {
            _myVM.IsBusy = true;
        }

        private void lstWallets_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _myVM.OpenSelectedFileCommand();
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }
        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!_myVM.SignedIn)
                return;

            passwordBoxWaterMark.Visibility = false.ToVisibility();
        }
        private void passwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!_myVM.SignedIn)
                return;

            passwordBoxWaterMark.Visibility = (string.IsNullOrEmpty(passwordBox.Password).ToVisibility());
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            _myVM.Password = passwordBox.Password;
            _myVM.NewWalletCommand();
        }

        private void btnNew1_Click(object sender, RoutedEventArgs e)
        {
            _myVM.Password = passwordBox.Password;
            _myVM.NewWalletCommand();
        }
    }
}