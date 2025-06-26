using System;

namespace KeyLayer.Models;

public class MacroAction
{
    public string Name { get; set; }
    public Action Action { get; set; }

    public void Execute()
    {
        Action?.Invoke();
    }
}