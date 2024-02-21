using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql;
using RetroMedieval.Savers.MySql.StatementsAndQueries;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class WhereTests
{
    [Theory]
    [ClassData(typeof(WhereQueryTestData))]
    public void WhereQueryTest(MySqlStatement test_executor, string query_string) =>
        Assert.True(test_executor.FilterConditionString == query_string,
            $"Test Executor Filter String does not equal correct filter string.\n\nTest Executor: {test_executor.FilterConditionString}\nFilter String: {query_string}");

    private class WhereQueryTestData : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                new MySqlQuery("Tests", "").Where(("TestString", "TestingValue")) as MySqlStatement,
                "WHERE TestString = @TestString;"
            },
            new object[]
            {
                new MySqlQuery("Tests", "").Where(("TestString", "TestingValue"), ("TestValue", 3)) as MySqlStatement,
                "WHERE TestString = @TestString AND TestValue = @TestValue;"
            }
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}