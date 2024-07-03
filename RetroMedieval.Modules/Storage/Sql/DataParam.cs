using System;
using System.Data;

namespace RetroMedieval.Modules.Storage.Sql;

public class DataParam(string name, object obj, DbType? type = null)
{
    public string ParamName { get; } = name;
    public object ParamObject { get; } = obj;
    public DbType? ParamDbType { get; } = type;
    public Type ParamType { get; set; } = null!;

    public override bool Equals(object? obj) =>
        obj is DataParam param && Equals(param);

    protected bool Equals(DataParam other) =>
        ParamName == other.ParamName && ParamObject.Equals(other.ParamObject) && ParamDbType == other.ParamDbType &&
        ParamType == other.ParamType;

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = ParamName.GetHashCode();
            hashCode = (hashCode * 397) ^ ParamObject.GetHashCode();
            hashCode = (hashCode * 397) ^ ParamDbType.GetHashCode();
            hashCode = (hashCode * 397) ^ ParamType.GetHashCode();
            return hashCode;
        }
    }
}