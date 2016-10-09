﻿using System;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Tests.Helpers;
using Xunit;

namespace FluentMigrator.Tests.Unit.Expressions
{
    public class DeleteSchemaExpressionTests
    {
        [Test]
        public void ErrorIsReturnedWhenSchemaNameIsEmptyString()
        {
            var expression = new DeleteSchemaExpression { SchemaName = String.Empty };

            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.SchemaNameCannotBeNullOrEmpty);
        }

        [Test]
        public void ErrorIsNotReturnedWhenSchemaNameIsSet()
        {
            var expression = new DeleteSchemaExpression { SchemaName = "schema1" };

            var errors = ValidationHelper.CollectErrors(expression);
            Assert.That(errors.Count, Is.EqualTo(0));
        }
    }
}