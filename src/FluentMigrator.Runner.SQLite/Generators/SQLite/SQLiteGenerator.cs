#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
// Copyright (c) 2010, Nathan Brown
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

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.SQLite
{
    // ReSharper disable once InconsistentNaming
    public class SQLiteGenerator : GenericGenerator
    {
        public SQLiteGenerator()
            : this(new SQLiteQuoter(new OptionsWrapper<QuoterOptions>(new QuoterOptions())))
        {
        }

        public SQLiteGenerator(
            [NotNull] SQLiteQuoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        public SQLiteGenerator(
            [NotNull] SQLiteQuoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(new SQLiteColumn(quoter), quoter, new EmptyDescriptionGenerator(), generatorOptions)
        {
        }

        public override string RenameTable { get { return "ALTER TABLE {0} RENAME TO {1}"; } }
        public override string RenameColumn { get { return "ALTER TABLE {0} RENAME COLUMN {1} TO {2}"; } }

        public override string Generate(AlterColumnExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("SQLite does not support alter column");
        }

        public override string Generate(RenameColumnExpression expression)
        {
            if (CompatibilityMode == CompatibilityMode.STRICT)
            {
                return CompatibilityMode.HandleCompatibilty("SQLite does not support renaming of columns");
            }

            return string.Format(
                RenameColumn,
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Quoter.QuoteColumnName(expression.OldName),
                Quoter.QuoteColumnName(expression.NewName)
            );
        }

        public override string Generate(DeleteColumnExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("SQLite does not support deleting of columns");
        }

        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("SQLite does not support altering of default constraints");
        }

        public override string Generate(CreateForeignKeyExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Foreign keys are not supported in SQLite");
        }

        public override string Generate(DeleteForeignKeyExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Foreign keys are not supported in SQLite");
        }

        public override string Generate(CreateSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences are not supported in SQLite");
        }

        public override string Generate(DeleteSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences are not supported in SQLite");
        }

        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Default constraints are not supported");
        }

        public override string Generate(CreateConstraintExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Constraints are not supported");
        }

        public override string Generate(DeleteConstraintExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Constraints are not supported");
        }
    }
}
