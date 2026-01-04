using LogManager.Models;
using LogManager.Services;

namespace LogManager.Forms;

public partial class SettingsForm : Form
{
    private readonly IConfigManager _configManager;
    private AppConfiguration _currentConfig;

    private TextBox txtHost;
    private NumericUpDown numPort;
    private TextBox txtUser;
    private TextBox txtPass;
    private TextBox txtRemotePath;
    private TextBox txtPassPattern;
    private TextBox txtFailPattern;
    private Button btnSave;
    private Button btnCancel;

    // Constructor for Designer support
    public SettingsForm()
    {
        InitializeComponent();
    }

    public SettingsForm(IConfigManager configManager)
    {
        _configManager = configManager;
        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        this.Text = "Settings";
        this.Size = new Size(500, 450);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        var mainLayout = new TableLayoutPanel();
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.RowCount = 2; // Content, Buttons
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        mainLayout.Padding = new Padding(10);
        this.Controls.Add(mainLayout);

        // Content
        var contentGroup = new GroupBox { Text = "Configuration", Dock = DockStyle.Fill };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 8, AutoSize = true };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        contentGroup.Controls.Add(layout);
        mainLayout.Controls.Add(contentGroup, 0, 0);

        // SFTP
        AddRow(layout, 0, "SFTP Host:", txtHost = new TextBox());
        AddRow(layout, 1, "SFTP Port:", numPort = new NumericUpDown { Minimum = 1, Maximum = 65535, Value = 22 });
        AddRow(layout, 2, "Username:", txtUser = new TextBox());
        AddRow(layout, 3, "Password:", txtPass = new TextBox { PasswordChar = '*' });
        AddRow(layout, 4, "Remote Path:", txtRemotePath = new TextBox());

        // Separator or Spacer
        layout.Controls.Add(new Label { Text = "--- Regex Patterns ---", AutoSize = true, ForeColor = Color.Gray }, 0, 5);
        layout.SetColumnSpan(layout.GetControlFromPosition(0, 5), 2);

        // Patterns
        AddRow(layout, 6, "Pattern:", txtPassPattern = new TextBox());
        AddRow(layout, 7, "", txtFailPattern = new TextBox());

        // Buttons
        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel };
        btnSave = new Button { Text = "Save", DialogResult = DialogResult.OK, BackColor = Color.LightBlue };
        
        btnSave.Click += BtnSave_Click;

        buttonPanel.Controls.Add(btnCancel);
        buttonPanel.Controls.Add(btnSave);
        mainLayout.Controls.Add(buttonPanel, 0, 1);
    }

    private void AddRow(TableLayoutPanel panel, int row, string label, Control control)
    {
        control.Dock = DockStyle.Fill;
        panel.Controls.Add(new Label { Text = label, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, row);
        panel.Controls.Add(control, 1, row);
    }

    private void LoadSettings()
    {
        _currentConfig = _configManager.LoadConfig();
        
        txtHost.Text = _currentConfig.Sftp.Host;
        numPort.Value = _currentConfig.Sftp.Port;
        txtUser.Text = _currentConfig.Sftp.Username;
        txtPass.Text = _currentConfig.Sftp.Password;
        txtRemotePath.Text = _currentConfig.Sftp.RemoteLogPath;
        
        txtPassPattern.Text = _currentConfig.PassPattern;
        txtFailPattern.Text = _currentConfig.FailPattern;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        _currentConfig.Sftp.Host = txtHost.Text;
        _currentConfig.Sftp.Port = (int)numPort.Value;
        _currentConfig.Sftp.Username = txtUser.Text;
        _currentConfig.Sftp.Password = txtPass.Text;
        _currentConfig.Sftp.RemoteLogPath = txtRemotePath.Text;
        
        _currentConfig.PassPattern = txtPassPattern.Text;
        _currentConfig.FailPattern = txtFailPattern.Text;

        try
        {
            _configManager.SaveConfig(_currentConfig);
            MessageBox.Show("Settings saved successfully!", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
