#region License
// Copyright (c) 2007-2018, FluentMigrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Linq;

using FluentMigrator.Runner.Generators.Redshift;
using FluentMigrator.Runner.Initialization;

using Microsoft.Extensions.Options;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Redshift
{
    [TestFixture]
    public class RedshiftDescriptionGeneratorTests : BaseDescriptionGeneratorTests
    {
        [SetUp]
        public void Setup()
        {
            var quoter = new RedshiftQuoter(new OptionsWrapper<QuoterOptions>(new QuoterOptions()));
            DescriptionGenerator = new RedshiftDescriptionGenerator(quoter);
        }

        [Test]
        public override void GenerateDescriptionStatementsForCreateTableReturnTableDescriptionStatement()
        {
            var createTableExpression = GeneratorTestHelper.GetCreateTableWithTableDescription();
            var statements = DescriptionGenerator.GenerateDescriptionStatements(createTableExpression);

            var result = statements.First();
            result.ShouldBe("COMMENT ON TABLE \"public\".\"TestTable1\" IS 'TestDescription';");
        }

        [Test]
        public override void
            GenerateDescriptionStatementsForCreateTableReturnTableDescriptionAndColumnDescriptionsStatements()
        {
            var createTableExpression = GeneratorTestHelper.GetCreateTableWithTableDescriptionAndColumnDescriptions();
            var statements = DescriptionGenerator.GenerateDescriptionStatements(createTableExpression).ToArray();

            var result = string.Join(string.Empty, statements);
            result.ShouldBe(
                "COMMENT ON TABLE \"public\".\"TestTable1\" IS 'TestDescription';COMMENT ON COLUMN \"public\".\"TestTable1\".\"TestColumn1\" IS 'TestColumn1Description';COMMENT ON COLUMN \"public\".\"TestTable1\".\"TestColumn2\" IS 'TestColumn2Description';");
        }

        [Test]
        public override void GenerateDescriptionStatementForAlterTableReturnTableDescriptionStatement()
        {
            var alterTableExpression = GeneratorTestHelper.GetAlterTableWithDescriptionExpression();
            var statement = DescriptionGenerator.GenerateDescriptionStatement(alterTableExpression);

            statement.ShouldBe("COMMENT ON TABLE \"public\".\"TestTable1\" IS 'TestDescription';");
        }

        [Test]
        public override void GenerateDescriptionStatementForCreateColumnReturnColumnDescriptionStatement()
        {
            var createColumnExpression = GeneratorTestHelper.GetCreateColumnExpressionWithDescription();
            var statement = DescriptionGenerator.GenerateDescriptionStatement(createColumnExpression);

            statement.ShouldBe("COMMENT ON COLUMN \"public\".\"TestTable1\".\"TestColumn1\" IS 'TestColumn1Description';");
        }

        [Test]
        public override void GenerateDescriptionStatementForAlterColumnReturnColumnDescriptionStatement()
        {
            var alterColumnExpression = GeneratorTestHelper.GetAlterColumnExpressionWithDescription();
            var statement = DescriptionGenerator.GenerateDescriptionStatement(alterColumnExpression);

            statement.ShouldBe("COMMENT ON COLUMN \"public\".\"TestTable1\".\"TestColumn1\" IS 'TestColumn1Description';");
        }
    }
}
