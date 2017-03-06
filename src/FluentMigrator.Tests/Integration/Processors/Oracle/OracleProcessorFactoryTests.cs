﻿
using FluentMigrator.Runner.Processors.Oracle;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.Oracle
{
    [TestFixture]
    [Category("Integration")]
    public class OracleProcessorFactoryTests : OracleProcessorFactoryTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUp(new OracleProcessorFactory());
        }
    }
}