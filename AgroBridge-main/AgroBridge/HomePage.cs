using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AgroBridge
{
    public partial class HomePage : Form
    {
        private String ID;
        private String selectedItemName;
        private String selectedItemPrice;
        private Image selectedItemImage;
        private String selectedItemQuantity;
        private double totalPrice;
        private bool searchBoxClicked = false; // Track if search box was clicked
        string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE




        public HomePage()
        {
           
            InitializeComponent();
           
            
        }

        public HomePage(String ID)
        {
            InitializeComponent();
            this.ID = ID;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.CenterToScreen();
            this.Size = new System.Drawing.Size(1080, 640);
        }

        private void addGoodsButton_Click(object sender, EventArgs e)
        {
            ProductAdd productAdd = new ProductAdd(ID);
            productAdd.ShowDialog();
            
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
                LoadGoods(searchTerm);
            }
                
        }



        private void HomePage_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();

            currentOrderButton.Visible = true;
            flowLayoutPanelForItemShown.Size = new Size(2750, 1467);
            allItemButton.ForeColor = Color.FromArgb(117, 55, 216);

            PanelForSearchByCatagory.Visible = false;
            flowLayoutPanelForOperationalButton0.Size = new Size(45, 1641);
            flowLayoutPanelForOperationalButton1.AutoScroll = false;
            flowLayoutPanelForOperationalButton2.AutoScroll = false;
            emptyPanel.Visible = true;



            LoadGoods(); // Call LoadGoods to populate the FlowLayoutPanel

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

            else if (ID.StartsWith("Admin"))
            {
                applyForDonateButton.Visible = false;
                addGoodsButton.Visible = false;
                updateAndDeleteButton.Visible = false;
                hireWorkerButton.Visible = true;
                updateEquipmentInformaationButton.Visible = false;
                DonateButton.Visible = false;
                OrderListButton.Visible = false;
                workingProfileButton.Visible = false;
                currentOrderButton.Visible = false;
                addToCart.Visible = false;
                profileButton.Visible = false;


            }

            // UI Styling for Order Labels and Buttons


            currentOrderButton.Cursor = Cursors.Hand;
            //currentOrderButton.BackColor = Color.Red;
            currentOrderButton.ForeColor = Color.White;

            

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



        private void profileButton_Click(object sender, EventArgs e)
        {
            ProfileInfoClass profileInfoClass = new ProfileInfoClass(ID);
            profileInfoClass.Show();
            this.Close();
        }

        private void flowLayoutPanelForItemShown_Paint(object sender, PaintEventArgs e)
        {
        }

        private void LoadGoods(string searchTerm = "", string categoryPrefix = "")
        {
            flowLayoutPanelForItemShown.Controls.Clear();

            try
            {
                
                string query = "SELECT GoodsName, GoodsPrice, GoodsImage, GoodsQuantity, GoodsID FROM Goods " +
                                "WHERE GoodsName LIKE @SearchTerm";

                if (!string.IsNullOrEmpty(categoryPrefix))
                {
                    query += " AND GoodsID LIKE @CategoryPrefix";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    if (!string.IsNullOrEmpty(categoryPrefix))
                    {
                        command.Parameters.AddWithValue("@CategoryPrefix", categoryPrefix + "%");
                    }

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string goodsName = reader["GoodsName"].ToString();
                        string goodsPrice = reader["GoodsPrice"].ToString();
                        byte[] goodsImageBytes = reader["GoodsImage"] as byte[];
                        int goodsQuantity = Convert.ToInt32(reader["GoodsQuantity"]);
                        string goodsID = reader["GoodsID"].ToString();

                        // Create main panel
                        Panel itemPanel = new Panel
                        {
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.FromArgb(242, 239, 231),
                            Padding = new Padding(5),
                            
                        };

                        // Adjust panel size dynamically
                        flowLayoutPanelForOperationalButton0.Refresh();
                        if (flowLayoutPanelForOperationalButton0.Width <= 45)
                        {
                            itemPanel.Size = new Size(194, 265);
                        }
                        else
                        {
                            itemPanel.Size = new Size(212, 265);
                        }

                        // Image box
                        PictureBox pictureBox = new PictureBox
                        {
                            Size = new Size(180, 150),
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Dock = DockStyle.Top,
                            Margin = new Padding(10)
                        };

                        if (goodsImageBytes != null)
                        {
                            using (var ms = new MemoryStream(goodsImageBytes))
                            {
                                pictureBox.Image = Image.FromStream(ms);
                            }
                        }

                        // Labels
                        Label nameLabel = new Label
                        {
                            Text = goodsName,
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Arial", 10, FontStyle.Bold),
                            ForeColor = Color.Black,
                            Dock = DockStyle.Top,
                            Height = 30
                        };

                        Label priceLabel = new Label
                        {
                            Text = $"Price: {goodsPrice}৳",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Arial", 9, FontStyle.Regular),
                            ForeColor = Color.FromArgb(41, 115, 178),
                            Dock = DockStyle.Top,
                            Height = 20
                        };

                        Label ratingLabel = new Label
                        {
                            Text = "★★★★★",
                            ForeColor = Color.Gold,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Arial", 12, FontStyle.Bold),
                            Dock = DockStyle.Top,
                            Height = 25
                        };

                        // Add to Cart Button (Initially Hidden)
                        Button addToCartButton = new Button
                        {
                            Text = "🛒 Add to Cart 🛒",
                            Font = new Font("Arial", 10, FontStyle.Bold),
                            BackColor = Color.Orange,
                            ForeColor = Color.Black,
                            FlatStyle = FlatStyle.Flat,
                            Dock = DockStyle.Bottom,
                            Height = 30,
                            Visible = false, // Initially hidden
                            Tag = goodsID
                        };

                        // Stock Status Label
                        Label stockLabel = new Label
                        {
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Arial", 9, FontStyle.Bold),
                            Dock = DockStyle.Top,
                            Height = 20,
                            ForeColor = Color.White
                        };

                        

                        if (goodsQuantity > 5)
                        {
                            stockLabel.Text = "In Stock";
                            stockLabel.BackColor = Color.Green;
                            
                            if (ID.StartsWith("Admin"))
                            {
                                
                            }
                            else
                            {
                                itemPanel.Controls.Add(addToCartButton);
                                itemPanel.Cursor = Cursors.Hand;
                            }
                                
                        }
                        else if (goodsQuantity > 0)
                        {
                            stockLabel.Text = "Limited Stock";
                            stockLabel.BackColor = Color.Teal;
                            

                            if (ID.StartsWith("Admin"))
                            {
                                
                            }
                            else
                            {
                                itemPanel.Controls.Add(addToCartButton);
                                itemPanel.Cursor = Cursors.Hand;
                            }
                        }

                        else
                        {
                            stockLabel.Text = "Stock Out";
                            stockLabel.BackColor = Color.Red;

                        }

                        

                        // Ensure the button remains visible when hovered
                        addToCartButton.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                        addToCartButton.MouseLeave += (sender, e) => addToCartButton.Visible = false;

                        // Show button when hovering over item
                        itemPanel.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                        pictureBox.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                        priceLabel.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                        ratingLabel.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                        nameLabel.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                        stockLabel.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                        itemPanel.MouseLeave += (sender, e) =>
                        {
                            // Only hide button if cursor is NOT on the button
                            if (!addToCartButton.ClientRectangle.Contains(addToCartButton.PointToClient(Cursor.Position)))
                            {
                                addToCartButton.Visible = false;
                            }
                        };
                        pictureBox.MouseLeave += (sender, e) =>
                        {
                            
                            if (!addToCartButton.ClientRectangle.Contains(addToCartButton.PointToClient(Cursor.Position)))
                            {
                                addToCartButton.Visible = false;
                            }
                        };

                        priceLabel.MouseLeave += (sender, e) =>
                        {
                            
                            if (!addToCartButton.ClientRectangle.Contains(addToCartButton.PointToClient(Cursor.Position)))
                            {
                                addToCartButton.Visible = false;
                            }
                        };
                        ratingLabel.MouseLeave += (sender, e) =>
                        {
                           
                            if (!addToCartButton.ClientRectangle.Contains(addToCartButton.PointToClient(Cursor.Position)))
                            {
                                addToCartButton.Visible = false;
                            }
                        };

                        nameLabel.MouseLeave += (sender, e) =>
                        {

                            if (!addToCartButton.ClientRectangle.Contains(addToCartButton.PointToClient(Cursor.Position)))
                            {
                                addToCartButton.Visible = false;
                            }
                        };

                        // Click Event for Add to Cart ✅ Now Always Works!
                        addToCartButton.Click += (sender, e) =>
                        {
                            
                            AddToCart(goodsID);
                        };

                        // Ensure button is added
                        itemPanel.Controls.Add(stockLabel);
                        itemPanel.Controls.Add(ratingLabel);
                        itemPanel.Controls.Add(priceLabel);
                        itemPanel.Controls.Add(nameLabel);
                        itemPanel.Controls.Add(pictureBox);

                        // Ensure button is in the front layer
                        addToCartButton.BringToFront();

                        // Add to main panel
                        flowLayoutPanelForItemShown.Controls.Add(itemPanel);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        /*
            // **Handle Empty Cart UI Visibility**
           
            if (panels.Count > 0)
            {
                emptyPanel.Visible = false;
                
            }
            else
            {
                emptyPanel.Visible = true;
            }
        */
        }




        // Add to cart function
        private void AddToCart(string goodsID)
        {
            //string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;";
            try
            {
                string customerID = "";
                string farmerID = "";
                int productQuantity = 1;

                if (ID.StartsWith("FRM"))
                {
                    farmerID = ID;
                }
                else if (ID.StartsWith("BUY") || ID.StartsWith("WOR"))
                {
                    customerID = ID;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"INSERT INTO AddCart (GoodsID, CustomerID, FarmerID, WasteID, EquipmentID,ProductQuantity) 
                             VALUES (@GoodsID, @CustomerID, @FarmerID, @WasteID, @EquipmentID,@ProductQuantity)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@GoodsID", goodsID);
                        cmd.Parameters.AddWithValue("@CustomerID", string.IsNullOrEmpty(customerID) ? DBNull.Value : (object)customerID);
                        cmd.Parameters.AddWithValue("@FarmerID", string.IsNullOrEmpty(farmerID) ? DBNull.Value : (object)farmerID);
                        cmd.Parameters.AddWithValue("@WasteID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@EquipmentID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductQuantity", productQuantity);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Product added to cart!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to add product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

            // Add a new row with the selected item's details
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
                LoadGoods();
            }
        }



        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm) || !searchTerm.Equals(""))
            {
                LoadGoods(searchTerm);
            }
            else 
            {
                return;
            }
        }




        

        

        private void totalPriceTextBox_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void fruitsButton_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "FR");
        }

        private void vegeatablesButton_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "VE");
        }

        private void meatButton_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "ME");
        }

        private void fishButton_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "FI");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "SE");
        }

        private void allItemButton_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "");
            //allItemButton.BackColor = Color.FromArgb(173, 216, 230); // RGB for AliceBlue
        }

        //all Button Functionlity
        private void allWasteButton_Click(object sender, EventArgs e)
        {
            AllWasteItem allWasteItem = new  AllWasteItem(ID);
            allWasteItem.Show();
            this.Close();
        }

        private void updateAndDeleteButton_Click(object sender, EventArgs e)
        {
            UpdateAndDeleteClass updateAndDeleteClass = new UpdateAndDeleteClass(ID);
            updateAndDeleteClass.Show();
            this.Close();
        }

        private void rentEquipmentButton_Click(object sender, EventArgs e)
        {
            RentEquipmentClass rentEquipmentClass = new RentEquipmentClass(ID);
            rentEquipmentClass.Show();
            this.Close();   
        }

        private void logOutButton_Click(object sender, EventArgs e)
        {
            DonorClass donorClass = new DonorClass(ID);
            donorClass.Show();
            this.Close();
        }

        private void updateEquipmentInformaationButton_Click(object sender, EventArgs e)
        {
            UpdateEquipmentInformaationClass updateEquipmentInformaationClass = new UpdateEquipmentInformaationClass(ID);
            updateEquipmentInformaationClass.Show();
            this.Close();
        }

        private void applyForDonateButton_Click(object sender, EventArgs e)
        {
            ApplyForDonateClass applyForDonateClass = new ApplyForDonateClass(ID);
            applyForDonateClass.Show();
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void currentOrderButton_Click(object sender, EventArgs e)
        {
            OrdersCartClass ordersCartClass = new OrdersCartClass(ID);
            ordersCartClass.Show();
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
            currentOrderButton.Visible = true;
            flowLayoutPanelForItemShown.Size = new Size(2750, 1467);
            flowLayoutPanelForItemShown.PerformLayout(); // Force re-layout of child controls
            LoadGoods();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginClass loginClass = new LoginClass();
            loginClass.Show();
            this.Close();
        }

        private void SkilledButton_Click(object sender, EventArgs e)
        {

        }

        private void logoutButtton_Click(object sender, EventArgs e)
        {
            LoginClass loginClass = new LoginClass();
            loginClass.Show();
            this.Close();
        }

        private void addEquipmentButton_Click(object sender, EventArgs e)
        {
            DashBoard addEqipment = new DashBoard(ID);
            addEqipment.Show();
            this.Close();
        }

        private void DonateButton_Click(object sender, EventArgs e)
        {
            DonorClass donorClass = new DonorClass(ID);
            donorClass.Show();
            this.Close();
        }

        private void payButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedItemName) || string.IsNullOrEmpty(ID))
            {
                MessageBox.Show("Please select an item before proceeding to payment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve GoodsID using selectedItemName
                    string goodsIdQuery = "SELECT GoodsID FROM Goods WHERE GoodsName = @GoodsName";
                    SqlCommand goodsIdCommand = new SqlCommand(goodsIdQuery, connection);
                    goodsIdCommand.Parameters.AddWithValue("@GoodsName", selectedItemName);
                    string goodsID = goodsIdCommand.ExecuteScalar()?.ToString();

                    if (string.IsNullOrEmpty(goodsID))
                    {
                        MessageBox.Show("The selected item does not exist in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Retrieve the FarmerID for the selected GoodsID
                    string farmerIdQuery = "SELECT FarmerID FROM Goods WHERE GoodsID = @GoodsID";
                    SqlCommand farmerIdCommand = new SqlCommand(farmerIdQuery, connection);
                    farmerIdCommand.Parameters.AddWithValue("@GoodsID", goodsID);
                    string farmerID = farmerIdCommand.ExecuteScalar()?.ToString();

                    if (string.IsNullOrEmpty(farmerID))
                    {
                        MessageBox.Show("Farmer information not found for the selected item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Insert into the Buy table
                    string insertBuyQuery = "INSERT INTO Buy (CustomerID, GoodsID) VALUES (@CustomerID, @GoodsID)";
                    SqlCommand buyCommand = new SqlCommand(insertBuyQuery, connection);
                    buyCommand.Parameters.AddWithValue("@CustomerID", ID);
                    buyCommand.Parameters.AddWithValue("@GoodsID", goodsID);
                    buyCommand.ExecuteNonQuery();

                    // Insert into the Sell table
                    string insertSellQuery = "INSERT INTO Sell (FarmerID, GoodsID) VALUES (@FarmerID, @GoodsID)";
                    SqlCommand sellCommand = new SqlCommand(insertSellQuery, connection);
                    sellCommand.Parameters.AddWithValue("@FarmerID", farmerID);
                    sellCommand.Parameters.AddWithValue("@GoodsID", goodsID);
                    sellCommand.ExecuteNonQuery();

                    MessageBox.Show("Payment successful! Transaction details recorded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Optionally, clear the selected item and total price
                    selectedItemName = null;
                    selectedItemPrice = null;
                    selectedItemQuantity = null;
                    totalPrice = 0;
                    
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error during payment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void workingProfileButton_Click(object sender, EventArgs e)
        {
            WorkerWorkingInformationClass workerWorkingInformationClass = new WorkerWorkingInformationClass(ID);
            workerWorkingInformationClass.Show();
            this.Close();
        }

        private void DashboardButton_Click(object sender, EventArgs e)
        {
            DashBoardClass dashBoard = new DashBoardClass(ID);
            dashBoard.Show();
            this.Close();
        }

        private void hireWorkerButton_Click(object sender, EventArgs e)
        {
            HireWorkerClass hireWorkerClass = new HireWorkerClass(ID);
            hireWorkerClass.Show();
            this.Close();
        }

        private void FilterbyCatagoryButton_Click(object sender, EventArgs e)
        {
           
            if (PanelForSearchByCatagory.Visible)
            {
                PanelForSearchByCatagory.Visible = false;
            }
            else
            {
                PanelForSearchByCatagory.Visible = true;
            }

            //LoadGoods();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanelForOperationalButton0.Width <= 45)
            {
                flowLayoutPanelForOperationalButton0.Size = new Size(172, 1641);
                
                
            }
            else
            {
                flowLayoutPanelForOperationalButton0.Size = new Size(45, 1641);
                
            }

            flowLayoutPanelForOperationalButton0.Refresh(); // Force UI update
            flowLayoutPanelForOperationalButton1.AutoScroll = false;
            flowLayoutPanelForOperationalButton2.AutoScroll = false;
            LoadGoods();
        }

        private void addToCart_Click(object sender, EventArgs e)
        {
            AddCartClass addCartClass = new AddCartClass(ID);
            addCartClass.Show();
            this.Close();
        }

        private void OrderListButton_Click(object sender, EventArgs e)
        {
            if (ID.StartsWith("FRM"))
            {
                
                OrdersCartClass ordersCartClass = new OrdersCartClass(ID,"Seller");
                ordersCartClass.Show();
                this.Close();

            }
        }

        private void buttonForTryDifferentKeywords_Click(object sender, EventArgs e)
        {

        }
    }
}
