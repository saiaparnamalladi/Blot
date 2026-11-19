using System;
using Microsoft.Win32;
namespace Blot.Services;

public static class StartupService
{
    const string Key = @"Software\Microsoft\Windows\CurrentVersion\Run";
    public static bool IsEnabled
    {
        get { using var k = Registry.CurrentUser.OpenSubKey(Key); return k?.GetValue("Blot") != null; }
    }
    public static void Set(bool on)
    {
        using var k = Registry.CurrentUser.OpenSubKey(Key, true)!;
        if (on) k.SetValue("Blot", $"\"{Environment.ProcessPath}\""); else k.DeleteValue("Blot", false);
    }
}
