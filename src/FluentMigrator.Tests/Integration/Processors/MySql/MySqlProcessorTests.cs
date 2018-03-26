using System.IO;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Generators.MySql;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.MySql;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using NUnit.Should;

namespace FluentMigrator.Tests.Integration.Processors.MySql
{
    [TestFixture]
    [Category("Integration")]
    public class MySqlProcessorTests : IntegrationTestBase
    {
        protected MySqlProcessor Processor;

        [SetUp]
        public void SetUp()
        {
            if (ConfiguredProcessor.IsAssignableFrom(typeof(MySqlProcessor)))
            {
                Processor = CreateProcessor() as MySqlProcessor;
            }
            else
                Assert.Ignore("Test is intended to run against MySql. Current configuration: {0}", ConfiguredDbEngine);
        }

        [TearDown]
        public void TearDown()
        {
            if (ConfiguredProcessor.IsAssignableFrom(typeof(MySqlProcessor)))
                Processor.Dispose();
        }

        [Test]
        public void CallingProcessWithPerformDBOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges()
        {
            var output = new StringWriter();

            var connection = new MySqlConnection(ConnectionString);

            var processor = SetupMySqlProcessorWithPreviewOnly(output, connection);

            bool tableExists;

            try
            {
                var expression =
                    new PerformDBOperationExpression
                    {
                        Operation = (con, trans) =>
                        {
                            var command = con.CreateCommand();
                            command.CommandText = "CREATE TABLE processtesttable (test int NULL) ";
                            command.Transaction = trans;

                            command.ExecuteNonQuery();
                        }
                    };

                processor.Process(expression);

                var com = connection.CreateCommand();
                com.CommandText = "";

                tableExists = processor.TableExists("", "processtesttable");
            }
            finally
            {
                processor.RollbackTransaction();
            }

            tableExists.ShouldBeFalse();
        }

        [Test]
        public void CallingExecuteWithPerformDBOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges()
        {
            var output = new StringWriter();

            var connection = new MySqlConnection(ConnectionString);

            var processor = SetupMySqlProcessorWithPreviewOnly(output, connection);

            bool tableExists;

            try
            {
                processor.Execute("CREATE TABLE processtesttable (test int NULL) ");

                tableExists = processor.TableExists("", "processtesttable");
            }
            finally
            {
                processor.RollbackTransaction();
            }

            tableExists.ShouldBeFalse();
        }

        [Test]
        public void CallingDefaultValueExistsReturnsTrueWhenMatches()
        {
            try
            {
                Processor.Execute("CREATE TABLE dftesttable (test int NULL DEFAULT 1) ");
                Processor.DefaultValueExists(null, "dftesttable", "test", 1).ShouldBeTrue();
            }
            finally
            {
                Processor.Execute("DROP TABLE dftesttable");
            }
        }

        [Test]
        public void CallingReadTableDataQuotesTableName()
        {
            try
            {
                Processor.Execute("CREATE TABLE `infrastructure.version` (test int null) ");
                Processor.ReadTableData(null, "infrastructure.version");
            }
            finally
            {
                Processor.Execute("DROP TABLE `infrastructure.version`");
            }
        }

        private static MySqlProcessor SetupMySqlProcessorWithPreviewOnly(StringWriter output, MySqlConnection connection)
        {
            var processor = new MySqlProcessor(
                connection,
                new MySqlGenerator(),
                new TextWriterAnnouncer(output),
                new ProcessorOptions { PreviewOnly = true },
                new MySqlDbFactory());
            return processor;
        }
    }
}