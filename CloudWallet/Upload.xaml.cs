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
    }
}
