using System;

namespace TechnikumDirekt.ServiceAgents.Exceptions
{
    public class ServiceAgentsBadArgumentException : ServiceAgentsExceptionBase
    {
        public ServiceAgentsBadArgumentException(): base() {}
        public ServiceAgentsBadArgumentException(string msg): base(msg) {}
        public ServiceAgentsBadArgumentException(string msg, Exception inner): base(msg, inner) {}
    }
}