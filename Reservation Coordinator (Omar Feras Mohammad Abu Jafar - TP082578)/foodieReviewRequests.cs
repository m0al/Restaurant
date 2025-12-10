using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public partial class foodieReviewRequests : Form
    {
    // SQL components for data loading.
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private BindingSource requestBinding = new BindingSource();

    // Used to manage the reservation requests methods.
        private reservationCoordinatorClass requestHelper = new reservationCoordinatorClass();

    // Used to manage the common form methods.
        private formHelper formHelper = new formHelper();

        public foodieReviewRequests()
        {
            InitializeComponent();

    // Load all pending requests from the database.
            requestHelper.loadPendingRequests(requestsTable, requestBinding);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) // Filter the requests shown based on user input.
        {
            string userInput = txtSearch.Text.Trim().ToLower();
            requestHelper.applySearchFilter(requestBinding, userInput);
        }

        private void requestsTable_CellClick(object sender, DataGridViewCellEventArgs e) // Display the details of the selected request into the TextBox.
        {
            if (e.RowIndex >= 0)
            {
                formHelper.DisplayPreview(
                    requestsTable,
                    txtRow,
                    new[] { "Request ID", "Reservation ID", "Customer", "Request", "Status", "Date" },
                    new[] { "Request ID", "Reservation", "Customer", "Request", "Status", "Date" }
                );
            }
        }

        private void btnExecute_Click(object sender, EventArgs e) // Update the request status.
        {
            requestHelper.updateRequestStatus(requestsTable, () =>
            {
                requestHelper.loadPendingRequests(requestsTable, requestBinding);
                txtRow.Clear();
            });
        }

        private void btnClear_Click(object sender, EventArgs e) // Clear the TextBox.
        {
            txtRow.Clear();
        }

        private void btnReturn_Click(object sender, EventArgs e) // Return to the dashboard.
        {
            formHelper.returnToDashboard(this);
        }
    }
}
