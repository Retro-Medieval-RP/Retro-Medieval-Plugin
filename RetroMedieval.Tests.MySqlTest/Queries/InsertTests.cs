using System;
using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql;
using RetroMedieval.Savers.MySql.Tables.Attributes;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class InsertTests
{
    [Theory]
    [ClassData(typeof(InsertQueryTestData))]
    public void InsertQueryTest(MySqlExecutor testExecutor, string queryString) =>
        Assert.True(testExecutor.SqlString == queryString,
            $"Test Executor Query String does not equal correct query string.\n\nTest Executor: {testExecutor.SqlString}\nQuery String: {queryString}");

    private class InsertQueryTestData : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                new MySqlStatement("Tests", "").Insert(new TestModel
                    { TestString = "Testing String" }) as MySqlExecutor,
                "INSERT INTO Tests (TestString) VALUES (@TestString);"
            },
            new object[]
            {
                new MySqlStatement("Tests", "").Insert(new TestModel
                    { TestGuid = Guid.Parse("f633a6b2-85e0-4fcf-806a-b73f397aca4b") }) as MySqlExecutor,
                "INSERT INTO Tests (TestGuid) VALUES (@TestGuid);"
            }
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [DatabaseTable("Tests")]
        public class TestModel
        {
            [DatabaseColumn("TestID", "INT(11)", "AUTO_INCREMENT")]
            [PrimaryKey]
            public int? TestID { get; set; } = null;

            [DatabaseColumn("TestString", "VARCHAR(255)")]
            public string TestString { get; set; }

            [DatabaseColumn("TestGuid", "CHAR(16)")]
            public Guid? TestGuid { get; set; }
        }
    }

    private class ConvertingTestData : IEnumerable<object[]>
    {
        private static Guid _guidVal = Guid.Parse("f633a6b2-85e0-4fcf-806a-b73f397aca4b");

        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                typeof(Guid),
                "Test",
                _guidVal,
                new DataParam("@Test", _guidVal.ToString()) { ParamType = typeof(Guid) }
            },
            new object[]
            {
                typeof(string),
                "Test",
                "Test String",
                new DataParam("@Test", "Test String") { ParamType = typeof(string) }
            }
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}