using System;
using System.Data;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using Microsoft.VisualStudio.Shell.Interop;
using NpgsqlTypes;

namespace Npgsql.Provider
{
    public class NpgsqlConnectionSupport : AdoDotNetConnectionSupport, IVsTrackProjectDocumentsEvents2//, IVsTrackProjectDocumentsEvents3
    {
        protected override IVsDataParameter CreateParameterCore()
        {
            var p = new NpgsqlParameter();
            p.Parameter.NpgsqlDbType = NpgsqlDbType.Varchar; //Set the NpgsqlDbType here to force UseCast...
            return p;
        }

        protected override IVsDataParameter CreateParameterFrom( System.Data.Common.DbParameter parameter )
        {
            var p = new NpgsqlParameter( parameter );
            p.Parameter.NpgsqlDbType = NpgsqlDbType.Varchar; //Set the NpgsqlDbType here to force UseCast...
            return p;
        }

        protected override System.Data.Common.DbCommand GetCommand( string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout )
        {
            var comm = new NpgsqlCommand( command, Connection as NpgsqlConnection );
            comm.CommandTimeout = commandTimeout;
            if ( parameters != null )
            {
                foreach ( var parameter in parameters )
                {
                    var par = new Npgsql.NpgsqlParameter( parameter.Name, parameter.Value );

                    //par.NpgsqlDbType = NpgsqlObjectSelector.MapNativeToNpgsqlDbType( parameter.Type );
                    if ( parameter.Descriptor.IsDerived )
                    {
                        par.IsNullable = parameter.Descriptor.IsNullable;
                    }
                    switch ( parameter.Direction )
                    {
                        case DataParameterDirection.In:
                            par.Direction = ParameterDirection.Input;
                            break;
                        case DataParameterDirection.Out:
                            par.Direction = ParameterDirection.Output;
                            break;
                        case DataParameterDirection.InOut:
                            par.Direction = ParameterDirection.InputOutput;
                            break;
                        case DataParameterDirection.ReturnValue:
                            par.Direction = ParameterDirection.ReturnValue;
                            break;
                        case DataParameterDirection.Unknown:
                        default:
                            break;
                    }
                    par.Size = parameter.Size;
                    comm.Parameters.Add( par );
                }
            }

            switch ( commandType )
            {
                case DataCommandType.Text:
                    comm.CommandType = CommandType.Text;
                    break;
                case DataCommandType.Prepared:
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Prepare();
                    break;
                case DataCommandType.Table:
                    comm.CommandType = CommandType.TableDirect;
                    break;
                case DataCommandType.ScalarFunction:
                case DataCommandType.Procedure:
                case DataCommandType.TabularFunction:
                    comm.CommandType = CommandType.StoredProcedure;
                    break;
                default:
                    throw new ArgumentOutOfRangeException( "commandType" );
            }

            return comm;

            //return base.GetCommand( command, commandType, parameters, commandTimeout );
        }

        protected override IVsDataReader ExecuteCore( string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout )
        {
            return base.ExecuteCore( command, commandType, parameters, commandTimeout );
        }

        #region Implementation of IVsTrackProjectDocumentsEvents2

        int IVsTrackProjectDocumentsEvents2.OnAfterAddDirectoriesEx( int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterAddFilesEx( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRemoveDirectories( int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRemoveFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRenameDirectories( int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRenameFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterSccStatusChanged( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryAddDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryAddFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRemoveDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRemoveFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRenameDirectories( IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRenameFiles( IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults )
        {
            return -2147467263;
        }

        #endregion Implementation of IVsTrackProjectDocumentsEvents2

        #region Implementation of IVsTrackProjectDocumentsEvents3
        /*
        int IVsTrackProjectDocumentsEvents3.HandsOffFiles( uint grfRequiredAccess, int cFiles, string[] rgpszMkDocuments )
        {
            return 0;
        }

        int IVsTrackProjectDocumentsEvents3.HandsOnFiles( int cFiles, string[] rgpszMkDocuments )
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents3.OnBeginQueryBatch()
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents3.OnCancelQueryBatch()
        {
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents3.OnEndQueryBatch( out int pfActionOK )
        {
            pfActionOK = 0;
            return -2147467263;
        }

        int IVsTrackProjectDocumentsEvents3.OnQueryAddFilesEx( IVsProject pProject, int cFiles, string[] rgpszNewMkDocuments, string[] rgpszSrcMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults )
        {
            return -2147467263;
        }
        */
        #endregion Implementation of IVsTrackProjectDocumentsEvents3
    }
}
