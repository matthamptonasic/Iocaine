using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;

using Iocaine2.Data.Client;
using Iocaine2.Data.Structures;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Properties;
using Iocaine2.Settings;

namespace Iocaine2
{
    //This file is for functions directly related to the Skill Up bot
    partial class Iocaine_2_Form
    {
        #region Enums
        public enum SU_TYPE
        {
            ENHANCING = 0,
            HEALING = 1,
            SUMMONING = 2,
            BLUE = 3,
            NINJUTSU = 4,
            SINGING = 5,
            STRING_INSTR = 6,
            WIND_INSTR = 7,
            GEO = 8,
            RUNE = 9
        }
        #endregion //Enums
        #region Members Section
        #region Threads
        private Thread SU_RunThread = null;
        #endregion Threads
        #region Settings
        private byte SU_CurrentSkill = (byte)SU_TYPE.ENHANCING;
        private int SU_SkillLevel = 0;
        private bool SU_StopAtGivenSkill = false;
        private bool SU_GoToCap = true;
        private bool SU_DoOnly = false;
        private int SU_DoOnlyCount = 10;
        private bool SU_DoneStop = true;
        private bool SU_DoneLogout = false;
        private bool SU_DoneShutdown = false;
        private bool SU_GiveRestCommand = false;
        private String SU_RestCommand = "/item \"Ginger Cookie\" <me>";
        private bool SU_GiveLogoutCommand = false;
        private String SU_LogoutCommand = "/ma Warp <me>";
        private bool SU_DelayBetweenCasts = false;
        private uint SU_DelayValueBetweenCasts = 0;
        #endregion Settings
        #endregion //Members Section
        #region GUI Synchronization
        #region Delegate Declarations
        private delegate void SU_SetGoToCapRadioButtonDelegate(bool value);
        private SU_SetGoToCapRadioButtonDelegate SU_SetGoToCapRadioButtonPtr;
        private delegate bool getGoToCapRadioButtonDelegate();
        private getGoToCapRadioButtonDelegate getGoToCapRadioButtonCallBack;
        private delegate void SU_SetStopAtRadioButtonDelegate(bool value);
        private SU_SetStopAtRadioButtonDelegate SU_SetStopAtRadioButtonCallBack;
        private delegate bool getStopAtRadioButtonDelegate();
        private getStopAtRadioButtonDelegate getStopAtRadioButtonCallBack;
        private delegate void SU_SetStopAtTextBoxTextDelegate(String text);
        private SU_SetStopAtTextBoxTextDelegate SU_SetStopAtTextBoxTextCallBack;
        private delegate String getStopAtTextBoxTextDelegate();
        private getStopAtTextBoxTextDelegate getStopAtTextBoxTextCallBack;
        private delegate void SU_SetDoOnlyRadioButtonDelegate(bool value);
        private SU_SetDoOnlyRadioButtonDelegate SU_SetDoOnlyRadioButtonCallBack;
        private delegate bool getDoOnlyRadioButtonDelegate();
        private getDoOnlyRadioButtonDelegate getDoOnlyRadioButtonCallBack;
        private delegate void SU_SetDoOnlyTextBoxTextDelegate(String text);
        private SU_SetDoOnlyTextBoxTextDelegate SU_SetDoOnlyTextBoxTextCallBack;
        private delegate String getDoOnlyTextBoxTextDelegate();
        private getDoOnlyTextBoxTextDelegate getDoOnlyTextBoxTextCallBack;
        private delegate void SU_SetStopRadioButtonDelegate(bool value);
        private SU_SetStopRadioButtonDelegate SU_SetStopRadioButtonCallBack;
        private delegate bool getStopRadioButtonDelegate();
        private getStopRadioButtonDelegate getStopRadioButtonCallBack;
        private delegate void SU_SetLogoutRadioButtonDelegate(bool value);
        private SU_SetLogoutRadioButtonDelegate SU_SetLogoutRadioButtonCallBack;
        private delegate bool getLogoutRadioButtonDelegate();
        private getLogoutRadioButtonDelegate getLogoutRadioButtonCallBack;
        private delegate void SU_SetShutdownRadioButtonDelegate(bool value);
        private SU_SetShutdownRadioButtonDelegate SU_SetShutdownRadioButtonCallBack;
        private delegate bool getShutdownRadioButtonDelegate();
        private getShutdownRadioButtonDelegate getShutdownRadioButtonCallBack;
        private delegate void SU_SetLogoutCommandCheckBoxDelegate(bool value);
        private SU_SetLogoutCommandCheckBoxDelegate SU_SetLogoutCommandCheckBoxCallBack;
        private delegate bool getLogoutCommandCheckBoxDelegate();
        private getLogoutCommandCheckBoxDelegate getLogoutCommandCheckBoxCallBack;
        private delegate void SU_SetLogoutCommandTextBoxTextDelegate(String text);
        private SU_SetLogoutCommandTextBoxTextDelegate SU_SetLogoutCommandTextBoxTextCallBack;
        private delegate String getLogoutCommandTextBoxTextDelegate();
        private getLogoutCommandTextBoxTextDelegate getLogoutCommandTextBoxTextCallBack;
        private delegate void SU_SetRestCommandCheckBoxDelegate(bool value);
        private SU_SetRestCommandCheckBoxDelegate SU_SetRestCommandCheckBoxCallBack;
        private delegate bool getRestCommandCheckBoxDelegate();
        private getRestCommandCheckBoxDelegate getRestCommandCheckBoxCallBack;
        private delegate void SU_SetRestCommandTextBoxTextDelegate(String text);
        private SU_SetRestCommandTextBoxTextDelegate SU_SetRestCommandTextBoxTextCallBack;
        private delegate String getRestCommandTextBoxTextDelegate();
        private getRestCommandTextBoxTextDelegate getRestCommandTextBoxTextCallBack;
        public delegate void SU_SetSUStartButtonDelegate(String text, System.Drawing.Color color);
        private SU_SetSUStartButtonDelegate SU_SetSUStartButtonCallBack;
        private delegate void setEnhancingRadioButtonDelegate(bool value);
        private setEnhancingRadioButtonDelegate setEnhancingRadioButtonCallBack;
        private delegate void SU_SetHealingRadioButtonDelegate(bool value);
        private SU_SetHealingRadioButtonDelegate SU_SetHealingRadioButtonCallBack;
        private delegate void SU_SetSummoningRadioButtonDelegate(bool value);
        private SU_SetSummoningRadioButtonDelegate SU_SetSummoningRadioButtonCallBack;
        private delegate void SU_SetBlueRadioButtonDelegate(bool value);
        private SU_SetBlueRadioButtonDelegate SU_SetBlueRadioButtonCallBack;
        private delegate void SU_SetNinjutsuRadioButtonDelegate(bool value);
        private SU_SetNinjutsuRadioButtonDelegate SU_SetNinjutsuRadioButtonCallBack;
        private delegate void SU_SetSingingRadioButtonDelegate(bool value);
        private SU_SetSingingRadioButtonDelegate SU_SetSingingRadioButtonCallBack;
        private delegate void SU_SetStringInstrumentRadioButtonDelegate(bool value);
        private SU_SetStringInstrumentRadioButtonDelegate SU_SetStringInstrumentRadioButtonCallBack;
        private delegate void SU_SetWindInstrumentRadioButtonDelegate(bool value);
        private SU_SetWindInstrumentRadioButtonDelegate SU_SetWindInstrumentRadioButtonCallBack;
        private delegate void SU_SetGeoRadioButtonDelegate(bool value);
        private SU_SetGeoRadioButtonDelegate SU_SetGeoRadioButtonCallBack;
        private delegate void setRuneRadioButtonDelegate(bool value);   //Left off TBD
        private setRuneRadioButtonDelegate setRuneRadioButtonCallBack;
        private delegate int getFullCommandListBoxCountDelegate();
        private getFullCommandListBoxCountDelegate getFullCommandListBoxCountCallBack;
        private delegate void clearFullCommandListBoxDelegate();
        private clearFullCommandListBoxDelegate clearFullCommandListBoxCallBack;
        private delegate void SU_SetFullCommandLBDataSourceDelegate(List<SpellCommand> iCmdList);
        private SU_SetFullCommandLBDataSourceDelegate SU_SetFullCommandLBSourcePtr;
        private delegate void addFullCommandListBoxItemDelegate(Command cmd);
        private addFullCommandListBoxItemDelegate addFullCommandListBoxItemCallBack;
        private delegate void addDoCommandListBoxItemDelegate(Command cmd);
        private addDoCommandListBoxItemDelegate addDoCommandListBoxItemCallBack;
        private delegate void SUSetDelayBetweenCastsChkBDelegate(bool iValue);
        private SUSetDelayBetweenCastsChkBDelegate SUSetDelayBetweenCastsChkBCallBack;
        private delegate void SUSetDelayValueBetweenCastBDelegate(uint iValue);
        private SUSetDelayValueBetweenCastBDelegate SUSetDelayValueBetweenCastsCallBack;
        #endregion Delegate Declarations
        #region Delegate Inits
        private void createSUDelegates()
        {
            if (SU_SetGoToCapRadioButtonPtr == null)
            {
                SU_SetGoToCapRadioButtonPtr = new SU_SetGoToCapRadioButtonDelegate(setGoToCapRadioButtonCallBackFunction);
                getGoToCapRadioButtonCallBack = new getGoToCapRadioButtonDelegate(getGoToCapRadioButtonCallBackFunction);
                SU_SetStopAtRadioButtonCallBack = new SU_SetStopAtRadioButtonDelegate(setStopAtRadioButtonCallBackFunction);
                getStopAtRadioButtonCallBack = new getStopAtRadioButtonDelegate(getStopAtRadioButtonCallBackFunction);
                SU_SetStopAtTextBoxTextCallBack = new SU_SetStopAtTextBoxTextDelegate(setStopAtTextBoxTextCallBackFunction);
                getStopAtTextBoxTextCallBack = new getStopAtTextBoxTextDelegate(getStopAtTextBoxTextCallBackFunction);
                SU_SetDoOnlyRadioButtonCallBack = new SU_SetDoOnlyRadioButtonDelegate(setDoOnlyRadioButtonCallBackFunction);
                getDoOnlyRadioButtonCallBack = new getDoOnlyRadioButtonDelegate(getDoOnlyRadioButtonCallBackFunction);
                SU_SetDoOnlyTextBoxTextCallBack = new SU_SetDoOnlyTextBoxTextDelegate(setDoOnlyTextBoxTextCallBackFunction);
                getDoOnlyTextBoxTextCallBack = new getDoOnlyTextBoxTextDelegate(getDoOnlyTextBoxTextCallBackFunction);
                SU_SetStopRadioButtonCallBack = new SU_SetStopRadioButtonDelegate(setStopRadioButtonCallBackFunction);
                getStopRadioButtonCallBack = new getStopRadioButtonDelegate(getStopRadioButtonCallBackFunction);
                SU_SetLogoutRadioButtonCallBack = new SU_SetLogoutRadioButtonDelegate(setLogoutRadioButtonCallBackFunction);
                getLogoutRadioButtonCallBack = new getLogoutRadioButtonDelegate(getLogoutRadioButtonCallBackFunction);
                SU_SetShutdownRadioButtonCallBack = new SU_SetShutdownRadioButtonDelegate(setShutdownRadioButtonCallBackFunction);
                getShutdownRadioButtonCallBack = new getShutdownRadioButtonDelegate(getShutdownRadioButtonCallBackFunction);
                SU_SetLogoutCommandCheckBoxCallBack = new SU_SetLogoutCommandCheckBoxDelegate(setLogoutCommandCheckBoxCallBackFunction);
                getLogoutCommandCheckBoxCallBack = new getLogoutCommandCheckBoxDelegate(getLogoutCommandCheckBoxCallBackFunction);
                SU_SetLogoutCommandTextBoxTextCallBack = new SU_SetLogoutCommandTextBoxTextDelegate(setLogoutCommandTextBoxTextCallBackFunction);
                getLogoutCommandTextBoxTextCallBack = new getLogoutCommandTextBoxTextDelegate(getLogoutCommandTextBoxTextCallBackFunction);
                SU_SetRestCommandCheckBoxCallBack = new SU_SetRestCommandCheckBoxDelegate(setRestCommandCheckBoxCallBackFunction);
                getRestCommandCheckBoxCallBack = new getRestCommandCheckBoxDelegate(getRestCommandCheckBoxCallBackFunction);
                SU_SetRestCommandTextBoxTextCallBack = new SU_SetRestCommandTextBoxTextDelegate(setRestCommandTextBoxTextCallBackFunction);
                getRestCommandTextBoxTextCallBack = new getRestCommandTextBoxTextDelegate(getRestCommandTextBoxTextCallBackFunction);
                SU_SetSUStartButtonCallBack = new SU_SetSUStartButtonDelegate(setSUStartButtonCallBackFunction);
                setEnhancingRadioButtonCallBack = new setEnhancingRadioButtonDelegate(setEnhancingRadioButtonCallBackFunction);
                SU_SetHealingRadioButtonCallBack = new SU_SetHealingRadioButtonDelegate(setHealingRadioButtonCallBackFunction);
                SU_SetSummoningRadioButtonCallBack = new SU_SetSummoningRadioButtonDelegate(setSummoningRadioButtonCallBackFunction);
                SU_SetBlueRadioButtonCallBack = new SU_SetBlueRadioButtonDelegate(setBlueRadioButtonCallBackFunction);
                SU_SetNinjutsuRadioButtonCallBack = new SU_SetNinjutsuRadioButtonDelegate(setNinjutsuRadioButtonCallBackFunction);
                SU_SetSingingRadioButtonCallBack = new SU_SetSingingRadioButtonDelegate(setSingingRadioButtonCallBackFunction);
                SU_SetStringInstrumentRadioButtonCallBack = new SU_SetStringInstrumentRadioButtonDelegate(setStringInstrumentRadioButtonCallBackFunction);
                SU_SetWindInstrumentRadioButtonCallBack = new SU_SetWindInstrumentRadioButtonDelegate(setWindInstrumentRadioButtonCallBackFunction);
                SU_SetGeoRadioButtonCallBack = new SU_SetGeoRadioButtonDelegate(setGeoRadioButtonCallBackFunction);
                setRuneRadioButtonCallBack = new setRuneRadioButtonDelegate(setRuneRadioButtonCallBackFunction);
                getFullCommandListBoxCountCallBack = new getFullCommandListBoxCountDelegate(getFullCommandListBoxCountCallBackFunction);
                clearFullCommandListBoxCallBack = new clearFullCommandListBoxDelegate(clearFullCommandListBoxCallBackFunction);
                addFullCommandListBoxItemCallBack = new addFullCommandListBoxItemDelegate(addFullCommandListBoxItemCallBackFunction);
                SU_SetFullCommandLBSourcePtr = new SU_SetFullCommandLBDataSourceDelegate(SU_SetFullCommandLBSourceCBF);
                addDoCommandListBoxItemCallBack = new addDoCommandListBoxItemDelegate(addDoCommandListBoxItemCallBackFunction);
                SUSetDelayBetweenCastsChkBCallBack = new SUSetDelayBetweenCastsChkBDelegate(SUSetDelayBetweenCastsChkBCallBackFunction);
                SUSetDelayValueBetweenCastsCallBack = new SUSetDelayValueBetweenCastBDelegate(SUSetDelayValueBetweenCastsCallBackFunction);
            }
        }
        #endregion Delegate Inits
        #region Call Back Functions
        private void setGoToCapRadioButton(bool value)
        {
            try
            {
                SUGoToCapRadioButton.BeginInvoke(SU_SetGoToCapRadioButtonPtr, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting go to cap RB: " + e.ToString());
            }
        }
        private void setGoToCapRadioButtonCallBackFunction(bool value)
        {
            SUGoToCapRadioButton.Checked = value;
        }
        private bool getGoToCapRadioButton()
        {
            try
            {
                return (bool)SUGoToCapRadioButton.Invoke(getGoToCapRadioButtonCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting go to cap RB: " + e.ToString());
                return false;
            }
        }
        private bool getGoToCapRadioButtonCallBackFunction()
        {
            return SUGoToCapRadioButton.Checked;
        }
        private void setStopAtRadioButton(bool value)
        {
            try
            {
                SUStopAtRadioButton.BeginInvoke(SU_SetStopAtRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting Stop at skill RB: " + e.ToString());
            }
        }
        private void setStopAtRadioButtonCallBackFunction(bool value)
        {
            SUStopAtRadioButton.Checked = value;
        }
        private bool getStopAtRadioButton()
        {
            try
            {
                return (bool)SUStopAtRadioButton.Invoke(getStopAtRadioButtonCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting Stop at skill RB: " + e.ToString());
                return false;
            }
        }
        private bool getStopAtRadioButtonCallBackFunction()
        {
            return SUStopAtRadioButton.Checked;
        }
        private void setStopAtTextBoxText(String text)
        {
            try
            {
                SUStopAtTextBox.BeginInvoke(SU_SetStopAtTextBoxTextCallBack, new object[] { text });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting Stop at skill TB: " + e.ToString());
            }
        }
        private void setStopAtTextBoxTextCallBackFunction(String text)
        {
            SUStopAtTextBox.Text = text;
        }
        private String getStopAtTextBoxText()
        {
            try
            {
                return (String)SUStopAtTextBox.Invoke(getStopAtTextBoxTextCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting Stop at skill TB: " + e.ToString());
                return "ERROR";
            }
        }
        private String getStopAtTextBoxTextCallBackFunction()
        {
            return SUStopAtTextBox.Text;
        }
        private void setDoOnlyRadioButton(bool value)
        {
            try
            {
                SUDoOnlyRadioButton.BeginInvoke(SU_SetDoOnlyRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting Do only RB: " + e.ToString());
            }
        }
        private void setDoOnlyRadioButtonCallBackFunction(bool value)
        {
            SUDoOnlyRadioButton.Checked = value;
        }
        private bool getDoOnlyRadioButton()
        {
            try
            {
                return (bool)SUDoOnlyRadioButton.Invoke(getDoOnlyRadioButtonCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting Do only RB: " + e.ToString());
                return false;
            }
        }
        private bool getDoOnlyRadioButtonCallBackFunction()
        {
            return SUDoOnlyRadioButton.Checked;
        }
        private void setDoOnlyTextBoxText(String text)
        {
            try
            {
                SUDoOnlyTextBox.BeginInvoke(SU_SetDoOnlyTextBoxTextCallBack, new object[] { text });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting Do only TB: " + e.ToString());
            }
        }
        private void setDoOnlyTextBoxTextCallBackFunction(String text)
        {
            SUDoOnlyTextBox.Text = text;
        }
        private String getDoOnlyTextBoxText()
        {
            try
            {
                return (String)SUDoOnlyTextBox.Invoke(getDoOnlyTextBoxTextCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting Do only TB: " + e.ToString());
                return "ERROR";
            }
        }
        private String getDoOnlyTextBoxTextCallBackFunction()
        {
            return SUDoOnlyTextBox.Text;
        }
        private void setStopRadioButton(bool value)
        {
            try
            {
                SUStopRadioButton.BeginInvoke(SU_SetStopRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting Stop RB: " + e.ToString());
            }
        }
        private void setStopRadioButtonCallBackFunction(bool value)
        {
            SUStopRadioButton.Checked = value;
        }
        private bool getStopRadioButton()
        {
            try
            {
                return (bool)SUStopRadioButton.Invoke(getStopRadioButtonCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting Stop RB: " + e.ToString());
                return false;
            }
        }
        private bool getStopRadioButtonCallBackFunction()
        {
            return SUStopRadioButton.Checked;
        }
        private void setLogoutRadioButton(bool value)
        {
            try
            {
                SULogoutRadioButton.BeginInvoke(SU_SetLogoutRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting logout RB: " + e.ToString());
            }
        }
        private void setLogoutRadioButtonCallBackFunction(bool value)
        {
            SULogoutRadioButton.Checked = value;
        }
        private bool getLogoutRadioButton()
        {
            try
            {
                return (bool)SULogoutRadioButton.Invoke(getLogoutRadioButtonCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting logout RB: " + e.ToString());
                return false;
            }
        }
        private bool getLogoutRadioButtonCallBackFunction()
        {
            return SULogoutRadioButton.Checked;
        }
        private void setShutdownRadioButton(bool value)
        {
            try
            {
                SUShutdownRadioButton.BeginInvoke(SU_SetShutdownRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting shutdown RB: " + e.ToString());
            }
        }
        private void setShutdownRadioButtonCallBackFunction(bool value)
        {
            SUShutdownRadioButton.Checked = value;
        }
        private bool getShutdownRadioButton()
        {
            try
            {
                return (bool)SUShutdownRadioButton.Invoke(getShutdownRadioButtonCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting shutdown RB: " + e.ToString());
                return false;
            }
        }
        private bool getShutdownRadioButtonCallBackFunction()
        {
            return SUShutdownRadioButton.Checked;
        }
        private void setLogoutCommandCheckBox(bool value)
        {
            try
            {
                SULogoutCommandCheckBox.BeginInvoke(SU_SetLogoutCommandCheckBoxCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting logout Checkbox: " + e.ToString());
            }
        }
        private void setLogoutCommandCheckBoxCallBackFunction(bool value)
        {
            SULogoutCommandCheckBox.Checked = value;
        }
        private bool getLogoutCommandCheckBox()
        {
            try
            {
                return (bool)SULogoutCommandCheckBox.Invoke(getLogoutCommandCheckBoxCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting logout Checkbox: " + e.ToString());
                return false;
            }
        }
        private bool getLogoutCommandCheckBoxCallBackFunction()
        {
            return SULogoutCommandCheckBox.Checked;
        }
        private void setLogoutCommandTextBoxText(String text)
        {
            try
            {
                SULogoutCommandTextBox.BeginInvoke(SU_SetLogoutCommandTextBoxTextCallBack, new object[] { text });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting Logout Command TB: " + e.ToString());
            }
        }
        private void setLogoutCommandTextBoxTextCallBackFunction(String text)
        {
            SULogoutCommandTextBox.Text = text;
        }
        private String getLogoutCommandTextBoxText()
        {
            try
            {
                return (String)SULogoutCommandTextBox.Invoke(getLogoutCommandTextBoxTextCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting Logout Command TB: " + e.ToString());
                return "ERROR";
            }
        }
        private String getLogoutCommandTextBoxTextCallBackFunction()
        {
            return SULogoutCommandTextBox.Text;
        }
        private void setRestCommandCheckBox(bool value)
        {
            try
            {
                SURestCommandCheckBox.BeginInvoke(SU_SetRestCommandCheckBoxCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting rest command Checkbox: " + e.ToString());
            }
        }
        private void setRestCommandCheckBoxCallBackFunction(bool value)
        {
            SURestCommandCheckBox.Checked = value;
        }
        private bool getRestCommandCheckBox()
        {
            try
            {
                return (bool)SURestCommandCheckBox.Invoke(SU_SetRestCommandCheckBoxCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting rest command Checkbox: " + e.ToString());
                return false;
            }
        }
        private bool getRestCommandCheckBoxCallBackFunction()
        {
            return SURestCommandCheckBox.Checked;
        }
        private void setRestCommandTextBoxText(String text)
        {
            try
            {
                SURestCommandTextBox.BeginInvoke(SU_SetRestCommandTextBoxTextCallBack, new object[] { text });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting Rest Command TB: " + e.ToString());
            }
        }
        private void setRestCommandTextBoxTextCallBackFunction(String text)
        {
            SURestCommandTextBox.Text = text;
        }
        private String getRestCommandTextBoxText()
        {
            try
            {
                return (String)SURestCommandTextBox.Invoke(getRestCommandTextBoxTextCallBack);

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Getting Rest Command TB: " + e.ToString());
                return "ERROR";
            }
        }
        private String getRestCommandTextBoxTextCallBackFunction()
        {
            return SURestCommandTextBox.Text;
        }
        private void setSUStartButton(String text, System.Drawing.Color color)
        {
            try
            {
                SU_Start_Button.BeginInvoke(SU_SetSUStartButtonCallBack, new object[] { text, color });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting Start Button Properties: " + e.ToString());
            }
        }
        private void setSUStartButtonCallBackFunction(String text, System.Drawing.Color color)
        {
            SU_Start_Button.UseMnemonic = true;
            SU_Start_Button.Text = text;
            SU_Start_Button.BackColor = color;
        }
        private void setEnhancingRadioButton(bool value)
        {
            try
            {
                suEnhancingRadioButton.BeginInvoke(setEnhancingRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting enhancing RB: " + e.ToString());
            }
        }
        private void setEnhancingRadioButtonCallBackFunction(bool value)
        {
            suEnhancingRadioButton.Checked = value;
        }
        private void setHealingRadioButton(bool value)
        {
            try
            {
                suHealingRadioButton.BeginInvoke(SU_SetHealingRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting healing RB: " + e.ToString());
            }
        }
        private void setHealingRadioButtonCallBackFunction(bool value)
        {
            suHealingRadioButton.Checked = value;
        }
        private void setSummoningRadioButton(bool value)
        {
            try
            {
                suSummoningRadioButton.BeginInvoke(SU_SetSummoningRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting summoning RB: " + e.ToString());
            }
        }
        private void setSummoningRadioButtonCallBackFunction(bool value)
        {
            suSummoningRadioButton.Checked = value;
        }
        private void setBlueRadioButton(bool value)
        {
            try
            {
                suBlueRadioButton.BeginInvoke(SU_SetBlueRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting blue RB: " + e.ToString());
            }
        }
        private void setBlueRadioButtonCallBackFunction(bool value)
        {
            suBlueRadioButton.Checked = value;
        }
        private void setNinjutsuRadioButton(bool value)
        {
            try
            {
                suNinjutsuRadioButton.BeginInvoke(SU_SetNinjutsuRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting ninjutsu RB: " + e.ToString());
            }
        }
        private void setNinjutsuRadioButtonCallBackFunction(bool value)
        {
            suNinjutsuRadioButton.Checked = value;
        }
        private void setSingingRadioButton(bool value)
        {
            try
            {
                suSingingRadioButton.BeginInvoke(SU_SetSingingRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting singing RB: " + e.ToString());
            }
        }
        private void setSingingRadioButtonCallBackFunction(bool value)
        {
            suSingingRadioButton.Checked = value;
        }
        private void setStringInstrumentRadioButton(bool value)
        {
            try
            {
                suStringInstrumentRadioButton.BeginInvoke(SU_SetStringInstrumentRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting stringed instr. RB: " + e.ToString());
            }
        }
        private void setStringInstrumentRadioButtonCallBackFunction(bool value)
        {
            suStringInstrumentRadioButton.Checked = value;
        }
        private void setWindInstrumentRadioButton(bool value)
        {
            try
            {
                suWindInstrumentRadioButton.BeginInvoke(SU_SetWindInstrumentRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting wind instr. RB: " + e.ToString());
            }
        }
        private void setWindInstrumentRadioButtonCallBackFunction(bool value)
        {
            suWindInstrumentRadioButton.Checked = value;
        }
        private void setGeoRadioButton(bool value)
        {
            try
            {
                suGeoRadioButton.BeginInvoke(SU_SetGeoRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting geo RB: " + e.ToString());
            }
        }
        private void setGeoRadioButtonCallBackFunction(bool value)
        {
            suGeoRadioButton.Checked = value;
        }
        private void setRuneRadioButton(bool value)
        {
            try
            {
                suRuneRadioButton.BeginInvoke(setRuneRadioButtonCallBack, new object[] { value });

            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Setting Rune RB: " + e.ToString());
            }
        }
        private void setRuneRadioButtonCallBackFunction(bool value)
        {
            suRuneRadioButton.Checked = value;
        }
        private int getFullCommandListBoxCount()
        {
            return (int)SUFullCommandListBox.Invoke(getFullCommandListBoxCountCallBack);
        }
        private int getFullCommandListBoxCountCallBackFunction()
        {
            return SUFullCommandListBox.Items.Count;
        }
        private void clearFullCommandListBox()
        {
            SUFullCommandListBox.Invoke(clearFullCommandListBoxCallBack);
        }
        private void clearFullCommandListBoxCallBackFunction()
        {
            SUFullCommandListBox.Items.Clear();
        }
        private void addFullCommandListBoxItem(Command cmd)
        {
            SUFullCommandListBox.Invoke(addFullCommandListBoxItemCallBack, new object[] { cmd });
        }
        private void addFullCommandListBoxItemCallBackFunction(Command cmd)
        {
            SUFullCommandListBox.Items.Add(cmd);
        }
        private void SU_SetFullCommandLBSource(List<SpellCommand> iCmdList)
        {
            try
            {
                if (SUFullCommandListBox.InvokeRequired)
                {
                    SUFullCommandListBox.Invoke(SU_SetFullCommandLBSourcePtr, new object[] { iCmdList });
                }
                else
                {
                    SU_SetFullCommandLBSourceCBF(iCmdList);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("SU_SetFullCommandLBSource: " + e.ToString());
            }
        }
        private void SU_SetFullCommandLBSourceCBF(List<SpellCommand> iCmdList)
        {
            SUFullCommandListBox.DataSource = iCmdList;
            SUFullCommandListBox.DisplayMember = "Name";
        }
        private void addDoCommandListBoxItem(Command cmd)
        {
            SUDoCommandListBox.Invoke(addDoCommandListBoxItemCallBack, new object[] { cmd });
        }
        private void addDoCommandListBoxItemCallBackFunction(Command cmd)
        {
            SUDoCommandListBox.Items.Add(cmd);
        }
        private void SUSetDelayBetweenCastsChkB(bool iValue)
        {
            try
            {
                if (SUDelayBetweenCastsChkB.InvokeRequired)
                {
                    SUDelayBetweenCastsChkB.Invoke(SUSetDelayBetweenCastsChkBCallBack, new object[] { iValue });
                }
                else
                {
                    SUSetDelayBetweenCastsChkBCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SUSetDelayBetweenCastsChkB: " + e.ToString());
            }
        }
        private void SUSetDelayBetweenCastsChkBCallBackFunction(bool iValue)
        {
            SUDelayBetweenCastsChkB.Checked = iValue;
        }
        private void SUSetDelayValueBetweenCasts(uint iValue)
        {
            try
            {
                if (SUDelayBetweenCastsUpDn.InvokeRequired)
                {
                    SUDelayBetweenCastsUpDn.Invoke(SUSetDelayValueBetweenCastsCallBack, new object[] { iValue });
                }
                else
                {
                    SUSetDelayValueBetweenCastsCallBackFunction(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("In SUSetDelayValueBetweenCasts: " + e.ToString());
            }
        }
        private void SUSetDelayValueBetweenCastsCallBackFunction(uint iValue)
        {
            SUDelayBetweenCastsUpDn.Value = (decimal)iValue;
        }
        #endregion Call Back Functions
        #endregion GUI Synchronization
        #region Inits Section
        private bool doSUInits()
        {
            LoggingFunctions.Debug("TopSU::doSUInits: Doing doSUInits().", LoggingFunctions.DBG_SCOPE.TOP);
            createSUDelegates();
            loadSUSettings();
            if (!initSkillType())
            {
                return false;
            }
            else if (!initSUDoCommandList())
            {
                return false;
            }
            else
            {
                setGoToCapRadioButton(SU_GoToCap);
                setStopAtRadioButton(SU_StopAtGivenSkill);
                setStopAtTextBoxText(SU_SkillLevel.ToString());
                setDoOnlyRadioButton(SU_DoOnly);
                setDoOnlyTextBoxText(SU_DoOnlyCount.ToString());
                setStopRadioButton(SU_DoneStop);
                setLogoutRadioButton(SU_DoneLogout);
                setShutdownRadioButton(SU_DoneShutdown);
                setLogoutCommandCheckBox(SU_GiveLogoutCommand);
                setLogoutCommandTextBoxText(SU_LogoutCommand);
                setRestCommandCheckBox(SU_GiveRestCommand);
                setRestCommandTextBoxText(SU_RestCommand);
                SUSetDelayBetweenCastsChkB(SU_DelayBetweenCasts);
                SUSetDelayValueBetweenCasts(SU_DelayValueBetweenCasts);
            }
            setSUParam();
            return true;
        }
        private void setSUParam()
        {
            Bots.SUParam prm = new Bots.SUParam();
            prm.StartButton = SU_Start_Button;
            prm.StatusBox = c_StatusBoxTB;
            prm.CommandListBox = SUDoCommandListBox;
            prm.CurrentSkil = SU_CurrentSkill;
            prm.GoToCap = SU_GoToCap;
            prm.SkillLevel = SU_SkillLevel;
            prm.StopAtGivenSkill = SU_StopAtGivenSkill;
            prm.DoOnly = SU_DoOnly;
            prm.DoOnlyCount = SU_DoOnlyCount;
            prm.DoneStop = SU_DoneStop;
            prm.DoneLogout = SU_DoneLogout;
            prm.DoneShutdown = SU_DoneShutdown;
            prm.GiveLogoutCommand = SU_GiveLogoutCommand;
            prm.LogoutCommand = SU_LogoutCommand;
            prm.GiveRestCommand = SU_GiveRestCommand;
            prm.RestCommand = SU_RestCommand;
            prm.DelayBetweenCasts = SU_DelayBetweenCasts;
            prm.DelayValueBetweenCasts = SU_DelayValueBetweenCasts;
            prm.SetStartButtonCallBack = SU_SetSUStartButtonCallBack;
            Bots.SkillUp.Access.SetParams(prm);
        }
        private void loadSUSettings()
        {
            SU_CurrentSkill = Convert.ToByte(UserSettings.GetValue(UserSettings.BOT.SU, "SUCurrentSkill"));
            SU_SkillLevel = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.SU, "SUSkillLevel"));
            SU_StopAtGivenSkill = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.SU, "SUStopAtGivenSkill"));
            SU_GoToCap = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.SU, "SUGoToCap"));
            SU_DoOnly = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.SU, "SUDoOnly"));
            SU_DoOnlyCount = Convert.ToInt32(UserSettings.GetValue(UserSettings.BOT.SU, "SUDoOnlyCount"));
            SU_DoneStop = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.SU, "SUDoneStop"));
            SU_DoneLogout = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.SU, "SUDoneLogout"));
            SU_DoneShutdown = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.SU, "SUDoneShutdown"));
            SU_GiveRestCommand = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.SU, "SUGiveRestCommand"));
            SU_RestCommand = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.SU, "SURestCommand"));
            SU_GiveLogoutCommand = Convert.ToBoolean(UserSettings.GetValue(UserSettings.BOT.SU, "SUGiveLogoutCommand"));
            SU_LogoutCommand = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.SU, "SULogoutCommand"));
            SU_DelayBetweenCasts = (bool)UserSettings.GetValue(UserSettings.BOT.SU, "SUDelayBetweenCasts");
            SU_DelayValueBetweenCasts = (uint)UserSettings.GetValue(UserSettings.BOT.SU, "SUDelayValueBetweenCasts");
        }
        private bool initSkillType()
        {
            switch (SU_CurrentSkill)
            {
                case (byte)SU_TYPE.BLUE:
                    setBlueRadioButton(true);
                    break;
                case (byte)SU_TYPE.ENHANCING:
                    setEnhancingRadioButton(true);
                    break;
                case (byte)SU_TYPE.HEALING:
                    setHealingRadioButton(true);
                    break;
                case (byte)SU_TYPE.NINJUTSU:
                    setNinjutsuRadioButton(true);
                    break;
                case (byte)SU_TYPE.SINGING:
                    setSingingRadioButton(true);
                    break;
                case (byte)SU_TYPE.STRING_INSTR:
                    setStringInstrumentRadioButton(true);
                    break;
                case (byte)SU_TYPE.SUMMONING:
                    setSummoningRadioButton(true);
                    break;
                case (byte)SU_TYPE.WIND_INSTR:
                    setWindInstrumentRadioButton(true);
                    break;
                case (byte)SU_TYPE.GEO:
                    setGeoRadioButton(true);
                    break;
                case (byte) SU_TYPE.RUNE:
                    setRuneRadioButton(true);
                    break;
                default:
                    return false;
            }
            return true;
        }
        private bool loadSUFullCommandList(List<SpellCommand> iCmdList)
        {
            //if (getFullCommandListBoxCount() != 0)
            //{
            //    LoggingFunctions.Debug("TopSU::loadSUFullCommandList: Clearing all items from Full Command List.", LoggingFunctions.DBG_SCOPE.TOP);
            //    clearFullCommandListBox();
            //}

            LoggingFunctions.Debug("TopSU::loadSUFullCommandList: Setting up full command list for SU.", LoggingFunctions.DBG_SCOPE.TOP);
            SU_SetFullCommandLBSource(iCmdList);
            //String filter = "Type = " + skillType.ToString();
            //LoggingFunctions.Debug("TopSU::loadSUFullCommandList: " + filter + ".", LoggingFunctions.DBG_SCOPE.TOP);
            //String sort = "Name ASC";
            //DataRow[] rowArray = Statics.Datasets.MainDb.Tables["Commands"].Select(filter, sort);
            //LoggingFunctions.Debug("TopSU::loadSUFullCommandList: There are " + rowArray.Length + " commands loaded.", LoggingFunctions.DBG_SCOPE.TOP);
            //foreach (DataRow row in rowArray)
            //{
            //    Command newCommand = new Command(row["Name"].ToString(), row["Text"].ToString(),
            //                                     System.Convert.ToInt16(row["Type"]),
            //                                     System.Convert.ToInt32(row["CastTime"]),
            //                                     System.Convert.ToInt32(row["RecastTime"]),
            //                                     System.Convert.ToInt32(row["Duration"]),
            //                                     System.Convert.ToInt32(row["MP"]),
            //                                     mainProc, System.Convert.ToBoolean(row["BaseLevel"]));
            //    LoggingFunctions.Debug("TopSU::loadSUFullCommandList: Adding new command: " + newCommand.ToString() + " to SU Full Command list.", LoggingFunctions.DBG_SCOPE.TOP);
            //    addFullCommandListBoxItem(newCommand);
            //}
            return true;
        }
        private bool initSUDoCommandList()
        {
            try
            {
                LoggingFunctions.Debug("TopSU::initSUDoCommandList: Doing init of SU Command List.", LoggingFunctions.DBG_SCOPE.TOP);
                List<List<Object>> commandList = UserSettings.GetList(UserSettings.BOT.SU, UserSettings.LIST_TABLE.SU_COMMANDS);
                if (commandList == null)
                {
                    return true;
                }
                foreach (List<Object> strList in commandList)
                {
                    foreach (Object str in strList)
                    {
                        String cmdName = (String)str;
                        foreach (SpellCommand cmd in CommandManager.SpellsManager.AllCommands)
                        {
                            if(cmd.Name == cmdName)
                            {
                                addDoCommandListBoxItem(cmd);
                            }
                        }
                        //if(cmdName.Contains("'"))
                        //{
                        //    cmdName = cmdName.Insert(cmdName.IndexOf("'"), "\'");
                        //}
                        //String subFilter = "Name = \'" + cmdName + "\'";
                        //LoggingFunctions.Debug("TopSU::initSUDoCommandList: " + subFilter + ".", LoggingFunctions.DBG_SCOPE.TOP);
                        //DataRow[] subRowArray = Statics.Datasets.MainDb.Tables["Commands"].Select(subFilter);
                        //LoggingFunctions.Debug("TopSU::initSUDoCommandList: Got back " + subRowArray.Length + " rows.", LoggingFunctions.DBG_SCOPE.TOP);
                        //if (subRowArray.Length == 0)
                        //{
                        //    LoggingFunctions.Error("No command found when loading Skill up command list for " + subFilter);
                        //}
                        //else
                        //{
                        //    Command newCommand = new Command(subRowArray[0]["Name"].ToString(), subRowArray[0]["Text"].ToString(),
                        //                             System.Convert.ToInt16(subRowArray[0]["Type"]),
                        //                             System.Convert.ToInt32(subRowArray[0]["CastTime"]),
                        //                             System.Convert.ToInt32(subRowArray[0]["RecastTime"]),
                        //                             System.Convert.ToInt32(subRowArray[0]["Duration"]),
                        //                             System.Convert.ToInt32(subRowArray[0]["MP"]),
                        //                             mainProc, System.Convert.ToBoolean(subRowArray[0]["BaseLevel"]));
                        //    addDoCommandListBoxItem(newCommand);
                        //}
                    }
                }
            }
            catch
            {
                LoggingFunctions.Error("Caught while trying to load SU Command List");
                return false;
            }
            return true;
        }
        #endregion //Inits Section
        #region UI Events
        #region Buttons
        private void SU_Start_Button_Click(object sender, EventArgs e)
        {
            if (Statics.Flags.ProcessState == 0)
            {
                MessageBox.Show("Cannot find pol process of given name. Check name and/or dual box checkbox");
            }
            else
            {
                if (Bots.SkillUp.Access.State == Bots.STATE.STOPPED)
                {
                    LoggingFunctions.Timestamp("Starting Skill Up Bot");
                    TOP_updateStatusBox("Starting Skill Up bot", Statics.Fields.Green);
                    SU_RunThread = new Thread(new ThreadStart(Bots.SkillUp.Access.publicRunThreadFunction));
                    SU_RunThread.Name = "SURunThread";
                    SU_RunThread.IsBackground = true;
                    SU_RunThread.Start();

                    SU_Start_Button.UseMnemonic = true;
                    SU_Start_Button.Text = "&Pause";
                    SU_Start_Button.BackColor = System.Drawing.Color.Yellow;
                }
                else if (Bots.SkillUp.Access.State == Bots.STATE.RUNNING)  //1: running
                {
                    LoggingFunctions.Timestamp("Pausing SU Bot");
                    Bots.SkillUp.Access.Pause();
                    SU_Start_Button.UseMnemonic = true;
                    SU_Start_Button.Text = "&Resume";
                    SU_Start_Button.BackColor = System.Drawing.Color.Lime;
                }
                else                                      //2, 3, 4: paused
                {
                    LoggingFunctions.Timestamp("Resuming SU Bot");
                    Bots.SkillUp.Access.Resume();
                    SU_Start_Button.UseMnemonic = true;
                    SU_Start_Button.Text = "&Pause";
                    SU_Start_Button.BackColor = System.Drawing.Color.Yellow;
                }
            }
        }
        private void SU_Stop_Button_Click(object sender, EventArgs e)
        {
            if (Bots.SkillUp.Access != null)
            {
                LoggingFunctions.Timestamp("Stopping PL Bot");
                Bots.SkillUp.Access.Stop();
                ALR_previousStateMap[BOT.SU] = BOT_STATE.STOPPED;
                while (Bots.SkillUp.Access.State != Bots.STATE.STOPPED)
                {
                    IocaineFunctions.delay(500);
                }
                SU_Start_Button.UseMnemonic = true;
                SU_Start_Button.Text = "S&tart";
                SU_Start_Button.BackColor = System.Drawing.Color.Lime;
            }
        }
        private void SUAddCmdButton_Click(object sender, EventArgs e)
        {
            foreach (Command cmd in SUFullCommandListBox.SelectedItems)
            {
                SUDoCommandListBox.Items.Add(cmd);
                UserSettings.AddListValue(UserSettings.BOT.SU, UserSettings.LIST_TABLE.SU_COMMANDS, new List<object>() { cmd.Name });
            }
        }
        private void SURemoveCmdButton_Click(object sender, EventArgs e)
        {
            int listLength = SUDoCommandListBox.Items.Count;
            for (int ii = listLength - 1; ii >= 0; ii--)
            {
                if (SUDoCommandListBox.GetSelected(ii))
                {
                    String cmdName = ((Command)SUDoCommandListBox.Items[ii]).Name;
                    UserSettings.RemoveListValue(UserSettings.BOT.SU, UserSettings.LIST_TABLE.SU_COMMANDS, cmdName);
                    SUDoCommandListBox.Items.RemoveAt(ii);
                }
            }
        }
        private void SUUpButton_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = SUDoCommandListBox.SelectedIndices;
            if (selectedIndices.Count == 0)
            {
                return;
            }
            int lowestSelectedIndex = SUDoCommandListBox.Items.Count - 1;
            int highestSelectedIndex = 0;
            foreach (int index in selectedIndices)
            {
                if (index < lowestSelectedIndex)
                {
                    lowestSelectedIndex = index;
                }
                if (index > highestSelectedIndex)
                {
                    highestSelectedIndex = index;
                }
            }
            if (lowestSelectedIndex == 0)
            {
                return;
            }
            int nextLowestIndex = lowestSelectedIndex - 1;
            Command cmdToMove = (Command)SUDoCommandListBox.Items[nextLowestIndex];
            SUDoCommandListBox.BeginUpdate();
            SUDoCommandListBox.Items.Insert(highestSelectedIndex + 1, cmdToMove);
            SUDoCommandListBox.Items.RemoveAt(nextLowestIndex);
            SUDoCommandListBox.EndUpdate();
            //Update settings list
            suSaveCommandListSettings();
        }
        private void SUDownButton_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = SUDoCommandListBox.SelectedIndices;
            if (selectedIndices.Count == 0)
            {
                return;
            }
            int lowestSelectedIndex = SUDoCommandListBox.Items.Count - 1;
            int highestSelectedIndex = 0;
            foreach (int index in selectedIndices)
            {
                if (index < lowestSelectedIndex)
                {
                    lowestSelectedIndex = index;
                }
                if (index > highestSelectedIndex)
                {
                    highestSelectedIndex = index;
                }
            }
            if (highestSelectedIndex == (SUDoCommandListBox.Items.Count - 1))
            {
                return;
            }
            int nextHighestIndex = highestSelectedIndex + 1;
            Command cmdToMove = (Command)SUDoCommandListBox.Items[nextHighestIndex];
            SUDoCommandListBox.BeginUpdate();
            SUDoCommandListBox.Items.Insert(lowestSelectedIndex, cmdToMove);
            SUDoCommandListBox.Items.RemoveAt(nextHighestIndex + 1);  //+1 cause we inserted one above this already
            SUDoCommandListBox.EndUpdate();
            //Update settings list
            suSaveCommandListSettings();
        }
        #endregion Buttons
        #region Radio Buttons
        private void suEnhancingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suEnhancingRadioButton.Checked)
            {
                return;
            }
            SU_CurrentSkill = (byte)SU_TYPE.ENHANCING;
            loadSUFullCommandList(CommandManager.SpellsManager.SU_EnhancingCommands);
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCurrentSkill(SU_CurrentSkill);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUCurrentSkill", SU_CurrentSkill.ToString());
        }
        private void suHealingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suHealingRadioButton.Checked)
            {
                return;
            }
            SU_CurrentSkill = (byte)SU_TYPE.HEALING;
            loadSUFullCommandList(CommandManager.SpellsManager.SU_HealingCommands);
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCurrentSkill(SU_CurrentSkill);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUCurrentSkill", SU_CurrentSkill.ToString());
        }
        private void suSummoningRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suSummoningRadioButton.Checked)
            {
                return;
            }
            SU_CurrentSkill = (byte)SU_TYPE.SUMMONING;
            loadSUFullCommandList(CommandManager.SpellsManager.SU_SummoningCommands);
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCurrentSkill(SU_CurrentSkill);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUCurrentSkill", SU_CurrentSkill.ToString());
        }
        private void suBlueRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suBlueRadioButton.Checked)
            {
                return;
            }
            SU_CurrentSkill = (byte)SU_TYPE.BLUE;
            loadSUFullCommandList(CommandManager.SpellsManager.SU_BlueCommands);
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCurrentSkill(SU_CurrentSkill);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUCurrentSkill", SU_CurrentSkill.ToString());
        }
        private void suNinjutsuRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suNinjutsuRadioButton.Checked)
            {
                return;
            }
            SU_CurrentSkill = (byte)SU_TYPE.NINJUTSU;
            loadSUFullCommandList(CommandManager.SpellsManager.SU_NinjutsuCommands);
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCurrentSkill(SU_CurrentSkill);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUCurrentSkill", SU_CurrentSkill.ToString());
        }
        private void suSingingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suSingingRadioButton.Checked)
            {
                return;
            }
            SU_CurrentSkill = (byte)SU_TYPE.SINGING;
            loadSUFullCommandList(CommandManager.SpellsManager.SU_SingingCommands);
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCurrentSkill(SU_CurrentSkill);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUCurrentSkill", SU_CurrentSkill.ToString());
        }
        private void suStringInstrumentRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suStringInstrumentRadioButton.Checked)
            {
                return;
            }
            SU_CurrentSkill = (byte)SU_TYPE.STRING_INSTR;
            loadSUFullCommandList(CommandManager.SpellsManager.SU_StringCommands);
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCurrentSkill(SU_CurrentSkill);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUCurrentSkill", SU_CurrentSkill.ToString());
        }
        private void suWindInstrumentRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suWindInstrumentRadioButton.Checked)
            {
                return;
            }
            SU_CurrentSkill = (byte)SU_TYPE.WIND_INSTR;
            loadSUFullCommandList(CommandManager.SpellsManager.SU_WindCommands);
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCurrentSkill(SU_CurrentSkill);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUCurrentSkill", SU_CurrentSkill.ToString());
        }
        private void suGeoRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suGeoRadioButton.Checked)
            {
                return;
            }
            SU_CurrentSkill = (byte)SU_TYPE.GEO;
            loadSUFullCommandList(CommandManager.SpellsManager.SU_GeoCommands);
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCurrentSkill(SU_CurrentSkill);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUCurrentSkill", SU_CurrentSkill.ToString());
        }
        private void suRuneRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suRuneRadioButton.Checked)
            {
                return;
            }
            SU_CurrentSkill = (byte)SU_TYPE.RUNE;
            loadSUFullCommandList(CommandManager.SpellsManager.SU_EnhancingCommands);
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCurrentSkill(SU_CurrentSkill);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUCurrentSkill", SU_CurrentSkill.ToString());
        }
        private void SUGoToCapRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SU_GoToCap = SUGoToCapRadioButton.Checked;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateStopSettings(SU_GoToCap, SU_StopAtGivenSkill, SU_SkillLevel, SU_DoOnly, SU_DoOnlyCount);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUGoToCap", SU_GoToCap.ToString());
        }
        private void SUStopAtRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SU_StopAtGivenSkill = SUStopAtRadioButton.Checked;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateStopSettings(SU_GoToCap, SU_StopAtGivenSkill, SU_SkillLevel, SU_DoOnly, SU_DoOnlyCount);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUStopAtGivenSkill", SU_StopAtGivenSkill.ToString());
        }
        private void SUDoOnlyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SU_DoOnly = SUDoOnlyRadioButton.Checked;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateStopSettings(SU_GoToCap, SU_StopAtGivenSkill, SU_SkillLevel, SU_DoOnly, SU_DoOnlyCount);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUDoOnly", SU_DoOnly.ToString());
        }
        private void SUStopRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SU_DoneStop = SUStopRadioButton.Checked;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateWhenDoneSettings(SU_DoneStop, SU_DoneLogout, SU_DoneShutdown);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUDoneStop", SU_DoneStop.ToString());
        }
        private void SULogoutRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SU_DoneLogout = SULogoutRadioButton.Checked;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateWhenDoneSettings(SU_DoneStop, SU_DoneLogout, SU_DoneShutdown);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUDoneLogout", SU_DoneLogout.ToString());
        }
        private void SUShutdownRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SU_DoneShutdown = SUShutdownRadioButton.Checked;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateWhenDoneSettings(SU_DoneStop, SU_DoneLogout, SU_DoneShutdown);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUDoneShutdown", SU_DoneShutdown.ToString());
        }
        #endregion Radio Buttons
        #region Check Boxes
        private void SURestCommandCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SU_GiveRestCommand = SURestCommandCheckBox.Checked;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateGiveCommandBeforeResting(SU_GiveRestCommand);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUGiveRestCommand", SU_GiveRestCommand.ToString());
        }
        private void SULogoutCommandCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SU_GiveLogoutCommand = SULogoutCommandCheckBox.Checked;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateGiveCommandBeforeLogout(SU_GiveLogoutCommand);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SUGiveLogoutCommand", SU_GiveLogoutCommand.ToString());
        }
        private void SUDelayBetweenCastsChkB_CheckedChanged(object sender, EventArgs e)
        {
            SU_DelayBetweenCasts = SUDelayBetweenCastsChkB.Checked;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateDelayBetweenCasts(SU_DelayBetweenCasts);
            }
            UserSettings.SetValue(UserSettings.BOT.SU, "SUDelayBetweenCasts", SU_DelayBetweenCasts.ToString());
        }
        #endregion Check Boxes
        #region Text Boxes
        private void SUStopAtTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SU_SkillLevel = Convert.ToInt32(SUStopAtTextBox.Text);
                if (Bots.SkillUp.Access != null)
                {
                    Bots.SkillUp.Access.updateStopSettings(SU_GoToCap, SU_StopAtGivenSkill, SU_SkillLevel, SU_DoOnly, SU_DoOnlyCount);
                }
            }
            catch
            {
                SU_SkillLevel = 300;
            }
            finally
            {
                //Update settings
                UserSettings.SetValue(UserSettings.BOT.SU, "SUSkillLevel", SU_SkillLevel.ToString());
            }
        }
        private void SUDoOnlyTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SU_DoOnlyCount = Convert.ToInt32(SUDoOnlyTextBox.Text);
                if (Bots.SkillUp.Access != null)
                {
                    Bots.SkillUp.Access.updateStopSettings(SU_GoToCap, SU_StopAtGivenSkill, SU_SkillLevel, SU_DoOnly, SU_DoOnlyCount);
                }
            }
            catch
            {
                SU_DoOnlyCount = 10;
            }
            finally
            {
                //Update settings
                UserSettings.SetValue(UserSettings.BOT.SU, "SUDoOnlyCount", SU_DoOnlyCount.ToString());
            }
        }
        private void SURestCommandTextBox_TextChanged(object sender, EventArgs e)
        {
            SU_RestCommand = SURestCommandTextBox.Text;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCommandBeforeResting(SU_RestCommand);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SURestCommand", SU_RestCommand.ToString());
        }
        private void SULogoutCommandTextBox_TextChanged(object sender, EventArgs e)
        {
            SU_LogoutCommand = SULogoutCommandTextBox.Text;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateCommandBeforeLogout(SU_LogoutCommand);
            }
            //Update settings
            UserSettings.SetValue(UserSettings.BOT.SU, "SULogoutCommand", SU_LogoutCommand.ToString());
        }
        private void SU_ReiveChkB_CheckedChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_DoLairReives = SU_ReiveChkB.Checked;
        }
        private void SU_RefreshChkB_CheckedChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_DoRefresh = SU_RefreshChkB.Checked;
        }
        private void SU_EnhancingChkB_CheckedChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_EnhancingCastName = SU_EnhancingTB.Text;
        }
        private void SU_EnfeeblingChkB_CheckedChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_DoEnfeebling = SU_EnfeeblingChkB.Checked;
        }
        private void SU_DarkChkB_CheckedChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_DoDark = SU_DarkChkB.Checked;
        }
        private void SU_DivineChkB_CheckedChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_DoDivine = SU_DivineChkB.Checked;
        }
        private void SU_OffensiveTB_TextChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_ElementalCastName = SU_OffensiveTB.Text;
        }
        private void SU_RefreshTB_TextChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_RefreshCastName = SU_RefreshTB.Text;
        }
        private void SU_EnhancingTB_TextChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_EnhancingCastName = SU_EnhancingTB.Text;
        }
        private void SU_EnhancingStatusTB_TextChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_EnhancingStatusName = SU_EnhancingStatusTB.Text;
        }
        private void SU_EnfeeblingTB_TextChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_EnfeeblingCastName = SU_EnfeeblingTB.Text;
        }
        private void SU_DarkTB_TextChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_DarkCastName = SU_DarkTB.Text;
        }
        private void SU_DivineTB_TextChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_DivineCastName = SU_DivineTB.Text;
        }
        #endregion Text Boxes
        #region Up Downs
        private void SUDelayBetweenCastsUpDn_ValueChanged(object sender, EventArgs e)
        {
            SU_DelayValueBetweenCasts = (uint)SUDelayBetweenCastsUpDn.Value;
            if (Bots.SkillUp.Access != null)
            {
                Bots.SkillUp.Access.updateDelayValueBetweenCasts(SU_DelayValueBetweenCasts);
            }
            UserSettings.SetValue(UserSettings.BOT.SU, "SUDelayValueBetweenCasts", SU_DelayValueBetweenCasts.ToString());
        }
        private void SU_LoopCountUpDn_ValueChanged(object sender, EventArgs e)
        {
            Bots.SkillUp.SU_CastCountLimit = (uint)SU_LoopCountUpDn.Value;
        }
        #endregion Up Downs
        #region Other
        private void suSaveCommandListSettings()
        {
            List<Object> commandList = new List<object>();
            for (int ii = 0; ii < SUDoCommandListBox.Items.Count; ii++)
            {
                commandList.Add(SUDoCommandListBox.Items[ii].ToString());
            }
            UserSettings.SetList(UserSettings.BOT.SU, UserSettings.LIST_TABLE.SU_COMMANDS, new List<List<object>>() { commandList });
        }
        #endregion Other
        #endregion //UI Events
    }
}