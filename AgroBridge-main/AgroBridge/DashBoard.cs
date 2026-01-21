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
    public partial class DashBoard : Form
    {

        private string FarmerID;
        private string imagePath;
        public DashBoard(string farmerID)
        {
            InitializeComponent();
            this.FarmerID = farmerID;
            this.StartPosition = FormStartPosition.WindowsDefaultBounds;
        }

        private void AddEqipment_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 620);
            this.CenterToScreen();

            
        }

        private void flowLayoutPanelForProfile_Paint(object sender, PaintEventArgs e)
        {

        }

        private void richTextBoxForText_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
