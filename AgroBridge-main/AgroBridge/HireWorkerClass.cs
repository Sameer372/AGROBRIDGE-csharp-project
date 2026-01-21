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
    public partial class HireWorkerClass : Form
    {
        String ID;
        string connectionString = "Data Source=LAPTOP-RN9LA27H\\SQLEXPRESS;Initial Catalog=AgroBridge;Integrated Security=True;"; //UPDATE
        public HireWorkerClass(String ID)
        {
            InitializeComponent();
            this.ID = ID;
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            LoginClass loginClass = new LoginClass();
            loginClass.Show();
            this.Close();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            HomePage homePage = new HomePage(ID);
            homePage.Show();
            this.Close();
        }

        private void displayDonationFlowLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void HireWorkerClass_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();

           
            flowLayoutPanelForOperationalButton0.Size = new Size(45, 1641);
            flowLayoutPanelForOperationalButton1.AutoScroll = false;
            flowLayoutPanelForOperationalButton2.AutoScroll = false;
            hireWorkerButton.Enabled = false;
            LoadWorkerInformation(); // Load data dynamically


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
                
            }
            else if (ID.StartsWith("WOR"))
            {
                applyForDonateButton.Visible = false;
                addGoodsButton.Visible = false;
                updateAndDeleteButton.Visible = false;
                hireWorkerButton.Visible = false;
                updateEquipmentInformaationButton.Visible = false;
                DonateButton.Visible = false;
                
            }

            else if (ID.StartsWith("Admin"))
            {
                applyForDonateButton.Visible = false;
                addGoodsButton.Visible = false;
                updateAndDeleteButton.Visible = false;
                hireWorkerButton.Visible = true;
                updateEquipmentInformaationButton.Visible = false;
                DonateButton.Visible = false;
                workingProfileButton.Visible = false;
                
            }
        }

        private void LoadWorkerInformation()
        {
            displayDonationFlowLayoutPanel.Controls.Clear(); // Clear any existing controls

            try
            {
                
                string query = "SELECT WorkerID, WorkerName, Phone, Status, Skills FROM WorkerInformations";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string workerID = reader["WorkerID"].ToString();
                        string workerName = reader["WorkerName"].ToString();
                        string phone = reader["Phone"].ToString();
                        string status = reader["Status"].ToString();
                        string skills = reader["Skills"].ToString();

                        // Create a panel for each worker
                        Panel workerPanel = new Panel
                        {
                            Size = new Size(350, 200),
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.FromArgb(242, 239, 231),
                            Padding = new Padding(5),
                            
                        };

                        // Adjust panel size dynamically
                        flowLayoutPanelForOperationalButton0.Refresh();

                        if (flowLayoutPanelForOperationalButton0.Width <= 45)
                        {
                            workerPanel.Size = new Size(330, 200);
                        }
                        else
                        {
                            workerPanel.Size = new Size(212, 200);
                        }

                        Label workerIDLabel = new Label
                        {
                            Text = $"Worker ID: {workerID}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Bold),
                            Location = new Point(10, 10),
                            Size = new Size(480, 20)
                        };

                        Label workerNameLabel = new Label
                        {
                            Text = $"Name: {workerName}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Regular),
                            Location = new Point(10, 40),
                            Size = new Size(480, 20)
                        };

                        Label phoneLabel = new Label
                        {
                            Text = $"Phone: {phone}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Regular),
                            Location = new Point(10, 70),
                            Size = new Size(480, 20)
                        };

                        Label statusLabel = new Label
                        {
                            Text = $"Status: {status}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Regular),
                            Location = new Point(10, 100),
                            Size = new Size(480, 20)
                        };

                        Label skillsLabel = new Label
                        {
                            Text = $"Skills: {skills}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("Arial", 10, FontStyle.Regular),
                            Location = new Point(10, 130),
                            Size = new Size(480, 20),
                            ForeColor = Color.Navy
                        };

                        if (status.Equals("Available"))
                        {
                            Button hireButton = new Button
                            {
                                Text = "Hire",
                                Location = new Point(110, 160),
                                Size = new Size(100, 30),
                                BackColor = Color.LightBlue
                            };

                            // Dynamically center the button inside the panel
                            hireButton.Location = new Point((workerPanel.Width - hireButton.Width) / 2, 160);

                            hireButton.Click += (s, args) =>
                            {
                                if (status.Equals("Available", StringComparison.OrdinalIgnoreCase))
                                {
                                    MessageBox.Show($"Worker {workerName} hired successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show($"Worker {workerName} is unavailable.", "Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            };
                            workerPanel.Controls.Add(hireButton);

                            hireButton.Cursor = Cursors.Hand;
                            statusLabel.ForeColor = Color.Green;
                        }

                        else
                        {
                            statusLabel.ForeColor = Color.Red;
                        }





                            // Add controls to the panel
                            workerPanel.Controls.Add(workerIDLabel);
                        workerPanel.Controls.Add(workerNameLabel);
                        workerPanel.Controls.Add(phoneLabel);
                        workerPanel.Controls.Add(statusLabel);
                        workerPanel.Controls.Add(skillsLabel);
                        

                        // Add the panel to the FlowLayoutPanel
                        displayDonationFlowLayoutPanel.Controls.Add(workerPanel);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading worker information: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //For All Button Functionlity
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
            LoadWorkerInformation();
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

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
