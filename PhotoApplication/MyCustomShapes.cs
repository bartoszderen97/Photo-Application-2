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

        public static Rectangle drawMyRectangle(ref Canvas canvas, Point first, Point second, bool type)
        {
            Rectangle rect;
            rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Blue);
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
            Debug.WriteLine("first: " + first + " second: " + second);
            canvas.Children.Add(rect);
            return rect;
        }
    }
}
