using System.Data.SqlClient;
using System.Security.Claims;

namespace Claim_Stuff.Models
{
    public class claim_query
    {

        //connection string
        private string connection = @"Server=(localdb)\poe_part2;Database=claim_stuff;Trusted_Connection=True;";

        //Method to create user table
        public void create_table()
        {
            //try and catch for error handling
            try
            {
                //connect first, to open the port.
                //using the using function
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    //opening the connection
                    connect.Open();
                    //temp variable to hold query
                    string query = @"CREATE TABLE Claim(
                                    CLAIMID INT IDENTITY(1,1) PRIMARY KEY,
                                    MONTH VARCHAR(30) NOT NULL,
                                    HOURS INT NOT NULL,
                                    RATE DECIMAL(10,2) NOT NULL,
                                    TOTALAMOUNT DECIMAL(10,2) NOT NULL,
                                    DOCUMENTNAME VARCHAR(255) NOT NULL,
                                    STATUS VARCHAR(30) NOT NULL
                                   );";

                    //Use the SQLCommand class ro run the Query
                    using (SqlCommand create_table = new SqlCommand(query/*variable name*/, connect/*object name*/))
                    {
                        //run the query
                        create_table.ExecuteNonQuery();
                        //then show success message
                        Console.WriteLine("Table is created");
                    }
                    //closing the connection
                    connect.Close();
                }
            }
            catch (Exception error)
            {
                //show error message
                Console.WriteLine(error.Message);
            }

        }
        public void store_claim(string month, int hour, decimal rate, decimal totalAmount, string documentName, string status)
        {

            //try and catch for error handling
            try
            {

                //connect first, to open the port.
                //using the using function
                using (SqlConnection connect = new SqlConnection(connection))
                {

                    //opening the connection
                    connect.Open();

                    //temp variable to hold query
                    string insert_query = @"INSERT INTO Claims VALUES
                                   ('" + month + "', '" + hour + "','" + rate + "','" + totalAmount + "','" + documentName + "','" + status + "')";

                    //Use the SQLCommand class ro run the Query
                    using (SqlCommand store_claims = new SqlCommand(insert_query/*variable name*/, connect/*object name*/))
                    {

                        //run the query
                        store_claims.ExecuteNonQuery();
                        //then show success message
                        Console.WriteLine("The data has been inserted successfully");


                    }

                    //closing the connection
                    connect.Close();

                }

            }
            catch (Exception error)
            {
                //show error message
                Console.WriteLine(error.Message);
            }

        }
        public List<Claims> get_all_claims()
        {
            List<Claims> claims = new List<Claims>();

            //try and catch for error handling
            try
            {
                // Connect first, to open the port.
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    // Opening the connection
                    connect.Open();

                    // Temp variable to hold the query
                    string select_query = @"SELECT * FROM Claims";

                    // Use the SqlCommand class to run the query
                    using (SqlCommand get_claims = new SqlCommand(select_query, connect))
                    {
                        // Execute the query
                        get_claims.ExecuteNonQuery();

                        // Display success message
                        Console.WriteLine("Fetching all claims...");

                        // Use SqlDataReader to read the returned data
                        using (SqlDataReader finds = get_claims.ExecuteReader())
                        {
                            while (finds.Read())
                            {
                                // Add each claim object to the list
                                claims.Add(new Claims
                                {
                                    ClaimId = Convert.ToInt32(finds["CLAIMID"]),
                                    month = finds["MONTH"].ToString(),
                                    hours = Convert.ToInt32(finds["HOURS"]),
                                    rate = Convert.ToDecimal(finds["RATE"]),
                                    totalAmount = Convert.ToDecimal(finds["TOTALAMOUNT"]),
                                    documentName = finds["DOCUMENTNAME"].ToString(),
                                    status = finds["STATUS"].ToString()
                                });
                            }
                        }
                    }
                    // Closing the connection
                    connect.Close();
                }
            }
            catch (Exception error)
            {
                // Show error message in console
                Console.WriteLine(error.Message);
            }
            // Return the list of claims
            return claims;
        }

        public List<Claims> ViewandPreApproveClaims()
        {
            // List to store claims
            List<Claims> claims = new List<Claims>();

            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    // Query to fetch all claims
                    string query = @"SELECT * FROM Claims";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        using (SqlDataReader finds = cmd.ExecuteReader())
                        {
                            while (finds.Read())
                            {
                                // Populating the claim list with the data from the reader
                                claims.Add(new Claims
                                {
                                    month = finds["MONTH"].ToString(),
                                    hours = Convert.ToInt32(finds["HOURS"]),
                                    rate = Convert.ToDecimal(finds["RATE"]),
                                    totalAmount = Convert.ToDecimal(finds["TOTALAMOUNT"]),
                                    documentName = finds["DOCUMENTNAME"].ToString(),
                                    status = finds["STATUS"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching claims: " + ex.Message);
            }

            return claims;
        }



        //a returning method that updates the claim status for the pre approval
        public bool update_claim_status(int claimId, string newStatus)
        {
            bool updated = false;
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string query = @"UPDATE Claims SET STATUS = @status WHERE CLAIMID = @id";

                    using (SqlCommand update = new SqlCommand(query, connect))
                    {
                        update.Parameters.AddWithValue("@status", newStatus);
                        update.Parameters.AddWithValue("@id", claimId);

                        int rows = update.ExecuteNonQuery();
                        updated = rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error updating claim status: " + ex.Message);

            }
            return updated;
        }

        //This method updates the table for the Final approval, it changes the status of the Claim
        public bool UpdateClaimStatusForFinalApproval(int claimId, string newStatus)
        {
            bool updated = false;
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string query = @"UPDATE Claims SET STATUS = @status WHERE CLAIMID = @id";

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@status", newStatus);
                        command.Parameters.AddWithValue("@id", claimId);

                        int rows = command.ExecuteNonQuery();
                        updated = rows > 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error updating final approval status: " + e.Message);
            }
            return updated;
        }
    }
}
