using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class CountTests
{
    [Theory]
    [ClassData(typeof(CountQueryTestData))]
    public void CountQueryTest(MySqlExecutor test_executor, string query_string) =>
        Assert.True(test_executor.SqlString == query_string,
            $"Test Executor Query String does not equal correct query string.\n\nTest Executor: {test_executor.SqlString}\nQuery String: {query_string}");

    private class CountQueryTestData : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                new MySqlStatement("Tests", "").Count().Where(("TestString", "TestingValue")).Finalise() as
                    MySqlExecutor,
                "SELECT COUNT(*) AS counter FROM Tests WHERE TestString = @TestString;"
            },
            new object[]
            {
                new MySqlStatement("Tests", "").Count().Where(("TestString", "TestingValue"), ("TestValue", 3)).Finalise() as
                    MySqlExecutor,
                "SELECT COUNT(*) AS counter FROM Tests WHERE TestString = @TestString AND TestValue = @TestValue;"
            }
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}