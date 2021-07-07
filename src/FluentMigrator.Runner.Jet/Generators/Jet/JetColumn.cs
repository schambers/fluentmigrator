using System;
using System.Data;

using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;

namespace FluentMigrator.Runner.Generators.Jet
{
    internal class JetColumn : ColumnBase
    {
        public JetColumn(JetQuoter quoter)
            : base(new JetTypeMap(), quoter)
        {

        }

        /// <inheritdoc />
        protected override string FormatType(ColumnDefinition column)
        {
            if (column.IsIdentity)
            {
                // In Jet an identity column always of type COUNTER which is a integer type
                if (column.Type != DbType.Int32)
                {
                    throw new ArgumentException("Jet Engine only allows identity columns on integer columns");
                }

                return "COUNTER";
            }
            return base.FormatType(column);
        }

        /// <inheritdoc />
        protected override string FormatIdentity(ColumnDefinition column)
        {
            //Identity type is handled by FormatType
            return string.Empty;
        }
    }
}
