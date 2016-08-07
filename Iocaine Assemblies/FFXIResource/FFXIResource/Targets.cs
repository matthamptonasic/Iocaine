using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Targets
    {
        #region Structures
        public struct TARGETS_INFO
        {
            public short ID;
            public bool Self;
            public bool Party;
            public bool Alliance;
            public bool Anyone;
            public bool Monster;
            public bool Trust;
            public bool Fellow;
            public bool Dead;
        }
        #endregion Structures

        #region Private Members
        private static bool initDone = false;
        private const short invalidId = 0x7fff;
        #endregion Private Members

        #region Public Properties
        public static short InvalidId
        {
            get
            {
                return invalidId;
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
        public static TARGETS_INFO GetTargetInfo(short iId)
        {
            FfxiResource.init();
            string filterString = "ID = " + iId;
            MainDatabase.TargetsRow[] targetsRows = (MainDatabase.TargetsRow[])FfxiResource.mainDb.Targets.Select(filterString);
            TARGETS_INFO info = new TARGETS_INFO();
            if (targetsRows.Length == 0)
            {
                info.ID = 0;
                info.Self = false;
                info.Party = false;
                info.Alliance = false;
                info.Anyone = false;
                info.Monster = false;
                info.Trust = false;
                info.Fellow = false;
                info.Dead = false;
            }
            else
            {
                info.ID = targetsRows[0].ID;
                info.Self = targetsRows[0].Self;
                info.Party = targetsRows[0].Party;
                info.Alliance = targetsRows[0].Alliance;
                info.Anyone = targetsRows[0].Anyone;
                info.Monster = targetsRows[0].Monster;
                info.Trust = targetsRows[0].Trust;
                info.Fellow = targetsRows[0].Fellow;
                info.Dead = targetsRows[0].Dead;
            }
            return info;
        }
        #endregion Public Methods
    }
}
