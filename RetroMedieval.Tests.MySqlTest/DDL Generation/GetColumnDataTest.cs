using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql.Tables;
using RetroMedieval.Savers.MySql.Tables.Attributes;
using RetroMedieval.Savers.MySql.Tables.Columns;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.DDL_Generation;

public class GetColumnDataTest
{
    [Theory]
    [ClassData(typeof(TestData<TestModel>))]
    public void Test1(TableColumn generatedColumn, TableColumn correctColumn) =>
        Assert.True(generatedColumn.Equals(correctColumn),
            $@"Generated Column: 
    Name: {generatedColumn.Name}
    DataType: {generatedColumn.DataType}
    Default: {generatedColumn.Default}
    Constraint: {generatedColumn.Constraint}

Correct Column:
    Name: {correctColumn.Name}
    DataType: {correctColumn.DataType}
    Default: {correctColumn.Default}
    Constraint: {correctColumn.Constraint}
");

    public class TestData<T> : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                TableGenerator.GetColumnDataTest(typeof(T).GetProperties()[0], "Tests"),
                new TableColumn
                {
                    Name = "TestID",
                    DataType = "INT(11)",
                    Default = "AUTO_INCREMENT",
                    Constraint = "CONSTRAINT PK_Tests PRIMARY KEY (TestID)"
                }
            },
        };

        public IEnumerator<object[]> GetEnumerator() => TestObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [DatabaseTable("Tests")]
    public class TestModel
    {
        [DatabaseColumn("TestID", "INT(11)", "AUTO_INCREMENT")]
        [PrimaryKey]
        public int TestID { get; set; }

        [DatabaseColumn("TestString", "VARCHAR(255)")]
        public string TestString { get; set; }
    }
}