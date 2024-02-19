using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class InsertTests
{
    [Theory]
    [ClassData(typeof(TestData))]
    public void ConvertTypeIntoParam(Type t, string t_name, object t_data,
        Savers.MySql.StatementsAndQueries.Queries.DataParam param)
    {
        var response = Savers.MySql.StatementsAndQueries.Queries.ConvertDataType(t_name, t_data, t);

        Assert.True(param.Equals(response),
            $"Response does not equal param.\nResponse:\nName: {response.ParamName}\nObj: {response.ParamObject}\nDbType: {(response.ParamDbType != null ? response.ParamDbType : "NULL")}\nType: {param.ParamType.Name}\n\nParam:\nName: {param.ParamName}\nObj: {param.ParamObject}\nDbType: {(param.ParamDbType != null ? param.ParamDbType : "NULL")}\nType: {param.ParamType.Name}");
    }

    private class TestData : IEnumerable<object[]>
    {
        private static Guid GuidVal = Guid.Parse("f633a6b2-85e0-4fcf-806a-b73f397aca4b");

        private static readonly IEnumerable<object[]> _TestObjects = new[]
        {
            new object[]
            {
                typeof(Guid),
                "Test",
                GuidVal,
                new Savers.MySql.StatementsAndQueries.Queries.DataParam("@Test", GuidVal.ToString())
                    { ParamType = typeof(Guid) }
            },
            new object[]
            {
                typeof(string),
                "Test",
                "Test String",
                new Savers.MySql.StatementsAndQueries.Queries.DataParam("@Test", "Test String")
                    { ParamType = typeof(string) }
            }
        };

        public IEnumerator<object[]> GetEnumerator() => _TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}