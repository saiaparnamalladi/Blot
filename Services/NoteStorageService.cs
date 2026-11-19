using System;
using System.IO;
using System.Text.Json;
namespace Blot.Services;

public static class NoteStorageService
{
    static readonly string Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Blot");

    public static T? Load<T>(string file)
    {
        try { var p = Path.Combine(Dir, file); return File.Exists(p) ? JsonSerializer.Deserialize<T>(File.ReadAllText(p)) : default; }
        catch { return default; }
    }

    public static void Save<T>(string file, T data)
    {
        Directory.CreateDirectory(Dir);
        var p = Path.Combine(Dir, file); var tmp = p + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(tmp, p, true); // atomic-ish: never leaves a half-written file
    }
}
