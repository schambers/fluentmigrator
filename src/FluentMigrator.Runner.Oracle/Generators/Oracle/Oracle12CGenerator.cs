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

using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.Oracle
{
    public class Oracle12CGenerator : OracleGenerator, IOracle12CGenerator
    {
        public Oracle12CGenerator()
            : this(false)
        {
        }

        public Oracle12CGenerator(bool useQuotedIdentifiers)
            : this(GetQuoter(useQuotedIdentifiers, new OptionsWrapper<QuoterOptions>(new QuoterOptions())))
        {
        }

        public Oracle12CGenerator(
            [NotNull] OracleQuoterBase quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        public Oracle12CGenerator(
            [NotNull] OracleQuoterBase quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(new Oracle12CColumn(quoter), quoter, new OracleDescriptionGenerator(), generatorOptions)
        {
        }
    }
}
