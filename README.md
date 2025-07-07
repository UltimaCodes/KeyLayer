# KeyLayer

**Advanced Macro and Layer Manager for HID Devices on Windows**

Turn any HID-compliant keyboard or keypad into a programmable macro device. Capture keys, assign macros, build layers, and automate workflows. Built for developers, gamers, and anyone tired of repeating the same actions manually.

Supports Windows 10+ and .NET 8.0.

---

## Overview

KeyLayer provides low-level HID device control with real macro-building infrastructure. It isolates input from standard keyboard behavior, allowing a single keypad or keyboard to perform actions beyond typing—layer switching, macro triggering, workflow scripting, and more.

**Not a key remapper. Not a shortcut spoofer. This is an interface-level control tool.**

---

## Highlights

* **Device Isolation**: Select and isolate any keyboard-like HID device. System ignores it; KeyLayer listens.
* **Visual Macro Editing**: Grid-based interface for drag-and-drop macro building.
* **Layer System**: Build layered keymaps with activation modes and logical grouping.
* **Action Engine**: 20+ types, from input simulation to HTTP requests.
* **Multi-Format Macros**: Work with JSON, XML, AutoHotkey, PowerShell, and custom formats.
* **Template Support**: Reuse common macros. Share or build your own collection.

---

## Features

### Device Management

* Real-time HID device detection and interface scanning
* Manual and automatic device isolation
* Multi-device support
* Device-specific profiles
* Connection monitoring

### Macro System

**Action Types**

* **Input**: KeyPress, Mouse actions, Text typing
* **System**: Launch apps, manage windows, open files
* **Logic**: Conditionals, loops, variable control
* **Automation**: Clipboard, commands, screenshots
* **Media**: Volume, mute, brightness

**Execution Modes**

* Sequential
* Parallel
* Conditional branches

**Other**

* Macro recorder (with timing)
* Error handling and fallbacks

### Layer Architecture

* Layer groups with hierarchical structure
* Activation types: Hold, Toggle, Momentary, Conditional, Sequential
* Layer priority control
* Visual editor for layer layouts
* Exclusive and shared grouping options

### Visual Macro Editor

* Node-based macro builder
* Flow visualization (action paths)
* Live testing inside the editor
* Zoom, pan, undo/redo
* Property inspector
* Snap-to-grid

### Import / Export

* Formats: JSON, XML, Binary (compressed), AHK, PowerShell, human-readable text
* Import from file, clipboard, URL, or template
* Metadata support (version, author, tags)
* Optional compression and encryption

### Template Library

* Pre-built macros grouped by type:

  * Text
  * System
  * Gaming
  * Productivity
  * Development
* Add your own templates
* Support for sharing across systems or teams

### Configuration

* Startup behavior, UI themes, language
* Scan interval, isolation behavior
* Default macro delay and logic options
* Debug/logging settings
* JSON-based config with auto-save
* Profile backup/restore

---

## Quick Start

**Step 1: Device Setup**

1. Connect a keyboard or keypad
2. Launch KeyLayer as Administrator
3. Scan for devices
4. Select one and isolate it

**Step 2: Create a Macro**

```csharp
// Macro: Output "Hello, World!" and press Enter
Key: F1
Actions:
  - Text: "Hello, World!"
  - KeyPress: Enter
```

1. Add macro to selected device
2. Bind to F1
3. Add actions
4. Save
5. Test by pressing F1

**Step 3: Expand**

* Use the visual editor for complex workflows
* Build layer-based key maps
* Import a template or share one
* Save/load profile

---

## Examples

**Gaming Macro**

```csharp
Layer: Gaming
Key: Q
Actions:
  - KeyPress: 1
  - Delay: 100ms
  - KeyPress: 2
```

**Productivity Macro**

```json
{
  "name": "Email Template",
  "actions": [
    { "type": "Text", "value": "Dear {{recipient}}," },
    { "type": "KeyPress", "value": "Enter" },
    { "type": "KeyPress", "value": "Enter" },
    { "type": "Text", "value": "Best regards,\n{{signature}}" }
  ]
}
```

---

## Architecture

| Component        | Technology                    |
| ---------------- | ----------------------------- |
| Framework        | .NET 8.0                      |
| HID              | HidSharp                      |
| Windows APIs     | P/Invoke + WindowsAPICodePack |
| Serialization    | Newtonsoft.Json               |
| UI               | Custom WPF controls           |
| Input Simulation | user32.dll, kernel32.dll      |

```csharp
public class MacroAction {
    public MacroActionType Type;
    public string Value;
    public int Duration;
    public bool IsEnabled;
}

public class LayerConfiguration {
    public LayerActivationType ActivationType;
    public List<int> TriggerKeys;
    public Dictionary<int, KeyMapping> LayerKeyMappings;
}
```

---

## Use Cases

**Gaming**

* Combo sequences
* Scene switching, media control
* Avoid using this for cheating

**Productivity**

* Quick text snippets
* Open apps, automate file work
* Multi-step tasks like report generation

**Development**

* Code templates
* Build/test/run automation
* Debugging sequences

**Enterprise**

* Mass operations
* QA macros
* Form filling and admin tools

---

## Advanced Config

**Performance**

```json
{
  "scanInterval": 10000,
  "bufferSize": 32,
  "enableLogging": false,
  "maxConcurrentMacros": 5,
  "inputLatency": "minimal"
}
```

**Security**

```json
{
  "requireAdminForIsolation": true,
  "encryptProfiles": true,
  "auditLogging": true,
  "restrictedActions": ["SystemCommand", "FileOperation"]
}
```

---

## Troubleshooting

1. **Device not detected** — Check USB, run as admin
2. **Isolation fails** — Another program might be intercepting the device
3. **Macros don’t trigger** — Make sure the macro is assigned and saved
4. **Lag or delay** — Try adjusting buffer size or scan interval

---

## Contributing

Open to pull requests. Bug reports and feature suggestions go in the Issues tab.

---

## Roadmap

* Plugin support for user-defined actions
* Lua/Python scripting engine
* AI-based macro generation
* Voice command input
* Linux/macOS support

---

## Acknowledgments

KeyLayer is built using:

* **HidSharp** – USB/HID communication
* **Newtonsoft.Json** – JSON handling
* **WindowsAPICodePack** – Low-level Windows features
* **Everyone reporting bugs or testing builds**

