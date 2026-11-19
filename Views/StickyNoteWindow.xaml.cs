using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Threading;
using Blot.Models;
using Blot.Services;

namespace Blot.Views;

public partial class StickyNoteWindow : Window
{
    public Note Note { get; }
    readonly WindowManager _wm;
    readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(600) };
    bool _loading = true;

    public StickyNoteWindow(Note note, WindowManager wm)
    {
        InitializeComponent();
        Note = note; _wm = wm;
        Left = note.Left; Top = note.Top; Width = note.Width; Height = note.Height;
        Topmost = note.Pinned; PinBtn.Opacity = note.Pinned ? 1 : 0.45;
        ApplyColor(); ApplyLook(); LoadText();

        _timer.Tick += (_, _) => { _timer.Stop(); _wm.SaveAll(); };
        Editor.TextChanged += (_, _) => Dirty();
        Editor.AddHandler(System.Windows.Controls.Primitives.ToggleButton.CheckedEvent, new RoutedEventHandler((_, _) => Dirty()));
        Editor.AddHandler(System.Windows.Controls.Primitives.ToggleButton.UncheckedEvent, new RoutedEventHandler((_, _) => Dirty()));
        LocationChanged += (_, _) => Dirty();
        SizeChanged += (_, _) => Dirty();
        _loading = false;
    }

    void Dirty() { if (_loading) return; _timer.Stop(); _timer.Start(); } // debounced autosave

    public void Capture()
    {
        Note.Left = Left; Note.Top = Top; Note.Width = Width; Note.Height = Height;
        Note.Pinned = Topmost;
        Note.Xaml = XamlWriter.Save(Editor.Document);
    }

    void LoadText()
    {
        if (string.IsNullOrWhiteSpace(Note.Xaml)) return;
        try { if (XamlReader.Parse(Note.Xaml) is FlowDocument d) Editor.Document = d; } catch { }
    }

    void ApplyColor()
    {
        var hex = NoteColors.All.TryGetValue(Note.Color, out var h) ? h : NoteColors.All["Yellow"];
        ColorLayer.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
    }

    void ApplyLook()
    {
        var t = TemplateService.Get(Note.Template);
        Editor.Margin = t.EditableRegion;
        if (File.Exists(t.Path))
        {
            var bi = new BitmapImage();
            bi.BeginInit(); bi.CacheOption = BitmapCacheOption.OnLoad; bi.UriSource = new Uri(t.Path); bi.EndInit();
            Bg.Source = bi;
        }
        else Bg.Source = null;
    }

    void Header_Drag(object s, MouseButtonEventArgs e) { if (e.ButtonState == MouseButtonState.Pressed) DragMove(); }
    void New_Click(object s, RoutedEventArgs e) => _wm.NewNote();
    void Pin_Click(object s, RoutedEventArgs e) { Topmost = !Topmost; PinBtn.Opacity = Topmost ? 1 : 0.45; Dirty(); }

    void Delete_Click(object s, RoutedEventArgs e)
    {
        if (MessageBox.Show("Delete this note?", "Blot", MessageBoxButton.YesNo) == MessageBoxResult.Yes) _wm.Delete(this);
    }

    void Check_Click(object s, RoutedEventArgs e)
    {
        var cb = new CheckBox { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 6, 0) };
        _ = new InlineUIContainer(cb, Editor.CaretPosition.GetInsertionPosition(LogicalDirection.Forward));
        Editor.Focus(); Dirty();
    }

    void Palette_Click(object s, RoutedEventArgs e)
    {
        var m = new ContextMenu();
        foreach (var c in NoteColors.All.Keys)
        {
            var mi = new MenuItem { Header = c, IsChecked = c == Note.Color };
            mi.Click += (_, _) => { Note.Color = c; ApplyColor(); Dirty(); };
            m.Items.Add(mi);
        }
        m.Items.Add(new Separator());
        foreach (var t in TemplateService.All())
        {
            var mi = new MenuItem { Header = "Style: " + t.Name, IsChecked = t.Name == Note.Template };
            mi.Click += (_, _) => { Note.Template = t.Name; ApplyLook(); Dirty(); };
            m.Items.Add(mi);
        }
        m.PlacementTarget = (UIElement)s; m.IsOpen = true;
    }
}
