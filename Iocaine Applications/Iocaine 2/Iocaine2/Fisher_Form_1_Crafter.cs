using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Bots;
using Iocaine2.Data.Client;
using Iocaine2.Data.Structures;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Properties;
using Iocaine2.Settings;
using Iocaine2.Tools;

namespace Iocaine2
{
    //This file is for functions directly related to the Crafter
    partial class Iocaine_2_Form
    {
        #region Member Variables
        private string recipeDir = ".\\Recipes\\";
        private string recipeWWFile = "Woodworking.xml";
        private string recipeSMFile = "Smithing.xml";
        private string recipeGSFile = "Goldsmithing.xml";
        private string recipeCCFile = "Clothcraft.xml";
        private string recipeLCFile = "Leathercraft.xml";
        private string recipeBCFile = "Bonecraft.xml";
        private string recipeALFile = "Alchemy.xml";
        private string recipeCKFile = "Cooking.xml";

        private string recipeWWTable = "RecipesWW";
        private string recipeSMTable = "RecipesSM";
        private string recipeGSTable = "RecipesGS";
        private string recipeCCTable = "RecipesCC";
        private string recipeLCTable = "RecipesLC";
        private string recipeBCTable = "RecipesBC";
        private string recipeALTable = "RecipesAL";
        private string recipeCKTable = "RecipesCK";

        private Recipe CB_MR_currentRecipe;
        private string currentMRRecipeTable;
        private string currentMRRecipeFile;
        private string currentCrafterRecipeTable;

        private List<Item.ITEM_TYPE> CB_InventoryFilterList;

        private CraftingTracker CB_Stats_Tracker;

        // Crafter textboxes list
        private List<System.Windows.Forms.TextBox> mCB_NamesBoxes;
        private List<System.Windows.Forms.TextBox> mCB_InvCountBoxes;
        private List<System.Windows.Forms.TextBox> mCB_CraftCountBoxes;

        private List<System.Windows.Forms.TextBox> mCB_ResultNamesBoxes;
        private List<System.Windows.Forms.TextBox> mCB_ResultQuantityBoxes;

        // Recipe Manager Textboxes list
        private List<System.Windows.Forms.TextBox> mCB_MR_IngrNameBoxes;
        private List<System.Windows.Forms.NumericUpDown> mCB_MR_IngrQuantityBoxes;
        private List<System.Windows.Forms.TextBox> mCB_MR_ResultNameBoxes;
        private List<System.Windows.Forms.NumericUpDown> mCB_MR_ResultQuantityBoxes;

        internal static bool CB_MoveInventory = true;

        #endregion Member Variables
        #region Thread Synchronization
        #region Delegate declarations
        public delegate void CB_craftNumberOfUpDownUpdateDelegate(int value);
        public delegate void CB_craftNumberOfRBUpdateDelegate(bool value);
        public delegate void CB_craftMaxTBDelegate(string text);
        public delegate void CB_craftMaxRBDelegate(bool value);
        public delegate void CB_craftUntilSkillTBDelegate(string text);
        public delegate void CB_craftUntilSkillRBDelegate(bool value);
        public delegate void CB_synthRBDelegate(bool value);
        public delegate void CB_desynthRBDelegate(bool value);
        public delegate void CB_MR_CreateRBDelegate(bool value);
        public delegate void CB_MR_EditRBDelegate(bool value);
        public delegate void CB_MR_DeleteRBDelegate(bool value);
        public delegate void CB_bagCountCurrentDelegate(byte value);
        public delegate void CB_bagCountMaxDelegate(byte value);
        public delegate void CB_satchelCountCurrentDelegate(byte value);
        public delegate void CB_satchelCountMaxDelegate(byte value);
        public delegate void CB_statsInitDelegate(DataGridView dgv, Label recipeLabel, Label currentRecipeLabel, Label maxRecipeLabel);
        public delegate void CB_statsRecipeNumberDelegate(int value);
        public delegate void CB_statsRecipeMaxDelegate(int value);
        public delegate void CB_statsAddRecipeDelegate(Recipe recipe, DataGridView dgv, Label recipeLabel, Label currentRecipeLabel, Label maxRecipeLabel);
        public delegate void CB_statsUpdateDelegate(DataGridView dgv, Label recipeLabel);
        public delegate void CB_rebuildInventoryLBDelegate();
        public delegate void CB_playSoundTBDelegate(string str);
        public delegate void CB_updateRecipeReportDelegate(Recipe recipe);
        public delegate void CB_clearRecipeInCrafterDelegate();
        public delegate void CB_MR_clearRecipeInManagerDelegate();

        #endregion Delegate declarations
        #region Delegate instances
        private CB_craftNumberOfUpDownUpdateDelegate CB_updateCraftNumberOfUpDownCallBack;
        private CB_craftNumberOfRBUpdateDelegate CB_updateCraftNumberOfRBCallBack;
        private CB_craftMaxTBDelegate CB_updateCraftMaxTBCallBack;
        private CB_craftMaxRBDelegate CB_updateCraftMaxRBCallBack;
        private CB_craftUntilSkillTBDelegate CB_updateCraftUntilSkillTBCallBack;
        private CB_craftUntilSkillRBDelegate CB_updateCraftUntilSkillRBCallBack;
        private CB_synthRBDelegate CB_updateSynthRBCallBack;
        private CB_desynthRBDelegate CB_updateDeSynthRBCallBack;
        private CB_MR_CreateRBDelegate CB_MR_updateCreateRBCallBack;
        private CB_MR_EditRBDelegate CB_MR_updateEditRBCallBack;
        private CB_MR_DeleteRBDelegate CB_MR_updateDeleteRBCallBack;
        private CB_bagCountCurrentDelegate CB_updateBagCountCurrentCallBack;
        private CB_bagCountMaxDelegate CB_updateBagCountMaxCallBack;
        private CB_satchelCountCurrentDelegate CB_updateSatchelCountCurrentCallBack;
        private CB_satchelCountMaxDelegate CB_updateSatchelCountMaxCallBack;
        private CB_statsInitDelegate CB_statsInitCallBack;
        private CB_statsRecipeNumberDelegate CB_statsRecipeNumberCallBack;
        private CB_statsRecipeMaxDelegate CB_statsRecipeMaxCallBack;
        private CB_statsAddRecipeDelegate CB_statsAddRecipeCallBack;
        private CB_statsUpdateDelegate CB_statsUpdateCallBack;
        private CB_rebuildInventoryLBDelegate CB_rebuildInventoryLBCallBack;
        private CB_playSoundTBDelegate CB_playSoundTBCallBack;
        private CB_updateRecipeReportDelegate CB_updateRecipeReportCallBack;
        private CB_clearRecipeInCrafterDelegate CB_clearRecipeInCrafterCallBack;
        private CB_MR_clearRecipeInManagerDelegate CB_MR_clearRecipeInManagerCallBack;

        #endregion Delegate instances
        #region Delegate instantiations
        private void CB_createDelegates()
        {
            CB_updateCraftNumberOfUpDownCallBack = new CB_craftNumberOfUpDownUpdateDelegate(CB_updateCraftNumberOfUpDownCallBackFunction);
            CB_updateCraftNumberOfRBCallBack = new CB_craftNumberOfRBUpdateDelegate(CB_updateCraftNumberOfRBCallBackFunction);
            CB_updateCraftMaxTBCallBack = new CB_craftMaxTBDelegate(CB_updateCraftMaxTBCallBackFunction);
            CB_updateCraftMaxRBCallBack = new CB_craftMaxRBDelegate(CB_updateCraftMaxRBCallBackFunction);
            CB_updateCraftUntilSkillTBCallBack = new CB_craftUntilSkillTBDelegate(CB_updateCraftUntilSkillTBCallBackFunction);
            CB_updateCraftUntilSkillRBCallBack = new CB_craftUntilSkillRBDelegate(CB_updateCraftUntilSkillRBCallBackFunction);
            CB_updateSynthRBCallBack = new CB_synthRBDelegate(CB_updateSynthRBCallBackFunction);
            CB_updateDeSynthRBCallBack = new CB_desynthRBDelegate(CB_updateDeSynthRBCallBackFunction);
            CB_MR_updateCreateRBCallBack = new CB_MR_CreateRBDelegate(CB_MR_updateCreateRBCallBackFunction);
            CB_MR_updateEditRBCallBack = new CB_MR_EditRBDelegate(CB_MR_updateEditRBCallBackFunction);
            CB_MR_updateDeleteRBCallBack = new CB_MR_DeleteRBDelegate(CB_MR_updateDeleteRBCallBackFunction);
            CB_updateBagCountCurrentCallBack = new CB_bagCountCurrentDelegate(CB_updateBagCountCurrentCallBackFunction);
            CB_updateBagCountMaxCallBack = new CB_bagCountMaxDelegate(CB_updateBagCountMaxCallBackFunction);
            CB_updateSatchelCountCurrentCallBack = new CB_satchelCountCurrentDelegate(CB_updateSatchelCountCurrentCallBackFunction);
            CB_updateSatchelCountMaxCallBack = new CB_satchelCountMaxDelegate(CB_updateSatchelCountMaxCallBackFunction);
            CB_statsInitCallBack = new CB_statsInitDelegate(CB_statsInitCallBackFunction);
            CB_statsRecipeNumberCallBack = new CB_statsRecipeNumberDelegate(CB_statsRecipeNumberCallBackFunction);
            CB_statsRecipeMaxCallBack = new CB_statsRecipeMaxDelegate(CB_statsRecipeMaxCallBackFunction);
            CB_statsAddRecipeCallBack = new CB_statsAddRecipeDelegate(CB_statsAddRecipeCallBackFunction);
            CB_statsUpdateCallBack = new CB_statsUpdateDelegate(CB_statsUpdateCallBackFunction);
            CB_rebuildInventoryLBCallBack = new CB_rebuildInventoryLBDelegate(CB_rebuildInventoryLBCallBackFunction);
            CB_playSoundTBCallBack = new CB_playSoundTBDelegate(CB_playSoundTBCallBackFunction);
            CB_updateRecipeReportCallBack = new CB_updateRecipeReportDelegate(CB_updateRecipeReportCallBackFunction);
            CB_clearRecipeInCrafterCallBack = new CB_clearRecipeInCrafterDelegate(CB_clearRecipeInCrafterCallBackFunction);
            CB_MR_clearRecipeInManagerCallBack = new CB_MR_clearRecipeInManagerDelegate(CB_MR_clearRecipeInManagerCallBackFunction);

        }
        #endregion Delegate instantiations
        #region Call Back Functions
        private void CB_updateCraftNumberOfUpDownCallBackFunction(int value)
        {
            CB_CraftNumberOf_UpDown.Value = value;
        }
        private void CB_updateCraftNumberOfRBCallBackFunction(bool value)
        {
            CB_CraftNumberOf_RadioButton.Checked = value;
        }
        private void CB_updateCraftMaxTBCallBackFunction(string text)
        {
            CB_CraftMax_Textbox.Text = text;
        }
        private void CB_updateCraftMaxRBCallBackFunction(bool value)
        {
            CB_CraftMax_RadioButton.Checked = value;
        }
        private void CB_updateCraftUntilSkillTBCallBackFunction(string text)
        {
            CB_CraftUntilSkill_Textbox.Text = text;
        }
        private void CB_updateCraftUntilSkillRBCallBackFunction(bool value)
        {
            CB_CraftUntilSkill_RadioButton.Checked = value;
        }
        private void CB_updateSynthRBCallBackFunction(bool value)
        {
            CB_Synth_RadioButton.Checked = value;
        }
        private void CB_updateDeSynthRBCallBackFunction(bool value)
        {
            CB_DeSynth_RadioButton.Checked = value;
        }
        private void CB_MR_updateCreateRBCallBackFunction(bool value)
        {
            CB_MR_CreateNew_RadioButton.Checked = value;
        }
        private void CB_MR_updateEditRBCallBackFunction(bool value)
        {
            CB_MR_EditExisting_RadioButton.Checked = value;
        }
        private void CB_MR_updateDeleteRBCallBackFunction(bool value)
        {
            CB_MR_DeleteExisting_RadioButton.Checked = value;
        }
        private void CB_updateBagCountCurrentCallBackFunction(byte value)
        {
            CB_Bag_Count_Current_Label.Text = value.ToString();
        }
        private void CB_updateBagCountMaxCallBackFunction(byte value)
        {
            CB_Bag_Count_Max_Label.Text = value.ToString();
        }
        private void CB_updateSatchelCountCurrentCallBackFunction(byte value)
        {
            CB_Satchel_Count_Current_Label.Text = value.ToString();
        }
        private void CB_updateSatchelCountMaxCallBackFunction(byte value)
        {
            CB_Satchel_Count_Max_Label.Text = value.ToString();
        }
        private void CB_statsInitCallBackFunction(DataGridView dgv, Label recipeLabel, Label currentRecipeLabel, Label maxRecipeLabel)
        {
            CB_Stats_Tracker.InitializeStatsDGV(dgv, recipeLabel, currentRecipeLabel, maxRecipeLabel);
        }
        private void CB_statsRecipeNumberCallBackFunction(int value)
        {
            CB_Stats_Recipe_Number_Label.Text = value.ToString();
        }
        private void CB_statsRecipeMaxCallBackFunction(int value)
        {
            CB_Stats_Recipe_Max_Label.Text = value.ToString();
        }
        private void CB_statsAddRecipeCallBackFunction(Recipe recipe, DataGridView dgv, Label recipeLabel, Label currentRecipeLabel, Label maxRecipeLabel)
        {
            CB_Stats_Tracker.AddRecipe(recipe, dgv, recipeLabel, currentRecipeLabel, maxRecipeLabel);
        }
        private void CB_statsUpdateCallBackFunction(DataGridView dgv, Label recipeLabel)
        {
            CB_Stats_Tracker.UpdateStatsDGV(dgv, recipeLabel);
        }
        private void CB_rebuildInventoryLBCallBackFunction()
        {
            rebuildInventoryListbox();
        }
        private void CB_playSoundTBCallBackFunction(string str)
        {
            CB_PlaySound_TB.Text = str;
        }
        private void CB_updateRecipeReportCallBackFunction(Recipe recipe)
        {
            loadRecipeIntoCrafter(recipe);
        }
        private void CB_clearRecipeInCrafterCallBackFunction()
        {
            clearRecipeInCrafter();
        }
        private void CB_MR_clearRecipeInManagerCallBackFunction()
        {
            clearRecipeInManager();
        }
        #endregion Call Back Functions
        #region Update Wrapper Functions
        private void CB_udpateCraftNumberOfUpDown(int value)
        {
            try
            {
                if (CB_CraftNumberOf_UpDown.InvokeRequired)
                {
                    CB_CraftNumberOf_UpDown.Invoke(CB_updateCraftNumberOfUpDownCallBack, new object[] { value });
                }
                else
                {
                    CB_updateCraftNumberOfUpDownCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_udpateCraftNumberOfRB(bool value)
        {
            try
            {
                if (CB_CraftNumberOf_RadioButton.InvokeRequired)
                {
                    CB_CraftNumberOf_RadioButton.Invoke(CB_updateCraftNumberOfRBCallBack, new object[] { value });
                }
                else
                {
                    CB_updateCraftNumberOfRBCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_udpateCraftMaxTB(string text)
        {
            try
            {
                if (CB_CraftMax_Textbox.InvokeRequired)
                {
                    CB_CraftMax_Textbox.Invoke(CB_updateCraftMaxTBCallBack, new object[] { text });
                }
                else
                {
                    CB_updateCraftMaxTBCallBackFunction(text);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_udpateCraftMaxRB(bool value)
        {
            try
            {
                if (CB_CraftMax_RadioButton.InvokeRequired)
                {
                    CB_CraftMax_RadioButton.Invoke(CB_updateCraftMaxRBCallBack, new object[] { value });
                }
                else
                {
                    CB_updateCraftMaxRBCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_udpateCraftUntilSkillTB(string text)
        {
            try
            {
                if (CB_CraftUntilSkill_Textbox.InvokeRequired)
                {
                    CB_CraftUntilSkill_Textbox.Invoke(CB_updateCraftUntilSkillTBCallBack, new object[] { text });
                }
                else
                {
                    CB_updateCraftUntilSkillTBCallBackFunction(text);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_udpateCraftUntilSkillRB(bool value)
        {
            try
            {
                if (CB_CraftUntilSkill_RadioButton.InvokeRequired)
                {
                    CB_CraftUntilSkill_RadioButton.Invoke(CB_updateCraftUntilSkillRBCallBack, new object[] { value });
                }
                else
                {
                    CB_updateCraftUntilSkillRBCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_updateSynthRB(bool value)
        {
            try
            {
                if (CB_Synth_RadioButton.InvokeRequired)
                {
                    CB_Synth_RadioButton.Invoke(CB_updateSynthRBCallBack, new object[] { value });
                }
                else
                {
                    CB_updateSynthRBCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_updateDeSynthRB(bool value)
        {
            try
            {
                if (CB_DeSynth_RadioButton.InvokeRequired)
                {
                    CB_DeSynth_RadioButton.Invoke(CB_updateDeSynthRBCallBack, new object[] { value });
                }
                else
                {
                    CB_updateDeSynthRBCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_MR_updateCreateRB(bool value)
        {
            try
            {
                if (CB_MR_CreateNew_RadioButton.InvokeRequired)
                {
                    CB_MR_CreateNew_RadioButton.Invoke(CB_MR_updateCreateRBCallBack, new object[] { value });
                }
                else
                {
                    CB_MR_updateCreateRBCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_MR_updateEditRB(bool value)
        {
            try
            {
                if (CB_MR_EditExisting_RadioButton.InvokeRequired)
                {
                    CB_MR_EditExisting_RadioButton.Invoke(CB_MR_updateEditRBCallBack, new object[] { value });
                }
                else
                {
                    CB_MR_updateEditRBCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_MR_updateDeleteRB(bool value)
        {
            try
            {
                if (CB_MR_DeleteExisting_RadioButton.InvokeRequired)
                {
                    CB_MR_DeleteExisting_RadioButton.Invoke(CB_MR_updateDeleteRBCallBack, new object[] { value });
                }
                else
                {
                    CB_MR_updateDeleteRBCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_updateBagCountCurrent(byte value)
        {
            try
            {
                if (CB_Bag_Count_Current_Label.InvokeRequired)
                {
                    CB_Bag_Count_Current_Label.Invoke(CB_updateBagCountCurrentCallBack, new object[] { value });
                }
                else
                {
                    CB_updateBagCountCurrentCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_updateBagCountMax(byte value)
        {
            try
            {
                if (CB_Bag_Count_Max_Label.InvokeRequired)
                {
                    CB_Bag_Count_Max_Label.Invoke(CB_updateBagCountMaxCallBack, new object[] { value });
                }
                else
                {
                    CB_updateBagCountMaxCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_updateSatchelCountCurrent(byte value)
        {
            try
            {
                if (CB_Satchel_Count_Current_Label.InvokeRequired)
                {
                    CB_Satchel_Count_Current_Label.Invoke(CB_updateSatchelCountCurrentCallBack, new object[] { value });
                }
                else
                {
                    CB_updateSatchelCountCurrentCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_updateSatchelCountMax(byte value)
        {
            try
            {
                if (CB_Satchel_Count_Max_Label.InvokeRequired)
                {
                    CB_Satchel_Count_Max_Label.Invoke(CB_updateSatchelCountMaxCallBack, new object[] { value });
                }
                else
                {
                    CB_updateSatchelCountMaxCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_statsInit(DataGridView dgv, Label recipeLabel, Label currentRecipeLabel, Label maxRecipeLabel)
        {
            try
            {
                if (CB_Stats_DGV.InvokeRequired)
                {
                    CB_Stats_DGV.Invoke(CB_statsInitCallBack, new object[] { dgv, recipeLabel, currentRecipeLabel, maxRecipeLabel });
                }
                else
                {
                    CB_statsInitCallBackFunction(dgv, recipeLabel, currentRecipeLabel, maxRecipeLabel);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_statsRecipeNumberUpdate(int value)
        {
            try
            {
                if (CB_Stats_Recipe_Number_Label.InvokeRequired)
                {
                    CB_Stats_Recipe_Number_Label.Invoke(CB_statsRecipeNumberCallBack, new object[] { value });
                }
                else
                {
                    CB_statsRecipeNumberCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_statsRecipeMaxUpdate(int value)
        {
            try
            {
                if (CB_Stats_Recipe_Max_Label.InvokeRequired)
                {
                    CB_Stats_Recipe_Max_Label.Invoke(CB_statsRecipeMaxCallBack, new object[] { value });
                }
                else
                {
                    CB_statsRecipeMaxCallBackFunction(value);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_statsAddRecipe(Recipe recipe, DataGridView dgv, Label recipeLabel, Label currentRecipeLabel, Label maxRecipeLabel)
        {
            try
            {
                if (CB_Stats_DGV.InvokeRequired)
                {
                    CB_Stats_DGV.Invoke(CB_statsAddRecipeCallBack, new object[] { recipe, dgv, recipeLabel, currentRecipeLabel, maxRecipeLabel });
                }
                else
                {
                    CB_statsAddRecipeCallBackFunction(recipe, dgv, recipeLabel, currentRecipeLabel, maxRecipeLabel);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_statsUpdate(DataGridView dgv, Label recipeLabel)
        {
            try
            {
                if (CB_Stats_DGV.InvokeRequired)
                {
                    CB_Stats_DGV.Invoke(CB_statsUpdateCallBack, new object[] { dgv, recipeLabel });
                }
                else
                {
                    CB_statsUpdateCallBackFunction(dgv, recipeLabel);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_rebuildInventoryLB()
        {
            try
            {
                if (CB_Inventory_Listbox.InvokeRequired)
                {
                    CB_Inventory_Listbox.Invoke(CB_rebuildInventoryLBCallBack);
                }
                else
                {
                    CB_rebuildInventoryLBCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating form control: " + e.ToString());
            }
        }
        private void CB_updatePlaySoundTB(string str)
        {
            try
            {
                if (CB_PlaySound_TB.InvokeRequired)
                {
                    CB_PlaySound_TB.Invoke(CB_playSoundTBCallBack, new object[] { str });
                }
                else
                {
                    CB_playSoundTBCallBackFunction(str);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating playSoundTB: " + e.ToString());
            }
        }
        // If we want to do this correctly, the whole report GUI element (so the group of textboxes) whould be contained by one custom control. 
        // This way, we know that all the textboxes belong to the same thread. (but this should work too)
        private void CB_updateRecipeReport(Recipe recipe)
        {
            try
            {
                if (CB_Ingredient1CraftCount_Textbox.InvokeRequired)
                {
                    CB_Ingredient1CraftCount_Textbox.Invoke(new CB_updateRecipeReportDelegate(CB_updateRecipeReportCallBackFunction), new object[] {recipe});
                }
                else
                {
                    CB_updateRecipeReportCallBackFunction(recipe);
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating the recipe report: " + e.ToString());
            }
        }

        private void CB_clearRecipeInCrafter()
        {
            try
            {
                if (CB_Ingredient1CraftCount_Textbox.InvokeRequired)
                {
                    CB_Ingredient1CraftCount_Textbox.Invoke(new CB_clearRecipeInCrafterDelegate(CB_clearRecipeInCrafterCallBackFunction), new object[] { });
                }
                else
                {
                    CB_clearRecipeInCrafterCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Clearing Recipe in Crafter: " + e.ToString());
            }
        }

        private void CB_MR_clearRecipeInManager()
        {
            try
            {
                if (CB_Ingredient1CraftCount_Textbox.InvokeRequired)
                {
                    CB_Ingredient1CraftCount_Textbox.Invoke(new CB_MR_clearRecipeInManagerDelegate(CB_MR_clearRecipeInManagerCallBackFunction), new object[] { });
                }
                else
                {
                    CB_MR_clearRecipeInManagerCallBackFunction();
                }
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Clearing Recipe in Manager: " + e.ToString());
            }
        }

        #endregion Update Wrapper Functions
        #endregion Thread Synchronization
        #region Initialization
        private bool doCBInits()
        {
            try
            {
                mCB_NamesBoxes = new List<System.Windows.Forms.TextBox>();
                mCB_InvCountBoxes = new List<System.Windows.Forms.TextBox>();
                mCB_CraftCountBoxes = new List<System.Windows.Forms.TextBox>();

                mCB_NamesBoxes.Add(CB_Ingredient1_Textbox);
                mCB_NamesBoxes.Add(CB_Ingredient2_Textbox);
                mCB_NamesBoxes.Add(CB_Ingredient3_Textbox);
                mCB_NamesBoxes.Add(CB_Ingredient4_Textbox);
                mCB_NamesBoxes.Add(CB_Ingredient5_Textbox);
                mCB_NamesBoxes.Add(CB_Ingredient6_Textbox);
                mCB_NamesBoxes.Add(CB_Ingredient7_Textbox);
                mCB_NamesBoxes.Add(CB_Ingredient8_Textbox);

                mCB_InvCountBoxes.Add(CB_Ingredient1Quan_Textbox);
                mCB_InvCountBoxes.Add(CB_Ingredient2Quan_Textbox);
                mCB_InvCountBoxes.Add(CB_Ingredient3Quan_Textbox);
                mCB_InvCountBoxes.Add(CB_Ingredient4Quan_Textbox);
                mCB_InvCountBoxes.Add(CB_Ingredient5Quan_Textbox);
                mCB_InvCountBoxes.Add(CB_Ingredient6Quan_Textbox);
                mCB_InvCountBoxes.Add(CB_Ingredient7Quan_Textbox);
                mCB_InvCountBoxes.Add(CB_Ingredient8Quan_Textbox);

                mCB_CraftCountBoxes.Add(CB_Ingredient1CraftCount_Textbox);
                mCB_CraftCountBoxes.Add(CB_Ingredient2CraftCount_Textbox);
                mCB_CraftCountBoxes.Add(CB_Ingredient3CraftCount_Textbox);
                mCB_CraftCountBoxes.Add(CB_Ingredient4CraftCount_Textbox);
                mCB_CraftCountBoxes.Add(CB_Ingredient5CraftCount_Textbox);
                mCB_CraftCountBoxes.Add(CB_Ingredient6CraftCount_Textbox);
                mCB_CraftCountBoxes.Add(CB_Ingredient7CraftCount_Textbox);
                mCB_CraftCountBoxes.Add(CB_Ingredient8CraftCount_Textbox);

                mCB_ResultNamesBoxes = new List<System.Windows.Forms.TextBox>();
                mCB_ResultQuantityBoxes = new List<System.Windows.Forms.TextBox>();

                mCB_ResultNamesBoxes.Add(CB_ResultNQ_Textbox);
                mCB_ResultNamesBoxes.Add(CB_ResultHQ1_Textbox);
                mCB_ResultNamesBoxes.Add(CB_ResultHQ2_Textbox);
                mCB_ResultNamesBoxes.Add(CB_ResultHQ3_Textbox);

                mCB_ResultQuantityBoxes.Add(CB_ResultNQQuan_Textbox);
                mCB_ResultQuantityBoxes.Add(CB_ResultHQ1Quan_Textbox);
                mCB_ResultQuantityBoxes.Add(CB_ResultHQ2Quan_Textbox);
                mCB_ResultQuantityBoxes.Add(CB_ResultHQ3Quan_Textbox);

                mCB_MR_IngrNameBoxes = new List<System.Windows.Forms.TextBox>();

                mCB_MR_IngrNameBoxes.Add(CB_MR_Ingredient1_Textbox);
                mCB_MR_IngrNameBoxes.Add(CB_MR_Ingredient2_Textbox);
                mCB_MR_IngrNameBoxes.Add(CB_MR_Ingredient3_Textbox);
                mCB_MR_IngrNameBoxes.Add(CB_MR_Ingredient4_Textbox);
                mCB_MR_IngrNameBoxes.Add(CB_MR_Ingredient5_Textbox);
                mCB_MR_IngrNameBoxes.Add(CB_MR_Ingredient6_Textbox);
                mCB_MR_IngrNameBoxes.Add(CB_MR_Ingredient7_Textbox);
                mCB_MR_IngrNameBoxes.Add(CB_MR_Ingredient8_Textbox);

                mCB_MR_IngrQuantityBoxes = new List<System.Windows.Forms.NumericUpDown>();

                mCB_MR_IngrQuantityBoxes.Add(CB_MR_Ingredient1Quan_UpDown);
                mCB_MR_IngrQuantityBoxes.Add(CB_MR_Ingredient2Quan_UpDown);
                mCB_MR_IngrQuantityBoxes.Add(CB_MR_Ingredient3Quan_UpDown);
                mCB_MR_IngrQuantityBoxes.Add(CB_MR_Ingredient4Quan_UpDown);
                mCB_MR_IngrQuantityBoxes.Add(CB_MR_Ingredient5Quan_UpDown);
                mCB_MR_IngrQuantityBoxes.Add(CB_MR_Ingredient6Quan_UpDown);
                mCB_MR_IngrQuantityBoxes.Add(CB_MR_Ingredient7Quan_UpDown);
                mCB_MR_IngrQuantityBoxes.Add(CB_MR_Ingredient8Quan_UpDown);

                mCB_MR_ResultNameBoxes = new List<System.Windows.Forms.TextBox>();

                mCB_MR_ResultNameBoxes.Add(CB_MR_ResultNQ_Textbox);
                mCB_MR_ResultNameBoxes.Add(CB_MR_ResultHQ1_Textbox);
                mCB_MR_ResultNameBoxes.Add(CB_MR_ResultHQ2_Textbox);
                mCB_MR_ResultNameBoxes.Add(CB_MR_ResultHQ3_Textbox);

                mCB_MR_ResultQuantityBoxes = new List<System.Windows.Forms.NumericUpDown>();

                mCB_MR_ResultQuantityBoxes.Add(CB_MR_ResultNQQuan_UpDown);
                mCB_MR_ResultQuantityBoxes.Add(CB_MR_ResultHQ1Quan_UpDown);
                mCB_MR_ResultQuantityBoxes.Add(CB_MR_ResultHQ2Quan_UpDown);
                mCB_MR_ResultQuantityBoxes.Add(CB_MR_ResultHQ3Quan_UpDown);

                CB_MR_currentRecipe = new Recipe();
                currentMRRecipeTable = "";
                currentMRRecipeFile = "";
                CB_udpateCraftMaxRB(true);
                CB_udpateCraftNumberOfUpDown(10);
                CB_MR_updateCreateRB(true);
                CB_updateSynthRB(true);
                setInventoryFilters();
                rebuildGobbieBag();
                setBagOccupancy();
                setSatchelOccupancy();
                CB_Stats_Tracker = new CraftingTracker(CB_Stats_DGV);
                CB_statsInit(CB_Stats_DGV, CB_Stats_Recipe_Label, CB_Stats_Recipe_Number_Label, CB_Stats_Recipe_Max_Label);
                CB_statsRecipeNumberUpdate(CB_Stats_Tracker.GetCurrentRecipeNumber());
                CB_statsRecipeMaxUpdate(CB_Stats_Tracker.GetMaxRecipeNumber());
                Statics.Settings.Crafter.DonePlaySound = Convert.ToString(UserSettings.GetValue(UserSettings.BOT.CRAFTER, "CrafterPlaySound"));
                CB_updatePlaySoundTB(Statics.Settings.Crafter.DonePlaySound);

              

                CB_MR_clearRecipeInManager();
                CB_clearRecipeInCrafter();

                // Load all the recipes.
                loadRecipes( recipeWWTable, recipeWWFile );
                loadRecipes( recipeSMTable, recipeSMFile );
                loadRecipes( recipeGSTable, recipeGSFile );
                loadRecipes( recipeCCTable, recipeCCFile );
                loadRecipes( recipeLCTable, recipeLCFile );
                loadRecipes( recipeBCTable, recipeBCFile );
                loadRecipes( recipeALTable, recipeALFile );
                loadRecipes( recipeCKTable, recipeCKFile );

                return true;
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Initializing the Crafter tab: " + e.ToString());
                return false;
            }
        }
        #endregion Initialization
        #region Event Handlers
        #region Crafting Guild Buttons
        private void CB_WW_Button_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            { 
                return; 
            }
            LoggingFunctions.Debug("TopCR::CB_WW_Button_CheckedChanged: Changed WW button state to: " + CB_WW_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_WW_Button.Checked == true)
            {
                CB_WW_Button.BackgroundImage = Resources.Woodworking_Inverted;
                CB_SM_Button.Checked = false;
                CB_GS_Button.Checked = false;
                CB_CC_Button.Checked = false;
                CB_LC_Button.Checked = false;
                CB_BC_Button.Checked = false;
                CB_AL_Button.Checked = false;
                CB_CK_Button.Checked = false;
                currentCrafterRecipeTable = recipeWWTable;
                loadRecipes(recipeWWTable, recipeWWFile);
                loadCrafterRecipeSelectionCB(recipeWWTable);
            }
            else
            {
                CB_WW_Button.BackgroundImage = Resources.Woodworking;
            }
        }
        private void CB_SM_Button_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            LoggingFunctions.Debug("TopCR::CB_SM_Button_CheckedChanged: Changed SM button state to: " + CB_SM_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_SM_Button.Checked == true)
            {
                CB_SM_Button.BackgroundImage = Resources.Smithing_Inverted;
                CB_WW_Button.Checked = false;
                CB_GS_Button.Checked = false;
                CB_CC_Button.Checked = false;
                CB_LC_Button.Checked = false;
                CB_BC_Button.Checked = false;
                CB_AL_Button.Checked = false;
                CB_CK_Button.Checked = false;
                currentCrafterRecipeTable = recipeSMTable;
                loadRecipes(recipeSMTable, recipeSMFile);
                loadCrafterRecipeSelectionCB(recipeSMTable);
            }
            else
            {
                CB_SM_Button.BackgroundImage = Resources.Smithing;
            }
        }
        private void CB_GS_Button_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            LoggingFunctions.Debug("TopCR::CB_GS_Button_CheckedChanged: Changed GS button state to: " + CB_GS_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_GS_Button.Checked == true)
            {
                CB_GS_Button.BackgroundImage = Resources.Goldsmithing_Inverted;
                CB_WW_Button.Checked = false;
                CB_SM_Button.Checked = false;
                CB_CC_Button.Checked = false;
                CB_LC_Button.Checked = false;
                CB_BC_Button.Checked = false;
                CB_AL_Button.Checked = false;
                CB_CK_Button.Checked = false;
                currentCrafterRecipeTable = recipeGSTable;
                loadRecipes(recipeGSTable, recipeGSFile);
                loadCrafterRecipeSelectionCB(recipeGSTable);
            }
            else
            {
                CB_GS_Button.BackgroundImage = Resources.Goldsmithing;
            }
        }
        private void CB_CC_Button_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            LoggingFunctions.Debug("TopCR::CB_CC_Button_CheckedChanged: Changed CC button state to: " + CB_CC_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_CC_Button.Checked == true)
            {
                CB_CC_Button.BackgroundImage = Resources.Clothcraft_Inverted;
                CB_WW_Button.Checked = false;
                CB_SM_Button.Checked = false;
                CB_GS_Button.Checked = false;
                CB_LC_Button.Checked = false;
                CB_BC_Button.Checked = false;
                CB_AL_Button.Checked = false;
                CB_CK_Button.Checked = false;
                currentCrafterRecipeTable = recipeCCTable;
                loadRecipes(recipeCCTable, recipeCCFile);
                loadCrafterRecipeSelectionCB(recipeCCTable);
            }
            else
            {
                CB_CC_Button.BackgroundImage = Resources.Clothcraft;
            }
        }
        private void CB_LC_Button_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            LoggingFunctions.Debug("TopCR::CB_LC_Button_CheckedChanged: Changed LC button state to: " + CB_LC_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_LC_Button.Checked == true)
            {
                CB_LC_Button.BackgroundImage = Resources.Leathercraft_Inverted;
                CB_WW_Button.Checked = false;
                CB_SM_Button.Checked = false;
                CB_GS_Button.Checked = false;
                CB_CC_Button.Checked = false;
                CB_BC_Button.Checked = false;
                CB_AL_Button.Checked = false;
                CB_CK_Button.Checked = false;
                currentCrafterRecipeTable = recipeLCTable;
                loadRecipes(recipeLCTable, recipeLCFile);
                loadCrafterRecipeSelectionCB(recipeLCTable);
            }
            else
            {
                CB_LC_Button.BackgroundImage = Resources.Leathercraft;
            }
        }
        private void CB_BC_Button_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            LoggingFunctions.Debug("TopCR::CB_BC_Button_CheckedChanged: Changed BC button state to: " + CB_BC_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_BC_Button.Checked == true)
            {
                CB_BC_Button.BackgroundImage = Resources.Bonecraft_Inverted;
                CB_WW_Button.Checked = false;
                CB_SM_Button.Checked = false;
                CB_GS_Button.Checked = false;
                CB_CC_Button.Checked = false;
                CB_LC_Button.Checked = false;
                CB_AL_Button.Checked = false;
                CB_CK_Button.Checked = false;
                currentCrafterRecipeTable = recipeBCTable;
                loadRecipes(recipeBCTable, recipeBCFile);
                loadCrafterRecipeSelectionCB(recipeBCTable);
            }
            else
            {
                CB_BC_Button.BackgroundImage = Resources.Bonecraft;
            }
        }
        private void CB_AL_Button_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            LoggingFunctions.Debug("TopCR::CB_AL_Button_CheckedChanged: Changed AL button state to: " + CB_AL_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_AL_Button.Checked == true)
            {
                CB_AL_Button.BackgroundImage = Resources.Alchemy_Inverted;
                CB_WW_Button.Checked = false;
                CB_SM_Button.Checked = false;
                CB_GS_Button.Checked = false;
                CB_CC_Button.Checked = false;
                CB_LC_Button.Checked = false;
                CB_BC_Button.Checked = false;
                CB_CK_Button.Checked = false;
                currentCrafterRecipeTable = recipeALTable;
                loadRecipes(recipeALTable, recipeALFile);
                loadCrafterRecipeSelectionCB(recipeALTable);
            }
            else
            {
                CB_AL_Button.BackgroundImage = Resources.Alchemy;
            }
        }
        private void CB_CK_Button_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            LoggingFunctions.Debug("TopCR::CB_CK_Button_CheckedChanged: Changed CK button state to: " + CB_CK_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_CK_Button.Checked == true)
            {
                CB_CK_Button.BackgroundImage = Resources.Cooking_Inverted;
                CB_WW_Button.Checked = false;
                CB_SM_Button.Checked = false;
                CB_GS_Button.Checked = false;
                CB_CC_Button.Checked = false;
                CB_LC_Button.Checked = false;
                CB_BC_Button.Checked = false;
                CB_AL_Button.Checked = false;
                currentCrafterRecipeTable = recipeCKTable;
                loadRecipes(recipeCKTable, recipeCKFile);
                loadCrafterRecipeSelectionCB(recipeCKTable);
            }
            else
            {
                CB_CK_Button.BackgroundImage = Resources.Cooking;
            }
        }
        #endregion Crafting Guild Buttons
        #region Crafter Events
        private void CB_Recipe_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            Recipe selectedRecipe = wrapCurrentCrafterRecipeSelection();
          //  loadRecipeIntoCrafter(selectedRecipe);
            //setCraftNumberOfValue(calculateMaxItems(selectedRecipe));
            setSatchelOccupancy();
            updateQuantities(selectedRecipe, 0, true);
        }
        private void CB_ExtraTimeUpDown_ValueChanged(object sender, EventArgs e)
        {
            Statics.Settings.Crafter.ExtraTime = (int)CB_ExtraTimeUpDown.Value;
        }
        private void CB_MoveInventory_ChkB_CheckedChanged(object sender, EventArgs e)
        {
            CB_MoveInventory = CB_MoveInventory_ChkB.Checked;
        }
        #endregion Crafter Events
        #region Recipe Management Guild Buttons
        private void CB_MR_WW_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_WW_Button_CheckedChanged: Changed MR_WW button state to: " + CB_MR_WW_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_WW_Button.Checked == true)
            {
                CB_MR_WW_Button.BackgroundImage = Resources.Woodworking_Inverted;
                CB_MR_SM_Button.Checked = false;
                CB_MR_GS_Button.Checked = false;
                CB_MR_CC_Button.Checked = false;
                CB_MR_LC_Button.Checked = false;
                CB_MR_BC_Button.Checked = false;
                CB_MR_AL_Button.Checked = false;
                CB_MR_CK_Button.Checked = false;
                CB_MR_Craft_Textbox.Text = "Woodworking";
                CB_MR_currentRecipe.PriCraft = FFXIEnums.CRAFTS.WW;
                setMRCurrentRecipeTableAndFile(FFXIEnums.CRAFTS.WW);
                if (CB_MR_EditExisting_RadioButton.Checked || CB_MR_DeleteExisting_RadioButton.Checked)
                {
                    LoggingFunctions.Debug("TopCR::CB_MR_WW_Button_CheckedChanged: Loading MR recipe selection CB.", LoggingFunctions.DBG_SCOPE.TOP);
                    loadRecipes(recipeWWTable, recipeWWFile);
                    loadMRRecipeSelectionCB(recipeWWTable);
                }
            }
            else
            {
                CB_MR_WW_Button.BackgroundImage = Resources.Woodworking;
            }
        }
        private void CB_MR_SM_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_SM_Button_CheckedChanged: Changed MR_SM button state to: " + CB_MR_SM_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_SM_Button.Checked == true)
            {
                CB_MR_SM_Button.BackgroundImage = Resources.Smithing_Inverted;
                CB_MR_WW_Button.Checked = false;
                CB_MR_GS_Button.Checked = false;
                CB_MR_CC_Button.Checked = false;
                CB_MR_LC_Button.Checked = false;
                CB_MR_BC_Button.Checked = false;
                CB_MR_AL_Button.Checked = false;
                CB_MR_CK_Button.Checked = false;
                CB_MR_Craft_Textbox.Text = "Smithing";
                CB_MR_currentRecipe.PriCraft = FFXIEnums.CRAFTS.SM;
                setMRCurrentRecipeTableAndFile(FFXIEnums.CRAFTS.SM);
                if (CB_MR_EditExisting_RadioButton.Checked || CB_MR_DeleteExisting_RadioButton.Checked)
                {
                    LoggingFunctions.Debug("TopCR::CB_MR_SM_Button_CheckedChanged: Loading MR recipe selection CB.", LoggingFunctions.DBG_SCOPE.TOP);
                    loadRecipes(recipeSMTable, recipeSMFile);
                    loadMRRecipeSelectionCB(recipeSMTable);
                }
            }
            else
            {
                CB_MR_SM_Button.BackgroundImage = Resources.Smithing;
            }
        }
        private void CB_MR_GS_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_GS_Button_CheckedChanged: Changed MR_GS button state to: " + CB_MR_GS_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_GS_Button.Checked == true)
            {
                CB_MR_GS_Button.BackgroundImage = Resources.Goldsmithing_Inverted;
                CB_MR_WW_Button.Checked = false;
                CB_MR_SM_Button.Checked = false;
                CB_MR_CC_Button.Checked = false;
                CB_MR_LC_Button.Checked = false;
                CB_MR_BC_Button.Checked = false;
                CB_MR_AL_Button.Checked = false;
                CB_MR_CK_Button.Checked = false;
                CB_MR_Craft_Textbox.Text = "Goldsmithing";
                CB_MR_currentRecipe.PriCraft = FFXIEnums.CRAFTS.GS;
                setMRCurrentRecipeTableAndFile(FFXIEnums.CRAFTS.GS);
                if (CB_MR_EditExisting_RadioButton.Checked || CB_MR_DeleteExisting_RadioButton.Checked)
                {
                    LoggingFunctions.Debug("TopCR::CB_MR_GS_Button_CheckedChanged: Loading MR recipe selection CB.", LoggingFunctions.DBG_SCOPE.TOP);
                    loadRecipes(recipeGSTable, recipeGSFile);
                    loadMRRecipeSelectionCB(recipeGSTable);
                }
            }
            else
            {
                CB_MR_GS_Button.BackgroundImage = Resources.Goldsmithing;
            }
        }
        private void CB_MR_CC_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_CC_Button_CheckedChanged: Changed MR_CC button state to: " + CB_MR_CC_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_CC_Button.Checked == true)
            {
                CB_MR_CC_Button.BackgroundImage = Resources.Clothcraft_Inverted;
                CB_MR_WW_Button.Checked = false;
                CB_MR_SM_Button.Checked = false;
                CB_MR_GS_Button.Checked = false;
                CB_MR_LC_Button.Checked = false;
                CB_MR_BC_Button.Checked = false;
                CB_MR_AL_Button.Checked = false;
                CB_MR_CK_Button.Checked = false;
                CB_MR_Craft_Textbox.Text = "Clothcraft";
                CB_MR_currentRecipe.PriCraft = FFXIEnums.CRAFTS.CC;
                setMRCurrentRecipeTableAndFile(FFXIEnums.CRAFTS.CC);
                if (CB_MR_EditExisting_RadioButton.Checked || CB_MR_DeleteExisting_RadioButton.Checked)
                {
                    LoggingFunctions.Debug("TopCR::CB_MR_CC_Button_CheckedChanged: Loading MR recipe selection CB.", LoggingFunctions.DBG_SCOPE.TOP);
                    loadRecipes(recipeCCTable, recipeCCFile);
                    loadMRRecipeSelectionCB(recipeCCTable);
                }
            }
            else
            {
                CB_MR_CC_Button.BackgroundImage = Resources.Clothcraft;
            }
        }
        private void CB_MR_LC_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_LC_Button_CheckedChanged: Changed MR_LC button state to: " + CB_MR_LC_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_LC_Button.Checked == true)
            {
                CB_MR_LC_Button.BackgroundImage = Resources.Leathercraft_Inverted;
                CB_MR_WW_Button.Checked = false;
                CB_MR_SM_Button.Checked = false;
                CB_MR_GS_Button.Checked = false;
                CB_MR_CC_Button.Checked = false;
                CB_MR_BC_Button.Checked = false;
                CB_MR_AL_Button.Checked = false;
                CB_MR_CK_Button.Checked = false;
                CB_MR_Craft_Textbox.Text = "Leathercraft";
                CB_MR_currentRecipe.PriCraft = FFXIEnums.CRAFTS.LC;
                setMRCurrentRecipeTableAndFile(FFXIEnums.CRAFTS.LC);
                if (CB_MR_EditExisting_RadioButton.Checked || CB_MR_DeleteExisting_RadioButton.Checked)
                {
                    LoggingFunctions.Debug("TopCR::CB_MR_LC_Button_CheckedChanged: Loading MR recipe selection CB.", LoggingFunctions.DBG_SCOPE.TOP);
                    loadRecipes(recipeLCTable, recipeLCFile);
                    loadMRRecipeSelectionCB(recipeLCTable);
                }
            }
            else
            {
                CB_MR_LC_Button.BackgroundImage = Resources.Leathercraft;
            }
        }
        private void CB_MR_BC_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_BC_Button_CheckedChanged: Changed MR_BC button state to: " + CB_MR_BC_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_BC_Button.Checked == true)
            {
                CB_MR_BC_Button.BackgroundImage = Resources.Bonecraft_Inverted;
                CB_MR_WW_Button.Checked = false;
                CB_MR_SM_Button.Checked = false;
                CB_MR_GS_Button.Checked = false;
                CB_MR_CC_Button.Checked = false;
                CB_MR_LC_Button.Checked = false;
                CB_MR_AL_Button.Checked = false;
                CB_MR_CK_Button.Checked = false;
                CB_MR_Craft_Textbox.Text = "Bonecraft";
                CB_MR_currentRecipe.PriCraft = FFXIEnums.CRAFTS.BC;
                setMRCurrentRecipeTableAndFile(FFXIEnums.CRAFTS.BC);
                if (CB_MR_EditExisting_RadioButton.Checked || CB_MR_DeleteExisting_RadioButton.Checked)
                {
                    LoggingFunctions.Debug("TopCR::CB_MR_BC_Button_CheckedChanged: Loading MR recipe selection CB.", LoggingFunctions.DBG_SCOPE.TOP);
                    loadRecipes(recipeBCTable, recipeBCFile);
                    loadMRRecipeSelectionCB(recipeBCTable);
                    CB_MR_RecipeSelect_ComboBox.Enabled = true;
                }
            }
            else
            {
                CB_MR_BC_Button.BackgroundImage = Resources.Bonecraft;
            }
        }
        private void CB_MR_AL_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_AL_Button_CheckedChanged: Changed MR_AL button state to: " + CB_MR_AL_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_AL_Button.Checked == true)
            {
                CB_MR_AL_Button.BackgroundImage = Resources.Alchemy_Inverted;
                CB_MR_WW_Button.Checked = false;
                CB_MR_SM_Button.Checked = false;
                CB_MR_GS_Button.Checked = false;
                CB_MR_CC_Button.Checked = false;
                CB_MR_LC_Button.Checked = false;
                CB_MR_BC_Button.Checked = false;
                CB_MR_CK_Button.Checked = false;
                CB_MR_Craft_Textbox.Text = "Alchemy";
                CB_MR_currentRecipe.PriCraft = FFXIEnums.CRAFTS.AL;
                setMRCurrentRecipeTableAndFile(FFXIEnums.CRAFTS.AL);
                if (CB_MR_EditExisting_RadioButton.Checked || CB_MR_DeleteExisting_RadioButton.Checked)
                {
                    LoggingFunctions.Debug("TopCR::CB_MR_AL_Button_CheckedChanged: Loading MR recipe selection CB.", LoggingFunctions.DBG_SCOPE.TOP);
                    loadRecipes(recipeALTable, recipeALFile);
                    loadMRRecipeSelectionCB(recipeALTable);
                }
            }
            else
            {
                CB_MR_AL_Button.BackgroundImage = Resources.Alchemy;
            }
        }
        private void CB_MR_CK_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_CK_Button_CheckedChanged: Changed MR_CK button state to: " + CB_MR_CK_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_CK_Button.Checked == true)
            {
                CB_MR_CK_Button.BackgroundImage = Resources.Cooking_Inverted;
                CB_MR_WW_Button.Checked = false;
                CB_MR_SM_Button.Checked = false;
                CB_MR_GS_Button.Checked = false;
                CB_MR_CC_Button.Checked = false;
                CB_MR_LC_Button.Checked = false;
                CB_MR_BC_Button.Checked = false;
                CB_MR_AL_Button.Checked = false;
                CB_MR_Craft_Textbox.Text = "Cooking";
                CB_MR_currentRecipe.PriCraft = FFXIEnums.CRAFTS.CK;
                setMRCurrentRecipeTableAndFile(FFXIEnums.CRAFTS.CK);
                if (CB_MR_EditExisting_RadioButton.Checked || CB_MR_DeleteExisting_RadioButton.Checked)
                {
                    LoggingFunctions.Debug("TopCR::CB_MR_CK_Button_CheckedChanged: Loading MR recipe selection CB.", LoggingFunctions.DBG_SCOPE.TOP);
                    loadRecipes(recipeCKTable, recipeCKFile);
                    loadMRRecipeSelectionCB(recipeCKTable);
                }
            }
            else
            {
                CB_MR_CK_Button.BackgroundImage = Resources.Cooking;
            }
        }
        #endregion Recipe Management Guild Buttons
        #region Recipe Management Crystal Buttons
        private void CB_MR_Fire_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_Fire_Button_CheckedChanged: Changed MR Fire button state to: " + CB_MR_Fire_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_Fire_Button.Checked == true)
            {
                CB_MR_Fire_Button.BackColor = Color.DarkGray;
                CB_MR_Earth_Button.Checked = false;
                CB_MR_Water_Button.Checked = false;
                CB_MR_Wind_Button.Checked = false;
                CB_MR_Ice_Button.Checked = false;
                CB_MR_Lightning_Button.Checked = false;
                CB_MR_Light_Button.Checked = false;
                CB_MR_Dark_Button.Checked = false;
                CB_MR_Crystal_Textbox.Text = "Fire Crystal";
                CB_MR_currentRecipe.Crystal = FFXIEnums.CRYSTALS.FIRE;
            }
            else
            {
                CB_MR_Fire_Button.BackColor = Color.Transparent;
            }
        }
        private void CB_MR_Earth_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_Earth_Button_CheckedChanged: Changed MR Earth button state to: " + CB_MR_Earth_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_Earth_Button.Checked == true)
            {
                CB_MR_Earth_Button.BackColor = Color.DarkGray;
                CB_MR_Fire_Button.Checked = false;
                CB_MR_Water_Button.Checked = false;
                CB_MR_Wind_Button.Checked = false;
                CB_MR_Ice_Button.Checked = false;
                CB_MR_Lightning_Button.Checked = false;
                CB_MR_Light_Button.Checked = false;
                CB_MR_Dark_Button.Checked = false;
                CB_MR_Crystal_Textbox.Text = "Earth Crystal";
                CB_MR_currentRecipe.Crystal = FFXIEnums.CRYSTALS.EARTH;
            }
            else
            {
                CB_MR_Earth_Button.BackColor = Color.Transparent;
            }
        }
        private void CB_MR_Water_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_Water_Button_CheckedChanged: Changed MR Water button state to: " + CB_MR_Water_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_Water_Button.Checked == true)
            {
                CB_MR_Water_Button.BackColor = Color.DarkGray;
                CB_MR_Fire_Button.Checked = false;
                CB_MR_Earth_Button.Checked = false;
                CB_MR_Wind_Button.Checked = false;
                CB_MR_Ice_Button.Checked = false;
                CB_MR_Lightning_Button.Checked = false;
                CB_MR_Light_Button.Checked = false;
                CB_MR_Dark_Button.Checked = false;
                CB_MR_Crystal_Textbox.Text = "Water Crystal";
                CB_MR_currentRecipe.Crystal = FFXIEnums.CRYSTALS.WATER;
            }
            else
            {
                CB_MR_Water_Button.BackColor = Color.Transparent;
            }
        }
        private void CB_MR_Wind_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_Wind_Button_CheckedChanged: Changed MR Wind button state to: " + CB_MR_Wind_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_Wind_Button.Checked == true)
            {
                CB_MR_Wind_Button.BackColor = Color.DarkGray;
                CB_MR_Fire_Button.Checked = false;
                CB_MR_Earth_Button.Checked = false;
                CB_MR_Water_Button.Checked = false;
                CB_MR_Ice_Button.Checked = false;
                CB_MR_Lightning_Button.Checked = false;
                CB_MR_Light_Button.Checked = false;
                CB_MR_Dark_Button.Checked = false;
                CB_MR_Crystal_Textbox.Text = "Wind Crystal";
                CB_MR_currentRecipe.Crystal = FFXIEnums.CRYSTALS.WIND;
            }
            else
            {
                CB_MR_Wind_Button.BackColor = Color.Transparent;
            }
        }
        private void CB_MR_Ice_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_Ice_Button_CheckedChanged: Changed MR Ice button state to: " + CB_MR_Ice_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_Ice_Button.Checked == true)
            {
                CB_MR_Ice_Button.BackColor = Color.DarkGray;
                CB_MR_Fire_Button.Checked = false;
                CB_MR_Earth_Button.Checked = false;
                CB_MR_Water_Button.Checked = false;
                CB_MR_Wind_Button.Checked = false;
                CB_MR_Lightning_Button.Checked = false;
                CB_MR_Light_Button.Checked = false;
                CB_MR_Dark_Button.Checked = false;
                CB_MR_Crystal_Textbox.Text = "Ice Crystal";
                CB_MR_currentRecipe.Crystal = FFXIEnums.CRYSTALS.ICE;
            }
            else
            {
                CB_MR_Ice_Button.BackColor = Color.Transparent;
            }
        }
        private void CB_MR_Lightning_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_Lightning_Button_CheckedChanged: Changed MR Lightning button state to: " + CB_MR_Lightning_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_Lightning_Button.Checked == true)
            {
                CB_MR_Lightning_Button.BackColor = Color.DarkGray;
                CB_MR_Fire_Button.Checked = false;
                CB_MR_Earth_Button.Checked = false;
                CB_MR_Water_Button.Checked = false;
                CB_MR_Wind_Button.Checked = false;
                CB_MR_Ice_Button.Checked = false;
                CB_MR_Light_Button.Checked = false;
                CB_MR_Dark_Button.Checked = false;
                CB_MR_Crystal_Textbox.Text = "Lightng. Crystal";
                CB_MR_currentRecipe.Crystal = FFXIEnums.CRYSTALS.LIGHTNING;
            }
            else
            {
                CB_MR_Lightning_Button.BackColor = Color.Transparent;
            }
        }
        private void CB_MR_Light_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_Light_Button_CheckedChanged: Changed MR Light button state to: " + CB_MR_Light_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_Light_Button.Checked == true)
            {
                CB_MR_Light_Button.BackColor = Color.DarkGray;
                CB_MR_Fire_Button.Checked = false;
                CB_MR_Earth_Button.Checked = false;
                CB_MR_Water_Button.Checked = false;
                CB_MR_Wind_Button.Checked = false;
                CB_MR_Ice_Button.Checked = false;
                CB_MR_Lightning_Button.Checked = false;
                CB_MR_Dark_Button.Checked = false;
                CB_MR_Crystal_Textbox.Text = "Light Crystal";
                CB_MR_currentRecipe.Crystal = FFXIEnums.CRYSTALS.LIGHT;
            }
            else
            {
                CB_MR_Light_Button.BackColor = Color.Transparent;
            }
        }
        private void CB_MR_Dark_Button_CheckedChanged(object sender, EventArgs e)
        {
            LoggingFunctions.Debug("TopCR::CB_MR_Dark_Button_CheckedChanged: Changed MR Dark button state to: " + CB_MR_Dark_Button.Checked + ".", LoggingFunctions.DBG_SCOPE.TOP);
            if (CB_MR_Dark_Button.Checked == true)
            {
                CB_MR_Dark_Button.BackColor = Color.DarkGray;
                CB_MR_Fire_Button.Checked = false;
                CB_MR_Earth_Button.Checked = false;
                CB_MR_Water_Button.Checked = false;
                CB_MR_Wind_Button.Checked = false;
                CB_MR_Ice_Button.Checked = false;
                CB_MR_Lightning_Button.Checked = false;
                CB_MR_Light_Button.Checked = false;
                CB_MR_Crystal_Textbox.Text = "Dark Crystal";
                CB_MR_currentRecipe.Crystal = FFXIEnums.CRYSTALS.DARK;
            }
            else
            {
                CB_MR_Dark_Button.BackColor = Color.Transparent;
            }
        }
        #endregion Recipe Management Crystal Buttons
        #region Recipe Management Textboxes
        private void CB_MR_ResultNQ_Textbox_Click(object sender, EventArgs e)
        {
       //     CB_MR_ResultNQ_Textbox.Text = "";
        }
        private void CB_MR_ResultNQ_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_ResultNQ_Textbox.BackColor = Statics.Fields.Grey;
            ushort id = Things.GetIdFromName(CB_MR_ResultNQ_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_ResultNQ_Textbox.BackColor = Statics.Fields.Red;
            }
        }

        private void CB_MR_ResultHQ1_Textbox_Click(object sender, EventArgs e)
        {
       //     CB_MR_ResultHQ1_Textbox.Text = "";
        }
        private void CB_MR_ResultHQ1_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_ResultHQ1_Textbox.BackColor = Statics.Fields.Grey;
            ushort id = Things.GetIdFromName(CB_MR_ResultHQ1_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_ResultHQ1_Textbox.BackColor = Statics.Fields.Red;
            }
        }

        private void CB_MR_ResultHQ2_Textbox_Click(object sender, EventArgs e)
        {
       //     CB_MR_ResultHQ2_Textbox.Text = "";
        }
        private void CB_MR_ResultHQ2_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_ResultHQ2_Textbox.BackColor = Statics.Fields.Grey;
            ushort id = Things.GetIdFromName(CB_MR_ResultHQ2_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_ResultHQ2_Textbox.BackColor = Statics.Fields.Red;
            }
        }

        private void CB_MR_ResultHQ3_Textbox_Click(object sender, EventArgs e)
        {
       //     CB_MR_ResultHQ3_Textbox.Text = "";
        }
        private void CB_MR_ResultHQ3_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_ResultHQ3_Textbox.BackColor = Statics.Fields.Grey;
            ushort id = Things.GetIdFromName(CB_MR_ResultHQ3_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_ResultHQ3_Textbox.BackColor = Statics.Fields.Red;
            }
        }

        private void CB_MR_Ingredient1_Textbox_Click(object sender, EventArgs e)
        {
       //     CB_MR_Ingredient1_Textbox.Text = "";
        }
        private void CB_MR_Ingredient1_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_Ingredient1_Textbox.BackColor = Statics.Fields.White;
            ushort id = Things.GetIdFromName(CB_MR_Ingredient1_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_Ingredient1_Textbox.BackColor = Statics.Fields.Red;
            }
        }

        private void CB_MR_Ingredient2_Textbox_Click(object sender, EventArgs e)
        {
       //     CB_MR_Ingredient2_Textbox.Text = "";
        }
        private void CB_MR_Ingredient2_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_Ingredient2_Textbox.BackColor = Statics.Fields.White;
            ushort id = Things.GetIdFromName(CB_MR_Ingredient2_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_Ingredient2_Textbox.BackColor = Statics.Fields.Red;
            }
        }

        private void CB_MR_Ingredient3_Textbox_Click(object sender, EventArgs e)
        {
      //      CB_MR_Ingredient3_Textbox.Text = "";
        }
        private void CB_MR_Ingredient3_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_Ingredient3_Textbox.BackColor = Statics.Fields.White;
            ushort id = Things.GetIdFromName(CB_MR_Ingredient3_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_Ingredient3_Textbox.BackColor = Statics.Fields.Red;
            }
        }


        private void CB_MR_Ingredient4_Textbox_Click(object sender, EventArgs e)
        {
       //     CB_MR_Ingredient4_Textbox.Text = "";
        }
        private void CB_MR_Ingredient4_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_Ingredient4_Textbox.BackColor = Statics.Fields.White;
            ushort id = Things.GetIdFromName(CB_MR_Ingredient4_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_Ingredient4_Textbox.BackColor = Statics.Fields.Red;
            }
        }

        private void CB_MR_Ingredient5_Textbox_Click(object sender, EventArgs e)
        {
       //     CB_MR_Ingredient5_Textbox.Text = "";
        }
        private void CB_MR_Ingredient5_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_Ingredient5_Textbox.BackColor = Statics.Fields.White;
            ushort id = Things.GetIdFromName(CB_MR_Ingredient5_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_Ingredient5_Textbox.BackColor = Statics.Fields.Red;
            }
        }

        private void CB_MR_Ingredient6_Textbox_Click(object sender, EventArgs e)
        {
       //     CB_MR_Ingredient6_Textbox.Text = "";
        }
        private void CB_MR_Ingredient6_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_Ingredient6_Textbox.BackColor = Statics.Fields.White;
            ushort id = Things.GetIdFromName(CB_MR_Ingredient6_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_Ingredient6_Textbox.BackColor = Statics.Fields.Red;
            }
        }

        private void CB_MR_Ingredient7_Textbox_Click(object sender, EventArgs e)
        {
      //      CB_MR_Ingredient7_Textbox.Text = "";
        }
        private void CB_MR_Ingredient7_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_Ingredient7_Textbox.BackColor = Statics.Fields.White;
            ushort id = Things.GetIdFromName(CB_MR_Ingredient7_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_Ingredient7_Textbox.BackColor = Statics.Fields.Red;
            }
        }

        private void CB_MR_Ingredient8_Textbox_Click(object sender, EventArgs e)
        {
          // CB_MR_Ingredient8_Textbox.Text = "";
        }
        private void CB_MR_Ingredient8_Textbox_Leave(object sender, EventArgs e)
        {
            CB_MR_Ingredient8_Textbox.BackColor = Statics.Fields.White;
            ushort id = Things.GetIdFromName(CB_MR_Ingredient8_Textbox.Text);
            if (id == Things.invalidID)
            {
                CB_MR_Ingredient8_Textbox.BackColor = Statics.Fields.Red;
            }
        }


        #endregion Recipe Management Textboxes
        #region Radio Buttons
        private void CB_MR_CreateNew_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_MR_CreateNew_RadioButton.Checked == true)
            {
                loadRecipeIntoManager(CB_MR_currentRecipe);
                CB_MR_RecipeSelect_ComboBox.Enabled = false;
            }
        }
        private void CB_MR_EditExisting_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_MR_EditExisting_RadioButton.Checked == true)
            {
                FFXIEnums.CRAFTS selectedCraft = getMRSelectedCraft();
                if (selectedCraft != FFXIEnums.CRAFTS.NONE)
                {
                    loadRecipes(currentMRRecipeTable, currentMRRecipeFile);
                    loadMRRecipeSelectionCB(currentMRRecipeTable);
                    CB_MR_RecipeSelect_ComboBox.Enabled = true;
                }
            }
        }
        private void CB_MR_DeleteExisting_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_MR_DeleteExisting_RadioButton.Checked == true)
            {
                FFXIEnums.CRAFTS selectedCraft = getMRSelectedCraft();
                if (selectedCraft != FFXIEnums.CRAFTS.NONE)
                {
                    loadRecipes(currentMRRecipeTable, currentMRRecipeFile);
                    loadMRRecipeSelectionCB(currentMRRecipeTable);
                    CB_MR_RecipeSelect_ComboBox.Enabled = true;
                }
            }
        }
        private void CB_MR_RecipeSelect_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CB_MR_RecipeSelect_ComboBox.Items.Count > 0)
            {
                loadRecipeIntoManager(wrapCurrentMRRecipeSelection());
            }
        }
        #endregion Radio Buttons
        #region Apply/Reset Buttons
        private void CB_MR_Apply_Button_Click(object sender, EventArgs e)
        {
            // Check whether a craft is selected.
            // Otherwise the recipes aren't loaded and they are replaced with this single recipe.
            if ( CB_MR_Craft_Textbox.Text == "" )
            {
                MessageBox.Show("Please select a craft.");
                return;
            }

            if (CB_MR_Crystal_Textbox.Text == "")
            {
                MessageBox.Show("Please select a crystal.");
                return;
            }

            if (CB_MR_CreateNew_RadioButton.Checked == true)
            {
                List<string> ingr_names = new List<string>();
                    
                ingr_names.Add(CB_MR_Ingredient1_Textbox.Text);
                ingr_names.Add(CB_MR_Ingredient2_Textbox.Text);
                ingr_names.Add(CB_MR_Ingredient3_Textbox.Text);
                ingr_names.Add(CB_MR_Ingredient4_Textbox.Text);
                ingr_names.Add(CB_MR_Ingredient5_Textbox.Text);
                ingr_names.Add(CB_MR_Ingredient6_Textbox.Text);
                ingr_names.Add(CB_MR_Ingredient7_Textbox.Text);
                ingr_names.Add(CB_MR_Ingredient8_Textbox.Text);
                    
                List<byte> ingr_quantities = new List<byte>();

                ingr_quantities.Add((byte)CB_MR_Ingredient1Quan_UpDown.Value);
                ingr_quantities.Add((byte)CB_MR_Ingredient2Quan_UpDown.Value);
                ingr_quantities.Add((byte)CB_MR_Ingredient3Quan_UpDown.Value);
                ingr_quantities.Add((byte)CB_MR_Ingredient4Quan_UpDown.Value);
                ingr_quantities.Add((byte)CB_MR_Ingredient5Quan_UpDown.Value);
                ingr_quantities.Add((byte)CB_MR_Ingredient6Quan_UpDown.Value);
                ingr_quantities.Add((byte)CB_MR_Ingredient7Quan_UpDown.Value);
                ingr_quantities.Add((byte)CB_MR_Ingredient8Quan_UpDown.Value);

                List<string> result_names = new List<string>();

                result_names.Add(CB_MR_ResultNQ_Textbox.Text);
                result_names.Add(CB_MR_ResultHQ1_Textbox.Text);
                result_names.Add(CB_MR_ResultHQ2_Textbox.Text);
                result_names.Add(CB_MR_ResultHQ3_Textbox.Text);

                List<byte> result_quantities = new List<byte>();

                result_quantities.Add((byte)CB_MR_ResultNQQuan_UpDown.Value);
                result_quantities.Add((byte)CB_MR_ResultHQ1Quan_UpDown.Value);
                result_quantities.Add((byte)CB_MR_ResultHQ2Quan_UpDown.Value);
                result_quantities.Add((byte)CB_MR_ResultHQ3Quan_UpDown.Value);

                List<byte> result_verified = new List<byte>();

                result_verified.Add(0);
                result_verified.Add(0);
                result_verified.Add(0);
                result_verified.Add(0);

                Recipe.StatusCode recipe_status = CB_MR_currentRecipe.SetRecipe( getMRSelectedCraft(),
                                                                    Convert.ToByte(CB_MR_Craft_Level_Textbox.Text),
                                                                    FFXIEnums.CRAFTS.NONE,
                                                                    0,
                                                                    FFXIEnums.CRAFTS.NONE,
                                                                    0,
                                                                    getSelectedCrystal(),
                                                                    result_names,
                                                                    result_quantities,
                                                                    result_verified,
                                                                    (CB_MR_Synth_RadioButton.Checked == true) ? Recipe.SYNTH_TYPE.SYNTH : Recipe.SYNTH_TYPE.DESYNTH,
                                                                    ingr_names,
                                                                    ingr_quantities );

                switch( recipe_status )
                {
                    default:
                    case Recipe.StatusCode.INGREDIENT_ID_NOT_FOUND:
                    case Recipe.StatusCode.RESULT_ID_NOT_FOUND:
                    case Recipe.StatusCode.OK:
                        setMRCurrentRecipeTableAndFile(CB_MR_currentRecipe.PriCraft);
                        addRecipe(CB_MR_currentRecipe, currentMRRecipeTable);
                        break;
                    case Recipe.StatusCode.TOO_MANY_INGREDIENTS:
                        MessageBox.Show("Recipe has too many ingredients.");
                        break;
                    case Recipe.StatusCode.TOO_MANY_RESULTS:
                        MessageBox.Show("Resipe has too many results.");
                        break;
                    case Recipe.StatusCode.NO_RESULTS:
                        MessageBox.Show("Please enter a result name and quantity.");
                        break;
                    case Recipe.StatusCode.NO_INGREDIENTS:
                        MessageBox.Show("Please enter an ingredient name and quantity.");
                        break;
                    case Recipe.StatusCode.CRYSTAL_STRING:
                        MessageBox.Show("Please select a crystal.");
                        break;
                    case Recipe.StatusCode.CRAFT_INVALID:
                        MessageBox.Show("Please select a craft.");
                        break;
                }
            }
            else if (CB_MR_EditExisting_RadioButton.Checked == true)
            {
                Statics.Datasets.MainDb.Tables[currentMRRecipeTable].Rows.RemoveAt(CB_MR_RecipeSelect_ComboBox.SelectedIndex);
                wrapCurrentMRRecipeEntry();
                addRecipe(CB_MR_currentRecipe, currentMRRecipeTable);
                saveRecipes(currentMRRecipeTable, currentMRRecipeFile);
                loadMRRecipeSelectionCB(currentMRRecipeTable);
            }
            else if (CB_MR_DeleteExisting_RadioButton.Checked == true)
            {
                Statics.Datasets.MainDb.Tables[currentMRRecipeTable].Rows.RemoveAt(CB_MR_RecipeSelect_ComboBox.SelectedIndex);
                saveRecipes(currentMRRecipeTable, currentMRRecipeFile);
                reloadRecipes(currentMRRecipeTable, currentMRRecipeFile);
                loadMRRecipeSelectionCB(currentMRRecipeTable);
            }
            setCrafterCurrentRecipeTable(getCrafterSelectedCraft());
            loadCrafterRecipeSelectionCB(currentCrafterRecipeTable);
        }
        private void CB_MR_Reset_Button_Click(object sender, EventArgs e)
        {
            CB_MR_currentRecipe = new Recipe();
            loadRecipeIntoManager(CB_MR_currentRecipe);
        }
        #endregion Apply/Reset Buttons
        #region Start & Stop Buttons
        private void CB_Start_Button_Click(object sender, EventArgs e)
        {
            if (Statics.Flags.ProcessState == 0)
            {
                MessageBox.Show("Cannot find pol process of given name. Check name and/or dual box checkbox");
            }
            else
            {
                if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
                {
                    return;
                }
                if ((Crafter1 == null) || (Crafter1.State == Bots.Crafter.CRAFTER_STATE.STOPPED))
                {
                    Recipe loadedRecipe = wrapCurrentCrafterRecipeSelection();
                    if (loadedRecipe.mResults.Count() == 0 )
                    {
                        MessageBox.Show("Please select a recipe first.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    if( !loadedRecipe.IsValid() )
                    {
                        MessageBox.Show("Error getting item ID of an ingredient.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    LoggingFunctions.Debug("TopCR::CB_Start_Button_Click: Starting Crafter.", LoggingFunctions.DBG_SCOPE.TOP);
                    Bots.Crafter.CRAFT_MODE mode = Bots.Crafter.CRAFT_MODE.NUMBER_OF;
                    int nbOf = 1;
                    float skill = 1;
                    setSatchelOccupancy();
                    setBagOccupancy();
                    Containers.RebuildListsMobileOnly();
                    if(CB_CraftNumberOf_RadioButton.Checked == true)
                    {
                        LoggingFunctions.Debug("TopCR::CB_Start_Button_Click: Craft number of RB was checked.", LoggingFunctions.DBG_SCOPE.TOP);
                        mode = Bots.Crafter.CRAFT_MODE.NUMBER_OF;
                        nbOf = (int)CB_CraftNumberOf_UpDown.Value;
                        setBagOccupancy();
                        skill = 120;
                    }
                    else if(CB_CraftMax_RadioButton.Checked == true)
                    {
                        LoggingFunctions.Debug("TopCR::CB_Start_Button_Click: Craft max RB was checked.", LoggingFunctions.DBG_SCOPE.TOP);
                        mode = Bots.Crafter.CRAFT_MODE.MAX;
                        if (CB_MoveInventory == true)
                        {
                            nbOf = loadedRecipe.calculateMaxSynths();
                        }
                        else
                        {
                            nbOf = loadedRecipe.calculateMaxSynths(Containers.Bag);
                        }
                        skill = 120;
                    }
                    else if(CB_CraftUntilSkill_RadioButton.Checked == true)
                    {
                        LoggingFunctions.Debug("TopCR::CB_Start_Button_Click: Craft to skill RB was checked.", LoggingFunctions.DBG_SCOPE.TOP);
                        mode = Bots.Crafter.CRAFT_MODE.UNTIL_SKILL;
                        if (CB_MoveInventory == true)
                        {
                            nbOf = loadedRecipe.calculateMaxSynths();
                        }
                        else
                        {
                            nbOf = loadedRecipe.calculateMaxSynths(Containers.Bag);
                        }
                        skill = Convert.ToSingle(CB_CraftUntilSkill_Textbox.Text);
                    }
                    else
                    {
                        LoggingFunctions.Error("Initializing crafter. No mode was selected.");
                        return;
                    }
                    LoggingFunctions.Debug("TopCR::CB_Start_Button_Click: Crafter start info: *************************", LoggingFunctions.DBG_SCOPE.TOP);
                    LoggingFunctions.Debug("TopCR::CB_Start_Button_Click: Recipe For: " + loadedRecipe.RecipeName + ".", LoggingFunctions.DBG_SCOPE.TOP);
                    LoggingFunctions.Debug("TopCR::CB_Start_Button_Click: Mode: " + mode.ToString() + ".", LoggingFunctions.DBG_SCOPE.TOP);
                    LoggingFunctions.Debug("TopCR::CB_Start_Button_Click: Number Of: " + nbOf + ".", LoggingFunctions.DBG_SCOPE.TOP);
                    LoggingFunctions.Debug("TopCR::CB_Start_Button_Click: Skill: " + skill + ".", LoggingFunctions.DBG_SCOPE.TOP);
                    updateQuantities(loadedRecipe, 0, false);
                    CB_statsAddRecipe(loadedRecipe, CB_Stats_DGV, CB_Stats_Recipe_Label, CB_Stats_Recipe_Number_Label, CB_Stats_Recipe_Max_Label);
                    Crafter1 = new Bots.Crafter(mode, loadedRecipe, nbOf, skill, CB_Start_Button, new Statics.FuncPtrs.TD_Void_String_Color(updateCrafterStartButtonCBF));
                    Crafter1.doInits();
                    Crafter1.CraftDone += new Bots.Crafter.CraftDoneEvent(Crafter1_CraftDone);

                    m_TOP_Thread_crafter = new Thread(new ThreadStart(Crafter1.StartCrafter));
                    m_TOP_Thread_crafter.Name = "CrafterThread";
                    m_TOP_Thread_crafter.IsBackground = true;
                    m_TOP_Thread_crafter.Start();

                    CB_Start_Button.UseMnemonic = true;
                    CB_Start_Button.Text = "&Pause";
                    CB_Start_Button.BackColor = System.Drawing.Color.Yellow;
                }
                else if (Crafter1.State == Bots.Crafter.CRAFTER_STATE.RUNNING)
                {
                    LoggingFunctions.Timestamp("Pausing Crafter");
                    Crafter1.PauseCrafter();
                    CB_Start_Button.UseMnemonic = true;
                    CB_Start_Button.Text = "&Resume";
                    CB_Start_Button.BackColor = System.Drawing.Color.Lime;
                }
                else if ((Crafter1.State == Bots.Crafter.CRAFTER_STATE.PAUSED_PROG)
                     || (Crafter1.State == Bots.Crafter.CRAFTER_STATE.PAUSED_USER))
                {
                    LoggingFunctions.Timestamp("Resuming Crafter");
                    Crafter1.ResumeCrafter();
                    CB_Start_Button.UseMnemonic = true;
                    CB_Start_Button.Text = "&Pause";
                    CB_Start_Button.BackColor = System.Drawing.Color.Yellow;
                }
            }
        }

        private void CB_Stop_Button_Click(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            if (Crafter1 != null)
            {
                LoggingFunctions.Timestamp("Stopping Crafter");
                Crafter1.StopCrafter();
                ALR_previousStateMap[BOT.CRAFTER] = BOT_STATE.STOPPED;
                while (Crafter1.State != Bots.Crafter.CRAFTER_STATE.STOPPED)
                {
                    IocaineFunctions.delay(500);
                }

                Crafter1 = null;
            }
            else
            {
                Containers.Bag.RebuildLists();
            }
            CB_Start_Button.UseMnemonic = true;
            CB_Start_Button.Text = "S&tart";
            CB_Start_Button.BackColor = Statics.Buttons.Green;
        }
        #endregion Start & Stop Buttons
        #region Filter Checkboxes
        private void CB_Inv_Filter_Weapons_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            setInventoryFilters();
            CB_rebuildInventoryLB();
        }
        private void CB_Inv_Filter_Armor_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            setInventoryFilters();
            CB_rebuildInventoryLB();
        }
        private void CB_Inv_Filter_Crystals_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            setInventoryFilters();
            CB_rebuildInventoryLB();
        }
        private void CB_Inv_Filter_Fish_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            setInventoryFilters();
            CB_rebuildInventoryLB();
        }
        private void CB_Inv_Filter_Linkpearl_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            setInventoryFilters();
            CB_rebuildInventoryLB();
        }
        private void CB_Inv_Filter_Misc_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            setInventoryFilters();
            CB_rebuildInventoryLB();
        }
        private void CB_Inv_Filter_Misc2_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if ((Statics.Flags.ProcessState == 0) || !m_TOP_cbInitDone)
            {
                return;
            }
            setInventoryFilters();
            CB_rebuildInventoryLB();
        }
        #endregion Filter Checkboxes
        #region Crafter Updates
        void Crafter1_CraftDone(Recipe recipe, int numberSoFar, RecipeLog.RESULT_TYPE result,
                                List<ushort> lostItem, List<ushort> lostQuan)
        {
            updateQuantities(recipe, numberSoFar, false);
            try
            {
                CB_Stats_Tracker.UpdateStatsLog(recipe, result, lostItem, lostQuan);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating crafter stats log: " + e.ToString());
            }
            try
            {
                CB_statsUpdate(CB_Stats_DGV, CB_Stats_Recipe_Label);
            }
            catch (Exception e)
            {
                LoggingFunctions.Error("Updating crafter stats DGV: " + e.ToString());
            }
        }
        #endregion Crafter Updates
        #region Stats Tracking Recipe Buttons
        private void CB_Stats_Left_Button_Click(object sender, EventArgs e)
        {
            int currRecipeNb = CB_Stats_Tracker.GetCurrentRecipeNumber();
            int maxRecipeNb = CB_Stats_Tracker.GetMaxRecipeNumber();
            if (currRecipeNb > 0)
            {
                CB_Stats_Tracker.DisplayPreviousRecipe(CB_Stats_DGV, CB_Stats_Recipe_Label, CB_Stats_Recipe_Number_Label, CB_Stats_Recipe_Max_Label);
            }
        }

        private void CB_Stats_Right_Button_Click(object sender, EventArgs e)
        {
            int currRecipeNb = CB_Stats_Tracker.GetCurrentRecipeNumber();
            int maxRecipeNb = CB_Stats_Tracker.GetMaxRecipeNumber();
            if (currRecipeNb < maxRecipeNb)
            {
                CB_Stats_Tracker.DisplayNextRecipe(CB_Stats_DGV, CB_Stats_Recipe_Label, CB_Stats_Recipe_Number_Label, CB_Stats_Recipe_Max_Label);
            }
        }
        #endregion Stats Tracking Recipe Buttons
        #region Play Sounds
        private void CB_PlaySound_TB_TextChanged(object sender, EventArgs e)
        {
            Statics.Settings.Crafter.DonePlaySound = CB_PlaySound_TB.Text;
            UserSettings.SetValue(UserSettings.BOT.CRAFTER, "CrafterPlaySound", Statics.Settings.Crafter.DonePlaySound);
        }
        private void CB_PlaySound_Button_Click(object sender, EventArgs e)
        {
            CB_PlaySound_OpenDialog.Filter = "Waveform Audio File (*.wav) |*.wav;";
            CB_PlaySound_OpenDialog.Multiselect = false;
            CB_PlaySound_OpenDialog.ShowDialog();
            if (CB_PlaySound_OpenDialog.FileName != "CB_PlaySound_OpenDialog")
            {
                CB_PlaySound_TB.Text = CB_PlaySound_OpenDialog.FileName;
            }
        }
        #endregion Play Sounds
        #endregion Event Handlers
        #region Utility Functions
        private void reloadRecipes(string tableName, string fileName)
        {
            Statics.Datasets.MainDb.Tables[tableName].Rows.Clear();
            loadRecipes(tableName, fileName);
        }
        private void loadRecipes(string tableName, string fileName)
        {
            if(Directory.Exists(recipeDir))
            {
                if (File.Exists(recipeDir + fileName))
                {
                    try
                    {
                        if (Statics.Datasets.MainDb.Tables[tableName].Rows.Count == 0)
                        {
                            Statics.Datasets.MainDb.Tables[tableName].ReadXml(recipeDir + fileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggingFunctions.Error("Loading file: " + recipeDir + fileName + ": " + ex.ToString());
                    }
                }
            }
        }
        private void saveRecipes(string tableName, string fileName)
        {
            if (!Directory.Exists(recipeDir))
            {
                try
                {
                    Directory.CreateDirectory(recipeDir);
                }
                catch (Exception ex)
                {
                    LoggingFunctions.Error("Creating recipes directory: " + ex.ToString());
                }
            }
            try
            {
                Statics.Datasets.MainDb.Tables[tableName].WriteXml(recipeDir + fileName);
            }
            catch (Exception ex)
            {
                LoggingFunctions.Error("Saving file: " + recipeDir + fileName + ": " + ex.ToString());
            }
        }
        private void addRecipe(Recipe recipeToAdd, string tableToAddTo)
        {
            DataRow row = Statics.Datasets.MainDb.Tables[tableToAddTo].NewRow();

            recipeToAdd.toDataRow( ref row );

            int nbRows = Statics.Datasets.MainDb.Tables[tableToAddTo].Rows.Count;
            bool inserted = false;
            for (int ii = 0; ii < nbRows; ii++)
            {
                if ((Statics.Datasets.MainDb.Tables[tableToAddTo].Rows[ii]["ResultNQ"]).ToString().CompareTo((string)row["ResultNQ"]) > 0)
                {
                    Statics.Datasets.MainDb.Tables[tableToAddTo].Rows.InsertAt(row, ii);
                    inserted = true;
                    break;
                }
            }
            if (!inserted)
            {
                Statics.Datasets.MainDb.Tables[tableToAddTo].Rows.Add(row);
            }
            Statics.Datasets.MainDb.AcceptChanges();
            saveRecipes(currentMRRecipeTable, currentMRRecipeFile);
        }
        private void setCrafterCurrentRecipeTable(FFXIEnums.CRAFTS craft)
        {
            switch (craft)
            {
                case FFXIEnums.CRAFTS.AL:
                    currentCrafterRecipeTable = recipeALTable;
                    break;
                case FFXIEnums.CRAFTS.BC:
                    currentCrafterRecipeTable = recipeBCTable;
                    break;
                case FFXIEnums.CRAFTS.CC:
                    currentCrafterRecipeTable = recipeCCTable;
                    break;
                case FFXIEnums.CRAFTS.CK:
                    currentCrafterRecipeTable = recipeCKTable;
                    break;
                case FFXIEnums.CRAFTS.GS:
                    currentCrafterRecipeTable = recipeGSTable;
                    break;
                case FFXIEnums.CRAFTS.LC:
                    currentCrafterRecipeTable = recipeLCTable;
                    break;
                case FFXIEnums.CRAFTS.SM:
                    currentCrafterRecipeTable = recipeSMTable;
                    break;
                case FFXIEnums.CRAFTS.WW:
                    currentCrafterRecipeTable = recipeWWTable;
                    break;
                default:
                    LoggingFunctions.Error("Unknown craft type, cannot find appropriate table name: Craft type: " + craft.ToString());
                    break;
            }
        }
        private void setMRCurrentRecipeTableAndFile(FFXIEnums.CRAFTS craft)
        {
            switch (craft)
            {
                case FFXIEnums.CRAFTS.AL:
                    currentMRRecipeTable = recipeALTable;
                    currentMRRecipeFile = recipeALFile;
                    break;
                case FFXIEnums.CRAFTS.BC:
                    currentMRRecipeTable = recipeBCTable;
                    currentMRRecipeFile = recipeBCFile;
                    break;
                case FFXIEnums.CRAFTS.CC:
                    currentMRRecipeTable = recipeCCTable;
                    currentMRRecipeFile = recipeCCFile;
                    break;
                case FFXIEnums.CRAFTS.CK:
                    currentMRRecipeTable = recipeCKTable;
                    currentMRRecipeFile = recipeCKFile;
                    break;
                case FFXIEnums.CRAFTS.GS:
                    currentMRRecipeTable = recipeGSTable;
                    currentMRRecipeFile = recipeGSFile;
                    break;
                case FFXIEnums.CRAFTS.LC:
                    currentMRRecipeTable = recipeLCTable;
                    currentMRRecipeFile = recipeLCFile;
                    break;
                case FFXIEnums.CRAFTS.SM:
                    currentMRRecipeTable = recipeSMTable;
                    currentMRRecipeFile = recipeSMFile;
                    break;
                case FFXIEnums.CRAFTS.WW:
                    currentMRRecipeTable = recipeWWTable;
                    currentMRRecipeFile = recipeWWFile;
                    break;
                default:
                    LoggingFunctions.Error("Unknown craft type, cannot find appropriate table name: Craft type: " + craft.ToString());
                    break;
            }
        }
        private void loadCrafterRecipeSelectionCB(string tableName)
        {
            if (tableName == null)
            {
                return;
            }
            else if (tableName == "")
            {
                return;
            }
            CB_Recipe_ComboBox.Items.Clear();
            CB_Recipe_ComboBox.Text = "";
            int nbRows = Statics.Datasets.MainDb.Tables[tableName].Rows.Count;
            for (int ii = 0; ii < nbRows; ii++)
            {
                CB_Recipe_ComboBox.Items.Add((string)Recipe.getNameFromDataRow(Statics.Datasets.MainDb.Tables[tableName].Rows[ii]));
            }
            if (CB_Recipe_ComboBox.Items.Count > 0)
            {
                CB_Recipe_ComboBox.SelectedIndex = 0;
            }
            else
            {
                // Clear the crafter
                clearRecipeInCrafter();
            }
        }

        private void loadMRRecipeSelectionCB(string tableName)
        {
            CB_MR_RecipeSelect_ComboBox.Items.Clear();
            CB_MR_RecipeSelect_ComboBox.Text = "";
            int nbRows = Statics.Datasets.MainDb.Tables[tableName].Rows.Count;
            for(int ii=0; ii < nbRows; ii++) 
            {
                
                CB_MR_RecipeSelect_ComboBox.Items.Add((string)Recipe.getNameFromDataRow(Statics.Datasets.MainDb.Tables[tableName].Rows[ii]));
            }
            if (CB_MR_RecipeSelect_ComboBox.Items.Count > 0)
            {
                CB_MR_RecipeSelect_ComboBox.SelectedIndex = 0;
            }
            else
            {
                clearRecipeInManager();
            }
        }
        private void clearRecipeInManager()
        {
            foreach (System.Windows.Forms.TextBox textbox in mCB_MR_ResultNameBoxes)
            {
                textbox.Text = "";
                textbox.BackColor = Statics.Fields.Grey;
            }

            foreach (System.Windows.Forms.NumericUpDown numericupdown in mCB_MR_ResultQuantityBoxes)
            {
                numericupdown.Value = 0;
                numericupdown.BackColor = Statics.Fields.Grey;
            }

            foreach (System.Windows.Forms.TextBox textbox in mCB_MR_IngrNameBoxes)
            {
                textbox.Text = "";
                textbox.BackColor = Statics.Fields.White;
            }

            foreach (System.Windows.Forms.NumericUpDown numericupdown in mCB_MR_IngrQuantityBoxes)
            {
                numericupdown.Value = 0;
                numericupdown.BackColor = Statics.Fields.White;
            }

            CB_MR_Craft_Level_Textbox.Text = "0";
        }
        private void loadRecipeIntoManager(Recipe recipeToLoad)
        {
            LoggingFunctions.Debug("TopCR::loadRecipeIntoManager: Trying to load recipe " + recipeToLoad.RecipeName + " into manager.", LoggingFunctions.DBG_SCOPE.TOP);

            clearRecipeInManager();
            
            selectMRCraftButton(recipeToLoad.PriCraft);
            selectMRCrystalButton(recipeToLoad.Crystal);

            CB_MR_Craft_Level_Textbox.Text = recipeToLoad.PriCraftSkill.ToString();

            for (int index = 0; index < mCB_MR_IngrNameBoxes.Count() && index < mCB_MR_IngrQuantityBoxes.Count() && index < recipeToLoad.mIngredients.Count(); index++)
            {
                mCB_MR_IngrNameBoxes[index].Text = recipeToLoad.mIngredients[index].Name;
                mCB_MR_IngrQuantityBoxes[index].Value = recipeToLoad.mIngredients[index].Quantity;
                if (recipeToLoad.mIngredients[index].ID == Things.invalidID)
                {
                    mCB_MR_IngrNameBoxes[index].BackColor = Statics.Fields.Red;
                    mCB_MR_IngrQuantityBoxes[index].BackColor = Statics.Fields.Red;
                }
            }


            for (int index = 0; index < mCB_MR_ResultNameBoxes.Count() && index < mCB_MR_ResultQuantityBoxes.Count() && index < recipeToLoad.mResults.Count(); index++)
            {
                mCB_MR_ResultNameBoxes[index].Text = recipeToLoad.mResults[index].Name;
                mCB_MR_ResultQuantityBoxes[index].Value = recipeToLoad.mResults[index].Quantity;

                if (recipeToLoad.mResults[index].Verified == 1)
                {
                    mCB_MR_ResultNameBoxes[index].BackColor = Statics.Fields.Green;
                }
                else
                {
                    mCB_MR_ResultNameBoxes[index].BackColor = Statics.Fields.Grey;
                }
                if (recipeToLoad.mResults[index].ID == Things.invalidID)
                {
                    mCB_MR_ResultNameBoxes[index].BackColor = Statics.Fields.Red;
                }
            }
            
            CB_MR_Synth_RadioButton.Checked = (recipeToLoad.Type == Recipe.SYNTH_TYPE.SYNTH);
            CB_MR_DeSynth_RadioButton.Checked = (recipeToLoad.Type == Recipe.SYNTH_TYPE.DESYNTH);
        }
        private void clearRecipeInCrafter()
        {
            CB_Crystal_Textbox.Text = "";
            CB_CrystalQuan_Textbox.Text = "";

            foreach (System.Windows.Forms.TextBox textbox in mCB_ResultNamesBoxes)
            {
                textbox.Text = "";
                textbox.BackColor = Statics.Fields.Grey;
            }

            foreach (System.Windows.Forms.TextBox textbox in mCB_ResultQuantityBoxes)
            {
                textbox.Text = "";
                textbox.BackColor = Statics.Fields.Grey;
            }

            foreach (System.Windows.Forms.TextBox textbox in mCB_NamesBoxes)
            {
                textbox.Text = "None";
                textbox.BackColor = Statics.Fields.Grey;
            }

            foreach (System.Windows.Forms.TextBox textbox in mCB_InvCountBoxes)
            {
                textbox.Text = "";
                textbox.BackColor = Statics.Fields.Grey;
            }

            foreach (System.Windows.Forms.TextBox textbox in mCB_CraftCountBoxes)
            {
                textbox.Text = "";
                textbox.BackColor = Statics.Fields.Grey;
            }

        }
        private void loadRecipeIntoCrafter(Recipe recipeToLoad)
        {
            Recipe.CCraftingReport report = recipeToLoad.CreateReport(Containers.Bag);

            clearRecipeInCrafter();
            CB_Crystal_Textbox.Text = report.mCrystalCount.Name;
            CB_CrystalQuan_Textbox.Text = report.mCrystalCount.InventoryCount.ToString();

            for (int index = 0; index < recipeToLoad.mResults.Count() && index < mCB_ResultNamesBoxes.Count() && index < mCB_ResultQuantityBoxes.Count() ; index++)
            {
                mCB_ResultNamesBoxes[index].Text = recipeToLoad.mResults[index].Name;
                mCB_ResultNamesBoxes[index].BackColor = (recipeToLoad.mResults[index].Verified == 1) ? Statics.Fields.Green : Statics.Fields.Red;
                mCB_ResultQuantityBoxes[index].Text = recipeToLoad.mResults[index].Quantity.ToString();
                mCB_ResultQuantityBoxes[index].BackColor = (recipeToLoad.mResults[index].Verified == 1) ? Statics.Fields.Green : Statics.Fields.Red;
            }

            for (int index = 0; index < report.mCraftingCount.Count() && index < mCB_NamesBoxes.Count() && index < mCB_InvCountBoxes.Count() && index < mCB_CraftCountBoxes.Count() ; index++)
            {
                mCB_NamesBoxes[index].Text = report.mCraftingCount[index].Name;
                mCB_NamesBoxes[index].BackColor = (report.mCraftingCount[index].CraftingCount == 0) ? Statics.Fields.Red : Statics.Fields.Blue;
                mCB_InvCountBoxes[index].Text = report.mCraftingCount[index].InventoryCount.ToString();
                mCB_InvCountBoxes[index].BackColor = (report.mCraftingCount[index].CraftingCount == 0) ? Statics.Fields.Red : Statics.Fields.Blue;
                mCB_CraftCountBoxes[index].Text = report.mCraftingCount[index].CraftingCount.ToString();
                mCB_CraftCountBoxes[index].BackColor = (report.mCraftingCount[index].CraftingCount == 0) ? Statics.Fields.Red : Statics.Fields.Blue;
            }
             
            CB_Synth_RadioButton.Checked = (recipeToLoad.Type == Recipe.SYNTH_TYPE.SYNTH);
            CB_DeSynth_RadioButton.Checked = (recipeToLoad.Type == Recipe.SYNTH_TYPE.DESYNTH);
        }
        private Recipe wrapCurrentCrafterRecipeSelection()
        {
            Recipe wrappedRecipe = new Recipe();
            DataRow recipeRow;
            if (CB_Recipe_ComboBox.Items.Count > 0)
            {
                LoggingFunctions.Debug("TopCR::wrapCurrentCrafterRecipeSelection: Selected index is: " + CB_Recipe_ComboBox.SelectedIndex.ToString() + ".", LoggingFunctions.DBG_SCOPE.TOP);
                recipeRow = Statics.Datasets.MainDb.Tables[currentCrafterRecipeTable].Rows[CB_Recipe_ComboBox.SelectedIndex];


                bool set_recipe_succeed = wrappedRecipe.fromDataRow(recipeRow);

                if (set_recipe_succeed == false)
                {
                    Logging.LoggingFunctions.Debug("TopCR::wrapCurrentCrafterRecipeSelection: Error parsing recipe.", LoggingFunctions.DBG_SCOPE.TOP);
                }
            }
            return wrappedRecipe;
        }
        private Recipe wrapCurrentMRRecipeSelection()
        {
            Recipe wrappedRecipe = new Recipe();
            DataRow recipeRow;
            if (CB_MR_RecipeSelect_ComboBox.Items.Count > 0)
            {
                LoggingFunctions.Debug("TopCR::wrapCurrentMRRecipeSelection: Selected index is: " + CB_MR_RecipeSelect_ComboBox.SelectedIndex.ToString() + ".", LoggingFunctions.DBG_SCOPE.TOP);
                recipeRow = Statics.Datasets.MainDb.Tables[currentMRRecipeTable].Rows[CB_MR_RecipeSelect_ComboBox.SelectedIndex];

                if (wrappedRecipe.fromDataRow(recipeRow) == false)
                {
                    Logging.LoggingFunctions.Debug("TopCR::wrapCurrentMRRecipeSelection: Error parsing recipe row.", LoggingFunctions.DBG_SCOPE.TOP);
                }
            }
            return wrappedRecipe;
        }
        private void wrapCurrentMRRecipeEntry()
        {

            List<string> ingr_names = new List<string>();
            List<byte> ingr_quantity = new List<byte>();

            ingr_names.Add((string)CB_MR_Ingredient1_Textbox.Text);
            ingr_names.Add((string)CB_MR_Ingredient2_Textbox.Text);
            ingr_names.Add((string)CB_MR_Ingredient3_Textbox.Text);
            ingr_names.Add((string)CB_MR_Ingredient4_Textbox.Text);
            ingr_names.Add((string)CB_MR_Ingredient5_Textbox.Text);
            ingr_names.Add((string)CB_MR_Ingredient6_Textbox.Text);
            ingr_names.Add((string)CB_MR_Ingredient7_Textbox.Text);
            ingr_names.Add((string)CB_MR_Ingredient8_Textbox.Text);

            ingr_quantity.Add((byte)CB_MR_Ingredient1Quan_UpDown.Value);
            ingr_quantity.Add((byte)CB_MR_Ingredient2Quan_UpDown.Value);
            ingr_quantity.Add((byte)CB_MR_Ingredient3Quan_UpDown.Value);
            ingr_quantity.Add((byte)CB_MR_Ingredient4Quan_UpDown.Value);
            ingr_quantity.Add((byte)CB_MR_Ingredient5Quan_UpDown.Value);
            ingr_quantity.Add((byte)CB_MR_Ingredient6Quan_UpDown.Value);
            ingr_quantity.Add((byte)CB_MR_Ingredient7Quan_UpDown.Value);
            ingr_quantity.Add((byte)CB_MR_Ingredient8Quan_UpDown.Value);

            List<string> result_names = new List<string>();
            List<byte> result_quantity = new List<byte>();
            List<byte> result_verified = new List<byte>();

            result_names.Add((string)CB_MR_ResultNQ_Textbox.Text);
            result_names.Add((string)CB_MR_ResultHQ1_Textbox.Text);
            result_names.Add((string)CB_MR_ResultHQ2_Textbox.Text);
            result_names.Add((string)CB_MR_ResultHQ3_Textbox.Text);

            result_quantity.Add((byte)CB_MR_ResultNQQuan_UpDown.Value);
            result_quantity.Add((byte)CB_MR_ResultHQ1Quan_UpDown.Value);
            result_quantity.Add((byte)CB_MR_ResultHQ2Quan_UpDown.Value);
            result_quantity.Add((byte)CB_MR_ResultHQ3Quan_UpDown.Value);

            result_verified.Add((byte)0);
            result_verified.Add((byte)0);
            result_verified.Add((byte)0);
            result_verified.Add((byte)0);

            Recipe.StatusCode set_recipe_succeed = CB_MR_currentRecipe.SetRecipe( getMRSelectedCraft(),
                                                                     Convert.ToByte(CB_MR_Craft_Level_Textbox.Text),
                                                                     FFXIEnums.CRAFTS.NONE,
                                                                     0,
                                                                     FFXIEnums.CRAFTS.NONE,
                                                                     0,
                                                                     getSelectedCrystal(),
                                                                     result_names,
                                                                     result_quantity,
                                                                     result_verified,
                                                                     ((CB_MR_Synth_RadioButton.Checked == true) ? Recipe.SYNTH_TYPE.SYNTH : Recipe.SYNTH_TYPE.DESYNTH),
                                                                     ingr_names,
                                                                     ingr_quantity );

            if (set_recipe_succeed != Recipe.StatusCode.OK)
            {
                Logging.LoggingFunctions.Debug("TopCR::wrapCurrentMRRecipeEntry: Error parsing Recipe.", LoggingFunctions.DBG_SCOPE.TOP);
            }
        }
        private void selectMRCraftButton(FFXIEnums.CRAFTS craft)
        {
            switch (craft)
            {
                case FFXIEnums.CRAFTS.AL:
                    CB_MR_AL_Button.Checked = true;
                    break;
                case FFXIEnums.CRAFTS.BC:
                    CB_MR_BC_Button.Checked = true;
                    break;
                case FFXIEnums.CRAFTS.CC:
                    CB_MR_CC_Button.Checked = true;
                    break;
                case FFXIEnums.CRAFTS.CK:
                    CB_MR_CK_Button.Checked = true;
                    break;
                case FFXIEnums.CRAFTS.GS:
                    CB_MR_GS_Button.Checked = true;
                    break;
                case FFXIEnums.CRAFTS.LC:
                    CB_MR_LC_Button.Checked = true;
                    break;
                case FFXIEnums.CRAFTS.SM:
                    CB_MR_SM_Button.Checked = true;
                    break;
                case FFXIEnums.CRAFTS.WW:
                    CB_MR_WW_Button.Checked = true;
                    break;
                default:
                    break;
            }
        }
        private void selectMRCrystalButton(FFXIEnums.CRYSTALS crystal)
        {
            switch (crystal)
            {
                case FFXIEnums.CRYSTALS.DARK:
                    CB_MR_Dark_Button.Checked = true;
                    break;
                case FFXIEnums.CRYSTALS.EARTH:
                    CB_MR_Earth_Button.Checked = true;
                    break;
                case FFXIEnums.CRYSTALS.FIRE:
                    CB_MR_Fire_Button.Checked = true;
                    break;
                case FFXIEnums.CRYSTALS.ICE:
                    CB_MR_Ice_Button.Checked = true;
                    break;
                case FFXIEnums.CRYSTALS.LIGHT:
                    CB_MR_Light_Button.Checked = true;
                    break;
                case FFXIEnums.CRYSTALS.LIGHTNING:
                    CB_MR_Lightning_Button.Checked = true;
                    break;
                case FFXIEnums.CRYSTALS.WATER:
                    CB_MR_Water_Button.Checked = true;
                    break;
                case FFXIEnums.CRYSTALS.WIND:
                    CB_MR_Wind_Button.Checked = true;
                    break;
                default:
                    break;
            }
        }
        private FFXIEnums.CRYSTALS getSelectedCrystal()
        {
            if (CB_MR_Dark_Button.Checked == true)
            {
                return FFXIEnums.CRYSTALS.DARK;
            }
            else if (CB_MR_Earth_Button.Checked == true)
            {
                return FFXIEnums.CRYSTALS.EARTH;
            }
            else if (CB_MR_Fire_Button.Checked == true)
            {
                return FFXIEnums.CRYSTALS.FIRE;
            }
            else if (CB_MR_Ice_Button.Checked == true)
            {
                return FFXIEnums.CRYSTALS.ICE;
            }
            else if (CB_MR_Light_Button.Checked == true)
            {
                return FFXIEnums.CRYSTALS.LIGHT;
            }
            else if (CB_MR_Lightning_Button.Checked == true)
            {
                return FFXIEnums.CRYSTALS.LIGHTNING;
            }
            else if (CB_MR_Water_Button.Checked == true)
            {
                return FFXIEnums.CRYSTALS.WATER;
            }
            else if (CB_MR_Wind_Button.Checked == true)
            {
                return FFXIEnums.CRYSTALS.WIND;
            }
            else
            {
                return FFXIEnums.CRYSTALS.UNKNOWN;
            }
        }
        private FFXIEnums.CRAFTS getCrafterSelectedCraft()
        {
            if (CB_AL_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.AL;
            }
            else if (CB_BC_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.BC;
            }
            else if (CB_CC_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.CC;
            }
            else if (CB_CK_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.CK;
            }
            else if (CB_GS_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.GS;
            }
            else if (CB_LC_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.LC;
            }
            else if (CB_SM_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.SM;
            }
            else if (CB_WW_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.WW;
            }
            else
            {
                return FFXIEnums.CRAFTS.NONE;
            }
        }
        private FFXIEnums.CRAFTS getMRSelectedCraft()
        {
            if (CB_MR_AL_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.AL;
            }
            else if (CB_MR_BC_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.BC;
            }
            else if (CB_MR_CC_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.CC;
            }
            else if (CB_MR_CK_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.CK;
            }
            else if (CB_MR_GS_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.GS;
            }
            else if (CB_MR_LC_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.LC;
            }
            else if (CB_MR_SM_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.SM;
            }
            else if (CB_MR_WW_Button.Checked == true)
            {
                return FFXIEnums.CRAFTS.WW;
            }
            else
            {
                return FFXIEnums.CRAFTS.NONE;
            }
        }
        private void rebuildGobbieBag()
        {
            Containers.Bag.RebuildLists();
            CB_rebuildInventoryLB();
        }
        private void rebuildInventoryListbox()
        {
            List<Item> inventory = Containers.Bag.GetSummaryItemList();
            List<ushort> inventoryQuan = Containers.Bag.GetSummaryItemQuanList();
            CB_Inventory_Listbox.BeginUpdate();
            CB_Inventory_Listbox.Items.Clear();
            int nbItems = inventory.Count;
            for (int ii = 0; ii < nbItems; ii++)
            {
                if (!CB_InventoryFilterList.Contains(inventory[ii].Type))
                {
                    string textToAdd = "(" + inventoryQuan[ii].ToString() + ")";
                    textToAdd = textToAdd.PadRight(7);
                    textToAdd += inventory[ii].Name;
                    CB_Inventory_Listbox.Items.Add(textToAdd);
                }
            }
            CB_Inventory_Listbox.EndUpdate();
        }
        private void setInventoryFilters()
        {
            if (CB_InventoryFilterList == null)
            {
                CB_InventoryFilterList = new List<Item.ITEM_TYPE>();
            }
            CB_InventoryFilterList.Clear();
            if (CB_Inv_Filter_Armor_Checkbox.Checked)
            {
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.ARMOR);
            }
            if (CB_Inv_Filter_Weapons_Checkbox.Checked)
            {
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.WEAPON);
            }
            if (CB_Inv_Filter_Crystals_Checkbox.Checked)
            {
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.CRYSTALS);
            }
            if (CB_Inv_Filter_Fish_Checkbox.Checked)
            {
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.FISH);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.MISC_KEY);
            }
            if (CB_Inv_Filter_Linkpearl_Checkbox.Checked)
            {
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.LINKPEARL);
            }
            if (CB_Inv_Filter_Misc_Checkbox.Checked)
            {
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.ASSAULT_ITEMS);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.CHOCO_ITEMS_A);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.CHOCO_ITEMS_B);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.CODEX);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.FLOWER_POT);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.FURNISHING);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.MANNEQUIN);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.MISC_TESTIMONY);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.MOG_MARBLE);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.SEEDS);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.SOUL_PLATE);
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.SOUL_REFLECTOR);
            }
            if (CB_Inv_Filter_Misc2_Checkbox.Checked)
            {
                CB_InventoryFilterList.Add(Item.ITEM_TYPE.MISC_TEMP_ITEMS);
            }
        }
        private void updateQuantities(Recipe recipe, int currentTotal, bool setToMax)
        {
            rebuildGobbieBag();
            setBagOccupancy();
            CB_updateRecipeReport(recipe);
            int maxLeft = 0;
            if(CB_MoveInventory == true)
            {
                maxLeft = recipe.calculateMaxSynths();
            }
            else
            {
                maxLeft = recipe.calculateMaxSynths(Containers.Bag);
            }
            int maxPossible = maxLeft + currentTotal;
            if (CB_CraftNumberOf_RadioButton.Checked == true)
            {
                //if ((CB_CraftNumberOf_UpDown.Value > maxPossible) || (Crafter1 == null))
                if ((CB_CraftNumberOf_UpDown.Value > maxPossible) || setToMax)
                {
                    CB_CraftNumberOf_UpDown.Value = maxPossible;
                }
                CB_udpateCraftMaxTB(maxPossible.ToString());
                if (Crafter1 != null)
                {
                    Crafter1.SetMaxCount((int)CB_CraftNumberOf_UpDown.Value);
                }
            }
            else if (CB_CraftMax_RadioButton.Checked || CB_CraftUntilSkill_RadioButton.Checked)
            {
                CB_udpateCraftMaxTB(maxPossible.ToString());
                if (Crafter1 != null)
                {
                    Crafter1.SetMaxCount(maxPossible);
                }
            }
        }
        private void setCraftNumberOfValue(int number)
        {
            CB_CraftNumberOf_UpDown.Value = number;
        }
        private void setBagOccupancy()
        {
            CB_updateBagCountCurrent(MemReads.Self.Inventory.get_bag_occupancy());
            CB_updateBagCountMax(MemReads.Self.Inventory.get_max_bag());
        }
        private void setSatchelOccupancy()
        {
            CB_updateSatchelCountCurrent(MemReads.Self.Inventory.get_satchel_occupancy());
            CB_updateSatchelCountMax(MemReads.Self.Inventory.get_max_satchel());
        }
        #endregion Utility Functions
    }
}