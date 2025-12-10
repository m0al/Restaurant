using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public partial class foodieManageInventory : Form
    {
    // Data connection helpers
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private BindingSource inventoryBinding = new BindingSource();

    // Logic handlers
        private chefClass chefHelper = new chefClass();
        private formHelper formHelper = new formHelper();

        public foodieManageInventory()
        {
            InitializeComponent();

            // Load all inventory data on form load
            chefHelper.loadInventory(inventoryTable, txtRow, inventoryBinding, ref dataAdapter, ref dataTable);

            // Set default action to "None"
            cmbAction.SelectedIndex = 0;
        }

        private void inventoryTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                formHelper.DisplayPreview(
                    inventoryTable,
                    txtRow,
                    new[] { "Inventory ID", "Ingredient", "Quantity", "Unit", "Last Updated" },
                    new[] { "Inventory ID", "Ingredient", "Quantity", "Unit", "Updated" });
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // Real-time filtering by selected columns
            string search = txtSearch.Text.Trim().ToLower();
            string[] searchable = { "Ingredient", "Unit", "Quantity","Last Updated" };
            formHelper.ApplySearchFilter(inventoryBinding, search, searchable);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRow.Clear();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            int actionIndex = cmbAction.SelectedIndex;

            if (actionIndex == 0)
            {
                MessageBox.Show("Please select an action.", "Missing Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Action dispatcher
            if (actionIndex == 1)
                chefHelper.addIngredient(() => chefHelper.loadInventory(inventoryTable, txtRow, inventoryBinding, ref dataAdapter, ref dataTable));
            else if (actionIndex == 2)
                chefHelper.editIngredient(inventoryTable, () => chefHelper.loadInventory(inventoryTable, txtRow, inventoryBinding, ref dataAdapter, ref dataTable));
            else if (actionIndex == 3)
                chefHelper.deleteIngredient(inventoryTable, () => chefHelper.loadInventory(inventoryTable, txtRow, inventoryBinding, ref dataAdapter, ref dataTable));

            cmbAction.SelectedIndex = 0;
            btnClear.PerformClick();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            formHelper.returnToDashboard(this);
        }
    }
}
