using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Logging;

namespace Iocaine2.Data.Structures
{
    public abstract partial class Action
    {
        public class InventoryItem : Condition
        {
            #region Enums
            #endregion Enums

            #region Private Members
            private ushort m_itemId;
            private bool m_anyMobile;
            private Inventory.ItemContainer.STORAGE_TYPE m_location;
            #endregion Private Members

            #region Public Properties
            public ushort ItemID
            {
                get
                {
                    return m_itemId;
                }
            }
            public bool AnyMobile
            {
                get
                {
                    return m_anyMobile;
                }
            }
            public Inventory.ItemContainer.STORAGE_TYPE Location
            {
                get
                {
                    return m_location;
                }
            }
            #endregion Public Properties

            #region Constructor(s)
            public InventoryItem(string iName, ushort iItemId, bool iAnyMobile = false)
                : base(CONDITION_TYPE.INV_ITEM, iName)
            {
                m_itemId = iItemId;
                m_anyMobile = iAnyMobile;
            }
            #endregion Constructor(s)

            #region Public Methods
            public override bool IsSatisfied()
            {
                if (m_anyMobile)
                {

                    if (Inventory.Containers.ContainsMobile((short)m_itemId, out m_location))
                    {
                        return true;
                    }
                }
                else if (Inventory.Containers.Bag.Contains((short)m_itemId))
                {
                    m_location = Inventory.ItemContainer.STORAGE_TYPE.BAG;
                    return true;
                }
                return false;
            }
            #endregion Public Methods

            #region Private Methods
            #endregion Private Methods
        }
    }
}