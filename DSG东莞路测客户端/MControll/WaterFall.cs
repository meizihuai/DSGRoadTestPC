using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace 电磁信息云服务CSharp
{
    class WaterFall:PictureBox
    {
        private Bitmap mBitmap;
        private int currentY = 0;
        public WaterFall()
        {
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Dock = DockStyle.Fill;
            
          // this.BackColor = Color.Black;
           // this.Resize += WaterFall_Resize;
        }

        private void WaterFall_Resize(object sender, EventArgs e)
        {
           // mBitmap = new Bitmap(this.Width, this.Height);
        }
        public void AddWaterFall(double[] yy)
        {
            if (yy == null || yy.Length == 0) return;
            int width = yy.Length;
            int height = this.Height;
            int leftWidth = (int)Math.Floor(width * 0.05);
            int rightWidth = (int)Math.Floor(width * 0.02);
            rightWidth = width - rightWidth;
            int levelValue = -75;
            if (mBitmap==null || mBitmap.Width != width)
            {
                mBitmap = new Bitmap(width, height);
                currentY = 0;
                for(int m = 0; m < width; m++)
                {
                    double value = yy[m];
                    int vR = Math.Abs((int)(value + 90));
                    int vG = Math.Abs((int)(value + 10));
                    int vB = Math.Abs((int)(value + 30));
                    if (value > levelValue)
                    {
                        vR = 255;vG = 255;vB = 0;
                    }
                    if (vR > 255) vR = 255;
                    if (vG > 255) vG = 255;
                    if (vB > 255) vB = 255;
                    if (vR <0) vR= 0;
                    if (vG < 0) vG = 0;
                    if (vB < 0) vB= 0;
                    if(m<=leftWidth || m >= rightWidth)
                    {
                        vR = 0; vG = 80; vB = 60;
                    }
                    mBitmap.SetPixel(m, currentY, Color.FromArgb(vR, vG, vB));
                }
                currentY++;
                this.Image = mBitmap;
              //  this.Image = Image.FromFile(@"C:\Users\meizi\source\repos\电磁信息云服务CSharp\电磁信息云服务CSharp\bin\Debug\tmp.jpg");
                return;
            }
            if (currentY < height)
            {
                for (int m = 0; m < width; m++)
                {
                    double value = yy[m];
                    int vR = Math.Abs((int)(value + 90));
                    int vG = Math.Abs((int)(value + 10));
                    int vB = Math.Abs((int)(value + 30));
                    if (value > levelValue)
                    {
                        vR = 255; vG = 255; vB = 0;
                    }
                    if (vR > 255) vR = 255;
                    if (vG > 255) vG = 255;
                    if (vB > 255) vB = 255;
                    if (vR < 0) vR = 0;
                    if (vG < 0) vG = 0;
                    if (vB < 0) vB = 0;
                    if (m <= leftWidth || m >= rightWidth)
                    {
                        vR = 0; vG = 80; vB = 60;
                    }
                    mBitmap.SetPixel(m, currentY, Color.FromArgb(vR, vG, vB));
                }
                currentY++;
                this.Image = mBitmap;
              //  mBitmap.Save("tmp.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                return;
            }
            else
            {
                Bitmap nb = mBitmap.Clone(new Rectangle(0, 1, width, height - 1), System.Drawing.Imaging.PixelFormat.DontCare);
                mBitmap = new Bitmap(width, height);
                Graphics gk = Graphics.FromImage(mBitmap);
                gk.DrawImage(nb, 0, 0);
                gk.Save();
                for (int m = 0; m < width; m++)
                {
                    double value = yy[m];
                    int vR = Math.Abs((int)(value + 90));
                    int vG = Math.Abs((int)(value + 10));
                    int vB = Math.Abs((int)(value + 30));
                    if (value > levelValue)
                    {
                        vR = 255; vG = 255; vB = 0;
                    }
                    if (vR > 255) vR = 255;
                    if (vG > 255) vG = 255;
                    if (vB > 255) vB = 255;
                    if (vR < 0) vR = 0;
                    if (vG < 0) vG = 0;
                    if (vB < 0) vB = 0;
                    if (m <= leftWidth || m >= rightWidth)
                    {
                        vR = 0; vG = 80; vB = 60;
                    }
                    mBitmap.SetPixel(m, height - 1, Color.FromArgb(vR, vG, vB));
                }
                this.Image = mBitmap;
            }
        }
    }
}
