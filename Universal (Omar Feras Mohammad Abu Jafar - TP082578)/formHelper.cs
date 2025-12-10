using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public class formHelper
    {
// This string holds the connection details used to connect to the SQL Server database.
        public string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\hamle\\OneDrive\\Desktop\\IOOP FoodPoint Restaurant\\Core\\foodiePoint.mdf\";Integrated Security=True";

        public void returnToDashboard(Form currentForm) // This method is responsible for redirecting the user to the dashboard form.
        {
            foodieDashboard dashboard = new foodieDashboard(); // Create a new instance of the dashboard.
            dashboard.Show(); // Display the dashboard.
            currentForm.Close(); // Close the current form.
        }

        public Form CreatePopupForm(string formTitle, Size formSize) // This method creates a custom pop-up form with a dark theme.
        {
            /*
                    - formTitle: The title text to be displayed on the form.
                    - formSize: The size of the form window.
            */
            return new Form
            {
                Text = formTitle, // Set the title of the form.
                Size = formSize, // Set the dimensions of the form.
                StartPosition = FormStartPosition.CenterScreen, // Center the form on screen.
                FormBorderStyle = FormBorderStyle.FixedDialog, // Prevent resizing by fixing border.
                BackColor = Color.FromArgb(30, 30, 30) // Set a dark background color.
            };
        }

        public void DisplayPreview(DataGridView dataTable, TextBox txtRow, string[] oneField, string[] oneHeader) // This method displays the details of a selected row in a textbox for preview.
        {
            /*      
                    - dataTable: DataGridView containing reservation data.
                    - txtRow: TextBox to show the formatted reservation details.
                    - oneField: Column names from the database to retrieve values.
                    - oneHeader: Friendly display labels corresponding to each field. 
            */

            // Ensure at least one row is selected in the DataGridView.
            if (dataTable.SelectedRows.Count > 0)
            {
                DataGridViewRow oneRow = dataTable.SelectedRows[0]; // Get the selected row.
                string displayDetails = ""; // Initialize a string to hold formatted reservation details.

                // Loop through each field and append label + value to the display string.
                for (int i = 0; i < oneField.Length; i++)
                {
                    displayDetails += $"{oneHeader[i]}: {oneRow.Cells[oneField[i]].Value}{Environment.NewLine}";
                }

                // Set the formatted string to the TextBox.
                txtRow.Text = displayDetails;
            }
        }

        public void executeSQL(string sqlQuery, SqlParameter[] sqlParams)
        {
// Open a connection to the database.
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

// Begin a transaction to ensure all queries are executed or none are.
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
// Execute the SQL query with the provided parameters.
                    using (SqlCommand sqlCMD = new SqlCommand(sqlQuery, conn, transaction))
                    {
                        sqlCMD.Parameters.AddRange(sqlParams);
                        sqlCMD.ExecuteNonQuery();
                    }

// Commit the transaction if the query is successful.
                    transaction.Commit();
                }
                catch (Exception)
                {
// Rollback the transaction if the query fails.
                    transaction.Rollback();
                    throw;
                }
            }
        } // This method executes a single SQL query with parameters and handles transactions.

        public void executeMultipleSQL((string query, SqlParameter[] parameters)[] queries)
        {
// Open a connection to the database.
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

// Begin a transaction to ensure all queries are executed or none are.
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
// Loop through each query and execute it with the provided parameters.
                    foreach (var (query, parameters) in queries)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn, transaction))
                        {
                            command.Parameters.AddRange(parameters);
                            command.ExecuteNonQuery();
                        }
                    }

// Commit the transaction if all queries are successful.
                    transaction.Commit();
                }
                catch (Exception)
                {
// Rollback the transaction if any query fails.
                    transaction.Rollback();
                    throw;
                }
            }
        } // This method executes multiple SQL queries with parameters and handles transactions.

        public void ApplySearchFilter(BindingSource dataSource, string searchText, string[] dataColumns) // This method allows searching/filtering that is executed after every key-press in a table that is bound to a BindingSource.
        {
/*
        - dataSource: The BindingSource linked to the DataGridView or other control.
        - searchText: The text entered by the user to search.
        - dataColumns: The columns where the search should be applied. 
*/
            string searchFilter = ""; // Initialize an empty search filter string.

// Loop through each column provided for filtering/
            foreach (string oneColumn in dataColumns)
            {
// If the searchFilter already has content, add OR to join the conditions.
                if (!string.IsNullOrEmpty(searchFilter))
                    searchFilter += " OR ";

// Add a filter condition that converts each column to a string and checks if it contains the search text.
                searchFilter += $"CONVERT([{oneColumn}], 'System.String') LIKE '%{searchText}%'";
            }

// Apply the built filter string to the data source.
            dataSource.Filter = searchFilter;
        }
    }
}