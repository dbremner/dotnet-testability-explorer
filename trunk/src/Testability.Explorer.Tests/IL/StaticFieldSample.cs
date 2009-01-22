using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thinklouder.Testability.Metrics;

namespace Thinklouder.Testability.Tests.IL
{
    /*
    .method private hidebysig specialname rtspecialname static 
        void  .cctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     bool Thinklouder.Testability.Tests.IL.StaticFieldSample::flag
      IL_0006:  ret
    } // end of method StaticFieldSample::.cctor
    */
    public class StaticFieldSample
    {
        private static bool flag = false;
        private static IClassRepository repo = new ClrClassRepository();
        private static object obj = new object();

        private class PopClass
        {
            static PopClass()
            {
                var a = 1l;
                var b = 2d;
                var c = add(a, b);

                var e = long.MaxValue;
                var f = double.MaxValue;
            }
        }

        static double add(long a, double b)
        {
            return 3d;
        }
    }
}
