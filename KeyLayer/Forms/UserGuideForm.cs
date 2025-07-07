namespace KeyLayer.Forms;

public partial class UserGuideForm : Form
{
    private TreeView _topicsTreeView;
    private RichTextBox _contentRichTextBox;
    private Button _closeButton;

    public UserGuideForm()
    {
        InitializeComponent();
        LoadUserGuide();
    }

    private void InitializeComponent()
    {
        Text = "KeyLayer User Guide";
        Size = new Size(800, 600);
        StartPosition = FormStartPosition.CenterParent;

        _topicsTreeView = new TreeView
        {
            Location = new Point(10, 10),
            Size = new Size(200, 520),
            HideSelection = false
        };
        _topicsTreeView.AfterSelect += OnTopicSelected;

        _contentRichTextBox = new RichTextBox
        {
            Location = new Point(220, 10),
            Size = new Size(560, 520),
            ReadOnly = true,
            Font = new Font("Segoe UI", 10)
        };

        _closeButton = new Button
        {
            Text = "Close",
            Location = new Point(700, 540),
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };

        Controls.AddRange(new Control[] { _topicsTreeView, _contentRichTextBox, _closeButton });
    }

    private void LoadUserGuide()
    {
        // Getting Started
        var gettingStarted = new TreeNode("Getting Started");
        gettingStarted.Nodes.Add(new TreeNode("Installation") { Tag = "installation" });
        gettingStarted.Nodes.Add(new TreeNode("First Setup") { Tag = "first_setup" });
        gettingStarted.Nodes.Add(new TreeNode("Basic Concepts") { Tag = "basic_concepts" });

        // Device Management
        var deviceManagement = new TreeNode("Device Management");
        deviceManagement.Nodes.Add(new TreeNode("Detecting Devices") { Tag = "detecting_devices" });
        deviceManagement.Nodes.Add(new TreeNode("Device Isolation") { Tag = "device_isolation" });
        deviceManagement.Nodes.Add(new TreeNode("Device Settings") { Tag = "device_settings" });

        // Macro Configuration
        var macroConfig = new TreeNode("Macro Configuration");
        macroConfig.Nodes.Add(new TreeNode("Creating Macros") { Tag = "creating_macros" });
        macroConfig.Nodes.Add(new TreeNode("Action Types") { Tag = "action_types" });
        macroConfig.Nodes.Add(new TreeNode("Recording Macros") { Tag = "recording_macros" });

        // Layer System
        var layerSystem = new TreeNode("Layer System");
        layerSystem.Nodes.Add(new TreeNode("Understanding Layers") { Tag = "understanding_layers" });
        layerSystem.Nodes.Add(new TreeNode("Layer Configuration") { Tag = "layer_configuration" });
        layerSystem.Nodes.Add(new TreeNode("Advanced Layer Features") { Tag = "advanced_layers" });

        // Visual Editor
        var visualEditor = new TreeNode("Visual Editor");
        visualEditor.Nodes.Add(new TreeNode("Editor Overview") { Tag = "editor_overview" });
        visualEditor.Nodes.Add(new TreeNode("Creating Visual Macros") { Tag = "visual_macros" });
        visualEditor.Nodes.Add(new TreeNode("Advanced Workflows") { Tag = "advanced_workflows" });

        // Import/Export
        var importExport = new TreeNode("Import/Export");
        importExport.Nodes.Add(new TreeNode("Exporting Macros") { Tag = "exporting_macros" });
        importExport.Nodes.Add(new TreeNode("Importing Macros") { Tag = "importing_macros" });
        importExport.Nodes.Add(new TreeNode("Template Library") { Tag = "template_library" });

        // Troubleshooting
        var troubleshooting = new TreeNode("Troubleshooting");
        troubleshooting.Nodes.Add(new TreeNode("Common Issues") { Tag = "common_issues" });
        troubleshooting.Nodes.Add(new TreeNode("Device Problems") { Tag = "device_problems" });
        troubleshooting.Nodes.Add(new TreeNode("Performance Tips") { Tag = "performance_tips" });

        _topicsTreeView.Nodes.AddRange(new[] {
            gettingStarted, deviceManagement, macroConfig,
            layerSystem, visualEditor, importExport, troubleshooting
        });

        _topicsTreeView.ExpandAll();
        _topicsTreeView.SelectedNode = gettingStarted.Nodes[0];
    }

    private void OnTopicSelected(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is string topic)
        {
            LoadTopicContent(topic);
        }
    }

    private void LoadTopicContent(string topic)
    {
        var content = topic switch
        {
            "installation" => GetInstallationContent(),
            "first_setup" => GetFirstSetupContent(),
            "basic_concepts" => GetBasicConceptsContent(),
            "detecting_devices" => GetDetectingDevicesContent(),
            "device_isolation" => GetDeviceIsolationContent(),
            "device_settings" => GetDeviceSettingsContent(),
            "creating_macros" => GetCreatingMacrosContent(),
            "action_types" => GetActionTypesContent(),
            "recording_macros" => GetRecordingMacrosContent(),
            "understanding_layers" => GetUnderstandingLayersContent(),
            "layer_configuration" => GetLayerConfigurationContent(),
            "advanced_layers" => GetAdvancedLayersContent(),
            "editor_overview" => GetEditorOverviewContent(),
            "visual_macros" => GetVisualMacrosContent(),
            "advanced_workflows" => GetAdvancedWorkflowsContent(),
            "exporting_macros" => GetExportingMacrosContent(),
            "importing_macros" => GetImportingMacrosContent(),
            "template_library" => GetTemplateLibraryContent(),
            "common_issues" => GetCommonIssuesContent(),
            "device_problems" => GetDeviceProblemsContent(),
            "performance_tips" => GetPerformanceTipsContent(),
            _ => "Select a topic from the left to view its content."
        };

        _contentRichTextBox.Rtf = content;
    }

    private string GetInstallationContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Installation\b0\par
\par
KeyLayer requires the following:\par
\par
\b System Requirements:\b0\par
• Windows 10 or later\par
• .NET 8.0 Runtime\par
• Administrator privileges (for device isolation)\par
\par
\b First Launch:\b0\par
On first launch, KeyLayer will:\par
• Request administrator privileges\par
• Scan for HID devices\par
• Create default configuration files\par
}";
    }

    private string GetFirstSetupContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b First Setup\b0\par
\par
\b Step 1: Device Detection\b0\par
1. Click 'Scan Devices' to detect HID keyboards\par
2. Select a device from the list\par
3. Click 'Isolate Device' to take control\par
\par
\b Step 2: Create Your First Macro\b0\par
1. With a device isolated, click 'Add Macro'\par
2. Choose a key and enter a name\par
3. Add actions to your macro\par
4. Save the configuration\par
\par
\b Step 3: Test Your Macro\b0\par
1. Press the configured key on your isolated device\par
2. The macro should execute\par
3. Adjust settings as needed\par
}";
    }

    private string GetBasicConceptsContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Basic Concepts\b0\par
\par
\b HID Devices:\b0\par
Human Interface Devices that can be keyboards, mice, or other input devices.\par
\par
\b Device Isolation:\b0\par
Taking exclusive control of a device so it doesn't send normal input to Windows.\par
\par
\b Macros:\b0\par
Sequences of actions triggered by key presses on isolated devices.\par
\par
\b Layers:\b0\par
Different sets of key mappings that can be activated dynamically.\par
\par
\b Actions:\b0\par
Individual operations like key presses, mouse clicks, or system commands.\par
\par
\b Profiles:\b0\par
Saved configurations for specific devices including all macros and settings.\par
}";
    }

    private string GetDetectingDevicesContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Detecting Devices\b0\par
\par
KeyLayer automatically scans for HID devices every 5 seconds.\par
\par
\b Manual Scanning:\b0\par
• Click 'Scan Devices' to force a scan\par
• New devices appear in the device list\par
• Disconnected devices are automatically removed\par
\par
\b Device Information:\b0\par
• Device Name: Product name from manufacturer\par
• Manufacturer: Company that made the device\par
• Status: Normal or Isolated\par
• Profile: Associated configuration profile\par
\par
\b Supported Devices:\b0\par
• USB keyboards\par
• Wireless keyboards (with USB receiver)\par
• Gaming keyboards\par
• Macro keyboards\par
}";
    }

    private string GetDeviceIsolationContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Device Isolation\b0\par
\par
\b What is Device Isolation?\b0\par
Isolation prevents a device from sending normal keyboard input to Windows, allowing KeyLayer to intercept and process all input.\par
\par
\b How to Isolate:\b0\par
1. Select a device from the list\par
2. Click 'Isolate Device'\par
3. The device status changes to 'Isolated'\par
\par
\b Important Notes:\b0\par
• Requires administrator privileges\par
• Isolated devices won't work as normal keyboards\par
• Always keep one keyboard non-isolated for system access\par
• Devices are automatically released when KeyLayer exits\par
\par
\b Releasing Devices:\b0\par
• Click 'Release Device' to restore normal function\par
• Devices are auto-released on application exit\par
}";
    }

    private string GetDeviceSettingsContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Device Settings\b0\par
\par
Access device settings through Edit > Device Settings.\par
\par
\b Device Info Tab:\b0\par
• View device details (read-only)\par
• Vendor and Product IDs\par
• Device path information\par
\par
\b Isolation Tab:\b0\par
• Auto-isolate on connection\par
• Isolation timeout settings\par
• Idle release configuration\par
\par
\b Monitoring Tab:\b0\par
• Enable/disable monitoring\par
• Key press logging\par
• Activity indicators\par
• Buffer size settings\par
}";
    }

    private string GetCreatingMacrosContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Creating Macros\b0\par
\par
\b Step-by-Step Process:\b0\par
1. Select an isolated device\par
2. Click 'Add Macro'\par
3. Enter key name and select key code\par
4. Choose if it's a layer key\par
5. Add actions to the macro\par
6. Save the configuration\par
\par
\b Key Configuration:\b0\par
• Key Name: Descriptive name for the key\par
• Key Code: HID scan code (1-255)\par
• Layer Key: Special keys that activate layers\par
\par
\b Action Management:\b0\par
• Add Action: Create new macro actions\par
• Remove Action: Delete selected actions\par
• Test Macro: Execute the macro for testing\par
}";
    }

    private string GetActionTypesContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Action Types\b0\par
\par
KeyLayer supports 20+ action types:\par
\par
\b Input Actions:\b0\par
• KeyPress: Simulate key presses\par
• MouseClick: Left/right mouse clicks\par
• MouseMove: Move cursor to position\par
• Text: Type text strings\par
\par
\b System Actions:\b0\par
• Application: Launch programs\par
• WindowControl: Manage windows\par
• SystemCommand: System operations\par
• FileOperation: File management\par
\par
\b Advanced Actions:\b0\par
• Conditional: If/then logic\par
• Loop: Repeat actions\par
• Variable: Store/retrieve data\par
• WebRequest: HTTP requests\par
• Screenshot: Capture screen\par
• AudioControl: Volume/media control\par
}";
    }

    private string GetRecordingMacrosContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Recording Macros\b0\par
\par
\b How to Record:\b0\par
1. Click 'Record Macro'\par
2. Perform the actions you want to record\par
3. Click 'Stop Recording'\par
4. Review and edit the recorded actions\par
5. Save the macro\par
\par
\b What Gets Recorded:\b0\par
• Key presses and releases\par
• Mouse clicks and movements\par
• Timing between actions\par
• Text input\par
\par
\b Editing Recorded Macros:\b0\par
• Add or remove actions\par
• Adjust timing delays\par
• Modify action parameters\par
• Test before saving\par
}";
    }

    private string GetUnderstandingLayersContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Understanding Layers\b0\par
\par
\b What are Layers?\b0\par
Layers allow different sets of key mappings to be active at different times, similar to Fn keys on laptops.\par
\par
\b Layer Types:\b0\par
• Hold: Active while layer key is held\par
• Toggle: Switch on/off with key press\par
• Momentary: Active for specified duration\par
• Conditional: Based on conditions\par
\par
\b Layer Keys:\b0\par
Special keys that activate layers instead of executing macros.\par
\par
\b Use Cases:\b0\par
• Gaming profiles\par
• Application-specific shortcuts\par
• Temporary mode changes\par
• Context-sensitive actions\par
}";
    }

    private string GetLayerConfigurationContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Layer Configuration\b0\par
\par
Access the Layer Manager through Tools > Advanced Layer Manager.\par
\par
\b Creating Layers:\b0\par
1. Click 'Add Layer'\par
2. Set layer name and description\par
3. Choose activation type\par
4. Configure trigger keys\par
5. Set priority and options\par
\par
\b Layer Properties:\b0\par
• Activation Type: How the layer is triggered\par
• Priority: Layer precedence (1-100)\par
• Toggle Mode: On/off vs momentary\par
• Auto-deactivate: Timeout settings\par
\par
\b Layer Groups:\b0\par
• Organize related layers\par
• Exclusive vs non-exclusive modes\par
• Default layer settings\par
}";
    }

    private string GetAdvancedLayersContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Advanced Layer Features\b0\par
\par
\b Hierarchical Organization:\b0\par
• Layer groups for organization\par
• Drag-and-drop reordering\par
• Visual layer preview\par
\par
\b Conditional Layers:\b0\par
• Time-based activation\par
• Application-specific layers\par
• System state conditions\par
\par
\b Layer Inheritance:\b0\par
• Parent-child relationships\par
• Override mechanisms\par
• Fallback behaviors\par
\par
\b Visual Management:\b0\par
• Color coding\par
• Layer preview panel\par
• Key layout visualization\par
• Real-time status indicators\par
}";
    }

    private string GetEditorOverviewContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Visual Editor Overview\b0\par
\par
The Visual Macro Editor provides a drag-and-drop interface for creating complex macros.\par
\par
\b Interface Elements:\b0\par
• Toolbox: Available action types\par
• Canvas: Macro design area\par
• Property Grid: Action configuration\par
• Control Panel: Playback and file operations\par
\par
\b Key Features:\b0\par
• Node-based design\par
• Visual connections between actions\par
• Real-time preview\par
• Grid alignment\par
• Zoom and pan controls\par
\par
\b Workflow:\b0\par
1. Drag actions from toolbox to canvas\par
2. Connect actions to create flow\par
3. Configure each action's properties\par
4. Test the macro\par
5. Save or export the result\par
}";
    }

    private string GetVisualMacrosContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Creating Visual Macros\b0\par
\par
\b Adding Actions:\b0\par
• Drag from toolbox to canvas\par
• Double-click to configure\par
• Right-click for context menu\par
\par
\b Connecting Actions:\b0\par
• Actions execute in connected order\par
• Multiple connections create branches\par
• Conditional flows supported\par
\par
\b Action Configuration:\b0\par
• Select action to view properties\par
• Modify parameters in property grid\par
• Enable/disable individual actions\par
\par
\b Testing:\b0\par
• Play button executes the macro\par
• Visual feedback during execution\par
• Error highlighting for issues\par
\par
\b Saving:\b0\par
• Save as .macro files\par
• Export to multiple formats\par
• Import existing macros\par
}";
    }

    private string GetAdvancedWorkflowsContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Advanced Workflows\b0\par
\par
\b Conditional Logic:\b0\par
• If/then/else branches\par
• Multiple condition types\par
• Nested conditionals\par
\par
\b Loops and Repetition:\b0\par
• For loops with counters\par
• While loops with conditions\par
• Break and continue logic\par
\par
\b Variables and Data:\b0\par
• Store and retrieve values\par
• Dynamic text replacement\par
• Cross-action data sharing\par
\par
\b Error Handling:\b0\par
• Try/catch blocks\par
• Timeout handling\par
• Fallback actions\par
\par
\b Complex Scenarios:\b0\par
• Multi-application workflows\par
• Web automation\par
• File processing\par
• System administration tasks\par
}";
    }

    private string GetExportingMacrosContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Exporting Macros\b0\par
\par
\b Export Formats:\b0\par
• JSON: Native KeyLayer format\par
• XML: Structured data format\par
• Binary: Compressed format\par
• AutoHotkey: .ahk script files\par
• PowerShell: .ps1 script files\par
• Text Script: Human-readable format\par
\par
\b Export Options:\b0\par
• Include metadata\par
• Compress files\par
• Add descriptions and author info\par
• Version information\par
\par
\b Sharing Macros:\b0\par
• Export to files\par
• Copy to clipboard\par
• Upload to online library\par
• Email or share via cloud storage\par
}";
    }

    private string GetImportingMacrosContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Importing Macros\b0\par
\par
\b Import Sources:\b0\par
• Local files\par
• URLs (web links)\par
• Clipboard content\par
• Template library\par
\par
\b Supported Formats:\b0\par
• All export formats\par
• AutoHotkey scripts\par
• PowerShell scripts\par
• Text-based macro descriptions\par
\par
\b Import Process:\b0\par
1. Choose import source\par
2. Select file or enter URL\par
3. Preview imported actions\par
4. Choose merge or replace\par
5. Confirm import\par
\par
\b Merge Options:\b0\par
• Replace existing macros\par
• Merge with current macros\par
• Create new profile\par
}";
    }

    private string GetTemplateLibraryContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Template Library\b0\par
\par
\b Built-in Templates:\b0\par
• Text shortcuts (email signatures, dates)\par
• System commands (screenshots, volume)\par
• Application launchers\par
• Gaming macros\par
• Productivity shortcuts\par
\par
\b Categories:\b0\par
• Text and Typing\par
• System Control\par
• Gaming\par
• Productivity\par
• Development\par
• Custom\par
\par
\b Using Templates:\b0\par
1. Open Template Library\par
2. Browse categories\par
3. Preview template description\par
4. Select and import\par
5. Customize as needed\par
\par
\b Creating Templates:\b0\par
• Export existing macros as templates\par
• Add metadata and descriptions\par
• Share with the community\par
}";
    }

    private string GetCommonIssuesContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Common Issues\b0\par
\par
\b Device Not Detected:\b0\par
• Check USB connection\par
• Try different USB port\par
• Restart KeyLayer as Administrator\par
• Update device drivers\par
\par
\b Isolation Failed:\b0\par
• Run as Administrator\par
• Close other applications using the device\par
• Check device compatibility\par
• Restart the application\par
\par
\b Macros Not Working:\b0\par
• Verify device is isolated\par
• Check macro configuration\par
• Test with simple actions first\par
• Review action parameters\par
\par
\b Performance Issues:\b0\par
• Reduce scan interval\par
• Disable unnecessary logging\par
• Close unused visual editors\par
• Restart the application\par
}";
    }

    private string GetDeviceProblemsContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Device Problems\b0\par
\par
\b Device Disconnects:\b0\par
• Check USB cable quality\par
• Try different USB port\par
• Update USB drivers\par
• Check power management settings\par
\par
\b Input Lag:\b0\par
• Reduce buffer size\par
• Close other applications\par
• Check system resources\par
• Update device drivers\par
\par
\b Device Not Responding:\b0\par
• Release and re-isolate device\par
• Restart KeyLayer\par
• Check device in Device Manager\par
• Try different device\par
\par
\b Compatibility Issues:\b0\par
• Check supported device list\par
• Update to latest KeyLayer version\par
• Contact support with device details\par
}";
    }

    private string GetPerformanceTipsContent()
    {
        return @"{\rtf1\ansi\deff0 {\fonttbl {\f0 Segoe UI;}}
\f0\fs24\b Performance Tips\b0\par
\par
\b Optimize Settings:\b0\par
• Increase scan interval to 10+ seconds\par
• Disable device event logging\par
• Reduce buffer sizes\par
• Turn off debug mode\par
\par
\b Macro Optimization:\b0\par
• Use appropriate delays\par
• Avoid infinite loops\par
• Minimize complex conditionals\par
• Test macros thoroughly\par
\par
\b System Optimization:\b0\par
• Close unnecessary applications\par
• Ensure adequate RAM\par
• Use SSD for better I/O\par
• Keep Windows updated\par
\par
\b Monitoring:\b0\par
• Check Task Manager for CPU usage\par
• Monitor memory consumption\par
• Watch for error messages\par
• Use built-in performance counters\par
}";
    }
}