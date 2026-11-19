using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using Blot.Models;
namespace Blot.Services;

public static class TemplateService
{
    static string Root => Path.Combine(AppContext.BaseDirectory, "Templates");

    public static List<NoteAppearance> All()
    {
        var list = new List<NoteAppearance>();
        if (!Directory.Exists(Root)) return list;
        var custom = Path.Combine(Root, "Custom");
        var files = Directory.GetFiles(Root, "*.jpeg")
            .Concat(Directory.Exists(custom) ? Directory.GetFiles(custom, "*.jpeg") : Array.Empty<string>());
        foreach (var f in files) list.Add(Load(f));
        return list.OrderBy(t => t.Name != "Default").ThenBy(t => t.Name).ToList();
    }

    // Optional sidecar "<name>.json" with {"left":..,"top":..,"right":..,"bottom":..}
    static NoteAppearance Load(string jpeg)
    {
        var m = new Thickness(16, 34, 16, 34);
        var js = System.IO.Path.ChangeExtension(jpeg, ".json");
        if (File.Exists(js))
        {
            try
            {
                using var d = JsonDocument.Parse(File.ReadAllText(js));
                var r = d.RootElement;
                double G(string k, double def) => r.TryGetProperty(k, out var v) ? v.GetDouble() : def;
                m = new Thickness(G("left", 16), G("top", 34), G("right", 16), G("bottom", 34));
            }
            catch { }
        }
        return new NoteAppearance { Name = System.IO.Path.GetFileNameWithoutExtension(jpeg), Path = jpeg, EditableRegion = m };
    }

    public static NoteAppearance Get(string name) =>
        All().FirstOrDefault(t => t.Name == name) ?? All().FirstOrDefault()
        ?? new NoteAppearance { Name = "Default", Path = "" };
}
