using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace IOOP_FoodPoint_Restaurant.Admin__Rinad_Salah_Mohammed_Alhuraibi___TP080052_
{
    internal class adminClass
    {
        formHelper formHelper = new formHelper();

// ================= \\
// foodieManageUsers \\
// ================= \\
        public void loadUsers(DataGridView usersTable, BindingSource userBinding) // This method loads user data from the database into a DataGridView.
        {
// Step 1: Define the SQL query to fetch user data.
            string fetchQuery = @"
            SELECT 
                userID AS [User ID],
                firstName AS [First Name],
                lastName AS [Last Name],
                userEmail AS [Email],
                phoneNumber AS [Phone],
                userRole AS [Role],
                FORMAT(createdAt, 'yyyy-MM-dd') AS [Created]
            FROM Users";

            using (SqlConnection conn = new SqlConnection(new formHelper().connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(fetchQuery, conn);
                DataTable userTable = new DataTable();

// Step 2: Fill the DataTable with the results of the SQL query.
                adapter.Fill(userTable);
                userBinding.DataSource = userTable;
                usersTable.DataSource = userBinding;
            }

    // Adjust the DataGridView properties for better visibility.
            foreach (DataGridViewColumn col in usersTable.Columns)
                col.Width = 160;
        }

        public void addUser(Action reloadUsers) // This method creates a pop-up form to add a new user.
        {
// Step 1: Create a new pop-up form with a specific size and title.
            Form popup = formHelper.CreatePopupForm("Add User", new Size(600, 500));
            int formWidth = popup.ClientSize.Width;
            int controlWidth = 280;
            int labelX = (formWidth - controlWidth) / 2 - 130;
            int inputX = (formWidth - controlWidth) / 2;
            int spacingY = 40;
            int currentY = 30;

    // Create labels and input fields for user details.
            Label lblFirst = new Label() { Text = "First Name:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            TextBox txtFirst = new TextBox() { Location = new Point(inputX, currentY), Width = controlWidth };
            currentY += spacingY;

            Label lblLast = new Label() { Text = "Last Name:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            TextBox txtLast = new TextBox() { Location = new Point(inputX, currentY), Width = controlWidth };
            currentY += spacingY;

            Label lblEmail = new Label() { Text = "Email:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            TextBox txtEmail = new TextBox() { Location = new Point(inputX, currentY), Width = controlWidth };
            currentY += spacingY;

            Label lblPhone = new Label() { Text = "Number:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            TextBox txtPhone = new TextBox() { Location = new Point(inputX, currentY), Width = controlWidth };
            currentY += spacingY;

            Label lblPassword = new Label() { Text = "Password:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            TextBox txtPassword = new TextBox() { Location = new Point(inputX, currentY), Width = controlWidth };
            currentY += spacingY;

            Label lblRole = new Label() { Text = "Role:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            ComboBox cmbRole = new ComboBox()
            {
                Location = new Point(inputX, currentY),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new[] { "Admin", "Manager", "Chef", "Reservation Coordinator", "Customer" });
            currentY += spacingY;

    // Add button.
            Button btnSubmit = new Button()
            {
                Text = "Add User",
                Width = 200,
                Height = 45,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Location = new Point((formWidth - 200) / 2, currentY + 20)
            };

// Step 2: Add a click event handler for the "Add User" button.
            btnSubmit.Click += (s, e) =>
            {
    // Validate input fields.
                if (cmbRole.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtFirst.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Please fill all required fields.");
                    return;
                }

// Step 3: Check if the email and phone number already exist in the database.
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE userEmail = @userEmail OR phoneNumber = @phoneNumber";
                using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
                {
                    SqlCommand cmd = new SqlCommand(checkQuery, conn);
                    cmd.Parameters.AddWithValue("@userEmail", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@phoneNumber", txtPhone.Text.Trim());
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Email or phone number already exists.");
                        return;
                    }
                }

    // Proceed with insertion.
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@first", txtFirst.Text.Trim()),
                new SqlParameter("@last", txtLast.Text.Trim()),
                new SqlParameter("@email", txtEmail.Text.Trim()),
                new SqlParameter("@pass", txtPassword.Text.Trim()),
                new SqlParameter("@phone", txtPhone.Text.Trim()),
                new SqlParameter("@role", cmbRole.SelectedItem.ToString())
                };

// Step 4: Execute the SQL command to insert the new user into the database.
                formHelper.executeSQL(@"
                INSERT INTO Users (firstName, lastName, userEmail, userPassword, phoneNumber, userRole)
                VALUES (@first, @last, @email, @pass, @phone, @role)", parameters);

                MessageBox.Show("User added successfully!");
                popup.Close();
                reloadUsers();
            };

    // Add controls to popup.
            popup.Controls.AddRange(new Control[] {
            lblFirst, txtFirst,
            lblLast, txtLast,
            lblEmail, txtEmail,
            lblPhone, txtPhone,
            lblPassword, txtPassword,
            lblRole, cmbRole,
            btnSubmit
            });

            popup.ShowDialog();
        }

        public void editUser(DataGridView usersTable, Action reloadUsers) // This method creates a pop-up form to edit an existing user.
        {
    // Check if a user is selected in the DataGridView.
            if (usersTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to edit.");
                return;
            }

// Step 1: Fetch current password from the database
            DataGridViewRow selectedRow = usersTable.SelectedRows[0];
            int userID = Convert.ToInt32(selectedRow.Cells["User ID"].Value);

    // Get the current password for the selected user.
            string currentPassword = "";
            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                string getPasswordQuery = "SELECT userPassword FROM Users WHERE userID = @id";
                SqlCommand cmd = new SqlCommand(getPasswordQuery, conn);
                cmd.Parameters.AddWithValue("@id", userID);
                conn.Open();
                currentPassword = cmd.ExecuteScalar().ToString();
            }

// Step 2: Create the edit popup.
            Form popup = formHelper.CreatePopupForm("Edit User", new Size(600, 550));
            int formWidth = popup.ClientSize.Width;
            int controlWidth = 280;
            int labelX = (formWidth - controlWidth) / 2 - 130;
            int inputX = (formWidth - controlWidth) / 2;
            int spacingY = 40;
            int currentY = 30;

    // Create labels and input fields for user details.
            Label lblFirst = new Label() { Text = "First Name:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            TextBox txtFirst = new TextBox() { Location = new Point(inputX, currentY), Width = controlWidth, Text = selectedRow.Cells["First Name"].Value.ToString() };
            currentY += spacingY;

            Label lblLast = new Label() { Text = "Last Name:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            TextBox txtLast = new TextBox() { Location = new Point(inputX, currentY), Width = controlWidth, Text = selectedRow.Cells["Last Name"].Value.ToString() };
            currentY += spacingY;

            Label lblEmail = new Label() { Text = "Email:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            TextBox txtEmail = new TextBox() { Location = new Point(inputX, currentY), Width = controlWidth, Text = selectedRow.Cells["Email"].Value.ToString() };
            currentY += spacingY;

            Label lblPhone = new Label() { Text = "Number:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            TextBox txtPhone = new TextBox() { Location = new Point(inputX, currentY), Width = controlWidth, Text = selectedRow.Cells["Phone"].Value.ToString() };
            currentY += spacingY;

            Label lblPassword = new Label() { Text = "Password:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            TextBox txtPassword = new TextBox() { Location = new Point(inputX, currentY), Width = controlWidth, Text = currentPassword };
            currentY += spacingY;

            Label lblRole = new Label() { Text = "Role:", Location = new Point(labelX, currentY), AutoSize = true, ForeColor = Color.White };
            ComboBox cmbRole = new ComboBox()
            {
                Location = new Point(inputX, currentY),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new[] { "Admin", "Manager", "Chef", "Reservation Coordinator", "Customer" });
            cmbRole.SelectedItem = selectedRow.Cells["Role"].Value.ToString();
            currentY += spacingY;

            Button btnUpdate = new Button()
            {
                Text = "Edit User",
                Width = 200,
                Height = 45,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Location = new Point((formWidth - 200) / 2, currentY + 20)
            };

// Step 3: Add a click event handler for the "Edit User" button.
            btnUpdate.Click += (s, e) =>
            {
    // Validate input fields.
                if (cmbRole.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtFirst.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Please fill all required fields.");
                    return;
                }

// Step 4: Check if the email and phone number already exist in the database.
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE userEmail = @userEmail OR phoneNumber = @phoneNumber";
                using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
                {
                    SqlCommand cmd = new SqlCommand(checkQuery, conn);
                    cmd.Parameters.AddWithValue("@userEmail", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@phoneNumber", txtPhone.Text.Trim());
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Email or phone number already exists.");
                        return;
                    }
                }

// Step 5: Proceed with the update.
                string updateQuery = @"
                UPDATE Users
                SET firstName = @first, lastName = @last, userEmail = @email,
                    userPassword = @pass, phoneNumber = @phone, userRole = @role
                WHERE userID = @id";

                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@first", txtFirst.Text),
                new SqlParameter("@last", txtLast.Text),
                new SqlParameter("@email", txtEmail.Text),
                new SqlParameter("@pass", txtPassword.Text),
                new SqlParameter("@phone", txtPhone.Text),
                new SqlParameter("@role", cmbRole.SelectedItem.ToString()),
                new SqlParameter("@id", userID)
                };

// Step 6: Execute the SQL command to update the user in the database.
                formHelper.executeSQL(updateQuery, parameters);
                MessageBox.Show("User updated!");
                popup.Close();
                reloadUsers();
            };

    // Add labels and controls
            popup.Controls.AddRange(new Control[] {
            lblFirst, txtFirst,
            lblLast, txtLast,
            lblEmail, txtEmail,
            lblPhone, txtPhone,
            lblPassword, txtPassword,
            lblRole, cmbRole,
            btnUpdate
            });

            popup.ShowDialog();
        }

        public void deleteUser(DataGridView usersTable, Action reloadUsers) // This method deletes a user from the database.
        {
    // Check if a user is selected in the DataGridView.
            if (usersTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to delete.");
                return;
            }

// Step 1: Get the selected user ID from the DataGridView.
            int userID = Convert.ToInt32(usersTable.SelectedRows[0].Cells["User ID"].Value);
            DialogResult confirm = MessageBox.Show($"Are you sure you want to delete user ID {userID}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

    // If confirmed, proceed to delete the user.
            if (confirm == DialogResult.Yes)
            {
    // Delete the user and all references across tables to maintain data integrity.
                var queries = new (string, SqlParameter[])[]
                {
                ("DELETE FROM Orders WHERE reservationID IN (SELECT reservationID FROM Reservations WHERE customerID = @id)", new[] { new SqlParameter("@id", userID) }),
                ("DELETE FROM reservationRequests WHERE reservationID IN (SELECT reservationID FROM Reservations WHERE customerID = @id)", new[] { new SqlParameter("@id", userID) }),
                ("DELETE FROM Feedback WHERE reservationID IN (SELECT reservationID FROM Reservations WHERE customerID = @id)", new[] { new SqlParameter("@id", userID) }),
                ("DELETE FROM Reservations WHERE customerID = @id", new[] { new SqlParameter("@id", userID) }),
                ("DELETE FROM Users WHERE userID = @id", new[] { new SqlParameter("@id", userID) })
                };

// Step 2: Execute the SQL commands to delete the user and their related data.
                formHelper.executeMultipleSQL(queries);
                MessageBox.Show("User deleted and related data removed.");
                reloadUsers();
            }
        }

// ====================== \\
// foodieViewSalesReport  \\
// ====================== \\
        public void loadSalesReport(DataGridView salesTable, string filterCategory = "", string filterValue = "") // This method loads sales data into a DataGridView.
        {
            string fetchQuery = "";

// Step 1: Determine the SQL query based on the selected filter category.
            if (filterCategory == "By Menu Item")
            {
                fetchQuery = @"
                SELECT 
                    m.menuID AS [Item ID],
                    m.itemName AS [Name],
                    SUM(o.totalPrice) AS [Total]
                FROM Orders o
                JOIN Menu m ON o.menuID = m.menuID
                WHERE FORMAT(o.orderDate, 'MMMM yyyy') = @filterValue
                GROUP BY m.menuID, m.itemName
                ORDER BY SUM(o.totalPrice) DESC";
            }
            else if (filterCategory == "By Orders")
            {
                fetchQuery = @"
                SELECT 
                    o.orderID AS [Order ID],
                    m.itemName AS [Item(s)],
                    o.totalPrice AS [Total],
                    FORMAT(o.orderDate, 'yyyy-MM-dd') AS [Date]
                FROM Orders o
                JOIN Menu m ON o.menuID = m.menuID
                WHERE FORMAT(o.orderDate, 'MMMM yyyy') = @filterValue
                ORDER BY o.orderDate DESC";
            }
            else
            {
                fetchQuery = @"
                SELECT 
                    o.orderID AS [Order ID],
                    m.itemName AS [Item(s)],
                    o.totalPrice AS [Total],
                    FORMAT(o.orderDate, 'yyyy-MM-dd') AS [Date]
                FROM Orders o
                JOIN Menu m ON o.menuID = m.menuID
                ORDER BY o.orderDate DESC";
            }

// Step 2: Create a connection to the database and execute the SQL query.
            using (SqlConnection conn = new SqlConnection(new formHelper().connectionString))
            using (SqlCommand cmd = new SqlCommand(fetchQuery, conn))
            {
                if (!string.IsNullOrEmpty(filterCategory) && filterCategory != "Select a filter." && !string.IsNullOrEmpty(filterValue))
                    cmd.Parameters.AddWithValue("@filterValue", filterValue);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable resultTable = new DataTable();

// Step 3: Fill the DataTable with the results of the SQL query.
                adapter.Fill(resultTable);
                salesTable.DataSource = resultTable;
            }

    // Adjust columns width based on the selected filter category.
            if (filterCategory == "By Menu Item")
            {
                foreach (DataGridViewColumn col in salesTable.Columns)
                    col.Width = 346;
            }
            else // Default for "By Orders" and "Select a filter."
            {
                foreach (DataGridViewColumn col in salesTable.Columns)
                    col.Width = 260;
            }
        }

        public void loadSalesMonths(ComboBox cmbFilter) // Loads the last 12 months into the ComboBox.
        {
    // Set the ComboBox to display the last 12 months.
            cmbFilter.Items.Clear();
            for (int i = 0; i < 12; i++)
            {
                DateTime month = DateTime.Now.AddMonths(-i);
                cmbFilter.Items.Add(month.ToString("MMMM yyyy"));
            }

            if (cmbFilter.Items.Count > 0)
                cmbFilter.SelectedIndex = 0;
        }

        public void loadSalesFilterOptions(string filterCategory, ComboBox cmbFilter) // Loads filter options based on the selected category.
        {
    // Clear the ComboBox and enable it.
            cmbFilter.Items.Clear();
            cmbFilter.Enabled = true;

    // Load the filter options based on the selected category.
            if (filterCategory == "By Menu Item" || filterCategory == "By Orders")
            {
                loadSalesMonths(cmbFilter);
            }
            else
            {
                cmbFilter.Enabled = false;
            }
        }

        // ================== \\
        // foodieViewFeedback \\
        // ================== \\
        public void loadFeedback(DataGridView feedbackTable, string filterCategory = "", string filterValue = "") // This method loads feedback data into a DataGridView.
        {
// Step 1: Define the SQL query to fetch feedback data.
            string fethQuery = @"
            SELECT 
                f.feedbackID AS [Feedback ID],
                f.reservationID AS [Reservation ID],
                '(' + CAST(u.userID AS VARCHAR) + ') ' + u.firstName + ' ' + u.lastName AS [Customer],
                f.feedbackText AS [Comment],
                FORMAT(f.createdAt, 'yyyy-MM-dd') AS [Date]
            FROM Feedback f
            JOIN Reservations r ON f.reservationID = r.reservationID
            JOIN Users u ON r.customerID = u.userID
";

    // Add filtering by month if selected.
            if (filterCategory == "By Month" && !string.IsNullOrEmpty(filterValue))
            {
                fethQuery += "WHERE FORMAT(f.createdAt, 'MMMM yyyy') = @filterValue ";
            }

            fethQuery += "ORDER BY f.createdAt DESC";

            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            using (SqlCommand cmd = new SqlCommand(fethQuery, conn))
            {
    // Declare the parameter if needed.
                if (filterCategory == "By Month" && !string.IsNullOrEmpty(filterValue))
                {
                    cmd.Parameters.AddWithValue("@filterValue", filterValue);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

// Step 2: Fill the DataTable with the results of the SQL query.
                adapter.Fill(dt);
                feedbackTable.DataSource = dt;

    // Adjust column widths for readability
                foreach (DataGridViewColumn col in feedbackTable.Columns)
                    col.Width = 213;
            }
        }

        public void loadFeedbackMonths(ComboBox cmbFilter) // Loads the last 12 months into the ComboBox.
        {
    // Clear the ComboBox.
            cmbFilter.Items.Clear();
    
    // Set the ComboBox to display the last 12 months.
            for (int i = 0; i < 12; i++)
            {
                DateTime month = DateTime.Now.AddMonths(-i);
                cmbFilter.Items.Add(month.ToString("MMMM yyyy"));
            }

    // Select the first item if available.
            if (cmbFilter.Items.Count > 0)
                cmbFilter.SelectedIndex = 0;
        }

        public void loadFeedbackFilterOptions(string selectedCategory, ComboBox cmbFilter) // Loads filter options based on the selected category.
        {
    // Clear the ComboBox and enable it.
            cmbFilter.Items.Clear();
            cmbFilter.Enabled = true;

    // Load the filter options based on the selected category.
            if (selectedCategory == "By Month")
            {
                loadFeedbackMonths(cmbFilter);
            }
            else
            {
                cmbFilter.Enabled = false;
            }
        }

    }
}
