using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Manager__Mohammed_Hani_Abdulmalek_Alshabi___TP079279_
{
    public partial class foodieManageHall : Form
    {
        private managerClass hallsHelper = new managerClass(); // Your class with add/edit/delete/load methods
        private formHelper formHelper = new formHelper(); // Common helper for return navigation and search
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private BindingSource hallBinding = new BindingSource();

        public foodieManageHall()
        {
            InitializeComponent();

            // Load all hall records initially
            hallsHelper.loadHalls(hallsTable, hallBinding);

            // Default action to "Select Action"
            cmbAction.SelectedIndex = 0;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string input = txtSearch.Text.Trim().ToLower();
            string[] searchableColumns = { "Hall ID", "Hall", "Capacity", "Type", "Status" };
            formHelper.ApplySearchFilter(hallBinding, input, searchableColumns);
        }

        private void hallsTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                formHelper.DisplayPreview(
                    hallsTable,
                    txtRow,
                    new[] { "Hall ID", "Hall", "Capacity", "Type", "Status" },
                    new[] { "Hall ID", "Hall", "Capacity", "Type", "Status" }
                );
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            switch (cmbAction.SelectedIndex)
            {
                case 0:
                    MessageBox.Show("Please select an action.");
                    break;
                case 1:
                    hallsHelper.addHall(() => hallsHelper.loadHalls(hallsTable, hallBinding));
                    break;
                case 2:
                    hallsHelper.editHall(hallsTable, () => hallsHelper.loadHalls(hallsTable, hallBinding));
                    break;
                case 3:
                    hallsHelper.deleteHall(hallsTable, () => hallsHelper.loadHalls(hallsTable, hallBinding));
                    break;
            }

            // Reset UI
            cmbAction.SelectedIndex = 0;
            txtSearch.Clear();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            formHelper.returnToDashboard(this);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRow.Clear();
        }
    }
}
