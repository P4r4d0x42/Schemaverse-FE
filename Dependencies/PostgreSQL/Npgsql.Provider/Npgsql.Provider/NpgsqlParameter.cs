using System.Data.Common;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;

namespace Npgsql.Provider
{
    internal class NpgsqlParameter : AdoDotNetParameter
    {
        private global::Npgsql.NpgsqlParameter _parameter;

        public NpgsqlParameter()
            : base( "Npgsql" )
        {
        }

        public NpgsqlParameter( global::Npgsql.NpgsqlParameter parameter )
            : base( parameter, true )
        {
        }

        #region Implementation of IVsDataParameter

        public NpgsqlParameter( string providerInvariantName )
            : base( providerInvariantName )
        {
        }

        public NpgsqlParameter( string providerInvariantName, bool isDerived )
            : base( providerInvariantName, isDerived )
        {
        }

        public NpgsqlParameter( DbParameter parameter )
            : base( parameter )
        {
        }

        public NpgsqlParameter( DbParameter parameter, bool isDerived )
            : base( parameter, isDerived )
        {
        }

        #endregion

        // Properties
        public new global::Npgsql.NpgsqlParameter Parameter
        {
            get
            {
                return ( base.Parameter as global::Npgsql.NpgsqlParameter );
            }
        }

    }
}
