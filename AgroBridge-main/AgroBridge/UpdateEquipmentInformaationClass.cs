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

    
    public partial class UpdateEquipmentInformaationClass : Form
    {

        private String ID;
        string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE
        private bool searchBoxClicked = false; // Track if search box was clicked
        public UpdateEquipmentInformaationClass(String ID)
        {
            InitializeComponent();
            this.ID = ID;
            this.StartPosition = FormStartPosition.WindowsDefaultBounds;
        }

        private void allItemButton_Click(object sender, EventArgs e)
        {
            HomePage homePage = new HomePage(ID);
            homePage.Show();
            this.Close();
        }

        private void UpdateEquipmentInformaationClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();

            currentOrderButton.Visible = true;
            updateEquipmentInformaationButton.ForeColor = Color.FromArgb(117, 55, 216);

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
            flowLayoutPanelForGoods.Controls.Clear();

            try
            {
                
                string query = "SELECT EquipmentID, EquipmentName, EquipmentRentalPrice, EqipmentImage, EquipmentQuantity, EquipmentAvailabilityStatus FROM Rent " +
                               "WHERE EquipmentName LIKE @SearchTerm AND FarmerID = @FarmerID";

                if (!string.IsNullOrEmpty(categoryPrefix))
                {
                    query += " AND EquipmentID LIKE @CategoryPrefix";
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
                        string goodsID = reader["EquipmentID"].ToString();
                        string goodsName = reader["EquipmentName"].ToString();
                        string goodsPrice = reader["EquipmentRentalPrice"].ToString();
                        byte[] goodsImageBytes = reader["EqipmentImage"] as byte[];
                        string equipmentAvailabilityStatus = reader["EquipmentAvailabilityStatus"].ToString();

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
                            itemPanel.Size = new Size(191, 300);
                        }
                        else
                        {
                            itemPanel.Size = new Size(205, 300);
                        }

                        // Add MouseEnter and MouseLeave events
                        itemPanel.MouseEnter += (s, e) =>
                        {
                            itemPanel.BackColor = Color.LightCyan; // Change to gray on mouse enter
                        };

                        itemPanel.MouseLeave += (s, e) =>
                        {
                            itemPanel.BackColor = Color.White; // Revert to original color on mouse leave
                        };

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

                        ComboBox statusComboBox = new ComboBox
                        {
                            Dock = DockStyle.Top,
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            Height = 30
                        };
                        statusComboBox.Items.Add("Available");
                        statusComboBox.Items.Add("Not Available");
                        statusComboBox.SelectedItem = equipmentAvailabilityStatus;

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
                            UpdateGoods(goodsID, nameTextBox.Text, priceTextBox.Text, statusComboBox.SelectedItem.ToString());
                        };

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

                        // Add controls to panel
                        itemPanel.Controls.Add(deleteButton);
                        itemPanel.Controls.Add(updateButton);
                        itemPanel.Controls.Add(statusComboBox);
                        itemPanel.Controls.Add(priceTextBox);
                        itemPanel.Controls.Add(nameTextBox);
                        itemPanel.Controls.Add(pictureBox);
                        itemPanel.Cursor = Cursors.Hand;

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
                
                string query = "DELETE FROM Rent WHERE EquipmentID = @EquipmentID AND FarmerID = @FarmerID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EquipmentID", goodsID);
                    command.Parameters.AddWithValue("@FarmerID", ID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Item deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        flowLayoutPanelForGoods.Controls.Remove(itemPanel); // Remove the item panel from the flowLayoutPanel
                    }
                    else
                    {
                        MessageBox.Show("Error deleting item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void UpdateGoods(string goodsID, string newName, string newPrice, string newStatus)
        {
            try
            {
                
                string query = "UPDATE Rent SET EquipmentName = @EquipmentName, EquipmentRentalPrice = @EquipmentRentalPrice, EquipmentAvailabilityStatus = @EquipmentAvailabilityStatus " +
                               "WHERE EquipmentID = @EquipmentID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EquipmentName", newName);
                    command.Parameters.AddWithValue("@EquipmentRentalPrice", newPrice);
                    command.Parameters.AddWithValue("@EquipmentAvailabilityStatus", newStatus);
                    command.Parameters.AddWithValue("@EquipmentID", goodsID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Item updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadEquipment(); // Refresh the goods list
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

        private void tillageButton_Click(object sender, EventArgs e)
        {
            LoadEquipment(categoryPrefix: "TIL");
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


        //All Button Functionality

        private void allItemButton_Click_1(object sender, EventArgs e)
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
            LoadEquipment("");
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

        private void DashboardButton_Click(object sender, EventArgs e)
        {
            DashBoardClass dashBoard = new DashBoardClass(ID);
            dashBoard.Show();
            this.Close();
        }

        private void flowLayoutPanelForGoods_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
