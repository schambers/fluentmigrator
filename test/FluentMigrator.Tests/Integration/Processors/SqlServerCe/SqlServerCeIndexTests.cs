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
using System.Data.SqlServerCe;
using System.IO;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.SqlServer;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.SqlServerCe
{
    [TestFixture]
    [Category("Integration")]
    [Category("SqlServerCe")]
    public class SqlServerCeIndexTests : BaseIndexTests
    {
        private string _tempDataDirectory;

        private string DatabaseFilename { get; set; }
        private ServiceProvider ServiceProvider { get; set; }
        private IServiceScope ServiceScope { get; set; }
        private SqlServerCeProcessor Processor { get; set; }
        private QuoterOptions QuoterOptions { get; set; }

        [Test]
        public override void CallingIndexExistsCanAcceptIndexNameWithSingleQuote()
        {
            using (var table = new SqlServerCeTestTable(Processor, QuoterOptions, "id int"))
            {
                table.WithIndexOn("id", "UI'id");
                Processor.IndexExists("NOTUSED", table.Name, "UI'id").ShouldBeTrue();
            }
        }

        [Test]
        public override void CallingIndexExistsCanAcceptTableNameWithSingleQuote()
        {
            using (var table = new SqlServerCeTestTable("Test'Table", Processor, QuoterOptions, "id int"))
            {
                table.WithIndexOn("id");
                Processor.IndexExists("NOTUSED", table.Name, "UI_id").ShouldBeTrue();
            }
        }

        [Test]
        public override void CallingIndexExistsReturnsFalseIfIndexDoesNotExist()
        {
            using (var table = new SqlServerCeTestTable(Processor, QuoterOptions, "id int"))
            {
                table.WithIndexOn("id");
                Processor.IndexExists(null, table.Name, "DoesNotExist").ShouldBeFalse();
            }
        }

        [Test]
        public override void CallingIndexExistsReturnsFalseIfIndexDoesNotExistWithSchema()
        {
            using (var table = new SqlServerCeTestTable(Processor, QuoterOptions, "id int"))
            {
                table.WithIndexOn("id");
                Processor.IndexExists("NOTUSED", table.Name, "DoesNotExist").ShouldBeFalse();
            }
        }

        [Test]
        public override void CallingIndexExistsReturnsFalseIfTableDoesNotExist()
        {
            Processor.IndexExists(null, "DoesNotExist", "DoesNotExist").ShouldBeFalse();
        }

        [Test]
        public override void CallingIndexExistsReturnsFalseIfTableDoesNotExistWithSchema()
        {
            Processor.IndexExists("NOTUSED", "DoesNotExist", "DoesNotExist").ShouldBeFalse();
        }


        [Test]
        public override void CallingIndexExistsReturnsTrueIfIndexExists()
        {
            using (var table = new SqlServerCeTestTable(Processor, QuoterOptions, "id int"))
            {
                table.WithIndexOn("id");
                Processor.IndexExists(null, table.Name, "UI_id").ShouldBeTrue();
            }
        }

        [Test]
        public override void CallingIndexExistsReturnsTrueIfIndexExistsWithSchema()
        {
            using (var table = new SqlServerCeTestTable(Processor, QuoterOptions, "id int"))
            {
                table.WithIndexOn("id");
                Processor.IndexExists("NOTUSED", table.Name, "UI_id").ShouldBeTrue();
            }
        }

        [OneTimeSetUp]
        public void ClassSetUp()
        {
            if (!IntegrationTestOptions.SqlServerCe.IsEnabled)
                Assert.Ignore();

            if (!HostUtilities.ProbeSqlServerCeBehavior())
            {
                Assert.Ignore("SQL Server CE binaries not found");
            }

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(r => r.AddSqlServerCe())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.SqlServerCe.ConnectionString));
            ServiceProvider = serivces.BuildServiceProvider();
        }

        [OneTimeTearDown]
        public void ClassTearDown()
        {
            ServiceProvider?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            _tempDataDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDataDirectory);
            AppDomain.CurrentDomain.SetData("DataDirectory", _tempDataDirectory);

            var csb = new SqlCeConnectionStringBuilder(IntegrationTestOptions.SqlServerCe.ConnectionString);
            DatabaseFilename = HostUtilities.ReplaceDataDirectory(csb.DataSource);
            RecreateDatabase();

            ServiceScope = ServiceProvider.CreateScope();
            Processor = ServiceScope.ServiceProvider.GetRequiredService<SqlServerCeProcessor>();
            QuoterOptions = ServiceScope.ServiceProvider.GetRequiredService<IOptions<QuoterOptions>>().Value;
        }

        [TearDown]
        public void TearDown()
        {
            ServiceScope?.Dispose();

            if (!string.IsNullOrEmpty(_tempDataDirectory) && Directory.Exists(_tempDataDirectory))
            {
                Directory.Delete(_tempDataDirectory, true);
            }
        }

        private void RecreateDatabase()
        {
            if (File.Exists(DatabaseFilename))
            {
                File.Delete(DatabaseFilename);
            }

            new SqlCeEngine(IntegrationTestOptions.SqlServerCe.ConnectionString).CreateDatabase();
        }
    }
}
