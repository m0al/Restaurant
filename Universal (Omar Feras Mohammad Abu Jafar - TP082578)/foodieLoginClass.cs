using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public class foodieLoginClass
    {
    // Connection string retrieved from a helper class.
        private string connectionString = new formHelper().connectionString;

        public string verifyCredentials(string inputIdentifier, string inputPassword, Form loginForm) // This method verifies the credentials given.
        {
            try
            {
// Step 1: Connect to the SQL Server using the connection string.
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

// Step 2: SQL query that matches login based on userID, email, or phone number.
                    string loginQuery = @"
                        SELECT userRole, userID, firstName, lastName
                        FROM users
                        WHERE 
                        (
                            (TRY_CAST(@userIdentifier AS INT) IS NOT NULL AND userID = TRY_CAST(@userIdentifier AS INT))
                            OR userEmail = @userIdentifier
                            OR phoneNumber = @userIdentifier
                        )
                        AND userPassword = @userPassword";

// Step 3: Prepare the SQL command with parameters.
                    using (SqlCommand sqlCommand = new SqlCommand(loginQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@userIdentifier", inputIdentifier);
                        sqlCommand.Parameters.AddWithValue("@userPassword", inputPassword);

// Step 4: Execute the command and retrieve the results.
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
// Step 5: If a record is found, login is successful.
                            if (reader.Read())
                            {
    // Save session data in the static UserSession class.
                                UserSession.UserID = Convert.ToInt32(reader["userID"]);
                                UserSession.UserName = reader["firstName"].ToString() + " " + reader["lastName"].ToString();
                                UserSession.UserRole = reader["userRole"].ToString();

                                return UserSession.UserRole;
                            }
                            else
                            {
    // No user found - show error message and clear form fields.
                                MessageBox.Show("Invalid credentials, please check your username and password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                // Call the ClearLoginFields method from the form
                                if (loginForm is foodieLogin loginInstance)
                                {
                                    loginInstance.clearLoginFields();
                                }

                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
    // Catch any errors (e.g., SQL connection issues)
                MessageBox.Show("Error while connecting to the database:\n" + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
