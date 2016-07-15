using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2.Data.Client
{
    public static partial class Maps
    {
        #region Structs
        private struct mapBox
        {
            public Int16 x1;
            public Int16 x2;
            public Int16 y1;
            public Int16 y2;
            public Int16 z1;
            public Int16 z2;
        }
        #endregion Structs
        #region Public Members
        private static int count = 0;
        public static int Count
        {
            get
            {
                return count;
            }
        }
        public static Brush DotColorAll = Brushes.Blue;
        public static Brush DotColorSingle = Brushes.Magenta;
        #region Function Pointers
        public delegate Boolean DownloadDirectory(String iPathFromRoot, String iSaveFolder);
        public delegate void DownloadFile(String iPathFromRoot, String iSaveFolder);
        public static DownloadDirectory DownloadDirectoryPtr
        {
            set
            {
                downloadDirectoryPtr = value;
            }
        }
        public static DownloadFile DownloadFilePtr
        {
            set
            {
                downloadFilePtr = value;
            }
        }
        public delegate String Decrypt(String iEncryptedString);
        public static SetString SetMapsPath
        {
            set
            {
                setMapsPath = value;
            }
        }
        public delegate void SendString(String iText);
        public delegate void SendDebug(String iText, UInt32 iDebugScope);
        public delegate void SetString(String iText);
        public delegate void SetBool(Boolean iValue);
        #endregion Function Pointers
        #endregion Public Members
        #region Private Members
        private static Boolean useResourcesNotFiles = false;
        private static Boolean isInitialized = false;
        private static String resourcePrefix = "Iocaine2.Images.";
        private static String noMap = "0_No_Map.png";
        private static String mapsPath = "";
        private static String defaultMapsDirFromRoot = "Map_Packs/Default";
        private static String mapPackZipFile = "IocaineMaps.zip";
        private const String localMapsDir = @".\Maps\";
        private const String localMapImagesDir = @".\Maps\Images\";
        private static Boolean useHexZoneId = false;
        private static Boolean useHexMapId = false;
        private static Boolean useApneaPack = false;
        private const String hexZoneIdFileNameCheck = "0b_0";
        private const String hexMapIdFileNameCheck = "09_a";
        private const String apneaZoneFileToCheck = "2bc_0";
        private static Dictionary<UInt16, Byte> zoneNbMaps;
        private static Dictionary<UInt16, List<String>> zoneMapList;
        private static Dictionary<UInt16, Dictionary<Byte, UInt16>> zoneToMapNbToMapId;
        private static MapInfoDS mapInfoDS = new MapInfoDS();
        #region Function Pointers
        private static DownloadDirectory downloadDirectoryPtr;
        private static DownloadFile downloadFilePtr;
        private static SendString timestampPtr;
        private static SendString errorPtr;
        private static SendString debugPtr;
        private static SetString setMapsPath;
        #endregion Function Pointers
        #endregion Private Members

        #region Public Member Functions
        #region Inits
        public delegate void MapsInitTypeDef();
        public static void SetValues(String iMapsPath,
                                DownloadDirectory iDownloadDirectoryPtr,
                                DownloadFile iDownloadFilePtr,
                                SendString iTimestampPtr,
                                SendString iErrorPtr,
                                SendString iDebugPtr,
                                SetString iSetMapsPath)
        {
            mapsPath = iMapsPath;
            downloadDirectoryPtr = iDownloadDirectoryPtr;
            downloadFilePtr = iDownloadFilePtr;
            timestampPtr = iTimestampPtr;
            errorPtr = iErrorPtr;
            debugPtr = iDebugPtr;
            setMapsPath = iSetMapsPath;
        }
        public static void init()
        {
            checkMapFiles();
            loadDictionaries();
            loadData();
            isInitialized = true;
        }
        private static void loadDictionaries()
        {
            zoneNbMaps = new Dictionary<ushort, byte>();
            zoneMapList = new Dictionary<ushort, List<string>>();
            zoneToMapNbToMapId = new Dictionary<ushort, Dictionary<Byte, UInt16>>();

            #region Use Resources
            if (useResourcesNotFiles)
            {
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                String[] resources = myAssembly.GetManifestResourceNames();
                count = resources.Length;
                foreach (String str in resources)
                {
                    if (str == resourcePrefix + noMap)
                    {
                        continue;
                    }
                    //Get zone ID
                    String[] strs = str.Split('_');
                    String zoneIdStr = strs[0];
                    zoneIdStr = zoneIdStr.Replace(resourcePrefix, "");
                    String fileName = str.Replace(resourcePrefix, "");
                    UInt16 zoneId = 0;
                    if (!UInt16.TryParse(zoneIdStr, out zoneId))
                        {
                            MessageBox.Show("Could not parse a UInt16 from '" + zoneIdStr + "'");
                        }
                    if (zoneNbMaps.ContainsKey(zoneId))
                    {
                        zoneNbMaps[zoneId]++;
                    }
                    else
                    {
                        zoneNbMaps[zoneId] = 1;
                        zoneMapList[zoneId] = new List<string>();
                    }
                    zoneMapList[zoneId].Add(fileName);
                }
            }
            #endregion Use Resources
            #region Use Files
            else
            {
                if(!Directory.Exists(mapsPath))
                {
                    if (!File.Exists(@".\NO_MAP_CHECK.txt"))
                    {
                        MessageBox.Show("Could not find maps file path:\n" + mapsPath);
                    }
                }
                else
                {
                    //String[] files = Directory.GetFiles(mapsPath, "*.png");
                    String[] files = Directory.GetFiles(mapsPath, "*.*", SearchOption.TopDirectoryOnly);
                    foreach(String file in files)
                    {
                        String fileName = Path.GetFileName(file);
                        if (fileName.ToLower().Contains(hexZoneIdFileNameCheck))
                        {
                            useHexZoneId = true;
                        }
                        else if (fileName.ToLower().Contains(hexMapIdFileNameCheck))
                        {
                            useHexMapId = true;
                        }
                        else if (fileName.ToLower().Contains(apneaZoneFileToCheck))
                        {
                            useApneaPack = true;
                        }

                        if (useHexZoneId && useHexMapId && useApneaPack)
                        {
                            break;
                        }
                    }

                    foreach(String file in files)
                    {
                        String fileName = Path.GetFileName(file);
                        //Get zone ID
                        String[] strs = fileName.Split('_');
                        String zoneIdStr = strs[0];
                        UInt16 zoneId = 0;
                        String mapIdStr = "";
                        UInt16 mapId = 0;
                        String[] rmdrStrs;
                        if(strs.Length < 2)
                        {
                            if(errorPtr != null)
                            {
                                errorPtr("filename split only returned 1 string.");
                                errorPtr("original filename was '" + fileName + "'");
                            }
                            continue;
                        }
                        else
                        {
                            rmdrStrs = strs[1].Split('.');
                            if(strs.Length < 2)
                            {
                                if(errorPtr != null)
                                {
                                    errorPtr("remaining file name split return less than 2 parts.");
                                    errorPtr("original filename was '" + fileName + "'");
                                }
                                continue;
                            }
                            else
                            {
                                mapIdStr = rmdrStrs[0];
                            }
                        }
                        System.Globalization.NumberStyles zoneIdStyle;
                        System.Globalization.NumberStyles mapIdStyle;
                        if (useHexZoneId)
                        {
                            zoneIdStyle = System.Globalization.NumberStyles.AllowHexSpecifier;
                        }
                        else
                        {
                            zoneIdStyle = System.Globalization.NumberStyles.Integer;
                        }
                        if(useHexMapId)
                        {
                            mapIdStyle = System.Globalization.NumberStyles.AllowHexSpecifier;
                        }
                        else
                        {
                            mapIdStyle = System.Globalization.NumberStyles.Integer;
                        }

                        if (!UInt16.TryParse(zoneIdStr, zoneIdStyle, System.Globalization.CultureInfo.InvariantCulture, out zoneId))
                        {
                            //MessageBox.Show("Could not parse a UInt16 (zoneID) from '" + zoneIdStr + "'");
                            continue;
                        }
                        if (useApneaPack)
                        {
                            if ((zoneId > 255) && (zoneId < 700))
                            {
                                continue;
                            }
                            if (zoneId > 699)
                            {
                                //Apneas maps over 255 restart at 700.
                                zoneId -= 444;
                            }
                        }
                        if (!UInt16.TryParse(mapIdStr, mapIdStyle, System.Globalization.CultureInfo.InvariantCulture, out mapId))
                        {
                            //MessageBox.Show("Could not parse a Byte (MapID) from '" + mapIdStr + "'");
                            continue;
                        }

                        //Set number of maps and zone ID to filename maps.
                        if (zoneNbMaps.ContainsKey(zoneId))
                        {
                            zoneNbMaps[zoneId]++;
                        }
                        else
                        {
                            zoneNbMaps[zoneId] = 1;
                            zoneMapList[zoneId] = new List<string>();
                        }
                        zoneMapList[zoneId].Add(fileName);

                        //Set the zone ID to Map number to Map ID maps.
                        if(zoneToMapNbToMapId.ContainsKey(zoneId))
                        {
                            Int32 nbMapsSoFar = zoneToMapNbToMapId[zoneId].Count;
                            zoneToMapNbToMapId[zoneId][(Byte)nbMapsSoFar] = mapId;
                        }
                        else
                        {
                            zoneToMapNbToMapId[zoneId] = new Dictionary<Byte, UInt16>();
                            zoneToMapNbToMapId[zoneId][0] = mapId;
                        }
                    }
                }
            }
            #endregion Use Files
        }
        #endregion Inits
        #region Get Functions
        public static MapSet GetMap(UInt16 iZoneID)
        {
            if (!isInitialized)
            {
                init();
            }
            if (!zoneMapList.ContainsKey(iZoneID))
            {
                MapSet ms = new MapSetNoMap(loadImageResource(noMap), iZoneID, 0);
                ms.SuppressArrow = true;
                return ms;
            }
            else
            {
                UInt16 mapId = 0;
                if(zoneToMapNbToMapId.ContainsKey(iZoneID))
                {
                    mapId = zoneToMapNbToMapId[iZoneID][0];
                }
                //filterPosPerMap(iZoneID, mapIdx, iPosXs, iPosYs, out fltPosXs, out fltPosYs);
                //posToPixels(iZoneID, mapIdx, fltPosXs, fltPosYs, out pxlXs, out pxlYs);
                return new MapSet(loadImageFile(zoneMapList[iZoneID][0]), iZoneID, (Byte)mapId);
            }
        }
        public static MapSet GetMap(UInt16 iZoneID, Byte iMapIdx)
        {
            if (!isInitialized)
            {
                init();
            }
            if (zoneMapList.ContainsKey(iZoneID))
            {
                if(zoneNbMaps[iZoneID] > iMapIdx)
                {
                    UInt16 mapId = 0;
                    if (zoneToMapNbToMapId.ContainsKey(iZoneID) && zoneToMapNbToMapId[iZoneID].ContainsKey((Byte)(iMapIdx)))
                    {
                        mapId = zoneToMapNbToMapId[iZoneID][(Byte)(iMapIdx)];
                    }
                    //filterPosPerMap(iZoneID, mapId, iPosXs, iPosYs, out fltPosXs, out fltPosYs);
                    //posToPixels(iZoneID, mapId, fltPosXs, fltPosYs, out pxlXs, out pxlYs);
                    return new MapSet(loadImageFile(zoneMapList[iZoneID][iMapIdx]), iZoneID, (Byte)mapId);
                }
                else
                {
                    MapSet ms = new MapSetNoMap(loadImageResource(noMap), iZoneID, 0);
                    ms.SuppressArrow = true;
                    return ms;
                }
            }
            else
            {
                MapSet ms = new MapSetNoMap(loadImageResource(noMap), iZoneID, 0);
                ms.SuppressArrow = true;
                return ms;
            }
        }
        public static Byte GetNumberOfMapsByID(UInt16 zoneID)
        {
            if (!isInitialized)
            {
                init();
            }
            if(zoneNbMaps.ContainsKey(zoneID))
            {
                return zoneNbMaps[zoneID];
            }
            else
            {
                return 0;
            }
        }
        public static Boolean GetMapId(UInt16 iZoneId, Int16 iX, Int16 iY, Int16 iZ, out Byte oMapID)
        {
            if (!isInitialized)
            {
                init();
            }
            String filter = "ZoneID=" + iZoneId + " ";
            filter += "AND X1<=" + iX + " ";
            filter += "AND X2>=" + iX + " ";
            filter += "AND Y1<=" + iY + " ";
            filter += "AND Y2>=" + iY + " ";
            filter += "AND Z1<=" + iZ + " ";
            filter += "AND Z2>=" + iZ;

            MapInfoDS.MapInfoBoxesRow[] boxRows = (MapInfoDS.MapInfoBoxesRow[])mapInfoDS.MapInfoBoxes.Select(filter);
            if(boxRows.Length == 0)
            {
                //The X,Y given does not exist inside any of the defined boxes. Return Byte.MaxValue
                oMapID = Byte.MaxValue;
                return false;
            }
            else
            {
                //This could be ambiguous if there are multiple levels in the zone.
                //Since we're not considering the Z plane, just return the first value.
                oMapID = boxRows[0].MapID;
                return true;
            }
        }
        public static Byte GetMapId(UInt16 iZoneId, Byte iMapIdx)
        {
            if (zoneMapList.ContainsKey(iZoneId))
            {
                if (zoneNbMaps[iZoneId] > iMapIdx)
                {
                    UInt16 mapId = 0;
                    if (zoneToMapNbToMapId.ContainsKey(iZoneId) && zoneToMapNbToMapId[iZoneId].ContainsKey((Byte)(iMapIdx)))
                    {
                        mapId = zoneToMapNbToMapId[iZoneId][(Byte)(iMapIdx)];
                    }
                    return (Byte)mapId;
                }
            }
            return Byte.MaxValue;
        }
        public static Boolean GetMapIndex(UInt16 iZoneId, Byte iMapId, out Byte oMapIdx)
        {
            oMapIdx = 0;
            if (!isInitialized)
            {
                init();
            }
            if (zoneMapList.ContainsKey(iZoneId))
            {
                if (zoneToMapNbToMapId.ContainsKey(iZoneId) && zoneToMapNbToMapId[iZoneId].ContainsValue(iMapId))
                {
                    Dictionary<Byte, UInt16> thisZone = zoneToMapNbToMapId[iZoneId];
                    foreach (KeyValuePair<Byte, UInt16> kvp in thisZone)
                    {
                        if (kvp.Value == iMapId)
                        {
                            oMapIdx = kvp.Key;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion Get Functions
        #endregion Public Member Functions

        #region Private Member Functions
        //private static Image loadImageFile(String fileName, int mapNb, List<Int16> iPosXs, List<Int16> iPosYs, Brush iColor)
        //{
        //    fileName += "_" + mapNb.ToString();
        //    return loadImageFile(fileName, iPosXs, iPosYs, iColor);
        //}
        private static Image loadImageFile(String fileName)
        {
            String fullPath;
            if (!System.IO.Path.IsPathRooted(mapsPath))
            {
                String exePath = Path.GetDirectoryName(Application.ExecutablePath);
                fullPath = exePath + mapsPath;
                fullPath = Path.GetFullPath(fullPath);
            }
            else
            {
                fullPath = mapsPath;
            }
            fullPath = Path.Combine(fullPath, fileName);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show("Could not find map file:\n" + fullPath);
                return null;
            }
            else
            {
                Bitmap origImage = (Bitmap)Image.FromFile(fullPath);
                System.Drawing.Imaging.PixelFormat frmt = origImage.PixelFormat;
                Bitmap tempBitmap;
                if (frmt == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    tempBitmap = new Bitmap(origImage.Width, origImage.Height);
                    Graphics g = Graphics.FromImage(tempBitmap);
                    g.DrawImage(origImage, 0, 0);
                }
                else
                {
                    tempBitmap = origImage;
                }

                return tempBitmap;
            }
        }
        private static Image loadImageResource(String fileName)
        {
            String fullFileName = "Iocaine2.Images." + fileName;
            try
            {
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                String[] resources = myAssembly.GetManifestResourceNames();
                //foreach (String str in resources)
                //{
                //    Console.WriteLine(str);
                //}
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
        #region File Management Related
        private static void checkMapFiles()
        {
            if(File.Exists("NO_MAP_CHECK.txt"))
            {
                return;
            }
            if (!Directory.Exists(localMapsDir))
            {
                Directory.CreateDirectory(localMapsDir);
            }
            if (!Directory.Exists(localMapImagesDir))
            {
                Directory.CreateDirectory(localMapImagesDir);
            }
            Boolean prompt = false;
            if (!Directory.Exists(mapsPath))
            {
                prompt = true;
            }
            else if (Directory.GetFiles(mapsPath).Length == 0)
            {
                prompt = true;
            }
            if (prompt)
            {
                //Prompt for user response.
                //Either download from server
                //or point to another location.
                //If another location, save that as a setting for next time.
                DialogResult result = DialogResult.Cancel;
                String message = "Iocaine now includes a map pack for 'Fish Stats' and\n";
                message += "possibly a radar section later on. The map pack doesn't appear\n";
                message += "to be downloaded here yet. Click Yes to download it now.\n\n";
                message += "Or, if you have it saved somewhere else already, click No to\n";
                message += "select the existing directory. (Select the folder with all\n";
                message += "of the map files in it (ie \\Maps\\Images\\).\n\n";
                message += "Cancel will take no action.";
                result = MessageBox.Show(message, "+=+=+  Map Pack Download  +=+=+", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    mapsPath = Path.GetDirectoryName(Application.ExecutablePath);
                    mapsPath = Path.GetFullPath(mapsPath + localMapImagesDir);
                    setMapsPath(mapsPath);
                    MessageBox.Show("This may take a minute, there's over 150MB of maps.\nPlease be patient.", "^.^b");
                    downloadMaps();
                }
                else if (result == DialogResult.No)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    String exePath = Path.GetDirectoryName(Application.ExecutablePath);
                    exePath = Path.GetFullPath(exePath);
                    fbd.SelectedPath = exePath;
                    fbd.ShowDialog();

                    String selFolder = fbd.SelectedPath;
                    mapsPath = selFolder;
                    setMapsPath(selFolder);
                }
            }
        }
        private static void downloadMaps()
        {
            downloadDirectoryPtr(defaultMapsDirFromRoot, mapsPath);
            //downloadDirectoryPtr(defaultMapsDirFromRoot + "/" + mapPackZipFile, mapsPath, getPassword());
            //if (File.Exists(Path.Combine(mapsPath, mapPackZipFile)))
            //{

            //}
        }
        #endregion File Management Related
        #endregion Private Member Functions
    }
}
