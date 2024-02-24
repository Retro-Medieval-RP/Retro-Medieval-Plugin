using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class OrderByTests
{
    [Theory]
    [ClassData(typeof(OrderByTestData))]
    public void OrderByQueryTests(MySqlExecutor conditions, string query_string) =>
        Assert.True(conditions.FilterConditionString == query_string,
            $"Test Executor Filter String does not equal correct filter string.\n\nTest Executor: {conditions.FilterConditionString}\nFilter String: {query_string}");

    private class OrderByTestData : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                new MySqlCondition(new MySqlStatement("Tests", "")).OrderBy(OrderBy.Ascending,"TestString")
                    .Finalise() as MySqlExecutor,
                "ORDER BY TestString ASC"
            },
            new object[]
            {
                new MySqlCondition(new MySqlStatement("Tests", "")).OrderBy(OrderBy.Descending,"TestString")
                    .Finalise() as MySqlExecutor,
                "ORDER BY TestString DESC"
            },
            new object[]
            {
                new MySqlCondition(new MySqlStatement("Tests", "")).OrderBy(("TestString", OrderBy.Ascending), ("TestString2", OrderBy.Descending))
                    .Finalise() as MySqlExecutor,
                "ORDER BY TestString ASC, TestString2 DESC"
            },
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}