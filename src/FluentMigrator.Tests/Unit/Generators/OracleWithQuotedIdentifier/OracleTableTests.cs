﻿using FluentMigrator.Exceptions;
using FluentMigrator.Runner.Generators.Oracle;
using NUnit.Framework;
using NUnit.Should;

namespace FluentMigrator.Tests.Unit.Generators.OracleWithQuotedIdentifier
{
    [TestFixture]
    public class OracleTableTests
    {
        protected OracleGenerator _generator;
        protected OracleGenerator generator;

        [SetUp]
        public void Setup()
        {
            _generator = new OracleGenerator(useQuotedIdentifiers: true);
            generator = new OracleGenerator(useQuotedIdentifiers: true);
        }

        [Test]
        public void CanCreateTableWithCustomColumnType()
        {
            //Do not knows any oracle custom types. Not testing for it.  If someone else knows please add a test
        }

        [Test]
        public void CanCreateTable()
        {
            var expression = GeneratorTestHelper.GetCreateTableExpression();
            string sql = _generator.Generate(expression);
            sql.ShouldBe("CREATE TABLE \"TestTable1\" (\"TestColumn1\" NVARCHAR2(255) NOT NULL, \"TestColumn2\" NUMBER(10,0) NOT NULL)");
        }

        [Test]
        public void CanCreateTableWithDefaultValueExplicitlySetToNull()
        {
            //Not sure how this would work in oracle.  Someone please add a test
        }

        [Test]
        public void CanCreateTableWithDefaultValue()
        {
            var expression = GeneratorTestHelper.GetCreateTableExpression();
            expression.Columns[0].DefaultValue = "abc";
            string sql = _generator.Generate(expression);

            // Oracle requires the DEFAULT clause to appear before the NOT NULL clause
            sql.ShouldBe("CREATE TABLE \"TestTable1\" (\"TestColumn1\" NVARCHAR2(255) DEFAULT 'abc' NOT NULL, \"TestColumn2\" NUMBER(10,0) NOT NULL)");
        }

        [Test]
        public void CanCreateTableWithIdentity()
        {
            Assert.Throws<DatabaseOperationNotSupportedException>(() => _generator.Generate(GeneratorTestHelper.GetCreateTableWithAutoIncrementExpression()));
        }

        [Test]
        public void CanCreateTableWithMultiColumnPrimaryKey()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithMultiColumnPrimaryKeyExpression();
            string sql = _generator.Generate(expression);
            // See the note in OracleColumn about why the PK should not be named
            sql.ShouldBe("CREATE TABLE \"TestTable1\" (\"TestColumn1\" NVARCHAR2(255) NOT NULL, \"TestColumn2\" NUMBER(10,0) NOT NULL, PRIMARY KEY (\"TestColumn1\", \"TestColumn2\"))");
        }

        [Test]
        public void CanCreateTableNamedMultiColumnPrimaryKey()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithMultiColumNamedPrimaryKeyExpression();
            string sql = _generator.Generate(expression);
            sql.ShouldBe("CREATE TABLE \"TestTable1\" (\"TestColumn1\" NVARCHAR2(255) NOT NULL, \"TestColumn2\" NUMBER(10,0) NOT NULL, CONSTRAINT \"TestKey\" PRIMARY KEY (\"TestColumn1\", \"TestColumn2\"))");
        }

        [Test]
        public void CanCreateTableNamedPrimaryKey()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithNamedPrimaryKeyExpression();
            string sql = _generator.Generate(expression);
            sql.ShouldBe("CREATE TABLE \"TestTable1\" (\"TestColumn1\" NVARCHAR2(255) NOT NULL, \"TestColumn2\" NUMBER(10,0) NOT NULL, CONSTRAINT \"TestKey\" PRIMARY KEY (\"TestColumn1\"))");
        }

        [Test]
        public void CanCreateTableWithNullableField()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithNullableColumn();

            var result = _generator.Generate(expression);

            result.ShouldBe(
                "CREATE TABLE \"TestTable1\" (\"TestColumn1\" NVARCHAR2(255), \"TestColumn2\" NUMBER(10,0) NOT NULL)");
        }

        [Test]
        public void CanCreateTableWithPrimaryKey()
        {
            //After a quick goolge the below tested statment should work.
            //CONSTRAINT constraint_name PRIMARY KEY (column1, column2, . column_n)
            var expression = GeneratorTestHelper.GetCreateTableWithPrimaryKeyExpression();
            string sql = _generator.Generate(expression);
            sql.ShouldBe("CREATE TABLE \"TestTable1\" (\"TestColumn1\" NVARCHAR2(255) NOT NULL, \"TestColumn2\" NUMBER(10,0) NOT NULL, PRIMARY KEY (\"TestColumn1\"))");
        }

        [Test]
        public void CanDropTable()
        {
            var expression = GeneratorTestHelper.GetDeleteTableExpression();
            string sql = _generator.Generate(expression);
            sql.ShouldBe("DROP TABLE \"TestTable1\"");
        }

        [Test]
        public void CanRenameTable()
        {
            var expression = GeneratorTestHelper.GetRenameTableExpression();
            string sql = generator.Generate(expression);
            sql.ShouldBe("ALTER TABLE \"TestTable1\" RENAME TO \"TestTable2\"");
        }
    }
}