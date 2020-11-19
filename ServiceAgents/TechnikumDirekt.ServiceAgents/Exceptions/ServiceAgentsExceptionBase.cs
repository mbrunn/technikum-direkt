using System;

namespace TechnikumDirekt.ServiceAgents.Exceptions
{
    public abstract class ServiceAgentsExceptionBase : Exception
    {
        protected ServiceAgentsExceptionBase(): base() {}
        protected ServiceAgentsExceptionBase(string msg): base(msg) {}
        protected ServiceAgentsExceptionBase(string msg, Exception inner): base(msg, inner) {}
    }
}