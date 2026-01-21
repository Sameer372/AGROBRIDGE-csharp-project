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
    public partial class UpdateAndDeleteClass : Form
    {
        private String ID;
        string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE
        private bool searchBoxClicked = false; // Track if search box was clicked
        public UpdateAndDeleteClass(String ID)
        {
            InitializeComponent();
            this.ID = ID;
            this.StartPosition = FormStartPosition.WindowsDefaultBounds;
            

        }

        private void UpdateAndDeleteClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();

            currentOrderButton.Visible = true;
            
            updateAndDeleteButton.ForeColor = Color.FromArgb(117, 55, 216);

            flowLayoutPanelForGoodsCatagory.Visible = false;
            flowLayoutPanelForWasteCatagory.Visible = false;
            flowLayoutPanelForOperationalButton0.Size = new Size(45, 1641);
            flowLayoutPanelForOperationalButton1.AutoScroll = false;
            flowLayoutPanelForOperationalButton2.AutoScroll = false;


            LoadGoods(); // Call LoadGoods to populate the FlowLayoutPanel
            LoadWaste();

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


        private void LoadGoods(string searchTerm = "", string categoryPrefix = "")
        {
            // Clear the FlowLayoutPanel before adding items
            flowLayoutPanelForGoods.Controls.Clear();

            try
            {
                
                string query = "SELECT GoodsID, GoodsName, GoodsPrice, GoodsImage, GoodsQuantity,Units FROM Goods " +
                               "WHERE GoodsName LIKE @SearchTerm AND FarmerID = @FarmerID";

                if (!string.IsNullOrEmpty(categoryPrefix))
                {
                    query += " AND GoodsID LIKE @CategoryPrefix";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    command.Parameters.AddWithValue("@FarmerID", ID);

                    if (!string.IsNullOrEmpty(categoryPrefix))
                    {
                        command.Parameters.AddWithValue("@CategoryPrefix", categoryPrefix + "%");
                    }

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string goodsID = reader["GoodsID"].ToString();
                        string goodsName = reader["GoodsName"].ToString();
                        string goodsPrice = reader["GoodsPrice"].ToString();
                        byte[] goodsImageBytes = reader["GoodsImage"] as byte[];
                        int goodsQuantity = Convert.ToInt32(reader["GoodsQuantity"]);
                        string units = reader["Units"].ToString();

                        Panel itemPanel = new Panel
                        {
                           
                            BorderStyle = BorderStyle.FixedSingle,
                            Margin = new Padding(5),
                            BackColor = Color.FromArgb(242, 239, 231)
                        };

                        // Adjust panel size dynamically
                        flowLayoutPanelForOperationalButton0.Refresh();
                        if (flowLayoutPanelForOperationalButton0.Width <= 45)
                        {
                            itemPanel.Size = new Size(175, 320);
                        }
                        else
                        {
                            itemPanel.Size = new Size(193, 320);
                        }



                        PictureBox pictureBox = new PictureBox
                        {
                            Size = new Size(180, 180),
                            Dock = DockStyle.Top,
                            SizeMode = PictureBoxSizeMode.Zoom
                        };

                        if (goodsImageBytes != null)
                        {
                            using (var ms = new MemoryStream(goodsImageBytes))
                            {
                                pictureBox.Image = Image.FromStream(ms);
                            }
                        }

                        TextBox nameTextBox = new TextBox
                        {
                            Text = goodsName,
                            Dock = DockStyle.Top,
                            TextAlign = HorizontalAlignment.Center
                        };

                        TextBox priceTextBox = new TextBox
                        {
                            Text = goodsPrice,
                            Dock = DockStyle.Top,
                            TextAlign = HorizontalAlignment.Center
                        };

                        TextBox quantityTextBox = new TextBox
                        {
                            Text = goodsQuantity.ToString(),
                            Dock = DockStyle.Top,
                            TextAlign = HorizontalAlignment.Center
                        }; 

                       
                        ComboBox unitComboBox = new ComboBox
                        {

                            Dock = DockStyle.Top,
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            Height = 30
                        };
                        unitComboBox.Items.Add("Kg");
                        unitComboBox.Items.Add("Liter");
                        unitComboBox.Items.Add("Dozen");
                        unitComboBox.Items.Add("Gram");
                        unitComboBox.SelectedItem = units;

                        




                        Button updateButton = new Button
                        {
                            Text = "Update",
                            Dock = DockStyle.Top,
                            Height = 30,
                            BackColor = Color.LightGreen,
                            Font = new Font("Arial", 10, FontStyle.Bold)
                        };

                        updateButton.Click += (sender, e) =>
                        {
                            int quantity = Convert.ToInt32(quantityTextBox.Text);
                            UpdateGoods(goodsID, nameTextBox.Text, priceTextBox.Text, quantity, unitComboBox.SelectedItem.ToString());
                        };

                        // Delete button
                        Button deleteButton = new Button
                        {
                            Text = "Delete",
                            Dock = DockStyle.Top,
                            Height = 30,
                            BackColor = Color.LightCoral,
                            Font = new Font("Arial", 10, FontStyle.Bold)
                        };

                        deleteButton.Click += (sender, e) =>
                        {
                            DeleteGoods(goodsID, itemPanel);
                        };

                        // Add buttons to panel
                        itemPanel.Controls.Add(deleteButton);
                        itemPanel.Controls.Add(updateButton);
                        itemPanel.Controls.Add(unitComboBox);
                        itemPanel.Controls.Add(quantityTextBox);
                        itemPanel.Controls.Add(priceTextBox);
                        itemPanel.Controls.Add(nameTextBox);
                        itemPanel.Controls.Add(pictureBox);

                        flowLayoutPanelForGoods.Controls.Add(itemPanel);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to delete goods
        private void DeleteGoods(string goodsID, Panel itemPanel)
{
    try
    {
        
        string checkBuyReferenceQuery = "SELECT COUNT(*) FROM Buy WHERE GoodsID = @GoodsID";
        string checkSellReferenceQuery = "SELECT COUNT(*) FROM Sell WHERE GoodsID = @GoodsID";
        string deleteBuyQuery = "DELETE FROM Buy WHERE GoodsID = @GoodsID";
        string deleteSellQuery = "DELETE FROM Sell WHERE GoodsID = @GoodsID";
        string deleteGoodsQuery = "DELETE FROM Goods WHERE GoodsID = @GoodsID AND FarmerID = @FarmerID";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Step 1: Check if the GoodsID is referenced in the Buy table
            using (SqlCommand checkBuyCommand = new SqlCommand(checkBuyReferenceQuery, connection))
            {
                checkBuyCommand.Parameters.AddWithValue("@GoodsID", goodsID);
                int buyReferenceCount = (int)checkBuyCommand.ExecuteScalar();

                if (buyReferenceCount > 0)
                {
                    DialogResult result = MessageBox.Show(
                        "This item is referenced in the Buy table. Do you want to delete all associated records?",
                        "Confirm Deletion",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.Yes)
                    {
                        using (SqlCommand deleteBuyCommand = new SqlCommand(deleteBuyQuery, connection))
                        {
                            deleteBuyCommand.Parameters.AddWithValue("@GoodsID", goodsID);
                            deleteBuyCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Deletion canceled.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return; // Exit if user cancels
                    }
                }
            }

            // Step 2: Check if the GoodsID is referenced in the Sell table
            using (SqlCommand checkSellCommand = new SqlCommand(checkSellReferenceQuery, connection))
            {
                checkSellCommand.Parameters.AddWithValue("@GoodsID", goodsID);
                int sellReferenceCount = (int)checkSellCommand.ExecuteScalar();

                if (sellReferenceCount > 0)
                {
                    DialogResult result = MessageBox.Show(
                        "This item is referenced in the Sell table. Do you want to delete all associated records?",
                        "Confirm Deletion",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.Yes)
                    {
                        using (SqlCommand deleteSellCommand = new SqlCommand(deleteSellQuery, connection))
                        {
                            deleteSellCommand.Parameters.AddWithValue("@GoodsID", goodsID);
                            deleteSellCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Deletion canceled.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return; // Exit if user cancels
                    }
                }
            }

            // Step 3: Delete the Goods record
            using (SqlCommand deleteCommand = new SqlCommand(deleteGoodsQuery, connection))
            {
                deleteCommand.Parameters.AddWithValue("@GoodsID", goodsID);
                deleteCommand.Parameters.AddWithValue("@FarmerID", ID);

                int rowsAffected = deleteCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Item deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    flowLayoutPanelForGoods.Controls.Remove(itemPanel); // Remove the item panel from the flowLayoutPanel
                }
                else
                {
                    MessageBox.Show("Error deleting item. Item may not exist or belong to another user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Error deleting item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}





        private void UpdateGoods(string goodsID, string newName, string newPrice, int Quantity, string units)
        {
            try
            {
                
                string query = "UPDATE Goods SET GoodsName = @GoodsName, GoodsPrice = @GoodsPrice, GoodsQuantity = @GoodsQuantity, Units = @Units WHERE GoodsID = @GoodsID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@GoodsName", newName);
                    command.Parameters.AddWithValue("@GoodsPrice", newPrice);
                    command.Parameters.AddWithValue("@GoodsID", goodsID);
                    command.Parameters.AddWithValue("@GoodsQuantity", Quantity);
                    command.Parameters.AddWithValue("@Units", units);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Item updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadGoods(); // Refresh the goods list
                    }
                    else
                    {
                        MessageBox.Show("Failed to update item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void LoadWaste(string searchTerm = "", string categoryPrefix = "")
        {
            // Clear the FlowLayoutPanel before adding items
            flowLayoutPanelForWeste.Controls.Clear();

            try
            {
               
                string query = "SELECT WasteID, WasteName, WastePrice, WasteImage, WasteQuantity,Units FROM Waste " +
                               "WHERE WasteName LIKE @SearchTerm AND FarmerID = @FarmerID";

                // Add category filtering if a prefix is provided
                if (!string.IsNullOrEmpty(categoryPrefix))
                {
                    query += " AND WasteID LIKE @CategoryPrefix";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    command.Parameters.AddWithValue("@FarmerID", ID); // Use the Farmer ID passed to the form

                    if (!string.IsNullOrEmpty(categoryPrefix))
                    {
                        command.Parameters.AddWithValue("@CategoryPrefix", categoryPrefix + "%");
                    }

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        // Retrieve item details
                        string wasteID = reader["WasteID"].ToString();
                        string wasteName = reader["WasteName"].ToString();
                        string wastePrice = reader["WastePrice"].ToString();
                        byte[] wasteImageBytes = reader["WasteImage"] as byte[];
                        int wasteQuantity = Convert.ToInt32(reader["WasteQuantity"]);
                        string units = reader["Units"].ToString();

                        // Create the item panel
                        Panel itemPanel = new Panel
                        {
                            Size = new Size(200, 325),
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.FromArgb(242, 239, 231),
                            Margin = new Padding(5)
                        };

                        PictureBox pictureBox = new PictureBox
                        {
                            Size = new Size(180, 180),
                            Dock = DockStyle.Top,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Margin = new Padding(10)
                        };

                        



                        if (wasteImageBytes != null)
                        {
                            using (var ms = new MemoryStream(wasteImageBytes))
                            {
                                pictureBox.Image = Image.FromStream(ms);
                            }
                        }

                        TextBox nameTextBox = new TextBox
                        {
                            Text = wasteName,
                            Dock = DockStyle.Top,
                            Height = 30,
                            TextAlign = HorizontalAlignment.Center,
                            Font = new Font("Arial", 10, FontStyle.Regular)
                        };

                        TextBox priceTextBox = new TextBox
                        {
                            Text = wastePrice,
                            Dock = DockStyle.Top,
                            Height = 30,
                            TextAlign = HorizontalAlignment.Center,
                            Font = new Font("Arial", 10, FontStyle.Regular)
                        };

                        TextBox quantityTextBox = new TextBox
                        {
                            Text = wasteQuantity.ToString(),
                            Dock = DockStyle.Top,
                            TextAlign = HorizontalAlignment.Center
                        };

                        ComboBox unitComboBox = new ComboBox
                        {

                            Dock = DockStyle.Top,
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            Height = 30
                        };
                        unitComboBox.Items.Add("Kg");
                        unitComboBox.Items.Add("Liter");
                        unitComboBox.Items.Add("Dozen");
                        unitComboBox.Items.Add("Gram");
                        unitComboBox.SelectedItem = units;

                        Button updateButton = new Button
                        {
                            Text = "Update",
                            Dock = DockStyle.Top,
                            Height = 30,
                            BackColor = Color.LightGreen,
                            Font = new Font("Arial", 10, FontStyle.Bold)
                        };

                        Button deleteButton = new Button
                        {
                            Text = "Delete",
                            Dock = DockStyle.Top,
                            Height = 30,
                            BackColor = Color.LightCoral,
                            Font = new Font("Arial", 10, FontStyle.Bold)
                        };

                        // Add click event for update button
                        updateButton.Click += (sender, e) =>
                        {
                            int quantity = Convert.ToInt32(quantityTextBox.Text);
                            UpdateWaste(wasteID, nameTextBox.Text, priceTextBox.Text, quantity, unitComboBox.SelectedItem.ToString());
                        };

                        // Add click event for delete button
                        deleteButton.Click += (sender, e) =>
                        {
                            DeleteWaste(wasteID);
                        };

                        // Add controls to the panel
                        itemPanel.Controls.Add(deleteButton);
                        itemPanel.Controls.Add(updateButton);
                        itemPanel.Controls.Add(unitComboBox);
                        itemPanel.Controls.Add(quantityTextBox);
                        itemPanel.Controls.Add(priceTextBox);
                        itemPanel.Controls.Add(nameTextBox);
                        itemPanel.Controls.Add(pictureBox);

                        // Add panel to the flow layout
                        flowLayoutPanelForWeste.Controls.Add(itemPanel);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading waste items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void UpdateWaste(string wasteID, string newWasteName, string newWastePrice, int Quantity, string units)
        {
            try
            {
                
                string query = "UPDATE Waste SET WasteName = @WasteName, WastePrice = @WastePrice, WasteQuantity = @WasteQuantity, Units = @Units WHERE WasteID = @WasteID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@WasteID", wasteID);
                    command.Parameters.AddWithValue("@WasteName", newWasteName);
                    command.Parameters.AddWithValue("@WastePrice", newWastePrice);
                    command.Parameters.AddWithValue("@WasteQuantity", Quantity);
                    command.Parameters.AddWithValue("@Units", units);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Waste item updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadWaste(); // Refresh the list after update
                    }
                    else
                    {
                        MessageBox.Show("Failed to update the waste item. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating waste item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void DeleteWaste(string wasteID)
        {
            try
            {
               

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Delete dependent records in the Trade table
                    string deleteTradeQuery = "DELETE FROM Trade WHERE WasteID = @WasteID";
                    using (SqlCommand deleteTradeCommand = new SqlCommand(deleteTradeQuery, connection))
                    {
                        deleteTradeCommand.Parameters.AddWithValue("@WasteID", wasteID);
                        deleteTradeCommand.ExecuteNonQuery();
                    }

                    // Delete dependent records in the Purchase table
                    string deletePurchaseQuery = "DELETE FROM Purchase WHERE WasteID = @WasteID";
                    using (SqlCommand deletePurchaseCommand = new SqlCommand(deletePurchaseQuery, connection))
                    {
                        deletePurchaseCommand.Parameters.AddWithValue("@WasteID", wasteID);
                        deletePurchaseCommand.ExecuteNonQuery();
                    }

                    // Delete the waste item
                    string deleteWasteQuery = "DELETE FROM Waste WHERE WasteID = @WasteID";
                    using (SqlCommand deleteWasteCommand = new SqlCommand(deleteWasteQuery, connection))
                    {
                        deleteWasteCommand.Parameters.AddWithValue("@WasteID", wasteID);
                        int rowsAffected = deleteWasteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Waste item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadWaste(); // Refresh the list after deletion
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the waste item. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting waste item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                LoadGoods("");
                LoadWaste("");
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm) || !searchTerm.Equals(""))
            {
                LoadGoods(searchTerm);
                LoadWaste(searchTerm);
            }
            else
            {
                return;
            }
        }

        private void searchTextBox_TextChanged_1(object sender, EventArgs e)
        {
            String searchTerm = searchTextBox.Text.Trim();



            if (searchTerm.Equals("Search Item") || searchTerm.Equals(""))
            {
                return;
            }
            else
            {
                LoadGoods(searchTerm);
                LoadWaste(searchTerm);
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
                LoadGoods(searchTerm);
                LoadWaste(searchTerm);
            }
        }

        private void SelectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectTypeComboBox.Text == "GOODS")
            {
                flowLayoutPanelForGoodsCatagory.Visible = true;
                flowLayoutPanelForWasteCatagory.Visible = false;
            }

            else if (SelectTypeComboBox.Text == "WASTES")
            {
                flowLayoutPanelForGoodsCatagory.Visible = false;
                flowLayoutPanelForWasteCatagory.Visible = true;
            }
        }



        private void fruitsButton_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "FR");
            flowLayoutPanelForWeste.Visible = false;
            flowLayoutPanelForGoods.Visible = true;
            flowLayoutPanelForGoods.Dock = DockStyle.Fill;
        }

        private void vegeatablesButton_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "VE");
            flowLayoutPanelForWeste.Visible = false;
            flowLayoutPanelForGoods.Visible = true;
            flowLayoutPanelForGoods.Dock = DockStyle.Fill;
        }

        private void meatButton_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "ME");
            flowLayoutPanelForWeste.Visible = false;
            flowLayoutPanelForGoods.Visible = true;
            flowLayoutPanelForGoods.Dock = DockStyle.Fill;
        }

        private void fishButton_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "FI");
            flowLayoutPanelForWeste.Visible = false;
            flowLayoutPanelForGoods.Visible = true;
            flowLayoutPanelForGoods.Dock = DockStyle.Fill;
        }

        private void seedsButton_Click(object sender, EventArgs e)
        {
            LoadGoods(categoryPrefix: "SE");
            flowLayoutPanelForWeste.Visible = false;
            flowLayoutPanelForGoods.Visible = true;
            flowLayoutPanelForGoods.Dock = DockStyle.Fill;
        }

        

        private void cropReiduesButton_Click(object sender, EventArgs e)
        {
            LoadWaste(categoryPrefix: "CRO");
            flowLayoutPanelForGoods.Visible = false;
            flowLayoutPanelForWeste.Visible = true;
            flowLayoutPanelForWeste.Dock = DockStyle.Fill;
        }

        private void animalWasteButton_Click(object sender, EventArgs e)
        {
            LoadWaste(categoryPrefix: "ANI");
            flowLayoutPanelForGoods.Visible = false;
            flowLayoutPanelForWeste.Visible = true;
            flowLayoutPanelForWeste.Dock = DockStyle.Fill;
        }

        private void processingWasteButton_Click(object sender, EventArgs e)
        {
            LoadWaste(categoryPrefix: "PRO");
            flowLayoutPanelForGoods.Visible = false;
            flowLayoutPanelForWeste.Visible = true;
            flowLayoutPanelForWeste.Dock = DockStyle.Fill;
        }

        private void ForesteyWasteButton_Click(object sender, EventArgs e)
        {
            LoadWaste(categoryPrefix: "FOR");
            flowLayoutPanelForGoods.Visible = false;
            flowLayoutPanelForWeste.Visible = true;
            flowLayoutPanelForWeste.Dock = DockStyle.Fill;
        }

        private void foodWasteButton_Click(object sender, EventArgs e)
        {
            LoadWaste(categoryPrefix: "FOO");
            flowLayoutPanelForGoods.Visible = false;
            flowLayoutPanelForWeste.Visible = true;
            flowLayoutPanelForWeste.Dock = DockStyle.Fill;
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
            LoadGoods("");
            
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
            
            LoadWaste("");
            LoadGoods("");
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

        private void flowLayoutPanelForWeste_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
