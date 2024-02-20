using System;
using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql;
using RetroMedieval.Savers.MySql.StatementsAndQueries;
using RetroMedieval.Savers.MySql.Tables.Attributes;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class InsertTests
{
    [Theory]
    [ClassData(typeof(ConvertingTestData))]
    public void ConvertTypeIntoParam(Type t, string t_name, object t_data,
        Savers.MySql.StatementsAndQueries.Queries.DataParam param)
    {
        var response = Savers.MySql.StatementsAndQueries.Queries.ConvertDataType(t_name, t_data, t);

        Assert.True(param.Equals(response),
            $"Response does not equal param.\nResponse:\nName: {response.ParamName}\nObj: {response.ParamObject}\nDbType: {(response.ParamDbType != null ? response.ParamDbType : "NULL")}\nType: {param.ParamType.Name}\n\nParam:\nName: {param.ParamName}\nObj: {param.ParamObject}\nDbType: {(param.ParamDbType != null ? param.ParamDbType : "NULL")}\nType: {param.ParamType.Name}");
    }

    [Theory]
    [ClassData(typeof(InsertQueryTestData))]
    public void InsertQueryTest(MySqlExecutor test_executor, string query_string) => 
        Assert.True(test_executor.Query.CurrentQueryString == query_string, $"Test Executor Query String does not equal correct query string.\n\nTest Executor: {test_executor.Query.CurrentQueryString}\nQuery Strnig: {query_string}");

    private class InsertQueryTestData : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                new MySqlQuery("Tests", "").Insert(new TestModel{TestString = "Testing String"}) as MySqlExecutor,
                ""
            },
            new object[]
            {
                new MySqlQuery("Tests", "").Insert(new TestModel {TestGuid = Guid.Parse("f633a6b2-85e0-4fcf-806a-b73f397aca4b")}) as MySqlExecutor,
                ""
            }
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        [DatabaseTable("Tests")]
        public class TestModel
        {
            [DatabaseColumn("TestID", "INT(11)", "AUTO_INCREMENT")]
            [PrimaryKey]
            public int TestID { get; set; }

            [DatabaseColumn("TestString", "VARCHAR(255)")]
            public string TestString { get; set; }

            [DatabaseColumn("TestGuid", "CHAR(16)")]
            public Guid TestGuid { get; set; }
        }
    }
    
    private class ConvertingTestData : IEnumerable<object[]>
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