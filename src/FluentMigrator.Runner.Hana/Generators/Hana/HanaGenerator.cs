#region License
//
// Copyright (c) 2018, Fluent Migrator Project
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

using System.Linq;
using System.Text;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.Hana
{
    public class HanaGenerator : GenericGenerator
    {
        public HanaGenerator()
            : this(new HanaQuoter(new OptionsWrapper<QuoterOptions>(new QuoterOptions())))
        {
        }

        public HanaGenerator(
            [NotNull] HanaQuoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        public HanaGenerator(
            [NotNull] HanaQuoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(new HanaColumn(quoter), quoter, new HanaDescriptionGenerator(), generatorOptions)
        {
        }

        public override string DropTable
        {
            get
            {
                return "DROP TABLE {0}";
            }
        }
        public override string Generate(DeleteTableExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        public override string Generate(RenameTableExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        public override string Generate(CreateSequenceExpression expression)
        {
            var result = new StringBuilder("CREATE SEQUENCE ");
            var seq = expression.Sequence;

            result.AppendFormat(Quoter.QuoteSequenceName(seq.Name));

            if (seq.Increment.HasValue)
            {
                result.AppendFormat(" INCREMENT BY {0}", seq.Increment);
            }

            if (seq.MinValue.HasValue)
            {
                result.AppendFormat(" MINVALUE {0}", seq.MinValue);
            }

            if (seq.MaxValue.HasValue)
            {
                result.AppendFormat(" MAXVALUE {0}", seq.MaxValue);
            }

            if (seq.StartWith.HasValue)
            {
                result.AppendFormat(" START WITH {0}", seq.StartWith);
            }

            const long MINIMUM_CACHE_VALUE = 2;
            if (seq.Cache.HasValue)
            {
                if (seq.Cache.Value < MINIMUM_CACHE_VALUE)
                {
                    return CompatibilityMode.HandleCompatibilty("Cache size must be greater than 1; if you intended to disable caching, set Cache to null.");
                }
                result.AppendFormat(" CACHE {0}", seq.Cache);
            }
            else
            {
                result.Append(" NO CACHE");
            }

            if (seq.Cycle)
            {
                result.Append(" CYCLE");
            }

            result.Append(";");


            return result.ToString();
        }

        public override string AddColumn
        {
            get { return "ALTER TABLE {0} ADD ({1})"; }
        }

        public override string AlterColumn
        {
            get { return "ALTER TABLE {0} ALTER ({1})"; }
        }

        public override string DropColumn
        {
            get { return "ALTER TABLE {0} DROP ({1})"; }
        }

        public override string RenameColumn { get { return "RENAME COLUMN {0}.{1} TO {2}"; } }

        public override string Generate(DeleteDataExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        public override string Generate(InsertDataExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        private string InnerGenerate(CreateTableExpression expression)
        {
            var tableName = Quoter.QuoteTableName(expression.TableName);
            return string.Format("CREATE COLUMN TABLE {0} ({1});", tableName, Column.Generate(expression.Columns, tableName));
        }

        public override string Generate(UpdateDataExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        public override string Generate(CreateTableExpression expression)
        {
            var descriptionStatements = DescriptionGenerator.GenerateDescriptionStatements(expression);
            var statements = descriptionStatements as string[] ?? descriptionStatements.ToArray();

            if (!statements.Any())
                return InnerGenerate(expression);

            var wrappedCreateTableStatement = InnerGenerate(expression);
            var createTableWithDescriptionsBuilder = new StringBuilder(wrappedCreateTableStatement);

            foreach (var descriptionStatement in statements)
            {
                if (!string.IsNullOrEmpty(descriptionStatement))
                {
                    createTableWithDescriptionsBuilder.Append(descriptionStatement);
                }
            }

            return WrapInBlock(createTableWithDescriptionsBuilder.ToString());
        }

        public override string Generate(AlterTableExpression expression)
        {
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);

            return string.Format("{0};",
                string.IsNullOrEmpty(descriptionStatement)
                ? base.Generate(expression) : descriptionStatement);
        }

        public override string Generate(CreateColumnExpression expression)
        {
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);

            if (string.IsNullOrEmpty(descriptionStatement))
                return string.Format("{0};",base.Generate(expression) );

            var wrappedCreateColumnStatement = base.Generate(expression);

            var createColumnWithDescriptionBuilder = new StringBuilder(wrappedCreateColumnStatement);
            createColumnWithDescriptionBuilder.Append(descriptionStatement);

            return WrapInBlock(createColumnWithDescriptionBuilder.ToString());
        }

        public override string Generate(AlterColumnExpression expression)
        {
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);

            if (string.IsNullOrEmpty(descriptionStatement))
                return string.Format("{0};", base.Generate(expression));

            var wrappedAlterColumnStatement = base.Generate(expression);

            var alterColumnWithDescriptionBuilder = new StringBuilder(wrappedAlterColumnStatement);
            alterColumnWithDescriptionBuilder.Append(descriptionStatement);

            return WrapInBlock(alterColumnWithDescriptionBuilder.ToString());
        }

        public override string Generate(DeleteColumnExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        public override string Generate(CreateForeignKeyExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }
        public override string Generate(CreateConstraintExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        public override string Generate(DeleteForeignKeyExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        public override string Generate(DeleteConstraintExpression expression)
        {
            if (expression.Constraint.IsPrimaryKeyConstraint)
            {
                return string.Format("ALTER TABLE {0} DROP PRIMARY KEY;", Quoter.QuoteTableName(expression.Constraint.TableName));
            }

            return string.Format("{0};", base.Generate(expression));
        }

        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Default constraints are not supported");
        }

        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Default constraints are not supported");
        }

        public override string Generate(CreateIndexExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        public override string Generate(DeleteIndexExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        public override string Generate(DeleteSequenceExpression expression)
        {
            var result = new StringBuilder("DROP SEQUENCE ")
                .Append(Quoter.QuoteSequenceName(expression.SequenceName))
                .Append(';');
            return result.ToString();
        }

        public override string Generate(RenameColumnExpression expression)
        {
            return string.Format("{0};", base.Generate(expression));
        }

        private static string WrapInBlock(string sql)
        {
            return string.IsNullOrEmpty(sql)
                ? string.Empty
                : string.Format("BEGIN {0} END;", sql);
        }
    }
}
