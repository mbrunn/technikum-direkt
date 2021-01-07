using System;

namespace TechnikumDirekt.DataAccess.Sql.Exceptions
{
    public class DataAccessUpdateException: DataAccessExceptionBase
    {
        public DataAccessUpdateException(): base() {}
        public DataAccessUpdateException(string msg): base(msg) {}
        public DataAccessUpdateException(string msg, Exception inner): base(msg, inner) {}
    }
}