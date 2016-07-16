using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iocaine2.Synergy
{
    public partial class SynergyDlg : UserControl
    {
        private Synergizer mSynergizer;
      
        private Synergizer.RecipeRequirementsUpdateDelegate _RecipeRequirementsUpdateDelegate;
        private Synergizer.ElementalBalanceUpdateDelegate _FewellLevelsUpdateDelegate;
        private Synergizer.StatusUpdateDelegate _StatusUpdateDelegate;
        private Synergizer.ElementalBalanceUpdateDelegate _InternalBalanceUpdateDelegate;
        private Synergizer.PressureImpurityUpdateDelegate _PressureImpurityUpdateDelegate;
        private Synergizer.SynergyEndDelegate _SynergyEndDelegate;

        private delegate void InternalPressureImpurityUpdateDelegate(int pressure, int impurity);
        private delegate void InternalSynergyEndDelegate( bool success, bool ingredients_sufficient );
        private InternalPressureImpurityUpdateDelegate _InternalPressureImpurityUpdateDelegate;
        private InternalSynergyEndDelegate _InternalSynergyEndDelegate;

        SyneRecipe _CurrentRecipe;

        public SynergyDlg()
        {
            InitializeComponent();

            _RecipeRequirementsUpdateDelegate = new Synergizer.RecipeRequirementsUpdateDelegate(UpdateFunctionCallback);
            _FewellLevelsUpdateDelegate = new Synergizer.ElementalBalanceUpdateDelegate(UpdateFewellLevelsFunctionCallback);
            _StatusUpdateDelegate = new Synergizer.StatusUpdateDelegate(UpdateStatusCallback);
            _InternalBalanceUpdateDelegate = new Synergizer.ElementalBalanceUpdateDelegate(UpdateInternalBalanceFunctionCallback);
            _PressureImpurityUpdateDelegate = new Synergizer.PressureImpurityUpdateDelegate(UpdatePressureImpurityFunctionCallback);
            _SynergyEndDelegate = new Synergizer.SynergyEndDelegate(SynergyEndFunctionCallBack);

            _InternalPressureImpurityUpdateDelegate = new InternalPressureImpurityUpdateDelegate(InternalUpdatePressureImpurityFunctionCallback);
            _InternalSynergyEndDelegate = new InternalSynergyEndDelegate(InternalSynergyEndFunctionCallback);

   
        }

        private void UpdateStatusCallback(String status, System.Drawing.Color color)
        {
            Statics.FuncPtrs.SetStatusBoxPtr(status, color);
        }

        private void UpdateFunctionCallback(FFXIEnums.CRAFT_RANK rank, String Name, ushort Quantity, ElementalBalance balance)
        {
            recipeRequirementsOverviewControl.update( rank, Name, Quantity, balance );
        }

        private void UpdateFewellLevelsFunctionCallback(ElementalBalance balance)
        {
            fewelBalance.updateLevels(balance);
        }

        private void UpdateInternalBalanceFunctionCallback(ElementalBalance balance)
        {
            InternalBalanceOverview.updateLevels(balance);
        }

        private void UpdatePressureImpurityFunctionCallback(int pressure, int impurity)
        {
            try
            {
                if (PressureImpurityLabel.InvokeRequired)
                {
                    this.Invoke(_InternalPressureImpurityUpdateDelegate, new object[] { pressure, impurity });
                }
                else
                {
                    InternalUpdatePressureImpurityFunctionCallback(pressure, impurity);
                }
            }
            catch (Exception e)
            {
                Iocaine2.Logging.LoggingFunctions.Error("Clearing Recipe in Manager: " + e.ToString());
            }

        }

        private void InternalUpdatePressureImpurityFunctionCallback(int pressure, int impurity)
        {
            String str = "Pressure: " + pressure + " Impurity: " + impurity + "%";
            PressureImpurityLabel.Text = str;
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            //    while( StopAtLevelCheckbox.Checked == true && Iocaine2.Memory.MemReads.Self.Skills.Crafting.get_synergy_skill() < Convert.ToUInt16(StopAtLevelTextBox.Text.ToString()))
            //    {
            // Do not get this instance in the constructor of a control. Grab it the first time we need it. 
            if (mSynergizer == null)
            {
                mSynergizer = Synergizer.Instance;
                mSynergizer.FewelLevelsUpdate = _FewellLevelsUpdateDelegate;
                mSynergizer.RecipeRequirementsUpdate = _RecipeRequirementsUpdateDelegate;
                mSynergizer.InternalBalanceUpdate = _InternalBalanceUpdateDelegate;
                mSynergizer.PressureImpurityUpdate = _PressureImpurityUpdateDelegate;
                mSynergizer.StatusUpdate = _StatusUpdateDelegate;
                mSynergizer.SynergyEnd = _SynergyEndDelegate;
            }

            _CurrentRecipe = new SyneRecipe();

            List<SyneRecipe.Ingredient> ingredients = new List<SyneRecipe.Ingredient>();
            SyneRecipe.Ingredient ingredient1 = new SyneRecipe.Ingredient();
            ingredient1.Name = Ingredient1TextBox.Text;
            ingredient1.Quantity = (ushort)IngredientQuan1TextBox.Value;
            ingredients.Add(ingredient1);
            SyneRecipe.Ingredient ingredient2 = new SyneRecipe.Ingredient();
            ingredient2.Name = Ingredient2TextBox.Text;
            ingredient2.Quantity = (ushort)IngredientQuan2TextBox.Value;
            ingredients.Add(ingredient2);
            SyneRecipe.Ingredient ingredient3 = new SyneRecipe.Ingredient();
            ingredient3.Name = Ingredient3TextBox.Text;
            ingredient3.Quantity = (ushort)IngredientQuan3TextBox.Value;
            ingredients.Add(ingredient3);
            SyneRecipe.Ingredient ingredient4 = new SyneRecipe.Ingredient();
            ingredient4.Name = Ingredient4TextBox.Text;
            ingredient4.Quantity = (ushort)IngredientQuan4TextBox.Value;
            ingredients.Add(ingredient4);
            SyneRecipe.Ingredient ingredient5 = new SyneRecipe.Ingredient();
            ingredient5.Name = Ingredient5TextBox.Text;
            ingredient5.Quantity = (ushort)IngredientQuan5TextBox.Value;
            ingredients.Add(ingredient5);
            SyneRecipe.Ingredient ingredient6 = new SyneRecipe.Ingredient();
            ingredient6.Name = Ingredient6TextBox.Text;
            ingredient6.Quantity = (ushort)IngredientQuan6TextBox.Value;
            ingredients.Add(ingredient6);
            SyneRecipe.Ingredient ingredient7 = new SyneRecipe.Ingredient();
            ingredient7.Name = Ingredient7TextBox.Text;
            ingredient7.Quantity = (ushort)IngredientQuan7TextBox.Value;
            ingredients.Add(ingredient7);
            SyneRecipe.Ingredient ingredient8 = new SyneRecipe.Ingredient();
            ingredient8.Name = Ingredient8TextBox.Text;
            ingredient8.Quantity = (ushort)IngredientQuan8TextBox.Value;
            ingredients.Add(ingredient8);

            List<SyneRecipe.Fewel> balance = new List<SyneRecipe.Fewel>();

            SyneRecipe.Ingredient result = new SyneRecipe.Ingredient();
            result.Name = "Mythril Nugget";
            result.Quantity = 1;

            if (_CurrentRecipe.SetRecipe(ingredients, balance, result) == false)
            {
                Logging.LoggingFunctions.Debug("Syn::ButtonStart_Click: Synergy Could not set recipe.", Logging.LoggingFunctions.DBG_SCOPE.SYNERGY);
            }

            mSynergizer.AllowFireFewell = UseFireFewellBox.Checked;
            mSynergizer.AllowIceFewell = UseIceFewellBox.Checked;
            mSynergizer.AllowWindFewell = UseWindFewellBox.Checked;
            mSynergizer.AllowEarthFewell = UseEarthFewellBox.Checked;
            mSynergizer.AllowLightningFewell = UseLightningFewellBox.Checked;
            mSynergizer.AllowWaterFewell = UseWaterFewellBox.Checked;
            mSynergizer.AllowLightFewell = UseLightFewellBox.Checked;
            mSynergizer.AllowDarkFewell = UseDarkFewellBox.Checked;

            mSynergizer.FixLeaks = FixLeaksBox.Checked;

            if (StopAtLevelCheckbox.Checked == false || Iocaine2.Memory.MemReads.Self.Skills.Crafting.get_synergy_skill() < Convert.ToUInt16(StopAtLevelTextBox.Text.ToString()))
            {
                mSynergizer.Synergize(_CurrentRecipe, PL_mode_button.Checked);
            }
        }

        void SynergyEndFunctionCallBack(bool success, bool ingredients_sufficient )
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(_InternalSynergyEndDelegate, new object[] { success, ingredients_sufficient });
                }
                else
                {
                    InternalSynergyEndFunctionCallback(success, ingredients_sufficient);
                }
            }
            catch (Exception e)
            {
                Iocaine2.Logging.LoggingFunctions.Error("Clearing Recipe in Manager: " + e.ToString());
            }
        }

        void InternalSynergyEndFunctionCallback(bool success, bool ingredients_sufficient )
        {
            if ( ingredients_sufficient )
            {
                if (StopAtLevelCheckbox.Checked == true && Iocaine2.Memory.MemReads.Self.Skills.Crafting.get_synergy_skill() < Convert.ToUInt16(StopAtLevelTextBox.Text.ToString()))
                {
                    mSynergizer.Synergize(_CurrentRecipe, PL_mode_button.Checked);
                }
            }
        }

        private void StopAtLevelTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 31 && (e.KeyChar < '0' || e.KeyChar > '9'))
            {
                e.Handled = true;
            }
        }


    }
}
