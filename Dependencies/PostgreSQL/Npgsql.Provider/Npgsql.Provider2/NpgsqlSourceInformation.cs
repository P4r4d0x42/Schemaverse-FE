//****************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//****************************************************************************

using System;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;

namespace Npgsql.Provider
{
    /// <summary>
    /// Represents a custom data source information class that is able to
    /// provide data source information values that require some form of
    /// computation, perhaps based on an active connection.
    /// </summary>
    internal class NpgsqlSourceInformation : AdoDotNetSourceInformation
    {
        #region Constructors

        public NpgsqlSourceInformation()
        {
            AddProperty( DefaultSchema );
            AddProperty( QuotedIdentifierPartsStorageCase, "M" );
            AddProperty( IdentifierPartsStorageCase, "M" );
            AddProperty( IdentifierPartsCaseSensitive, true );
            AddProperty( SupportsQuotedIdentifierParts, true );
            AddProperty( QuotedIdentifierPartsCaseSensitive, true );
            AddProperty( ColumnSupported, true );
            AddProperty( SupportsVerifySql, true );
            AddProperty( "ProtocolVersion", ProtocolVersion.Version3 );
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// RetrieveValue is called once per property that was identified
        /// as existing but without a value (specified in the constructor).
        /// </summary>
        protected override object RetrieveValue( string propertyName )
        {
            if ( propertyName.Equals( DefaultSchema, StringComparison.OrdinalIgnoreCase ) )
            {
                if ( Site.State != DataConnectionState.Open )
                {
                    Site.Open();
                }
                DbConnection conn = Connection;
                Debug.Assert( conn != null, "Invalid provider object." );
                if ( conn != null )
                {
                    DbCommand comm = conn.CreateCommand();
                    try
                    {
                        comm.CommandText = "SELECT current_schema";
                        return comm.ExecuteScalar() as string;
                    }
                    catch ( NpgsqlException )
                    {
                        // We let the base class apply default behavior
                    }
                }
            }
            else if ( propertyName.Equals( "ReservedWords", StringComparison.OrdinalIgnoreCase ) )
            {
                //Npgsql does not support it yet.
                if ( Site.State != DataConnectionState.Open )
                {
                    Site.Open();
                }
                DbConnection conn = Connection;
                Debug.Assert( conn != null, "Invalid provider object." );
                if ( conn != null )
                {
                    DbCommand comm = conn.CreateCommand();
                    DbDataReader reader = null;
                    try
                    {
                        comm.CommandText = "SELECT array_to_string( ARRAY( SELECT UPPER(word) AS word FROM pg_get_keywords() ), ',')";
                        var sb = new StringBuilder( comm.ExecuteScalar() as string );
                        comm.CommandText = "SELECT DISTINCT UPPER(c.column_name)::varchar as column_name FROM information_schema.columns c WHERE c.table_schema != 'pg_catalog' AND c.table_schema != 'information_schema' ORDER BY column_name";
                        reader = comm.ExecuteReader();

                        while ( reader.Read() )
                        {
                            sb.Append( "," );
                            sb.Append( reader.GetString( 0 ) );
                        }

                        return sb.ToString();
                    }
                    catch ( NpgsqlException )
                    {
                        return null;
                    }
                    finally
                    {
                        if ( reader != null && !reader.IsClosed )
                        {
                            reader.Close();
                        }
                    }
                }

                return null;
            }
            else if ( propertyName.Equals( "DataTypes", StringComparison.OrdinalIgnoreCase ) )
            {
                return ( Connection as NpgsqlConnection ).GetSchema( propertyName );
            }
            else if ( propertyName.Equals( "DataSourceVersion", StringComparison.OrdinalIgnoreCase ) )
            {
                return "2.0.11.91";
            }
            else if ( propertyName.Equals( "ProtocolVersion", StringComparison.OrdinalIgnoreCase ) )
            {
                return ProtocolVersion.Version3;
            }


            return base.RetrieveValue( propertyName );
        }

        #endregion
    }
}
