using C5;
using NUnit.Framework;
using Thinklouder.Testability.Metrics;
using Thinklouder.Testability.Metrics.Method.Op.Stack;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Tests.Metrics.Method.Op.Stack
{
    [TestFixture]
    public class StackOperationsTest
    {
        private void assertOperations<T>(params string[] expectedOps)
        {
            var actualOps = new LinkedList<Operation>();
            var methodInfo = methodForClass<T>();
            IList<Operation> operations = methodInfo.Operations;
            actualOps.AddAll(operations);

            string error = "\nExpected: " + expectedOps + "\n   Actual: " + actualOps;
            
            // TODO, assert all the instructions by sunlw
            //foreach (var expected in expectedOps)
            //{
            //    Assert.AreEqual(expected, actualOps.RemoveAt(0).ToString(), error);
            //}
        }

        private static MethodInfo methodForClass<T>()
        {
            return new ClrClassRepository().GetClass(ClassInfo.GetFullName<T>()).GetMethod(".ctor()System.Void");
        }

        private class LoadClass
        {
            public LoadClass()
            {
                this.GetHashCode();
            }
        }

        [Test]
        public void testLoad()
        {
            assertOperations<LoadClass>("System.Object..ctor()System.Void", "System.Object.GetHashCode()System.Int32");
            Assert.AreEqual("load null", new Load(-1, null).ToString());
        }

        private class PopClass
        {
            public PopClass()
            {
                long a = 1l;
                double b = 2d;
                var c = add(a, b);

                var e = long.MaxValue;
                var f = double.MaxValue;

                var var0 = 0;
                var var1 = 1;
                var var2 = 2;
                var var3 = 3;
                var var4 = 4;
                var var5 = 5;
                var var6 = 6;
                var var7 = 7;
                var var8 = 8;

                var varldci4num = 20;
                var varldci8num = 20l;
                var varldcr4num = 20f;
                var varldcr8num = 20d;
            }
        }

        static double add(long a, double b)
        {
            return 3d;
        }


        [Test]
        public void testPop()
        {
            assertOperations<PopClass>(
                "System.Object..ctor()System.Void",
                "a{System.Single} <- 1{System.Single}",
                "b{System.Double} <- 2.0{System.Double}",
                ClassInfo.GetFullName<StackOperationsTest>() + ".add(System.Single)System.Double");
            Assert.AreEqual("pop", new Pop(-1, 1).ToString());
        }
    }
}
