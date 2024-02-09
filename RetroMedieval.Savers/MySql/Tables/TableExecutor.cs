using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RetroMedieval.Savers.MySql.Attributes;
using RetroMedieval.Savers.MySql.Columns;

namespace RetroMedieval.Savers.MySql.Tables;

internal class TableExecutor
{
    private static Dictionary<Type, string> TypeToTable { get; set; } = [];

    internal string ExecuteGenerationAndExecution(Type type)
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

        var ddl =
            $"CREATE TABLE IF NOT EXISTS {table.TableName} ({string.Join(",\n", columns.Select(x => x.DDLColumn))},\n {string.Join(",\n", columns.Select(x => x.Constraint))}); {string.Join("\n", columns.Select(x => x.ReferenceTableDDL))}";

        return ddl;
    }

    private TableColumn GetColumnData(PropertyInfo property, string table_name)
    {
        var column = new TableColumn();

        var column_data = GetColumnAttribute(property);

        if (!property.PropertyType.IsSubclassOf(typeof(IEnumerable<>)))
        {
            return column;
        }

        var reference_data = GetColumnReferenceData(property);

        if (!TypeToTable.ContainsValue(reference_data.TableName))
        {
            var new_table_ddl = ExecuteGenerationAndExecution(property.PropertyType);
            column.ReferenceTableDDL = new_table_ddl;
        }

        column.Constraint =
            $"CONSTRAINT FK_{table_name}_{column.Name} REFERENCES {reference_data.TableName}({reference_data.ColumnName})";

        return column;
    }

    private DatabaseColumn GetColumnAttribute(MemberInfo property) =>
        property.GetCustomAttributes<DatabaseColumn>().Any()
            ? property.GetCustomAttribute<DatabaseColumn>()
            : new DatabaseColumn(property.Name, "VARCHAR(255)");

    private static bool GetTableAttribute(MemberInfo type, out DatabaseTable table)
    {
        if (type.GetCustomAttributes<DatabaseTable>().Any())
        {
            table = type.GetCustomAttribute<DatabaseTable>();
            return true;
        }

        table = new DatabaseTable { TableName = type.Name };
        return true;
    }

    private TableReference GetColumnReferenceData(MemberInfo property) =>
        property.GetCustomAttributes<TableReference>().Any()
            ? property.GetCustomAttribute<TableReference>() 
            : null;
}