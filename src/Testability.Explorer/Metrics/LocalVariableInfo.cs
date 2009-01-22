using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinklouder.Testability.Metrics
{
    public class LocalVariableInfo : Variable
    {
        public LocalVariableInfo(string name, Type type) : base(name, type, false, false)
        {
            
        }
    }
}
