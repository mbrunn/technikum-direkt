using System;

namespace TechnikumDirekt.DataAccess.Sql.Exceptions
{
    public class DataAccessAddException: DataAccessExceptionBase
    {
        public DataAccessAddException(): base() {}
        public DataAccessAddException(string msg): base(msg) {}
        public DataAccessAddException(string msg, Exception inner): base(msg, inner) {}
    }
}