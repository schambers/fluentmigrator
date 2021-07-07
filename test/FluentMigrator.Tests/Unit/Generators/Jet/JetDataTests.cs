using FluentMigrator.Runner.Generators.Jet;
using FluentMigrator.Runner.Initialization;

using Microsoft.Extensions.Options;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Jet
{
    [TestFixture]
    public class JetDataTests : BaseDataTests
    {
        private static JetGenerator CreateFixture(QuoterOptions options = null) =>
            new JetGenerator(new JetQuoter(new OptionsWrapper<QuoterOptions>(options)));

        [Test]
        public override void CanDeleteDataForAllRowsWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataAllRowsExpression();
            expression.SchemaName = "TestSchema";

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("DELETE FROM [TestTable1] WHERE 1 = 1");
        }

        [Test]
        public override void CanDeleteDataForAllRowsWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataAllRowsExpression();

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("DELETE FROM [TestTable1] WHERE 1 = 1");
        }

        [Test]
        public override void CanDeleteDataForMultipleRowsWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataMultipleRowsExpression();
            expression.SchemaName = "TestSchema";

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("DELETE FROM [TestTable1] WHERE [Name] = 'Just''in' AND [Website] IS NULL; DELETE FROM [TestTable1] WHERE [Website] = 'github.com'");
        }

        [Test]
        public override void CanDeleteDataForMultipleRowsWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataMultipleRowsExpression();

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("DELETE FROM [TestTable1] WHERE [Name] = 'Just''in' AND [Website] IS NULL; DELETE FROM [TestTable1] WHERE [Website] = 'github.com'");
        }

        [Test]
        public override void CanDeleteDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataExpression();
            expression.SchemaName = "TestSchema";

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("DELETE FROM [TestTable1] WHERE [Name] = 'Just''in' AND [Website] IS NULL");
        }

        [Test]
        public override void CanDeleteDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataExpression();

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("DELETE FROM [TestTable1] WHERE [Name] = 'Just''in' AND [Website] IS NULL");
        }

        [Test]
        public override void CanDeleteDataWithDbNullCriteria()
        {
            var expression = GeneratorTestHelper.GetDeleteDataExpressionWithDbNullValue();
            var result = CreateFixture().Generate(expression);
            result.ShouldBe("DELETE FROM [TestTable1] WHERE [Name] = 'Just''in' AND [Website] IS NULL");
        }

        [Test]
        public override void CanInsertDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetInsertDataExpression();
            expression.SchemaName = "TestSchema";

            var expected = "INSERT INTO [TestTable1] ([Id], [Name], [Website]) VALUES (1, 'Just''in', 'codethinked.com');";
            expected += @" INSERT INTO [TestTable1] ([Id], [Name], [Website]) VALUES (2, 'Na\te', 'kohari.org')";

            var result = CreateFixture().Generate(expression);
            result.ShouldBe(expected);
        }

        [Test]
        public override void CanInsertDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetInsertDataExpression();

            var expected = "INSERT INTO [TestTable1] ([Id], [Name], [Website]) VALUES (1, 'Just''in', 'codethinked.com');";
            expected += @" INSERT INTO [TestTable1] ([Id], [Name], [Website]) VALUES (2, 'Na\te', 'kohari.org')";

            var result = CreateFixture().Generate(expression);
            result.ShouldBe(expected);
        }

        [Test]
        public override void CanInsertGuidDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetInsertGUIDExpression();
            expression.SchemaName = "TestSchema";

            var result = CreateFixture().Generate(expression);
            result.ShouldBe(string.Format("INSERT INTO [TestTable1] ([guid]) VALUES ('{0}')", GeneratorTestHelper.TestGuid.ToString()));
        }

        [Test]
        public override void CanInsertGuidDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetInsertGUIDExpression();

            var result = CreateFixture().Generate(expression);
            result.ShouldBe(string.Format("INSERT INTO [TestTable1] ([guid]) VALUES ('{0}')", GeneratorTestHelper.TestGuid.ToString()));
        }

        [Test]
        public override void CanInsertEnumAsString()
        {
            var expression = GeneratorTestHelper.GetInsertEnumExpression();

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("INSERT INTO [TestTable1] ([enum]) VALUES ('Boo')");
        }

        [Test]
        public override void CanInsertEnumAsUnderlyingType()
        {
            var options = new QuoterOptions
            {
                EnumAsUnderlyingType = true
            };

            var expression = GeneratorTestHelper.GetInsertEnumExpression();

            var result = CreateFixture(options).Generate(expression);
            result.ShouldBe("INSERT INTO [TestTable1] ([enum]) VALUES (2)");
        }

        [Test]
        public override void CanUpdateDataForAllDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpressionWithAllRows();
            expression.SchemaName = "TestSchema";

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("UPDATE [TestTable1] SET [Name] = 'Just''in', [Age] = 25 WHERE 1 = 1");
        }

        [Test]
        public override void CanUpdateDataForAllDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpressionWithAllRows();

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("UPDATE [TestTable1] SET [Name] = 'Just''in', [Age] = 25 WHERE 1 = 1");
        }

        [Test]
        public override void CanUpdateDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpression();
            expression.SchemaName = "TestSchema";

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("UPDATE [TestTable1] SET [Name] = 'Just''in', [Age] = 25 WHERE [Id] = 9 AND [Homepage] IS NULL");
        }

        [Test]
        public override void CanUpdateDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpression();

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("UPDATE [TestTable1] SET [Name] = 'Just''in', [Age] = 25 WHERE [Id] = 9 AND [Homepage] IS NULL");
        }

        [Test]
        public override void CanUpdateDataWithDbNullCriteria()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpressionWithDbNullValue();

            var result = CreateFixture().Generate(expression);
            result.ShouldBe("UPDATE [TestTable1] SET [Name] = 'Just''in', [Age] = 25 WHERE [Id] = 9 AND [Homepage] IS NULL");
        }
    }
}
