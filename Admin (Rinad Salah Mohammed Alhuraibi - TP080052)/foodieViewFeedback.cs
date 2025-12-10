using System;
using System.Data;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Admin__Rinad_Salah_Mohammed_Alhuraibi___TP080052_
{
    public partial class foodieViewFeedback : Form
    {
    // This class is responsible for displaying and managing feedback from users in the application.
        adminClass adminHelper = new adminClass();

    // This class is responsible for handling general form-related operations.
        formHelper formHelper = new formHelper();

        public foodieViewFeedback()
        {
            InitializeComponent();
            cmbFilterCategory.SelectedIndex = 0;
            cmbFilter.Enabled = false;
            adminHelper.loadFeedback(feedbackTable, "Select a filter."); // Load all feedback by default.
        }

        private void feedbackTable_CellClick(object sender, DataGridViewCellEventArgs e) // This method handles the event when a cell in the feedback table is clicked.
        {
    // Check if the clicked cell is valid and contains a comment.
            if (e.RowIndex >= 0 && feedbackTable.Rows[e.RowIndex].Cells["Comment"] != null)
            {
                DataGridViewRow selectedRow = feedbackTable.Rows[e.RowIndex];
                txtFeedback.Text = selectedRow.Cells["Comment"].Value.ToString();
            }
        }


        private void cmbFilterCategory_SelectedIndexChanged(object sender, EventArgs e) // This method handles the event when a filter category is selected from the dropdown.
        {
    // Get the selected category from the dropdown.
            string selectedCategory = cmbFilterCategory.SelectedItem.ToString();

    // Disable the filter dropdown and clear its items.
            cmbFilter.Enabled = false;
            cmbFilter.Items.Clear();

    // Load the feedback based on the selected category.
            if (selectedCategory == "Select a filter.")
            {
                adminHelper.loadFeedback(feedbackTable, selectedCategory);
            }
            else if (selectedCategory == "By Month")
            {
                // Now load months and enable
                adminHelper.loadFeedbackFilterOptions(selectedCategory, cmbFilter);
                cmbFilter.Enabled = true;
                cmbFilter.SelectedIndex = 0;
            }
        }


        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e) // This method handles the event when a filter option is selected from the dropdown.
        {
    // Get the selected category and value from the dropdowns.
            string category = cmbFilterCategory.SelectedItem.ToString();
            string value = cmbFilter.SelectedItem?.ToString();

    // Load the feedback based on the selected category and value.
            if (!string.IsNullOrEmpty(value))
                adminHelper.loadFeedback(feedbackTable, category, value);
        }

        private void btnReturn_Click(object sender, EventArgs e) // This method handles the event when the return button is clicked.
        {
            formHelper.returnToDashboard(this);
        }
    }
}
