using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Npgsql.Provider.Properties;
using SQLite.Designer;
using SQLite.Designer.Editors;

namespace Npgsql.Provider
{
    internal class NpgsqlDataViewCommandProvider : DataViewCommandProvider
    {
        public static CommandID NewTable = new CommandID( new Guid( "501822E1-B5AF-11d0-B4DC-00A0C91506EF" ), 0x3520 );
        public static CommandID DesignTable = new CommandID( new Guid( "501822E1-B5AF-11d0-B4DC-00A0C91506EF" ), 12291 );

        protected override MenuCommand CreateCommand( int itemId, CommandID commandId, object[] parameters )
        {
            EventHandler statusHandler = null;
            EventHandler handler = null;
            MenuCommand command = null;

            if ( commandId.Equals( NewTable ) )
            {
                if ( statusHandler == null )
                {
                    statusHandler = delegate
                    {
                        command.Visible = command.Enabled = CanOpen( this.Site.ExplorerConnection.FindNode( itemId ) );
                        if ( command.Visible )
                        {
                            command.Properties[ "Text" ] = "Open this";
                        }
                    };
                }
                if ( handler == null )
                {
                    handler = delegate
                    {
                        //this.OnOpen( this.Site.ExplorerConnection.FindNode( itemId ) );
                        this.Open( this.Site.ExplorerConnection.FindNode( itemId ), false );
                    };
                }
                command = new DataViewMenuCommand( itemId, commandId, statusHandler, handler );

            }
            else if ( commandId.Equals( DesignTable ) )
            {
                if ( statusHandler == null )
                {
                    statusHandler = delegate
                    {
                        command.Visible = command.Enabled = CanOpen( this.Site.ExplorerConnection.FindNode( itemId ) );
                        if ( command.Visible )
                        {
                            command.Properties[ "Text" ] = "Design Table";
                        }
                    };
                }
                if ( handler == null )
                {
                    handler = delegate
                    {
                        //this.OnOpen( this.Site.ExplorerConnection.FindNode( itemId ) );
                        this.Design( this.Site.ExplorerConnection.FindNode( itemId ), false );
                    };
                }
                command = new DataViewMenuCommand( itemId, commandId, statusHandler, handler );

            }

            if ( command != null )
            {
                return command;
            }

            return base.CreateCommand( itemId, commandId, parameters );
        }

        private IVsWindowFrame Design( IVsDataExplorerNode node, bool doNotShowWindow )
        {
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider provider = Site.ServiceProvider as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
            IVsUIShell shell = Site.ServiceProvider.GetService( typeof( IVsUIShell ) ) as IVsUIShell;
            //IVsUIHierarchy hier = this.Site as IVsUIHierarchy;
            IVsWindowFrame frame = null;

            if ( shell != null )
            {
                var name = node.Name.Substring( node.Name.LastIndexOf( '.' ) + 1 ).Replace( "]", "" );
                TableDesignerDoc form = new TableDesignerDoc( node.ItemId, this.Site, node.Parent.Parent.Name, name );
                IntPtr formptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject( form );
                Guid empty = Guid.Empty;
                FakeHierarchy fake = new FakeHierarchy( form, null );

                int code = shell.CreateDocumentWindow(
                  0, // (uint)(__VSCREATEDOCWIN.CDW_fCreateNewWindow | __VSCREATEDOCWIN.CDW_RDTFLAGS_MASK) | (uint)(_VSRDTFLAGS.RDT_CanBuildFromMemory | _VSRDTFLAGS.RDT_NonCreatable | _VSRDTFLAGS.RDT_VirtualDocument | _VSRDTFLAGS.RDT_DontAddToMRU),
                  form.Name, fake, ( uint ) node.ItemId, formptr, formptr, ref empty, null, ref NpgsqlCommandHandler.guidTableDesignContext, provider, "", form.Caption, null, out frame );

                if ( frame != null && !doNotShowWindow )
                {
                    object ret;
                    int prop = ( int ) __VSFPROPID.VSFPROPID_Caption;

                    code = frame.GetProperty( prop, out ret );

                    code = frame.Show();
                }
            }

            return frame;            
        }

        private bool CanOpen( IVsDataExplorerNode findNode )
        {
            return true;
        }

        internal IVsWindowFrame Open( IVsDataExplorerNode node, bool doNotShowWindow )
        {
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider provider = Site.ServiceProvider as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
            IVsUIShell shell = Site.ServiceProvider.GetService( typeof( IVsUIShell ) ) as IVsUIShell;
            //IVsUIHierarchy hier = this.Site as IVsUIHierarchy;
            IVsWindowFrame frame = null;

            if ( shell != null )
            {
                TableDesignerDoc form = new TableDesignerDoc( node.ItemId, this.Site, node.Parent.Name, "New_Table" );
                IntPtr formptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject( form );
                Guid empty = Guid.Empty;
                FakeHierarchy fake = new FakeHierarchy( form, null );

                int code = shell.CreateDocumentWindow(
                  0, // (uint)(__VSCREATEDOCWIN.CDW_fCreateNewWindow | __VSCREATEDOCWIN.CDW_RDTFLAGS_MASK) | (uint)(_VSRDTFLAGS.RDT_CanBuildFromMemory | _VSRDTFLAGS.RDT_NonCreatable | _VSRDTFLAGS.RDT_VirtualDocument | _VSRDTFLAGS.RDT_DontAddToMRU),
                  form.Name, fake, ( uint ) node.ItemId, formptr, formptr, ref empty, null, ref NpgsqlCommandHandler.guidTableDesignContext, provider, "", form.Caption, null, out frame );

                if ( frame != null && !doNotShowWindow )
                {
                    object ret;
                    int prop = ( int ) __VSFPROPID.VSFPROPID_Caption;

                    code = frame.GetProperty( prop, out ret );

                    code = frame.Show();
                }
            }

            return frame;
        }



    }
}
