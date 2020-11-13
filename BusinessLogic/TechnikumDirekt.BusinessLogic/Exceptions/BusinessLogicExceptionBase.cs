using System;

namespace TechnikumDirekt.BusinessLogic.Exceptions
{
    public abstract class BusinessLogicExceptionBase: Exception
    {
        protected BusinessLogicExceptionBase(): base() {}
        protected BusinessLogicExceptionBase(string msg): base(msg) {}
        protected BusinessLogicExceptionBase(string msg, Exception inner): base(msg, inner) {}
    }
}