namespace PWFAuthCSharp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlLoginPage;
        private System.Windows.Forms.Panel pnlLoginCard;
        private System.Windows.Forms.TextBox txtLicenseKey;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.ProgressBar loginProgress;
        private System.Windows.Forms.Label lblLoginStatus;
        private System.Windows.Forms.Button btnMoveLicense;

        private System.Windows.Forms.Panel pnlDashboard;
        private System.Windows.Forms.Label lblDashboardTitle;
        private System.Windows.Forms.Label lblDashboardSubtitle;
        private System.Windows.Forms.Label lblOnline;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblStatusValue;
        private System.Windows.Forms.Label lblTypeValue;
        private System.Windows.Forms.Label lblExpiryValue;
        private System.Windows.Forms.Label lblRemainingValue;
        private System.Windows.Forms.TabControl detailsTabs;
        private System.Windows.Forms.DataGridView dgvDetails;
        private System.Windows.Forms.TextBox txtRawJson;
        private System.Windows.Forms.Button btnCopyJson;
        private System.Windows.Forms.Label lblSessionInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();

            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(11, 18, 32);
            this.ClientSize = new System.Drawing.Size(1084, 721);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 650);
            this.Name = "Form1";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.RightToLeftLayout = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PWF Auth - Sign In";

            this.ResumeLayout(false);
        }
    }
}
