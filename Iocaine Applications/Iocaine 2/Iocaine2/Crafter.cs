using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Data.Structures;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Properties;
using Iocaine2.Tools;

namespace Iocaine2.Bots
{
    public class Crafter
    {
        #region Enums
        public enum CRAFT_MODE
        {
            NUMBER_OF = 0,
            MAX = 1,
            UNTIL_SKILL = 2
        }
        public enum CRAFTER_STATE
        {
            STOPPED = 0,
            RUNNING = 1,
            PAUSED_USER = 2,
            PAUSED_PROG = 3
        }
        #endregion Enums

        #region Private Members
        private Recipe recipe;
        private CRAFT_MODE mode;
        private int numberOf;
        private int counter;
        private Single skill;
        private bool maxCountUpdated = false;
        #region Controls
        private Button StartButton;
        private Statics.FuncPtrs.TD_Void_String_Color updateCrafterStartButtonCallBack;
        #endregion Controls
        #region Helper Objects
        private ChatLoggerAsync chatLog;
        private Audio player;
        #endregion Helper Objects
        #region State Variables
        private CRAFTER_STATE state;
        #endregion State Variables
        #endregion Private Members

        #region Public Properties
        public CRAFTER_STATE State
        {
            get
            {
                return state;
            }
        }
        #endregion Public Properties

        #region Public Events/Delegates
        public delegate void CraftDoneEvent(Recipe recipe, int numberSoFar, RecipeLog.RESULT_TYPE result, List<ushort> lostItems, List<ushort> lostQuan);
        public event CraftDoneEvent CraftDone;
        #endregion Public Events/Delegates

        #region Constructors
        public Crafter(CRAFT_MODE iMode,
                       Recipe iRecipe, int iNumberOf, Single iSkill, Button iStartButton,
                       Statics.FuncPtrs.TD_Void_String_Color iUpdateCrafterStartButtonCallBack)
        {
            mode = iMode;
            recipe = iRecipe;
            numberOf = iNumberOf;
            skill = iSkill;
            StartButton = iStartButton;
            updateCrafterStartButtonCallBack = iUpdateCrafterStartButtonCallBack;
            chatLog = ChatLogManager.Access.GetAsynchronousLogger("Crafter");
            chatLog.Flags = ChatLine.CHAT_FLAGS.LEAVE_ITEM_DELINIATION;
            player = new Audio();
        }
        #endregion Constructors

        #region Member Functions
        public void doInits()
        {

        }
        public void StartCrafter()
        {
            try
            {
                state = CRAFTER_STATE.RUNNING;
                chatLog.Reset();
                String modeInfo = "";
                if (mode == CRAFT_MODE.NUMBER_OF)
                {
                    modeInfo = "Crafting " + numberOf + " items.";
                }
                else if (mode == CRAFT_MODE.MAX)
                {
                    modeInfo = "Crafting until ingredients are depleted or inventory is full.";
                }
                else
                {
                    modeInfo = "Crafting until skill level " + skill + " is reached.";
                }
                LoggingFunctions.Timestamp("Starting crafter. " + modeInfo);
                Player_MenuNavigation.OpenBag();
                Inventory.Movement.SortInventory(false);
                goToInventorySpot(1);
                LoggingFunctions.Debug("Crafter::StartCrafter: Crystal is: " + recipe.CrystalString + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                foreach (Recipe.CIngredient ingredient in recipe.mIngredients)
                {
                    LoggingFunctions.Debug("Crafter::StartCrafter: Ing is: " + ingredient.Name + " (" + ingredient.ID + ") * " + ingredient.Quantity + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                }

                maxCountUpdated = true;
                while (checkState() && checkStatus())
                {
                    // First do a check for all required items in our bag and if we have room.
                    // If neither is true, try swapping.
                    if (Containers.Bag.LiveFull || !checkIngredients())
                    {
                        swapIngredientsAndResults();
                    }
                    // We should have either a crystal or a cluster.
                    // If there are no crystals, but clusters, break one.
                    if (Containers.Bag.GetItemQuan(recipe.CrystalId) == 0)
                    {
                        if (breakCluster())
                        {
                            Inventory.Movement.SortInventory(false);
                            IocaineFunctions.delay(1000);
                        }
                    }
                    if ((Containers.Bag.GetItemQuan(recipe.CrystalId) == 0) || !findItem(recipe.CrystalString))
                    {
                        // This should be an exit scenario.
                        swapIngredientsAndResults();
                        LoggingFunctions.Error("Could not find any crystals or clusters to craft with even though there should be.");
                        StopCrafter();
                        continue;
                    }
                    maxCountUpdated = false;    // This needs to be true during the swapping ingredients call. Otherwise the checkStatus gets hung.
                    useItem();
                    setIngredients();
                    IocaineFunctions.delay(500);
                    do
                    {
                        Player_MenuNavigation.HitOK();
                        IocaineFunctions.delay(2000);
                    } while (chatlogContains("wait longer"));
                    //Wait for the status to become "crafting".
                    //Create a timeout of a few seconds in case of lag or error.
                    int timeout = 20;
                    byte myStatus = MemReads.Self.get_status();
                    while ((myStatus != (byte)FFXIEnums.STATUS.SYNTHING) && (timeout > 0))
                    {
                        IocaineFunctions.delay(500);
                        timeout--;
                        myStatus = MemReads.Self.get_status();
                    }
                    if (timeout <= 0)
                    {
                        LoggingFunctions.Error("Crafter timed out waiting for player status to be 'SYNTHING'.");
                        LoggingFunctions.Error("Last status read was " + myStatus + ".");
                        continue;
                    }
                    timeout = 90;
                    while ((myStatus == (byte)FFXIEnums.STATUS.SYNTHING) && (timeout > 0))
                    {
                        IocaineFunctions.delay(500);
                        timeout--;
                        myStatus = MemReads.Self.get_status();
                    }
                    if (timeout <= 0)
                    {
                        LoggingFunctions.Error("Crafter timed out waiting for synthesis status to change.");
                        LoggingFunctions.Error("Last status read was " + myStatus + ".");
                        LoggingFunctions.Error("Stopping crafter due to this error.");
                        StopCrafter();
                        continue;
                    }

                    if (Statics.Settings.Crafter.ExtraTime > 0)
                    {
                        IocaineFunctions.delay((uint)Statics.Settings.Crafter.ExtraTime);
                    }
                    counter++;
                    LoggingFunctions.Debug("Crafter::StartCrafter: Crafter loop count is now: " + counter + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);

                    List<ushort> lostItems = new List<ushort>();
                    List<ushort> lostQuan = new List<ushort>();
                    RecipeLog.RESULT_TYPE result = RecipeLog.RESULT_TYPE.UNK;
                    parseResult(ref lostItems, ref lostQuan, ref result);
                    CraftDone(recipe, counter, result, lostItems, lostQuan);
                }
                return;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::StartCrafter: " + e.ToString());
            }
        }
        #endregion Member Functions

        #region Crafter State Utility Functions
        private bool checkState()
        {
            try
            {
                LoggingFunctions.Debug("Crafter::checkState: Entering CB checkstate.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                switch (state)
                {
                    case CRAFTER_STATE.STOPPED:
                        updateStartButton("S&tart", Statics.Buttons.Green);
                        Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                        return false;
                    case CRAFTER_STATE.RUNNING:
                        return true;
                    case CRAFTER_STATE.PAUSED_PROG:
                        goto case CRAFTER_STATE.PAUSED_USER;
                    case CRAFTER_STATE.PAUSED_USER:
                        Statics.FuncPtrs.SetStatusBoxPtr("Pausing bot", Statics.Fields.Yellow);
                        while (state != CRAFTER_STATE.RUNNING)
                        {
                            if (state == CRAFTER_STATE.STOPPED)
                            {
                                updateStartButton("S&tart", Statics.Buttons.Green);
                                Statics.FuncPtrs.SetStatusBoxPtr("Stopping bot", Statics.Fields.Blue);
                                LoggingFunctions.Debug("Crafter::checkState: Returning false from CB checkstate.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                                return false;
                            }
                            IocaineFunctions.delay(1000);
                            if ((ChangeMonitor.MainProc == null) || (ChangeMonitor.MainProc.HasExited == true))
                            {
                                state = CRAFTER_STATE.STOPPED;
                            }
                        }
                        LoggingFunctions.Debug("Crafter::checkState: Returning true from CB checkstate.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                        return true;
                    default:
                        LoggingFunctions.Debug("Crafter::checkState: Returning false from CB checkstate.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                        return false;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::checkState: " + e.ToString());
                return false;
            }
        }
        private bool checkStatus()
        {
            try
            {
                while (!maxCountUpdated)
                {
                    IocaineFunctions.delay(10);
                }
                switch (mode)
                {
                    case CRAFT_MODE.NUMBER_OF:
                        if (MemReads.Self.Inventory.get_bag_occupancy() == MemReads.Self.Inventory.get_max_bag())
                        {
                            Containers.RebuildListsMobileOnly();
                            if (Iocaine_2_Form.CB_MoveInventory && (!Containers.Satchel.Full || !Containers.Sack.Full || !Containers.MCase.Full))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Crafting item " + (counter + 1).ToString() + " of " + numberOf, Statics.Fields.Green);
                                return true;
                            }
                            else
                            {
                                updateStartButton("S&tart", Statics.Buttons.Green);
                                Statics.FuncPtrs.SetStatusBoxPtr("Inventory full, stopping...", Statics.Fields.Blue);
                                player.PlaySound(Statics.Settings.Crafter.DonePlaySound);
                                state = CRAFTER_STATE.STOPPED;
                                return false;
                            }
                        }
                        else if (counter >= numberOf)
                        {
                            updateStartButton("S&tart", Statics.Buttons.Green);
                            Statics.FuncPtrs.SetStatusBoxPtr("Finished crafting " + counter + " items. Stopping...", Statics.Fields.Blue);
                            player.PlaySound(Statics.Settings.Crafter.DonePlaySound);
                            state = CRAFTER_STATE.STOPPED;
                            return false;
                        }
                        else
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Crafting item " + (counter + 1).ToString() + " of " + numberOf, Statics.Fields.Green);
                            return true;
                        }
                    case CRAFT_MODE.MAX:
                        if (MemReads.Self.Inventory.get_bag_occupancy() == MemReads.Self.Inventory.get_max_bag())
                        {
                            Containers.RebuildListsMobileOnly();
                            if (Iocaine_2_Form.CB_MoveInventory && (!Containers.Satchel.Full || !Containers.Sack.Full || !Containers.MCase.Full))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Crafting item " + (counter + 1).ToString() + " of " + numberOf, Statics.Fields.Green);
                                return true;
                            }
                            else
                            {
                                updateStartButton("S&tart", Statics.Buttons.Green);
                                Statics.FuncPtrs.SetStatusBoxPtr("Inventory full, stopping...", Statics.Fields.Blue);
                                player.PlaySound(Statics.Settings.Crafter.DonePlaySound);
                                state = CRAFTER_STATE.STOPPED;
                                return false;
                            }
                        }
                        else if (counter >= numberOf)
                        {
                            updateStartButton("S&tart", Statics.Buttons.Green);
                            Statics.FuncPtrs.SetStatusBoxPtr("Finished crafting " + counter + " items. Stopping...", Statics.Fields.Blue);
                            player.PlaySound(Statics.Settings.Crafter.DonePlaySound);
                            state = CRAFTER_STATE.STOPPED;
                            return false;
                        }
                        else
                        {
                            Statics.FuncPtrs.SetStatusBoxPtr("Crafting item " + (counter + 1).ToString() + " of " + numberOf, Statics.Fields.Green);
                            return true;
                        }
                    case CRAFT_MODE.UNTIL_SKILL:

                        if (MemReads.Self.Inventory.get_bag_occupancy() == MemReads.Self.Inventory.get_max_bag())
                        {
                            Containers.RebuildListsMobileOnly();
                            if (Iocaine_2_Form.CB_MoveInventory && (!Containers.Satchel.Full || !Containers.Sack.Full || !Containers.MCase.Full))
                            {
                                Statics.FuncPtrs.SetStatusBoxPtr("Crafting item " + (counter + 1).ToString() + " of " + numberOf, Statics.Fields.Green);
                                return true;
                            }
                            else
                            {
                                updateStartButton("S&tart", Statics.Buttons.Green);
                                Statics.FuncPtrs.SetStatusBoxPtr("Inventory full, stopping...", Statics.Fields.Blue);
                                player.PlaySound(Statics.Settings.Crafter.DonePlaySound);
                                state = CRAFTER_STATE.STOPPED;
                                return false;
                            }
                        }
                        else if (getCurrentSkillLevel() >= skill)
                        {
                            updateStartButton("S&tart", Statics.Buttons.Green);
                            Statics.FuncPtrs.SetStatusBoxPtr("Finished crafting to skill " + skill + ". Stopping...", Statics.Fields.Blue);
                            player.PlaySound(Statics.Settings.Crafter.DonePlaySound);
                            state = CRAFTER_STATE.STOPPED;
                            return false;
                        }
                        else
                        {

                            Statics.FuncPtrs.SetStatusBoxPtr("Crafting item " + (counter + 1).ToString() + " of " + Math.Max(numberOf, counter + 1).ToString(), Statics.Fields.Green);
                            return true;
                        }
                    default:
                        return false;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::checkStatus: " + e.ToString());
                return false;
            }
        }
        public void PauseCrafter()
        {
            state = CRAFTER_STATE.PAUSED_USER;
        }
        public void StopCrafter()
        {
            state = CRAFTER_STATE.STOPPED;
        }
        public void ResumeCrafter()
        {
            state = CRAFTER_STATE.RUNNING;
        }
        #endregion Crafter State Utility Functions

        #region Utility Functions
        #region general
        private ushort getCurrentSkillLevel()
        {
            ushort lastSkillLevel = 0;
            try
            {
                switch (recipe.PriCraft)
                {
                    case FFXIEnums.CRAFTS.AL:
                        lastSkillLevel = MemReads.Self.Skills.Crafting.get_alch_skill();
                        break;
                    case FFXIEnums.CRAFTS.BC:
                        lastSkillLevel = MemReads.Self.Skills.Crafting.get_bone_skill();
                        break;
                    case FFXIEnums.CRAFTS.CC:
                        lastSkillLevel = MemReads.Self.Skills.Crafting.get_cloth_skill();
                        break;
                    case FFXIEnums.CRAFTS.CK:
                        lastSkillLevel = MemReads.Self.Skills.Crafting.get_cook_skill();
                        break;
                    case FFXIEnums.CRAFTS.FI:
                        lastSkillLevel = MemReads.Self.Skills.Crafting.get_fish_skill();
                        break;
                    case FFXIEnums.CRAFTS.GS:
                        lastSkillLevel = MemReads.Self.Skills.Crafting.get_gold_skill();
                        break;
                    case FFXIEnums.CRAFTS.LC:
                        lastSkillLevel = MemReads.Self.Skills.Crafting.get_leather_skill();
                        break;
                    case FFXIEnums.CRAFTS.SM:
                        lastSkillLevel = MemReads.Self.Skills.Crafting.get_smith_skill();
                        break;
                    case FFXIEnums.CRAFTS.WW:
                        lastSkillLevel = MemReads.Self.Skills.Crafting.get_wood_skill();
                        break;
                    default:
                        lastSkillLevel = 255;
                        break;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::getCurrentSkillLevel: " + e.ToString());
            }
            return lastSkillLevel;
        }
        #endregion general
        #region interaction functions
        private void goToInventorySpot(byte spot)
        {
            try
            {
                if ((spot < 1) || (spot > 100))
                {
                    LoggingFunctions.Error("Tried to go to illegal inventory slot " + spot + ".");
                    return;
                }
                if (spot == 1)
                {
                    while (MemReads.Windows.Items.get_selection_index() > 1)
                    {
                        IocaineFunctions.arrowKeyDown(Keys.Left, 300);
                        IocaineFunctions.delay(300);
                    }
                }
                else
                {
                    //TBD - add logic to get to specific slot with the fewest keystrokes
                }
                IocaineFunctions.delay(500);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::goToInventorySpot: " + e.ToString());
            }
        }
        private bool findItem(string item)
        {
            try
            {
                LoggingFunctions.Debug("Crafter::findItem: Searching for item: " + item + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);

                Player_MenuNavigation.OpenBag();
                return Inventory.Movement.SelectItem(item);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::findItem: " + e.ToString());
                return false;
            }

        }
        private bool useItem()
        {
            try
            {
                IocaineFunctions.keyDown(Keys.Enter, 300);
                IocaineFunctions.delay(500);
                if (MemReads.Windows.BannerText.get_help_text() == "Dispose of item.")
                {
                    IocaineFunctions.arrowKeyDown(System.Windows.Forms.Keys.Down, 250);
                    IocaineFunctions.delay(200);
                }
                if (MemReads.Windows.BannerText.get_help_text() != "Use an item.")
                {
                    LoggingFunctions.Error("While trying to use item, did not get expected help text \"Use an item.\"");
                    return false;
                }
                else
                {
                    IocaineFunctions.keyDown(Keys.Enter, 300);
                    IocaineFunctions.delay(500);
                }
                return true;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter:useItem: " + e.ToString());
                return false;
            }
        }
        private void setIngredients()
        {
            try
            {
                List<byte> usedIndexList = new List<byte>();
                foreach (Recipe.CIngredient ingredient in recipe.mIngredients)
                {
                    byte index = MemReads.Self.Inventory.get_bag_index(ingredient.ID, ingredient.Quantity);
                    while (usedIndexList.Contains(index) && (index != 0))
                    {
                        index = MemReads.Self.Inventory.get_bag_index(ingredient.ID, ingredient.Quantity, (byte)(index + 1));
                    }

                    if (index == 0)
                    {
                        LoggingFunctions.Error("Could not find item " + ingredient.Name + " in inventory.");
                        MessageBox.Show("[ERROR] Could not find item " + ingredient.Name + " in inventory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    usedIndexList.Add(index);
                    byte ingredientindex = (byte)recipe.mIngredients.IndexOf(ingredient);
                    LoggingFunctions.Debug("Crafter::setIngredients: Ingredient " + ingredientindex + ": " + ingredient.Name + ": " + ingredient.ID + " : " + index + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                    MemReads.Windows.Crafting.set_item(ingredientindex, ingredient.ID, ingredient.Quantity, index);
                }

                LoggingFunctions.Debug("Crafter::setIngredients: Setting ingredients...", LoggingFunctions.DBG_SCOPE.CRAFTER);
                for (byte ii = 0; ii < 8; ii++)
                {
                    ushort id = 0;
                    byte quan = 0;
                    byte idx = 0;
                    MemReads.Windows.Crafting.get_item(ii, ref id, ref quan, ref idx);
                    LoggingFunctions.Debug("Crafter::setIngredients: Actuall read ID: " + id + ", quan: " + quan + ", index: " + idx + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::setIngredients: " + e.ToString());
            }
        }
        private bool checkIngredients()
        {
            try
            {
                foreach (Recipe.CIngredient ingredient in recipe.mIngredients)
                {
                    if (Containers.Bag.GetItemQuan(ingredient.ID) < ingredient.Quantity)
                    {
                        return false;
                    }
                }
                if ((Containers.Bag.GetItemQuan(recipe.CrystalId) == 0) && (Containers.Bag.GetItemQuan(Things.CrystalAndClusterExchange(recipe.CrystalId)) == 0))
                {
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::checkIngredients: " + e.ToString());
                return false;
            }
        }
        private void swapIngredientsAndResults()
        {
            try
            {
                if (Iocaine_2_Form.CB_MoveInventory == false)
                {
                    return;
                }

                byte nbAvailBag = 0;
                byte nbAvailSatchel = 0;
                byte nbAvailSack = 0;
                byte nbAvailCase = 0;
                #region Move to S/S/C
                // First move the result items to s/s/c to make room.
                List<Recipe.CResult> resultItems = recipe.mResults;
                foreach (Recipe.CResult result in resultItems)
                {
                    Item localItem = new Item(result.Name, result.ID, (Item.ITEM_TYPE)Things.GetTypeFromId(result.ID));
                    Containers.RebuildListsMobileOnly();
                    ushort localNbItem = Containers.Bag.GetItemQuan(localItem.ItemID);
                    if (localNbItem == 0)
                    {
                        LoggingFunctions.Debug("Crafter::swapIngredientsAndResults: No results[" + localItem.Name + "] to move from bag.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                        continue;
                    }

                    nbAvailBag = (byte)(Containers.Bag.Capacity - Containers.Bag.Occupancy);
                    nbAvailSatchel = (byte)(Containers.Satchel.Capacity - Containers.Satchel.Occupancy);
                    nbAvailSack = (byte)(Containers.Sack.Capacity - Containers.Sack.Occupancy);
                    nbAvailCase = (byte)(Containers.MCase.Capacity - Containers.MCase.Occupancy);
                    if ((nbAvailSack == 0) && (nbAvailSatchel == 0) && (nbAvailCase == 0))
                    {
                        Inventory.Containers.RebuildListsMobileOnly();
                        break;
                    }

                    Inventory.Movement.SortInventory(true);
                    byte stackSize = Things.GetStackSizeFromId(localItem.ItemID);
                    ushort nbItemSlots = (ushort)(localNbItem / stackSize);
                    if (localNbItem % stackSize != 0)
                    {
                        nbItemSlots++;
                    }
                    ushort nbMoved = 0;
                    // CASE //
                    if (nbItemSlots <= nbAvailCase)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Moving items into case.", Statics.Fields.Green);
                        nbMoved += Inventory.Movement.MoveItem(localItem.ItemID, (ushort)((nbItemSlots * stackSize) - nbMoved), Containers.Bag, Containers.MCase, checkState);
                    }
                    else if (nbAvailCase > 0)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Moving items into case.", Statics.Fields.Green);
                        nbMoved += Inventory.Movement.MoveItem(localItem.ItemID, (ushort)((nbAvailCase * stackSize) - nbMoved), Containers.Bag, Containers.MCase, checkState);
                    }
                    if (!checkState())
                    {
                        return;
                    }

                    // SACK //
                    if (nbItemSlots <= nbAvailSack)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Moving items into sack.", Statics.Fields.Green);
                        nbMoved += Inventory.Movement.MoveItem(localItem.ItemID, (ushort)((nbItemSlots * stackSize) - nbMoved), Containers.Bag, Containers.Sack, checkState);
                    }
                    else if (nbAvailSack > 0)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Moving items into sack.", Statics.Fields.Green);
                        nbMoved += Inventory.Movement.MoveItem(localItem.ItemID, (ushort)((nbAvailSack * stackSize) - nbMoved), Containers.Bag, Containers.Sack, checkState);
                    }
                    if (!checkState())
                    {
                        return;
                    }

                    // SATCHEL //
                    if (nbItemSlots <= nbAvailSatchel)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Moving items into satchel.", Statics.Fields.Green);
                        nbMoved += Inventory.Movement.MoveItem(localItem.ItemID, (ushort)(nbItemSlots * stackSize), Containers.Bag, Containers.Satchel, checkState);
                    }
                    else if (nbAvailSatchel > 0)
                    {
                        Statics.FuncPtrs.SetStatusBoxPtr("Moving items into satchel.", Statics.Fields.Green);
                        nbMoved += Inventory.Movement.MoveItem(localItem.ItemID, (ushort)(nbAvailSatchel * stackSize), Containers.Bag, Containers.Satchel, checkState);
                    }
                    Inventory.Containers.RebuildListsMobileOnly();
                }
                #endregion Move to S/S/C
                #region Move to Bag
                // Now comes the really tricky part.
                // We need to figure out what ingredients are needed
                // and how many of each to move into the bag.
                // Each will be based on number of open slots and how many of each item is required.
                Inventory.Containers.RebuildListsMobileOnly();
                ushort maxCraftable = recipe.calculateMaxSynths(Containers.Bag);
                if (maxCraftable > 0)
                {
                    return;
                }

                // We can't craft any, so figure out what items we need.
                // Figure out how many items we can craft with ingredients from ALL containers.
                ushort maxCraftableAll = recipe.calculateMaxSynths();
                if (maxCraftableAll == 0)
                {
                    StopCrafter();
                    LoggingFunctions.Timestamp("Could not find enough materials in all mobile containers to synthesize again. Stopping.");
                    return;
                }

                List<ushort> requiredItemIds = new List<ushort>();
                requiredItemIds.Add(recipe.CrystalId);
                requiredItemIds.Add(Things.CrystalAndClusterExchange(recipe.CrystalId));
                foreach (Recipe.CIngredient ingr in recipe.mIngredients)
                {
                    requiredItemIds.Add(ingr.ID);
                }

                List<Item> nonRecipeItemsInBag = new List<Item>();
                List<ushort> nonRecipeQuanInBag = new List<ushort>();
                Containers.Bag.GetPrunedItemList(requiredItemIds, ref nonRecipeItemsInBag, ref nonRecipeQuanInBag);

                byte nonRecipeSlotsOpen = (byte)(Containers.Bag.Capacity - nonRecipeItemsInBag.Count);

                // If we have any partial crystal/ingredient stacks in our bag, we need to deduct those from the nonRecipeSlotsOpen.
                // This is because we always import full stacks (when possible) into the bag.  So if we have 4 crystals in our bag
                // and we calculate we need to move ingredients for 20 synths, it will move over 2 stacks instead of 1 + 8 crystals,
                // leaving us with an extra slot filled. This is why we have the modulus conditions in the loop below.

                // Based on the non-recipe slots open (nb of open slots if we removed the recipe crystal/ingredients),
                // we can calculate how many total synth's we can do.
                ushort nbSlotsNeeded = 0;
                ushort nbSlotsNeededPrev = 0;
                ushort nbSynthsPossible = 0;
                for (int ii = 1; ii <= maxCraftableAll; ii++)
                {
                    nbSlotsNeededPrev = nbSlotsNeeded;
                    ushort nbItem = Containers.Bag.GetItemQuan(recipe.CrystalId);
                    byte stackSize = Things.GetStackSizeFromId(recipe.CrystalId);
                    nbSlotsNeeded = (ushort)Math.Ceiling(((decimal)ii * 1) / Things.GetStackSizeFromId(recipe.CrystalId));
                    if ((nbItem % stackSize) != 0)
                    {
                        nbSlotsNeeded++;
                    }
                    for (int kk = 0; kk < recipe.mIngredients.Count; kk++)
                    {
                        nbSlotsNeeded += (ushort)Math.Ceiling(ii * (decimal)recipe.mIngredients[kk].Quantity / Things.GetStackSizeFromId(recipe.mIngredients[kk].ID));
                        nbItem = Containers.Bag.GetItemQuan(recipe.mIngredients[kk].ID);
                        stackSize = Things.GetStackSizeFromId(recipe.mIngredients[kk].ID);
                        if ((nbItem % stackSize) != 0)
                        {
                            nbSlotsNeeded++;
                        }
                    }
                    //for (int mm = 0; mm < recipe.mResults.Count; mm++)
                    for (int mm = 0; mm < 1; mm++)  // The first result is the NQ which is what we care about.
                    {
                        nbSlotsNeeded += (ushort)Math.Ceiling(ii * (decimal)recipe.mResults[mm].Quantity / Things.GetStackSizeFromId(recipe.mResults[mm].ID));
                    }
                    if (nbSlotsNeeded > nonRecipeSlotsOpen)
                    {
                        nbSynthsPossible = (ushort)(ii - 1);
                        nbSlotsNeeded = nbSlotsNeededPrev;
                        LoggingFunctions.Debug("swapIngredientsAndResults: nbSlotsNeededPrev = " + nbSlotsNeededPrev + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                        LoggingFunctions.Debug("swapIngredientsAndResults: nbSynthsPossible = " + nbSynthsPossible + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                        break;
                    }
                    else if (maxCraftableAll == ii)
                    {
                        nbSynthsPossible = (ushort)ii;
                        LoggingFunctions.Debug("swapIngredientsAndResults: nbSlotsNeededPrev = " + nbSlotsNeededPrev + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                        LoggingFunctions.Debug("swapIngredientsAndResults: nbSynthsPossible = " + nbSynthsPossible + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                    }
                }

                // Now just move over the difference between what we need (nbSynthsPossible * ingredientQuan) and what we already have (bag summary list quan).
                // To know where to move them FROM, we need to create a list of where these are in quantity.
                // That is, keep a list of each container and what items / how many to move from each.
                Dictionary<ushort, ushort> itemsToMoveTotal = new Dictionary<ushort, ushort>();
                List<ushort> itemsToMoveList = new List<ushort>();
                ushort localQuanNeeded = nbSynthsPossible;
                ushort localQuanInBag = (ushort)(Containers.Bag.GetItemQuan(recipe.CrystalId) + (Containers.Bag.GetItemQuan(Things.CrystalAndClusterExchange(recipe.CrystalId)) * 12));
                if (localQuanNeeded > localQuanInBag) //if total number of synths possible > # crystals + broken clusters in bag, move over more crystals/clusters
                {
                    //Figure out if we need to move over crystals, clusters, or both.
                    ushort nbCrystalsShortInBag = (ushort)(localQuanNeeded - localQuanInBag);
                    ushort nbCrystalsNotInBag = (ushort)(Containers.GetItemQuanMobile(recipe.CrystalId) - localQuanInBag);
                    bool moveCrystals = ((nbCrystalsShortInBag > 0) && (nbCrystalsNotInBag > 0));
                    // If we NEED more than we have, move all of them. Otherwise, move only what we need.
                    if (moveCrystals)
                    {
                        ushort nbCrystalsToMove = nbCrystalsShortInBag > nbCrystalsNotInBag ? nbCrystalsNotInBag : nbCrystalsShortInBag;

                        itemsToMoveTotal.Add(recipe.CrystalId, nbCrystalsToMove);
                        itemsToMoveList.Add(recipe.CrystalId);
                    }

                    ushort nbCrystalsTotal = Containers.GetItemQuanMobile(recipe.CrystalId);
                    ushort nbClustersInBag = Containers.Bag.GetItemQuan(Things.CrystalAndClusterExchange(recipe.CrystalId));
                    ushort nbCrystalsShortTotal = (ushort)(localQuanNeeded - nbCrystalsTotal);
                    // If we add in the clusters in the bag already, are we still short crystals?
                    if (nbCrystalsShortTotal >= (nbClustersInBag * 12))
                    {
                        // We still need to move over clusters.
                        // Move over nbCrystalsShortTotal - nbClustersInBag * 12 , / 12
                        ushort nbClustersTotal = Containers.GetItemQuanMobile(Things.CrystalAndClusterExchange(recipe.CrystalId));
                        ushort nbClustersToMove = (ushort)(nbCrystalsShortTotal / 12);
                        if (nbCrystalsShortTotal % 12 != 0)
                        {
                            nbClustersToMove++;
                        }
                        nbClustersToMove = nbClustersToMove > nbClustersTotal ? nbClustersTotal : nbClustersToMove;

                        itemsToMoveTotal.Add(Things.CrystalAndClusterExchange(recipe.CrystalId), nbClustersToMove);
                        itemsToMoveList.Add(Things.CrystalAndClusterExchange(recipe.CrystalId));
                    }
                }
                for (int ii = 0; ii < recipe.mIngredients.Count; ii++)
                {
                    localQuanNeeded = (ushort)(recipe.mIngredients[ii].Quantity * nbSynthsPossible);
                    localQuanInBag = Containers.Bag.GetItemQuan(recipe.mIngredients[ii].ID);
                    if (localQuanNeeded > localQuanInBag)
                    {
                        itemsToMoveTotal.Add(recipe.mIngredients[ii].ID, (ushort)(localQuanNeeded - localQuanInBag));
                        itemsToMoveList.Add(recipe.mIngredients[ii].ID);
                    }
                }
                // Now go through each container to find them and populate the lists.
                ushort currQuan = 0;
                List<ItemContainer> srcContainers = new List<ItemContainer>();
                srcContainers.Add(Containers.MCase);
                srcContainers.Add(Containers.Sack);
                srcContainers.Add(Containers.Satchel);

                for (int ii = 0; ii < srcContainers.Count; ii++)
                {

                    foreach (ushort id in itemsToMoveList)
                    {
                        Containers.RebuildListsMobileOnly();
                        currQuan = srcContainers[ii].GetItemQuan(id);
                        if (currQuan > itemsToMoveTotal[id])
                        {
                            currQuan = itemsToMoveTotal[id];
                        }
                        if (currQuan > 0)
                        {
                            ushort nbMoved = Inventory.Movement.MoveItem(id, currQuan, srcContainers[ii], Containers.Bag, checkStatus);
                            if (nbMoved <= itemsToMoveTotal[id])
                            {
                                itemsToMoveTotal[id] -= nbMoved;
                            }
                            else
                            {
                                itemsToMoveTotal[id] = 0;
                            }
                        }
                    }
                }
                Inventory.Movement.SortInventory(false);
                Containers.RebuildListsMobileOnly();
                #endregion Move to Bag
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::swapIngredientsAndResults: " + e.ToString());
            }
        }
        private bool breakCluster()
        {
            if (MemReads.Self.get_in_mog_house())
            {
                Statics.FuncPtrs.SetStatusBoxPtr("Cannot break cluster inside mog house.", Statics.Fields.Yellow);
                return false;
            }
            if (Containers.Bag.GetItemQuan(Things.CrystalAndClusterExchange(recipe.CrystalId)) > 0)
            {
                string cmdStr = "/item \"" + Things.GetNameFromId(Things.CrystalAndClusterExchange(recipe.CrystalId)) + "\" <me>";
                RawCommand brkCluster = new RawCommand("Break Cluster", cmdStr, true); // Will execute immediately.
                IocaineFunctions.delay(5000);
                return true;
            }
            return false;
        }
        #endregion interaction functions
        #region control methods
        public void SetMaxCount(int maxCount)
        {
            numberOf = maxCount;
            maxCountUpdated = true;
        }
        #endregion control methods
        #region GUI synchronization
        private void updateStartButton(String text, System.Drawing.Color color)
        {
            try
            {
                LoggingFunctions.Debug("Crafter::updateStartButton: Trying to update CB start button.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                StartButton.BeginInvoke(updateCrafterStartButtonCallBack, new object[] { text, color });
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Crafter::updateStartButton: " + e.ToString());
            }
        }
        #endregion GUI synchronization
        #region Chatlog Functions
        private void parseResult(ref List<ushort> lostItem, ref List<ushort> lostQuan, ref RecipeLog.RESULT_TYPE result)
        {
            try
            {
                LoggingFunctions.Debug("Crafter::parseResult: Entering Crafter::parseResult.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                uint nbLines = 0;
                if (chatLog.Update(ref nbLines))
                {
                    LoggingFunctions.Debug("Crafter::parseResult: Parsing " + nbLines + " lines.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                    bool foundResult = false;
                    for (int ii = 0; ii < nbLines; ii++)
                    {
                        ChatLine chatString = null;
                        //If we've already gone thru and determine that it failed,
                        //build the lost item list.
                        if (foundResult)
                        {
                            chatLog.Read(out chatString);
                            continue;
                        }
                        if (result == RecipeLog.RESULT_TYPE.FAIL)
                        {
                            if (!chatLog.Read(out chatString))
                            {
                                LoggingFunctions.Error("Could not read chat log from Crafter");
                                continue;
                            }
                            if ((chatString.Mode == FFXIEnums.CHAT_MODE.SYNTH_LOST_ITEM)
                                && chatString.ProcessedLine.Contains("was lost"))
                            {
                                //We know we lost an item, need to figure out which one it was.
                                foreach (Recipe.CIngredient ingredient in recipe.mIngredients)
                                {
                                    if (chatString.ProcessedLine.Contains(Things.GetLogNameSFromId(ingredient.ID)))
                                    {
                                        pushLostItem(ref lostItem, ref lostQuan, ingredient.ID, 1);
                                    }
                                }
                            }
                            else
                            {
                                //If its not the right chat code or doesn't say an item was lost, 
                                //then its some other chat, continue.
                                continue;
                            }
                        }
                        //If its not fail, it must be unknown since we return upon a successful synth parse.
                        else
                        {
                            if (!chatLog.Read(out chatString))
                            {
                                LoggingFunctions.Error("Could not read chat log from Crafter");
                                continue;
                            }
                            LoggingFunctions.Debug("Crafter::parseResult: Chat string being parsed: \"" + chatString + "\"", LoggingFunctions.DBG_SCOPE.CRAFTER);
                            if ((chatString.Mode == FFXIEnums.CHAT_MODE.SYNTH_LOST_ITEM)
                                && chatString.ProcessedLine.Contains("Synthesis failed. You lost"))
                            {
                                result = RecipeLog.RESULT_TYPE.FAIL;
                                continue;
                            }
                            else if ((chatString.Mode == FFXIEnums.CHAT_MODE.SYNTH_MADE_ITEM)
                                    && chatString.ProcessedLine.Contains("You synthesized "))
                            {
                                // This part of the code parses whether a 

                                ushort quan = parseQuantity(chatString.ProcessedLine);
                                String itemText = parseItemText(chatString.ProcessedLine);

                                Recipe.RESULT_TYPE result_type = recipe.parseResult(itemText, quan);

                                switch (result_type)
                                {
                                    case Recipe.RESULT_TYPE.RESULT_NQ:
                                        result = RecipeLog.RESULT_TYPE.NQ;
                                        break;
                                    case Recipe.RESULT_TYPE.RESULT_HQ1:
                                        result = RecipeLog.RESULT_TYPE.HQ1;
                                        break;
                                    case Recipe.RESULT_TYPE.RESULT_HQ2:
                                        result = RecipeLog.RESULT_TYPE.HQ2;
                                        break;
                                    case Recipe.RESULT_TYPE.RESULT_HQ3:
                                        result = RecipeLog.RESULT_TYPE.HQ3;
                                        break;
                                    default:
                                        result = RecipeLog.RESULT_TYPE.UNK;
                                        break;
                                };
                                foundResult = true;
                            }
                            else
                            {
                                //We haven't gotten to the result yet, or we didn't actually
                                //synth anything. Keep unknown result and continue.
                                continue;
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::parseResult: " + e.ToString());
            }
        }
        private void pushLostItem(ref List<ushort> lostItem, ref List<ushort> lostQuan, ushort item, ushort quan)
        {
            try
            {
                if (lostItem.Contains(item))
                {
                    int index = lostItem.IndexOf(item);
                    lostQuan[index] += quan;
                }
                else
                {
                    lostItem.Add(item);
                    lostQuan.Add(quan);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::pushLostItem: " + e.ToString());
            }
        }
        private ushort parseQuantity(String str)
        {
            ushort quan = 0;
            try
            {
                Regex expression = new Regex("([0-9]+)");
                Match match1 = expression.Match(str);
                if (!match1.Success)
                {
                    //We didn't find any numbers in the string, set quantity 1
                    LoggingFunctions.Debug("Crafter::parseQuantity: Parsing craft result, did not find a quantity.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                    quan = 1;
                }
                else
                {
                    //We found a specific quantity, need to record it.
                    quan = UInt16.Parse(match1.Groups[0].ToString());
                    LoggingFunctions.Debug("Crafter::parseQuantity: Parsing craft result, found quantity: " + quan + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::parseQuantity: " + e.ToString());
            }
            return quan;
        }
        private String parseItemText(String str)
        {
            String itemText = "";
            try
            {
                byte itemPrefix0 = 30;
                byte itemPrefix1 = 2;
                byte itemSuffix1 = 1;
                int startIndex = 0;
                int endIndex = 0;
                LoggingFunctions.Debug("Crafter::parseItemText: Input string: '" + str + "'.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                for (int ii = 0; ii < str.Length; ii++)
                {
                    if (((byte)str[ii] == itemPrefix0) && ((byte)str[ii + 1] == itemPrefix1))
                    {
                        startIndex = ii + 2;
                        ii += 2;
                    }
                    else if (((byte)str[ii] == itemPrefix0) && ((byte)str[ii + 1] == itemSuffix1))
                    {
                        endIndex = ii - 1;
                    }
                }
                if ((startIndex < 0) || (endIndex <= 0))
                {
                    LoggingFunctions.Error("Crafter.parseItemText: str: " + str + " || startIndex: " + startIndex + " || endIndex: " + endIndex);
                    return "";
                }
                itemText = str.Substring(startIndex, (endIndex - startIndex + 1));
                LoggingFunctions.Debug("Crafter::parseItemText: Parsed out \"" + itemText + "\"", LoggingFunctions.DBG_SCOPE.CRAFTER);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::parseItemText: " + e.ToString());
            }
            return itemText;
        }
        private bool chatlogContains(String str)
        {
            uint nbLines = 0;
            try
            {
                chatLog.Update(ref nbLines);
                if (nbLines > 0)
                {
                    for (int ii = 0; ii < nbLines; ii++)
                    {
                        ChatLine chatString = null;
                        int chatCode = 0;
                        if (!chatLog.Read(out chatString))
                        {
                            LoggingFunctions.Error("Could not read chat log from Crafter");
                            return false;
                        }
                        if (chatString.ProcessedLine.Contains(str))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("Crafter::chatlogContains: " + e.ToString());
            }
            return false;
        }
        #endregion Chatlog Functions
        #endregion Utility Functions
    }
}
