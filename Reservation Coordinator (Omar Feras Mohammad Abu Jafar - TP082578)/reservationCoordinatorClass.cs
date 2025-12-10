using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public class reservationCoordinatorClass
    {

// Create a reusable helper object from formFunctions.
        private formHelper formsHelper = new formHelper();

// ========================== \\
//    foodieReviewRequests    \\
// ========================== \\

        public void loadPendingRequests(DataGridView requestsTable, BindingSource requestBinding) // Load pending requests from the database and bind to DataGridView.
        {
// Step 1: Create a SQL query to fetch pending requests from the database.
            string fetchQuery = @"
                SELECT 
                    rr.requestID AS 'Request ID',
                    r.reservationID AS 'Reservation ID',
                    u.firstName + ' ' + u.lastName AS 'Customer',
                    rr.requestNote AS 'Request',
                    rr.requestStatus AS 'Status',
                    FORMAT(rr.requestDate, 'yyyy-MM-dd') AS 'Date'
                FROM reservationRequests rr
                JOIN Reservations r ON rr.reservationID = r.reservationID
                JOIN Users u ON r.customerID = u.userID
                WHERE rr.requestStatus = 'Pending'
                ORDER BY rr.requestDate DESC";

            using (SqlConnection connection = new SqlConnection(formsHelper.connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(fetchQuery, connection);
                DataTable dataTable = new DataTable();

// Step 2: Fill the DataTable with data from the database.
                adapter.Fill(dataTable);
                requestBinding.DataSource = dataTable;
                requestsTable.DataSource = requestBinding;
            }

// Step 3: Clear the TextBox for displaying selected row details.
            requestsTable.Columns["Request ID"].Width = 150;
            requestsTable.Columns["Reservation ID"].Width = 150;
            requestsTable.Columns["Customer"].Width = 150;
            requestsTable.Columns["Request"].Width = 290;
            requestsTable.Columns["Status"].Width = 150;
            requestsTable.Columns["Date"].Width = 150;

        }

        public void applySearchFilter(BindingSource requestBinding, string input) // Apply search filter to the DataGridView based on user input.
        {
// Step 1: Filter the DataGridView based on the input.
            string filter = $"Convert([Request ID], 'System.String') LIKE '%{input}%' " +
                            $"OR Convert([Reservation ID], 'System.String') LIKE '%{input}%' " +
                            $"OR [Customer] LIKE '%{input}%' " +
                            $"OR [Request] LIKE '%{input}%'";

// Step 2: Apply the filter to the BindingSource.
            requestBinding.Filter = filter;
        }

        public void updateRequestStatus(DataGridView requestsTable, Action reload) // Update the status of a selected request in the database.
        {
    // Ensure a row is selected.
            if (requestsTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a request to review.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

// Step 1: Get the selected request ID.
            int requestID = Convert.ToInt32(requestsTable.SelectedRows[0].Cells["Request ID"].Value);

// Step 2: Create a popup for status update.
            Form popup = formsHelper.CreatePopupForm("Update Request Status", new Size(400, 180));

            Label lblStatus = new Label()
            {
                Text = "New Status:",
                Location = new Point(30, 40),
                AutoSize = true,
                ForeColor = Color.White
            };

            ComboBox cmbStatus = new ComboBox()
            {
                Location = new Point(140, 35),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new[] { "Approved", "Denied" });

            Button btnSubmit = new Button()
            {
                Text = "Confirm",
                Location = new Point(140, 85),
                Width = 100,
                Height = 35,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 3: Add event handler for the Submit button.
            btnSubmit.Click += (s, e) =>
            {
    // Validate input.
                if (cmbStatus.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a status.");
                    return;
                }

// Step 4: Prepare SQL query to update the request status in the database.
                string updateQuery = "UPDATE reservationRequests SET requestStatus = @status WHERE requestID = @id";
                SqlParameter[] parameters = {
                    new SqlParameter("@status", cmbStatus.SelectedItem.ToString()),
                    new SqlParameter("@id", requestID)
                };

// Step 5: Execute the SQL query to update the request status.
                formsHelper.executeSQL(updateQuery, parameters);
                MessageBox.Show("Request status updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                popup.Close();
                reload();
            };

    // Add all controls to the form and display it.
            popup.Controls.AddRange(new Control[] { lblStatus, cmbStatus, btnSubmit });
            popup.ShowDialog();
        }

// ========================== \\
//  foodieManageReservations  \\
// ========================== \\

        public void loadReservations(DataGridView reservationsTable, TextBox txtRow, BindingSource reservationBinding, ref SqlDataAdapter dataAdapter, ref DataTable dataTable) // Load reservation data from the database and bind to DataGridView.
        {
// Step 1: SQL query to fetch reservation data from the database.
            string fetchQuery = @"
            SELECT 
                r.reservationID AS 'ID',
                u.firstName + ' ' + u.lastName AS 'Customer',
                r.resPax AS 'Pax',
                h.hallName AS 'Hall',
                FORMAT(r.reservationDate, 'yyyy-MM-dd') + ' | ' + FORMAT(CAST(r.startTime AS DATETIME), 'hh:mm tt') AS 'Time',
                r.requestNotes AS 'Notes',
                r.resStatus AS 'Status',
                a.firstName + ' ' + a.lastName AS 'Creator'
            FROM Reservations r
            JOIN Users u ON r.customerID = u.userID
            JOIN Halls h ON r.hallID = h.hallID
            JOIN Users a ON r.assignedBy = a.userID
            WHERE r.resStatus != 'Complete'
            AND r.reservationDate >= CAST(GETDATE() AS DATE);";

    // Open a connection to the database and fetch the data.
            using (SqlConnection sqlConnection = new SqlConnection(formsHelper.connectionString))
            {
                dataAdapter = new SqlDataAdapter(fetchQuery, sqlConnection);
                dataTable = new DataTable();

// Step 2: Fill the DataTable with data from the database.
                dataAdapter.Fill(dataTable);
                reservationBinding.DataSource = dataTable;
                reservationsTable.DataSource = reservationBinding;
            }

            txtRow.Text = "";

// Step 3: Set the column widths for better readability.
            reservationsTable.Columns["ID"].Width = 50;
            reservationsTable.Columns["Customer"].Width = 150;
            reservationsTable.Columns["Pax"].Width = 90;
            reservationsTable.Columns["Hall"].Width = 150;
            reservationsTable.Columns["Time"].Width = 130;
            reservationsTable.Columns["Notes"].Width = 200;
            reservationsTable.Columns["Status"].Width = 120;
            reservationsTable.Columns["Creator"].Width = 150;

        }

        public void addReservation(Action reloadData) // Add a new reservation to the database.
        {
// Step 1: Create a new pop-up form for adding a reservation.
            Form addForm = formsHelper.CreatePopupForm("Add Reservation", new Size(600, 550));
            int formWidth = addForm.ClientSize.Width;
            int controlWidth = 280;
            int labelX = (formWidth - controlWidth) / 2 - 130;
            int inputX = (formWidth - controlWidth) / 2;

// Step 2: Create and position the form elements.
            Label lblCustomer = new Label() { Text = "Customer:", Location = new Point(labelX, 30), AutoSize = true, ForeColor = Color.White };
            ComboBox cmbCustomer = new ComboBox() { Location = new Point(inputX, 30), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblHall = new Label() { Text = "Hall:", Location = new Point(labelX, 70), AutoSize = true, ForeColor = Color.White };
            ComboBox cmbHall = new ComboBox() { Location = new Point(inputX, 70), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };

// Step 3: Populate the ComboBoxes with data from the database.
            using (SqlConnection sqlConnection = new SqlConnection(formsHelper.connectionString))
            {
                sqlConnection.Open();

    // Customers ComboBox.
                SqlCommand customerCmd = new SqlCommand("SELECT userID, firstName, lastName FROM Users WHERE userRole = 'Customer'", sqlConnection);
                SqlDataReader sqlReader = customerCmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    cmbCustomer.Items.Add($"({sqlReader["userID"]}) - {sqlReader["firstName"]} {sqlReader["lastName"]}");
                }
                sqlReader.Close();

    // Halls ComboBox.
                SqlCommand hallCmd = new SqlCommand("SELECT hallID, hallName, hallCap FROM Halls", sqlConnection);
                SqlDataReader hallReader = hallCmd.ExecuteReader();
                while (hallReader.Read())
                {
                    cmbHall.Items.Add($"({hallReader["hallID"]}) - {hallReader["hallName"]} ({hallReader["hallCap"]} Pax)");
                }
                hallReader.Close();
            }

// Step 4: Create and position the rest of the form elements.
            Label lblPax = new Label() { Text = "Pax:", Location = new Point(labelX, 110), AutoSize = true, ForeColor = Color.White };
            TextBox txtPax = new TextBox() { Location = new Point(inputX, 110), Width = controlWidth };

            Label lblDate = new Label() { Text = "Date:", Location = new Point(labelX, 150), AutoSize = true, ForeColor = Color.White };
            DateTimePicker dtpDate = new DateTimePicker() { Location = new Point(inputX, 150), Width = controlWidth };

            Label lblStart = new Label() { Text = "Time:", Location = new Point(labelX, 190), AutoSize = true, ForeColor = Color.White };
            DateTimePicker dtpStart = new DateTimePicker() { Location = new Point(inputX, 190), Width = controlWidth, Format = DateTimePickerFormat.Time, ShowUpDown = true };

            Label lblNotes = new Label() { Text = "Notes:", Location = new Point(labelX, 270), AutoSize = true, ForeColor = Color.White };
            TextBox txtNotes = new TextBox() { Location = new Point(inputX, 270), Width = controlWidth, Height = 60, Multiline = true };

            Button btnSubmit = new Button()
            {
                Text = "Submit",
                Location = new Point((formWidth - 200) / 2, 350),
                Width = 200,
                Height = 50,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 5: Add event handler for the Submit button.
            btnSubmit.Click += (s, e) =>
            {
    // Validate input fields.
                if (cmbCustomer.SelectedIndex == -1 || cmbHall.SelectedIndex == -1 || !int.TryParse(txtPax.Text.Trim(), out int pax))
                {
                    MessageBox.Show("Please complete all fields.");
                    return;
                }

// Step 6: Prepare SQL query to insert the reservation into the database.
                string customerID = cmbCustomer.SelectedItem.ToString().Split(')')[0].Substring(1);
                string hallID = cmbHall.SelectedItem.ToString().Split(')')[0].Substring(1);
                string notes = txtNotes.Text.Trim();

                string insertQuery = @"
                INSERT INTO Reservations (customerID, hallID, reservationDate, startTime, resPax, requestNotes, resStatus, assignedBy)
                VALUES (@cid, @hid, @date, @start, @pax, @notes, 'In-Complete', @creator);";

// Step 7: Execute the SQL query to insert the reservation.
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@cid", customerID),
                new SqlParameter("@hid", hallID),
                new SqlParameter("@pax", pax),
                new SqlParameter("@date", dtpDate.Value),
                new SqlParameter("@start", dtpStart.Value.ToString("HH:mm:ss")),
                new SqlParameter("@notes", notes),
                new SqlParameter("@creator", UserSession.UserID)
                };

                formsHelper.executeSQL(insertQuery, parameters);

    // Show success message and close the form.
                MessageBox.Show("Reservation added successfully!");
                addForm.Close();
                reloadData();
            };

    // Add all controls to the form and display it.
            addForm.Controls.AddRange(new Control[]
            {
            lblCustomer, cmbCustomer,
            lblHall, cmbHall,
            lblPax, txtPax,
            lblDate, dtpDate,
            lblStart, dtpStart,
            lblNotes, txtNotes,
            btnSubmit
            });

            addForm.ShowDialog();
        }

        public void editReservation(DataGridView reservationsTable, Action reloadData) // Edit an existing reservation in the database.
        {
    // Make sure a row is selected.
            if (reservationsTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a reservation to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = reservationsTable.SelectedRows[0];
            int reservationID = Convert.ToInt32(selectedRow.Cells["ID"].Value);

// Step 1: Create a new pop-up form for editing the reservation.
            Form editForm = formsHelper.CreatePopupForm("Edit Reservation", new Size(600, 550));
            int formWidth = editForm.ClientSize.Width;
            int controlWidth = 280;
            int labelX = (formWidth - controlWidth) / 2 - 130;
            int inputX = (formWidth - controlWidth) / 2;

    // Create and position the form elements.
        // Customer ComboBox.
            Label lblCustomer = new Label() { Text = "Customer:", Location = new Point(labelX, 30), AutoSize = true, ForeColor = Color.White };
            ComboBox cmbCustomer = new ComboBox() { Location = new Point(inputX, 30), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };

        // Hall ComboBox.
            Label lblHall = new Label() { Text = "Hall:", Location = new Point(labelX, 70), AutoSize = true, ForeColor = Color.White };
            ComboBox cmbHall = new ComboBox() { Location = new Point(inputX, 70), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };

    // Load customers and halls.
            using (SqlConnection conn = new SqlConnection(formsHelper.connectionString))
            {
                conn.Open();

    // Populate the ComboBoxes with data from the database.
                SqlCommand customerCmd = new SqlCommand("SELECT userID, firstName, lastName FROM Users WHERE userRole = 'Customer'", conn);
                using (SqlDataReader reader = customerCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbCustomer.Items.Add($"({reader["userID"]}) - {reader["firstName"]} {reader["lastName"]}");
                    }
                }

                SqlCommand hallCmd = new SqlCommand("SELECT hallID, hallName, hallCap FROM Halls", conn);
                using (SqlDataReader reader = hallCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbHall.Items.Add($"({reader["hallID"]}) - {reader["hallName"]} ({reader["hallCap"]} Pax)");
                    }
                }
            }

        // Pax.
            Label lblPax = new Label() { Text = "Pax:", Location = new Point(labelX, 110), AutoSize = true, ForeColor = Color.White };
            TextBox txtPax = new TextBox() { Location = new Point(inputX, 110), Width = controlWidth, Text = selectedRow.Cells["Pax"].Value.ToString() };

        // Date.
            Label lblDate = new Label() { Text = "Date:", Location = new Point(labelX, 150), AutoSize = true, ForeColor = Color.White };
            string currentDate = selectedRow.Cells["Time"].Value.ToString().Split('|')[0].Trim();
            string currentTime = selectedRow.Cells["Time"].Value.ToString().Split('|')[1].Trim();
            DateTimePicker dtpDate = new DateTimePicker() { Location = new Point(inputX, 150), Width = controlWidth, Value = DateTime.ParseExact(currentDate, "yyyy-MM-dd", null) };

            // Start Time
            Label lblStart = new Label() { Text = "Start Time:", Location = new Point(labelX, 190), AutoSize = true, ForeColor = Color.White };
            DateTimePicker dtpStart = new DateTimePicker() { Location = new Point(inputX, 190), Width = controlWidth, Format = DateTimePickerFormat.Time, ShowUpDown = true, Value = DateTime.ParseExact(currentTime, "hh:mm tt", null) };

            // Notes
            Label lblNotes = new Label() { Text = "Notes:", Location = new Point(labelX, 270), AutoSize = true, ForeColor = Color.White };
            TextBox txtNotes = new TextBox() { Location = new Point(inputX, 270), Width = controlWidth, Height = 60, Multiline = true, Text = selectedRow.Cells["Notes"].Value.ToString() };

// Step 2: Pre-select current values
            foreach (string item in cmbCustomer.Items)
            {
                if (item.Contains(selectedRow.Cells["Customer"].Value.ToString()))
                {
                    cmbCustomer.SelectedItem = item;
                    break;
                }
            }

            foreach (string item in cmbHall.Items)
            {
                if (item.Contains(selectedRow.Cells["Hall"].Value.ToString()))
                {
                    cmbHall.SelectedItem = item;
                    break;
                }
            }

        // Update button.
            Button btnUpdate = new Button()
            {
                Text = "Update",
                Location = new Point((formWidth - 200) / 2, 350),
                Width = 200,
                Height = 50,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 3: Add event handler for the Update button.
            btnUpdate.Click += (s, e) =>
            {
    // Validate input fields.
                if (cmbCustomer.SelectedIndex == -1 || cmbHall.SelectedIndex == -1 || !int.TryParse(txtPax.Text.Trim(), out int pax))
                {
                    MessageBox.Show("Please fill all fields with valid data.");
                    return;
                }

// Step 4: Prepare SQL query to update the reservation in the database.
                string customerID = cmbCustomer.SelectedItem.ToString().Split('(', ')')[1];
                string hallID = cmbHall.SelectedItem.ToString().Split('(', ')')[1];

                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@customerID", customerID),
                new SqlParameter("@hallID", hallID),
                new SqlParameter("@resPax", pax),
                new SqlParameter("@reservationDate", dtpDate.Value),
                new SqlParameter("@startTime", dtpStart.Value.ToString("HH:mm:ss")),
                new SqlParameter("@requestNotes", txtNotes.Text.Trim()),
                new SqlParameter("@reservationID", reservationID)
                };

                string updateQuery = @"
                UPDATE Reservations 
                SET customerID = @customerID, hallID = @hallID, resPax = @resPax,
                    reservationDate = @reservationDate, startTime = @startTime,
                    requestNotes = @requestNotes
                WHERE reservationID = @reservationID";

// Step 5: Execute the SQL query to update the reservation.
                formsHelper.executeSQL(updateQuery, parameters);

    // Show success message and close the form.
                MessageBox.Show("Reservation updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                editForm.Close();
                reloadData();
            };

    // Add all controls to the form and display it.
            editForm.Controls.AddRange(new Control[]
            {
            lblCustomer, cmbCustomer,
            lblHall, cmbHall,
            lblPax, txtPax,
            lblDate, dtpDate,
            lblStart, dtpStart,
            lblNotes, txtNotes,
            btnUpdate
            });

            editForm.ShowDialog();
        }

        public void deleteReservation(DataGridView reservationsTable, Action reloadData) // Delete a reservation from the database.
        {
    // Check if a row is selected in the DataGridView.
            if (reservationsTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a reservation to delete.");
                return;
            }

// Step 1: Get the selected reservation ID.
            int reservationID = Convert.ToInt32(reservationsTable.SelectedRows[0].Cells["ID"].Value);

// Step 2: Create a confirmation dialog to confirm the deletion of the reservation.
            DialogResult deleteConfirmation = MessageBox.Show($"Delete reservation ID {reservationID}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (deleteConfirmation == DialogResult.Yes)
            {
// Step 3: Prepare SQL query to delete the reservation from the database.
                var sqlQueries = new[] {("DELETE FROM Reservations WHERE reservationID = @id", new[] { new SqlParameter("@id", reservationID) })};

    // Execute the SQL query to delete the reservation.
                formsHelper.executeMultipleSQL(sqlQueries);

    // Show success message and reload the reservation data.
                MessageBox.Show("Reservation has been deleted.");
                reloadData();
            }
        }

// ============================= \\
// foodieUpdateReservationStatus \\
// ============================= \\

        public void updateReservationStatus(DataGridView reservationsTable, Action reload) // Update the status of a reservation in the database.
        {
        // Ensure a row is selected
            if (reservationsTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a reservation to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int reservationID = Convert.ToInt32(reservationsTable.SelectedRows[0].Cells["ID"].Value);
            string currentStatus = reservationsTable.SelectedRows[0].Cells["Status"].Value.ToString();

// Step 1: Create a new pop-up form for updating the reservation status.
            Form popup = formsHelper.CreatePopupForm("Update Reservation Status", new Size(400, 180));

            Label lblStatus = new Label()
            {
                Text = "New Status:",
                Location = new Point(30, 40),
                AutoSize = true,
                ForeColor = Color.White
            };

            ComboBox cmbStatus = new ComboBox()
            {
                Location = new Point(140, 35),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new[] { "Pending", "In-Complete", "In Progress", "Complete" });
            cmbStatus.SelectedItem = currentStatus;

            Button btnSubmit = new Button()
            {
                Text = "Confirm",
                Location = new Point(140, 85),
                Width = 100,
                Height = 35,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 2: Add event handler for the Submit button.
            btnSubmit.Click += (s, e) =>
            {
                if (cmbStatus.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a status.");
                    return;
                }

// Step 3: Prepare SQL query to update the reservation status in the database.
                string newStatus = cmbStatus.SelectedItem.ToString();

                string updateQuery = "UPDATE Reservations SET resStatus = @status WHERE reservationID = @id";
                SqlParameter[] parameters = {
            new SqlParameter("@status", newStatus),
            new SqlParameter("@id", reservationID)
        };

// Step 4: Execute the SQL query to update the reservation status.
                formsHelper.executeSQL(updateQuery, parameters);
                MessageBox.Show("Reservation status updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                popup.Close();
                reload();
            };

    // Add all controls to the form and display it.
            popup.Controls.AddRange(new Control[] { lblStatus, cmbStatus, btnSubmit });
            popup.ShowDialog();
        }

// ======================== \\
// foodieReservationHistory \\
// ======================== \\

        public void loadReservationHistory(DataGridView reservationsTable, TextBox txtRow, BindingSource reservationBinding) // Load reservation history from the database and bind to DataGridView.
        {
// Step 1: SQL query to fetch reservation history from the database.
            string fetchQuery = @"
            SELECT 
                r.reservationID AS 'ID',
                u.firstName + ' ' + u.lastName AS 'Customer',
                r.resPax AS 'Pax',
                h.hallName AS 'Hall',
                FORMAT(r.reservationDate, 'yyyy-MM-dd') + ' | ' + FORMAT(CAST(r.startTime AS DATETIME), 'hh:mm tt') AS 'Time',
                r.requestNotes AS 'Notes',
                r.resStatus AS 'Status',
                a.firstName + ' ' + a.lastName AS 'Creator'
            FROM Reservations r
            JOIN Users u ON r.customerID = u.userID
            JOIN Halls h ON r.hallID = h.hallID
            JOIN Users a ON r.assignedBy = a.userID
            WHERE r.resStatus = 'Complete' OR r.reservationDate < CAST(GETDATE() AS DATE)
            ORDER BY r.reservationDate DESC";

            using (SqlConnection conn = new SqlConnection(formsHelper.connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(fetchQuery, conn);
                DataTable data = new DataTable();

// Step 2: Fill the DataTable with data from the database.
                adapter.Fill(data);
                reservationBinding.DataSource = data;
                reservationsTable.DataSource = reservationBinding;
            }

            txtRow.Text = "";

// Step 3: Set the column widths for better readability.
            reservationsTable.Columns["ID"].Width = 50;
            reservationsTable.Columns["Customer"].Width = 150;
            reservationsTable.Columns["Pax"].Width = 90;
            reservationsTable.Columns["Hall"].Width = 150;
            reservationsTable.Columns["Time"].Width = 130;
            reservationsTable.Columns["Notes"].Width = 200;
            reservationsTable.Columns["Status"].Width = 120;
            reservationsTable.Columns["Creator"].Width = 150;

        }

        public void applyReservationHistoryFilter(string selectedFilter, DataGridView reservationsTable, TextBox txtRow, BindingSource reservationBinding) // Apply filter to the reservation history based on user selection.
        {
// Step 1: SQL query to filter reservation history based on the selected filter.
            string filterQuery = "";

    // Check the selected filter and modify the SQL query accordingly.
            if (selectedFilter == "Complete")
            {
                filterQuery = @"
                SELECT 
                    r.reservationID AS 'ID',
                    u.firstName + ' ' + u.lastName AS 'Customer',
                    r.resPax AS 'Pax',
                    h.hallName AS 'Hall',
                    FORMAT(r.reservationDate, 'yyyy-MM-dd') + ' | ' + FORMAT(CAST(r.startTime AS DATETIME), 'hh:mm tt') AS 'Time',
                    r.requestNotes AS 'Notes',
                    r.resStatus AS 'Status',
                    a.firstName + ' ' + a.lastName AS 'Creator'
                FROM Reservations r
                JOIN Users u ON r.customerID = u.userID
                JOIN Halls h ON r.hallID = h.hallID
                JOIN Users a ON r.assignedBy = a.userID
                WHERE r.resStatus = 'Complete'
                ORDER BY r.reservationDate DESC";
            }
            else if (selectedFilter == "Missed")
            {
                filterQuery = @"
                SELECT 
                    r.reservationID AS 'ID',
                    u.firstName + ' ' + u.lastName AS 'Customer',
                    r.resPax AS 'Pax',
                    h.hallName AS 'Hall',
                    FORMAT(r.reservationDate, 'yyyy-MM-dd') + ' | ' + FORMAT(CAST(r.startTime AS DATETIME), 'hh:mm tt') AS 'Time',
                    r.requestNotes AS 'Notes',
                    r.resStatus AS 'Status',
                    a.firstName + ' ' + a.lastName AS 'Creator'
                FROM Reservations r
                JOIN Users u ON r.customerID = u.userID
                JOIN Halls h ON r.hallID = h.hallID
                JOIN Users a ON r.assignedBy = a.userID
                WHERE r.reservationDate < CAST(GETDATE() AS DATE) AND r.resStatus != 'Complete'
                ORDER BY r.reservationDate DESC";
            }
            else
            {
                loadReservationHistory(reservationsTable, txtRow, reservationBinding);
                return;
            }

            using (SqlConnection conn = new SqlConnection(formsHelper.connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(filterQuery, conn);
                DataTable data = new DataTable();

// Step 2: Fill the DataTable with data from the database.
                adapter.Fill(data);
                reservationBinding.DataSource = data;
                reservationsTable.DataSource = reservationBinding;
            }
        }



    }
}