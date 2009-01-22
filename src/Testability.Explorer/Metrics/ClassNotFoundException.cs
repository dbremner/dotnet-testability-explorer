using System;

namespace Thinklouder.Testability.Metrics
{
    public class ClassNotFoundException : Exception
    {
        public string ClassName { get; private set; }
        public ClassNotFoundException(string className) : base("Class '" + className + "' not found.")
        {
            ClassName = className;
        }
    }
}