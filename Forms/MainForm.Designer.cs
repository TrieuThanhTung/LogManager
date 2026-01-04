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
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1000, 800);
        this.Text = "Log Manager";
        this.Padding = new Padding(10);
        
        // Main Container
        var mainLayout = new TableLayoutPanel();
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.RowCount = 3;
        mainLayout.ColumnCount = 1;
        // Top Config Area: AutoSize to fit content
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); 
        // Grid Area: Fill remaining space
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  
        // Status Bar: Fixed
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));  
        this.Controls.Add(mainLayout);

        // --- TOP CONFIG PANEL ---
        var topGroup = new GroupBox { Text = "Configuration", Dock = DockStyle.Fill, Padding = new Padding(10) };
        topGroup.AutoSize = true;
        mainLayout.Controls.Add(topGroup, 0, 0);

        // Split Config into Source (Left) and Filters (Right)
        var configLayout = new TableLayoutPanel();
        configLayout.Dock = DockStyle.Top; // Or Fill inside GroupBox
        configLayout.AutoSize = true;
        configLayout.RowCount = 1;
        configLayout.ColumnCount = 3; // Source | Spacer | Filters
        configLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        configLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F)); 
        configLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        topGroup.Controls.Add(configLayout);

        // 1. SOURCE PANEL (Left)
        var sourcePanel = new Panel { Dock = DockStyle.Fill, AutoSize = true };
        var sourceLayout = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, RowCount = 5, ColumnCount = 2 };
        sourceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        sourceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        
        sourceLayout.Controls.Add(new Label { Text = "Type:", Anchor = AnchorStyles.Left }, 0, 0);
        comboSourceType = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        comboSourceType.Items.AddRange(new object[] { "Local File System", "SFTP Server" });
        comboSourceType.SelectedIndex = 0;
        sourceLayout.Controls.Add(comboSourceType, 1, 0);

        sourceLayout.Controls.Add(new Label { Text = "Path:", Anchor = AnchorStyles.Left }, 0, 1);
        txtSourcePath = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Path to logs..." };
        sourceLayout.Controls.Add(txtSourcePath, 1, 1);

        chkRecursive = new CheckBox { Text = "Include Subdirectories", Dock = DockStyle.Fill, AutoSize = true };
        sourceLayout.Controls.Add(chkRecursive, 1, 2);

        btnBrowseSource = new Button { Text = "Browse", Dock = DockStyle.Right, AutoSize = true };
        sourceLayout.Controls.Add(btnBrowseSource, 1, 3);
        
        btnSettings = new Button { Text = "⚙ Settings", Dock = DockStyle.Left, AutoSize = false, BackColor = Color.WhiteSmoke, Width = 60};
        sourceLayout.Controls.Add(btnSettings, 0, 4);

        sourcePanel.Controls.Add(sourceLayout);
        configLayout.Controls.Add(sourcePanel, 0, 0);

        // 2. FILTERS PANEL (Right)
        var filterPanel = new Panel { Dock = DockStyle.Fill, AutoSize = true };
        var filterLayout = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, RowCount = 5, ColumnCount = 4 };
        // Cols: Label | Control | Label | Control
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));

        // Row 0: Date
        filterLayout.Controls.Add(new Label { Text = "From:", Anchor = AnchorStyles.Left }, 0, 0);
        dtpFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill, Checked = false, ShowCheckBox = true };
        filterLayout.Controls.Add(dtpFrom, 1, 0);
        
        filterLayout.Controls.Add(new Label { Text = "To:", Anchor = AnchorStyles.Left }, 2, 0);
        dtpTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill, Checked = false, ShowCheckBox = true };
        filterLayout.Controls.Add(dtpTo, 3, 0);

        // Row 1: MAC
        filterLayout.Controls.Add(new Label { Text = "MAC File:", Anchor = AnchorStyles.Left }, 0, 1);
        txtMacFilePath = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Path to MACs file..." };
        filterLayout.Controls.Add(txtMacFilePath, 1, 1);
        
        btnBrowseMac = new Button { Text = " Browse ", Dock = DockStyle.Fill, AutoSize = true};
        filterLayout.Controls.Add(btnBrowseMac, 2, 1); 
        filterLayout.SetColumnSpan(txtMacFilePath, 1);

        // Row 2: Options
        chkOnlyNewest = new CheckBox { Text = "Only Newest", Dock = DockStyle.Fill, AutoSize = true };
        filterLayout.Controls.Add(chkOnlyNewest, 1, 2);

        var statusPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0) };
        chkPass = new RadioButton { Text = "PASS", Checked = true, AutoSize = true };
        chkFail = new RadioButton { Text = "FAIL", Checked = false, AutoSize = true };
        statusPanel.Controls.Add(chkPass);
        statusPanel.Controls.Add(chkFail);
        filterLayout.Controls.Add(statusPanel, 3, 2);
        filterLayout.Controls.Add(new Label { Text = "Status:", Anchor = AnchorStyles.Left }, 2, 2);

        // Row 3: Action Button
        btnLoad = new Button { Text = "LOAD & FILTER", Height = 40, BackColor = Color.LightBlue, Dock = DockStyle.Fill, Font = new Font(this.Font, FontStyle.Bold) };
        filterLayout.Controls.Add(btnLoad, 3, 3);
        
        // Row 4: Export Button
        btnExport = new Button { Text = "EXPORT FILES", Height = 30, BackColor = Color.LightGreen, Dock = DockStyle.Fill, Enabled = false };
        filterLayout.Controls.Add(btnExport, 2, 3); // Span 2 cols
        filterLayout.SetColumnSpan(btnExport, 1); // Only for col 2? Actually, let's put it next to Load or below.
        
        // Re-arrange for better UX: Load on right (Row 3), Export on Row 4?
        // Let's put Load in Row 3, Col 2-3 (Span 2) and Export in Row 4, Col 2-3 (Span 2)
        filterLayout.Controls.Remove(btnLoad);
        filterLayout.Controls.Add(btnLoad, 2, 3);
        filterLayout.SetColumnSpan(btnLoad, 2);

        filterLayout.Controls.Add(btnExport, 2, 4);
        filterLayout.SetColumnSpan(btnExport, 2);

        filterPanel.Controls.Add(filterLayout);
        configLayout.Controls.Add(filterPanel, 2, 0);

        // --- GRID AREA ---
        gridLogs = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        gridLogs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width = 250 });
        gridLogs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Size (Bytes)", DataPropertyName = "Size", Width = 100 });
        gridLogs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Date", DataPropertyName = "LastModified", Width = 150 });
        gridLogs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Path", DataPropertyName = "FullPath", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        mainLayout.Controls.Add(gridLogs, 0, 1);

        // --- STATUS BAR ---
        statusStrip = new StatusStrip();
        progressBar = new ToolStripProgressBar { Width = 200 };
        lblStatus = new ToolStripStatusLabel { Text = "Ready" };
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
