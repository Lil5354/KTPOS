namespace KTPOS.STAFF
{
    partial class fStaff_F
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
            this.components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Guna2Button btnClose;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fStaff_F));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2CustomGradientPanel2 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.Filter = new Guna.UI2.WinForms.Guna2ComboBox();
            this.dgvBillDetails = new Guna.UI2.WinForms.Guna2DataGridView();
            this.TABLE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QUANTITY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TIME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.METHOD = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.PAYMENT = new System.Windows.Forms.DataGridViewButtonColumn();
            this.BILL = new System.Windows.Forms.DataGridViewButtonColumn();
            this.guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.flTable = new System.Windows.Forms.FlowLayoutPanel();
            this.btnMinSize = new Guna.UI2.WinForms.Guna2Button();
            this.btnMaxSize = new Guna.UI2.WinForms.Guna2Button();
            this.btnManage = new Guna.UI2.WinForms.Guna2Button();
            btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.guna2CustomGradientPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBillDetails)).BeginInit();
            this.guna2CustomGradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            btnClose.Animated = true;
            btnClose.AutoRoundedCorners = true;
            btnClose.BackColor = System.Drawing.Color.Transparent;
            btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
            btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            btnClose.BorderRadius = 11;
            btnClose.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.ToogleButton;
            btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            btnClose.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            btnClose.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            btnClose.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            btnClose.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            btnClose.FillColor = System.Drawing.Color.Transparent;
            btnClose.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnClose.ForeColor = System.Drawing.Color.Transparent;
            btnClose.Location = new System.Drawing.Point(1344, 38);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(25, 25);
            btnClose.TabIndex = 38;
            btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.TargetControl = this;
            // 
            // guna2CustomGradientPanel2
            // 
            this.guna2CustomGradientPanel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2CustomGradientPanel2.BorderRadius = 20;
            this.guna2CustomGradientPanel2.Controls.Add(this.Filter);
            this.guna2CustomGradientPanel2.Controls.Add(this.dgvBillDetails);
            this.guna2CustomGradientPanel2.FillColor = System.Drawing.Color.Maroon;
            this.guna2CustomGradientPanel2.FillColor2 = System.Drawing.Color.Maroon;
            this.guna2CustomGradientPanel2.FillColor3 = System.Drawing.Color.Maroon;
            this.guna2CustomGradientPanel2.FillColor4 = System.Drawing.Color.Maroon;
            this.guna2CustomGradientPanel2.Location = new System.Drawing.Point(547, 120);
            this.guna2CustomGradientPanel2.Name = "guna2CustomGradientPanel2";
            this.guna2CustomGradientPanel2.Size = new System.Drawing.Size(828, 550);
            this.guna2CustomGradientPanel2.TabIndex = 42;
            // 
            // Filter
            // 
            this.Filter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Filter.BackColor = System.Drawing.Color.Transparent;
            this.Filter.BorderRadius = 7;
            this.Filter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Filter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Filter.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Filter.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Filter.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Filter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.Filter.ItemHeight = 30;
            this.Filter.Items.AddRange(new object[] {
            "Best Sellers",
            "New Arrivals",
            "Featured Dishes",
            "Combo Deals",
            "Most Loved"});
            this.Filter.Location = new System.Drawing.Point(597, 29);
            this.Filter.Name = "Filter";
            this.Filter.Size = new System.Drawing.Size(211, 36);
            this.Filter.TabIndex = 51;
            // 
            // dgvBillDetails
            // 
            this.dgvBillDetails.AllowUserToAddRows = false;
            this.dgvBillDetails.AllowUserToDeleteRows = false;
            this.dgvBillDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(185)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dgvBillDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBillDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvBillDetails.ColumnHeadersHeight = 32;
            this.dgvBillDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvBillDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TABLE,
            this.QUANTITY,
            this.TIME,
            this.METHOD,
            this.PAYMENT,
            this.BILL});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(185)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvBillDetails.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvBillDetails.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvBillDetails.Location = new System.Drawing.Point(31, 87);
            this.dgvBillDetails.Name = "dgvBillDetails";
            this.dgvBillDetails.ReadOnly = true;
            this.dgvBillDetails.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBillDetails.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvBillDetails.RowHeadersVisible = false;
            this.dgvBillDetails.RowHeadersWidth = 51;
            this.dgvBillDetails.RowTemplate.Height = 24;
            this.dgvBillDetails.Size = new System.Drawing.Size(777, 415);
            this.dgvBillDetails.TabIndex = 0;
            this.dgvBillDetails.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvBillDetails.ThemeStyle.AlternatingRowsStyle.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvBillDetails.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Maroon;
            this.dgvBillDetails.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(185)))));
            this.dgvBillDetails.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dgvBillDetails.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvBillDetails.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvBillDetails.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.Maroon;
            this.dgvBillDetails.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvBillDetails.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvBillDetails.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvBillDetails.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvBillDetails.ThemeStyle.HeaderStyle.Height = 32;
            this.dgvBillDetails.ThemeStyle.ReadOnly = true;
            this.dgvBillDetails.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvBillDetails.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvBillDetails.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvBillDetails.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.Maroon;
            this.dgvBillDetails.ThemeStyle.RowsStyle.Height = 24;
            this.dgvBillDetails.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(185)))));
            this.dgvBillDetails.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            // 
            // TABLE
            // 
            this.TABLE.HeaderText = "TABLE";
            this.TABLE.MinimumWidth = 6;
            this.TABLE.Name = "TABLE";
            this.TABLE.ReadOnly = true;
            // 
            // QUANTITY
            // 
            this.QUANTITY.HeaderText = "QTY";
            this.QUANTITY.MinimumWidth = 6;
            this.QUANTITY.Name = "QUANTITY";
            this.QUANTITY.ReadOnly = true;
            // 
            // TIME
            // 
            this.TIME.HeaderText = "TIME";
            this.TIME.MinimumWidth = 6;
            this.TIME.Name = "TIME";
            this.TIME.ReadOnly = true;
            // 
            // METHOD
            // 
            this.METHOD.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.METHOD.HeaderText = "METHOD";
            this.METHOD.Items.AddRange(new object[] {
            "Cash",
            "Transfer"});
            this.METHOD.MinimumWidth = 6;
            this.METHOD.Name = "METHOD";
            this.METHOD.ReadOnly = true;
            // 
            // PAYMENT
            // 
            this.PAYMENT.HeaderText = "PAYMENT";
            this.PAYMENT.MinimumWidth = 6;
            this.PAYMENT.Name = "PAYMENT";
            this.PAYMENT.ReadOnly = true;
            // 
            // BILL
            // 
            this.BILL.HeaderText = "BILL";
            this.BILL.MinimumWidth = 6;
            this.BILL.Name = "BILL";
            this.BILL.ReadOnly = true;
            // 
            // guna2CustomGradientPanel1
            // 
            this.guna2CustomGradientPanel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2CustomGradientPanel1.BorderRadius = 20;
            this.guna2CustomGradientPanel1.Controls.Add(this.flTable);
            this.guna2CustomGradientPanel1.FillColor = System.Drawing.Color.Maroon;
            this.guna2CustomGradientPanel1.FillColor2 = System.Drawing.Color.Maroon;
            this.guna2CustomGradientPanel1.FillColor3 = System.Drawing.Color.Maroon;
            this.guna2CustomGradientPanel1.FillColor4 = System.Drawing.Color.Maroon;
            this.guna2CustomGradientPanel1.Location = new System.Drawing.Point(32, 120);
            this.guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            this.guna2CustomGradientPanel1.Size = new System.Drawing.Size(478, 550);
            this.guna2CustomGradientPanel1.TabIndex = 41;
            // 
            // flTable
            // 
            this.flTable.AutoScroll = true;
            this.flTable.BackColor = System.Drawing.Color.Transparent;
            this.flTable.Location = new System.Drawing.Point(24, 29);
            this.flTable.Name = "flTable";
            this.flTable.Size = new System.Drawing.Size(427, 473);
            this.flTable.TabIndex = 1;
            // 
            // btnMinSize
            // 
            this.btnMinSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinSize.BackColor = System.Drawing.Color.Transparent;
            this.btnMinSize.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMinSize.BackgroundImage")));
            this.btnMinSize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnMinSize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinSize.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnMinSize.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnMinSize.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnMinSize.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnMinSize.FillColor = System.Drawing.Color.Transparent;
            this.btnMinSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnMinSize.ForeColor = System.Drawing.Color.Transparent;
            this.btnMinSize.Location = new System.Drawing.Point(1313, 38);
            this.btnMinSize.Name = "btnMinSize";
            this.btnMinSize.Size = new System.Drawing.Size(25, 25);
            this.btnMinSize.TabIndex = 40;
            this.btnMinSize.Click += new System.EventHandler(this.btnMinSize_Click);
            // 
            // btnMaxSize
            // 
            this.btnMaxSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMaxSize.BackColor = System.Drawing.Color.Transparent;
            this.btnMaxSize.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMaxSize.BackgroundImage")));
            this.btnMaxSize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnMaxSize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMaxSize.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnMaxSize.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnMaxSize.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnMaxSize.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnMaxSize.FillColor = System.Drawing.Color.Transparent;
            this.btnMaxSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnMaxSize.ForeColor = System.Drawing.Color.Transparent;
            this.btnMaxSize.Location = new System.Drawing.Point(1313, 38);
            this.btnMaxSize.Name = "btnMaxSize";
            this.btnMaxSize.Size = new System.Drawing.Size(25, 25);
            this.btnMaxSize.TabIndex = 39;
            this.btnMaxSize.Click += new System.EventHandler(this.btnMaxSize_Click);
            // 
            // btnManage
            // 
            this.btnManage.Animated = true;
            this.btnManage.AutoRoundedCorners = true;
            this.btnManage.BackColor = System.Drawing.Color.Transparent;
            this.btnManage.BorderRadius = 25;
            this.btnManage.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.btnManage.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnManage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnManage.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnManage.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnManage.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnManage.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnManage.FillColor = System.Drawing.Color.Transparent;
            this.btnManage.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold);
            this.btnManage.ForeColor = System.Drawing.Color.White;
            this.btnManage.HoverState.BorderColor = System.Drawing.Color.White;
            this.btnManage.HoverState.CustomBorderColor = System.Drawing.Color.Transparent;
            this.btnManage.HoverState.FillColor = System.Drawing.Color.White;
            this.btnManage.HoverState.ForeColor = System.Drawing.Color.Maroon;
            this.btnManage.Location = new System.Drawing.Point(32, 38);
            this.btnManage.Name = "btnManage";
            this.btnManage.Size = new System.Drawing.Size(148, 52);
            this.btnManage.TabIndex = 43;
            this.btnManage.Text = "Management";
            this.btnManage.UseTransparentBackground = true;
            this.btnManage.Click += new System.EventHandler(this.btnManage_Click);
            // 
            // fStaff_F
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackgroundImage = global::KTPOS.Properties.Resources.bground;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1418, 927);
            this.Controls.Add(this.btnManage);
            this.Controls.Add(this.guna2CustomGradientPanel2);
            this.Controls.Add(this.guna2CustomGradientPanel1);
            this.Controls.Add(this.btnMinSize);
            this.Controls.Add(this.btnMaxSize);
            this.Controls.Add(btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fStaff_F";
            this.Text = "fStaff_F";
            this.guna2CustomGradientPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBillDetails)).EndInit();
            this.guna2CustomGradientPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel2;
        private Guna.UI2.WinForms.Guna2ComboBox Filter;
        private Guna.UI2.WinForms.Guna2DataGridView dgvBillDetails;
        private System.Windows.Forms.DataGridViewTextBoxColumn TABLE;
        private System.Windows.Forms.DataGridViewTextBoxColumn QUANTITY;
        private System.Windows.Forms.DataGridViewTextBoxColumn TIME;
        private System.Windows.Forms.DataGridViewComboBoxColumn METHOD;
        private System.Windows.Forms.DataGridViewButtonColumn PAYMENT;
        private System.Windows.Forms.DataGridViewButtonColumn BILL;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private System.Windows.Forms.FlowLayoutPanel flTable;
        private Guna.UI2.WinForms.Guna2Button btnMinSize;
        private Guna.UI2.WinForms.Guna2Button btnMaxSize;
        private Guna.UI2.WinForms.Guna2Button btnManage;
    }
}