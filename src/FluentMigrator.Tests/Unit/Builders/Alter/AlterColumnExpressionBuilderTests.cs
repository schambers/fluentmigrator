#region License
// 
// Copyright (c) 2007-2009, Sean Chambers <schambers80@gmail.com>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using FluentMigrator.Builders.Alter.Column;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using Moq;
using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Builders.Alter
{
	[TestFixture]
	public class AlterColumnExpressionBuilderTests
	{
		[Test]
		public void CallingOnTableSetsTableName()
		{
			var expressionMock = new Mock<AlterColumnExpression>();
			expressionMock.VerifySet(x => x.TableName = "Bacon", Times.AtMostOnce());

			var contextMock = new Mock<IMigrationContext>();

			var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);
			builder.OnTable("Bacon");

			expressionMock.VerifyAll();
		}

		[Test]
		public void CallingAsAnsiStringSetsColumnDbTypeToAnsiString()
		{
			VerifyColumnDbType(DbType.AnsiString, b => b.AsAnsiString());
		}

		[Test]
		public void CallingAsAnsiStringWithSizeSetsColumnDbTypeToAnsiString()
		{
			VerifyColumnDbType(DbType.AnsiString, b => b.AsAnsiString(42));
		}

		[Test]
		public void CallingAsAnsiStringWithSizeSetsColumnSizeToSpecifiedValue()
		{
			VerifyColumnSize(42, b => b.AsAnsiString(42));
		}

		[Test]
		public void CallingAsBinarySetsColumnDbTypeToBinary()
		{
			VerifyColumnDbType(DbType.Binary, b => b.AsBinary());
		}

		[Test]
		public void CallingAsBinaryWithSizeSetsColumnDbTypeToBinary()
		{
			VerifyColumnDbType(DbType.Binary, b => b.AsBinary(42));
		}

		[Test]
		public void CallingAsBinaryWithSizeSetsColumnSizeToSpecifiedValue()
		{
			VerifyColumnSize(42, b => b.AsBinary(42));
		}

		[Test]
		public void CallingAsBooleanSetsColumnDbTypeToBoolean()
		{
			VerifyColumnDbType(DbType.Boolean, b => b.AsBoolean());
		}

		[Test]
		public void CallingAsByteSetsColumnDbTypeToByte()
		{
			VerifyColumnDbType(DbType.Byte, b => b.AsByte());
		}

		[Test]
		public void CallingAsCurrencySetsColumnDbTypeToCurrency()
		{
			VerifyColumnDbType(DbType.Currency, b => b.AsCurrency());
		}

		[Test]
		public void CallingAsDateSetsColumnDbTypeToDate()
		{
			VerifyColumnDbType(DbType.Date, b => b.AsDate());
		}

		[Test]
		public void CallingAsDateTimeSetsColumnDbTypeToDateTime()
		{
			VerifyColumnDbType(DbType.DateTime, b => b.AsDateTime());
		}

		[Test]
		public void CallingAsDecimalSetsColumnDbTypeToDecimal()
		{
			VerifyColumnDbType(DbType.Decimal, b => b.AsDecimal());
		}

		[Test]
		public void CallingAsDecimalWithSizeAndPrecisionSetsColumnDbTypeToDecimal()
		{
			VerifyColumnDbType(DbType.Decimal, b => b.AsDecimal(1, 2));
		}

		[Test]
		public void CallingAsDecimalStringSetsColumnSizeToSpecifiedValue()
		{
			VerifyColumnSize(1, b => b.AsDecimal(1, 2));
		}

		[Test]
		public void CallingAsDecimalStringSetsColumnPrecisionToSpecifiedValue()
		{
			VerifyColumnPrecision(2, b => b.AsDecimal(1, 2));
		}

		[Test]
		public void CallingAsDoubleSetsColumnDbTypeToDouble()
		{
			VerifyColumnDbType(DbType.Double, b => b.AsDouble());
		}

		[Test]
		public void CallingAsGuidSetsColumnDbTypeToGuid()
		{
			VerifyColumnDbType(DbType.Guid, b => b.AsGuid());
		}

		[Test]
		public void CallingAsFixedLengthStringSetsColumnDbTypeToStringFixedLength()
		{
			VerifyColumnDbType(DbType.StringFixedLength, e => e.AsFixedLengthString(255));
		}

		[Test]
		public void CallingAsFixedLengthStringSetsColumnSizeToSpecifiedValue()
		{
			VerifyColumnSize(255, b => b.AsFixedLengthString(255));
		}

		[Test]
		public void CallingAsFixedLengthAnsiStringSetsColumnDbTypeToAnsiStringFixedLength()
		{
			VerifyColumnDbType(DbType.AnsiStringFixedLength, b => b.AsFixedLengthAnsiString(255));
		}

		[Test]
		public void CallingAsFixedLengthAnsiStringSetsColumnSizeToSpecifiedValue()
		{
			VerifyColumnSize(255, b => b.AsFixedLengthAnsiString(255));
		}

		[Test]
		public void CallingAsFloatSetsColumnDbTypeToSingle()
		{
			VerifyColumnDbType(DbType.Single, b => b.AsFloat());
		}

		[Test]
		public void CallingAsInt16SetsColumnDbTypeToInt16()
		{
			VerifyColumnDbType(DbType.Int16, b => b.AsInt16());
		}

		[Test]
		public void CallingAsInt32SetsColumnDbTypeToInt32()
		{
			VerifyColumnDbType(DbType.Int32, b => b.AsInt32());
		}

		[Test]
		public void CallingAsInt64SetsColumnDbTypeToInt64()
		{
			VerifyColumnDbType(DbType.Int64, b => b.AsInt64());
		}

		[Test]
		public void CallingAsStringSetsColumnDbTypeToString()
		{
			VerifyColumnDbType(DbType.String, b => b.AsString());
		}

		[Test]
		public void CallingAsStringWithLengthSetsColumnDbTypeToString()
		{
			VerifyColumnDbType(DbType.String, b => b.AsString(255));
		}

		[Test]
		public void CallingAsStringSetsColumnSizeToSpecifiedValue()
		{
			VerifyColumnSize(255, b => b.AsFixedLengthAnsiString(255));
		}

		[Test]
		public void CallingAsTimeSetsColumnDbTypeToTime()
		{
			VerifyColumnDbType(DbType.Time, b => b.AsTime());
		}

		[Test]
		public void CallingAsXmlSetsColumnDbTypeToXml()
		{
			VerifyColumnDbType(DbType.Xml, b => b.AsXml());
		}

		[Test]
		public void CallingAsXmlWithSizeSetsColumnDbTypeToXml()
		{
			VerifyColumnDbType(DbType.Xml, b => b.AsXml(255));
		}

		[Test]
		public void CallingAsXmlSetsColumnSizeToSpecifiedValue()
		{
			VerifyColumnSize(255, b => b.AsXml(255));
		}

		[Test]
		public void CallingAsCustomSetsTypeToNullAndSetsCustomType()
		{
			this.VerifyColumnProperty(c => c.Type = null, b => b.AsCustom("Test"));
			this.VerifyColumnProperty(c => c.CustomType = "Test", b => b.AsCustom("Test"));
		}

		[Test]
		public void CallingWithDefaultValueAddsAlterDefaultConstraintExpression()
		{
			const int value = 42;

			var columnMock = new Mock<ColumnDefinition>();
            columnMock.VerifySet(c => c.DefaultValue = value, Times.AtMostOnce());

            var expressionMock = new Mock<AlterColumnExpression>();
			expressionMock.SetupProperty(e => e.Column);

			var expression = expressionMock.Object;
			expression.Column = columnMock.Object;

            var collectionMock = new Mock<ICollection<IMigrationExpression>>();
            collectionMock.Verify(x => x.Add(It.Is<AlterDefaultConstraintExpression>(e => e.DefaultValue.Equals(value))), Times.AtMostOnce());

            var contextMock = new Mock<IMigrationContext>();
            contextMock.Setup(x => x.Expressions).Returns(collectionMock.Object);
            contextMock.VerifyGet(x => x.Expressions, Times.AtMostOnce());

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);
			builder.WithDefaultValue(value);

            collectionMock.VerifyAll();
            contextMock.VerifyAll();
		}

		[Test]
		public void CallingIdentitySetsIsIdentityToTrue()
		{
			VerifyColumnProperty(c => c.IsIdentity = true, b => b.Identity());
		}

		[Test]
		public void CallingIndexedSetsIsIndexedToTrue()
		{
			VerifyColumnProperty(c => c.IsIndexed = true, b => b.Indexed());
		}

		[Test]
		public void CallingPrimaryKeySetsIsPrimaryKeyToTrue()
		{
			VerifyColumnProperty(c => c.IsPrimaryKey = true, b => b.PrimaryKey());
		}

		[Test]
		public void CallingNullableSetsIsNullableToTrue()
		{
			VerifyColumnProperty(c => c.IsNullable = true, b => b.Nullable());
		}

		[Test]
		public void CallingNotNullableSetsIsNullableToFalse()
		{
			VerifyColumnProperty(c => c.IsNullable = false, b => b.NotNullable());
		}

		[Test]
		public void CallingUniqueSetsIsUniqueToTrue()
		{
			VerifyColumnProperty(c => c.IsUnique = true, b => b.Unique());
		}

		private void VerifyColumnProperty(Action<ColumnDefinition> columnExpression, Action<AlterColumnExpressionBuilder> callToTest)
		{
			var columnMock = new Mock<ColumnDefinition>();
            columnMock.VerifySet(columnExpression, Times.AtMostOnce());

			var expressionMock = new Mock<AlterColumnExpression>();
			expressionMock.SetupProperty(e => e.Column);

			var expression = expressionMock.Object;
			expression.Column = columnMock.Object;

			var contextMock = new Mock<IMigrationContext>();

			callToTest(new AlterColumnExpressionBuilder(expression, contextMock.Object));

			columnMock.VerifyAll();
		}

		private void VerifyColumnDbType(DbType expected, Action<AlterColumnExpressionBuilder> callToTest)
		{
			var columnMock = new Mock<ColumnDefinition>();
            columnMock.VerifySet(c => c.Type = expected, Times.AtMostOnce());

			var expressionMock = new Mock<AlterColumnExpression>();
			expressionMock.SetupProperty(e => e.Column);

			var expression = expressionMock.Object;
			expression.Column = columnMock.Object;

			var contextMock = new Mock<IMigrationContext>();

			callToTest(new AlterColumnExpressionBuilder(expression, contextMock.Object));

			columnMock.VerifyAll();
		}

        private void VerifyColumnSize(int expected, Action<AlterColumnExpressionBuilder> callToTest)
		{
			var columnMock = new Mock<ColumnDefinition>();
            columnMock.VerifySet(c => c.Size = expected, Times.AtMostOnce());

            var expressionMock = new Mock<AlterColumnExpression>();
			expressionMock.SetupProperty(e => e.Column);

			var expression = expressionMock.Object;
			expression.Column = columnMock.Object;

			var contextMock = new Mock<IMigrationContext>();

            callToTest(new AlterColumnExpressionBuilder(expression, contextMock.Object));

			columnMock.VerifyAll();
		}

        private void VerifyColumnPrecision(int expected, Action<AlterColumnExpressionBuilder> callToTest)
		{
			var columnMock = new Mock<ColumnDefinition>();
            columnMock.VerifySet(c => c.Precision = expected, Times.AtMostOnce());

            var expressionMock = new Mock<AlterColumnExpression>();
			expressionMock.SetupProperty(e => e.Column);

			var expression = expressionMock.Object;
			expression.Column = columnMock.Object;

			var contextMock = new Mock<IMigrationContext>();

            callToTest(new AlterColumnExpressionBuilder(expression, contextMock.Object));

			columnMock.VerifyAll();
		}
	}
}