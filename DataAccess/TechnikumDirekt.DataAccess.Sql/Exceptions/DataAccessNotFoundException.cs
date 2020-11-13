using System;

namespace TechnikumDirekt.DataAccess.Sql.Exceptions
{
    public class DataAccessNotFoundException: DataAccessExceptionBase
    {
        public DataAccessNotFoundException(): base() {}
        public DataAccessNotFoundException(string msg): base(msg) {}
        public DataAccessNotFoundException(string msg, Exception inner): base(msg, inner) {}
    }
}