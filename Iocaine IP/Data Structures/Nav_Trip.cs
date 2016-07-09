using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Iocaine2.Data.Client;

namespace Iocaine2.Data.Structures
{
    public class Nav_Trip
    {
        #region Constructors
        public Nav_Trip()
        {
        }
        public Nav_Trip(String iTripName, UInt32 iTripID, String iTripTags)
        {
            tripName = iTripName;
            tripID = iTripID;
            tripTags = iTripTags;
        }
        public Nav_Trip(Nav_Trip iTrip)
        {
            this.tripName = iTrip.tripName;
            this.tripID = iTrip.tripID;
            this.tripTags = iTrip.tripTags;
            foreach (Routes.UserTripsRow tripRow in iTrip.tripNodes)
            {
                Routes.UserTripsRow newRow = Routes.CloneTripRow(Statics.Datasets.RoutesDb, tripRow);
                this.tripNodes.Add(newRow);
            }
            foreach (Nav_Route route in iTrip.tripRoutes)
            {
                this.tripRoutes.Add(new Nav_Route(route));
            }
        }
        #endregion Constructors
        #region Private Members
        private String tripName = "";
        private UInt32 tripID = 0;
        private String tripTags = "";
        private List<Routes.UserTripsRow> tripNodes = new List<Routes.UserTripsRow>();
        private List<Nav_Route> tripRoutes = new List<Nav_Route>();
        #endregion Private Members
        #region Public Members
        public String TripName
        {
            get
            {
                return tripName;
            }
            set
            {
                tripName = value;
            }
        }
        public UInt32 TripID
        {
            get
            {
                return tripID;
            }
            set
            {
                tripID = value;
            }
        }
        public String TripTags
        {
            get
            {
                return tripTags;
            }
            set
            {
                tripTags = value;
            }
        }
        public List<Routes.UserTripsRow> TripNodes
        {
            get
            {
                return tripNodes;
            }
        }
        public List<Nav_Route> TripRoutes
        {
            get
            {
                return tripRoutes;
            }
            set
            {
                tripRoutes = value;
            }
        }
        #endregion Public Members
        #region Utility Functions
        public void Clear()
        {
            tripName = "";
            tripID = 0;
            tripNodes.Clear();
            tripRoutes.Clear();
        }
        public Nav_Route FindRoute(String iRouteName)
        {
            foreach (Nav_Route route in tripRoutes)
            {
                if (route.RouteName == iRouteName)
                {
                    return route;
                }
            }
            return null;
        }
        public Nav_Route FindRoute(UInt32 iRouteID)
        {
            foreach (Nav_Route route in tripRoutes)
            {
                if (route.RouteID == iRouteID)
                {
                    return route;
                }
            }
            return null;
        }
        public override string ToString()
        {
            return tripName;
        }
        #endregion Utility Functions
    }
}
