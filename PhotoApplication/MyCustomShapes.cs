using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            bool[] selectedPixels = new bool[stride * pixelHeight + 4];

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
            bool[] selectedPixels = new bool[stride * pixelHeight + 4];

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

    }
}
