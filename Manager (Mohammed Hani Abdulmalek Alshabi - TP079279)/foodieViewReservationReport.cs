using System;
using System.Data;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Manager__Mohammed_Hani_Abdulmalek_Alshabi___TP079279_
{
    public partial class foodieViewReservationReport : Form
    {
    // Used to load the reservation report and filter options.
        private managerClass managerHelper = new managerClass();

        public foodieViewReservationReport()
        {
            InitializeComponent();
            cmbFilterCategory.SelectedIndex = 0;
            cmbFilter.Enabled = false;

    // Load the reservation report into the DataGridView.
            managerHelper.loadReservationReport(reportsTable);
        }

        private void cmbFilterCategory_SelectedIndexChanged(object sender, EventArgs e) // Sets the filter options based on the selected category.
        {
// Step 1: Get the selected category from the combo box.
            string selectedCategory = cmbFilterCategory.SelectedItem.ToString();

// Step 2: If the selected category is "Select a filter.", disable the filter combo box and load the full report.
            if (selectedCategory == "Select a filter.")
            {
                cmbFilter.Enabled = false;
                managerHelper.loadReservationReport(reportsTable);
            }

// Step 3: If the selected category is not "Select a filter.", enable the filter combo box and load the filter options.
            else
            {
                cmbFilter.Enabled = true;
                managerHelper.loadFilterOptions(selectedCategory, cmbFilter);
            }
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e) // Loads the filtered report based on the selected filter.
        {
            if (cmbFilter.Enabled && cmbFilter.SelectedIndex != -1)
            {
// Step 1: Get the selected category and filter value.
                string category = cmbFilterCategory.SelectedItem.ToString();

// Step 2: Get the selected filter value from the combo box.
                string value = cmbFilter.SelectedItem.ToString();

// Step 3: Load the filtered report into the DataGridView.
                managerHelper.loadReservationReport(reportsTable, category, value);
            }
        }

        private void btnReturn_Click(object sender, EventArgs e) // Redirects to the dashboard.
        {
            new formHelper().returnToDashboard(this);
        }
    }
}
