namespace AgroBridge
{
    partial class RegInfoClass
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegInfoClass));
            this.UpperPannel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.UpperTextPannel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.BottomPannel = new System.Windows.Forms.Panel();
            this.logoutButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.MiddelPannel = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.backButton = new System.Windows.Forms.Button();
            this.UpperPannel.SuspendLayout();
            this.UpperTextPannel.SuspendLayout();
            this.BottomPannel.SuspendLayout();
            this.MiddelPannel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // UpperPannel
            // 
            this.UpperPannel.BackColor = System.Drawing.Color.LightCyan;
            this.UpperPannel.Controls.Add(this.button1);
            this.UpperPannel.Dock = System.Windows.Forms.DockStyle.Top;
            this.UpperPannel.Location = new System.Drawing.Point(0, 0);
            this.UpperPannel.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.UpperPannel.Name = "UpperPannel";
            this.UpperPannel.Size = new System.Drawing.Size(3334, 77);
            this.UpperPannel.TabIndex = 0;
            this.UpperPannel.Paint += new System.Windows.Forms.PaintEventHandler(this.UpperPannel_Paint);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Red;
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button1.Location = new System.Drawing.Point(3255, 0);
            this.button1.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 77);
            this.button1.TabIndex = 0;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // UpperTextPannel
            // 
            this.UpperTextPannel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.UpperTextPannel.Controls.Add(this.label1);
            this.UpperTextPannel.Dock = System.Windows.Forms.DockStyle.Top;
            this.UpperTextPannel.Location = new System.Drawing.Point(0, 77);
            this.UpperTextPannel.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.UpperTextPannel.Name = "UpperTextPannel";
            this.UpperTextPannel.Size = new System.Drawing.Size(3334, 293);
            this.UpperTextPannel.TabIndex = 1;
            this.UpperTextPannel.Paint += new System.Windows.Forms.PaintEventHandler(this.UpperTextPannel_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(1320, 71);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(573, 135);
            this.label1.TabIndex = 0;
            this.label1.Text = "UserData";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // BottomPannel
            // 
            this.BottomPannel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.BottomPannel.Controls.Add(this.backButton);
            this.BottomPannel.Controls.Add(this.logoutButton);
            this.BottomPannel.Controls.Add(this.DeleteButton);
            this.BottomPannel.Controls.Add(this.AddButton);
            this.BottomPannel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomPannel.Location = new System.Drawing.Point(0, 1438);
            this.BottomPannel.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.BottomPannel.Name = "BottomPannel";
            this.BottomPannel.Size = new System.Drawing.Size(3334, 273);
            this.BottomPannel.TabIndex = 2;
            // 
            // logoutButton
            // 
            this.logoutButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logoutButton.ForeColor = System.Drawing.Color.White;
            this.logoutButton.Image = ((System.Drawing.Image)(resources.GetObject("logoutButton.Image")));
            this.logoutButton.Location = new System.Drawing.Point(2082, 65);
            this.logoutButton.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(342, 137);
            this.logoutButton.TabIndex = 3;
            this.logoutButton.Text = "Log Out";
            this.logoutButton.UseVisualStyleBackColor = true;
            this.logoutButton.Click += new System.EventHandler(this.logoutButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteButton.ForeColor = System.Drawing.Color.White;
            this.DeleteButton.Image = ((System.Drawing.Image)(resources.GetObject("DeleteButton.Image")));
            this.DeleteButton.Location = new System.Drawing.Point(1120, 65);
            this.DeleteButton.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(342, 137);
            this.DeleteButton.TabIndex = 1;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.AddButton.Image = ((System.Drawing.Image)(resources.GetObject("AddButton.Image")));
            this.AddButton.Location = new System.Drawing.Point(1621, 65);
            this.AddButton.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(342, 137);
            this.AddButton.TabIndex = 0;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // MiddelPannel
            // 
            this.MiddelPannel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(38)))));
            this.MiddelPannel.Controls.Add(this.dataGridView1);
            this.MiddelPannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MiddelPannel.Location = new System.Drawing.Point(0, 370);
            this.MiddelPannel.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.MiddelPannel.Name = "MiddelPannel";
            this.MiddelPannel.Size = new System.Drawing.Size(3334, 1068);
            this.MiddelPannel.TabIndex = 3;
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(38)))));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 123;
            this.dataGridView1.Size = new System.Drawing.Size(3334, 1068);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // backButton
            // 
            this.backButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backButton.ForeColor = System.Drawing.Color.White;
            this.backButton.Image = ((System.Drawing.Image)(resources.GetObject("backButton.Image")));
            this.backButton.Location = new System.Drawing.Point(637, 65);
            this.backButton.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(342, 137);
            this.backButton.TabIndex = 4;
            this.backButton.Text = "Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // RegInfoClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(19F, 37F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(254)))), ((int)(((byte)(225)))));
            this.ClientSize = new System.Drawing.Size(3334, 1711);
            this.Controls.Add(this.MiddelPannel);
            this.Controls.Add(this.BottomPannel);
            this.Controls.Add(this.UpperTextPannel);
            this.Controls.Add(this.UpperPannel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.Name = "RegInfoClass";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RegInfo";
            this.Load += new System.EventHandler(this.RegInfoClass_Load);
            this.UpperPannel.ResumeLayout(false);
            this.UpperTextPannel.ResumeLayout(false);
            this.UpperTextPannel.PerformLayout();
            this.BottomPannel.ResumeLayout(false);
            this.MiddelPannel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel UpperPannel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel UpperTextPannel;
        private System.Windows.Forms.Panel BottomPannel;
        private System.Windows.Forms.Panel MiddelPannel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button logoutButton;
        private System.Windows.Forms.Button backButton;
    }
}