using KeyLayer.Models;

namespace KeyLayer.Forms;

public partial class PreferencesForm : Form
{
    private TabControl _tabControl;
    private Button _okButton;
    private Button _cancelButton;
    private Button _applyButton;

    // General tab controls
    private CheckBox _startWithWindowsCheckBox;
    private CheckBox _minimizeToTrayCheckBox;
    private CheckBox _showNotificationsCheckBox;
    private ComboBox _languageComboBox;
    private ComboBox _themeComboBox;

    // Device tab controls
    private NumericUpDown _scanIntervalNumeric;
    private CheckBox _autoIsolateCheckBox;
    private CheckBox _releaseOnExitCheckBox;
    private CheckBox _logDeviceEventsCheckBox;

    // Macro tab controls
    private NumericUpDown _defaultDelayNumeric;
    private CheckBox _enableRecordingCheckBox;
    private CheckBox _playbackSoundsCheckBox;
    private ComboBox _executionModeComboBox;

    // Advanced tab controls
    private CheckBox _enableLoggingCheckBox;
    private TextBox _logFilePathTextBox;
    private Button _browseLogPathButton;
    private CheckBox _enableDebugModeCheckBox;
    private NumericUpDown _maxLogSizeNumeric;

    public PreferencesForm()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        Text = "Preferences";
        Size = new Size(500, 400);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        _tabControl = new TabControl
        {
            Location = new Point(10, 10),
            Size = new Size(470, 320)
        };

        CreateGeneralTab();
        CreateDeviceTab();
        CreateMacroTab();
        CreateAdvancedTab();

        _okButton = new Button
        {
            Text = "OK",
            Location = new Point(240, 340),
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };

        _cancelButton = new Button
        {
            Text = "Cancel",
            Location = new Point(330, 340),
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        _applyButton = new Button
        {
            Text = "Apply",
            Location = new Point(420, 340),
            Size = new Size(60, 30)
        };

        Controls.AddRange(new Control[] { _tabControl, _okButton, _cancelButton, _applyButton });

        _okButton.Click += OnOK;
        _applyButton.Click += OnApply;
    }

    private void CreateGeneralTab()
    {
        var generalTab = new TabPage("General");

        _startWithWindowsCheckBox = new CheckBox
        {
            Text = "Start with Windows",
            Location = new Point(10, 15),
            Size = new Size(150, 23)
        };

        _minimizeToTrayCheckBox = new CheckBox
        {
            Text = "Minimize to system tray",
            Location = new Point(10, 45),
            Size = new Size(180, 23)
        };

        _showNotificationsCheckBox = new CheckBox
        {
            Text = "Show notifications",
            Location = new Point(10, 75),
            Size = new Size(150, 23),
            Checked = true
        };

        var languageLabel = new Label { Text = "Language:", Location = new Point(10, 105), Size = new Size(80, 23) };
        _languageComboBox = new ComboBox
        {
            Location = new Point(100, 102),
            Size = new Size(120, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _languageComboBox.Items.AddRange(new[] { "English", "Spanish", "French", "German", "Chinese" });
        _languageComboBox.SelectedIndex = 0;

        var themeLabel = new Label { Text = "Theme:", Location = new Point(10, 135), Size = new Size(80, 23) };
        _themeComboBox = new ComboBox
        {
            Location = new Point(100, 132),
            Size = new Size(120, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _themeComboBox.Items.AddRange(new[] { "Light", "Dark", "Auto" });
        _themeComboBox.SelectedIndex = 0;

        generalTab.Controls.AddRange(new Control[] {
            _startWithWindowsCheckBox, _minimizeToTrayCheckBox, _showNotificationsCheckBox,
            languageLabel, _languageComboBox, themeLabel, _themeComboBox
        });

        _tabControl.TabPages.Add(generalTab);
    }

    private void CreateDeviceTab()
    {
        var deviceTab = new TabPage("Device");

        var scanLabel = new Label { Text = "Scan Interval (sec):", Location = new Point(10, 15), Size = new Size(120, 23) };
        _scanIntervalNumeric = new NumericUpDown
        {
            Location = new Point(140, 12),
            Size = new Size(80, 23),
            Minimum = 1,
            Maximum = 60,
            Value = 5
        };

        _autoIsolateCheckBox = new CheckBox
        {
            Text = "Auto-isolate new devices",
            Location = new Point(10, 45),
            Size = new Size(180, 23)
        };

        _releaseOnExitCheckBox = new CheckBox
        {
            Text = "Release devices on exit",
            Location = new Point(10, 75),
            Size = new Size(180, 23),
            Checked = true
        };

        _logDeviceEventsCheckBox = new CheckBox
        {
            Text = "Log device events",
            Location = new Point(10, 105),
            Size = new Size(150, 23)
        };

        deviceTab.Controls.AddRange(new Control[] {
            scanLabel, _scanIntervalNumeric, _autoIsolateCheckBox,
            _releaseOnExitCheckBox, _logDeviceEventsCheckBox
        });

        _tabControl.TabPages.Add(deviceTab);
    }

    private void CreateMacroTab()
    {
        var macroTab = new TabPage("Macro");

        var delayLabel = new Label { Text = "Default Delay (ms):", Location = new Point(10, 15), Size = new Size(120, 23) };
        _defaultDelayNumeric = new NumericUpDown
        {
            Location = new Point(140, 12),
            Size = new Size(80, 23),
            Minimum = 0,
            Maximum = 5000,
            Value = 100
        };

        _enableRecordingCheckBox = new CheckBox
        {
            Text = "Enable macro recording",
            Location = new Point(10, 45),
            Size = new Size(180, 23),
            Checked = true
        };

        _playbackSoundsCheckBox = new CheckBox
        {
            Text = "Play sounds during playback",
            Location = new Point(10, 75),
            Size = new Size(200, 23)
        };

        var executionLabel = new Label { Text = "Execution Mode:", Location = new Point(10, 105), Size = new Size(100, 23) };
        _executionModeComboBox = new ComboBox
        {
            Location = new Point(120, 102),
            Size = new Size(120, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _executionModeComboBox.Items.AddRange(new[] { "Sequential", "Parallel", "Conditional" });
        _executionModeComboBox.SelectedIndex = 0;

        macroTab.Controls.AddRange(new Control[] {
            delayLabel, _defaultDelayNumeric, _enableRecordingCheckBox,
            _playbackSoundsCheckBox, executionLabel, _executionModeComboBox
        });

        _tabControl.TabPages.Add(macroTab);
    }

    private void CreateAdvancedTab()
    {
        var advancedTab = new TabPage("Advanced");

        _enableLoggingCheckBox = new CheckBox
        {
            Text = "Enable logging",
            Location = new Point(10, 15),
            Size = new Size(120, 23)
        };

        var logPathLabel = new Label { Text = "Log File Path:", Location = new Point(10, 45), Size = new Size(80, 23) };
        _logFilePathTextBox = new TextBox
        {
            Location = new Point(100, 42),
            Size = new Size(250, 23),
            Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KeyLayer", "logs", "keylayer.log")
        };

        _browseLogPathButton = new Button
        {
            Text = "Browse",
            Location = new Point(360, 42),
            Size = new Size(80, 23)
        };

        var maxLogLabel = new Label { Text = "Max Log Size (MB):", Location = new Point(10, 75), Size = new Size(120, 23) };
        _maxLogSizeNumeric = new NumericUpDown
        {
            Location = new Point(140, 72),
            Size = new Size(80, 23),
            Minimum = 1,
            Maximum = 100,
            Value = 10
        };

        _enableDebugModeCheckBox = new CheckBox
        {
            Text = "Enable debug mode",
            Location = new Point(10, 105),
            Size = new Size(150, 23)
        };

        advancedTab.Controls.AddRange(new Control[] {
            _enableLoggingCheckBox, logPathLabel, _logFilePathTextBox, _browseLogPathButton,
            maxLogLabel, _maxLogSizeNumeric, _enableDebugModeCheckBox
        });

        _tabControl.TabPages.Add(advancedTab);

        _browseLogPathButton.Click += OnBrowseLogPath;
    }

    private void OnBrowseLogPath(object? sender, EventArgs e)
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "Log Files (*.log)|*.log|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            Title = "Select Log File Location",
            FileName = "keylayer.log"
        };

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            _logFilePathTextBox.Text = saveDialog.FileName;
        }
    }

    private void LoadSettings()
    {
        try
        {
            var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KeyLayer", "settings.json");
            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(json);
                
                if (settings != null)
                {
                    ApplySettingsToControls(settings);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading settings: {ex.Message}", "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ApplySettingsToControls(AppSettings settings)
    {
        _startWithWindowsCheckBox.Checked = settings.StartWithWindows;
        _minimizeToTrayCheckBox.Checked = settings.MinimizeToTray;
        _showNotificationsCheckBox.Checked = settings.ShowNotifications;
        _languageComboBox.SelectedItem = settings.Language;
        _themeComboBox.SelectedItem = settings.Theme;
        
        _scanIntervalNumeric.Value = settings.ScanInterval;
        _autoIsolateCheckBox.Checked = settings.AutoIsolateDevices;
        _releaseOnExitCheckBox.Checked = settings.ReleaseDevicesOnExit;
        _logDeviceEventsCheckBox.Checked = settings.LogDeviceEvents;
        
        _defaultDelayNumeric.Value = settings.DefaultDelay;
        _enableRecordingCheckBox.Checked = settings.EnableRecording;
        _playbackSoundsCheckBox.Checked = settings.PlaybackSounds;
        _executionModeComboBox.SelectedItem = settings.ExecutionMode;
        
        _enableLoggingCheckBox.Checked = settings.EnableLogging;
        _logFilePathTextBox.Text = settings.LogFilePath;
        _maxLogSizeNumeric.Value = settings.MaxLogSizeMB;
        _enableDebugModeCheckBox.Checked = settings.EnableDebugMode;
    }

    private AppSettings GetSettingsFromControls()
    {
        return new AppSettings
        {
            StartWithWindows = _startWithWindowsCheckBox.Checked,
            MinimizeToTray = _minimizeToTrayCheckBox.Checked,
            ShowNotifications = _showNotificationsCheckBox.Checked,
            Language = _languageComboBox.SelectedItem?.ToString() ?? "English",
            Theme = _themeComboBox.SelectedItem?.ToString() ?? "Light",
            
            ScanInterval = (int)_scanIntervalNumeric.Value,
            AutoIsolateDevices = _autoIsolateCheckBox.Checked,
            ReleaseDevicesOnExit = _releaseOnExitCheckBox.Checked,
            LogDeviceEvents = _logDeviceEventsCheckBox.Checked,
            
            DefaultDelay = (int)_defaultDelayNumeric.Value,
            EnableRecording = _enableRecordingCheckBox.Checked,
            PlaybackSounds = _playbackSoundsCheckBox.Checked,
            ExecutionMode = _executionModeComboBox.SelectedItem?.ToString() ?? "Sequential",
            
            EnableLogging = _enableLoggingCheckBox.Checked,
            LogFilePath = _logFilePathTextBox.Text,
            MaxLogSizeMB = (int)_maxLogSizeNumeric.Value,
            EnableDebugMode = _enableDebugModeCheckBox.Checked
        };
    }

    private void SaveSettings()
    {
        try
        {
            var settings = GetSettingsFromControls();
            var settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KeyLayer");
            Directory.CreateDirectory(settingsDir);
            
            var settingsPath = Path.Combine(settingsDir, "settings.json");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(settingsPath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnOK(object? sender, EventArgs e)
    {
        SaveSettings();
    }

    private void OnApply(object? sender, EventArgs e)
    {
        SaveSettings();
    }
}

public class AppSettings
{
    public bool StartWithWindows { get; set; }
    public bool MinimizeToTray { get; set; } = true;
    public bool ShowNotifications { get; set; } = true;
    public string Language { get; set; } = "English";
    public string Theme { get; set; } = "Light";
    
    public int ScanInterval { get; set; } = 5;
    public bool AutoIsolateDevices { get; set; }
    public bool ReleaseDevicesOnExit { get; set; } = true;
    public bool LogDeviceEvents { get; set; }
    
    public int DefaultDelay { get; set; } = 100;
    public bool EnableRecording { get; set; } = true;
    public bool PlaybackSounds { get; set; }
    public string ExecutionMode { get; set; } = "Sequential";
    
    public bool EnableLogging { get; set; }
    public string LogFilePath { get; set; } = "";
    public int MaxLogSizeMB { get; set; } = 10;
    public bool EnableDebugMode { get; set; }
}