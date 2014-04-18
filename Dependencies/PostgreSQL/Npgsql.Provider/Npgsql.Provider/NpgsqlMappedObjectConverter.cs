using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using NpgsqlTypes;

namespace Npgsql.Provider
{
    [SuppressMessage( "Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses" )]
    internal class NpgsqlMappedObjectConverter : AdoDotNetMappedObjectConverter
    {
        // Methods
        protected override object ConvertToMappedMember( string typeName, string mappedMemberName, object[] underlyingValues, object[] parameters )
        {
            if ( typeName == null )
            {
                throw new ArgumentNullException( "typeName" );
            }
            if ( mappedMemberName == null )
            {
                throw new ArgumentNullException( "mappedMemberName" );
            }
            if ( parameters != null )
            {
                throw new ArgumentException( "Invalid parameters", "parameters" );
            }
            if ( mappedMemberName.Equals( "Name", StringComparison.OrdinalIgnoreCase ) && typeName.Equals( "Column", StringComparison.OrdinalIgnoreCase ) )
            {
                var str = underlyingValues[ 0 ] as string;
                return string.Format( "\"{0}\"", str );
            }

            return base.ConvertToMappedMember( typeName, mappedMemberName, underlyingValues, parameters );
        }

        protected override DbType GetDbTypeFromNativeType( string nativeType )
        {
            var result = GetDbTypeFromNativeTypeString( nativeType );
            if ( result != DbType.Object || ( nativeType == "inet" || nativeType == "array" ) )
                return result;

            return base.GetDbTypeFromNativeType( GetPureType( nativeType ) );
        }

        internal static DbType GetDbTypeFromNativeTypeString( string nativeType )
        {
            switch ( nativeType )
            {
                case "int4":
                    return DbType.Int32;
                case "int8":
                    return DbType.Int64;
                case "bool":
                    return DbType.Boolean;
                case "bytea":
                    return DbType.Object;
                case "date":
                    return DbType.Date;
                case "float8":
                    return DbType.Double;
                case "money":
                    return DbType.Currency;
                case "numeric":
                    return DbType.Decimal;
                case "float4":
                    return DbType.Single;
                case "int2":
                    return DbType.Int16;
                case "text":
                    return DbType.AnsiString;
                case "time":
                    return DbType.Time;
                case "timetz":
                    return DbType.Time;
                case "timestamp":
                    return DbType.DateTimeOffset;
                case "timestamptz":
                    return DbType.DateTimeOffset;
                case "interval":
                    return DbType.DateTimeOffset;
                case "varchar":
                    return DbType.AnsiStringFixedLength;
                case "inet":
                    return DbType.Object;
                case "bit":
                    return DbType.Boolean;
                case "uuid":
                    return DbType.Guid;
                case "array":
                    return DbType.Object;
                default:
                    return DbType.Object;
            }
        }

        protected override Type GetFrameworkTypeFromNativeType( string nativeType )
        {
            return base.GetFrameworkTypeFromNativeType( GetPureType( nativeType ) );
        }

        protected override int GetProviderTypeFromNativeType( string nativeType )
        {
            switch ( nativeType )
            {
                case "int4":
                    return ( int ) NpgsqlDbType.Integer;
                case "int8":
                    return ( int ) NpgsqlDbType.Bigint;
                case "bool":
                    return ( int ) NpgsqlDbType.Boolean;
                case "bytea":
                    return ( int ) NpgsqlDbType.Bytea;
                case "date":
                    return ( int ) NpgsqlDbType.Date;
                case "float8":
                    return ( int ) NpgsqlDbType.Double;
                case "money":
                    return ( int ) NpgsqlDbType.Money;
                case "numeric":
                    return ( int ) NpgsqlDbType.Numeric;
                case "float4":
                    return ( int ) NpgsqlDbType.Real;
                case "int2":
                    return ( int ) NpgsqlDbType.Smallint;
                case "text":
                    return ( int ) NpgsqlDbType.Text;
                case "time":
                    return ( int ) NpgsqlDbType.Time;
                case "timetz":
                    return ( int ) NpgsqlDbType.TimeTZ;
                case "timestamp":
                    return ( int ) NpgsqlDbType.Timestamp;
                case "timestamptz":
                    return ( int ) NpgsqlDbType.TimestampTZ;
                case "interval":
                    return ( int ) NpgsqlDbType.Interval;
                case "varchar":
                    return ( int ) NpgsqlDbType.Varchar;
                case "inet":
                    return ( int ) NpgsqlDbType.Inet;
                case "bit":
                    return ( int ) NpgsqlDbType.Bit;
                case "uuid":
                    return ( int ) NpgsqlDbType.Uuid;
                case "array":
                    return ( int ) NpgsqlDbType.Array;
            }
            return base.GetProviderTypeFromNativeType( GetPureType( nativeType ) );
        }

        private static string GetPureType( string nativeType )
        {
            return Regex.Replace( nativeType, @"\([0-9]+\)", string.Empty, RegexOptions.IgnoreCase );
        }

        protected override object ConvertToUnderlyingRestriction( string mappedTypeName, int substitutionValueIndex, object[] mappedRestrictions, object[] parameters )
        {
            return base.ConvertToUnderlyingRestriction( mappedTypeName, substitutionValueIndex, mappedRestrictions, parameters );
        }
    }

}
