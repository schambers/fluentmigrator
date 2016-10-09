﻿using FluentMigrator.Exceptions;
using FluentMigrator.Runner.Generators.SqlServer;
using Xunit;

namespace FluentMigrator.Tests.Unit.Generators.SqlServerCe
{
    public class SqlServerCeColumnTests : BaseColumnTests
    {
        protected SqlServerCeGenerator Generator;

        [SetUp]
        public void Setup()
        {
            Generator = new SqlServerCeGenerator();
        }

        [Fact]
        public override void CanAlterColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ALTER COLUMN [TestColumn1] NVARCHAR(20) NOT NULL");
        }

        [Fact]
        public void CanAlterColumnToNullableWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.SchemaName = "TestSchema";
            expression.Column.IsNullable = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ALTER COLUMN [TestColumn1] NVARCHAR(20) NULL");
        }

        [Fact]
        public override void CanAlterColumnWithDefaultSchema()
        {
            //TODO: This will fail if there are any keys attached 
            var expression = GeneratorTestHelper.GetAlterColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ALTER COLUMN [TestColumn1] NVARCHAR(20) NOT NULL");
        }

        [Fact]
        public void CanAlterColumnToNullableWithDefaultSchema()
        {
            //TODO: This will fail if there are any keys attached 
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.Column.IsNullable = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ALTER COLUMN [TestColumn1] NVARCHAR(20) NULL");
        }

        [Fact]
        public override void CanCreateAutoIncrementColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnAddAutoIncrementExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ALTER COLUMN [TestColumn1] INT NOT NULL IDENTITY(1,1)");
        }

        [Fact]
        public override void CanCreateAutoIncrementColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnAddAutoIncrementExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ALTER COLUMN [TestColumn1] INT NOT NULL IDENTITY(1,1)");
        }

        [Fact]
        public override void CanCreateColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD [TestColumn1] NVARCHAR(5) NOT NULL");
        }

        [Fact]
        public override void CanCreateColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD [TestColumn1] NVARCHAR(5) NOT NULL");
        }

        [Fact]
        public override void CanCreateDecimalColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateDecimalColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD [TestColumn1] NUMERIC(19,2) NOT NULL");
        }

        [Fact]
        public override void CanCreateDecimalColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateDecimalColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD [TestColumn1] NUMERIC(19,2) NOT NULL");
        }

        [Fact]
        public override void CanDropColumnWithCustomSchema()
        {
            //This does not work if column in used in constraint, index etc.
            var expression = GeneratorTestHelper.GetDeleteColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] DROP COLUMN [TestColumn1];");
        }

        [Fact]
        public override void CanDropColumnWithDefaultSchema()
        {
            //This does not work if column in used in constraint, index etc.
            var expression = GeneratorTestHelper.GetDeleteColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] DROP COLUMN [TestColumn1];");
        }

        [Fact]
        public override void CanDropMultipleColumnsWithCustomSchema()
        {
            //This does not work if it is a primary key
            var expression = GeneratorTestHelper.GetDeleteColumnExpression(new[] { "TestColumn1", "TestColumn2" });
            expression.SchemaName = "TestSchema";

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }

        [Fact]
        public override void CanDropMultipleColumnsWithDefaultSchema()
        {
            //This does not work if it is a primary key
            var expression = GeneratorTestHelper.GetDeleteColumnExpression(new[] { "TestColumn1", "TestColumn2" });

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }

        [Fact]
        public override void CanRenameColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();
            expression.SchemaName = "TestSchema";

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }

        [Fact]
        public override void CanRenameColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }
    }
}
