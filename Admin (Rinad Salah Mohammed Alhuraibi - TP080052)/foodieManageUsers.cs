using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Admin__Rinad_Salah_Mohammed_Alhuraibi___TP080052_
{
    public partial class foodieManageUsers : Form
    {
    // Used to manage users in the application.
        private adminClass adminHelper = new adminClass();

    // Used to handle general form operations.
        private formHelper formHelper = new formHelper();

        private SqlDataAdapter dataAdapter; // Adapter to fill the DataTable with data from the database.
        private DataTable dataTable; // DataTable to hold the data from the database.
        private BindingSource userBinding = new BindingSource(); // BindingSource to bind the DataTable to the DataGridView.

        public foodieManageUsers()
        {
            InitializeComponent();
            cmbAction.SelectedIndex = 0;

    // Load the users into the DataGridView when the form loads.
            adminHelper.loadUsers(usersTable, userBinding);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) // Event handler for the search text box.
        {
            string search = txtSearch.Text.Trim().ToLower();
            string[] fields = { "User ID", "First Name", "Last Name", "Email", "Phone", "Role" };
            formHelper.ApplySearchFilter(userBinding, search, fields);
        }

        private void usersTable_CellClick(object sender, DataGridViewCellEventArgs e) // Event handler for when a cell in the DataGridView is clicked.
        {
            if (usersTable.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = usersTable.SelectedRows[0];
                string[] fields = { "User ID", "First Name", "Last Name", "Email", "Phone", "Role" };
                string[] headers = { "User ID", "First Name", "Last Name", "Email", "Phone", "Role" };
                formHelper.DisplayPreview(usersTable, txtRow, fields, headers);
            }
        }

        private void btnExecute_Click(object sender, EventArgs e) // Event handler for the execute button.
        {
    // Check if a row is selected in the DataGridView.
            int selectedIndex = cmbAction.SelectedIndex;

    // Check if the selected index is valid.
            if (selectedIndex == 0)
            {
                MessageBox.Show("Please select an action first.");
                return;
            }

    // Perform the selected action based on the index.
            if (selectedIndex == 1)
                adminHelper.addUser(() => adminHelper.loadUsers(usersTable, userBinding));
            else if (selectedIndex == 2)
                adminHelper.editUser(usersTable, () => adminHelper.loadUsers(usersTable, userBinding));
            else if (selectedIndex == 3)
                adminHelper.deleteUser(usersTable, () => adminHelper.loadUsers(usersTable, userBinding));

            cmbAction.SelectedIndex = 0;
        }

        private void btnClear_Click(object sender, EventArgs e) // Event handler for the clear button.
        {
            txtRow.Clear();
        }

        private void btnReturn_Click(object sender, EventArgs e) // Event handler for the return button.
        {
            formHelper.returnToDashboard(this);
        }
    }
}
