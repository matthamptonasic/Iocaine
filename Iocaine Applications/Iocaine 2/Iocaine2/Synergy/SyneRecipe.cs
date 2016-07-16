using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Iocaine2.Data.Client;

namespace Iocaine2.Synergy
{
    
    public class SyneRecipe
    {
        public class Ingredient
        {
            private String mName;
            private ushort mId;
            private ushort mQuantity;
            public Ingredient()
            {
                mName = "None";
                mId = Things.invalidID;
                mQuantity = 0;
            }
            public String Name
            {
                get
                {
                    return mName;
                }
                set
                {
                    mId = Things.GetIdFromName(value);
                    if (mId == Things.invalidID)
                    {
                        Logging.LoggingFunctions.Debug("SyneRecipe::Ingredient::Name: Id not found (" + Name + ").", Logging.LoggingFunctions.DBG_SCOPE.SYNERGY);
                    }
                    mName = value;
                }
            }
            public ushort Id
            {
                get
                {
                    return mId;
                }
                set
                {
                    mId = value;
                }
            }
            public ushort Quantity
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
            public bool Valid
            {
                get
                {
                    return ( mQuantity > 0 && mId != Things.invalidID );
                }
            }
        }
        public List<Ingredient> mIngredients;
        public Ingredient mResult;
        public class Fewel
        {
            private FFXIEnums.CRYSTALS mElement;
            private ushort mBalance;
            public Fewel()
            {
                mElement = FFXIEnums.CRYSTALS.UNKNOWN;
                mBalance = 0;
            }
            public FFXIEnums.CRYSTALS Element
            {
                get
                {
                    return mElement;
                }
                set
                {
                    mElement = value;
                }
            }
            public ushort Balance
            {
                get
                {
                    return mBalance;
                }
                set
                {
                    mBalance = value;
                }
            }
        }
        public List<Fewel> mElementalBalance;
        public SyneRecipe()
        {
            mIngredients = new List<Ingredient>();
            mElementalBalance = new List<Fewel>();
            mResult = new Ingredient();
        }
        public bool SetRecipe( List<Ingredient> Ingredients,
            List<Fewel> ElementalBalance,
            Ingredient Result )
        {
            foreach (Ingredient ingredient in Ingredients)
            {
                if (ingredient.Quantity > 0)
                {
                    mIngredients.Add(ingredient);
                }
            }
            mElementalBalance = ElementalBalance;
            mResult = Result;
            return true;
        }
        public String Name
        {
            get
            {
                return mResult.Name;
            }
        }
        public List<ushort> IdList
        {
            get
            {
                List<ushort> list = new List<ushort>();
                foreach (Ingredient ingredient in mIngredients)
                {
                    if (ingredient.Valid == true )
                    {
                        list.Add(ingredient.Id);
                    }
                }
                return list;
            }
        }
        public List<byte> QuantityList
        {
            get
            {
                List<byte> list = new List<byte>();
                foreach (Ingredient ingredient in mIngredients)
                {
                    if (ingredient.Valid == true)
                    {
                        list.Add((byte)ingredient.Quantity);
                    }
                }
                return list;
            }
        }
    }
}
