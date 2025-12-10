using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public partial class foodieReservationHistory : Form
    {
        // Used for binding data to the DataGridView.
        private BindingSource reservationBinding = new BindingSource();

        // Helper classes.
        private reservationCoordinatorClass reservationHelper = new reservationCoordinatorClass();
        private formHelper formHelper = new formHelper();

        public foodieReservationHistory()
        {
            InitializeComponent();

            // Load all past/completed reservations by default.
            reservationHelper.loadReservationHistory(reservationsTable, txtRow, reservationBinding);

            // Set default filter.
            cmbFilter.SelectedIndex = 0;
        }
        private void reservationsTable_CellClick(object sender, DataGridViewCellEventArgs e) // Event handler for when a cell in the DataGridView is clicked.
        {
            // Show row data in the preview box only when a row is clicked.
            if (e.RowIndex >= 0)
            {
                formHelper.DisplayPreview(
                reservationsTable,
                txtRow,
                new[] { "ID", "Customer", "Pax", "Hall", "Time", "Notes", "Status", "Creator" },
                new[] { "Reservation ID", "Customer", "Pax", "Hall", "Time", "Notes", "Status", "Creator" });
            }
        }

        private void cmbFilter_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // Fetch selected filter option.
            string selectedFilter = cmbFilter.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedFilter) || selectedFilter == "Select a filter.")
            {
                // Default behavior: show all Complete or Missed reservations.
                reservationHelper.loadReservationHistory(reservationsTable, txtRow, reservationBinding);
            }
            else
            {
                // Apply filtered query based on selection.
                reservationHelper.applyReservationHistoryFilter(selectedFilter, reservationsTable, txtRow, reservationBinding);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string userInput = txtSearch.Text.Trim().ToLower();
            string[] filterFields = { "ID", "Customer", "Hall", "Status" };
            formHelper.ApplySearchFilter(reservationBinding, userInput, filterFields);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRow.Clear();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            // Return to main dashboard.
            formHelper.returnToDashboard(this);
        }
    }
}