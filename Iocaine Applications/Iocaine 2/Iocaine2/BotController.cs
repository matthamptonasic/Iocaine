using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Bots
{
    public static class BotController
    {
        #region Members
        private static List<Bot> loadedBots = new List<Bot>();
        #endregion Members

        #region Properties
        public static List<Bot> LoadedBots
        {
            get
            {
                List<Bot> temp = new List<Bot>();
                foreach(Bot bot in loadedBots)
                {
                    temp.Add(bot);
                }
                return temp;
            }
        }
        #endregion Properties

        #region Public Methods
        public static Boolean LoadBot(String iFileName)
        {
            // TBD
            // Do file exists check.
            
            // Load the tab page.

            // Load bot settings.
            
            // Load bot parameters.

            return true;
        }
        public static Boolean UnloadBot(String iFileName)
        {


            return true;
        }
        public static void StopAllBots()
        {
            foreach(Bot bot in loadedBots)
            {
                bot.Stop();
            }
        }
        public static void PauseAllBots(Boolean iNow = true)
        {
            foreach(Bot bot in loadedBots)
            {
                if (bot.State == STATE.RUNNING)
                {
                    bot.Pause(iNow);
                }
            }
        }
        #endregion Public Methods
    }
}
