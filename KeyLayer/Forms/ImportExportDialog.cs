using KeyLayer.Services;

namespace KeyLayer.Forms;

public partial class ImportExportDialog : Form
{
    private readonly MacroService _macroService;
    private TabControl _tabControl;
    private Button _closeButton;

    public ImportExportDialog(MacroService macroService)
    {
        _macroService = macroService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Import/Export Manager";
        Size = new Size(600, 500);
        StartPosition = FormStartPosition.CenterParent;

        _tabControl = new TabControl
        {
            Dock = DockStyle.Fill
        };

        CreateImportTab();
        CreateExportTab();
        CreateTemplateTab();

        _closeButton = new Button
        {
            Text = "Close",
            Location = new Point(500, 450),
            Size = new Size(80, 30),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            DialogResult = DialogResult.OK
        };

        Controls.AddRange(new Control[] { _tabControl, _closeButton });
    }

    private void CreateImportTab()
    {
        var importTab = new TabPage("Import");
        
        var importButton = new Button
        {
            Text = "Import Macro",
            Location = new Point(10, 10),
            Size = new Size(120, 30)
        };
        importButton.Click += (s, e) => {
            var importForm = new MacroImportForm();
            importForm.ShowDialog();
        };

        var importProfileButton = new Button
        {
            Text = "Import Profile",
            Location = new Point(140, 10),
            Size = new Size(120, 30)
        };

        var importFromUrlButton = new Button
        {
            Text = "Import from URL",
            Location = new Point(270, 10),
            Size = new Size(120, 30)
        };

        importTab.Controls.AddRange(new Control[] { 
            importButton, importProfileButton, importFromUrlButton 
        });

        _tabControl.TabPages.Add(importTab);
    }

    private void CreateExportTab()
    {
        var exportTab = new TabPage("Export");
        
        var exportButton = new Button
        {
            Text = "Export Macro",
            Location = new Point(10, 10),
            Size = new Size(120, 30)
        };
        exportButton.Click += (s, e) => {
            var exportForm = new MacroExportForm(new List<Models.MacroAction>());
            exportForm.ShowDialog();
        };

        var exportProfileButton = new Button
        {
            Text = "Export Profile",
            Location = new Point(140, 10),
            Size = new Size(120, 30)
        };

        var shareOnlineButton = new Button
        {
            Text = "Share Online",
            Location = new Point(270, 10),
            Size = new Size(120, 30)
        };

        exportTab.Controls.AddRange(new Control[] { 
            exportButton, exportProfileButton, shareOnlineButton 
        });

        _tabControl.TabPages.Add(exportTab);
    }

    private void CreateTemplateTab()
    {
        var templateTab = new TabPage("Templates");
        
        var libraryButton = new Button
        {
            Text = "Template Library",
            Location = new Point(10, 10),
            Size = new Size(120, 30)
        };
        libraryButton.Click += (s, e) => {
            var libraryForm = new MacroTemplateLibraryForm();
            libraryForm.ShowDialog();
        };

        var createTemplateButton = new Button
        {
            Text = "Create Template",
            Location = new Point(140, 10),
            Size = new Size(120, 30)
        };

        templateTab.Controls.AddRange(new Control[] { 
            libraryButton, createTemplateButton 
        });

        _tabControl.TabPages.Add(templateTab);
    }
}