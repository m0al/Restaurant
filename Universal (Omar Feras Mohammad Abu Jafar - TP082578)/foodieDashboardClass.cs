using System;
using System.Windows.Forms;
using IOOP_FoodPoint_Restaurant.Admin__Rinad_Salah_Mohammed_Alhuraibi___TP080052_;
using IOOP_FoodPoint_Restaurant.Customer__Ahmed_Mansour_Mutahar_Saeed_Saleh___TP081788_;
using IOOP_FoodPoint_Restaurant.Manager__Mohammed_Hani_Abdulmalek_Alshabi___TP079279_;
using IOOP_FoodPoint_Restaurant.Universal__Omar_Feras_Mohammad_Abu_Jafar___TP082578_;

namespace IOOP_FoodPoint_Restaurant
{
    public class foodieDashboardClass
    {
        public void setupDashboardLabels(Label lblTitle, Label lblDescription, Panel dashboardPanel) // Setup the labels on the dashboard to show a welcome message and user role.
        {
            lblTitle.Text = $"Welcome, {UserSession.UserName}";
            lblDescription.Text = $"{UserSession.UserRole} Dashboard";

            lblTitle.Left = (dashboardPanel.Width - lblTitle.Width) / 2;
            lblDescription.Left = (dashboardPanel.Width - lblDescription.Width) / 2;
        }

        public void setupMenuByRole(MenuStrip menuBar, string userRole, EventHandler logoutHandler, Form parentForm) // Setup the menu bar based on the user's role.
        {
            menuBar.Items.Clear(); // Clear any existing items to avoid duplicates.

    // Add menus based on each user role.
            switch (userRole)
            {
                case "Admin":
                    menuBar.Items.Add(createMenu("User Management", new[]
                    {
                        "Manage Users", "View Sales Reports", "View Customer Feedback"
                    }, parentForm));
                    break;

                case "Manager":
                    menuBar.Items.Add(createMenu("Business Management", new[]
                    {
                        "Manage Menu", "Manage Halls", "View Reservation Reports"
                    }, parentForm));
                    break;

                case "Chef":
                    menuBar.Items.Add(createMenu("Kitchen Operations", new[]
                    {
                        "Manage Orders", "Manage Inventory"
                    }, parentForm));

                    menuBar.Items.Add(createMenu("View History", new[]
                    {
                        "Orders"
                    }, parentForm));
                    break;

                case "Reservation Coordinator":
                    menuBar.Items.Add(createMenu("Manage Reservations", new[]
                    {
                        "Manage Reservations", "Update Reservation Status", "View Reservation History"
                    }, parentForm));

                    menuBar.Items.Add(createMenu("Manage Requests", new[]
                    {
                        "Review Requests"
                    }, parentForm));
                    break;

                case "Customer":
                    menuBar.Items.Add(createMenu("My Orders", new[]
                    {
                        "Search and Order Food", "View Order Status"
                    }, parentForm));

                    menuBar.Items.Add(createMenu("My Reservations", new[]
                    {
                        "View Reservation Status", "View Request Status", "Send Request"
                    }, parentForm));

                    menuBar.Items.Add(createMenu("Feedback", new[]
                    {
                        "Send Feedback"
                    }, parentForm));
                    break;
            }

    // Add universal items for all users.
            menuBar.Items.Add(createMenu("My Account", new[]
            {
                "Update Profile", "Change Password"
            }, parentForm));

    // Add logout option.
            menuBar.Items.Add(createMenu("Logout", new[] { "Log Out" }, parentForm, logoutHandler));
        }
  
        private ToolStripMenuItem createMenu(string title, string[] subItems, Form parentForm, EventHandler generalHandler = null) // This method creates a menu with sub-items and attaches click handlers.
        {
            ToolStripMenuItem mainItem = new ToolStripMenuItem(title);

// Step 1: Set up the main menu item.
            foreach (string subItem in subItems)
            {
                ToolStripMenuItem subMenu = new ToolStripMenuItem(subItem);

    // If a general handler is provided (e.g., for logout), use that.
                if (generalHandler != null)
                {
                    subMenu.Click += generalHandler;
                }
                else
                {
// Step 2: Attach specific handlers based on submenu item name.
                    subMenu.Click += (sender, e) =>
                    {
                        Form.ActiveForm.Hide();

                        switch (subItem)
                        {
                            // ADMIN //
                            case "Manage Users":
                                new foodieManageUsers().Show();
                                break;
                            case "View Sales Reports":
                                new foodieViewSalesReport().Show();
                                break;
                            case "View Customer Feedback":
                                new foodieViewFeedback().Show();
                                break;

                            // MANAGER //
                            case "Manage Menu":
                                new foodieManageMenu().Show();
                                break;
                            case "Manage Halls":
                                new foodieManageHall().Show();
                                break;
                            case "View Reservation Reports":
                                new foodieViewReservationReport().Show();
                                break;


                            // RESERVATION COORDINATOR //
                            case "Manage Reservations":
                                new foodieManageReservations().Show();
                                break;
                            case "Update Reservation Status":
                                new foodieUpdateReservationStatus().Show();
                                break;
                            case "View Reservation History":
                                new foodieReservationHistory().Show();
                                break;
                            case "Review Requests":
                                new foodieReviewRequests().Show();
                                break;


                            // CHEF //
                            case "Orders":
                                new foodieOrderHistory().Show();
                                break;
                            case "Manage Orders":
                                new foodieUpdateOrderStatus().Show();
                                break;
                            case "Manage Inventory":
                                new foodieManageInventory().Show();
                                break;

                            // CUSTOMER //
                            case "Search and Order Food":
                                new foodieOrderFood().Show();
                                break;
                            case "View Order Status":
                                new foodieViewOrderStatus().Show();
                                break;
                             case "View Reservation Status":
                                new foodieViewReservationStatus().Show();
                                break;
                            case "View Request Status":
                                new foodieViewRequestStatus().Show();
                                break;
                            case "Send Feedback":
                                new foodieSendFeedback().Show();
                                break;
                            case "Send Request":
                                new foodieSendRequest().Show();
                                break;


                            case "Change Password":
                                new foodieForgotPassword().Show();
                                break;
                            case "Update Profile":
                                new foodieUpdateProfile().Show();
                                break;
                        }
                    };
                }

// Step 3: Add the submenu to the main menu item.
                mainItem.DropDownItems.Add(subMenu);
            }

            return mainItem;
        }
    }
}
