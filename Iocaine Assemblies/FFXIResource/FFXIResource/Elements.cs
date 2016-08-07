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
            public short ID;
            public string Name;
        }
        #endregion Structures

        #region Private Members
        private static bool initDone = false;
        private const short invalidElement = 0x7fff;
        #endregion Private Members

        #region Public Properties
        public static short InvalidElement
        {
            get
            {
                return invalidElement;
            }
        }
        #endregion Public Properties

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

        #region Public Methods
        /// <summary>
        /// Gets the element ID of the given name.
        /// </summary>
        /// <param name="iName">Name of the status effect to look up.
        /// Do NOT use quotation marks around the name.</param>
        /// <returns>ID of the element with the given name.</returns>
        public static short GetElementID(string iName)
        {
            FfxiResource.init();
            string filterString = "Name = '" + FfxiResource.EncodeApostrophy(iName) + "'";
            MainDatabase.ElementsRow[] elementRows = (MainDatabase.ElementsRow[])FfxiResource.mainDb.Elements.Select(filterString);
            if (elementRows.Length == 0)
            {
                return (short)FFXIEnums.ELEMENT.UNKNOWN;
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
        public static string GetElementName(short iId)
        {
            FfxiResource.init();
            string filterString = "ID = " + iId;
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
        #endregion Public Methods
    }
}
