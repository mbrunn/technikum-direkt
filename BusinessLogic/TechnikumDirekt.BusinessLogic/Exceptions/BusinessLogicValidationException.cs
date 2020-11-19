using System;

namespace TechnikumDirekt.BusinessLogic.Exceptions
{
    public class BusinessLogicValidationException: BusinessLogicExceptionBase
    {
        public BusinessLogicValidationException(): base() {}
        public BusinessLogicValidationException(string msg): base(msg) {}
        public BusinessLogicValidationException(string msg, Exception inner): base(msg, inner) {}
    }
}