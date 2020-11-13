using System;

namespace TechnikumDirekt.BusinessLogic.Exceptions
{
    public class BusinessLogicBadArgumentException: BusinessLogicExceptionBase
    {
        public BusinessLogicBadArgumentException(): base() {}
        public BusinessLogicBadArgumentException(string msg): base(msg) {}
        public BusinessLogicBadArgumentException(string msg, Exception inner): base(msg, inner) {}
    }
}