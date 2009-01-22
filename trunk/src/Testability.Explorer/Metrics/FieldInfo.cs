using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinklouder.Testability.Metrics
{
    public class FieldInfo : Variable
    {
        private readonly ClassInfo classInfo;
        private readonly bool isPrivate;

        public FieldInfo(ClassInfo classInfo, string name, Type type, bool isFinal, bool isGlobal, bool isPrivate)
            : base(name, type, isFinal, isGlobal)
        {
            this.classInfo = classInfo;
            this.isPrivate = isPrivate;
        }

        public override string ToString()
        {
            return string.Format("{0}::{1}{{{2}}}", (classInfo == null ? "null" : classInfo.ToString()), GetName(), base.Type);
        }

        public bool IsPrivate()
        {
            return isPrivate;
        }

    }
}
