using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql.Attributes;
using RetroMedieval.Savers.MySql.Columns;
using RetroMedieval.Savers.MySql.Tables;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest
{
    public class ForeignTableTest
    {
        [Theory]
        [ClassData(typeof(TestData<TestModel>))]
        public void Test1(string generated_ddl, string correct_ddl) =>
            Assert.True(generated_ddl == correct_ddl, $"Generated DDL: {generated_ddl} | Correct DDL: {correct_ddl}");

        public class TestData<T> : IEnumerable<object[]>
        {
            private static readonly IEnumerable<object[]> TestObjects = new[]
            {
                new object[]
                {
                    TableGenerator.GenerateDDL(typeof(T)),
                    "CREATE TABLE IF NOT EXISTS Tests (TestID INT(11) DEFAULT AUTO_INCREMENT,TestString VARCHAR(255),Test2ID INT(11),CONSTRAINT PK_Tests PRIMARY KEY (TestID),CONSTRAINT FK_Tests_Test2ID FOREIGN KEY (Test2ID) REFERENCES TestTable2(TestID));CREATE TABLE IF NOT EXISTS TestTable2 (TestID INT(11) DEFAULT AUTO_INCREMENT,Test2String VARCHAR(255),CONSTRAINT PK_TestTable2 PRIMARY KEY (TestID));"
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

            [DatabaseColumn("Test2ID", "INT(11)")]
            [ForeignKey("TestTable2", "TestID")]
            public Test2Model Table2 { get; set; }
        }

        [DatabaseTable("TestTable2")]
        public class Test2Model
        {
            [DatabaseColumn("TestID", "INT(11)", "AUTO_INCREMENT")]
            [PrimaryKey]
            public int TestID { get; set; }

            [DatabaseColumn("Test2String", "VARCHAR(255)")]
            public string Test2String { get; set; }
        }
    }
}