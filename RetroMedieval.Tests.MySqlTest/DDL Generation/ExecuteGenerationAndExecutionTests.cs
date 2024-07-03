using System.Collections;
using System.Collections.Generic;
using RetroMedieval.Savers.MySql.Tables;
using RetroMedieval.Savers.MySql.Tables.Attributes;
using Xunit;

namespace RetroMedieval.Tests.MySqlTest.DDL_Generation;

public class ExecuteGenerationAndExecutionTests
{
    [Theory]
    [ClassData(typeof(TestData))]
    public void Test1(string generatedDLL, string correctDdl, string modelName) => 
        Assert.True(generatedDLL == correctDdl, $"Model: {modelName} | Generated DDL: {generatedDLL} | Correct DDL: {correctDdl}");
}

public class TestData : IEnumerable<object[]>
{
    private static readonly IEnumerable<object[]> TestObjects = new[]
    {
        new object[]
        {
            TableGenerator.GenerateDdl(typeof(TestModel), out _),
            "CREATE TABLE IF NOT EXISTS Tests (TestID INT(11) DEFAULT AUTO_INCREMENT,TestString VARCHAR(255),CONSTRAINT PK_Tests PRIMARY KEY (TestID));",
            "TestModel"
        },
        new object[]
        {
            TableGenerator.GenerateDdl(typeof(Test2Model), out _),
            "CREATE TABLE IF NOT EXISTS Tests2 (TestID INT(11) DEFAULT AUTO_INCREMENT,TestString VARCHAR(255),CONSTRAINT PK_Tests2 PRIMARY KEY (TestID));",
            "Test2Model"
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
    
[DatabaseTable("Tests2")]
public class Test2Model
{
    [DatabaseColumn("TestID", "INT(11)", "AUTO_INCREMENT")]
    [PrimaryKey]
    public int TestID { get; set; }
        
    [DatabaseColumn("TestString", "VARCHAR(255)")]
    public string TestString { get; set; }
        
    [DatabaseIgnore]
    public string IgnoreString { get; set; }
}