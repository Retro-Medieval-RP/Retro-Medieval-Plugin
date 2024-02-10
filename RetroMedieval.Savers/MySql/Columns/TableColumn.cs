namespace RetroMedieval.Savers.MySql.Columns;

public class TableColumn
{
    public string Name { get; set; }
    public string DataType { get; set; }
    public string Default { get; set; }
    public string Constraint { get; set; }

    public string DDLColumn => $"{Name} {DataType}{(!string.IsNullOrEmpty(Default) ? $" DEFAULT {Default}" : "")}";
    public string ReferenceTableDDL { get; set; }

    public bool Equals(TableColumn obj) =>
        Name == obj.Name && DataType == obj.DataType && Default == obj.Default && Constraint == obj.Constraint &&
        ReferenceTableDDL == obj.ReferenceTableDDL;
}