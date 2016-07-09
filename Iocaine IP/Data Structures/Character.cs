using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Logging;
using Iocaine2.Memory;

namespace Iocaine2.Char
{
    public class Character
    {
        public Character(String iName)
        {
            name = iName;
            doInit();
        }
        private String name = "";
        public String Name
        {
            get
            {
                return name;
            }
        }
        private UInt32 pointer = 0;
        private int index = 0;
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }
        private byte hpPerc = 100;
        public byte HpPerc
        {
            get
            {
                if (pointer != 0)
                {
                    hpPerc = MemReads.PCs.get_hp_perc(pointer);
                    return hpPerc;
                }
                else
                {
                    return 0;
                }
            }
        }
        private bool active = false;
        public bool Active
        {
            get
            {
                return active;
            }
        }
        private bool inRange = false;
        public bool InRange
        {
            get
            {
                return inRange;
            }
        }
        private float posX = 0;
        public float PosX
        {
            get
            {
                return posX;
            }
        }
        private float posY = 0;
        public float PosY
        {
            get
            {
                return posY;
            }
        }
        private float posZ = 0;
        public float PosZ
        {
            get
            {
                return posZ;
            }
        }
        private float posH = 0;
        public float PosH
        {
            get
            {
                return posH;
            }
        }
        private float distance = 0;
        /// <summary>
        /// Returns a live mem read of player's distance.
        /// </summary>
        public float Distance
        {
            get
            {
                distance = MemReads.PCs.get_distance(pointer);
                LoggingFunctions.Debug("Distance to " + name + " is " + distance.ToString(), LoggingFunctions.DBG_SCOPE.NPC_PC);
                return distance;
            }
        }
        public Byte Status
        {
            get
            {
                if (active)
                {
                    return MemReads.PCs.get_status(pointer);
                }
                else
                {
                    return 0;
                }
            }
        }

        private void doInit()
        {
            Update_Pointer();
            active = Check_Active();
            if (active)
            {
                Update_HP_Perc();
                Update_Position();
            }
        }
        /// <summary>
        /// Returns true only if the character's structure exists in the PC map
        /// AND is currently being updated with valid data.
        /// Returning false means either:
        /// 1. The character's info is not in the PC map.
        /// 2. The character's info IS in the PC map, but is not being updated
        ///    because they are out of range, logged, or zoned.
        /// </summary>
        /// <returns></returns>
        public bool Check_Active()
        {
            if (pointer != 0)
            {
                //String tempName = MemReads.info_pc_name(mainProc, mainMod, pointer);
                //active = (name == tempName.Substring(0, tempName.Length - 1));
                active = MemReads.PCs.get_pointer_valid(pointer);
            }
            else
            {
                active = false;
            }
            return active;
        }
        /// <summary>
        /// Checks that the character's data structure is still valid at the current pointer location.
        /// Returning true means that the player's info is loaded at the saved pointer,
        /// but it may not be updated if they are not active.
        /// </summary>
        /// <returns></returns>
        public bool Check_Pointer_Valid()
        {
            if (pointer == 0)
            {
                return false;
            }
            else
            {
                if (MemReads.PCs.get_name(pointer) == name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool Check_In_Range()
        {
            //Update_Position();
            inRange = Distance < Statics.Settings.PowerLevel.MaxCastDistance;
            return inRange;
        }
        /// <summary>
        /// Returns true if the character's data is still in the PC map, even if it is no longer active
        /// and being updated. Use the Check_Active to see if the structure is actively being updated.
        /// Returning false means that the character's name was not found in the PC map.
        /// </summary>
        /// <returns></returns>
        public bool Update_Pointer()
        {
            pointer = MemReads.PCs.get_pointer(name);
            return (pointer == 0) ? false : true;
        }
        public byte Update_HP_Perc()
        {
            hpPerc = MemReads.PCs.get_hp_perc(pointer);
            return hpPerc;
        }
        public bool Update_Position()
        {
            posX = MemReads.PCs.get_posx(pointer);
            posY = MemReads.PCs.get_posy(pointer);
            posZ = MemReads.PCs.get_posz(pointer);
            posH = MemReads.PCs.get_posh(pointer);
            LoggingFunctions.Debug("Updated pos from character is: " + posX.ToString() + ", " + posZ.ToString() + ", " + posY.ToString(), LoggingFunctions.DBG_SCOPE.NPC_PC);

            distance = MemReads.PCs.get_distance(pointer);
            return true;
        }
    }
}
