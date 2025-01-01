namespace KTPOS.STAFF
{
    partial class fBillViewer
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
            this.rpBill = new Microsoft.Reporting.WinForms.ReportViewer();
            this.kTPOSDataSet = new KTPOS.KTPOSDataSet();
            this.kTPOSDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.kTPOSDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kTPOSDataSetBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // rpBill
            // 
            this.rpBill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rpBill.LocalReport.ReportEmbeddedResource = "KTPOS.STAFF.rptBill.rdlc";
            this.rpBill.Location = new System.Drawing.Point(0, 0);
            this.rpBill.Name = "rpBill";
            this.rpBill.ServerReport.BearerToken = null;
            this.rpBill.Size = new System.Drawing.Size(800, 450);
            this.rpBill.TabIndex = 0;
            // 
            // kTPOSDataSet
            // 
            this.kTPOSDataSet.DataSetName = "KTPOSDataSet";
            this.kTPOSDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // kTPOSDataSetBindingSource
            // 
            this.kTPOSDataSetBindingSource.DataSource = this.kTPOSDataSet;
            this.kTPOSDataSetBindingSource.Position = 0;
            // 
            // fBillViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.rpBill);
            this.Name = "fBillViewer";
            this.Text = "Bill";
            this.Load += new System.EventHandler(this.fBillViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kTPOSDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kTPOSDataSetBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rpBill;
        private KTPOSDataSet kTPOSDataSet;
        private System.Windows.Forms.BindingSource kTPOSDataSetBindingSource;
    }
}