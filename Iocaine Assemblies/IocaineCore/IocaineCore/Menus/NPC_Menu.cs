using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Logging;
using Iocaine2.Memory;

namespace Iocaine2.Data.Client
{
    public class NPC_Menu
    {
        #region Enums
        #endregion Enums

        #region Private Members
        private string m_topText;
        private bool m_isTop;
        private NPCs.NPC_TYPE m_npcType;
        private List<NPCs.NPC_INFO> m_npcInfo;
        private const float m_npcInteractDist = 3.5f;
        private const float m_npcMaxXyDist = 10;
        private const float m_npcMaxZDist = 2;
        private bool m_npcsAreUnique = true;
        private List<NPC_MenuNode> m_nodes;
        private NPC_MenuParsingParameters m_parsingParams;
        #endregion Private Members

        #region Public Properties
        public List<NPCs.NPC_INFO> NpcInfoList
        {
            get
            {
                return m_npcInfo;
            }
            set
            {
                m_npcInfo = value;
            }
        }
        /// <summary>
        /// Specifies whether the menu for all NPCs of this type are unique.
        /// Unique example is the task delegator NPCs in Adoulin, each menu is different.
        /// Non-Unique example is the RoE NPCs, each menu is the same as the rest.
        /// </summary>
        public bool NpcsAreUnique
        {
            get
            {
                return m_npcsAreUnique;
            }
            set
            {
                m_npcsAreUnique = value;
            }
        }
        public NPC_MenuParsingParameters ParsingParams
        {
            get
            {
                return m_parsingParams;
            }
            set
            {
                m_parsingParams = value;
            }
        }
        #endregion Public Properties

        #region Constructors
        public NPC_Menu(NPCs.NPC_TYPE iNpcType, string iTopText, bool iIsTopMenu = false)
        {
            m_npcType = iNpcType;
            m_topText = iTopText;
            m_isTop = iIsTopMenu;
        }
        #endregion Constructors

        #region Public Methods
        #region Get/Add
        public void Add(NPC_MenuNode iNode)
        {
            if (iNode != null)
            {
                m_nodes.Add(iNode);
            }
        }
        public NPC_MenuNode GetNode(string iText, bool iMatchPartial = true)
        {
            foreach (NPC_MenuNode node in m_nodes)
            {
                if (iMatchPartial)
                {
                    if (node.Text.Contains(iText))
                    {
                        return node;
                    }
                }
                else
                {
                    if (node.Text == iText)
                    {
                        return node;
                    }
                }
            }
            return null;
        }
        #endregion Get/Add
        #region Menu Parsing
        public static bool ParseMenu(NPC_Menu iMenu, string iNPC = "")
        {
            if (!ChangeMonitor.LoggedIn || (iMenu == null))
            {
                return false;
            }
            // If the iNPC is empty, find the one in this zone. Exit if none.
            string npcName = "";
            bool inRange = false;
            bool mustMove = false;
            List<MemReads.NPCs.NPCInfoStruct> npcStructs = MemReads.NPCs.get_NPCInfoStructList(true);
            ushort currentZone = PlayerCache.Environment.ZoneId;
            float currentX = MemReads.Self.Position.get_x();
            float currentY = MemReads.Self.Position.get_y();
            float currentZ = MemReads.Self.Position.get_z();
            if (iNPC == "")
            {
                foreach (NPCs.NPC_INFO info in iMenu.m_npcInfo)
                {
                    if (info.Zone == currentZone)
                    {
                        // The NPC from the FFXINpcs library is in the same zone we're in.
                        // Check if that NPC is in the current NPC map in memory.
                        foreach (MemReads.NPCs.NPCInfoStruct npc in npcStructs)
                        {
                            if (MemReads.NPCs.getName(npc) == info.Name)
                            {
                                // If so, is it close enough to interact with.
                                float z_dist = npc.PosZ - currentZ;
                                float xy_dist = (float)Math.Sqrt(Math.Pow(npc.PosX - currentX, 2) + Math.Pow(npc.PosY - currentY, 2));
                                if ((xy_dist <= m_npcMaxXyDist) && (z_dist <= m_npcMaxZDist))
                                {
                                    inRange = true;
                                    if (xy_dist > m_npcInteractDist)
                                    {
                                        mustMove = true;
                                    }
                                    npcName = info.Name;
                                    break;
                                }
                            }
                        }
                    }
                    if (npcName != "")
                    {
                        break;
                    }
                }
            }
            else
            {
                // Check that the given NPC is in this zone and nearby.
                foreach (MemReads.NPCs.NPCInfoStruct npc in npcStructs)
                {
                    if (MemReads.NPCs.getName(npc) == iNPC)
                    {
                        // If so, is it close enough to interact with.
                        float z_dist = npc.PosZ - currentZ;
                        float xy_dist = (float)Math.Sqrt(Math.Pow(npc.PosX - currentX, 2) + Math.Pow(npc.PosY - currentY, 2));
                        if ((xy_dist <= m_npcMaxXyDist) && (z_dist <= m_npcMaxZDist))
                        {
                            inRange = true;
                            if (xy_dist > m_npcInteractDist)
                            {
                                mustMove = true;
                            }
                            npcName = iNPC;
                            break;
                        }
                    }
                }
            }
            if (npcName == "")
            {
                string msg = "Could not find ";
                if (iNPC == "")
                {
                    msg += "any NPC's for this menu type ";
                }
                else
                {
                    msg += "NPC " + iNPC + " ";
                }
                msg += "in range.";
                MessageBox.Show(msg);
                return false;
            }

            // Check if we're already in a menu. If so, try to exit out.
            if (MemReads.Windows.Menus.TextStyle.is_open())
            {
                if (!Tools.NPC_MenuNavigation.ExitMenu(iMenu))
                {
                    return false;
                }
            }

            // Target the nearby NPC.
            byte errCnt = 5;
            byte loopCnt = 0;
            while (loopCnt < errCnt)
            {
                if (Tools.Interaction.TargetNPC(npcName))
                {
                    break;
                }
                loopCnt++;
            }
            if (loopCnt == errCnt)
            {
                LoggingFunctions.Error("Could not target NPC " + npcName + ".");
                return false;
            }

            // Enter the menu at the top.


            // Parse it depth-first.

            return true;
        }
        #endregion Menu Parsing
        #endregion Public Methods

        #region Private Methods
        #region Menu Parsing
        #endregion Menu Parsing
        #endregion Private Methods
    }
}
