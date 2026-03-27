using ShipzoBackend.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
namespace ShipzoBackend.BusinessLayer
{
    public class AgentService
    {
        private readonly Database db;
        public AgentService(Database database)
        {
            db = database;
        }
        public object RegisterAgent(Agent agent)
        {
            string newAgentId = "AGT" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            agent.AgentId = newAgentId;
            SqlParameter[] parameters =
            {
                new SqlParameter("@AgentId",agent.AgentId),
                new SqlParameter("@AgentName",agent.AgentName),

                new SqlParameter("@Email",agent.Email),
                new SqlParameter("@Phone",agent.Phone),
                new SqlParameter("@Password",agent.Password)

,            };
            DataTable result = db.ExecuteProcedure("sp_RegisterAgent", parameters);
            return new
            {
                
                Message = result.Rows[0]["Message"].ToString()
            };

        }

        public object LoginAgent(string email, string password)
        {
            SqlParameter[] parameters =
            {
        new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email },
        new SqlParameter("@Password", SqlDbType.NVarChar) { Value = password }
    };

            DataTable result = db.ExecuteProcedure("sp_LoginAgent", parameters);

            return new
            {
                AgentId = result.Rows[0]["AgentId"]?.ToString(),
                AgentName = result.Rows[0]["AgentName"]?.ToString(),
                Message = result.Rows[0]["Message"].ToString()
            };
        }
        public List<Agent> GetAllAgents()
        {
            DataTable table = db.ExecuteProcedure("sp_GetAllAgents", null);

            List<Agent> agents = new List<Agent>();

            foreach (DataRow row in table.Rows)
            {
                agents.Add(new Agent
                {
                    AgentId = row["AgentId"].ToString(),
                    AgentName = row["AgentName"].ToString(),
                    Email = row["Email"].ToString(),
                    Phone = row["Phone"].ToString()
                });
            }

            return agents;
        }
        public Agent GetAgentById(string agentId)
        {
            SqlParameter[] parameters =
            {
        new SqlParameter("@AgentId", agentId)
    };

            DataTable table = db.ExecuteProcedure("sp_GetAgentById", parameters);

            if (table.Rows.Count == 0)
                return null;

            DataRow row = table.Rows[0];

            return new Agent
            {
                AgentId = row["AgentId"].ToString(),
                AgentName = row["AgentName"].ToString(),
                Email = row["Email"].ToString(),
                Phone = row["Phone"].ToString()
            };
        }

        public string DeleteAgent(string agentId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@AgentId", agentId)
            };

            DataTable result = db.ExecuteProcedure("sp_DeleteAgent", parameters);

            return result.Rows[0]["Message"].ToString();
        }
        public List<object> GetAgentPerformance()
        {
            DataTable table = db.ExecuteProcedure("sp_GetAgentPerformance", null);

            List<object> performanceList = new List<object>();

            foreach (DataRow row in table.Rows)
            {
                if (table.Columns.Contains("Message"))
                {
                    performanceList.Add(new
                    {
                        Message = row["Message"].ToString()
                    });
                }
                else
                {
                    performanceList.Add(new
                    {
                        AgentId = row["AgentId"].ToString(),
                        Name = row["Name"].ToString(),
                        Delivered = Convert.ToInt32(row["Delivered"]),
                        InTransit = Convert.ToInt32(row["InTransit"]),
                        PickedUp = Convert.ToInt32(row["PickedUp"])
                    });
                }
            }

            return performanceList;
        }

        public object GetDashboardData(string agentId)
        {
            SqlParameter[] parameters =
            {
        new SqlParameter("@AgentId", agentId)
    };

            DataTable table = db.ExecuteProcedure("sp_GetAgentDashboard", parameters);

            if (table.Columns.Contains("Message"))
            {
                return new
                {
                    Message = table.Rows[0]["Message"].ToString()
                };
            }

            DataRow row = table.Rows[0];

            return new
            {
                CreatedToday = row["CreatedToday"] == DBNull.Value ? 0 : Convert.ToInt32(row["CreatedToday"]),
                PickedUpToday = row["PickedUpToday"] == DBNull.Value ? 0 : Convert.ToInt32(row["PickedUpToday"]),
                InTransitToday = row["InTransitToday"] == DBNull.Value ? 0 : Convert.ToInt32(row["InTransitToday"]),
                DeliveredToday = row["DeliveredToday"] == DBNull.Value ? 0 : Convert.ToInt32(row["DeliveredToday"]),
                PendingCount = row["PendingCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["PendingCount"])
            };
        }





    }
}
