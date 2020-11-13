using System;

namespace TechnikumDirekt.BusinessLogic.Exceptions
{
    public class BusinessLogicNotFoundException: BusinessLogicExceptionBase
    {
        public BusinessLogicNotFoundException(): base() {}
        public BusinessLogicNotFoundException(string msg): base(msg) {}
        public BusinessLogicNotFoundException(string msg, Exception inner): base(msg, inner) {}
    }
}