using System;

namespace TechnikumDirekt.ServiceAgents.Exceptions
{
    public class ServiceAgentsBadResponseException : ServiceAgentsExceptionBase
    {
        public ServiceAgentsBadResponseException(): base() {}
        public ServiceAgentsBadResponseException(string msg): base(msg) {}
        public ServiceAgentsBadResponseException(string msg, Exception inner): base(msg, inner) {}
    }
}