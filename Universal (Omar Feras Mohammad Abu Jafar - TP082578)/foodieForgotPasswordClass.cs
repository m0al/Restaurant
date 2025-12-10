using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;
using RestSharp.Authenticators;

namespace IOOP_FoodPoint_Restaurant
{
    public class foodieForgotPasswordClass
    {
        private string connectionString = new formHelper().connectionString;

        public string UserEmail { get; private set; } // User's email address.
        public string UserFullName { get; private set; } // User's full name.
        public string VerificationCode { get; private set; } // Verification code sent to the user.

        public async Task<bool> getUserEmailAsync(string userInput) // This method checks if the user exists in the database based on their ID, email, or phone number.
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

// Step 1: Prepare the SQL query to fetch user details based on ID, email, or phone number.
                    string fetchQuery = @"
                        SELECT firstName, lastName, userEmail
                        FROM users
                        WHERE 
                            (TRY_CAST(@input AS INT) IS NOT NULL AND userID = TRY_CAST(@input AS INT))
                            OR userEmail = @input
                            OR phoneNumber = @input";

// Step 2: Execute the query with the provided user input.
                    using (SqlCommand cmd = new SqlCommand(fetchQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@input", userInput);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
// Step 3: Check if the user exists and retrieve their email and name.
                            if (await reader.ReadAsync())
                            {
                                UserEmail = reader["userEmail"].ToString();
                                UserFullName = $"{reader["firstName"]} {reader["lastName"]}";
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
    // Handle any exceptions that occur during database access.
                MessageBox.Show("Error while accessing the database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public async Task<bool> sendVerificationEmailAsync() // This method sends a verification code to the user's email address using Mailgun API.
        {
            try
            {
// Step 1: Generate a random verification code.
                Random random = new Random();
                VerificationCode = random.Next(100000, 999999).ToString();

// Step 2: Set up the Mailgun API client.
                var clientOptions = new RestClientOptions("https://api.mailgun.net/v3/sandboxdd3b81a816dd4b7383069e40df93a6fb.mailgun.org")
                {
                    Authenticator = new HttpBasicAuthenticator("api", "81b00e0b00d9fc21bff518c98e491de6-24bda9c7-939b941e")
                };

// Step 3: Create a new RestClient instance and prepare the email request.
                var client = new RestClient(clientOptions);
                var request = new RestRequest("/messages", Method.Post);
                request.AlwaysMultipartFormData = true;
                request.AddParameter("from", "FoodiePoint <postmaster@YOUR_DOMAIN_HERE>");
                request.AddParameter("to", UserEmail);
                request.AddParameter("subject", "FoodiePoint - Password Reset Verification Code");
                request.AddParameter("text", $"Hello {UserFullName},\n\nYour verification code is: {VerificationCode}\n\nIf you did not request this, please ignore this email.");

// Step 4: Execute the request and check for success.
                var response = await client.ExecuteAsync(request);
                return response.IsSuccessful;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send email: " + ex.Message, "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool verifyCode(string enteredCode) // This method verifies the code entered by the user against the generated verification code.
        {
            return enteredCode == VerificationCode;
        }

        public async Task<bool> ResetPasswordAsync(string newPassword) // This method updates the user's password in the database.
        {
            try {
            
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

// Step 1: Create the update query to change the user's password.
                    string updateQuery = "UPDATE users SET userPassword = @password WHERE userEmail = @userEmail";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
// Step 2: Add parameters to the command to prevent SQL injection.
                        cmd.Parameters.AddWithValue("@password", newPassword);
                        cmd.Parameters.AddWithValue("@userEmail", UserEmail);

// Step 3: Execute the command and check if any rows were affected.
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
    // Handle any exceptions that occur during database access.
                MessageBox.Show("Error while updating password: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
