using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql.Tables;
using RetroMedieval.Savers.MySql.Tables.Attributes;
using RetroMedieval.Savers.MySql.Tables.Columns;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.DDL_Generation
{
    public class GetColumnDataTest
    {
        [Theory]
        [ClassData(typeof(TestData<TestModel>))]
        public void Test1(TableColumn generated_column, TableColumn correct_column) =>
            Assert.True(generated_column.Equals(correct_column), 
                $@"Generated Column: 
    Name: {generated_column.Name}
    DataType: {generated_column.DataType}
    Default: {generated_column.Default}
    Constraint: {generated_column.Constraint}

Correct Column:
    Name: {correct_column.Name}
    DataType: {correct_column.DataType}
    Default: {correct_column.Default}
    Constraint: {correct_column.Constraint}
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
}