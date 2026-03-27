using Microsoft.Data.SqlClient;
using ShipzoBackend.Models;
using System.Data;

namespace ShipzoBackend.BusinessLayer
{
    public class ParcelService
    {
        private readonly Database db;

        public ParcelService(Database database)
        {
            db = database;
        }


        public object CreateParcel(Parcel parcel)
        {
            string newParcelId = "PAR" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            parcel.ParcelId = newParcelId;

            SqlParameter[] parameters =
            {
        new SqlParameter("@ParcelId", parcel.ParcelId),
        new SqlParameter("@SenderName", parcel.SenderName),
        new SqlParameter("@ReceiverName", parcel.ReceiverName),
        new SqlParameter("@SenderAddress", parcel.SenderAddress),
        new SqlParameter("@ReceiverAddress", parcel.ReceiverAddress),
        new SqlParameter("@ReceiverContactNumber", parcel.ReceiverContactNumber),
        new SqlParameter("@Weight", parcel.Weight),
        new SqlParameter("@AgentId", parcel.AgentId),
        new SqlParameter("@DeliveryAmount", parcel.DeliveryAmount),
        new SqlParameter("@FastDelivery", parcel.FastDelivery),
        new SqlParameter("@PaymentMode", parcel.PaymentMode),
        new SqlParameter("@IsPaid", parcel.IsPaid)
    };

            DataTable result = db.ExecuteProcedure("sp_CreateParcel", parameters);

            string message = result.Rows[0]["Message"].ToString();

            return new
            {
                ParcelId = parcel.ParcelId,
                Message = message
            };
        }


        public List<ParcelDetails> GetAllParcels()
        {
            DataTable table = db.ExecuteProcedure("sp_GetAllParcels", null);

            List<ParcelDetails> parcels = new List<ParcelDetails>();

            foreach (DataRow row in table.Rows)
            {
                parcels.Add(MapParcelDetails(row));
            }

            return parcels;
        }

      
        public ParcelDetails GetParcelById(string parcelId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ParcelId", parcelId)
            };

            DataTable table = db.ExecuteProcedure("sp_GetParcelById", parameters);

            if (table.Rows.Count == 0)
                return null;

            return MapParcelDetails(table.Rows[0]);
        }
        public List<ParcelDetails> GetParcelsByAgentId(string agentId)
        {
            SqlParameter[] parameters =
            {
        new SqlParameter("@AgentId", agentId)
    };

            DataTable table = db.ExecuteProcedure("sp_GetParcelsByAgentId", parameters);

            List<ParcelDetails> parcels = new List<ParcelDetails>();

            foreach (DataRow row in table.Rows)
            {
                parcels.Add(MapParcelDetails(row));
            }

            return parcels;
        }


        public string UpdateParcelStatus(string parcelId, string status, string remarks)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ParcelId", parcelId),
                new SqlParameter("@Status", status),
                new SqlParameter("@Remarks", remarks)
            };

            DataTable result = db.ExecuteProcedure("sp_UpdateParcelStatus", parameters);

            return result.Rows[0]["Message"].ToString();
        }

        public List<ParcelDetails> GetPendingParcels()
        {
            DataTable table = db.ExecuteProcedure("sp_GetPendingParcels", null);

            List<ParcelDetails> parcels = new List<ParcelDetails>();

            foreach (DataRow row in table.Rows)
            {
                parcels.Add(MapParcelDetails(row));
            }

            return parcels;
        }

      
        public List<ParcelDetails> FilterParcels(string status, string agentId, DateTime? date)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Status", (object?)status ?? DBNull.Value),
                new SqlParameter("@AgentID", (object?)agentId ?? DBNull.Value),
                new SqlParameter("@Date", (object?)date ?? DBNull.Value)
            };

            DataTable table = db.ExecuteProcedure("sp_FilterParcels", parameters);

            List<ParcelDetails> parcels = new List<ParcelDetails>();

            foreach (DataRow row in table.Rows)
            {
                parcels.Add(MapParcelDetails(row));
            }

            return parcels;
        }

     
        public string UpdateParcelLocation(string parcelId, double lat, double lng, string locationText)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ParcelId", parcelId),
                new SqlParameter("@Lat", lat),
                new SqlParameter("@Lng", lng),
                new SqlParameter("@LocationText", locationText)
            };

            DataTable result = db.ExecuteProcedure("sp_UpdateParcelLocation", parameters);

            return result.Rows[0]["Message"].ToString();
        }

        public TrackParcel TrackParcel(string parcelId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ParcelId", parcelId)
            };

            DataTable table = db.ExecuteProcedure("sp_TrackParcel", parameters);

            if (table.Rows.Count == 0)
                return null;

            DataRow row = table.Rows[0];

            return new TrackParcel
            {
                ParcelId = row["ParcelId"].ToString(),
                Status = row["Status"].ToString(),
                ReceiverAddress = row["ReceiverAddress"].ToString(),
                CurrentLat = row["CurrentLat"] == DBNull.Value ? null : Convert.ToDouble(row["CurrentLat"]),
                CurrentLng = row["CurrentLng"] == DBNull.Value ? null : Convert.ToDouble(row["CurrentLng"]),
                CurrentLocationText = row["CurrentLocationText"].ToString(),
                LocationUpdatedAt = row["LocationUpdatedAt"] == DBNull.Value ? null : Convert.ToDateTime(row["LocationUpdatedAt"])
            };
        }


        private ParcelDetails MapParcelDetails(DataRow row)
        {
            bool fastDelivery = row["FastDelivery"] != DBNull.Value && Convert.ToBoolean(row["FastDelivery"]);

            return new ParcelDetails
            {
                ParcelId = row["ParcelId"].ToString(),
                SenderName = row["SenderName"].ToString(),
                ReceiverName = row["ReceiverName"].ToString(),
                SenderAddress = row["SenderAddress"].ToString(),
                ReceiverAddress = row["ReceiverAddress"].ToString(),
                ReceiverContactNumber = row["ReceiverContactNumber"].ToString(),
                Weight = row["Weight"] == DBNull.Value ? null : Convert.ToDecimal(row["Weight"]),
                Status = row["Status"].ToString(),
                AgentId = row["AgentId"].ToString(),
                Date = Convert.ToDateTime(row["Date"]),
                Remarks = row["Remarks"].ToString(),

                DeliveryType = fastDelivery ? "Fast Delivery" : "Normal Delivery"
            };
        }
    }
}