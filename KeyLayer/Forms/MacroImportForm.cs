using KeyLayer.Models;

namespace KeyLayer.Forms;

public partial class MacroImportForm : Form
{
    public List<MacroAction> ImportedActions { get; private set; } = new();
    
    private ComboBox _sourceComboBox;
    private TextBox _filePathTextBox;
    private Button _browseButton;
    private TextBox _urlTextBox;
    private ListView _previewListView;
    private Button _importButton;
    private Button _cancelButton;
    private CheckBox _mergeCheckBox;

    public MacroImportForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Import Macro";
        Size = new Size(600, 500);
        StartPosition = FormStartPosition.CenterParent;

        var sourceLabel = new Label { Text = "Import Source:", Location = new Point(10, 15), Size = new Size(100, 23) };
        _sourceComboBox = new ComboBox 
        { 
            Location = new Point(120, 12), 
            Size = new Size(150, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _sourceComboBox.Items.AddRange(new[] { "File", "URL", "Clipboard", "Template Library" });
        _sourceComboBox.SelectedIndex = 0;
        _sourceComboBox.SelectedIndexChanged += OnSourceChanged;

        var pathLabel = new Label { Text = "File Path:", Location = new Point(10, 45), Size = new Size(100, 23) };
        _filePathTextBox = new TextBox { Location = new Point(120, 42), Size = new Size(300, 23) };
        _browseButton = new Button { Text = "Browse", Location = new Point(430, 42), Size = new Size(80, 23) };

        var urlLabel = new Label { Text = "URL:", Location = new Point(10, 75), Size = new Size(100, 23), Visible = false };
        _urlTextBox = new TextBox { Location = new Point(120, 72), Size = new Size(380, 23), Visible = false };

        _mergeCheckBox = new CheckBox 
        { 
            Text = "Merge with existing actions", 
            Location = new Point(10, 105), 
            Size = new Size(200, 23)
        };

        var previewLabel = new Label { Text = "Preview:", Location = new Point(10, 135), Size = new Size(100, 23) };
        _previewListView = new ListView
        {
            Location = new Point(10, 160),
            Size = new Size(570, 250),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true
        };
        _previewListView.Columns.Add("Type", 100);
        _previewListView.Columns.Add("Value", 200);
        _previewListView.Columns.Add("Duration", 80);
        _previewListView.Columns.Add("Details", 190);

        _importButton = new Button 
        { 
            Text = "Import", 
            Location = new Point(420, 430), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK,
            Enabled = false
        };
        
        _cancelButton = new Button 
        { 
            Text = "Cancel", 
            Location = new Point(510, 430), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[] {
            sourceLabel, _sourceComboBox, pathLabel, _filePathTextBox, _browseButton,
            urlLabel, _urlTextBox, _mergeCheckBox, previewLabel, _previewListView,
            _importButton, _cancelButton
        });

        _browseButton.Click += OnBrowse;
        _importButton.Click += OnImport;
        _filePathTextBox.TextChanged += OnPathChanged;
        _urlTextBox.TextChanged += OnPathChanged;
    }

    private void OnSourceChanged(object? sender, EventArgs e)
    {
        var source = _sourceComboBox.SelectedItem?.ToString();
        
        _filePathTextBox.Visible = source == "File";
        _browseButton.Visible = source == "File";
        _urlTextBox.Visible = source == "URL";
        
        var pathLabel = Controls.OfType<Label>().First(l => l.Text == "File Path:");
        var urlLabel = Controls.OfType<Label>().First(l => l.Text == "URL:");
        
        pathLabel.Visible = source == "File";
        urlLabel.Visible = source == "URL";

        if (source == "Clipboard")
        {
            LoadFromClipboard();
        }
        else if (source == "Template Library")
        {
            ShowTemplateLibrary();
        }
    }

    private void OnBrowse(object? sender, EventArgs e)
    {
        var openDialog = new OpenFileDialog
        {
            Filter = "All Supported|*.json;*.xml;*.macro;*.ahk;*.ps1;*.txt|" +
                    "JSON Files|*.json|XML Files|*.xml|Macro Files|*.macro|" +
                    "AutoHotkey Files|*.ahk|PowerShell Files|*.ps1|Text Files|*.txt|" +
                    "All Files|*.*"
        };

        if (openDialog.ShowDialog() == DialogResult.OK)
        {
            _filePathTextBox.Text = openDialog.FileName;
            LoadPreview();
        }
    }

    private void OnPathChanged(object? sender, EventArgs e)
    {
        LoadPreview();
    }

    private void LoadPreview()
    {
        try
        {
            var source = _sourceComboBox.SelectedItem?.ToString();
            List<MacroAction> actions = new();

            switch (source)
            {
                case "File":
                    if (File.Exists(_filePathTextBox.Text))
                    {
                        actions = LoadFromFile(_filePathTextBox.Text);
                    }
                    break;
                case "URL":
                    if (!string.IsNullOrWhiteSpace(_urlTextBox.Text))
                    {
                        actions = LoadFromUrl(_urlTextBox.Text);
                    }
                    break;
            }

            UpdatePreview(actions);
            _importButton.Enabled = actions.Count > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading preview: {ex.Message}", "Preview Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _importButton.Enabled = false;
        }
    }

    private List<MacroAction> LoadFromFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        
        return extension switch
        {
            ".json" => LoadFromJson(File.ReadAllText(filePath)),
            ".xml" => LoadFromXml(filePath),
            ".macro" => LoadFromBinary(filePath),
            ".ahk" => LoadFromAutoHotkey(File.ReadAllText(filePath)),
            ".ps1" => LoadFromPowerShell(File.ReadAllText(filePath)),
            ".txt" => LoadFromTextScript(File.ReadAllText(filePath)),
            _ => throw new NotSupportedException($"File format {extension} is not supported")
        };
    }

    private List<MacroAction> LoadFromJson(string json)
    {
        try
        {
            // Try to load as MacroTemplate first
            var template = Newtonsoft.Json.JsonConvert.DeserializeObject<MacroTemplate>(json);
            if (template?.Actions != null)
            {
                return template.Actions;
            }
        }
        catch
        {
            // If that fails, try to load as List<MacroAction>
            try
            {
                var actions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MacroAction>>(json);
                return actions ?? new List<MacroAction>();
            }
            catch
            {
                throw new InvalidDataException("Invalid JSON format");
            }
        }
        
        return new List<MacroAction>();
    }

    private List<MacroAction> LoadFromXml(string filePath)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MacroTemplate));
        using var reader = new StreamReader(filePath);
        var template = (MacroTemplate?)serializer.Deserialize(reader);
        return template?.Actions ?? new List<MacroAction>();
    }

    private List<MacroAction> LoadFromBinary(string filePath)
    {
        var bytes = File.ReadAllBytes(filePath);
        
        // Try to decompress if it's compressed
        try
        {
            using var input = new MemoryStream(bytes);
            using var gzip = new System.IO.Compression.GZipStream(input, System.IO.Compression.CompressionMode.Decompress);
            using var output = new MemoryStream();
            gzip.CopyTo(output);
            bytes = output.ToArray();
        }
        catch
        {
            // Not compressed, use original bytes
        }

        var json = System.Text.Encoding.UTF8.GetString(bytes);
        return LoadFromJson(json);
    }

    private List<MacroAction> LoadFromAutoHotkey(string content)
    {
        var actions = new List<MacroAction>();
        var lines = content.Split('\n');

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith(";") || string.IsNullOrEmpty(trimmed))
                continue;

            if (trimmed.StartsWith("Send,"))
            {
                var value = trimmed.Substring(5).Trim();
                actions.Add(new MacroAction { Type = MacroActionType.KeyPress, Value = value });
            }
            else if (trimmed.StartsWith("SendRaw,"))
            {
                var value = trimmed.Substring(8).Trim();
                actions.Add(new MacroAction { Type = MacroActionType.Text, Value = value });
            }
            else if (trimmed.StartsWith("Click,"))
            {
                var value = trimmed.Substring(6).Trim();
                actions.Add(new MacroAction { Type = MacroActionType.MouseClick, Value = value });
            }
            else if (trimmed.StartsWith("Sleep,"))
            {
                if (int.TryParse(trimmed.Substring(6).Trim(), out int duration))
                {
                    actions.Add(new MacroAction { Type = MacroActionType.Delay, Duration = duration });
                }
            }
            else if (trimmed.StartsWith("Run,"))
            {
                var value = trimmed.Substring(4).Trim();
                actions.Add(new MacroAction { Type = MacroActionType.Application, Value = value });
            }
        }

        return actions;
    }

    private List<MacroAction> LoadFromPowerShell(string content)
    {
        var actions = new List<MacroAction>();
        var lines = content.Split('\n');

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("#") || string.IsNullOrEmpty(trimmed))
                continue;

            if (trimmed.Contains("SendKeys]::SendWait"))
            {
                var start = trimmed.IndexOf("('") + 2;
                var end = trimmed.LastIndexOf("')");
                if (start > 1 && end > start)
                {
                    var value = trimmed.Substring(start, end - start);
                    actions.Add(new MacroAction { Type = MacroActionType.Text, Value = value });
                }
            }
            else if (trimmed.StartsWith("Start-Sleep -Milliseconds"))
            {
                var durationStr = trimmed.Substring(25).Trim();
                if (int.TryParse(durationStr, out int duration))
                {
                    actions.Add(new MacroAction { Type = MacroActionType.Delay, Duration = duration });
                }
            }
            else if (trimmed.StartsWith("Start-Process"))
            {
                var start = trimmed.IndexOf("'") + 1;
                var end = trimmed.LastIndexOf("'");
                if (start > 0 && end > start)
                {
                    var value = trimmed.Substring(start, end - start);
                    actions.Add(new MacroAction { Type = MacroActionType.Application, Value = value });
                }
            }
        }

        return actions;
    }

    private List<MacroAction> LoadFromTextScript(string content)
    {
        var actions = new List<MacroAction>();
        var lines = content.Split('\n');
        MacroAction? currentAction = null;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("#") || string.IsNullOrEmpty(trimmed))
                continue;

            if (trimmed.StartsWith("Action:"))
            {
                if (currentAction != null)
                {
                    actions.Add(currentAction);
                }
                
                var actionType = trimmed.Substring(7).Trim();
                if (Enum.TryParse<MacroActionType>(actionType, out var type))
                {
                    currentAction = new MacroAction { Type = type };
                }
            }
            else if (currentAction != null)
            {
                if (trimmed.StartsWith("Value:"))
                {
                    currentAction.Value = trimmed.Substring(6).Trim();
                }
                else if (trimmed.StartsWith("Duration:"))
                {
                    var durationStr = trimmed.Substring(9).Replace("ms", "").Trim();
                    if (int.TryParse(durationStr, out int duration))
                    {
                        currentAction.Duration = duration;
                    }
                }
                else if (trimmed.StartsWith("Position:"))
                {
                    var posStr = trimmed.Substring(9).Trim().Trim('(', ')');
                    var coords = posStr.Split(',');
                    if (coords.Length == 2 && 
                        int.TryParse(coords[0].Trim(), out int x) && 
                        int.TryParse(coords[1].Trim(), out int y))
                    {
                        currentAction.X = x;
                        currentAction.Y = y;
                    }
                }
            }
        }

        if (currentAction != null)
        {
            actions.Add(currentAction);
        }

        return actions;
    }

    private List<MacroAction> LoadFromUrl(string url)
    {
        using var client = new System.Net.Http.HttpClient();
        var content = client.GetStringAsync(url).Result;
        
        // Determine format based on URL or content
        if (url.EndsWith(".json") || content.TrimStart().StartsWith("{"))
        {
            return LoadFromJson(content);
        }
        else if (url.EndsWith(".ahk"))
        {
            return LoadFromAutoHotkey(content);
        }
        else if (url.EndsWith(".ps1"))
        {
            return LoadFromPowerShell(content);
        }
        else
        {
            return LoadFromTextScript(content);
        }
    }

    private void LoadFromClipboard()
    {
        try
        {
            if (Clipboard.ContainsText())
            {
                var content = Clipboard.GetText();
                List<MacroAction> actions;

                // Try different formats
                if (content.TrimStart().StartsWith("{") || content.TrimStart().StartsWith("["))
                {
                    actions = LoadFromJson(content);
                }
                else if (content.Contains("Send,") || content.Contains("Click,"))
                {
                    actions = LoadFromAutoHotkey(content);
                }
                else if (content.Contains("SendKeys") || content.Contains("Start-Process"))
                {
                    actions = LoadFromPowerShell(content);
                }
                else
                {
                    actions = LoadFromTextScript(content);
                }

                UpdatePreview(actions);
                _importButton.Enabled = actions.Count > 0;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading from clipboard: {ex.Message}", "Clipboard Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ShowTemplateLibrary()
    {
        var libraryForm = new MacroTemplateLibraryForm();
        if (libraryForm.ShowDialog() == DialogResult.OK && libraryForm.SelectedTemplate != null)
        {
            UpdatePreview(libraryForm.SelectedTemplate.Actions);
            _importButton.Enabled = true;
        }
    }

    private void UpdatePreview(List<MacroAction> actions)
    {
        _previewListView.Items.Clear();
        
        foreach (var action in actions)
        {
            var item = new ListViewItem(action.Type.ToString());
            item.SubItems.Add(action.Value);
            item.SubItems.Add(action.Duration.ToString());
            item.SubItems.Add(GetActionDetails(action));
            item.Tag = action;
            _previewListView.Items.Add(item);
        }

        ImportedActions = actions;
    }

    private string GetActionDetails(MacroAction action)
    {
        return action.Type switch
        {
            MacroActionType.KeyPress => $"Key: {action.Value}",
            MacroActionType.MouseClick => $"Button: {action.Value}",
            MacroActionType.MouseMove => $"Position: ({action.X}, {action.Y})",
            MacroActionType.Text => $"Text: {action.Value}",
            MacroActionType.Application => $"App: {action.Value}",
            MacroActionType.Delay => $"Wait: {action.Duration}ms",
            _ => action.Description
        };
    }

    private void OnImport(object? sender, EventArgs e)
    {
        if (!_mergeCheckBox.Checked)
        {
            // Clear existing actions if not merging
            ImportedActions = _previewListView.Items.Cast<ListViewItem>()
                .Select(item => (MacroAction)item.Tag)
                .ToList();
        }
    }
}