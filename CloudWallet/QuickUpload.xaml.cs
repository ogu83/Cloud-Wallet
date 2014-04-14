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
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CloudWallet
{
    /// <summary>
    /// Interaction logic for QuickUpload.xaml
    /// </summary>
    public partial class QuickUpload : Window
    {
        public QuickUpload()
        {
            InitializeComponent();
        }
 

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (File.Exists(UploadFile.Text) && (UploadFile.Text.ToLowerInvariant().EndsWith(".wlt")
                                                || UploadFile.Text.ToLowerInvariant().EndsWith(".cw")))
                    StatsShape.Fill = new SolidColorBrush(Colors.Green);
                else
                    StatsShape.Fill = new SolidColorBrush(Colors.Red);
            }
            catch (Exception) { }
        }

        private void Uploadbtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(EmailTo.Text);
           
        }

        private void OpenFolder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var Folder = new Microsoft.Win32.OpenFileDialog();

            Folder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Folder.Title = "Cloud Wallet - File Transmission";
            Folder.Filter = "Wallet File (.wlt)|*.wlt|Packaged Wallets (.cw)|*.cw";
            var Result = Folder.ShowDialog();

          
            UploadFile.Text = Result.Value == true ? Folder.FileName :"";
        }

        public void Setprogress(int evla)
        {
            Progressbar1.Value = evla;
        }
        public void UploadProgressCallback(object sender, System.Net.UploadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.

            Setprogress(e.ProgressPercentage);

        }

        private void UploadProgressComplete(object sender, System.Net.UploadFileCompletedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.

           EmailTo.Text =System.Text.Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
           

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(UploadFile.Text) && (UploadFile.Text.ToLowerInvariant().EndsWith(".wlt")
                                              || UploadFile.Text.ToLowerInvariant().EndsWith(".cw")))
            {

                Random ExtendedTimeStampForPhpUniqueIDentity = new Random();
                int TimeStamp = ExtendedTimeStampForPhpUniqueIDentity.Next(11498, 99328);

               
                string Extended = Convert.ToString(TimeStamp);

                System.Net.WebClient Client = new System.Net.WebClient();
                Client.UploadProgressChanged += new System.Net.UploadProgressChangedEventHandler(UploadProgressCallback);

                Client.UploadFileCompleted += new System.Net.UploadFileCompletedEventHandler(UploadProgressComplete);



                Uri ParemURI = new Uri("http://fusionservers.x10.mx/CloudWallet/Upload.php?RandomInt=" + Extended);

                Client.UploadFileAsync(ParemURI, "POST", UploadFile.Text);

                  
            }
        }
    }
}
