using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgroBridge
{
    public partial class ApplyForDonateClass : Form
    {
        private String ID;
        // Database connection string
        string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE


        public ApplyForDonateClass(string id)
        {
            InitializeComponent();
            ID = id;
        }

        private void ApplyForDonateClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();
            idTextBox.Text = ID; // Display the ID
            idTextBox.ReadOnly = true; // Make it read-only
            
            applyForDonateButton.Enabled = false;
            applyForDonateButton.ForeColor = Color.FromArgb(117, 55, 216);


            flowLayoutPanelForOperationalButton0.Size = new Size(45, 1641);
            flowLayoutPanelForOperationalButton1.AutoScroll = false;
            flowLayoutPanelForOperationalButton2.AutoScroll = false;


            LoadData(); // Load data into DataGridView
            CalculateTotal(); // Calculate the total amount




        }

        private string GenerateDonateID()
        {
            // Generate a random 4-digit number
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999);
            return $"D-{randomNumber}";
        }
        

        private void totalPriceTextBox_TextChanged(object sender, EventArgs e)
        {
            // Intentionally left empty.
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Intentionally left empty.
        }

        private void LoadData()
        {
            try
            {
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT DonateID, Amount AS DonateAmount, Status FROM Applys WHERE FarmerID = @FarmerID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FarmerID", ID);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable table = new DataTable();

                        connection.Open();
                        adapter.Fill(table);
                        connection.Close();

                        dataGridView1.DataSource = table; // Bind the data

                        // Customize DataGridView Header Style
                        dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8);
                        dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
                        dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                        dataGridView1.EnableHeadersVisualStyles = false;
                        

                        // Apply row-level formatting
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["Status"].Value != null)
                            {
                                string status = row.Cells["Status"].Value.ToString();
                                if (status == "Under Review")
                                {
                                    row.Cells["Status"].Style.ForeColor = Color.Orange;
                                }
                                else if (status == "Accepted")
                                {
                                    row.Cells["Status"].Style.ForeColor = Color.Green;
                                }
                                else if (status == "Rejected")
                                {
                                    row.Cells["Status"].Style.ForeColor = Color.Red;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void CalculateTotal()
        {
            try
            {
                decimal total = 0;
                decimal totalAccepted = 0;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["DonateAmount"].Value != null)
                    {
                        decimal amount = Convert.ToDecimal(row.Cells["DonateAmount"].Value);
                        total += amount;

                        // Check if status is "Accepted" and add to accepted total
                        if (row.Cells["Status"].Value != null && row.Cells["Status"].Value.ToString() == "Accepted")
                        {
                            totalAccepted += amount;
                        }
                    }
                }

                // Display totals
                totalPriceTextBox.Text = total.ToString("0.00 TK");
                totalAcceptedPriceTextBox.Text = totalAccepted.ToString("0.00 TK"); // Ensure you have a TextBox named totalAcceptedAmountTextBox
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating total: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void applyButton_Click_1(object sender, EventArgs e)
        {
            // Get values from textboxes
            string name = nameTextBox.Text.Trim();
            string contact = phoneTextBox.Text.Trim();
            string amountText = donateAmountTextBox.Text.Trim();
            string message = applyReasonTextBox.Text.Trim();
            string donateID = GenerateDonateID();
            string status = "Under Review"; // Default status for new applications

            // Validate the inputs
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(contact) || string.IsNullOrWhiteSpace(amountText) || string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("All fields are required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(amountText, out decimal amount))
            {
                MessageBox.Show("Invalid amount. Please enter a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
               

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert query
                    string query = "INSERT INTO Applys (FarmerID, Name, Contact, Amount, Message, DonateID, Status) " +
                                   "VALUES (@FarmerID, @Name, @Contact, @Amount, @Message, @DonateID, @Status)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FarmerID", ID);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Contact", contact);
                        command.Parameters.AddWithValue("@Amount", amount);
                        command.Parameters.AddWithValue("@Message", message);
                        command.Parameters.AddWithValue("@DonateID", donateID);
                        command.Parameters.AddWithValue("@Status", status);

                        // Execute the query
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Application submitted successfully! Your Donate ID: {donateID}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Clear the textboxes
                            nameTextBox.Clear();
                            phoneTextBox.Clear();
                            donateAmountTextBox.Clear();
                            applyReasonTextBox.Clear();

                            // Refresh the data
                            LoadData();
                            CalculateTotal();
                        }
                        else
                        {
                            MessageBox.Show("Failed to submit application. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuBarButton_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanelForOperationalButton0.Width <= 45)
            {
                flowLayoutPanelForOperationalButton0.Size = new Size(172, 1641);


            }
            else
            {
                flowLayoutPanelForOperationalButton0.Size = new Size(45, 1641);

            }

            flowLayoutPanelForOperationalButton0.Refresh(); // Force UI update
            flowLayoutPanelForOperationalButton1.AutoScroll = false;
            flowLayoutPanelForOperationalButton2.AutoScroll = false;
        }


        //For All Button Functionlity
        private void allItemButton_Click(object sender, EventArgs e)
        {
            HomePage homePage = new HomePage(ID);
            homePage.Show();
            this.Close();
        }

        private void allWasteButton_Click(object sender, EventArgs e)
        {
            AllWasteItem allWasteItem = new AllWasteItem(ID);
            allWasteItem.Show();
            this.Close();
        }

        private void rentEquipmentButton_Click(object sender, EventArgs e)
        {
            RentEquipmentClass rentEquipmentClass = new RentEquipmentClass(ID);
            rentEquipmentClass.Show();
            this.Close();
        }

        private void hireWorkerButton_Click(object sender, EventArgs e)
        {
            HireWorkerClass hireWorkerClass = new HireWorkerClass(ID);
            hireWorkerClass.Show();
            this.Close();
        }

        private void updateAndDeleteButton_Click(object sender, EventArgs e)
        {
            UpdateAndDeleteClass updateAndDeleteClass = new UpdateAndDeleteClass(ID);
            updateAndDeleteClass.Show();
            this.Close();
        }

        private void updateEquipmentInformaationButton_Click(object sender, EventArgs e)
        {
            UpdateEquipmentInformaationClass updateEquipmentInformaationClass = new UpdateEquipmentInformaationClass(ID);
            updateEquipmentInformaationClass.Show();
            this.Close();
        }

        private void addGoodsButton_Click(object sender, EventArgs e)
        {
            ProductAdd productAdd = new ProductAdd(ID);
            productAdd.ShowDialog();
        }

        private void logoutButtton_Click(object sender, EventArgs e)
        {
            LoginClass loginClass = new LoginClass();
            loginClass.Show();
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
