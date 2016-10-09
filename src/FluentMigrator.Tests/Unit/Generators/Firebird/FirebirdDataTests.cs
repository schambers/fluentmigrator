﻿using FluentMigrator.Runner.Generators.Firebird;
using FluentMigrator.Runner.Processors.Firebird;
using Xunit;

namespace FluentMigrator.Tests.Unit.Generators.Firebird
{
    public class FirebirdDataTests : BaseDataTests
    {
        protected FirebirdGenerator Generator;

        [SetUp]
        public void Setup()
        {
            Generator = new FirebirdGenerator(FirebirdOptions.StandardBehaviour());
        }

        [Fact]
        public override void CanDeleteDataForAllRowsWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataAllRowsExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("DELETE FROM TestTable1 WHERE 1 = 1");
        }

        [Fact]
        public override void CanDeleteDataForAllRowsWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataAllRowsExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DELETE FROM TestTable1 WHERE 1 = 1");

        }

        [Fact]
        public override void CanDeleteDataForMultipleRowsWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataMultipleRowsExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("DELETE FROM TestTable1 WHERE Name = 'Just''in' AND Website IS NULL; DELETE FROM TestTable1 WHERE Website = 'github.com'");
        }

        [Fact]
        public override void CanDeleteDataForMultipleRowsWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataMultipleRowsExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DELETE FROM TestTable1 WHERE Name = 'Just''in' AND Website IS NULL; DELETE FROM TestTable1 WHERE Website = 'github.com'");
        }

        [Fact]
        public override void CanDeleteDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("DELETE FROM TestTable1 WHERE Name = 'Just''in' AND Website IS NULL");
        }

        [Fact]
        public override void CanDeleteDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DELETE FROM TestTable1 WHERE Name = 'Just''in' AND Website IS NULL");
        }

        [Fact]
        public override void CanInsertDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetInsertDataExpression();
            expression.SchemaName = "TestSchema";

            var expected = "INSERT INTO TestTable1 (Id, Name, Website) VALUES (1, 'Just''in', 'codethinked.com');";
            expected += " INSERT INTO TestTable1 (Id, Name, Website) VALUES (2, 'Na\\te', 'kohari.org')";

            var result = Generator.Generate(expression);
            result.ShouldBe(expected);
        }

        [Fact]
        public override void CanInsertDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetInsertDataExpression();

            var expected = "INSERT INTO TestTable1 (Id, Name, Website) VALUES (1, 'Just''in', 'codethinked.com');";
            expected += " INSERT INTO TestTable1 (Id, Name, Website) VALUES (2, 'Na\\te', 'kohari.org')";

            var result = Generator.Generate(expression);
            result.ShouldBe(expected);
        }

        [Fact]
        public override void CanInsertGuidDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetInsertGUIDExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe(System.String.Format("INSERT INTO TestTable1 (guid) VALUES ('{0}')", GeneratorTestHelper.TestGuid));
        }

        [Fact]
        public override void CanInsertGuidDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetInsertGUIDExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe(System.String.Format("INSERT INTO TestTable1 (guid) VALUES ('{0}')", GeneratorTestHelper.TestGuid));
        }

        [Fact]
        public override void CanUpdateDataForAllDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpressionWithAllRows();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("UPDATE TestTable1 SET Name = 'Just''in', Age = 25 WHERE 1 = 1");
        }

        [Fact]
        public override void CanUpdateDataForAllDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpressionWithAllRows();

            var result = Generator.Generate(expression);
            result.ShouldBe("UPDATE TestTable1 SET Name = 'Just''in', Age = 25 WHERE 1 = 1");
        }

        [Fact]
        public override void CanUpdateDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("UPDATE TestTable1 SET Name = 'Just''in', Age = 25 WHERE Id = 9 AND Homepage IS NULL");
        }

        [Fact]
        public override void CanUpdateDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("UPDATE TestTable1 SET Name = 'Just''in', Age = 25 WHERE Id = 9 AND Homepage IS NULL");
        }
    }
}
