using System.Collections.Generic;
using System.Linq;
using Blot.Models;
using Blot.Views;
namespace Blot.Services;

public class WindowManager
{
    readonly List<StickyNoteWindow> _windows = new();
    readonly AppSettings _settings = NoteStorageService.Load<AppSettings>("settings.json") ?? new();

    public AppSettings Settings => _settings;

    public void LoadAll()
    {
        var notes = NoteStorageService.Load<List<Note>>("notes.json") ?? new();
        if (notes.Count == 0) notes.Add(new Note { Template = _settings.DefaultTemplate });
        foreach (var n in notes) Open(n);
    }

    public void NewNote()
    {
        var off = _windows.Count * 24 % 240;
        Open(new Note { Template = _settings.DefaultTemplate, Left = 120 + off, Top = 120 + off });
        SaveAll();
    }

    void Open(Note n) { var w = new StickyNoteWindow(n, this); _windows.Add(w); w.Show(); }

    public void Delete(StickyNoteWindow w) { _windows.Remove(w); w.Close(); SaveAll(); }

    public void SaveAll()
    {
        foreach (var w in _windows) w.Capture();
        NoteStorageService.Save("notes.json", _windows.Select(w => w.Note).ToList());
    }
}
