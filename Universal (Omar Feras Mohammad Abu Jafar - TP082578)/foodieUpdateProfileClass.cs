using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Universal__Omar_Feras_Mohammad_Abu_Jafar___TP082578_
{
    internal class foodieUpdateProfileClass
    {

        public void loadUserProfileData(int userID, TextBox txtFirstName, TextBox txtLastName, TextBox txtEmail, TextBox txtNumber) // Loads the user's data into the TextBoxes.
        {
// Step 1: Prepare the SQL query to fetch user details based on userID.
            string fetchQuery = "SELECT firstName, lastName, userEmail, phoneNumber FROM Users WHERE userID = @id";

// Step 2: Execute the query with the provided userID.
            using (SqlConnection conn = new SqlConnection(new formHelper().connectionString))
            using (SqlCommand cmd = new SqlCommand(fetchQuery, conn))
            {
                cmd.Parameters.AddWithValue("@id", userID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

// Step 3: Check if the user exists and retrieve their details.
                if (reader.Read())
                {
// Step 4: Populate the TextBoxes with the user's data.
                    txtFirstName.Text = reader["firstName"].ToString();
                    txtLastName.Text = reader["lastName"].ToString();
                    txtEmail.Text = reader["userEmail"].ToString();
                    txtNumber.Text = reader["phoneNumber"].ToString();
                }
            }
        }

        public bool updateUserProfile(int userID, string firstName, string lastName, string email, string phone) // Updates the user's profile data.
        {
    // Validate the input fields.
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("All fields must be filled out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

    // Validate the email and ensure it is a valid format.
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

    // Validate phone number and ensure its a valid format.
            if (!Regex.IsMatch(phone, @"^01\d{8}$"))
            {
                MessageBox.Show("Invalid phone number format. It should be in the format 01XXXXXXXX.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

// Step 1: Prepare the SQL query to update user details based on userID.
            string updateQuery = @"
            UPDATE Users 
            SET firstName = @first, lastName = @last, userEmail = @email, phoneNumber = @phone
            WHERE userID = @id";

    // Create an array of SqlParameter objects to hold the parameters for the SQL query.
            SqlParameter[] parameters = {
            new SqlParameter("@first", firstName),
            new SqlParameter("@last", lastName),
            new SqlParameter("@email", email),
            new SqlParameter("@phone", phone),
            new SqlParameter("@id", userID)
            };

            try
            {
// Step 2: Execute the update query with the provided parameters.
                new formHelper().executeSQL(updateQuery, parameters);
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
    // Handle any exceptions that occur during the update process.
                MessageBox.Show("An error occurred while updating the profile: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }



    }
}
