using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Claim_Stuff.Models
{
    public class claim_query
    {
        private string connection = @"Server=(localdb)\poe_part2;Database=claim_stuff;Trusted_Connection=True;";

        public void create_table()
        {
            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();
                string query = @"
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Claim' AND xtype='U')
CREATE TABLE Claim(
    CLAIMID INT IDENTITY(1,1) PRIMARY KEY,
    MONTH VARCHAR(30),
    HOURS INT,
    RATE DECIMAL(10,2),
    TOTALAMOUNT DECIMAL(10,2),
    DOCUMENTNAME VARCHAR(255),
    STATUS VARCHAR(30)
);";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.ExecuteNonQuery();
            }
        }

        public void store_claim(string month, int hour, decimal rate, decimal totalAmount, string documentName, string status)
        {
            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();
                string query = @"INSERT INTO Claim 
(MONTH, HOURS, RATE, TOTALAMOUNT, DOCUMENTNAME, STATUS)
VALUES (@month,@hours,@rate,@total,@doc,@status)";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@hours", hour);
                cmd.Parameters.AddWithValue("@rate", rate);
                cmd.Parameters.AddWithValue("@total", totalAmount);
                cmd.Parameters.AddWithValue("@doc", documentName);
                cmd.Parameters.AddWithValue("@status", status);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Claims> get_all_claims()
        {
            List<Claims> claims = new List<Claims>();

            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Claim", connect);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        claims.Add(new Claims
                        {
                            ClaimId = Convert.ToInt32(reader["CLAIMID"]),
                            month = reader["MONTH"].ToString(),
                            hours = Convert.ToInt32(reader["HOURS"]),
                            rate = Convert.ToDecimal(reader["RATE"]),
                            totalAmount = Convert.ToDecimal(reader["TOTALAMOUNT"]),
                            documentName = reader["DOCUMENTNAME"].ToString(),
                            status = reader["STATUS"].ToString()
                        });
                    }
                }
            }

            return claims;
        }
    }
}
