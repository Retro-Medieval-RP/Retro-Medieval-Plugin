using System;
using System.Collections.Generic;

namespace Moderation.Models.Discord;

internal class Embed
{
    // ReSharper disable once InconsistentNaming
    public string title { get; set; }
    // ReSharper disable once InconsistentNaming
    public string url { get; set; }
    // ReSharper disable once InconsistentNaming
    public int color { get; set; }
    // ReSharper disable once InconsistentNaming
    public string timestamp { get; set; }
    // ReSharper disable once InconsistentNaming
    public string description { get; set; }
    // ReSharper disable once InconsistentNaming
    public List<Field> fields { get; set; }
    // ReSharper disable once InconsistentNaming
    public Footer footer { get; set; }
    
    public Embed(string description, int color)
    {
        this.description = description;
        this.color = color;
        timestamp = DateTime.Now.ToString("u");
    }

    public Embed(List<Field> fields, int color, Footer footer)
    {
        this.fields = fields;
        this.color = color;
        timestamp = DateTime.Now.ToString("u");
        this.footer = footer;
    }
}