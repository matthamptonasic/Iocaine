using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Iocaine2;

namespace Iocaine2.Data.Client
{
    public class BaitImages
    {
        //Public Members

        //Private Members

        //Public Member Functions
        public static void init()
        {
        }
        public static Image getImageByID(UInt16 iItemID)
        {
            String baitName = Bait.GetBaitInfo(iItemID).BaitName;

            baitName = baitName.Replace(" ", "_");
            baitName = baitName.Replace(".", "");
            baitName += "_Full";
            return loadImage(baitName);
        }

        //Private Member Functions
        private static Image loadImage(String fileName)
        {
            String fullFileName = "Iocaine2.Data.Client.Images." + fileName + ".jpg";
            try
            {
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                Stream myStream;
                try
                {
                    myStream = myAssembly.GetManifestResourceStream(fullFileName);
                    return new Bitmap(myStream);
                }
                catch (Exception ex_in)
                {
                    System.Console.WriteLine("Error loading " + fullFileName + " resource into stream: " + ex_in.ToString());
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error loading " + fullFileName + " resource into memory: " + ex.ToString());
                return null;
            }
        }
    }
}
