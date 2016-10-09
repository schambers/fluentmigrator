﻿using System;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Tests.Helpers;
using Xunit;

namespace FluentMigrator.Tests.Unit.Expressions
{
    public class CreateSchemaExpressionTests
    {
        [Test]
        public void ErrorIsReturnedWhenSchemaNameIsEmptyString()
        {
            var expression = new CreateSchemaExpression { SchemaName = String.Empty };

            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.SchemaNameCannotBeNullOrEmpty);
        }

        [Test]
        public void ErrorIsNotReturnedWhenSchemaNameIsSet()
        {
            var expression = new CreateSchemaExpression { SchemaName = "schema1" };

            var errors = ValidationHelper.CollectErrors(expression);
            Assert.That(errors.Count, Is.EqualTo(0));
        }
    }
}