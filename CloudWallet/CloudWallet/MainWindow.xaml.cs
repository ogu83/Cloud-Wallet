﻿using CloudWallet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CloudWallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WalletVM _myVM;
        private void SetMyVM(WalletVM vm)
        {
            _myVM = vm;
            _myVM.ResetChanges();
            this.DataContext = _myVM;
        }

        public MainWindow()
        {
            InitializeComponent();

            SetMyVM(new WalletVM());

            this.Width = Properties.Settings.Default.LastWidth;
            this.Height = Properties.Settings.Default.LastHeight;
            this.Left = Properties.Settings.Default.LastLeft;
            this.Top = Properties.Settings.Default.LastTop;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_myVM.CloseCommand())
            {
                e.Cancel = true;
                return;
            }

            Properties.Settings.Default.LastWidth = (int)this.Width;
            Properties.Settings.Default.LastHeight = (int)this.Height;
            Properties.Settings.Default.LastLeft = (int)this.Left;
            Properties.Settings.Default.LastTop = (int)this.Top;
            Properties.Settings.Default.Save();

            base.OnClosing(e);
        }

        private void btnExit_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click_1(object sender, RoutedEventArgs e)
        {
            if (_myVM.CloseCommand())
                SetMyVM(new WalletVM());
        }

        private void btnOpen_Click_1(object sender, RoutedEventArgs e)
        {
            if (_myVM.CloseCommand())
            {
                WalletVM wallet = WalletVM.OpenCommand();
                if (wallet != null)
                    SetMyVM(wallet);
            }
        }

        private void btnOpenCloud_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            _myVM.SaveCommand();
        }

        private void btnSaveAs_Click_1(object sender, RoutedEventArgs e)
        {
            _myVM.SaveAsCommand();
        }

        private void btnSaveCloud_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click_1(object sender, RoutedEventArgs e)
        {
            _myVM.DeleteCommand();
        }
        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            _myVM.AddCommand();
        }

        private void btnAbout_Click_1(object sender, RoutedEventArgs e)
        {
            (new About()).ShowDialog();
        }
    }
}
