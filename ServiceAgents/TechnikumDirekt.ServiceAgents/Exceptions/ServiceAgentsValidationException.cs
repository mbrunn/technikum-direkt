using System;
using TechnikumDirekt.ServiceAgents.Exceptions;

namespace TechnikumDirekt.ServiceAgents.Exceptions
{
    public class ServiceAgentsValidationException: ServiceAgentsExceptionBase
    {
        public ServiceAgentsValidationException(): base() {}
        public ServiceAgentsValidationException(string msg): base(msg) {}
        public ServiceAgentsValidationException(string msg, Exception inner): base(msg, inner) {}
    }
}