using System;

namespace RetroMedieval.Savers.MySql.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DatabaseColumn(string name, string data_type, string @default = "") : Attribute
{
    internal string ColumnName { get; set; } = name;
    internal string ColumnDataType { get; set; } = data_type;
    internal string ColumnDefault { get; set; } = @default.StartsWith("DEFAULT") || @default.StartsWith("default") ? @default.Remove(0, "DEFAULT".Length) : @default;
}