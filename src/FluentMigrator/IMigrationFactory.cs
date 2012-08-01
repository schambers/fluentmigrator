namespace FluentMigrator
{
    /// <summary>Constructs implementations of FluentMigrator interfaces.</summary>
    public interface IMigrationFactory
    {
        /// <summary>Get the object which defines default rules for migration mappings.</summary>
        /// <param name="applicationContext">The arbitrary application context passed to the task runner.</param>
        IMigrationConventions GetMigrationConventions(object applicationContext);
    }
}