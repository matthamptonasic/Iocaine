using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iocaine2.Bots
{
    public static class BotController
    {
        #region Private Members
        private static List<Bot> loadedBots = new List<Bot>();
        #endregion Private Members

        #region Public Properties
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
        #endregion Public Properties

        #region Public Methods
        public static bool LoadBot(string iFileName)
        {
            // TBD
            // Do file exists check.
            
            // Load the tab page.

            // Load bot settings.
            
            // Load bot parameters.

            return true;
        }
        public static bool UnloadBot(string iFileName)
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
        public static void PauseAllBots(bool iNow = true)
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
