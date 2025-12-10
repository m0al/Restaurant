using System;
using System.Data;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Customer__Ahmed_Mansour_Mutahar_Saeed_Saleh___TP081788_
{
    public partial class foodieViewOrderStatus : Form
    {

// Used to handle customer-related operations.
        private customerClass customerHelper = new customerClass();

// Used to handle general form-related operations.
        private formHelper formHelper = new formHelper();

        private BindingSource orderBinding = new BindingSource(); // Binding source for the DataGridView

        public foodieViewOrderStatus()
        {
            InitializeComponent();
            cmbFilter.SelectedIndex = 0;
            customerHelper.filterOrdersByStatus("Select a filter.", ordersTable, orderBinding);
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e) // Handles the filter selection change.
        {
            string selectedFilter = cmbFilter.SelectedItem.ToString();
            customerHelper.filterOrdersByStatus(selectedFilter, ordersTable, orderBinding);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) // Handles the search text change.
        {
            string searchText = txtSearch.Text.Trim();
            formHelper.ApplySearchFilter(orderBinding, searchText, new[] { "Order ID", "Item", "Price" });
        }

        private void btnReturn_Click(object sender, EventArgs e) // Handles the return button click.
        {
            formHelper.returnToDashboard(this);
        }
    }
}
