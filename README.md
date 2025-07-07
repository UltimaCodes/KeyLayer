# KeyLayer  
**v1.0**  
*"Turn your spare keyboard into a machine that does things so you don’t have to."*

This is KeyLayer, a low-level macro tool for Windows HID devices. It’s not a key remapper. It’s not a hotkey thing. It’s more like:  
**“What if this random keypad controlled your entire life?”**

Built for people who are either extremely efficient or extremely lazy (ideally both). I made this because I got tired of doing the same 6 actions 80 times a day and figured a dedicated macro keyboard deserved a bit more respect.

---

## Features (allegedly)

### Device Isolation  
Pick any HID keyboard and make it invisible to Windows. Now it only talks to KeyLayer. Like a walkie-talkie but for automating your bad habits.

### Visual Macro Editor  
A drag-and-drop grid thing. You connect boxes. The boxes do things. Like a flowchart, if flowcharts had keypresses and HTTP requests and existential dread.

### Layers  
Multiple keymaps per device. You can toggle them, hold them, conditionally activate them. Basically like having 12 keyboards in one, but without the shame of actually owning 12 keyboards.

### Action Types  
It simulates input, launches apps, takes screenshots, moves the mouse, opens files, controls media, sends HTTP requests, loops stuff, branches logic, and probably does your taxes if you script it hard enough.

### Template System  
Save your macros. Reuse them. Share them. Pretend you have a system. Never manually type “Best regards” again.

### Import/Export  
Supports JSON, XML, AutoHotkey, PowerShell, and some compressed binary thing that even I barely remember writing.

### Macro Recorder  
Press record. Smash some keys. Get a macro with suspiciously long delays and timings you’ll spend 20 minutes cleaning up.

### Profiles & Settings  
Stored in JSON because YAML made me irrationally angry. Config auto-saves. There’s a backup/restore thing. It usually works. Probably.

---

## Stuff That Technically Works

- Real-time HID device detection  
- Multi-device profiles  
- Manual and automatic isolation  
- Live macro testing  
- Node zoom/pan so you can pretend you're using After Effects  
- Undo/redo for when you immediately regret everything  
- Macro templates with optional metadata no one reads  
- Profile encryption for all your “sensitive” macros  

---

## Example Use Cases

**Gaming**: Press one button, do three things. Perfect for cooldown rotation or summoning emotional support macros.  
**Productivity**: Insert email templates and run scripts so fast you’ll look like you're working.  
**Development**: One button builds, tests, runs, and maybe launches your IDE. Feels like cheating. Isn’t.  
**QA/Enterprise**: Automate form-filling. Mass operations. Pretend you work in RPA.

---

## Configuration Highlights

- UI themes (light, dark, "meh")  (Might not work at all oops)
- Scan intervals and buffer sizes for performance tuning you’ll never do  
- Default macro delays so you can pretend timing matters  
- Debug logging that will either help or confuse you more  
- JSON-based config that survives most user errors

---

## Troubleshooting

**Device not detected** – Try plugging it in. Like, really plug it in. Harder.  
**Isolation fails** – Probably some other app messing with the device. Blame them.  
**Macros don’t trigger** – Did you bind it? Did you save it? Don’t lie.  
**Lag or delay** – Adjust a config setting until it feels slightly less terrible.

---

## Want to Actually Use It?

Check out the [Tutorial](./tutorial.md) if you want actual instructions instead of this.

---

## Roadmap (in theory)

- Plugin support for custom actions  
- Embedded scripting with Lua or Python or something equally worse   
- Linux/macOS support when I finally break

---

KeyLayer is stitched together with the help of:

- **HidSharp** – USB/HID communication without too much crying  
- **Newtonsoft.Json** – because System.Text.Json still feels off  
- **WindowsAPICodePack** – for poking Windows in ways it tolerates  

---

That’s it. That’s the README.

Go forth and automate something.  
