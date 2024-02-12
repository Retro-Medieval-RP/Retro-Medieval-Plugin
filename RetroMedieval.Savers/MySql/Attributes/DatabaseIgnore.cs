using System;

namespace RetroMedieval.Savers.MySql.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DatabaseIgnore : Attribute
{
}