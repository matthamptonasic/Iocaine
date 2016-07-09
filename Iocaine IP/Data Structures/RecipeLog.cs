using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public class RecipeLog
    {
        #region Constructors
        public RecipeLog(Recipe iRecipe)
        {
            recipeName = iRecipe.mResults[0].Name;
            if(iRecipe.mResults[0].Quantity > 1)
            {
                recipeName += (" x" + iRecipe.mResults[0].Quantity.ToString());
            }
            iRecipe.createLists(ref recipeItemIDs, ref recipeItemQuan);
            byte nbIng = (byte)recipeItemIDs.Count;
            for (byte ii = 0; ii < nbIng; ii++)
            {
                successQuan.Add(0);
                failedQuan.Add(0);
                failedQuanPerc.Add(0);
            }
            for (int kk = 0; kk < 4; kk++)
            {
                successType[kk] = 0;
            }
        }
        #endregion Constructors

        #region Enums
        public enum RESULT_TYPE
        {
            NQ = 0,
            HQ1 = 1,
            HQ2 = 2,
            HQ3 = 3,
            FAIL = 4,
            UNK = 5
        }
        #endregion Enums

        #region Member Variables
        String recipeName = "";
        private List<ushort> recipeItemIDs = new List<ushort>();
        private List<ushort> recipeItemQuan = new List<ushort>();
        private List<ushort> successQuan = new List<ushort>();
        private List<ushort> failedQuan = new List<ushort>();
        private List<float> failedQuanPerc = new List<float>();
        private ushort[] successType = new ushort[4];
        private ushort succeededAttempts = 0;
        private ushort failedAttempts = 0;
        #endregion Member Variables

        #region Public Functions / Access
        #region update functions
        public void SuccessInc(RESULT_TYPE nq_hq)
        {
            succeededAttempts++;
            successType[(int)nq_hq]++;
            int ingCnt = recipeItemIDs.Count;
            for (int ii = 0; ii < ingCnt; ii++)
            {
                successQuan[ii] += recipeItemQuan[ii];
            }
            updatePercentage();
        }

        public void FailedInc(List<ushort> lostItemIDs, List<ushort> lostItemQuan)
        {
            failedAttempts++;
            int itemCnt = lostItemIDs.Count;
            int idx = 0;
            for (byte ii = 0; ii < itemCnt; ii++)
            {
                idx = recipeItemIDs.IndexOf(lostItemIDs[ii]);
                failedQuan[idx] += lostItemQuan[ii];
                LoggingFunctions.Debug("Incrementing lost item " + lostItemIDs[ii] + " by " + lostItemQuan[ii], LoggingFunctions.DBG_SCOPE.CRAFTER);
            }
            updatePercentage();
        }
        #endregion update functions
        #region access functions
        public String Name
        {
            get
            {
                return recipeName;
            }
        }
        public ushort TotalAttempts
        {
            get
            {
                return (ushort)(succeededAttempts + failedAttempts);
            }
        }
        public ushort TotalSuccess
        {
            get
            {
                return succeededAttempts;
            }
        }
        public float SuccessPerc
        {
            get
            {
                if ((succeededAttempts + failedAttempts) > 0)
                {
                    return (float)100 * (float)succeededAttempts / (float)(failedAttempts + succeededAttempts);
                }
                else
                {
                    return 0;
                }
            }
        }
        public ushort TotalFailed
        {
            get
            {
                return failedAttempts;
            }
        }
        public float FailedPerc
        {
            get
            {
                if ((succeededAttempts + failedAttempts) > 0)
                {
                    return (float)(100 * (float)failedAttempts / (float)(failedAttempts + succeededAttempts));
                }
                else
                {
                    return (float)0.0;
                }
            }
        }
        public ushort SuccessfulNQ
        {
            get
            {
                return successType[(int)RESULT_TYPE.NQ];
            }
        }
        public ushort SuccessfulHQ1
        {
            get
            {
                return successType[(int)RESULT_TYPE.HQ1];
            }
        }
        public ushort SuccessfulHQ2
        {
            get
            {
                return successType[(int)RESULT_TYPE.HQ2];
            }
        }
        public ushort SuccessfulHQ3
        {
            get
            {
                return successType[(int)RESULT_TYPE.HQ3];
            }
        }
        public byte NumberIngredients
        {
            get
            {
                return (byte)recipeItemIDs.Count;
            }
        }
        public ushort ItemID(byte index)
        {
            if (index > (recipeItemIDs.Count - 1))
            {
                return 0;
            }
            else
            {
                return recipeItemIDs[index];
            }
        }
        public ushort LostQuantity(byte index)
        {
            if (index > (failedQuan.Count - 1))
            {
                return 0;
            }
            else
            {
                return failedQuan[index];
            }
        }
        public float LostPercentage(byte index)
        {
            if (index > (failedQuan.Count - 1))
            {
                return 0;
            }
            else
            {
                return failedQuanPerc[index];
            }
        }
        #endregion access functions
        #region compare function
        public bool Compare(Recipe iRecipe)
        {
            List<ushort> localItemIDs = new List<ushort>();
            List<ushort> localItemQuan = new List<ushort>();
            iRecipe.createLists(ref localItemIDs, ref localItemQuan);
            //Check for same number of items
            if (recipeItemIDs.Count != localItemIDs.Count)
            {
                return false;
            }
            //Now that we know both recipes have the same number of items,
            //check that each item from the passed in recipe is present
            //in the our object's list and that the quantities are the same.
            byte ingCnt = (byte)localItemIDs.Count;
            for (byte ii = 0; ii < ingCnt; ii++)
            {
                byte idx = 0;
                if (recipeItemIDs.Contains(localItemIDs[ii]))
                {
                    idx = (byte)recipeItemIDs.IndexOf(localItemIDs[ii]);
                    if (recipeItemQuan[idx] != localItemQuan[ii])
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        #endregion compare function
        #endregion Public Functions / Access

        #region Private Functions / Utilities

        private void updatePercentage()
        {
            byte nbItems = (byte)recipeItemIDs.Count;
            for (byte ii = 0; ii < nbItems; ii++)
            {
                LoggingFunctions.Debug("failedQuan[" + ii + "] = " + failedQuan[ii], LoggingFunctions.DBG_SCOPE.CRAFTER);
                LoggingFunctions.Debug("succedQuan[" + ii + "] = " + successQuan[ii], LoggingFunctions.DBG_SCOPE.CRAFTER);
                failedQuanPerc[ii] = ((float)(failedQuan[ii] * 100) / (float)((succeededAttempts + failedAttempts) * recipeItemQuan[ii]));
            }
        }
        #endregion Private Functions / Utilities
    }
}
