using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Logging;
using Iocaine2.Memory;

namespace Iocaine2.Tools
{

    public class VanaTime
    {
        #region Members
        private static DateTime unixBase = new DateTime(1970, 1, 1, 0, 0, 0);
        private static DateTime vanaBase = new DateTime(2002, 6, 23, 15, 0, 0);
        private static UInt64 gameStartOffsetVanaSeconds = 27933984000; //Number of vana seconds since time 0 when the game started.
        private static UInt64 gameStartOffsetEarthSeconds = 1024844400; //Number of earch seconds since 1/1/70 when the game started.
        private static UInt32 vanaTimeMultiplier = 25; //Vana time runs 25x faster than earth time.
        private double vanaSeconds;
        private int day;
        #region Testing
        private const bool testJpMidnight = false;
        private const int testMinutesToReset = 1;
        private static DateTime testTimeStarted;
        private static bool testTimeStartedSet = false;
        #endregion Testing
        #endregion Members
        #region Constructor
        public VanaTime(double vanaSecs) {
            vanaSeconds = vanaSecs;
            //Aikar's method:
            //int vday = abs(floor((( (27933984000 + ( (UTCTime - 1,024,844,400) * 25) ) % (8 * 86400)) / 86400)));

            //UTCTime - 1,024,844,400 = earth seconds since game start (6/23/02).
            // * 25 = vana seconds since 6/23/02.
            // + 27.9m = total vana seconds since vana time 0
            // % (8 * 86400) : 86400 = nb seconds in a day, so 8 * 86400 = nb seconds in a vana week.
            //    so total vana seconds since 0 modulus nb seconds in a week = nb seconds since week start.
            // / 86400 = day since week start.
            
            //Our method does the same thing leaving out the modulo part.
            day = (int)Math.Floor((decimal)vanaSecs / 86400);
        }
        #endregion Constructor
        #region Member Functions
        public static VanaTime Now
        {
            get
            {
                if (ChangeMonitor.LoggedIn)
                {
                    UInt64 earthSecondsSince1970 = MemReads.Environment.get_time();
                    return (new VanaTime(((earthSecondsSince1970 - gameStartOffsetEarthSeconds) * vanaTimeMultiplier) + gameStartOffsetVanaSeconds));
                }
                else
                {
                    return new VanaTime(gameStartOffsetVanaSeconds);
                }
            }
        }
        public static VanaTime EarthToVana(DateTime iEarthDateTime)
        {
            return new VanaTime((iEarthDateTime - vanaBase).TotalSeconds * vanaTimeMultiplier + 27933984000);
        }
        public static DateTime VanaToEarth(VanaTime iVanaDateTime)
        {
            double tempSec = iVanaDateTime.vanaSeconds;
            //MessageBox.Show("vanatime seconds: " + tempSec);
            long tempTicks = (long)tempSec * 1000 * 1000 * 10;
            long earthTicks = (tempTicks - (27933984000 * 1000 * 1000 * 10)) / vanaTimeMultiplier + vanaBase.Ticks;
            DateTime earthTime = new DateTime(earthTicks);
            return earthTime;
        }
        /// <summary>
        /// Returns the current time as converted from game memory.
        /// Time is set as UTC. Do not try to convert to UTC again as it will modify the time incorrectly.
        /// </summary>
        /// <returns></returns>
        public DateTime ToEarth()
        {
            if (ChangeMonitor.LoggedIn)
            {
                return EarthTime();
            }
            else
            {
                long tempTicks = (long)vanaSeconds * 1000 * 1000 * 10;
                long earthTicks = (tempTicks - (27933984000 * 1000 * 1000 * 10)) / vanaTimeMultiplier + vanaBase.Ticks;
                DateTime earthTime = new DateTime(earthTicks);
                return earthTime;
            }
        }
        public DateTime EarthTime()
        {
            if (ChangeMonitor.LoggedIn)
            {
                UInt64 earthSeconds = MemReads.Environment.get_time();
                return unixBase.AddSeconds((double)earthSeconds);
            }
            else
            {
                return DateTime.Now;
            }
        }
        public string DayName
        {
            get
            {
                switch (day % 8)
                {
                    case 0: return "Firesday";
                    case 1: return "Earthsday";
                    case 2: return "Watersday";
                    case 3: return "Windsday";
                    case 4: return "Iceday";
                    case 5: return "Lightningday";
                    case 6: return "Lightsday";
                    case 7: return "Darksday";
                    default: return "Unknown";
                }
            }
        }
        public byte Day
        {
            get
            {
                return (byte)(day % 8);
            }
        }
        public int DayMonth
        {
            get
            {
                return day % 30 + 1;
            }
        }
        public int Year
        {
            get
            {
                return (int)Math.Floor((decimal)day / 360);
            }
        }
        public int Month
        {
            get
            {
                return (int)Math.Floor((decimal)(day % 360) / 30 + 1);
            }
        }
        public int Hour
        {
            get
            {
                return (int)Math.Floor((decimal)(vanaSeconds / 3600) % 24);
            }
        }
        public int Minute
        {
            get
            {
                return (int)Math.Floor((decimal)(vanaSeconds / 60) % 60);
            }
        }
        public int Second
        {
            get
            {
                return (int)Math.Floor((decimal)vanaSeconds % 60);
            }
        }
        public int MoonPercent
        {
            get
            {
                return (int)Math.Floor(Math.Abs(((42 - (((decimal)day + 26) % 84)) * 100) / 42));
            }
        }
        public byte MoonPercentFullSwing
        {
            get
            {
                return (byte)(Math.Floor((((42 - (((decimal)day + 26) % 84)) * 100) / 42) + (decimal)100.5) % 201);
            }
        }
        public bool Waxing
        {
            get
            {
                int phase = (day + 26) % 84;
                int phasePerc = (int)(((42 - phase) * 100) / 42);
                return (phasePerc < 0);
            }
        }
        public string MoonPhase {
            get
            {
                int phase = (day + 26)%84;  //phase varies from 0-83
                //phase = 0,  phase% = +100
                //phase = 21, phase% =  +50
                //phase = 42, phase% =    0
                //phase = 63, phase% =  -50
                //phase = 83, phase% =  -98
                int phasePerc = (int)(((42 - phase) * 100) / 42);   //phasePerc varies from -100 to 100 (-100 being waxing to full, 100 being waning to 0)
                bool waxing = false;
                if (phasePerc < 0)
                {
                    waxing = true;
                }
                phasePerc = Math.Abs(phasePerc);
                phasePerc = (int)Math.Floor((double)phasePerc + 0.5);
                if ((phasePerc <= 5) && (waxing == true))
                {
                    return "New +";
                }
                else if ((phasePerc <= 10) && (waxing == false))    //verified
                {
                    return "New -";
                }
                else if ((phasePerc <= 38) && (waxing == true))     //verified
                {
                    return "Waxing Cr.";
                }
                else if ((phasePerc <= 43) && (waxing == false))
                {
                    return "Waning Cr.";
                }
                else if ((phasePerc <= 55) && (waxing == true))     //verified
                {
                    return "First Quarter";
                }
                else if ((phasePerc <= 60) && (waxing == false))
                {
                    return "Last Quarter";
                }
                else if ((phasePerc <= 88) && (waxing == true))     //verified
                {
                    return "Waxing Gb.";
                }
                else if ((phasePerc <= 93) && (waxing == false))    //verified
                {
                    return "Waning Gb.";
                }
                else if (waxing == true)
                {
                    return "Full +";
                }
                else
                {
                    return "Full -";
                }
            }
        }
        string MoonName(int i) {
            switch (i) {
                case 0: return "New";
                case 1: return "Waxing Crescent";
                case 2: return "First Quarter";
                case 3: return "Waxing Gibbous";
                case 4: return "Full";
                case 5: return "Waning Gibbous";
                case 6: return "Last Quarter";
                case 7: return "Waning Crescent";
            }
            return null;
        }
        public override string ToString() {
            return String.Format("{0}/{1}/{2}, {3}, {4:0#}:{5:0#}, {6}%", Month, DayMonth, Year, DayName, Hour, Minute, MoonPercent);
        }
        //seconds elapsed from start of program(moment when answer received from time server)
        //static int SecondsElapsed() {
        //    return (int)(DateTime.Now - localStartTime).TotalSeconds;
        //}

        // Other time functions.
        private static DateTime JapanTime
        {
            get
            {
                return Now.ToEarth().AddHours(9);
            }
        }
        public static DateTime LastJapaneseMidnight
        {
            get
            {
                DateTime localJpTime = JapanTime;
                if (!testJpMidnight)
                {
                    return new DateTime(localJpTime.Year, localJpTime.Month, localJpTime.Day, 0, 0, 0);
                }
                if (!testTimeStartedSet)
                {
                    testTimeStarted = Now.ToEarth().AddHours(9);
                    testTimeStartedSet = true;
                }
                TimeSpan sinceStart = localJpTime - testTimeStarted;
                if (sinceStart.TotalMinutes < testMinutesToReset)
                {
                    return new DateTime(localJpTime.Year, localJpTime.Month, localJpTime.Day, 0, 0, 0);
                }
                else
                {
                    return testTimeStarted.AddMinutes(testMinutesToReset);
                }
            }
        }
        public static DateTime LastJapaneseMidnightUTC
        {
            get
            {
                return LastJapaneseMidnight.AddHours(-9);
            }
        }
        #endregion Member Functions
    }
}
