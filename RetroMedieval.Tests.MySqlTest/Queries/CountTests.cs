using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql;
using RetroMedieval.Savers.MySql.StatementsAndQueries;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class CountTests
{
    [Theory]
    [ClassData(typeof(CountQueryTestData))]
    public void CountQueryTest(MySqlExecutor test_executor, string query_string) => 
        Assert.True(test_executor.Query.CurrentQueryString == query_string, $"Test Executor Query String does not equal correct query string.\n\nTest Executor: {test_executor.Query.CurrentQueryString}\nQuery String: {query_string}");

    private class CountQueryTestData : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                new MySqlQuery("Tests", "").Count(("TestString", "TestingValue")) as MySqlExecutor,
                "SELECT COUNT(TestString) FROM Tests WHERE TestString = @TestString;"
            },
            new object[]
            {
                new MySqlQuery("Tests", "").Count(("TestString", "TestingValue"), ("TestValue", 3)) as MySqlExecutor,
                "SELECT COUNT(TestString) FROM Tests WHERE TestString = @TestString AND TestValue = @TestValue;"
            }
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}