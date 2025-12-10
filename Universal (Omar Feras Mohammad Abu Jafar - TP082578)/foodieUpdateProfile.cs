using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace IOOP_FoodPoint_Restaurant.Universal__Omar_Feras_Mohammad_Abu_Jafar___TP082578_
{
    public partial class foodieUpdateProfile : Form
    {
    // Used to handle profile update operations.
        private foodieUpdateProfileClass updateProfileHelper = new foodieUpdateProfileClass();

    // Used to handle general form operations.
        private formHelper formHelper = new formHelper();

    // User ID of the logged-in user.
        private int userID = UserSession.UserID;

        public foodieUpdateProfile()
        {
            InitializeComponent();
            updateProfileHelper.loadUserProfileData(userID, txtFirstName, txtLastName, txtEmail, txtNumber); // Load user data into the form.
        }

        private void btnReturn_Click(object sender, EventArgs e) // Handles the return button click event.
        {
            formHelper.returnToDashboard(this);
        }

        private void btnUpdate_Click(object sender, EventArgs e) // Handles updating the user profile.
        {
    // Gather the inputs (regardless of change or not).
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtNumber.Text.Trim();

    // Update the user profile using the helper class, through a boolean to check if the update was successful.
            bool updated = updateProfileHelper.updateUserProfile(userID, firstName, lastName, email, phone);

            if (updated)
            {
    // Return to the dashboard if the update was successful.
                btnReturn.PerformClick();
            }
        }

       
    }
}
