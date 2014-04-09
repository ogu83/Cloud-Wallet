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
using Microsoft.Phone.Tasks;

namespace CloudWallet
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            this.DataContext = new AboutVM();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Uri u = new Uri("http://en.oguzkoroglu.net/post/2012/11/11/Cloud-Wallet.aspx", UriKind.Absolute);
            WebBrowserTask myTask = new WebBrowserTask();
            myTask.Uri = u;
            myTask.Show();
        }
    }
}