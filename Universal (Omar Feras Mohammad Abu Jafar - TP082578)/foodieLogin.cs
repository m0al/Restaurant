using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public partial class foodieLogin : Form
    {
// Used to handle login operations.
        private foodieLoginClass loginHandler = new foodieLoginClass();

        public foodieLogin()
        {
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true; // Hide password by default.
        }

        private void btnReveal_Click(object sender, EventArgs e) // Toggle password visibility.
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            btnReveal.Text = txtPassword.UseSystemPasswordChar ? "Show" : "Hide";
        }

        private void btnClear_Click(object sender, EventArgs e) // Clear both text fields and reset toggle.
        {
            clearLoginFields(); // Use the new helper method to clear fields.
        }

        private void btnLogin_Click(object sender, EventArgs e) // Login button click event.
        {
// Step 1: Get user input from text fields.
            string userInput = txtUsername.Text.Trim();
            string inputPassword = txtPassword.Text.Trim();

// Step 2: Check if any of the fields are empty.
            if (string.IsNullOrEmpty(userInput) || string.IsNullOrEmpty(inputPassword))
            {
    // Show a message box if any field is empty and reset form.
                MessageBox.Show("Please enter your User ID, Email, or Phone Number & Password.", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                clearLoginFields();
                return;
            }

// Step 3: Call the login handler to verify credentials.
            string userRole = loginHandler.verifyCredentials(userInput, inputPassword, this);

// Step 4: Check if the user role is not null (indicating a successful login).
            if (userRole != null)
            {
                this.Hide();
                new foodieDashboard().Show();
            }
        }

        private void btnForgot_Click(object sender, EventArgs e) // Forgot password button click event.
        {
            foodieForgotPassword forgotPasswordForm = new foodieForgotPassword();
            forgotPasswordForm.Show();
            this.Hide();
        }

        public void clearLoginFields() // Method to clear the form when needed.
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtPassword.UseSystemPasswordChar = true;
            btnReveal.Text = "Show";
        }
    }
}