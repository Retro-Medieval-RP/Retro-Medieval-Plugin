using System.Collections.Generic;

namespace RetroMedieval.Modules.Storage.Sql;

public interface IStatement : IQuery
{
    public List<DataParam> Parameters { get; set; }
}