using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public partial class foodieOrderHistory : Form
    {
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private BindingSource orderBinding = new BindingSource();

        private chefClass chefHelper = new chefClass();
        private formHelper formHelper = new formHelper();

        public foodieOrderHistory()
        {
            InitializeComponent();

            // Load all relevant order history data into the table
            chefHelper.loadOrderHistory(ordersTable, orderBinding);
            cmbFilter.SelectedIndex = 0;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string input = txtSearch.Text.Trim().ToLower();
            string[] fields = { "Order ID", "Reservation ID", "Status", "Date" };
            formHelper.ApplySearchFilter(orderBinding, input, fields);
        }

        private void cmbFilter_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selected = cmbFilter.SelectedItem.ToString();

            if (selected == "Select a filter.")
            {
                chefHelper.loadOrderHistory(ordersTable, orderBinding);
            }
            else
            {
                chefHelper.filterOrderHistory(selected, ordersTable, orderBinding);
            }
        }

        private void ordersTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                formHelper.DisplayPreview(
                    ordersTable,
                    txtRow,
                    new[] { "Order ID", "Reservation ID", "Item", "Price", "Date", "Status" },
                    new[] { "Order ID", "Reservation ID", "Item", "Price", "Date", "Status" });
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRow.Clear();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            formHelper.returnToDashboard(this);
        }
    }
}
