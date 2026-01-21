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
    public partial class RegInfoClass : Form
    {
        private String ID;
        public RegInfoClass(String ID)
        {
            InitializeComponent();
            this.ID = ID;
       
        }

        
        private void RegInfoClass_Load(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(1080, 640);
            this.CenterToScreen();

            // Set up your connection string
            string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

            // Create a connection object
            SqlConnection con = new SqlConnection(connectionString);

            // SQL query to get all data
            string query = "SELECT * FROM Registration";  

            // Create a SqlDataAdapter to fetch data
            SqlDataAdapter da = new SqlDataAdapter(query, con);

            // Create a DataTable to hold the data
            DataTable dt = new DataTable();

            try
            {
                
                con.Open();

                // Fill the DataTable with the data from the database
                da.Fill(dt);

                // Bind the DataTable to the DataGridView
                dataGridView1.DataSource = dt;

                // Increase Row Height
                dataGridView1.RowTemplate.Height = 40; 

                // Increase Column Width
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.Width = 150; 
                }


                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            }
            catch (Exception ex)
            {
               
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
            finally
            {
                
                con.Close();
            }
        }


        //WindowCloseButton
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            RegisterClass register = new RegisterClass();
            register.Show();
            this.Hide();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // Ensure that a row is selected
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row's data (RegistrationID is the primary key)
                int selectedIndex = dataGridView1.SelectedRows[0].Index;
                string registrationID = dataGridView1.Rows[selectedIndex].Cells["RegistrationID"].Value.ToString();

                // Confirm the deletion with the user
                DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Your connection string
                    string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

                    // Create the connection object
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        try
                        {
                            // SQL query to delete the record
                            string deleteQuery = "DELETE FROM Registration WHERE RegistrationID = @RegistrationID";

                            using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                            {
                                // Add parameter to prevent SQL injection
                                cmd.Parameters.AddWithValue("@RegistrationID", registrationID);

                                // Open the connection
                                con.Open();

                                // Execute the delete command
                                cmd.ExecuteNonQuery();

                                // Provide feedback to the user
                                MessageBox.Show("Record deleted successfully!");

                                
                                RegInfoClass reg = new RegInfoClass(ID);
                                reg.Show();
                                this.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            
                            MessageBox.Show("Error deleting data: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a record to delete.");
            }
        }




        private void UpperPannel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void UpperTextPannel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            LoginClass loginClass = new LoginClass();
            loginClass.Show();
            this.Close();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            DashBoardClass dashBoard = new DashBoardClass(ID);
            dashBoard.Show();
            this.Close();
        }
    }
}
