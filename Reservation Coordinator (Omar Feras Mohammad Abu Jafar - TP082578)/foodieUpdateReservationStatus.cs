using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public partial class foodieUpdateReservationStatus : Form
    {
    // Adapter and table to load reservation data.
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;

    // BindingSource to allow filtering/searching.
        private BindingSource reservationBinding = new BindingSource();

    // Reuse the shared form helper and reservation coordinator class.
        private formHelper formHelper = new formHelper();
        private reservationCoordinatorClass reservationHelper = new reservationCoordinatorClass();

        public foodieUpdateReservationStatus()
        {
            InitializeComponent();

    // Load the reservations into the table when the form loads.
            reservationHelper.loadReservations(reservationsTable, txtRow, reservationBinding, ref dataAdapter, ref dataTable);

    // Reset selection.
            txtRow.Text = string.Empty;
        }

        private void reservationsTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
        // Display a simple preview of the selected reservation.
                formHelper.DisplayPreview(
                    reservationsTable,
                    txtRow,
                    new[] { "ID", "Customer", "Pax", "Hall", "Time", "Notes", "Status", "Creator" },
                    new[] { "ID", "Customer", "Pax", "Hall", "Time", "Notes", "Status", "Creator" });
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
    // Filter the reservation list based on input.
            string input = txtSearch.Text.Trim().ToLower();
            string[] filterFields = { "ID", "Customer", "Hall", "Status" };
            formHelper.ApplySearchFilter(reservationBinding, input, filterFields);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
    // Trigger popup to update the selected reservation's status.
            reservationHelper.updateReservationStatus(reservationsTable, () =>
                reservationHelper.loadReservations(reservationsTable, txtRow, reservationBinding, ref dataAdapter, ref dataTable));

    // Clear the row preview after update.
            txtRow.Clear();
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
