using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinklouder.Testability.Tests.IL
{
    public class ArraySample
    {
        public ArraySample()
        {
            var int1 = new[] { 1, 2, 3 };
            var int2 = new int[3] { 1, 2, 3 };
            int2[0] = 0;
            var int3 = int1;
            var int4 = new int[3];
        }

        public void ArrayWithoutPropertyInitializer()
        {
            var int1 = new int[10];
            for (int i = 0; i < 10; i++)
            {
                int1[i] = i;
            }
        }

        public void ArrayWithPropertyInitializer()
        {
            var int1 = new int[10]{0,1,2,3,4,5,6,7,8,9};
        }

        public void Method()
        {
            var obj1 = new object[3];
            var obj2 = new[] { new object(), "test", 1 };

            var obj3 = new[] {new object(), "test", 1, new object(), "test", 1};
        }
    }
}
