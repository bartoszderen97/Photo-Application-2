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
        protected double[] pixelDataHSV;

        public BitmapSource getConvertedImage() { return mySourceBitmap; }
        public int getStride() { return rawStride; }
        public byte[] getPixelData() { return pixelDataRGB; }

        private void setPixelDataRGB() { mySourceBitmap.CopyPixels(pixelDataRGB, rawStride, 0); /* possible ArgumentOutOfRangeException */}

        public AllConversions(BitmapSource src)
        {
            mySourceBitmap = src;
            width = src.PixelWidth;
            height = src.PixelHeight;
            rawStride = (width * PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
            pixelDataRGB = new byte[rawStride * height + 4]; /// possible OutOfMemoryException 
            setPixelDataRGB();
            pixelDataHSV = new double[rawStride * height + 4];  /// possible OutOfMemoryException 
            convertRGBtoHSV();
            convertHSVtoRGB();
        }

        protected void convertHSVtoRGB()
        {
            double r, f, a, b, c;

            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += 4)
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
                for (int x = 0; x < rawStride; x += 4)
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
        
        public void changePixels()
        {
            byte k = 255;

            for (int y = 0; y < height; y++)
            {
                int yIndex = y * rawStride;
                for (int x = 0; x < rawStride; x += 4)
                {
                    
                    pixelDataRGB[x + yIndex + 2] = (byte)(k - pixelDataRGB[x + yIndex + 2]);
                    pixelDataRGB[x + yIndex + 1] = (byte)(k - pixelDataRGB[x + yIndex + 1]);
                    pixelDataRGB[x + yIndex] = (byte)(k - pixelDataRGB[x + yIndex]);
                    
                }
            }
        }
    }
}