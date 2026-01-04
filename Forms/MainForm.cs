using LogManager.Interfaces;
using LogManager.Services; // For LocalFileProvider / SftpFileProvider
using Microsoft.Extensions.DependencyInjection;

namespace LogManager.Forms;

public partial class MainForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogFilterStrategy _filterStrategy;
    private readonly MacAddressProvider _macProvider;
    
    private IFileSourceProvider? _currentProvider;

    // Constructor for Designer support
    public MainForm()
    {
        InitializeComponent();
    }

    public MainForm(IServiceProvider serviceProvider, ILogFilterStrategy filterStrategy, MacAddressProvider macProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _filterStrategy = filterStrategy;
        _macProvider = macProvider;

        // Bind Events
        btnBrowseSource.Click += BtnBrowseSource_Click;
        btnBrowseMac.Click += BtnBrowseMac_Click;
        btnLoad.Click += BtnLoad_Click;
        btnSettings.Click += BtnSettings_Click;
        btnExport.Click += BtnExport_Click;
        comboSourceType.SelectedIndexChanged += ComboSourceType_SelectedIndexChanged;
    }

    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        using var scope = _serviceProvider.CreateScope();
        var form = scope.ServiceProvider.GetRequiredService<SettingsForm>();
        // Note: For SFTP provider to pick up changes without app restart, we might need to manually refresh config or re-create provider.
        // Since SftpFileProvider is Transient and created on "Load" click, it will pick up new IOptions snapshot if configuration is reloaded.
        if (form.ShowDialog() == DialogResult.OK)
        {
            // Settings saved.
        }
    }

    private async void BtnExport_Click(object? sender, EventArgs e)
    {
        var filesToExport = gridLogs.DataSource as List<FileMetadata>;
        if (filesToExport == null || !filesToExport.Any())
        {
            MessageBox.Show("No files to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new FolderBrowserDialog();
        dlg.Description = "Select Destination Folder";
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            var destPath = dlg.SelectedPath;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
            progressBar.Maximum = filesToExport.Count;
            lblStatus.Text = "Exporting...";
            btnExport.Enabled = false;

            try
            {
                int count = 0;
                foreach (var file in filesToExport)
                {
                    var destFile = Path.Combine(destPath, file.Name);
                    // Handle duplicate names? Append timestamp or random?
                    // For now, overwrite or simple copy.
                    
                    using var sourceStream = await _currentProvider!.GetFileStreamAsync(file.FullPath); // Using current provider state
                    using var destStream = new FileStream(destFile, FileMode.Create, FileAccess.Write);
                    await sourceStream.CopyToAsync(destStream);
                    
                    count++;
                    progressBar.Value = count;
                    lblStatus.Text = $"Exported {count}/{filesToExport.Count}: {file.Name}";
                }
                MessageBox.Show($"Successfully exported {count} files.", "Export Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = "Export Complete.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Export Failed.";
            }
            finally
            {
                btnExport.Enabled = true;
                progressBar.Value = 0;
                progressBar.Style = ProgressBarStyle.Blocks;
            }
        }
    }

    private void ComboSourceType_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // Adjust UI if needed (enable/disable fields)
        bool isLocal = comboSourceType.SelectedIndex == 0;
        txtSourcePath.PlaceholderText = isLocal ? "Local Directory Path" : "Remote Directory Path (Leave empty for default)";
    }

    private void BtnBrowseSource_Click(object? sender, EventArgs e)
    {
        if (comboSourceType.SelectedIndex == 0) // Local
        {
            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtSourcePath.Text = dlg.SelectedPath;
            }
        }
        else
        {
             // Maybe open settings?
             MessageBox.Show("Please use Settings to configure SFTP connection details.", "SFTP", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void BtnBrowseMac_Click(object? sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog();
        dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            txtMacFilePath.Text = dlg.FileName;
        }
    }

    private async void BtnLoad_Click(object? sender, EventArgs e)
    {
        try
        {
            btnLoad.Enabled = false;
            btnExport.Enabled = false; // Disable until loaded
            progressBar.Style = ProgressBarStyle.Marquee;
            lblStatus.Text = "Loading files...";

            // 1. Resolve Provider
            if (comboSourceType.SelectedIndex == 0)
            {
                _currentProvider = _serviceProvider.GetRequiredService<LocalFileProvider>();
            }
            else
            {
                _currentProvider = _serviceProvider.GetRequiredService<SftpFileProvider>();
            }

            // 2. Get Files
            var path = txtSourcePath.Text;
            var recursive = chkRecursive.Checked;
            var files = await _currentProvider.GetFilesAsync(path, recursive);

            lblStatus.Text = $"Found {files.Count()} files. Filtering...";

            // 3. Prepare Filter Options
            var options = new FilterOptions
            {
                OnlyNewest = chkOnlyNewest.Checked,
                PassStatus = (chkPass.Checked && chkFail.Checked) ? null : (chkPass.Checked ? true : (chkFail.Checked ? false : null)), 
            };
            
            if (!chkPass.Checked && !chkFail.Checked)
            {
                MessageBox.Show("Please select at least one status (PASS or FAIL).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnLoad.Enabled = true;
                return;
            }
            if (chkPass.Checked && chkFail.Checked) options.PassStatus = null;
            else if (chkPass.Checked) options.PassStatus = true;
            else options.PassStatus = false;


            if (dtpFrom.Checked) options.FromDate = dtpFrom.Value;
            if (dtpTo.Checked) options.ToDate = dtpTo.Value;

            if (!string.IsNullOrWhiteSpace(txtMacFilePath.Text))
            {
                // Load MACs
                lblStatus.Text = "Loading MAC addresses...";
                options.MacAddresses = await _macProvider.GetMacAddressesAsync(txtMacFilePath.Text);
            }

            // 4. Apply Filter
            var filteredFiles = await _filterStrategy.ApplyFilterAsync(files, options);

            // 5. Display
            gridLogs.DataSource = filteredFiles.ToList();
            lblStatus.Text = $"Done. Found {gridLogs.Rows.Count} files.";
            
            if (gridLogs.Rows.Count > 0)
            {
                btnExport.Enabled = true;
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Error.";
        }
        finally
        {
            btnLoad.Enabled = true;
            progressBar.Style = ProgressBarStyle.Blocks;
        }
    }
}
