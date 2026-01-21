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
    public partial class DonorClass : Form
    {

        private String ID;
        // Database connection string
        string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

        public DonorClass(string iD)
        {
            InitializeComponent();
            ID = iD;
        }

        

        private void DonorClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();
            idTextBox.Text = ID;
            idTextBox.ReadOnly = true;

            // Check User Role for UI Adjustments
            if (ID.StartsWith("FRM"))
            {
                DonateButton.Visible = false;
                workingProfileButton.Visible = false;
                
            }
            else if (ID.StartsWith("BUY"))
            {
                applyForDonateButton.Visible = false;
                addGoodsButton.Visible = false;
                updateAndDeleteButton.Visible = false;
                workingProfileButton.Visible = false;
                updateEquipmentInformaationButton.Visible = false;
                
            }
            else if (ID.StartsWith("WOR"))
            {
                applyForDonateButton.Visible = false;
                addGoodsButton.Visible = false;
                updateAndDeleteButton.Visible = false;
                hireWorkerButton.Visible = false;
                updateEquipmentInformaationButton.Visible = false;
                DonateButton.Visible = false;
                
            }

            DonateButton.Enabled = false;
        }

        

        private void idTextBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void donateDoneButton_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int randomFiveDigitNumber = random.Next(1000, 100000);

            // Retrieve donor details from the text boxes
            string donationID = "D-" + randomFiveDigitNumber;
            string donorName = nameTextBox.Text.Trim();
            string contactInfo = phoneTextBox.Text.Trim();
            decimal donationAmount;
            string customerID = idTextBox.Text.Trim(); // Assuming the ID is already set in the text box

            // Validate input
            if (string.IsNullOrEmpty(donationID) || string.IsNullOrEmpty(donorName) || string.IsNullOrEmpty(contactInfo) ||
                string.IsNullOrEmpty(customerID) || !decimal.TryParse(donateAmountTextBox.Text, out donationAmount))
            {
                MessageBox.Show("Please fill in all the fields with valid data.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                
                // SQL query to insert donor details into the Donor table
                string donorQuery = "INSERT INTO Donor (DonationID, DonorName, DonationAmount, ContractInfo) VALUES (@DonationID, @DonorName, @DonationAmount, @ContractInfo)";

                // SQL query to insert data into the Can table
                string canQuery = "INSERT INTO Can (CustomerID, DonationID) VALUES (@CustomerID, @DonationID)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Open the connection
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insert into Donor table
                            using (SqlCommand donorCommand = new SqlCommand(donorQuery, connection, transaction))
                            {
                                donorCommand.Parameters.AddWithValue("@DonationID", donationID);
                                donorCommand.Parameters.AddWithValue("@DonorName", donorName);
                                donorCommand.Parameters.AddWithValue("@DonationAmount", donationAmount);
                                donorCommand.Parameters.AddWithValue("@ContractInfo", contactInfo);

                                donorCommand.ExecuteNonQuery();
                            }

                            // Insert into Can table
                            using (SqlCommand canCommand = new SqlCommand(canQuery, connection, transaction))
                            {
                                canCommand.Parameters.AddWithValue("@CustomerID", customerID);
                                canCommand.Parameters.AddWithValue("@DonationID", donationID);

                                canCommand.ExecuteNonQuery();
                            }

                            // Commit the transaction
                            transaction.Commit();

                            // Show success message
                            MessageBox.Show("Donation details and association saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Clear input fields
                            nameTextBox.Clear();
                            donateAmountTextBox.Clear();
                            phoneTextBox.Clear();
                            //idTextBox.Clear();
                        }
                        catch
                        {
                            // Rollback transaction in case of an error
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                MessageBox.Show("An error occurred while saving donation details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void profileButton_Click_1(object sender, EventArgs e)
        {
            ProfileInfoClass profileInfoClass = new ProfileInfoClass(ID);
            profileInfoClass.Show();
            this.Close();
        }

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

        private void DonateButton_Click(object sender, EventArgs e)
        {
            DonorClass donorClass = new DonorClass(ID);
            donorClass.Show();
            this.Close();
        }

        private void logoutButtton_Click(object sender, EventArgs e)
        {
            LoginClass loginClass = new LoginClass();
            loginClass.Show();
            this.Close();
        }

        private void currentOrderButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID);
            ordersCartClass.Show();
            this.Close();
        }

        private void addToCart_Click(object sender, EventArgs e)
        {
            AddCartClass addCartClass = new AddCartClass(ID);
            addCartClass.Show();
            this.Close();
        }

        private void panelForDisplayAllLavel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
