using System;
using System.Windows.Forms;
using IOOP_FoodPoint_Restaurant.Customer__Ahmed_Mansour_Mutahar_Saeed_Saleh___TP081788_;
using IOOP_FoodPoint_Restaurant.Manager__Mohammed_Hani_Abdulmalek_Alshabi___TP079279_;

namespace IOOP_FoodPoint_Restaurant
{
    public partial class foodieDashboard : Form
    {
    // Used to manage the live clock.
        private Timer clockTimer;

    // Used to manage the dashboard menu and labels.
        private foodieDashboardClass dashboardHelper = new foodieDashboardClass();

        public foodieDashboard()
        {
            InitializeComponent();

    // Set up and start the live clock.
            clockTimer = new Timer();
            clockTimer.Interval = 1000; // Update every second.
            clockTimer.Tick += updateClock;
            clockTimer.Start();

    // Set welcome message and build dashboard menu.
            dashboardHelper.setupDashboardLabels(lblTitle, lblDescription, dashboardPanel);
            dashboardHelper.setupMenuByRole(menuBar, UserSession.UserRole, logoutItem_Click, this);
        }

        private void updateClock(object sender, EventArgs e) // Update the clock label every second.
        {
            lblClock.Text = DateTime.Now.ToString("HH:mm:ss"); // 24-hour format
            lblClock.Left = (dashboardPanel.Width - lblClock.Width) / 2;
        }

        private void logoutItem_Click(object sender, EventArgs e) // Handle logout action.
        {
    // Confirm logout action.
            DialogResult confirmLogout = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

    // If user confirms, log out and show login form.
            if (confirmLogout == DialogResult.Yes)
            {
                UserSession.Logout();
                foodieLogin loginForm = new foodieLogin();
                loginForm.Show();
                this.Close();
            }
        }




        // ========== PLACEHOLDER ========== //
    }
}
