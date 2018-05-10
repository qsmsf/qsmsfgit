using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;

namespace FineUIMvc.QuickStart.Code
{
    public class RectSize
    {
        public int width{get;set;}
        public int height {get;set;}
        public RectSize(RectSize size)
        {
            this.width = size.width;
            this.height = size.height;
        }
        public RectSize()
        {
            this.width = 0;
            this.height = 0;
        }
    }
    public class UtilsTool
    {
        public static RectSize getConfSize(System.Drawing.Image file)
        {
            RectSize size = new RectSize();
            if(file.Width* 25/35 >= file.Height)
            {
                size.width = 350;
                size.height = file.Height * 350 / file.Width;
            }
            else
            {
                size.height = 250;
                size.width = file.Width * 250 / file.Height;
            }
            return size;
        }

        public static RectSize getCTSize(System.Drawing.Image file)
        {
            RectSize size = new RectSize();
            if (file.Width * 8 / 5 >= file.Height)
            {
                size.width = 500;
                size.height = file.Height * 500 / file.Width;
            }
            else
            {
                size.height = 800;
                size.width = file.Width * 800 / file.Height;
            }
            return size;
        }

        /// <summary>
        /// 根据角度旋转图标
        /// </summary>
        /// <param name="img"></param>
        public static System.Drawing.Image RotateImg(System.Drawing.Image img, float angle)
        {
            //通过Png图片设置图片透明，修改旋转图片变黑问题。
            int width = img.Width;
            int height = img.Height;
            //角度
            Matrix mtrx = new Matrix();
            mtrx.RotateAt(angle, new PointF((width / 2), (height / 2)), MatrixOrder.Append);
            //得到旋转后的矩形
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, width, height));
            RectangleF rct = path.GetBounds(mtrx);
            //生成目标位图
            Bitmap devImage = new Bitmap((int)(rct.Width), (int)(rct.Height));
            Graphics g = Graphics.FromImage(devImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //计算偏移量
            Point Offset = new Point((int)(rct.Width - width) / 2, (int)(rct.Height - height) / 2);
            //构造图像显示区域：让图像的中心与窗口的中心点一致
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, (int)width, (int)height);
            Point center = new Point((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(angle);
            //恢复图像在水平和垂直方向的平移
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(img, rect);
            //重至绘图的所有变换
            g.ResetTransform();
            g.Save();
            g.Dispose();
            path.Dispose();
            return devImage;
        }

        public static int paintWordOnPic(string imgPath, string destPath, string word, int x,int y)
        {
            Bitmap bmp = new Bitmap(imgPath);
            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("宋体", 20);
            SolidBrush sbrush = new SolidBrush(Color.Black);
            g.DrawString(word, font, sbrush, new PointF(x, y));
            
            bmp.Save(destPath, System.Drawing.Imaging.ImageFormat.Jpeg);

            return 0;
        }

        //把数字转换为大写
        public static string NumtoUpper(int num)
        {
            String str = num.ToString();
            string rstr = "";
            int n;
            for (int i = 0; i < str.Length; i++)
            {
                n = Convert.ToInt16(str[i].ToString());//char转数字,转换为字符串，再转数字
                switch (n)
                {
                    case 0: rstr = rstr + "〇"; break;
                    case 1: rstr = rstr + "一"; break;
                    case 2: rstr = rstr + "二"; break;
                    case 3: rstr = rstr + "三"; break;
                    case 4: rstr = rstr + "四"; break;
                    case 5: rstr = rstr + "五"; break;
                    case 6: rstr = rstr + "六"; break;
                    case 7: rstr = rstr + "七"; break;
                    case 8: rstr = rstr + "八"; break;
                    default: rstr = rstr + "九"; break;
                }

            }
            return rstr;
        }
        //月转化为大写
        public static string MonthtoUpper(int month)
        {
            if (month < 10)
            {
                return NumtoUpper(month);
            }
            else
                if (month == 10) { return "十"; }

            else
            {
                return "十" + NumtoUpper(month - 10);
            }
        }
        //日转化为大写
        public static string DaytoUpper(int day)
        {
            if (day < 20)
            {
                return MonthtoUpper(day);
            }
            else
            {
                String str = day.ToString();
                if (str[1] == '0')
                {
                    return NumtoUpper(Convert.ToInt16(str[0].ToString())) + "十";
                }
                else
                {
                    return NumtoUpper(Convert.ToInt16(str[0].ToString())) + "十"
                        + NumtoUpper(Convert.ToInt16(str[1].ToString()));
                }
            }
        }
        //日期转换为大写
        public static string DateToUpper(System.DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            return NumtoUpper(year) + "年" + MonthtoUpper(month) + "月" + DaytoUpper(day) + "日";

        }
    }
}