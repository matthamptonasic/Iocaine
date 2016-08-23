using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Data.Structures;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Tools;

namespace Iocaine2.Bots
{
    public static class Navigation
    {
        #region Enums
        public enum PROCESSING_STATUS : byte
        {
            STOPPED = 0,
            RUNNING = 1,
            PAUSED = 2,
            UNKNOWN = 3
        }
        #endregion Enums
        #region Structures
        public struct Nav_Trip_Processing_Param
        {
            public Nav_Trip Trip;
            public Nav_Trip FullTrip;
            public bool PlaySound;
            public String SoundToPlay;
            public UInt32 LoopCount;
            public Iocaine_2_Form.Nav_Prc_setLoopCountDelegate SetLoopCountPtr;
        }
        #endregion Structures
        #region Members
        private static Audio player = new Audio();
        private static byte m_nbSequenceTries = 1;
        private static uint m_delayBetweenSequenceTries = 3000;
        private static byte m_nbTargetNpcTries = 3;
        #region Movement
        private static bool releasedForwardKey = false;
        private static float facingMargin = (float)(Math.PI / 8);
        private static float nodeDistanceMargin = 1.0f;
        private static float maxJumpDistance = 8.0f;
        private static float nodeInitialHeadingMargin = 0.5f;
        private static float nodeRunningHeadingMarginSmall = 0.05f;
        private static float nodeRunningHeadingMarginLarge = 1f;
        private static uint nodeZoneMovementTime = 4000;
        private static uint nodeZoneDelayTime = 10000;
        private static uint posNodeLoopDelay = 100;
        public static Keys MoveForwardKey = Keys.NumPad8;
        public static Keys MoveBackKey = Keys.NumPad2;
        public static Keys MoveLeftKey = Keys.NumPad4;
        public static Keys MoveRightKey = Keys.NumPad6;
        #endregion Movement
        #region Trip Processing
        #region Time Delays
        private static UInt32 timeWaitAfterCommand = 1000;
        private static UInt32 timeKeystrokePress = 200;
        private static UInt32 timeWaitAfterTarget = 1000;
        private static UInt32 timeWaitAfterTrade = 1000;
        private static UInt32 timeWaitAfterZone = 6000;
        #endregion Time Delays
        #region Status
        private static PROCESSING_STATUS processingStatus = PROCESSING_STATUS.STOPPED;
        #endregion Status
        #region Time Remaining
        private static UInt32 timeRemainingMs = 0;
        private static object syncRoot = new object();
        private static bool pauseTimeRemainingThread = false;
        private static UInt32 timeRemainingLoopDelay = 100;
        #endregion Time Remaining
        #endregion Trip/Route Processing
        #region Threading
        private static Thread processingThread = null;
        private static Thread timeRemainingThread = null;
        #endregion Threading
        #endregion Members
        #region Properties
        public static PROCESSING_STATUS ProcessingStatus
        {
            get
            {
                return processingStatus;
            }
        }
        #endregion Properties
        #region Basic Movement
        public static void MoveForward(UInt32 iTimeMs)
        {
            try
            {
                IocaineFunctions.keyDown(Keys.NumPad8, iTimeMs);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Navigation::MoveForward: " + e.ToString());
            }
        }
        public static void Stand()
        {
            try
            {
                if (MemReads.Self.get_status() == (int)FFXIEnums.STATUS.HEALING)
                {
                    do
                    {
                        IocaineFunctions.keyDown(Keys.NumPad8, 500);
                    } while (MemReads.Self.get_status() == (int)FFXIEnums.STATUS.HEALING);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::Stand: " + e.ToString());
            }
        }
        #endregion Basic Movement
        #region Advanced Movement
        #region Following
        public static bool FollowPlayer(String iPlayerName)
        {
            try
            {
                UInt32 playerPntr = MemReads.PCs.get_pointer(iPlayerName);
                return FollowPlayer(playerPntr, (float)0);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::FollowPlayer(String): " + e.ToString());
                return false;
            }
        }
        public static bool FollowPlayer(String iPlayerName, float iMinDistance)
        {
            try
            {
                UInt32 playerPntr = MemReads.PCs.get_pointer(iPlayerName);
                return FollowPlayer(playerPntr, iMinDistance);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::FollowPlayer(String, float): " + e.ToString());
                return false;
            }
        }
        public static bool FollowPlayer(UInt32 iPlayerPointer, float iMinDistance)
        {
            try
            {
                String playerName = MemReads.PCs.get_name(iPlayerPointer);
                return FollowPlayer(playerName, iPlayerPointer, iMinDistance);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::FollowPlayer(UInt32, float): " + e.ToString());
                return false;
            }
        }
        public static bool FollowPlayer(String iPlayerName, UInt32 iPlayerPointer, float iMinDistance)
        {
            try
            {
                if (!Interaction.TargetPlayer(iPlayerName))
                {
                    return false;
                }
                //Now he should be targeted
                if (MemReads.Target.get_distance() > iMinDistance)
                {
                    Stand();
                    float lastPosX = MemReads.Self.Position.get_x();
                    //updateStatusBox("Following " + charToFollow.Name + ", distance = " + charToFollow.Distance, Statics.Fields.Green);
                    if (!MemReads.PCs.get_pointer_valid(iPlayerPointer))
                    {
                        //The player most likely went thru a zone.  Just move forward for 5 seconds
                        //and hopefully we'll follow.
                        MoveForward(3000);
                        return true;
                    }
                    IocaineFunctions.keys("/follow " + iPlayerName);
                    do
                    {
                        Stand();
                        IocaineFunctions.delay(400);
                        float posX = MemReads.Self.Position.get_x();
                        if ((posX == lastPosX) || (MemReads.Target.get_name() != iPlayerName))
                        {
                            if (!Interaction.TargetPlayer(iPlayerName))
                            {
                                if (!MemReads.PCs.get_pointer_valid(iPlayerPointer))
                                {
                                    //The player most likely went thru a zone.  Just move forward for 5 seconds
                                    //and hopefully we'll follow.
                                    //moveForward(5000);
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            IocaineFunctions.keys("/follow " + iPlayerName);
                        }
                        lastPosX = posX;
                        LoggingFunctions.Debug("Nav::FollowPlayer: Distance to target is now " + MemReads.Target.get_distance() + ".", LoggingFunctions.DBG_SCOPE.NAV);
                    }
                    while (MemReads.Target.get_distance() > iMinDistance);
                    IocaineFunctions.keyDown(Keys.NumPad7, 400);
                }
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::FollowPlayer(String, UInt32, float): " + e.ToString());
                return false;
            }
        }
        #endregion Following
        public static void MoveToTarget(float iDistance)
        {
            try
            {
                while (iDistance < MemReads.Target.get_distance())
                {
                    LoggingFunctions.Timestamp("Distance to target is now: " + MemReads.Target.get_distance());
                    if (MemReads.Target.get_name() == "")
                    {
                        break;
                    }
                    IocaineFunctions.holdKey(Keys.NumPad8);
                    IocaineFunctions.delay(100);
                }
                IocaineFunctions.releaseKey(Keys.NumPad8);
                //Check that target is within a 60 degree field of vision.
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::MoveToTarget: " + e.ToString());
            }
        }
        public static void FaceTarget()
        {
            try
            {
                float myAngle = MemReads.Self.Position.get_heading();
                float tgtAngle = AngleToTarget();
                float difference = tgtAngle - myAngle;
                bool turnRight = false;

                while (Math.Abs(difference) > (facingMargin / 2))
                {
                    if (MemReads.Target.get_name() == "")
                    {
                        break;
                    }
                    if (MemReads.Target.get_locked())
                    {
                        //Target is locked, just tap the side key to turn
                        IocaineFunctions.keyDown(Keys.NumPad6, 200);
                    }
                    else
                    {
                        //Target is NOT locked, need to actually turn around
                        if (difference > (float)Math.PI)
                        {
                            turnRight = false;
                        }
                        else if (difference < (float)-Math.PI)
                        {
                            turnRight = true;
                        }
                        else if (difference >= 0)
                        {
                            turnRight = true;
                        }
                        else
                        {
                            turnRight = false;
                        }
                        setFirstPerson();
                        //Key down whichever direction to turn
                        if (turnRight)
                        {
                            IocaineFunctions.holdKey(Keys.NumPad6);
                        }
                        else
                        {
                            IocaineFunctions.holdKey(Keys.NumPad4);
                        }
                        IocaineFunctions.delay(100);
                    }
                    myAngle = MemReads.Self.Position.get_heading();
                    tgtAngle = AngleToTarget();
                    difference = tgtAngle - myAngle;
                }
                IocaineFunctions.releaseKey(Keys.NumPad6);
                IocaineFunctions.releaseKey(Keys.NumPad4);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::FaceTarget: " + e.ToString());
            }
        }
        #endregion Advanced Movement
        #region Non-Movement Actions
        #region View
        private static void setFirstPerson()
        {
            try
            {
                if (MemReads.Self.Camera.get_view_perspective() == 0)
                {
                    IocaineFunctions.keyDown(Keys.NumPad5, 200);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::setFirstPerson: " + e.ToString());
            }
        }
        private static void setThirdPerson()
        {
            try
            {
                if (MemReads.Self.Camera.get_view_perspective() == 1)
                {
                    IocaineFunctions.keyDown(Keys.NumPad5, 200);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::setThirdPerson: " + e.ToString());
            }
        }
        #endregion View
        #endregion Non-Movement Actions
        #region Math Functions
        #region Heading
        public static float AngleToTarget()
        {
            float theta = 0;
            try
            {
                float myX = MemReads.Self.Position.get_x();
                float myY = MemReads.Self.Position.get_y();
                float targetX = MemReads.Target.get_position_x();
                float targetY = MemReads.Target.get_position_y();
                if (targetX == myX)
                {
                    if (targetY > myY)
                    {
                        return (float)(-Math.PI / 4);
                    }
                    else
                    {
                        return (float)(Math.PI / 4);
                    }
                }
                theta = (float)Math.Atan((targetY - myY) / (targetX - myX)); // * (float)Math.PI / 180;
                if (targetX > myX)
                {
                    theta = theta * -1;
                }
                else if (targetY > myY)
                {
                    theta = -3.1415f - theta;
                }
                else
                {
                    theta = (float)Math.PI - theta;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::AngleToTarget: " + e.ToString());
            }
            return theta;
        }
        public static float AngleToPosition(float iPosX, float iPosY)
        {
            float theta = 0;
            try
            {
                float myX = MemReads.Self.Position.get_x();
                float myY = MemReads.Self.Position.get_y();
                if (iPosX == myX)
                {
                    if (iPosY > myY)
                    {
                        return (float)(-Math.PI / 4);
                    }
                    else
                    {
                        return (float)(Math.PI / 4);
                    }
                }
                theta = (float)Math.Atan((iPosY - myY) / (iPosX - myX)); // * (float)Math.PI / 180;
                if (iPosX > myX)
                {
                    theta = theta * -1;
                }
                else if (iPosY > myY)
                {
                    theta = (-1 * (float)Math.PI) - theta;
                }
                else
                {
                    theta = (float)Math.PI - theta;
                }
                LoggingFunctions.Debug("Nav::AngleToPosition: angle to next pos = " + theta + ".", LoggingFunctions.DBG_SCOPE.NAV);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::AngleToPosition: " + e.ToString());
            }
            return theta;
        }
        public static bool AngleGreaterThanMargin(float iNextPosX, float iNextPosY)
        {
            return AngleGreaterThanMargin(iNextPosX, iNextPosY, nodeInitialHeadingMargin);
        }
        public static bool AngleGreaterThanMargin(float iNextPosX, float iNextPosY, float iMargin)
        {
            try
            {
                float myX = MemReads.Self.Position.get_x();
                float myY = MemReads.Self.Position.get_y();
                float myH = MemReads.Self.Position.get_heading();
                float headingToNextPos = 0f;
                if (iNextPosX == myX)
                {
                    if (iNextPosY > myY)
                    {
                        headingToNextPos = (float)(-Math.PI / 4);
                    }
                    else
                    {
                        headingToNextPos = (float)(Math.PI / 4);
                    }
                }
                else
                {
                    headingToNextPos = (float)Math.Atan((iNextPosY - myY) / (iNextPosX - myX)); // * (float)Math.PI / 180;
                    if (iNextPosX > myX)
                    {
                        headingToNextPos = headingToNextPos * -1;
                    }
                    else if (iNextPosY > myY)
                    {
                        headingToNextPos = (-1 * (float)Math.PI) - headingToNextPos;
                    }
                    else
                    {
                        headingToNextPos = (float)Math.PI - headingToNextPos;
                    }
                }
                float delta = myH - headingToNextPos;
                if (delta >= (float)Math.PI)
                {
                    //If delta > pi, subtract it from 2pi.
                    delta = (2 * (float)Math.PI) - delta;
                }
                else if (delta <= ((2 * -1) * (float)Math.PI))
                {
                    //If delta < -pi, add 2pi to it.
                    delta += (2 * (float)Math.PI);
                }
                LoggingFunctions.Debug("Navigation::AngleGreaterThanMargin: My heading: " + myH + ", tHeading: " + headingToNextPos + ", delta: " + delta + ", my pos: (" + myX + ", " + myY + ", next pos: (" + iNextPosX + ", " + iNextPosY + ").", LoggingFunctions.DBG_SCOPE.NAV);
                if (Math.Abs(delta) > iMargin)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::AngleGreaterThanMargin(float, float, float): " + e.ToString());
            }
            return false;
        }
        #endregion Heading
        #region Distance
        public static float DistanceToPosition(float iPosX, float iPosY)
        {
            try
            {
                float myX = MemReads.Self.Position.get_x();
                float myY = MemReads.Self.Position.get_y();
                return (float)Math.Sqrt(Math.Pow(iPosX - myX, 2) + Math.Pow(iPosY - myY, 2));
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::DistanceToPosition: " + e.ToString());
                return -1;
            }
        }
        #endregion Distance
        #endregion Math Functions
        #region Trip Processing
        #region Main Functions
        private static void processTrip(Object iTripParam)
        {
            try
            {
                bool wasFirstPerson = MemReads.Self.Camera.get_view_perspective() == 1;
                Nav_Trip_Processing_Param iParam = (Nav_Trip_Processing_Param)iTripParam;
                Nav_Trip trip = (Nav_Trip)iParam.Trip;
                Nav_Trip tripCopy = new Nav_Trip(trip);
                Nav_Trip fullTrip = (Nav_Trip)iParam.FullTrip;
                Nav_Trip fullTripCopy = new Nav_Trip(fullTrip);
                UInt32 loopCount = (UInt32)(iParam.LoopCount > 0 ? iParam.LoopCount : 1);
                Iocaine_2_Form.Nav_Prc_setLoopCountDelegate setLoopCountPtr = iParam.SetLoopCountPtr;
                UInt32 loopsProcessed = 0;

                //Update the time remaining variable and start the thread to track the time remaining label.
                updateTimeRemaining((Iocaine_2_Form.Nav_getTripTime(tripCopy) + ((loopCount - 1) * Iocaine_2_Form.Nav_getTripTime(fullTripCopy))) * 1000);
                timeRemainingThread = new Thread(new ThreadStart(timeRemainingThreadFunction));
                timeRemainingThread.Name = "timeRemainingThread";
                timeRemainingThread.IsBackground = true;
                timeRemainingThread.Start();

                for (int ii = 0; ii < trip.TripRoutes.Count; ii++)
                {
                    LoggingFunctions.Debug("Nav::processTrip: Processing trip[" + ii + "] of " + (trip.TripRoutes.Count - 1) + ".", LoggingFunctions.DBG_SCOPE.NAV);
                    if (!processRoute(trip.TripRoutes[ii], tripCopy))
                    {
                        LoggingFunctions.Error("In ProcessTrip, route[" + ii + "]: " + trip.TripRoutes[ii].ToString() + " returned false.");
                        StopProcessing();
                        Statics.FuncPtrs.SetNavButtonPtr("S&tart", Statics.Buttons.Green);
                        return;
                    }
                }
                if (loopCount > 1)
                {
                    loopsProcessed++;
                    if (setLoopCountPtr == null)
                    {
                        LoggingFunctions.Error("Nav::processTrip: setLoopCountPtr was null when the loop count was passed as " + loopCount + ". Exiting...");
                    }
                    else
                    {
                        Nav_Trip localSubTrip = Iocaine_2_Form.Nav_getSubTrip(fullTrip,
                                                                              MemReads.Self.get_zone_id(),
                                                                              MemReads.Self.Position.get_x(),
                                                                              MemReads.Self.Position.get_y());
                        Nav_Trip localSubTripCopy = new Nav_Trip(localSubTrip);
                        setLoopCountPtr(loopCount - loopsProcessed);
                        while ((loopCount - loopsProcessed) > 0)
                        {
                            for (int ii = 0; ii < localSubTrip.TripRoutes.Count; ii++)
                            {
                                LoggingFunctions.Debug("Nav::processTrip: Processing full trip[" + ii + "] of " + (localSubTrip.TripRoutes.Count - 1) + ".", LoggingFunctions.DBG_SCOPE.NAV);
                                if (!processRoute(localSubTrip.TripRoutes[ii], localSubTripCopy))
                                {
                                    LoggingFunctions.Error("In ProcessTrip, route[" + ii + "]: " + localSubTrip.TripRoutes[ii].ToString() + " returned false.");
                                    StopProcessing();
                                    Statics.FuncPtrs.SetNavButtonPtr("S&tart", Statics.Buttons.Green);
                                    return;
                                }
                            }
                            loopsProcessed++;
                            // Do not decrement the up/down count if this was the last loop (leave it at 1).
                            if ((loopCount - loopsProcessed) > 0)
                            {
                                setLoopCountPtr(loopCount - loopsProcessed);
                            }
                        }
                    }
                }

                processingStatus = PROCESSING_STATUS.STOPPED;
                checkProcessingStatus();
                if (!wasFirstPerson)
                {
                    setThirdPerson();
                }
                if (iParam.PlaySound)
                {
                    player.PlaySound(iParam.SoundToPlay);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::processTrip: " + e.ToString());
            }
        }
        private static bool processRoute(Nav_Route iRoute, Nav_Trip iTrip)
        {
            try
            {
                if (iRoute == null)
                {
                    LoggingFunctions.Error("In ProcessRoute, iRoute was null.");
                    return false;
                }
                if (iRoute.Direction == true)
                {
                    for (int ii = 0; ii < iRoute.RouteNodes.Count; ii++)
                    {
                        if (!checkProcessingStatus())
                        {
                            return true;
                        }
                        Routes.UserRoutesRow nodeCurrent = iRoute.RouteNodes[ii];
                        Routes.UserRoutesRow nodeNext = null;
                        if (ii != (iRoute.RouteNodes.Count - 1))
                        {
                            //This is NOT the last node in the route, so just send the next node in the sequence.
                            nodeNext = iRoute.RouteNodes[ii + 1];
                        }
                        else
                        {
                            //This IS the last node in the route. Check if there is a following route,
                            //and if so, send the first node in that route (based on direction).
                            if (iTrip.TripRoutes.Count > 1)
                            {
                                //We are not the last route in the trip.
                                if (iTrip.TripRoutes[1].RouteNodes.Count > 0)
                                {
                                    if (iTrip.TripRoutes[1].Direction == true)
                                    {
                                        nodeNext = iTrip.TripRoutes[1].RouteNodes[0];
                                    }
                                    else
                                    {
                                        nodeNext = iTrip.TripRoutes[1].RouteNodes[iTrip.TripRoutes[1].RouteNodes.Count - 1];
                                    }
                                }
                            }
                        }
                        if (!processNode(nodeCurrent, nodeNext))
                        {
                            LoggingFunctions.Error("In ProcessRoute, node " + ii + " in route " + iRoute.RouteName + " returned false.");
                            return false;
                        }
                        clipOneNode(iTrip);
                        updateTimeRemaining(Iocaine_2_Form.Nav_getTripTime(iTrip) * 1000);
                    }
                }
                else
                {
                    for (int ii = iRoute.RouteNodes.Count - 1; ii >= 0; ii--)
                    {
                        if (!checkProcessingStatus())
                        {
                            return true;
                        }
                        Routes.UserRoutesRow nodeCurrent = iRoute.RouteNodes[ii];
                        Routes.UserRoutesRow nodeNext = null;
                        if (ii != 0)
                        {
                            //This is NOT the last node in the route, so just send the next node in the sequence.
                            nodeNext = iRoute.RouteNodes[ii - 1];
                        }
                        else
                        {
                            //This IS the last node in the route. Check if there is a following route,
                            //and if so, send the first node in that route (based on direction).
                            if (iTrip.TripRoutes.Count > 1)
                            {
                                //We are not the last route in the trip.
                                if (iTrip.TripRoutes[1].RouteNodes.Count > 0)
                                {
                                    if (iTrip.TripRoutes[1].Direction == true)
                                    {
                                        nodeNext = iTrip.TripRoutes[1].RouteNodes[0];
                                    }
                                    else
                                    {
                                        nodeNext = iTrip.TripRoutes[1].RouteNodes[iTrip.TripRoutes[1].RouteNodes.Count - 1];
                                    }
                                }
                            }
                        }
                        if (!processNode(nodeCurrent, nodeNext))
                        {
                            LoggingFunctions.Error("In ProcessRoute, node " + ii + " in route " + iRoute.RouteName + " returned false.");
                            return false;
                        }
                        clipOneNode(iTrip);
                        updateTimeRemaining(Iocaine_2_Form.Nav_getTripTime(iTrip) * 1000);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::processRoute: " + e.ToString());
                return false;
            }
        }
        private static bool processNode(Routes.UserRoutesRow iNodeCurrent, Routes.UserRoutesRow iNodeNext)
        {
            try
            {
                LoggingFunctions.Debug("Nav::processNode: Processing node type: " + ((Iocaine_2_Form.NAV_NODE_TYPE)iNodeCurrent.NodeType).ToString() + ", " + "routeID: " + iNodeCurrent.RouteID + ", nodeID: " + iNodeCurrent.NodeID + ", detail: " + iNodeCurrent.NodeDetail + ".", LoggingFunctions.DBG_SCOPE.NAV);
                Statics.FuncPtrs.SetStatusBoxPtr("Processing " + iNodeCurrent.NodeDetail, Statics.Fields.Green);
                switch (iNodeCurrent.NodeType)
                {
                    case (ushort)Iocaine_2_Form.NAV_NODE_TYPE.COMMAND:
                        ReleaseAllMovementKeys(true);
                        IocaineFunctions.keys(iNodeCurrent.NodeDetail);
                        IocaineFunctions.delay(timeWaitAfterCommand);
                        return true;
                    case (ushort)Iocaine_2_Form.NAV_NODE_TYPE.KEYSTROKE:
                        ReleaseAllMovementKeys(true);
                        String keyString = Statics.Constants.Navigation.KeystrokeStrings[(int)iNodeCurrent.NodeData];
                        if (keyString.Contains("Arrow"))
                        {
                            IocaineFunctions.arrowKeyDown(Statics.Constants.Navigation.KeystrokesKeyMap[Statics.Constants.Navigation.KeystrokeStrings[(int)iNodeCurrent.NodeData]], timeKeystrokePress);
                        }
                        else
                        {
                            IocaineFunctions.keyDown(Statics.Constants.Navigation.KeystrokesKeyMap[Statics.Constants.Navigation.KeystrokeStrings[(int)iNodeCurrent.NodeData]], timeKeystrokePress);
                        }
                        return true;
                    case (ushort)Iocaine_2_Form.NAV_NODE_TYPE.NPC_TARGET:
                        ReleaseAllMovementKeys(true);
                        String targetName = "";
                        if (!Iocaine_2_Form.Nav_decodeStringToNpcName(iNodeCurrent.NodeDetail, ref targetName))
                        {
                            LoggingFunctions.Error("In ProcessNonPosNode: Could not decode node detail '" + iNodeCurrent.NodeDetail + "' to target name.");
                            return false;
                        }
                        bool failed = true;
                        for (int ii = 0; ii < m_nbTargetNpcTries; ii++)
                        {
                            if (Interaction.TargetNPC(targetName))
                            {
                                failed = false;
                                break;
                            }
                        }
                        if (failed)
                        {
                            LoggingFunctions.Error("In ProcessNonPosNode: Could not target " + targetName + ".");
                            return false;
                        }
                        else
                        {
                            IocaineFunctions.delay(timeWaitAfterTarget);
                            return true;
                        }
                    case (ushort)Iocaine_2_Form.NAV_NODE_TYPE.NPC_TRADE_GIL:
                        ReleaseAllMovementKeys(true);
                        if (!Interaction.TradeGilNpc(iNodeCurrent.NodeData))
                        {
                            LoggingFunctions.Error("In ProcessNonPosNode: Could not trade " + iNodeCurrent.NodeData + " gil to npc.");
                            return false;
                        }
                        else
                        {
                            IocaineFunctions.delay(timeWaitAfterTrade);
                            return true;
                        }
                    case (ushort)Iocaine_2_Form.NAV_NODE_TYPE.NPC_TRADE_ITEM:
                        ReleaseAllMovementKeys(true);
                        byte quan = 0;
                        String name = "";
                        ushort itemId = 0;
                        if (!Iocaine_2_Form.Nav_decodeStringToItem(iNodeCurrent.NodeDetail, ref name, ref quan))
                        {
                            LoggingFunctions.Error("In ProcessNonPosNode: Could not decode node detail '" + iNodeCurrent.NodeDetail + "' to trade item.");
                            return false;
                        }
                        itemId = Things.GetIdFromName(name);
                        if (itemId == 0)
                        {
                            LoggingFunctions.Error("In ProcessNonPosNode: Could not get item ID for item '" + name + "'.");
                            return false;
                        }
                        if (!Interaction.TradeItemToNpc(itemId, quan))
                        {
                            LoggingFunctions.Error("In ProcessNonPosNode: Could not perform " + iNodeCurrent.NodeDetail + ".");
                            return false;
                        }
                        else
                        {
                            IocaineFunctions.delay(timeWaitAfterTrade);
                            return true;
                        }
                    case (ushort)Iocaine_2_Form.NAV_NODE_TYPE.WAIT:
                        ReleaseAllMovementKeys(true);
                        IocaineFunctions.delay(iNodeCurrent.NodeData);
                        return true;
                    case (ushort)Iocaine_2_Form.NAV_NODE_TYPE.IOC_SEQUENCE:
                        ushort id = (ushort)iNodeCurrent.NodeData;
                        return processSequence(id);
                    default:
                        if ((iNodeCurrent.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_START)
                            || (iNodeCurrent.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_NODE)
                            || (iNodeCurrent.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_END)
                            || (iNodeCurrent.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_ZONE))
                        {
                            return processPosNode(iNodeCurrent, iNodeNext, false);
                        }
                        else
                        {
                            MessageBox.Show("[ERROR] In ProcessNonPosNode: Got an unknown node type: " + iNodeCurrent.NodeType.ToString(), "Unknown node type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::processNode: " + e.ToString());
                return false;
            }
        }
        private static bool processPosNode(Routes.UserRoutesRow iNodeCurrent, Routes.UserRoutesRow iNodeNext, bool iReleaseKeyAlways)
        {
            try
            {
                //Regardless of the node type we should always check that we're
                //in first person mode before we go to start moving.
                setFirstPerson();

                //We also need to make sure that we're in the correct zone.
                //If someone zones via npc warp and they don't do a Wait after the
                //npc interaction, they'll start trying to move in the previous zone.
                int zoneWaitCount = 30;
                bool waitedForZone = false;
                while (iNodeCurrent.NodeZoneID != MemReads.Self.get_zone_id())
                {
                    waitedForZone = true;
                    Statics.FuncPtrs.SetStatusBoxPtr("Waiting for zone change to " + iNodeCurrent.NodeZoneID, Statics.Fields.Yellow);
                    IocaineFunctions.releaseKey(MoveForwardKey);
                    releasedForwardKey = true;
                    IocaineFunctions.delay(1000);
                    if (zoneWaitCount-- == 0)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Timed out waiting for zone change, check log for info.", Statics.Fields.Red);
                        LoggingFunctions.Error("Timed out waiting for zone change from " + MemReads.Self.get_zone_id() + " to " + iNodeCurrent.NodeZoneID + " on node[" + iNodeCurrent.NodeID + "]");
                        return false;
                    }
                }
                if (waitedForZone == true)
                {
                    //We just zoned, we need to wait for a few seconds for the
                    //position data to get loaded.
                    IocaineFunctions.delay(timeWaitAfterZone);
                }

                //If this is a start node we'll begin by changing our heading to 
                //face the direction of the coordinates.
                if (AngleGreaterThanMargin(iNodeCurrent.NodePosX, iNodeCurrent.NodePosY))
                {
                    MemReads.Self.Position.set_heading(AngleToPosition(iNodeCurrent.NodePosX, iNodeCurrent.NodePosY));
                }

                //We shouldn't need to hold the forward movement key if this is
                //NOT a start node, but we'll do so anyway just in case something
                //got messed up in the database.
                IocaineFunctions.holdKey(MoveForwardKey);

                //Now we start our loop.
                //The loop will check for the distance to the next point and break once
                //we're closer than the margin OR our status is not RUNNING.
                //While we're in the loop we'll continually check that our heading is correct
                //and if it's not we'll keep changing the heading.
                //If the heading delta is large we'll kick off a thread that will adjust
                //the heading over a period of time (not implemented in initial release).
                //If the heading delta is small we'll just write the new heading to memory.
                while (checkProcessingStatus() && (DistanceToPosition(iNodeCurrent.NodePosX, iNodeCurrent.NodePosY) > nodeDistanceMargin))
                {
                    if (releasedForwardKey)
                    {
                        IocaineFunctions.holdKey(MoveForwardKey);
                        releasedForwardKey = false;
                    }
                    if (AngleGreaterThanMargin(iNodeCurrent.NodePosX, iNodeCurrent.NodePosY, nodeRunningHeadingMarginLarge))
                    {
                        //This is where we'll kick off a thread that will set the heading gradually.
                        //For now we'll just write it to mem and change it later if it looks unnatural.
                        MemReads.Self.Position.set_heading(AngleToPosition(iNodeCurrent.NodePosX, iNodeCurrent.NodePosY));
                    }
                    else if (AngleGreaterThanMargin(iNodeCurrent.NodePosX, iNodeCurrent.NodePosY, nodeRunningHeadingMarginSmall))
                    {
                        MemReads.Self.Position.set_heading(AngleToPosition(iNodeCurrent.NodePosX, iNodeCurrent.NodePosY));
                    }
                    IocaineFunctions.delay(posNodeLoopDelay);
                }

                //Once we're close to the point we'll check if we're at an end point (release key)
                //or a zone point (zone forward).
                bool stopDueToEndPoint = false;
                if (iNodeCurrent.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_END)
                {
                    if (iNodeNext == null)
                    {
                        stopDueToEndPoint = true;
                    }
                    else if ((iNodeNext.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_END)
                        || (iNodeNext.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_NODE)
                        || (iNodeNext.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_START)
                        || (iNodeNext.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_ZONE))
                    {
                        stopDueToEndPoint = false;
                    }
                    else
                    {
                        stopDueToEndPoint = true;
                    }
                }
                if (iNodeCurrent.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_ZONE)
                {
                    zoneForward();
                }
                if ((iNodeCurrent.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_ZONE) || (stopDueToEndPoint == true) || iReleaseKeyAlways)
                {
                    IocaineFunctions.releaseKey(MoveForwardKey);
                    IocaineFunctions.delay(500);
                }
                if (iNodeCurrent.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_END)
                {
                    ushort zoneId = MemReads.Self.get_zone_id();
                    if (iNodeCurrent.NodeZoneID != zoneId)
                    {
                        LoggingFunctions.Error("Got to an end node and found that we're not in the right zone.");
                        LoggingFunctions.Error("Got Zone: " + zoneId + ", Exp Zone: " + iNodeCurrent.NodeZoneID);
                        return false;
                    }
                    float distToNodePos = DistanceToPosition(iNodeCurrent.NodePosX, iNodeCurrent.NodePosY);
                    if (distToNodePos > maxJumpDistance)
                    {
                        LoggingFunctions.Error("Got to an end node and found that we're still " + distToNodePos + " away from the node.");
                        return false;
                    }
                    if (stopDueToEndPoint == true)
                    {
                        MemReads.Self.Position.set_x(iNodeCurrent.NodePosX);
                        MemReads.Self.Position.set_y(iNodeCurrent.NodePosY);
                        IocaineFunctions.delay(500);
                    }
                }
                if ((iNodeCurrent.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_ZONE) || (iNodeCurrent.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_END))
                {
                    //Set our heading.
                    MemReads.Self.Position.set_heading(iNodeCurrent.NodePosHeading);
                }
                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::processPosNode: " + e.ToString());
                return false;
            }
        }
        private static void processOneNode(Object iNode)
        {
            try
            {
                Routes.UserRoutesRow node = (Routes.UserRoutesRow)iNode;

                //Update the time remaining variable and start the thread to track the time remaining label.
                updateTimeRemaining(getNodeTime(node) * 1000);
                timeRemainingThread = new Thread(new ThreadStart(timeRemainingThreadFunction));
                timeRemainingThread.Name = "timeRemainingThread";
                timeRemainingThread.IsBackground = true;
                timeRemainingThread.Start();

                bool wasFirstPerson = MemReads.Self.Camera.get_view_perspective() == 1;
                if ((node.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_END)
                    || (node.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_NODE)
                    || (node.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_START)
                    || (node.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_ZONE))
                {
                    processPosNode(node, null, true);
                }
                else
                {
                    processNode(node, null);
                }
                processingStatus = PROCESSING_STATUS.STOPPED;
                checkProcessingStatus();
                if (!wasFirstPerson)
                {
                    setThirdPerson();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::processOneNode: " + e.ToString());
            }
        }
        private static bool processSequence(ushort iSeqId)
        {
            ActionSequence seq = ActionManager.GetSequence(iSeqId);
            if ((seq == null) || !seq.IsCapable())
            {
                return false;
            }
            byte cnt = 1;
            while (cnt <= m_nbSequenceTries)
            {
                cnt++;
                if (seq.CanPerform())
                {
                    // TBA - Add reason why it cannot be processed and act accordingly.
                    if (seq.Execute())
                    {
                        return true;
                    }
                }
                IocaineFunctions.delay(m_delayBetweenSequenceTries);
                continue;
            }
            return false;
        }
        #endregion Main Functions
        #region Time Functions
        private static void timeRemainingThreadFunction()
        {
            try
            {
                while ((processingStatus != PROCESSING_STATUS.STOPPED) && (timeRemainingMs > 0))
                {
                    while (pauseTimeRemainingThread == true)
                    {
                        IocaineFunctions.delay(timeRemainingLoopDelay);
                    }
                    Monitor.Enter(syncRoot);
                    try
                    {
                        Statics.FuncPtrs.SetTimeRemainingPtr(timeRemainingMs);
                        if (timeRemainingMs >= timeRemainingLoopDelay)
                        {
                            timeRemainingMs -= timeRemainingLoopDelay;
                        }
                        else
                        {
                            timeRemainingMs = 0;
                        }
                        IocaineFunctions.delay(timeRemainingLoopDelay);
                    }
                    catch (Exception e)
                    {
                        LoggingFunctions.Error("In timeRemainingThreadFunction: " + e.ToString());
                    }
                    finally
                    {
                        Monitor.Exit(syncRoot);
                    }
                }
                Statics.FuncPtrs.SetTimeRemainingPtr(0);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::timeRemainingThreadFunction: " + e.ToString());
            }
        }
        private static void updateTimeRemaining(UInt32 iTimeMs)
        {
            Monitor.Enter(syncRoot);
            try
            {
                timeRemainingMs = iTimeMs;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::updateTimeRemaining: " + e.ToString());
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }
        private static UInt32 getNodeTime(Routes.UserRoutesRow iNode)
        {
            UInt32 time = 0;
            try
            {
                if ((iNode.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_START)
                    || (iNode.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_NODE)
                    || (iNode.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_END)
                    || (iNode.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_ZONE))
                {
                    float distanceToNode = DistanceToPosition(iNode.NodePosX, iNode.NodePosY);
                    time = (UInt32)(distanceToNode / Statics.Constants.Navigation.SpeedWalking);
                    if (iNode.NodeType == (ushort)Iocaine_2_Form.NAV_NODE_TYPE.POS_ZONE)
                    {
                        time += (UInt32)Statics.Constants.Navigation.TimeZone;
                    }
                }
                else
                {
                    Nav_Route routeWrapper = new Nav_Route();
                    routeWrapper.Direction = true;
                    routeWrapper.RouteNodes.Add(Routes.CloneRouteRow(Statics.Datasets.RoutesDb, iNode));
                    time = Iocaine_2_Form.Nav_getRouteTime(routeWrapper, 0d);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::getNodeTime: " + e.ToString());
            }
            return time;
        }
        #endregion Time Functions
        #region Status Functions
        private static bool checkProcessingStatus()
        {
            try
            {
                if (processingStatus != PROCESSING_STATUS.RUNNING)
                {
                    if (processingStatus == PROCESSING_STATUS.STOPPED)
                    {
                        ReleaseAllMovementKeys(true);
                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping Trip Navigation", Statics.Fields.Blue);
                        Statics.FuncPtrs.SetNavButtonPtr("S&tart", Statics.Buttons.Green);
                        Statics.FuncPtrs.SetTimeRemainingPtr(0);
                        timeRemainingThread = null;
                        pauseTimeRemainingThread = false;
                        return false;
                    }
                    else if (processingStatus == PROCESSING_STATUS.PAUSED)
                    {
                        ReleaseAllMovementKeys(true);
                        releasedForwardKey = true;
                        Statics.FuncPtrs.SetStatusBoxPtr("Pausing Trip Navigation", Statics.Fields.Blue);
                        Statics.FuncPtrs.SetNavButtonPtr("&Resume", Statics.Buttons.Green);
                        pauseTimeRemainingThread = true;
                        while (processingStatus != PROCESSING_STATUS.RUNNING)
                        {
                            if (processingStatus == PROCESSING_STATUS.STOPPED)
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Stopping Trip Navigation", Statics.Fields.Blue);
                                Statics.FuncPtrs.SetNavButtonPtr("S&tart", Statics.Buttons.Green);
                                Statics.FuncPtrs.SetTimeRemainingPtr(0);
                                timeRemainingThread = null;
                                pauseTimeRemainingThread = false;
                                return false;
                            }
                            IocaineFunctions.delay(1000);
                        }
                        pauseTimeRemainingThread = false;
                    }
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::checkProcessingStatus: " + e.ToString());
                return false;
            }
        }
        public static void StartProcessing(Nav_Trip iTrip)
        {
            StartProcessing(iTrip, iTrip, true, Statics.Settings.Navigation.TripCompleteSound, 1, null);
        }
        public static void StartProcessing(Nav_Trip iTrip, Nav_Trip iFullTrip, bool iPlaySound, String iSound, UInt32 iLoopCount, Iocaine_2_Form.Nav_Prc_setLoopCountDelegate iSetLoopCountPtr)
        {
            try
            {
                if (processingStatus != PROCESSING_STATUS.STOPPED)
                {
                    StopProcessing();
                    while (processingStatus != PROCESSING_STATUS.STOPPED)
                    {
                        IocaineFunctions.delay(100);
                    }
                }
                if (processingThread != null)
                {
                    processingThread = null;
                }
                Nav_Trip_Processing_Param param = new Nav_Trip_Processing_Param();
                param.PlaySound = iPlaySound;
                param.SoundToPlay = iSound;
                param.Trip = iTrip;
                param.FullTrip = iFullTrip;
                param.LoopCount = iLoopCount;
                param.SetLoopCountPtr = iSetLoopCountPtr;
                processingThread = new Thread(new ParameterizedThreadStart(Navigation.processTrip));
                processingThread.Name = "processingThread";
                processingThread.IsBackground = true;
                processingThread.Start(param);
                processingStatus = PROCESSING_STATUS.RUNNING;
                Statics.FuncPtrs.SetNavButtonPtr("&Pause", Statics.Buttons.Yellow);
                Statics.FuncPtrs.SetStatusBoxPtr("Starting route '" + iTrip.TripRoutes[0].ToString() + "'.", Statics.Fields.Green);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::StartProcessing: " + e.ToString());
            }
        }
        public static void ProcessNode(Routes.UserRoutesRow iNode)
        {
            try
            {
                if (processingStatus != PROCESSING_STATUS.STOPPED)
                {
                    StopProcessing();
                    while (processingStatus != PROCESSING_STATUS.STOPPED)
                    {
                        IocaineFunctions.delay(100);
                    }
                }
                if (processingThread != null)
                {
                    processingThread = null;
                }
                processingThread = new Thread(new ParameterizedThreadStart(Navigation.processOneNode));
                processingThread.Name = "ProcessingThread";
                processingThread.IsBackground = true;
                processingThread.Start(iNode);
                processingStatus = PROCESSING_STATUS.RUNNING;
                Statics.FuncPtrs.SetNavButtonPtr("&Pause", Statics.Buttons.Yellow);
                Statics.FuncPtrs.SetStatusBoxPtr("Running node '" + iNode.NodeDetail + "'.", Statics.Fields.Green);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::ProcessNode: " + e.ToString());
            }
        }
        public static void PauseProcessing()
        {
            processingStatus = PROCESSING_STATUS.PAUSED;
        }
        public static void ResumeProcessing()
        {
            processingStatus = PROCESSING_STATUS.RUNNING;
        }
        public static void StopProcessing()
        {
            processingStatus = PROCESSING_STATUS.STOPPED;
        }
        #endregion Status Functions
        #region Trip Utilities
        private static void clipOneNode(Nav_Trip iTrip)
        {
            try
            {
                if ((iTrip == null) || (iTrip.TripRoutes.Count == 0))
                {
                    return;
                }
                if ((iTrip.TripRoutes.Count == 1) && (iTrip.TripRoutes[0].RouteNodes.Count == 0))
                {
                    return;
                }
                Nav_Route currentRoute = iTrip.TripRoutes[0];
                if (currentRoute.RouteNodes.Count == 0)
                {
                    //This route is empty, get rid of it and clip one node from the next route.
                    iTrip.TripRoutes.RemoveAt(0);
                    currentRoute = iTrip.TripRoutes[0];
                }
                if (currentRoute.Direction == true)
                {
                    currentRoute.RouteNodes.RemoveAt(0);
                }
                else
                {
                    currentRoute.RouteNodes.RemoveAt(currentRoute.RouteNodes.Count - 1);
                }
                if (currentRoute.RouteNodes.Count == 0)
                {
                    iTrip.TripRoutes.RemoveAt(0);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::clipOneNode: " + e.ToString());
            }
        }
        #endregion Trip Utilities
        #endregion Trip Processing
        #region Utility Functions
        #region Key State Checks
        public static void ReleaseAllMovementKeys(bool iNumKeypad)
        {
            if (iNumKeypad)
            {
                IocaineFunctions.releaseKey(Keys.NumPad8);
                IocaineFunctions.releaseKey(Keys.NumPad2);
                IocaineFunctions.releaseKey(Keys.NumPad4);
                IocaineFunctions.releaseKey(Keys.NumPad6);
            }
            else
            {
                IocaineFunctions.releaseKey(Keys.W);
                IocaineFunctions.releaseKey(Keys.S);
                IocaineFunctions.releaseKey(Keys.A);
                IocaineFunctions.releaseKey(Keys.D);
            }
        }
        public static void ReleaseAllCameraKeys(bool iNumKeypad)
        {
            if (iNumKeypad)
            {
                IocaineFunctions.releaseKey(Keys.Left);
                IocaineFunctions.releaseKey(Keys.Right);
                IocaineFunctions.releaseKey(Keys.Up);
                IocaineFunctions.releaseKey(Keys.Down);
            }
            else
            {
                IocaineFunctions.releaseKey(Keys.I);
                IocaineFunctions.releaseKey(Keys.K);
                IocaineFunctions.releaseKey(Keys.J);
                IocaineFunctions.releaseKey(Keys.L);
            }
        }
        public static void ReleaseControlKeys()
        {
            IocaineFunctions.releaseKey(Keys.Escape);
            IocaineFunctions.releaseKey(Keys.Enter);
            IocaineFunctions.releaseKey(Keys.Control);
            IocaineFunctions.releaseKey(Keys.ControlKey);
            IocaineFunctions.releaseKey(Keys.Alt);
            IocaineFunctions.releaseKey(Keys.Add);
            IocaineFunctions.releaseKey(Keys.Delete);
            IocaineFunctions.releaseKey(Keys.Divide);
            IocaineFunctions.releaseKey(Keys.End);
            IocaineFunctions.releaseKey(Keys.Home);
            IocaineFunctions.releaseKey(Keys.Insert);
            IocaineFunctions.releaseKey(Keys.LControlKey);
            IocaineFunctions.releaseKey(Keys.Multiply);
            IocaineFunctions.releaseKey(Keys.NumPad0);
            IocaineFunctions.releaseKey(Keys.NumPad1);
            IocaineFunctions.releaseKey(Keys.NumPad3);
            IocaineFunctions.releaseKey(Keys.NumPad5);
            IocaineFunctions.releaseKey(Keys.NumPad7);
            IocaineFunctions.releaseKey(Keys.NumPad9);
            IocaineFunctions.releaseKey(Keys.PageDown);
            IocaineFunctions.releaseKey(Keys.PageUp);
            IocaineFunctions.releaseKey(Keys.Return);
            IocaineFunctions.releaseKey(Keys.Shift);
            IocaineFunctions.releaseKey(Keys.ShiftKey);
            IocaineFunctions.releaseKey(Keys.Space);
            IocaineFunctions.releaseKey(Keys.Subtract);
            IocaineFunctions.releaseKey(Keys.Tab);
        }
        #endregion Key State Checks
        #region Zoning
        private static void zoneForward()
        {
            try
            {
                IocaineFunctions.keyDown(Keys.NumPad8, nodeZoneMovementTime);
                IocaineFunctions.delay(nodeZoneDelayTime);
                while (MemReads.Self.get_is_zoning())
                {
                    IocaineFunctions.delay(1000);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Navigation::zoneForward: " + e.ToString());
            }
        }
        #endregion Zoning
        #endregion Utility Functions
    }
}
