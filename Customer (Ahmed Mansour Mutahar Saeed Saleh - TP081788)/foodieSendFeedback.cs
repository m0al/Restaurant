using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Customer__Ahmed_Mansour_Mutahar_Saeed_Saleh___TP081788_
{
    public partial class foodieSendFeedback : Form
    {
        private customerClass customerHelper = new customerClass();
        private formHelper formHelper = new formHelper();

        public foodieSendFeedback()
        {
            InitializeComponent();
            customerHelper.loadPastReservationsForFeedback(cmbReservation);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string selectedReservation = cmbReservation.SelectedItem?.ToString();
            string feedbackText = txtFeedback.Text.Trim();

            customerHelper.submitFeedback(selectedReservation, feedbackText);
            txtFeedback.Clear();
            cmbReservation.SelectedIndex = -1;
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            formHelper.returnToDashboard(this);
        }
    }
}
