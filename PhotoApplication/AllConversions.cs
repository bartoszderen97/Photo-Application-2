using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PhotoApplication
{
    class AllConversions
    {
        protected BitmapSource mySourceBitmap;
        protected int width, height, rawStride;
        protected byte[] pixelDataRGB;
        protected double[] pixelDataHSV, pixelDataConversion3RGB;
        private int[] blurArray = { 1, 2, 1, 2, 4, 2, 1, 2, 1 },
            sharpenArray = { 0, -2, 0, -2, 11, -2, 0, -2, 0 },
            edgeArray = { -1, -1, -1, -1, 8, -1, -1, -1, -1 }; 

        public BitmapSource getConvertedImage() { return mySourceBitmap; }
        public int getStride() { return rawStride; }
        public byte[] getPixelDataRGB()
        {
            if (pixelDataRGB != null)
                return pixelDataRGB;
            else return null;
        }
        public double[] getPixelDataHSV()
        {
            if (pixelDataHSV != null)
                return pixelDataHSV;
            else return null;
        }

        public void setPixelDataRGB() { mySourceBitmap.CopyPixels(pixelDataRGB, rawStride, 0); /* possible ArgumentOutOfRangeException */}

        public AllConversions(BitmapSource src)
        {
            mySourceBitmap = src;
            width = src.PixelWidth;
            height = src.PixelHeight;
            try
            {
                rawStride = (width * src.Format.BitsPerPixel + 7) / 8;
                pixelDataRGB = new byte[rawStride * height + 4]; /// possible OutOfMemoryException 
                setPixelDataRGB();
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
                MessageBox.Show("Wybrane zdjęcie jest zbyt duże! \nWybierz inne", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }

        }

        public void convertHSVtoRGB()
        {
            double r, f, a, b, c;
            
            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += (mySourceBitmap.Format.BitsPerPixel/8))
                {
                    r = pixelDataHSV[x + yIndex] / 60;
                    f = r - Math.Floor(r);
                    a = (pixelDataHSV[x + yIndex + 2] * (1 - pixelDataHSV[x + yIndex + 1]));
                    b = (pixelDataHSV[x + yIndex + 2] * (1 - (pixelDataHSV[x + yIndex + 1] * f)));
                    c = (pixelDataHSV[x + yIndex + 2] * (1 - (pixelDataHSV[x + yIndex + 1] * (1 - f))));
                    int d = Convert.ToInt32(Math.Floor(r));

                    switch(d)
                    {
                        case 0:
                            pixelDataRGB[x + yIndex + 2] = (byte)pixelDataHSV[x + yIndex + 2];
                            pixelDataRGB[x + yIndex + 1] = (byte)c;
                            pixelDataRGB[x + yIndex] = (byte)a;
                            break;
                        case 1:
                            pixelDataRGB[x + yIndex + 2] = (byte)b;
                            pixelDataRGB[x + yIndex + 1] = (byte)pixelDataHSV[x + yIndex + 2];
                            pixelDataRGB[x + yIndex] = (byte)a;
                            break;
                        case 2:
                            pixelDataRGB[x + yIndex + 2] = (byte)a;
                            pixelDataRGB[x + yIndex + 1] = (byte)pixelDataHSV[x + yIndex + 2];
                            pixelDataRGB[x + yIndex] = (byte)c;
                            break;
                        case 3:
                            pixelDataRGB[x + yIndex + 2] = (byte)a;
                            pixelDataRGB[x + yIndex + 1] = (byte)b;
                            pixelDataRGB[x + yIndex] = (byte)pixelDataHSV[x + yIndex + 2];
                            break;
                        case 4:
                            pixelDataRGB[x + yIndex + 2] = (byte)c;
                            pixelDataRGB[x + yIndex + 1] = (byte)a;
                            pixelDataRGB[x + yIndex] = (byte)pixelDataHSV[x + yIndex + 2];
                            break;
                        case 5:
                            pixelDataRGB[x + yIndex + 2] = (byte)pixelDataHSV[x + yIndex + 2];
                            pixelDataRGB[x + yIndex + 1] = (byte)a;
                            pixelDataRGB[x + yIndex] = (byte)b;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void convertRGBtoHSV()
        {
            pixelDataHSV = new double[rawStride * height + 4];  /// possible OutOfMemoryException 

            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += (mySourceBitmap.Format.BitsPerPixel / 8))
                {
                    byte min = Math.Min(pixelDataRGB[x+yIndex + 2], Math.Min(pixelDataRGB[x + yIndex + 1], pixelDataRGB[x + yIndex]));
                    byte max = Math.Max(pixelDataRGB[x + yIndex + 2], Math.Max(pixelDataRGB[x + yIndex + 1], pixelDataRGB[x + yIndex]));
                    pixelDataHSV[x + yIndex + 2] = max;

                    double r = max - min;

                    if (max != 0)
                        pixelDataHSV[x + yIndex + 1] = r / max;
                    else
                        pixelDataHSV[x + yIndex + 1] = 0;

                    if (max == pixelDataRGB[x + yIndex + 1])
                    {
                        if (r != 0)
                            pixelDataHSV[x + yIndex] = (((60 * (pixelDataRGB[x + yIndex] - pixelDataRGB[x + yIndex + 2])) / r) + 120);
                        else
                            pixelDataHSV[x + yIndex] = 0;
                    }
                    else if (max == pixelDataRGB[x + yIndex])
                    {
                        if (r != 0)
                            pixelDataHSV[x + yIndex] = (((60 * (pixelDataRGB[x + yIndex + 2] - pixelDataRGB[x + yIndex + 1])) / r) + 240);
                        else
                            pixelDataHSV[x + yIndex] = 0;
                    }
                    else
                    {
                        if (r != 0)
                            pixelDataHSV[x + yIndex] = ((60 * (pixelDataRGB[x + yIndex + 1] - pixelDataRGB[x + yIndex])) / r);
                        else
                            pixelDataHSV[x + yIndex] = 0;
                    }

                    if (pixelDataHSV[x + yIndex] < 0)
                        pixelDataHSV[x + yIndex] += 360;
                }
            }
        }
        
        public void doConversion1(double hue, double saturation, double brightness)
        {
            convertRGBtoHSV();

            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += (mySourceBitmap.Format.BitsPerPixel / 8))
                {
                    pixelDataHSV[x + yIndex] *= hue;
                    if (pixelDataHSV[x + yIndex] > 360)
                        pixelDataHSV[x + yIndex] = 360;
                    pixelDataHSV[x + yIndex + 1] *= saturation;
                    if (pixelDataHSV[x + yIndex + 1] > 1)
                        pixelDataHSV[x + yIndex + 1] = 1;
                    pixelDataHSV[x + yIndex + 2] *= brightness;
                    if (pixelDataHSV[x + yIndex + 2] > 255)
                        pixelDataHSV[x + yIndex + 2] = 255;
                }

            }
            convertHSVtoRGB();
        }
        public void doConversion2(double param)
        {
            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += (mySourceBitmap.Format.BitsPerPixel / 8))
                {
                    double B = pixelDataRGB[x + yIndex] - 128;
                    double G = pixelDataRGB[x + yIndex + 1] - 128;
                    double R = pixelDataRGB[x + yIndex + 2] - 128;
                    B *= param;
                    G *= param;
                    R *= param;

                    if (R + 128 > 255)
                        R = 255;
                    else if (R + 128 < 0)
                        R = 0;
                    else
                        R -= 128;

                    if (G + 128 > 255)
                        G = 255;
                    else if (G + 128 < 0)
                        G = 0;
                    else
                        G -= 128;

                    if (B + 128 > 255)
                        B = 255;
                    else if (B + 128 < 0)
                        B = 0;
                    else
                        B -= 128;

                    pixelDataRGB[x + yIndex] = (byte)B;
                    pixelDataRGB[x + yIndex + 1] = (byte)G;
                    pixelDataRGB[x + yIndex + 2] = (byte)R;
                }
            }
        }
        public void doConversion3(int choice)
        {
            pixelDataConversion3RGB = new double[rawStride * height + 4];

            switch (choice)
            {
                case 1:
                    useFilter(blurArray);
                    break;
                case 2:
                    useFilter(sharpenArray);
                    break;
                case 3:
                    useFilter(edgeArray);
                    break;
                default:
                    break;
            }
        }
        
        private void useFilter(int[] filter)
        {
            double arraySum = 0;
            for (int i = 0; i < filter.Length; i++)
                arraySum += filter[i];
            if (arraySum == 0)
                arraySum = 1;
            for (int y = 1; y < height - 1; y++)
            {
                int yIndex = y * rawStride, yIndexLower = (y - 1) * rawStride, yIndexUpper = (y + 1) * rawStride;
                
                for (int x = 4; x < rawStride - 4; x += (mySourceBitmap.Format.BitsPerPixel / 8))
                {
                    int sumB = 0, sumG = 0, sumR = 0;
                    sumB += (pixelDataRGB[x - 4 + yIndexLower] * filter[0]) + (pixelDataRGB[x + yIndexLower] * filter[1]) + (pixelDataRGB[x + 4 + yIndexLower] * filter[2]);
                    sumB += (pixelDataRGB[x - 4 + yIndex] * filter[3]) + (pixelDataRGB[x + yIndex] * filter[4]) + (pixelDataRGB[x + 4 + yIndex] * filter[5]);
                    sumB += (pixelDataRGB[x - 4 + yIndexUpper] * filter[6]) + (pixelDataRGB[x + yIndexUpper] * filter[7]) + (pixelDataRGB[x + 4 + yIndexUpper] * filter[8]);

                    sumG += (pixelDataRGB[x - 3 + yIndexLower] * filter[0]) + (pixelDataRGB[x + 1 + yIndexLower] * filter[1]) + (pixelDataRGB[x + 5 + yIndexLower] * filter[2]);
                    sumG += (pixelDataRGB[x - 3 + yIndex] * filter[3]) + (pixelDataRGB[x + 1 + yIndex] * filter[4]) + (pixelDataRGB[x + 5 + yIndex] * filter[5]);
                    sumG += (pixelDataRGB[x - 3 + yIndexUpper] * filter[6]) + (pixelDataRGB[x + 1 + yIndexUpper] * filter[7]) + (pixelDataRGB[x + 5 + yIndexUpper] * filter[8]);

                    sumR += (pixelDataRGB[x - 2 + yIndexLower] * filter[0]) + (pixelDataRGB[x + 2 + yIndexLower] * filter[1]) + (pixelDataRGB[x + 6 + yIndexLower] * filter[2]);
                    sumR += (pixelDataRGB[x - 2 + yIndex] * filter[3]) + (pixelDataRGB[x + 2 + yIndex] * filter[4]) + (pixelDataRGB[x + 6 + yIndex] * filter[5]);
                    sumR += (pixelDataRGB[x - 2 + yIndexUpper] * filter[6]) + (pixelDataRGB[x + 2 + yIndexUpper] * filter[7]) + (pixelDataRGB[x + 6 + yIndexUpper] * filter[8]);

                    double srednia = sumB/arraySum;

                    if (srednia < 0)
                        pixelDataConversion3RGB[x + yIndex] = 0;
                    else if (srednia > 255)
                        pixelDataConversion3RGB[x + yIndex] = 255;
                    else
                        pixelDataConversion3RGB[x + yIndex] = srednia;

                    srednia = sumG / arraySum;

                    if (srednia < 0)
                        pixelDataConversion3RGB[x + 1 + yIndex] = 0;
                    else if (srednia > 255)
                        pixelDataConversion3RGB[x + 1 + yIndex] = 255;
                    else
                        pixelDataConversion3RGB[x + 1 + yIndex] = srednia;
                    
                    srednia = sumR / arraySum;

                    if (srednia < 0)
                        pixelDataConversion3RGB[x + 2 + yIndex] = 0;
                    else if (srednia > 255)
                        pixelDataConversion3RGB[x + 2 + yIndex] = 255;
                    else
                        pixelDataConversion3RGB[x + 2 + yIndex] = srednia;

                }
            }
            restorePixels();
        }
        private void restorePixels()
        {
            for (int y = 1; y < height - 1; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 4; x < rawStride - 4; x += (mySourceBitmap.Format.BitsPerPixel / 8))
                {
                    pixelDataRGB[x + yIndex] = (byte)(pixelDataConversion3RGB[x + yIndex]);
                    pixelDataRGB[x + yIndex + 1] = (byte)(pixelDataConversion3RGB[x + yIndex + 1]);
                    pixelDataRGB[x + yIndex + 2] = (byte)(pixelDataConversion3RGB[x + yIndex + 2]);
                }
            }
        }
        public void doNegative()
        {
            byte k = 255;

            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += (mySourceBitmap.Format.BitsPerPixel / 8))
                {
                    
                    pixelDataRGB[x + yIndex + 2] = (byte)(k - pixelDataRGB[x + yIndex + 2]);
                    pixelDataRGB[x + yIndex + 1] = (byte)(k - pixelDataRGB[x + yIndex + 1]);
                    pixelDataRGB[x + yIndex] = (byte)(k - pixelDataRGB[x + yIndex]);
                    
                }
            }
        }
        public void doThresholding(double param)
        {
            double brightness, threshold = 255 * param / 100;
            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += (mySourceBitmap.Format.BitsPerPixel / 8))
                {
                    double B = pixelDataRGB[x + yIndex] * 0.114;
                    double G = pixelDataRGB[x + yIndex + 1] * 0.587;
                    double R = pixelDataRGB[x + yIndex + 2] * 0.299;
                    brightness = R + B + G;
                    if (brightness < threshold)
                    {
                        pixelDataRGB[x + yIndex + 2] = 0;
                        pixelDataRGB[x + yIndex + 1] = 0;
                        pixelDataRGB[x + yIndex] = 0;
                    }
                    else
                    {
                        pixelDataRGB[x + yIndex + 2] = 255;
                        pixelDataRGB[x + yIndex + 1] = 255;
                        pixelDataRGB[x + yIndex] = 255;
                    }
                }
            }
        }
    }
}