using KeyLayer.Models;
using KeyLayer.Services;

namespace KeyLayer.Forms;

public partial class VisualMacroEditor : Form
{
    private readonly MacroService _macroService;
    private List<MacroAction> _actions = new();
    private MacroAction? _selectedAction;
    private bool _isDragging = false;
    private Point _dragOffset;
    private Panel _canvasPanel;
    private Panel _toolboxPanel;
    private PropertyGrid _propertyGrid;
    private Button _playButton;
    private Button _saveButton;
    private Button _loadButton;
    private Button _clearButton;
    private MenuStrip _menuStrip;
    private ContextMenuStrip _canvasContextMenu;
    private MacroAction? _clipboardAction;
    private Stack<List<MacroAction>> _undoStack = new();
    private Stack<List<MacroAction>> _redoStack = new();

    public VisualMacroEditor(MacroService macroService)
    {
        _macroService = macroService;
        InitializeComponent();
        SetupToolbox();
        SetupContextMenu();
    }

    private void InitializeComponent()
    {
        Text = "Visual Macro Editor";
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;

        // Menu Strip
        _menuStrip = new MenuStrip();
        var fileMenu = new ToolStripMenuItem("File");
        fileMenu.DropDownItems.Add("New", null, (s, e) => NewMacro());
        fileMenu.DropDownItems.Add("Open", null, (s, e) => OpenMacro());
        fileMenu.DropDownItems.Add("Save", null, (s, e) => SaveMacro());
        fileMenu.DropDownItems.Add("Export", null, (s, e) => ExportMacro());
        fileMenu.DropDownItems.Add("Import", null, (s, e) => ImportMacro());
        
        var editMenu = new ToolStripMenuItem("Edit");
        editMenu.DropDownItems.Add("Undo", null, (s, e) => Undo());
        editMenu.DropDownItems.Add("Redo", null, (s, e) => Redo());
        editMenu.DropDownItems.Add("Copy", null, (s, e) => CopyAction());
        editMenu.DropDownItems.Add("Paste", null, (s, e) => PasteAction());
        editMenu.DropDownItems.Add("Delete", null, (s, e) => DeleteAction());
        
        _menuStrip.Items.AddRange(new[] { fileMenu, editMenu });

        // Toolbox Panel
        _toolboxPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 200,
            BackColor = Color.LightGray,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Canvas Panel
        _canvasPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            AllowDrop = true
        };
        _canvasPanel.Paint += OnCanvasPaint;
        _canvasPanel.MouseDown += OnCanvasMouseDown;
        _canvasPanel.MouseMove += OnCanvasMouseMove;
        _canvasPanel.MouseUp += OnCanvasMouseUp;
        _canvasPanel.DragEnter += OnCanvasDragEnter;
        _canvasPanel.DragDrop += OnCanvasDragDrop;

        // Property Grid
        _propertyGrid = new PropertyGrid
        {
            Dock = DockStyle.Right,
            Width = 300
        };

        // Control Panel
        var controlPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            BackColor = Color.LightGray
        };

        _playButton = new Button { Text = "Play", Location = new Point(10, 10), Size = new Size(80, 30) };
        _saveButton = new Button { Text = "Save", Location = new Point(100, 10), Size = new Size(80, 30) };
        _loadButton = new Button { Text = "Load", Location = new Point(190, 10), Size = new Size(80, 30) };
        _clearButton = new Button { Text = "Clear", Location = new Point(280, 10), Size = new Size(80, 30) };

        controlPanel.Controls.AddRange(new Control[] { _playButton, _saveButton, _loadButton, _clearButton });

        Controls.AddRange(new Control[] { _menuStrip, _toolboxPanel, _canvasPanel, _propertyGrid, controlPanel });
        MainMenuStrip = _menuStrip;

        // Event handlers
        _playButton.Click += async (s, e) => await PlayMacro();
        _saveButton.Click += (s, e) => SaveMacro();
        _loadButton.Click += (s, e) => LoadMacro();
        _clearButton.Click += (s, e) => ClearCanvas();
    }

    private void SetupToolbox()
    {
        var toolboxLabel = new Label
        {
            Text = "Action Types",
            Location = new Point(10, 10),
            Size = new Size(180, 20),
            Font = new Font("Arial", 10, FontStyle.Bold)
        };
        _toolboxPanel.Controls.Add(toolboxLabel);

        var actionTypes = Enum.GetValues<MacroActionType>();
        int y = 40;

        foreach (var actionType in actionTypes)
        {
            var button = new Button
            {
                Text = actionType.ToString(),
                Location = new Point(10, y),
                Size = new Size(180, 30),
                Tag = actionType,
                BackColor = GetActionTypeColor(actionType)
            };
            
            button.MouseDown += OnToolboxButtonMouseDown;
            _toolboxPanel.Controls.Add(button);
            y += 35;
        }
    }

    private Color GetActionTypeColor(MacroActionType actionType)
    {
        return actionType switch
        {
            MacroActionType.KeyPress => Color.LightBlue,
            MacroActionType.MouseClick => Color.LightGreen,
            MacroActionType.Delay => Color.LightYellow,
            MacroActionType.Text => Color.LightPink,
            MacroActionType.Application => Color.LightCyan,
            MacroActionType.Conditional => Color.Orange,
            MacroActionType.Loop => Color.Purple,
            _ => Color.LightGray
        };
    }

    private void SetupContextMenu()
    {
        _canvasContextMenu = new ContextMenuStrip();
        _canvasContextMenu.Items.Add("Add Action", null, (s, e) => ShowAddActionDialog());
        _canvasContextMenu.Items.Add("Paste", null, (s, e) => PasteAction());
        _canvasContextMenu.Items.Add("-");
        _canvasContextMenu.Items.Add("Select All", null, (s, e) => SelectAllActions());
        _canvasContextMenu.Items.Add("Clear Canvas", null, (s, e) => ClearCanvas());
        
        _canvasPanel.ContextMenuStrip = _canvasContextMenu;
    }

    private void OnToolboxButtonMouseDown(object? sender, MouseEventArgs e)
    {
        if (sender is Button button && button.Tag is MacroActionType actionType)
        {
            var dragData = new DataObject("ActionType", actionType);
            DoDragDrop(dragData, DragDropEffects.Copy);
        }
    }

    private void OnCanvasDragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent("ActionType") == true)
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void OnCanvasDragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData("ActionType") is MacroActionType actionType)
        {
            var location = _canvasPanel.PointToClient(new Point(e.X, e.Y));
            CreateActionAtLocation(actionType, location);
        }
    }

    private void CreateActionAtLocation(MacroActionType actionType, Point location)
    {
        SaveState();
        
        var action = new MacroAction
        {
            Type = actionType,
            VisualX = location.X,
            VisualY = location.Y,
            Description = $"New {actionType}"
        };

        _actions.Add(action);
        _canvasPanel.Invalidate();
        
        // Show configuration dialog for the new action
        ShowActionConfigDialog(action);
    }

    private void ShowActionConfigDialog(MacroAction action)
    {
        var configForm = new AdvancedActionConfigForm(action);
        if (configForm.ShowDialog() == DialogResult.OK)
        {
            _canvasPanel.Invalidate();
        }
    }

    private void OnCanvasPaint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Draw grid
        DrawGrid(g);

        // Draw connections between actions
        DrawConnections(g);

        // Draw actions
        foreach (var action in _actions)
        {
            DrawAction(g, action);
        }
    }

    private void DrawGrid(Graphics g)
    {
        var gridSize = 20;
        var pen = new Pen(Color.LightGray, 1);

        for (int x = 0; x < _canvasPanel.Width; x += gridSize)
        {
            g.DrawLine(pen, x, 0, x, _canvasPanel.Height);
        }

        for (int y = 0; y < _canvasPanel.Height; y += gridSize)
        {
            g.DrawLine(pen, 0, y, _canvasPanel.Width, y);
        }
    }

    private void DrawConnections(Graphics g)
    {
        var pen = new Pen(Color.Blue, 2);
        
        foreach (var action in _actions)
        {
            foreach (var connectedId in action.ConnectedActions)
            {
                var connectedAction = _actions.FirstOrDefault(a => a.Id == connectedId);
                if (connectedAction != null)
                {
                    var startPoint = new Point(action.VisualX + 50, action.VisualY + 25);
                    var endPoint = new Point(connectedAction.VisualX, connectedAction.VisualY + 25);
                    
                    g.DrawLine(pen, startPoint, endPoint);
                    
                    // Draw arrow
                    DrawArrow(g, pen, startPoint, endPoint);
                }
            }
        }
    }

    private void DrawArrow(Graphics g, Pen pen, Point start, Point end)
    {
        var arrowSize = 10;
        var angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
        
        var arrowPoint1 = new Point(
            (int)(end.X - arrowSize * Math.Cos(angle - Math.PI / 6)),
            (int)(end.Y - arrowSize * Math.Sin(angle - Math.PI / 6))
        );
        
        var arrowPoint2 = new Point(
            (int)(end.X - arrowSize * Math.Cos(angle + Math.PI / 6)),
            (int)(end.Y - arrowSize * Math.Sin(angle + Math.PI / 6))
        );
        
        g.DrawLine(pen, end, arrowPoint1);
        g.DrawLine(pen, end, arrowPoint2);
    }

    private void DrawAction(Graphics g, MacroAction action)
    {
        var rect = new Rectangle(action.VisualX, action.VisualY, 100, 50);
        var brush = new SolidBrush(GetActionTypeColor(action.Type));
        var pen = new Pen(action == _selectedAction ? Color.Red : Color.Black, 2);

        g.FillRectangle(brush, rect);
        g.DrawRectangle(pen, rect);

        var font = new Font("Arial", 8, FontStyle.Bold);
        var textBrush = new SolidBrush(Color.Black);
        var textRect = new RectangleF(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
        
        var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        g.DrawString(action.Type.ToString(), font, textBrush, textRect, format);
        
        if (!action.IsEnabled)
        {
            var disabledPen = new Pen(Color.Red, 3);
            g.DrawLine(disabledPen, rect.Left, rect.Top, rect.Right, rect.Bottom);
            g.DrawLine(disabledPen, rect.Right, rect.Top, rect.Left, rect.Bottom);
        }
    }

    private void OnCanvasMouseDown(object? sender, MouseEventArgs e)
    {
        var clickedAction = GetActionAtPoint(e.Location);
        
        if (clickedAction != null)
        {
            _selectedAction = clickedAction;
            _propertyGrid.SelectedObject = clickedAction;
            _isDragging = true;
            _dragOffset = new Point(e.X - clickedAction.VisualX, e.Y - clickedAction.VisualY);
            
            if (e.Button == MouseButtons.Right)
            {
                ShowActionContextMenu(clickedAction, e.Location);
            }
            else if (e.Clicks == 2)
            {
                ShowActionConfigDialog(clickedAction);
            }
        }
        else
        {
            _selectedAction = null;
            _propertyGrid.SelectedObject = null;
        }
        
        _canvasPanel.Invalidate();
    }

    private void OnCanvasMouseMove(object? sender, MouseEventArgs e)
    {
        if (_isDragging && _selectedAction != null)
        {
            _selectedAction.VisualX = e.X - _dragOffset.X;
            _selectedAction.VisualY = e.Y - _dragOffset.Y;
            _canvasPanel.Invalidate();
        }
    }

    private void OnCanvasMouseUp(object? sender, MouseEventArgs e)
    {
        _isDragging = false;
    }

    private MacroAction? GetActionAtPoint(Point point)
    {
        return _actions.FirstOrDefault(action =>
        {
            var rect = new Rectangle(action.VisualX, action.VisualY, 100, 50);
            return rect.Contains(point);
        });
    }

    private void ShowActionContextMenu(MacroAction action, Point location)
    {
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Configure", null, (s, e) => ShowActionConfigDialog(action));
        contextMenu.Items.Add("Copy", null, (s, e) => CopyAction(action));
        contextMenu.Items.Add("Delete", null, (s, e) => DeleteAction(action));
        contextMenu.Items.Add("-");
        contextMenu.Items.Add(action.IsEnabled ? "Disable" : "Enable", null, (s, e) => ToggleActionEnabled(action));
        
        contextMenu.Show(_canvasPanel, location);
    }

    private void ToggleActionEnabled(MacroAction action)
    {
        SaveState();
        action.IsEnabled = !action.IsEnabled;
        _canvasPanel.Invalidate();
    }

    private async Task PlayMacro()
    {
        if (_actions.Count > 0)
        {
            await _macroService.ExecuteMacroAsync(_actions.Where(a => a.IsEnabled).ToList());
        }
    }

    private void SaveMacro()
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "Macro Files (*.macro)|*.macro|All Files (*.*)|*.*",
            DefaultExt = "macro"
        };

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            var template = new MacroTemplate
            {
                Name = Path.GetFileNameWithoutExtension(saveDialog.FileName),
                Actions = _actions,
                Created = DateTime.Now
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(template, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(saveDialog.FileName, json);
        }
    }

    private void LoadMacro()
    {
        var openDialog = new OpenFileDialog
        {
            Filter = "Macro Files (*.macro)|*.macro|All Files (*.*)|*.*"
        };

        if (openDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var json = File.ReadAllText(openDialog.FileName);
                var template = Newtonsoft.Json.JsonConvert.DeserializeObject<MacroTemplate>(json);
                
                if (template != null)
                {
                    SaveState();
                    _actions = template.Actions;
                    _canvasPanel.Invalidate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading macro: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void NewMacro()
    {
        SaveState();
        _actions.Clear();
        _selectedAction = null;
        _propertyGrid.SelectedObject = null;
        _canvasPanel.Invalidate();
    }

    private void OpenMacro() => LoadMacro();
    
    private void ExportMacro()
    {
        var exportForm = new MacroExportForm(_actions);
        exportForm.ShowDialog();
    }

    private void ImportMacro()
    {
        var importForm = new MacroImportForm();
        if (importForm.ShowDialog() == DialogResult.OK)
        {
            SaveState();
            _actions.AddRange(importForm.ImportedActions);
            _canvasPanel.Invalidate();
        }
    }

    private void ClearCanvas()
    {
        if (MessageBox.Show("Are you sure you want to clear the canvas?", "Confirm", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            SaveState();
            _actions.Clear();
            _selectedAction = null;
            _propertyGrid.SelectedObject = null;
            _canvasPanel.Invalidate();
        }
    }

    private void ShowAddActionDialog()
    {
        var actionTypes = Enum.GetValues<MacroActionType>();
        var dialog = new Form
        {
            Text = "Add Action",
            Size = new Size(300, 400),
            StartPosition = FormStartPosition.CenterParent
        };

        var listBox = new ListBox
        {
            Location = new Point(10, 10),
            Size = new Size(260, 300)
        };
        listBox.Items.AddRange(actionTypes.Cast<object>().ToArray());

        var okButton = new Button
        {
            Text = "OK",
            Location = new Point(120, 320),
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };

        var cancelButton = new Button
        {
            Text = "Cancel",
            Location = new Point(210, 320),
            Size = new Size(60, 30),
            DialogResult = DialogResult.Cancel
        };

        dialog.Controls.AddRange(new Control[] { listBox, okButton, cancelButton });

        if (dialog.ShowDialog() == DialogResult.OK && listBox.SelectedItem is MacroActionType actionType)
        {
            CreateActionAtLocation(actionType, new Point(100, 100));
        }
    }

    private void SelectAllActions()
    {
        // For simplicity, just select the first action
        if (_actions.Count > 0)
        {
            _selectedAction = _actions[0];
            _propertyGrid.SelectedObject = _selectedAction;
            _canvasPanel.Invalidate();
        }
    }

    private void SaveState()
    {
        var state = _actions.Select(a => new MacroAction
        {
            Id = a.Id,
            Type = a.Type,
            Value = a.Value,
            VisualX = a.VisualX,
            VisualY = a.VisualY,
            IsEnabled = a.IsEnabled,
            ConnectedActions = new List<Guid>(a.ConnectedActions)
        }).ToList();
        
        _undoStack.Push(state);
        _redoStack.Clear();
    }

    private void Undo()
    {
        if (_undoStack.Count > 0)
        {
            _redoStack.Push(_actions.ToList());
            _actions = _undoStack.Pop();
            _selectedAction = null;
            _propertyGrid.SelectedObject = null;
            _canvasPanel.Invalidate();
        }
    }

    private void Redo()
    {
        if (_redoStack.Count > 0)
        {
            _undoStack.Push(_actions.ToList());
            _actions = _redoStack.Pop();
            _selectedAction = null;
            _propertyGrid.SelectedObject = null;
            _canvasPanel.Invalidate();
        }
    }

    private void CopyAction(MacroAction? action = null)
    {
        if (action != null || _selectedAction != null)
        {
            _clipboardAction = action ?? _selectedAction;
        }
    }

    private void PasteAction()
    {
        if (_clipboardAction != null)
        {
            SaveState();
            var newAction = new MacroAction
            {
                Type = _clipboardAction.Type,
                Value = _clipboardAction.Value,
                VisualX = _clipboardAction.VisualX + 20,
                VisualY = _clipboardAction.VisualY + 20,
                IsEnabled = _clipboardAction.IsEnabled,
                Description = _clipboardAction.Description + " Copy"
            };
            
            _actions.Add(newAction);
            _canvasPanel.Invalidate();
        }
    }

    private void DeleteAction(MacroAction? action = null)
    {
        var actionToDelete = action ?? _selectedAction;
        if (actionToDelete != null)
        {
            SaveState();
            _actions.Remove(actionToDelete);
            if (_selectedAction == actionToDelete)
            {
                _selectedAction = null;
                _propertyGrid.SelectedObject = null;
            }
            _canvasPanel.Invalidate();
        }
    }
}