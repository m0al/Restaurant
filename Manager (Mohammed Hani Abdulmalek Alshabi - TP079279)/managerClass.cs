using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Manager__Mohammed_Hani_Abdulmalek_Alshabi___TP079279_
{
    internal class managerClass
    {

    // Creates a reusable formHelper object to access the connection string and executeSQL method.
        formHelper formHelper = new formHelper();

// ================ \\
// foodieManageMenu \\
// ================ \\
        public void loadMenu(DataGridView menuTable, TextBox txtRow, BindingSource menuBinding, ref SqlDataAdapter dataAdapter, ref DataTable dataTable) // This method loads the menu data into a DataGridView.
        {
// Step 1: Create a SQL query to fetch menu data.
            string fetchQuery = @"
            SELECT 
                menuID AS 'Menu ID',
                menuCategory AS 'Category',
                itemName AS 'Item',
                itemPrice AS 'Price',
                menuStatus AS 'Status'
            FROM Menu";

            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                dataAdapter = new SqlDataAdapter(fetchQuery, conn);
                dataTable = new DataTable();

// Step 2: Fill the DataTable with the results of the query.
                dataAdapter.Fill(dataTable);
                menuBinding.DataSource = dataTable;
                menuTable.DataSource = menuBinding;
            }

    // Adjust column widths for better visibility
            foreach (DataGridViewColumn col in menuTable.Columns)
            {
                col.Width = 208;
            }

            txtRow.Text = "";
        }

        public void addMenu(Action reloadMenu) // This method creates a pop-up form to add a new menu item.
        {
// Step 1: Create a new form for adding menu items.
            Form addForm = formHelper.CreatePopupForm("Add Menu Item", new Size(600, 400));
            int controlWidth = 280;
            int labelX = 60;
            int inputX = 160;

    // Create labels and input fields for the form.
            Label lblCategory = new Label() { Text = "Category:", Location = new Point(labelX, 40), AutoSize = true, ForeColor = Color.White };
            ComboBox cmbCategory = new ComboBox()
            {
                Location = new Point(inputX, 35),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            cmbCategory.Items.AddRange(new[] { "Appetizers", "Main Course", "Desserts", "Beverages" });

            Label lblName = new Label() { Text = "Item Name:", Location = new Point(labelX, 90), AutoSize = true, ForeColor = Color.White };
            TextBox txtName = new TextBox() { Location = new Point(inputX, 85), Width = controlWidth };

            Label lblPrice = new Label() { Text = "Price (RM):", Location = new Point(labelX, 140), AutoSize = true, ForeColor = Color.White };
            TextBox txtPrice = new TextBox() { Location = new Point(inputX, 135), Width = controlWidth };

            Button btnSubmit = new Button()
            {
                Text = "Add",
                Width = 180,
                Height = 40,
                Location = new Point((addForm.Width - 180) / 2, 200),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 2: Add a click event handler for the submit button.
            btnSubmit.Click += (s, e) =>
            {
    // Validate the input fields.
                if (cmbCategory.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtName.Text) || !decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    MessageBox.Show("Please enter valid data for all fields.");
                    return;
                }

// Step 3: Create a SQL query to insert the new menu item into the database.
                string query = "INSERT INTO Menu (menuCategory, itemName, itemPrice, menuStatus) VALUES (@cat, @name, @price, 'Active')";
                SqlParameter[] parameters = {
                new SqlParameter("@cat", cmbCategory.SelectedItem.ToString()),
                new SqlParameter("@name", txtName.Text.Trim()),
                new SqlParameter("@price", price)
                };

// Step 4: Execute the SQL query using the formHelper's executeSQL method.
                formHelper.executeSQL(query, parameters);
                MessageBox.Show("Menu item added successfully!");
                addForm.Close();
                reloadMenu();
            };

    // Add all controls to the form.
            addForm.Controls.AddRange(new Control[] { lblCategory, cmbCategory, lblName, txtName, lblPrice, txtPrice, btnSubmit });
            addForm.ShowDialog();
        }

        public void editMenu(DataGridView menuTable, Action reloadMenu) // This method creates a pop-up form to edit an existing menu item.
        {
    // Check if a row is selected in the DataGridView.
            if (menuTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a menu item to edit.");
                return;
            }

// Step 1: Get the selected row and retrieve its values.
            var row = menuTable.SelectedRows[0];
            int menuID = Convert.ToInt32(row.Cells["Menu ID"].Value);
            string currentCategory = row.Cells["Category"].Value.ToString();
            string currentItem = row.Cells["Item"].Value.ToString();
            string currentPrice = row.Cells["Price"].Value.ToString();

// Step 2: Create a new form for editing the menu item.
            Form editForm = formHelper.CreatePopupForm("Edit Menu Item", new Size(600, 400));
            int controlWidth = 280;
            int labelX = 60;
            int inputX = 160;

    // Create labels and input fields for the form.
            Label lblCategory = new Label() { Text = "Category:", Location = new Point(labelX, 40), AutoSize = true, ForeColor = Color.White };
            ComboBox cmbCategory = new ComboBox()
            {
                Location = new Point(inputX, 35),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            cmbCategory.Items.AddRange(new[] { "Appetizers", "Main Course", "Desserts", "Beverages" });
            cmbCategory.SelectedItem = currentCategory;

            Label lblName = new Label() { Text = "Item Name:", Location = new Point(labelX, 90), AutoSize = true, ForeColor = Color.White };
            TextBox txtName = new TextBox() { Location = new Point(inputX, 85), Width = controlWidth, Text = currentItem };

            Label lblPrice = new Label() { Text = "Price (RM):", Location = new Point(labelX, 140), AutoSize = true, ForeColor = Color.White };
            TextBox txtPrice = new TextBox() { Location = new Point(inputX, 135), Width = controlWidth, Text = currentPrice };

            Button btnSubmit = new Button()
            {
                Text = "Update",
                Width = 180,
                Height = 40,
                Location = new Point((editForm.Width - 180) / 2, 200),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 3: Add a click event handler for the submit button.
            btnSubmit.Click += (s, e) =>
            {
    // Validate the input fields.
                if (cmbCategory.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtName.Text) || !decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    MessageBox.Show("Please enter valid data.");
                    return;
                }

// Step 4: Create a SQL query to update the menu item in the database.
                string query = "UPDATE Menu SET menuCategory = @cat, itemName = @name, itemPrice = @price WHERE menuID = @id";
                SqlParameter[] parameters = {
                new SqlParameter("@cat", cmbCategory.SelectedItem.ToString()),
                new SqlParameter("@name", txtName.Text.Trim()),
                new SqlParameter("@price", price),
                new SqlParameter("@id", menuID)
                };

// Step 5: Execute the SQL query using the formHelper's executeSQL method.
                formHelper.executeSQL(query, parameters);
                MessageBox.Show("Menu item updated successfully!");
                editForm.Close();
                reloadMenu();
            };

    // Add all controls to the form.
            editForm.Controls.AddRange(new Control[] { lblCategory, cmbCategory, lblName, txtName, lblPrice, txtPrice, btnSubmit });
            editForm.ShowDialog();
        }

        public void deleteMenu(DataGridView menuTable, Action reloadMenu) // This method marks a menu item as inactive.
        {
    // Check if a row is selected in the DataGridView.
            if (menuTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a menu item to disable.");
                return;
            }

// Step 1: Get the selected row and retrieve its menu ID.
            int menuID = Convert.ToInt32(menuTable.SelectedRows[0].Cells["Menu ID"].Value);

    // Confirm the action with the user.
            DialogResult confirm = MessageBox.Show("Are you sure you want to mark this item as Un-Active?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

// Step 2: If confirmed, create a SQL query to update the menu item's status.
            if (confirm == DialogResult.Yes)
            {
                string query = "UPDATE Menu SET menuStatus = 'Un-Active' WHERE menuID = @id";
                SqlParameter[] parameters = { new SqlParameter("@id", menuID) };

// Step 3: Execute the SQL query using the formHelper's executeSQL method.
                formHelper.executeSQL(query, parameters);
                MessageBox.Show("Menu item status updated to Un-Active.");
                reloadMenu();
            }
        }

// ================ \\
// foodieManageHall \\
// ================ \\
        public void loadHalls(DataGridView hallsTable, BindingSource hallBinding) // This method loads the hall data into a DataGridView.
        {
// Step 1: Create a SQL query to fetch hall data.
            string fetchQuery = @"
            SELECT 
                hallID AS 'Hall ID',
                hallName AS 'Hall',
                hallCap AS 'Capacity',
                hallType AS 'Type',
                halStatus AS 'Status'
            FROM Halls";

            using (SqlConnection conn = new SqlConnection(new formHelper().connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(fetchQuery, conn);
                DataTable dt = new DataTable();

// Step 2: Fill the DataTable with the results of the query.
                adapter.Fill(dt);
                hallBinding.DataSource = dt;
                hallsTable.DataSource = hallBinding;
            }

    // Adjust column widths for better visibility
            foreach (DataGridViewColumn col in hallsTable.Columns)
                col.Width = 214;
        }

        public void addHall(Action reload) // This method creates a pop-up form to add a new hall.
        {
// Step 1: Create a new form for adding halls.
            Form popup = new formHelper().CreatePopupForm("Add Hall", new Size(500, 350));

    // Create labels and input fields for the form.
            Label lblName = new Label() { Text = "Hall Name:", Location = new Point(40, 40), ForeColor = Color.White, AutoSize = true };
            TextBox txtName = new TextBox() { Location = new Point(160, 35), Width = 250 };

            Label lblCap = new Label() { Text = "Capacity:", Location = new Point(40, 90), ForeColor = Color.White, AutoSize = true };
            TextBox txtCap = new TextBox() { Location = new Point(160, 85), Width = 250 };

            Label lblType = new Label() { Text = "Hall Type:", Location = new Point(40, 140), ForeColor = Color.White, AutoSize = true };
            TextBox txtType = new TextBox() { Location = new Point(160, 135), Width = 250 };

            Button btnAdd = new Button()
            {
                Text = "Add Hall",
                Width = 150,
                Height = 40,
                Location = new Point(160, 200),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 2: Add a click event handler for the add button.
            btnAdd.Click += (s, e) =>
            {
    // Validate the input fields.
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtCap.Text) || string.IsNullOrWhiteSpace(txtType.Text))
                {
                    MessageBox.Show("All fields are required.");
                    return;
                }

                if (!int.TryParse(txtCap.Text.Trim(), out int capacity))
                {
                    MessageBox.Show("Invalid capacity value.");
                    return;
                }

// Step 3: Create a SQL query to insert the new hall into the database.
                string insertQuery = "INSERT INTO Halls (hallName, hallCap, hallType, halStatus) VALUES (@name, @cap, @type, 'Active')";
                SqlParameter[] param = {
                new SqlParameter("@name", txtName.Text.Trim()),
                new SqlParameter("@cap", capacity),
                new SqlParameter("@type", txtType.Text.Trim())
                };

// Step 4: Execute the SQL query using the formHelper's executeSQL method.
                new formHelper().executeSQL(insertQuery, param);
                MessageBox.Show("Hall added successfully!");
                popup.Close();
                reload();
            };

    // Add all controls to the form.
            popup.Controls.AddRange(new Control[] { lblName, txtName, lblCap, txtCap, lblType, txtType, btnAdd });
            popup.ShowDialog();
        }

        public void editHall(DataGridView hallsTable, Action reload) // This method creates a pop-up form to edit an existing hall.
        {
    // Check if a row is selected in the DataGridView.
            if (hallsTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a hall to edit.");
                return;
            }

// Step 1: Get the selected row and retrieve its values.
            DataGridViewRow row = hallsTable.SelectedRows[0];
            int hallID = Convert.ToInt32(row.Cells["Hall ID"].Value);
            string currentName = row.Cells["Hall"].Value.ToString();
            string currentCap = row.Cells["Capacity"].Value.ToString();
            string currentType = row.Cells["Type"].Value.ToString();

// Step 2: Create a new form for editing the hall.
            Form popup = new formHelper().CreatePopupForm("Edit Hall", new Size(500, 350));

    // Create labels and input fields for the form.
            Label lblName = new Label() { Text = "Hall Name:", Location = new Point(40, 40), ForeColor = Color.White, AutoSize = true };
            TextBox txtName = new TextBox() { Location = new Point(160, 35), Width = 250, Text = currentName };

            Label lblCap = new Label() { Text = "Capacity:", Location = new Point(40, 90), ForeColor = Color.White, AutoSize = true };
            TextBox txtCap = new TextBox() { Location = new Point(160, 85), Width = 250, Text = currentCap };

            Label lblType = new Label() { Text = "Hall Type:", Location = new Point(40, 140), ForeColor = Color.White, AutoSize = true };
            TextBox txtType = new TextBox() { Location = new Point(160, 135), Width = 250, Text = currentType };

            Button btnUpdate = new Button()
            {
                Text = "Update Hall",
                Width = 150,
                Height = 40,
                Location = new Point(160, 200),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 3: Add a click event handler for the update button.
            btnUpdate.Click += (s, e) =>
            {
    // Validate the input fields.
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtCap.Text) || string.IsNullOrWhiteSpace(txtType.Text))
                {
                    MessageBox.Show("All fields must be filled.");
                    return;
                }

                if (!int.TryParse(txtCap.Text, out int newCap))
                {
                    MessageBox.Show("Capacity must be a number.");
                    return;
                }

// Step 4: Create a SQL query to update the hall in the database.
                string updateQuery = "UPDATE Halls SET hallName = @name, hallCap = @cap, hallType = @type WHERE hallID = @id";
                SqlParameter[] param = {
                new SqlParameter("@name", txtName.Text.Trim()),
                new SqlParameter("@cap", newCap),
                new SqlParameter("@type", txtType.Text.Trim()),
                new SqlParameter("@id", hallID)
                };

// Step 5: Execute the SQL query using the formHelper's executeSQL method.
                new formHelper().executeSQL(updateQuery, param);
                MessageBox.Show("Hall updated successfully.");
                popup.Close();
                reload();
            };

    // Add all controls to the form.
            popup.Controls.AddRange(new Control[] { lblName, txtName, lblCap, txtCap, lblType, txtType, btnUpdate });
            popup.ShowDialog();
        }

        public void deleteHall(DataGridView hallsTable, Action reload) // This method marks a hall as inactive.
        {
    // Check if a row is selected in the DataGridView.
            if (hallsTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a hall to deactivate.");
                return;
            }

// Step 1: Get the selected row and retrieve its hall ID and name.
            int hallID = Convert.ToInt32(hallsTable.SelectedRows[0].Cells["Hall ID"].Value);
            string hallName = hallsTable.SelectedRows[0].Cells["Hall"].Value.ToString();

// Step 2: Confirm the action with the user.
            DialogResult confirm = MessageBox.Show($"Mark '{hallName}' as Un-Active?", "Confirm Deactivation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

// Step 3: If confirmed, create a SQL query to update the hall's status.
            if (confirm == DialogResult.Yes)
            {
                string updateQuery = "UPDATE Halls SET halStatus = 'Un-Active' WHERE hallID = @id";
                SqlParameter[] param = { new SqlParameter("@id", hallID) };

// Step 4: Execute the SQL query using the formHelper's executeSQL method.
                new formHelper().executeSQL(updateQuery, param);
                MessageBox.Show("Hall deactivated successfully.");
                reload();
            }
        }

// =========================== \\
// foodieViewReservationReport \\
// =========================== \\
        public void loadReservationReport(DataGridView reportTable, string filterCategory = "", string filterValue = "") // This method loads the reservation report data into a DataGridView.
        {

// Step 1: Create a SQL query to fetch reservation report data.
            string fetchQuery = @"
            SELECT 
                h.hallID AS [Hall ID],
                h.hallName AS [Hall],
                COUNT(DISTINCT r.reservationID) AS [Total Bookings],
                ISNULL(SUM(o.totalPrice), 0) AS [Total Revenue]
            FROM Halls h
            LEFT JOIN Reservations r ON h.hallID = r.hallID
            LEFT JOIN Orders o ON r.reservationID = o.reservationID
            WHERE 1 = 1";

            if (filterCategory == "Hall Type")
            {
                fetchQuery += " AND h.hallType = @filterValue";
            }
            else if (filterCategory == "Month - Year")
            {
                fetchQuery += " AND FORMAT(r.reservationDate, 'MMMM yyyy') = @filterValue";
            }

// Step 2: Add the GROUP BY and ORDER BY clauses to the query.
            fetchQuery += @"
            GROUP BY h.hallID, h.hallName
            ORDER BY h.hallID;";

            using (SqlConnection conn = new SqlConnection(new formHelper().connectionString))
            using (SqlCommand cmd = new SqlCommand(fetchQuery, conn))
            {
                if (!string.IsNullOrEmpty(filterCategory) && filterCategory != "Select a filter." && !string.IsNullOrEmpty(filterValue))
                    cmd.Parameters.AddWithValue("@filterValue", filterValue);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable resultTable = new DataTable();

// Step 3: Fill the DataTable with the results of the query.
                adapter.Fill(resultTable);
                reportTable.DataSource = resultTable;

    // Adjust column widths for better visibility
                foreach (DataGridViewColumn col in reportTable.Columns)
                    col.Width = 180;
            }
        }

        public void loadReservationMonths(ComboBox cmbFilter) // This method populates a ComboBox with the last 12 months for filtering.
        {
            cmbFilter.Items.Clear();

    // Populate the ComboBox with the last 12 months.
            for (int i = 0; i < 12; i++)
            {
                DateTime targetMonth = DateTime.Now.AddMonths(-i);
                cmbFilter.Items.Add(targetMonth.ToString("MMMM yyyy"));
            }

        // Set the ComboBox to the current month by default.
            if (cmbFilter.Items.Count > 0)
                cmbFilter.SelectedIndex = 0;
        }

        public void loadFilterOptions(string filterCategory, ComboBox cmbFilter) // This method populates a ComboBox with filter options based on the selected category.
        {
    // Clear the ComboBox and enable it.
            cmbFilter.Items.Clear();
            cmbFilter.Enabled = true;

            using (SqlConnection conn = new SqlConnection(new formHelper().connectionString))
            {
                conn.Open();

// Step 1: Populate the ComboBox based on the selected filter category.
                if (filterCategory == "Hall Type")
                {
                    SqlCommand cmd = new SqlCommand("SELECT DISTINCT hallType FROM Halls", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cmbFilter.Items.Add(reader["hallType"].ToString());
                    }
                    reader.Close();
                }
                else if (filterCategory == "Month - Year")
                {
                    // Populate using the dynamic method instead
                    conn.Close();
                    loadReservationMonths(cmbFilter);
                    return;
                }
    // If no valid filter category is selected, disable the ComboBox.
                else
                {
                    cmbFilter.Enabled = false;
                }
            }

    // Set the ComboBox to the first item by default.
            if (cmbFilter.Items.Count > 0)
                cmbFilter.SelectedIndex = 0;
        }


    }
}
