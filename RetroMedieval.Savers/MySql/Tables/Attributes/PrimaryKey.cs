using System;

namespace RetroMedieval.Savers.MySql.Tables.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class PrimaryKey : Attribute
{
}