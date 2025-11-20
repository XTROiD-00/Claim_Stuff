using System.Data.SqlClient;

namespace Claim_Stuff.Models
{
    public class sql_queries_register
    {

        //connection string
        private string connection_string = @"Server=(localdb)\poe_part2;Database=claim_stuff;";

        //Method to create user table
        public void create_table()
        {
            //try and catch for error handling
            try
            {
                //connect first, to open the port.
                //using the using function
                using (SqlConnection connect = new SqlConnection(connection_string))
                {
                    //opening the connection
                    connect.Open();
                    //temp variable to hold query
                    string query = @"CREATE TABLE Register(
                                     REGISTERID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                                     NAME VARCHAR(100) NOT NULL,
                                     Email VARCHAR(100) NOT NULL,
                                     USERNAME CHAR(50) NOT NULL,
                                     PASSWORD CHAR(100) NOT NULL,
                                     ROLE VARCHAR(30) NOT NULL
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
        public void store_user(string name, string email, string username, string password, string role)
        {

            //try and catch for error handling
            try
            {

                //connect first, to open the port.
                //using the using function
                using (SqlConnection connect = new SqlConnection(connection_string))
                {

                    //opening the connection
                    connect.Open();

                    //temp variable to hold query
                    string insert_query = @"INSERT INTO Register VALUES
                                   ('" + name + "', '" + email + "','" + username + "','" + password + "','" + role + "')";

                    //Use the SQLCommand class ro run the Query
                    using (SqlCommand store_users = new SqlCommand(insert_query/*variable name*/, connect/*object name*/))
                    {

                        //run the query
                        store_users.ExecuteNonQuery();
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
        public bool Login_user(string username, string password, string role)
        {
            bool found = false;

            //try and catch for error handling
            try
            {
                //connect first, to open the port.
                //using the using function
                using (SqlConnection connect = new SqlConnection(connection_string))
                {
                    //opening the connection
                    connect.Open();
                    //temp variable to hold query
                    string select_query = @"SELECT * FROM Register
                                   WHERE Register.USERNAME = '" + username + "' AND Register.PASSWORD = '" + password + "' AND Register.ROLE = '" + role + "' ";

                    //Use the SQLCommand class ro run the Query
                    using (SqlCommand login_users = new SqlCommand(select_query/*variable name*/, connect/*object name*/))
                    {
                        //run the query
                        login_users.ExecuteNonQuery();
                        //displaying the success message 
                        using (SqlDataReader finds = login_users.ExecuteReader())
                        {

                            Console.WriteLine("user found***");
                            while (finds.Read())
                            {

                                Console.WriteLine(finds["REGISTERID"]);
                                Console.WriteLine(finds["NAME"]);
                                Console.WriteLine(finds["USERNAME"]);
                                Console.WriteLine(finds["ROLE"]);
                                found = true;

                            }
                        }
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
            return found;
        }
    }
}
