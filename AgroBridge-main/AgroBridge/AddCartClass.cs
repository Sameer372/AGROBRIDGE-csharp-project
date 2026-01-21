using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgroBridge
{
    public partial class AddCartClass: Form
    {
        private String ID;
        private string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;";  //UPDATE
        private double totalPrice;
        private bool searchBoxClicked = false; // Track if search box was clicked
        public AddCartClass(string id)
        {
            InitializeComponent();
            this.ID = id;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.CenterToScreen();
            this.Size = new System.Drawing.Size(1080, 640);

            // Prevent resizing
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false; // Disable maximize button


        }

        private void AddCartClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();

            currentOrderButton.Visible = true;
            flowLayoutPanelForItemShown.Size = new Size(2750, 1467);
            addToCart.ForeColor = Color.FromArgb(117, 55, 216);
            flowLayoutPanelForPayaMethod.Visible = false;
            flowLayoutPanelForBackUpdateCheckOut.Visible = false;
            flowLayoutPanelForToatalPrice.Visible = false;


            //flowLayoutPanelForOperationalButton0.Size = new Size(45, 1641);
            //flowLayoutPanelForOperationalButton1.AutoScroll = false;
            //flowLayoutPanelForOperationalButton2.AutoScroll = false;

            LoadCart(); // Call LoadGoods to populate the FlowLayoutPanel
            PriceLabel.Text = totalPrice+".00 TK".ToString();

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


            addToCart.Enabled = false;

            // SearchBox Placeholder Events
            this.searchTextBox.Enter += new System.EventHandler(this.searchTextBox_Enter);
            this.searchTextBox.Leave += new System.EventHandler(this.searchTextBox_Leave);
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);

            this.Click += Form_Click; // Attach click event to the form
            foreach (Control control in this.Controls)
            {
                control.Click += Form_Click; // Attach to all controls
            }

        }


        private void LoadCart(string searchTerm = "")
        {
            string customerID = "";
            string farmerID = "";
            List<Panel> panels = new List<Panel>(); // Store panels in memory first
            string query = "";

            if (ID.StartsWith("FRM"))
            {
                farmerID = ID;
                query = @"
                    SELECT G.GoodsID AS ProductID, 
                           G.GoodsName AS ProductName, 
                           G.GoodsImage AS Image, 
                           G.GoodsPrice AS Price, 
                           G.Units AS Unit, 
                           AC.ProductQuantity AS Quantity, 
                           (G.GoodsPrice * AC.ProductQuantity) AS TotalPrice
                    FROM AgroBridge.dbo.Goods G
                    JOIN AgroBridge.dbo.AddCart AC ON G.GoodsID = AC.GoodsID
                    WHERE AC.FarmerID = @FarmerID
                    AND G.GoodsName LIKE '%' + @SearchTerm + '%'

                    UNION ALL

                    SELECT W.WasteID AS ProductID, 
                           W.WasteName AS ProductName, 
                           W.WasteImage AS Image, 
                           W.WastePrice AS Price, 
                           W.Units AS Unit, 
                           AC.ProductQuantity AS Quantity, 
                           (W.WastePrice * AC.ProductQuantity) AS TotalPrice
                    FROM AgroBridge.dbo.Waste W
                    JOIN AgroBridge.dbo.AddCart AC ON W.WasteID = AC.WasteID
                    WHERE AC.FarmerID = @FarmerID
                    AND W.WasteName LIKE '%' + @SearchTerm + '%'

                    UNION ALL

                    SELECT R.EquipmentID AS ProductID, 
                           R.EquipmentName AS ProductName, 
                           R.EqipmentImage AS Image, 
                           R.EquipmentRentalPrice AS Price, 
                           'Rental' AS Unit, 
                           AC.ProductQuantity AS Quantity, 
                           (R.EquipmentRentalPrice * AC.ProductQuantity) AS TotalPrice
                    FROM AgroBridge.dbo.Rent R
                    JOIN AgroBridge.dbo.AddCart AC ON R.EquipmentID = AC.EquipmentID
                    WHERE AC.FarmerID = @FarmerID
                    AND R.EquipmentName LIKE '%' + @SearchTerm + '%';";

            }
            else if (ID.StartsWith("BUY") || ID.StartsWith("WOR"))
            {
                customerID = ID;
                query = @"
                    SELECT G.GoodsID AS ProductID, 
                           G.GoodsName AS ProductName, 
                           G.GoodsImage AS Image, 
                           G.GoodsPrice AS Price, 
                           G.Units AS Unit, 
                           AC.ProductQuantity AS Quantity, 
                           (G.GoodsPrice * AC.ProductQuantity) AS TotalPrice
                    FROM AgroBridge.dbo.Goods G
                    JOIN AgroBridge.dbo.AddCart AC ON G.GoodsID = AC.GoodsID
                    WHERE AC.CustomerID = @CustomerID
                    AND G.GoodsName LIKE '%' + @SearchTerm + '%'

                    UNION ALL

                    SELECT W.WasteID AS ProductID, 
                           W.WasteName AS ProductName, 
                           W.WasteImage AS Image, 
                           W.WastePrice AS Price, 
                           W.Units AS Unit, 
                           AC.ProductQuantity AS Quantity, 
                           (W.WastePrice * AC.ProductQuantity) AS TotalPrice
                    FROM AgroBridge.dbo.Waste W
                    JOIN AgroBridge.dbo.AddCart AC ON W.WasteID = AC.WasteID
                    WHERE AC.CustomerID = @CustomerID
                    AND W.WasteName LIKE '%' + @SearchTerm + '%'

                    UNION ALL

                    SELECT R.EquipmentID AS ProductID, 
                           R.EquipmentName AS ProductName, 
                           R.EqipmentImage AS Image, 
                           R.EquipmentRentalPrice AS Price, 
                           'Rental' AS Unit, 
                           AC.ProductQuantity AS Quantity, 
                           (R.EquipmentRentalPrice * AC.ProductQuantity) AS TotalPrice
                    FROM AgroBridge.dbo.Rent R
                    JOIN AgroBridge.dbo.AddCart AC ON R.EquipmentID = AC.EquipmentID
                    WHERE AC.CustomerID = @CustomerID
                    AND R.EquipmentName LIKE '%' + @SearchTerm + '%';";

            }

            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Invalid ID type. Unable to load cart.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(customerID))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    }
                    else if (!string.IsNullOrEmpty(farmerID))
                    {
                        cmd.Parameters.AddWithValue("@FarmerID", farmerID);
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    }

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows) return; // Prevent access if no data exists

                        while (reader.Read())
                        {
                            if (reader.IsClosed) break; // Ensure reader is open
                            double price = Convert.ToDouble(reader["Price"]);
                            int quantity = Convert.ToInt32(reader["Quantity"]);
                            totalPrice += price * quantity; // Accumulate total price

                            Panel panel = new Panel
                            {
                                Size = new Size(865, 60),
                                BorderStyle = BorderStyle.FixedSingle,
                                BackColor = Color.FromArgb(242, 239, 231)
                            };

                            Label nameLabel = new Label
                            {
                                Text = reader["ProductName"].ToString(),
                                Location = new Point(10, 10),
                                Font = new Font("Arial", 9),
                                ForeColor = Color.Black,
                                AutoSize = false, // Prevents auto-sizing
                                Size = new Size(100, 30), // Set a fixed size for the label
                                TextAlign = ContentAlignment.MiddleCenter, // Centers text
                            };

                            PictureBox pictureBox = new PictureBox
                            {
                                Size = new Size(50, 50),
                                Location = new Point(205, 5),
                                SizeMode = PictureBoxSizeMode.StretchImage
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("Image")))
                            {
                                byte[] imageData = (byte[])reader["Image"];
                                using (MemoryStream ms = new MemoryStream(imageData))
                                {
                                    pictureBox.Image = Image.FromStream(ms);
                                }
                            }

                            
                           

                            Label priceLabel = new Label
                            {
                                Text = reader["Price"].ToString()+ " ৳",
                                Location = new Point(320, 10),
                                Font = new Font("Arial", 10),
                                ForeColor = Color.Black,
                                AutoSize = false, // Prevents auto-sizing
                                Size = new Size(100, 30), // Set a fixed size for the label
                                TextAlign = ContentAlignment.MiddleCenter, // Centers text
                            };

                            Label unitLabel = new Label
                            {
                                Text = reader["Unit"].ToString(),
                                Location = new Point(400, 10),
                                Font = new Font("Arial", 10),
                                ForeColor = Color.Black,
                                AutoSize = false, // Prevents auto-sizing
                                Size = new Size(100, 30), // Set a fixed size for the label
                                TextAlign = ContentAlignment.MiddleCenter, // Centers text
                            };

                            string productID = reader["ProductID"].ToString(); // Store the ProductID before the reader closes

                            NumericUpDown quantityUpDown = new NumericUpDown
                            {
                                Value = Convert.ToDecimal(reader["Quantity"]),
                                Minimum = 1,
                                Maximum = 100, // Adjust according to your needs
                                Location = new Point(540, 20),
                                Size = new Size(60, 30),
                                TextAlign = HorizontalAlignment.Center // Centers the value text
                            };

                            // Use the stored productID instead of accessing reader
                            quantityUpDown.ValueChanged += (s, e) => UpdateQuantity(productID, (int)quantityUpDown.Value);


                            Label totalPriceLabel = new Label
                            {
                                Text = reader["TotalPrice"].ToString() + " ৳",
                                TextAlign = ContentAlignment.MiddleCenter, // Centers text
                                Location = new Point(665, 10),
                                Font = new Font("Arial", 10),
                                ForeColor = Color.Black,
                                AutoSize = false, // Prevents auto-sizing
                                Size = new Size(100, 30) // Set a fixed size for the label
                                
                            };


                            Button deleteButton = new Button
                            {
                                Text = " X ",
                                Size = new Size(60, 30),
                                Font = new Font("Arial", 10, FontStyle.Bold),
                                Location = new Point(800, 15),
                                ForeColor = Color.Red
                               
                            };

                            
                            deleteButton.Click += (s, e) => DeleteFromCart(productID);

                            panel.Controls.Add(nameLabel);
                            panel.Controls.Add(pictureBox);
                            panel.Controls.Add(priceLabel);
                            panel.Controls.Add(unitLabel);
                            panel.Controls.Add(quantityUpDown); // Add the NumericUpDown control
                            panel.Controls.Add(totalPriceLabel);
                            panel.Controls.Add(deleteButton);

                            panels.Add(panel);
                        }
                    }
                }
            }

            flowLayoutPanelForItemShown.Controls.Clear();
            foreach (var panel in panels)
            {
                flowLayoutPanelForItemShown.Controls.Add(panel);
            }

            // **Handle Empty Cart UI Visibility**
            if (panels.Count > 0)
            {
                emptyPanel.Visible = false;
                flowLayoutPanelForBackUpdateCheckOut.Visible = true;
                flowLayoutPanelForToatalPrice.Visible = true;


            }
            else
            {
                emptyPanel.Visible = true;
                
            }
        }

        // Event to update the quantity in the database
        private void UpdateQuantity(string productID, int newQuantity)
        {
            string query = @"
    UPDATE AddCart 
    SET ProductQuantity = @NewQuantity 
    WHERE GoodsID = @ProductID OR WasteID = @ProductID OR EquipmentID = @ProductID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productID);
                    cmd.Parameters.AddWithValue("@NewQuantity", newQuantity);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    
                }
            }
        }



        private void DeleteFromCart(string productID)
        {
            string query = @"
    DELETE FROM AddCart 
    WHERE GoodsID = @ProductID OR WasteID = @ProductID OR EquipmentID = @ProductID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productID);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Product deleted successfully.");
                        LoadCart(); // Reload cart and recalculate total price
                    }
                    else
                    {
                        MessageBox.Show("Product not found in the cart.");
                    }
                }
            }
        }






        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //if (flowLayoutPanelForOperationalButton0.Width <= 45)
            //{
            //    flowLayoutPanelForOperationalButton0.Size = new Size(172, 1641);


            //}
            //else
            //{
            //    flowLayoutPanelForOperationalButton0.Size = new Size(45, 1641);

            //}

            //flowLayoutPanelForOperationalButton0.Refresh(); // Force UI update
            //flowLayoutPanelForOperationalButton1.AutoScroll = false;
            //flowLayoutPanelForOperationalButton2.AutoScroll = false;
            //LoadCart();
        }

        private void PriceLabel_Click(object sender, EventArgs e)
        {
            
        }

        private void flowLayoutPanelForItemShown_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanelForProfile_Paint(object sender, PaintEventArgs e)
        {

        }

        private void updateCart_Click(object sender, EventArgs e)
        {

            AddCartClass addCartClass = new AddCartClass(ID);
            addCartClass.Show();
            this.Close();
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void backToHomeButton_Click(object sender, EventArgs e)
        {
            HomePage homePage = new HomePage(ID);
            homePage.Show();
            this.Close();
        }




        // Handles Click Outside the Search Box


        private void Form_Click(object sender, EventArgs e)
        {
            if (!searchTextBox.Focused)
            {
                if (searchBoxClicked && string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchBoxClicked = false; // Reset flag
                    ResetSearchBox(); // Reset placeholder text
                    return;
                }

                ResetSearchBox();
            }
        }


        // Reset Search Box Placeholder
        private void ResetSearchBox()
        {
            if (string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchTextBox.Text = "Search Item";
                searchTextBox.ForeColor = Color.Gray;
            }
        }
        private void searchTextBox_Enter(object sender, EventArgs e)
        {
            searchBoxClicked = true; // Mark search box as clicked

            if (searchTextBox.Text == "Search Item")
            {
                searchTextBox.Text = ""; // Clears the default text when clicked
                searchTextBox.ForeColor = Color.Black;
            }
        }
        private void searchTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchTextBox.Text = "Search Item"; // Restore default text if empty
                searchTextBox.ForeColor = Color.Gray; // Optional: Indicate placeholder text
                LoadCart();
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm) || !searchTerm.Equals(""))
            {
                LoadCart(searchTerm);
            }
            else
            {
                return;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            String searchTerm = searchTextBox.Text.Trim();



            if (searchTerm.Equals("Search Item") || searchTerm.Equals(""))
            {
                return;
            }
            else
            {
                LoadCart(searchTerm);
            }
        }

        private void CheckOutButton_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanelForPayaMethod.Visible)
            {
                flowLayoutPanelForPayaMethod.Visible = false;
            }
            else
            {
                flowLayoutPanelForPayaMethod.Visible = true;
            }
        }

        private void BkashButton_Click(object sender, EventArgs e)
        {
            PaymentClass paymentClass = new PaymentClass(ID, "Bkash");
            paymentClass.ShowDialog();
            

        }

        private void button8_Click(object sender, EventArgs e)
        {
            PaymentClass paymentClass = new PaymentClass(ID, "Nagad");
            paymentClass.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            PaymentClass paymentClass = new PaymentClass(ID, "Rocket");
            paymentClass.ShowDialog();
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
            HireWorkerClass hireWorkerClass = new HireWorkerClass(ID);
            hireWorkerClass.Show();
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

        private void currentOrderButton_Click(object sender, EventArgs e)
        {
           
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID);
            ordersCartClass.Show();
            this.Close();
        }

        private void profileButton_Click(object sender, EventArgs e)
        {
            ProfileInfoClass profileInfoClass = new ProfileInfoClass(ID);
            profileInfoClass.Show();
            this.Close();
        }

        private void buttonForContinueShopping_Click(object sender, EventArgs e)
        {
            HomePage homePage = new HomePage(ID);
            homePage.Show();
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

        private void OrderListButton_Click(object sender, EventArgs e)
        {
            if (ID.StartsWith("FRM"))
            {

                OrdersCartClass ordersCartClass = new OrdersCartClass(ID, "Seller");
                ordersCartClass.Show();
                this.Close();

            }
        }
    }
    
}
