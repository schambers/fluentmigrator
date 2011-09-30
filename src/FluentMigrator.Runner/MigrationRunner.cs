#region License
// 
// Copyright (c) 2007-2009, Sean Chambers <schambers80@gmail.com>
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
using System.Linq;
using System.Reflection;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Versioning;
using FluentMigrator.VersionTableInfo;

namespace FluentMigrator.Runner
{
    public class MigrationRunner : IMigrationRunner
    {
        private readonly Assembly _migrationAssembly;
        private readonly IAnnouncer _announcer;
        private readonly IStopWatch _stopWatch;
        private bool _alreadyOutputPreviewOnlyModeWarning;
        public bool SilentlyFail { get; set; }

        public IMigrationProcessor Processor { get; private set; }
        public IMigrationLoader MigrationLoader { get; set; }
        public IProfileLoader ProfileLoader { get; set; }
        public IMigrationConventions Conventions { get; private set; }
        public IList<Exception> CaughtExceptions { get; private set; }

        public MigrationRunner(Assembly assembly, IRunnerContext runnerContext, IMigrationProcessor processor)
        {
            _migrationAssembly = assembly;
            _announcer = runnerContext.Announcer;
            Processor = processor;
            _stopWatch = runnerContext.StopWatch;

            SilentlyFail = false;
            CaughtExceptions = null;

            Conventions = new MigrationConventions();
            if (!string.IsNullOrEmpty(runnerContext.WorkingDirectory))
                Conventions.GetWorkingDirectory = () => runnerContext.WorkingDirectory;

            VersionLoader = new VersionLoader(this, _migrationAssembly, Conventions);
            MigrationLoader = new MigrationLoader(Conventions, _migrationAssembly, runnerContext.Namespace);
            ProfileLoader = new ProfileLoader(runnerContext, this, Conventions);
        }

        public IVersionLoader VersionLoader { get; set; }

        public void ApplyProfiles()
        {
            ProfileLoader.ApplyProfiles();
        }

        public void MigrateUp()
        {
            MigrateUp(true);
        }

        public void MigrateUp(bool useAutomaticTransactionManagement)
        {
            try
            {
                foreach (var version in MigrationLoader.Migrations.Keys)
                {
                    ApplyMigrationUp(version);
                }

                ApplyProfiles();

                if (useAutomaticTransactionManagement) { Processor.CommitTransaction(); }
                VersionLoader.LoadVersionInfo();
            }
            catch (Exception)
            {
                if (useAutomaticTransactionManagement) { Processor.RollbackTransaction(); }
                throw;
            }
        }

        public void MigrateUp(long targetVersion)
        {
            MigrateUp(targetVersion, true);
        }

        public void MigrateUp(long targetVersion, bool useAutomaticTransactionManagement)
        {
            try
            {
                foreach (var neededMigrationVersion in GetUpMigrationsToApply(targetVersion))
                {
                    ApplyMigrationUp(neededMigrationVersion);
                }
                if (useAutomaticTransactionManagement) { Processor.CommitTransaction(); }
                VersionLoader.LoadVersionInfo();
            }
            catch (Exception)
            {
                if (useAutomaticTransactionManagement) { Processor.RollbackTransaction(); }
                throw;
            }
        }

        private IEnumerable<long> GetUpMigrationsToApply(long version)
        {
            return MigrationLoader.Migrations.Keys.Where(x => IsMigrationStepNeededForUpMigration(x, version));
        }


        private bool IsMigrationStepNeededForUpMigration(long versionOfMigration, long targetVersion)
        {
            if (versionOfMigration <= targetVersion && !VersionLoader.VersionInfo.HasAppliedMigration(versionOfMigration))
            {
                return true;
            }
            return false;

        }

        public void MigrateDown(long targetVersion)
        {
            MigrateDown(targetVersion, true);
        }

        public void MigrateDown(long targetVersion, bool useAutomaticTransactionManagement)
        {
            try
            {
                foreach (var neededMigrationVersion in GetDownMigrationsToApply(targetVersion))
                {
                    ApplyMigrationDown(neededMigrationVersion);
                }

                if (useAutomaticTransactionManagement) { Processor.CommitTransaction(); }
                VersionLoader.LoadVersionInfo();
            }
            catch (Exception)
            {
                if (useAutomaticTransactionManagement) { Processor.RollbackTransaction(); }
                throw;
            }
        }

        private IEnumerable<long> GetDownMigrationsToApply(long targetVersion)
        {
            return MigrationLoader.Migrations.Keys.Where(x => IsMigrationStepNeededForDownMigration(x, targetVersion));
        }


        private bool IsMigrationStepNeededForDownMigration(long versionOfMigration, long targetVersion)
        {
            if (versionOfMigration > targetVersion && VersionLoader.VersionInfo.HasAppliedMigration(versionOfMigration))
            {
                return true;
            }
            return false;

        }

        private void ApplyMigrationUp(long version)
        {
            if (!_alreadyOutputPreviewOnlyModeWarning && Processor.Options.PreviewOnly)
            {
                _announcer.Heading("PREVIEW-ONLY MODE");
                _alreadyOutputPreviewOnlyModeWarning = true;
            }

            if (!VersionLoader.VersionInfo.HasAppliedMigration(version))
            {
                Up(MigrationLoader.Migrations[version]);
                VersionLoader.UpdateVersionInfo(version);
            }
        }

        private void ApplyMigrationDown(long version)
        {
            try
            {
                Down(MigrationLoader.Migrations[version]);
                VersionLoader.DeleteVersion(version);
            }
            catch (KeyNotFoundException ex)
            {
                string msg = string.Format("VersionInfo references version {0} but no Migrator was found attributed with that version.", version);
                throw new Exception(msg, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error rolling back version " + version, ex);
            }
        }

        public void Rollback(int steps)
        {
            Rollback(steps, true);
        }

        public void Rollback(int steps, bool useAutomaticTransactionManagement)
        {
            try
            {
                foreach (var migrationNumber in VersionLoader.VersionInfo.AppliedMigrations().Take(steps))
                {
                    ApplyMigrationDown(migrationNumber);
                }

                VersionLoader.LoadVersionInfo();

                if (!VersionLoader.VersionInfo.AppliedMigrations().Any())
                    VersionLoader.RemoveVersionTable();

                if (useAutomaticTransactionManagement) { Processor.CommitTransaction(); }
            }
            catch (Exception)
            {
                if (useAutomaticTransactionManagement) { Processor.RollbackTransaction(); }
                throw;
            }
        }

        public void RollbackToVersion(long version)
        {
            RollbackToVersion(version, true);
        }

        public void RollbackToVersion(long version, bool useAutomaticTransactionManagement)
        {
            //TODO: Extract VersionsToApply Strategy
            try
            {
                // Get the migrations between current and the to version
                foreach (var migrationNumber in VersionLoader.VersionInfo.AppliedMigrations())
                {
                    if (version < migrationNumber || version == 0)
                    {
                        ApplyMigrationDown(migrationNumber);
                    }
                }

                if (version == 0)
                    VersionLoader.RemoveVersionTable();
                else
                    VersionLoader.LoadVersionInfo();

                if (useAutomaticTransactionManagement) { Processor.CommitTransaction(); }
            }
            catch (Exception)
            {
                if (useAutomaticTransactionManagement) { Processor.RollbackTransaction(); }
                throw;
            }
        }

        public Assembly MigrationAssembly
        {
            get { return _migrationAssembly; }
        }

        public void Up(IMigration migration)
        {
            if (migration == null) throw new ArgumentNullException("migration");

            var migrationName = GetMigrationName(migration);
            var migrationVersion = GetMigrationVersion(migration);

            _announcer.Heading(string.Format("{0} {1}: migrating", migrationVersion, migrationName));

            CaughtExceptions = new List<Exception>();

            var context = new MigrationContext(Conventions, Processor, MigrationAssembly);
            migration.GetUpExpressions(context);

            _stopWatch.Start();
            ExecuteExpressions(context.Expressions);
            _stopWatch.Stop();

            _announcer.Say(string.Format("{0} {1}: migrated", migrationVersion, migrationName));
            _announcer.ElapsedTime(_stopWatch.ElapsedTime());
        }

        public void Down(IMigration migration)
        {
            if (migration == null) throw new ArgumentNullException("migration");

            var migrationName = GetMigrationName(migration);
            var migrationVersion = GetMigrationVersion(migration);

            _announcer.Heading(string.Format("{0} {1}: reverting", migrationVersion, migrationName));

            CaughtExceptions = new List<Exception>();

            var context = new MigrationContext(Conventions, Processor, MigrationAssembly);
            migration.GetDownExpressions(context);

            _stopWatch.Start();
            ExecuteExpressions(context.Expressions);
            _stopWatch.Stop();

            _announcer.Say(string.Format("{0} {1}: reverted", migrationVersion, migrationName));
            _announcer.ElapsedTime(_stopWatch.ElapsedTime());
        }

        private static string GetMigrationName(IMigration migration)
        {
            if (migration == null) throw new ArgumentNullException("migration");

            var migrationName = migration.GetType().Name;
            return migrationName;
        }

        private static string GetMigrationVersion(IMigration migration)
        {
            if (migration == null) throw new ArgumentNullException("migration");

            var migrationType = migration.GetType();

            if (migrationType == typeof(VersionSchemaMigration) || migrationType == typeof(VersionMigration))
                return string.Empty;

            var attributes = migrationType.GetCustomAttributes(false); //VersionTableMetaData
            if (attributes.Any(x => x.GetType() == typeof(ProfileAttribute) || x.GetType() == typeof(VersionTableMetaDataAttribute)))
                return string.Empty;

            var migrationAttribute = attributes
                .Where(x => x.GetType() == typeof (MigrationAttribute))
                .FirstOrDefault() as MigrationAttribute;
            if (migrationAttribute == null)
                throw new InvalidOperationException(string.Format("Migration should have attribute {0}", typeof(MigrationAttribute).Name));

            var migrationVersion = migrationAttribute.Version;
            return migrationVersion.ToString();
        }

        /// <summary>
        /// execute each migration expression in the expression collection
        /// </summary>
        /// <param name="expressions"></param>
        protected void ExecuteExpressions(ICollection<IMigrationExpression> expressions)
        {
            long insertTicks = 0;
            int insertCount = 0;
            foreach (IMigrationExpression expression in expressions)
            {
                try
                {
                    expression.ApplyConventions(Conventions);
                    if (expression is InsertDataExpression)
                    {
                        insertTicks += Time(() => expression.ExecuteWith(Processor));
                        insertCount++;
                    }
                    else
                    {
                        AnnounceTime(expression.ToString(), () => expression.ExecuteWith(Processor));
                    }
                }
                catch (Exception er)
                {
                    _announcer.Error(er.Message);

                    //catch the error and move onto the next expression
                    if (SilentlyFail)
                    {
                        CaughtExceptions.Add(er);
                        continue;
                    }
                    throw;
                }
            }

            if (insertCount > 0)
            {
                var avg = new TimeSpan(insertTicks / insertCount);
                var msg = string.Format("-> {0} Insert operations completed in {1} taking an average of {2}", insertCount, new TimeSpan(insertTicks), avg);
                _announcer.Say(msg);
            }
        }

        private void AnnounceTime(string message, Action action)
        {
            _announcer.Say(message);

            _stopWatch.Start();
            action();
            _stopWatch.Stop();

            _announcer.ElapsedTime(_stopWatch.ElapsedTime());
        }

        private long Time(Action action)
        {
            _stopWatch.Start();

            action();

            _stopWatch.Stop();

            return _stopWatch.ElapsedTime().Ticks;
        }
    }
}