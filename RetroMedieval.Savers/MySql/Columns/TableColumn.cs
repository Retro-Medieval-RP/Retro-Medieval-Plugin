namespace RetroMedieval.Savers.MySql.Columns;

internal class TableColumn
{
    public string Name { get; set; }
    public string DataType { get; set; }
    public string Default { get; set; }
    public string Constraint { get; set; }

    public string DDLColumn => $"{Name} {DataType} {Default}";
    public string ReferenceTableDDL { get; set; }
}