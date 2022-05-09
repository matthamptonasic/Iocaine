using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Settings;

namespace Iocaine2
{
    partial class Iocaine_2_Form
    {
        #region Enums
        private enum POS_DIRECTION : byte
        {
            N,
            NE,
            E,
            SE,
            S,
            SW,
            W,
            NW,
            UP,
            DOWN,
            TARGET
        }
        #endregion Enums

        #region Private Members
        private float POS_curX = 0f;
        private float POS_curY = 0f;
        private float POS_curZ = 0f;
        private bool POS_speedCurrentlyEnabled = false;
        private bool POS_speedDisabledFromAlert = false;
        private volatile bool POS_loadingSettings = false;
        #endregion Private Members

        #region Inits
        private bool POS_Init_Iocaine()
        {
            ChangeMonitor._zoneChanged += POS_zoneChanged;
            ChangeMonitor._inMogChanged += POS_inMogChanged;
            _ALR_AlertChanged += POS_AlertChanged;
            return true;
        }
        private bool POS_Init_LoggedIn()
        {
            POS_LoadSettings();
            if (Statics.Settings.POS.EnableOnStartup && MemReads.PosEnabled)
            {
                POS_SetEnableSpeedChkB(true);
            }
            else if(MemReads.PosEnabled == false)
            {
                POS_SetEnableSpeedChkB(false);
            }
            return true;
        }
        #endregion Inits

        #region Settings
        private void POS_LoadSettings()
        {
            POS_loadingSettings = true;
            Statics.Settings.POS.DistancePerNudge = (float)UserSettings.GetValue(UserSettings.BOT.POS, "POS_DistancePerNudge");
            Statics.Settings.POS.Speed = (float)UserSettings.GetValue(UserSettings.BOT.POS, "POS_Speed");
            Statics.Settings.POS.SpeedDisable = (float)UserSettings.GetValue(UserSettings.BOT.POS, "POS_SpeedDisable");
            Statics.Settings.POS.DisableOnAlert = (bool)UserSettings.GetValue(UserSettings.BOT.POS, "POS_DisableOnAlert");
            Statics.Settings.POS.EnableOnStartup = (bool)UserSettings.GetValue(UserSettings.BOT.POS, "POS_EnableOnStartup");
            Statics.Settings.POS.Enable = Statics.Settings.POS.EnableOnStartup;

            POS_SetDistanceUpDown(Statics.Settings.POS.DistancePerNudge);
            POS_SetSpeedUpDown(Statics.Settings.POS.Speed);
            POS_SetSpeedDisabledUpDown(Statics.Settings.POS.SpeedDisable);
            POS_SetDisableOnAlertChkB(Statics.Settings.POS.DisableOnAlert);
            POS_SetEnableAtStartupChkB(Statics.Settings.POS.EnableOnStartup);
            POS_SetEnableSpeedChkB(Statics.Settings.POS.Enable);
            POS_loadingSettings = false;
        }
        #endregion Settings

        #region Utility Methods
        private bool POS_updatePos()
        {
            if (ChangeMonitor.LoggedIn)
            {
                POS_curX = MemReads.Self.Position.get_x();
                POS_curY = MemReads.Self.Position.get_y();
                POS_curZ = MemReads.Self.Position.get_z();
                return true;
            }
            return false;
        }
        private void POS_MoveDirection(POS_DIRECTION iDir)
        {
            if (POS_updatePos())
            {
                float endX = POS_curX;
                float endY = POS_curY;
                float endZ = POS_curZ;
                if(MemReads.PosEnabled == false)
                {
                    return;
                }
                float dirDistance = 0;
                if ((iDir == POS_DIRECTION.NE) || (iDir == POS_DIRECTION.SE) || (iDir == POS_DIRECTION.SW) || (iDir == POS_DIRECTION.NW))
                {
                    dirDistance = (float)Math.Cos(Math.PI / 4) * Statics.Settings.POS.DistancePerNudge;
                }
                switch (iDir)
                {
                    case POS_DIRECTION.N:
                        endY = POS_curY + Statics.Settings.POS.DistancePerNudge;
                        break;
                    case POS_DIRECTION.NE:
                        endX = POS_curX + dirDistance;
                        endY = POS_curY + dirDistance;
                        break;
                    case POS_DIRECTION.E:
                        endX = POS_curX + Statics.Settings.POS.DistancePerNudge;
                        break;
                    case POS_DIRECTION.SE:
                        endX = POS_curX + dirDistance;
                        endY = POS_curY - dirDistance;
                        break;
                    case POS_DIRECTION.S:
                        endY = POS_curY - Statics.Settings.POS.DistancePerNudge;
                        break;
                    case POS_DIRECTION.SW:
                        endX = POS_curX - dirDistance;
                        endY = POS_curY - dirDistance;
                        break;
                    case POS_DIRECTION.W:
                        endX = POS_curX - Statics.Settings.POS.DistancePerNudge;
                        break;
                    case POS_DIRECTION.NW:
                        endX = POS_curX - dirDistance;
                        endY = POS_curY + dirDistance;
                        break;
                    case POS_DIRECTION.DOWN:
                        endZ = POS_curZ + Statics.Settings.POS.DistancePerNudge;
                        break;
                    case POS_DIRECTION.UP:
                        endZ = POS_curZ - Statics.Settings.POS.DistancePerNudge;
                        break;
                    case POS_DIRECTION.TARGET:
                        float distance = MemReads.Target.get_distance();
                        if ((MemReads.Target.get_id() != MemReads.Target.InvalidStructId) && (distance != 0) && (distance < 50))
                        {
                            endX = MemReads.Target.get_position_x();
                            endY = MemReads.Target.get_position_y();
                            endZ = MemReads.Target.get_position_z();
                        }
                        break;
                    default:
                        return;
                }
                if (endX != POS_curX)
                {
                    MemReads.Self.Position.set_x(endX);
                }
                if (endY != POS_curY)
                {
                    MemReads.Self.Position.set_y(endY);
                }
                if (endZ != POS_curZ)
                {
                    MemReads.Self.Position.set_z(endZ);
                }
            }
        }
        private void POS_SetSpeed()
        {
            if (!ChangeMonitor.LoggedIn)
            {
                return;
            }
            if (MemReads.PosEnabled == false)
            {
                POS_speedCurrentlyEnabled = false;
                return;
            }
            MemReads.Self.Speed.set_speed(true, Statics.Settings.POS.Speed);
            POS_speedCurrentlyEnabled = true;
        }
        private void POS_ClearSpeed()
        {
            if (!ChangeMonitor.LoggedIn)
            {
                return;
            }
            if (MemReads.PosEnabled == false)
            {
                POS_speedCurrentlyEnabled = false;
                return;
            }
            MemReads.Self.Speed.set_speed(false, Statics.Settings.POS.SpeedDisable);
            POS_speedCurrentlyEnabled = false;
        }
        #endregion Utility Methods

        #region Event Handlers
        #region Buttons
        private void POS_N_Button_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.N);
        }
        private void POS_NE_Button_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.NE);
        }
        private void POS_E_Button_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.E);
        }
        private void POS_SE_Button_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.SE);
        }
        private void POS_S_Button_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.S);
        }
        private void POS_SW_Button_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.SW);
        }
        private void POS_W_Button_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.W);
        }
        private void POS_NW_Button_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.NW);
        }
        private void POS_Dn_Button_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.DOWN);
        }
        private void POS_Up_Button_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.UP);
        }
        private void POS_TargetButton_Click(object sender, EventArgs e)
        {
            POS_MoveDirection(POS_DIRECTION.TARGET);
        }
        #endregion Buttons
        #region Up Downs
        private void POS_DistanceUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!POS_loadingSettings)
            {
                Statics.Settings.POS.DistancePerNudge = (float)POS_DistanceUpDown.Value;
                UserSettings.SetValue(UserSettings.BOT.POS, "POS_DistancePerNudge", Statics.Settings.POS.DistancePerNudge.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void POS_SpeedUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!POS_loadingSettings)
            {
                Statics.Settings.POS.Speed = (float)POS_SpeedUpDown.Value;
                UserSettings.SetValue(UserSettings.BOT.POS, "POS_Speed", Statics.Settings.POS.Speed.ToString());
                UserSettings.SaveSettings();
                if (Statics.Settings.POS.Enable && MemReads.PosEnabled)
                {
                    POS_SetSpeed();
                }
            }
        }
        private void POS_SpeedDisabledUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!POS_loadingSettings)
            {
                Statics.Settings.POS.SpeedDisable = (float)POS_SpeedDisabledUpDown.Value;
                UserSettings.SetValue(UserSettings.BOT.POS, "POS_SpeedDisable", Statics.Settings.POS.SpeedDisable.ToString());
                UserSettings.SaveSettings();
            }
        }
        #endregion Up Downs
        #region Check Boxes
        private void POS_EnableSpeedChkB_CheckedChanged(object sender, EventArgs e)
        {
            Statics.Settings.POS.Enable = POS_EnableSpeedChkB.Checked;
            if (Statics.Settings.POS.Enable && MemReads.PosEnabled)
            {
                POS_SetSpeed();
            }
            else
            {
                if(!MemReads.PosEnabled)
                {
                    POS_EnableSpeedChkB.Enabled = false;
                }
                POS_ClearSpeed();
            }
        }
        private void POS_DisableOnAlertChkB_CheckedChanged(object sender, EventArgs e)
        {
            if (!POS_loadingSettings)
            {
                Statics.Settings.POS.DisableOnAlert = POS_DisableOnAlertChkB.Checked;
                if (!Statics.Settings.POS.DisableOnAlert && POS_speedDisabledFromAlert)
                {
                    POS_SetSpeed();
                    POS_speedDisabledFromAlert = false;
                }
                UserSettings.SetValue(UserSettings.BOT.POS, "POS_DisableOnAlert", Statics.Settings.POS.DisableOnAlert.ToString());
                UserSettings.SaveSettings();
            }
        }
        private void POS_EnableAtStartupChkB_CheckedChanged(object sender, EventArgs e)
        {
            if (!POS_loadingSettings)
            {
                Statics.Settings.POS.EnableOnStartup = POS_EnableAtStartupChkB.Checked;
                UserSettings.SetValue(UserSettings.BOT.POS, "POS_EnableOnStartup", Statics.Settings.POS.EnableOnStartup.ToString());
                UserSettings.SaveSettings();
            }
        }
        #endregion Check Boxes
        #region Iocaine Events
        private void POS_zoneChanged(ushort newZoneId, ushort oldZoneId)
        {
            if (Statics.Settings.POS.Enable && POS_speedCurrentlyEnabled)
            {
                POS_ClearSpeed();
                do
                {
                    IocaineFunctions.delay(100);
                }
                while (MemReads.Self.get_is_zoning());
                POS_SetSpeed();
            }
        }
        private void POS_inMogChanged()
        {
            POS_zoneChanged(0, 0);
        }
        private void POS_AlertChanged(bool iAlertSet)
        {
            if (iAlertSet)
            {
                if (POS_speedCurrentlyEnabled && Statics.Settings.POS.DisableOnAlert)
                {
                    POS_ClearSpeed();
                    POS_speedDisabledFromAlert = true;
                }
            }
            else
            {
                if (!POS_speedCurrentlyEnabled && POS_speedDisabledFromAlert)
                {
                    POS_SetSpeed();
                    POS_speedDisabledFromAlert = false;
                }
            }
        }
        #endregion Iocaine Events
        #endregion Event Handlers

        #region GUI Updates
        #region Up Downs
        private void POS_SetDistanceUpDown(float iValue)
        {
            try
            {
                if (POS_DistanceUpDown.InvokeRequired)
                {
                    POS_DistanceUpDown.Invoke(new Statics.FuncPtrs.TD_Void_Float(POS_SetDistanceUpDownCBF), new object[] { iValue });
                }
                else
                {
                    POS_SetDistanceUpDownCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        private void POS_SetDistanceUpDownCBF(float iValue)
        {
            POS_DistanceUpDown.Value = (decimal)iValue;
        }
        private void POS_SetSpeedUpDown(float iValue)
        {
            try
            {
                if (POS_SpeedUpDown.InvokeRequired)
                {
                    POS_SpeedUpDown.Invoke(new Statics.FuncPtrs.TD_Void_Float(POS_SetSpeedUpDownCBF), new object[] { iValue });
                }
                else
                {
                    POS_SetSpeedUpDownCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        private void POS_SetSpeedUpDownCBF(float iValue)
        {
            POS_SpeedUpDown.Value = (decimal)iValue;
        }
        private void POS_SetSpeedDisabledUpDown(float iValue)
        {
            try
            {
                if (POS_SpeedDisabledUpDown.InvokeRequired)
                {
                    POS_SpeedDisabledUpDown.Invoke(new Statics.FuncPtrs.TD_Void_Float(POS_SetSpeedDisabledUpDownCBF), new object[] { iValue });
                }
                else
                {
                    POS_SetSpeedDisabledUpDownCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        private void POS_SetSpeedDisabledUpDownCBF(float iValue)
        {
            POS_SpeedDisabledUpDown.Value = (decimal)iValue;
        }
        #endregion Up Downs
        #region Check Boxes
        private void POS_SetEnableSpeedChkB(bool iValue)
        {
            try
            {
                if (POS_EnableSpeedChkB.InvokeRequired)
                {
                    POS_EnableSpeedChkB.Invoke(new Statics.FuncPtrs.TD_Void_Bool(POS_SetEnableSpeedChkBCBF), new object[] { iValue });
                }
                else
                {
                    POS_SetEnableSpeedChkBCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        private void POS_SetEnableSpeedChkBCBF(bool iValue)
        {
            POS_EnableSpeedChkB.Checked = iValue;
        }
        private void POS_SetDisableOnAlertChkB(bool iValue)
        {
            try
            {
                if (POS_DisableOnAlertChkB.InvokeRequired)
                {
                    POS_DisableOnAlertChkB.Invoke(new Statics.FuncPtrs.TD_Void_Bool(POS_SetDisableOnAlertChkBCBF), new object[] { iValue });
                }
                else
                {
                    POS_SetDisableOnAlertChkBCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        private void POS_SetDisableOnAlertChkBCBF(bool iValue)
        {
            POS_DisableOnAlertChkB.Checked = iValue;
        }
        private void POS_SetEnableAtStartupChkB(bool iValue)
        {
            try
            {
                if (POS_EnableAtStartupChkB.InvokeRequired)
                {
                    POS_EnableAtStartupChkB.Invoke(new Statics.FuncPtrs.TD_Void_Bool(POS_SetEnableAtStartupChkBCBF), new object[] { iValue });
                }
                else
                {
                    POS_SetEnableAtStartupChkBCBF(iValue);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error(e.ToString());
            }
        }
        private void POS_SetEnableAtStartupChkBCBF(bool iValue)
        {
            POS_EnableAtStartupChkB.Checked = iValue;
        }
        #endregion Check Boxes
        #endregion GUI Updates
    }
}
