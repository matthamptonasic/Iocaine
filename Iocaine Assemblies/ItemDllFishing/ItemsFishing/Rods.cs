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
    public class RodImages
    {
        //Private Members

        //Public Member Functions
        public static void init()
        {
            loadRepairRecipes();
        }
        public static Image getImageByID(ushort itemID)
        {
            string rodName = Data.Client.Rods.GetRodName(itemID);
            //Translations for old values of file names that I didn't want to change.
            if (rodName == "Ebisu F. Rod +1")
            {
                rodName = "Ebisu_Fishing_Rod_+1";
            }
            else if (rodName == "Lu Sh. F. Rod +1")
            {
                rodName = "Lu_Shangs_Fishing_Rod_+1";
            }
            else
            {
                rodName = rodName.Replace(" ", "_");
            }

            rodName += "_Full";
            return loadImage(rodName);
        }

        //Private Member Functions
        private static Image loadImage(string fileName)
        {
            string fullFileName = "Iocaine2.Data.Client.Images." + fileName + ".jpg";
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
        private static void loadRepairRecipes()
        {
            //List<ushort> itemIDs;
            //List<string> itemNames;
            //List<byte> itemQuantities;
            //List<ushort> itemResultIDs;
            //List<string> itemResultNames;
            //List<byte> itemResultQuantities;

            //itemIDs = new List<ushort>();
            //itemNames = new List<string>();
            //itemQuantities = new List<byte>();
            //itemResultIDs = new List<ushort>();
            //itemResultNames = new List<string>();
            //itemResultQuantities = new List<byte>();
        }
    }
}
