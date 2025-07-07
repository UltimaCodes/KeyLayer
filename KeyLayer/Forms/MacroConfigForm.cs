using KeyLayer.Models;
using KeyLayer.Services;

namespace KeyLayer.Forms;

public partial class MacroConfigForm : Form
{
    private readonly string _devicePath;
    private readonly MacroService _macroService;
    private DeviceProfile? _profile;
    
    private TextBox _keyNameTextBox;
    private ComboBox _keyCodeComboBox;
    private CheckBox _isLayerKeyCheckBox;
    private ComboBox _layerComboBox;
    private ListView _actionsListView;
    private Button _addActionButton;
    private Button _removeActionButton;
    private Button _testMacroButton;
    private Button _saveButton;
    private Button _cancelButton;

    public MacroConfigForm(string devicePath, MacroService macroService)
    {
        _devicePath = devicePath;
        _macroService = macroService;
        _profile = _macroService.GetDeviceProfile(devicePath);
        
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        Text = "Configure Macro";
        Size = new Size(600, 500);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        // Key Configuration
        var keyLabel = new Label { Text = "Key Name:", Location = new Point(10, 15), Size = new Size(80, 23) };
        _keyNameTextBox = new TextBox { Location = new Point(100, 12), Size = new Size(150, 23) };

        var keyCodeLabel = new Label { Text = "Key Code:", Location = new Point(260, 15), Size = new Size(70, 23) };
        _keyCodeComboBox = new ComboBox 
        { 
            Location = new Point(340, 12), 
            Size = new Size(100, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        // Populate key codes (simplified - you'd want a more complete list)
        for (int i = 1; i <= 255; i++)
        {
            _keyCodeComboBox.Items.Add(i.ToString());
        }

        _isLayerKeyCheckBox = new CheckBox 
        { 
            Text = "Layer Key", 
            Location = new Point(450, 12), 
            Size = new Size(100, 23) 
        };

        var layerLabel = new Label { Text = "Layer:", Location = new Point(10, 45), Size = new Size(50, 23) };
        _layerComboBox = new ComboBox 
        { 
            Location = new Point(70, 42), 
            Size = new Size(100, 23),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Enabled = false
        };
        
        _layerComboBox.Items.AddRange(new[] { "Default", "Layer 1", "Layer 2", "Layer 3" });
        _layerComboBox.SelectedIndex = 0;

        // Actions List
        var actionsLabel = new Label { Text = "Actions:", Location = new Point(10, 75), Size = new Size(60, 23) };
        _actionsListView = new ListView
        {
            Location = new Point(10, 100),
            Size = new Size(560, 250),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true
        };
        
        _actionsListView.Columns.Add("Type", 100);
        _actionsListView.Columns.Add("Value", 200);
        _actionsListView.Columns.Add("Duration", 80);
        _actionsListView.Columns.Add("Details", 180);

        _addActionButton = new Button 
        { 
            Text = "Add Action", 
            Location = new Point(10, 360), 
            Size = new Size(100, 30) 
        };
        
        _removeActionButton = new Button 
        { 
            Text = "Remove Action", 
            Location = new Point(120, 360), 
            Size = new Size(100, 30),
            Enabled = false
        };

        _testMacroButton = new Button 
        { 
            Text = "Test Macro", 
            Location = new Point(230, 360), 
            Size = new Size(100, 30) 
        };

        // Dialog Buttons
        _saveButton = new Button 
        { 
            Text = "Save", 
            Location = new Point(400, 420), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };
        
        _cancelButton = new Button 
        { 
            Text = "Cancel", 
            Location = new Point(490, 420), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[] {
            keyLabel, _keyNameTextBox, keyCodeLabel, _keyCodeComboBox, _isLayerKeyCheckBox,
            layerLabel, _layerComboBox, actionsLabel, _actionsListView,
            _addActionButton, _removeActionButton, _testMacroButton,
            _saveButton, _cancelButton
        });

        // Event Handlers
        _isLayerKeyCheckBox.CheckedChanged += OnLayerKeyChanged;
        _addActionButton.Click += OnAddAction;
        _removeActionButton.Click += OnRemoveAction;
        _testMacroButton.Click += OnTestMacro;
        _saveButton.Click += OnSave;
        _actionsListView.SelectedIndexChanged += OnActionSelectionChanged;
    }

    private void LoadData()
    {
        // Load existing key mappings if editing
    }

    private void OnLayerKeyChanged(object? sender, EventArgs e)
    {
        _layerComboBox.Enabled = !_isLayerKeyCheckBox.Checked;
        _actionsListView.Enabled = !_isLayerKeyCheckBox.Checked;
        _addActionButton.Enabled = !_isLayerKeyCheckBox.Checked;
    }

    private void OnAddAction(object? sender, EventArgs e)
    {
        var actionForm = new ActionConfigForm();
        if (actionForm.ShowDialog() == DialogResult.OK)
        {
            var action = actionForm.GetAction();
            AddActionToList(action);
        }
    }

    private void AddActionToList(MacroAction action)
    {
        var item = new ListViewItem(action.Type.ToString());
        item.SubItems.Add(action.Value);
        item.SubItems.Add(action.Duration.ToString());
        item.SubItems.Add(GetActionDetails(action));
        item.Tag = action;
        _actionsListView.Items.Add(item);
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
            _ => ""
        };
    }

    private void OnRemoveAction(object? sender, EventArgs e)
    {
        if (_actionsListView.SelectedItems.Count > 0)
        {
            _actionsListView.Items.Remove(_actionsListView.SelectedItems[0]);
        }
    }

    private async void OnTestMacro(object? sender, EventArgs e)
    {
        var actions = _actionsListView.Items.Cast<ListViewItem>()
            .Select(item => (MacroAction)item.Tag)
            .ToList();
        
        if (actions.Count > 0)
        {
            await _macroService.ExecuteMacroAsync(actions);
        }
    }

    private void OnSave(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_keyNameTextBox.Text) || _keyCodeComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("Please enter a key name and select a key code.", "Validation Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var keyCode = int.Parse(_keyCodeComboBox.SelectedItem.ToString()!);
        var keyMapping = new KeyMapping
        {
            KeyCode = keyCode,
            KeyName = _keyNameTextBox.Text,
            IsLayerKey = _isLayerKeyCheckBox.Checked
        };

        if (!_isLayerKeyCheckBox.Checked)
        {
            keyMapping.Actions = _actionsListView.Items.Cast<ListViewItem>()
                .Select(item => (MacroAction)item.Tag)
                .ToList();
        }

        // Save to profile
        if (_profile == null)
        {
            _profile = new DeviceProfile
            {
                DevicePath = _devicePath,
                ProfileName = "Default Profile",
                Created = DateTime.Now,
                IsActive = true
            };
        }

        _profile.KeyMappings[keyCode] = keyMapping;
        
        if (_isLayerKeyCheckBox.Checked)
        {
            _profile.LayerKeys.Add(keyCode);
        }

        _profile.Modified = DateTime.Now;
        _macroService.SaveDeviceProfile(_profile);
    }

    private void OnActionSelectionChanged(object? sender, EventArgs e)
    {
        _removeActionButton.Enabled = _actionsListView.SelectedItems.Count > 0;
    }
}