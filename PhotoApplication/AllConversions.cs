using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoApplication
{
    class AllConversions
    {
        protected BitmapSource mySourceBitmap;
        protected int width, height, rawStride;
        protected byte[] pixelDataRGB;
        protected double[] pixelDataHSV, pixelDataConversion1RGB;
        private double[] rozmycieMacierz = { 1, 2, 1, 2, 4, 2, 1, 2, 1 },
            wyostrzenieMacierz = { 0, -2, 0, -2, 11, -2, 0, -2, 9 },
            krawedzieMacierz = { -1, -1, -1, -1, 8, -1, -1, -1, -1 }; 

        public BitmapSource getConvertedImage() { return mySourceBitmap; }
        public int getStride() { return rawStride; }
        public byte[] getPixelData() { return pixelDataRGB; }

        private void setPixelDataRGB() { mySourceBitmap.CopyPixels(pixelDataRGB, rawStride, 0); /* possible ArgumentOutOfRangeException */}

        public AllConversions(BitmapSource src)
        {
            mySourceBitmap = src;
            width = src.PixelWidth;
            height = src.PixelHeight;
            rawStride = (width * src.Format.BitsPerPixel + 7) / 8;
            pixelDataRGB = new byte[rawStride * height + 4]; /// possible OutOfMemoryException 
            setPixelDataRGB();
            
        }

        protected void convertHSVtoRGB()
        {
            double r, f, a, b, c;
            pixelDataHSV = new double[rawStride * height + 4];  /// possible OutOfMemoryException 

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

        protected void convertRGBtoHSV()
        {
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
        
        public void doConversion1(double barwa, double nasycenie, double jasnosc)
        {
            convertRGBtoHSV();

            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += (mySourceBitmap.Format.BitsPerPixel / 8))
                {
                    pixelDataHSV[x + yIndex] *= barwa;
                    if (pixelDataHSV[x + yIndex] > 360)
                        pixelDataHSV[x + yIndex] = 360;
                    pixelDataHSV[x + yIndex + 1] *= nasycenie;
                    if (pixelDataHSV[x + yIndex + 1] > 1)
                        pixelDataHSV[x + yIndex + 1] = 1;
                    pixelDataHSV[x + yIndex + 2] *= jasnosc;
                    if (pixelDataHSV[x + yIndex + 2] > 255)
                        pixelDataHSV[x + yIndex + 2] = 255;
                }

            }
            convertHSVtoRGB();
        }
        public void doConversion2(double value)
        {
            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += (mySourceBitmap.Format.BitsPerPixel / 8))
                {
                    double B = pixelDataRGB[x + yIndex] - 128;
                    double G = pixelDataRGB[x + yIndex + 1] - 128;
                    double R = pixelDataRGB[x + yIndex + 2] - 128;
                    B *= value;
                    G *= value;
                    R *= value;

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
            setPixelsAtBorder();

            switch (choice)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        }
        private void setPixelsAtBorder()
        {
            pixelDataConversion1RGB = new double[rawStride * height + 4];
            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                pixelDataConversion1RGB[yIndex + 2] =  pixelDataRGB[yIndex + 2];
                pixelDataConversion1RGB[yIndex + 1] = pixelDataRGB[yIndex + 1];
                pixelDataConversion1RGB[yIndex] = pixelDataRGB[yIndex];
                pixelDataConversion1RGB[yIndex + rawStride - 2] = pixelDataRGB[yIndex + rawStride - 2];
                pixelDataConversion1RGB[yIndex + rawStride - 3] = pixelDataRGB[yIndex + rawStride - 3];
                pixelDataConversion1RGB[yIndex + rawStride - 4] = pixelDataRGB[yIndex + rawStride - 4];
            }
            for (int x = 0; x < rawStride; x += (mySourceBitmap.Format.BitsPerPixel / 8))
            {
                int yIndex = (height - 1) * rawStride;
                pixelDataConversion1RGB[x + 2] = pixelDataRGB[x + 2];
                pixelDataConversion1RGB[x + 1] = pixelDataRGB[x + 1];
                pixelDataConversion1RGB[x] = pixelDataRGB[x];
                pixelDataConversion1RGB[yIndex + x + 2] = pixelDataRGB[yIndex + x + 2];
                pixelDataConversion1RGB[yIndex + x + 1] = pixelDataRGB[yIndex + x + 1];
                pixelDataConversion1RGB[yIndex + x] = pixelDataRGB[yIndex + x];
            }

        }
        public void doNegatyw()
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
        public void doProgowanie(double value)
        {
            double jasnosc, prog = 255 * value / 100;
            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += (mySourceBitmap.Format.BitsPerPixel / 8))
                {
                    double B = pixelDataRGB[x + yIndex] * 0.114;
                    double G = pixelDataRGB[x + yIndex + 1] * 0.587;
                    double R = pixelDataRGB[x + yIndex + 2] * 0.299;
                    jasnosc = R + B + G;
                    if (jasnosc < prog)
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