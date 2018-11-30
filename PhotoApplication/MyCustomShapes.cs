using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PhotoApplication
{
    class MyCustomShapes
    {
        public static SolidColorBrush borderColor = new SolidColorBrush(Colors.Blue);
        public static double range = 20;

        public static bool[] selectedPixels, alreadyChecked, borderPixels;
        public static Rectangle drawMyRectangle(ref Canvas canvas, Point first, Point second)
        {
            Rectangle rect;
            rect = new Rectangle();
            rect.Stroke = borderColor;
            rect.StrokeThickness = 3;
            rect.Fill = new SolidColorBrush(Colors.Transparent);
            rect.Width = Math.Abs(first.X - second.X);
            rect.Height = Math.Abs(first.Y - second.Y);
            if (first.X > second.X) 
                Canvas.SetLeft(rect, second.X);
            else
                Canvas.SetLeft(rect, first.X);

            if (first.Y > second.Y)
                Canvas.SetTop(rect, second.Y);
            else
                Canvas.SetTop(rect, first.Y);

            canvas.Children.Add(rect);
            return rect;
        }

        public static Ellipse drawMyEllipse(ref Canvas canvas, Point first, Point second)
        {
            Ellipse ellipse;
            ellipse = new Ellipse();
            ellipse.Stroke = borderColor;
            ellipse.StrokeThickness = 3;
            ellipse.Fill = new SolidColorBrush(Colors.Transparent);
            ellipse.Width = Math.Abs(first.X - second.X);
            ellipse.Height = Math.Abs(first.Y - second.Y);

            if (first.X > second.X)
                Canvas.SetLeft(ellipse, second.X);
            else
                Canvas.SetLeft(ellipse, first.X);

            if (first.Y > second.Y)
                Canvas.SetTop(ellipse, second.Y);
            else
                Canvas.SetTop(ellipse, first.Y);

            canvas.Children.Add(ellipse);
            return ellipse;
        }
        public static void setRandomColor()
        {
            Random r = new Random();
            borderColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 233)));
        }

        public static bool[] getSelectedPixelsArrayForRect(Point leftTop, Point rightBottom, int stride, int pixelHeight, double actualHeight, double actualWidth)
        {
            selectedPixels = new bool[stride * pixelHeight + 4];

            if (!(leftTop.X < rightBottom.X && leftTop.Y < rightBottom.Y))
                exchangePointsHelper(ref leftTop, ref rightBottom);

            double leftBorder, topBorder, rightBorder, bottomBorder;
            leftBorder = (leftTop.X / actualWidth) * stride;
            rightBorder = (rightBottom.X / actualWidth) * stride;
            topBorder = (leftTop.Y / actualHeight) * pixelHeight;
            bottomBorder = (rightBottom.Y / actualHeight) * pixelHeight;

            for (int y = 0; y < pixelHeight; y++)
            {
                int yIndex = y * stride;
                for (int x = 0; x < stride; x += 4)
                {
                    if ((x > leftBorder && x <= rightBorder) && (y > topBorder && y < bottomBorder))
                    {
                        selectedPixels[yIndex + x] = true;
                        selectedPixels[yIndex + x + 1] = true;
                        selectedPixels[yIndex + x + 2] = true;
                        selectedPixels[yIndex + x + 3] = true;
                    }

                }
            }


            return selectedPixels;
        }

        public static bool[] getSelectedPixelsArrayForEllipse(Point leftTop, Point rightBottom, int stride, int pixelHeight, double actualHeight, double actualWidth)
        {
            selectedPixels = new bool[stride * pixelHeight + 4];

            if (!(leftTop.X < rightBottom.X && leftTop.Y < rightBottom.Y))
                exchangePointsHelper(ref leftTop, ref rightBottom);

            double leftBorder, topBorder, rightBorder, bottomBorder;
            leftBorder = (leftTop.X / actualWidth) * stride;
            rightBorder = (rightBottom.X / actualWidth) * stride;
            topBorder = (leftTop.Y / actualHeight) * pixelHeight;
            bottomBorder = (rightBottom.Y / actualHeight) * pixelHeight;

            double centralPointX, centralPointY, ellWidth, ellHeight;
            ellWidth = (rightBorder - leftBorder) / 2;
            ellHeight = (bottomBorder - topBorder) / 2;
            centralPointX = leftBorder + (ellWidth);
            centralPointY = topBorder + (ellHeight);

            for (int y = 0; y < pixelHeight; y++)
            {
                int yIndex = y * stride;
                for (int x = 0; x < stride; x += 4)
                {
                    if (calculateIfInsideEllipse(x,y,centralPointX,centralPointY,ellWidth,ellHeight))
                    {
                        selectedPixels[yIndex + x] = true;
                        selectedPixels[yIndex + x + 1] = true;
                        selectedPixels[yIndex + x + 2] = true;
                        selectedPixels[yIndex + x + 3] = true;
                    }

                }
            }

            return selectedPixels;
        }

        protected static void exchangePointsHelper(ref Point leftTop, ref Point rightBottom)
        {
            if (leftTop.X >= rightBottom.X && leftTop.Y >= rightBottom.Y)  // just exchange points
            {
                Point buff = leftTop;
                leftTop = rightBottom;
                rightBottom = buff;
            }
            else if (leftTop.X >= rightBottom.X && leftTop.Y <= rightBottom.Y)  // leftTop == rightTop && rightBottom == leftBottom
            {
                Point buff = leftTop;
                leftTop.X = rightBottom.X;
                rightBottom.X = buff.X;
            }
            else if (leftTop.X <= rightBottom.X && leftTop.Y >= rightBottom.Y)  // leftTop == leftBottom && rightBottom == rightTop
            {
                Point buff = leftTop;
                leftTop.Y = rightBottom.Y;
                rightBottom.Y = buff.Y;
            }
        }
        protected static bool calculateIfInsideEllipse(double x, double y, double a, double b, double c, double d)
        {
            double pattern = ((x - a) * (x - a) / (c * c));
            pattern += ((y - b) * (y - b) / (d * d));
            if (pattern <= 1) return true;
            else return false;
        }
        public static bool[] getSelectedPixelsArrayForWand2(Point point, double[] pixelDataHSV, byte[] pixelDataRGB, int stride, int pixelHeight, double actualHeight, double actualWidth)
        {
            selectedPixels = new bool[stride * pixelHeight + 4];
            double pointX, pointY;
            pointX = (point.X / actualWidth) * stride;
            pointY = (point.Y / actualHeight) * pixelHeight;

            if ((int)pointX % 4 != 0)
                pointX -= pointX % 4;
            double value = pixelDataHSV[(int)pointY * stride + (int)pointX];

            borderColor = new SolidColorBrush(Color.FromRgb(pixelDataRGB[(int)pointY*stride+(int)pointX + 2], pixelDataRGB[(int)pointY * stride + (int)pointX + 1], pixelDataRGB[(int)pointY * stride + (int)pointX]));

            double rangeMin, rangeMax;
            bool ifValueOnBorder = false;

            if (value < range)
            {
                rangeMax = value + range;
                rangeMin = 360 + value - range;
                ifValueOnBorder = true;
            }
            else if (value > 360 - range)
            {
                rangeMin = value - range;
                rangeMax = rangeMin - 360 - range;
                ifValueOnBorder = true;
            }
            else
            {
                rangeMin = value - range;
                rangeMax = value + range;
            }

            for(int i = 0; i < pixelDataHSV.Length; i += 4)
            {
                if (ifValueOnBorder)
                {
                    if (pixelDataHSV[i] > rangeMin || pixelDataHSV[i] < rangeMax)
                    {
                        selectedPixels[i] = true;
                    }
                   
                }
                else
                {
                    if (pixelDataHSV[i] > rangeMin && pixelDataHSV[i] < rangeMax)
                    {
                        selectedPixels[i] = true;
                    }
                }
            }
            return selectedPixels;
        }
        
        public static bool[] getSelectedPixelsArrayForWand(Point point, double[] pixelDataHSV, int stride, int pixelHeight, double actualHeight, double actualWidth)
        {
            selectedPixels = new bool[stride * pixelHeight + 4];
            alreadyChecked = new bool[stride * pixelHeight + 4];
            borderPixels = new bool[stride * pixelHeight + 4];

            double pointX, pointY;
            pointX = (point.X / actualWidth) * stride;
            pointY = (point.Y / actualHeight) * pixelHeight;
            double value = pixelDataHSV[(int)pointY * stride + (int)pointX];

            if ((int)pointX % 4 != 4)
                pointX -= pointX % 4;

            double rangeMin, rangeMax, range = 20;
            bool ifValueOnBorder = false;

            WandHelper wandHelper = new WandHelper(range, value, pixelDataHSV, stride, pixelHeight);
            
            return wandHelper.WandAlg((int)pointX, (int)pointY);
        }
        public static List<Ellipse> getBorderPoints(int stride, int pixelHeight, double actualHeight, double actualWidth)
        {
            List<Ellipse> myPoints = new List<Ellipse>();
            for(int i=0; i < borderPixels.Length; i++)
            {
                if (borderPixels[i])
                {
                    int y = i / stride, x = (i - y);
                    int dotSize = 3;

                    Ellipse currentDot = new Ellipse();
                    currentDot.Stroke = new SolidColorBrush(Colors.Green);
                    currentDot.StrokeThickness = 3;
                    Canvas.SetZIndex(currentDot, 3);
                    currentDot.Height = dotSize;
                    currentDot.Width = dotSize;
                    currentDot.Fill = new SolidColorBrush(Colors.Green);
                    currentDot.Margin = new Thickness(x, y, 0, 0); // Sets the position.
                    myPoints.Add(currentDot);
                }
            }
            return myPoints;
        }
        public static void floodFillAlg(int x, int y, double rangeMin, double rangeMax, bool ifValueOnBorder, double[] pixelDataHSV, int stride, int pixelHeight)
        {

            alreadyChecked[y * stride + x] = true;
            alreadyChecked[y * stride + x + 4] = true;
            alreadyChecked[y * stride + x - 4] = true;
            alreadyChecked[(y - 1) * stride + x - 4] = true;
            alreadyChecked[(y + 1) * stride + x + 4] = true;
            alreadyChecked[(y - 1) * stride + x + 4] = true;
            alreadyChecked[(y + 1) * stride + x - 4] = true;
            alreadyChecked[(y - 1) * stride + x] = true;
            alreadyChecked[(y + 1) * stride + x] = true;


            if (ifValueOnBorder)
            {
                if (pixelDataHSV[y * stride + x] > rangeMin || pixelDataHSV[y * stride + x] < rangeMax)
                {
                    selectedPixels[y * stride + x] = true;
                    selectedPixels[y * stride + x + 4] = true;
                    selectedPixels[y * stride + x - 4] = true;
                    selectedPixels[(y - 1) * stride + x - 4] = true;
                    selectedPixels[(y + 1) * stride + x + 4] = true;
                    selectedPixels[(y - 1) * stride + x + 4] = true;
                    selectedPixels[(y + 1) * stride + x - 4] = true;
                    selectedPixels[(y - 1) * stride + x] = true;
                    selectedPixels[(y + 1) * stride + x] = true;
                }
                else
                {
                    borderPixels[y * stride + x] = true;
                    borderPixels[y * stride + x + 4] = true;
                    borderPixels[y * stride + x - 4] = true;
                    borderPixels[(y - 1) * stride + x - 4] = true;
                    borderPixels[(y + 1) * stride + x + 4] = true;
                    borderPixels[(y - 1) * stride + x + 4] = true;
                    borderPixels[(y + 1) * stride + x - 4] = true;
                    borderPixels[(y - 1) * stride + x] = true;
                    borderPixels[(y + 1) * stride + x] = true;

                    return;
                }
            }
            else
            {
                if (pixelDataHSV[y * stride + x] > rangeMin && pixelDataHSV[y * stride + x] < rangeMax)
                {
                    selectedPixels[y * stride + x] = true;
                    selectedPixels[y * stride + x + 4] = true;
                    selectedPixels[y * stride + x - 4] = true;
                    selectedPixels[(y - 1) * stride + x - 4] = true;
                    selectedPixels[(y + 1) * stride + x + 4] = true;
                    selectedPixels[(y - 1) * stride + x + 4] = true;
                    selectedPixels[(y + 1) * stride + x - 4] = true;
                    selectedPixels[(y - 1) * stride + x] = true;
                    selectedPixels[(y + 1) * stride + x] = true;
                }
                else
                {
                    borderPixels[y * stride + x] = true;
                    borderPixels[y * stride + x + 4] = true;
                    borderPixels[y * stride + x - 4] = true;
                    borderPixels[(y - 1) * stride + x - 4] = true;
                    borderPixels[(y + 1) * stride + x + 4] = true;
                    borderPixels[(y - 1) * stride + x + 4] = true;
                    borderPixels[(y + 1) * stride + x - 4] = true;
                    borderPixels[(y - 1) * stride + x] = true;
                    borderPixels[(y + 1) * stride + x] = true;
                    return;
                }
            }
            // left center
            if (x >= 12 && !(alreadyChecked[y * stride + x - 12])) 
            {
                floodFillAlg(x - 12, y, rangeMin, rangeMax, ifValueOnBorder, pixelDataHSV, stride, pixelHeight);
            }
            // right center
            if (x < stride - 12 && !(alreadyChecked[y * stride + x + 12]))
            {
                floodFillAlg(x + 12, y, rangeMin, rangeMax, ifValueOnBorder, pixelDataHSV, stride, pixelHeight);
            }
            // top center
            if (y > 2 && !(alreadyChecked[(y - 3) * stride + x]))
            {
                floodFillAlg(x, y - 3, rangeMin, rangeMax, ifValueOnBorder, pixelDataHSV, stride, pixelHeight);
            }
            // bottom center
            if (y < pixelHeight - 3 && !(alreadyChecked[(y + 3) * stride + x]))
            {
                floodFillAlg(x, y + 3, rangeMin, rangeMax, ifValueOnBorder, pixelDataHSV, stride, pixelHeight);
            }
            
            // left top
            if (x >= 12 && y > 2 && !(alreadyChecked[(y - 3) * stride + x - 12]))
            {
                floodFillAlg(x - 12, y - 3, rangeMin, rangeMax, ifValueOnBorder, pixelDataHSV, stride, pixelHeight);
            }
            // right top
            if (x < stride - 12 && y > 2 && !(alreadyChecked[(y - 3) * stride + x + 12]))
            {
                floodFillAlg(x + 12, y - 3, rangeMin, rangeMax, ifValueOnBorder, pixelDataHSV, stride, pixelHeight);
            }
            // left bottom
            if (x >= 12 && y < pixelHeight - 3 && !(alreadyChecked[(y + 3) * stride + x - 12]))
            {
                floodFillAlg(x - 12, y + 3, rangeMin, rangeMax, ifValueOnBorder, pixelDataHSV, stride, pixelHeight);
            }
            // right bottom
            if (x < stride - 12 && y < pixelHeight - 3 && !(alreadyChecked[(y + 3) * stride + x + 12]))
            {
                floodFillAlg(x + 12, y + 3, rangeMin, rangeMax, ifValueOnBorder, pixelDataHSV, stride, pixelHeight);
            }
            
        }
        
    }
}
