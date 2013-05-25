﻿using FluentMigrator.Runner.Generators.Jet;
using NUnit.Framework;
using NUnit.Should;

namespace FluentMigrator.Tests.Unit.Generators.Jet
{
    [TestFixture]
    public class JetTableTests
    {
        protected JetGenerator Generator;

        [SetUp]
        public void Setup()
        {
            Generator = new JetGenerator();
        }

        [Test]
        public void CanCreateTableWithCustomColumnTypeWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableExpression();
            expression.Columns[1].Type = null;
            expression.Columns[1].CustomType = "[timestamp]";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] VARCHAR(255) NOT NULL, [TestColumn2] [timestamp] NOT NULL)");
        }

        [Test]
        public void CanCreateTableWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] VARCHAR(255) NOT NULL, [TestColumn2] INTEGER NOT NULL)");
        }

        [Test]
        public void CanCreateTableWithDefaultValueExplicitlySetToNullWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithDefaultValue();
            expression.Columns[0].DefaultValue = null;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] VARCHAR(255) NOT NULL DEFAULT NULL, [TestColumn2] INTEGER NOT NULL DEFAULT 0)");
        }

        [Test]
        public void CanCreateTableWithDefaultValueWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithDefaultValue();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] VARCHAR(255) NOT NULL DEFAULT 'Default', [TestColumn2] INTEGER NOT NULL DEFAULT 0)");
        }

        [Test]
        public void CanCreateTableWithIdentityWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithAutoIncrementExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] COUNTER NOT NULL, [TestColumn2] INTEGER NOT NULL)");
        }

        [Test]
        public void CanCreateTableWithMultiColumnPrimaryKeyWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithMultiColumnPrimaryKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] VARCHAR(255) NOT NULL, [TestColumn2] INTEGER NOT NULL, PRIMARY KEY ([TestColumn1], [TestColumn2]))");
        }

        [Test]
        public void CanCreateTableWithNamedMultiColumnPrimaryKeyWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithNamedMultiColumnPrimaryKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] VARCHAR(255) NOT NULL, [TestColumn2] INTEGER NOT NULL, CONSTRAINT [TestKey] PRIMARY KEY ([TestColumn1], [TestColumn2]))");
        }

        [Test]
        public void CanCreateTableWithNamedPrimaryKeyWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithNamedPrimaryKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] VARCHAR(255) NOT NULL, [TestColumn2] INTEGER NOT NULL, CONSTRAINT [TestKey] PRIMARY KEY ([TestColumn1]))");
        }

        [Test]
        public void CanCreateTableWithNullableFieldWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithNullableColumn();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] VARCHAR(255), [TestColumn2] INTEGER NOT NULL)");
        }

        [Test]
        public void CanCreateTableWithPrimaryKeyWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithPrimaryKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] VARCHAR(255) NOT NULL, [TestColumn2] INTEGER NOT NULL, PRIMARY KEY ([TestColumn1]))");
        }

        [Test]
        public void CanDropTableWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteTableExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP TABLE [TestTable1]");
        }

        [Test]
        public void CanRenameTableWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }
    }
}
