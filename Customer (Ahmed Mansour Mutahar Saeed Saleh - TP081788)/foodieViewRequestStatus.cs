using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using IOOP_FoodPoint_Restaurant;

namespace IOOP_FoodPoint_Restaurant.Customer__Ahmed_Mansour_Mutahar_Saeed_Saleh___TP081788_
{
    public partial class foodieViewRequestStatus : Form
    {
// Used to handle customer-related operations.
        customerClass customerHelper = new customerClass();

// Used to handle general form-related operations.
        formHelper formHelper = new formHelper();

        private BindingSource requestBinding = new BindingSource(); // Binding source for the requests table.

        public foodieViewRequestStatus()
        {
            InitializeComponent();
            customerHelper.filterRequestsByStatus("Select a filter.", requestsTable, requestBinding);
            cmbFilter.SelectedIndex = 0;
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e) // Handles the filter selection change event.
        {
            string selected = cmbFilter.SelectedItem.ToString();
            customerHelper.filterRequestsByStatus(selected, requestsTable, requestBinding);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) // Handles the search text change event.
        {
            string input = txtSearch.Text.Trim();
            string[] fields = { "Request ID", "Reservation ID", "Request", "Status", "Date" };
            formHelper.ApplySearchFilter(requestBinding, input, fields);
        }

        private void btnReturn_Click(object sender, EventArgs e) // Handles the return button click event.
        {
            formHelper.returnToDashboard(this);
        }
    }
}