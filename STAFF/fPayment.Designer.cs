namespace KTPOS.STAFF
{
    partial class fPayment
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
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnDone = new Guna.UI2.WinForms.Guna2Button();
            this.txtTotal = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtCustomerPayment = new Guna.UI2.WinForms.Guna2TextBox();
            this.SuspendLayout();
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(26, 27);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(290, 22);
            this.guna2HtmlLabel1.TabIndex = 7;
            this.guna2HtmlLabel1.Text = "Enter the amount given by the customer.";
            // 
            // btnDone
            // 
            this.btnDone.BorderRadius = 20;
            this.btnDone.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDone.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDone.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDone.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDone.FillColor = System.Drawing.Color.Maroon;
            this.btnDone.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnDone.ForeColor = System.Drawing.Color.White;
            this.btnDone.Location = new System.Drawing.Point(346, 69);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(110, 45);
            this.btnDone.TabIndex = 6;
            this.btnDone.Text = "Done";
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // txtTotal
            // 
            this.txtTotal.BackColor = System.Drawing.Color.Transparent;
            this.txtTotal.Location = new System.Drawing.Point(346, 27);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(18, 18);
            this.txtTotal.TabIndex = 5;
            this.txtTotal.Text = "0đ";
            // 
            // txtCustomerPayment
            // 
            this.txtCustomerPayment.BorderColor = System.Drawing.Color.Maroon;
            this.txtCustomerPayment.BorderRadius = 10;
            this.txtCustomerPayment.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCustomerPayment.DefaultText = "";
            this.txtCustomerPayment.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCustomerPayment.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCustomerPayment.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCustomerPayment.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCustomerPayment.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCustomerPayment.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCustomerPayment.ForeColor = System.Drawing.Color.Maroon;
            this.txtCustomerPayment.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCustomerPayment.Location = new System.Drawing.Point(26, 69);
            this.txtCustomerPayment.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCustomerPayment.Name = "txtCustomerPayment";
            this.txtCustomerPayment.PasswordChar = '\0';
            this.txtCustomerPayment.PlaceholderText = "";
            this.txtCustomerPayment.SelectedText = "";
            this.txtCustomerPayment.Size = new System.Drawing.Size(290, 45);
            this.txtCustomerPayment.TabIndex = 4;
            // 
            // fPayment
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(525, 155);
            this.Controls.Add(this.guna2HtmlLabel1);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtCustomerPayment);
            this.Name = "fPayment";
            this.Text = "LeftOver";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2Button btnDone;
        private Guna.UI2.WinForms.Guna2HtmlLabel txtTotal;
        private Guna.UI2.WinForms.Guna2TextBox txtCustomerPayment;
    }
}