using System.Threading;
using System.Windows;
using Blot.Services;
using WinForms = System.Windows.Forms;

namespace Blot;

public partial class App : Application
{
    Mutex? _mutex;
    WinForms.NotifyIcon? _tray;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _mutex = new Mutex(true, "Blot.SingleInstance", out bool first);
        if (!first) { Shutdown(); return; }

        var wm = new WindowManager();
        wm.LoadAll();

        var menu = new WinForms.ContextMenuStrip();
        menu.Items.Add("New note", null, (_, _) => wm.NewNote());
        var startup = new WinForms.ToolStripMenuItem("Launch on startup")
            { CheckOnClick = true, Checked = StartupService.IsEnabled };
        startup.CheckedChanged += (_, _) => StartupService.Set(startup.Checked);
        menu.Items.Add(startup);
        menu.Items.Add("Exit", null, (_, _) => { wm.SaveAll(); _tray!.Visible = false; Shutdown(); });

        _tray = new WinForms.NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application, // swap for your own .ico
            Text = "Blot", Visible = true, ContextMenuStrip = menu
        };
        _tray.DoubleClick += (_, _) => wm.NewNote();
    }
}
