using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Net;
using System.Xml.Linq;

namespace AgroBridge
{
    public partial class PaymentClass: Form
    {

        public string ID;
        private string method;
        private string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

        public PaymentClass(string id,String method)
        {
            InitializeComponent();
            this.ID = id;
            this.method = method;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.CenterToScreen();
            this.Size = new System.Drawing.Size(534, 463);

            // Prevent resizing
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false; // Disable maximize button
        }

        private void BkashClass_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            if (method.Equals("Bkash"))
            {
                NagadPictureBox.Visible=false;
                RocketPictureBox.Visible = false;
            }
            else if (method.Equals("Nagad"))
            {
                pictureBoxForBkash.Visible = false;
                RocketPictureBox.Visible = false;
                this.BackColor = Color.FromArgb(227, 152, 60);
            }

            else if (method.Equals("Rocket"))
            {
                pictureBoxForBkash.Visible = false;
                NagadPictureBox.Visible = false;
                this.BackColor = Color.FromArgb(122, 49, 138);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PaymentButton_Click(object sender, EventArgs e)
        {
            string number = NumberTextBox.Text.Trim();
            string pin = PinTextBox.Text.Trim();

            try
            {
                // Input validation
                if (string.IsNullOrEmpty(number) || !number.All(char.IsDigit) || number.Length != 11)
                {
                    MessageBox.Show("Invalid mobile number! It must be 11 digits and numeric.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(pin) || !pin.All(char.IsDigit))
                {
                    MessageBox.Show("Invalid PIN! PIN must be numeric.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (method.Equals("Bkash") && pin.Length != 5)
                {
                    MessageBox.Show("Bkash requires a 5-digit PIN.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if ((method.Equals("Nagad") || method.Equals("Rocket")) && pin.Length != 4)
                {
                    MessageBox.Show($"{method} requires a 4-digit PIN.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Payment Success Message
                MessageBox.Show($"{method} payment successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string customerID = "";
                string farmerID = "";
                string query = "";

                if (ID.StartsWith("FRM"))
                {
                    farmerID = ID;
                    query = @"
                            SELECT AC.GoodsID, AC.WasteID, AC.EquipmentID, AC.ProductQuantity, 
                                   G.FarmerID AS GoodsFarmerID, 
                                   W.FarmerID AS WasteFarmerID, 
                                   R.FarmerID AS RentFarmerID  
                            FROM AgroBridge.dbo.AddCart AC
                            LEFT JOIN AgroBridge.dbo.Goods G ON AC.GoodsID = G.GoodsID
                            LEFT JOIN AgroBridge.dbo.Waste W ON AC.WasteID = W.WasteID
                            LEFT JOIN AgroBridge.dbo.Rent R ON AC.EquipmentID = R.EquipmentID
                            WHERE AC.FarmerID = @FarmerID;";
                }
                else if (ID.StartsWith("BUY") || ID.StartsWith("WOR"))
                {
                    customerID = ID;
                    query = @"
                            SELECT AC.GoodsID, AC.WasteID, AC.EquipmentID, AC.ProductQuantity, 
                                   G.FarmerID AS GoodsFarmerID, 
                                   W.FarmerID AS WasteFarmerID, 
                                   R.FarmerID AS RentFarmerID  
                            FROM AgroBridge.dbo.AddCart AC
                            LEFT JOIN AgroBridge.dbo.Goods G ON AC.GoodsID = G.GoodsID
                            LEFT JOIN AgroBridge.dbo.Waste W ON AC.WasteID = W.WasteID
                            LEFT JOIN AgroBridge.dbo.Rent R ON AC.EquipmentID = R.EquipmentID
                            WHERE AC.CustomerID = @CustomerID;";
                }


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(farmerID))
                            command.Parameters.AddWithValue("@FarmerID", farmerID);
                        else
                            command.Parameters.AddWithValue("@CustomerID", customerID);

                        // Read data first and store it
                        List<Dictionary<string, object>> cartItems = new List<Dictionary<string, object>>();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Determine the first non-null FarmerID
                                object farmerIDValue = reader["GoodsFarmerID"] != DBNull.Value ? reader["GoodsFarmerID"] :
                                                       reader["WasteFarmerID"] != DBNull.Value ? reader["WasteFarmerID"] :
                                                       reader["RentFarmerID"] != DBNull.Value ? reader["RentFarmerID"] : null;

                                var item = new Dictionary<string, object>
                                {
                                    { "GoodsID", reader["GoodsID"] != DBNull.Value ? reader["GoodsID"] : null },
                                    { "WasteID", reader["WasteID"] != DBNull.Value ? reader["WasteID"] : null },
                                    { "EquipmentID", reader["EquipmentID"] != DBNull.Value ? reader["EquipmentID"] : null },
                                    { "FarmerID", farmerIDValue }, // Store only the first non-null FarmerID
                                    { "ProductQuantity", reader["ProductQuantity"] }
                                };

                                cartItems.Add(item);
                            }
                        }
                        // Reader is now closed

                        // Now execute the INSERT queries
                        foreach (var item in cartItems)
                        {
                            string status = "Pending";
                            DateTime dateTime = DateTime.UtcNow;

                            if (item["GoodsID"] != null)
                            {
                                string sql = "INSERT INTO Buy (CustomerID, GoodsID, BuyOrderID, FarmerID, Status, Quantity, PaymentMethod, DateTime) " +
                                             "VALUES (@CustomerID, @GoodsID, @BuyOrderID, @FarmerID, @Status, @Quantity, @PaymentMethod, @DateTime)";

                                using (SqlCommand cmd = new SqlCommand(sql, connection))
                                {
                                    cmd.Parameters.AddWithValue("@CustomerID", ID);
                                    cmd.Parameters.AddWithValue("@GoodsID", item["GoodsID"]);
                                    cmd.Parameters.AddWithValue("@BuyOrderID", GenerateOrderId());
                                    cmd.Parameters.AddWithValue("@FarmerID", item["FarmerID"]);
                                    cmd.Parameters.AddWithValue("@Status", status);
                                    cmd.Parameters.AddWithValue("@Quantity", item["ProductQuantity"]);
                                    cmd.Parameters.AddWithValue("@PaymentMethod", method);
                                    cmd.Parameters.AddWithValue("@DateTime", dateTime);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            if (item["WasteID"] != null)
                            {
                                string sql = "INSERT INTO Purchase (CustomerID, WasteID, PurchaseOrderID, FarmerID, Status, Quantity, PaymentMethod, DateTime) " +
                                             "VALUES (@CustomerID, @WasteID, @PurchaseOrderID, @FarmerID, @Status, @Quantity, @PaymentMethod, @DateTime)";

                                using (SqlCommand cmd = new SqlCommand(sql, connection))
                                {
                                    cmd.Parameters.AddWithValue("@CustomerID", ID);
                                    cmd.Parameters.AddWithValue("@WasteID", item["WasteID"]);
                                    cmd.Parameters.AddWithValue("@PurchaseOrderID", GenerateOrderId());
                                    cmd.Parameters.AddWithValue("@FarmerID", item["FarmerID"]);
                                    cmd.Parameters.AddWithValue("@Status", status);
                                    cmd.Parameters.AddWithValue("@Quantity", item["ProductQuantity"]);
                                    cmd.Parameters.AddWithValue("@PaymentMethod", method);
                                    cmd.Parameters.AddWithValue("@DateTime", dateTime);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            if (item["EquipmentID"] != null)
                            {
                                string sql = "INSERT INTO Rental (CustomerID, EquipmentID, EquipmentOrderID, FarmerID, Status, Quantity, PaymentMethod, DateTime) " +
                                             "VALUES (@CustomerID, @EquipmentID, @EquipmentOrderID, @FarmerID, @Status, @Quantity, @PaymentMethod, @DateTime)";

                                using (SqlCommand cmd = new SqlCommand(sql, connection))
                                {
                                    cmd.Parameters.AddWithValue("@CustomerID", ID);
                                    cmd.Parameters.AddWithValue("@EquipmentID", item["EquipmentID"]);
                                    cmd.Parameters.AddWithValue("@EquipmentOrderID", GenerateOrderId());
                                    cmd.Parameters.AddWithValue("@FarmerID", item["FarmerID"]);
                                    cmd.Parameters.AddWithValue("@Status", status);
                                    cmd.Parameters.AddWithValue("@Quantity", item["ProductQuantity"]);
                                    cmd.Parameters.AddWithValue("@PaymentMethod", method);
                                    cmd.Parameters.AddWithValue("@DateTime", dateTime);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    DeleteFormCart();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        private void DeleteFormCart()
        {
            try
            {
                string customerID = "";
                string farmerID = "";
                string query = "";

                if (ID.StartsWith("FRM"))
                {
                    farmerID = ID;
                    query = "DELETE FROM AddCart WHERE FarmerID = @FarmerID";
                }
                else if (ID.StartsWith("BUY") || ID.StartsWith("WOR"))
                {
                    customerID = ID;
                    query = "DELETE FROM AddCart WHERE CustomerID = @CustomerID";
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (ID.StartsWith("FRM"))
                        {
                            cmd.Parameters.AddWithValue("@FarmerID", farmerID);
                        }
                        else if (ID.StartsWith("BUY") || ID.StartsWith("WOR"))
                        {
                            cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            //MessageBox.Show("Product removed from cart!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No products found to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Function to generate a unique order ID
        private string GenerateOrderId()
        {
            // Get the current UTC time in a compact format
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // millisecond precision

            // Generate a new GUID and take only part of it (first 5 characters) for uniqueness
            string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8); // Use first 8 characters of GUID

            return $"AGRO-{timestamp}-{guidPart}";
        }

        private void pictureBoxForBkash_Click(object sender, EventArgs e)
        {

        }
    }
}
