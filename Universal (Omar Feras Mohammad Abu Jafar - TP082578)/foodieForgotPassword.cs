using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant
{
    public partial class foodieForgotPassword : Form
    {
    // Used to handle password reset operations.
        private foodieForgotPasswordClass passwordManager = new foodieForgotPasswordClass();

        public foodieForgotPassword()
        {
            InitializeComponent();

    // Hide password-related controls on load.
            txtConfirmPassword.UseSystemPasswordChar = true;
            txtNewPassword.UseSystemPasswordChar = true;

            lblDigit.Visible = false;
            txtCode.Visible = false;
            btnConfirm.Visible = false;

            lblNewPassword.Visible = false;
            txtNewPassword.Visible = false;
            btnNewReveal.Visible = false;

            lblConfirmPassword.Visible = false;
            txtConfirmPassword.Visible = false;
            btnConfirmReveal.Visible = false;

            btnChangePassword.Visible = false;
            btnClear.Visible = false;
        }

        private void btnCancel_Click(object sender, EventArgs e) // Returns user to the login screen.
        {
            int userID = UserSession.UserID;
            if (userID != 0)
            {
                this.Hide();
                foodieDashboard dashboard = new foodieDashboard();
                dashboard.Show();
                this.Close();

            } else {
                this.Hide();
                foodieLogin foodieLogin = new foodieLogin();
                foodieLogin.Show();
                this.Close();
            }
        }

        private async void btnSend_Click(object sender, EventArgs e) // Handles the sending of the verification email.
        {
// Step 1: Get user input from the text field.
            string userInput = txtUsername.Text.Trim();

// Step 2: Check if the input field is empty.
            if (string.IsNullOrEmpty(userInput))
            {
                MessageBox.Show("Please enter your User ID, Email, or Phone Number.", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

// Step 3: Validate the user and retrieve their email
            bool userFound = await passwordManager.getUserEmailAsync(userInput);

            if (!userFound)
            {
                MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

// Step 4: Send the verification email
            bool emailSent = await passwordManager.sendVerificationEmailAsync();

            if (emailSent)
            {
    // Show success message.
                MessageBox.Show("Verification code has been sent to your email.", "Email Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);

    // Show verification input.
                lblDigit.Visible = true;
                txtCode.Visible = true;
                btnConfirm.Visible = true;
            }
            else
            {
    // Show error message.
                MessageBox.Show("Failed to send verification code.", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e) // Handles the verification code confirmation.
        {
// Step 1: Get user input from the verification code field.
            string userCode = txtCode.Text.Trim();

// Step 2: Check if the verification code field is empty.
            if (string.IsNullOrEmpty(userCode))
            {
                MessageBox.Show("Please enter the verification code.", "Missing Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

// Step 3: Verify the code.
            if (passwordManager.verifyCode(userCode))
            {
                MessageBox.Show("Verification successful. Please enter your new password.", "Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);

    // Show password reset fields.
                lblNewPassword.Visible = true;
                txtNewPassword.Visible = true;
                btnNewReveal.Visible = true;

                lblConfirmPassword.Visible = true;
                txtConfirmPassword.Visible = true;
                btnConfirmReveal.Visible = true;

                btnChangePassword.Visible = true;
                btnClear.Visible = true;

                txtUsername.Enabled = false;
                btnSend.Enabled = false;
                txtCode.Enabled = false;
            }
            else
            {
    // Show error message.
                MessageBox.Show("Incorrect verification code.", "Invalid Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnChangePassword_Click(object sender, EventArgs e) // Handles the password change.
        {
// Step 1: Get user input from the new password fields.
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

// Step 2: Check if any of the password fields are empty.
            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill in both password fields.", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

// Step 3: Check if the new password meets the criteria.
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

// Step 4: Check if the new password successfully changed.
            bool passwordChangeSatus = await passwordManager.ResetPasswordAsync(newPassword);

            if (passwordChangeSatus)
            {
    // Show success message and redirect to login.
                MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                foodieLogin loginForm = new foodieLogin();
                loginForm.Show();
                this.Close();
            }
            else
            {
    // Show error message.
                MessageBox.Show("Password update failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e) // Clears all input fields.
        {
            txtConfirmPassword.Clear();
            txtNewPassword.Clear();

            txtNewPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.UseSystemPasswordChar = true;

            btnNewReveal.Text = "Show";
            btnConfirmReveal.Text = "Show";
        }

        private void btnNewReveal_Click(object sender, EventArgs e) // Toggles the visibility of the new password.
        {
            txtNewPassword.UseSystemPasswordChar = !txtNewPassword.UseSystemPasswordChar;
            btnNewReveal.Text = txtNewPassword.UseSystemPasswordChar ? "Show" : "Hide";
        }

        private void btnConfirmReveal_Click(object sender, EventArgs e) // Toggles the visibility of the confirm password.
        {
            txtConfirmPassword.UseSystemPasswordChar = !txtConfirmPassword.UseSystemPasswordChar;
            btnConfirmReveal.Text = txtConfirmPassword.UseSystemPasswordChar ? "Show" : "Hide";
        }
    }
}
