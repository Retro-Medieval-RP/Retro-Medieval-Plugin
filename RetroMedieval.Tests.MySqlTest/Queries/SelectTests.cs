using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql;
using RetroMedieval.Savers.MySql.StatementsAndQueries;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class SelectTests
{
    [Theory]
    [ClassData(typeof(SelectQueryTestData))]
    public void SelectQueryTests(MySqlExecutor test_executor, string query_string) =>
        Assert.True(
            test_executor.Query.CurrentQueryString + " " + test_executor.Query.FilterConditionString == query_string,
            $"Test Executor Filter String does not equal correct filter string.\n\nTest Executor: {test_executor.Query.CurrentQueryString} {test_executor.Query.FilterConditionString}\nFilter String: {query_string}");

    private class SelectQueryTestData : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                new MySqlQuery("Tests", "").Where(("TestString", "TestingValue")).Select("TestID") as MySqlExecutor,
                "SELECT TestID FROM Tests WHERE TestString = @TestString;"
            },
            new object[]
            {
                new MySqlQuery("Tests", "").Where(("TestString", "TestingValue"), ("TestValue", 3)).Select("TestID") as MySqlExecutor,
                "SELECT TestID FROM Tests WHERE TestString = @TestString AND TestValue = @TestValue;"
            },
            new object[]
            {
                new MySqlQuery("Tests", "").Where(("TestString", "TestingValue"), ("TestValue", 3)).Select("TestID", "JustSomeRandomColumn") as MySqlExecutor,
                "SELECT TestID, JustSomeRandomColumn FROM Tests WHERE TestString = @TestString AND TestValue = @TestValue;"
            }
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}