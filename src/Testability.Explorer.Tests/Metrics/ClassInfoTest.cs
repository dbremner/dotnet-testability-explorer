#region

using System;
using C5;
using Mono.Cecil;
using NUnit.Framework;
using Thinklouder.Testability.Metrics;

#endregion

namespace Thinklouder.Testability.Tests.Metrics
{
    [TestFixture]
    public class ClassInfoTest
    {
        private readonly IClassRepository repo = new ClrClassRepository();

        public class EmptyClass
        {
            #region Nested type: InnerEmptyClass

            public class InnerEmptyClass
            {
            }

            #endregion
        }

        public class SingleMethodClass
        {
            public void MethodA()
            {}

            public IList<object> MethodB()
            {
                return null;
            }

            public void MethodC(IList<object> objects)
            {}
        }

        [Test]
        public void TestFoundClass()
        {
            const string name = "Thinklouder.Testability.Tests.Metrics.ClassInfoTest/SingleMethodClass";
            var classInfo = repo.GetClass(name);
            Assert.AreEqual(name, classInfo.Name);
        }

        [Test]
        public void TestMethodNotFoundException()
        {
            var className = ClassInfo.GetFullName<EmptyClass>();
            var methodName = "IDontExistMethod()V";
            var clazz = repo.GetClass(className);
            try
            {
                clazz.GetMethod(methodName);
            }
            catch (MethodNotFoundException e)
            {
                Assert.IsTrue(e.Message.Contains(methodName));
                Assert.IsTrue(e.Message.Contains(className));
                Assert.AreEqual(methodName, e.MethodName);
                Assert.AreEqual(clazz, e.ClassInfo);
            }
        }

        [Test]
        public void TestNonExistingClass()
        {
            const string name = "IDontExistClass";
            try
            {
                repo.GetClass(name);
            }
            catch (ClassNotFoundException ex)
            {
                Assert.IsTrue(ex.Message.Contains(name));
                Assert.AreEqual(name, ex.ClassName);
            }
        }

        [Test]
        public void TestParseEmptyClass()
        {
            var className = ClassInfo.GetFullName<EmptyClass>();
            var clazz = repo.GetClass(className);
            Assert.AreEqual(className, clazz.Name);
            Assert.AreEqual(className, clazz.ToString());
            Assert.AreSame(clazz, repo.GetClass(className));
        }

        [Test]
        public void TestParseSingleMethodClass()
        {
            var className = ClassInfo.GetFullName<SingleMethodClass>();
            //var methodName = "MethodA()V";
            var methodName = "MethodA()System.Void";
            var clazz = repo.GetClass(className);
            var method = clazz.GetMethod(methodName);
            Assert.AreEqual(methodName, method.GetNameDescriptor());
            Assert.AreEqual("System.Void MethodA()", method.ToString());
            Assert.AreSame(method, clazz.GetMethod(methodName));
        }
        
        public class MoreMethodsClass
        {
            public IList<int> GetInts(int arg1, IList<object> arg2, out object arg3, double[][] arg4, params string[] argz)
            {
                arg3 = new object();
                return null;
            }
        }

        [Test]
        public void TestParseMoreMethods()
        {
            var className = ClassInfo.GetFullName<MoreMethodsClass>();
            //var methodName = "MethodA()System.Void";
            var clazz = repo.GetClass(className);
            //var method = clazz.GetMethod(methodName);
            //Assert.AreEqual(methodName, method.GetNameDescriptor());
            //Assert.AreEqual("System.Void MethodA()", method.ToString());
            //Assert.AreSame(method, clazz.GetMethod(methodName));

            foreach(MethodInfo info in clazz.GetMethods())
            {
                Console.Write(info.Descriptor);
                Console.WriteLine(info.ToString());
            }
        }

        [Test]
        public void TestFieldNotFound()
        {
            var fieldName = "IDontExistField";
            var className = ClassInfo.GetFullName<EmptyClass>();
            var clazz = repo.GetClass(className);
            try
            {
                clazz.GetField(fieldName);
            }
            catch (FieldNotFoundException e)
            {
                Assert.IsTrue(e.Message.Contains(fieldName));
                Assert.IsTrue(e.Message.Contains(className));
                Assert.AreEqual(fieldName, e.FieldName);
                Assert.AreEqual(clazz, e.ClassInfo);
            }
        }

        public class SingleFieldClass
        {
            object fieldA;
        }

        [Test]
        public void TestParseFields()
        {
            var fieldName = "fieldA";
            var className = ClassInfo.GetFullName<SingleFieldClass>();
            var clazz = repo.GetClass(className);
            var field = clazz.GetField("fieldA");

            Assert.AreEqual(fieldName, field.Name);
            Assert.AreEqual(className+"::"+fieldName+"{System.Object}", field.ToString());
            Assert.AreSame(field, clazz.GetField(fieldName));
        }

        public class FieldWithDefaultClass
        {
            private short fieldShort;
            private int fieldInt;
            private long fieldLong;
            private double fieldDouble;
            private float fieldFloat;

            private bool fieldBool;
            private byte fieldByte;
            private char fieldChar;
            private string fieldString;

            private IntPtr var9;
        }

        [Test]
        public void TestParseField2()
        {
            var className = ClassInfo.GetFullName<FieldWithDefaultClass>();
            var clazz = repo.GetClass(className);
        }



        public class LocalVarsClass
        {
            public void Method(){}

            public static void StaticMethod(){}

            public void Method3(object a, int b, int[] c)
            {
                object d = null;
                a = d;
            }

            public static void StaticMethod3(object a, int b, int[] c)
            {
                object d = null;
                a = d;
            }
        }

        [Test]
        public void TestLocalVarsMethod()
        {
            
        }

        public class ClassWithCollectionField
        {
            private bool[] fieldBoolArray;
            private bool[] fieldBoolArray5;

            private bool[][] fieldBoolArrayArray;

            private Cost cost;

            private IList<Cost> costs;

            private IntPtr pointer;

            private TestDelegate testDelegate;
        }

        public delegate void TestDelegate();

        [Test]
        public void PrintTypeReferenceInfo()
        {
            var className = ClassInfo.GetFullName<ClassWithCollectionField>();
            var clazz = repo.GetClass(className);

            TypeReference typeReference = clazz.GetTypeReference() as TypeReference;

            Assert.IsNotNull(typeReference);
            
            Console.WriteLine("ToString()\t:" + typeReference);
            Console.WriteLine("Name\t:" + typeReference.Name);
            Console.WriteLine("FullName\t:" + typeReference.FullName);
            Console.WriteLine("Namespace\t:" + typeReference.Namespace);
            Console.WriteLine("Scope\t:" + typeReference.Scope);

            foreach (FieldInfo fieldInfo in clazz.GetFields())
            {
                ////fieldInfo.Name
                //Console.WriteLine("ToString()\t:" + fieldInfo);
                //Console.WriteLine("Name\t:" + fieldInfo.Name);
                //Console.WriteLine("Type\t:" + fieldInfo.Type);
                //Console.WriteLine("TypeReference\t:" + fieldInfo.Type.TypeReference);
                //Console.WriteLine("IsValueType\t:" + fieldInfo.Type.TypeReference.IsValueType);

                //Console.Write("Current Type is:");
                //if(fieldInfo.Type.TypeReference is Mono.Cecil.GenericInstanceType)
                //{
                //    Console.WriteLine("Mono.Cecil.GenericInstanceType");
                //}
                //else if ( fieldInfo.Type.TypeReference is Mono.Cecil.ArrayType )
                //{
                //    Console.WriteLine("Mono.Cecil.ArrayType");
                //}
                //else if ( fieldInfo.Type.TypeReference is Mono.Cecil.SentinelType )
                //{
                //    Console.WriteLine("Mono.Cecil.SentinelType");
                //}
                //else if ( fieldInfo.Type.TypeReference is Mono.Cecil.FunctionPointerType )
                //{
                //    Console.WriteLine("Mono.Cecil.FunctionPointerType");
                //}
                //else if ( fieldInfo.Type.TypeReference is Mono.Cecil.PointerType )
                //{
                //    Console.WriteLine("Mono.Cecil.PointerType");
                //}
                //else if ( fieldInfo.Type.TypeReference is Mono.Cecil.TypeDefinition )
                //{
                //    Console.WriteLine("Mono.Cecil.TypeDefinition(" + fieldInfo.Type.TypeReference + ")");
                //    Console.WriteLine("\tBaseType:" + ((TypeDefinition)fieldInfo.Type.TypeReference).BaseType + ")");
                //}
                //else 
                //{
                //    Console.WriteLine("unknown type(" + fieldInfo.Type.TypeReference + ")");
                //}
            }
        }

        public class ClassWithNumericOperator
        {
            public int Increment()
            {
                int i = 0;
                int b = i++;
                return b;
            }
        }
    }
}