using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public partial class foodieUpdateOrderStatus : Form
    {
        // SQL components
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private BindingSource orderBinding = new BindingSource();

        // Helpers
        private chefClass chefHelper = new chefClass();
        private formHelper formHelper = new formHelper();

        public foodieUpdateOrderStatus()
        {
            InitializeComponent();

            // Load orders on form load
            chefHelper.loadActiveOrders(ordersTable, txtRow, orderBinding, ref dataAdapter, ref dataTable);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string input = txtSearch.Text.Trim().ToLower();
            string[] fields = { "Order ID", "Reservation ID", "Status" };
            formHelper.ApplySearchFilter(orderBinding, input, fields);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRow.Clear();
        }

        private void ordersTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                formHelper.DisplayPreview(
                    ordersTable,
                    txtRow,
                    new[] { "Order ID", "Reservation ID", "Price", "Status", "Date" },
                    new[] { "Order ID", "Reservation ID", "Price", "Status", "Date" });
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            chefHelper.updateOrderStatus(ordersTable, () =>
                chefHelper.loadActiveOrders(ordersTable, txtRow, orderBinding, ref dataAdapter, ref dataTable));
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            formHelper.returnToDashboard(this);
        }
    }
}
