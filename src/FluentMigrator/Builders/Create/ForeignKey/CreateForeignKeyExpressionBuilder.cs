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

using System.Data;
using FluentMigrator.Expressions;

namespace FluentMigrator.Builders.Create.ForeignKey
{
	public class CreateForeignKeyExpressionBuilder: ExpressionBuilderBase<CreateForeignKeyExpression>,
		ICreateForeignKeyFromTableSyntax,
		ICreateForeignKeyForeignColumnOrInSchemaSyntax,
		ICreateForeignKeyToTableSyntax,
		ICreateForeignKeyPrimaryColumnOrInSchemaSyntax,
		ICreateForiegnKeyCascadeSyntax
	{
		public CreateForeignKeyExpressionBuilder(CreateForeignKeyExpression expression)
			: base(expression)
		{
		}

		public ICreateForeignKeyForeignColumnOrInSchemaSyntax FromTable(string table)
		{
			Expression.ForeignKey.TableContainingForeignKey = table;
			return this;
		}

		public ICreateForeignKeyToTableSyntax ForeignColumn(string column)
		{
			Expression.ForeignKey.ColumnsInForeignKeyTableToInclude.Add(column);
			return this;
		}

		public ICreateForeignKeyToTableSyntax ForeignColumns(params string[] columns)
		{
			foreach(var column in columns)
				Expression.ForeignKey.ColumnsInForeignKeyTableToInclude.Add(column);
			return this;
		}

		ICreateForeignKeyForeignColumnSyntax ICreateForeignKeyForeignColumnOrInSchemaSyntax.InSchema(string schemaName)
		{
			Expression.ForeignKey.SchemaOfTableContainingForeignKey = schemaName;
			return this;
		}

		public ICreateForeignKeyPrimaryColumnOrInSchemaSyntax ToTable(string table)
		{
			Expression.ForeignKey.TableContainingPrimayKey = table;
			return this;
		}

		public ICreateForiegnKeyCascadeSyntax PrimaryColumn(string column)
		{
			Expression.ForeignKey.ColumnsInPrimaryKeyTableToInclude.Add(column);
			return this;
		}

		public ICreateForiegnKeyCascadeSyntax PrimaryColumns(params string[] columns)
		{
			foreach(var column in columns)
				Expression.ForeignKey.ColumnsInPrimaryKeyTableToInclude.Add(column);
			return this;
		}

		public ICreateForiegnKeyCascadeSyntax OnDelete(Rule rule)
		{
			Expression.ForeignKey.OnDelete = rule;
			return this;
		}

		public ICreateForiegnKeyCascadeSyntax OnUpdate(Rule rule)
		{
			Expression.ForeignKey.OnUpdate = rule;
			return this;
		}

		public void OnDeleteOrUpdate(Rule rule)
		{
			Expression.ForeignKey.OnDelete = rule;
			Expression.ForeignKey.OnUpdate = rule;
		}

		ICreateForeignKeyPrimaryColumnSyntax ICreateForeignKeyPrimaryColumnOrInSchemaSyntax.InSchema(string schemaName)
		{
			Expression.ForeignKey.SchemaOfTableContainingPrimaryKey = schemaName;
			return this;
		}
	}
}