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
    public partial class OrdersCartClass: Form
    {

        private string ID;
        private string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE
        private double totalPrice;
        private string Seller;
        private bool searchBoxClicked = false; // Track if search box was clicked
        private string statusInfo;
        public OrdersCartClass(string ID, string seller= "", string statusInfo="")
        {
            InitializeComponent();
            this.Seller = seller;
            this.ID = ID;
            this.statusInfo = statusInfo;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.CenterToScreen();
            this.Size = new System.Drawing.Size(1080, 640);

            // Prevent resizing
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false; // Disable maximize button
            Seller = seller;

            // SearchBox Placeholder Events
            //this.searchTextBox.Enter += new System.EventHandler(this.searchTextBox_Enter);
            //this.searchTextBox.Leave += new System.EventHandler(this.searchTextBox_Leave);

        }

        private void OrdersCartClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();

            currentOrderButton.Visible = true;
            flowLayoutPanelForItemShown.Size = new Size(2750, 1467);
            addToCart.ForeColor = Color.FromArgb(117, 55, 216);

            flowLayoutPanelForToatalPrice.Visible = false;
            LoadOrders(); // Call LoadGoods to populate the FlowLayoutPanel
            PriceLabel.Text = totalPrice + ".00 TK".ToString();
            

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
            if (Seller.Equals(""))
            {
                currentOrderButton.Enabled = false;
            }
            
                

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

        private void LoadOrders(string searchTerm = "")
        {
            string customerID = "";
            string farmerID = "";
            List<Panel> panels = new List<Panel>(); // Store panels in memory first
            totalPrice = 0; // Initialize totalPrice at the start of the loop
            string query;

            if (Seller.Equals("Seller") && statusInfo.Equals(""))
            {
                farmerID = ID;
                query = @"
                    SELECT 
                        G.GoodsID AS ProductID, 
                        G.GoodsName AS ProductName, 
                        G.GoodsImage AS Image, 
                        G.GoodsPrice AS Price, 
                        G.Units AS Unit, 
                        BU.Quantity AS Quantity, 
                        (G.GoodsPrice * BU.Quantity) AS TotalPrice,
                        BU.BuyOrderID AS OrderID,
                        BU.Status AS Status,
                        BU.PaymentMethod AS PaymentMethod
                    FROM AgroBridge.dbo.Goods G
                    JOIN AgroBridge.dbo.Buy BU ON G.GoodsID = BU.GoodsID
                    WHERE BU.FarmerID = @FarmerID
                    AND BU.Status NOT IN ('Completed')
                    AND G.GoodsName LIKE '%' + @SearchTerm + '%'

                    UNION ALL

                    SELECT 
                        W.WasteID AS ProductID, 
                        W.WasteName AS ProductName, 
                        W.WasteImage AS Image, 
                        W.WastePrice AS Price, 
                        W.Units AS Unit, 
                        P.Quantity AS Quantity,  
                        (W.WastePrice * P.Quantity) AS TotalPrice,
                        P.PurchaseOrderID AS OrderID,
                        P.Status AS Status,
                        P.PaymentMethod AS PaymentMethod
                    FROM AgroBridge.dbo.Waste W
                    JOIN AgroBridge.dbo.Purchase P ON W.WasteID = P.WasteID
                    WHERE P.FarmerID = @FarmerID
                    AND P.Status NOT IN ('Completed')
                    AND W.WasteName LIKE '%' + @SearchTerm + '%'

                    UNION ALL

                    SELECT 
                        R.EquipmentID AS ProductID, 
                        R.EquipmentName AS ProductName, 
                        R.EqipmentImage AS Image, 
                        R.EquipmentRentalPrice AS Price, 
                        'Rental' AS Unit, 
                        RE.Quantity AS Quantity,  
                        (R.EquipmentRentalPrice * RE.Quantity) AS TotalPrice,
                        RE.EquipmentOrderID AS OrderID,
                        RE.Status AS Status,
                        RE.PaymentMethod AS PaymentMethod
                    FROM AgroBridge.dbo.Rent R
                    JOIN AgroBridge.dbo.Rental RE ON R.EquipmentID = RE.EquipmentID
                    WHERE RE.FarmerID = @FarmerID
                    AND RE.Status NOT IN ('Completed')
                    AND R.EquipmentName LIKE '%' + @SearchTerm + '%';";
            }

            else if (Seller.Equals("Buyers") && statusInfo.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                customerID = ID;
                query = @"
            SELECT G.GoodsID AS ProductID, 
               G.GoodsName AS ProductName, 
               G.GoodsImage AS Image, 
               G.GoodsPrice AS Price, 
               G.Units AS Unit, 
               BU.Quantity AS Quantity, 
               (G.GoodsPrice * BU.Quantity) AS TotalPrice,
               BU.BuyOrderID AS OrderID,
               BU.Status AS Status,
               BU.PaymentMethod AS PaymentMethod
        FROM AgroBridge.dbo.Goods G
        JOIN AgroBridge.dbo.Buy BU ON G.GoodsID = BU.GoodsID
        WHERE BU.CustomerID = @CustomerID
        AND BU.Status = @Status
        AND G.GoodsName LIKE '%' + @SearchTerm + '%'
        
        UNION ALL
        
        SELECT W.WasteID AS ProductID, 
               W.WasteName AS ProductName, 
               W.WasteImage AS Image, 
               W.WastePrice AS Price, 
               W.Units AS Unit, 
               P.Quantity AS Quantity,  
               (W.WastePrice * P.Quantity) AS TotalPrice,
               P.PurchaseOrderID AS OrderID,
               P.Status AS Status,
               P.PaymentMethod AS PaymentMethod
        FROM AgroBridge.dbo.Waste W
        JOIN AgroBridge.dbo.Purchase P ON W.WasteID = P.WasteID
        WHERE P.CustomerID = @CustomerID
        AND P.Status = @Status
        AND W.WasteName LIKE '%' + @SearchTerm + '%'
        
        UNION ALL
        
        SELECT R.EquipmentID AS ProductID, 
               R.EquipmentName AS ProductName, 
               R.EqipmentImage AS Image, 
               R.EquipmentRentalPrice AS Price, 
               'Rental' AS Unit, 
               RE.Quantity AS Quantity,  
               (R.EquipmentRentalPrice * RE.Quantity) AS TotalPrice,
               RE.EquipmentOrderID AS OrderID,
               RE.Status AS Status,
               RE.PaymentMethod AS PaymentMethod
        FROM AgroBridge.dbo.Rent R
        JOIN AgroBridge.dbo.Rental RE ON R.EquipmentID = RE.EquipmentID
        WHERE RE.CustomerID = @CustomerID
        AND RE.Status = @Status
        AND R.EquipmentName LIKE '%' + @SearchTerm + '%';";
            }

            else if (Seller.Equals("Seller") && statusInfo.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
            {
                farmerID = ID;
                query = @"
SELECT G.GoodsID AS ProductID, 
       G.GoodsName AS ProductName, 
       G.GoodsImage AS Image, 
       G.GoodsPrice AS Price, 
       G.Units AS Unit, 
       BU.Quantity AS Quantity, 
       (G.GoodsPrice * BU.Quantity) AS TotalPrice,
       BU.BuyOrderID AS OrderID,
       BU.Status AS Status,
       BU.PaymentMethod AS PaymentMethod
FROM AgroBridge.dbo.Goods G
JOIN AgroBridge.dbo.Buy BU ON G.GoodsID = BU.GoodsID
WHERE BU.FarmerID = @FarmerID
AND BU.Status = @Status
AND G.GoodsName LIKE '%' + @SearchTerm + '%'

UNION ALL

SELECT W.WasteID AS ProductID, 
       W.WasteName AS ProductName, 
       W.WasteImage AS Image, 
       W.WastePrice AS Price, 
       W.Units AS Unit, 
       P.Quantity AS Quantity,  
       (W.WastePrice * P.Quantity) AS TotalPrice,
       P.PurchaseOrderID AS OrderID,
       P.Status AS Status,
       P.PaymentMethod AS PaymentMethod
FROM AgroBridge.dbo.Waste W
JOIN AgroBridge.dbo.Purchase P ON W.WasteID = P.WasteID
WHERE P.FarmerID = @FarmerID
AND P.Status = @Status
AND W.WasteName LIKE '%' + @SearchTerm + '%'

UNION ALL

SELECT R.EquipmentID AS ProductID, 
       R.EquipmentName AS ProductName, 
       R.EqipmentImage AS Image, 
       R.EquipmentRentalPrice AS Price, 
       'Rental' AS Unit, 
       RE.Quantity AS Quantity,  
       (R.EquipmentRentalPrice * RE.Quantity) AS TotalPrice,
       RE.EquipmentOrderID AS OrderID,
       RE.Status AS Status,
       RE.PaymentMethod AS PaymentMethod
FROM AgroBridge.dbo.Rent R
JOIN AgroBridge.dbo.Rental RE ON R.EquipmentID = RE.EquipmentID
WHERE RE.FarmerID = @FarmerID
AND RE.Status = @Status
AND R.EquipmentName LIKE '%' + @SearchTerm + '%';";
            }

            else if (Seller.Equals("Seller") && statusInfo.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                farmerID = ID;
                query = @"
SELECT G.GoodsID AS ProductID, 
       G.GoodsName AS ProductName, 
       G.GoodsImage AS Image, 
       G.GoodsPrice AS Price, 
       G.Units AS Unit, 
       BU.Quantity AS Quantity, 
       (G.GoodsPrice * BU.Quantity) AS TotalPrice,
       BU.BuyOrderID AS OrderID,
       BU.Status AS Status,
       BU.PaymentMethod AS PaymentMethod
FROM AgroBridge.dbo.Goods G
JOIN AgroBridge.dbo.Buy BU ON G.GoodsID = BU.GoodsID
WHERE BU.FarmerID = @FarmerID
AND BU.Status = @Status
AND G.GoodsName LIKE '%' + @SearchTerm + '%'

UNION ALL

SELECT W.WasteID AS ProductID, 
       W.WasteName AS ProductName, 
       W.WasteImage AS Image, 
       W.WastePrice AS Price, 
       W.Units AS Unit, 
       P.Quantity AS Quantity,  
       (W.WastePrice * P.Quantity) AS TotalPrice,
       P.PurchaseOrderID AS OrderID,
       P.Status AS Status,
       P.PaymentMethod AS PaymentMethod
FROM AgroBridge.dbo.Waste W
JOIN AgroBridge.dbo.Purchase P ON W.WasteID = P.WasteID
WHERE P.FarmerID = @FarmerID
AND P.Status = @Status
AND W.WasteName LIKE '%' + @SearchTerm + '%'

UNION ALL

SELECT R.EquipmentID AS ProductID, 
       R.EquipmentName AS ProductName, 
       R.EqipmentImage AS Image, 
       R.EquipmentRentalPrice AS Price, 
       'Rental' AS Unit, 
       RE.Quantity AS Quantity,  
       (R.EquipmentRentalPrice * RE.Quantity) AS TotalPrice,
       RE.EquipmentOrderID AS OrderID,
       RE.Status AS Status,
       RE.PaymentMethod AS PaymentMethod
FROM AgroBridge.dbo.Rent R
JOIN AgroBridge.dbo.Rental RE ON R.EquipmentID = RE.EquipmentID
WHERE RE.FarmerID = @FarmerID
AND RE.Status = @Status
AND R.EquipmentName LIKE '%' + @SearchTerm + '%';";
            }

            else if (Seller.Equals("Sellers") && statusInfo.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                farmerID = ID;
                query = @"
SELECT G.GoodsID AS ProductID, 
       G.GoodsName AS ProductName, 
       G.GoodsImage AS Image, 
       G.GoodsPrice AS Price, 
       G.Units AS Unit, 
       BU.Quantity AS Quantity, 
       (G.GoodsPrice * BU.Quantity) AS TotalPrice,
       BU.BuyOrderID AS OrderID,
       BU.Status AS Status,
       BU.PaymentMethod AS PaymentMethod
FROM AgroBridge.dbo.Goods G
JOIN AgroBridge.dbo.Buy BU ON G.GoodsID = BU.GoodsID
WHERE BU.FarmerID = @FarmerID
AND BU.Status = @Status
AND G.GoodsName LIKE '%' + @SearchTerm + '%'

UNION ALL

SELECT W.WasteID AS ProductID, 
       W.WasteName AS ProductName, 
       W.WasteImage AS Image, 
       W.WastePrice AS Price, 
       W.Units AS Unit, 
       P.Quantity AS Quantity,  
       (W.WastePrice * P.Quantity) AS TotalPrice,
       P.PurchaseOrderID AS OrderID,
       P.Status AS Status,
       P.PaymentMethod AS PaymentMethod
FROM AgroBridge.dbo.Waste W
JOIN AgroBridge.dbo.Purchase P ON W.WasteID = P.WasteID
WHERE P.FarmerID = @FarmerID
AND P.Status = @Status
AND W.WasteName LIKE '%' + @SearchTerm + '%'

UNION ALL

SELECT R.EquipmentID AS ProductID, 
       R.EquipmentName AS ProductName, 
       R.EqipmentImage AS Image, 
       R.EquipmentRentalPrice AS Price, 
       'Rental' AS Unit, 
       RE.Quantity AS Quantity,  
       (R.EquipmentRentalPrice * RE.Quantity) AS TotalPrice,
       RE.EquipmentOrderID AS OrderID,
       RE.Status AS Status,
       RE.PaymentMethod AS PaymentMethod
FROM AgroBridge.dbo.Rent R
JOIN AgroBridge.dbo.Rental RE ON R.EquipmentID = RE.EquipmentID
WHERE RE.FarmerID = @FarmerID
AND RE.Status = @Status
AND R.EquipmentName LIKE '%' + @SearchTerm + '%';";
            }

            else
            {
                customerID = ID;
                query = @"
        SELECT G.GoodsID AS ProductID, 
               G.GoodsName AS ProductName, 
               G.GoodsImage AS Image, 
               G.GoodsPrice AS Price, 
               G.Units AS Unit, 
               BU.Quantity AS Quantity, 
               (G.GoodsPrice * BU.Quantity) AS TotalPrice,
               BU.BuyOrderID AS OrderID,
               BU.Status AS Status,
               BU.PaymentMethod AS PaymentMethod
        FROM AgroBridge.dbo.Goods G
        JOIN AgroBridge.dbo.Buy BU ON G.GoodsID = BU.GoodsID
        WHERE BU.CustomerID = @CustomerID
        AND BU.Status NOT IN ('Completed')
        AND G.GoodsName LIKE '%' + @SearchTerm + '%'
        
        UNION ALL
        
        SELECT W.WasteID AS ProductID, 
               W.WasteName AS ProductName, 
               W.WasteImage AS Image, 
               W.WastePrice AS Price, 
               W.Units AS Unit, 
               P.Quantity AS Quantity,  
               (W.WastePrice * P.Quantity) AS TotalPrice,
               P.PurchaseOrderID AS OrderID,
               P.Status AS Status,
               P.PaymentMethod AS PaymentMethod
        FROM AgroBridge.dbo.Waste W
        JOIN AgroBridge.dbo.Purchase P ON W.WasteID = P.WasteID
        WHERE P.CustomerID = @CustomerID
        AND P.Status NOT IN ('Completed')
        AND W.WasteName LIKE '%' + @SearchTerm + '%'
        
        UNION ALL
        
        SELECT R.EquipmentID AS ProductID, 
               R.EquipmentName AS ProductName, 
               R.EqipmentImage AS Image, 
               R.EquipmentRentalPrice AS Price, 
               'Rental' AS Unit, 
               RE.Quantity AS Quantity,  
               (R.EquipmentRentalPrice * RE.Quantity) AS TotalPrice,
               RE.EquipmentOrderID AS OrderID,
               RE.Status AS Status,
               RE.PaymentMethod AS PaymentMethod
        FROM AgroBridge.dbo.Rent R
        JOIN AgroBridge.dbo.Rental RE ON R.EquipmentID = RE.EquipmentID
        WHERE RE.CustomerID = @CustomerID
        AND RE.Status NOT IN ('Completed')
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
                    if (Seller.Equals("") && statusInfo.Equals(""))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                    }

                    else if (Seller.Equals("Buyers") && statusInfo.Equals("Completed"))
                    {
                        
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.Parameters.AddWithValue("@Status", statusInfo);
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                    }


                    else if(Seller.Equals("Seller") && statusInfo.Equals(""))
                    {
                        cmd.Parameters.AddWithValue("@FarmerID", farmerID);
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                    }

                    else if (Seller.Equals("Seller") && statusInfo.Equals("Delivered"))
                    {
                        cmd.Parameters.AddWithValue("@FarmerID", farmerID);
                        cmd.Parameters.AddWithValue("@Status", statusInfo);
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                    }

                    else if (Seller.Equals("Seller") && statusInfo.Equals("Pending"))
                    {
                        cmd.Parameters.AddWithValue("@FarmerID", farmerID);
                        cmd.Parameters.AddWithValue("@Status", statusInfo);
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                    }

                    else if (Seller.Equals("Sellers") && statusInfo.Equals("Completed"))
                    {
                        cmd.Parameters.AddWithValue("@FarmerID", farmerID);
                        cmd.Parameters.AddWithValue("@Status", statusInfo);
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                    }

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows) return; // Prevent access if no data exists

                        while (reader.Read())
                        {
                            if (reader.IsClosed) break; // Ensure reader is open

                            string productName = reader["ProductName"].ToString();
                            double price = Convert.ToDouble(reader["Price"]);
                            int quantity = Convert.ToInt32(reader["Quantity"]);
                            string status = reader["Status"].ToString();
                            string paymentMethod = reader["PaymentMethod"].ToString();
                            string productID = reader["ProductID"].ToString();
                            string totalPriceText = reader["TotalPrice"].ToString() + " ৳";
                            string orderID = reader["OrderID"].ToString();
                            double itemTotalPrice = Convert.ToDouble(reader["TotalPrice"]);
                            
                            totalPrice += itemTotalPrice;


                            Panel panel = new Panel
                            {
                                Size = new Size(865, 60),
                                BorderStyle = BorderStyle.FixedSingle,
                                BackColor = Color.FromArgb(242, 239, 231)
                            };

                            Label nameLabel = new Label
                            {
                                Text = productName,
                                Location = new Point(10, 10),
                                Font = new Font("Arial", 9),
                                ForeColor = Color.Black,
                                AutoSize = false,
                                Size = new Size(100, 30),
                                TextAlign = ContentAlignment.MiddleCenter,
                            };

                            PictureBox pictureBox = new PictureBox
                            {
                                Size = new Size(50, 50),
                                Location = new Point(160, 5),
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
                                Text = price.ToString() + " ৳",
                                Location = new Point(250, 10),
                                Font = new Font("Arial", 10),
                                ForeColor = Color.Black,
                                AutoSize = false,
                                Size = new Size(50, 30),
                                TextAlign = ContentAlignment.MiddleCenter,
                            };

                            Label unitLabel = new Label
                            {
                                Text = reader["Unit"].ToString(),
                                Location = new Point(315, 10),
                                Font = new Font("Arial", 10),
                                ForeColor = Color.Black,
                                AutoSize = false,
                                Size = new Size(50, 30),
                                TextAlign = ContentAlignment.MiddleCenter,
                            };

                            Label quantityLabel = new Label
                            {
                                Text = quantity.ToString(),
                                Location = new Point(360, 10),
                                Font = new Font("Arial", 10),
                                ForeColor = Color.Black,
                                AutoSize = false,
                                Size = new Size(50, 30),
                                TextAlign = ContentAlignment.MiddleCenter,
                            };

                            Label totalPriceLabel = new Label
                            {
                                Text = totalPriceText,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Location = new Point(420, 10),
                                Font = new Font("Arial", 10),
                                ForeColor = Color.Black,
                                AutoSize = false,
                                Size = new Size(50, 30)
                            };

                            Label orderIDLabel = new Label
                            {
                                Text = orderID,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Location = new Point(500, 10),
                                Font = new Font("Arial", 8),
                                ForeColor = Color.Black,
                                AutoSize = false,
                                Size = new Size(100, 30)
                            };

                            ComboBox statusComboBox = null;

                            if (Seller.Equals("Seller"))
                            {
                                statusComboBox = new ComboBox
                                {
                                    Location = new Point(635, 10),
                                    Font = new Font("Arial", 10),
                                    ForeColor = GetStatusColor(reader["Status"].ToString()), // Apply color
                                    Size = new Size(80, 30),
                                    DropDownStyle = ComboBoxStyle.DropDownList
                                };

                                statusComboBox.Items.AddRange(new string[] { "Pending", "Dispatched", "Delivered", "Completed" });
                                statusComboBox.SelectedItem = status;

                                UpdateStatusColor(statusComboBox);

                                statusComboBox.SelectedIndexChanged += (s, e) =>
                                {
                                    UpdateStatusColor(statusComboBox);
                                    UpdateOrderStatus(orderID, statusComboBox.SelectedItem.ToString());
                                };


                                panel.Controls.Add(statusComboBox);
                            }
                            else
                            {
                                Label statusLabel = new Label
                                {
                                    Text = status,
                                    TextAlign = ContentAlignment.MiddleCenter,
                                    Location = new Point(635, 10),
                                    Font = new Font("Arial", 8),
                                    ForeColor = GetStatusColor(reader["Status"].ToString()), // Apply color
                                    AutoSize = false,
                                    Size = new Size(70, 30),
                                };

                                UpdateStatusColor(statusLabel);  // Apply color and bold font
                                panel.Controls.Add(statusLabel);

                            }

                            Label payMethodLabel = new Label
                            {
                                Text = paymentMethod,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Location = new Point(730, 10),
                                Font = new Font("Arial", 9),
                                ForeColor = Color.Black,
                                AutoSize = false,
                                Size = new Size(55, 30)
                            };

                            Button deleteButton = new Button
                            {
                                Text = " X ",
                                Size = new Size(30, 30),
                                Font = new Font("Arial", 10, FontStyle.Bold),
                                Location = new Point(800, 15),
                                ForeColor = Color.Red
                            };

                            if (status == "Delivered" || status == "Completed")
                            {
                                deleteButton.Enabled = false; // Disable the delete button
                                deleteButton.Cursor = Cursors.Default; // Default cursor when disabled
                            }
                            else
                            {
                                deleteButton.Enabled = true; // Enable the delete button
                                deleteButton.Cursor = Cursors.Hand; // Hand cursor when enabled
                            }

                            deleteButton.Click += (s, e) =>
                            {
                                // Check the status of the order
                                if (status == "Delivered" || status == "Completed")
                                {
                                    MessageBox.Show("Cannot delete a product that is delivered or completed.");
                                    return;
                                }

                                // Call the function to delete the product
                                DeleteOrder(productID, orderID);
                                // Optionally, remove the panel from the UI
                                flowLayoutPanelForItemShown.Controls.Remove(panel);
                            };



                            panel.Controls.Add(nameLabel);
                            panel.Controls.Add(pictureBox);
                            panel.Controls.Add(priceLabel);
                            panel.Controls.Add(unitLabel);
                            panel.Controls.Add(quantityLabel);
                            panel.Controls.Add(totalPriceLabel);
                            panel.Controls.Add(orderIDLabel);
                            panel.Controls.Add(payMethodLabel);
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
                flowLayoutPanelForToatalPrice.Visible = true;


            }
            else
            {
                emptyPanel.Visible = true;

            }
        }


        private void DeleteOrder(string productID, string orderID)
        {
            string query = @"
                        IF EXISTS (SELECT 1 FROM AgroBridge.dbo.Buy WHERE GoodsID = @ProductID)
                        BEGIN
                            DELETE FROM AgroBridge.dbo.Buy WHERE GoodsID = @ProductID;
                        END
                        ELSE IF EXISTS (SELECT 1 FROM AgroBridge.dbo.Purchase WHERE WasteID = @ProductID)
                        BEGIN
                            DELETE FROM AgroBridge.dbo.Purchase WHERE WasteID = @ProductID;
                        END
                        ELSE IF EXISTS (SELECT 1 FROM AgroBridge.dbo.Rental WHERE EquipmentID = @ProductID)
                        BEGIN
                            DELETE FROM AgroBridge.dbo.Rental WHERE EquipmentID = @ProductID;
                        END";

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
                    }
                    else
                    {
                        MessageBox.Show("Product ID not found.");
                    }
                }
            }
        }


        private void UpdateOrderStatus(string orderID, string newStatus)
        {
            string query = @"
                        IF EXISTS (SELECT 1 FROM AgroBridge.dbo.Buy WHERE BuyOrderID = @OrderID)
                        BEGIN
                            UPDATE AgroBridge.dbo.Buy SET Status = @Status WHERE BuyOrderID = @OrderID;
                        END
                        ELSE IF EXISTS (SELECT 1 FROM AgroBridge.dbo.Purchase WHERE PurchaseOrderID = @OrderID)
                        BEGIN
                            UPDATE AgroBridge.dbo.Purchase SET Status = @Status WHERE PurchaseOrderID = @OrderID;
                        END
                        ELSE IF EXISTS (SELECT 1 FROM AgroBridge.dbo.Rental WHERE EquipmentOrderID = @OrderID)
                        BEGIN
                            UPDATE AgroBridge.dbo.Rental SET Status = @Status WHERE EquipmentOrderID = @OrderID;
                        END";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    cmd.Parameters.AddWithValue("@OrderID", orderID);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Order status updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Order ID not found.");
                    }
                }
            }
        }



        private void UpdateStatusColor(ComboBox comboBox)
        {
            Color statusColor = GetStatusColor(comboBox.SelectedItem.ToString());
            comboBox.ForeColor = statusColor;
            comboBox.Font = new Font(comboBox.Font, FontStyle.Bold);  // Set font to bold
        }

        private void UpdateStatusColor(Label statusLabel)
        {
            Color statusColor = GetStatusColor(statusLabel.Text);
            statusLabel.ForeColor = statusColor;
            statusLabel.Font = new Font(statusLabel.Font, FontStyle.Bold);  // Set font to bold
        }


        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "Dispatched":
                    return Color.Blue;
                case "Delivered":
                    return Color.Green;
                case "Completed":
                    return Color.Gray;
                case "Pending":
                    return Color.Orange;
                default:
                    return Color.White;
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
                LoadOrders();
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm) || !searchTerm.Equals(""))
            {
                LoadOrders(searchTerm);
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
                LoadOrders(searchTerm);
            }
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

        private void buttonForContinueShopping_Click(object sender, EventArgs e)
        {
            HomePage homePage = new HomePage(ID);
            homePage.Show();
            this.Close();
        }

        private void DashboardButton_Click(object sender, EventArgs e)
        {
            DashBoardClass dashBoard = new DashBoardClass(ID);
            dashBoard.Show();
            this.Close();
        }

        private void currentOrderButton_Click(object sender, EventArgs e)
        {
            if (!Seller.Equals(""))
            {
                OrdersCartClass ordersCartClass = new OrdersCartClass(ID);
                ordersCartClass.Show();
                this.Close();
            }

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
