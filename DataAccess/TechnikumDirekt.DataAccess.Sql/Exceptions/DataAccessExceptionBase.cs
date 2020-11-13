using System;

namespace TechnikumDirekt.DataAccess.Sql.Exceptions
{
    public abstract class DataAccessExceptionBase: Exception
    {
        protected DataAccessExceptionBase(): base() {}
        protected DataAccessExceptionBase(string msg): base(msg) {}
        protected DataAccessExceptionBase(string msg, Exception inner): base(msg, inner) {}
    }
}