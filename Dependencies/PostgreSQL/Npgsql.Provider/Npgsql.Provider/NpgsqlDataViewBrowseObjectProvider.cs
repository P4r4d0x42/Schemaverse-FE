using Microsoft.VisualStudio.Data.Framework;

namespace Npgsql.Provider
{
    public class NpgsqlDataViewBrowseObjectProvider : DataViewBrowseObjectProvider
    {
        public override object CreateBrowseObject( int itemId, object autoBrowseObj )
        {
            return base.CreateBrowseObject( itemId, autoBrowseObj );
        }
    }
}
