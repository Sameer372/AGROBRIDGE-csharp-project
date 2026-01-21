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
    public partial class DonaationRequest: Form
    {
        private String ID;
        private string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE
        public DonaationRequest(String ID)
        {
            InitializeComponent();
            this.ID = ID;
        }

        private void DonaationRequest_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();

            LoadDonationRequests(); // Load data dynamically
            donationAmount();
        }

        private void LoadDonationRequests()
        {
            displayDonationFlowLayoutPanel.Controls.Clear(); // Clear any existing controls

            try
            {

                string query = "SELECT FarmerID, Name, Contact, Amount, Message, DonateID, Status FROM Applys WHERE Status = 'Under Review'";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string farmerID = reader["FarmerID"].ToString();
                        string name = reader["Name"].ToString();
                        string contact = reader["Contact"].ToString();
                        string amount = reader["Amount"].ToString();
                        string message = reader["Message"].ToString();
                        string donateID = reader["DonateID"].ToString();
                        string status = reader["Status"].ToString();

                        // Create panel for each request
                        Panel requestPanel = new Panel
                        {
                            Size = new Size(500, 250),
                            BorderStyle = BorderStyle.FixedSingle,
                            Margin = new Padding(10)
                        };

                        Label farmerIDLabel = new Label
                        {
                            Text = $"Farmer ID: {farmerID}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Bold),
                            Location = new Point(10, 10),
                            Size = new Size(480, 20)
                        };

                        Label nameLabel = new Label
                        {
                            Text = $"Name: {name}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Regular),
                            Location = new Point(10, 40),
                            Size = new Size(480, 20)
                        };

                        Label contactLabel = new Label
                        {
                            Text = $"Contact: {contact}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Regular),
                            Location = new Point(10, 70),
                            Size = new Size(480, 20)
                        };

                        Label amountLabel = new Label
                        {
                            Text = $"Amount: {amount}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Regular),
                            Location = new Point(10, 100),
                            Size = new Size(480, 20)
                        };

                        Label messageLabel = new Label
                        {
                            Text = $"Message: {message}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Regular),
                            Location = new Point(10, 130),
                            Size = new Size(480, 20)
                        };

                        Label statusLabel = new Label
                        {
                            Text = $"Status: {status}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Regular),
                            Location = new Point(10, 160),
                            Size = new Size(480, 20)
                        };

                        Button acceptButton = new Button
                        {
                            Text = "Accept",
                            Location = new Point(10, 190),
                            Size = new Size(100, 30),
                            BackColor = Color.LightGreen
                        };

                        Button rejectButton = new Button
                        {
                            Text = "Reject",
                            Location = new Point(120, 190),
                            Size = new Size(100, 30),
                            BackColor = Color.IndianRed
                        };

                        // Attach click events for Accept and Reject buttons
                        acceptButton.Click += (sender, e) => UpdateStatus(donateID, "Accepted", statusLabel);
                        rejectButton.Click += (sender, e) => UpdateStatus(donateID, "Rejected", statusLabel);

                        // Add controls to the panel
                        requestPanel.Controls.Add(farmerIDLabel);
                        requestPanel.Controls.Add(nameLabel);
                        requestPanel.Controls.Add(contactLabel);
                        requestPanel.Controls.Add(amountLabel);
                        requestPanel.Controls.Add(messageLabel);
                        requestPanel.Controls.Add(statusLabel);
                        requestPanel.Controls.Add(acceptButton);
                        requestPanel.Controls.Add(rejectButton);
                        rejectButton.Cursor = Cursors.Hand;
                        acceptButton.Cursor = Cursors.Hand;


                        // Add the panel to the FlowLayoutPanel
                        displayDonationFlowLayoutPanel.Controls.Add(requestPanel);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading donation requests: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void donationAmount()
        {
            try
            {
                string query = "SELECT SUM(DonationAmount) AS TotalAmount FROM Donor"; // Correct query for getting total donation amount

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal totalAmount = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : 0;
                                showAvailableAmountlabel.Text = totalAmount.ToString("N2") + " ৳"; // Format to 2 decimal places
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void UpdateStatus(string donateID, string newStatus, Label statusLabel)
        {
            try
            {
                string query = @"
            UPDATE Applys SET Status = @NewStatus WHERE DonateID = @DonateID;
            SELECT Amount FROM Applys WHERE DonateID = @DonateID"; // Get the amount for this donation request

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NewStatus", newStatus);
                    command.Parameters.AddWithValue("@DonateID", donateID);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        decimal donationAmount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : 0;


                        // Update the status label on the UI
                        statusLabel.Text = $"Status: {newStatus}";
                        MessageBox.Show($"Request {newStatus} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to update the status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            DashBoardClass dashBoard = new DashBoardClass(ID);
            dashBoard.Show();
            this.Close();
        }

        private void DashboardButton_Click(object sender, EventArgs e)
        {
            DashBoardClass dashBoard = new DashBoardClass(ID);
            dashBoard.Show();
            this.Close();
        }

        private void displayDonationFlowLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
