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
using System.Diagnostics.Eventing.Reader;


namespace AgroBridge
{
    public partial class LoginClass : Form
    {
        public LoginClass()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(1080, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.CenterToScreen();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(1080, 640);

            //background Frame color
            this.BackColor = System.Drawing.Color.FromArgb(237, 247, 250);

            //panelRed
            panel1.BackColor = System.Drawing.Color.FromArgb(200, 50, 50);


            // For Transparent RegisterButton
            RegisterButton.FlatStyle = FlatStyle.Flat;
            RegisterButton.FlatAppearance.BorderSize = 0;
            RegisterButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RegisterButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RegisterButton.BackColor = Color.Transparent;


        }



        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {

        

           RegisterClass register = new RegisterClass();
            register.Show();
            this.Hide();  

        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            String id = usernameTextBox.Text.Trim();
            String password = passwordTextBox.Text.Trim();

            // Check if username or password is empty
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit the method so it doesn't continue with login process
            }

            if (id == "Admin" && password == "admin")
            {
                DashBoardClass adminHomePage = new DashBoardClass(id);
                adminHomePage.Show();
                this.Hide();
            }
            else
            {
                string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        string query = "SELECT COUNT(*) FROM Registration WHERE RegistrationID = @id AND Password = @password";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@password", password);

                            con.Open();
                            int count = (int)cmd.ExecuteScalar();

                            if (count > 0)
                            {
                                HomePage homePage = new HomePage(id);
                                homePage.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



        //WindowCloseButton
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                // Show password
                passwordTextBox.PasswordChar = '\0';
               
            }
            else
            {
                // Hide password
                passwordTextBox.PasswordChar = '*';
               
            }
        }

        private void passwordLabel_Click(object sender, EventArgs e)
        {

        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
