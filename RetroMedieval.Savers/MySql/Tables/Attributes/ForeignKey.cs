using System;

namespace RetroMedieval.Savers.MySql.Tables.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ForeignKey(Type type, string columnName) : Attribute
{
    public string ColumnName { get; set; } = columnName;
    public Type ColumnReferenceType { get; set; } = type;
}