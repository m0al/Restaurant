using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Customer__Ahmed_Mansour_Mutahar_Saeed_Saleh___TP081788_
{
    public partial class foodieOrderFood : Form
    {
        // Used to manage customer-related operations.
        private customerClass customerHelper = new customerClass();

        // Used to manage general form-related operations.
        private formHelper formHelper = new formHelper();

        private SqlDataAdapter dataAdapter; // Used to retrieve data from the database.
        private DataTable dataTable; // Holds the data retrieved from the database.
        private BindingSource menuBinding = new BindingSource(); // Used to bind data to UI controls.

        public foodieOrderFood()
        {
            InitializeComponent();
            customerHelper.loadMenu(menuTable, menuBinding, ref dataAdapter, ref dataTable);
            cmbFilter.SelectedIndex = 0;
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e) // Event handler for filter selection change.
        {
            string selectedCategory = cmbFilter.SelectedItem.ToString();
            customerHelper.filterMenuByCategory(selectedCategory, menuTable, menuBinding);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) // Event handler for search text change.
        {
            string input = txtSearch.Text.Trim().ToLower();
            string[] fields = { "Menu ID", "Category", "Item", "Price" };
            formHelper.ApplySearchFilter(menuBinding, input, fields);
        }

        private void btnOrder_Click(object sender, EventArgs e) // Event handler for order button click.
        {
            customerHelper.orderItem(menuTable);
        }

        private void btnReturn_Click(object sender, EventArgs e) // Event handler to return to the dashboard.
        { 
                formHelper.returnToDashboard(this);
        }
    }
}
