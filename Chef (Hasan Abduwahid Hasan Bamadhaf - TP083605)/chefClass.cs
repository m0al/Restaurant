using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public class chefClass
    {
        private formHelper formHelper = new formHelper();
        
// ========================== \\
//    foodieManageInventory   \\
// ========================== \\

        public void loadInventory(DataGridView inventoryTable, TextBox txtRow, BindingSource inventoryBinding, ref SqlDataAdapter dataAdapter, ref DataTable dataTable) // Loads the inventory data into the table.
        {
// Step 1: Define the SQL query to fetch inventory data.
            string fetchQuery = @"
                SELECT 
                    ingredientID AS 'Inventory ID',
                    ingredientName AS 'Ingredient',
                    itemQuantity AS 'Quantity',
                    measuredUnit AS 'Unit',
                    FORMAT(lastUpdated, 'yyyy-MM-dd HH:mm') AS 'Last Updated'
                FROM inventory";

            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                dataAdapter = new SqlDataAdapter(fetchQuery, conn);
                dataTable = new DataTable();

// Step 2: Fill the DataTable with the results of the query.
                dataAdapter.Fill(dataTable);
                inventoryBinding.DataSource = dataTable;
                inventoryTable.DataSource = inventoryBinding;
            }

            txtRow.Text = "";

    // Adjust the DataGridView columns to fit the data.
            foreach (DataGridViewColumn col in inventoryTable.Columns)
                col.Width = 208;
        }

        public void addIngredient(Action reload) // Adds a new ingredient to the inventory.
        {
// Step 1: Create a new form for adding an ingredient.
            Form addForm = formHelper.CreatePopupForm("Add Ingredient", new Size(600, 400));
            int width = addForm.ClientSize.Width;
            int controlWidth = 280;
            int labelX = (width - controlWidth) / 2 - 130;
            int inputX = (width - controlWidth) / 2;

    // Create labels and input fields for adding an ingredient.
            Label lblName = new Label() { Text = "Ingredient Name:", Location = new Point(labelX, 30), ForeColor = Color.White };
            TextBox txtName = new TextBox() { Location = new Point(inputX, 30), Width = controlWidth };

            Label lblQty = new Label() { Text = "Quantity:", Location = new Point(labelX, 80), ForeColor = Color.White };
            TextBox txtQty = new TextBox() { Location = new Point(inputX, 80), Width = controlWidth };

            Label lblUnit = new Label() { Text = "Unit:", Location = new Point(labelX, 130), ForeColor = Color.White };
            ComboBox cmbUnit = new ComboBox()
            {
                Location = new Point(inputX, 130),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbUnit.Items.AddRange(new[] { "G", "KG", "ML", "L", "Pieces", "Packs", "Tablespoons", "Teaspoons", "Cups" });

            Button btnSubmit = new Button()
            {
                Text = "Add",
                Location = new Point((width - 200) / 2, 200),
                Width = 200,
                Height = 50,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 2: Create a button to submit the new ingredient.
            btnSubmit.Click += (s, e) =>
            {
    // Validate the input fields.
                string name = txtName.Text.Trim();
                string qty = txtQty.Text.Trim();
                string unit = cmbUnit.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(name) || !int.TryParse(qty, out int quantity) || string.IsNullOrEmpty(unit))
                {
                    MessageBox.Show("Please fill all fields correctly.");
                    return;
                }

// Step 3: Insert the new ingredient into the database.
                string insert = "INSERT INTO inventory (ingredientName, itemQuantity, measuredUnit, lastUpdated) VALUES (@name, @qty, @unit, @updated)";
                SqlParameter[] parameters = {
                    new SqlParameter("@name", name),
                    new SqlParameter("@qty", quantity),
                    new SqlParameter("@unit", unit),
                    new SqlParameter("@updated", DateTime.Now)
                };

// Step 4: Execute the SQL command to insert the new ingredient.
                formHelper.executeSQL(insert, parameters);
                MessageBox.Show("Ingredient added successfully!");
                addForm.Close();
                reload();
            };

    // Add controls to the form.
            addForm.Controls.AddRange(new Control[] { lblName, txtName, lblQty, txtQty, lblUnit, cmbUnit, btnSubmit });
            addForm.ShowDialog();
        }

        public void editIngredient(DataGridView inventoryTable, Action reload) // Edits a selected ingredient in the inventory.
        {
    // Check if any ingredient is selected.
            if (inventoryTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to edit.");
                return;
            }

// Step 1: Get the selected ingredient details.
            DataGridViewRow row = inventoryTable.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["Inventory ID"].Value);
            string name = row.Cells["Ingredient"].Value.ToString();
            string qty = row.Cells["Quantity"].Value.ToString();
            string unit = row.Cells["Unit"].Value.ToString();

// Step 2: Create a new form for editing the ingredient.
            Form editForm = formHelper.CreatePopupForm("Edit Ingredient", new Size(600, 400));
            int width = editForm.ClientSize.Width;
            int controlWidth = 280;
            int labelX = (width - controlWidth) / 2 - 130;
            int inputX = (width - controlWidth) / 2;

    // Create labels and input fields for editing.
            Label lblName = new Label() { Text = "Ingredient Name:", Location = new Point(labelX, 30), ForeColor = Color.White };
            TextBox txtName = new TextBox() { Location = new Point(inputX, 30), Width = controlWidth, Text = name };

            Label lblQty = new Label() { Text = "Quantity:", Location = new Point(labelX, 80), ForeColor = Color.White };
            TextBox txtQty = new TextBox() { Location = new Point(inputX, 80), Width = controlWidth, Text = qty };

            Label lblUnit = new Label() { Text = "Unit:", Location = new Point(labelX, 130), ForeColor = Color.White };
            ComboBox cmbUnit = new ComboBox()
            {
                Location = new Point(inputX, 130),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbUnit.Items.AddRange(new[] { "G", "KG", "ML", "L", "Pieces", "Packs", "Tablespoons", "Teaspoons", "Cups" });
            cmbUnit.SelectedItem = unit;

            Button btnUpdate = new Button()
            {
                Text = "Update",
                Location = new Point((width - 200) / 2, 200),
                Width = 200,
                Height = 50,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 3: Create a button to submit the changes.
            btnUpdate.Click += (s, e) =>
            {
    // Validate the input fields.
                string updatedName = txtName.Text.Trim();
                string updatedQty = txtQty.Text.Trim();
                string updatedUnit = cmbUnit.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(updatedName) || !int.TryParse(updatedQty, out int quantity) || string.IsNullOrEmpty(updatedUnit))
                {
                    MessageBox.Show("Please fill all fields correctly.");
                    return;
                }

// Step 4: Update the ingredient in the database.
                string update = "UPDATE inventory SET ingredientName = @name, itemQuantity = @qty, measuredUnit = @unit, lastUpdated = @updated WHERE ingredientID = @id";
                SqlParameter[] parameters = {
                    new SqlParameter("@name", updatedName),
                    new SqlParameter("@qty", quantity),
                    new SqlParameter("@unit", updatedUnit),
                    new SqlParameter("@updated", DateTime.Now),
                    new SqlParameter("@id", id)
                };

// Step 5: Execute the SQL command to update the ingredient.
                formHelper.executeSQL(update, parameters);
                MessageBox.Show("Ingredient updated successfully!");
                editForm.Close();
                reload();
            };

    // Add controls to the form.
            editForm.Controls.AddRange(new Control[] { lblName, txtName, lblQty, txtQty, lblUnit, cmbUnit, btnUpdate });
            editForm.ShowDialog();
        }

        public void deleteIngredient(DataGridView inventoryTable, Action reload) // Deletes a selected ingredient from the inventory.
        {
    // Check if any ingredient is selected.
            if (inventoryTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to delete.");
                return;
            }

// Step 1: Get the selected ingredient ID.
            int id = Convert.ToInt32(inventoryTable.SelectedRows[0].Cells["Inventory ID"].Value);

    // Confirm deletion with the user.
            DialogResult result = MessageBox.Show($"Delete ingredient ID {id}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                string delete = "DELETE FROM inventory WHERE ingredientID = @id";
                SqlParameter[] parameters = { new SqlParameter("@id", id) };
                formHelper.executeSQL(delete, parameters);
                MessageBox.Show("Ingredient deleted.");
                reload();
            }
        }

// ========================== \\
//  foodieUpdateOrderHistory  \\
// ========================== \\

        public void loadOrderHistory(DataGridView ordersTable, BindingSource orderBinding) // Loads the order history into the table.
        {
// Step 1: Define the SQL query to fetch order history.
            string fetchQuery = @"
            SELECT 
                o.orderID AS 'Order ID',
                o.reservationID AS 'Reservation ID',
                m.itemName AS 'Item',
                o.totalPrice AS 'Price',
                FORMAT(o.orderDate, 'yyyy-MM-dd') AS 'Date',
                o.orderStatus AS 'Status'
            FROM Orders o
            JOIN Menu m ON o.menuID = m.menuID
            WHERE o.orderStatus = 'Complete' OR o.orderDate < CAST(GETDATE() AS DATE)
            ORDER BY o.orderDate DESC";

            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(fetchQuery, conn);
                DataTable orderTable = new DataTable();
                adapter.Fill(orderTable);

// Step 2: Fill the DataTable with the results of the query.
                orderBinding.DataSource = orderTable;
                ordersTable.DataSource = orderBinding;
            }

    // Adjust the DataGridView columns to fit the data.
            foreach (DataGridViewColumn col in ordersTable.Columns)
                col.Width = 173;
        }

        public void filterOrderHistory(string selectedFilter, DataGridView ordersTable, BindingSource orderBinding) // Filters the order history based on the selected filter.
        {
// Step 1: Define the SQL query to fetch order history based on the selected filter.
            string fetchQuery = "";

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
                WHERE o.orderStatus = 'Complete'
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
                WHERE o.orderStatus != 'Complete' AND o.orderDate < CAST(GETDATE() AS DATE)
                ORDER BY o.orderDate DESC";
            }
            else // "Select a filter." or any unrecognized filter
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
                WHERE o.orderStatus = 'Complete' OR o.orderDate < CAST(GETDATE() AS DATE)
                ORDER BY o.orderDate DESC";
            }

            using (SqlConnection conn = new SqlConnection(formHelper.connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(fetchQuery, conn);
                DataTable filteredTable = new DataTable();

// Step 2: Fill the DataTable with the results of the query.
                adapter.Fill(filteredTable);
                orderBinding.DataSource = filteredTable;
                ordersTable.DataSource = orderBinding;
            }
        }

// ========================== \\
//  foodieUpdateOrderStatus   \\
// ========================== \\

        public void loadActiveOrders(DataGridView ordersTable, TextBox txtRow, BindingSource orderBinding, ref SqlDataAdapter dataAdapter, ref DataTable dataTable) // Loads active orders into the table. 
        {
// Step 1: Define the SQL query to fetch active orders.
            string fetchQuery = @"
            SELECT 
                o.orderID AS 'Order ID',
                o.reservationID AS 'Reservation ID',
                m.itemName AS 'Item',
                o.totalPrice AS 'Price',
                o.orderStatus AS 'Status',
                o.orderDate AS 'Date'
            FROM orders o
            JOIN menu m ON o.menuID = m.menuID
            WHERE o.orderStatus != 'Complete'
              AND o.orderDate >= CAST(GETDATE() AS DATE)
            ORDER BY o.orderDate DESC";

            using (SqlConnection dbConnection = new SqlConnection(formHelper.connectionString))
            {
                dataAdapter = new SqlDataAdapter(fetchQuery, dbConnection);
                dataTable = new DataTable();

// Step 2: Fill the DataTable with the results of the query.
                dataAdapter.Fill(dataTable);
                orderBinding.DataSource = dataTable;
                ordersTable.DataSource = orderBinding;
            }

    // Adjust the DataGridView columns to fit the data.
            foreach (DataGridViewColumn col in ordersTable.Columns)
                col.Width = 177;

            txtRow.Text = "";
        }

        public void updateOrderStatus(DataGridView ordersTable, Action reloadOrders) // Updates the status of a selected order.
        {
    // Check if any order is selected.
            if (ordersTable.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an order to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

// Step 1: Get the selected order details.
            int orderID = Convert.ToInt32(ordersTable.SelectedRows[0].Cells["Order ID"].Value);
            string currentStatus = ordersTable.SelectedRows[0].Cells["Status"].Value.ToString();

// Step 2: Create a new form for updating the order status.
            Form updateForm = formHelper.CreatePopupForm("Update Order Status", new Size(400, 200));

            Label lblStatus = new Label()
            {
                Text = "New Status:",
                Location = new Point(30, 40),
                AutoSize = true,
                ForeColor = Color.White
            };

            ComboBox cmbStatus = new ComboBox()
            {
                Location = new Point(150, 35),
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

    // Populate the ComboBox with status options.
            cmbStatus.Items.AddRange(new string[] { "Pending", "Preparing", "Ready", "Complete" });
            cmbStatus.SelectedItem = currentStatus;

            Button btnConfirm = new Button()
            {
                Text = "Update",
                Location = new Point(150, 90),
                Width = 100,
                Height = 40,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

// Step 3: Handle the button click event to update the order status.
            btnConfirm.Click += (s, e) =>
            {
    // Validate the selected status.
                string newStatus = cmbStatus.SelectedItem.ToString();

                if (newStatus == currentStatus)
                {
                    MessageBox.Show("No changes made. Status remains the same.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

// Step 4: Update the order status in the database.
                string updateQuery = "UPDATE Orders SET orderStatus = @status WHERE orderID = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@status", newStatus),
                new SqlParameter("@id", orderID)
                };

// Step 5: Execute the SQL command to update the order status.
                formHelper.executeSQL(updateQuery, parameters);
                MessageBox.Show("Order status updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                updateForm.Close();
                reloadOrders();
            };

    // Add controls to the form.
            updateForm.Controls.AddRange(new Control[] { lblStatus, cmbStatus, btnConfirm });
            updateForm.ShowDialog();
        }
        }
    }