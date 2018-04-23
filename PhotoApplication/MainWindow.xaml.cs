using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string orginalPhotoName;
        private BitmapSource orginalPhoto, currentPhoto, bufferredPhoto;
        private AllConversions myConversion;
        private MyHistogramWindow histogramWindow;
        public static bool isHistogramOpen = false;

        public MainWindow()
        {
            InitializeComponent();
            saveButton.IsEnabled = false;
            histogramButton.IsEnabled = false;
            hueSlider.IsEnabled = false;
            saturationSlider.IsEnabled = false;
            brightnessSlider.IsEnabled = false;
            contrastSlider.IsEnabled = false;
            WindowState = WindowState.Maximized;
        }

        private void checkIfHistogramOpen()
        {
            if (isHistogramOpen)
            {
                isHistogramOpen = false;
                histogramWindow.Close();
            }
        }
        private void checkIfPixelsAreSet()
        {
            if (myConversion.getPixelDataRGB() == null)
                myConversion.setPixelDataRGB();
        }
        private void showMessageBox()
        {
            MessageBox.Show(this, "Najpierw musisz wybrać zdjęcie!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None);
        }
        private void setNewImage()
        {
            currentPhoto = BitmapSource.Create(orginalPhoto.PixelWidth, orginalPhoto.PixelHeight, orginalPhoto.DpiX, orginalPhoto.DpiY, PixelFormats.Bgr32, null, myConversion.getPixelDataRGB(), myConversion.getStride());
            image.Source = currentPhoto;
        }

        private void myWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window wnd = (Window)sender;
            imageGrid.Width = e.NewSize.Width - 200;
            image.Width = e.NewSize.Width - 200;
            if (e.NewSize.Height < 629)
            {
                imageGrid.Width = e.NewSize.Width - 380;
                image.Width = e.NewSize.Width - 380;
            }
            if (wnd.WindowState== WindowState.Maximized)
            {
                if (SystemParameters.PrimaryScreenHeight > 629)
                {
                    imageGrid.Width = e.NewSize.Width - 200;
                    image.Width = e.NewSize.Width - 200;
                }
                else
                {
                    imageGrid.Width = e.NewSize.Width - 380;
                    image.Width = e.NewSize.Width - 380;
                }
                
            }
        }
        private void myWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            checkIfHistogramOpen();
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
                try
                {
                    orginalPhoto = new BitmapImage(new Uri(orginalPhotoName));
                    currentPhoto = orginalPhoto;
                    bufferredPhoto = currentPhoto;
                    image.Source = orginalPhoto;
                    myConversion = new AllConversions(orginalPhoto);
                    hueSlider.Value = 0;
                    brightnessSlider.Value = 0;
                    saturationSlider.Value = 1;
                    contrastSlider.Value = 1;
                    thresholdSlider.Value = 50;
                    saveButton.IsEnabled = true;
                    histogramButton.IsEnabled = true;
                    hueSlider.IsEnabled = true;
                    saturationSlider.IsEnabled = true;
                    brightnessSlider.IsEnabled = true;
                    contrastSlider.IsEnabled = true;
                }
                catch(Exception ex)
                {

                    Debug.Print(ex.Message);
                    MessageBox.Show(this, "Wybrane zdjęcie jest uszkodzone! \nWybierz inne", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                }

                checkIfHistogramOpen();
            }
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
            checkIfHistogramOpen();
        }

        private void apply1button_Click(object sender, RoutedEventArgs e)
        {
            if (myConversion != null)
            {
                checkIfPixelsAreSet();
                bufferredPhoto = currentPhoto;
                myConversion = new AllConversions(bufferredPhoto);
                hueSlider.Value = 0;
                brightnessSlider.Value = 0;
                saturationSlider.Value = 1;
                contrastSlider.Value = 1;
            }
            else
                showMessageBox();
            checkIfHistogramOpen();
        }
        private void reset1button_Click(object sender, RoutedEventArgs e)
        {
            if (orginalPhoto != null)
            {
                image.Source = orginalPhoto;
                bufferredPhoto = orginalPhoto;
                currentPhoto = orginalPhoto;
                myConversion = new AllConversions(orginalPhoto);
                hueSlider.Value = 0;
                brightnessSlider.Value = 0;
                saturationSlider.Value = 1;
                contrastSlider.Value = 1;
                currentPhoto = orginalPhoto;
            }
            else
                showMessageBox();
            checkIfHistogramOpen();
        }
      
        private void mySliders_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (bufferredPhoto != null)
            {
                myConversion = new AllConversions(bufferredPhoto);
                myConversion.doConversion1(hueSlider.Value, saturationSlider.Value, brightnessSlider.Value);
                myConversion.doConversion2(contrastSlider.Value);
                setNewImage();
            }
            checkIfHistogramOpen();
        }
        
        private void conversion3Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (myConversion != null)
            {
                checkIfPixelsAreSet();
                switch (btn.Name)
                {
                    case "blurButton":
                        myConversion.doConversion3(1);
                        break;
                    case "sharpenButton":
                        myConversion.doConversion3(2);
                        break;
                    case "edgeButton":
                        myConversion.doConversion3(3);
                        break;
                    default:
                        break;
                }
                setNewImage();
            }
            else
                showMessageBox();
            checkIfHistogramOpen();
        }

        private void negativeButton_Click(object sender, RoutedEventArgs e)
        {
            if (myConversion != null)
            {
                checkIfPixelsAreSet();
                myConversion.doNegative();
                setNewImage();
            }
            else
                showMessageBox();
            checkIfHistogramOpen();
        }

        private void thresholdButton_Click(object sender, RoutedEventArgs e)
        {
            if (myConversion != null)
            {
                checkIfPixelsAreSet();
                myConversion.doThresholding(thresholdSlider.Value);
                setNewImage();
            }
            else
                showMessageBox();
            checkIfHistogramOpen();
        }
        private void histogramButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isHistogramOpen)
            {
                isHistogramOpen = true;
                histogramWindow = new MyHistogramWindow();

                if (myConversion != null)                
                    checkIfPixelsAreSet();                
                else
                    myConversion = new AllConversions(currentPhoto);
                    
                histogramWindow.setMyPixelData(myConversion.getPixelDataRGB(), currentPhoto);
                histogramWindow.drawHistogram();
                histogramWindow.Show();
            }
        }
    }
}