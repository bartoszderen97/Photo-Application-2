using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        private Size wndSize;
        private bool ifMaximizedWhileOpen, previousStateMinimized;

        // variables for extended part
        private int clickCounter = 0;
        private Point firstPoint, secondPoint;   // points to draw shapes
        private List<Rectangle> rects;
        private List<Ellipse> ellipses, myPoints;

        public MainWindow()
        {
            InitializeComponent();
            saveButton.IsEnabled = false;
            histogramButton.IsEnabled = false;
            hueSlider.IsEnabled = false;
            saturationSlider.IsEnabled = false;
            brightnessSlider.IsEnabled = false;
            contrastSlider.IsEnabled = false;
            selectionPowerChbx.IsEnabled = false;
            changeChbxState(false);
            rects = new List<Rectangle>();
            ellipses = new List<Ellipse>();
            myPoints = new List<Ellipse>();
        }

        private void checkIfHistogramOpen()
        {
            if (isHistogramOpen)
            {
                isHistogramOpen = false;
                histogramWindow.Close();
            }
        }

        public void deleteShapes()
        {
            
            canvas.Children.RemoveRange(1, canvas.Children.Count);
            rects.Clear();
            ellipses.Clear();
            myPoints.Clear();
            clickCounter = 0;
            firstPoint = new Point();
            secondPoint = new Point();
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
        
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                if (!previousStateMinimized)
                {
                    wndSize.Height = myWindow.ActualHeight;
                    wndSize.Width = myWindow.ActualWidth;
                }
                Size maxSize = wndSize;
                maxSize.Height = SystemParameters.MaximizedPrimaryScreenHeight;
                maxSize.Width = SystemParameters.MaximizedPrimaryScreenWidth;
                changeWindowSize(maxSize);
                previousStateMinimized = false;
            }
            else if (WindowState == WindowState.Normal)
            {
                if (wndSize.Height != 0 && wndSize.Width != 0)
                    changeWindowSize(wndSize);

                previousStateMinimized = false;
            }
            
            else if (WindowState == WindowState.Minimized)
            { 
                if (ifMaximizedWhileOpen)
                {
                    previousStateMinimized = true;
                    WindowState = WindowState.Maximized;
                }
                previousStateMinimized = true;
            }
            base.OnStateChanged(e);
        }
        
        private void changeWindowSize(Size size)
        {
            if(!previousStateMinimized)
                deleteShapes();
            if (size.Height > 753)
            {
                imageGrid.Width = size.Width - 220;
                imageGrid.Height = size.Height - 50;
                imageBorder.Width = size.Width - 220;
                image.Width = size.Width - 240;
                image.Height = size.Height - 70;
            }
            else
            {
                imageGrid.Width = size.Width - 400;
                imageGrid.Height = size.Height - 30;
                imageBorder.Width = size.Width - 400;
                image.Width = size.Width - 420;
                image.Height = size.Height - 50;
            }
            
            imageBorder.Height = image.ActualHeight + 20;
            imageBorder.Width = image.ActualWidth + 20;

            canvas.Width = image.ActualWidth;
            canvas.Height = image.ActualHeight;
        }
        private void myWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                wndSize.Width = e.NewSize.Width;
                wndSize.Height = e.NewSize.Height;
            }
            if (selectionPowerChbx.IsChecked == true)
            {
                selectionPowerChbx.IsChecked = false;
                changeChbxState(false);
                deleteShapes();
            }
            changeWindowSize(e.NewSize);
            if(myConversion!=null)
            myConversion.setIfWholeImg(true);
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
                    selectionPowerChbx.IsEnabled = true;
                    selectionPowerChbx.IsChecked = false;
                    changeChbxState(false);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    MessageBox.Show(this, "Wybrane zdjęcie jest uszkodzone! \nWybierz inne", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                }

                checkIfHistogramOpen();
                
                if (WindowState == WindowState.Normal)
                {
                    wndSize.Width = ActualWidth;
                    wndSize.Height = ActualHeight;
                    WindowState = WindowState.Maximized;
                    WindowState = WindowState.Normal;
                }
                if (WindowState == WindowState.Maximized)
                {
                    ifMaximizedWhileOpen = true;
                    WindowState = WindowState.Minimized;
                    WindowState = WindowState.Minimized;
                    ifMaximizedWhileOpen = false;
                }
                deleteShapes();
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
                myConversion.setSourceBitmap(bufferredPhoto);
                hueSlider.Value = 0;
                brightnessSlider.Value = 0;
                saturationSlider.Value = 1;
                contrastSlider.Value = 1;
                thresholdSlider.Value = 50;
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
                thresholdSlider.Value = 50;
                currentPhoto = orginalPhoto;
                deleteShapes();
                selectionPowerChbx.IsChecked = false;
                changeChbxState(false);
            }
            else
                showMessageBox();

            checkIfHistogramOpen();
        }

        private void mySliders_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (bufferredPhoto != null)
            {
                myConversion.setSourceBitmap(bufferredPhoto);
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

        private void selectionPowerChbx_Click(object sender, RoutedEventArgs e)
        {
            if (selectionPowerChbx.IsChecked == true)
            {
                changeChbxState(true);
                deleteShapes();
                myConversion.truncateSelectedPixels();
                myConversion.setIfWholeImg(false);
            }
            else
            {
                changeChbxState(false);
                deleteShapes();
                myConversion.truncateSelectedPixels();
                myConversion.setIfWholeImg(true);
            }
        }
        
        private void changeChbxState(bool ifEnabled)
        {
            rectSelectionRb.IsChecked = false;
            elipseSelectionRb.IsChecked = false;
            wandSelectionRb.IsChecked = false;
            rectSelectionRb.IsEnabled = ifEnabled;
            elipseSelectionRb.IsEnabled = ifEnabled;
            wandSelectionRb.IsEnabled = ifEnabled;
            selectedColorLabel.Background = Brushes.White;
        }

        private void SelectionRb_Click(object sender, RoutedEventArgs e)
        {
            if (rectSelectionRb.IsChecked == true || elipseSelectionRb.IsChecked == true)
                selectedColorLabel.Background = MyCustomShapes.borderColor;
            else
                selectedColorLabel.Background = new SolidColorBrush(Colors.White);
            clickCounter = 0;
        }

        private void randomColor_Click(object sender, RoutedEventArgs e)
        {
            MyCustomShapes.setRandomColor();
            selectedColorLabel.Background = MyCustomShapes.borderColor;
        }

        private void rangeSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private void rangeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyCustomShapes.range = rangeSlider.Value;
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

        private void canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectionPowerChbx.IsChecked == true && (rectSelectionRb.IsChecked == true || elipseSelectionRb.IsChecked == true)) 
            {
                if (clickCounter == 0)
                {
                    clickCounter++;
                    firstPoint = e.GetPosition((Canvas)sender);
                }
                else if (clickCounter == 1)
                {
                    // check which shape
                    secondPoint = e.GetPosition((Canvas)sender);
                    if (rectSelectionRb.IsChecked == true)
                    {
                        rects.Add(MyCustomShapes.drawMyRectangle(ref canvas, firstPoint, secondPoint));
                        myConversion.setSelectedPixels(MyCustomShapes.getSelectedPixelsArrayForRect(firstPoint, secondPoint, myConversion.getStride(), orginalPhoto.PixelHeight, canvas.ActualHeight, canvas.ActualWidth));
                    }
                    else if (elipseSelectionRb.IsChecked == true)
                    {
                        ellipses.Add(MyCustomShapes.drawMyEllipse(ref canvas, firstPoint, secondPoint));
                        myConversion.setSelectedPixels(MyCustomShapes.getSelectedPixelsArrayForEllipse(firstPoint, secondPoint, myConversion.getStride(), orginalPhoto.PixelHeight, canvas.ActualHeight, canvas.ActualWidth));
                        
                    }

                    clickCounter = 0;
                }
            }
            else if (selectionPowerChbx.IsChecked == true && wandSelectionRb.IsChecked == true)
            {
                double[] hsvpixels = myConversion.getPixelDataHSV();
                byte[] rgbpixels = myConversion.getPixelDataRGB();
                if (hsvpixels == null)
                {
                    myConversion.convertRGBtoHSV();
                    hsvpixels = myConversion.getPixelDataHSV();
                }
                MyCustomShapes.range = rangeSlider.Value;
                
                selectedColorLabel.Background = MyCustomShapes.borderColor;
                
                myConversion.setSelectedPixels(MyCustomShapes.getSelectedPixelsArrayForWand(e.GetPosition((Canvas)sender), hsvpixels, myConversion.getStride(), orginalPhoto.PixelHeight, canvas.ActualHeight, canvas.ActualWidth));

                int s = myConversion.getStride();
                bool[] borderPixels = WandHelper.getBorder();

                myPoints = new List<Ellipse>();

                for (int y = 0; y < currentPhoto.PixelHeight; y++)
                {
                    int yIndex = y * s;
                    for (int x = 0; x < s; x += 4)
                    {
                        if (borderPixels[yIndex + x]) 
                        {
                            int dotSize = 3;
                            double w = ((double)x / currentPhoto.PixelWidth / 4) * canvas.ActualWidth, h = ((double)y / currentPhoto.PixelHeight) * canvas.ActualHeight;
                            Ellipse currentDot = new Ellipse();
                            currentDot.Stroke = new SolidColorBrush(Colors.Green);
                            currentDot.StrokeThickness = 3;
                            currentDot.Height = dotSize;
                            currentDot.Width = dotSize;
                            currentDot.Fill = new SolidColorBrush(Colors.Green);
                            currentDot.Margin = new Thickness(w , h, 0, 0);
                            myPoints.Add(currentDot);
                            canvas.Children.Add(currentDot);
                        }
                    }
                }
                
                
            }
            
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