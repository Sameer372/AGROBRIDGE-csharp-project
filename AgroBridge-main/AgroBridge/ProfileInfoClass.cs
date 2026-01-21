using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Net;
using System.Windows.Forms.VisualStyles;

namespace AgroBridge
{
	public partial class ProfileInfoClass : Form
	{

		private String ID;
        string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; // Update
        public ProfileInfoClass()
		{
			InitializeComponent();
			this.StartPosition = FormStartPosition.WindowsDefaultBounds;
		}

		public ProfileInfoClass(string ID)
		{
			InitializeComponent();
			this.StartPosition = FormStartPosition.WindowsDefaultBounds;
			this.ID = ID;

			


		}

        private void ProfileInfoClass_Load(object sender, EventArgs e)
        {

            this.Size = new Size(1080, 620);
            this.CenterToScreen();
            profileButton.Enabled = false;
            // Check User Role for UI Adjustments

            if (ID.StartsWith("FRM"))
            {
                DonateButton.Visible = false;
                workingProfileButton.Visible = false;
                OrderListButton.Visible = true;
            }
            else if (ID.StartsWith("BUY"))
            {
                applyForDonateButton.Visible = false;
                addGoodsButton.Visible = false;
                updateAndDeleteButton.Visible = false;
                workingProfileButton.Visible = false;
                updateEquipmentInformaationButton.Visible = false;
                OrderListButton.Visible = false;
            }
            else if (ID.StartsWith("WOR"))
            {
                applyForDonateButton.Visible = false;
                addGoodsButton.Visible = false;
                updateAndDeleteButton.Visible = false;
                hireWorkerButton.Visible = false;
                updateEquipmentInformaationButton.Visible = false;
                DonateButton.Visible = false;
                OrderListButton.Visible = false;
            }




            // Create a connection object
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                // Open the connection
                con.Open();

                // Query to get data for the specified ID
                string query = "SELECT UserName, ContractInfo, Email FROM Registration WHERE RegistrationID = @ID";

                // Create a command object
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", ID);

                // Execute the query
                SqlDataReader reader = cmd.ExecuteReader();

                // Check if a record is found
                if (reader.Read())
                {
                    // Populate the text fields with the data
                    UserIDTextBox.Text = ID;
                    NameTextBox.Text = reader["UserName"].ToString();
                    PhoneNumberTextBox.Text = reader["ContractInfo"].ToString();
                    EmailTextBox.Text = reader["Email"].ToString();
                }
                else
                {
                    MessageBox.Show("No record found for the specified ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
            finally
            {
                // Close the connection
                con.Close();
            }
        }

        private void CrossButton_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		//Phone Number TextBox
		private void textBox3_TextChanged(object sender, EventArgs e)
		{

		}

		private void UserIDTextBox_TextChanged(object sender, EventArgs e)
		{

		}

		private void NameTextBox_TextChanged(object sender, EventArgs e)
		{

		}

		private void EmailTextBox_TextChanged(object sender, EventArgs e)
		{

		}

        private void Save_Click(object sender, EventArgs e)
        {
            // Set up your connection string
            string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

            // Create a connection object
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                // Open the connection
                con.Open();

                // Query to update the user information while keeping the ID fixed
                string query = "UPDATE Registration SET UserName = @Name, ContractInfo = @Phone, Email = @Email WHERE RegistrationID = @ID";

                // Create a command object
                SqlCommand cmd = new SqlCommand(query, con);

                // Add parameters to the query
                cmd.Parameters.AddWithValue("@ID", ID); // ID is fixed, retrieved during initialization
                cmd.Parameters.AddWithValue("@Name", NameTextBox.Text);
                cmd.Parameters.AddWithValue("@Phone", PhoneNumberTextBox.Text);
                cmd.Parameters.AddWithValue("@Email", EmailTextBox.Text);

                // Execute the query
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Your information has been successfully updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No changes were made. Please check your inputs.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                MessageBox.Show("Error updating data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close the connection
                con.Close();
            }
        }


        private void UpperPannel_Paint(object sender, PaintEventArgs e)
		{

		}


        //All Button Functionality
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

        private void workingProfileButton_Click(object sender, EventArgs e)
        {
            WorkerWorkingInformationClass workerWorkingInformationClass = new WorkerWorkingInformationClass(ID);
            workerWorkingInformationClass.Show();
            this.Close();
        }

        private void applyForDonateButton_Click(object sender, EventArgs e)
        {
            ApplyForDonateClass applyForDonateClass = new ApplyForDonateClass(ID);
            applyForDonateClass.Show();
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

        private void OrderListButton_Click(object sender, EventArgs e)
        {
            if (ID.StartsWith("FRM"))
            {

                OrdersCartClass ordersCartClass = new OrdersCartClass(ID, "Seller");
                ordersCartClass.Show();
                this.Close();

            }
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

        private void DashboardButton_Click(object sender, EventArgs e)
        {
            DashBoardClass dashBoard = new DashBoardClass(ID);
            dashBoard.Show();
            this.Close();
        }
    }
}
