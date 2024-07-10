namespace Moderation.Models.Discord;

internal class Field(string name, string value, bool inline)
{
    public string Name { get; set; } = name;
    public string Value { get; set; } = value;
    public bool Inline { get; set; } = inline;
}