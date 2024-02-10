using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql.Attributes;
using RetroMedieval.Savers.MySql.Tables;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest
{
    public class ExecuteGenerationAndExecutionTests
    {
        [Theory]
        [ClassData(typeof(TestData<TestModel>))]
        public void Test1(string generated_dll, string correct_ddl)
        {
            Assert.True(generated_dll == correct_ddl, $"Generated DDL: {generated_dll} | Correct DDL: {correct_ddl}");
        }
    }

    public class TestData<T> : IEnumerable<object[]>
    {
        private static readonly IEnumerable<object[]> TestObjects = new[]
        {
            new object[]
            {
                TableGenerator.GenerateDDL(typeof(T)),
                "CREATE TABLE IF NOT EXISTS Tests (TestID INT(11) DEFAULT AUTO_INCREMENT,TestString VARCHAR(255),CONSTRAINT PK_Tests PRIMARY KEY (TestID));"
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