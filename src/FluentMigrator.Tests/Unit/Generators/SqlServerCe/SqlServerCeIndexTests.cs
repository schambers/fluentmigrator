﻿using FluentMigrator.Runner.Generators.SqlServer;
using NUnit.Framework;
using NUnit.Should;

namespace FluentMigrator.Tests.Unit.Generators.SqlServerCe
{
    [TestFixture]
    public class SqlServerCeIndexTests
    {
        protected SqlServerCeGenerator generator;

        [SetUp]
        public void Setup()
        {
            generator = new SqlServerCeGenerator();
        }

        [Test]
        public void CanCreateIndex()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();
            var sql = generator.Generate(expression);
            sql.ShouldBe("CREATE INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC)");
        }

        [Test]
        public void CanCreateMultiColumnIndex()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnCreateIndexExpression();
            var sql = generator.Generate(expression);
            sql.ShouldBe("CREATE INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        [Test]
        public void CanCreatUniqueMultiColumnIndex()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();
            var sql = generator.Generate(expression);
            sql.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        [Test]
        public void CanCreateUniqueIndex()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();
            var sql = generator.Generate(expression);
            sql.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC)");
        }

        [Test]
        public void CanDropIndex()
        {
            var expression = GeneratorTestHelper.GetDeleteIndexExpression();
            var sql = generator.Generate(expression);
            sql.ShouldBe("DROP INDEX [TestTable1].[TestIndex]");
        }
    }
}
