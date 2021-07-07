#region License
// Copyright (c) 2018, FluentMigrator Project
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

using System.Collections.Generic;

using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.Postgres
{
    class Postgres10_0Processor : PostgresProcessor
    {
        public Postgres10_0Processor(
            [NotNull] PostgresDbFactory factory,
            [NotNull] Postgres10_0Generator generator,
            [NotNull] ILogger<PostgresProcessor> logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor,
            [NotNull] PostgresOptions pgOptions,
            [NotNull] IOptions<QuoterOptions> quoterOptions)
            : base(factory, generator, logger, options, connectionStringAccessor, pgOptions, quoterOptions)
        {
        }

        public override string DatabaseType => "PostgreSQL10_0";

        public override IList<string> DatabaseTypeAliases { get; } = new List<string> { "PostgreSQL" };

    }
}
