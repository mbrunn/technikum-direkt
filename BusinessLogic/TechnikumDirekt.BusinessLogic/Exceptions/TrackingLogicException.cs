using System;

namespace TechnikumDirekt.BusinessLogic.Exceptions
{
    public class TrackingLogicException: Exception
    {
        public TrackingLogicException(): base() {}
        public TrackingLogicException(string msg): base(msg) {}
    }
}