namespace RetroMedieval.Savers.MySql.Tables.Columns;

public class TableColumn
{
    public string Name { get; set; } = "";
    public string DataType { get; set; } = "";
    public string Default { get; set; } = "";
    public string Constraint { get; set; } = "";

    public bool IgnoreColumn { get; set; } = false;

    public string DdlColumn => $"{Name} {DataType}{(!string.IsNullOrEmpty(Default) ? $" DEFAULT {Default}" : "")}";
    public string ReferenceTableDdl { get; set; } = "";

    public bool Equals(TableColumn obj) =>
        Name == obj.Name && DataType == obj.DataType && Default == obj.Default && Constraint == obj.Constraint &&
        ReferenceTableDdl == obj.ReferenceTableDdl;
}