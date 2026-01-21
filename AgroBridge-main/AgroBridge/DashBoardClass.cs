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
    public partial class DashBoardClass: Form
    {

        private string ID;
        string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

        public DashBoardClass(string id)
        {
            InitializeComponent();
            this.ID = id;
            
        }

        private void DashBoardClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();
            loadGoods();
            loadWaste();
            loadDeliveredItems();
            loadPendingItems();
            loadSoldAmount();
            loadUsers();
            loadTotalDonationReq();
            loadDonationReq();
            loadTotalOrders();
            loadPurchaseHistory();
            loadRichTextBoxForText();

            // Check User Role for UI Adjustments
            if (ID.StartsWith("FRM"))
            {
                //all buutton
                DashBoardButton.Visible = true;
                OrdersButton.Visible = true;
                accountDetailsButton.Visible = true;
                goodsProductButton.Visible = true;
                wasteProductButton.Visible = true;
                deliveredItemButtton.Visible = true;
                pendingItemButton.Visible = true;
                soldAmountButton.Visible = false;
                usersInfoButton.Visible = false;

                //all panel
                panelForGoods.Visible = true;
                panelForWaste.Visible = true;
                panelForDeliveredItems.Visible = true;
                panelForPendingItems.Visible = true;
                panelForSoldAmount.Visible = true;
                panelForUsers.Visible = false;
                panelForTotalOrders.Visible = true;
                panelForPurchaseHistory.Visible = true;
                panelForDonateReq.Visible = false;
                panelForFeedbacks.Visible = false;
                panelForContactUs.Visible = true;
                panelForTotalDonateReq.Visible = false;
            }
            else if (ID.StartsWith("BUY"))
            {
                //all buutton
                DashBoardButton.Visible = true;
                OrdersButton.Visible = true;
                accountDetailsButton.Visible = true;
                goodsProductButton.Visible = false;
                wasteProductButton.Visible = false;
                deliveredItemButtton.Visible = false;
                pendingItemButton.Visible = false;
                soldAmountButton.Visible = false;
                usersInfoButton.Visible = false;
                OrderListButton.Visible = false;

                //all panel
                panelForGoods.Visible = false;
                panelForWaste.Visible = false;
                panelForDeliveredItems.Visible = false;
                panelForPendingItems.Visible = false;
                panelForSoldAmount.Visible = false;
                panelForUsers.Visible = false;
                panelForTotalOrders.Visible = true;
                panelForPurchaseHistory.Visible = true;
                panelForDonateReq.Visible = false;
                panelForFeedbacks.Visible = false;
                panelForContactUs.Visible = true;
                panelForTotalDonateReq.Visible = false;


            }
            else if (ID.StartsWith("WOR"))
            {
                //all buutton
                DashBoardButton.Visible = true;
                OrdersButton.Visible = true;
                accountDetailsButton.Visible = true;
                goodsProductButton.Visible = false;
                wasteProductButton.Visible = false;
                deliveredItemButtton.Visible = false;
                pendingItemButton.Visible = false;
                soldAmountButton.Visible = false;
                usersInfoButton.Visible = false;
                OrderListButton.Visible = false;

                //all panel
                panelForGoods.Visible = false;
                panelForWaste.Visible = false;
                panelForDeliveredItems.Visible = false;
                panelForPendingItems.Visible = false;
                panelForSoldAmount.Visible = false;
                panelForUsers.Visible = false;
                panelForTotalOrders.Visible = true;
                panelForPurchaseHistory.Visible = true;
                panelForDonateReq.Visible = false;
                panelForFeedbacks.Visible = false;
                panelForContactUs.Visible = true;
                panelForTotalDonateReq.Visible = false;


            }

            else if (ID.StartsWith("Admin"))
            {
                //all buutton
                DashBoardButton.Visible = true;
                OrdersButton.Visible = false;
                accountDetailsButton.Visible = false;
                goodsProductButton.Visible = true; ;
                wasteProductButton.Visible = true; ;
                deliveredItemButtton.Visible = false;
                pendingItemButton.Visible = false;
                soldAmountButton.Visible = false;
                usersInfoButton.Visible = true;
                contactUsButton.Visible = false;

                //top panel button
                currentOrderButton.Visible = false;
                addToCart.Visible = false;
                profileButton.Visible = false;
                OrderListButton.Visible = false;

                //all panel
                panelForGoods.Visible = true;
                panelForWaste.Visible = true;
                panelForDeliveredItems.Visible = false;
                panelForPendingItems.Visible = false;
                panelForSoldAmount.Visible = false;
                panelForUsers.Visible = true;
                panelForTotalOrders.Visible = false;
                panelForPurchaseHistory.Visible = false;
                panelForDonateReq.Visible = true;
                panelForFeedbacks.Visible = true;
                panelForContactUs.Visible = true;


                //Enable
                panelForTotalDonateReq.Enabled = false;
                panelForFeedbacks.Enabled = false;
                panelForContactUs.Enabled = false;


            }

            DashBoardButton.Enabled = false;
            panelForContactUs.Enabled = false;
            panelForFeedbacks.Enabled = false;



        }
        private void loadRichTextBoxForText()
        {
            richTextBoxForText.Text = $"Hello {ID} (not {ID}? Log out)\r\n\r\n" +
                                      "From your account dashboard, you can view your recent orders, " +
                                      "manage your shipping and billing addresses, and edit your password " +
                                      "and account details.";
        }


        private void loadGoods()
        {
            //flowLayoutPanelForFunctionality.Controls.Clear();

            try
            {
                string query = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (ID.StartsWith("Admin"))
                    {
                        query = "SELECT COUNT(GoodsID) AS GoodsNumber FROM Goods";
                    }
                    else if (ID.StartsWith("FRM"))
                    {
                        query = "SELECT COUNT(GoodsID) AS GoodsNumber FROM Goods WHERE FarmerID = @FarmerID";
                    }
                    else
                    {
                        return; // Exit method if ID does not match expected patterns
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (ID.StartsWith("FRM"))
                        {
                            command.Parameters.AddWithValue("@FarmerID", ID);
                        }

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                GoodsNumberLabel.Text = reader["GoodsNumber"].ToString();
                             
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

        private void loadWaste()
        {
            //flowLayoutPanelForFunctionality.Controls.Clear();

            try
            {
                string query = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (ID.StartsWith("Admin"))
                    {
                        query = "SELECT COUNT(WasteID) AS WasteNumber FROM Waste";
                    }
                    else if (ID.StartsWith("FRM"))
                    {
                        query = "SELECT COUNT(WasteID) AS WasteNumber FROM Waste WHERE FarmerID = @FarmerID";
                    }
                    else
                    {
                        return; // Exit method if ID does not match expected patterns
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (ID.StartsWith("FRM"))
                        {
                            command.Parameters.AddWithValue("@FarmerID", ID);
                        }

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                WasteNumberLabel.Text = reader["WasteNumber"].ToString();

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

        private void loadDeliveredItems()
        {
            // flowLayoutPanelForFunctionality.Controls.Clear();

            try
            {
                string query = @"
        SELECT 
            (SELECT COUNT(*) FROM AgroBridge.dbo.Buy WHERE FarmerID = @FarmerID AND Status IN ('Delivered')) AS NoOfGoods,
            (SELECT COUNT(*) FROM AgroBridge.dbo.Purchase WHERE FarmerID = @FarmerID AND Status IN ('Delivered')) AS NoOfWaste,
            (SELECT COUNT(*) FROM AgroBridge.dbo.Rental WHERE FarmerID = @FarmerID AND Status IN ('Delivered')) AS NoOfEquipment;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Correct parameter assignment
                    command.Parameters.AddWithValue("@FarmerID", ID);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int noOfGoods = reader["NoOfGoods"] != DBNull.Value ? Convert.ToInt32(reader["NoOfGoods"]) : 0;
                            int noOfWaste = reader["NoOfWaste"] != DBNull.Value ? Convert.ToInt32(reader["NoOfWaste"]) : 0;
                            int noOfEquipment = reader["NoOfEquipment"] != DBNull.Value ? Convert.ToInt32(reader["NoOfEquipment"]) : 0;

                            // Summing them correctly
                            int totalDeliveredItems = noOfGoods + noOfWaste + noOfEquipment;

                            // Assign the value to the label
                            deliveredNumberLabel.Text = totalDeliveredItems.ToString();
                            
                        }
                        else
                        {
                            // Handle the case where no data is returned
                            deliveredNumberLabel.Text = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception message
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void loadPendingItems()
        {
            try
            {
                string query = @"
            SELECT 
                (SELECT COUNT(*) FROM AgroBridge.dbo.Buy WHERE FarmerID = @FarmerID AND Status IN ('Pending', 'Dispatched')) AS NoOfGoods,
                (SELECT COUNT(*) FROM AgroBridge.dbo.Purchase WHERE FarmerID = @FarmerID AND Status IN ('Pending', 'Dispatched')) AS NoOfWaste,
                (SELECT COUNT(*) FROM AgroBridge.dbo.Rental WHERE FarmerID = @FarmerID AND Status IN ('Pending', 'Dispatched')) AS NoOfEquipment;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Assigning parameters correctly
                    command.Parameters.AddWithValue("@FarmerID", ID);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int noOfGoods = reader.GetInt32(reader.GetOrdinal("NoOfGoods"));
                            int noOfWaste = reader.GetInt32(reader.GetOrdinal("NoOfWaste"));
                            int noOfEquipment = reader.GetInt32(reader.GetOrdinal("NoOfEquipment"));

                            // Summing them correctly
                            int totalPendingItems = noOfGoods + noOfWaste + noOfEquipment;

                            // Assign the value to the label
                            pendingNumberLabel.Text = totalPendingItems.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void loadSoldAmount()
        {
            try
            {
                string query = @"
            SELECT 
                (SELECT COALESCE(SUM(G.GoodsPrice * BU.Quantity), 0) 
                 FROM AgroBridge.dbo.Buy BU
                 JOIN AgroBridge.dbo.Goods G ON BU.GoodsID = G.GoodsID
                 WHERE BU.FarmerID = @FarmerID AND BU.Status = 'Completed') AS TotalPrice1,

                (SELECT COALESCE(SUM(W.WastePrice * P.Quantity), 0) 
                 FROM AgroBridge.dbo.Purchase P
                 JOIN AgroBridge.dbo.Waste W ON P.WasteID = W.WasteID
                 WHERE P.FarmerID = @FarmerID AND P.Status = 'Completed') AS TotalPrice2,

                (SELECT COALESCE(SUM(R.EquipmentRentalPrice * RE.Quantity), 0)  
                 FROM AgroBridge.dbo.Rental RE
                 JOIN AgroBridge.dbo.Rent R ON RE.EquipmentID = R.EquipmentID
                 WHERE RE.FarmerID = @FarmerID AND RE.Status = 'Completed') AS TotalPrice3;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Correct parameter assignment
                    command.Parameters.AddWithValue("@FarmerID", ID);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int totalGoods = reader["TotalPrice1"] != DBNull.Value ? Convert.ToInt32(reader["TotalPrice1"]) : 0;
                            int totalWaste = reader["TotalPrice2"] != DBNull.Value ? Convert.ToInt32(reader["TotalPrice2"]) : 0;
                            int totalEquipment = reader["TotalPrice3"] != DBNull.Value ? Convert.ToInt32(reader["TotalPrice3"]) : 0;

                            // Summing them correctly
                            int totalSoldAmount = totalGoods + totalWaste + totalEquipment;

                            // Assign the value to the label
                            soldNumberLabel.Text = totalSoldAmount.ToString()+ " ৳";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void loadUsers()
        {
            //flowLayoutPanelForFunctionality.Controls.Clear();

            try
            {
                string query = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                        query = "SELECT COUNT(*) AS NumberOfUsers FROM Registration";
                  
                    

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userNumberLabel.Text = reader["NumberOfUsers"].ToString();

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

        private void loadTotalDonationReq()
        {
            //flowLayoutPanelForFunctionality.Controls.Clear();

            try
            {
                string query = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    query = "SELECT COUNT(*) AS NumberOfReq FROM Applys";



                    using (SqlCommand command = new SqlCommand(query, connection))
                    {


                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalDonateRewNumberLabel.Text = reader["NumberOfReq"].ToString();

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

        private void loadDonationReq()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) AS NumberOfReq FROM Applys WHERE Status = 'Under Review'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            DonateRewNumberLabel.Text = result.ToString();
                        }
                        else
                        {
                            DonateRewNumberLabel.Text = "0"; // Default if no result
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void loadTotalOrders()
        {
            try
            {
                string query = @"
        SELECT 
            COALESCE((SELECT COUNT(*) FROM AgroBridge.dbo.Buy WHERE CustomerID = @CustomerID AND Status NOT IN ('Completed')), 0) AS NoOfGoods, 
            COALESCE((SELECT COUNT(*) FROM AgroBridge.dbo.Purchase WHERE CustomerID = @CustomerID AND Status NOT IN ('Completed')), 0) AS NoOfWaste, 
            COALESCE((SELECT COUNT(*) FROM AgroBridge.dbo.Rental WHERE CustomerID = @CustomerID AND Status NOT IN ('Completed')), 0) AS NoOfEquipment;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Assigning parameters correctly
                    command.Parameters.AddWithValue("@CustomerID", ID);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int noOfGoods = reader["NoOfGoods"] != DBNull.Value ? Convert.ToInt32(reader["NoOfGoods"]) : 0;
                            int noOfWaste = reader["NoOfWaste"] != DBNull.Value ? Convert.ToInt32(reader["NoOfWaste"]) : 0;
                            int noOfEquipment = reader["NoOfEquipment"] != DBNull.Value ? Convert.ToInt32(reader["NoOfEquipment"]) : 0;

                            // Summing them correctly
                            int totalOrders = noOfGoods + noOfWaste + noOfEquipment;

                            // Assign the value to the label
                            totalOrdersNumberLabel.Text = totalOrders.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void loadPurchaseHistory()
        {
            try
            {
                string query = @"
        SELECT 
            (SELECT COUNT(*) 
             FROM AgroBridge.dbo.Buy 
             WHERE CustomerID = @CustomerID AND Status = 'Completed') AS TotalOrders1,

            (SELECT COUNT(*) 
             FROM AgroBridge.dbo.Purchase 
             WHERE CustomerID = @CustomerID AND Status = 'Completed') AS TotalOrders2,

            (SELECT COUNT(*)  
             FROM AgroBridge.dbo.Rental 
             WHERE CustomerID = @CustomerID AND Status = 'Completed') AS TotalOrders3;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Correct parameter assignment
                    command.Parameters.AddWithValue("@CustomerID", ID);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int totalGoodsOrders = reader["TotalOrders1"] != DBNull.Value ? Convert.ToInt32(reader["TotalOrders1"]) : 0;
                            int totalWasteOrders = reader["TotalOrders2"] != DBNull.Value ? Convert.ToInt32(reader["TotalOrders2"]) : 0;
                            int totalEquipmentOrders = reader["TotalOrders3"] != DBNull.Value ? Convert.ToInt32(reader["TotalOrders3"]) : 0;

                            // Summing the total number of completed orders
                            int totalCompletedOrders = totalGoodsOrders + totalWasteOrders + totalEquipmentOrders;

                            // Assign the value to the label
                            purchaseHistoryNumberLabel.Text = totalCompletedOrders.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void OrderListButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID, "Seller");
            ordersCartClass.Show();
            this.Close();
        }

        private void DonateReqViewDetailsButton_Click(object sender, EventArgs e)
        {
            DonaationRequest donaationRequestClass = new DonaationRequest(ID);
            donaationRequestClass.Show();
            this.Close();

        }



        private void goodsViewDetailsButton_Click(object sender, EventArgs e)
        {
            if (ID.StartsWith("Admin"))
            {
                HomePage homePage = new HomePage(ID);
                homePage.Show();
                this.Close();

            }

            else if (ID.StartsWith("FRM"))
            {
                UpdateAndDeleteClass updateAndDeleteClass = new UpdateAndDeleteClass(ID);
                updateAndDeleteClass.Show();
                this.Close();
            }
        }

        private void userViewDetailsButton_Click(object sender, EventArgs e)
        {
            RegInfoClass regInfoClass = new RegInfoClass(ID);
            regInfoClass.Show();
            this.Close();
        }

        private void wasteViewDetailsButton_Click(object sender, EventArgs e)
        {
            if (ID.StartsWith("Admin"))
            {
                AllWasteItem allWasteItem = new AllWasteItem(ID);
                allWasteItem.Show();
                this.Close();

            }

            else if (ID.StartsWith("FRM"))
            {
                UpdateAndDeleteClass updateAndDeleteClass = new UpdateAndDeleteClass(ID);
                updateAndDeleteClass.Show();
                this.Close();
            }
        }

        private void deliveredViewDetailsButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID, "Seller", "Delivered");
            ordersCartClass.Show();
            this.Close();
        }

        private void pendingViewDetailsButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID, "Seller", "Pending");
            ordersCartClass.Show();
            this.Close();
        }

        private void soldGoodsViewDetailsButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID, "Sellers", "Completed");
            ordersCartClass.Show();
            this.Close();
        }

        private void purchaseHistoryViewDetailsButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID, "Buyers", "Completed");
            ordersCartClass.Show();
            this.Close();
        }

        private void totalOrdersViewDetailsButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID);
            ordersCartClass.Show();
            this.Close();
        }

        private void OrdersButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID);
            ordersCartClass.Show();
            this.Close();
        }

        private void accountDetailsButton_Click(object sender, EventArgs e)
        {
            ProfileInfoClass profileInfoClass = new ProfileInfoClass(ID);
            profileInfoClass.Show();
            this.Close();
        }

        private void goodsProductButton_Click(object sender, EventArgs e)
        {
            if (ID.StartsWith("Admin"))
            {
                HomePage homePage = new HomePage(ID);
                homePage.Show();
                this.Close();

            }

            else if (ID.StartsWith("FRM"))
            {
                UpdateAndDeleteClass updateAndDeleteClass = new UpdateAndDeleteClass(ID);
                updateAndDeleteClass.Show();
                this.Close();
            }
        }

        private void wasteProductButton_Click(object sender, EventArgs e)
        {
            if (ID.StartsWith("Admin"))
            {
                AllWasteItem allWasteItem = new AllWasteItem(ID);
                allWasteItem.Show();
                this.Close();

            }

            else if (ID.StartsWith("FRM"))
            {
                UpdateAndDeleteClass updateAndDeleteClass = new UpdateAndDeleteClass(ID);
                updateAndDeleteClass.Show();
                this.Close();
            }
        }

        private void deliveredItemButtton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID, "Seller", "Delivered");
            ordersCartClass.Show();
            this.Close();
        }

        private void pendingItemButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID, "Seller", "Pending");
            ordersCartClass.Show();
            this.Close();
        }

        private void soldAmountButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID, "Sellers", "Completed");
            ordersCartClass.Show();
            this.Close();
        }

        private void usersInfoButton_Click(object sender, EventArgs e)
        {
            RegInfoClass regInfoClass = new RegInfoClass(ID);
            regInfoClass.Show();
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

        private void profileButton_Click(object sender, EventArgs e)
        {
            ProfileInfoClass profileInfoClass = new ProfileInfoClass(ID);
            profileInfoClass.Show();
            this.Close();
        }

        private void richTextBoxForText_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
