using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Media;
using System.Speech.Synthesis;
using System.Text;

namespace Iocaine2.Tools
{
    public static class Audio
    {
        private static SpeechSynthesizer speaker = new SpeechSynthesizer();
        private static SoundPlayer player = new SoundPlayer();
        public static void PlaySound(String str)
        {
            if ((str == "") || (str.ToLower() == "none"))
            {
                return;
            }
            if (str.Contains("\\"))
            {
                if (File.Exists(str))
                {
                    player.SoundLocation = str;
                    player.Play();
                }
            }
            else
            {
                speaker.SpeakAsync(str);
            }
        }
        public static void StopSound()
        {
            player.Stop();
            speaker.SpeakAsyncCancelAll();
        }
    }
}
