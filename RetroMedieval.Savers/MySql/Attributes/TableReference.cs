using System;

namespace RetroMedieval.Savers.MySql.Attributes;

public class TableReference<T> : TableReference where T : class
{
    public T TableType { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public abstract class TableReference : Attribute
{
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public Type TableType { get; set; }
}