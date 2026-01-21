using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace AgroBridge
{
    public partial class EquipmentAddClass: Form
    {

        private string FarmerID;
        private string imagePath;
        public EquipmentAddClass(string farmerID)
        {
            InitializeComponent();
            this.FarmerID = farmerID;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false; // Disable maximize button
        }

        private void EquipmentAddClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(820, 500);
            this.CenterToScreen();

            categoryComboBox.SelectedIndex = 0; // Reset combo box selection
            equipmentStatusComboBox.SelectedIndex = 0;
            addEqipmentButton.Enabled = false;
            //flowLayoutPanelForOperationalButton0.Size = new Size(172, 500);
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

        private void addButton_Click(object sender, EventArgs e)
        {
            // Database connection string
            string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE

            // Generate a unique EquipmentID
            string category = categoryComboBox.SelectedItem.ToString();
            string status = equipmentStatusComboBox.SelectedItem.ToString(); // Availability Status
            string categoryPrefix = category.Substring(0, 3).ToUpper();
            Random rnd = new Random();
            string equipmentID = $"{categoryPrefix}-{rnd.Next(100, 1000)}";
            int quantity = int.Parse(quantityTextBox1.Text);

            try
            {

                byte[] imageBytes = null;

                if (!string.IsNullOrEmpty(imagePath))
                {
                    imageBytes = File.ReadAllBytes(imagePath);
                }


                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Insert query
                    string query = "INSERT INTO Rent (EquipmentID, EquipmentName, EquipmentRentalPrice, EquipmentQuantity, EquipmentAvailabilityStatus, FarmerID,EqipmentImage ) " +
                                   "VALUES (@EquipmentID, @EquipmentName, @EquipmentRentalPrice, @EquipmentQuantity, @EquipmentAvailabilityStatus, @FarmerID, @EqipmentImage)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EquipmentID", equipmentID);
                        cmd.Parameters.AddWithValue("@EquipmentName", equipmentNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@EquipmentRentalPrice", decimal.Parse(rentalPriceTextBox.Text));
                        cmd.Parameters.AddWithValue("@EquipmentQuantity", quantity);
                        cmd.Parameters.AddWithValue("@EquipmentAvailabilityStatus", status);
                        cmd.Parameters.AddWithValue("@FarmerID", FarmerID);
                        cmd.Parameters.AddWithValue("@EqipmentImage", (object)imageBytes ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Equipment added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add the equipment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            equipmentNameTextBox.Text = string.Empty;
            rentalPriceTextBox.Text = string.Empty;
            quantityTextBox1.Text = string.Empty;
            categoryComboBox.SelectedIndex = 0; // Reset category combo box selection
            equipmentStatusComboBox.SelectedIndex = 0; // Reset status combo box selection
            imagePath = string.Empty; // Reset image path variable
        }

        private void AddGoodsButton_Click(object sender, EventArgs e)
        {
            ProductAdd productAdd = new ProductAdd(FarmerID);
            this.Close();
            productAdd.ShowDialog();
            
        }

        private void goToAddWasteClass_Click(object sender, EventArgs e)
        {
            WasteAddClass wasteAddClass = new WasteAddClass(FarmerID);
            this.Close();
            wasteAddClass.ShowDialog();
            
        }
    }
    
}
