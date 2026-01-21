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
using System;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;


namespace AgroBridge
{
    public partial class RegisterClass : Form
    {
        public RegisterClass()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(1080, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.CenterToScreen();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(1080, 640);
            this.BackColor = System.Drawing.Color.FromArgb(237, 247, 250);
            panel1.BackColor = System.Drawing.Color.FromArgb(43, 54, 38);
            panel2.BackColor = System.Drawing.Color.FromArgb(200, 50, 50);
        }

        private void panel2_Paint(object sender, PaintEventArgs e) { }

        private void panel1_Paint(object sender, PaintEventArgs e) { }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        // Submit Button
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrEmpty(NameTextBox.Text) || string.IsNullOrEmpty(emailTextBox.Text) ||
                string.IsNullOrEmpty(PhoneNumberTextBox.Text) || string.IsNullOrEmpty(userTypeComboBox.Text) ||
                string.IsNullOrEmpty(PasswordTextBox.Text) || string.IsNullOrEmpty(ConfirmPasswordTextBox.Text))
            {
                MessageBox.Show("Please fill all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (PasswordTextBox.Text != ConfirmPasswordTextBox.Text)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate email format
            if (!IsValidEmail(emailTextBox.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate phone number format
            if (!IsValidPhoneNumber(PhoneNumberTextBox.Text))
            {
                MessageBox.Show("Please enter a valid phone number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate password length
            if (PasswordTextBox.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //update
            Random random = new Random();
            int randomFiveDigitNumber = random.Next(10000, 100000);

            string id;
            string name = NameTextBox.Text;
            string email = emailTextBox.Text;
            string phoneNumber = PhoneNumberTextBox.Text.ToString();
            string userType = userTypeComboBox.Text;
            string password = PasswordTextBox.Text;
            string address = addressTextBox.Text;
            string dob = dateTimePicker1.Value.ToString("yyyy-MM-dd");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Insert logic as per userType (Farmer, Worker, Buyer)
                    if (userType == "Farmer")
                    {
                        id = "FRM-" + randomFiveDigitNumber;

                        // Farmer-specific insert logic
                        string sql1 = "INSERT INTO Farmer(FarmerID, FarmerName, ContactInfo, Address, Email) " +
                                      "VALUES(@ID, @Name, @Phone, @Address, @Email)";
                        using (SqlCommand cmd = new SqlCommand(sql1, con))
                        {
                            cmd.Parameters.AddWithValue("@ID", id);
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                            cmd.Parameters.AddWithValue("@Address", address);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.ExecuteNonQuery();
                        }

                        // Registration insert query (common for Farmer, Worker, Buyer)
                        string sql2 = "INSERT INTO Registration(RegistrationID, UserType, UserName, DateofBirth, ContractInfo, Email, Address, Password) " +
                                      "VALUES(@ID, @UserType, @Name, @DOB, @Phone, @Email, @Address, @Password)";
                        using (SqlCommand cmd = new SqlCommand(sql2, con))
                        {
                            cmd.Parameters.AddWithValue("@ID", id);
                            cmd.Parameters.AddWithValue("@UserType", userType);
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@DOB", dob);
                            cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Address", address);
                            cmd.Parameters.AddWithValue("@Password", password);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (userType == "Worker")
                    {
                        id = "WOR-" + randomFiveDigitNumber;
                        InsertUser(con, id, name, phoneNumber, address, email, password, dob, userType);
                    }
                    else // Buyer
                    {
                        id = "BUY-" + randomFiveDigitNumber;
                        InsertUser(con, id, name, phoneNumber, address, email, password, dob, userType);
                    }

                    MessageBox.Show("Registration successful!\nPlease check you email for your UserId", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //SendWhatsAppMessage(phoneNumber, id, password);
                    SendEmail(email, id, name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LoginClass l1 = new LoginClass();
            l1.Show();
            this.Hide();
        }

        // Back to previous page[login]
        private void button1_Click(object sender, EventArgs e)
        {
            LoginClass login = new LoginClass();
            login.Show();
            this.Hide();
        }

        // WindowCloseButton
        private void button1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel3_Paint(object sender, PaintEventArgs e) { }

        private void emailTextBox_TextChanged(object sender, EventArgs e) { }

        private void PhoneNumberTextBox_TextChanged(object sender, EventArgs e) { }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                // Show password
                PasswordTextBox.PasswordChar = '\0';
                ConfirmPasswordTextBox.PasswordChar = '\0';
            }
            else
            {
                // Hide password
                PasswordTextBox.PasswordChar = '*';
                ConfirmPasswordTextBox.PasswordChar = '*';
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) { }

        // Email validation method
        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Phone number validation method (checks if it's all digits and has 10 characters)
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return phoneNumber.All(char.IsDigit) && phoneNumber.Length == 11;
        }

        // User insert method for common insert queries (Worker and Buyer)
        private void InsertUser(SqlConnection con, string id, string name, string phoneNumber, string address, string email, string password, string dob, string userType)
        {
            string sql1 = "INSERT INTO Customer(CustomerID, CustomerName, Address, ContractInfo, Email) " +
                          "VALUES(@ID, @Name, @Address, @Phone, @Email)";
            using (SqlCommand cmd = new SqlCommand(sql1, con))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.ExecuteNonQuery();
            }

            // Registration insert query (common for Worker and Buyer)
            string sql2 = "INSERT INTO Registration(RegistrationID, UserType, UserName, DateofBirth, ContractInfo, Email, Address, Password) " +
                          "VALUES(@ID, @UserType, @Name, @DOB, @Phone, @Email, @Address, @Password)";
            using (SqlCommand cmd = new SqlCommand(sql2, con))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@UserType", userType);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@DOB", dob);
                cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.ExecuteNonQuery();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        

        private void SendWhatsAppMessage(string phoneNumber, string userId, string password)
        {
            string message = $"Dear User, Your registration was successful! User ID: {userId} Password: {password} - AgroBridge Team";
            string url = $"https://api.whatsapp.com/send?phone={phoneNumber}&text={Uri.EscapeDataString(message)}";

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void SendEmail(string email, string userId, string name)
        {
            try
            {
                string fromEmail = "agrobridgeteam@gmail.com";
                string fromPassword = "wuncnrkwzjrnaihe"; 

                string subject = "🔔 AgroBridge - Registration Successful";
                string body = $@"
        <div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; text-align: center;'>
            <div style='max-width: 500px; margin: auto; background: white; padding: 20px; border-radius: 10px; box-shadow: 0px 4px 10px rgba(0,0,0,0.1);'>
                <h2 style='color: #28a745;'>🎉 Registration Successful!</h2>
                <p style='color: #333; font-size: 16px;'>Dear <b>{name}</b>,</p>
                <p style='color: #555; font-size: 14px;'>Welcome to <b>AgroBridge</b>! Your registration is successful.</p>
                <div style='background: #28a745; color: white; font-size: 20px; font-weight: bold; padding: 10px; margin: 20px auto; width: 250px; border-radius: 5px;'>
                    User ID: {userId}
                </div>
                <p style='color: #777; font-size: 12px;'>If you did not register, please ignore this email.</p>
                <hr style='border: 0; height: 1px; background: #ddd;'>
                <p style='color: #888; font-size: 12px;'>Best Regards, <br><b>AgroBridge Team</b></p>
            </div>
        </div>";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
                smtp.EnableSsl = true;

                smtp.Send(mail);
                //MessageBox.Show("Email sent successfully!", "User ID Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending Email: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }






    }
}
