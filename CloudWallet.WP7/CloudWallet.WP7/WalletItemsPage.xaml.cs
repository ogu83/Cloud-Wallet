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
using CloudWallet.ViewModels;

namespace CloudWallet
{
    public partial class WalletItemsPage : PhoneApplicationPage
    {
        private WalletVM _myVM;

        public WalletItemsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (App.MyWallet != null)
            {
                _myVM = App.MyWallet;
                _myVM.OnWalletDeleted += new EventHandler(_myVM_OnWalletDeleted);
                this.DataContext = _myVM;
            }
            else
                NavigationService.GoBack(); //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            _myVM.OnWalletDeleted -= _myVM_OnWalletDeleted;
            base.OnNavigatedFrom(e);
        }

        void _myVM_OnWalletDeleted(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }
        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordBoxWaterMark.Visibility = false.ToVisibility();
        }
        private void passwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            passwordBoxWaterMark.Visibility = (string.IsNullOrEmpty(passwordBox.Password).ToVisibility());
        }

        private void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_myVM.SelectedItem != null)
            {
                App.MyWalletItem = _myVM.SelectedItem;
                NavigationService.Navigate(new Uri("/WalletItemDetailPage.xaml", UriKind.Relative));
            }
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            searchTextWaterMark.Visibility = false.ToVisibility();
        }
        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            searchTextWaterMark.Visibility = (string.IsNullOrEmpty(searchBox.Text).ToVisibility());
        }

        private void btnDeleteWallet_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to delete " + _myVM.FileName, "Delete Wallet", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _myVM.DeleteWalletCommand();
                NavigationService.GoBack();
            }
        }

        private void btnNewItem_Click(object sender, EventArgs e)
        {
            _myVM.AddCommand();
            App.MyWallet.SaveFile();
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
    }
}