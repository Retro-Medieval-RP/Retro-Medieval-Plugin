using System;

namespace RetroMedieval.Savers.MySql.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DatabaseTable(string name) : Attribute
{
    public string TableName { get; set; } = name;
}