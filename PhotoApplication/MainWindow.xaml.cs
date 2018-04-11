using Microsoft.Win32;
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

namespace PhotoApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string orginalPhotoName;
        private BitmapSource orginalPhoto, currentPhoto;
        private AllConversions myConversion;

        public MainWindow()
        {
            InitializeComponent();
            saveButton.IsEnabled = false;
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp";
            openFileDialog.Title = "Otwórz wybrany plik w aplikacji PhotoApplication";
                
            if (openFileDialog.ShowDialog() == true)
            {
                orginalPhotoName = openFileDialog.FileName;
                
                orginalPhoto = new BitmapImage(new Uri(orginalPhotoName));
                currentPhoto = orginalPhoto;
                image.Source = orginalPhoto;
                myConversion = new AllConversions(orginalPhoto);
                barwaSlider.Value = 1;
                jasnoscSlider.Value = 1;
                nasycenieSlider.Value = 1;
                kontrastSlider.Value = 1;
                progSlider.Value = 50;
            }
            saveButton.IsEnabled = true;
        }
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Plik JPG|*.jpg|Plik PNG|*.png|Plik BMP|*.bmp";
            saveFileDialog1.Title = "Zapisz obecny obraz na dysku komputera";
            saveFileDialog1.ShowDialog();
            
            if (saveFileDialog1.FileName != "")
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(currentPhoto));
                using (var stream = saveFileDialog1.OpenFile())
                {
                    encoder.Save(stream);
                }
            }
        }
        private void zastosuj1button_Click(object sender, RoutedEventArgs e)
        {
            if (myConversion != null)
            {
                myConversion.doConversion1(barwaSlider.Value, nasycenieSlider.Value, jasnoscSlider.Value);
                currentPhoto = BitmapSource.Create(orginalPhoto.PixelWidth, orginalPhoto.PixelHeight, orginalPhoto.DpiX, orginalPhoto.DpiY, PixelFormats.Bgr32, null, myConversion.getPixelData(), myConversion.getStride());
                /* possible ArgumentOutOfRangeException */
            image.Source = currentPhoto;
            }
        }
        private void zresetuj1button_Click(object sender, RoutedEventArgs e)
        {
            image.Source = orginalPhoto;
            myConversion = new AllConversions(orginalPhoto);
            barwaSlider.Value = 1;
            jasnoscSlider.Value = 1;
            nasycenieSlider.Value = 1;
            currentPhoto = orginalPhoto;
        }
        private void zastosuj2button_Click(object sender, RoutedEventArgs e)
        {
            if (myConversion != null)
            {
                myConversion.doConversion2(kontrastSlider.Value);
                currentPhoto = BitmapSource.Create(orginalPhoto.PixelWidth, orginalPhoto.PixelHeight, orginalPhoto.DpiX, orginalPhoto.DpiY, PixelFormats.Bgr32, null, myConversion.getPixelData(), myConversion.getStride());
                /* possible ArgumentOutOfRangeException */
                image.Source = currentPhoto;
            }
        }
        private void zresetuj2button_Click(object sender, RoutedEventArgs e)
        {
            image.Source = orginalPhoto;
            myConversion = new AllConversions(orginalPhoto);
            kontrastSlider.Value = 1;
            currentPhoto = orginalPhoto;
        }

        private void negatywButton_Click(object sender, RoutedEventArgs e)
        {
            if (myConversion != null)
            {
                myConversion.doNegatyw();
                currentPhoto = BitmapSource.Create(orginalPhoto.PixelWidth, orginalPhoto.PixelHeight, orginalPhoto.DpiX, orginalPhoto.DpiY, PixelFormats.Bgr32, null, myConversion.getPixelData(), myConversion.getStride());
                /* possible ArgumentOutOfRangeException */
                image.Source = currentPhoto;
            }
        }

        private void progButton_Click(object sender, RoutedEventArgs e)
        {
            if (myConversion != null)
            {
                myConversion.doProgowanie(progSlider.Value);
                currentPhoto = BitmapSource.Create(orginalPhoto.PixelWidth, orginalPhoto.PixelHeight, orginalPhoto.DpiX, orginalPhoto.DpiY, PixelFormats.Bgr32, null, myConversion.getPixelData(), myConversion.getStride());
                /* possible ArgumentOutOfRangeException */
                image.Source = currentPhoto;
            }
        }

    }
}