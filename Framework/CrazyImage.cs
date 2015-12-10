using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Framework
{
    public class CrazyImage
    {

        public enum ConvertType
        {
            [Description("Кроп")]
            Crop = 0,
            [Description("Фрейм")]
            Frame = 1
        }

        private static byte[] GetThumb(Image image, int canvasWidth, int canvasHeight, ConvertType CType)
        {
            var originalWidth = image.Width;
            var originalHeight = image.Height;

            //var aspectRatio = (double) originalWidth / originalHeight;

            canvasWidth = canvasWidth == 0 ? originalWidth : canvasWidth;
            canvasHeight = canvasHeight == 0 ? originalHeight : canvasHeight;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            var ratioX = canvasWidth / (double)originalWidth;
            var ratioY = canvasHeight / (double)originalHeight;

            double ratio = 0;

            //Crop or Frame
            if (CType == ConvertType.Crop)
                ratio = ratioX > ratioY ? ratioX : ratioY;
            else
                ratio = ratioX <= ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            var newHeight = Convert.ToInt32(originalHeight * ratio);
            var newWidth = Convert.ToInt32(originalWidth * ratio);

            if (newHeight < canvasHeight)
                canvasHeight = newHeight;

            if (newWidth < canvasWidth)
                canvasWidth = newWidth;

            using (Image thumbnail = new Bitmap(canvasWidth, canvasHeight))
            {

                using (var graphic = Graphics.FromImage(thumbnail))
                {

                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;


                    // Now calculate the X,Y position of the upper-left corner 
                    // (one of these will always be zero)
                    var posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
                    var posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

                    graphic.Clear(Color.Transparent); // white padding
                    graphic.DrawImage(image, posX, posY, newWidth, newHeight);

                    image.Dispose();
                    /* ------------- end new code ---------------- */

                    //var info = ImageCodecInfo.GetImageEncoders();

                    //var encoderParameters = new EncoderParameters(1)
                    //{
                    //    Param = {[0] = new EncoderParameter(Encoder.Quality, 100L)}
                    //};

                    using (var stream = new MemoryStream())
                    {
                        thumbnail.Save(stream, ImageFormat.Png);
                        return stream.ToArray();
                    }
                }
            }
        }

        public static byte[] GetThumbImage(Image file, int? canvasWidth, int? canvasHeight, ConvertType CType)
        {
            return GetThumb(file, canvasWidth ?? file.Width, canvasHeight ?? file.Height, CType);
        }

        public static byte[] GetThumbImage(string fileName, int? canvasWidth, int? canvasHeight, ConvertType CType)
        {
            Image image = null;

            try
            {
                image = Image.FromFile(fileName);
            }
            catch
            {
                return null;
            }

            return GetThumb(image, canvasWidth ?? image.Width, canvasHeight ?? image.Height, CType);

        }

        public async static Task<byte[]> GetThumbImageAsync(string fileName, int? canvasWidth, int? canvasHeight, ConvertType CType)
        {
            Image image = null;

            try
            {
                byte[] result;

                using (var sourceStream = File.Open(fileName, FileMode.Open))
                {
                    result = new byte[sourceStream.Length];
                    await sourceStream.ReadAsync(result, 0, (int)sourceStream.Length);
                }

                image = Image.FromStream(new MemoryStream(result));
            }
            catch
            {
                return null;
            }

            return GetThumb(image, canvasWidth ?? image.Width, canvasHeight ?? image.Height, CType);

        }



    }
}
