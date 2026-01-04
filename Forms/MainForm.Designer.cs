namespace LogManager.Forms;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        System.Windows.Forms.DataGridViewCellStyle gridStyle = new System.Windows.Forms.DataGridViewCellStyle();

        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1100, 700);
        this.Text = "Log Manager";
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        
        // 1. Main Layout Container
        var mainLayout = new TableLayoutPanel();
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.RowCount = 3;
        mainLayout.ColumnCount = 1;
        // Rows: Config (Auto), Grid (Star), Status (Auto)
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); 
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));  
        this.Controls.Add(mainLayout);

        // --- TOP CONFIGURATION GROUP ---
        var topGroup = new GroupBox { Text = "Configuration", Dock = DockStyle.Fill, Padding = new Padding(10) };
        topGroup.AutoSize = true;
        mainLayout.Controls.Add(topGroup, 0, 0);

        // Split Config into: Source (Left) | Separator | Filters/Actions (Right)
        var configLayout = new TableLayoutPanel();
        configLayout.Dock = DockStyle.Top;
        configLayout.AutoSize = true;
        configLayout.RowCount = 1;
        configLayout.ColumnCount = 3; 
        configLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350F)); // Source (Fixed)
        configLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));  // Separator (visual buffer)
        configLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // Filters (Fill)
        topGroup.Controls.Add(configLayout);

        // === SEPARATOR ===
        // Create a visual vertical line in the middle column
        var separatorPanel = new Panel();
        separatorPanel.Dock = DockStyle.Left;
        separatorPanel.Width = 1;
        separatorPanel.BackColor = Color.LightGray;
        separatorPanel.Margin = new Padding(10, 5, 0, 5); // Center the line in the 20px column
        configLayout.Controls.Add(separatorPanel, 1, 0);

        // === LEFT COLUMN: SOURCE SETTINGS ===
        var sourcePanel = new Panel { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 0, 5, 0) };
        var sourceLayout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, RowCount = 4, ColumnCount = 3 };
        sourceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F)); // Label
        sourceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Input
        sourceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F)); // Button (Browse)
        
        // Row 0: Type
        sourceLayout.Controls.Add(new Label { Text = "Source:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
        comboSourceType = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Height = 30 };
        comboSourceType.Items.AddRange(new object[] { "Local File System", "SFTP Server" });
        comboSourceType.SelectedIndex = 0;
        sourceLayout.Controls.Add(comboSourceType, 1, 0);
        sourceLayout.SetColumnSpan(comboSourceType, 2);
        
        // Row 1: Path
        sourceLayout.Controls.Add(new Label { Text = "Path:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
        txtSourcePath = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Select directory path...", Height = 30 };
        sourceLayout.Controls.Add(txtSourcePath, 1, 1);
        btnBrowseSource = new Button { Text = "...", Dock = DockStyle.Fill, Height = 28, Margin = new Padding(1) };
        sourceLayout.Controls.Add(btnBrowseSource, 2, 1);
        
        // Row 2: Recursive
        chkRecursive = new CheckBox { Text = "Include Subdirectories", Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(80, 0, 0, 0) };
        sourceLayout.Controls.Add(chkRecursive, 0, 2);
        sourceLayout.SetColumnSpan(chkRecursive, 3);
        
        // Row 3: Settings Button
        btnSettings = new Button { Text = "Settings", Width = 120, Height = 30, BackColor = Color.WhiteSmoke, Anchor = AnchorStyles.Left };
        sourceLayout.Controls.Add(btnSettings, 0, 3);
        
        sourcePanel.Controls.Add(sourceLayout);
        configLayout.Controls.Add(sourcePanel, 0, 0);

        // === RIGHT COLUMN: FILTERS & ACTIONS ===
        var filterPanel = new Panel { Dock = DockStyle.Fill, AutoSize = true };
        var filterLayout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, RowCount = 4, ColumnCount = 4 };
        // Cols: Label | Input | Label | Input
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        // Row 0: Date
        filterLayout.Controls.Add(new Label { Text = "From:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
        dtpFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill, ShowCheckBox = true };
        filterLayout.Controls.Add(dtpFrom, 1, 0);

        filterLayout.Controls.Add(new Label { Text = "To:", Anchor = AnchorStyles.Left, AutoSize = true }, 2, 0);
        dtpTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill, ShowCheckBox = true };
        filterLayout.Controls.Add(dtpTo, 3, 0);

        // Row 1: Add extra spacing for logical separation
        // Let's use Row 1 for MAC
        filterLayout.Controls.Add(new Label { Text = "MAC:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
        
        var macPanel = new TableLayoutPanel { Dock = DockStyle.Fill, Margin = new Padding(0), ColumnCount = 2, RowCount = 1, Height = 30};
        macPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        macPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
        txtMacFilePath = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "MAC Address filter file..." };
        btnBrowseMac = new Button { Text = "...", Dock = DockStyle.None, Anchor = AnchorStyles.Top, Size = new Size(30, 28), Margin = new Padding(0) };
        macPanel.Controls.Add(txtMacFilePath, 0, 0);
        macPanel.Controls.Add(btnBrowseMac, 1, 0);
        
        filterLayout.Controls.Add(macPanel, 1, 1);
        filterLayout.SetColumnSpan(macPanel, 3);

        // Row 2: Status & Options
        filterLayout.Controls.Add(new Label { Text = "Status:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 2);
        var statusFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0) };
        chkPass = new RadioButton { Text = "PASS", AutoSize = true, Checked = true };
        chkFail = new RadioButton { Text = "FAIL", AutoSize = true };
        statusFlow.Controls.AddRange(new Control[] { chkPass, chkFail });
        filterLayout.Controls.Add(statusFlow, 1, 2);

        chkOnlyNewest = new CheckBox { Text = "Only Newest Logs", AutoSize = true, Anchor = AnchorStyles.Left };
        filterLayout.Controls.Add(chkOnlyNewest, 3, 2);

        // Row 3: Action Buttons (Big)
        var actionsPanel = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 1, Height = 40, Margin = new Padding(0, 10, 0, 0) };
        actionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        actionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        
        btnLoad = new Button { Text = "LOAD LOGS", Dock = DockStyle.Fill, Height = 45, BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,  Font = new Font("Segoe UI", 10F, FontStyle.Bold), Margin = new Padding(0, 0, 0, 0) };
        btnLoad.FlatAppearance.BorderSize = 0;
        
        btnExport = new Button { Text = "EXPORT DATA", Dock = DockStyle.Fill, Height = 45, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Enabled = false, Margin = new Padding(10, 0, 0, 0) };
        btnExport.FlatAppearance.BorderSize = 0;

        actionsPanel.Controls.Add(btnLoad, 0, 0);
        actionsPanel.Controls.Add(btnExport, 1, 0);

        filterLayout.Controls.Add(actionsPanel, 0, 3);
        filterLayout.SetColumnSpan(actionsPanel, 4);

        filterPanel.Controls.Add(filterLayout);
        configLayout.Controls.Add(filterPanel, 2, 0);

        // 2. GRID AREA
        gridLogs = new DataGridView();
        gridLogs.Dock = DockStyle.Fill;
        gridLogs.BackgroundColor = Color.White;
        gridLogs.BorderStyle = BorderStyle.None;
        gridLogs.RowHeadersVisible = false;
        gridLogs.AllowUserToAddRows = false;
        gridLogs.ReadOnly = true;
        gridLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        gridLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        
        // Grid Styles
        gridLogs.EnableHeadersVisualStyles = false;
        gridLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        gridLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
        gridLogs.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        gridLogs.ColumnHeadersHeight = 35;
        gridLogs.RowTemplate.Height = 30;
        gridLogs.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

        gridLogs.AutoGenerateColumns = false;
        gridLogs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Log Name", DataPropertyName = "Name", FillWeight = 30 });
        gridLogs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Size", DataPropertyName = "Size", FillWeight = 10 });
        gridLogs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Date Modified", DataPropertyName = "LastModified", FillWeight = 15 });
        gridLogs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Full Path", DataPropertyName = "FullPath", FillWeight = 45 });

        mainLayout.Controls.Add(gridLogs, 0, 1);

        // 3. STATUS BAR
        statusStrip = new StatusStrip();
        statusStrip.Padding = new Padding(1, 0, 14, 0); // Default weird padding fix
        lblStatus = new ToolStripStatusLabel { Text = "Ready", Spring = true, TextAlign = ContentAlignment.MiddleLeft };
        progressBar = new ToolStripProgressBar { Width = 200, Visible = false };
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, progressBar });
        mainLayout.Controls.Add(statusStrip, 0, 2);
    }

    #endregion

    // Controls
    public ComboBox comboSourceType;
    public TextBox txtSourcePath;
    public CheckBox chkRecursive;
    public Button btnBrowseSource;
    public Button btnSettings;
    public CheckBox chkOnlyNewest;
    public DateTimePicker dtpFrom;
    public DateTimePicker dtpTo;
    public TextBox txtMacFilePath;
    public Button btnBrowseMac;
    public RadioButton chkPass;
    public RadioButton chkFail;
    public Button btnLoad;
    public Button btnExport;
    public DataGridView gridLogs;
    public StatusStrip statusStrip;
    public ToolStripProgressBar progressBar;
    public ToolStripStatusLabel lblStatus;
}
