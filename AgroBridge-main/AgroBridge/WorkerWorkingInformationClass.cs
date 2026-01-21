using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace AgroBridge
{
    public partial class WorkerWorkingInformationClass : Form
    {
        private string ID;
        // Connection string (adjust as necessary)
        string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

        public WorkerWorkingInformationClass(string ID)
        {
            InitializeComponent();
            this.ID = ID;

            

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to fetch worker information
                    string query = "SELECT * FROM WorkerInformations WHERE WorkerID = @WorkerID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@WorkerID", ID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // If a record exists
                        {
                            // Populate the controls with data from the database
                            nameTextBox.Text = reader["WorkerName"].ToString();
                            phoneTextBox.Text = reader["Phone"].ToString();
                            statusComboBox1.Text = reader["Status"].ToString();

                            string skills = reader["Skills"].ToString();

                            // Set the corresponding radio button for skills
                            if (skills == radioButton1.Text)
                                radioButton1.Checked = true;
                            else if (skills == radioButton2.Text)
                                radioButton2.Checked = true;
                            else if (skills == radioButton3.Text)
                                radioButton3.Checked = true;
                            else if (skills == radioButton4.Text)
                                radioButton4.Checked = true;
                        }
                        else
                        {
                            // If no record is found, clear the controls
                            MessageBox.Show("No record found for the given ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ClearWorkerDetails();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the database operation
                MessageBox.Show("An error occurred while fetching data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void donateDoneButton_Click(object sender, EventArgs e)
        {
            string id = idTextBox.Text;
            string name = nameTextBox.Text;
            string phone = phoneTextBox.Text;
            string status = statusComboBox1.Text;
            string skills = "";

            // Determine which skill is selected
            if (radioButton1.Checked)
                skills = radioButton1.Text;
            else if (radioButton2.Checked)
                skills = radioButton2.Text;
            else if (radioButton3.Checked)
                skills = radioButton3.Text;
            else if (radioButton4.Checked)
                skills = radioButton4.Text;

            

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if the ID already exists
                string checkQuery = "SELECT COUNT(*) FROM WorkerInformations WHERE WorkerID = @WorkerID";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@WorkerID", id);

                int count = (int)checkCommand.ExecuteScalar();

                if (count > 0) // ID exists, update the record
                {
                    string updateQuery = "UPDATE WorkerInformations SET WorkerName = @WorkerName, Phone = @Phone, Status = @Status, Skills = @Skills WHERE WorkerID = @WorkerID";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

                    updateCommand.Parameters.AddWithValue("@WorkerID", id);
                    updateCommand.Parameters.AddWithValue("@WorkerName", name);
                    updateCommand.Parameters.AddWithValue("@Phone", phone);
                    updateCommand.Parameters.AddWithValue("@Status", status);
                    updateCommand.Parameters.AddWithValue("@Skills", skills);

                    updateCommand.ExecuteNonQuery();
                    MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else // ID does not exist, insert new record
                {
                    string insertQuery = "INSERT INTO WorkerInformations (WorkerID, WorkerName, Phone, Status, Skills) VALUES (@WorkerID, @WorkerName, @Phone, @Status, @Skills)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

                    insertCommand.Parameters.AddWithValue("@WorkerID", id);
                    insertCommand.Parameters.AddWithValue("@WorkerName", name);
                    insertCommand.Parameters.AddWithValue("@Phone", phone);
                    insertCommand.Parameters.AddWithValue("@Status", status);
                    insertCommand.Parameters.AddWithValue("@Skills", skills);

                    insertCommand.ExecuteNonQuery();
                    MessageBox.Show("Record inserted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void WorkerWorkingInformationClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();
            idTextBox.Text = ID;

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

            workingProfileButton.Enabled = false;
        }

 

        private void ClearWorkerDetails()
        {
            nameTextBox.Text = string.Empty;
            phoneTextBox.Text = string.Empty;
            statusComboBox1.SelectedIndex = -1; // Clear selection in ComboBox
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
        }


        private void statusComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Add logic here if needed for status selection changes
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

        private void profileButton_Click(object sender, EventArgs e)
        {
            ProfileInfoClass profileInfoClass = new ProfileInfoClass(ID);
            profileInfoClass.Show();
            this.Close();
        }

        private void logoutButtton_Click(object sender, EventArgs e)
        {
            LoginClass loginClass = new LoginClass();
            loginClass.Show();
            this.Close();
        }
    }
}
