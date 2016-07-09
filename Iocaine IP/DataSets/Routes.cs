using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Text;

namespace Iocaine2.Data.Client
{
    public partial class Routes
    {
        public static UserRoutesRow CloneRouteRow(Routes iRoutesDS, UserRoutesRow iRouteRow)
        {
            Routes.UserRoutesRow newRow = iRoutesDS._UserRoutes.NewUserRoutesRow();
            newRow.ItemArray = (object[])iRouteRow.ItemArray.Clone();
            return newRow;
        }
        public static UserTripsRow CloneTripRow(Routes iRoutesDS, UserTripsRow iTripRow)
        {
            Routes.UserTripsRow newRow = iRoutesDS.UserTrips.NewUserTripsRow();
            newRow.Direction = iTripRow.Direction;
            newRow.RouteID = iTripRow.RouteID;
            newRow.RouteSequenceID = iTripRow.RouteSequenceID;
            newRow.TripID = iTripRow.TripID;
            newRow.TripName = iTripRow.TripName;
            if (!iTripRow.IsTripTagsNull())
            {
                newRow.TripTags = iTripRow.TripTags;
            }
            else
            {
                newRow.TripTags = "";
            }
            return newRow;
        }
    }
}