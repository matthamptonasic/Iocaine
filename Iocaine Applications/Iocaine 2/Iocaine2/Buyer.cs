using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Tools;

namespace Iocaine2.Bots
{
    public static class Buyer
    {
        #region Enums
        public enum STATE
        {
            STOPPED = 0,
            RUNNING = 1,
            PAUSED_USER = 2,
            PAUSED_PROG = 3
        }
        public enum MODES
        {
            BUY_COMBINED_TOTAL = 0,
            BUY_EXACT_QUAN = 1,
            BUY_PERC_OPEN_SLOTS = 2,
            LEAVE_NUMBER_SLOTS_OPEN = 3
        }
        #endregion Enums

        #region Private Members
        private static STATE state = STATE.STOPPED;
        private static Thread runThread = null;
        private static Buyer_Script script = null;
        private static String soundOnDone = "";
        private static ChatLoggerAsync chatLog = null;
        private static Audio player = null;
        private static bool isGuild = false;
        private static bool isGuildSet = false;
        private static float nbGameMinPerItemMove = 1.0f;
        private static float nbGameMinPerInvBase = 3.0f;
        private static UInt32 mainLoopDelay = 1000;
        //Master lists
        //  Maps to hold the information from the shop menu.
        private static bool npcMapsLoaded = false;
        private static Dictionary<UInt16, UInt32> npcPriceMap;
        private static Dictionary<UInt16, UInt16> npcIndexMap;
        private static bool guildMapsLoaded = false;
        private static Dictionary<UInt16, UInt32> guildPriceMap;
        private static Dictionary<UInt16, Byte> guildStockMap;
        private static Dictionary<UInt16, UInt16> guildIndexMap;
        //  The Script Lists hold what we NEED to buy.
        private static List<Item> scriptItemList;
        private static List<UInt16> scriptQuanList;
        private static List<UInt32> scriptPriceList;
        //  The Total Purchased Lists hold what we HAVE bought.
        private static List<UInt16> totalPurchasedList = new List<UInt16>();
        private static List<ushort> totalPurchasedQuanList = new List<ushort>();
        private static Byte totalPercentageSlots = 0;
        private static Byte totalLeaveOpenSlots = 0;
        private static ushort totalItemsPurchased = 0;
        private static ushort totalSlotsPurchased = 0;
        private static UInt32 gilAtStart = 0;
        private static UInt32 gilAtEnd = 0;
        #endregion Private Members

        #region Public Properties
        public static STATE State
        {
            get
            {
                return state;
            }
        }
        #endregion Public Properties

        #region Inits
        public static void Init_Iocaine()
        {
            player = new Audio();
        }
        #endregion Inits

        #region Public Methods
        #region State
        public static void Start(Buyer_Script iScript, String iAudioOnDone)
        {
            try
            {
                if ((state != STATE.STOPPED) || (iScript == null))
                {
                    return;
                }
                if (iScript.ItemList.Count == 0)
                {
                    String errMsg = "There were no items set in the list to buy. Stopping now.";
                    LoggingFunctions.Error(errMsg);
                    MessageBox.Show(errMsg);
                    return;
                }
                Statics.FuncPtrs.SetBuyerButtonPtr("&Pause", Statics.Buttons.Yellow);
                Statics.FuncPtrs.SetStatusBoxPtr("Starting the Buyer.", Statics.Fields.Green);
                state = STATE.RUNNING;
                script = new Buyer_Script(iScript);
                scriptItemList = script.ItemList;
                scriptQuanList = script.ItemQuanList;
                scriptPriceList = script.PricePerItemList;
                initStaticValues();
                setQuantities();
                soundOnDone = iAudioOnDone;
                isGuildSet = false;
                Inventory.Containers.RebuildListsMobileOnly();
                //Do any pre-run organization.

                //Start the script.
                runThread = new Thread(new ThreadStart(runThreadFunction));
                runThread.Name = "Buyer";
                runThread.IsBackground = true;
                runThread.Start();
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Buyer::Start: " + e.ToString());
            }
        }
        public static void Start(Buyer_Script iScript)
        {
            Start(iScript, "");
            Statics.FuncPtrs.SetBuyerButtonPtr("&Pause", Statics.Buttons.Yellow);
        }
        public static void Stop()
        {
            state = STATE.STOPPED;
            Statics.FuncPtrs.SetBuyerButtonPtr("S&tart", Statics.Buttons.Green);
        }
        public static void Pause()
        {
            state = STATE.PAUSED_USER;
            Statics.FuncPtrs.SetBuyerButtonPtr("&Resume", Statics.Buttons.Green);
        }
        public static void Resume()
        {
            state = STATE.RUNNING;
            Statics.FuncPtrs.SetBuyerButtonPtr("&Pause", Statics.Buttons.Yellow);
        }
        #endregion State
        #endregion Public Methods

        #region Private Methods
        #region Main Run Thread
        private static void runThreadFunction()
        {
            try
            {
                totalItemsPurchased = 0;
                totalSlotsPurchased = 0;
                gilAtStart = MemReads.Self.Inventory.get_gil();
                if (chatLog == null)
                {
                    chatLog = ChatLogManager.Access.GetAsynchronousLogger("Buyer");
                }

                //Before we even enter the loop, check if it's a delayed start or not.
                //If it's a delayed start AND we have sufficient time (say 5 game minutes for windows
                //and 1 game minute per item movement) AND we need to move items around,
                //we'll go ahead and do that while we're waiting.
                Byte startDay = 0;
                Byte startHour = 0;
                UInt16 minToStart = 0;
                if (isGuildPurchase() && !Schedules.Guilds.IsOpen(script.NpcName, ref startDay, ref startHour, ref minToStart))
                {
                    waitForGuildToOpen(startDay, startHour, minToStart);
                }
                else
                {
                    Interaction.TargetNPC(script.NpcName, checkState);
                    //IocaineFunctions.delay(250);
                }
                Inventory.Containers.RebuildListsMobileOnly();

                while (checkState() && checkStatus())
                {
                    //Like the Seller our main loop will either buy items or move inventory around.
                    if (Inventory.Containers.Bag.Full)
                    {
                        //Our bag is full.  Try to move items if we can.
                        if (!moveItems())
                        {
                            Stop();
                            continue;
                        }
                    }

                    //Now we should have space. Make sure we have the buy menu open.
                    String topLeft = MemReads.Windows.BannerText.get_top_left_text();
                    if ((isGuildPurchase() && (topLeft != "Guild Shop")) || (!isGuildPurchase() && (topLeft != "Shop")))
                    {
                        //Our top left text is not as expected.  Need to re-enter the NPC buy menu.
                        Interaction.GetToBuySellDialog(true, isGuild, script.NpcName);
                    }
                    //After we've opened the Buy dialog, fill the price list.
                    if (isGuild)
                    {
                        MemReads.Windows.Shops.Guild.get_buy_maps(ref guildPriceMap, ref guildStockMap, ref guildIndexMap);
                        guildMapsLoaded = true;
                    }
                    else
                    {
                        MemReads.Windows.Shops.NPC.get_buy_id_to_price_map(ref npcPriceMap, ref npcIndexMap);
                        npcMapsLoaded = true;
                    }

                    //Now we have the list of items and their indices.
                    //Go through each item in the list and check if we can buy it or not.
                    bool purchaseItem = false;
                    UInt32 purchaseItemIdx = 0xffffffff;
                    UInt16 purchaseItemQuan = 0;
                    UInt16 itemId = 0;
                    bool brkForAndCont = false;
                    bool brkForAndStop = false;
                    for (int ii = 0; ii < scriptItemList.Count; ii++)
                    {
                        //Check each item.
                        //1. Do we need to buy more of this item.
                        //2. Is there stock left of this item.
                        //3. Is the price of this item acceptable.
                        itemId = scriptItemList[ii].ItemID;
                        purchaseItem = checkStatusQuantity(itemId, ref purchaseItemQuan)
                                    && checkStatusStock(itemId)
                                    && checkStatusPrice(itemId);
                        if (purchaseItem)
                        {
                            if (isGuild)
                            {
                                purchaseItemIdx = guildIndexMap[itemId];
                            }
                            else
                            {
                                purchaseItemIdx = npcIndexMap[itemId];
                            }
                            //Now try to buy the item here.
                            Interaction.BUY_RETURN_CODE buyRetCode = Interaction.BUY_RETURN_CODE.NORMAL;
                            UInt16 nbPurchased = Interaction.BuyItemFromNPC(isGuild, itemId, purchaseItemQuan, (UInt16)purchaseItemIdx, Statics.Settings.Helpers.GuildSetIndex_Buyer, chatLog, checkState, ref buyRetCode);
                            //Update the lists.
                            bool foundItem = false;
                            for (int kk = 0; kk < totalPurchasedList.Count; kk++)
                            {
                                if (totalPurchasedList[kk] == itemId)
                                {
                                    foundItem = true;
                                    totalPurchasedQuanList[kk] += nbPurchased;
                                    break;
                                }
                            }
                            if (!foundItem)
                            {
                                totalPurchasedList.Add(itemId);
                                totalPurchasedQuanList.Add(nbPurchased);
                            }
                            totalItemsPurchased += nbPurchased;
                            Byte stackSize = Things.GetStackSizeFromId(itemId);
                            totalSlotsPurchased += (UInt16)(nbPurchased / stackSize);
                            if ((nbPurchased % stackSize) != 0)
                            {
                                totalSlotsPurchased++;
                            }
                            brkForAndCont = false;
                            brkForAndStop = false;
                            switch (buyRetCode)
                            {
                                case Interaction.BUY_RETURN_CODE.NORMAL:
                                    break;
                                case Interaction.BUY_RETURN_CODE.STOPPED:
                                    brkForAndStop = true;
                                    break;
                                case Interaction.BUY_RETURN_CODE.WINDOW_ISSUE:
                                    brkForAndCont = true;
                                    break;
                                case Interaction.BUY_RETURN_CODE.CLOSED:
                                    brkForAndCont = true;
                                    Schedules.Guilds.IsOpen(script.NpcName, ref startDay, ref startHour, ref minToStart);
                                    waitForGuildToOpen(startDay, startHour, minToStart);
                                    break;
                                case Interaction.BUY_RETURN_CODE.NO_STOCK:
                                    guildStockMap[itemId] = 0;
                                    continue;
                                case Interaction.BUY_RETURN_CODE.INV_FULL:
                                    brkForAndCont = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (brkForAndCont || brkForAndStop)
                        {
                            break;
                        }
                    }
                    if (brkForAndCont)
                    {
                        IocaineFunctions.delay(mainLoopDelay);
                        continue;
                    }
                    else if (brkForAndStop)
                    {
                        Stop();
                        continue;
                    }
                    IocaineFunctions.delay(mainLoopDelay);
                    Inventory.Containers.RebuildListsMobileOnly();
                }
                MenuNavigation.CloseCheck();
                if (soundOnDone != "")
                {
                    player.PlaySound(soundOnDone);
                }
                Stop();
                gilAtEnd = MemReads.Self.Inventory.get_gil();
                String statusString = "Buying complete. Purchased " + totalItemsPurchased + " item";
                if (totalItemsPurchased != 1)
                {
                    statusString += "s";
                }
                statusString += "for " + String.Format("{0:0,0}", (int)(gilAtStart - gilAtEnd)) + " gil.";
                Statics.FuncPtrs.SetStatusBoxPtr(statusString, Statics.Fields.Blue);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::runThreadFunction: " + e.ToString());
            }
        }
        #endregion Main Run Thread
        #region State
        private static bool checkState()
        {
            try
            {
                switch (state)
                {
                    case STATE.RUNNING:
                        return true;
                    case STATE.STOPPED:
                        return false;
                    case STATE.PAUSED_USER:
                        while (state == STATE.PAUSED_USER)
                        {
                            IocaineFunctions.delay(500);
                        }
                        if (state == STATE.RUNNING)
                        {
                            return true;
                        }
                        if (state == STATE.STOPPED)
                        {
                            return false;
                        }
                        break;
                    case STATE.PAUSED_PROG:
                        while (state == STATE.PAUSED_USER)
                        {
                            IocaineFunctions.delay(500);
                        }
                        if (state == STATE.RUNNING)
                        {
                            return true;
                        }
                        if (state == STATE.STOPPED)
                        {
                            return false;
                        }
                        break;
                }
                return true;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::checkState: " + e.ToString());
                return false;
            }
        }
        /// <summary>
        /// Checks whether we're done with our buying or not.
        /// Checks our counts, the NPC's stock, and the item price(s).
        /// </summary>
        /// <returns>Returns false if we're done, true if we're not.</returns>
        private static bool checkStatus()
        {
            //This function checks 3 things.
            //1. If we still need to purchase more items based on the item quantities.
            //2. If we CAN purchase this item based on the item stock conditions.
            //3. If we WANT to purchase this item based on the shop's price.
            //In each mode we'll check these 3 conditions in the order above.
            try
            {
                #region Combined Total
                if (script.Mode == (Byte)MODES.BUY_COMBINED_TOTAL)
                {
                    //When we're checking if we're done, we just check the totalItemsPurchased value
                    //against the script.CombinedTotalQuan value.
                    if (totalItemsPurchased >= script.CombinedTotalQuan)
                    {
                        return false;
                    }
                    else
                    {
                        //We need to buy more. Check if any of the items are in stock.
                        if (!checkStatusStock(scriptItemList))
                        {
                            //None of the items are in stock.
                            return false;
                        }
                        else
                        {
                            //There is stock left, check if the price of the item(s) is within range.
                            if (!checkStatusPrice(scriptItemList))
                            {
                                //None of the items are priced to our specifications, we're done.
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                #endregion Combined Total
                #region Percentage
                else if (script.Mode == (Byte)MODES.BUY_PERC_OPEN_SLOTS)
                {
                    //When checking if we're done, we just check the totalItemsPurchased value
                    //against the totalPercentageSlots.
                    if (totalSlotsPurchased >= totalPercentageSlots)
                    {
                        return false;
                    }
                    else
                    {
                        //We need to buy more. Check if any of the items are in stock.
                        if (!checkStatusStock(scriptItemList))
                        {
                            //None of the items are in stock.
                            return false;
                        }
                        else
                        {
                            //There is stock left, check if the price of the item(s) is within range.
                            if (!checkStatusPrice(scriptItemList))
                            {
                                //None of the items are priced to our specifications, we're done.
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                #endregion Percentage
                #region Leave Slots Open
                else if (script.Mode == (Byte)MODES.LEAVE_NUMBER_SLOTS_OPEN)
                {
                    //When checking if we're done, we just check the totalItemsPurchased value
                    //against the totalLeaveOpenSlots.
                    if (totalSlotsPurchased >= totalLeaveOpenSlots)
                    {
                        return false;
                    }
                    else
                    {
                        //We need to buy more. Check if any of the items are in stock.
                        if (!checkStatusStock(scriptItemList))
                        {
                            //None of the items are in stock.
                            return false;
                        }
                        else
                        {
                            //There is stock left, check if the price of the item(s) is within range.
                            if (!checkStatusPrice(scriptItemList))
                            {
                                //None of the items are priced to our specifications, we're done.
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                #endregion Leave Slots Open
                #region Exact Quantity
                else if (script.Mode == (Byte)MODES.BUY_EXACT_QUAN)
                {
                    //When checking if we're done we have to go through each item and compare with the
                    //quantities in totalPurchasedList and totalPurchasedQuanList lists.
                    for (int ii = 0; ii < scriptItemList.Count; ii++)
                    {
                        for (int kk = 0; kk < totalPurchasedList.Count; kk++)
                        {
                            if (scriptItemList[ii].ItemID == totalPurchasedList[kk])
                            {
                                //This is our item, check the quantities.
                                if (totalPurchasedQuanList[kk] < scriptQuanList[ii])
                                {
                                    List<Item> remainingItems = null;
                                    List<UInt16> remainingQuan = null;
                                    getItemsRemainingList(ref remainingItems, ref remainingQuan);
                                    //We need to buy more. Check if any of the items are in stock.
                                    if (!checkStatusStock(remainingItems))
                                    {
                                        //None of the items are in stock.
                                        return false;
                                    }
                                    else
                                    {
                                        //There is stock left, check if the price of the item(s) is within range.
                                        if (!checkStatusPrice(remainingItems))
                                        {
                                            //None of the items are priced to our specifications, we're done.
                                            return false;
                                        }
                                        else
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //If we got here it means that we went through all of the script items and
                    //we have purchased the required amount of each, so we're done.
                    return false;
                }
                #endregion Exact Quantity
                #region Default
                else
                {
                    LoggingFunctions.Error("In Buyer::checkStatus: Unknown mode '" + script.Mode + "' set.");
                    return false;
                }
                #endregion Default
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::checkStatus: " + e.ToString());
                return false;
            }
        }
        /// <summary>
        /// This function checks the current NPC buy menu for stock remaining of the given item.
        /// If we're at a non-guild npc, then we only need to check that this
        /// item is available, and if so, there is no stock limit, so just return true.
        /// </summary>
        /// <param name="iItemIds">A list of the items in question.</param>
        /// <returns>True if the NPC has this item in stock, false otherwise.</returns>
        private static bool checkStatusStock(List<Item> iItemIds)
        {
            try
            {
                for (int ii = 0; ii < iItemIds.Count; ii++)
                {
                    if (checkStatusStock(iItemIds[ii].ItemID))
                    {
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::checkStatusStock: " + e.ToString());
            }
            return false;
        }
        /// <summary>
        /// This function checks the current NPC buy menu for stock remaining of the given item.
        /// If we're at a non-guild npc, then we only need to check that this
        /// item is available, and if so, there is no stock limit, so just return true.
        /// </summary>
        /// <param name="iItemId">The item in question.</param>
        /// <returns>True if the NPC has this item in stock, false otherwise.</returns>
        private static bool checkStatusStock(UInt16 iItemId)
        {
            //This function checks the current NPC buy menu for stock remaining of the given item.
            //If we're at a non-guild npc, then we only need to check that this
            //item is available, and if so, there is no stock limit, so just return true.
            try
            {
                if (!isGuild)
                {
                    if (!npcMapsLoaded)
                    {
                        //We haven't yet loaded the maps which will be the case the first time through the loop.
                        //We will load them for the first time within the main loop.
                        //So in this case, just return true.
                        return true;
                    }
                    if (npcPriceMap.ContainsKey(iItemId))
                    {
                        //The NPC has this item to sell. Return true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (!guildMapsLoaded)
                    {
                        //We haven't yet loaded the maps which will be the case the first time through the loop.
                        //We will load them for the first time within the main loop.
                        //So in this case, just return true.
                        return true;
                    }
                    MemReads.Windows.Shops.Guild.get_buy_maps(ref guildPriceMap, ref guildStockMap, ref guildIndexMap);
                    if (!guildStockMap.ContainsKey(iItemId))
                    {
                        return false;
                    }
                    else
                    {
                        if (guildStockMap[iItemId] == 0)
                        {
                            //0 of this item left.
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::checkStatusStock: " + e.ToString());
                return false;
            }
        }
        /// <summary>
        /// This function checks the current NPC buy menu for items that are priced
        /// at or below the maximum price we are willing to pay.
        /// </summary>
        /// <param name="iItemId">The items in question.</param>
        /// <returns>True if the NPC has the item(s) at a good price, false otherwise.</returns>
        private static bool checkStatusPrice(List<Item> iItemIds)
        {
            try
            {
                for (int ii = 0; ii < iItemIds.Count; ii++)
                {
                    if (checkStatusPrice(iItemIds[ii].ItemID))
                    {
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::checkStatusPrice(List<Items>): " + e.ToString());
            }
            return false;
        }
        /// <summary>
        /// This function checks the current NPC buy menu for items that are priced
        /// at or below the maximum price we are willing to pay.
        /// </summary>
        /// <param name="iItemId">The item in question.</param>
        /// <returns>True if the NPC has this item at a good price, false otherwise.</returns>
        private static bool checkStatusPrice(UInt16 iItemId)
        {
            try
            {
                Int32 itemIdx = getItemIndex(iItemId);
                UInt32 npcPrice = 0;
                UInt32 maxPrice = 0;
                if (itemIdx < 0)
                {
                    LoggingFunctions.Error("In checkStatusPrice: Item index was < " + itemIdx);
                    return false;
                }
                maxPrice = scriptPriceList[itemIdx];
                if (!isGuild)
                {
                    if (!npcMapsLoaded)
                    {
                        //We haven't yet loaded the maps which will be the case the first time through the loop.
                        //We will load them for the first time within the main loop.
                        //So in this case, just return true.
                        return true;
                    }
                    if (npcPriceMap.ContainsKey(iItemId))
                    {
                        npcPrice = npcPriceMap[iItemId];
                        if (maxPrice >= npcPrice)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (!guildMapsLoaded)
                    {
                        //We haven't yet loaded the maps which will be the case the first time through the loop.
                        //We will load them for the first time within the main loop.
                        //So in this case, just return true.
                        return true;
                    }
                    MemReads.Windows.Shops.Guild.get_buy_maps(ref guildPriceMap, ref guildStockMap, ref guildIndexMap);
                    if (!guildPriceMap.ContainsKey(iItemId))
                    {
                        return false;
                    }
                    else
                    {
                        npcPrice = guildPriceMap[iItemId];
                        if (maxPrice >= npcPrice)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::checkStatusPric(ushort): " + e.ToString());
                return false;
            }
        }
        /// <summary>
        /// Checks if we still need to buy more of this item.
        /// </summary>
        /// <param name="iItemId">Item ID to check for.</param>
        /// <returns>Returns true if we need to buy more, otherwise false.</returns>
        private static bool checkStatusQuantity(UInt16 iItemId, ref UInt16 oItemQuan)
        {
            //This function will check if we need to buy more of the given item.
            //It is meant to be called from the main run thread in order to decide
            //if we need to buy each item in the list or go to the next one.
            //If we need to buy more, oItemQuan will give the quantity we need to/can buy.
            try
            {
                #region Combined Total
                if (script.Mode == (Byte)MODES.BUY_COMBINED_TOTAL)
                {
                    //When we're checking if we're done, we just check the totalItemsPurchased value
                    //against the script.CombinedTotalQuan value.
                    if (totalItemsPurchased >= script.CombinedTotalQuan)
                    {
                        return false;
                    }
                    else
                    {
                        oItemQuan = (UInt16)(script.CombinedTotalQuan - totalItemsPurchased);
                        return true;
                    }
                }
                #endregion Combined Total
                #region Percentage
                else if (script.Mode == (Byte)MODES.BUY_PERC_OPEN_SLOTS)
                {
                    //When checking if we're done, we just check the totalItemsPurchased value
                    //against the totalPercentageSlots.
                    if (totalSlotsPurchased >= totalPercentageSlots)
                    {
                        return false;
                    }
                    else
                    {
                        oItemQuan = (UInt16)((totalPercentageSlots - totalSlotsPurchased) * Things.GetStackSizeFromId(iItemId));
                        return true;
                    }
                }
                #endregion Percentage
                #region Leave Slots Open
                else if (script.Mode == (Byte)MODES.LEAVE_NUMBER_SLOTS_OPEN)
                {
                    //When checking if we're done, we just check the totalItemsPurchased value
                    //against the totalLeaveOpenSlots.
                    if (totalSlotsPurchased >= totalLeaveOpenSlots)
                    {
                        return false;
                    }
                    else
                    {
                        oItemQuan = (UInt16)((totalLeaveOpenSlots - totalSlotsPurchased) * Things.GetStackSizeFromId(iItemId));
                        return true;
                    }
                }
                #endregion Leave Slots Open
                #region Exact Quantity
                else if (script.Mode == (Byte)MODES.BUY_EXACT_QUAN)
                {
                    //When checking if we're done we just need to check if we have enough of this item.
                    //1st get the number required.
                    int scriptIdx = getItemIndex(iItemId);
                    ushort requiredQuan = scriptQuanList[scriptIdx];
                    ushort purchasedQuan = 0;
                    int purchasedIdx = totalPurchasedList.IndexOf(iItemId);
                    if (purchasedIdx < 0)
                    {
                        if (requiredQuan > 0)
                        {
                            //We have purchased 0 and we need more than 0, so buy more of this item.
                            oItemQuan = requiredQuan;
                            return true;
                        }
                        else
                        {
                            //The required amount is actually 0. Shouldn't happen, but you never know.
                            return false;
                        }
                    }
                    else
                    {
                        purchasedQuan = totalPurchasedQuanList[purchasedIdx];
                    }
                    if (purchasedQuan >= requiredQuan)
                    {
                        return false;
                    }
                    else
                    {
                        oItemQuan = (UInt16)(requiredQuan - purchasedQuan);
                        return true;
                    }
                }
                #endregion Exact Quantity
                #region Default
                else
                {
                    LoggingFunctions.Error("In Buyer::checkStatusQuantity: Unknown mode '" + script.Mode + "' set.");
                    return false;
                }
                #endregion Default
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::checkStatusQuantity: " + e.ToString());
                return false;
            }
        }
        #endregion Status
        #region Quantity Tracking
        private static void setQuantities()
        {
            try
            {
                if (script.Mode == (Byte)MODES.BUY_COMBINED_TOTAL)
                {
                    //If we're using a combined total, we don't need to do anything here.
                    //When we're checking if we're done, we just check the totalItemsPurchased value
                    //against the script.CombinedTotalQuan value.
                }
                else if (script.Mode == (Byte)MODES.BUY_PERC_OPEN_SLOTS)
                {
                    //If we're using the percentage of open slots, we need to get the total allowable
                    //slots available (dependant on includeSatchel and includeSack) and use the
                    //script.PercentageValue to determine the quantity we want to buy.
                    //Then we set totalPercentageItems to this value.
                    //When checking if we're done, we just check the totalItemsPurchased value
                    //against the totalPercentageItems.
                    Containers.RebuildListsMobileOnly();
                    UInt16 openSlots = (UInt16)(Containers.Bag.Capacity - Containers.Bag.Occupancy);
                    UInt16 totalSlots = (UInt16)Containers.Bag.Capacity;
                    if (script.IncludeSatchel)
                    {
                        openSlots += (UInt16)(Containers.Satchel.Capacity - Containers.Satchel.Occupancy);
                        totalSlots += Containers.Satchel.Capacity;
                    }
                    if (script.IncludeSack)
                    {
                        openSlots += (UInt16)(Containers.Sack.Capacity - Containers.Sack.Occupancy);
                        totalSlots += Containers.Sack.Capacity;
                    }
                    totalPercentageSlots = (Byte)Math.Ceiling((double)openSlots * (double)script.PercentageValue / (double)100);
                }
                else if (script.Mode == (Byte)MODES.LEAVE_NUMBER_SLOTS_OPEN)
                {
                    //In this mode we calculate the number of items similar to the Buy percentage of open slots mode
                    //and set totalLeaveSlotsOpenItems to this value.
                    //When checking if we're done, we just check the totalItemsPurchased value
                    //against the totalLeaveSlotsOpenItems.
                    Containers.RebuildListsMobileOnly();
                    UInt16 openSlots = (UInt16)(Containers.Bag.Capacity - Containers.Bag.Occupancy);
                    UInt16 totalSlots = (UInt16)Containers.Bag.Capacity;
                    if (script.IncludeSatchel)
                    {
                        openSlots += (UInt16)(Containers.Satchel.Capacity - Containers.Satchel.Occupancy);
                        totalSlots += Containers.Satchel.Capacity;
                    }
                    if (script.IncludeSack)
                    {
                        openSlots += (UInt16)(Containers.Sack.Capacity - Containers.Sack.Occupancy);
                        totalSlots += Containers.Sack.Capacity;
                    }
                    if (openSlots < script.LeaveOpenSlotsQuan)
                    {
                        totalLeaveOpenSlots = 0;
                    }
                    else
                    {
                        totalLeaveOpenSlots = (Byte)(openSlots - script.LeaveOpenSlotsQuan);
                    }
                }
                else if (script.Mode == (Byte)MODES.BUY_EXACT_QUAN)
                {
                    //In this mode we don't need to do anything special here, the values should all be saved
                    //in the script for each item.
                    //When checking if we're done we have to go through each item and compare with the
                    //quantities in totalPurchasedList and totalPurchasedQuanList lists.
                }
                else
                {
                    LoggingFunctions.Error("In Buyer::setQuantities: Unknown mode '" + script.Mode + "' set.");
                    return;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::setQuantities: " + e.ToString());
            }
        }
        private static void initStaticValues()
        {
            try
            {
                if (npcPriceMap != null)
                {
                    npcPriceMap.Clear();
                }
                if (npcIndexMap != null)
                {
                    npcIndexMap.Clear();
                }
                npcMapsLoaded = false;
                if (guildPriceMap != null)
                {
                    guildPriceMap.Clear();
                }
                if (guildStockMap != null)
                {
                    guildStockMap.Clear();
                }
                if (guildIndexMap != null)
                {
                    guildIndexMap.Clear();
                }
                guildMapsLoaded = false;

                totalPurchasedList.Clear();
                totalPurchasedQuanList.Clear();
                for (int ii = 0; ii < scriptItemList.Count; ii++)
                {
                    totalPurchasedList.Add(scriptItemList[ii].ItemID);
                    totalPurchasedQuanList.Add(0);
                }
                totalPercentageSlots = 0;
                totalLeaveOpenSlots = 0;
                totalItemsPurchased = 0;
                totalSlotsPurchased = 0;
                gilAtEnd = 0;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::initStaticValues: " + e.ToString());
            }
        }
        #endregion Quantity Tracking
        #region General/Utility
        private static Byte getNbSlotsWeNeedToMove()
        {
            //This funciton simply determines how many items we need to move around.
            //We don't want to simply move everything into our satchel/sack if we
            //don't have to.  So it will depend on:
            //1. What mode we're in.
            //2. From the mode, how many slots will we ideally need to move (if we buy everything we want).
            //3. How many items that we are buying that are actually in our bag right now.
            //This entire process should be based on slots, not actual item quantity.

            //First-First, do a quick check if we're allowed to move items.
            Byte nbAdditionalOpenSlotsNeeded = 0;   //Number of additional slots we need to free up in our bag.
            try
            {
                if (!script.IncludeSatchel && !script.IncludeSack)
                {
                    return 0;
                }

                //First, determine if we actually have any of these items in our bag.
                bool itemsInBag = false;
                List<Item> bagItems = new List<Item>();
                List<UInt16> bagQuan = new List<ushort>();
                for (int ii = 0; ii < scriptItemList.Count; ii++)
                {
                    if (Inventory.Containers.Bag.Contains(scriptItemList[ii]))
                    {
                        bagItems.Add(scriptItemList[ii]);
                        bagQuan.Add(Inventory.Containers.Bag.GetItemQuan(scriptItemList[ii]));
                        itemsInBag = true;
                    }
                }
                if (!itemsInBag)
                {
                    return 0;
                }

                //Now we know that our bag contains items that we potentially want to move.
                //Let's do a quick check if we can even move them or not based on capacities.
                if (script.IncludeSatchel && !script.IncludeSack)
                {
                    if (Inventory.Containers.Satchel.Capacity == Inventory.Containers.Satchel.Occupancy)
                    {
                        //Our satchel is full and we can't move into our sack, so we're done.
                        return 0;
                    }
                }
                else if (!script.IncludeSatchel && script.IncludeSack)
                {
                    if (Inventory.Containers.Sack.Capacity == Inventory.Containers.Sack.Occupancy)
                    {
                        //Our sack is full and we can't move into our satchel, so we're done.
                        return 0;
                    }
                }
                else
                {
                    //We can move into either one, so check if they're both at capacity.
                    if ((Inventory.Containers.Sack.Capacity == Inventory.Containers.Sack.Occupancy) &&
                        (Inventory.Containers.Satchel.Capacity == Inventory.Containers.Satchel.Occupancy))
                    {
                        //They're both at capacity, so we're done.
                        return 0;
                    }
                }

                //Ok, now we know that we have items to move AND we have room to move them.
                //So we need to check if we really NEED to move them or not.
                //This is done by checking how many open slots we have in our bag
                //versus how many total slots all of the items we have left to buy will take up.
                Byte nbOpenSlotsNow = (Byte)(Inventory.Containers.Bag.Capacity - Inventory.Containers.Bag.Occupancy);
                Byte nbSlotsItemsWillFill = 0;          //Total slots of taken by the items we wish to buy.
                Byte nbSlotsPurchased = getNbSlotsPurchased();  //Number of slots worth of items that we've already purchased.
                #region Combined Total
                if (script.Mode == (Byte)MODES.BUY_COMBINED_TOTAL)
                {
                    //If we're set to 'Buy Combined Total', it's assumed that all items stack the same.
                    //So for our stack size, just take the stack size of the first item in the list.
                    Byte stackSize = Things.GetStackSizeFromId(scriptItemList[0].ItemID);
                    nbSlotsItemsWillFill = (Byte)((script.CombinedTotalQuan % stackSize == 0) ? script.CombinedTotalQuan / stackSize : (script.CombinedTotalQuan / stackSize) + 1);
                    nbSlotsItemsWillFill -= nbSlotsPurchased;
                    if (nbOpenSlotsNow >= nbSlotsItemsWillFill)
                    {
                        return 0;
                    }
                    else
                    {
                        nbAdditionalOpenSlotsNeeded = (Byte)(nbSlotsItemsWillFill - nbOpenSlotsNow);
                    }
                }
                #endregion Combined Total
                #region Percentage
                else if (script.Mode == (Byte)MODES.BUY_PERC_OPEN_SLOTS)
                {
                    //If we're set to 'Buy Percentage of Open Slots', it's assumed that all items stack the same.
                    //So for our stack size, just take the stack size of the first item in the list.
                    Byte stackSize = Things.GetStackSizeFromId(scriptItemList[0].ItemID);
                    nbSlotsItemsWillFill = (Byte)(totalPercentageSlots - nbSlotsPurchased);
                    if (nbOpenSlotsNow >= nbSlotsItemsWillFill)
                    {
                        return 0;
                    }
                    else
                    {
                        nbAdditionalOpenSlotsNeeded = (Byte)(nbSlotsItemsWillFill - nbOpenSlotsNow);
                    }
                }
                #endregion Percentage
                #region Leave Slots Open
                else if (script.Mode == (Byte)MODES.LEAVE_NUMBER_SLOTS_OPEN)
                {
                    //If we're set to 'Leave Number of Slots Open', it's assumed that all items stack the same.
                    //So for our stack size, just take the stack size of the first item in the list.
                    Byte stackSize = Things.GetStackSizeFromId(scriptItemList[0].ItemID);
                    //The # of slots we will buy is just the totalLeaveOpenSlots value.
                    nbSlotsItemsWillFill = (Byte)(totalLeaveOpenSlots - nbSlotsPurchased);
                    if (nbOpenSlotsNow >= nbSlotsItemsWillFill)
                    {
                        return 0;
                    }
                    else
                    {
                        nbAdditionalOpenSlotsNeeded = (Byte)(nbSlotsItemsWillFill - nbOpenSlotsNow);
                    }
                }
                #endregion Leave Slots Open
                #region Exact Quantity
                else if (script.Mode == (Byte)MODES.BUY_EXACT_QUAN)
                {
                    //If we're set to 'Buy Exact Quantity of Each' then we need to actually figure out how
                    //many slots each individual item will require.
                    for (int ii = 0; ii < scriptItemList.Count; ii++)
                    {
                        Byte stackSize = Things.GetStackSizeFromId(scriptItemList[ii].ItemID);
                        if (scriptQuanList[ii] % stackSize == 0)
                        {
                            nbSlotsItemsWillFill += (Byte)(scriptQuanList[ii] / stackSize);
                        }
                        else
                        {
                            nbSlotsItemsWillFill += (Byte)((scriptQuanList[ii] / stackSize) + 1);
                        }
                    }
                    nbSlotsItemsWillFill -= nbSlotsPurchased;
                    if (nbOpenSlotsNow >= nbSlotsItemsWillFill)
                    {
                        return 0;
                    }
                    else
                    {
                        nbAdditionalOpenSlotsNeeded = (Byte)(nbSlotsItemsWillFill - nbOpenSlotsNow);
                    }
                }
                #endregion Exact Quantity
                #region Default
                else
                {
                    LoggingFunctions.Error("In Buyer::moveItemsAround: Unknown mode '" + script.Mode + "' set.");
                    return 0;
                }
                #endregion Default
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::getNbSlotsWeNeedToMove: " + e.ToString());
            }
            return nbAdditionalOpenSlotsNeeded;
        }
        private static bool moveItems()
        {
            return moveItems(getNbSlotsWeNeedToMove());
        }
        private static bool moveItems(Byte iNbSlotsToMove)
        {
            if (iNbSlotsToMove == 0)
            {
                return false;
            }
            //If we got to here without returning it means that we need to move items.
            //Now just move to the Satchel, then the Sack if required based on their occupancy & capacity.
            try
            {
                Byte nbOpenSatchelSlots = (Byte)(Inventory.Containers.Satchel.Capacity - Inventory.Containers.Satchel.Occupancy);
                Byte nbOpenSackSlots = (Byte)(Inventory.Containers.Sack.Capacity - Inventory.Containers.Sack.Occupancy);

                Byte nbToMove = 0;
                if ((nbOpenSatchelSlots > 0) && script.IncludeSatchel)
                {
                    //Move items into satchel.
                    if (iNbSlotsToMove > nbOpenSatchelSlots)
                    {
                        nbToMove = nbOpenSatchelSlots;
                    }
                    else
                    {
                        nbToMove = iNbSlotsToMove;
                    }
                    iNbSlotsToMove -= (Byte)(Inventory.Movement.MoveItem(scriptItemList, scriptQuanList, Inventory.Containers.Bag, Inventory.Containers.Satchel, checkState, false, nbToMove)[0]);
                }
                if ((iNbSlotsToMove > 0) && (nbOpenSackSlots > 0) && script.IncludeSack)
                {
                    //Move items into sack.
                    if (iNbSlotsToMove > nbOpenSackSlots)
                    {
                        nbToMove = nbOpenSackSlots;
                    }
                    else
                    {
                        nbToMove = iNbSlotsToMove;
                    }
                    Inventory.Movement.MoveItem(scriptItemList, scriptQuanList, Inventory.Containers.Bag, Inventory.Containers.Sack, checkState, false, nbToMove);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::moveItems: " + e.ToString());
            }
            return true;
        }
        private static Int32 getItemIndex(UInt16 iItemId)
        {
            try
            {
                for (int ii = 0; ii < scriptItemList.Count; ii++)
                {
                    if (scriptItemList[ii].ItemID == iItemId)
                    {
                        return ii;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::getItemIndex: " + e.ToString());
            }
            return -1;
        }
        private static void getItemsRemainingList(ref List<Item> oRemainingItems, ref List<UInt16> oRemainingQuan)
        {
            try
            {
                #region Clear Lists
                if (oRemainingItems == null)
                {
                    oRemainingItems = new List<Item>();
                }
                else
                {
                    oRemainingItems.Clear();
                }
                if (oRemainingQuan == null)
                {
                    oRemainingQuan = new List<ushort>();
                }
                else
                {
                    oRemainingQuan.Clear();
                }
                #endregion Clear Lists
                for (int ii = 0; ii < scriptItemList.Count; ii++)
                {
                    bool inPurchasedList = false;
                    for (int kk = 0; kk < totalPurchasedList.Count; kk++)
                    {
                        //Match the item from the script with the item that we've purchased (if it's there).
                        if (scriptItemList[ii].ItemID == totalPurchasedList[kk])
                        {
                            inPurchasedList = true;
                            //This is our item. Check the quantity that we've bought.
                            if (scriptQuanList[ii] > totalPurchasedQuanList[kk])
                            {
                                //We haven't bought enough yet. Add it to our remaining list.
                                oRemainingItems.Add(scriptItemList[ii]);
                                oRemainingQuan.Add((UInt16)(scriptQuanList[ii] - totalPurchasedQuanList[kk]));
                            }
                        }
                    }
                    if (!inPurchasedList)
                    {
                        oRemainingItems.Add(scriptItemList[ii]);
                        oRemainingQuan.Add(scriptQuanList[ii]);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Buyer::getItemsRemainingList: " + e.ToString());
            }
        }
        private static Byte getNbSlotsPurchased()
        {
            Byte nbSlots = 0;
            Byte stackSize = 0;
            try
            {
                for (int ii = 0; ii < totalPurchasedList.Count; ii++)
                {
                    stackSize = Things.GetStackSizeFromId(totalPurchasedList[ii]);
                    nbSlots += (Byte)(totalPurchasedQuanList[ii] / stackSize);
                    if ((totalPurchasedQuanList[ii] % stackSize) != 0)
                    {
                        nbSlots++;
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::getNbSlotsPurchased: " + e.ToString());
            }
            return nbSlots;
        }
        #endregion General/Utility
        #region Guild Related
        private static bool isGuildPurchase()
        {
            try
            {
                if (isGuildSet)
                {
                    return isGuild;
                }
                if (NPCs.Type(script.NpcName) == NPCs.NPC_TYPE.GUILD_MERCH)
                {
                    isGuild = true;
                }
                else
                {
                    isGuild = false;
                }
                isGuildSet = true;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::isGuildPurchase: " + e.ToString());
            }
            return isGuild;
        }
        private static void waitForGuildToOpen(Byte iStartDay, Byte iStartHour, UInt16 iMinToStart)
        {
            try
            {
                //Get the number of items we need to move and the estimated time it will take to move them.
                Byte nbItemsToMove = getNbSlotsWeNeedToMove();
                UInt16 timeNeededToMoveItems = (UInt16)(nbGameMinPerInvBase + (float)((float)nbItemsToMove * nbGameMinPerItemMove));
                //If it's going to take too long, move less.
                if (timeNeededToMoveItems > iMinToStart)
                {
                    Statics.FuncPtrs.SetStatusBoxPtr("Moving items around while waiting for the guild to open.", Statics.Fields.Green);
                    nbItemsToMove = (Byte)((float)((float)timeNeededToMoveItems - nbGameMinPerInvBase) / nbGameMinPerItemMove);
                    if (nbItemsToMove > 1)
                    {
                        if (!moveItems(nbItemsToMove))
                        {
                            LoggingFunctions.Error("In Buyer::runThreadFunction: moveItems returned false while waiting for the guild to open.");
                        }
                    }
                }

                Interaction.TargetNPC(script.NpcName, checkState);

                VanaTime vTimeNow = VanaTime.Now;
                Byte currHour = (Byte)vTimeNow.Hour;
                Byte currDay = vTimeNow.Day;
                bool firstLoop = true;
                while (((currDay != iStartDay) || (currHour != iStartHour)) && checkState())
                {
                    if (firstLoop)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Waiting for the guild to open at " + iStartHour + ".", Statics.Fields.Green);
                        firstLoop = false;
                    }
                    IocaineFunctions.delay(500);
                    vTimeNow = VanaTime.Now;
                    currHour = (Byte)vTimeNow.Hour;
                    currDay = vTimeNow.Day;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Buyer::waitForGuildToOpen: " + e.ToString());
            }
        }
        #endregion Guild Related
        #endregion Private Methods
        
    }
}
