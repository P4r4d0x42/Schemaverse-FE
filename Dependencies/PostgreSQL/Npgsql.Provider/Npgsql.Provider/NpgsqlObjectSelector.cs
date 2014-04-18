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
using System.Collections;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System.Data.Common;
using NpgsqlTypes;

namespace Npgsql.Provider
{
    /// <summary>
    /// Represents a custom data object selector to supplement or replace
    /// the schema collections supplied by the .NET Framework Data Provider
    /// for SQL Server.  Many of the enumerations here are required for full
    /// support of the built in data design scenarios.
    /// </summary>
    internal class NpgsqlObjectSelector : AdoDotNetObjectSelector
    {
        #region Protected Methods

        protected override IList<string> GetRequiredRestrictions(
            string typeName, object[] parameters )
        {
            if ( typeName == null )
            {
                throw new ArgumentNullException( "typeName" );
            }

            // All types except the root require a restriction on the database
            // because it is hard to enumerate objects on SQL Server across
            // multiple databases at the same time.  Also, the method used to
            // enumerate stored procedure columns only works to retrieve
            // columns for a single stored procedure at a time.
            if ( !typeName.Equals( NpgsqlObjectTypes.Root,
                StringComparison.OrdinalIgnoreCase ) )
            {
                if ( typeName.Equals( NpgsqlObjectTypes.StoredProcedureColumn,
                        StringComparison.OrdinalIgnoreCase ) )
                {
                    return new string[] {
                        "Database",
                        "Schema",
                        "StoredProcedure"
                    };
                }
                if ( typeName.Equals( NpgsqlObjectTypes.Schema,
                        StringComparison.OrdinalIgnoreCase ) )
                {
                    return new string[] {
                        "Database"
                    };
                }
                if ( typeName.Equals( NpgsqlObjectTypes.UserDefinedType,
                        StringComparison.OrdinalIgnoreCase ) )
                {
                    return new string[] {
                        "Database",
                        "Schema"
                    };
                }
                if ( typeName.Equals( NpgsqlObjectTypes.UserDefinedTypeColumn,
                        StringComparison.OrdinalIgnoreCase ) )
                {
                    return new string[] {
                        "Database",
                        "Schema",
                        "TypeName"
                    };
                }
            }

            return base.GetRequiredRestrictions( typeName, parameters );
        }

        protected override IList<string> GetSupportedRestrictions( string typeName, object[] parameters )
        {
            return base.GetSupportedRestrictions( typeName, parameters );
        }

        protected override IVsDataReader SelectObjects( string typeName,
            object[] restrictions, string[] properties, object[] parameters )
        {
            if ( typeName == null )
            {
                throw new ArgumentNullException( "typeName" );
            }

            // Execute a SQL statement to get the property values
            DbConnection conn = Site.GetLockedProviderObject() as DbConnection;
            Debug.Assert( conn != null, "Invalid provider object." );
            if ( conn == null )
            {
                // This should never occur
                throw new NotSupportedException();
            }
            try
            {
                // Ensure the connection is open
                if ( Site.State != DataConnectionState.Open )
                {
                    Site.Open();
                }

                // Create a command object
                DbCommand comm = conn.CreateCommand();

                // Choose and format SQL based on the type
                if ( typeName.Equals( NpgsqlObjectTypes.Root,
                        StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = rootEnumerationSql;
                }
                else if ( restrictions.Length == 0 ||
                    !( restrictions[ 0 ] is string ) )
                {
                    throw new ArgumentException(
                        "Missing required restriction(s)." );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.Index,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = FormatSqlString(
                        indexEnumerationSql,
                        restrictions,
                        indexEnumerationDefaults );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.IndexColumn,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = FormatSqlString(
                        indexColumnEnumerationSql,
                        restrictions,
                        indexColumnEnumerationDefaults );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.ForeignKey,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = FormatSqlString(
                        foreignKeyEnumerationSql,
                        restrictions,
                        foreignKeyEnumerationDefaults );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.ForeignKeyColumn,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = FormatSqlString(
                        foreignKeyColumnEnumerationSql,
                        restrictions,
                        foreignKeyColumnEnumerationDefaults );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.StoredProcedure,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = FormatSqlString(
                        storedProcedureEnumerationSql,
                        restrictions,
                        storedProcedureEnumerationDefaults );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.StoredProcedureParameter,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = FormatSqlString(
                        storedProcedureParameterEnumerationSql,
                        restrictions,
                        storedProcedureParameterEnumerationDefaults );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.StoredProcedureColumn,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    if ( restrictions.Length < 3 ||
                        !( restrictions[ 0 ] is string ) ||
                        !( restrictions[ 1 ] is string ) ||
                        !( restrictions[ 2 ] is string ) )
                    {
                        throw new ArgumentException(
                            "Missing required restriction(s)." );
                    }

                    //
                    // In order to implement stored procedure columns we
                    // execute the stored procedure in schema only mode
                    // and intepret the resulting schema table.
                    //
                    // First we need parameters information
                    // we create a new command for that
                    var paramCmd = conn.CreateCommand();
                    paramCmd.CommandText = FormatSqlString(
                        storedProcedureParameterEnumerationSql,
                        restrictions,
                        storedProcedureParameterEnumerationDefaults );
                    paramCmd.CommandType = CommandType.Text;
                    var parameterTypes = new StringBuilder();
                    DbDataReader paramsReader = null;
                    try
                    {
                        paramsReader = paramCmd.ExecuteReader();
                        var first = true;
                        while ( paramsReader.Read() )
                        {
                            var paramterTypeName = paramsReader[ "DataType" ].ToString();
                            parameterTypes.Append( ( !first ? ", " : string.Empty ) + "(NULL)::" + paramterTypeName );
                            first = false;
                        }
                    }
                    catch ( NpgsqlException )
                    {
                    }
                    catch ( InvalidOperationException )
                    {
                    }
                    finally
                    {
                        if ( paramsReader != null )
                        {
                            paramsReader.Close();
                        }
                    }
                    // Format the command type and text
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = String.Format(
                        CultureInfo.CurrentCulture,
                        "\"{0}\".\"{1}\".\"{2}\"({3})",
                        ( restrictions[ 0 ] as string ).Replace( "]", "\"" ),
                        ( restrictions[ 1 ] as string ).Replace( "]", "\"" ),
                        ( restrictions[ 2 ] as string ).Replace( "]", "\"" ),
                        parameterTypes );

                    // Get the schema of the stored procedure
                    DataTable schemaTable = null;
                    DbDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader( CommandBehavior.SchemaOnly );
                        schemaTable = reader.GetSchemaTable();
                    }
                    catch ( NpgsqlException )
                    {
                        // The DeriveParameters and GetSchemaTable calls can
                        // be flaky; catch SqlException here because we would
                        // rather return an empty result set than an error.
                    }
                    catch ( InvalidOperationException )
                    {
                        // DeriveParameters sometimes throws this as well
                    }
                    finally
                    {
                        if ( reader != null )
                        {
                            reader.Close();
                        }
                    }

                    // Build a different data table to contain the right
                    // information (must have full identifier)
                    DataTable dataTable = new DataTable();
                    dataTable.Locale = CultureInfo.CurrentCulture;
                    dataTable.Columns.Add( "Database", typeof( string ) );
                    dataTable.Columns.Add( "Schema", typeof( string ) );
                    dataTable.Columns.Add( "StoredProcedure", typeof( string ) );
                    dataTable.Columns.Add( "Name", typeof( string ) );
                    dataTable.Columns.Add( "Ordinal", typeof( int ) );
                    dataTable.Columns.Add( "ProviderType", typeof( int ) );
                    dataTable.Columns.Add( "FrameworkType", typeof( Type ) );
                    dataTable.Columns.Add( "NativeDataType" );
                    dataTable.Columns.Add( "MaxLength", typeof( int ) );
                    dataTable.Columns.Add( "Precision", typeof( short ) );
                    dataTable.Columns.Add( "Scale", typeof( short ) );
                    dataTable.Columns.Add( "IsNullable", typeof( bool ) );

                    // Populate the data table if a schema table was returned
                    if ( schemaTable != null )
                    {
                        foreach ( DataRow row in schemaTable.Rows )
                        {
                            dataTable.Rows.Add(
                                restrictions[ 0 ],
                                restrictions[ 1 ],
                                restrictions[ 2 ],
                                row[ "ColumnName" ],
                                row[ "ColumnOrdinal" ],
                                MapNativeToInt( row[ "ProviderType" ].ToString() ),
                                row[ "DataType" ],
                                row[ "ProviderType" ],
                                row[ "ColumnSize" ],
                                row[ "NumericPrecision" ],
                                row[ "NumericScale" ],
                                row[ "AllowDBNull" ] );
                        }
                    }

                    return new AdoDotNetTableReader( dataTable );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.Function,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = FormatSqlString(
                        functionEnumerationSql,
                        restrictions,
                        functionEnumerationDefaults );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.FunctionParameter,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = FormatSqlString(
                        functionParameterEnumerationSql,
                        restrictions,
                        functionParameterEnumerationDefaults );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.FunctionColumn,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    if ( restrictions.Length < 3 ||
                        !( restrictions[ 0 ] is string ) ||
                        !( restrictions[ 1 ] is string ) ||
                        !( restrictions[ 2 ] is string ) )
                    {
                        throw new ArgumentException(
                            "Missing required restriction(s)." );
                    }

                    //
                    // In order to implement stored procedure columns we
                    // execute the stored procedure in schema only mode
                    // and intepret the resulting schema table.
                    //
                    // First we need parameters information
                    // we create a new command for that
                    var paramCmd = conn.CreateCommand();
                    paramCmd.CommandText = FormatSqlString(
                        functionParameterEnumerationSql,
                        restrictions,
                        functionParameterEnumerationDefaults );
                    paramCmd.CommandType = CommandType.Text;
                    var parameterTypes = new StringBuilder();
                    DbDataReader paramsReader = null;
                    try
                    {
                        paramsReader = paramCmd.ExecuteReader();
                        var first = true;
                        while ( paramsReader.Read() )
                        {
                            var paramterTypeName = paramsReader[ "DataType" ].ToString();
                            parameterTypes.Append( ( !first ? ", " : string.Empty ) + "(NULL)::" + paramterTypeName );
                            first = false;
                        }
                    }
                    catch ( NpgsqlException )
                    {
                    }
                    catch ( InvalidOperationException )
                    {
                    }
                    finally
                    {
                        if ( paramsReader != null )
                        {
                            paramsReader.Close();
                        }
                    }
                    // Format the command type and text
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = String.Format(
                        CultureInfo.CurrentCulture,
                        "\"{0}\".\"{1}\".\"{2}\"({3})",
                        ( restrictions[ 0 ] as string ).Replace( "]", "\"" ),
                        ( restrictions[ 1 ] as string ).Replace( "]", "\"" ),
                        ( restrictions[ 2 ] as string ).Replace( "]", "\"" ),
                        parameterTypes );

                    // Get the schema of the stored procedure
                    DataTable schemaTable = null;
                    DbDataReader reader = null;
                    try
                    {
                        reader = comm.ExecuteReader( CommandBehavior.SchemaOnly );
                        schemaTable = reader.GetSchemaTable();
                    }
                    catch ( NpgsqlException )
                    {
                    }
                    catch ( InvalidOperationException )
                    {
                    }
                    finally
                    {
                        if ( reader != null )
                        {
                            reader.Close();
                        }
                    }

                    // Build a different data table to contain the right
                    // information (must have full identifier)
                    DataTable dataTable = new DataTable();
                    dataTable.Locale = CultureInfo.CurrentCulture;
                    dataTable.Columns.Add( "Database", typeof( string ) );
                    dataTable.Columns.Add( "Schema", typeof( string ) );
                    dataTable.Columns.Add( "Function", typeof( string ) );
                    dataTable.Columns.Add( "Name", typeof( string ) );
                    dataTable.Columns.Add( "Ordinal", typeof( int ) );
                    dataTable.Columns.Add( "ProviderType", typeof( int ) );
                    dataTable.Columns.Add( "FrameworkType", typeof( Type ) );
                    dataTable.Columns.Add( "NativeDataType" );
                    dataTable.Columns.Add( "MaxLength", typeof( int ) );
                    dataTable.Columns.Add( "Precision", typeof( short ) );
                    dataTable.Columns.Add( "Scale", typeof( short ) );
                    dataTable.Columns.Add( "IsNullable", typeof( bool ) );

                    // Populate the data table if a schema table was returned
                    if ( schemaTable != null )
                    {
                        foreach ( DataRow row in schemaTable.Rows )
                        {
                            dataTable.Rows.Add(
                                restrictions[ 0 ],
                                restrictions[ 1 ],
                                restrictions[ 2 ],
                                row[ "ColumnName" ],
                                row[ "ColumnOrdinal" ],
                                MapNativeToInt( row[ "ProviderType" ].ToString() ),
                                row[ "DataType" ],
                                row[ "ProviderType" ],
                                row[ "ColumnSize" ],
                                row[ "NumericPrecision" ],
                                row[ "NumericScale" ],
                                row[ "AllowDBNull" ] );
                        }
                    }

                    return new AdoDotNetTableReader( dataTable );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.Column,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    var columnsTable = ( conn as NpgsqlConnection ).GetSchema( "Columns",
                                                                               restrictions.Cast<string>().ToArray() );
                    ApplyMappingsIfNeeded( columnsTable, parameters );

                    return new AdoDotNetTableReader( columnsTable );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.Schema, StringComparison.OrdinalIgnoreCase ) )
                {
                    var schemas = new DataTable( typeName );
                    schemas.Columns.AddRange( new[] { new DataColumn( "Database" ), new DataColumn( "Name" ) } );

                    var cmd = conn.CreateCommand() as NpgsqlCommand;
                    var db = restrictions[ 0 ] as string;
                    //This is kind of a hack, i have to figure this out, we need a database name here...
                    if ( db.Equals( "Database", StringComparison.OrdinalIgnoreCase ) )
                    {
                        db = conn.Database;
                    }
                    cmd.CommandText = string.Format( "SELECT current_database() as \"Database\", nspname AS \"Name\", d.description AS \"Description\" FROM pg_catalog.pg_namespace LEFT JOIN pg_catalog.pg_description d ON d.objoid = oid WHERE nspname !~ '^pg_'", db );

                    using ( var adapter = new NpgsqlDataAdapter( cmd ) )
                    {
                        adapter.Fill( schemas );
                    }

                    ApplyMappingsIfNeeded( schemas, parameters );

                    return new AdoDotNetTableReader( schemas );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.UserDefinedType,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = FormatSqlString(
                        userDefinedTypeEnumerationSql,
                        restrictions,
                        userDefinedTypeEnumerationDefaults );

                    var types = new DataTable( typeName );

                    using ( var adapter = new NpgsqlDataAdapter( comm as NpgsqlCommand ) )
                    {
                        adapter.Fill( types );
                    }

                    IDictionary<string, object> mappings = null;
                    if ( ( ( parameters != null ) && ( parameters.Length > 1 ) ) && ( parameters[ 1 ] is DictionaryEntry ) )
                    {
                        object[] mappingParameters = null;
                        DictionaryEntry entry2 = ( DictionaryEntry ) parameters[ 1 ];
                        mappingParameters = entry2.Value as object[];
                        mappings = GetMappings( mappingParameters );
                    }

                    ApplyMappings( types, mappings );

                    return new AdoDotNetTableReader( types );
                }
                else if ( typeName.Equals( NpgsqlObjectTypes.UserDefinedTypeColumn,
                    StringComparison.OrdinalIgnoreCase ) )
                {
                    comm.CommandText = FormatSqlString(
                        userDefinedTypeColumnEnumerationSql,
                        restrictions,
                        userDefinedTypeColumnEnumerationDefaults );

                    var typesColumns = new DataTable( typeName );

                    using ( var adapter = new NpgsqlDataAdapter( comm as NpgsqlCommand ) )
                    {
                        adapter.Fill( typesColumns );
                    }

                    //IDictionary<string, object> mappings = null;
                    //if ( ( ( parameters != null ) && ( parameters.Length > 1 ) ) && ( parameters[ 1 ] is DictionaryEntry ) )
                    //{
                    //    object[] mappingParameters = null;
                    //    DictionaryEntry entry2 = ( DictionaryEntry ) parameters[ 1 ];
                    //    mappingParameters = entry2.Value as object[];
                    //    mappings = GetMappings( mappingParameters );
                    //}

                    //ApplyMappings( typesColumns, mappings );
                    ApplyMappingsIfNeeded( typesColumns, parameters );

                    return new AdoDotNetTableReader( typesColumns );
                }
                else
                {
                    throw new NotSupportedException();
                }

                return new AdoDotNetReader( comm.ExecuteReader() );
            }
            finally
            {
                Site.UnlockProviderObject();
            }
        }

        private static void ApplyMappingsIfNeeded( DataTable dataTable, object[] parameters )
        {
            IDictionary<string, object> mappings = null;
            if ( ( ( parameters != null ) && ( parameters.Length > 1 ) ) && ( parameters[ 1 ] is DictionaryEntry ) )
            {
                object[] mappingParameters = null;
                DictionaryEntry entry2 = ( DictionaryEntry ) parameters[ 1 ];
                mappingParameters = entry2.Value as object[];
                mappings = GetMappings( mappingParameters );
            }

            ApplyMappings( dataTable, mappings );
        }

        internal static DataTable GetSchemaTable( string schemaName, DbCommand command, object[] restrictions )
        {
            switch ( schemaName )
            {
                case "Indexes":
                    command.CommandText = FormatSqlString(
                        indexEnumerationSql,
                        restrictions,
                        indexEnumerationDefaults );
                    break;
                case "IndexColumns":
                    command.CommandText = FormatSqlString(
                        indexColumnEnumerationSql,
                        restrictions,
                        indexColumnEnumerationDefaults );
                    break;
            }
            var table = new DataTable( schemaName );
            using ( NpgsqlDataAdapter adapter = new NpgsqlDataAdapter( command as NpgsqlCommand ) )
            {
                adapter.Fill( table );
            }

            return table;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method formats a SQL string by specifying format arguments
        /// based on restrictions.  All enumerations require at least a
        /// database restriction, which is specified twice with different
        /// escape characters.  This is followed by each restriction in turn
        /// with the quote character escaped.  Where there is no restriction,
        /// a default restriction value is added to ensure the SQL statement
        /// is still valid.
        /// </summary>
        internal static string FormatSqlString( string sql,
            object[] restrictions, object[] defaultRestrictions )
        {
            Debug.Assert( sql != null );
            Debug.Assert( restrictions != null );
            Debug.Assert( restrictions.Length > 0 );
            Debug.Assert( restrictions[ 0 ] is string );
            Debug.Assert( defaultRestrictions != null );
            Debug.Assert( defaultRestrictions.Length >= restrictions.Length );

            object[] formatArgs = new object[ defaultRestrictions.Length + 1 ];
            formatArgs[ 0 ] = ( restrictions[ 0 ] as string ).Replace( "]", "\"" );
            for ( int i = 0; i < defaultRestrictions.Length; i++ )
            {
                if ( restrictions.Length > i && restrictions[ i ] != null )
                {
                    formatArgs[ i + 1 ] = "'" + restrictions[ i ].ToString().Replace( "'", "''" ) + "'";
                }
                else
                {
                    formatArgs[ i + 1 ] = defaultRestrictions[ i ];
                }
            }
            return String.Format( CultureInfo.CurrentCulture, sql, formatArgs );
        }

        #endregion

        #region Private Constants

        private const string rootEnumerationSql =
            @"SELECT" +
            "    'localhost' as \"Server\"," +
            "    session_user as \"Instance\"," +
            "    session_user as \"Login\"," +
            "    current_database() as \"Database\"," +
            "    current_user as \"User\"," +
            "    current_schemas(true) as \"Schema\"";
        //"SELECT" +
        //"   [Server] = SERVERPROPERTY('ServerName')," +
        //"   [Instance] = SERVERPROPERTY('InstanceName')," +
        //"   [Login] = SUSER_SNAME(SUSER_SID())," +
        //"   [Database] = DB_NAME()," +
        //"   [User] = USER_NAME()," +
        //"   [Schema] = SCHEMA_NAME()";

        internal const string indexEnumerationSql =
            "SELECT " +
            "    current_database() AS \"Database\", " +
            "    n.nspname AS \"Schema\", " +
            "    pc.relname AS \"Table\", " +
            "    c.relname AS \"Name\", " +
            "    indisunique AS \"IsUnique\", " +
            "    indisprimary AS \"IsPrimary\" " +
            "FROM pg_class c " +
            "JOIN " +
            "    pg_index i ON c.oid = i.indexrelid " +
            "JOIN " +
            "    pg_class pc ON pc.oid = i.indrelid " +
            "LEFT JOIN " +
            "    pg_namespace n ON n.oid = c.relnamespace " +
            "WHERE " +
            "    n.nspname !~ '^pg_' " +
            "AND " +
            "    c.relname = {4} " +
            "AND " +
            "    current_database() = {1} " +
            "AND " +
            "    n.nspname = {2} " +
            "AND " +
            "    pc.relname = {3} " +
            "ORDER BY 1,2,3,4";
        /*
        "SELECT" +
        "   [Database] = d.name," +
        "   [Schema] = SCHEMA_NAME(o.schema_id)," +
        "   [Table] = OBJECT_NAME(o.object_id)," +
        "   [Name] = i.name," +
        "   [IsUnique] = i.is_unique," +
        "   [IsPrimary] = i.is_primary_key" +
        " FROM" +
        "   [{0}].sys.indexes i INNER JOIN" +
        "   [{0}].sys.objects o ON i.object_id = o.object_id INNER JOIN" +
        "   master.sys.databases d ON d.name = {1}" +
        " WHERE" +
        "   i.type <> 0 AND" +
        "   SCHEMA_NAME(o.schema_id) = {2} AND" +
        "   OBJECT_NAME(o.object_id) = {3} AND" +
        "   i.name = {4}" +
        " ORDER BY" +
        "   1,2,3,4";
         * */
        internal static string[] indexEnumerationDefaults =
        {
            "current_database()",
            "nspname",
            "pc.relname",
            "c.relname"
        };

        internal const string indexColumnEnumerationSql =
            " SELECT " +
            "   current_database() AS \"Database\"," +
            "   n.nspname AS \"Schema\", " +
            "   c.relname AS \"Table\", " +
            "   i.relname AS \"Index\", " +
            "   pg_get_indexdef(ss.indexrelid, (ss.iopc).n, true) AS \"Name\", " +
            "   (ss.iopc).n as \"Ordinal\"" +
            "  FROM pg_index x" +
            "  JOIN pg_class c ON c.oid = x.indrelid" +
            "  JOIN pg_class i ON i.oid = x.indexrelid" +
            "  LEFT JOIN pg_namespace n ON n.oid = c.relnamespace" +
            "  LEFT JOIN (SELECT indexrelid, information_schema._pg_expandarray(indclass) AS iopc" +
            "  FROM pg_index ) ss ON ss.indexrelid = x.indexrelid" +
            " WHERE c.relkind = 'r'::\"char\" AND i.relkind = 'i'::\"char\"" +
            " AND n.nspname !~ '^pg_'" +
            " AND current_database() = {1}" +
            " AND n.nspname={2}" +
            " AND c.relname={3}" +
            " AND i.relname={4}" +
            " AND pg_get_indexdef(ss.indexrelid, (ss.iopc).n, true) = {5}" +
            " ORDER BY 1,2,3,4,6";
        /*"SELECT" +
        "   [Database] = d.name," +
        "   [Schema] = SCHEMA_NAME(o.schema_id)," +
        "   [Table] = OBJECT_NAME(o.object_id)," +
        "   [Index] = i.name," +
        "   [Name] = c.name," +
        "   [Ordinal] = ic.key_ordinal" +
        " FROM" +
        "   [{0}].sys.index_columns ic INNER JOIN" +
        "   [{0}].sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id INNER JOIN" +
        "   [{0}].sys.indexes i ON c.object_id = i.object_id AND ic.index_id = i.index_id INNER JOIN" +
        "   [{0}].sys.objects o ON i.object_id = o.object_id INNER JOIN" +
        "   master.sys.databases d ON d.name = {1}" +
        " WHERE" +
        "   ic.column_id > 0 AND" +
        "   i.type <> 0 AND" +
        "   SCHEMA_NAME(o.schema_id) = {2} AND" +
        "   OBJECT_NAME(o.object_id) = {3} AND" +
        "   i.name = {4} AND" +
        "   c.name = {5}" +
        " ORDER BY" +
        "   1,2,3,4,6";*/
        internal static string[] indexColumnEnumerationDefaults =
        {
            "current_database()",
            "n.nspname",
            "c.relname",
            "i.relname",
            "pg_get_indexdef(ss.indexrelid, (ss.iopc).n, true)"
        };

        private const string foreignKeyEnumerationSql =
            " SELECT " +
            "         current_database() AS \"Database\", " +
            "         n.nspname AS \"Schema\", " +
            "         t.relname AS \"Table\", " +
            "         c.conname AS \"Name\", " +
            "         reln.nspname AS \"ReferencedTableSchema\", " +
            "         rel.relname AS \"ReferencedTableName\", " +
            "         c.confupdtype AS \"UpdateAction\", " +
            "         c.confdeltype AS \"DeleteAction\" " +
            " FROM  " +
            "         pg_constraint c " +
            " LEFT JOIN  " +
            "         pg_namespace n ON n.oid = c.connamespace " +
            " LEFT OUTER JOIN  " +
            "         pg_class t ON t.oid = c.conrelid " +
            " LEFT OUTER JOIN  " +
            "         pg_class rel ON rel.oid = c.confrelid " +
            " LEFT OUTER JOIN  " +
            "         pg_namespace reln ON reln.oid = rel.relnamespace " +
            " WHERE  " +
            "         c.contype = 'f' " +
            " AND  " +
            "         current_database() = {1} " +
            " AND " +
            "         n.nspname = {2} " +
            " AND " +
            "         t.relname = {3} " +
            " AND " +
            "         c.conname = {4} " +
            " ORDER BY " +
            "   1,2,3,4";
        /*
        "SELECT" +
        "   [Database] = d.name," +
        "   [Schema] = SCHEMA_NAME(o.schema_id)," +
        "   [Table] = OBJECT_NAME(o.object_id)," +
        "   [Name] = fk.name," +
        "   [ReferencedTableSchema] = SCHEMA_NAME(rk.schema_id)," +
        "   [ReferencedTableName] = OBJECT_NAME(rk.object_id)," +
        "   [UpdateAction] = fk.update_referential_action," +
        "   [DeleteAction] = fk.delete_referential_action" +
        " FROM" +
        "   [{0}].sys.foreign_keys fk INNER JOIN" +
        "   [{0}].sys.objects rk ON fk.referenced_object_id = rk.object_id INNER JOIN" +
        "   [{0}].sys.objects o ON fk.parent_object_id = o.object_id INNER JOIN" +
        "   master.sys.databases d ON d.name = {1}" +
        " WHERE" +
        "   SCHEMA_NAME(o.schema_id) = {2} AND" +
        "   OBJECT_NAME(o.object_id) = {3} AND" +
        "   fk.name = {4}" +
        " ORDER BY" +
        "   1,2,3,4";
         **/
        private static string[] foreignKeyEnumerationDefaults =
        {
            "current_database()",
            "n.nspname",
            "t.relname",
            "c.conname"
        };

        private const string foreignKeyColumnEnumerationSql =
            " SELECT " +
            "         current_database() AS \"Database\", " +
            "         n.nspname AS \"Schema\", " +
            "         pt.relname AS \"Table\", " +
            "         c.conname AS \"ForeignKey\", " +
            "         pkc.attname AS \"Name\", " +
            "         (c.confkey).n AS \"Ordinal\", " +
            "         fkc.attname AS \"ReferencedColumnName\" " +
            "         from (select c.oid, " +
            "         c.conname, " +
            "         c.conrelid, " +
            "         information_schema._pg_expandarray(c.conkey) as conkey, " +
            "         c.confrelid, " +
            "         c.connamespace, " +
            "         information_schema._pg_expandarray(c.confkey) as confkey " +
            " FROM " +
            "         pg_constraint c " +
            " WHERE " +
            "         contype = 'f') c " +
            " JOIN " +
            "         pg_class pt on pt.oid = c.conrelid " +
            " JOIN " +
            "         pg_class ft on ft.oid = c.confrelid " +
            " JOIN " +
            "         pg_namespace n ON n.oid = c.connamespace " +
            " JOIN " +
            "         pg_attribute pkc on pkc.attrelid = c.conrelid and pkc.attnum = (c.conkey).x " +
            " JOIN " +
            "         pg_attribute fkc on fkc.attrelid = c.confrelid and fkc.attnum = (c.confkey).x " +
            " WHERE " +
            "         current_database() = {1} " +
            " AND " +
            "         n.nspname = {2} " +
            " AND " +
            "         pt.relname = {3} " +
            " AND " +
            "         c.conname = {4} " +
            " AND " +
            "         pkc.attname = {5} " +
            " ORDER BY " +
            "         1,2,3,4,6";
        /*
        "SELECT" +
        "   [Database] = d.name," +
        "   [Schema] = SCHEMA_NAME(o.schema_id)," +
        "   [Table] = OBJECT_NAME(o.object_id)," +
        "   [ForeignKey] = fk.name," +
        "   [Name] = fc.name," +
        "   [Ordinal] = fkc.constraint_column_id," +
        "   [ReferencedColumnName] = rc.name" +
        " FROM" +
        "   [{0}].sys.foreign_key_columns fkc INNER JOIN" +
        "   [{0}].sys.columns fc ON fkc.parent_object_id = fc.object_id AND fkc.parent_column_id = fc.column_id INNER JOIN" +
        "   [{0}].sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id INNER JOIN" +
        "   [{0}].sys.foreign_keys fk ON fkc.constraint_object_id = fk.object_id INNER JOIN" +
        "   [{0}].sys.objects rk ON fk.referenced_object_id = rk.object_id INNER JOIN" +
        "   [{0}].sys.objects o ON fk.parent_object_id = o.object_id INNER JOIN" +
        "   master.sys.databases d ON d.name = {1}" +
        " WHERE" +
        "   SCHEMA_NAME(o.schema_id) = {2} AND" +
        "   OBJECT_NAME(o.object_id) = {3} AND" +
        "   fk.name = {4} AND" +
        "   fc.name = {5}" +
        " ORDER BY" +
        "   1,2,3,4,6";
         * */
        private static string[] foreignKeyColumnEnumerationDefaults =
        {
            "current_database()",
            "n.nspname",
            "pt.relname",
            "c.conname",
            "pkc.attname"
        };

        private const string storedProcedureEnumerationSql =
            " select " +
            "         current_database() AS \"Database\", " +
            "         n.nspname AS \"Schema\", " +
            "         p.proname as \"Name\" " +
            "         from pg_proc p " +
            "         left join pg_namespace n " +
            "         on n.oid = p.pronamespace " +
            "         left join pg_type t " +
            "         on p.prorettype = t.oid " +
            "         where (p.proretset = true or t.typname = 'void') and n.nspname not in ('pg_catalog','information_schema') and p.proname not in (select pg_proc.proname from pg_proc group by pg_proc.proname having count(pg_proc.proname) > 1) " +
            "         AND current_database() = {1} " +
            "         AND n.nspname = {2} " +
            "         AND p.proname = {3} " +
            " ORDER BY 1, 2, 3 ";
        /*
        "SELECT" +
        "   [Database] = d.name," +
        "   [Schema] = SCHEMA_NAME(o.schema_id)," +
        "   [Name] = o.name" +
        " FROM" +
        "   [{0}].sys.objects o INNER JOIN" +
        "   master.sys.databases d ON d.name = {1}" +
        " WHERE" +
        "   o.type IN ('P', 'PC') AND" +
        "   SCHEMA_NAME(o.schema_id) = {2} AND" +
        "   OBJECT_NAME(o.object_id) = {3}" +
        " ORDER BY" +
        "   1,2,3";
         * */
        private static string[] storedProcedureEnumerationDefaults =
        {
            "current_database()",
            "n.nspname",
            "p.proname"
        };

        private const string storedProcedureParameterEnumerationSql =
            " SELECT " +
            "    current_database() AS \"Database\", " +
            "    ss.n_nspname AS \"Schema\", " +
            "    ss.proname as \"StoredProcedure\", " +
            "    case " +
            "      when NULLIF(ss.proargnames[(ss.x).n], '') is null then 'x' " +
            "      else ss.proargnames[(ss.x).n] " +
            "    end as \"Name\", " +
            "    (ss.x).n as \"Ordinal\", " +
            "    t.typname as \"DataType\", " +
            "    -1::int4 as \"MaxLength\", " +
            "    -1::int4 as \"Precision\", " +
            "    -1::int4 as \"Scale\", " +
            "    case " +
            "      when ss.proargmodes IS null then false " +
            "      when ss.proargmodes[(ss.x).n] = 'i' then false " +
            "      when ss.proargmodes[(ss.x).n] = 'o' then true " +
            "      when ss.proargmodes[(ss.x).n] = 'b' then true " +
            "      else false " +
            "    end as \"IsOutput\" " +
            " FROM pg_type t " +
            "      join (select " +
            "        n.nspname AS n_nspname, " +
            "        p.proname, " +
            "        p.oid AS p_oid, " +
            "        p.proargnames, " +
            "        p.proargmodes, " +
            "        p.proretset, " +
            "        p.prorettype, " +
            "        information_schema._pg_expandarray(COALESCE(p.proallargtypes, p.proargtypes::oid[])) AS x " +
            "        from pg_namespace n " +
            "          join pg_proc p " +
            "          on n.oid = p.pronamespace and p.proname not in (select pg_proc.proname from pg_proc group by pg_proc.proname having count(pg_proc.proname) > 1)) ss " +
            "      on t.oid = (ss.x).x " +
            "        join pg_type rt " +
            "      on ss.prorettype = rt.oid " +
            "      where (ss.proretset = true or rt.typname = 'void')  " +
            "      and ss.n_nspname not in ('pg_catalog','information_schema') " +
            "      AND current_database() = {1} " +
            "      AND ss.n_nspname = {2} " +
            "      AND ss.proname = {3} " +
            "      AND (case " +
            "				when NULLIF(ss.proargnames[(ss.x).n], '') is null then 'x' " +
            "				else ss.proargnames[(ss.x).n] " +
            "			 end) = {4} " +
            " ORDER BY " +
            "   1,2,3,5";
        /*
        "SELECT" +
        "   [Database] = d.name," +
        "   [Schema] = SCHEMA_NAME(o.schema_id)," +
        "   [StoredProcedure] = o.name," +
        "   [Name] = p.name," +
        "   [Ordinal] = p.parameter_id," +
        "   [DataType] = t.name," +
        "   [MaxLength] = CASE WHEN t.name IN (N'nchar', N'nvarchar') THEN p.max_length/2 ELSE p.max_length END," +
        "   [Precision] = p.precision," +
        "   [Scale] = p.scale," +
        "   [IsOutput] = p.is_output" +
        " FROM" +
        "   [{0}].sys.parameters p INNER JOIN" +
        "   [{0}].sys.types t ON p.system_type_id = t.user_type_id INNER JOIN" +
        "   [{0}].sys.objects o ON p.object_id = o.object_id INNER JOIN" +
        "   master.sys.databases d ON d.name = {1}" +
        " WHERE" +
        "   o.type IN ('P', 'PC') AND" +
        "   SCHEMA_NAME(o.schema_id) = {2} AND" +
        "   OBJECT_NAME(o.object_id) = {3} AND" +
        "   p.name = {4}" +
        " ORDER BY" +
        "   1,2,3,5";
         * */
        private static string[] storedProcedureParameterEnumerationDefaults =
        {
                "current_database()",
                "ss.n_nspname",
                "ss.proname",
                "(case " +
                "				when NULLIF(ss.proargnames[(ss.x).n], '') is null then 'x' " +
                "				else ss.proargnames[(ss.x).n] " +
                "			 end)"
        };

        private const string functionEnumerationSql =
            " select" +
            "       current_database() as \"Database\"," +
            "       n.nspname AS \"Schema\"," +
            "       p.proname as \"Name\"," +
            "       'TF'::char(2) as \"Type\"" +
            "       from pg_proc p" +
            "       left join pg_namespace n" +
            "       on n.oid = p.pronamespace" +
            "       left join pg_type t" +
            "       on p.prorettype = t.oid" +
            "       where (t.typname != 'void') " +
            "       and n.nspname not in ('pg_catalog','information_schema') " +
            "       and p.proname not in (select pg_proc.proname from pg_proc group by pg_proc.proname having count(pg_proc.proname) > 1)" +
            "       AND current_database() = {1}" +
            "       AND n.nspname = {2}" +
            "       AND p.proname = {3}" +
            "   ORDER BY 1,2,3";
        //"SELECT" +
        //"   [Database] = d.name," +
        //"   [Schema] = SCHEMA_NAME(o.schema_id)," +
        //"   [Name] = o.name," +
        //"   [Type] = o.type" +
        //" FROM" +
        //"   [{0}].sys.objects o INNER JOIN" +
        //"   master.sys.databases d ON d.name = {1}" +
        //" WHERE" +
        //"   o.type IN ('AF', 'FN', 'FS', 'FT', 'IF', 'TF') AND" +
        //"   SCHEMA_NAME(o.schema_id) = {2} AND" +
        //"   OBJECT_NAME(o.object_id) = {3}" +
        //" ORDER BY" +
        //"   1,2,3";
        private static string[] functionEnumerationDefaults =
        {
            "current_database()",
            "n.nspname",
            "p.proname"
        };

        private const string functionParameterEnumerationSql =
            " SELECT " +
            "    current_database() AS \"Database\", " +
            "    ss.n_nspname AS \"Schema\", " +
            "    ss.proname as \"Function\", " +
            "    case " +
            "      when NULLIF(ss.proargnames[(ss.x).n], '') is null then 'x' " +
            "      else ss.proargnames[(ss.x).n] " +
            "    end as \"Name\", " +
            "    (ss.x).n as \"Ordinal\", " +
            "    t.typname as \"DataType\", " +
            "    -1::int4 as \"MaxLength\", " +
            "    -1::int4 as \"Precision\", " +
            "    -1::int4 as \"Scale\", " +
            "    case " +
            "      when ss.proargmodes IS null then false " +
            "      when ss.proargmodes[(ss.x).n] = 'i' then false " +
            "      when ss.proargmodes[(ss.x).n] = 'o' then true " +
            "      when ss.proargmodes[(ss.x).n] = 'b' then true " +
            "      else false " +
            "    end as \"IsOutput\" " +
            " FROM pg_type t " +
            "      join (select " +
            "        n.nspname AS n_nspname, " +
            "        p.proname, " +
            "        p.oid AS p_oid, " +
            "        p.proargnames, " +
            "        p.proargmodes, " +
            "        p.proretset, " +
            "        p.prorettype, " +
            "        information_schema._pg_expandarray(COALESCE(p.proallargtypes, p.proargtypes::oid[])) AS x " +
            "        from pg_namespace n " +
            "          join pg_proc p " +
            "          on n.oid = p.pronamespace and p.proname not in (select pg_proc.proname from pg_proc group by pg_proc.proname having count(pg_proc.proname) > 1)) ss " +
            "      on t.oid = (ss.x).x " +
            "        join pg_type rt " +
            "      on ss.prorettype = rt.oid " +
            "      where (rt.typname != 'void')  " +
            "      and ss.n_nspname not in ('pg_catalog','information_schema') " +
            "      AND current_database() = {1} " +
            "      AND ss.n_nspname = {2} " +
            "      AND ss.proname = {3} " +
            "      AND (case " +
            "				when NULLIF(ss.proargnames[(ss.x).n], '') is null then 'x' " +
            "				else ss.proargnames[(ss.x).n] " +
            "			 end) = {4} " +
            " ORDER BY " +
            "   1,2,3,5";
        /*"SELECT" +
        "   [Database] = d.name," +
        "   [Schema] = SCHEMA_NAME(o.schema_id)," +
        "   [Function] = o.name," +
        "   [Name] = CASE WHEN p.parameter_id = 0 THEN N'@RETURN_VALUE' ELSE p.name END," +
        "   [Ordinal] = p.parameter_id," +
        "   [DataType] = t.name," +
        "   [MaxLength] = CASE WHEN t.name IN (N'nchar', N'nvarchar') THEN p.max_length/2 ELSE p.max_length END," +
        "   [Precision] = p.precision," +
        "   [Scale] = p.scale," +
        "   [IsOutput] = p.is_output" +
        " FROM" +
        "   [{0}].sys.parameters p INNER JOIN" +
        "   [{0}].sys.types t ON p.system_type_id = t.user_type_id INNER JOIN" +
        "   [{0}].sys.objects o ON p.object_id = o.object_id INNER JOIN" +
        "   master.sys.databases d ON d.name = {1}" +
        " WHERE" +
        "   o.type IN ('AF', 'FN', 'FS', 'FT', 'IF', 'TF') AND" +
        "   SCHEMA_NAME(o.schema_id) = {2} AND" +
        "   OBJECT_NAME(o.object_id) = {3} AND" +
        "   p.name = {4}" +
        " ORDER BY" +
        "   1,2,3,5";*/

        private static string[] functionParameterEnumerationDefaults =
            {
                "current_database()",
                "ss.n_nspname",
                "ss.proname",
                "(case " +
                "				when NULLIF(ss.proargnames[(ss.x).n], '') is null then 'x' " +
                "				else ss.proargnames[(ss.x).n] " +
                "			 end)"
            };

        private const string functionColumnEnumerationSql =
                " SELECT " +
                "     current_database() AS \"Database\",  " +
                "     n.nspname AS \"Schema\",  " +
                "     p.proname AS \"Function\",  " +
                "     attname AS \"Name\",  " +
                "     attnum AS \"Ordinal\",  " +
                "     bt.typname AS \"DataType\", " +
                "     attlen AS \"MaxLength\",  " +
                "     -1 AS \"Precision\",  " +
                "     -1 AS \"Scale\" " +
                " FROM pg_type t " +
                " JOIN  " +
                "     pg_class c on (c.reltype = t.oid) " +
                " JOIN  " +
                "     pg_attribute on (attrelid = c.oid) " +
                " LEFT OUTER JOIN  " +
                "     pg_type bt ON bt.oid = atttypid " +
                " LEFT OUTER JOIN  " +
                "     pg_catalog.pg_proc p ON p.prorettype = t.oid " +
                " LEFT OUTER JOIN  " +
                "     pg_catalog.pg_namespace n ON n.oid = t.typnamespace " +
                " WHERE " +
                "     current_database() = {1} " +
                " AND " +
                "     n.nspname = {2} " +
                " AND  " +
                "     p.proname = {3} " +
                " AND  " +
                "     attname = {4} " +
                " ORDER BY" +
                "   1,2,3,5";
        /*
            "SELECT" +
            "   [Database] = d.name," +
            "   [Schema] = SCHEMA_NAME(o.schema_id)," +
            "   [Function] = o.name," +
            "   [Name] = c.name," +
            "   [Ordinal] = c.column_id," +
            "   [DataType] = t.name," +
            "   [MaxLength] = CASE WHEN t.name IN (N'nchar', N'nvarchar') THEN c.max_length/2 ELSE c.max_length END," +
            "   [Precision] = c.precision," +
            "   [Scale] = c.scale" +
            " FROM" +
            "   [{0}].sys.columns c INNER JOIN" +
            "   [{0}].sys.types t ON c.system_type_id = t.user_type_id INNER JOIN" +
            "   [{0}].sys.objects o ON c.object_id = o.object_id AND o.type IN ('AF', 'FN', 'FS', 'FT', 'IF', 'TF') INNER JOIN" +
            "   master.sys.databases d ON d.name = {1}" +
            " WHERE" +
            "   SCHEMA_NAME(o.schema_id) = {2} AND" +
            "   OBJECT_NAME(o.object_id) = {3} AND" +
            "   c.name = {4}" +
            " ORDER BY" +
            "   1,2,3,5";
             * */
        private static string[] functionColumnEnumerationDefaults =
        {
            "current_database()",
            "n.nspname",
            "p.proname",
            "attname"
        };

        private const string userDefinedTypeEnumerationSql =
            " SELECT " +
            "     current_database() as type_database, " +
            "     n.nspname as type_schema, " +
            "     c.oid as type_oid, " +
            "     c.relname as type_name " +
            " FROM " +
            "     pg_class c" +
            " JOIN " +
            "     pg_namespace n ON n.oid = c.relnamespace " +
            " WHERE " +
            "     relkind = 'c' " +
            " AND " +
            "     current_database() = {1} " +
            " AND " +
            "     n.nspname = {2} " +
            " AND " +
            "     c.oid = {3} " +
            " ORDER BY" +
            "   1,2,4";

        private static string[] userDefinedTypeEnumerationDefaults =
        {
            "current_database()",
            "n.nspname",
            "c.oid"
        };

        private const string userDefinedTypeColumnEnumerationSql =
            " SELECT " +
            "     current_database() AS \"Database\", " +
            "     n.nspname AS \"Schema\", " +
            "     t.typname AS \"TypeName\", " +
            "     attname AS \"Name\", " +
            "     t.oid::int4 AS \"Oid\", " +
            "     attnum AS \"Ordinal\", " +
            "     bt.typname AS \"DataType\", " +
            "     attlen AS \"MaxLength\", " +
            "     -1 AS \"Precision\", " +
            "     -1 AS \"Scale\" " +
            " FROM pg_type t " +
            " LEFT OUTER JOIN " +
            "     pg_class c on (c.reltype = t.oid) " +
            " LEFT OUTER JOIN " +
            "     pg_attribute on (attrelid = c.oid) " +
            " LEFT OUTER JOIN " +
            "     pg_type bt ON bt.oid = atttypid " +
            "  LEFT OUTER JOIN " +
            "     pg_catalog.pg_namespace n ON n.oid = t.typnamespace " +
            " WHERE " +
            "     current_database() = {1} " +
            "  AND " +
            "     n.nspname = {2} " +
            " AND " +
            "     t.typname = {3} " +
            " ORDER BY " +
            "     1,2,3,4";
        private static string[] userDefinedTypeColumnEnumerationDefaults =
        {
            "current_database()",
            "n.nspname",
            "t.typname"
        };

        #endregion

        internal static NpgsqlDbType MapNativeToNpgsqlDbType( string native )
        {
            //Postgresql Type	NpgsqlDbType	System.DbType Enum	.Net System Type
            //int8	Bigint	Int64	Int64
            //bool	Boolean	Boolean	Boolean
            //Box, Circle, Line, LSeg, Path, Point, Polygon	Box, Circle, Line, LSeg, Path, Point, Polygon	Object	Object
            //bytea	Bytea	Binary	Byte[]
            //date	Date	Date	DateTime, NpgsqlDate
            //float8	Double	Double	Double
            //int4	Integer	Int32	Int32
            //money	Money	Decimal	Decimal
            //numeric	Numeric	Decimal	Decimal
            //float4	Real	Single	Single
            //int2	Smallint	Int16	Int16
            //text	Text	String	String
            //time	Time	Time	DateTime, NpgsqlTime
            //timetz	Time	Time	DateTime, NpgsqlTimeTZ
            //timestamp	Timestamp	DateTime	DateTime, NpgsqlTimestamp
            //timestamptz	TimestampTZ	DateTime	DateTime, NpgsqlTimestampTZ
            //interval	Interval	Object	TimeSpan, NpgsqlInterval
            //varchar	Varchar	String	String
            //inet	Inet	Object	NpgsqlInet, IPAddress 
            //(there is an implicity cast operator to convert NpgsqlInet objects into IPAddress if you need to use IPAddress and have only NpgsqlInet)
            //bit	Bit	Boolean	Boolean, Int32 
            //(If you use an Int32 value, odd values will be translated to bit 1 and even values to bit 0)
            //uuid	Uuid	Guid	Guid
            //array	Array	Object	Array 
            //In order to explicitly use array type, specify NpgsqlDbType as an 'OR'ed type: NpgsqlDbType.Array | NpgsqlDbType.Integer for an array of Int32 for example.
            switch ( native )
            {
                case "int4":
                    return NpgsqlDbType.Integer;
                case "int8":
                    return NpgsqlDbType.Bigint;
                case "bool":
                    return NpgsqlDbType.Boolean;
                case "bytea":
                    return NpgsqlDbType.Bytea;
                case "date":
                    return NpgsqlDbType.Date;
                case "float8":
                    return NpgsqlDbType.Double;
                case "money":
                    return NpgsqlDbType.Money;
                case "numeric":
                    return NpgsqlDbType.Numeric;
                case "float4":
                    return NpgsqlDbType.Real;
                case "int2":
                    return NpgsqlDbType.Smallint;
                case "text":
                    return NpgsqlDbType.Text;
                case "time":
                    return NpgsqlDbType.Time;
                case "timetz":
                    return NpgsqlDbType.TimeTZ;
                case "timestamp":
                    return NpgsqlDbType.Timestamp;
                case "timestamptz":
                    return NpgsqlDbType.TimestampTZ;
                case "interval":
                    return NpgsqlDbType.Interval;
                case "varchar":
                    return NpgsqlDbType.Varchar;
                case "inet":
                    return NpgsqlDbType.Inet;
                case "bit":
                    return NpgsqlDbType.Bit;
                case "uuid":
                    return NpgsqlDbType.Uuid;
                case "array":
                    return NpgsqlDbType.Array;
                case "bpchar":
                case "char":
                    return NpgsqlDbType.Char;
            }

            return NpgsqlDbType.Text;
        }

        internal static int MapNativeToInt( string native )
        {
            return ( int ) MapNativeToNpgsqlDbType( native );
        }
    }
}
