using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.Jet
{
    public class JetGenerator : GenericGenerator
    {
        public JetGenerator()
            : this(new JetQuoter(new OptionsWrapper<QuoterOptions>(new QuoterOptions())))
        {
        }

        public JetGenerator(
            [NotNull] JetQuoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        public JetGenerator(
            [NotNull] JetQuoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(new JetColumn(quoter), quoter, new EmptyDescriptionGenerator(), generatorOptions)
        {
        }

        public override string DropIndex { get { return "DROP INDEX {0} ON {1}"; } }

        public override string Generate(RenameTableExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Renaming of tables is not supported for Jet");
        }

        public override string Generate(RenameColumnExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Renaming of columns is not supported for Jet");
        }

        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Altering of default constraints is not supported for Jet");
        }

        public override string Generate(CreateSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences are not supported for Jet");
        }

        public override string Generate(DeleteSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences are not supported for Jet");
        }

        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Default constraints are not supported");
        }
    }
}
