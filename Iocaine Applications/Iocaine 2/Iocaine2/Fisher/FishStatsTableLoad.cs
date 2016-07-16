using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Iocaine2.Data.Client;

namespace Iocaine2.Bots
{
    public sealed partial class Fisher : Bot
    {
        private partial class FishStatsDataSet : System.Data.DataSet
        {
            public partial class ResultTable : System.Data.TypedTableBase<ResultTableRow>
            {
                internal void loadData()
                {
                    FishStatsDataSet.ResultTableRow row;

                    row = NewResultTableRow();
                    row.Result = "Caught Fish";
                    row.ID = 1;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Caught Item";
                    row.ID = 2;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Caught Monster";
                    row.ID = 3;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Gave Up";
                    row.ID = 4;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Didn't Catch Anything";
                    row.ID = 5;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Line Broke";
                    row.ID = 6;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Released (Inventory Full)";
                    row.ID = 7;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Not Enough Skill";
                    row.ID = 8;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Got Away";
                    row.ID = 9;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Rod Broke";
                    row.ID = 10;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Too Small";
                    row.ID = 11;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Too Large";
                    row.ID = 12;
                    AddResultTableRow(row);

                    row = NewResultTableRow();
                    row.Result = "Unknown";
                    row.ID = 13;
                    AddResultTableRow(row);

                    AcceptChanges();
                }
            }
        }
    }
}