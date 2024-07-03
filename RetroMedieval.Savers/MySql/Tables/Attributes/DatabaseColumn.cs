using System;

namespace RetroMedieval.Savers.MySql.Tables.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DatabaseColumn(string name, string dataType, string @default = "") : Attribute
{
    internal string ColumnName { get; set; } = name;
    internal string ColumnDataType { get; set; } = dataType;

    internal string ColumnDefault { get; set; } = @default.StartsWith("DEFAULT") || @default.StartsWith("default")
        ? @default.Remove(0, "DEFAULT".Length)
        : @default;
}