using KeyLayer.Models;

namespace KeyLayer.Forms;

public partial class AdvancedActionConfigForm : Form
{
    private readonly MacroAction _action;
    private TabControl _tabControl;
    private ComboBox _actionTypeComboBox;
    private TextBox _descriptionTextBox;
    private CheckBox _enabledCheckBox;

    // Basic tab controls
    private TextBox _valueTextBox;
    private NumericUpDown _durationNumeric;
    private NumericUpDown _xNumeric;
    private NumericUpDown _yNumeric;

    // Advanced tab controls
    private ComboBox _windowActionComboBox;
    private ComboBox _systemCommandComboBox;
    private ComboBox _fileOperationComboBox;
    private TextBox _value2TextBox;

    // Conditional tab controls
    private ComboBox _conditionalTypeComboBox;
    private TextBox _conditionalValueTextBox;
    private ListView _conditionalActionsListView;
    private ListView _elseActionsListView;

    // Loop tab controls
    private NumericUpDown _loopCountNumeric;
    private CheckBox _infiniteLoopCheckBox;

    // Variable tab controls
    private TextBox _variableNameTextBox;
    private TextBox _variableValueTextBox;

    private Button _okButton;
    private Button _cancelButton;
    private Button _testButton;

    public AdvancedActionConfigForm(MacroAction action)
    {
        _action = action;
        InitializeComponent();
        LoadActionData();
    }

    private void InitializeComponent()
    {
        Text = "Advanced Action Configuration";
        Size = new Size(600, 500);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        // Header controls
        var typeLabel = new Label { Text = "Action Type:", Location = new Point(10, 15), Size = new Size(80, 23) };
        _actionTypeComboBox = new ComboBox 
        { 
            Location = new Point(100, 12), 
            Size = new Size(150, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _actionTypeComboBox.Items.AddRange(Enum.GetNames(typeof(MacroActionType)));
        _actionTypeComboBox.SelectedIndexChanged += OnActionTypeChanged;

        var descLabel = new Label { Text = "Description:", Location = new Point(260, 15), Size = new Size(70, 23) };
        _descriptionTextBox = new TextBox { Location = new Point(340, 12), Size = new Size(200, 23) };

        _enabledCheckBox = new CheckBox 
        { 
            Text = "Enabled", 
            Location = new Point(10, 45), 
            Size = new Size(80, 23),
            Checked = true
        };

        // Tab Control
        _tabControl = new TabControl
        {
            Location = new Point(10, 75),
            Size = new Size(570, 350)
        };

        CreateBasicTab();
        CreateAdvancedTab();
        CreateConditionalTab();
        CreateLoopTab();
        CreateVariableTab();

        // Buttons
        _testButton = new Button 
        { 
            Text = "Test", 
            Location = new Point(350, 440), 
            Size = new Size(80, 30) 
        };
        
        _okButton = new Button 
        { 
            Text = "OK", 
            Location = new Point(440, 440), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };
        
        _cancelButton = new Button 
        { 
            Text = "Cancel", 
            Location = new Point(530, 440), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[] {
            typeLabel, _actionTypeComboBox, descLabel, _descriptionTextBox, _enabledCheckBox,
            _tabControl, _testButton, _okButton, _cancelButton
        });

        _testButton.Click += OnTest;
        _okButton.Click += OnOK;
    }

    private void CreateBasicTab()
    {
        var basicTab = new TabPage("Basic");
        
        var valueLabel = new Label { Text = "Value:", Location = new Point(10, 15), Size = new Size(80, 23) };
        _valueTextBox = new TextBox { Location = new Point(100, 12), Size = new Size(250, 23) };

        var value2Label = new Label { Text = "Value 2:", Location = new Point(10, 45), Size = new Size(80, 23) };
        _value2TextBox = new TextBox { Location = new Point(100, 42), Size = new Size(250, 23) };

        var durationLabel = new Label { Text = "Duration (ms):", Location = new Point(10, 75), Size = new Size(80, 23) };
        _durationNumeric = new NumericUpDown 
        { 
            Location = new Point(100, 72), 
            Size = new Size(100, 23),
            Maximum = 60000,
            Value = 100
        };

        var positionLabel = new Label { Text = "Position:", Location = new Point(10, 105), Size = new Size(80, 23) };
        _xNumeric = new NumericUpDown 
        { 
            Location = new Point(100, 102), 
            Size = new Size(70, 23),
            Maximum = 9999
        };
        
        var xLabel = new Label { Text = "X:", Location = new Point(85, 105), Size = new Size(15, 23) };
        
        _yNumeric = new NumericUpDown 
        { 
            Location = new Point(200, 102), 
            Size = new Size(70, 23),
            Maximum = 9999
        };
        
        var yLabel = new Label { Text = "Y:", Location = new Point(180, 105), Size = new Size(15, 23) };

        basicTab.Controls.AddRange(new Control[] {
            valueLabel, _valueTextBox, value2Label, _value2TextBox,
            durationLabel, _durationNumeric, positionLabel, 
            xLabel, _xNumeric, yLabel, _yNumeric
        });

        _tabControl.TabPages.Add(basicTab);
    }

    private void CreateAdvancedTab()
    {
        var advancedTab = new TabPage("Advanced");

        var windowLabel = new Label { Text = "Window Action:", Location = new Point(10, 15), Size = new Size(100, 23) };
        _windowActionComboBox = new ComboBox 
        { 
            Location = new Point(120, 12), 
            Size = new Size(150, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _windowActionComboBox.Items.AddRange(Enum.GetNames(typeof(WindowAction)));

        var systemLabel = new Label { Text = "System Command:", Location = new Point(10, 45), Size = new Size(100, 23) };
        _systemCommandComboBox = new ComboBox 
        { 
            Location = new Point(120, 42), 
            Size = new Size(150, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _systemCommandComboBox.Items.AddRange(Enum.GetNames(typeof(SystemCommandType)));

        var fileLabel = new Label { Text = "File Operation:", Location = new Point(10, 75), Size = new Size(100, 23) };
        _fileOperationComboBox = new ComboBox 
        { 
            Location = new Point(120, 72), 
            Size = new Size(150, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _fileOperationComboBox.Items.AddRange(Enum.GetNames(typeof(FileOperationType)));

        advancedTab.Controls.AddRange(new Control[] {
            windowLabel, _windowActionComboBox,
            systemLabel, _systemCommandComboBox,
            fileLabel, _fileOperationComboBox
        });

        _tabControl.TabPages.Add(advancedTab);
    }

    private void CreateConditionalTab()
    {
        var conditionalTab = new TabPage("Conditional");

        var typeLabel = new Label { Text = "Condition Type:", Location = new Point(10, 15), Size = new Size(100, 23) };
        _conditionalTypeComboBox = new ComboBox 
        { 
            Location = new Point(120, 12), 
            Size = new Size(150, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _conditionalTypeComboBox.Items.AddRange(Enum.GetNames(typeof(ConditionalType)));

        var valueLabel = new Label { Text = "Condition Value:", Location = new Point(10, 45), Size = new Size(100, 23) };
        _conditionalValueTextBox = new TextBox { Location = new Point(120, 42), Size = new Size(200, 23) };

        var ifLabel = new Label { Text = "If True Actions:", Location = new Point(10, 75), Size = new Size(100, 23) };
        _conditionalActionsListView = new ListView
        {
            Location = new Point(10, 100),
            Size = new Size(250, 150),
            View = View.Details,
            FullRowSelect = true
        };
        _conditionalActionsListView.Columns.Add("Action", 200);

        var elseLabel = new Label { Text = "If False Actions:", Location = new Point(280, 75), Size = new Size(100, 23) };
        _elseActionsListView = new ListView
        {
            Location = new Point(280, 100),
            Size = new Size(250, 150),
            View = View.Details,
            FullRowSelect = true
        };
        _elseActionsListView.Columns.Add("Action", 200);

        conditionalTab.Controls.AddRange(new Control[] {
            typeLabel, _conditionalTypeComboBox,
            valueLabel, _conditionalValueTextBox,
            ifLabel, _conditionalActionsListView,
            elseLabel, _elseActionsListView
        });

        _tabControl.TabPages.Add(conditionalTab);
    }

    private void CreateLoopTab()
    {
        var loopTab = new TabPage("Loop");

        var countLabel = new Label { Text = "Loop Count:", Location = new Point(10, 15), Size = new Size(80, 23) };
        _loopCountNumeric = new NumericUpDown 
        { 
            Location = new Point(100, 12), 
            Size = new Size(100, 23),
            Minimum = 1,
            Maximum = 1000,
            Value = 1
        };

        _infiniteLoopCheckBox = new CheckBox 
        { 
            Text = "Infinite Loop", 
            Location = new Point(10, 45), 
            Size = new Size(100, 23) 
        };
        _infiniteLoopCheckBox.CheckedChanged += (s, e) => _loopCountNumeric.Enabled = !_infiniteLoopCheckBox.Checked;

        loopTab.Controls.AddRange(new Control[] {
            countLabel, _loopCountNumeric, _infiniteLoopCheckBox
        });

        _tabControl.TabPages.Add(loopTab);
    }

    private void CreateVariableTab()
    {
        var variableTab = new TabPage("Variable");

        var nameLabel = new Label { Text = "Variable Name:", Location = new Point(10, 15), Size = new Size(100, 23) };
        _variableNameTextBox = new TextBox { Location = new Point(120, 12), Size = new Size(200, 23) };

        var valueLabel = new Label { Text = "Variable Value:", Location = new Point(10, 45), Size = new Size(100, 23) };
        _variableValueTextBox = new TextBox { Location = new Point(120, 42), Size = new Size(200, 23) };

        variableTab.Controls.AddRange(new Control[] {
            nameLabel, _variableNameTextBox,
            valueLabel, _variableValueTextBox
        });

        _tabControl.TabPages.Add(variableTab);
    }

    private void LoadActionData()
    {
        _actionTypeComboBox.SelectedItem = _action.Type.ToString();
        _descriptionTextBox.Text = _action.Description;
        _enabledCheckBox.Checked = _action.IsEnabled;
        _valueTextBox.Text = _action.Value;
        _value2TextBox.Text = _action.Value2;
        _durationNumeric.Value = _action.Duration;
        _xNumeric.Value = _action.X;
        _yNumeric.Value = _action.Y;
        _windowActionComboBox.SelectedItem = _action.WindowAction.ToString();
        _systemCommandComboBox.SelectedItem = _action.SystemCommand.ToString();
        _fileOperationComboBox.SelectedItem = _action.FileOperation.ToString();
        _conditionalTypeComboBox.SelectedItem = _action.ConditionalType.ToString();
        _conditionalValueTextBox.Text = _action.ConditionalValue;
        _loopCountNumeric.Value = _action.LoopCount;
        _variableNameTextBox.Text = _action.VariableName;
        _variableValueTextBox.Text = _action.VariableValue;

        OnActionTypeChanged(null, EventArgs.Empty);
    }

    private void OnActionTypeChanged(object? sender, EventArgs e)
    {
        if (_actionTypeComboBox.SelectedItem == null) return;

        var selectedType = (MacroActionType)Enum.Parse(typeof(MacroActionType), _actionTypeComboBox.SelectedItem.ToString()!);
        
        // Show/hide tabs based on action type
        _tabControl.TabPages.Clear();
        _tabControl.TabPages.Add(_tabControl.TabPages[0]); // Basic tab always visible

        switch (selectedType)
        {
            case MacroActionType.WindowControl:
                _tabControl.TabPages.Add(_tabControl.TabPages[1]); // Advanced tab
                break;
            case MacroActionType.Conditional:
                _tabControl.TabPages.Add(_tabControl.TabPages[2]); // Conditional tab
                break;
            case MacroActionType.Loop:
                _tabControl.TabPages.Add(_tabControl.TabPages[3]); // Loop tab
                break;
            case MacroActionType.Variable:
                _tabControl.TabPages.Add(_tabControl.TabPages[4]); // Variable tab
                break;
        }

        // Update value label based on type
        var valueLabel = _tabControl.TabPages[0].Controls.OfType<Label>().First(l => l.Text == "Value:");
        valueLabel.Text = selectedType switch
        {
            MacroActionType.KeyPress or MacroActionType.KeyRelease or MacroActionType.KeyHold => "Key Code:",
            MacroActionType.MouseClick => "Button (left/right):",
            MacroActionType.Text => "Text to Type:",
            MacroActionType.Application => "Application Path:",
            MacroActionType.WebRequest => "URL:",
            MacroActionType.FileOperation => "File Path:",
            _ => "Value:"
        };
    }

    private void OnTest(object? sender, EventArgs e)
    {
        SaveActionData();
        // Test the action (implementation depends on MacroService)
        MessageBox.Show("Action test would be executed here.", "Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void OnOK(object? sender, EventArgs e)
    {
        SaveActionData();
    }

    private void SaveActionData()
    {
        if (_actionTypeComboBox.SelectedItem != null)
        {
            _action.Type = (MacroActionType)Enum.Parse(typeof(MacroActionType), _actionTypeComboBox.SelectedItem.ToString()!);
        }
        
        _action.Description = _descriptionTextBox.Text;
        _action.IsEnabled = _enabledCheckBox.Checked;
        _action.Value = _valueTextBox.Text;
        _action.Value2 = _value2TextBox.Text;
        _action.Duration = (int)_durationNumeric.Value;
        _action.X = (int)_xNumeric.Value;
        _action.Y = (int)_yNumeric.Value;
        
        if (_windowActionComboBox.SelectedItem != null)
        {
            _action.WindowAction = (WindowAction)Enum.Parse(typeof(WindowAction), _windowActionComboBox.SelectedItem.ToString()!);
        }
        
        if (_systemCommandComboBox.SelectedItem != null)
        {
            _action.SystemCommand = (SystemCommandType)Enum.Parse(typeof(SystemCommandType), _systemCommandComboBox.SelectedItem.ToString()!);
        }
        
        if (_fileOperationComboBox.SelectedItem != null)
        {
            _action.FileOperation = (FileOperationType)Enum.Parse(typeof(FileOperationType), _fileOperationComboBox.SelectedItem.ToString()!);
        }
        
        if (_conditionalTypeComboBox.SelectedItem != null)
        {
            _action.ConditionalType = (ConditionalType)Enum.Parse(typeof(ConditionalType), _conditionalTypeComboBox.SelectedItem.ToString()!);
        }
        
        _action.ConditionalValue = _conditionalValueTextBox.Text;
        _action.LoopCount = (int)_loopCountNumeric.Value;
        _action.VariableName = _variableNameTextBox.Text;
        _action.VariableValue = _variableValueTextBox.Text;
    }
}