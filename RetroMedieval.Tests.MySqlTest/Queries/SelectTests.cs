using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.Queries;

public class SelectTests
{
    [Theory]
    [ClassData(typeof(SelectQueryTestData))]
    public void SelectQueryTests(MySqlExecutor testExecutor, string queryString) =>
        Assert.True(testExecutor.SqlString == queryString,
            $"Test Executor Filter String does not equal correct filter string.\n\nTest Executor: {testExecutor.SqlString}\nFilter String: {queryString}");

    private class SelectQueryTestData : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                new MySqlStatement("Tests", "").Select("TestID").Where(("TestString", "TestingValue"))
                    .Finalise() as MySqlExecutor,
                "SELECT TestID FROM Tests WHERE TestString = @TestString;"
            },
            new object[]
            {
                new MySqlStatement("Tests", "").Select("TestID").Where(("TestString", "TestingValue"), ("TestValue", 3))
                    .Finalise() as MySqlExecutor,
                "SELECT TestID FROM Tests WHERE TestString = @TestString AND TestValue = @TestValue;"
            },
            new object[]
            {
                new MySqlStatement("Tests", "").Select("TestID", "JustSomeRandomColumn")
                    .Where(("TestString", "TestingValue"), ("TestValue", 3)).Finalise() as MySqlExecutor,
                "SELECT TestID, JustSomeRandomColumn FROM Tests WHERE TestString = @TestString AND TestValue = @TestValue;"
            }
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}