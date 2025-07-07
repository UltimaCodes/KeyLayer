namespace KeyLayer.Models;

public class MacroTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<MacroAction> Actions { get; set; } = new();
    public Dictionary<string, string> Parameters { get; set; } = new();
    public string Author { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public DateTime Created { get; set; }
    public List<string> Tags { get; set; } = new();
    public string IconPath { get; set; } = string.Empty;
}

public class MacroLibrary
{
    public List<MacroTemplate> Templates { get; set; } = new();
    public Dictionary<string, List<MacroTemplate>> Categories { get; set; } = new();
    public DateTime LastUpdated { get; set; }
    public string Version { get; set; } = "1.0";
}