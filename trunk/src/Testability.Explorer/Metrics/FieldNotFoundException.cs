using System;

namespace Thinklouder.Testability.Metrics
{
    public class FieldNotFoundException : Exception
    {
        public ClassInfo ClassInfo { get; private set; }
        public string FieldName { get; private set; }

        public FieldNotFoundException(ClassInfo classInfo, string fieldName)
            : base("Field '" + fieldName + "' not found in '" + classInfo.Name + "'.")
        {
            ClassInfo = classInfo;
            FieldName = fieldName;
        }
    }
}