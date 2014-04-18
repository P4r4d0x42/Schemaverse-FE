using Microsoft.VisualStudio.Data.AdoDotNet;

namespace Npgsql.Provider
{
    /// <summary>
    /// Provides rudimentary connectionproperties support
    /// </summary>
    internal sealed class NpgsqlDataConnectionProperties : AdoDotNetConnectionProperties
    {
        public NpgsqlDataConnectionProperties()
            : base( "Npgsql" )
        {
        }

        // Methods
        public override void Reset()
        {
            base.Reset();
            this[ "Integrated Security" ] = false;
        }

        // Properties
        public override bool IsComplete
        {
            get
            {
                string str = this[ "Host" ] as string;
                if ( string.IsNullOrEmpty( str ) )
                {
                    return false;
                }
                if ( !( ( bool ) this[ "Integrated Security" ] ) )
                {
                    string str2 = this[ "User ID" ] as string;
                    if ( string.IsNullOrEmpty( str2 ) )
                    {
                        return false;
                    }

                    string str3 = this[ "Password" ] as string;
                    if ( string.IsNullOrEmpty( str3 ) )
                    {
                        return false;
                    }
                }
                return true;
            }
        }

    }
}
