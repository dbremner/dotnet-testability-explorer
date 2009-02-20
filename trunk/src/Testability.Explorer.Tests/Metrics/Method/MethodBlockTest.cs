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
                "Thinklouder.Testability.Tests.Metrics.Method.MethodBlockTest/Simple::a{System.Object} <- newobj{System.Object}"
                );
        }

        public class TryCatchFinally
        {
            public int method()
            {
                int b = 1;
                try
                {
                    b = 2;
                }
                catch (SystemException e)
                {
                    b = 3;
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    b = 4;
                }
                b = 5;

                return b;
            }
        }

        [Test]
        public void testTryCatchBlock()
        {

            MethodInfo method = getMethod<TryCatchFinally>("method()System.Int32");
            string operations = method.Operations.ToString("long", null);

            Assert.IsTrue(operations.Contains("local_0{System.Int32} <- 1{System.Int32}"), "local_0{System.Int32} <- 1{System.Int32}");
            Assert.IsTrue(operations.Contains("local_0{System.Int32} <- 2{System.Int32}"), "local_0{System.Int32} <- 2{System.Int32}");
            Assert.IsTrue(operations.Contains("local_0{System.Int32} <- 3{System.Int32}"), "local_0{System.Int32} <- 3{System.Int32}");
            Assert.IsTrue(operations.Contains("local_0{System.Int32} <- 4{System.Int32}"), "local_0{System.Int32} <- 4{System.Int32}");
            Assert.IsTrue(operations.Contains("local_0{System.Int32} <- 5{System.Int32}"), "local_0{System.Int32} <- 5{System.Int32}");
            Assert.IsTrue(operations.Contains("local_1{System.SystemException} <- ?{System.SystemException}"),"local_1{System.SystemException} <- ?{System.SystemException}");
        }

        public class IIF
        {
            private object a;

            //public void method()
            //{
            //    int b = 1;
            //    a = b > 0 ? null : new object();
            //    b = 2;
            //}

            public void method1()
            {
                int b = 1;
                int c = 2;
                a = b < c ? null : new object();
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
            MethodInfo method = getMethod<IIF>("method1()System.Void");
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

            public int length(int a)
            {
                return text.Length + a;
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

        public class VarInsn
        {
            public VarInsn()
            {
                var obj = new object();
                var dt = new DateTime();
            }
        }
        
        [Test]
        public void testVarInsn()
        {
            var method = getMethod<VarInsn>(".ctor()System.Void");
            var stackOperations = method.Operations.ToString("long", null);
        }

        public class ParamInsn
        {
            private object obj1;
            private object obj2;
            private object obj3;
            private object obj4;
            private object obj5;
            private object obj6;
            private object obj;

            private ParamInsn1 obj7;

            public static object objStatic;

            public ParamInsn(object obj)
            {
                this.obj = obj;
            }

            public ParamInsn(object obj1, object obj2, object obj3, object obj4, object obj5, object obj6)
            {
                this.obj1 = obj1;
                this.obj2 = obj2;
                this.obj3 = obj3;
                this.obj4 = obj4;
                this.obj5 = obj5;
                this.obj6 = obj6;
            }

            public void setInstance(object obj1, object obj2, object obj3)
            {
                this.obj1 = obj1;
                this.obj2 = obj2;
                this.obj3 = obj3;
            }

            public object setInstanceReturn(object obj1, object obj2)
            {
                this.obj7.obj1 = obj1;
                this.obj2 = obj2;
                return this.obj2;
            }

            public static void setStatic(object obj1)
            {
                objStatic = obj1;
            }

            public static object setStaticReturnArg(object obj1)
            {
                objStatic = obj1;
                return obj1;
            }

            public static object setStaticReturnField(object obj1)
            {
                objStatic = obj1;
                return objStatic;
            }

            public object setInstanceFieldReturnField(object obj)
            {
                object localObject = obj;
                this.obj1 = obj;
                return this.obj1;
            }

            public object setInstanceFieldReturnLocal(object obj)
            {
                object localObject = obj;
                this.obj1 = obj;
                return localObject;
            }
        }

        public class ParamInsn1
        {
            public object obj1;
        }
        
        [Test]
        public void testParamInsn()
        {
            var method = getMethod<ParamInsn>(".ctor(System.Object,System.Object,System.Object,System.Object,System.Object,System.Object)System.Void");
            var stackOperations = method.Operations.ToString("long", null);
        }
    }
}