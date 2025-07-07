using KeyLayer.Models;
using KeyLayer.Services;

namespace KeyLayer.Forms;

public partial class MainForm : Form
{
    private readonly HIDDeviceManager _deviceManager;
    private readonly AdvancedMacroService _macroService;
    private readonly KeyboardHookService _keyboardHook;
    
    private ListView _deviceListView;
    private ListView _keyMappingListView;
    private Button _scanButton;
    private Button _isolateButton;
    private Button _releaseButton;
    private Button _addMacroButton;
    private Button _recordMacroButton;
    private Button _saveProfileButton;
    private Button _visualEditorButton;
    private Button _layerManagerButton;
    private Button _importExportButton;
    private ComboBox _profileComboBox;
    private GroupBox _deviceGroup;
    private GroupBox _macroGroup;
    private StatusStrip _statusStrip;
    private ToolStripStatusLabel _statusLabel;
    private MenuStrip _menuStrip;

    public MainForm(HIDDeviceManager deviceManager, AdvancedMacroService macroService, KeyboardHookService keyboardHook)
    {
        _deviceManager = deviceManager;
        _macroService = macroService;
        _keyboardHook = keyboardHook;
        
        InitializeComponent();
        SetupEventHandlers();
        SetupMenus();
        
        _deviceManager.StartScanning();
        _keyboardHook.StartHook();
        _macroService.LoadProfilesFromFile();
    }

    private void InitializeComponent()
    {
        Text = "KeyLayer - Advanced HID Macro Manager";
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;

        // Menu Strip
        _menuStrip = new MenuStrip();

        // Device Group
        _deviceGroup = new GroupBox
        {
            Text = "HID Devices",
            Location = new Point(10, 35),
            Size = new Size(580, 350)
        };

        _deviceListView = new ListView
        {
            Location = new Point(10, 20),
            Size = new Size(560, 250),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            MultiSelect = false
        };
        
        _deviceListView.Columns.Add("Device Name", 250);
        _deviceListView.Columns.Add("Manufacturer", 150);
        _deviceListView.Columns.Add("Status", 80);
        _deviceListView.Columns.Add("Profile", 80);

        _scanButton = new Button
        {
            Text = "Scan Devices",
            Location = new Point(10, 280),
            Size = new Size(100, 30)
        };

        _isolateButton = new Button
        {
            Text = "Isolate Device",
            Location = new Point(120, 280),
            Size = new Size(100, 30),
            Enabled = false
        };

        _releaseButton = new Button
        {
            Text = "Release Device",
            Location = new Point(230, 280),
            Size = new Size(100, 30),
            Enabled = false
        };

        _deviceGroup.Controls.AddRange(new Control[] { _deviceListView, _scanButton, _isolateButton, _releaseButton });

        // Macro Group
        _macroGroup = new GroupBox
        {
            Text = "Macro Configuration",
            Location = new Point(600, 35),
            Size = new Size(580, 350)
        };

        _profileComboBox = new ComboBox
        {
            Location = new Point(10, 20),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        _keyMappingListView = new ListView
        {
            Location = new Point(10, 55),
            Size = new Size(560, 190),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            MultiSelect = false
        };
        
        _keyMappingListView.Columns.Add("Key", 80);
        _keyMappingListView.Columns.Add("Type", 80);
        _keyMappingListView.Columns.Add("Actions", 200);
        _keyMappingListView.Columns.Add("Layer", 80);
        _keyMappingListView.Columns.Add("Status", 80);

        _addMacroButton = new Button
        {
            Text = "Add Macro",
            Location = new Point(10, 255),
            Size = new Size(90, 30)
        };

        _recordMacroButton = new Button
        {
            Text = "Record Macro",
            Location = new Point(110, 255),
            Size = new Size(90, 30)
        };

        _visualEditorButton = new Button
        {
            Text = "Visual Editor",
            Location = new Point(210, 255),
            Size = new Size(90, 30)
        };

        _layerManagerButton = new Button
        {
            Text = "Layer Manager",
            Location = new Point(310, 255),
            Size = new Size(90, 30)
        };

        _importExportButton = new Button
        {
            Text = "Import/Export",
            Location = new Point(410, 255),
            Size = new Size(90, 30)
        };

        _saveProfileButton = new Button
        {
            Text = "Save Profile",
            Location = new Point(510, 255),
            Size = new Size(90, 30)
        };

        _macroGroup.Controls.AddRange(new Control[] { 
            _profileComboBox, _keyMappingListView, _addMacroButton, _recordMacroButton, 
            _visualEditorButton, _layerManagerButton, _importExportButton, _saveProfileButton 
        });

        // Status Strip
        _statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel("Ready - KeyLayer Advanced HID Macro Manager");
        _statusStrip.Items.Add(_statusLabel);

        Controls.AddRange(new Control[] { _menuStrip, _deviceGroup, _macroGroup, _statusStrip });
        MainMenuStrip = _menuStrip;
    }

    private void SetupMenus()
    {
        // File Menu
        var fileMenu = new ToolStripMenuItem("File");
        fileMenu.DropDownItems.Add("New Profile", null, (s, e) => CreateNewProfile());
        fileMenu.DropDownItems.Add("Open Profile", null, (s, e) => OpenProfile());
        fileMenu.DropDownItems.Add("Save Profile", null, (s, e) => SaveCurrentProfile());
        fileMenu.DropDownItems.Add("Save Profile As...", null, (s, e) => SaveProfileAs());
        fileMenu.DropDownItems.Add("-");
        fileMenu.DropDownItems.Add("Import Macro", null, (s, e) => ImportMacro());
        fileMenu.DropDownItems.Add("Export Macro", null, (s, e) => ExportMacro());
        fileMenu.DropDownItems.Add("-");
        fileMenu.DropDownItems.Add("Exit", null, (s, e) => Close());

        // Edit Menu
        var editMenu = new ToolStripMenuItem("Edit");
        editMenu.DropDownItems.Add("Preferences", null, (s, e) => ShowPreferences());
        editMenu.DropDownItems.Add("Device Settings", null, (s, e) => ShowDeviceSettings());

        // Tools Menu
        var toolsMenu = new ToolStripMenuItem("Tools");
        toolsMenu.DropDownItems.Add("Visual Macro Editor", null, (s, e) => OpenVisualEditor());
        toolsMenu.DropDownItems.Add("Advanced Layer Manager", null, (s, e) => OpenLayerManager());
        toolsMenu.DropDownItems.Add("Template Library", null, (s, e) => OpenTemplateLibrary());
        toolsMenu.DropDownItems.Add("-");
        toolsMenu.DropDownItems.Add("Backup Profiles", null, (s, e) => BackupProfiles());
        toolsMenu.DropDownItems.Add("Restore Profiles", null, (s, e) => RestoreProfiles());

        // Help Menu
        var helpMenu = new ToolStripMenuItem("Help");
        helpMenu.DropDownItems.Add("User Guide", null, (s, e) => ShowUserGuide());
        helpMenu.DropDownItems.Add("Keyboard Shortcuts", null, (s, e) => ShowKeyboardShortcuts());
        helpMenu.DropDownItems.Add("About", null, (s, e) => ShowAbout());

        _menuStrip.Items.AddRange(new[] { fileMenu, editMenu, toolsMenu, helpMenu });
    }

    private void SetupEventHandlers()
    {
        _deviceManager.DeviceConnected += OnDeviceConnected;
        _deviceManager.DeviceDisconnected += OnDeviceDisconnected;
        _deviceManager.KeyPressed += OnKeyPressed;
        
        _scanButton.Click += (s, e) => RefreshDeviceList();
        _isolateButton.Click += OnIsolateDevice;
        _releaseButton.Click += OnReleaseDevice;
        _addMacroButton.Click += OnAddMacro;
        _recordMacroButton.Click += OnRecordMacro;
        _visualEditorButton.Click += (s, e) => OpenVisualEditor();
        _layerManagerButton.Click += (s, e) => OpenLayerManager();
        _importExportButton.Click += (s, e) => ShowImportExportDialog();
        _saveProfileButton.Click += OnSaveProfile;
        
        _deviceListView.SelectedIndexChanged += OnDeviceSelectionChanged;
        _profileComboBox.SelectedIndexChanged += OnProfileChanged;
        
        FormClosing += OnFormClosing;
    }

    private void OnDeviceConnected(object? sender, HIDDeviceInfo device)
    {
        if (InvokeRequired)
        {
            Invoke(() => OnDeviceConnected(sender, device));
            return;
        }

        var item = new ListViewItem(device.ProductName);
        item.SubItems.Add(device.ManufacturerName);
        item.SubItems.Add(device.IsIsolated ? "Isolated" : "Normal");
        
        var profile = _macroService.GetDeviceProfile(device.DevicePath);
        item.SubItems.Add(profile?.ProfileName ?? "None");
        item.Tag = device;
        
        _deviceListView.Items.Add(item);
        _statusLabel.Text = $"Device connected: {device.ProductName}";
    }

    private void OnDeviceDisconnected(object? sender, HIDDeviceInfo device)
    {
        if (InvokeRequired)
        {
            Invoke(() => OnDeviceDisconnected(sender, device));
            return;
        }

        var itemToRemove = _deviceListView.Items.Cast<ListViewItem>()
            .FirstOrDefault(item => ((HIDDeviceInfo)item.Tag).DevicePath == device.DevicePath);
        
        if (itemToRemove != null)
        {
            _deviceListView.Items.Remove(itemToRemove);
            _statusLabel.Text = $"Device disconnected: {device.ProductName}";
        }
    }

    private async void OnKeyPressed(object? sender, Services.KeyEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => OnKeyPressed(sender, e));
            return;
        }

        await _macroService.ProcessKeyPress(e.DevicePath, e.KeyCode, e.Modifiers);
    }

    private void RefreshDeviceList()
    {
        _deviceListView.Items.Clear();
        foreach (var device in _deviceManager.GetConnectedDevices())
        {
            OnDeviceConnected(null, device);
        }
    }

    private void OnIsolateDevice(object? sender, EventArgs e)
    {
        if (_deviceListView.SelectedItems.Count == 0) return;

        var selectedDevice = (HIDDeviceInfo)_deviceListView.SelectedItems[0].Tag;
        if (_deviceManager.IsolateDevice(selectedDevice.DevicePath))
        {
            _deviceListView.SelectedItems[0].SubItems[2].Text = "Isolated";
            KeyboardHookService.AddIsolatedDevice(selectedDevice.DevicePath);
            _statusLabel.Text = $"Device isolated: {selectedDevice.ProductName}";
            LoadDeviceProfile(selectedDevice.DevicePath);
        }
        else
        {
            MessageBox.Show("Failed to isolate device.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnReleaseDevice(object? sender, EventArgs e)
    {
        if (_deviceListView.SelectedItems.Count == 0) return;

        var selectedDevice = (HIDDeviceInfo)_deviceListView.SelectedItems[0].Tag;
        _deviceManager.ReleaseDevice(selectedDevice.DevicePath);
        _deviceListView.SelectedItems[0].SubItems[2].Text = "Normal";
        KeyboardHookService.RemoveIsolatedDevice(selectedDevice.DevicePath);
        _statusLabel.Text = $"Device released: {selectedDevice.ProductName}";
    }

    private void OnDeviceSelectionChanged(object? sender, EventArgs e)
    {
        bool hasSelection = _deviceListView.SelectedItems.Count > 0;
        _isolateButton.Enabled = hasSelection;
        _releaseButton.Enabled = hasSelection;
    }

    private void LoadDeviceProfile(string devicePath)
    {
        var profile = _macroService.GetDeviceProfile(devicePath);
        if (profile != null)
        {
            RefreshKeyMappingsList(profile);
        }
        else
        {
            var newProfile = new DeviceProfile
            {
                DevicePath = devicePath,
                ProfileName = "Default Profile",
                Created = DateTime.Now,
                Modified = DateTime.Now,
                IsActive = true
            };
            _macroService.SaveDeviceProfile(newProfile);
            RefreshKeyMappingsList(newProfile);
        }
    }

    private void RefreshKeyMappingsList(DeviceProfile profile)
    {
        _keyMappingListView.Items.Clear();
        
        foreach (var mapping in profile.KeyMappings.Values)
        {
            var item = new ListViewItem(mapping.KeyName);
            item.SubItems.Add(mapping.IsLayerKey ? "Layer" : "Macro");
            item.SubItems.Add($"{mapping.Actions.Count} actions");
            item.SubItems.Add($"Layer {mapping.LayerId}");
            item.SubItems.Add("Active");
            item.Tag = mapping;
            _keyMappingListView.Items.Add(item);
        }
    }

    private void OnAddMacro(object? sender, EventArgs e)
    {
        if (_deviceListView.SelectedItems.Count == 0)
        {
            MessageBox.Show("Please select a device first.", "No Device Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedDevice = (HIDDeviceInfo)_deviceListView.SelectedItems[0].Tag;
        var macroForm = new MacroConfigForm(selectedDevice.DevicePath, _macroService);
        macroForm.ShowDialog();
        
        var profile = _macroService.GetDeviceProfile(selectedDevice.DevicePath);
        if (profile != null)
        {
            RefreshKeyMappingsList(profile);
        }
    }

    private void OnRecordMacro(object? sender, EventArgs e)
    {
        if (_recordMacroButton.Text == "Record Macro")
        {
            _macroService.StartRecording();
            _recordMacroButton.Text = "Stop Recording";
            _statusLabel.Text = "Recording macro...";
        }
        else
        {
            var actions = _macroService.StopRecording();
            _recordMacroButton.Text = "Record Macro";
            _statusLabel.Text = $"Recorded {actions.Count} actions";
            
            var recordedForm = new RecordedMacroForm(actions, _macroService);
            recordedForm.ShowDialog();
        }
    }

    private void OpenVisualEditor()
    {
        var visualEditor = new VisualMacroEditor(_macroService);
        visualEditor.Show();
    }

    private void OpenLayerManager()
    {
        if (_deviceListView.SelectedItems.Count == 0)
        {
            MessageBox.Show("Please select a device first.", "No Device Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedDevice = (HIDDeviceInfo)_deviceListView.SelectedItems[0].Tag;
        var layerManager = new AdvancedLayerManager(selectedDevice.DevicePath, _macroService);
        layerManager.ShowDialog();
    }

    private void ShowImportExportDialog()
    {
        var importExportForm = new ImportExportDialog(_macroService);
        importExportForm.ShowDialog();
    }

    private void OnSaveProfile(object? sender, EventArgs e)
    {
        if (_deviceListView.SelectedItems.Count == 0) return;

        var selectedDevice = (HIDDeviceInfo)_deviceListView.SelectedItems[0].Tag;
        var profile = _macroService.GetDeviceProfile(selectedDevice.DevicePath);
        if (profile != null)
        {
            profile.Modified = DateTime.Now;
            _macroService.SaveDeviceProfile(profile);
            _statusLabel.Text = "Profile saved successfully";
        }
    }

    private void OnProfileChanged(object? sender, EventArgs e)
    {
        if (_profileComboBox.SelectedItem is DeviceProfile selectedProfile)
        {
            RefreshKeyMappingsList(selectedProfile);
            _statusLabel.Text = $"Switched to profile: {selectedProfile.ProfileName}";
        }
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        _keyboardHook.StopHook();
        
        foreach (var device in _deviceManager.GetConnectedDevices().Where(d => d.IsIsolated))
        {
            _deviceManager.ReleaseDevice(device.DevicePath);
        }
    }

    // Menu event handlers
    private void CreateNewProfile()
    {
        if (_deviceListView.SelectedItems.Count == 0)
        {
            MessageBox.Show("Please select a device first.", "No Device Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedDevice = (HIDDeviceInfo)_deviceListView.SelectedItems[0].Tag;
        var profileName = Microsoft.VisualBasic.Interaction.InputBox(
            "Enter profile name:", "New Profile", "New Profile");
        
        if (!string.IsNullOrWhiteSpace(profileName))
        {
            var newProfile = new DeviceProfile
            {
                DevicePath = selectedDevice.DevicePath,
                ProfileName = profileName,
                Created = DateTime.Now,
                Modified = DateTime.Now,
                IsActive = true
            };
            
            _macroService.SaveDeviceProfile(newProfile);
            RefreshKeyMappingsList(newProfile);
            _statusLabel.Text = $"Created new profile: {profileName}";
        }
    }

    private void OpenProfile()
    {
        var openDialog = new OpenFileDialog
        {
            Filter = "Profile Files (*.json)|*.json|All Files (*.*)|*.*",
            Title = "Open Profile"
        };

        if (openDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var json = File.ReadAllText(openDialog.FileName);
                var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceProfile>(json);
                
                if (profile != null)
                {
                    _macroService.SaveDeviceProfile(profile);
                    RefreshKeyMappingsList(profile);
                    _statusLabel.Text = $"Loaded profile: {profile.ProfileName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void SaveCurrentProfile()
    {
        if (_deviceListView.SelectedItems.Count == 0) return;

        var selectedDevice = (HIDDeviceInfo)_deviceListView.SelectedItems[0].Tag;
        var profile = _macroService.GetDeviceProfile(selectedDevice.DevicePath);
        
        if (profile != null)
        {
            profile.Modified = DateTime.Now;
            _macroService.SaveDeviceProfile(profile);
            _statusLabel.Text = "Profile saved successfully";
        }
    }

    private void SaveProfileAs()
    {
        if (_deviceListView.SelectedItems.Count == 0) return;

        var selectedDevice = (HIDDeviceInfo)_deviceListView.SelectedItems[0].Tag;
        var profile = _macroService.GetDeviceProfile(selectedDevice.DevicePath);
        
        if (profile != null)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Profile Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Save Profile As",
                FileName = $"{profile.ProfileName}.json"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(profile, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(saveDialog.FileName, json);
                    _statusLabel.Text = $"Profile saved to: {saveDialog.FileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void ImportMacro()
    {
        var importForm = new MacroImportForm();
        if (importForm.ShowDialog() == DialogResult.OK && importForm.ImportedActions.Count > 0)
        {
            _statusLabel.Text = $"Imported {importForm.ImportedActions.Count} actions";
        }
    }

    private void ExportMacro()
    {
        if (_keyMappingListView.SelectedItems.Count == 0)
        {
            MessageBox.Show("Please select a macro to export.", "No Macro Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedMapping = (KeyMapping)_keyMappingListView.SelectedItems[0].Tag;
        var exportForm = new MacroExportForm(selectedMapping.Actions);
        exportForm.ShowDialog();
    }

    private void ShowPreferences()
    {
        var preferencesForm = new PreferencesForm();
        preferencesForm.ShowDialog();
    }

    private void ShowDeviceSettings()
    {
        if (_deviceListView.SelectedItems.Count == 0)
        {
            MessageBox.Show("Please select a device first.", "No Device Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedDevice = (HIDDeviceInfo)_deviceListView.SelectedItems[0].Tag;
        var settingsForm = new DeviceSettingsForm(selectedDevice);
        settingsForm.ShowDialog();
    }

    private void OpenTemplateLibrary()
    {
        var libraryForm = new MacroTemplateLibraryForm();
        libraryForm.ShowDialog();
    }

    private void BackupProfiles()
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "Backup Files (*.backup)|*.backup|All Files (*.*)|*.*",
            Title = "Backup Profiles",
            FileName = $"KeyLayer_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.backup"
        };

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                if (File.Exists("profiles.json"))
                {
                    File.Copy("profiles.json", saveDialog.FileName, true);
                    _statusLabel.Text = $"Profiles backed up to: {saveDialog.FileName}";
                }
                else
                {
                    MessageBox.Show("No profiles found to backup.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error backing up profiles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void RestoreProfiles()
    {
        var openDialog = new OpenFileDialog
        {
            Filter = "Backup Files (*.backup)|*.backup|Profile Files (*.json)|*.json|All Files (*.*)|*.*",
            Title = "Restore Profiles"
        };

        if (openDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                File.Copy(openDialog.FileName, "profiles.json", true);
                _macroService.LoadProfilesFromFile();
                _statusLabel.Text = "Profiles restored successfully";
                
                // Refresh the current view
                if (_deviceListView.SelectedItems.Count > 0)
                {
                    var selectedDevice = (HIDDeviceInfo)_deviceListView.SelectedItems[0].Tag;
                    LoadDeviceProfile(selectedDevice.DevicePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring profiles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void ShowUserGuide()
    {
        var userGuideForm = new UserGuideForm();
        userGuideForm.ShowDialog();
    }

    private void ShowKeyboardShortcuts()
    {
        var shortcutsText = @"KeyLayer Keyboard Shortcuts:

File Operations:
Ctrl+N - New Profile
Ctrl+O - Open Profile
Ctrl+S - Save Profile
Ctrl+Shift+S - Save Profile As
Ctrl+I - Import Macro
Ctrl+E - Export Macro

Device Operations:
F5 - Refresh Device List
F6 - Isolate Selected Device
F7 - Release Selected Device

Macro Operations:
Ctrl+M - Add Macro
Ctrl+R - Record Macro
Ctrl+V - Visual Editor
Ctrl+L - Layer Manager

General:
F1 - Help
Alt+F4 - Exit";

        MessageBox.Show(shortcutsText, "Keyboard Shortcuts", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowAbout()
    {
        var aboutText = @"KeyLayer - Advanced HID Macro Manager
Version 1.0

A powerful Windows application for converting HID devices into advanced macro keyboards with layer support and visual editing.

Features:
• HID Device Detection and Isolation
• Advanced Macro Configuration (20+ Action Types)
• Visual Macro Editor
• Layer Management System
• Import/Export Support
• Template Library

Built with C# and .NET 8.0
Uses HidSharp for HID device communication

© 2024 KeyLayer Project";

        MessageBox.Show(aboutText, "About KeyLayer", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}