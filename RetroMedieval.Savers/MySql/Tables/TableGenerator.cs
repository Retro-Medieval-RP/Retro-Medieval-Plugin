using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RetroMedieval.Savers.MySql.Attributes;
using RetroMedieval.Savers.MySql.Columns;

namespace RetroMedieval.Savers.MySql.Tables;

public class TableGenerator
{
    private static Dictionary<Type, string> TypeToTable { get; set; } = [];

    public static string GenerateDDL(Type type)
    {
        if (!GetTableAttribute(type, out var table))
        {
            return "";
        }

        if (TypeToTable.ContainsValue(table.TableName))
        {
            return "";
        }

        var properties = type.GetProperties();
        var columns = properties.Select(property => GetColumnData(property, table.TableName)).ToList();
        var columns_and_contrains = new List<string>();
        columns_and_contrains.AddRange(columns.Where(r => !r.IgnoreColumn).Select(r => r.DDLColumn));
        columns_and_contrains.AddRange(columns.Where(r => !r.IgnoreColumn).Where(r => !string.IsNullOrWhiteSpace(r.Constraint))
            .Select(r => r.Constraint));

        var ddl = $"CREATE TABLE IF NOT EXISTS {table.TableName} ({string.Join(",", columns_and_contrains)});{string.Join("", columns.Select(x => x.ReferenceTableDDL))}";

        TypeToTable.Add(type, table.TableName);
        return ddl;
    }

    public static TableColumn GetColumnDataTest(PropertyInfo property, string table_name)
    {
        return GetColumnData(property, table_name);
    }

    private static TableColumn GetColumnData(PropertyInfo property, string table_name)
    {
        var column = new TableColumn();

        if (IsIgnore(property))
        {
            column.IgnoreColumn = true;
            return column;
        }
        
        var column_data = GetColumnAttribute(property);
        column.Name = column_data.ColumnName;
        column.DataType = column_data.ColumnDataType;
        column.Default = column_data.ColumnDefault;

        if (IsColumnPrimaryKey(property))
        {
            column.Constraint = $"CONSTRAINT PK_{table_name} PRIMARY KEY ({column.Name})";
        }

        if (IsColumnForeignKey(property))
        {
            var foreign_key = GetColumnForeignKey(property);

            if (!TypeToTable.ContainsKey(foreign_key.ColumnReferenceType))
            {
                var new_table_ddl = GenerateDDL(foreign_key.ColumnReferenceType);
                column.ReferenceTableDDL = new_table_ddl;
            }
            
            column.Constraint = $"CONSTRAINT FK_{table_name}_{column.Name} FOREIGN KEY ({column.Name}) REFERENCES {TypeToTable[foreign_key.ColumnReferenceType]}({foreign_key.ColumnName})";
        }
        
        return column;
    }

    private static bool IsIgnore(MemberInfo property) =>
        property.GetCustomAttributes<DatabaseIgnore>().Any();

    private static ForeignKey GetColumnForeignKey(MemberInfo property) => 
        property.GetCustomAttribute<ForeignKey>();

    private static DatabaseColumn GetColumnAttribute(MemberInfo property) =>
        property.GetCustomAttributes<DatabaseColumn>().Any()
            ? property.GetCustomAttribute<DatabaseColumn>()
            : new DatabaseColumn(property.Name, "VARCHAR(255)");

    private static bool IsColumnPrimaryKey(MemberInfo property) =>
        property.GetCustomAttributes<PrimaryKey>().Any();

    private static bool IsColumnForeignKey(MemberInfo property)
    {
        var attributes = property.GetCustomAttributes<ForeignKey>();
        return attributes.Any();
    }

    private static bool GetTableAttribute(MemberInfo type, out DatabaseTable table)
    {
        if (type.GetCustomAttributes<DatabaseTable>().Any())
        {
            table = type.GetCustomAttribute<DatabaseTable>();
            return true;
        }

        table = new DatabaseTable(type.Name);
        return true;
    }

    private static ForeignKey GetColumnReferenceData(MemberInfo property) =>
        property.GetCustomAttributes<ForeignKey>().Any()
            ? property.GetCustomAttribute<ForeignKey>()
            : null;
}