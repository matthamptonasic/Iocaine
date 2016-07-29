using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Media;
using System.Speech.Synthesis;
using System.Text;

namespace Iocaine2.Tools
{
    public class Audio
    {
        #region Private Members
        private SpeechSynthesizer speaker = new SpeechSynthesizer();
        private SoundPlayer player = new SoundPlayer();
        private bool enabled = true;
        private string message = "";
        private bool loop = false;
        #endregion Private Members

        #region Public Properties
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }
        public bool Loop
        {
            get
            {
                return loop;
            }
            set
            {
                loop = value;
            }
        }
        #endregion Public Properties

        #region Public Methods
        public void PlaySound(string str)
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
                    if (loop)
                    {
                        player.PlayLooping();
                    }
                    else
                    {
                        player.Play();
                    }
                }
            }
            else
            {
                //if (Statics.Settings.Alert.Enable && Statics.Settings.Alert.PlayMessage)
                if (enabled)
                {
                    speaker.SpeakAsync(str);
                }
                //if (Statics.Settings.Alert.LoopMessage)
                if(loop)
                {
                    message = str;
                    speaker.SpeakCompleted -= Speaker_SpeakCompleted;
                    speaker.SpeakCompleted += Speaker_SpeakCompleted;
                }
                //if (!Statics.Settings.Alert.Enable || !Statics.Settings.Alert.PlayMessage || !Statics.Settings.Alert.LoopMessage)
                if(!enabled || !loop)
                {
                    speaker.SpeakCompleted -= Speaker_SpeakCompleted;
                }
            }
        }
        public void StopSound()
        {
            player.Stop();
            speaker.SpeakAsyncCancelAll();
        }
        #endregion Public Methods

        #region Private Methods
        private void Speaker_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            PlaySound(message);
        }
        #endregion Private Methods
    }
}
