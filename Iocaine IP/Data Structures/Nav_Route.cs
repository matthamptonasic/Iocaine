using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Iocaine2.Data.Client;

namespace Iocaine2.Data.Structures
{
    public class Nav_Route
    {
        #region Constructors
        public Nav_Route()
        {
        }
        public Nav_Route(String iRouteName, UInt32 iRouteID, bool iDirection)
        {
            routeName = iRouteName;
            routeID = iRouteID;
            direction = iDirection;
        }
        public Nav_Route(Nav_Route iRoute)
        {
            this.routeName = iRoute.routeName;
            this.routeID = iRoute.routeID;
            this.direction = iRoute.direction;
            foreach (Routes.UserRoutesRow routeRow in iRoute.routeNodes)
            {
                this.routeNodes.Add(Routes.CloneRouteRow(Statics.Datasets.RoutesDb, routeRow));
            }
        }
        #endregion Constructors
        #region Private Members
        private String routeName = "";
        private UInt32 routeID = 0;
        private bool direction = true;  //true = forward, false = reverse
        private List<Routes.UserRoutesRow> routeNodes = new List<Routes.UserRoutesRow>();
        #endregion Private Members
        #region Public Members
        public String RouteName
        {
            get
            {
                return routeName;
            }
            set
            {
                routeName = value;
            }
        }
        public UInt32 RouteID
        {
            get
            {
                return routeID;
            }
            set
            {
                routeID = value;
            }
        }
        public bool Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }
        public List<Routes.UserRoutesRow> RouteNodes
        {
            get
            {
                return routeNodes;
            }
        }
        #endregion Public Members
        #region Utility Functions
        public void Clear()
        {
            routeName = "";
            routeID = 0;
            direction = true;
            routeNodes.Clear();
        }
        public override string ToString()
        {
            if (direction == true)
            {
                return routeName;
            }
            else
            {
                return routeName + " (Reverse)";
            }
        }
        #endregion Utility Functions
    }
}