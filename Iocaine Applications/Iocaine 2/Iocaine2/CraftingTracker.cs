using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Data.Structures;
using Iocaine2.Logging;

namespace Iocaine2.Tools
{
    class CraftingTracker
    {
        #region Enums
        #endregion Enums

        #region Constructors
        public CraftingTracker(DataGridView iDGV)
        {
            currentView = iDGV;
        }
        #endregion Constructors

        #region Member Variables
        private DataGridView currentView;
        private List<RecipeLog> recipeList = new List<RecipeLog>();
        private int currentDisplayIndex = 0; //index of currently displayed recipe
        private int currentRecipeIndex = 0;  //index of recipe being crafted now
        private int maxIndex = 0;            //max index of all recipes (nb recipes - 1)
        #endregion Member Variables

        #region Public Functions / Access
        public int CurrentRecipe
        {
            get
            {
                return currentDisplayIndex + 1;
            }
        }
        public int TotalRecipes
        {
            get
            {
                return maxIndex + 1;
            }
        }
        /// <summary>
        /// Add a recipe to be tracked. If the same recipe already exists,
        /// the existing recipe's statistics will be used as a starting point.
        /// </summary>
        /// <param name="iRecipe">The recipe to add to the tracker.</param>
        public void InitializeStatsDGV(DataGridView iDGV, Label iRecipeLabel, Label iCurrentRecipeNbLabel, Label iMaxRecipeNbLabel)
        {
            try
            {
                iDGV.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell titleCell = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell numberCell = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell percentCell = new DataGridViewTextBoxCell();

                DataGridViewColumn titleCol = new DataGridViewColumn(titleCell);
                DataGridViewColumn numberCol = new DataGridViewColumn(numberCell);
                DataGridViewColumn percentCol = new DataGridViewColumn(percentCell);

                titleCol.Width = 140;
                numberCol.Width = 43;
                percentCol.Width = 43;
                iDGV.Columns.Add(titleCol);
                iDGV.Columns.Add(numberCol);
                iDGV.Columns.Add(percentCol);

                iRecipeLabel.Text = "Recipe: N/A";

                titleCell.Value = "Attempts:";
                numberCell.Value = 0;
                percentCell.Value = "100%";
                numberCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                percentCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                row.Cells.Add(titleCell);
                row.Cells.Add(numberCell);
                row.Cells.Add(percentCell);
                row.Height = 16;
                iDGV.Rows.Add(row);

                titleCell = new DataGridViewTextBoxCell();
                numberCell = new DataGridViewTextBoxCell();
                percentCell = new DataGridViewTextBoxCell();
                row = new DataGridViewRow();
                titleCell.Value = "Successful:";
                numberCell.Value = 0;
                percentCell.Value = "0%";
                numberCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                percentCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                row.Cells.Add(titleCell);
                row.Cells.Add(numberCell);
                row.Cells.Add(percentCell);
                row.Height = 16;
                iDGV.Rows.Add(row);

                titleCell = new DataGridViewTextBoxCell();
                numberCell = new DataGridViewTextBoxCell();
                percentCell = new DataGridViewTextBoxCell();
                row = new DataGridViewRow();
                titleCell.Value = "Failed:";
                numberCell.Value = "0";
                percentCell.Value = "0%";
                numberCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                percentCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                row.Cells.Add(titleCell);
                row.Cells.Add(numberCell);
                row.Cells.Add(percentCell);
                row.Height = 16;
                iDGV.Rows.Add(row);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("CraftingTracker::InitializeStatsDGV: " + e.ToString());
            }
        }
        public void AddRecipe(Recipe iRecipe, DataGridView iDGV, Label iRecipeLabel, Label iCurrentRecipeNbLabel, Label iMaxRecipeNbLabel)
        {
            try
            {
                int nbExistingRecipes = recipeList.Count;
                int foundIndex = 0;
                bool found = false;
                for (int ii = 0; ii < nbExistingRecipes; ii++)
                {
                    if (recipeList[ii].Compare(iRecipe))
                    {
                        found = true;
                        foundIndex = ii;
                        break;
                    }
                }
                if (!found)
                {
                    recipeList.Add(new RecipeLog(iRecipe));
                    currentDisplayIndex = recipeList.Count - 1;
                    currentRecipeIndex = currentDisplayIndex;
                    maxIndex = currentDisplayIndex;
                    ClearDGV(iDGV);
                    InitializeStatsDGV(iDGV, iRecipeLabel, iCurrentRecipeNbLabel, iMaxRecipeNbLabel);
                }
                else
                {
                    currentRecipeIndex = foundIndex;
                }
                iCurrentRecipeNbLabel.Text = (currentDisplayIndex + 1).ToString();
                iMaxRecipeNbLabel.Text = (maxIndex + 1).ToString();
                UpdateStatsDGV(iDGV, iRecipeLabel);
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("CraftingTracker::AddRecipe: " + e.ToString());
            }
        }
        public void UpdateStatsDGV(DataGridView iDGV, Label iRecipeLabel)
        {
            int currentRow = 0;
            try
            {
                RecipeLog currentLog = recipeList[currentDisplayIndex];
                //Index is [col, row]
                //1st row is recipe name
                iRecipeLabel.Text = "Recipe: " + currentLog.Name;
                //2nd row is total attempts
                iDGV[1, currentRow].Value = currentLog.TotalAttempts;
                iDGV[2, currentRow++].Value = "100%";
                //3rd row is successful attempts
                iDGV[1, currentRow].Value = currentLog.TotalSuccess;
                iDGV[2, currentRow++].Value = String.Format("{0:0.#}", currentLog.SuccessPerc) + "%";

                //Check NQ count
                ushort nqCount = currentLog.SuccessfulNQ;
                if (nqCount > 0)
                {
                    float nqPerc = (float)100 * (float)nqCount / (float)currentLog.TotalAttempts;
                    if ((String)iDGV[0, currentRow].Value != "Successful NQ:")
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewTextBoxCell titleCell = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell numberCell = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell percentCell = new DataGridViewTextBoxCell();
                        numberCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        percentCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        titleCell.Value = "Successful NQ:";
                        numberCell.Value = nqCount.ToString();
                        percentCell.Value = String.Format("{0:0.#}", nqPerc) + "%";
                        row.Cells.Add(titleCell);
                        row.Cells.Add(numberCell);
                        row.Cells.Add(percentCell);
                        row.Height = 16;
                        iDGV.Rows.Insert(currentRow, row);
                    }
                    else
                    {
                        iDGV[1, currentRow].Value = nqCount.ToString();
                        iDGV[2, currentRow].Value = String.Format("{0:0.#}", nqPerc) + "%";
                    }
                    currentRow++;
                }
                //Check HQ1 count
                ushort hq1Count = currentLog.SuccessfulHQ1;
                if (hq1Count > 0)
                {
                    float hq1Perc = (float)100 * (float)hq1Count / (float)currentLog.TotalAttempts;
                    if ((String)iDGV[0, currentRow].Value != "Successful HQ1:")
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewTextBoxCell titleCell = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell numberCell = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell percentCell = new DataGridViewTextBoxCell();
                        numberCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        percentCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        titleCell.Value = "Successful HQ1:";
                        numberCell.Value = hq1Count.ToString();
                        percentCell.Value = String.Format("{0:0.#}", hq1Perc) + "%";
                        row.Cells.Add(titleCell);
                        row.Cells.Add(numberCell);
                        row.Cells.Add(percentCell);
                        row.Height = 16;
                        iDGV.Rows.Insert(currentRow, row);
                    }
                    else
                    {
                        iDGV[1, currentRow].Value = hq1Count.ToString();
                        iDGV[2, currentRow].Value = String.Format("{0:0.#}", hq1Perc) + "%";
                    }
                    currentRow++;
                }
                //Check HQ2 count
                ushort hq2Count = currentLog.SuccessfulHQ2;
                if (hq2Count > 0)
                {
                    float hq2Perc = (float)100 * (float)hq2Count / (float)currentLog.TotalAttempts;
                    if ((String)iDGV[0, currentRow].Value != "Successful HQ2:")
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewTextBoxCell titleCell = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell numberCell = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell percentCell = new DataGridViewTextBoxCell();
                        numberCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        percentCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        titleCell.Value = "Successful HQ2:";
                        numberCell.Value = hq2Count.ToString();
                        percentCell.Value = String.Format("{0:0.#}", hq2Perc) + "%";
                        row.Cells.Add(titleCell);
                        row.Cells.Add(numberCell);
                        row.Cells.Add(percentCell);
                        row.Height = 16;
                        iDGV.Rows.Insert(currentRow, row);
                    }
                    else
                    {
                        iDGV[1, currentRow].Value = hq2Count.ToString();
                        iDGV[2, currentRow].Value = String.Format("{0:0.#}", hq2Perc) + "%";
                    }
                    currentRow++;
                }
                //Check HQ3 count
                ushort hq3Count = currentLog.SuccessfulHQ3;
                if (hq3Count > 0)
                {
                    float hq3Perc = (float)100 * (float)hq3Count / (float)currentLog.TotalAttempts;
                    if ((String)iDGV[0, currentRow].Value != "Successful HQ3:")
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewTextBoxCell titleCell = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell numberCell = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell percentCell = new DataGridViewTextBoxCell();
                        numberCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        percentCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        titleCell.Value = "Successful HQ3:";
                        numberCell.Value = hq3Count.ToString();
                        percentCell.Value = String.Format("{0:0.#}", hq3Perc) + "%";
                        row.Cells.Add(titleCell);
                        row.Cells.Add(numberCell);
                        row.Cells.Add(percentCell);
                        row.Height = 16;
                        iDGV.Rows.Insert(currentRow, row);
                    }
                    else
                    {
                        iDGV[1, currentRow].Value = hq3Count.ToString();
                        iDGV[2, currentRow].Value = String.Format("{0:0.#}", hq3Perc) + "%";
                    }
                    currentRow++;
                }

                //Next row is failed attempts
                iDGV[1, currentRow].Value = currentLog.TotalFailed;
                iDGV[2, currentRow].Value = String.Format("{0:0.#}", currentLog.FailedPerc) + "%";
                currentRow++;

                //Now go thru lost materials
                byte nbMaterials = currentLog.NumberIngredients;
                for (byte ii = 0; ii < nbMaterials; ii++)
                {
                    ushort nbLost = currentLog.LostQuantity(ii);
                    if (nbLost > 0)
                    {
                        String lostItemName = Things.GetNameFromId(currentLog.ItemID(ii));
                        //If we're at the end of DGV or the current row != the item we're looking at, add row.
                        if ((currentRow >= iDGV.Rows.Count)
                            || ((String)iDGV[0, currentRow].Value != ("Lost " + lostItemName + "(s):")))
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            DataGridViewTextBoxCell titleCell = new DataGridViewTextBoxCell();
                            DataGridViewTextBoxCell numberCell = new DataGridViewTextBoxCell();
                            DataGridViewTextBoxCell percentCell = new DataGridViewTextBoxCell();
                            numberCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            percentCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            titleCell.Value = "Lost " + lostItemName + "(s):";
                            numberCell.Value = currentLog.LostQuantity(ii);
                            percentCell.Value = String.Format("{0:0.#}", currentLog.LostPercentage(ii)) + "%";
                            row.Cells.Add(titleCell);
                            row.Cells.Add(numberCell);
                            row.Cells.Add(percentCell);
                            row.Height = 16;
                            iDGV.Rows.Insert(currentRow, row);
                        }
                        else
                        {
                            iDGV[1, currentRow].Value = currentLog.LostQuantity(ii);
                            iDGV[2, currentRow].Value = String.Format("{0:0.#}", currentLog.LostPercentage(ii)) + "%";
                        }
                        currentRow++;
                    }
                }
                iDGV.Refresh();
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("CraftingTracker::UpdateStatsDGV: " + e.ToString());
            }
        }
        public void UpdateStatsLog(Recipe iRecipe, RecipeLog.RESULT_TYPE iResult, List<ushort> lostItems, List<ushort> lostQuan)
        {
            try
            {
                RecipeLog currentLog = recipeList[currentRecipeIndex];
                if (!currentLog.Compare(iRecipe))
                {
                    LoggingFunctions.Error("Updating stats log. Current recipe index did not match recipe being crafted");
                    return;
                }
                if (iResult == RecipeLog.RESULT_TYPE.UNK)
                {
                    return;
                }
                else if (iResult == RecipeLog.RESULT_TYPE.FAIL)
                {
                    currentLog.FailedInc(lostItems, lostQuan);
                }
                else
                {
                    currentLog.SuccessInc(iResult);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("CraftingTracker::UpdateStatsLog: " + e.ToString());
            }
        }
        public int GetCurrentRecipeNumber()
        {
            return currentDisplayIndex;
        }
        public int GetMaxRecipeNumber()
        {
            return maxIndex;
        }
        public void DisplayNextRecipe(DataGridView iDGV, Label iRecipeLabel, Label iCurrentRecipeNbLabel, Label iMaxRecipeNbLabel)
        {
            try
            {
                if (currentDisplayIndex < maxIndex)
                {
                    currentDisplayIndex++;
                    iCurrentRecipeNbLabel.Text = (currentDisplayIndex + 1).ToString();
                    iMaxRecipeNbLabel.Text = (maxIndex + 1).ToString();
                    ClearDGV(iDGV);
                    InitializeStatsDGV(iDGV, iRecipeLabel, iCurrentRecipeNbLabel, iMaxRecipeNbLabel);
                    UpdateStatsDGV(iDGV, iRecipeLabel);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("CraftingTracker::DisplayNextRecipe: " + e.ToString());
            }
        }
        public void DisplayPreviousRecipe(DataGridView iDGV, Label iRecipeLabel, Label iCurrentRecipeNbLabel, Label iMaxRecipeNbLabel)
        {
            try
            {
                if (currentDisplayIndex > 0)
                {
                    currentDisplayIndex--;
                    iCurrentRecipeNbLabel.Text = (currentDisplayIndex + 1).ToString();
                    iMaxRecipeNbLabel.Text = (maxIndex + 1).ToString();
                    ClearDGV(iDGV);
                    InitializeStatsDGV(iDGV, iRecipeLabel, iCurrentRecipeNbLabel, iMaxRecipeNbLabel);
                    UpdateStatsDGV(iDGV, iRecipeLabel);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("CraftingTracker::DisplayPreviousRecipe: " + e.ToString());
            }
        }
        #endregion Public Functions / Access

        #region Private Functions / Utilities
        private void ClearDGV(DataGridView iDGV)
        {
            try
            {
                int nbRows = iDGV.Rows.Count;
                for (int ii = nbRows - 1; ii >= 0; ii--)
                {
                    iDGV.Rows.RemoveAt(ii);
                }
                int nbCols = iDGV.Columns.Count;
                for (int ii = nbCols - 1; ii >= 0; ii--)
                {
                    iDGV.Columns.RemoveAt(ii);
                }
            }
            catch(Exception e)
            {
                LoggingFunctions.Error("CraftingTracker::ClearDGV: " + e.ToString());
            }
        }
        #endregion Private Functions / Utilities
    }
}