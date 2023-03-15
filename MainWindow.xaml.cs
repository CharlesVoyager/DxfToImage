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
using System.IO;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;

namespace DxfToImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSelectDxfFile_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "DXF";            // Default file name
            dialog.DefaultExt = ".dxf";         // Default file extension
            dialog.Filter = "DXF (.dxf)|*.dxf"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                FileNameLabel.Content = dialog.FileName;

                // Layer0.dxf ==> Layer0.png
                DxfToImg dxfToImg = new DxfToImg();
                dxfToImg.Convert(   FileNameLabel.Content.ToString(), 
                                    System.Drawing.Imaging.ImageFormat.Png, 
                                    System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar);

                string pngFile = System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + 
                    System.IO.Path.GetFileNameWithoutExtension(FileNameLabel.Content.ToString()) + ".png";

                // Display the image
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(pngFile);
                bitmap.EndInit();

                ImageViewer.Source = bitmap;
            }
        }
    }
}
