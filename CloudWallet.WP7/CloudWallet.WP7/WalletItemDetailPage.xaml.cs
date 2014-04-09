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
    public partial class WalletItemDetailPage : PhoneApplicationPage
    {
        private ItemVM _myVM;

        public WalletItemDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (App.MyWalletItem != null)
            {
                _myVM = App.MyWalletItem;
                this.DataContext = _myVM;
            }
            else
                NavigationService.GoBack();

            base.OnNavigatedTo(e);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (EditPanel.Visibility == System.Windows.Visibility.Visible)
                return;

            Storyboard sb = Resources["SB_EditOpen"] as Storyboard;
            sb.Begin();
            this.EditPanel.Visibility = true.ToVisibility();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to delete " + _myVM.Title, "Delete Item", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _myVM.DeleteCommand();
                App.MyWallet.SaveFile();
                NavigationService.GoBack();
            }
        }

        private void btnCloseEdit_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["SB_EditClose"] as Storyboard;
            sb.Completed += (object sender1, EventArgs e1) =>
            {
                this.EditPanel.Visibility = false.ToVisibility();
                App.MyWallet.SaveFile();
            };
            sb.Begin();
        }
    }
}