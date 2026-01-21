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
using System.IO;

namespace AgroBridge
{
    public partial class ProductAdd : Form
    {
        private string FarmerID;
        private string imagePath;
        public ProductAdd()
        {
            InitializeComponent();
            
        }

        public ProductAdd(String ID)
        {
            InitializeComponent();
            this.FarmerID = ID;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false; // Disable maximize button
        }
        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            // Open File Dialog to browse and select an image
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                imagePath = ofd.FileName;

                // Display the selected image in PictureBox with zoom
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = Image.FromFile(imagePath);
            }
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // i want to show the browse image here
        }

        private void goodsNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void goodsPriceTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void categoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // Database connection string
            string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

            // Generate a unique GoodsID
            string category = categoryComboBox.SelectedItem.ToString();
            string categoryPrefix = category.Substring(0, 2).ToUpper();
            Random rnd = new Random();
            string goodsID = $"{categoryPrefix}-{rnd.Next(100, 1000)}";
            int Quantity = int.Parse(quantityTextBox1.Text);
            string units = UnitComboBox.Text;

            try
            {
                // Read the image as byte array for storing in the database
                byte[] imageBytes = null;

                if (!string.IsNullOrEmpty(imagePath))
                {
                    imageBytes = File.ReadAllBytes(imagePath);
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Insert query
                    string query = "INSERT INTO Goods (GoodsID, GoodsName, GoodsPrice, GoodsQuantity, GoodsImage, FarmerID, Units) " +
                                   "VALUES (@GoodsID, @GoodsName, @GoodsPrice, @GoodsQuantity, @GoodsImage, @FarmerID, @Units)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@GoodsID", goodsID);
                        cmd.Parameters.AddWithValue("@GoodsName", goodsNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@GoodsPrice", int.Parse(goodsPriceTextBox.Text));
                        cmd.Parameters.AddWithValue("@GoodsQuantity", Quantity);
                        cmd.Parameters.AddWithValue("@GoodsImage", (object)imageBytes ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FarmerID", FarmerID);
                        cmd.Parameters.AddWithValue("@Units", units);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add the product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ClearFields()
        {
            goodsNameTextBox.Text = string.Empty;
            goodsPriceTextBox.Text = string.Empty;
            categoryComboBox.SelectedIndex = 1; // Reset combo box selection
            pictureBox1.Image = null; // Clear the image in PictureBox
            imagePath = string.Empty; // Reset the image path variable
            quantityTextBox1.Text = string.Empty;
            UnitComboBox.SelectedIndex = 1;
        }

        private void ProductAdd_Load(object sender, EventArgs e)
        {
            this.Size = new Size(820, 500);
            this.CenterToScreen();

            categoryComboBox.SelectedIndex = 0; // Reset combo box selection
            UnitComboBox.SelectedIndex = 0;
            AddGoodsButton.Enabled = false;
        }

        private void goToAddWasteClass_Click(object sender, EventArgs e)
        {
            
            WasteAddClass wasteAddClass = new WasteAddClass(FarmerID);
            wasteAddClass.ShowDialog();
            this.Close(); // Close after the dialog is closed
        }

        

        private void addEqipmentButton_Click(object sender, EventArgs e)
        {
            
            EquipmentAddClass addEqipment = new EquipmentAddClass(FarmerID);
            addEqipment.ShowDialog();
            this.Close(); // Close after the dialog is closed
        }

        private void UnitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
