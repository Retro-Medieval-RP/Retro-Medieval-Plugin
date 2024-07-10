namespace Moderation.Models.Discord;

internal class Field(string name, string value, bool inline)
{
    // ReSharper disable once InconsistentNaming
    public string name { get; set; } = name;
    // ReSharper disable once InconsistentNaming
    public string value { get; set; } = value;
    // ReSharper disable once InconsistentNaming
    public bool inline { get; set; } = inline;
}