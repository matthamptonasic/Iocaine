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
    public class FishImages
    {
        //Public Members

        //Private Members

        //Public Member Functions
        public static void init()
        {
        }
        public static Image getImageByID(UInt16 iItemID)
        {
            String itemName = Fish.GetFishName(iItemID);
            itemName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(itemName);
            if((itemName.Contains("s 2") || (itemName.Contains("s 3"))) && !itemName.Contains("Quus"))
            {
                itemName = itemName.Substring(0, itemName.Length - 3);
            }
            else if (itemName.Contains(" 2") || (itemName.Contains(" 3")))
            {
                itemName = itemName.Substring(0, itemName.Length - 2);
            }
            itemName = itemName.Replace(" ", "_");
            itemName = itemName.Replace("-", "_");
            itemName += "_Full";
            return loadImage(itemName);
        }
        public static Image getImageByName(String iItemName)
        {
            UInt16 itemId = Fish.GetFishInfo(iItemName).ItemID;
            return getImageByID(itemId);
        }

        //Private Member Functions
        private static Image loadImage(String fileName)
        {
            String fullFileName = "Iocaine2.Data.Client.Images." + fileName + ".jpg";
            try
            {
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                Stream myStream;
                //String[] resources = myAssembly.GetManifestResourceNames();
                //foreach (String str in resources)
                //{
                //    Console.WriteLine(str);
                //}
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
