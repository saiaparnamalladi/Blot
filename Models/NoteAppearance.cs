using System.Collections.Generic;
using System.Windows;
namespace Blot.Models;

public class NoteAppearance
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    /// <summary>Margins (from the window edge) of the area where text may be typed.</summary>
    public Thickness EditableRegion { get; set; } = new(16, 34, 16, 34);
}

public static class NoteColors
{
    public static readonly Dictionary<string, string> All = new()
    {
        ["Yellow"] = "#FFF7D1", ["Pink"] = "#FFD1E3", ["Blue"] = "#CFE8FF",
        ["Green"] = "#D4F5D0", ["Lavender"] = "#E3D9FF"
    };
}
