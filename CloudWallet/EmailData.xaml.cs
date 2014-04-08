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
using System.IO;
using System.Threading;


namespace CloudWallet
{
    /// <summary>
    /// Interaction logic for Upload.xaml
    /// </summary>
    public partial class Upload : Window
    {
        public Upload()
        {
            InitializeComponent();
            UploadFile.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" ;
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
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
        bool BlackListed(string Email)
        {
            bool  Clean = true ;
            List<string> InValidChar = new List<string> { "!","\"","£","%","^","&","*","(",")","-","_","=","+","`","¬",
                                                          "\\","|" ,"[","{","]","}",";",":","'","@","#","~",",","<",".",
                                                          ">","/","?"};
            foreach (string Char in InValidChar)
            {
                if (EmailTo.Text.ToLowerInvariant().EndsWith(Char.ToLowerInvariant ()))
                {
                    Clean = false;
                }
                if (EmailTo.Text.ToLowerInvariant().StartsWith(Char.ToLowerInvariant()))
                {
                    Clean = false;
                }
            }
            return Clean;
        }
        private void Uploadbtn_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(UploadFile.Text) && (UploadFile.Text.ToLowerInvariant().EndsWith(".wlt")
                                               || UploadFile.Text.ToLowerInvariant().EndsWith(".cw")))
            {
                if (IsValidEmail(EmailTo.Text) == true &&
                    (!EmailTo.Text.EndsWith("@") 
                    && EmailTo.Text.Contains(".")
                    && !EmailTo.Text.EndsWith(".")
                    && EmailTo.Text.Contains("@"))
                    && BlackListed(EmailTo.Text) == true
                    )
                {
                    Lbstats.Content = "Stats: Please Wait..";
                    Random ExtendedTimeStampForPhpUniqueIDentity = new Random();
                    int TimeStamp = ExtendedTimeStampForPhpUniqueIDentity.Next(11498, 99328);

                    string SendTo = EmailTo.Text, From = EmailFrom.Text;
                    string Extended = Convert.ToString(TimeStamp);

                    System.Net.WebClient Client = new System.Net.WebClient();
                    Client.Headers.Add("Content-Type", "binary/octet-stream");
                    
                    String ParemURI = "http://fusionservers.x10.mx/CloudWallet/Upload.php?from="+From+"&to="+SendTo+"&RandomInt="+Extended;
                 
                 byte[] result = Client.UploadFile(ParemURI, "POST", UploadFile.Text);


                    if( System.Text.Encoding.UTF8.GetString(result, 0, result.Length).StartsWith("http://"))
                    {
                       Lbstats.Content = "Stats: File Uploaded, Message Sent";

                    }
                    else
                    {
                        Lbstats.Content = "Fault -Report to Cloud Wallet: " + System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
                    }
                }
                else
                    MessageBox.Show("Can not mail to an invalid email");
            }
            else
                MessageBox.Show("This is not a valid Cloud Wallet file.");
        }

        private void OpenFolder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var Folder = new Microsoft.Win32.OpenFileDialog();
         
            Folder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Folder.Title = "Cloud Wallet - File Transmission";
            Folder.Filter = "Wallet File (.wlt)|*.wlt|Packaged Wallets (.cw)|*.cw";
          var Result =  Folder.ShowDialog();

            if(Result.Value == true)
            { UploadFile.Text  = Folder.FileName; }
        }

    

       
    }
}
