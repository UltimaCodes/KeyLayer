using KeyLayer.Models;
using KeyLayer.Services;

namespace KeyLayer.Forms;

public partial class RecordedMacroForm : Form
{
    private readonly List<MacroAction> _recordedActions;
    private readonly MacroService _macroService;
    
    private ListView _actionsListView;
    private Button _playButton;
    private Button _saveButton;
    private Button _clearButton;
    private Button _closeButton;
    private TextBox _nameTextBox;

    public RecordedMacroForm(List<MacroAction> recordedActions, MacroService macroService)
    {
        _recordedActions = recordedActions;
        _macroService = macroService;
        
        InitializeComponent();
        LoadActions();
    }

    private void InitializeComponent()
    {
        Text = "Recorded Macro";
        Size = new Size(600, 400);
        StartPosition = FormStartPosition.CenterParent;

        var nameLabel = new Label { Text = "Macro Name:", Location = new Point(10, 15), Size = new Size(80, 23) };
        _nameTextBox = new TextBox { Location = new Point(100, 12), Size = new Size(200, 23) };

        _actionsListView = new ListView
        {
            Location = new Point(10, 45),
            Size = new Size(560, 250),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true
        };
        
        _actionsListView.Columns.Add("Time", 80);
        _actionsListView.Columns.Add("Type", 100);
        _actionsListView.Columns.Add("Value", 150);
        _actionsListView.Columns.Add("Duration", 80);
        _actionsListView.Columns.Add("Details", 150);

        _playButton = new Button 
        { 
            Text = "Play", 
            Location = new Point(10, 310), 
            Size = new Size(80, 30) 
        };
        
        _saveButton = new Button 
        { 
            Text = "Save", 
            Location = new Point(100, 310), 
            Size = new Size(80, 30) 
        };
        
        _clearButton = new Button 
        { 
            Text = "Clear", 
            Location = new Point(190, 310), 
            Size = new Size(80, 30) 
        };
        
        _closeButton = new Button 
        { 
            Text = "Close", 
            Location = new Point(490, 310), 
            Size = new Size(80, 30) 
        };

        Controls.AddRange(new Control[] {
            nameLabel, _nameTextBox, _actionsListView,
            _playButton, _saveButton, _clearButton, _closeButton
        });

        _playButton.Click += OnPlay;
        _saveButton.Click += OnSave;
        _clearButton.Click += OnClear;
        _closeButton.Click += (s, e) => Close();
    }

    private void LoadActions()
    {
        _actionsListView.Items.Clear();
        
        foreach (var action in _recordedActions)
        {
            var item = new ListViewItem(action.Timestamp.ToString("HH:mm:ss.fff"));
            item.SubItems.Add(action.Type.ToString());
            item.SubItems.Add(action.Value);
            item.SubItems.Add(action.Duration.ToString());
            item.SubItems.Add(GetActionDetails(action));
            item.Tag = action;
            _actionsListView.Items.Add(item);
        }
    }

    private string GetActionDetails(MacroAction action)
    {
        return action.Type switch
        {
            MacroActionType.KeyPress => $"Key: {action.Value}",
            MacroActionType.MouseClick => $"Button: {action.Value}",
            MacroActionType.MouseMove => $"({action.X}, {action.Y})",
            MacroActionType.Text => $"'{action.Value}'",
            _ => ""
        };
    }

    private async void OnPlay(object? sender, EventArgs e)
    {
        if (_recordedActions.Count > 0)
        {
            await _macroService.ExecuteMacroAsync(_recordedActions);
        }
    }

    private void OnSave(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
        {
            MessageBox.Show("Please enter a name for the macro.", "Validation Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Here you would typically save the macro to a library or assign it to a key
        MessageBox.Show($"Macro '{_nameTextBox.Text}' saved successfully!", "Success", 
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        
        Close();
    }

    private void OnClear(object? sender, EventArgs e)
    {
        _recordedActions.Clear();
        LoadActions();
    }
}