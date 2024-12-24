namespace KTPOS.STAFF
{
    partial class fStaff_S
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fStaff_S));
            Guna.UI2.WinForms.Guna2Button btnClose;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnMinSize = new Guna.UI2.WinForms.Guna2Button();
            this.btnMaxSize = new Guna.UI2.WinForms.Guna2Button();
            this.Filter = new Guna.UI2.WinForms.Guna2ComboBox();
            this.FlowMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.PanelBillCus = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.nUDItem = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.guna2TextBox1 = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2HtmlLabel5 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtTotal = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.dtgvBillCus = new Guna.UI2.WinForms.Guna2DataGridView();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();
            this.txtTable = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            this.NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QUANTITY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PRICE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DELETE = new System.Windows.Forms.DataGridViewButtonColumn();
            this.txtPeople = new Guna.UI2.WinForms.Guna2TextBox();
            this.lbCancel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.PanelBillCus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtgvBillCus)).BeginInit();
            this.SuspendLayout();
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
            this.btnMinSize.Location = new System.Drawing.Point(1326, 29);
            this.btnMinSize.Name = "btnMinSize";
            this.btnMinSize.Size = new System.Drawing.Size(25, 25);
            this.btnMinSize.TabIndex = 43;
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
            this.btnMaxSize.Location = new System.Drawing.Point(1326, 29);
            this.btnMaxSize.Name = "btnMaxSize";
            this.btnMaxSize.Size = new System.Drawing.Size(25, 25);
            this.btnMaxSize.TabIndex = 42;
            this.btnMaxSize.Click += new System.EventHandler(this.btnMaxSize_Click);
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
            btnClose.Location = new System.Drawing.Point(1357, 29);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(25, 25);
            btnClose.TabIndex = 41;
            btnClose.Click += new System.EventHandler(this.btnClose_Click);
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
            this.Filter.Location = new System.Drawing.Point(1149, 123);
            this.Filter.Name = "Filter";
            this.Filter.Size = new System.Drawing.Size(211, 36);
            this.Filter.TabIndex = 54;
            // 
            // FlowMenu
            // 
            this.FlowMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FlowMenu.AutoScroll = true;
            this.FlowMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FlowMenu.BackColor = System.Drawing.Color.Transparent;
            this.FlowMenu.Location = new System.Drawing.Point(500, 192);
            this.FlowMenu.Name = "FlowMenu";
            this.FlowMenu.Size = new System.Drawing.Size(860, 570);
            this.FlowMenu.TabIndex = 53;
            // 
            // PanelBillCus
            // 
            this.PanelBillCus.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.PanelBillCus.AutoSize = true;
            this.PanelBillCus.BackColor = System.Drawing.Color.Transparent;
            this.PanelBillCus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PanelBillCus.BorderColor = System.Drawing.Color.Maroon;
            this.PanelBillCus.BorderRadius = 15;
            this.PanelBillCus.Controls.Add(this.lbCancel);
            this.PanelBillCus.Controls.Add(this.txtPeople);
            this.PanelBillCus.Controls.Add(this.nUDItem);
            this.PanelBillCus.Controls.Add(this.guna2TextBox1);
            this.PanelBillCus.Controls.Add(this.guna2HtmlLabel5);
            this.PanelBillCus.Controls.Add(this.txtTotal);
            this.PanelBillCus.Controls.Add(this.dtgvBillCus);
            this.PanelBillCus.Controls.Add(this.btnSave);
            this.PanelBillCus.Controls.Add(this.txtTable);
            this.PanelBillCus.FillColor = System.Drawing.Color.Maroon;
            this.PanelBillCus.FillColor2 = System.Drawing.Color.Maroon;
            this.PanelBillCus.FillColor3 = System.Drawing.Color.Maroon;
            this.PanelBillCus.FillColor4 = System.Drawing.Color.Maroon;
            this.PanelBillCus.Location = new System.Drawing.Point(46, 123);
            this.PanelBillCus.Name = "PanelBillCus";
            this.PanelBillCus.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(20);
            this.PanelBillCus.Size = new System.Drawing.Size(395, 639);
            this.PanelBillCus.TabIndex = 52;
            // 
            // nUDItem
            // 
            this.nUDItem.BackColor = System.Drawing.Color.Transparent;
            this.nUDItem.BorderRadius = 10;
            this.nUDItem.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.nUDItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nUDItem.Location = new System.Drawing.Point(276, 69);
            this.nUDItem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nUDItem.Name = "nUDItem";
            this.nUDItem.Size = new System.Drawing.Size(81, 36);
            this.nUDItem.TabIndex = 51;
            this.nUDItem.UpDownButtonFillColor = System.Drawing.Color.Maroon;
            this.nUDItem.UpDownButtonForeColor = System.Drawing.Color.White;
            // 
            // guna2TextBox1
            // 
            this.guna2TextBox1.BorderRadius = 10;
            this.guna2TextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.guna2TextBox1.DefaultText = "";
            this.guna2TextBox1.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.guna2TextBox1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.guna2TextBox1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.guna2TextBox1.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.guna2TextBox1.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.guna2TextBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2TextBox1.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.guna2TextBox1.Location = new System.Drawing.Point(34, 475);
            this.guna2TextBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.guna2TextBox1.Name = "guna2TextBox1";
            this.guna2TextBox1.PasswordChar = '\0';
            this.guna2TextBox1.PlaceholderText = "Note";
            this.guna2TextBox1.SelectedText = "";
            this.guna2TextBox1.Size = new System.Drawing.Size(323, 42);
            this.guna2TextBox1.TabIndex = 20;
            // 
            // guna2HtmlLabel5
            // 
            this.guna2HtmlLabel5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.guna2HtmlLabel5.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel5.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel5.ForeColor = System.Drawing.Color.White;
            this.guna2HtmlLabel5.Location = new System.Drawing.Point(44, 420);
            this.guna2HtmlLabel5.Name = "guna2HtmlLabel5";
            this.guna2HtmlLabel5.Size = new System.Drawing.Size(64, 33);
            this.guna2HtmlLabel5.TabIndex = 17;
            this.guna2HtmlLabel5.Text = "Total:";
            // 
            // txtTotal
            // 
            this.txtTotal.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtTotal.BackColor = System.Drawing.Color.Transparent;
            this.txtTotal.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.ForeColor = System.Drawing.Color.White;
            this.txtTotal.Location = new System.Drawing.Point(308, 431);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(42, 22);
            this.txtTotal.TabIndex = 14;
            this.txtTotal.Text = "$0.00";
            // 
            // dtgvBillCus
            // 
            this.dtgvBillCus.AllowUserToAddRows = false;
            this.dtgvBillCus.AllowUserToDeleteRows = false;
            this.dtgvBillCus.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(241)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dtgvBillCus.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtgvBillCus.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtgvBillCus.ColumnHeadersHeight = 40;
            this.dtgvBillCus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dtgvBillCus.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NAME,
            this.QUANTITY,
            this.PRICE,
            this.DELETE});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(241)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dtgvBillCus.DefaultCellStyle = dataGridViewCellStyle6;
            this.dtgvBillCus.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(241)))), ((int)(((byte)(243)))));
            this.dtgvBillCus.Location = new System.Drawing.Point(34, 126);
            this.dtgvBillCus.Name = "dtgvBillCus";
            this.dtgvBillCus.ReadOnly = true;
            this.dtgvBillCus.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtgvBillCus.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dtgvBillCus.RowHeadersVisible = false;
            this.dtgvBillCus.RowHeadersWidth = 51;
            this.dtgvBillCus.RowTemplate.Height = 25;
            this.dtgvBillCus.Size = new System.Drawing.Size(323, 267);
            this.dtgvBillCus.TabIndex = 5;
            this.dtgvBillCus.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.LightGrid;
            this.dtgvBillCus.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dtgvBillCus.ThemeStyle.AlternatingRowsStyle.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtgvBillCus.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Maroon;
            this.dtgvBillCus.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(241)))), ((int)(((byte)(243)))));
            this.dtgvBillCus.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dtgvBillCus.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dtgvBillCus.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(241)))), ((int)(((byte)(243)))));
            this.dtgvBillCus.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.Maroon;
            this.dtgvBillCus.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtgvBillCus.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtgvBillCus.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dtgvBillCus.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dtgvBillCus.ThemeStyle.HeaderStyle.Height = 40;
            this.dtgvBillCus.ThemeStyle.ReadOnly = true;
            this.dtgvBillCus.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dtgvBillCus.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dtgvBillCus.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtgvBillCus.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.Maroon;
            this.dtgvBillCus.ThemeStyle.RowsStyle.Height = 25;
            this.dtgvBillCus.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(241)))), ((int)(((byte)(243)))));
            this.dtgvBillCus.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.Black;
            // 
            // btnSave
            // 
            this.btnSave.Animated = true;
            this.btnSave.AutoRoundedCorners = true;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BorderRadius = 30;
            this.btnSave.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSave.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSave.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSave.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSave.FillColor = System.Drawing.Color.White;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.Maroon;
            this.btnSave.Location = new System.Drawing.Point(93, 545);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(213, 62);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseTransparentBackground = true;
            // 
            // txtTable
            // 
            this.txtTable.BackColor = System.Drawing.Color.Transparent;
            this.txtTable.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txtTable.ForeColor = System.Drawing.Color.White;
            this.txtTable.Location = new System.Drawing.Point(34, 16);
            this.txtTable.Name = "txtTable";
            this.txtTable.Size = new System.Drawing.Size(109, 43);
            this.txtTable.TabIndex = 3;
            this.txtTable.Text = "TABLE 1";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.AutoRoundedCorners = true;
            this.txtSearch.AutoSize = true;
            this.txtSearch.BackColor = System.Drawing.Color.Transparent;
            this.txtSearch.BorderColor = System.Drawing.Color.Black;
            this.txtSearch.BorderRadius = 18;
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.DefaultText = "";
            this.txtSearch.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtSearch.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtSearch.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSearch.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSearch.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearch.ForeColor = System.Drawing.Color.Black;
            this.txtSearch.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSearch.IconLeft = ((System.Drawing.Image)(resources.GetObject("txtSearch.IconLeft")));
            this.txtSearch.IconLeftOffset = new System.Drawing.Point(6, 0);
            this.txtSearch.Location = new System.Drawing.Point(1031, 29);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSearch.MaximumSize = new System.Drawing.Size(240, 38);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PasswordChar = '\0';
            this.txtSearch.PlaceholderText = "Search";
            this.txtSearch.SelectedText = "";
            this.txtSearch.Size = new System.Drawing.Size(240, 38);
            this.txtSearch.TabIndex = 51;
            // 
            // NAME
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.NAME.DefaultCellStyle = dataGridViewCellStyle3;
            this.NAME.HeaderText = "NAME";
            this.NAME.MinimumWidth = 6;
            this.NAME.Name = "NAME";
            this.NAME.ReadOnly = true;
            // 
            // QUANTITY
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.QUANTITY.DefaultCellStyle = dataGridViewCellStyle4;
            this.QUANTITY.HeaderText = "QTY";
            this.QUANTITY.MinimumWidth = 6;
            this.QUANTITY.Name = "QUANTITY";
            this.QUANTITY.ReadOnly = true;
            // 
            // PRICE
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.PRICE.DefaultCellStyle = dataGridViewCellStyle5;
            this.PRICE.HeaderText = "PRICE";
            this.PRICE.MinimumWidth = 6;
            this.PRICE.Name = "PRICE";
            this.PRICE.ReadOnly = true;
            // 
            // DELETE
            // 
            this.DELETE.HeaderText = "DEL";
            this.DELETE.MinimumWidth = 6;
            this.DELETE.Name = "DELETE";
            this.DELETE.ReadOnly = true;
            // 
            // txtPeople
            // 
            this.txtPeople.BorderRadius = 5;
            this.txtPeople.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPeople.DefaultText = "";
            this.txtPeople.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtPeople.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtPeople.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtPeople.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtPeople.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPeople.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPeople.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPeople.Location = new System.Drawing.Point(34, 69);
            this.txtPeople.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPeople.Name = "txtPeople";
            this.txtPeople.PasswordChar = '\0';
            this.txtPeople.PlaceholderText = "People";
            this.txtPeople.SelectedText = "";
            this.txtPeople.Size = new System.Drawing.Size(109, 36);
            this.txtPeople.TabIndex = 52;
            // 
            // lbCancel
            // 
            this.lbCancel.BackColor = System.Drawing.Color.Transparent;
            this.lbCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lbCancel.ForeColor = System.Drawing.Color.White;
            this.lbCancel.Location = new System.Drawing.Point(310, 28);
            this.lbCancel.Name = "lbCancel";
            this.lbCancel.Size = new System.Drawing.Size(47, 22);
            this.lbCancel.TabIndex = 53;
            this.lbCancel.Text = "Cancel";
            // 
            // FStaff_S
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1418, 927);
            this.Controls.Add(this.Filter);
            this.Controls.Add(this.FlowMenu);
            this.Controls.Add(this.PanelBillCus);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnMinSize);
            this.Controls.Add(this.btnMaxSize);
            this.Controls.Add(btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FStaff_S";
            this.Text = "FStaff_S";
            this.PanelBillCus.ResumeLayout(false);
            this.PanelBillCus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtgvBillCus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button btnMinSize;
        private Guna.UI2.WinForms.Guna2Button btnMaxSize;
        private Guna.UI2.WinForms.Guna2ComboBox Filter;
        private System.Windows.Forms.FlowLayoutPanel FlowMenu;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel PanelBillCus;
        private Guna.UI2.WinForms.Guna2NumericUpDown nUDItem;
        private Guna.UI2.WinForms.Guna2TextBox guna2TextBox1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel5;
        private Guna.UI2.WinForms.Guna2HtmlLabel txtTotal;
        private Guna.UI2.WinForms.Guna2DataGridView dtgvBillCus;
        private Guna.UI2.WinForms.Guna2Button btnSave;
        private Guna.UI2.WinForms.Guna2HtmlLabel txtTable;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbCancel;
        private Guna.UI2.WinForms.Guna2TextBox txtPeople;
        private System.Windows.Forms.DataGridViewTextBoxColumn NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn QUANTITY;
        private System.Windows.Forms.DataGridViewTextBoxColumn PRICE;
        private System.Windows.Forms.DataGridViewButtonColumn DELETE;
    }
}