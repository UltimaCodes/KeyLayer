using KeyLayer.Models;

namespace KeyLayer.Forms;

public partial class ActionConfigForm : Form
{
    private ComboBox _actionTypeComboBox;
    private TextBox _valueTextBox;
    private NumericUpDown _durationNumeric;
    private NumericUpDown _xNumeric;
    private NumericUpDown _yNumeric;
    private Label _valueLabel;
    private Label _durationLabel;
    private Label _positionLabel;
    private Button _okButton;
    private Button _cancelButton;

    public ActionConfigForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Configure Action";
        Size = new Size(400, 250);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var typeLabel = new Label { Text = "Action Type:", Location = new Point(10, 15), Size = new Size(80, 23) };
        _actionTypeComboBox = new ComboBox 
        { 
            Location = new Point(100, 12), 
            Size = new Size(150, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        
        _actionTypeComboBox.Items.AddRange(Enum.GetNames(typeof(MacroActionType)));
        _actionTypeComboBox.SelectedIndex = 0;

        _valueLabel = new Label { Text = "Value:", Location = new Point(10, 45), Size = new Size(80, 23) };
        _valueTextBox = new TextBox { Location = new Point(100, 42), Size = new Size(250, 23) };

        _durationLabel = new Label { Text = "Duration (ms):", Location = new Point(10, 75), Size = new Size(80, 23) };
        _durationNumeric = new NumericUpDown 
        { 
            Location = new Point(100, 72), 
            Size = new Size(100, 23),
            Maximum = 10000,
            Value = 100
        };

        _positionLabel = new Label { Text = "Position:", Location = new Point(10, 105), Size = new Size(80, 23) };
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

        _okButton = new Button 
        { 
            Text = "OK", 
            Location = new Point(200, 150), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };
        
        _cancelButton = new Button 
        { 
            Text = "Cancel", 
            Location = new Point(290, 150), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[] {
            typeLabel, _actionTypeComboBox, _valueLabel, _valueTextBox,
            _durationLabel, _durationNumeric, _positionLabel, 
            xLabel, _xNumeric, yLabel, _yNumeric,
            _okButton, _cancelButton
        });

        _actionTypeComboBox.SelectedIndexChanged += OnActionTypeChanged;
        OnActionTypeChanged(null, EventArgs.Empty); // Initialize visibility
    }

    private void OnActionTypeChanged(object? sender, EventArgs e)
    {
        var selectedType = (MacroActionType)Enum.Parse(typeof(MacroActionType), _actionTypeComboBox.SelectedItem.ToString()!);
        
        // Show/hide controls based on action type
        _valueLabel.Visible = selectedType != MacroActionType.Delay;
        _valueTextBox.Visible = selectedType != MacroActionType.Delay;
        
        _durationLabel.Visible = selectedType == MacroActionType.KeyPress || 
                                selectedType == MacroActionType.KeyHold || 
                                selectedType == MacroActionType.Delay;
        _durationNumeric.Visible = _durationLabel.Visible;
        
        _positionLabel.Visible = selectedType == MacroActionType.MouseMove;
        _xNumeric.Visible = _positionLabel.Visible;
        _yNumeric.Visible = _positionLabel.Visible;

        // Update value label text based on type
        _valueLabel.Text = selectedType switch
        {
            MacroActionType.KeyPress or MacroActionType.KeyRelease or MacroActionType.KeyHold => "Key Code:",
            MacroActionType.MouseClick => "Button (left/right):",
            MacroActionType.Text => "Text to Type:",
            MacroActionType.Application => "Application Path:",
            _ => "Value:"
        };
    }

    public MacroAction GetAction()
    {
        var actionType = (MacroActionType)Enum.Parse(typeof(MacroActionType), _actionTypeComboBox.SelectedItem.ToString()!);
        
        return new MacroAction
        {
            Type = actionType,
            Value = _valueTextBox.Text,
            Duration = (int)_durationNumeric.Value,
            X = (int)_xNumeric.Value,
            Y = (int)_yNumeric.Value
        };
    }
}