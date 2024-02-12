using System;

namespace RetroMedieval.Savers.MySql.Tables.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ForeignKey(Type type, string column_name) : Attribute
{
    public string ColumnName { get; set; } = column_name;
    public Type ColumnReferenceType { get; set; } = type;
}