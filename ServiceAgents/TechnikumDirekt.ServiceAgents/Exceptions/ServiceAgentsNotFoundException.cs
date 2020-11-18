using System;
using TechnikumDirekt.ServiceAgents.Exceptions;

namespace TechnikumDirekt.ServiceAgents.Exceptions
{
    public class ServiceAgentsNotFoundException: ServiceAgentsExceptionBase
    {
        public ServiceAgentsNotFoundException(): base() {}
        public ServiceAgentsNotFoundException(string msg): base(msg) {}
        public ServiceAgentsNotFoundException(string msg, Exception inner): base(msg, inner) {}
    }
}