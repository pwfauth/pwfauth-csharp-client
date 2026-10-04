using PWFAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PWFAuthCSharp
{
    [System.ComponentModel.DesignerCategory("Form")]
    public partial class Form1 : Form
    {
        private static readonly Color ErrorColor = Color.FromArgb(248, 113, 113);
        private static readonly Color MutedColor = Color.FromArgb(148, 163, 184);

        private readonly Dictionary<string, string> _fieldDisplayNames =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "success", "Request successful" },
                { "message", "Server message" },
                { "error_code", "Error code" },
                { "session_id", "Session ID" },
                { "heartbeat_interval", "Heartbeat interval" },
                { "user.license_key", "License key" },
                { "user.key_type", "Key type" },
                { "user.duration", "Key duration" },
                { "user.hwid", "Device HWID" },
                { "user.activated_at", "Activation date" },
                { "user.expires_at", "Expiration date" },
                { "user.days_remaining", "Days remaining" },
                { "user.status", "Key status" },
                { "seller.type", "Sold by (type)" },
                { "seller.name", "Sold by" },
                { "seller.contact", "Seller contact" },
                { "app.name", "Application name" },
                { "app.version", "Application version" },
                { "app.message", "Application message" },
                { "texts", "Remote texts" },
                { "features", "Key features" },
                { "slides", "Announcement slides" }
            };

        private PwfClient _client;
        private bool _isLoggingIn;
        private bool _allowClose;

        public Form1()
        {
            InitializeComponent();
            BuildLoginInterface();
            BuildDashboardInterface();
            Controls.Add(pnlDashboard);
            Controls.Add(pnlLoginPage);
            AcceptButton = btnLogin;
            CenterLoginCard();
            txtLicenseKey.Text = string.Empty;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (_isLoggingIn)
                return;

            string licenseKey = txtLicenseKey.Text.Trim();
            if (licenseKey.Length == 0)
            {
                SetLoginMessage("Enter your license key first.", true);
                txtLicenseKey.Focus();
                return;
            }

            await SignInAsync(licenseKey, false);
        }

        // Shared by Sign In and "Move this license to this PC". moveFirst = the user
        // agreed to unbind the key from the computer it is bound to, then sign in here.
        private async Task SignInAsync(string licenseKey, bool moveFirst)
        {
            string appSecret = GetAppSecret();
            if (string.IsNullOrWhiteSpace(appSecret))
            {
                SetLoginMessage("The application secret is not configured. Set PWFAuthAppSecret in App.config, then restart the application.", true);
                return;
            }

            SetLoginBusy(true);
            SetMoveLicenseVisible(false);
            SetLoginMessage(moveFirst
                ? "Moving the license to this computer..."
                : "Verifying the key and connecting to the server...", false);

            PwfClient pendingClient = null;
            bool loginCompleted = false;
            try
            {
                pendingClient = new PwfClient(new PwfClientOptions { AppSecret = appSecret, BaseUrl = GetBaseUrl() });
                pendingClient.SessionEnded += Client_SessionEnded;

                if (moveFirst)
                {
                    // Self-service move: unbinds the key from every computer it is bound
                    // to, so the login below binds it here. The developer's cooldown
                    // (12 hours by default) applies between two moves.
                    PwfResponse moved = await pendingClient.ResetHardwareIdAsync(licenseKey, "Moved with the C# desktop client");
                    if (!moved.Success)
                    {
                        SetLoginMessage(Describe(moved, "The license could not be moved."), true);
                        return;
                    }
                }

                PwfResponse login = await pendingClient.LoginAsync(licenseKey);
                if (!login.Success)
                {
                    SetLoginMessage(Describe(login, "Sign-in failed."), true);
                    // Bound to another computer: offer the self-service move.
                    if (login.ErrorCode == PwfErrorCodes.HwidMismatch || login.ErrorCode == PwfErrorCodes.DeviceLimit)
                    {
                        SetLoginMessage("This key is bound to another computer (" + login.ErrorCode + ").\r\n" +
                            "You can move it here — it then stops working on the other one.", true);
                        SetMoveLicenseVisible(true);
                    }
                    return;
                }

                _client = pendingClient;
                pendingClient = null;
                PopulateDashboard(login);
                // Started on the UI thread, so SessionEnded is raised on the UI thread too.
                _client.StartHeartbeat();
                ShowDashboard();
                loginCompleted = true;
            }
            catch (PwfHttpException ex) when (ex.StatusCode == 401)
            {
                SetLoginMessage("The license server refused the application secret (HTTP 401). Check PWFAuthAppSecret in App.config.", true);
            }
            catch (PwfCryptoException ex)
            {
                SetLoginMessage("The encrypted response could not be verified. Check the application secret.\r\n" + ex.Message, true);
            }
            catch (PwfHttpException ex)
            {
                SetLoginMessage("The license server did not return a valid response (HTTP " + ex.StatusCode + ").\r\n" + ex.Message, true);
            }
            catch (PwfException ex)
            {
                SetLoginMessage(ex.Message, true);
            }
            catch (HttpRequestException)
            {
                // No connection at all: offline, DNS, firewall, proxy or TLS.
                SetLoginMessage("Cannot reach the license server. Check your internet connection and try again.", true);
            }
            catch (Exception ex)
            {
                SetLoginMessage("An unexpected error occurred: " + ex.Message, true);
            }
            finally
            {
                if (pendingClient != null)
                {
                    pendingClient.SessionEnded -= Client_SessionEnded;
                    pendingClient.Dispose();
                }

                if (!loginCompleted && _client != null)
                    ReleaseClient(_client);

                SetLoginBusy(false);
            }
        }

        private async void btnMoveLicense_Click(object sender, EventArgs e)
        {
            if (_isLoggingIn)
                return;

            string licenseKey = txtLicenseKey.Text.Trim();
            if (licenseKey.Length == 0)
                return;

            await SignInAsync(licenseKey, true);
        }

        // The server's message (safe to show the user) plus its error code.
        private static string Describe(PwfResponse response, string fallback)
        {
            string code = string.IsNullOrWhiteSpace(response.ErrorCode)
                ? string.Empty
                : " (" + response.ErrorCode + ")";
            return (response.Message ?? fallback) + code;
        }

        private async void btnLogout_Click(object sender, EventArgs e)
        {
            btnLogout.Enabled = false;
            lblOnline.Text = "Closing session...";
            lblOnline.ForeColor = Color.FromArgb(217, 119, 6);

            PwfClient client = _client;
            try
            {
                if (client != null && client.IsSignedIn)
                    await client.LogoutAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                SetLoginMessage("You were signed out locally, but the server could not be notified: " + ex.Message, true);
            }
            finally
            {
                ReleaseClient(client);
                ShowLoginPage();
                btnLogout.Enabled = true;
            }
        }

        // The heartbeat ended the session: the key was banned, paused, expired, reset or
        // revoked, maintenance started, or the server could not be reached for several
        // beats in a row. PWFAuth raises this on the UI thread (StartHeartbeat was called
        // there); the InvokeRequired check is only a safety net.
        private void Client_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            if (IsDisposed || Disposing)
                return;

            Action endSession = delegate
            {
                PwfClient endedClient = _client;
                ReleaseClient(endedClient);
                ShowLoginPage();
                SetLoginMessage("Session ended: " + e.Message + " (" + e.ErrorCode + ")", true);
            };

            if (InvokeRequired)
                BeginInvoke(endSession);
            else
                endSession();
        }

        private void PopulateDashboard(PwfResponse login)
        {
            JsonElement user;
            JsonElement app;
            bool hasUser = login.TryGetProperty("user", out user) && user.ValueKind == JsonValueKind.Object;
            bool hasApp = login.TryGetProperty("app", out app) && app.ValueKind == JsonValueKind.Object;

            JsonElement seller;
            bool hasSeller = login.TryGetProperty("seller", out seller) && seller.ValueKind == JsonValueKind.Object;

            string appName = hasApp ? ReadString(app, "name") : null;
            string appVersion = hasApp ? ReadString(app, "version") : null;
            string appMessage = hasApp ? ReadString(app, "message") : null;
            string sellerName = hasSeller ? ReadString(seller, "name") : null;
            string licenseKey = hasUser ? ReadString(user, "license_key") : _client.LicenseKey;
            string status = hasUser ? ReadString(user, "status") : null;
            string keyType = hasUser ? ReadString(user, "key_type") : null;
            string duration = hasUser ? ReadValue(user, "duration") : null;
            string expiresAt = hasUser ? ReadString(user, "expires_at") : null;
            string daysRemaining = hasUser ? ReadValue(user, "days_remaining") : null;

            lblDashboardTitle.Text = string.IsNullOrWhiteSpace(appName)
                ? "License Information"
                : appName + " — License Information";
            lblDashboardSubtitle.Text = BuildSubtitle(licenseKey, appVersion, sellerName, appMessage);
            lblStatusValue.Text = TranslateStatus(status);
            lblTypeValue.Text = TranslateKeyType(keyType, duration);
            lblExpiryValue.Text = FormatDateOrLifetime(expiresAt);
            lblRemainingValue.Text = string.IsNullOrWhiteSpace(daysRemaining)
                ? "Lifetime"
                : daysRemaining + " days";

            lblOnline.Text = "● Session active and protected";
            lblOnline.ForeColor = Color.FromArgb(22, 163, 74);
            lblSessionInfo.Text = string.Format(
                "Device ID: {0}   •   Heartbeat every {1} seconds   •   Session ID: {2}",
                _client.HardwareId,
                _client.HeartbeatIntervalSeconds,
                _client.SessionId);

            dgvDetails.Rows.Clear();
            AddJsonRows(login.Root, string.Empty);

            try
            {
                txtRawJson.Text = JsonSerializer.Serialize(
                    login.Root,
                    new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                txtRawJson.Text = login.RawJson;
            }
        }

        private void AddJsonRows(JsonElement element, string path)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                bool hadChildren = false;
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    hadChildren = true;
                    string childPath = string.IsNullOrEmpty(path)
                        ? property.Name
                        : path + "." + property.Name;
                    AddJsonRows(property.Value, childPath);
                }

                if (!hadChildren && !string.IsNullOrEmpty(path))
                    AddDetailRow(path, "{ }");
                return;
            }

            if (element.ValueKind == JsonValueKind.Array)
            {
                int index = 0;
                foreach (JsonElement item in element.EnumerateArray())
                {
                    AddJsonRows(item, path + "[" + index + "]");
                    index++;
                }

                if (index == 0)
                    AddDetailRow(path, "[ ]");
                return;
            }

            AddDetailRow(path, FormatJsonValue(path, element));
        }

        private void AddDetailRow(string path, string value)
        {
            string normalizedPath = RemoveArrayIndexes(path);
            string displayName;
            if (!_fieldDisplayNames.TryGetValue(path, out displayName) &&
                !_fieldDisplayNames.TryGetValue(normalizedPath, out displayName))
            {
                int dot = normalizedPath.LastIndexOf('.');
                displayName = dot >= 0 ? normalizedPath.Substring(dot + 1) : normalizedPath;
            }

            int rowIndex = dgvDetails.Rows.Add(displayName, path, value);
            DataGridViewRow row = dgvDetails.Rows[rowIndex];
            row.Cells[1].Style.Font = new Font("Consolas", 9F);
            row.Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            row.Cells[1].Style.WrapMode = DataGridViewTriState.False;
            row.Cells[2].Style.Alignment = ContainsArabic(value)
                ? DataGridViewContentAlignment.MiddleRight
                : DataGridViewContentAlignment.MiddleLeft;
        }

        private string FormatJsonValue(string path, JsonElement value)
        {
            if (value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined)
            {
                if (path.EndsWith("expires_at", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith("days_remaining", StringComparison.OrdinalIgnoreCase))
                    return "Lifetime (null)";

                return "null";
            }

            if (value.ValueKind == JsonValueKind.String)
            {
                string text = value.GetString();
                if (path.EndsWith("_at", StringComparison.OrdinalIgnoreCase))
                    return FormatDateWithUtc(text);

                return text ?? string.Empty;
            }

            if (value.ValueKind == JsonValueKind.True)
                return "true (Yes)";
            if (value.ValueKind == JsonValueKind.False)
                return "false (No)";

            return value.GetRawText();
        }

        private static string BuildSubtitle(string licenseKey, string appVersion, string sellerName, string appMessage)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(licenseKey))
                parts.Add(licenseKey);
            if (!string.IsNullOrWhiteSpace(appVersion))
                parts.Add("Version " + appVersion);
            if (!string.IsNullOrWhiteSpace(sellerName))
                parts.Add("Sold by " + sellerName);
            if (!string.IsNullOrWhiteSpace(appMessage))
                parts.Add(appMessage);
            return string.Join("  •  ", parts);
        }

        private static string ReadString(JsonElement parent, string propertyName)
        {
            JsonElement value;
            if (!parent.TryGetProperty(propertyName, out value) || value.ValueKind != JsonValueKind.String)
                return null;

            return value.GetString();
        }

        private static string ReadValue(JsonElement parent, string propertyName)
        {
            JsonElement value;
            if (!parent.TryGetProperty(propertyName, out value) ||
                value.ValueKind == JsonValueKind.Null ||
                value.ValueKind == JsonValueKind.Undefined)
                return null;

            return value.ValueKind == JsonValueKind.String ? value.GetString() : value.GetRawText();
        }

        private static string TranslateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "Active";

            switch (status.Trim().ToLowerInvariant())
            {
                case "active": return "Active";
                case "paused": return "Paused";
                case "expired": return "Expired";
                case "banned": return "Banned";
                default: return status;
            }
        }

        private static string TranslateKeyType(string keyType, string duration)
        {
            string translated;
            switch ((keyType ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "days": translated = "days"; break;
                case "hours": translated = "hours"; break;
                case "months": translated = "months"; break;
                case "years": translated = "years"; break;
                case "lifetime": return "Lifetime";
                default: translated = string.IsNullOrWhiteSpace(keyType) ? "Not specified" : keyType; break;
            }

            return string.IsNullOrWhiteSpace(duration) ? translated : duration + " " + translated;
        }

        private static string FormatDateOrLifetime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Never expires";

            DateTime parsed;
            return DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsed)
                ? parsed.ToLocalTime().ToString("yyyy/MM/dd  HH:mm")
                : value;
        }

        private static string FormatDateWithUtc(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value ?? string.Empty;

            DateTime parsed;
            if (!DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsed))
                return value;

            return string.Format(
                "{0:yyyy/MM/dd HH:mm:ss} Local  |  {1:yyyy/MM/dd HH:mm:ss} UTC",
                parsed.ToLocalTime(), parsed.ToUniversalTime());
        }

        private static string RemoveArrayIndexes(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            var result = new System.Text.StringBuilder(path.Length);
            bool insideIndex = false;
            foreach (char character in path)
            {
                if (character == '[')
                {
                    insideIndex = true;
                    continue;
                }
                if (character == ']')
                {
                    insideIndex = false;
                    continue;
                }
                if (!insideIndex)
                    result.Append(character);
            }
            return result.ToString();
        }

        private static bool ContainsArabic(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            foreach (char character in value)
            {
                if (character >= '\u0600' && character <= '\u06FF')
                    return true;
            }
            return false;
        }

        private static string GetAppSecret()
        {
            string fromEnvironment = Environment.GetEnvironmentVariable("PWFAUTH_APP_SECRET");
            if (!string.IsNullOrWhiteSpace(fromEnvironment))
                return fromEnvironment.Trim();

            string fromConfig = ConfigurationManager.AppSettings["PWFAuthAppSecret"];
            // The placeholder shipped in App.config counts as "not configured".
            if (string.IsNullOrWhiteSpace(fromConfig) || fromConfig.Trim() == "YOUR_64_CHARACTER_APP_SECRET")
                return null;
            return fromConfig.Trim();
        }

        // Optional: another server, e.g. a staging copy. Empty = https://pwfauth.com.
        private static string GetBaseUrl()
        {
            string fromEnvironment = Environment.GetEnvironmentVariable("PWFAUTH_BASE_URL");
            if (!string.IsNullOrWhiteSpace(fromEnvironment))
                return fromEnvironment.Trim();

            string fromConfig = ConfigurationManager.AppSettings["PWFAuthBaseUrl"];
            return string.IsNullOrWhiteSpace(fromConfig) ? "https://pwfauth.com" : fromConfig.Trim();
        }

        private void SetMoveLicenseVisible(bool visible)
        {
            btnMoveLicense.Visible = visible;
        }

        private void ShowDashboard()
        {
            SetLoginMessage(string.Empty, false);
            pnlLoginPage.Visible = false;
            pnlDashboard.Visible = true;
            pnlDashboard.BringToFront();
            AcceptButton = null;
            Text = "PWF Auth - License Information";
        }

        private void ShowLoginPage()
        {
            pnlDashboard.Visible = false;
            pnlLoginPage.Visible = true;
            pnlLoginPage.BringToFront();
            AcceptButton = btnLogin;
            Text = "PWF Auth - Sign In";
            txtLicenseKey.SelectAll();
            txtLicenseKey.Focus();
            CenterLoginCard();
        }

        private void SetLoginBusy(bool busy)
        {
            _isLoggingIn = busy;
            txtLicenseKey.Enabled = !busy;
            btnLogin.Enabled = !busy;
            btnLogin.Text = busy ? "Verifying..." : "Sign In";
            loginProgress.Visible = busy;
            UseWaitCursor = busy;
        }

        private void SetLoginMessage(string message, bool isError)
        {
            lblLoginStatus.Text = message ?? string.Empty;
            lblLoginStatus.ForeColor = isError ? ErrorColor : MutedColor;
        }

        private void ReleaseClient(PwfClient client)
        {
            if (client == null)
                return;

            if (ReferenceEquals(_client, client))
                _client = null;

            client.SessionEnded -= Client_SessionEnded;
            client.StopHeartbeat();
            client.Dispose();
        }

        private void btnCopyJson_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRawJson.Text))
                return;

            try
            {
                Clipboard.SetText(txtRawJson.Text);
                btnCopyJson.Text = "Copied ✓";
                var timer = new System.Windows.Forms.Timer { Interval = 1600 };
                timer.Tick += delegate
                {
                    btnCopyJson.Text = "Copy JSON";
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not copy the JSON: " + ex.Message, "PWF Auth",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void pnlLoginPage_Resize(object sender, EventArgs e)
        {
            CenterLoginCard();
        }

        private void CenterLoginCard()
        {
            if (pnlLoginPage == null || pnlLoginCard == null)
                return;

            pnlLoginCard.Left = Math.Max(0, (pnlLoginPage.ClientSize.Width - pnlLoginCard.Width) / 2);
            pnlLoginCard.Top = Math.Max(0, (pnlLoginPage.ClientSize.Height - pnlLoginCard.Height) / 2);
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_allowClose && _client != null && _client.IsSignedIn &&
                e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Enabled = false;
                PwfClient client = _client;
                try
                {
                    using (var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
                        await client.LogoutAsync(timeout.Token);
                }
                catch
                {
                    // Closing must continue even when the server is unreachable.
                }
                finally
                {
                    ReleaseClient(client);
                    _allowClose = true;
                    Enabled = true;
                    Close();
                }
                return;
            }

            if (_client != null)
                ReleaseClient(_client);

            base.OnFormClosing(e);
        }
    }
}
