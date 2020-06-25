using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Testserver
{
    class DbController
    {
        private string connectionString = "Data Source=iot.database.windows.net;Initial Catalog=casus11;User ID=iot;Password=Wachtwoord@01;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public int CreateSessionFor(int patientID, DateTime date)
        {
            string tbl = "tbl_Session";
            string clmn_PatientID = "PatientID";
            string clmn_Date = "Date";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO " + tbl + "(" + clmn_PatientID + ", " + clmn_Date + ") VALUES(@" + clmn_PatientID + ", @" + clmn_Date + "); SELECT CAST(scope_identity() AS int)";
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@" + clmn_PatientID + "", patientID);
                    cmd.Parameters.AddWithValue("@" + clmn_Date + "", date);

                    int modified = (int)cmd.ExecuteScalar();

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    return modified;
                }
            }
        }

        public int CreateGameDataFor(int sessionID)
        {
            string tbl = "tbl_Data";
            string clmn_SessionID = "SessionID";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO " + tbl + "(" + clmn_SessionID + ") VALUES(@" + clmn_SessionID + "); SELECT CAST(scope_identity() AS int)";
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@" + clmn_SessionID + "", sessionID);

                    int modified = (int)cmd.ExecuteScalar();

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    return modified;
                }
            }
        }

        public void CreateSpeedDataFor(int gameDataID, DateTime triggered, DateTime pressed, float distance)
        {
            string tbl = "tbl_SpeedData";
            string clmn_GameDataID = "DataID";
            string clmn_TimeTriggered = "TimeTriggered";
            string clmn_TimePressed = "TimePressed";
            string clmn_Distance = "Distance";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO " + tbl + "(" + clmn_GameDataID + ", " + clmn_TimeTriggered + ", " + clmn_TimePressed + ", " + clmn_Distance + ") " +
                          "VALUES(@" + clmn_GameDataID + ", @" + clmn_TimeTriggered + ", @" + clmn_TimePressed + ", @" + clmn_Distance + ")";
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@" + clmn_GameDataID + "", gameDataID);
                    cmd.Parameters.AddWithValue("@" + clmn_TimeTriggered + "", triggered);
                    cmd.Parameters.AddWithValue("@" + clmn_TimePressed + "", pressed);
                    cmd.Parameters.AddWithValue("@" + clmn_Distance + "", distance);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CreateForceDataFor(int gameDataID, float force)
        {
            string tbl = "tbl_ForceData";
            string clmn_GameDataID = "DataID";
            string clmn_Force = "Force";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO " + tbl + "(" + clmn_GameDataID + ", " + clmn_Force + ") VALUES(@" + clmn_GameDataID + ", @" + clmn_Force + ")";
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@" + clmn_GameDataID + "", gameDataID);
                    cmd.Parameters.AddWithValue("@" + clmn_Force + "", force);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
