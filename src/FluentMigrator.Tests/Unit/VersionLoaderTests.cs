using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using FluentMigrator.VersionTableInfo;
using Moq;
using NUnit.Framework;
using NUnit.Should;

namespace FluentMigrator.Tests.Unit
{
    public class TestMigrationProcessorOptions : IMigrationProcessorOptions
    {
        public bool PreviewOnly
        {
            get { return false; }
        }

        public int Timeout
        {
            get { return 30; }
        }
    }

    [TestFixture]
    public class VersionLoaderTests
    {
        [Test]
        public void CanLoadCustomVersionTableMetaData()
        {
            var runner = new Mock<IMigrationRunner>();
            runner.SetupGet(r => r.Processor.Options).Returns(new TestMigrationProcessorOptions());

            var conventions = new MigrationConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new VersionLoader(runner.Object, asm, conventions);

            var versionTableMetaData = loader.GetVersionTableMetaData();
            versionTableMetaData.ShouldBeOfType<TestVersionTableMetaData>();
        }

        [Test]
        public void CanLoadDefaultVersionTableMetaData()
        {
            var runner = new Mock<IMigrationRunner>();
            runner.SetupGet(r => r.Processor.Options).Returns(new TestMigrationProcessorOptions());

            var conventions = new MigrationConventions();
            var asm = "s".GetType().Assembly;
            var loader = new VersionLoader(runner.Object, asm, conventions);

            var versionTableMetaData = loader.GetVersionTableMetaData();
            versionTableMetaData.ShouldBeOfType<DefaultVersionTableMetaData>();
        }

        [Test]
        public void DeleteVersionShouldExecuteDeleteDataExpression()
        {
            var processor = new Mock<IMigrationProcessor>();
            var runner = new Mock<IMigrationRunner>();

            runner.SetupGet(r => r.Processor).Returns(processor.Object);

            var conventions = new MigrationConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new VersionLoader(runner.Object, asm, conventions);

            Func<IDataDefinition, bool> predicate = 
                definition =>
                {
                    if (definition is ExplicitDataDefinition)
                    {
                        IDataValue kvp = ((ExplicitDataDefinition)definition).Data.First();

                        return kvp.ColumnName == loader.VersionTableMetaData.ColumnName && kvp.Value.Equals(1L);
                    }
                    else
                    {
                        return false;
                    }
                };

            processor.Setup(p => p.Process(It.Is<DeleteDataExpression>(expression =>
                                                                       expression.SchemaName == loader.VersionTableMetaData.SchemaName
                                                                       && expression.TableName == loader.VersionTableMetaData.TableName
                                                                       && expression.Rows.All(predicate)))).Verifiable();

            loader.DeleteVersion(1);

            processor.VerifyAll();
        }

        [Test]
        public void RemoveVersionTableShouldBehaveAsExpected()
        {
            var processor = new Mock<IMigrationProcessor>();
            var runner = new Mock<IMigrationRunner>();

            runner.SetupGet(r => r.Processor).Returns(processor.Object);

            var conventions = new MigrationConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new VersionLoader(runner.Object, asm, conventions);

            processor.Setup(p => p.Process(It.Is<DeleteTableExpression>(expression =>
                                                                        expression.SchemaName == loader.VersionTableMetaData.SchemaName
                                                                        && expression.TableName == loader.VersionTableMetaData.TableName)))
                .Verifiable();

            processor.Setup(p => p.Process(It.Is<DeleteSchemaExpression>(expression =>
                                                                         expression.SchemaName == loader.VersionTableMetaData.SchemaName)))
                .Verifiable();

            loader.RemoveVersionTable();

            processor.VerifyAll();
        }

        [Test]
        public void UpdateVersionShouldExecuteInsertDataExpression()
        {
            var processor = new Mock<IMigrationProcessor>();
            var runner = new Mock<IMigrationRunner>();

            runner.SetupGet(r => r.Processor).Returns(processor.Object);

            var conventions = new MigrationConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new VersionLoader(runner.Object, asm, conventions);

            Func<IDataDefinition, bool> predicate =
                definition =>
                {
                    if (definition is ExplicitDataDefinition)
                    {
                        IDataValue dataValue = ((ExplicitDataDefinition)definition).Data.First();

                        return dataValue.ColumnName == loader.VersionTableMetaData.ColumnName && dataValue.Value.Equals(1L);
                    }
                    else
                    {
                        return false;
                    }
                };

            processor.Setup(p => p.Process(It.Is<InsertDataExpression>(expression =>
                                                                       expression.SchemaName == loader.VersionTableMetaData.SchemaName
                                                                       && expression.TableName == loader.VersionTableMetaData.TableName
                                                                       && expression.Rows.Any(predicate))))
                .Verifiable();

            loader.UpdateVersionInfo(1);

            processor.VerifyAll();
        }

        [Test]
        public void VersionSchemaMigrationOnlyRunOnceEvenIfExistenceChecksReturnFalse()
        {
            var conventions = new MigrationConventions();
            var processor = new Mock<IMigrationProcessor>();
            var runner = new Mock<IMigrationRunner>();
            var asm = Assembly.GetExecutingAssembly();

            runner.SetupGet(r => r.Processor).Returns(processor.Object);

            processor.Setup(p => p.SchemaExists(It.IsAny<string>())).Returns(false);

            var loader = new VersionLoader(runner.Object, asm, conventions);

            loader.LoadVersionInfo();

            runner.Verify(r => r.Up(loader.VersionSchemaMigration), Times.Once());
        }

        [Test]
        public void VersionMigrationOnlyRunOnceEvenIfExistenceChecksReturnFalse()
        {
            var conventions = new MigrationConventions();
            var processor = new Mock<IMigrationProcessor>();
            var runner = new Mock<IMigrationRunner>();
            var asm = Assembly.GetExecutingAssembly();

            runner.SetupGet(r => r.Processor).Returns(processor.Object);

            processor.Setup(p => p.TableExists(new TestVersionTableMetaData().SchemaName, TestVersionTableMetaData.TABLENAME)).Returns(false);

            var loader = new VersionLoader(runner.Object, asm, conventions);

            loader.LoadVersionInfo();

            runner.Verify(r => r.Up(loader.VersionMigration), Times.Once());
        }

        [Test]
        public void VersionUniqueMigrationOnlyRunOnceEvenIfExistenceChecksReturnFalse()
        {
            var conventions = new MigrationConventions();
            var processor = new Mock<IMigrationProcessor>();
            var runner = new Mock<IMigrationRunner>();
            var asm = Assembly.GetExecutingAssembly();

            runner.SetupGet(r => r.Processor).Returns(processor.Object);

            processor.Setup(p => p.ColumnExists(new TestVersionTableMetaData().SchemaName, TestVersionTableMetaData.TABLENAME, "AppliedOn")).Returns(false);

            var loader = new VersionLoader(runner.Object, asm, conventions);

            loader.LoadVersionInfo();

            runner.Verify(r => r.Up(loader.VersionUniqueMigration), Times.Once());
        }
    }
}