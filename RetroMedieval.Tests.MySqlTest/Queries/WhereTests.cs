using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class WhereTests
{
    [Theory]
    [ClassData(typeof(WhereQueryTestData))]
    public void WhereQueryTest(MySqlExecutor conditions, string queryString) =>
        Assert.True(conditions.FilterConditionString == queryString,
            $"Test Executor Filter String does not equal correct filter string.\n\nTest Executor: {conditions.FilterConditionString}\nFilter String: {queryString}");

    private class WhereQueryTestData : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                new MySqlCondition(new MySqlStatement("Tests", "")).Where(("TestString", "TestingValue"))
                    .Finalise() as MySqlExecutor,
                "WHERE TestString = @TestString"
            },
            new object[]
            {
                new MySqlCondition(new MySqlStatement("Tests", ""))
                    .Where(("TestString", "TestingValue"), ("TestValue", 3)).Finalise() as MySqlExecutor,
                "WHERE TestString = @TestString AND TestValue = @TestValue"
            }
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}