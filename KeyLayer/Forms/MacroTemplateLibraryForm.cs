using KeyLayer.Models;

namespace KeyLayer.Forms;

public partial class MacroTemplateLibraryForm : Form
{
    public MacroTemplate? SelectedTemplate { get; private set; }
    
    private TreeView _categoryTreeView;
    private ListView _templateListView;
    private TextBox _searchTextBox;
    private RichTextBox _descriptionTextBox;
    private Button _selectButton;
    private Button _cancelButton;
    private Button _downloadButton;
    private MacroLibrary _library;

    public MacroTemplateLibraryForm()
    {
        _library = LoadLibrary();
        InitializeComponent();
        LoadCategories();
    }

    private void InitializeComponent()
    {
        Text = "Macro Template Library";
        Size = new Size(800, 600);
        StartPosition = FormStartPosition.CenterParent;

        var searchLabel = new Label { Text = "Search:", Location = new Point(10, 15), Size = new Size(50, 23) };
        _searchTextBox = new TextBox { Location = new Point(70, 12), Size = new Size(200, 23) };
        _searchTextBox.TextChanged += OnSearchTextChanged;

        _downloadButton = new Button { Text = "Download More", Location = new Point(280, 12), Size = new Size(100, 23) };
        _downloadButton.Click += OnDownloadMore;

        _categoryTreeView = new TreeView
        {
            Location = new Point(10, 45),
            Size = new Size(200, 450),
            HideSelection = false
        };
        _categoryTreeView.AfterSelect += OnCategorySelected;

        _templateListView = new ListView
        {
            Location = new Point(220, 45),
            Size = new Size(350, 300),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true
        };
        _templateListView.Columns.Add("Name", 150);
        _templateListView.Columns.Add("Author", 100);
        _templateListView.Columns.Add("Version", 60);
        _templateListView.Columns.Add("Created", 100);
        _templateListView.SelectedIndexChanged += OnTemplateSelected;
        _templateListView.DoubleClick += OnTemplateDoubleClick;

        var descLabel = new Label { Text = "Description:", Location = new Point(220, 355), Size = new Size(80, 23) };
        _descriptionTextBox = new RichTextBox
        {
            Location = new Point(220, 380),
            Size = new Size(350, 115),
            ReadOnly = true
        };

        _selectButton = new Button 
        { 
            Text = "Select", 
            Location = new Point(580, 450), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK,
            Enabled = false
        };
        
        _cancelButton = new Button 
        { 
            Text = "Cancel", 
            Location = new Point(670, 450), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[] {
            searchLabel, _searchTextBox, _downloadButton,
            _categoryTreeView, _templateListView, descLabel, _descriptionTextBox,
            _selectButton, _cancelButton
        });

        _selectButton.Click += OnSelect;
    }

    private MacroLibrary LoadLibrary()
    {
        try
        {
            if (File.Exists("template_library.json"))
            {
                var json = File.ReadAllText("template_library.json");
                return Newtonsoft.Json.JsonConvert.DeserializeObject<MacroLibrary>(json) ?? CreateDefaultLibrary();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading template library: {ex.Message}", "Library Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        return CreateDefaultLibrary();
    }

    private MacroLibrary CreateDefaultLibrary()
    {
        var library = new MacroLibrary();
        
        // Add some default templates
        var textTemplates = new List<MacroTemplate>
        {
            new MacroTemplate
            {
                Name = "Email Signature",
                Description = "Types a professional email signature",
                Category = "Text",
                Actions = new List<MacroAction>
                {
                    new MacroAction { Type = MacroActionType.Text, Value = "Best regards,\n" },
                    new MacroAction { Type = MacroActionType.Text, Value = "Your Name\n" },
                    new MacroAction { Type = MacroActionType.Text, Value = "Your Title\n" },
                    new MacroAction { Type = MacroActionType.Text, Value = "Company Name" }
                },
                Author = "System",
                Created = DateTime.Now
            },
            new MacroTemplate
            {
                Name = "Current Date",
                Description = "Types the current date in MM/DD/YYYY format",
                Category = "Text",
                Actions = new List<MacroAction>
                {
                    new MacroAction { Type = MacroActionType.Text, Value = DateTime.Now.ToString("MM/dd/yyyy") }
                },
                Author = "System",
                Created = DateTime.Now
            }
        };

        var systemTemplates = new List<MacroTemplate>
        {
            new MacroTemplate
            {
                Name = "Screenshot",
                Description = "Takes a screenshot and saves it",
                Category = "System",
                Actions = new List<MacroAction>
                {
                    new MacroAction { Type = MacroActionType.Screenshot, Value = "screenshot.png" }
                },
                Author = "System",
                Created = DateTime.Now
            },
            new MacroTemplate
            {
                Name = "Lock Computer",
                Description = "Locks the computer screen",
                Category = "System",
                Actions = new List<MacroAction>
                {
                    new MacroAction { Type = MacroActionType.SystemCommand, SystemCommand = SystemCommandType.Lock }
                },
                Author = "System",
                Created = DateTime.Now
            }
        };

        library.Categories["Text"] = textTemplates;
        library.Categories["System"] = systemTemplates;
        library.Templates.AddRange(textTemplates);
        library.Templates.AddRange(systemTemplates);

        return library;
    }

    private void LoadCategories()
    {
        _categoryTreeView.Nodes.Clear();
        
        var allNode = new TreeNode("All Templates") { Tag = "All" };
        _categoryTreeView.Nodes.Add(allNode);

        foreach (var category in _library.Categories.Keys)
        {
            var categoryNode = new TreeNode(category) { Tag = category };
            _categoryTreeView.Nodes.Add(categoryNode);
        }

        _categoryTreeView.SelectedNode = allNode;
        LoadTemplates("All");
    }

    private void OnCategorySelected(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is string category)
        {
            LoadTemplates(category);
        }
    }

    private void LoadTemplates(string category)
    {
        _templateListView.Items.Clear();
        
        var templates = category == "All" 
            ? _library.Templates 
            : _library.Categories.GetValueOrDefault(category, new List<MacroTemplate>());

        var searchText = _searchTextBox.Text.ToLower();
        if (!string.IsNullOrEmpty(searchText))
        {
            templates = templates.Where(t => 
                t.Name.ToLower().Contains(searchText) || 
                t.Description.ToLower().Contains(searchText) ||
                t.Tags.Any(tag => tag.ToLower().Contains(searchText))
            ).ToList();
        }

        foreach (var template in templates)
        {
            var item = new ListViewItem(template.Name);
            item.SubItems.Add(template.Author);
            item.SubItems.Add(template.Version);
            item.SubItems.Add(template.Created.ToShortDateString());
            item.Tag = template;
            _templateListView.Items.Add(item);
        }
    }

    private void OnSearchTextChanged(object? sender, EventArgs e)
    {
        var selectedCategory = _categoryTreeView.SelectedNode?.Tag as string ?? "All";
        LoadTemplates(selectedCategory);
    }

    private void OnTemplateSelected(object? sender, EventArgs e)
    {
        if (_templateListView.SelectedItems.Count > 0)
        {
            var template = (MacroTemplate)_templateListView.SelectedItems[0].Tag;
            _descriptionTextBox.Text = $"{template.Description}\n\n" +
                                     $"Actions: {template.Actions.Count}\n" +
                                     $"Author: {template.Author}\n" +
                                     $"Version: {template.Version}\n" +
                                     $"Created: {template.Created}\n";
            
            if (template.Tags.Count > 0)
            {
                _descriptionTextBox.Text += $"Tags: {string.Join(", ", template.Tags)}";
            }

            _selectButton.Enabled = true;
            SelectedTemplate = template;
        }
        else
        {
            _descriptionTextBox.Clear();
            _selectButton.Enabled = false;
            SelectedTemplate = null;
        }
    }

    private void OnTemplateDoubleClick(object? sender, EventArgs e)
    {
        if (_templateListView.SelectedItems.Count > 0)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void OnDownloadMore(object? sender, EventArgs e)
    {
        MessageBox.Show("Online template download feature would be implemented here.", 
            "Download Templates", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void OnSelect(object? sender, EventArgs e)
    {
        // SelectedTemplate is already set in OnTemplateSelected
    }
}