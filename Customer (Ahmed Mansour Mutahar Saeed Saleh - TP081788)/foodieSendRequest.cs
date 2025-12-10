using System;
using System.Data;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Customer__Ahmed_Mansour_Mutahar_Saeed_Saleh___TP081788_
{
    public partial class foodieSendRequest : Form
    {
        // Create an instance of customerClass to use its methods
        private customerClass customerHelper = new customerClass();

        public foodieSendRequest()
        {
            InitializeComponent();

            // Load user's upcoming reservations into ComboBox
            customerHelper.loadUpcomingReservationsForRequest(cmbReservation);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // Submit the user's request
            customerHelper.submitReservationRequest(cmbReservation, txtRequest);

            // Clear the form after submission
            cmbReservation.SelectedIndex = -1;
            txtRequest.Clear();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            // Navigate back to dashboard
            formHelper formHelper = new formHelper();
            formHelper.returnToDashboard(this);
        }
    }
}
