namespace PWFAuthCSharp
{
    partial class Form1
    {
        private void BuildLoginInterface()
        {
            this.pnlLoginPage = new System.Windows.Forms.Panel
            {
                BackColor = System.Drawing.Color.FromArgb(11, 18, 32),
                Dock = System.Windows.Forms.DockStyle.Fill,
                Name = "pnlLoginPage"
            };
            this.pnlLoginPage.Resize += new System.EventHandler(this.pnlLoginPage_Resize);

            this.pnlLoginCard = new System.Windows.Forms.Panel
            {
                BackColor = System.Drawing.Color.FromArgb(23, 33, 51),
                Name = "pnlLoginCard",
                Size = new System.Drawing.Size(500, 465)
            };

            var accent = new System.Windows.Forms.Panel
            {
                BackColor = System.Drawing.Color.FromArgb(59, 130, 246),
                Dock = System.Windows.Forms.DockStyle.Top,
                Height = 5
            };
            var logo = MakeLabel("◆  PWF AUTH", 42, 43, 416, 44, 16F,
                System.Drawing.Color.FromArgb(96, 165, 250), true,
                System.Drawing.ContentAlignment.MiddleCenter);
            var welcome = MakeLabel("Welcome Back", 42, 102, 416, 41, 18F,
                System.Drawing.Color.White, true, System.Drawing.ContentAlignment.MiddleCenter);
            var hint = MakeLabel("Enter your key to connect to the license server\r\nand view your complete subscription details", 42, 150, 416, 42, 9F,
                System.Drawing.Color.FromArgb(148, 163, 184), false,
                System.Drawing.ContentAlignment.TopCenter);
            var keyCaption = MakeLabel("License key", 42, 207, 416, 23, 9F,
                System.Drawing.Color.FromArgb(203, 213, 225), false,
                System.Drawing.ContentAlignment.MiddleLeft);

            this.txtLicenseKey = new System.Windows.Forms.TextBox
            {
                BackColor = System.Drawing.Color.FromArgb(15, 23, 42),
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                CharacterCasing = System.Windows.Forms.CharacterCasing.Upper,
                Font = new System.Drawing.Font("Consolas", 13F),
                ForeColor = System.Drawing.Color.White,
                Location = new System.Drawing.Point(42, 239),
                Name = "txtLicenseKey",
                RightToLeft = System.Windows.Forms.RightToLeft.No,
                Size = new System.Drawing.Size(416, 28),
                TextAlign = System.Windows.Forms.HorizontalAlignment.Center
            };

            this.btnLogin = MakeButton("Sign In", 42, 289, 416, 46,
                System.Drawing.Color.FromArgb(59, 130, 246), System.Drawing.Color.White);
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            this.loginProgress = new System.Windows.Forms.ProgressBar
            {
                Location = new System.Drawing.Point(42, 341),
                MarqueeAnimationSpeed = 28,
                Name = "loginProgress",
                Size = new System.Drawing.Size(416, 4),
                Style = System.Windows.Forms.ProgressBarStyle.Marquee,
                Visible = false
            };
            this.lblLoginStatus = MakeLabel(string.Empty, 42, 358, 416, 44, 9F,
                System.Drawing.Color.FromArgb(248, 113, 113), false,
                System.Drawing.ContentAlignment.TopCenter);
            var securityNote = MakeLabel("Encrypted connection  •  Key bound to this device", 42, 412, 416, 22, 9F,
                System.Drawing.Color.FromArgb(139, 151, 173), false,
                System.Drawing.ContentAlignment.MiddleCenter);

            this.pnlLoginCard.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                accent, logo, welcome, hint, keyCaption, this.txtLicenseKey,
                this.btnLogin, this.loginProgress, this.lblLoginStatus, securityNote
            });
            this.pnlLoginPage.Controls.Add(this.pnlLoginCard);
        }

        private void BuildDashboardInterface()
        {
            this.pnlDashboard = new System.Windows.Forms.Panel
            {
                BackColor = System.Drawing.Color.FromArgb(241, 245, 249),
                Dock = System.Windows.Forms.DockStyle.Fill,
                Name = "pnlDashboard",
                Padding = new System.Windows.Forms.Padding(22, 0, 22, 0),
                Visible = false
            };

            var layout = new System.Windows.Forms.TableLayoutPanel
            {
                ColumnCount = 1,
                Dock = System.Windows.Forms.DockStyle.Fill,
                RowCount = 4
            };
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));

            layout.Controls.Add(BuildDashboardHeader(), 0, 0);
            layout.Controls.Add(BuildSummaryCards(), 0, 1);
            layout.Controls.Add(BuildDetailsTabs(), 0, 2);

            this.lblSessionInfo = new System.Windows.Forms.Label
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ForeColor = System.Drawing.Color.FromArgb(100, 116, 139),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(this.lblSessionInfo, 0, 3);
            this.pnlDashboard.Controls.Add(layout);
        }

        private System.Windows.Forms.Control BuildDashboardHeader()
        {
            var header = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Size = new System.Drawing.Size(1040, 96)
            };

            this.lblDashboardTitle = MakeLabel("License Information", 0, 13, 700, 43, 20F,
                System.Drawing.Color.FromArgb(15, 23, 42), true,
                System.Drawing.ContentAlignment.MiddleLeft);
            this.lblDashboardTitle.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left;

            this.lblDashboardSubtitle = MakeLabel(string.Empty, 0, 55, 700, 28, 9F,
                System.Drawing.Color.FromArgb(100, 116, 139), false,
                System.Drawing.ContentAlignment.MiddleLeft);
            this.lblDashboardSubtitle.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left;

            this.lblOnline = MakeLabel("● Session active and protected", 697, 58, 200, 22, 9F,
                System.Drawing.Color.FromArgb(22, 163, 74), true,
                System.Drawing.ContentAlignment.MiddleRight);
            this.lblOnline.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;

            this.btnLogout = MakeButton("Sign Out", 912, 29, 128, 39,
                System.Drawing.Color.White, System.Drawing.Color.FromArgb(51, 65, 85));
            this.btnLogout.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnLogout.FlatAppearance.BorderSize = 1;
            this.btnLogout.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            header.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.lblDashboardTitle, this.lblDashboardSubtitle, this.lblOnline, this.btnLogout
            });
            return header;
        }

        private System.Windows.Forms.Control BuildSummaryCards()
        {
            var summary = new System.Windows.Forms.TableLayoutPanel
            {
                ColumnCount = 4,
                Dock = System.Windows.Forms.DockStyle.Fill,
                Padding = new System.Windows.Forms.Padding(0, 8, 0, 8),
                RowCount = 1
            };
            for (int i = 0; i < 4; i++)
                summary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            summary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.lblStatusValue = new System.Windows.Forms.Label();
            this.lblTypeValue = new System.Windows.Forms.Label();
            this.lblExpiryValue = new System.Windows.Forms.Label();
            this.lblRemainingValue = new System.Windows.Forms.Label();

            summary.Controls.Add(MakeSummaryCard("KEY STATUS", this.lblStatusValue,
                System.Drawing.Color.FromArgb(22, 163, 74), 16F), 0, 0);
            summary.Controls.Add(MakeSummaryCard("SUBSCRIPTION TYPE", this.lblTypeValue,
                System.Drawing.Color.FromArgb(30, 41, 59), 16F), 1, 0);
            summary.Controls.Add(MakeSummaryCard("EXPIRATION DATE", this.lblExpiryValue,
                System.Drawing.Color.FromArgb(30, 41, 59), 13F), 2, 0);
            summary.Controls.Add(MakeSummaryCard("TIME REMAINING", this.lblRemainingValue,
                System.Drawing.Color.FromArgb(37, 99, 235), 16F), 3, 0);
            return summary;
        }

        private System.Windows.Forms.Control MakeSummaryCard(string caption, System.Windows.Forms.Label valueLabel,
            System.Drawing.Color valueColor, float valueSize)
        {
            var card = new System.Windows.Forms.Panel
            {
                BackColor = System.Drawing.Color.White,
                Dock = System.Windows.Forms.DockStyle.Fill,
                Margin = new System.Windows.Forms.Padding(5)
            };
            var captionLabel = new System.Windows.Forms.Label
            {
                Dock = System.Windows.Forms.DockStyle.Top,
                ForeColor = System.Drawing.Color.FromArgb(100, 116, 139),
                Height = 37,
                Padding = new System.Windows.Forms.Padding(15, 8, 15, 0),
                Text = caption,
                TextAlign = System.Drawing.ContentAlignment.BottomLeft
            };
            valueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            valueLabel.Font = new System.Drawing.Font("Segoe UI", valueSize, System.Drawing.FontStyle.Bold);
            valueLabel.ForeColor = valueColor;
            valueLabel.Padding = new System.Windows.Forms.Padding(15, 0, 15, 8);
            valueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            card.Controls.Add(valueLabel);
            card.Controls.Add(captionLabel);
            return card;
        }

        private System.Windows.Forms.Control BuildDetailsTabs()
        {
            this.detailsTabs = new System.Windows.Forms.TabControl
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 10F),
                RightToLeft = System.Windows.Forms.RightToLeft.No,
                RightToLeftLayout = false
            };
            var detailsPage = new System.Windows.Forms.TabPage("All Information")
            {
                Padding = new System.Windows.Forms.Padding(8),
                UseVisualStyleBackColor = true
            };
            var jsonPage = new System.Windows.Forms.TabPage("Complete JSON")
            {
                Padding = new System.Windows.Forms.Padding(8),
                UseVisualStyleBackColor = true
            };

            this.dgvDetails = new System.Windows.Forms.DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeight = 40,
                ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                Dock = System.Windows.Forms.DockStyle.Fill,
                EnableHeadersVisualStyles = false,
                GridColor = System.Drawing.Color.FromArgb(226, 232, 240),
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            };
            this.dgvDetails.ColumnHeadersDefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
            {
                Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft,
                BackColor = System.Drawing.Color.FromArgb(30, 41, 59),
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                Padding = new System.Windows.Forms.Padding(5),
                SelectionBackColor = System.Drawing.Color.FromArgb(30, 41, 59),
                WrapMode = System.Windows.Forms.DataGridViewTriState.True
            };
            this.dgvDetails.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
            {
                Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft,
                BackColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 9.5F),
                ForeColor = System.Drawing.Color.FromArgb(30, 41, 59),
                Padding = new System.Windows.Forms.Padding(5),
                SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254),
                SelectionForeColor = System.Drawing.Color.FromArgb(15, 23, 42),
                WrapMode = System.Windows.Forms.DataGridViewTriState.True
            };
            this.dgvDetails.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                HeaderText = "Field", Name = "colField", ReadOnly = true, Width = 220
            });
            this.dgvDetails.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                HeaderText = "API Path", Name = "colPath", ReadOnly = true, Width = 230
            });
            this.dgvDetails.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill,
                HeaderText = "Value", Name = "colValue", ReadOnly = true
            });
            detailsPage.Controls.Add(this.dgvDetails);

            var jsonLayout = new System.Windows.Forms.TableLayoutPanel
            {
                ColumnCount = 1,
                Dock = System.Windows.Forms.DockStyle.Fill,
                RowCount = 2
            };
            jsonLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            jsonLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            var jsonToolbar = new System.Windows.Forms.Panel { Dock = System.Windows.Forms.DockStyle.Fill };
            var jsonHint = new System.Windows.Forms.Label
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ForeColor = System.Drawing.Color.FromArgb(100, 116, 139),
                Text = "The complete original response returned by the server",
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            this.btnCopyJson = MakeButton("Copy JSON", 0, 0, 130, 48,
                System.Drawing.Color.FromArgb(51, 65, 85), System.Drawing.Color.White);
            this.btnCopyJson.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCopyJson.Click += new System.EventHandler(this.btnCopyJson_Click);
            jsonToolbar.Controls.Add(jsonHint);
            jsonToolbar.Controls.Add(this.btnCopyJson);
            this.txtRawJson = new System.Windows.Forms.TextBox
            {
                BackColor = System.Drawing.Color.FromArgb(15, 23, 42),
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                Dock = System.Windows.Forms.DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 10F),
                ForeColor = System.Drawing.Color.FromArgb(226, 232, 240),
                Multiline = true,
                ReadOnly = true,
                RightToLeft = System.Windows.Forms.RightToLeft.No,
                ScrollBars = System.Windows.Forms.ScrollBars.Both,
                WordWrap = false
            };
            jsonLayout.Controls.Add(jsonToolbar, 0, 0);
            jsonLayout.Controls.Add(this.txtRawJson, 0, 1);
            jsonPage.Controls.Add(jsonLayout);

            this.detailsTabs.TabPages.Add(detailsPage);
            this.detailsTabs.TabPages.Add(jsonPage);
            return this.detailsTabs;
        }

        private static System.Windows.Forms.Label MakeLabel(string text, int x, int y, int width, int height,
            float size, System.Drawing.Color color, bool bold, System.Drawing.ContentAlignment alignment)
        {
            return new System.Windows.Forms.Label
            {
                Font = new System.Drawing.Font("Segoe UI", size,
                    bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular),
                ForeColor = color,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(width, height),
                Text = text,
                TextAlign = alignment
            };
        }

        private static System.Windows.Forms.Button MakeButton(string text, int x, int y, int width, int height,
            System.Drawing.Color backColor, System.Drawing.Color foreColor)
        {
            var button = new System.Windows.Forms.Button
            {
                BackColor = backColor,
                Cursor = System.Windows.Forms.Cursors.Hand,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                ForeColor = foreColor,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(width, height),
                Text = text,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }
    }
}
