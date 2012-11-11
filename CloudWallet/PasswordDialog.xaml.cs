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
using System.Windows.Shapes;

namespace CloudWallet
{
    /// <summary>
    /// Interaction logic for PasswordDialog.xaml
    /// </summary>
    public partial class PasswordDialog : Window
    {
        public static bool? ShowPasswordDialog(string password)
        {
            PasswordDialog dialog = new PasswordDialog(password);
            return dialog.ShowDialog();
        }

        public string Password { get; set; }

        public PasswordDialog() : this(string.Empty) { }
        public PasswordDialog(string password)
        {
            Password = password;
            InitializeComponent();

            this.passwordBox.PasswordChanged += passwordBox_PasswordChanged;
            if (!string.IsNullOrEmpty(password))
                passwordBox.Password = password;

            this.passwordBox.Focus();
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            btnOK.IsEnabled = !string.IsNullOrWhiteSpace(passwordBox.Password);
            Password = passwordBox.Password;
        }

        private void btnOK_Click_1(object sender, RoutedEventArgs e)
        {
            OkCommand();
        }
        private void btnCancel_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void PasswordBox_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OkCommand();
        }

        private void OkCommand()
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
