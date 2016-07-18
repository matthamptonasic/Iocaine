using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Iocaine2.Data.Client
{
    class Base64ToImage
    {
        public static Image ConvertThis(string ImageText)
        {
            if (ImageText.Length > 0)
            {
                byte[] bitmapData = new byte[ImageText.Length];
                bitmapData = Convert.FromBase64String(FixBase64ForImage(ImageText));

                System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);

                Bitmap bitImage = new Bitmap((Bitmap)Image.FromStream(streamBitmap));
                return bitImage;
            }
            else
            {
                return null;
            }
        }
        private static string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);

            sbText.Replace("\r\n", string.Empty);

            sbText.Replace(" ", string.Empty);

            return sbText.ToString();
        }
    }
}
