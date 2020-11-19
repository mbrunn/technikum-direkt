using System;

namespace TechnikumDirekt.BusinessLogic.Exceptions
{
    public class BusinessLogicFutureHopsPrediction : BusinessLogicExceptionBase
    {
        public BusinessLogicFutureHopsPrediction(): base() {}
        public BusinessLogicFutureHopsPrediction(string msg): base(msg) {}
        public BusinessLogicFutureHopsPrediction(string msg, Exception inner): base(msg, inner) {}
    }
}