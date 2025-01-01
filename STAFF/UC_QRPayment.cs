using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ZXing.Rendering;
using ZXing;
using KTPOS.Proccess;

namespace KTPOS.STAFF
{
    public partial class UC_QRPayment : UserControl
    {
        private const string MOMO_PHONE = "0941440611"; // Replace with actual MoMo merchant phone
        private const string MOMO_NAME = "Dương Thị Thanh Thảo";
        private int? billId;
        private decimal currentAmount;

        public EventHandler TxtContent_TextChanged { get; }

        internal void GetBillId(int id)
        {
             billId = id;
        }

        public UC_QRPayment()
        {
            InitializeComponent();
            txtContent.TextChanged += TxtContent_TextChanged;
        }
        public void UpdateQRCode(string content, decimal amount)
        {
            try
            {
                if (string.IsNullOrEmpty(content) || amount <= 0)
                {
                    throw new ArgumentException("Invalid content or amount for QR code");
                }
                currentAmount = amount;  // Store the amount

                // Update the text fields
                txtContent.Text = content;
                txtCost.Text = amount.ToString("N0") + " VND";
                txt_phone.Text = MOMO_PHONE;
                name.Text = MOMO_NAME;

                // Generate initial QR code
                GenerateQRCode(content, amount);
                // Add your QR code generation logic here
                // Make sure to handle the content and amount appropriately
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private void GenerateQRCode(string content, decimal amount)
        {
            try
            {
                string messageContent = txtContent.Text;

                // Format the QR code text according to Momo's specification
                // The extra parameters control message behavior
                // Format: 2|99|Phone|Name|Message|Store ID|Terminal ID|Amount|Message Type|Default Message
                string qrcode_text = string.Format("2|99|{0}|{1}|{2}|0|0|{3}|{4}|{2}",
                    MOMO_PHONE,
                    MOMO_NAME,
                    messageContent,
                    amount,
                    messageContent);

                BarcodeWriter barcodeWriter = new BarcodeWriter();
                EncodingOptions encodingOptions = new EncodingOptions()
                {
                    Width = 250,
                    Height = 250,
                    Margin = 0,
                    PureBarcode = false
                };
                encodingOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
                barcodeWriter.Renderer = new BitmapRenderer();
                barcodeWriter.Options = encodingOptions;
                barcodeWriter.Format = BarcodeFormat.QR_CODE;

                using (Bitmap bitmap = barcodeWriter.Write(qrcode_text))
                using (Bitmap logo = resizeImage(Properties.Resources.logo_momo, 64, 64) as Bitmap)
                {
                    if (logo != null)
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.DrawImage(logo, new Point((bitmap.Width - logo.Width) / 2, (bitmap.Height - logo.Height) / 2));
                        }
                    }

                    Bitmap finalBitmap = new Bitmap(bitmap);
                    pic_qrcode.Image?.Dispose();
                    pic_qrcode.Image = finalBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public Image resizeImage(Image image, int new_height, int new_width)
        {
            Bitmap new_image = new Bitmap(new_width, new_height);
            Graphics g = Graphics.FromImage((Image)new_image);
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, new_width, new_height);
            return new_image;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Do you really want to exit?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                if (this.Parent != null)
                {
                    this.Parent.Controls.Remove(this);
                    this.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Exit cancelled. Continue your activity.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Focus();
            }
        }
        private void btn_Pay_Click(object sender, EventArgs e)
        {
            try
            {
                if (!billId.HasValue)
                {
                    MessageBox.Show("Invalid bill ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = "UPDATE BILL SET STATUS = 1, CHKOUT_TIME = GETDATE() WHERE ID = @billId";
                int result = GetDatabase.Instance.ExecuteNonQuery(query, new object[] { billId.Value });

                if (result > 0)
                {
                    MessageBox.Show("Payment completed successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Form parentForm = this.FindForm();
                    if (parentForm is fStaff_F staffForm)
                    {
                        // Find and update the corresponding row in ListBill
                        foreach (DataGridViewRow row in staffForm.ListBill.Rows)
                        {
                            if (row.Cells["BillID"].Value != null &&
                                Convert.ToInt32(row.Cells["BillID"].Value) == billId.Value)
                            {
                                row.Cells["PAYMENT"].Value = "Done";
                                row.Cells["METHOD"].Value = "Transfer"; // Changed from "Cash" to "Transfer"
                                row.Cells["CHECKOUT"].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                                if (row.Cells["PAYMENT"] is DataGridViewButtonCell paymentCell)
                                {
                                    paymentCell.Style.BackColor = Color.Green;
                                    paymentCell.Style.ForeColor = Color.White;
                                }
                                break;
                            }
                        }
                    }

                    if (this.Parent != null)
                    {
                        this.Parent.Controls.Remove(this);
                        this.Dispose();
                    }
                }
                else
                {
                    MessageBox.Show("Failed to update payment status.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            GenerateQRCode(txtContent.Text, currentAmount);
        }
    }
}
