using System;
namespace Blot.Models;

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Xaml { get; set; } = "";
    public double Left { get; set; } = 120;
    public double Top { get; set; } = 120;
    public double Width { get; set; } = 280;
    public double Height { get; set; } = 300;
    public string Color { get; set; } = "Yellow";
    public string Template { get; set; } = "Default";
    public bool Pinned { get; set; }
}
