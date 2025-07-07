using KeyLayer.Models;

namespace KeyLayer.Forms;

public partial class MacroExportForm : Form
{
    private readonly List<MacroAction> _actions;
    private ComboBox _formatComboBox;
    private TextBox _filePathTextBox;
    private Button _browseButton;
    private CheckBox _includeMetadataCheckBox;
    private CheckBox _compressCheckBox;
    private TextBox _descriptionTextBox;
    private TextBox _authorTextBox;
    private TextBox _versionTextBox;
    private Button _exportButton;
    private Button _cancelButton;

    public MacroExportForm(List<MacroAction> actions)
    {
        _actions = actions;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Export Macro";
        Size = new Size(500, 400);
        StartPosition = FormStartPosition.CenterParent;

        var formatLabel = new Label { Text = "Export Format:", Location = new Point(10, 15), Size = new Size(100, 23) };
        _formatComboBox = new ComboBox 
        { 
            Location = new Point(120, 12), 
            Size = new Size(150, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _formatComboBox.Items.AddRange(new[] { "JSON", "XML", "Binary", "Text Script", "AutoHotkey", "PowerShell" });
        _formatComboBox.SelectedIndex = 0;

        var pathLabel = new Label { Text = "File Path:", Location = new Point(10, 45), Size = new Size(100, 23) };
        _filePathTextBox = new TextBox { Location = new Point(120, 42), Size = new Size(250, 23) };
        _browseButton = new Button { Text = "Browse", Location = new Point(380, 42), Size = new Size(80, 23) };

        _includeMetadataCheckBox = new CheckBox 
        { 
            Text = "Include Metadata", 
            Location = new Point(10, 75), 
            Size = new Size(150, 23),
            Checked = true
        };

        _compressCheckBox = new CheckBox 
        { 
            Text = "Compress File", 
            Location = new Point(170, 75), 
            Size = new Size(120, 23)
        };

        var descLabel = new Label { Text = "Description:", Location = new Point(10, 105), Size = new Size(100, 23) };
        _descriptionTextBox = new TextBox 
        { 
            Location = new Point(120, 102), 
            Size = new Size(350, 60),
            Multiline = true
        };

        var authorLabel = new Label { Text = "Author:", Location = new Point(10, 175), Size = new Size(100, 23) };
        _authorTextBox = new TextBox { Location = new Point(120, 172), Size = new Size(200, 23) };

        var versionLabel = new Label { Text = "Version:", Location = new Point(10, 205), Size = new Size(100, 23) };
        _versionTextBox = new TextBox { Location = new Point(120, 202), Size = new Size(100, 23), Text = "1.0" };

        _exportButton = new Button 
        { 
            Text = "Export", 
            Location = new Point(300, 320), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };
        
        _cancelButton = new Button 
        { 
            Text = "Cancel", 
            Location = new Point(390, 320), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[] {
            formatLabel, _formatComboBox, pathLabel, _filePathTextBox, _browseButton,
            _includeMetadataCheckBox, _compressCheckBox,
            descLabel, _descriptionTextBox, authorLabel, _authorTextBox,
            versionLabel, _versionTextBox, _exportButton, _cancelButton
        });

        _browseButton.Click += OnBrowse;
        _exportButton.Click += OnExport;
        _formatComboBox.SelectedIndexChanged += OnFormatChanged;
    }

    private void OnFormatChanged(object? sender, EventArgs e)
    {
        var format = _formatComboBox.SelectedItem?.ToString();
        var extension = format switch
        {
            "JSON" => ".json",
            "XML" => ".xml",
            "Binary" => ".macro",
            "Text Script" => ".txt",
            "AutoHotkey" => ".ahk",
            "PowerShell" => ".ps1",
            _ => ".macro"
        };

        if (!string.IsNullOrEmpty(_filePathTextBox.Text))
        {
            var directory = Path.GetDirectoryName(_filePathTextBox.Text);
            var filename = Path.GetFileNameWithoutExtension(_filePathTextBox.Text);
            _filePathTextBox.Text = Path.Combine(directory ?? "", filename + extension);
        }
    }

    private void OnBrowse(object? sender, EventArgs e)
    {
        var format = _formatComboBox.SelectedItem?.ToString();
        var filter = format switch
        {
            "JSON" => "JSON Files (*.json)|*.json",
            "XML" => "XML Files (*.xml)|*.xml",
            "Binary" => "Macro Files (*.macro)|*.macro",
            "Text Script" => "Text Files (*.txt)|*.txt",
            "AutoHotkey" => "AutoHotkey Files (*.ahk)|*.ahk",
            "PowerShell" => "PowerShell Files (*.ps1)|*.ps1",
            _ => "All Files (*.*)|*.*"
        };

        var saveDialog = new SaveFileDialog { Filter = filter };
        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            _filePathTextBox.Text = saveDialog.FileName;
        }
    }

    private void OnExport(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_filePathTextBox.Text))
        {
            MessageBox.Show("Please specify a file path.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var format = _formatComboBox.SelectedItem?.ToString();
            var template = new MacroTemplate
            {
                Name = Path.GetFileNameWithoutExtension(_filePathTextBox.Text),
                Description = _descriptionTextBox.Text,
                Author = _authorTextBox.Text,
                Version = _versionTextBox.Text,
                Actions = _actions,
                Created = DateTime.Now
            };

            switch (format)
            {
                case "JSON":
                    ExportAsJson(template);
                    break;
                case "XML":
                    ExportAsXml(template);
                    break;
                case "Binary":
                    ExportAsBinary(template);
                    break;
                case "Text Script":
                    ExportAsTextScript(template);
                    break;
                case "AutoHotkey":
                    ExportAsAutoHotkey(template);
                    break;
                case "PowerShell":
                    ExportAsPowerShell(template);
                    break;
            }

            MessageBox.Show("Macro exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Export failed: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExportAsJson(MacroTemplate template)
    {
        var settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
        };

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(template, settings);
        File.WriteAllText(_filePathTextBox.Text, json);
    }

    private void ExportAsXml(MacroTemplate template)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MacroTemplate));
        using var writer = new StreamWriter(_filePathTextBox.Text);
        serializer.Serialize(writer, template);
    }

    private void ExportAsBinary(MacroTemplate template)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(template);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        
        if (_compressCheckBox.Checked)
        {
            using var output = new FileStream(_filePathTextBox.Text, FileMode.Create);
            using var gzip = new System.IO.Compression.GZipStream(output, System.IO.Compression.CompressionMode.Compress);
            gzip.Write(bytes, 0, bytes.Length);
        }
        else
        {
            File.WriteAllBytes(_filePathTextBox.Text, bytes);
        }
    }

    private void ExportAsTextScript(MacroTemplate template)
    {
        var script = new System.Text.StringBuilder();
        script.AppendLine($"# Macro: {template.Name}");
        script.AppendLine($"# Description: {template.Description}");
        script.AppendLine($"# Author: {template.Author}");
        script.AppendLine($"# Version: {template.Version}");
        script.AppendLine($"# Created: {template.Created}");
        script.AppendLine();

        foreach (var action in template.Actions)
        {
            script.AppendLine($"Action: {action.Type}");
            if (!string.IsNullOrEmpty(action.Value))
                script.AppendLine($"  Value: {action.Value}");
            if (action.Duration > 0)
                script.AppendLine($"  Duration: {action.Duration}ms");
            if (action.X != 0 || action.Y != 0)
                script.AppendLine($"  Position: ({action.X}, {action.Y})");
            script.AppendLine();
        }

        File.WriteAllText(_filePathTextBox.Text, script.ToString());
    }

    private void ExportAsAutoHotkey(MacroTemplate template)
    {
        var ahk = new System.Text.StringBuilder();
        ahk.AppendLine($"; Macro: {template.Name}");
        ahk.AppendLine($"; Description: {template.Description}");
        ahk.AppendLine($"; Author: {template.Author}");
        ahk.AppendLine();

        foreach (var action in template.Actions)
        {
            switch (action.Type)
            {
                case MacroActionType.KeyPress:
                    ahk.AppendLine($"Send, {{{action.Value}}}");
                    break;
                case MacroActionType.Text:
                    ahk.AppendLine($"SendRaw, {action.Value}");
                    break;
                case MacroActionType.MouseClick:
                    ahk.AppendLine($"Click, {action.Value}");
                    break;
                case MacroActionType.MouseMove:
                    ahk.AppendLine($"MouseMove, {action.X}, {action.Y}");
                    break;
                case MacroActionType.Delay:
                    ahk.AppendLine($"Sleep, {action.Duration}");
                    break;
                case MacroActionType.Application:
                    ahk.AppendLine($"Run, {action.Value}");
                    break;
            }
        }

        File.WriteAllText(_filePathTextBox.Text, ahk.ToString());
    }

    private void ExportAsPowerShell(MacroTemplate template)
    {
        var ps = new System.Text.StringBuilder();
        ps.AppendLine($"# Macro: {template.Name}");
        ps.AppendLine($"# Description: {template.Description}");
        ps.AppendLine($"# Author: {template.Author}");
        ps.AppendLine();
        ps.AppendLine("Add-Type -AssemblyName System.Windows.Forms");
        ps.AppendLine();

        foreach (var action in template.Actions)
        {
            switch (action.Type)
            {
                case MacroActionType.KeyPress:
                    ps.AppendLine($"[System.Windows.Forms.SendKeys]::SendWait('{action.Value}')");
                    break;
                case MacroActionType.Text:
                    ps.AppendLine($"[System.Windows.Forms.SendKeys]::SendWait('{action.Value}')");
                    break;
                case MacroActionType.Delay:
                    ps.AppendLine($"Start-Sleep -Milliseconds {action.Duration}");
                    break;
                case MacroActionType.Application:
                    ps.AppendLine($"Start-Process '{action.Value}'");
                    break;
            }
        }

        File.WriteAllText(_filePathTextBox.Text, ps.ToString());
    }
}