using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RetroMedieval.Savers.MySql.Tables.Attributes;
using RetroMedieval.Savers.MySql.Tables.Columns;

namespace RetroMedieval.Savers.MySql.Tables;

public class TableGenerator
{
    private static Dictionary<Type, string> TypeToTable { get; set; } = [];

    public static string GenerateDdl(Type type, out string tableName)
    {
        if (!GetTableAttribute(type, out var table))
        {
            tableName = "";
            return "";
        }

        tableName = table.TableName;

        if (TypeToTable.ContainsValue(table.TableName))
        {
            return "";
        }

        var properties = type.GetProperties();
        var columns = properties.Select(property => GetColumnData(property, table.TableName)).ToList();
        var columnsAndContrains = new List<string>();
        columnsAndContrains.AddRange(columns.Where(r => !r.IgnoreColumn).Select(r => r.DdlColumn));
        columnsAndContrains.AddRange(columns.Where(r => !r.IgnoreColumn)
            .Where(r => !string.IsNullOrWhiteSpace(r.Constraint))
            .Select(r => r.Constraint));

        var ddl = $"CREATE TABLE IF NOT EXISTS {table.TableName} ({string.Join(",", columnsAndContrains)});{string.Join("", columns.Select(x => x.ReferenceTableDdl))}";

        TypeToTable.Add(type, table.TableName);
        return ddl;
    }

    public static TableColumn GetColumnDataTest(PropertyInfo property, string tableName)
    {
        return GetColumnData(property, tableName);
    }

    private static TableColumn GetColumnData(PropertyInfo property, string tableName)
    {
        var column = new TableColumn();

        if (IsIgnore(property))
        {
            column.IgnoreColumn = true;
            return column;
        }

        var columnData = GetColumnAttribute(property);
        column.Name = columnData.ColumnName;
        column.DataType = columnData.ColumnDataType;
        column.Default = columnData.ColumnDefault;

        if (IsColumnPrimaryKey(property))
        {
            column.Constraint = $"CONSTRAINT PK_{tableName} PRIMARY KEY ({column.Name})";
        }

        if (IsColumnForeignKey(property))
        {
            var foreignKey = GetColumnForeignKey(property);

            if (!TypeToTable.ContainsKey(foreignKey.ColumnReferenceType))
            {
                var newTableDdl = GenerateDdl(foreignKey.ColumnReferenceType, out _);
                column.ReferenceTableDdl = newTableDdl;
            }

            column.Constraint =
                $"CONSTRAINT FK_{tableName}_{column.Name} FOREIGN KEY ({column.Name}) REFERENCES {TypeToTable[foreignKey.ColumnReferenceType]}({foreignKey.ColumnName})";
        }

        return column;
    }

    private static bool IsIgnore(MemberInfo property) =>
        property.GetCustomAttributes().Any(x => x.GetType() == typeof(DatabaseIgnore));

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

    private static ForeignKey? GetColumnReferenceData(MemberInfo property) =>
        property.GetCustomAttributes<ForeignKey>().Any()
            ? property.GetCustomAttribute<ForeignKey>()
            : null;
}