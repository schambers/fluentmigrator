﻿#region License
// 
// Copyright (c) 2011, Grant Archibald
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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Oracle;
using FluentMigrator.Runner.Processors.SqlServer;
using FluentMigrator.SchemaDump.SchemaMigrations;
using NUnit.Framework;
using NUnit.Should;

namespace FluentMigrator.Tests.Integration.SchemaMigration
{
   [TestFixture]
   public class SqlServerToOracleMigrationTests
   {
      private SqlServerUnitTest _sqlContext;
      private OracleUnitTest _oracleContext;
      private string _tempDirectory;

      [TestFixtureSetUp]
      public void TestFixtureSetUp()
      {
         _sqlContext = new SqlServerUnitTest();
         _oracleContext = new OracleUnitTest();
         _sqlContext.TestFixtureSetUp();
         _oracleContext.TestFixtureSetUp();
      }

      [TestFixtureTearDown]
      public void TestFixtureTearDown()
      {
         _sqlContext.TestFixtureTearDown();
      }

      [SetUp]
      public void SetUp()
      {
         _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
         Directory.CreateDirectory(_tempDirectory);
         _sqlContext.Setup();
         _oracleContext.Setup();
      }


      [TearDown]
      public void TearDown()
      {
         _sqlContext.TearDown();
         Directory.Delete(_tempDirectory, true);
      }

      [Test]
      public void CanMigrateIdentityColumn()
      {
         // Arrange
         var create = new CreateTableExpression
                         {
                            TableName = "Foo",
                            Columns = new[]
                                         {
                                            new ColumnDefinition
                                               {
                                                  Name = "Id", Type = DbType.Int32
                                                  , IsIdentity = true
                                               }
                                         }
                         };
         


         // Act
         MigrateTable(GetDefaultContext(), create);
         
      }

      [Test]
      public void CanMigrateGuidAndDateWithDefaultValues()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]
                                         {
                                            new ColumnDefinition
                                               {
                                                  Name = "Id", Type = DbType.Guid
                                                  , DefaultValue = SystemMethods.NewGuid
                                               },
                                               new ColumnDefinition
                                               {
                                                  Name = "Added", Type = DbType.DateTime
                                                  , DefaultValue = SystemMethods.CurrentDateTime
                                               }
                                         }
         };



         // Act
         MigrateTable(GetDefaultContext(), create);

      }

      [Test]
      public void CanMigrateDefaultDate()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]
                                         {
                                               new ColumnDefinition
                                               {
                                                  Name = "Added", Type = DbType.DateTime
                                                  , DefaultValue = "1900-01-01"
                                               }
                                         }
         };

         // Act
         MigrateTable(GetDefaultContext(), create);

      }

      [Test]
      public void CanMigrateDecimal()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]
                                         {
                                               new ColumnDefinition
                                               {
                                                  Name = "Value", Type = DbType.Decimal
                                                  , Size = 38
                                                  , Precision = 4
                                               }
                                         }
         };

         // Act
         MigrateTable(GetDefaultContext(), create);

      }

      [Test]
      public void CanMigrateSupportedTypes()
      {
         // Arrange

         // Add in broad coverage of supported data types

         var create = new CreateTableExpression
         {
            TableName = "SupportedTypes",
            Columns = new[]{
                   new ColumnDefinition {Name = "Col1", Type = DbType.Int32 }
                   , new ColumnDefinition {Name = "Col2", Type = DbType.String, Size = 20}
                   , new ColumnDefinition {Name = "Col3", Type = DbType.String}
                   , new ColumnDefinition {Name = "Col4", Type = DbType.Guid }
                   , new ColumnDefinition {Name = "Col5", Type = DbType.Byte}
                   , new ColumnDefinition {Name = "Col6", Type = DbType.DateTime}
                   , new ColumnDefinition {Name = "Col7", Type = DbType.Int64}
                   , new ColumnDefinition {Name = "Col8", Type = DbType.Int16}
                   , new ColumnDefinition {Name = "Col9", Type = DbType.Decimal, Size = 20, Precision = 4}
                   , new ColumnDefinition {Name = "Col10", Type = DbType.Double}
                   , new ColumnDefinition {Name = "Col11", Type = DbType.Boolean}
                    }
         };



         // Act
         MigrateTable(GetDefaultContext(), create);

      }

      [Test]
      [ExpectedException]
      public void ThrowsExceptionIfInvalidCodeGenerated()
      {
         // Arrange

         // Add in broad coverage of supported data types

         var create = new CreateTableExpression
         {
            TableName = "SupportedTypes",
            Columns = new[]{
                   new ColumnDefinition {Name = "Col1", Type = DbType.Int32 }
                    }
         };

         var settings = GetDefaultContext();
         // Modify the context so that invalid class name returned
         // ... in this case make is start with numeric
         settings.MigrationClassNamer = (index, table) => "1";


         // Act
         MigrateTable(settings, create);

      }

      [Test]
      public void CanMigrateTableAndView()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Id", Type = DbType.Int32 }
                    }
         };

         var view = new ViewDefinition {Name = "FooView", CreateViewSql = "CREATE VIEW FooView AS SELECT * FROM Foo"};

         // Act
         MigrateToOracle(create, view, 2);
        

         // Assert
      }

      

      [Test]
      public void CanExcludeTable()
      {
         // Arrange

         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Id", Type = DbType.Int32 }
                    }
         };

         var context = GetDefaultContext();
         context.GenerateAlternateMigrationsFor.Add(DatabaseType.Oracle);
         context.ExcludeTables.Add("Foo");

         // Act
         MigrateToOracle(create, null, 0, c => c.ExcludeTables.Add("Foo"));
         

         // Assert
         
      }

      [Test]
      public void CanMigrateTableAndExcludeView()
      {
         // Arrange

         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Id", Type = DbType.Int32 }
                    }
         };

         var view = new ViewDefinition {Name = "FooView", CreateViewSql = "CREATE VIEW FooView AS SELECT * FROM Foo"}; 

         var context = GetDefaultContext();
         context.GenerateAlternateMigrationsFor.Add(DatabaseType.Oracle);
         context.ExcludeViews.Add("FooView");

         // Act
         MigrateToOracle(create, view, 1, c => c.ExcludeViews.Add("FooView"));

         // Assert
      }

      [Test]
      public void CanMigrateTableData()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Id", Type = DbType.Int32 }
                    }
         };

         var row = new InsertDataExpression
                      {
                         TableName = "Foo"
                      };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("Id", 1)});

         // Act
         var data = MigrateToOracleWithData(create, row, 1);


         // Assert
         data.Tables[0].Rows.Count.ShouldBe(1);
      }

      [Test]
      public void CanMigrateTableHexStringToRaw()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Data", Type = DbType.String, Size = 120}
                    }
         };

         var row = new InsertDataExpression
         {
            TableName = "Foo"
         };

         var testData = Encoding.ASCII.GetBytes("HELLO");
         var hexData = BitConverter.ToString(testData).Replace("-", "");

         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("Data", hexData) });

         // Act
         var data = MigrateToOracleWithData(create, row, 1,
            c => c.PreMigrationTableUpdate = (table) =>
                                                {
                                                   table[0].Columns.Where(column => column.Name == "Data").
                                                      FirstOrDefault().Type = DbType.Binary;
                                                });


         // Assert
         data.Tables[0].Rows.Count.ShouldBe(1);
         data.Tables[0].Rows[0]["Data"].ShouldBe(testData);
      }


      [Test]
      public void CanMigrateTableWithDate()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Test", Type = DbType.DateTime }
                    }
         };

         var row = new InsertDataExpression
                      {
            TableName = "Foo"
         };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("Test", DateTime.Now) });
        

         // Act
         var data = MigrateToOracleWithData(create, row, 1);

         // Assert
         data.Tables[0].Rows.Count.ShouldBe(1);
      }

      [Test]
      public void CanMigrateTableWithChar()
      {
         // Arrange

         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Test", Type = DbType.AnsiString, Size = 20}
                    }
         };

         var row = new InsertDataExpression
                      {
            TableName = "Foo"
         };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("Test", "12345678901234567890") });

         // Act
         var data = MigrateToOracleWithData(create, row, 1);

         // Assert
         data.Tables[0].Rows.Count.ShouldBe(1);
      }

      [Test]
      public void CanMigrateTableWithEmptyString()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Test", Type = DbType.AnsiString, Size = 20, IsNullable = true}
                    }
         };

         var row = new InsertDataExpression
                      {
            TableName = "Foo"
         };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("Test", string.Empty) });

         // Act
         var data = MigrateToOracleWithData(create, row, 1);

         // Assert
         data.Tables[0].Rows.Count.ShouldBe(1);
      }

      [Test]
      public void CanMigrateTableWithGuid()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Test", Type = DbType.Guid }
                    }
         };

         var row = new InsertDataExpression
                      {
            TableName = "Foo"
         };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("Test", Guid.NewGuid()) });

         // Act
         var data = MigrateToOracleWithData(create, row, 1);

         // Assert
         data.Tables[0].Rows.Count.ShouldBe(1);
      }

      [Test]
      public void CanMigrateStringWithNullvalue()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Test", Type = DbType.String, IsNullable = true}
                    }
         };

         var row = new InsertDataExpression
                      {
            TableName = "Foo"
         };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("Test", null) });

         // Act
         var data = MigrateToOracleWithData(create, row, 1);

         // Assert
         data.Tables[0].Rows.Count.ShouldBe(1);
      }

      [Test]
      public void CanMigrateAnsiStringToString()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Test", Type = DbType.AnsiString, IsNullable = true}
                    }
         };

         var row = new InsertDataExpression
                      {
            TableName = "Foo"
         };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("Test", "A Test") });
        
         // Act
         var data = MigrateToOracleWithData(create, row, 1, c => c.PreMigrationTableUpdate = tables => { foreach (var column in from table in tables
                                                                              from tableDefinition in tables
                                                                              from column in tableDefinition.Columns
                                                                              where column.Type == DbType.AnsiString
                                                                              select column)
         {
            column.Type = DbType.String;
         }});

         // Assert
         data.Tables[0].Rows.Count.ShouldBe(1);
      }

      [Test]
      public void CanMigrateDate()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Test", Type = DbType.Date, IsNullable = true, DefaultValue = "1900-01-01"}
                    }
         };


         // Act
         MigrateToOracle(create, null, 1);

         // Assert
         
      }

      [Test]
      public void CanMigrateDateTimeAndReplaceDefaultDate()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Test", Type = DbType.DateTime, IsNullable = true, DefaultValue = "1900-01-01"}
                    }
         };


         // Act
         MigrateToOracle(create, null, 1, c => c.PreMigrationTableUpdate = tables => tables[0].Columns.FirstOrDefault().DefaultValue = "'2000-01-01'");

         // Assert
      }

      [Test]
      public void CanMigrateDateTimeAndReplaceDateValue()
      {
         // Arrange
         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Test", Type = DbType.DateTime, IsNullable = true, DefaultValue = "1900-01-01"}
                    }
         };

         var oldVal = DateTime.ParseExact("1900-01-01", "yyyy-MM-dd", null);
         var newVal = DateTime.ParseExact("2000-01-01", "yyyy-MM-dd", null);

         var row = new InsertDataExpression { TableName = "Foo"};
         row.Rows.Add(new InsertionDataDefinition() { new KeyValuePair<string, object>("Test", oldVal) });

         // Act
         var data = MigrateToOracleWithData(create, row, 1, c => c.InsertColumnReplacements.Add(
            new InsertColumnReplacement
               {
                  ColumnDataToMatch = new ColumnDefinition {Type = DbType.DateTime, IsNullable = true}
                  , OldValue = oldVal
                  , NewValue = newVal
               }));

         // Assert
         data.Tables[0].Rows[0]["Test"].ShouldBe(newVal);
      }

      [Test]
      public void CanMigrateTableDataWithIdentity()
      {
         // Arrange

         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Id", Type = DbType.Int32, IsIdentity = true}
                    }
         };

         var row = new InsertDataExpression
                      {
            TableName = "Foo"
         };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("Id", 1) });
         row.WithIdentity = true;

         // Act
         var data = MigrateToOracleWithData(create, row, 1);

         // Assert
        
         data.Tables[0].Rows.Count.ShouldBe(1);
      }

      [Test]
      public void CanMigrateTableDataWithCustomIdentity()
      {
         // Arrange

         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "Id", Type = DbType.Int32, IsIdentity = true}
                    }
         };

         var row = new InsertDataExpression
         {
            TableName = "Foo"
         };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("Id", 1) });
         row.WithIdentity = true;

         // Act
         var data = MigrateToOracleWithData(create, row, 1, c => c.OracleSequenceNamer = table => "MyTest");

         // Assert

         data.Tables[0].Rows.Count.ShouldBe(1);
      }

      [Test]
      public void CanMigrateTableWithLowercaseColumnName()
      {
         // Arrange

         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "id", Type = DbType.Int32}
                    }
         };

         var row = new InsertDataExpression
         {
            TableName = "Foo"
         };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("id", 1) });

         // Act
         var data = MigrateToOracleWithData(create, row, 1
            , c => c.PreMigrationTableUpdate = tables => tables[0].Columns.FirstOrDefault().Name = "\\\"id\\\""
            , c => c.CaseSenstiveColumnNames = true
            , c => c.CaseSenstiveColumns.Add("id"));

         // Assert

         data.Tables[0].Columns[0].ColumnName.ShouldBe("id");
      }

      [Test]
      public void CanMigrateTableWithOneColumnNameCaseSensitive()
      {
         // Arrange

         var create = new CreateTableExpression
         {
            TableName = "Foo",
            Columns = new[]{
                   new ColumnDefinition {Name = "id", Type = DbType.Int32}
                   , new ColumnDefinition {Name = "Data", Type = DbType.Int32}
                    }
         };

         var row = new InsertDataExpression
         {
            TableName = "Foo"
         };
         row.Rows.Add(new InsertionDataDefinition { new KeyValuePair<string, object>("id", 1), new KeyValuePair<string, object>("Data", 1) });

         // Act
         var data = MigrateToOracleWithData(create, row, 1
            , c => c.PreMigrationTableUpdate = tables => tables[0].Columns.Where(column => column.Name.Equals("id")).FirstOrDefault().Name = "\\\"id\\\""
            , c => c.CaseSenstiveColumns.Add("id"));

         // Assert

         data.Tables[0].Columns[0].ColumnName.ShouldBe("id");
         data.Tables[0].Columns[1].ColumnName.ShouldBe("DATA");
      }

      private void InsertData(InsertDataExpression row)
      {
         using (var connection = new SqlConnection(_sqlContext.ConnectionString))
         {
            var processor = new SqlServerProcessor(connection, new SqlServer2005Generator(), new DebugAnnouncer(), new ProcessorOptions());

            processor.Process(row);

            processor.CommitTransaction();
         }
      }

      private void MigrateTable(SchemaMigrationContext context, params CreateTableExpression[] createTables)
      {
         CreateTables(createTables);

         var migrator = new SqlServerSchemaMigrator(new DebugAnnouncer());

         migrator.Migrate(context);

         AssertOracleTablesExist(createTables);
      }

      private void AssertOracleTablesExist(params CreateTableExpression[] createTables)
      {
         if (createTables == null)
            return;
         var oracleProcessor = new OracleProcessorFactory().Create(_oracleContext.ConnectionString, new NullAnnouncer(),
                                                                   new ProcessorOptions());

         foreach ( var create in createTables)
            Assert.IsTrue(oracleProcessor.TableExists(string.Empty, create.TableName), "Oracle");   
      }

      private void AssertOracleTablesDoNotExist(params CreateTableExpression[] createTables)
      {
         if (createTables == null)
            return;
         var oracleProcessor = new OracleProcessorFactory().Create(_oracleContext.ConnectionString, new NullAnnouncer(),
                                                                   new ProcessorOptions());

         foreach (var create in createTables)
            Assert.IsFalse(oracleProcessor.TableExists(string.Empty, create.TableName), "Oracle");
      }

      private DataSet GetOracleTableData(string tableName)
      {
         var oracleProcessor = new OracleProcessorFactory().Create(_oracleContext.ConnectionString, new NullAnnouncer(),
                                                                   new ProcessorOptions());

         return oracleProcessor.ReadTableData(string.Empty, tableName);
      }

      private void CreateTables(params CreateTableExpression[] createTables)
      {
         if (createTables == null)
            return;

         using (var connection = new SqlConnection(_sqlContext.ConnectionString))
         {
            var processor = new SqlServerProcessor(connection, new SqlServer2005Generator(), new DebugAnnouncer(), new ProcessorOptions());

            foreach (var create in createTables)
            {
               processor.Process(create);
               Assert.IsTrue(processor.TableExists(string.Empty, create.TableName), "SqlServer " + create.TableName);
            }

            processor.CommitTransaction();
         }
      }

      private void CreateViews(params ViewDefinition[] view)
      {
         using (var connection = new SqlConnection(_sqlContext.ConnectionString))
         {
            var processor = new SqlServerProcessor(connection, new SqlServer2005Generator(), new DebugAnnouncer(), new ProcessorOptions());

            foreach (var viewDefinition in view)
            {
               processor.Execute(viewDefinition.CreateViewSql);
            }

            processor.CommitTransaction();
         }
      }

      private SchemaMigrationContext GetDefaultContext()
      {
         return new SchemaMigrationContext
                   {
                      FromConnectionString = _sqlContext.ConnectionString
                      , ToDatabaseType = DatabaseType.Oracle
                      , ToConnectionString = _oracleContext.ConnectionString
                      , WorkingDirectory = _tempDirectory
                   };
      }

      private void MigrateToOracle(CreateTableExpression create, ViewDefinition view, int expectedMigrations, params Action<SchemaMigrationContext>[] contextAction)
      {
         var context = GetDefaultContext();
         context.GenerateAlternateMigrationsFor.Add(DatabaseType.Oracle);

         if (contextAction != null)
         {
            foreach (var action in contextAction)
            {
               action(context);
            }
         }

         CreateTables(create);

         if (view != null)
            CreateViews(view);

         MigrateTable(context);

         if ( context.ExcludeTables.Contains(create.TableName))
            AssertOracleTablesDoNotExist(create);
         else
         {
            AssertOracleTablesExist(create);
         }

         context.MigrationIndex.ShouldBe(expectedMigrations);
      }

      private DataSet MigrateToOracleWithData(CreateTableExpression create, InsertDataExpression row, int expectedMigrations, params Action<SchemaMigrationContext>[] contextAction)
      {
         var context = GetDefaultContext();
         context.GenerateAlternateMigrationsFor.Add(DatabaseType.Oracle);
         context.MigrateData = true;

         if (contextAction != null)
         {
            foreach (var action in contextAction)
            {
               action(context);
            }
         }

         CreateTables(create);

         InsertData(row);

         MigrateTable(context);

         AssertOracleTablesExist(create);

         context.MigrationIndex.ShouldBe(expectedMigrations);

         return GetOracleTableData(create.TableName);
      }
   }
}
