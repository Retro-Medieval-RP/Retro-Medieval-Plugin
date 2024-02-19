using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class InsertTests
{
    [Theory]
    [ClassData(typeof(TestData))]
    public void ConvertTypeIntoParam(Type t, string t_name, object t_data, Savers.MySql.StatementsAndQueries.Queries.DataParam param)
    {
        var response = Savers.MySql.StatementsAndQueries.Queries.ConvertDataType(t_name, t_data, t);
        
        Assert.True(response == param, $"Response does not equal param.\nResponse:\nName: {response.ParamName}\nObj: {response.ParamObject}\nDbType: {(response.ParamType != null ? response.ParamType : "NULL")}");
    }

    private class TestData : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                typeof(Guid),
                "@Test",
                Guid.Parse(""),
                new Savers.MySql.StatementsAndQueries.Queries.DataParam("@Test", Guid.Parse("").ToString())
            },
            new object[]
            {
                typeof(string),
                "@Test",
                "",
                new Savers.MySql.StatementsAndQueries.Queries.DataParam("@Test", "")
            }
        };
        
        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}