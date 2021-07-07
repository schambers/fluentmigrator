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

using System;

using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Initialization;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.Postgres
{
    [Obsolete]
    public class PostgresProcessorFactory : MigrationProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        [Obsolete]
        public PostgresProcessorFactory()
            : this(serviceProvider: null)
        {
        }

        public PostgresProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [Obsolete]
        public override IMigrationProcessor Create(string connectionString, IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var optionsParsed = PostgresOptions.ParseProviderSwitches(options.ProviderSwitches);
            var quoterOptions = new OptionsWrapper<QuoterOptions>(new QuoterOptions());
            var factory = new PostgresDbFactory(_serviceProvider);
            var connection = factory.CreateConnection(connectionString);
            return new PostgresProcessor(connection, new PostgresGenerator(new PostgresQuoter(quoterOptions, optionsParsed)), announcer, options, factory, optionsParsed);
        }
    }
}
