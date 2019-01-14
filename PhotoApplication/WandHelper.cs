using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoApplication
{
    class WandHelper
    {
        private int value, range, maxRange, minRange;
        private bool ifValueOnBorder;

        private bool[] selectedPixels, borderPixels;
        private double[] pdHSV;
        private int height, stride;


        public WandHelper(double r, double v, double[] pdh, int s, int ph)
        {
            range = (int)r;
            pdHSV = pdh;
            stride = s;
            height = ph;
            value = (int)v;
            setRange();
            selectedPixels = new bool[stride * height + 4];
        }
        protected void setRange()
        {
            ifValueOnBorder = false;

            if (value < range)
            {
                maxRange = value + range;
                minRange = 360 + value - range;
                ifValueOnBorder = true;
            }
            else if (value > 360 - range)
            {
                minRange = value - range;
                maxRange = minRange - 360 - range;
                ifValueOnBorder = true;
            }
            else
            {
                minRange = value - range;
                maxRange = value + range;
            }
        }
        public bool[] WandAlg(int x, int y)
        {
            int offset = 0, endLoop = 0;
            selectedPixels = new bool[stride * height + 4];
            selectedPixels[y * stride + x] = true;

            do
            {
                offset++;
                endLoop = 0;

                for (int ix = -(offset * 4); ix < (offset * 4); ix += 4)     // left top direction to right
                {
                    
                    if (ifValueOnBorder)
                    {

                        if ((y - offset) * stride + x + ix >= 0 && (y - offset) * stride + x + ix < pdHSV.Length - 4 && (pdHSV[(y - offset) * stride + x + ix] > minRange || pdHSV[(y - offset) * stride + x + ix] < maxRange))
                        {
                            if (checkIfSelectedAround(x + ix, y - offset))
                            {
                                selectedPixels[(y - offset) * stride + x + ix] = true;
                            }
                            else
                            {
                                endLoop++;
                            }
                        }
                        else
                        {
                            endLoop++;
                        }



                    }
                    else
                    {
                        if ((y - offset) * stride + x + ix >= 0 && (y - offset) * stride + x + ix < pdHSV.Length - 4 && pdHSV[(y - offset) * stride + x + ix] > minRange && pdHSV[(y - offset) * stride + x + ix] < maxRange)
                        {
                            if (checkIfSelectedAround(x + ix, y - offset))
                            {
                                selectedPixels[(y - offset) * stride + x + ix] = true;
                            }
                            else
                            {
                                endLoop++;
                            }
                        }
                        else
                        {
                            endLoop++;
                        }
                    }
                    
                    
                }


                for (int iy = -offset; iy < offset; iy++)     // right top direction to bottom
                {
                    
                    if (ifValueOnBorder)
                    {
                        if ((y + iy) * stride + x + (4 * offset) >= 0 && (y + iy) * stride + x + (4 * offset) < pdHSV.Length - 4 && (pdHSV[(y + iy) * stride + x + (4 * offset)] > minRange || pdHSV[(y + iy) * stride + x + (4 * offset)] < maxRange))
                        {
                            if (checkIfSelectedAround(x + (4 * offset), y + iy))
                            {
                                selectedPixels[(y + iy) * stride + x + (4 * offset)] = true;
                            }
                            else
                            {
                                endLoop++;
                            }
                        }
                        else
                        {
                            endLoop++;
                        }
                    }
                    else
                    {
                        if ((y + iy) * stride + x + (4 * offset) >= 0 && (y + iy) * stride + x + (4 * offset) < pdHSV.Length - 4 && pdHSV[(y + iy) * stride + x + (4 * offset)] > minRange && pdHSV[(y + iy) * stride + x + (4 * offset)] < maxRange)
                        {
                            if (checkIfSelectedAround(x + (4 * offset), y + iy))
                            {
                                selectedPixels[(y + iy) * stride + x + (4 * offset)] = true;
                            }
                            else
                            {
                                endLoop++;
                            }
                        }
                        else
                        {
                            endLoop++;
                        }
                    }
                    
                    
                }


                for (int ix = (offset * 4); ix > -(offset * 4); ix -= 4)     // right bottom direction to left
                {
                    
                        if (ifValueOnBorder)
                        {
                            if ((y + offset) * stride + x + ix >= 0 && (y + offset) * stride + x + ix < pdHSV.Length && (pdHSV[(y + offset) * stride + x + ix] > minRange || pdHSV[(y + offset) * stride + x + ix] < maxRange))
                            {
                                if (checkIfSelectedAround(x + ix, y + offset))
                                {
                                    selectedPixels[(y + offset) * stride + x + ix] = true;
                                }
                                else
                                {
                                    endLoop++;
                                }
                            }
                            else
                            {
                                endLoop++;
                            }
                        }
                        else
                        {
                            if ((y + offset) * stride + x + ix >= 0 && (y + offset) * stride + x + ix < pdHSV.Length && pdHSV[(y + offset) * stride + x + ix] > minRange && pdHSV[(y + offset) * stride + x + ix] < maxRange)
                            {
                                if (checkIfSelectedAround(x + ix, y + offset))
                                {
                                    selectedPixels[(y + offset) * stride + x + ix] = true;
                                }
                                else
                                {
                                    endLoop++;
                                }
                            }
                            else
                            {
                                endLoop++;
                            }
                        }
                   
                }


                for (int iy = offset; iy > -offset; iy--)     // left bottom direction to top
                {
                    
                        if (ifValueOnBorder)
                        {
                            if ((y + iy) * stride + x - (4 * offset) < pdHSV.Length && (y + iy) * stride + x - (4 * offset) >= 0 && (pdHSV[(y + iy) * stride + x - (4 * offset)] > minRange || pdHSV[(y + iy) * stride + x - (4 * offset)] < maxRange))
                            {
                                if (checkIfSelectedAround(x - (4 * offset), y + iy))
                                {
                                    selectedPixels[(y + iy) * stride + x - (4 * offset)] = true;
                                }
                                else
                                {
                                    endLoop++;
                                }
                            }
                            else
                            {
                                endLoop++;
                            }
                        }
                        else
                        {
                            if ((y + iy) * stride + x - (4 * offset) < pdHSV.Length && (y + iy) * stride + x - (4 * offset) >= 0 && pdHSV[(y + iy) * stride + x - (4 * offset)] > minRange && pdHSV[(y + iy) * stride + x - (4 * offset)] < maxRange)
                            {
                                if (checkIfSelectedAround(x - (4 * offset), y + iy))
                                {
                                    selectedPixels[(y + iy) * stride + x - (4 * offset)] = true;
                                }
                                else
                                {
                                    endLoop++;
                                }
                            }
                            else
                            {
                                endLoop++;
                            }
                        }
                    
                }

                Debug.WriteLine(endLoop);
            }
            while (endLoop < (offset * 2) * 4 && endLoop<50000);

            return selectedPixels;
        }

        protected bool checkIfSelectedAround(int x, int y)
        {
            if (y * stride + x - 4 >= 0 && y * stride + x - 4 < pdHSV.Length - 4 && x > 0 && selectedPixels[y * stride + x - 4]) return true;   // left center
            if ((y - 1) * stride + x >= 0 && (y - 1) * stride + x < pdHSV.Length - 4 && y > 0 && selectedPixels[(y - 1) * stride + x]) return true;   // top center
            if (y * stride + x + 4 >= 0 && y * stride + x + 4 < pdHSV.Length - 4 && x < stride - 4 && selectedPixels[y * stride + x + 4]) return true;   // right center
            if ((y + 1) * stride + x >= 0 && (y + 1) * stride + x < pdHSV.Length - 4 && y < height - 1 && selectedPixels[(y + 1) * stride + x]) return true;    // bottom center
            if ((y - 1) * stride + x - 4 >= 0 && (y - 1) * stride + x - 4 < pdHSV.Length - 4 && x > 0 && y > 0 && selectedPixels[(y - 1) * stride + x - 4]) return true;    // left top
            if ((y - 1) * stride + x + 4 >= 0 && (y - 1) * stride + x + 4 < pdHSV.Length - 4 && x < stride - 4 && y > 0 && selectedPixels[(y - 1) * stride + x + 4]) return true;    // right top
            if ((y + 1) * stride + x - 4 >= 0 && (y + 1) * stride + x - 4 < pdHSV.Length - 4 && x > 0 && y < height - 1 && selectedPixels[(y + 1) * stride + x - 4]) return true;    // left bottom  
            if ((y + 1) * stride + x + 4 >= 0 && (y + 1) * stride + x + 4 < pdHSV.Length - 4 && x < stride - 4 && y < height - 1 && selectedPixels[(y + 1) * stride + x + 4]) return true;    // right bottom

            return false;
        }

    }
}
