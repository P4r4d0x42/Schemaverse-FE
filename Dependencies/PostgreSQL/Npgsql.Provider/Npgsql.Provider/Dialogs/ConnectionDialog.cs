using System;
using System.Windows.Forms;
using Microsoft.Data.ConnectionUI;
using Microsoft.VisualStudio.Data;

namespace Npgsql.Provider.Dialogs
{
    public partial class ConnectionDialog : UserControl, IDataConnectionUIControl
    {
        // Fields
        private DataConnectionProperties _connectionProperties;

        public ConnectionDialog()
        {
            InitializeComponent();
        }

        // Methods
        public void Initialize( DataConnectionProperties connectionProperties )
        {
            if ( connectionProperties == null )
            {
                throw new ArgumentNullException( "connectionProperties" );
            }
            this._connectionProperties = connectionProperties;
        }

        void IDataConnectionUIControl.Initialize( IDataConnectionProperties connectionProperties )
        {
            this.Initialize( ( DataConnectionProperties ) connectionProperties );
        }

        public void LoadProperties()
        {
            this.tbServerName.Text = ConnectionProperties[ "Host" ] as string;
            this.tbPort.Text = ConnectionProperties[ "Port" ] as string;
            this.tbDatabase.Text = ConnectionProperties[ "Database" ] as string;
            this.tbUser.Text = ConnectionProperties[ "User ID" ] as string;
            this.tbPassword.Text = ConnectionProperties[ "Password" ] as string;
            this.tbSearchPath.Text = ConnectionProperties[ "SearchPath" ] as string;
            if ( ConnectionProperties.IsComplete )
            {
                this.tbServerName.ReadOnly = true;
                this.tbDatabase.ReadOnly = true;
                this.tbUser.ReadOnly = true;
            }
        }

        private void tbServerName_Leave( object sender, EventArgs e )
        {
            if ( String.IsNullOrEmpty( tbServerName.Text ) )
                ConnectionProperties.Remove( "Host" );
            else
                ConnectionProperties[ "Host" ] = tbServerName.Text;
        }

        private void tbPort_Leave( object sender, EventArgs e )
        {
            if ( String.IsNullOrEmpty( tbPort.Text ) )
                ConnectionProperties.Remove( "Port" );
            else
                ConnectionProperties[ "Port" ] = tbPort.Text;
        }

        private void tbUser_Leave( object sender, EventArgs e )
        {
            if ( String.IsNullOrEmpty( tbUser.Text ) )
                ConnectionProperties.Remove( "User Id" );
            else
                ConnectionProperties[ "User Id" ] = tbUser.Text;
        }

        private void tbPassword_Leave( object sender, EventArgs e )
        {
            if ( String.IsNullOrEmpty( tbPassword.Text ) )
                ConnectionProperties.Remove( "Password" );
            else
                ConnectionProperties[ "Password" ] = tbPassword.Text;
        }

        private void tbDatabase_Leave( object sender, EventArgs e )
        {
            if ( String.IsNullOrEmpty( tbDatabase.Text ) )
                ConnectionProperties.Remove( "Database" );
            else
                ConnectionProperties[ "Database" ] = tbDatabase.Text;
        }

        private void tbSearchPath_Leave( object sender, EventArgs e )
        {
            if ( String.IsNullOrEmpty( tbSearchPath.Text ) )
                ConnectionProperties.Remove( "SearchPath" );
            else
                ConnectionProperties[ "SearchPath" ] = tbSearchPath.Text;
        }

        // Properties
        protected DataConnectionProperties ConnectionProperties
        {
            get
            {
                return this._connectionProperties;
            }
        }
    }
}
