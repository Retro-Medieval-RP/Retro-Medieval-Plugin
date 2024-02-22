using System;

namespace RetroMedieval.Savers.MySql.Tables.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DatabaseTable(string name) : Attribute
{
    public string TableName { get; set; } = name;
}