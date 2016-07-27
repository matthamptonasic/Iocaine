using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Things
    {
        #region Crystals & Clusters
        /// <summary>
        /// Returns the corresponding crystal or cluster ID.
        /// That is, if you pass in a crystal ID, returns the corresponding cluster ID.
        /// If you pass in a cluster ID, returns the corresponding crystal ID.
        /// </summary>
        /// <param name="iItemId">Crystal or Cluster item ID.</param>
        /// <returns>Corresponding Cluster or Crystal item ID.</returns>
        public static ushort CrystalAndClusterExchange(ushort iItemId)
        {
            if ((iItemId < 4096) || (iItemId > 4111))
            {
                return invalidID;
            }
            if (iItemId < 4104)
            {
                return (ushort)(iItemId + 8);
            }
            else
            {
                return (ushort)(iItemId - 8);
            }
        }
        #endregion Crystals & Clusters
    }
}