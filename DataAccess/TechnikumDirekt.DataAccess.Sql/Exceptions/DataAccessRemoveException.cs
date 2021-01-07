using System;

namespace TechnikumDirekt.DataAccess.Sql.Exceptions
{
    public class DataAccessRemoveException: DataAccessExceptionBase
    {
        public DataAccessRemoveException(): base() {}
        public DataAccessRemoveException(string msg): base(msg) {}
        public DataAccessRemoveException(string msg, Exception inner): base(msg, inner) {}
    }
}