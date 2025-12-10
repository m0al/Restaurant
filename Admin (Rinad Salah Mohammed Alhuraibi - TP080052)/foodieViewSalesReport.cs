using System;
using System.Data;
using System.Windows.Forms;

namespace IOOP_FoodPoint_Restaurant.Admin__Rinad_Salah_Mohammed_Alhuraibi___TP080052_
{
    public partial class foodieViewSalesReport : Form
    {
        private adminClass adminHelper = new adminClass();

        public foodieViewSalesReport()
        {
            InitializeComponent();
            cmbFilterCategory.SelectedIndex = 0;

            adminHelper.loadSalesReport(salesTable);
        }

        private void cmbFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCategory = cmbFilterCategory.SelectedItem.ToString();

            if (selectedCategory == "Select a filter.")
            {
                cmbFilter.Enabled = false;
                adminHelper.loadSalesReport(salesTable); // Load full report
                displayTotalRevenue();
            }
            else
            {
                adminHelper.loadSalesFilterOptions(selectedCategory, cmbFilter);
                displayTotalRevenue();
            }
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCategory = cmbFilterCategory.SelectedItem.ToString();
            string selectedFilter = cmbFilter.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedFilter))
            {
                adminHelper.loadSalesReport(salesTable, selectedCategory, selectedFilter);
                displayTotalRevenue();
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            new formHelper().returnToDashboard(this);
        }

        private void displayTotalRevenue()
        {
            decimal totalRevenue = 0;

            // Loop through all rows in the salesTable
            foreach (DataGridViewRow row in salesTable.Rows)
            {
                if (row.Cells["Total"] != null && decimal.TryParse(row.Cells["Total"].Value?.ToString(), out decimal value))
                {
                    totalRevenue += value;
                }
            }

            txtSummary.Text = $"TOTAL REVENUE: {totalRevenue:N2} MYR";
        }
    }
}