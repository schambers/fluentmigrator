﻿using System.Data;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.Firebird;
using FluentMigrator.Runner.Processors.Firebird;

namespace FluentMigrator.Tests.Integration.Processors.Firebird
{
    public class FirebirdTestProcessorFactory : AbstractTestProcessorFactoryOf<FirebirdProcessor>, TestCleaner
    {
        private readonly FbConnectionStringBuilder _connectionString;

        public FirebirdTestProcessorFactory(string connectionString)
        {
            _connectionString = new FbConnectionStringBuilder(connectionString);
            if (string.IsNullOrEmpty(Path.GetDirectoryName(_connectionString.Database)))
                _connectionString.Database = Path.Combine(Directory.GetCurrentDirectory(), _connectionString.Database);
        }

        public override IMigrationProcessor MakeProcessor(IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var fbOptions = FirebirdOptions.AutoCommitBehaviour();
            var connection = MakeConnection();
            return new FirebirdProcessor(connection, new FirebirdGenerator(fbOptions), announcer, options, new FirebirdDbFactory(), fbOptions);
        }

        public override string ConnectionString
        {
            get { return _connectionString.ToString(); }
        }

        public void CleanUp()
        {
            FbDatabase.DropDatabase(_connectionString.ToString());
        }

        private IDbConnection MakeConnection()
        {
            var usedConnectionString = _connectionString.ToString();
            FbDatabase.CreateDatabase(usedConnectionString);

            return new FbConnection(usedConnectionString);
        }
    }
}
