using System;
using System.Collections.Generic;

namespace Moderation.Models.Discord;

internal class Embed
{
    public string Title { get; set; }
    public string URL { get; set; }
    public int Color { get; set; }
    public string Timestamp { get; set; }
    public string Description { get; set; }
    public List<Field> Fields { get; set; }
    public Footer Footer { get; set; }
    
    public Embed(string description, int color)
    {
        Description = description;
        Color = color;
        Timestamp = DateTime.Now.ToString("u");
    }

    public Embed(List<Field> fields, int color, Footer footer)
    {
        Fields = fields;
        Color = color;
        Timestamp = DateTime.Now.ToString("u");
        Footer = footer;
    }
}