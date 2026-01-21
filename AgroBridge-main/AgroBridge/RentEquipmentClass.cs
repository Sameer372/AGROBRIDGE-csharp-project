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
    public partial class RentEquipmentClass : Form
    {

        private String ID;
        string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE
        private bool searchBoxClicked = false; // Track if search box was clicked
        public RentEquipmentClass(String ID)
        {
            InitializeComponent();
            this.ID = ID;
            this.Size = new System.Drawing.Size(1080, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.CenterToScreen();
        }

        
        
        

        private void RentEquipmentClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();

            currentOrderButton.Visible = true;
            flowLayoutPanelForItemShown.Size = new Size(2750, 1467);
            rentEquipmentButton.ForeColor = Color.FromArgb(117, 55, 216);

            flowLayoutPanelForSearchByCatagory.Visible = false;
            flowLayoutPanelForOperationalButton0.Size = new Size(45, 1641);
            flowLayoutPanelForOperationalButton1.AutoScroll = false;
            flowLayoutPanelForOperationalButton2.AutoScroll = false;
            LoadEquipment();




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

        

        

        


        private void LoadEquipment(string searchTerm = "", string categoryPrefix = "")
        {
            // Clear the FlowLayoutPanel before adding items
            flowLayoutPanelForItemShown.Controls.Clear();

            try
            {
                
                string query = "SELECT EquipmentName, EquipmentRentalPrice, EqipmentImage, EquipmentQuantity, EquipmentAvailabilityStatus, EquipmentID FROM Rent " +
                               "WHERE EquipmentName LIKE @SearchTerm";

                // Add category filtering if a prefix is provided
                if (!string.IsNullOrEmpty(categoryPrefix))
                {
                    query += " AND EquipmentID LIKE @CategoryPrefix";
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
                        string equipmentName = reader["EquipmentName"].ToString();
                        string equipmentPrice = reader["EquipmentRentalPrice"].ToString();
                        byte[] equipmentImageBytes = reader["EqipmentImage"] as byte[];
                        int equipmentQuantity = Convert.ToInt32(reader["EquipmentQuantity"]);
                        string equipmentID = reader["EquipmentID"].ToString();
                        string equipmentAvailabilityStatus = reader["EquipmentAvailabilityStatus"].ToString();

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
                            itemPanel.Size = new Size(194, 270);
                        }
                        else
                        {
                            itemPanel.Size = new Size(212, 270);
                        }

                        // Image box
                        PictureBox pictureBox = new PictureBox
                        {
                            Size = new Size(180, 150),
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Dock = DockStyle.Top,
                            Margin = new Padding(10)
                        };

                        if (equipmentImageBytes != null)
                        {
                            using (var ms = new MemoryStream(equipmentImageBytes))
                            {
                                pictureBox.Image = Image.FromStream(ms);
                            }
                        }

                        // Labels
                        Label nameLabel = new Label
                        {
                            Text = equipmentName,
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Arial", 10, FontStyle.Bold),
                            ForeColor = Color.Black,
                            Dock = DockStyle.Top,
                            Height = 30
                        };

                        Label priceLabel = new Label
                        {
                            Text = $"Price: {equipmentPrice}৳",
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
                        /*
                        // Stock Status Label
                        Label stockLabel = new Label
                        {
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Arial", 9, FontStyle.Bold),
                            Dock = DockStyle.Top,
                            Height = 20,
                            ForeColor = Color.White
                        };

                        if (equipmentQuantity > 5)
                        {
                            stockLabel.Text = "In Stock";
                            stockLabel.BackColor = Color.Green;
                        }
                        else if (equipmentQuantity > 0)
                        {
                            stockLabel.Text = "Limited Stock";
                            stockLabel.BackColor = Color.Teal;
                        }
                        else
                        {
                            stockLabel.Text = "Stock Out";
                            stockLabel.BackColor = Color.Red;
                        }*/

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
                            Tag = equipmentID
                        };

                        Label statusLabel = new Label
                        {
                            Text = equipmentAvailabilityStatus,
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Arial", 10, FontStyle.Bold),
                            Dock = DockStyle.Top,
                            Height = 30
                        };

                        if (equipmentAvailabilityStatus == "Available")
                        {
                            statusLabel.BackColor = Color.Green; // Available - Green
                            statusLabel.ForeColor = Color.White; // White text for better contrast
                            
                            
                            if(ID.StartsWith("Admin"))
                            { }
                            else
                            {
                                itemPanel.Cursor = Cursors.Hand;
                                // Ensure the button remains visible when hovered
                                addToCartButton.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                                addToCartButton.MouseLeave += (sender, e) => addToCartButton.Visible = false;

                                // Show button when hovering over item
                                itemPanel.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                                pictureBox.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                                priceLabel.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                                ratingLabel.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                                nameLabel.MouseEnter += (sender, e) => addToCartButton.Visible = true;
                                //stockLabel.MouseEnter += (sender, e) => addToCartButton.Visible = true;
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
                            }
                                
                        }
                        else
                        {
                            statusLabel.BackColor = Color.Red; // Not Available - Red
                            statusLabel.ForeColor = Color.White; // White text for better contrast

                            // Ensure the button remains visible when hovered
                            addToCartButton.MouseEnter += (sender, e) => addToCartButton.Visible = false;
                            addToCartButton.MouseLeave += (sender, e) => addToCartButton.Visible = false;
                            itemPanel.MouseEnter += (sender, e) => addToCartButton.Visible = false;
                            pictureBox.MouseEnter += (sender, e) => addToCartButton.Visible = false;
                            priceLabel.MouseEnter += (sender, e) => addToCartButton.Visible = false;
                            ratingLabel.MouseEnter += (sender, e) => addToCartButton.Visible = false;
                            nameLabel.MouseEnter += (sender, e) => addToCartButton.Visible = false;
                            //stockLabel.MouseEnter += (sender, e) => addToCartButton.Visible = false;
                        }

                        

                        // Click Event for Add to Cart ✅ Now Always Works!
                        addToCartButton.Click += (sender, e) =>
                        {

                            AddToCart(equipmentID);
                        };

                        // Ensure button is added
                        itemPanel.Controls.Add(addToCartButton);
                        //itemPanel.Controls.Add(stockLabel);
                        itemPanel.Controls.Add(ratingLabel);
                        itemPanel.Controls.Add(priceLabel);
                        itemPanel.Controls.Add(nameLabel);
                        itemPanel.Controls.Add(pictureBox);
                        itemPanel.Controls.Add(statusLabel);

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
        }

        private void AddToCart(string equipmentID)
        {
           
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
                        cmd.Parameters.AddWithValue("@GoodsID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@CustomerID", string.IsNullOrEmpty(customerID) ? DBNull.Value : (object)customerID);
                        cmd.Parameters.AddWithValue("@FarmerID", string.IsNullOrEmpty(farmerID) ? DBNull.Value : (object)farmerID);
                        cmd.Parameters.AddWithValue("@WasteID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@EquipmentID", equipmentID);
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

        //For All Search Functionlity

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
                LoadEquipment();
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm) || !searchTerm.Equals(""))
            {
                LoadEquipment(searchTerm);
            }
            else
            {
                return;
            }
        }

        private void SearchIcon_Click(object sender, EventArgs e)
        {
            String searchTerm = searchTextBox.Text.Trim();



            if (searchTerm.Equals("Search Item") || searchTerm.Equals(""))
            {
                return;
            }
            else
            {
                LoadEquipment(searchTerm);
            }
        }

        private void FilterbyCatagoryButton_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanelForSearchByCatagory.Visible)
            {
                flowLayoutPanelForSearchByCatagory.Visible = false;
            }
            else
            {
                flowLayoutPanelForSearchByCatagory.Visible = true;
            }
        }
        private void foodWasteButton_Click(object sender, EventArgs e)
        {
            LoadEquipment(categoryPrefix: "TIL");
        }

       
        private void tractorButton_Click(object sender, EventArgs e)
        {
            LoadEquipment(categoryPrefix: "TRA");
        }

        private void plantingButton_Click(object sender, EventArgs e)
        {
            LoadEquipment(categoryPrefix: "PLA");
        }

        private void harvestingButton_Click(object sender, EventArgs e)
        {
            LoadEquipment(categoryPrefix: "HAR");
        }

        private void cropCareButton_Click(object sender, EventArgs e)
        {
            LoadEquipment(categoryPrefix: "CRO");
        }


        //All Button Functionality
        private void menuBarButton_Click(object sender, EventArgs e)
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
            LoadEquipment();
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
            LoadEquipment("");
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

        private void profileButton_Click(object sender, EventArgs e)
        {
            ProfileInfoClass profileInfoClass = new ProfileInfoClass(ID);
            profileInfoClass.Show();
            this.Close();
        }

        private void DashboardButton_Click(object sender, EventArgs e)
        {
            DashBoardClass dashBoard = new DashBoardClass(ID);
            dashBoard.Show();
            this.Close();
        }

        private void flowLayoutPanelForItemShown_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
