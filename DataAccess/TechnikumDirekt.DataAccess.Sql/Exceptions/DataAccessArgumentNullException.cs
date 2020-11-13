using System;

namespace TechnikumDirekt.DataAccess.Sql.Exceptions
{
    public class DataAccessArgumentNullException: DataAccessExceptionBase
    {
        public DataAccessArgumentNullException(): base() {}
        public DataAccessArgumentNullException(string msg): base(msg) {}
        public DataAccessArgumentNullException(string msg, Exception inner): base(msg, inner) {}
    }
}