using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ShipzoBackend.BusinessLayer;
using ShipzoBackend.Models;
using System.Data;

namespace ShipzoBackend.Services
{
    public class SupervisorService
    {
        private readonly Database db;

        public SupervisorService(Database database)
        {
            db = database;
        }


        public string Login(string SupervisorId, string password)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@SupervisorId", SupervisorId),
                new SqlParameter("@Password", password)
            };

            DataTable result = db.ExecuteProcedure("sp_LoginSupervisor", parameters);



            return result.Rows[0]["Message"].ToString();
        }

        public DashboardSummary GetDashboardSummary()
        {
            DataTable table = db.ExecuteProcedure("sp_GetDashboardSummary", null);

            DataRow row = table.Rows[0];

            return new DashboardSummary
            {
                TotalParcels = Convert.ToInt32(row["TotalParcels"]),
                DeliveredParcels = Convert.ToInt32(row["DeliveredParcels"]),
                InTransitParcels = Convert.ToInt32(row["InTransitParcels"]),
                PickedUpParccels = Convert.ToInt32(row["PickedUpParccels"]),
                TotalAgents = Convert.ToInt32(row["TotalAgents"]),
                TodaysEarning = Convert.ToDecimal(row["TodaysEarning"])
            };
        }


        public Supervisor GetSupervisorById(string supervisorId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@SupervisorId", supervisorId)
            };

            DataTable table = db.ExecuteProcedure("sp_GetSupervisorById", parameters);

            if (table.Rows.Count == 0)
                return null;

            DataRow row = table.Rows[0];

            return new Supervisor
            {
                SupervisorId = row["SupervisorId"].ToString(),
                Name = row["Name"].ToString(),
                Email = row["Email"].ToString(),
                Phone = row["Phone"].ToString()
            };
        }


        public string UpdateSupervisorProfile(ChangePassword dto)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@SupervisorId", dto.SupervisorId),
                new SqlParameter("@Name", dto.Name),
                new SqlParameter("@Email", dto.Email),
                new SqlParameter("@Phone", dto.Phone),
                new SqlParameter("@CurrentPassword", (object?)dto.CurrentPassword ?? DBNull.Value),
                new SqlParameter("@NewPassword", (object?)dto.NewPassword ?? DBNull.Value)
            };

            DataTable result = db.ExecuteProcedure("sp_UpdateSupervisorProfile", parameters);

            return result.Rows[0]["Message"].ToString();
        }


        public List<NotificationResponse> GetNotifications()
        {
            DataTable table = db.ExecuteProcedure("sp_GetSupervisorNotifications", null);

            List<NotificationResponse> notifications = new List<NotificationResponse>();

            foreach (DataRow row in table.Rows)
            {
                notifications.Add(new NotificationResponse
                {
                    ParcelId = row["ParcelId"].ToString(),
                    Status = row["Status"].ToString(),
                    AgentID = row["AgentId"] == DBNull.Value ? null : row["AgentId"].ToString(),
                    AgentName = row["AgentName"].ToString(),
                    Type = row["Type"].ToString(),
                    Message = row["Message"].ToString()
                });
            }

            return notifications;
        }

        public string AssignAgent(AssignAgent request)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ParcelId", request.ParcelId),
                new SqlParameter("@AgentId", request.AgentId)
            };

            DataTable result = db.ExecuteProcedure("sp_AssignAgentToParcel", parameters);

            return result.Rows[0]["Message"].ToString();

        }
        public string GetAgentEmail(string agentId)
        {
            SqlParameter[] parameters =
            {
        new SqlParameter("@AgentId", agentId)
    };

            DataTable table = db.ExecuteProcedure("sp_GetAgentEmail", parameters);

            if (table.Rows.Count > 0)
            {
                return table.Rows[0]["Email"].ToString();
            }

            return null;
        }
        public string SendReminder(AssignAgent request)
        {

            AssignAgent(request);


            string agentEmail = GetAgentEmail(request.AgentId);

            if (string.IsNullOrEmpty(agentEmail))
                return "Agent email not found";


            string subject = "Parcel Delivery Reminder";

            string body = $"Reminder: Parcel {request.ParcelId} is pending for delivery. Please update the parcel status.";


            EmailService.SendEmail(agentEmail, subject, body);

            return "Reminder email sent successfully";
        }
    }
}
