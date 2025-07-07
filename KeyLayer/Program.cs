using KeyLayer.Forms;
using KeyLayer.Services;

namespace KeyLayer;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        // Initialize services
        var deviceManager = new HIDDeviceManager();
        var macroService = new AdvancedMacroService();
        var keyboardHook = new KeyboardHookService();
        
        Application.Run(new MainForm(deviceManager, macroService, keyboardHook));
    }
}