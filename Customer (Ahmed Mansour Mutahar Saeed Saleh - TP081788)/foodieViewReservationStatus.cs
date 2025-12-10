using System;
using System.Data;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Customer__Ahmed_Mansour_Mutahar_Saeed_Saleh___TP081788_
{
    public partial class foodieViewReservationStatus : Form
    {
// Used to handle customer-related operations.
        private customerClass customerHelper = new customerClass();

// Used to handle general form-related operations.
        private formHelper formHelper = new formHelper();

        private BindingSource reservationBinding = new BindingSource(); // Binding source to manage the reservations.

        public foodieViewReservationStatus()
        {
            InitializeComponent();
            cmbFilter.SelectedIndex = 0;
            customerHelper.filterReservationsByStatus("Select a filter.", reservationsTable, reservationBinding);
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e) // Event handler for filter selection change.
        {
            string selectedFilter = cmbFilter.SelectedItem.ToString();
            customerHelper.filterReservationsByStatus(selectedFilter, reservationsTable, reservationBinding);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) // Event handler for search text change.
        {
            // Get search input and apply the filter on reservationBinding
            string input = txtSearch.Text.Trim().ToLower();
            string[] fields = { "Reservation ID", "Hall", "Pax", "Status" }; // Fields to search in the reservations table
            formHelper.ApplySearchFilter(reservationBinding, input, fields);
        }

        private void btnReturn_Click(object sender, EventArgs e) // Event handler for return to dashboard.
        {
            formHelper.returnToDashboard(this);
        }
    }
}
