﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator.Runner.Generators;
using Xunit;

namespace FluentMigrator.Tests.Unit.Generators
{
    public abstract class BaseDescriptionGeneratorTests
    {
        protected IDescriptionGenerator descriptionGenerator;

        public abstract void GenerateDescriptionStatementsForCreateTableReturnTableDescriptionStatement();
        public abstract void GenerateDescriptionStatementsForCreateTableReturnTableDescriptionAndColumnDescriptionsStatements();
        public abstract void GenerateDescriptionStatementForAlterTableReturnTableDescriptionStatement();
        public abstract void GenerateDescriptionStatementForCreateColumnReturnColumnDescriptionStatement();
        public abstract void GenerateDescriptionStatementForAlterColumnReturnColumnDescriptionStatement();

        [Fact]
        public void GenerateDescriptionStatementsReturnEmptyForNoDescriptionsOnCreateTable()
        {
            var createTableExpression = GeneratorTestHelper.GetCreateTableExpression();
            var result = descriptionGenerator.GenerateDescriptionStatements(createTableExpression);

            result.ShouldBe(Enumerable.Empty<string>());
        }

        [Fact]
        public void GenerateDescriptionStatementReturnEmptyForNoDescriptionOnAlterTable()
        {
            var alterTableExpression = GeneratorTestHelper.GetAlterTableAutoIncrementColumnExpression();
            var result = descriptionGenerator.GenerateDescriptionStatement(alterTableExpression);

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void GenerateDescriptionStatementReturnEmptyForNoDescriptionOnCreateColumn()
        {
            var createColumnExpression = GeneratorTestHelper.GetCreateColumnExpression();
            var result = descriptionGenerator.GenerateDescriptionStatement(createColumnExpression);

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void GenerateDescriptionStatementReturnEmptyForNoDescriptionOnAlterColumn()
        {
            var alterColumnExpression = GeneratorTestHelper.GetAlterColumnExpression();
            var result = descriptionGenerator.GenerateDescriptionStatement(alterColumnExpression);

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void GenerateDescriptionStatementsHaveSingleStatementForDescriptionOnCreate()
        {
            var createTableExpression = GeneratorTestHelper.GetCreateTableWithTableDescription();
            var result = descriptionGenerator.GenerateDescriptionStatements(createTableExpression);

            result.Count().ShouldBe(1);
        }
    }
}
