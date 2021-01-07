using System;

namespace TechnikumDirekt.DataAccess.Sql.Exceptions
{
    public class DataAccessNotPossibleException: DataAccessExceptionBase
    {
        public DataAccessNotPossibleException(): base() {}
        public DataAccessNotPossibleException(string msg): base(msg) {}
        public DataAccessNotPossibleException(string msg, Exception inner): base(msg, inner) {}
    }
}