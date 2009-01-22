using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinklouder.Testability.Metrics
{
    public interface IClassRepository
    {
        ClassInfo GetClass(string clazzName);

        void AddClass(ClassInfo classInfo);

        bool ExistClass(string className);

        /// <summary>
        /// The name of the module we current analyze on
        /// 
        /// TODO, we might support multiple module analyze concurrently, 
        /// module can reference to each other which is normal in daily life
        /// </summary>
        string ScopeName { get; }
    }
}
