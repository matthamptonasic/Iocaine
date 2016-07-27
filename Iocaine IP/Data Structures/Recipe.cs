using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Iocaine2.Logging;
using Iocaine2.Inventory;
using Iocaine2.Data.Client;

namespace Iocaine2.Data.Structures
{
    public class Recipe
    {
        #region Enums
        public enum SYNTH_TYPE
        {
            SYNTH = 0,
            DESYNTH = 1
        }
        public enum RESULT_TYPE
        {
            RESULT_NQ = 0,
            RESULT_HQ1 = 1,
            RESULT_HQ2 = 2,
            RESULT_HQ3 = 3,
            RESULT_UNK = 5
        }
        public enum StatusCode
        {
            OK,
            TOO_MANY_INGREDIENTS,
            TOO_MANY_RESULTS,
            INGREDIENT_ID_NOT_FOUND,
            RESULT_ID_NOT_FOUND,
            NO_RESULTS,
            NO_INGREDIENTS,
            CRYSTAL_STRING,
            CRAFT_INVALID
        }
        #endregion Enums
        #region Data Row Headers
        private static String[] mDataRowIngredientName = { "Ingredient1", "Ingredient2", "Ingredient3", "Ingredient4", "Ingredient5", "Ingredient6", "Ingredient7", "Ingredient8" };
        private static String[] mDataRowIngredientQuantity = { "Ingredient1Quan","Ingredient2Quan","Ingredient3Quan","Ingredient4Quan","Ingredient5Quan","Ingredient6Quan","Ingredient7Quan","Ingredient8Quan"};
        private static String[] mDataRowResultName = { "ResultNQ", "ResultHQ1", "ResultHQ2", "ResultHQ3" };
        private static String[] mDataRowResultQuantity = { "ResultNQQuan", "ResultHQ1Quan", "ResultHQ2Quan", "ResultHQ3Quan" };
        private static String[] mDataRowResultVerified = { "ResultNQVerified", "ResultHQ1Verified", "ResultHQ2Verified", "ResultHQ3Verified" };
        private static String[] mDataRowCraftName = { "PriCraft", "SecCraft", "TerCraft" };
        private static String[] mDataRowCraftSkill = { "PriCraftSkill", "SecCraftSkill", "TerCraftSkill" };
        private static String mDataRowCrystal = "Crystal";
        private static String mDataRowType = "Type";
        #endregion Data Row Headers
        #region Sub-Classes
        #region class CIngredient
        /// <summary>
        /// Represents a recipe ingredient including the quantity required.
        /// </summary>
        public class CIngredient
        {
            #region Constructors
            public CIngredient()
            {
                mName = "";
                mID = 0;
                mQuantity = 0;
            }
            public CIngredient(String name, ushort id, byte quantity)
            {
                mName = name;
                mID = id;
                mQuantity = quantity;
            }
            #endregion Constructors
            #region Member Variables
            private String mName;
            private ushort mID;
            private byte mQuantity;
            #endregion Member Variables
            #region Member Function
            /// <summary>
            /// Gets the number of this ingredient in the given container.
            /// </summary>
            /// <param name="container">Which container to check.</param>
            /// <returns>The number of this item.</returns>
            public ushort getItemQuantity(ItemContainer container)
            {
                LoggingFunctions.Debug("Obtaining item "+ mID +" in " + container.GetType().ToString(), LoggingFunctions.DBG_SCOPE.CRAFTER);
 
                List<Item> bagSummary = container.GetSummaryItemList();
                List<ushort> bagSumQuan = container.GetSummaryItemQuanList();
                int bagCount = bagSummary.Count;

                ushort itemsFound = 0;
                for (int ii = 0; ii < bagCount; ii++)
                {
                    if (bagSummary[ii].ItemID == mID)
                    {
                        itemsFound += bagSumQuan[ii];
                    }
                }
                return itemsFound;
            }
            /// <summary>
            /// Checks the given item name against the singular and plural chat log strings for this ingredient.
            /// Also checks that the passed quantity is the same as the required quantity of this ingredient.
            /// </summary>
            /// <param name="name">Item name to check against.</param>
            /// <param name="quantity">Quantity to check against.</param>
            /// <returns>Returns true if both the quantity matches as well as the name to either the singular or plural log strings.</returns>
            public bool compare(String name, ushort quantity)
            {
                String logNameS = Things.GetLogNameSFromId(mID);
                String logNameP = Things.GetLogNamePFromId(mID);
                
                LoggingFunctions.Debug("Looking for sng log name: \"" + logNameS + "\" or plural log name: \"" + logNameP, LoggingFunctions.DBG_SCOPE.CRAFTER);
                if (( name  == logNameS) || (name == logNameP))
                {
                    if( mQuantity == quantity )
                    {
                        return true;
                    }
                }
                return false;
            }
            #endregion MemberFunction
            #region Member Accessors
            public String Name
            {
                get
                {
                    return mName;
                }
                set
                {
                    mName = value;
                }
            }
            public ushort ID
            {
                get
                {
                    return mID;
                }
                set
                {
                    mID = value;
                }
            }
            public byte Quantity
            {
                get
                {
                    return mQuantity;
                }
                set
                {
                    mQuantity = value;
                }
            }
            #endregion Member Accessors
        };
        #endregion class CIngredient
        #region class CResult
        public class CResult : CIngredient
        {
            public CResult()
            {
                mVerified = 0;
            }
            public CResult( String name, ushort id, byte quantity, byte verified ) : base( name, id, quantity )
            {
                mVerified = verified;
            }
            private byte mVerified;
            public byte Verified
            {
                get
                {
                    return mVerified;
                }
                set
                {
                    mVerified = value;
                }
            }
        }
        #endregion class CResult
        #region class CCraftingCount
        /// <summary>
        /// Represents an ingredient, required quantity, as well as inventory and craft counts.
        /// </summary>
        public class CCraftingCount : CIngredient
        {
            public CCraftingCount()
            {
                mCraftCount = 0;
            }
            public CCraftingCount(String name, ushort id, byte quantity, ushort count, ushort inventorycount):base(name, id, quantity)
            {
                mInventoryCount = inventorycount;
                mCraftCount = count;
            }
            private ushort mCraftCount;
            private ushort mInventoryCount;
            public ushort CraftingCount
            {
                get
                {
                    return mCraftCount;
                }
                set
                {
                    mCraftCount = value;
                }
            }
            public ushort InventoryCount
            {
                get
                {
                    return mInventoryCount;
                }
                set
                {
                    mInventoryCount = value;
                }
            }
        }
        #endregion CCraftingCount
        #region class CCraftingReport
        /// <summary>
        /// Represents a full recipe including max craft counts for each ingredent, crystal, and cluster.
        /// </summary>
        public class CCraftingReport
        {
            public List<CCraftingCount> mCraftingCount;
            public CCraftingCount mCrystalCount;
            public CCraftingCount mClusterCount;

            public CCraftingReport()
            {
                mCraftingCount = new List<CCraftingCount>();
                mCrystalCount = new CCraftingCount();
                mClusterCount = new CCraftingCount();
            }
        }
        #endregion CCraftingReport
        #endregion Sub-Classes
        #region Member Variables
        #region Private Members
        private FFXIEnums.CRAFTS priCraft;
        private byte priCraftSkill;
        private FFXIEnums.CRAFTS secCraft;
        private byte secCraftSkill;
        private FFXIEnums.CRAFTS terCraft;
        private byte terCraftSkill;
        private FFXIEnums.CRYSTALS crystal;
        private const uint MAX_NUMBER_OF_RESULTS_PER_RECIPE = 4;
        public List<CResult> mResults;
        private SYNTH_TYPE type;
        private const uint MAX_NUMBER_OF_INGREDIENTS_PER_RECIPE = 8;
        #endregion Private Members
        #region Public Members
        #region Craft Skills
        public FFXIEnums.CRAFTS PriCraft
        {
            get
            {
                return priCraft;
            }
            set
            {
                priCraft = value;
            }
        }
        public String PriCraftString
        {
            get
            {
                return getCraftString(priCraft);
            }
        }
        public byte PriCraftSkill
        {
            get
            {
                return priCraftSkill;
            }
            set
            {
                priCraftSkill = value;
            }
        }
        public FFXIEnums.CRAFTS SecCraft
        {
            get
            {
                return secCraft;
            }
            set
            {
                secCraft = value;
            }
        }
        public String SecCraftString
        {
            get
            {
                return getCraftString(secCraft);
            }
        }
        public byte SecCraftSkill
        {
            get
            {
                return secCraftSkill;
            }
            set
            {
                secCraftSkill = value;
            }
        }
        public FFXIEnums.CRAFTS TerCraft
        {
            get
            {
                return terCraft;
            }
            set
            {
                terCraft = value;
            }
        }
        public String TerCraftString
        {
            get
            {
                return getCraftString(terCraft);
            }
        }
        public byte TerCraftSkill
        {
            get
            {
                return terCraftSkill;
            }
            set
            {
                terCraftSkill = value;
            }
        }
        #endregion Craft Skills
        public List<CIngredient> mIngredients;
        public FFXIEnums.CRYSTALS Crystal
        {
            get
            {
                return crystal;
            }
            set
            {
                crystal = value;
            }
        }
        public String CrystalString
        {
            get
            {
                return getCrystalString(crystal);
            }
        }
        public ushort CrystalId
        {
            get
            {
                return getCrystalId();
            }
        }
        /// <summary>
        /// Type meaning 0: Synthesis, 1: Desynthesis
        /// </summary>
        public SYNTH_TYPE Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public String TypeString
        {
            get
            {
                return getTypeString(type);
            }
        }
        public String RecipeName
        {
            get
            {
                if (type == SYNTH_TYPE.DESYNTH && mIngredients.Count() > 0)
                {
                    return mIngredients[0].Name + " (Desynth)";
                }
                return mResults[0].Name;
            }
        }
        #endregion Public Members
        #endregion Member Variables
        #region Constructors
        public Recipe()
        {
            mResults = new List<CResult>();
            mIngredients = new List<CIngredient>();

            // Add one fake result, to act as name placeholder.
            mResults.Add( new CResult( "None", 0, 0, 0 ));

            priCraft = FFXIEnums.CRAFTS.NONE;
            priCraftSkill = 0;
            secCraft = FFXIEnums.CRAFTS.NONE;
            secCraftSkill = 0;
            terCraft = FFXIEnums.CRAFTS.NONE;
            terCraftSkill = 0;
            crystal = FFXIEnums.CRYSTALS.UNKNOWN;
            type = SYNTH_TYPE.SYNTH;
        }
        #endregion Constructors
        #region Utility Functions
        /// <summary>
        /// Checks for proper ingredient and result counts as well as ingredient quantity != 0.
        /// </summary>
        /// <returns>Returns true if no issues are found.</returns>
        public bool IsValid()
        {
            if (mIngredients.Count() < 1 || mIngredients.Count() > MAX_NUMBER_OF_INGREDIENTS_PER_RECIPE)
            {
                return false;
            }

            if (mResults.Count() < 1 || mResults.Count() > MAX_NUMBER_OF_RESULTS_PER_RECIPE)
            {
                return false;
            }

            foreach( CIngredient ingredient in mIngredients ) 
            {
                if (ingredient.ID == 0 && ingredient.Quantity != 0)
                {
                    return false;
                }
            }
            return true;
        }
        public StatusCode SetRecipe(FFXIEnums.CRAFTS iPriCraft, 
                        byte iPriCraftSkill,
                        FFXIEnums.CRAFTS iSecCraft, 
                        byte iSecCraftSkill,
                        FFXIEnums.CRAFTS iTerCraft, 
                        byte iTerCraftSkill,
                        FFXIEnums.CRYSTALS iCrystal,
                        List<String> iResults, 
                        List<byte> iResultsQuan, 
                        List<byte> iResultsVerified,
                        SYNTH_TYPE iType,
                        List<String> iIngredients,
                        List<byte> iQuantities)
        {
            StatusCode return_value = StatusCode.OK;
            // Build a new ingredient list
            if( iIngredients.Count() < MAX_NUMBER_OF_INGREDIENTS_PER_RECIPE || iQuantities.Count() < MAX_NUMBER_OF_INGREDIENTS_PER_RECIPE )
            {
                Logging.LoggingFunctions.Debug( "SetRecipe: iIngredients or iQuantities invalid length", LoggingFunctions.DBG_SCOPE.CRAFTER);
                return StatusCode.TOO_MANY_INGREDIENTS;
            }

            if( iResults.Count() < MAX_NUMBER_OF_RESULTS_PER_RECIPE || 
                iResultsQuan.Count() < MAX_NUMBER_OF_RESULTS_PER_RECIPE || 
                iResultsVerified.Count() < MAX_NUMBER_OF_RESULTS_PER_RECIPE )
            {
                Logging.LoggingFunctions.Debug( "SetRecipe: iResults or iResultsQuan or iResultsVerified invalid length", LoggingFunctions.DBG_SCOPE.CRAFTER);
                return StatusCode.TOO_MANY_RESULTS;
            }

            List<CIngredient> new_ingredient_list = new List<CIngredient>();

            for (int index = 0; index < MAX_NUMBER_OF_INGREDIENTS_PER_RECIPE; index++)
            {
                byte quantity = iQuantities[index];

                if (quantity > 0)
                {
                    ushort id = Things.GetIdFromName(iIngredients[index]);
                    if (id == Things.invalidID)
                    {
                        Logging.LoggingFunctions.Debug("SetRecipe: Ingredient not found (" + iIngredients[index] + ")", LoggingFunctions.DBG_SCOPE.CRAFTER);
                        return_value = StatusCode.INGREDIENT_ID_NOT_FOUND;
                    }
                    else
                    {
                        // A stack has a minimum size of 1. Even if it is defined as 0 (which puts us in an infinite loop).
                        byte stacksize = Math.Max(Things.GetStackSizeFromId(id), (byte)1);
                        while (quantity > stacksize)
                        {
                            new_ingredient_list.Add(new CIngredient(iIngredients[index], id, stacksize));
                            quantity -= stacksize;
                        }
                    }
                    new_ingredient_list.Add(new CIngredient(iIngredients[index], id, quantity));
                }
            }
            
            if( new_ingredient_list.Count() > MAX_NUMBER_OF_INGREDIENTS_PER_RECIPE  ) 
            {
                Logging.LoggingFunctions.Debug( "SetRecipe: More than 8 ingredients after destacking: " + new_ingredient_list.Count(), LoggingFunctions.DBG_SCOPE.CRAFTER);
                return StatusCode.TOO_MANY_INGREDIENTS;
            }

            if (new_ingredient_list.Count() < 1)
            {
                Logging.LoggingFunctions.Debug("SetRecipe: No ingredients after destacking: " + new_ingredient_list.Count(), LoggingFunctions.DBG_SCOPE.CRAFTER);
                return StatusCode.NO_INGREDIENTS;
            }

            List<CResult> new_result_list = new List<CResult>();

            for (int index = 0; index < MAX_NUMBER_OF_RESULTS_PER_RECIPE ; index++)
            {
                if( iResultsQuan[ index ] > 0 )
                {
                    ushort id = Things.GetIdFromName( iResults[ index ] );
                    if( id == Things.invalidID )
                    {
                        Logging.LoggingFunctions.Debug( "SetRecipe: Result not found (" + iResults[index] +")", LoggingFunctions.DBG_SCOPE.CRAFTER);
                        return_value = StatusCode.RESULT_ID_NOT_FOUND;
                    }
                    new_result_list.Add(new CResult(iResults[index], id, iResultsQuan[index], iResultsVerified[index]));
                }
            }

            if( new_result_list.Count() < 1 )
            {
                Logging.LoggingFunctions.Debug( "SetRecipe: No Results set.", LoggingFunctions.DBG_SCOPE.CRAFTER);
                return StatusCode.NO_RESULTS;
            }

            type = iType;

            priCraft = iPriCraft;
            priCraftSkill = iPriCraftSkill;
            secCraft = iSecCraft;
            secCraftSkill = iSecCraftSkill;
            terCraft = iTerCraft;
            terCraftSkill = iTerCraftSkill;
            crystal = iCrystal;

            // Copy the ingredients list
            mIngredients.Clear();
            mIngredients = new_ingredient_list;

            // Copy all the results;
            mResults.Clear();
            mResults = new_result_list;

            return return_value;
        }
        public bool fromDataRow(System.Data.DataRow row)
        {
            List<String> ingr_names = new List<String>();
            List<byte> ingr_quantity = new List<byte>();
           
            foreach( String rowname in mDataRowIngredientName )
            {
                ingr_names.Add((String)row[rowname]);
            }

            foreach( String rowname in mDataRowIngredientQuantity )
            {
                ingr_quantity.Add((byte)row[rowname]);
            }

            List<String> result_names = new List<String>();
            List<byte> result_quantity = new List<byte>();
            List<byte> result_verified = new List<byte>();

            foreach( String rowname in mDataRowResultName )
            {
                result_names.Add((String)row[rowname]);
            }

            foreach( String rowname in mDataRowResultQuantity )
            {
                result_quantity.Add((byte)row[rowname]);
            }

            foreach( String rowname in mDataRowResultVerified )
            {
                result_verified.Add((byte)row[rowname]);
            }

            return SetRecipe((FFXIEnums.CRAFTS)(Convert.ToByte(row[mDataRowCraftName[0]])),
                                (byte)row[mDataRowCraftSkill[0]],
                                (FFXIEnums.CRAFTS)(Convert.ToByte(row[mDataRowCraftName[1]])),
                                (byte)row[mDataRowCraftSkill[0]],
                                (FFXIEnums.CRAFTS)(Convert.ToByte(row[mDataRowCraftName[2]])),
                                (byte)row[mDataRowCraftSkill[0]],
                                (FFXIEnums.CRYSTALS)(Convert.ToByte(row[mDataRowCrystal])),
                                result_names,
                                result_quantity,
                                result_verified,
                                ((byte)row[mDataRowType] == 0?Recipe.SYNTH_TYPE.SYNTH:Recipe.SYNTH_TYPE.DESYNTH),
                                ingr_names,
                                ingr_quantity) == Recipe.StatusCode.OK;
        }
        public bool toDataRow(ref System.Data.DataRow row)
        {
            row[mDataRowCraftName[0]] = PriCraft;
            row[mDataRowCraftSkill[0]] = PriCraftSkill;
            row[mDataRowCraftName[1]] = SecCraft;
            row[mDataRowCraftSkill[1]] = SecCraftSkill;
            row[mDataRowCraftName[2]] = TerCraft;
            row[mDataRowCraftSkill[2]] = TerCraftSkill;
            row[mDataRowCrystal] = Crystal;
                        
            foreach (String rowname in mDataRowResultName)
            {
                row[rowname] = "";
            }
            foreach (String rowname in mDataRowResultQuantity)
            {
                row[rowname] = 0;
            }
            foreach (String rowname in mDataRowResultVerified)
            {
                row[rowname] = 0;
            }

            foreach (CResult result in mResults)
            {
                int index = mResults.IndexOf(result);
                row[mDataRowResultName[index]] = result.Name;
                row[mDataRowResultQuantity[index]] = result.Quantity;
                row[mDataRowResultVerified[index]] = result.Verified;
            }

            row[mDataRowType] = Type;

            foreach (String rowname in mDataRowIngredientName)
            {
                row[rowname] = "";
            }
            foreach (String rowname in mDataRowIngredientQuantity)
            {
                row[rowname] = 0;
            }

            foreach (CIngredient ingredient in mIngredients)
            {
                int index = mIngredients.IndexOf(ingredient);
                row[mDataRowIngredientName[index]] = ingredient.Name;
                row[mDataRowIngredientQuantity[index]] = ingredient.Quantity;
            }
            
            return true;
        }
        public static String getNameFromDataRow(System.Data.DataRow row)
        {
            SYNTH_TYPE synth_type = ((byte)row[mDataRowType] == 0) ? SYNTH_TYPE.SYNTH:SYNTH_TYPE.DESYNTH;
            String name = System.String.Empty;
            switch( synth_type )
            {
                case SYNTH_TYPE.SYNTH:
                    name = (String)row[mDataRowResultName[0]];
                    break;
                case SYNTH_TYPE.DESYNTH:
                    name = (String)row[mDataRowIngredientName[0]] + " (Desynth)";
                    break;
            }
            
            return name + " (" + ((byte)row[mDataRowCraftSkill[0]]).ToString() + ")";
        }
        public CCraftingReport CreateReport( ItemContainer container )
        {
            CCraftingReport report = new CCraftingReport();

            LoggingFunctions.Debug("Creating report in " + container.GetType().ToString(), LoggingFunctions.DBG_SCOPE.CRAFTER);
            LoggingFunctions.Debug("Recipe to calc items:" + RecipeName, LoggingFunctions.DBG_SCOPE.CRAFTER);
            foreach( CIngredient ingredient in mIngredients )
            {
                LoggingFunctions.Debug("Ing: " + ingredient.Name + " (" + ingredient.ID + ") * " + ingredient.Quantity, LoggingFunctions.DBG_SCOPE.CRAFTER);
            }

            List<Item> bagSummary = container.GetSummaryItemList();
            List<ushort> bagSumQuan = container.GetSummaryItemQuanList();
            int bagCount = bagSummary.Count;

            String crystalString = getCrystalString(crystal);
            ushort crystalsFound = 0;
            for (int ii = 0; ii < bagCount; ii++)
            {
                if (bagSummary[ii].Name == crystalString)
                {
                    crystalsFound += bagSumQuan[ii];
                }
            }

            report.mCrystalCount.Name = crystalString;
            report.mCrystalCount.ID = Things.GetIdFromName(crystalString);
            report.mCrystalCount.Quantity = (byte)1;
            report.mCrystalCount.InventoryCount = crystalsFound;


            String clusterString = getClusterString(crystal);
            ushort clustersFound = 0;
            for (int ii = 0; ii < bagCount; ii++)
            {
                if (bagSummary[ii].Name == clusterString)
                {
                    clustersFound += bagSumQuan[ii];
                }
            }

            report.mClusterCount.Name = clusterString;
            report.mClusterCount.ID = Things.GetIdFromName(clusterString);
            report.mClusterCount.Quantity = (byte)1;
            report.mClusterCount.InventoryCount = clustersFound;



            Dictionary<ushort, ushort> aggregatedIngredientList = new Dictionary<ushort, ushort>();
  
            foreach( CIngredient ingredient in mIngredients ) 
            {
                // Ingredients with 0 items are of no interest to us.
                if( ingredient.Quantity > 0 )
                {
                    ushort currentcount = 0;
                    if (aggregatedIngredientList.ContainsKey(ingredient.ID) == true)
                    {
                        currentcount = (ushort)aggregatedIngredientList[ingredient.ID];
                    }
                    currentcount += ingredient.Quantity;
                    aggregatedIngredientList[ingredient.ID] = currentcount;
                }
            }



            foreach (ushort key in aggregatedIngredientList.Keys)
            {
                CCraftingCount count = new CCraftingCount();

                foreach (CIngredient ingredient in mIngredients)
                {
                    if (ingredient.ID == key)
                    {
                        count.Name = ingredient.Name;
                        count.ID = ingredient.ID;
                        count.Quantity = ingredient.Quantity;
                        break;
                    }
                }
                int bagindex = 0;
                for (bagindex = 0; bagindex < bagCount; bagindex++)
                {
                    if (bagSummary[bagindex].ItemID == key)
                    {
                        count.CraftingCount = (ushort)(bagSumQuan[bagindex] / aggregatedIngredientList[key]);
                        count.InventoryCount = bagSumQuan[bagindex];
                        break;
                    }
                }

                report.mCraftingCount.Add(count);
            }

            return report;
        }
        /// <summary>
        /// Calculates the maximum number of successful synthesis attempts possible for all mobile containers.
        /// </summary>
        /// <returns>Maximum number of synthesis attempts assuming no lost/broken materials/crystals.</returns>
        public ushort calculateMaxSynths()
        {
            return calculateMaxSynths(Containers.Bag, true, false, null);
        }
        /// <summary>
        /// Calculates the maximum number of successful synthesis attempts possible for the given container.
        /// </summary>
        /// <param name="container">Container to examine.</param>
        /// <returns>Maximum number of synthesis attempts assuming no lost/broken materials/crystals.</returns>
        public ushort calculateMaxSynths(ItemContainer container)
        {
            return calculateMaxSynths(container, false, false, null);
        }
        /// <summary>
        /// Calculates the maximum number of successful synthesis attempts possible for the given container.
        /// </summary>
        /// <param name="container">Container to examine.</param>
        /// <param name="checkAllMobile">If true, ignores the container passed and calculates based on ALL mobile containers.</param>
        /// <param name="ignoreCrystal">If true, crystals will not be used in the calculation of max synthesis attempts.</param>
        /// <param name="ignoreIngredients">List of ingredients that should be ignored when calculating max synthesis attempts.</param>
        /// <returns>Maximum number of synthesis attempts assuming no lost/broken materials/crystals.</returns>
        public ushort calculateMaxSynths(ItemContainer container, bool checkAllMobile, bool ignoreCrystal, List<CIngredient> ignoreIngredients, bool ignoreClusters = false)
        {
            if(ignoreIngredients == null)
            {
                ignoreIngredients = new List<CIngredient>();
            }
            LoggingFunctions.Debug("Recipe::calculateMaxSynths: Recalculating max items in " + container.GetType().ToString(), LoggingFunctions.DBG_SCOPE.CRAFTER);
            LoggingFunctions.Debug("Recipe::calculateMaxSynths: Recipe to calc items:" + mResults[0].Name, LoggingFunctions.DBG_SCOPE.CRAFTER);
            foreach( CIngredient ingredient in mIngredients )
            {
                LoggingFunctions.Debug("Ing: " + ingredient.Name + " (" + ingredient.ID + ") * " + ingredient.Quantity, LoggingFunctions.DBG_SCOPE.CRAFTER);
            }

            List<Item> bagSummary = null;
            List<ushort> bagSumQuan = null;
            if(checkAllMobile == false)
            {
                bagSummary = container.GetSummaryItemList();
                bagSumQuan = container.GetSummaryItemQuanList();
                LoggingFunctions.Debug("Recipe::calculateMaxSynths: Only checking the " + container.StorageType.ToString() + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
            }
            else
            {
                bagSummary = Containers.SummaryItemListMobile;
                bagSumQuan = Containers.SummaryItemQuanListMobile;
                LoggingFunctions.Debug("Recipe::calculateMaxSynths: Checking ALL mobile containers.", LoggingFunctions.DBG_SCOPE.CRAFTER);
            }
            int bagCount = bagSummary.Count;

            String clusterString = getClusterString(crystal);
            ushort crystalsFound = 0;
            ushort clustersFound = 0;
            for (int ii = 0; ii < bagCount; ii++)
            {
                if (bagSummary[ii].Name == CrystalString)
                {
                    crystalsFound += bagSumQuan[ii];
                }
                if (bagSummary[ii].Name == clusterString)
                {
                    clustersFound += bagSumQuan[ii];
                }
            }
            // #cry=0 ignCry  #clu=0  ignClu
            //  F       F       F       F       have both,                                      break
            //  F       F       F       T       have crystals,                                  break
            //  F       F       T       F       have crystals,                                  break
            //  F       F       T       T       have crystals,                                  break
            //  F       T       F       F       we have clusters,                               break
            //  F       T       F       T       don't care about either, return 0
            //  F       T       T       F       only care about clusters, no clusters, return 0
            //  F       T       T       T       don't care about either, return 0
            //  T       F       F       F       we have clusters,                               break
            //  T       F       F       T       only care about crystals, no crystals, return 0
            //  T       F       T       F       don't have either, return 0
            //  T       F       T       T       only care about crystals, no crystals, return 0
            //  T       T       F       F       we have clusters,                               break
            //  T       T       F       T       don't care about either, return 0
            //  T       T       T       F       only care about clusters, no clusters, return 0
            //  T       T       T       T       don't care about either, return 0
            if ((ignoreCrystal && ignoreClusters)
                || (ignoreCrystal && (clustersFound == 0))
                || ((crystalsFound == 0) && ignoreClusters)
                || (crystalsFound == 0) && (clustersFound == 0))
            {
                string msg = "Recipe::calculateMaxSynths: Found no ";
                if ((crystalsFound == 0) && (ignoreCrystal == false) && (clustersFound == 0) && (ignoreClusters == false))
                {
                    msg += "crystals or clusters.";
                }
                else if ((crystalsFound == 0) && ignoreClusters)
                {
                    msg += "crystals.";
                }
                else
                {
                    msg += "clusters.";
                }
                LoggingFunctions.Debug(msg, LoggingFunctions.DBG_SCOPE.CRAFTER);
                return 0;
            }

            LoggingFunctions.Debug("Recipe::calculateMaxSynths: crystalsFound: '" + crystalsFound + "', clustersFound: '" + clustersFound + "'.", LoggingFunctions.DBG_SCOPE.CRAFTER);

            // Aggregrate all the ingredients. Some ingredients don't stack and some recipes require multiple of them.
            Dictionary<ushort, ushort> aggregatedIngredientList = new Dictionary<ushort, ushort>();
  
            foreach( CIngredient ingredient in mIngredients ) 
            {
                // Ingredients with 0 items are of no interest to us.
                if( ingredient.Quantity > 0 )
                {
                    // This bit takes into account if we have multiple of the same ingredients because they
                    // are not stackable. For instance:
                    // Ingredient #1 = Iron Ore x 1
                    // Ingredient #2 = Iron Ore x 1
                    // The 2nd time around the dictionary value would become 2.
                    ushort currentcount = 0;
                    if (aggregatedIngredientList.ContainsKey(ingredient.ID) == true)
                    {
                        currentcount = aggregatedIngredientList[ingredient.ID];
                    }
                    currentcount += ingredient.Quantity;
                    aggregatedIngredientList[ingredient.ID] = currentcount;
                }
            }

            // Then determine how much of each ID we have and then determine the maximum amount we can craft
            ushort maximumamount = (ushort)(crystalsFound + clustersFound * 12);
            foreach (ushort key in aggregatedIngredientList.Keys)
            {
                int bagindex = 0;
                foreach (CIngredient localIngredient in ignoreIngredients)
                {
                    if (key == localIngredient.ID)
                    {
                        LoggingFunctions.Debug("Recipe::calculateMaxSynths: Ignoring item " + localIngredient.Name + " (" + localIngredient.ID + ").", LoggingFunctions.DBG_SCOPE.CRAFTER);
                        continue;
                    }
                }

                for (bagindex = 0; bagindex < bagCount; bagindex++)
                {
                    if (bagSummary[bagindex].ItemID == key)
                    {
                    
                        maximumamount = Math.Min(maximumamount, (ushort)(bagSumQuan[bagindex] / aggregatedIngredientList[key]));
                        break;
                    }
                }
                if (bagindex == bagCount)
                {
                    LoggingFunctions.Debug("Recipe::calculateMaxSynths: Item Not found ID: ( " + key + " )", LoggingFunctions.DBG_SCOPE.CRAFTER);
                    return 0;
                }
            }

            LoggingFunctions.Debug("Recipe::calculateMaxSynths: Maximum Craftable Amount (pre): " + maximumamount + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);

            return maximumamount;
        }
        /// <summary>
        /// Calculates the ration between the number of ingredients required to the results produced in terms of inventory slots used.
        /// </summary>
        /// <returns>Returns a Single that represents the ration of ingredient slots to result slots.</returns>
        public Single calculateIngredientToResultRatio()
        {
            Single aggregateSlots = 0f;
            Single aggregateResultSlots = 0f;
            byte currentStackSize = 0;
            foreach(CIngredient ingredient in mIngredients)
            {
                if(ingredient.Quantity == 0)
                {
                    continue;
                }
                currentStackSize = Things.GetStackSizeFromId(ingredient.ID);
                aggregateSlots += ((Single)ingredient.Quantity / (Single)currentStackSize);
            }

            foreach(CResult result in mResults)
            {
                if(result.Quantity == 0)
                {
                    continue;
                }
                currentStackSize = Things.GetStackSizeFromId(result.ID);
                aggregateResultSlots += ((Single)result.Quantity / (Single)currentStackSize);
            }

            return (aggregateSlots / aggregateResultSlots);
        }
        /// <summary>
        /// Creates a list of required ingredients that are NOT in our bag.
        /// </summary>
        /// <param name="oIngredients">The output list of lacking ingredients. Null/empty check is performed.</param>
        /// <returns>Returns true if crystals are lacking.</returns>
        public bool findIngredientDeficiencies(ref List<CIngredient> oIngredients)
        {
            if(oIngredients == null)
            {
                oIngredients = new List<CIngredient>();
            }
            else
            {
                oIngredients.Clear();
            }

            foreach(CIngredient ingredient in mIngredients)
            {
                LoggingFunctions.Debug("Recipe::findIngredientDeficiencies: Checking if our bag has enough " + ingredient.Name + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                ushort nbInBag = Containers.Bag.GetItemQuan(ingredient.ID);
                if(nbInBag < ingredient.Quantity)
                {
                    if(!oIngredients.Contains(ingredient))
                    {
                        LoggingFunctions.Debug("Recipe::findIngredientDeficiencies: Do not have enough " + ingredient.Name + ".", LoggingFunctions.DBG_SCOPE.CRAFTER);
                        oIngredients.Add(ingredient);
                    }
                }
            }

            return !Containers.Bag.Contains((short)Crystal);
        }
        /// <summary>
        /// Extracts lists of names, ID's, and quantities from the list of ingredient objects.
        /// </summary>
        /// <param name="names">Output list of item name strings.</param>
        /// <param name="IDs">Output list of item ID's.</param>
        /// <param name="quantities">Output list of item quantities.</param>
        public void createUnstackedLists(ref List<String> names, ref List<ushort> IDs, ref List<byte> quantities)
        {
            foreach (CIngredient ingredient in mIngredients)
            {
                names.Add(ingredient.Name);
                IDs.Add(ingredient.ID);
                quantities.Add(ingredient.Quantity);
            }
        }
        /// <summary>
        /// Creates a consolidated list of ingredients if the same ingredient is listed more than once (i.e. non-stackable).
        /// </summary>
        /// <param name="itemIDs">Output list of item ID's.</param>
        /// <param name="itemQuan">Output list of item quantities.</param>
        public void createLists(ref List<ushort> itemIDs, ref List<ushort> itemQuan)
        {
            foreach( CIngredient ingredient in mIngredients ) 
            {
                if( itemIDs.Contains(ingredient.ID ) )
                {
                    int index = itemIDs.IndexOf( ingredient.ID );
                    itemQuan[index] += ingredient.Quantity;
                }
                else
                {
                    itemIDs.Add( ingredient.ID );
                    itemQuan.Add( ingredient.Quantity );
                }
            }
        }
        /// <summary>
        /// Attempts to match the resultant item name and quantity from the chat log with the saved NQ, HQ1, HQ2, or HQ3 result.
        /// </summary>
        /// <param name="result_name">Resultant item name taken from the chat log (not the entire string).</param>
        /// <param name="result_quantity">Resultant item quantity taken from the chat log.</param>
        /// <returns>NQ, HQ1/2/3, or Unknown synthesis result.</returns>
        public RESULT_TYPE parseResult(String result_name, ushort result_quantity)
        {
            // A result string and quantity was found from the chatlog.
            // Now match it with our result list.

            LoggingFunctions.Debug("Recipe::parseResult( " + result_name + " * " + result_quantity + ")", LoggingFunctions.DBG_SCOPE.CRAFTER );

            foreach( CResult result in mResults )
            {
                if( result.compare( result_name, result_quantity ) )
                {
                    switch( mResults.IndexOf( result ) )
                    {
                        case 0:
                            return RESULT_TYPE.RESULT_NQ;
                        case 1:
                            return RESULT_TYPE.RESULT_HQ1;
                        case 2:
                            return RESULT_TYPE.RESULT_HQ2;
                        case 3:
                            return RESULT_TYPE.RESULT_HQ3;
                        default:
                            return RESULT_TYPE.RESULT_UNK;
                    }
                }
            }
            return RESULT_TYPE.RESULT_UNK;
        }
        /// <summary>
        /// Converts the FFXIEnums.CRAFTS value passed to the corresponding string value.
        /// </summary>
        /// <param name="iCraft">Enum craft value to convert.</param>
        /// <returns>Returns the corresponding craft string value.</returns>
        private String getCraftString(FFXIEnums.CRAFTS iCraft)
        {
            String craft = "";
            switch (iCraft)
            {
                case FFXIEnums.CRAFTS.AL:
                    craft = "Alchemy";
                    break;
                case FFXIEnums.CRAFTS.BC:
                    craft = "Bonecraft";
                    break;
                case FFXIEnums.CRAFTS.CC:
                    craft = "Clothcraft";
                    break;
                case FFXIEnums.CRAFTS.CK:
                    craft = "Cooking";
                    break;
                case FFXIEnums.CRAFTS.FI:
                    craft = "Fishing";
                    break;
                case FFXIEnums.CRAFTS.GS:
                    craft = "Goldsmithing";
                    break;
                case FFXIEnums.CRAFTS.LC:
                    craft = "Leathercraft";
                    break;
                case FFXIEnums.CRAFTS.SM:
                    craft = "Smithing";
                    break;
                case FFXIEnums.CRAFTS.WW:
                    craft = "Woodworking";
                    break;
                default:
                    craft = "Unknown Craft";
                    break;
            }
            return craft;
        }
        /// <summary>
        /// Converts the FFXIEnums.CRYSTALS value passed to the corresponding string value.
        /// </summary>
        /// <param name="iCrystal">Enum crystal value to convert.</param>
        /// <returns>Returns the corresponding crystal string value.</returns>
        private String getCrystalString(FFXIEnums.CRYSTALS iCrystal)
        {
            switch (iCrystal)
            {
                case FFXIEnums.CRYSTALS.DARK:
                    return "Dark Crystal";
                case FFXIEnums.CRYSTALS.EARTH:
                    return "Earth Crystal";
                case FFXIEnums.CRYSTALS.FIRE:
                    return "Fire Crystal";
                case FFXIEnums.CRYSTALS.ICE:
                    return "Ice Crystal";
                case FFXIEnums.CRYSTALS.LIGHT:
                    return "Light Crystal";
                case FFXIEnums.CRYSTALS.LIGHTNING:
                    return "Lightng. Crystal";
                case FFXIEnums.CRYSTALS.WATER:
                    return "Water Crystal";
                case FFXIEnums.CRYSTALS.WIND:
                    return "Wind Crystal";
                default:
                    return "Unknown Crystal";
            }
        }
        private ushort getCrystalId()
        {
            return (ushort)Things.GetIdFromName(getCrystalString(Crystal));
        }
        /// <summary>
        /// Converts the FFXIEnums.CRYSTALS value passed to the corresponding cluster string value.
        /// </summary>
        /// <param name="iElement">Enum cluster value to convert.</param>
        /// <returns>Returns the corresponding cluster string value.</returns>
        private String getClusterString(FFXIEnums.CRYSTALS iElement)
        {
            switch (iElement)
            {
                case FFXIEnums.CRYSTALS.DARK:
                    return "Dark Cluster";
                case FFXIEnums.CRYSTALS.EARTH:
                    return "Earth Cluster";
                case FFXIEnums.CRYSTALS.FIRE:
                    return "Fire Cluster";
                case FFXIEnums.CRYSTALS.ICE:
                    return "Ice Cluster";
                case FFXIEnums.CRYSTALS.LIGHT:
                    return "Light Cluster";
                case FFXIEnums.CRYSTALS.LIGHTNING:
                    return "Lightng. Cluster";
                case FFXIEnums.CRYSTALS.WATER:
                    return "Water Cluster";
                case FFXIEnums.CRYSTALS.WIND:
                    return "Wind Cluster";
                default:
                    return "Unknown Cluster";
            }
        }
        /// <summary>
        /// Converts the SYNTH_TYPE enum value passed to the corresponding string value (Synthesis/Desynthesis).
        /// </summary>
        /// <param name="iType">Enum type to convert.</param>
        /// <returns>Returns the converted synthesis type string (Synthesis, Desynthesis, or Unknown Type).</returns>
        private String getTypeString(SYNTH_TYPE iType)
        {
            String typeString = "";
            switch (iType)
            {
                case SYNTH_TYPE.SYNTH:
                    typeString = "Synthesis";
                    break;
                case SYNTH_TYPE.DESYNTH:
                    typeString = "Desynthesis";
                    break;
                default:
                    typeString = "Unknown Type";
                    break;
            }
            return typeString;
        }
        #endregion Utility Functions
    }
}
