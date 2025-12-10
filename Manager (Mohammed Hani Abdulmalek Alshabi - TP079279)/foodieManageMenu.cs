using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Manager__Mohammed_Hani_Abdulmalek_Alshabi___TP079279_
{
    public partial class foodieManageMenu : Form
    {
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private BindingSource menuBinding = new BindingSource();
        private managerClass managerHelper = new managerClass();
        private formHelper formHelper = new formHelper();

        public foodieManageMenu()
        {
            InitializeComponent();
            managerHelper.loadMenu(menuTable, txtRow, menuBinding, ref dataAdapter, ref dataTable);
            cmbAction.SelectedIndex = 0;
        }

        private void menuTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                formHelper.DisplayPreview(
                    menuTable,
                    txtRow,
                    new[] { "Menu ID", "Category", "Item", "Price", "Status" },
                    new[] { "Menu ID", "Category", "Item", "Price", "Status" }
                );
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string input = txtSearch.Text.Trim().ToLower();
            string[] fields = { "Menu ID", "Category", "Item", "Price", "Status" };
            formHelper.ApplySearchFilter(menuBinding, input, fields);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRow.Clear();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            formHelper.returnToDashboard(this);
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            int selection = cmbAction.SelectedIndex;

            if (selection == 0)
            {
                MessageBox.Show("Please select an action.", "Missing Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selection == 1)
                managerHelper.addMenu(() => managerHelper.loadMenu(menuTable, txtRow, menuBinding, ref dataAdapter, ref dataTable));
            else if (selection == 2)
                managerHelper.editMenu(menuTable, () => managerHelper.loadMenu(menuTable, txtRow, menuBinding, ref dataAdapter, ref dataTable));
            else if (selection == 3)
                managerHelper.deleteMenu(menuTable, () => managerHelper.loadMenu(menuTable, txtRow, menuBinding, ref dataAdapter, ref dataTable));

            cmbAction.SelectedIndex = 0;
            btnClear.PerformClick();
        }
    }
}
