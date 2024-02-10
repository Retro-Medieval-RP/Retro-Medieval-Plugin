using System;

namespace RetroMedieval.Savers.MySql.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class DatabaseTable : Attribute
{
    public string TableName { get; set; }
}