using KeyLayer.Models;

namespace KeyLayer.Forms;

public partial class DeviceSettingsForm : Form
{
    private readonly HIDDeviceInfo _device;
    private TabControl _tabControl;
    private Button _okButton;
    private Button _cancelButton;

    // Device Info tab controls
    private TextBox _deviceNameTextBox;
    private TextBox _manufacturerTextBox;
    private TextBox _devicePathTextBox;
    private TextBox _vendorIdTextBox;
    private TextBox _productIdTextBox;
    private CheckBox _isKeyboardCheckBox;

    // Isolation tab controls
    private CheckBox _autoIsolateCheckBox;
    private NumericUpDown _isolationTimeoutNumeric;
    private CheckBox _releaseOnIdleCheckBox;
    private NumericUpDown _idleTimeoutNumeric;

    // Monitoring tab controls
    private CheckBox _enableMonitoringCheckBox;
    private CheckBox _logKeyPressesCheckBox;
    private CheckBox _showActivityIndicatorCheckBox;
    private NumericUpDown _bufferSizeNumeric;

    public DeviceSettingsForm(HIDDeviceInfo device)
    {
        _device = device;
        InitializeComponent();
        LoadDeviceInfo();
    }

    private void InitializeComponent()
    {
        Text = $"Device Settings - {_device.ProductName}";
        Size = new Size(500, 400);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        _tabControl = new TabControl
        {
            Location = new Point(10, 10),
            Size = new Size(470, 320)
        };

        CreateDeviceInfoTab();
        CreateIsolationTab();
        CreateMonitoringTab();

        _okButton = new Button
        {
            Text = "OK",
            Location = new Point(330, 340),
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };

        _cancelButton = new Button
        {
            Text = "Cancel",
            Location = new Point(420, 340),
            Size = new Size(60, 30),
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[] { _tabControl, _okButton, _cancelButton });
    }

    private void CreateDeviceInfoTab()
    {
        var infoTab = new TabPage("Device Info");

        var nameLabel = new Label { Text = "Device Name:", Location = new Point(10, 15), Size = new Size(100, 23) };
        _deviceNameTextBox = new TextBox
        {
            Location = new Point(120, 12),
            Size = new Size(300, 23),
            ReadOnly = true
        };

        var manufacturerLabel = new Label { Text = "Manufacturer:", Location = new Point(10, 45), Size = new Size(100, 23) };
        _manufacturerTextBox = new TextBox
        {
            Location = new Point(120, 42),
            Size = new Size(300, 23),
            ReadOnly = true
        };

        var pathLabel = new Label { Text = "Device Path:", Location = new Point(10, 75), Size = new Size(100, 23) };
        _devicePathTextBox = new TextBox
        {
            Location = new Point(120, 72),
            Size = new Size(300, 23),
            ReadOnly = true
        };

        var vendorLabel = new Label { Text = "Vendor ID:", Location = new Point(10, 105), Size = new Size(100, 23) };
        _vendorIdTextBox = new TextBox
        {
            Location = new Point(120, 102),
            Size = new Size(100, 23),
            ReadOnly = true
        };

        var productLabel = new Label { Text = "Product ID:", Location = new Point(230, 105), Size = new Size(80, 23) };
        _productIdTextBox = new TextBox
        {
            Location = new Point(320, 102),
            Size = new Size(100, 23),
            ReadOnly = true
        };

        _isKeyboardCheckBox = new CheckBox
        {
            Text = "Is Keyboard Device",
            Location = new Point(10, 135),
            Size = new Size(150, 23),
            Enabled = false
        };

        infoTab.Controls.AddRange(new Control[] {
            nameLabel, _deviceNameTextBox, manufacturerLabel, _manufacturerTextBox,
            pathLabel, _devicePathTextBox, vendorLabel, _vendorIdTextBox,
            productLabel, _productIdTextBox, _isKeyboardCheckBox
        });

        _tabControl.TabPages.Add(infoTab);
    }

    private void CreateIsolationTab()
    {
        var isolationTab = new TabPage("Isolation");

        _autoIsolateCheckBox = new CheckBox
        {
            Text = "Auto-isolate on connection",
            Location = new Point(10, 15),
            Size = new Size(180, 23)
        };

        var timeoutLabel = new Label { Text = "Isolation Timeout (sec):", Location = new Point(10, 45), Size = new Size(140, 23) };
        _isolationTimeoutNumeric = new NumericUpDown
        {
            Location = new Point(160, 42),
            Size = new Size(80, 23),
            Minimum = 0,
            Maximum = 3600,
            Value = 0
        };

        _releaseOnIdleCheckBox = new CheckBox
        {
            Text = "Release on idle",
            Location = new Point(10, 75),
            Size = new Size(120, 23)
        };

        var idleLabel = new Label { Text = "Idle Timeout (min):", Location = new Point(10, 105), Size = new Size(120, 23) };
        _idleTimeoutNumeric = new NumericUpDown
        {
            Location = new Point(140, 102),
            Size = new Size(80, 23),
            Minimum = 1,
            Maximum = 60,
            Value = 5
        };

        isolationTab.Controls.AddRange(new Control[] {
            _autoIsolateCheckBox, timeoutLabel, _isolationTimeoutNumeric,
            _releaseOnIdleCheckBox, idleLabel, _idleTimeoutNumeric
        });

        _tabControl.TabPages.Add(isolationTab);
    }

    private void CreateMonitoringTab()
    {
        var monitoringTab = new TabPage("Monitoring");

        _enableMonitoringCheckBox = new CheckBox
        {
            Text = "Enable device monitoring",
            Location = new Point(10, 15),
            Size = new Size(180, 23),
            Checked = true
        };

        _logKeyPressesCheckBox = new CheckBox
        {
            Text = "Log key presses",
            Location = new Point(10, 45),
            Size = new Size(150, 23)
        };

        _showActivityIndicatorCheckBox = new CheckBox
        {
            Text = "Show activity indicator",
            Location = new Point(10, 75),
            Size = new Size(180, 23),
            Checked = true
        };

        var bufferLabel = new Label { Text = "Buffer Size (KB):", Location = new Point(10, 105), Size = new Size(100, 23) };
        _bufferSizeNumeric = new NumericUpDown
        {
            Location = new Point(120, 102),
            Size = new Size(80, 23),
            Minimum = 1,
            Maximum = 1024,
            Value = 64
        };

        monitoringTab.Controls.AddRange(new Control[] {
            _enableMonitoringCheckBox, _logKeyPressesCheckBox,
            _showActivityIndicatorCheckBox, bufferLabel, _bufferSizeNumeric
        });

        _tabControl.TabPages.Add(monitoringTab);
    }

    private void LoadDeviceInfo()
    {
        _deviceNameTextBox.Text = _device.ProductName;
        _manufacturerTextBox.Text = _device.ManufacturerName;
        _devicePathTextBox.Text = _device.DevicePath;
        _vendorIdTextBox.Text = $"0x{_device.VendorId:X4}";
        _productIdTextBox.Text = $"0x{_device.ProductId:X4}";
        _isKeyboardCheckBox.Checked = _device.IsKeyboard;
    }
}