using System;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Collections.Generic;

namespace IOOP_FoodPoint_Restaurant
{
    public class customerClass
    {
        private formHelper formHelper = new formHelper();

// =============== \\
// foodieOrderFood \\
// =============== \\
        public void loadMenu(DataGridView menuTable, BindingSource menuBinding, ref SqlDataAdapter dataAdapter, ref DataTable dataTable) // Load the menu items from the database into the DataGridView.
        {
// Step 1: Define the SQL query to fetch menu items.
            string fetchQuery = @"
            SELECT 
                menuID AS 'Menu ID',
                menuCategory AS 'Category',
                itemName AS 'Item',
                itemPrice AS 'Price'
            FROM Menu
            ORDER BY menuCategory, itemName";

            using (SqlConnection conn = new SqlConnection(new formHelper().connectionString))
            {
                dataAdapter = new SqlDataAdapter(fetchQuery, conn);
                dataTable = new DataTable();

// Step 2: Fill the DataTable with the results of the SQL query.
                dataAdapter.Fill(dataTable);
                menuBinding.DataSource = dataTable;
                menuTable.DataSource = menuBinding;
            }

    // Set the column widths for better readability.
            foreach (DataGridViewColumn col in menuTable.Columns)
                col.Width = 260;
        }

        public void filterMenuByCategory(string selectedCategory, DataGridView menuTable, BindingSource menuBinding) // Filter the menu items based on the selected category.
        {
// Step 1: Define the SQL query to fetch menu items based on the selected category.
            string fetchQuery = "";

            if (selectedCategory == "Select a filter.")
            {
                fetchQuery = @"
            SELECT 
                menuID AS 'Menu ID',
                menuCategory AS 'Category',
                itemName AS 'Item',
                itemPrice AS 'Price'
            FROM Menu";
            }
            else
            {
                fetchQuery = @"
            SELECT 
                menuID AS 'Menu ID',
                menuCategory AS 'Category',
                itemName AS 'Item',
                itemPrice AS 'Price'
            FROM Menu
            WHERE menuCategory = @category";
            }

            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(fetchQuery, conn);
                if (selectedCategory != "Select a filter.")
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@category", selectedCategory);
                }

                DataTable filteredMenu = new DataTable();

// Step 2: Fill the DataTable with the results of the SQL query.
                adapter.Fill(filteredMenu);
                menuBinding.DataSource = filteredMenu;
                menuTable.DataSource = menuBinding;
            }
        }

        public void orderItem(DataGridView menuTable) // Handle the ordering of a selected menu item.
        {
    // Check if a row is selected in the DataGridView.
            if (menuTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to order.");
                return;
            }

// Step 1: Get the selected row and extract the menu ID and price.
            DataGridViewRow selectedRow = menuTable.SelectedRows[0];
            int menuID = Convert.ToInt32(selectedRow.Cells["Menu ID"].Value);
            decimal price = Convert.ToDecimal(selectedRow.Cells["Price"].Value);

// Step 2: Create a pop-up form for placing the order.
            Form orderForm = formHelper.CreatePopupForm("Place Order", new Size(500, 300));

            Label lblReservation = new Label() { Text = "Select Reservation:", Location = new Point(40, 40), ForeColor = Color.White };
            ComboBox cmbReservations = new ComboBox() { Location = new Point(200, 35), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblQty = new Label() { Text = "Quantity:", Location = new Point(40, 90), ForeColor = Color.White };
            TextBox txtQty = new TextBox() { Location = new Point(200, 85), Width = 200 };

            Button btnSubmit = new Button()
            {
                Text = "Confirm Order",
                Location = new Point(200, 150),
                Width = 120,
                Height = 40,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

    // Load only active reservations for the current user.
            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
// Step 3: Fetch active reservations for the current user.
                conn.Open();
                string fetchQuery = @"
                    SELECT reservationID 
                    FROM Reservations 
                    WHERE customerID = @cid AND resStatus != 'Complete' AND reservationDate >= CAST(GETDATE() AS DATE)";

                SqlCommand cmd = new SqlCommand(fetchQuery, conn);
                cmd.Parameters.AddWithValue("@cid", UserSession.UserID);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmbReservations.Items.Add(reader["reservationID"].ToString());
                }
            }

// Step 4: Set the default selection for the ComboBox.
            btnSubmit.Click += (s, e) =>
            {
                if (cmbReservations.SelectedIndex == -1 || !int.TryParse(txtQty.Text, out int qty) || qty <= 0)
                {
                    MessageBox.Show("Please select a reservation and enter a valid quantity.");
                    return;
                }

                int reservationID = int.Parse(cmbReservations.SelectedItem.ToString());

                

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@reservationID", reservationID),
                    new SqlParameter("@menuID", menuID),
                    new SqlParameter("@totalPrice", price),
                    new SqlParameter("@orderDate", DateTime.Now),
                    new SqlParameter("@orderStatus", "Pending")
                };

// Step 5: Insert the order into the database.
                string insertQuery = @"
                    INSERT INTO Orders (reservationID, menuID, totalPrice, orderDate, orderStatus)
                    VALUES (@reservationID, @menuID, @totalPrice, @orderDate, @orderStatus)";

// Step 6: Execute the SQL command to insert the order.
                for (int i = 0; i < qty; i++)
                {
    // Create a new list to hold a fresh set of parameters.
                    List<SqlParameter> newParams = new List<SqlParameter>();

    // Copy each parameter manually to avoid reuse issues.
                    foreach (SqlParameter param in parameters)
                    {
                        SqlParameter newParam = new SqlParameter(param.ParameterName, param.SqlDbType);
                        newParam.Value = param.Value;
                        newParams.Add(newParam);
                    }

    // Convert the list to an array and execute.
                    formHelper.executeSQL(insertQuery, newParams.ToArray());
                }

                MessageBox.Show("Order placed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                orderForm.Close();
            };

    // Add controls to the pop-up form.
            orderForm.Controls.AddRange(new Control[] { lblReservation, cmbReservations, lblQty, txtQty, btnSubmit });
            orderForm.ShowDialog();
        }

// ================  \\
// foodieOrderStatus \\
// ================  \\
        public void filterOrdersByStatus(string selectedFilter, DataGridView ordersTable, BindingSource orderBinding) // Filter the orders based on the selected status.
        {
// Step 1: Get the current logged-in user's ID (make sure you get this from your actual session)
            int currentUserID = UserSession.UserID;

            string fetchQuery = "";

// Step 2: Define the SQL query to fetch orders based on the selected status.
            if (selectedFilter == "Complete")
            {
                fetchQuery = @"
                SELECT 
                    o.orderID AS 'Order ID',
                    o.reservationID AS 'Reservation ID',
                    m.itemName AS 'Item',
                    o.totalPrice AS 'Price',
                    FORMAT(o.orderDate, 'yyyy-MM-dd') AS 'Date',
                    o.orderStatus AS 'Status'
                FROM Orders o
                JOIN Menu m ON o.menuID = m.menuID
                JOIN Reservations r ON o.reservationID = r.reservationID
                WHERE o.orderStatus = 'Complete' 
                AND r.customerID = @userID
                ORDER BY o.orderDate DESC";
            }
            else if (selectedFilter == "Active")
            {
                fetchQuery = @"
                SELECT 
                    o.orderID AS 'Order ID',
                    o.reservationID AS 'Reservation ID',
                    m.itemName AS 'Item',
                    o.totalPrice AS 'Price',
                    FORMAT(o.orderDate, 'yyyy-MM-dd') AS 'Date',
                    o.orderStatus AS 'Status'
                FROM Orders o
                JOIN Menu m ON o.menuID = m.menuID
                JOIN Reservations r ON o.reservationID = r.reservationID
                WHERE o.orderStatus != 'Complete' 
                AND o.orderDate >= CAST(GETDATE() AS DATE)
                AND r.customerID = @userID
                ORDER BY o.orderDate DESC";
            }
            else if (selectedFilter == "Missed")
            {
                fetchQuery = @"
                SELECT 
                    o.orderID AS 'Order ID',
                    o.reservationID AS 'Reservation ID',
                    m.itemName AS 'Item',
                    o.totalPrice AS 'Price',
                    FORMAT(o.orderDate, 'yyyy-MM-dd') AS 'Date',
                    o.orderStatus AS 'Status'
                FROM Orders o
                JOIN Menu m ON o.menuID = m.menuID
                JOIN Reservations r ON o.reservationID = r.reservationID
                WHERE o.orderStatus != 'Complete' 
                AND o.orderDate < CAST(GETDATE() AS DATE)
                AND r.customerID = @userID
                ORDER BY o.orderDate DESC";
            }
            else // "Select a filter."
            {
                fetchQuery = @"
                SELECT 
                    o.orderID AS 'Order ID',
                    o.reservationID AS 'Reservation ID',
                    m.itemName AS 'Item',
                    o.totalPrice AS 'Price',
                    FORMAT(o.orderDate, 'yyyy-MM-dd') AS 'Date',
                    o.orderStatus AS 'Status'
                FROM Orders o
                JOIN Menu m ON o.menuID = m.menuID
                JOIN Reservations r ON o.reservationID = r.reservationID
                WHERE (o.orderStatus = 'Complete' OR o.orderDate < CAST(GETDATE() AS DATE))
                AND r.customerID = @userID
                ORDER BY o.orderDate DESC";
            }

            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(fetchQuery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@userID", currentUserID); // Add the current user ID as a parameter
                DataTable filteredTable = new DataTable();

// Step 3: Fill the DataTable with the results of the SQL query.
                adapter.Fill(filteredTable);
                orderBinding.DataSource = filteredTable;
                ordersTable.DataSource = orderBinding;
            }

    // Adjust column width for a consistent display.
            foreach (DataGridViewColumn col in ordersTable.Columns)
            {
                col.Width = 180;
            }
        }

// =========================== \\
// foodieViewReservationStatus \\
// =========================== \\
        public void filterReservationsByStatus(string selectedFilter, DataGridView reservationsTable, BindingSource reservationBinding) // Filter the reservations based on the selected status.
        {
            string fetchQuery = "";

// Step 1: Define the SQL query to fetch reservations based on the selected status.
            if (selectedFilter == "Complete")
            {
                fetchQuery = @"
                SELECT 
                    r.reservationID AS 'Reservation ID',
                    r.customerID AS 'Customer ID',
                    h.hallName AS 'Hall',
                    r.resPax AS 'Pax',
                    FORMAT(r.reservationDate, 'yyyy-MM-dd') AS 'Date',
                    r.resStatus AS 'Status'
                FROM Reservations r
                JOIN Halls h ON r.hallID = h.hallID
                WHERE r.customerID = @customerID AND r.resStatus = 'Complete'
                ORDER BY r.reservationDate DESC";
            }
            
            else if (selectedFilter == "Active")
            {
                fetchQuery = @"
                SELECT 
                    r.reservationID AS 'Reservation ID',
                    r.customerID AS 'Customer ID',
                    h.hallName AS 'Hall',
                    r.resPax AS 'Pax',
                    FORMAT(r.reservationDate, 'yyyy-MM-dd') AS 'Date',
                    r.resStatus AS 'Status'
                FROM Reservations r
                JOIN Halls h ON r.hallID = h.hallID
                WHERE r.customerID = @customerID AND r.resStatus != 'Complete' AND r.reservationDate >= CAST(GETDATE() AS DATE)
                ORDER BY r.reservationDate DESC";
            }
            
            else if (selectedFilter == "Missed")
            {
                fetchQuery = @"
                SELECT 
                    r.reservationID AS 'Reservation ID',
                    r.customerID AS 'Customer ID',
                    h.hallName AS 'Hall',
                    r.resPax AS 'Pax',
                    FORMAT(r.reservationDate, 'yyyy-MM-dd') AS 'Date',
                    r.resStatus AS 'Status'
                FROM Reservations r
                JOIN Halls h ON r.hallID = h.hallID
                WHERE r.customerID = @customerID AND r.resStatus != 'Complete' AND r.reservationDate < CAST(GETDATE() AS DATE)
                ORDER BY r.reservationDate DESC";
            }
            else
            {
                fetchQuery = @"
                SELECT 
                    r.reservationID AS 'Reservation ID',
                    r.customerID AS 'Customer ID',
                    h.hallName AS 'Hall',
                    r.resPax AS 'Pax',
                    FORMAT(r.reservationDate, 'yyyy-MM-dd') AS 'Date',
                    r.resStatus AS 'Status'
                FROM Reservations r
                JOIN Halls h ON r.hallID = h.hallID
                WHERE r.customerID = @customerID
                ORDER BY r.reservationDate DESC";
            }

            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(fetchQuery, conn);

                adapter.SelectCommand.Parameters.AddWithValue("@customerID", UserSession.UserID);

                DataTable filteredTable = new DataTable();

// Step 2: Fill the DataTable with the results of the SQL query.
                adapter.Fill(filteredTable);
                reservationBinding.DataSource = filteredTable;
                reservationsTable.DataSource = reservationBinding;
            }

    // Adjust column width for a consistent display.
            foreach (DataGridViewColumn col in reservationsTable.Columns)
            {
                col.Width = 174;
            }
        }

// ======================= \\
// foodieViewRequestStatus \\
// ======================= \\
        public void filterRequestsByStatus(string selectedFilter, DataGridView requestsTable, BindingSource requestBinding) // Filter the requests based on the selected status.
        {
// Step 1: Define the SQL query to fetch requests based on the selected status.
            string fetchQuery = @"
            SELECT 
                rr.requestID AS 'Request ID',
                rr.reservationID AS 'Reservation ID',
                rr.requestNote AS 'Request',
                rr.requestStatus AS 'Status',
                FORMAT(rr.requestDate, 'yyyy-MM-dd') AS 'Date'
            FROM reservationRequests rr
            JOIN Reservations r ON rr.reservationID = r.reservationID
            WHERE r.customerID = @userID";

// Step 2: Add conditions based on the selected filter.
            if (selectedFilter == "Pending")
                fetchQuery += " AND rr.requestStatus = 'Pending'";
            else if (selectedFilter == "Approved")
                fetchQuery += " AND rr.requestStatus = 'Approved'";
            else if (selectedFilter == "Denied")
                fetchQuery += " AND rr.requestStatus = 'Denied'";

            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(fetchQuery, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@userID", UserSession.UserID);

                DataTable filteredData = new DataTable();
                adapter.Fill(filteredData);

// Step 3: Bind the filtered data to the DataGridView.
                requestBinding.DataSource = filteredData;
                requestsTable.DataSource = requestBinding;
            }

    // Adjust column width for a consistent display.
            foreach (DataGridViewColumn col in requestsTable.Columns)
                col.Width = 217;
        }

// ================== \\
// foodieSendFeedback \\
// ================== \\        
        public void loadPastReservationsForFeedback(ComboBox cmbReservation) // Loads all past reservations made by the current user into the ComboBox.
        {
            cmbReservation.Items.Clear();

// Step 1: Define the SQL query to fetch past reservations.
            string fetchQuery = @"
            SELECT r.reservationID
            FROM Reservations r
            WHERE r.customerID = @userID
              AND r.reservationDate < CAST(GETDATE() AS DATE)";

// Step 2: Execute the SQL query and populate the ComboBox.
            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                SqlCommand cmd = new SqlCommand(fetchQuery, conn);
                cmd.Parameters.AddWithValue("@userID", UserSession.UserID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmbReservation.Items.Add($"Reservation #{reader["reservationID"]}");
                }
                reader.Close();
            }
        }

        public void submitFeedback(string selectedReservation, string feedbackText) // Submits the feedback for a selected reservation.
        {
    // Validate the input.
            if (string.IsNullOrWhiteSpace(selectedReservation) || string.IsNullOrWhiteSpace(feedbackText))
            {
                MessageBox.Show("Please select a reservation and write your feedback.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

// Step 1: Extract the reservation ID from the selected item.
            int reservationID = int.Parse(selectedReservation.Split('#')[1]);

// Step 2: Define the SQL query to insert feedback.
            string insertQuery = "INSERT INTO Feedback (reservationID, feedbackText) VALUES (@resID, @feedback)";
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@resID", reservationID),
            new SqlParameter("@feedback", feedbackText.Trim())
            };

// Step 3: Execute the SQL command to insert the feedback.
            formHelper.executeSQL(insertQuery, parameters);

            MessageBox.Show("Thank you for your feedback!", "Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ================== \\
        // foodieSendRequest  \\
        // ================== \\
        public void loadUpcomingReservationsForRequest(ComboBox cmbReservation) // Loads all upcoming reservations made by the current user into the ComboBox.
        {
            cmbReservation.Items.Clear();

// Step 1: Define the SQL query to fetch upcoming reservations.
            string fetchQuery = @"
            SELECT r.reservationID
            FROM Reservations r
            WHERE r.customerID = @userID
            AND (
                r.reservationDate >= CAST(GETDATE() AS DATE)
                OR r.resStatus != 'Complete'
              )
            ";

// Step 2: Execute the SQL query and populate the ComboBox.
            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(fetchQuery, conn);
                cmd.Parameters.AddWithValue("@userID", UserSession.UserID);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmbReservation.Items.Add($"Reservation #{reader["reservationID"]}");
                }
            }
        }

        public void submitReservationRequest(ComboBox cmbReservation, TextBox txtRequest) // Submits a request for a selected reservation.
        {
    // Validate the input.
            if (cmbReservation.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtRequest.Text))
            {
                MessageBox.Show("Please select a reservation and enter a request message.", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

// Step 1: Extract the reservation ID from the selected item.
            int reservationID = int.Parse(cmbReservation.SelectedItem.ToString().Replace("Reservation #", "").Trim());
            string requestText = txtRequest.Text.Trim();

            string insertQuery = @"
            INSERT INTO reservationRequests (reservationID, requestNote, requestDate)
            VALUES (@resID, @note, CAST(GETDATE() AS DATE))";

            SqlParameter[] parameters = {
            new SqlParameter("@resID", reservationID),
            new SqlParameter("@note", requestText)
    };

// Step 2: Execute the SQL command to insert the request.
            formHelper.executeSQL(insertQuery, parameters);
            MessageBox.Show("Request sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
