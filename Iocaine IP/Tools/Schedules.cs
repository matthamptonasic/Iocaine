using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Tools
{
    public static class Schedules
    {
        public static class Guilds
        {
            public static bool IsOpen(FFXIEnums.GUILDS iGuild, FFXIEnums.ZONES iZone)
            {
                Byte dayOpen = 0;
                Byte hourOpen = 0;
                UInt16 minToOpen = 0;
                return IsOpen(iGuild, iZone, ref dayOpen, ref hourOpen, ref minToOpen);
            }
            public static bool IsOpen(FFXIEnums.GUILDS iGuild, FFXIEnums.ZONES iZone, ref Byte oDayOpen, ref Byte oHourOpen, ref ushort oMinutesToOpen)
            {
                Byte guildDayOff = 0;
                Byte guildHourOpen = 0;
                Byte guildHourClose = 0;
                if (!NPCs.GetGuildInfo(iGuild, iZone, ref guildHourOpen, ref guildHourClose, ref guildDayOff))
                {
                    //The NPC name didn't show up as a guild merchant even though they should have.
                    //So something's wrong, but we'll just log the error and continue (return false).
                    LoggingFunctions.Error("In Schedules::Guilds::IsOpen: NPCs.GuildInfo returned false.");
                    return true;
                }
                else
                {
                    return IsOpen(guildHourOpen, guildHourClose, guildDayOff, ref oDayOpen, ref oHourOpen, ref oMinutesToOpen);
                }
            }
            public static bool IsOpen(String iNpcName)
            {
                Byte dayOpen = 0;
                Byte hourOpen = 0;
                UInt16 minToOpen = 0;
                return IsOpen(iNpcName, ref dayOpen, ref hourOpen, ref minToOpen);
            }
            public static bool IsOpen(String iNpcName, ref Byte oDayOpen, ref Byte oHourOpen, ref ushort oMinutesToOpen)
            {
                Byte guildDayOff = 0;
                Byte guildHourOpen = 0;
                Byte guildHourClose = 0;
                if (!NPCs.GetGuildInfo(iNpcName, ref guildHourOpen, ref guildHourClose, ref guildDayOff))
                {
                    //The NPC name didn't show up as a guild merchant even though they should have.
                    //So something's wrong, but we'll just log the error and continue (assuming it's a normal npc, so return true).
                    LoggingFunctions.Error("In Schedules::Guilds::IsOpen: NPCs.GuildInfo returned false.");
                    return true;
                }
                else
                {
                    return IsOpen(guildHourOpen, guildHourClose, guildDayOff, ref oDayOpen, ref oHourOpen, ref oMinutesToOpen);
                }
            }
            private static bool IsOpen(Byte iHourOpen, Byte iHourClose, Byte iDayOff, ref Byte oDayOpen, ref Byte oHourOpen, ref ushort oMinutesToOpen)
            {
                VanaTime vTimeNow = VanaTime.Now;
                Byte currentMin = (Byte)vTimeNow.Minute;
                Byte currentHour = (Byte)vTimeNow.Hour;
                Byte currentDay = vTimeNow.Day;
                oHourOpen = iHourOpen;
                if ((currentDay == iDayOff) || (currentHour >= iHourClose))
                {
                    oDayOpen = (Byte)((currentDay + 1) % 8);
                    //Minutes to open will be the open hour * 60 + the minutes remaining in this day.
                    //Ex: Opens tomorrow at 1:00 and it's now 12:40 => (1*60) + ((24-12)*60) - 40 => 60 + 12*60 - 40 => 740
                    oMinutesToOpen = (ushort)((iHourOpen * 60) + ((24 - currentHour) * 60) - currentMin);
                    return false;
                }
                else if (currentHour < iHourOpen)
                {
                    oDayOpen = currentDay;
                    //It opens today, just not yet. Minutes is just the number of minutes until it opens.
                    //Ex: Opens at 3:00 and it's now 2:15 => (3 - 2) * 60 - 15 => 1 * 60 - 15 => 45
                    //Ex: Opens at 5:00 and it's now 1:20 => (5 - 1) * 60 - 20 => 4 * 60 - 20 => 240 - 20 => 220
                    oMinutesToOpen = (ushort)(((iHourOpen - currentHour) * 60) - currentMin);
                    return false;
                }
                else
                {
                    oDayOpen = currentDay;
                    oMinutesToOpen = 0;
                    return true;
                }
            }
        }
        public static class Airships
        {

        }
        public static class Ferries
        {

        }
    }
}
