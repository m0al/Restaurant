using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public partial class foodieManageReservations : Form
    {
    // SQL adapter and datatable to interact with the database.
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;

    // Binding source to dynamically bind filtered/updated data.
        private BindingSource reservationBinding = new BindingSource();

    // Used to display handle the functionality of the form.
        private reservationCoordinatorClass reservationHelper = new reservationCoordinatorClass();

    // Used to handle the functionality of the form (general stuff that other forms use).
        private formHelper formHelper = new formHelper();

        public foodieManageReservations()
        {
            InitializeComponent();

    // Load all active reservations into the DataGridView.
            reservationHelper.loadReservations(reservationsTable, txtRow, reservationBinding, ref dataAdapter, ref dataTable);

    // Default dropdown to no action selected.
            cmbAction.SelectedIndex = 0;
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

        private void txtSearch_TextChanged(object sender, EventArgs e) // Event handler for when the text in the search box changes.
        {
    // Apply real-time filtering of the reservations list.
            string searchInput = txtSearch.Text.Trim().ToLower();
            string[] searchableFields = { "ID", "Customer", "Pax", "Hall", "Time", "Notes", "Status", "Creator" };

            formHelper.ApplySearchFilter(reservationBinding, searchInput, searchableFields);
        }

        private void btnClear_Click(object sender, EventArgs e) // Clear the preview textbox.
        {
            txtRow.Clear();
        }

        private void btnExecute_Click(object sender, EventArgs e) // Execute the selected action from the dropdown.
        {
    // Get the selected action from the dropdown.
            int selectedIndex = cmbAction.SelectedIndex;

    // Warn user if no action was selected.
            if (selectedIndex == 0)
            {
                MessageBox.Show("Please select an action.", "Missing Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

    // Calls the appropriate reservation function.
            if (selectedIndex == 1)
                reservationHelper.addReservation(() => reservationHelper.loadReservations(reservationsTable, txtRow, reservationBinding, ref dataAdapter, ref dataTable));
            else if (selectedIndex == 2)
                reservationHelper.editReservation(reservationsTable, () => reservationHelper.loadReservations(reservationsTable, txtRow, reservationBinding, ref dataAdapter, ref dataTable));
            else if (selectedIndex == 3)
                reservationHelper.deleteReservation(reservationsTable, () => reservationHelper.loadReservations(reservationsTable, txtRow, reservationBinding, ref dataAdapter, ref dataTable));

    // Reset the UI controls after action completes.
            cmbAction.SelectedIndex = 0;
            btnClear.PerformClick();
        }

        private void btnReturn_Click(object sender, EventArgs e) // Return the user to the main dashboard.
        {
            formHelper.returnToDashboard(this);
        }
    }
}
