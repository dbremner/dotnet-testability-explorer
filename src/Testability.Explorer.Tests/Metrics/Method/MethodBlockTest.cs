using System;
using C5;
using NUnit.Framework;
using Thinklouder.Testability.Metrics;
using Thinklouder.Testability.Metrics.Method.Op.Turing;
using Thinklouder.Testability.Tests.IL;

namespace Thinklouder.Testability.Tests.Metrics.Method
{
    [TestFixture]
    public class MethodBlockTest
    {
        public class Simple
        {

            private readonly object a;

            public Simple()
                : base()
            {
                a = new object();
            }
        }

        private void assertOperations(IList<Operation> block, params string[] operations)
        {
            string error = "\nExpecting:" + operations
              + "\n   Actual:" + block;
            Assert.AreEqual(operations.Length, block.Count, error);
            IList<string> expectingOps = new ArrayList<string>();
            expectingOps.AddAll(operations);
            for (int i = 0; i < operations.Length; i++)
            {
                string actual = block[i].ToString();
                Assert.IsTrue(expectingOps.Remove(actual), actual);
            }
        }

        [Test]
        public void testConstructor()
        {
            MethodInfo method = getMethod<Simple>(".ctor()System.Void");
            IList<Operation> operations = method.Operations;
            assertOperations(
                operations,
                //".ctor()System.Void",
                "System.Object..ctor()System.Void",
                "Thinklouder.Testability.Tests.Metrics.Method.MethodBlockTest/Simple::a{System.Object} <- newobj{System.Object}");
        }

        public class TryCatchFinally
        {
            public void method()
            {

                int b = 1;
                try
                {
                    b = 2;
                }
                catch (SystemException e)
                {
                    b = 3;
                }
                finally
                {
                    b = 4;
                }
                b = 5;
            }
        }

        [Test]
        public void testTryCatchBlock()
        {

            MethodInfo method = getMethod<TryCatchFinally>("method()System.Void");
            string operations = method.Operations.ToString("long", null);

            Assert.IsTrue(operations.Contains("local_0{System.Int32} <- 1{System.Int32}"));
            Assert.IsTrue(operations.Contains("local_0{System.Int32} <- 2{System.Int32}"));
            Assert.IsTrue(operations.Contains("local_0{System.Int32} <- 3{System.Int32}"));
            Assert.IsTrue(operations.Contains("local_0{System.Int32} <- 4{System.Int32}"));
            Assert.IsTrue(operations.Contains("local_0{System.Int32} <- 5{System.Int32}"));
            Assert.IsTrue(operations.Contains("local_1{System.SystemException} <- ?{System.SystemException}"));
        }

        public class IIF
        {
            private object a;

            public void method()
            {
                int b = 1;
                a = b > 0 ? null : new object();
                b = 2;
            }
        }

        private MethodInfo getMethod<T>(string methodName)
        {
            IClassRepository repo = new ClrClassRepository();
            //repo.GetClass(ClassInfo.GetFullName<object>()); // pre-cache for easier debugging.
            ClassInfo classInfo = repo.GetClass(ClassInfo.GetFullName<T>());
            return classInfo.GetMethod(methodName);
        }

        [Test]
        public void testMethodWithIIF()
        {
            //Class<IIF> clazz = IIF.class;
            MethodInfo method = getMethod<IIF>("method()System.Void");
            assertOperations(method.Operations,
                             "local_0{System.Int32} <- 1{System.Int32}",
                             "System.Object..ctor()System.Void",
                             ClassInfo.GetFullName<IIF>() + ".a{System.Object} <- newobj{System.Object}",
                             ClassInfo.GetFullName<IIF>() + ".a{System.Object} <- null{System.Object}",
                             "local_0{System.Int32} <- 2{System.Int32}");
        }

        public class SwitchTable
        {
            public void method()
            {
                int a = 0;
                switch (a)
                {
                    case 1:
                        a = 1;
                        break;
                    case 2:
                        a = 2;
                        break;
                    case 3:
                        a = 3;
                        break;
                    default:
                        a = 4;
                        break;
                }
                a = 5;
            }
        }

        [Test]
        public void testSwitchTable()
        {
            MethodInfo method = getMethod<SwitchTable>("method()System.Void");
            assertOperations(method.Operations, "a{int} <- 0{int}",
                "a{int} <- 1{int}", "a{int} <- 2{int}", "a{int} <- 3{int}",
                "a{int} <- 4{int}", "a{int} <- 5{int}");
        }

        public class CallMethods
        {
            private readonly string text = "ABC";
            private static string staticText = "abc";

            public int length()
            {
                return text.Length;
            }

            public static int staticLength()
            {
                return staticText.Length;
            }
        }

        [Test]
        public void testCallMethodsLength()
        {
            MethodInfo method = getMethod<CallMethods>("length()");
            assertOperations(method.Operations, "java.lang.string.length()I", "return ?{int}");
        }

        [Test]
        public void testCallMethodsStaticLength()
        {
            MethodInfo method = getMethod<CallMethods>("staticLength()I");
            assertOperations(method.Operations, "java.lang.string.length()I", "return ?{int}");
        }

        class Foreach
        {

            public void method()
            {
                foreach (string names in new string[0])
                {
                }
            }
        }

        [Test]
        public void testForEach()
        {
            IClassRepository repo = new ClrClassRepository();
            repo.GetClass(ClassInfo.GetFullName<Foreach>()).GetMethod("method()System.Void");
        }

        [Test]
        public void testSwitchSample()
        {
            //var methodInfo1 = getMethod<SwitchSample>("SwitchWith6Cases");
            var methodInfo2 = getMethod<SwitchSample>("SwitchWith7Cases");
            //var methodInfo3 = getMethod<SwitchSample>("SmallSwitch");
        }
    }
}