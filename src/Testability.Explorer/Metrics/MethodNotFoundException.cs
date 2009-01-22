using System;

namespace Thinklouder.Testability.Metrics
{
    public class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(ClassInfo classInfo, string methodName)
            : base("Method '" + methodName + "' not found in class '"
                   + classInfo.Name + "'")
        {
            this.MethodName = methodName;
            this.ClassInfo = classInfo;
        }

        public ClassInfo ClassInfo { get; private set; }

        public string MethodName { get; private set; }
    }
}