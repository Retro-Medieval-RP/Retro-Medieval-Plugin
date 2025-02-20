using System;
using RetroMedieval.Savers.MySql.Tables;
using RetroMedieval.Savers.MySql.Tables.Attributes;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.DDL_Generation;

public class ForeignTableTest
{
    [Theory]
    [InlineData(typeof(ForeignKeyTestModel),
        "CREATE TABLE IF NOT EXISTS ForeignKeyTestTable (TestID INT(11) DEFAULT AUTO_INCREMENT,TestString VARCHAR(255),Test2ID INT(11),CONSTRAINT PK_ForeignKeyTestTable PRIMARY KEY (TestID),CONSTRAINT FK_ForeignKeyTestTable_Test2ID FOREIGN KEY (Test2ID) REFERENCES ForeignKeyTestTable2(TestID));CREATE TABLE IF NOT EXISTS ForeignKeyTestTable2 (TestID INT(11) DEFAULT AUTO_INCREMENT,Test2String VARCHAR(255),CONSTRAINT PK_ForeignKeyTestTable2 PRIMARY KEY (TestID));")]
    public void Test1(Type type, string correctDdl)
    {
        var generatedDdl = TableGenerator.GenerateDdl(type, out _);
        Assert.True(generatedDdl == correctDdl, $"Generated DDL: {generatedDdl} | Correct DDL: {correctDdl}");
    }

    [DatabaseTable("ForeignKeyTestTable")]
    public class ForeignKeyTestModel
    {
        [DatabaseColumn("TestID", "INT(11)", "AUTO_INCREMENT")]
        [PrimaryKey]
        public int TestID { get; set; }

        [DatabaseColumn("TestString", "VARCHAR(255)")]
        public string TestString { get; set; }

        [DatabaseColumn("Test2ID", "INT(11)")]
        [ForeignKey(typeof(ForeignKeyTestModel2), "TestID")]
        public ForeignKeyTestModel2 Table2 { get; set; }
    }

    [DatabaseTable("ForeignKeyTestTable2")]
    public class ForeignKeyTestModel2
    {
        [DatabaseColumn("TestID", "INT(11)", "AUTO_INCREMENT")]
        [PrimaryKey]
        public int TestID { get; set; }

        [DatabaseColumn("Test2String", "VARCHAR(255)")]
        public string Test2String { get; set; }
    }
}