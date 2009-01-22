using System;

namespace Thinklouder.Testability.Metrics
{
    public class LoadAssemblyException : Exception
    {
        public LoadAssemblyException(string assemblyPath, Exception ex) : base("Load assembly '" + assemblyPath + "' error : ", ex)
        {
            
        }
    }
}