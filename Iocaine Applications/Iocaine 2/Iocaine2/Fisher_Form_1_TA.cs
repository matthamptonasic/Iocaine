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

using Iocaine2.Bots;
using Iocaine2.Logging;
using Iocaine2.Properties;

namespace Iocaine2
{
    //This file is for functions directly related to the TA
    partial class Iocaine_2_Form
    {
        #region Enums
        #endregion Enums
        #region Members Section
        #region Map Info
        private static Single TA_MapCurrZoom = 1.0f;
        private static Point TA_MapCurrCenter = new Point(256, 256);
        private static UInt16 TA_MapCurrZone = 256;
        private static Byte TA_MapCurrMapID = 0;
        private static Data.Client.Maps.MapSet TA_MapCurrMapSet = null;
        private const Single TA_MapPercZoomPerRoll = 10.0f;
        private static Boolean TA_MapIsPanning = false;
        private static Point TA_MapDragStartPoint = Point.Empty;
        private static Point TA_MapStartCenter = Point.Empty;
        private static Point TA_MapDragCurrPoint = Point.Empty;
        #endregion Map Info
        #region Threads
        private Thread TARunThread = null;
        #endregion Threads
        #region Containers
        #endregion Containers
        #endregion Members Section
        #region GUI Thread Synchronization
        #region Delegates
        public delegate void TA_redrawPlayerLBDelegate(int idx);
        #endregion Delegates
        #region Delegate Instances
        private TA_redrawPlayerLBDelegate TA_redrawPlayerLBCallBack;
        private TA_redrawPlayerLBDelegate TA_redrawPlayerLBWrapperCallBack;
        #endregion Delegate Instances
        #region Create Delegates
        private void TA_createDelegates()
        {
            if (TA_redrawPlayerLBCallBack == null)
            {
                TA_redrawPlayerLBCallBack = new TA_redrawPlayerLBDelegate(TA_redrawPlayerLBCallBackFunction);
                TA_redrawPlayerLBWrapperCallBack = new TA_redrawPlayerLBDelegate(TA_redrawPlayerLB);
            }
        }
        #endregion Create Delegates
        #region Call Back Functions
        private void TA_redrawPlayerLBCallBackFunction(int idx)
        {
            TA_PlayerLB.Invalidate(TA_PlayerLB.GetItemRectangle(idx));
            TA_PlayerLB.Update();
        }
        #endregion Call Back Functions
        #region Wrapper Functions
        private void TA_redrawPlayerLB(int idx)
        {
            try
            {
                if (TA_PlayerLB.InvokeRequired)
                {
                    TA_PlayerLB.Invoke(TA_redrawPlayerLBCallBack, new object[] { idx });
                }
                else
                {
                    TA_redrawPlayerLBCallBackFunction(idx);
                }
            }
            catch (Exception ex)
            {
                LoggingFunctions.Error("Trying to redraw the TA_PlayerLB: " + ex.ToString());
            }
        }
        #endregion Wrapper Functions
        #endregion GUI Thread Synchronization
        #region Inits Section
        private bool doTAInits()
        {
            LoggingFunctions.Debug("Initializing the TA object.", LoggingFunctions.DBG_SCOPE.TA);
            TA_createDelegates();
            Bots.TeachersAssistant.UpdateFollow(TA_FollowCkB.Checked);
            Bots.TeachersAssistant.UpdateFollowDistance(int.Parse(TA_Follow_DistanceTB.Text));
            Bots.TeachersAssistant.UpdatePlayerName(TA_PlayerTB.Text);
            Bots.TeachersAssistant.UpdateWSCommand(TA_WS_CommandTB.Text);
            TA_MapPB.Enabled = false;
            TA_MapPB.Visible = false;
            return true;
        }
        #endregion
        #region Util Functions Section
        private void TA_drawMap()
        {
            if (TA_MapPB == null)
            {
                return;
            }
            if (TA_MapPB.InvokeRequired)
            {
                TA_MapPB.Invoke(new Statics.FuncPtrs.TD_Void_Void(TA_drawMapCBF));
            }
            else
            {
                TA_drawMapCBF();
            }
        }
        private void TA_drawMapCBF()
        {
            if (TA_MapCurrZoom < 0.5)
            {
                TA_MapCurrZoom = 0.5f;
            }
            Image originalImg = TA_MapCurrMapSet.CleanMap;
            Image nwImg = new Bitmap(512, 512);
            // Number of original map pixels = 512 / zoom factor.
            Int32 nbOrigImgPxls = (Int32)(512 / TA_MapCurrZoom);
            Int32 xStart = TA_MapCurrCenter.X - (nbOrigImgPxls / 2);
            Int32 yStart = TA_MapCurrCenter.Y - (nbOrigImgPxls / 2);

            if (xStart < ((nbOrigImgPxls - 10) * -1))
            {
                //Whole map is to the right
                xStart = nbOrigImgPxls * -1 + 20;
            }
            else if (xStart > (512 - 10))
            {
                //While map is to the left
                xStart = 512 - 10;
            }
            if (yStart < ((nbOrigImgPxls - 10) * -1))
            {
                yStart = nbOrigImgPxls * -1 + 20;
            }
            else if (yStart > (512 - 10))
            {
                yStart = 512 - 10;
            }

            TA_MapCurrCenter.X = xStart + (nbOrigImgPxls / 2);
            TA_MapCurrCenter.Y = yStart + (nbOrigImgPxls / 2);
            Graphics gr = Graphics.FromImage(nwImg);
            gr.DrawImage(originalImg, new Rectangle(0, 0, TA_MapPB.Width, TA_MapPB.Height), new Rectangle(xStart, yStart, nbOrigImgPxls, nbOrigImgPxls), GraphicsUnit.Pixel);
            TA_MapPB.SizeMode = PictureBoxSizeMode.Normal;
            TA_MapPB.Image = nwImg;
        }

        private void TA_setMap(UInt16 iZoneId)
        {
            TA_MapCurrZone = iZoneId;
            TA_MapCurrMapSet = Iocaine2.Data.Client.Maps.GetMap(TA_MapCurrZone);
            TA_drawMap();
        }
        private void TA_setMap(UInt16 iZoneId, Byte iMapId)
        {
            TA_MapCurrZone = iZoneId;
            TA_MapCurrMapID = iMapId;
            Byte mapIdx;
            Data.Client.Maps.GetMapIndex(iZoneId, iMapId, out mapIdx);
            TA_MapCurrMapSet = Iocaine2.Data.Client.Maps.GetMap(TA_MapCurrZone, mapIdx);
            TA_drawMap();
        }

        private void TA_MapPB_MouseWheel(object sender, MouseEventArgs e)
        {
            // Location is x/y of where the mouse is relative to the 0,0 of the picturebox.
            Int32 mouseX = e.Location.X;
            Int32 mouseY = e.Location.Y;
            Single pntrPercToRight = mouseX / TA_MapPB.Size.Width;
            Single pntrPercToBottom = mouseY / TA_MapPB.Size.Height;
            // If we're 80% to the right/bottom, we want 80% of the zoom in to be to the right/bottom.
            // Current zoom will give us the pixel count.
            Int32 zoomSwitch = e.Delta > 0 ? 1 : -1;
            Single currPxlCnt = 512 / TA_MapCurrZoom;
            Single newPxlCnt = currPxlCnt - currPxlCnt / TA_MapPercZoomPerRoll * zoomSwitch;
            TA_MapCurrZoom = TA_MapCurrZoom + zoomSwitch / TA_MapPercZoomPerRoll;
            Point newCenter = new Point();
            newCenter.X = (Int32)(TA_MapCurrCenter.X + (zoomSwitch * (mouseX - 128) / TA_MapPercZoomPerRoll));
            newCenter.Y = (Int32)(TA_MapCurrCenter.Y + (zoomSwitch * (mouseY - 128) / TA_MapPercZoomPerRoll));
            TA_MapCurrCenter = newCenter;
            TA_drawMap();
        }
        private void TA_MapPB_MouseEnter(object sender, EventArgs e)
        {
            TA_MapPB.Focus();
        }
        private void TA_MapPB_MouseDown(object sender, MouseEventArgs e)
        {
            TA_MapIsPanning = true;
            //TA_MapDragStartPoint = new Point(e.Location.X - TA_MapDragCurrPoint.X, e.Location.Y - TA_MapDragCurrPoint.Y);
            TA_MapDragStartPoint = new Point(e.Location.X, e.Location.Y);
            TA_MapStartCenter = TA_MapCurrCenter;
            this.Cursor = Cursors.Hand;
        }
        private void TA_MapPB_MouseUp(object sender, MouseEventArgs e)
        {
            TA_MapIsPanning = false;
            this.Cursor = Cursors.Default;
            TA_drawMap();
        }
        private void TA_MapPB_MouseMove(object sender, MouseEventArgs e)
        {
            if (TA_MapIsPanning)
            {
                TA_MapDragCurrPoint = new Point(e.Location.X - TA_MapDragStartPoint.X, e.Location.Y - TA_MapDragStartPoint.Y);
                TA_MapCurrCenter = new Point(TA_MapStartCenter.X - TA_MapDragCurrPoint.X, TA_MapStartCenter.Y - TA_MapDragCurrPoint.Y);
                TA_drawMap();
            }
        }
        #endregion
        #region Windows Event Handlers
        private void TA_StartButton_Click(object sender, EventArgs e)
        {
            if (Bots.TeachersAssistant.TAState == 0) //Currently stopped
            {
                TARunThread = new Thread(new ThreadStart(Bots.TeachersAssistant.Start));
                TARunThread.Name = "TARunThread";
                TARunThread.IsBackground = true;
                TARunThread.Start();
                TA_Start_Button.BackColor = Statics.Buttons.Red;
                TA_Start_Button.Text = "&Stop";
            }
            else
            {
                Bots.TeachersAssistant.Stop();
                TA_Start_Button.BackColor = Statics.Buttons.Green;
                TA_Start_Button.Text = "&Start";
            }
        }
        private void TA_PlayerTB_Click(object sender, EventArgs e)
        {
            TA_PlayerTB.Text = "";
        }
        private void TA_WS_CommandTB_Click(object sender, EventArgs e)
        {
            TA_WS_CommandTB.Text = "";
            TA_WS_CommandTB.SelectionStart = TA_WS_CommandTB.Text.Length;
        }
        private void TA_PlayerTB_TextChanged(object sender, EventArgs e)
        {
            Bots.TeachersAssistant.UpdatePlayerName(TA_PlayerTB.Text);
        }
        private void TA_FollowCkB_CheckedChanged(object sender, EventArgs e)
        {
            Bots.TeachersAssistant.UpdateFollow(TA_FollowCkB.Checked);
        }
        private void TA_Follow_DistanceTB_TextChanged(object sender, EventArgs e)
        {
            if (TA_Follow_DistanceTB.Text.Length > 0)
            {
                Bots.TeachersAssistant.UpdateFollowDistance(int.Parse(TA_Follow_DistanceTB.Text));
            }
        }
        private void TA_Hold_WS_HPPTB_TextChanged(object sender, EventArgs e)
        {
            if (TA_Hold_WS_HPPTB.Text != "")
            {
                int parsedHpp = 0;
                if (int.TryParse(TA_Hold_WS_HPPTB.Text, out parsedHpp))
                {
                    Bots.TeachersAssistant.UpdateWSHPP(parsedHpp);
                }
            }
        }
        private void TA_WS_CommandTB_TextChanged(object sender, EventArgs e)
        {
            Bots.TeachersAssistant.UpdateWSCommand(TA_WS_CommandTB.Text);
        }
        private void TA_Fighting_DistanceUpDown_ValueChanged(object sender, EventArgs e)
        {
            Statics.Settings.TA.FightingDistance = (float)TA_Fighting_DistanceUpDown.Value;
        }
        private void TA_AddTriggerButton_Click(object sender, EventArgs e)
        {
            TriggerForm trigForm = new TriggerForm(Bots.TeachersAssistant.PlayerList, FFXIEnums.JOBS.RDM, FFXIEnums.JOBS.WHM, 30, 15);
            trigForm._SaveTrigger += new TriggerForm.TriggerSaveFunction(Bots.TeachersAssistant.AddTrigger);
            trigForm.ShowDialog();
        }
        private void TA_AddPlayerTB_Click(object sender, EventArgs e)
        {
            TA_AddPlayerTB.Text = "";
        }
        private void TA_AddPlayerTB_TextChanged(object sender, EventArgs e)
        {
            if (TA_AddPlayerTB.Text != "")
            {
                if (char.IsLower(TA_AddPlayerTB.Text[0]))
                {
                    TA_AddPlayerTB.Text = char.ToUpper(TA_AddPlayerTB.Text[0]) + TA_AddPlayerTB.Text.Substring(1);
                    TA_AddPlayerTB.SelectionStart = TA_AddPlayerTB.Text.Length;
                }
            }
        }
        private void TA_AddPlayerTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Return))
            {
                String text = TA_AddPlayerTB.Text;
                Bots.TeachersAssistant.AddPlayer(text);
                ChangeMonitor.AddPlayer(text);
                TA_PlayerLB.Items.Add(text);
                TA_AddPlayerTB.Text = "";
                e.Handled = true;
            }
        }
        private void TA_PlayerLB_KeyDown(object sender, KeyEventArgs e)
        {
            if((e.KeyCode == Keys.Delete) || (e.KeyCode == Keys.Back))
            {
                String playerName = (String)TA_PlayerLB.Items[TA_PlayerLB.SelectedIndex];
                Bots.TeachersAssistant.RemovePlayer(playerName);
                ChangeMonitor.RemovePlayer(playerName);
                TA_PlayerLB.Items.RemoveAt(TA_PlayerLB.SelectedIndex);
            }
        }
        private void TA_PlayerLB_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                // Draw the background of the ListBox control for each item.
                // Create a new Brush and initialize to a Black colored brush
                // by default.
                e.DrawBackground();
                Brush myBrush = Brushes.Black;
                // Determine the color of the brush to draw each item based on 
                // the index of the item to draw.
                Bots.TeachersAssistant.PlayerState state = Bots.TeachersAssistant.GetPlayerState(e.Index);
                switch (state)
                {
                    case Bots.TeachersAssistant.PlayerState.Active:
                        myBrush = Brushes.Green;
                        break;
                    case Bots.TeachersAssistant.PlayerState.Inactive:
                        myBrush = Brushes.Red;
                        break;
                    case Bots.TeachersAssistant.PlayerState.OutOfRange:
                        myBrush = Brushes.Blue;
                        break;
                }
                //
                // Draw the current item text based on the current 
                // Font and the custom brush settings.
                //
                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                    e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
                //
                // If the ListBox has focus, draw a focus rectangle 
                // around the selected item.
                //
                e.DrawFocusRectangle();
            }
        }
        #endregion Windows Event Handlers
    }
}