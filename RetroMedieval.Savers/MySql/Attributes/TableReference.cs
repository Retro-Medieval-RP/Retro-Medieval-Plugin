using System;

namespace RetroMedieval.Savers.MySql.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TableReference(string table_name, string column_name) : Attribute
{
    public string TableName { get; set; } = table_name;
    public string ColumnName { get; set; } = column_name;
}