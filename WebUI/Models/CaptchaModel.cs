using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;


namespace WebUI.Models
{
    public class CaptchaModel
    {
        /// <summary>
        /// Set the Default values for model
        /// </summary>
        public CaptchaModel()
        {
            setCaptchaParams();
        }

        /// <summary>
        /// Result of the Captcha image
        /// </summary>
        public string Result { get; internal set; }

        /// <summary>
        /// Captcha string
        /// </summary>
        public string CaptchaString { get; internal set; }

        /// <summary>
        /// Set the new values for Result and CaptchaString
        /// </summary>
        public void setCaptchaParams()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);

            //generate new question
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);

            Result = (a + b).ToString();
            CaptchaString = string.Format("{0} + {1} = ?", a, b);
        }

        /// <summary>
        /// Generate Captcha image
        /// </summary>
        /// <param name="CaptchaString">String for display captcha</param>
        /// <param name="FileType">Image file type </param>
        /// <param name="noisy">True value for noising </param>
        /// <returns>byte[]</returns>
        public byte[] getCaptcha(string CaptchaString, CaptchaType FileType, int CaptchaWidth, int CaptchaHeight, bool noisy)
        {
            //string fileformat;
            byte[] ResultByte;
            Random rand = new Random((int)DateTime.Now.Ticks);

            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(CaptchaWidth, CaptchaHeight))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

                //add noise
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);

                        gfx.DrawEllipse(pen, x - r, y - r, r, r);
                    }
                }

                //add question
                gfx.DrawString(CaptchaString, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

                #region Renderer
                switch (FileType)
                {
                    case CaptchaType.Bmp:
                        bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case CaptchaType.Gif:
                        bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case CaptchaType.Jpeg:
                        bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case CaptchaType.Png:
                        bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }
                #endregion

                ResultByte = mem.GetBuffer();
            }

            return ResultByte;

        }

        /// <summary>
        /// Enum image file format (Just helper)
        /// </summary>
        public enum CaptchaType
        {
            Bmp,
            Jpeg,
            Png,
            Gif
        }
    }
}