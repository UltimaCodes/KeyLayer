using KeyLayer.Models;
using KeyLayer.Services;

namespace KeyLayer.Forms;

public partial class AdvancedLayerManager : Form
{
    private readonly string _devicePath;
    private readonly MacroService _macroService;
    private DeviceProfile? _profile;
    
    private TreeView _layerTreeView;
    private ListView _layerKeysListView;
    private PropertyGrid _layerPropertyGrid;
    private Panel _layerPreviewPanel;
    private Button _addLayerButton;
    private Button _addGroupButton;
    private Button _deleteButton;
    private Button _copyButton;
    private Button _testLayerButton;
    private Button _saveButton;
    private Button _cancelButton;
    private ComboBox _activationTypeComboBox;
    private NumericUpDown _priorityNumeric;
    private CheckBox _isToggleCheckBox;
    private ColorDialog _colorDialog;

    public AdvancedLayerManager(string devicePath, MacroService macroService)
    {
        _devicePath = devicePath;
        _macroService = macroService;
        _profile = _macroService.GetDeviceProfile(devicePath);
        
        InitializeComponent();
        LoadLayers();
    }

    private void InitializeComponent()
    {
        Text = "Advanced Layer Manager";
        Size = new Size(1000, 700);
        StartPosition = FormStartPosition.CenterParent;

        // Left Panel - Layer Tree
        var leftPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 250,
            BorderStyle = BorderStyle.FixedSingle
        };

        var treeLabel = new Label { Text = "Layer Hierarchy:", Location = new Point(10, 10), Size = new Size(100, 23) };
        _layerTreeView = new TreeView
        {
            Location = new Point(10, 35),
            Size = new Size(230, 400),
            HideSelection = false,
            AllowDrop = true
        };
        _layerTreeView.AfterSelect += OnLayerSelected;
        _layerTreeView.DragEnter += OnTreeDragEnter;
        _layerTreeView.DragDrop += OnTreeDragDrop;
        _layerTreeView.ItemDrag += OnTreeItemDrag;

        _addLayerButton = new Button { Text = "Add Layer", Location = new Point(10, 445), Size = new Size(70, 30) };
        _addGroupButton = new Button { Text = "Add Group", Location = new Point(90, 445), Size = new Size(70, 30) };
        _deleteButton = new Button { Text = "Delete", Location = new Point(170, 445), Size = new Size(70, 30) };

        _copyButton = new Button { Text = "Copy Layer", Location = new Point(10, 485), Size = new Size(100, 30) };
        _testLayerButton = new Button { Text = "Test Layer", Location = new Point(120, 485), Size = new Size(100, 30) };

        leftPanel.Controls.AddRange(new Control[] {
            treeLabel, _layerTreeView, _addLayerButton, _addGroupButton, _deleteButton,
            _copyButton, _testLayerButton
        });

        // Center Panel - Layer Configuration
        var centerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };

        var configLabel = new Label { Text = "Layer Configuration:", Location = new Point(10, 10), Size = new Size(150, 23) };
        
        var activationLabel = new Label { Text = "Activation Type:", Location = new Point(10, 40), Size = new Size(100, 23) };
        _activationTypeComboBox = new ComboBox 
        { 
            Location = new Point(120, 37), 
            Size = new Size(150, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _activationTypeComboBox.Items.AddRange(Enum.GetNames(typeof(LayerActivationType)));

        var priorityLabel = new Label { Text = "Priority:", Location = new Point(280, 40), Size = new Size(60, 23) };
        _priorityNumeric = new NumericUpDown 
        { 
            Location = new Point(350, 37), 
            Size = new Size(80, 23),
            Maximum = 100,
            Value = 1
        };

        _isToggleCheckBox = new CheckBox 
        { 
            Text = "Toggle Mode", 
            Location = new Point(10, 70), 
            Size = new Size(100, 23) 
        };

        var keysLabel = new Label { Text = "Trigger Keys:", Location = new Point(10, 100), Size = new Size(100, 23) };
        _layerKeysListView = new ListView
        {
            Location = new Point(10, 125),
            Size = new Size(500, 150),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true
        };
        _layerKeysListView.Columns.Add("Key Name", 150);
        _layerKeysListView.Columns.Add("Key Code", 100);
        _layerKeysListView.Columns.Add("Modifier", 100);
        _layerKeysListView.Columns.Add("Action", 150);

        var previewLabel = new Label { Text = "Layer Preview:", Location = new Point(10, 285), Size = new Size(100, 23) };
        _layerPreviewPanel = new Panel
        {
            Location = new Point(10, 310),
            Size = new Size(500, 200),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White
        };
        _layerPreviewPanel.Paint += OnPreviewPaint;

        centerPanel.Controls.AddRange(new Control[] {
            configLabel, activationLabel, _activationTypeComboBox, priorityLabel, _priorityNumeric,
            _isToggleCheckBox, keysLabel, _layerKeysListView, previewLabel, _layerPreviewPanel
        });

        // Right Panel - Properties
        var rightPanel = new Panel
        {
            Dock = DockStyle.Right,
            Width = 250,
            BorderStyle = BorderStyle.FixedSingle
        };

        var propLabel = new Label { Text = "Properties:", Location = new Point(10, 10), Size = new Size(100, 23) };
        _layerPropertyGrid = new PropertyGrid
        {
            Location = new Point(10, 35),
            Size = new Size(230, 400)
        };

        rightPanel.Controls.AddRange(new Control[] { propLabel, _layerPropertyGrid });

        // Bottom Panel - Buttons
        var bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            BackColor = Color.LightGray
        };

        _saveButton = new Button 
        { 
            Text = "Save", 
            Location = new Point(800, 10), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };
        
        _cancelButton = new Button 
        { 
            Text = "Cancel", 
            Location = new Point(890, 10), 
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        bottomPanel.Controls.AddRange(new Control[] { _saveButton, _cancelButton });

        Controls.AddRange(new Control[] { leftPanel, centerPanel, rightPanel, bottomPanel });

        // Event handlers
        _addLayerButton.Click += OnAddLayer;
        _addGroupButton.Click += OnAddGroup;
        _deleteButton.Click += OnDelete;
        _copyButton.Click += OnCopyLayer;
        _testLayerButton.Click += OnTestLayer;
        _saveButton.Click += OnSave;
        _activationTypeComboBox.SelectedIndexChanged += OnActivationTypeChanged;
    }

    private void LoadLayers()
    {
        _layerTreeView.Nodes.Clear();
        
        if (_profile == null) return;

        // Create a root node for the device
        var deviceNode = new TreeNode($"Device: {Path.GetFileName(_profile.DevicePath)}")
        {
            Tag = _profile,
            ImageIndex = 0
        };
        _layerTreeView.Nodes.Add(deviceNode);

        // Add default layer
        var defaultLayer = new LayerConfiguration
        {
            LayerId = 0,
            LayerName = "Default Layer",
            Description = "Base layer - always active",
            ActivationType = LayerActivationType.Hold,
            IsActive = true
        };
        
        var defaultNode = new TreeNode("Default Layer")
        {
            Tag = defaultLayer,
            ImageIndex = 1
        };
        deviceNode.Nodes.Add(defaultNode);

        // Load existing layers from profile
        // This would be implemented based on your layer storage structure

        _layerTreeView.ExpandAll();
    }

    private void OnLayerSelected(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is LayerConfiguration layer)
        {
            LoadLayerConfiguration(layer);
            _layerPropertyGrid.SelectedObject = layer;
        }
        else
        {
            ClearLayerConfiguration();
            _layerPropertyGrid.SelectedObject = null;
        }
    }

    private void LoadLayerConfiguration(LayerConfiguration layer)
    {
        _activationTypeComboBox.SelectedItem = layer.ActivationType.ToString();
        _priorityNumeric.Value = layer.Priority;
        _isToggleCheckBox.Checked = layer.IsToggle;
        
        // Load trigger keys
        _layerKeysListView.Items.Clear();
        foreach (var keyCode in layer.TriggerKeys)
        {
            var item = new ListViewItem($"Key {keyCode}");
            item.SubItems.Add(keyCode.ToString());
            item.SubItems.Add("None"); // Modifier
            item.SubItems.Add("Trigger"); // Action
            item.Tag = keyCode;
            _layerKeysListView.Items.Add(item);
        }

        _layerPreviewPanel.Invalidate();
    }

    private void ClearLayerConfiguration()
    {
        _activationTypeComboBox.SelectedIndex = -1;
        _priorityNumeric.Value = 1;
        _isToggleCheckBox.Checked = false;
        _layerKeysListView.Items.Clear();
        _layerPreviewPanel.Invalidate();
    }

    private void OnPreviewPaint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Color.White);

        if (_layerTreeView.SelectedNode?.Tag is LayerConfiguration layer)
        {
            // Draw a visual representation of the layer
            var brush = new SolidBrush(ColorTranslator.FromHtml(layer.BackgroundColor));
            var textBrush = new SolidBrush(ColorTranslator.FromHtml(layer.TextColor));
            var font = new Font("Arial", 10);

            g.FillRectangle(brush, 10, 10, 200, 100);
            g.DrawString($"Layer: {layer.LayerName}", font, textBrush, 15, 15);
            g.DrawString($"Type: {layer.ActivationType}", font, textBrush, 15, 35);
            g.DrawString($"Priority: {layer.Priority}", font, textBrush, 15, 55);
            g.DrawString($"Keys: {layer.TriggerKeys.Count}", font, textBrush, 15, 75);

            // Draw key layout preview
            DrawKeyLayout(g, layer);
        }
    }

    private void DrawKeyLayout(Graphics g, LayerConfiguration layer)
    {
        var keySize = 30;
        var spacing = 5;
        var startX = 250;
        var startY = 20;

        // Draw a simplified keyboard layout
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                var x = startX + col * (keySize + spacing);
                var y = startY + row * (keySize + spacing);
                var rect = new Rectangle(x, y, keySize, keySize);

                var keyCode = row * 10 + col + 1; // Simplified key code calculation
                var isLayerKey = layer.TriggerKeys.Contains(keyCode);
                var brush = isLayerKey ? Brushes.Yellow : Brushes.LightGray;

                g.FillRectangle(brush, rect);
                g.DrawRectangle(Pens.Black, rect);

                if (isLayerKey)
                {
                    g.DrawString("L", new Font("Arial", 8), Brushes.Black, x + 10, y + 10);
                }
            }
        }
    }

    private void OnAddLayer(object? sender, EventArgs e)
    {
        var layer = new LayerConfiguration
        {
            LayerId = GetNextLayerId(),
            LayerName = "New Layer",
            Description = "New layer description",
            ActivationType = LayerActivationType.Hold,
            Priority = 1,
            Created = DateTime.Now,
            Modified = DateTime.Now
        };

        var layerNode = new TreeNode(layer.LayerName)
        {
            Tag = layer,
            ImageIndex = 1
        };

        if (_layerTreeView.SelectedNode != null)
        {
            _layerTreeView.SelectedNode.Nodes.Add(layerNode);
        }
        else
        {
            _layerTreeView.Nodes[0].Nodes.Add(layerNode);
        }

        _layerTreeView.SelectedNode = layerNode;
        _layerTreeView.ExpandAll();
    }

    private void OnAddGroup(object? sender, EventArgs e)
    {
        var group = new LayerGroup
        {
            GroupName = "New Group",
            IsExclusive = true
        };

        var groupNode = new TreeNode(group.GroupName)
        {
            Tag = group,
            ImageIndex = 2
        };

        if (_layerTreeView.SelectedNode != null)
        {
            _layerTreeView.SelectedNode.Nodes.Add(groupNode);
        }
        else
        {
            _layerTreeView.Nodes[0].Nodes.Add(groupNode);
        }

        _layerTreeView.SelectedNode = groupNode;
        _layerTreeView.ExpandAll();
    }

    private void OnDelete(object? sender, EventArgs e)
    {
        if (_layerTreeView.SelectedNode != null && _layerTreeView.SelectedNode.Parent != null)
        {
            if (MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _layerTreeView.SelectedNode.Remove();
            }
        }
    }

    private void OnCopyLayer(object? sender, EventArgs e)
    {
        if (_layerTreeView.SelectedNode?.Tag is LayerConfiguration layer)
        {
            var copy = new LayerConfiguration
            {
                LayerId = GetNextLayerId(),
                LayerName = layer.LayerName + " Copy",
                Description = layer.Description,
                ActivationType = layer.ActivationType,
                TriggerKeys = new List<int>(layer.TriggerKeys),
                IsToggle = layer.IsToggle,
                Priority = layer.Priority,
                LayerKeyMappings = new Dictionary<int, KeyMapping>(layer.LayerKeyMappings),
                BackgroundColor = layer.BackgroundColor,
                TextColor = layer.TextColor,
                Created = DateTime.Now,
                Modified = DateTime.Now
            };

            var copyNode = new TreeNode(copy.LayerName)
            {
                Tag = copy,
                ImageIndex = 1
            };

            _layerTreeView.SelectedNode.Parent?.Nodes.Add(copyNode);
            _layerTreeView.SelectedNode = copyNode;
        }
    }

    private void OnTestLayer(object? sender, EventArgs e)
    {
        if (_layerTreeView.SelectedNode?.Tag is LayerConfiguration layer)
        {
            MessageBox.Show($"Testing layer: {layer.LayerName}\n" +
                          $"Activation: {layer.ActivationType}\n" +
                          $"Trigger Keys: {string.Join(", ", layer.TriggerKeys)}\n" +
                          $"Priority: {layer.Priority}",
                          "Layer Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void OnActivationTypeChanged(object? sender, EventArgs e)
    {
        if (_layerTreeView.SelectedNode?.Tag is LayerConfiguration layer)
        {
            if (Enum.TryParse<LayerActivationType>(_activationTypeComboBox.SelectedItem?.ToString(), out var type))
            {
                layer.ActivationType = type;
                layer.Modified = DateTime.Now;
            }
        }
    }

    private void OnTreeDragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.Move;
    }

    private void OnTreeDragDrop(object? sender, DragEventArgs e)
    {
        // Implement drag and drop reordering
        var targetPoint = _layerTreeView.PointToClient(new Point(e.X, e.Y));
        var targetNode = _layerTreeView.GetNodeAt(targetPoint);
        
        if (e.Data?.GetData(typeof(TreeNode)) is TreeNode dragNode && targetNode != null && dragNode != targetNode)
        {
            dragNode.Remove();
            targetNode.Nodes.Add(dragNode);
            _layerTreeView.SelectedNode = dragNode;
        }
    }

    private void OnTreeItemDrag(object? sender, ItemDragEventArgs e)
    {
        if (e.Item is TreeNode node)
        {
            DoDragDrop(node, DragDropEffects.Move);
        }
    }

    private int GetNextLayerId()
    {
        // Implementation to get the next available layer ID
        return DateTime.Now.Millisecond; // Simplified
    }

    private void OnSave(object? sender, EventArgs e)
    {
        if (_profile != null)
        {
            // Save layer configurations to profile
            // This would involve serializing the layer tree structure
            _profile.Modified = DateTime.Now;
            _macroService.SaveDeviceProfile(_profile);
        }
    }
}