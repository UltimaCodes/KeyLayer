# KeyLayer Tutorial

A full guide for learning how to use KeyLayer: a Windows-based macro management tool for HID input devices.

---

## Table of Contents

1. Understanding KeyLayer
2. Installation
3. Device Setup
4. Creating Macros
5. Action Types
6. Using Layers
7. Visual Macro Editor
8. Macro Recording
9. Import / Export
10. Advanced Features
11. Troubleshooting
12. Best Practices

---

## 1. Understanding KeyLayer

**What it is:**
KeyLayer turns USB keyboards into programmable macro devices. It isolates input from the OS and triggers custom scripts instead.

**Terminology:**

* **HID Devices:** USB input devices. This tool only works with keyboard-like HID devices.
* **Isolation:** Makes a keyboard invisible to Windows and only readable by KeyLayer.
* **Macros:** Sequences of actions triggered by pressing a key.
* **Layers:** Context-based key maps. One device, multiple roles.
* **Actions:** Individual operations like typing, clicking, launching apps, etc.

---

## 2. Installation

**Steps:**

1. Download from the GitHub releases page.
2. Unzip the file.
3. Launch KeyLayer.exe from the Main Folder.

**Note:** Always run KeyLayer as administrator. It won’t function properly without elevated permissions.

---

## 3. Device Setup

### Requirements

You need two keyboards:

* One for normal typing
* One to dedicate to macros (this one gets isolated)

### Steps

1. Connect both keyboards.
2. Open KeyLayer → click "Scan Devices".
3. Identify your macro keyboard → click "Isolate Device".
4. Pressing keys on the isolated keyboard should now do nothing unless a macro is assigned.
5. If you isolated the wrong one, click "Release Device".

---

## 4. Creating Macros

### Setup

1. Select and isolate your macro device.
2. Click "Add Macro".

### Configuration

* Name: “Hello Key”
* Key Code: 65 (A key)
* Layer: Default
* Leave “Layer Key” unchecked

### Add Action

* Type: Text
* Value: "Hello, World!"

Click Save. Press A on the macro keyboard → “Hello, World!” should be typed.

---

## 5. Action Types

### Input

* **KeyPress**: Emulates keyboard keys
* **MouseClick**: Emulates mouse buttons
* **Text**: Types full strings
* **Delay**: Pauses macro execution

### System

* **Application**: Launch programs
* **WindowControl**: Move/minimize windows

### Examples

**Email Signature Macro**

* Text: "Best regards,"
* KeyPress: Enter
* Text: "Your Name"

**App Launcher**

* Application: calc.exe

**Screenshot**

* KeyPress: PrintScreen
* Delay: 500ms
* Application: mspaint.exe

---

## 6. Using Layers

### Overview

Layers let the same key do different things in different contexts.

### Activation Types

* **Hold**: Active while holding
* **Toggle**: Press once to activate, again to deactivate
* **Momentary**: Auto-deactivates after a set time

### Setup

**Create Layer Key**

* Macro Key: L
* Mark as “Layer Key”

**Layered Macro**

* Key: 1
* Assign to Layer 1
* Add a spell combo or similar action

**Testing**

* Hold L → press 1 → macro triggers
* Release L → press 1 → nothing happens

---

## 7. Visual Macro Editor

### Layout

* Left: Action types
* Center: Canvas
* Right: Properties
* Bottom: Save, Load, Test

### Example

Build macro:

1. Application: notepad.exe
2. Delay: 2000ms
3. Text: "This is my automated note"

Connect them in order → click Play → Save if it works.

---

## 8. Recording Macros

### Steps

1. Click “Record Macro”
2. Perform your actions
3. Stop recording
4. Review and save

**Captured:**

* Key presses
* Mouse clicks
* Timing
* Typed text

**Editing:**

* Remove extra steps
* Add or modify delays
* Insert new actions

---

## 9. Import / Export

### Export

1. Select macro
2. File → Export
3. Choose format:

   * JSON
   * AutoHotkey (.ahk)
   * PowerShell (.ps1)
   * Text

### Import

* File → Import → from file, URL, clipboard, or built-in templates

**Template Categories:**

* Text
* System
* Gaming
* Productivity

---

## 10. Advanced Features

### Conditionals

Add decision-making:

* TimeRange, VariableCheck, etc.
* Example: Different macros for work hours vs. after-hours

### Loops

Repeat an action X times

### Variables

* Store values
* Use them dynamically in macros

### Web Requests

* Trigger HTTP requests
* Store and use responses in macros

---

## 11. Troubleshooting

| Problem               | Solution                                          |
| --------------------- | ------------------------------------------------- |
| Device not detected   | Reconnect device, run as admin                    |
| Isolation failed      | Close other input programs, try different USB     |
| Macros not triggering | Check isolation status, test with simpler macro   |
| Slow app performance  | Close editors, reduce scan interval, disable logs |

Check logs and help menu if unsure.

---

## 12. Best Practices

### Safety

* Keep one keyboard un-isolated
* Backup configs regularly
* Test before use

### Design

* Use clear macro names
* Add delay between steps
* Avoid infinite loops
* Document complex flows

### Structure

* Use layers to group related macros
* Use templates for standard tasks
* Use profiles for different devices

---

## Practical Use Cases

### Gaming

* Q → Spell Combo
* H → Use potion
* L1 (Hold) → Combat Layer
* L2 (Toggle) → Build Layer

### Productivity

* E → Insert email template
* M → Open Notepad and log meeting notes

### Development

* / → Insert comment block
* B → Build + Run (Ctrl+Shift+B → Delay → F5)

### Streaming

* 1–5 → OBS scene switch
* S → Auto-fill social media post

---


## Done

What you’ve learned:

* How to install and run KeyLayer
* How to isolate devices and build macros
* How to use layers, conditions, loops, and scripts
* How to organize and optimize workflows

### Next:

Start simple. Expand gradually. Backup often. Don’t isolate your only keyboard.
