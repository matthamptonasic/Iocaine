using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Elements
    {
        #region Structures
        public struct ELEMENTS_INFO
        {
            public Int16 ID;
            public String Name;
        }
        #endregion Structures
        #region Member Variables
        private static bool initDone = false;
        #endregion Member Variables
        #region Init
        internal static void init()
        {
            if (!initDone)
            {
                loadData();
                initDone = true;
            }
        }
        #endregion Init
        #region Get Functions
        /// <summary>
        /// Gets the element ID of the given name.
        /// </summary>
        /// <param name="iName">Name of the status effect to look up.
        /// Do NOT use quotation marks around the name.</param>
        /// <returns>ID of the element with the given name.</returns>
        public static Int16 GetElementID(String iName)
        {
            FfxiResource.init();
            String filterString = "Name = '" + FfxiResource.EncodeApostrophy(iName) + "'";
            MainDatabase.ElementsRow[] elementRows = (MainDatabase.ElementsRow[])FfxiResource.mainDb.Elements.Select(filterString);
            if (elementRows.Length == 0)
            {
                return (Int16)FFXIEnums.ELEMENT.UNKNOWN;
            }
            else
            {
                return elementRows[0].ID;
            }
        }
        /// <summary>
        /// Gets the element name of the given ID.
        /// </summary>
        /// <param name="iId">Element ID. Physical is -1, unknown is 8.</param>
        /// <returns>Name of element.</returns>
        public static String GetElementName(Int16 iId)
        {
            FfxiResource.init();
            String filterString = "ID = " + iId;
            MainDatabase.ElementsRow[] elementRows = (MainDatabase.ElementsRow[])FfxiResource.mainDb.Elements.Select(filterString);
            if (elementRows.Length == 0)
            {
                return FFXIEnums.ELEMENT.UNKNOWN.ToString();
            }
            else
            {
                return elementRows[0].Name;
            }
        }
        #endregion Get Functions
    }
}
