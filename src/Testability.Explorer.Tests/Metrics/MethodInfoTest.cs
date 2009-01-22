using System;
using System.Linq;
using System.Text;
using C5;
using NUnit.Framework;
using Thinklouder.Testability.Metrics;

namespace Thinklouder.Testability.Tests.Metrics
{
    [TestFixture]
    public class MethodInfoTest
    {
        private readonly IList<int> cost1 = new ArrayList<int>();
        [Test]
        public void TestGetFullName()
        {
            MethodInfo method;

            method = new MethodInfo(new ClassInfo("com.foo.bar", false,
                null, null), "method", 0, "(System.String)System.Void", null, null, null,
                Visibility.NULL, cost1, null, false);
            Assert.AreEqual("System.Void method(System.String)", method.GetFullName());

            method = new MethodInfo(new ClassInfo("f.a.b", false, null, null),
                "mymethod", 0, "(int, double, java.lang.Thread)java.lang.Object", null, null,
                null, Visibility.NULL, cost1, null, false);
            Assert.AreEqual("java.lang.Object mymethod(int, double, java.lang.Thread)", method.GetFullName());

            method = new MethodInfo(new ClassInfo("c.b.ui.UI$ViewHandler", false, null,
                null), "<clinit>", 0, "()V", null, null, null, Visibility.NULL, cost1, null, false);
            Assert.AreEqual("c.b.ui.UI$ViewHandler()", method.GetFullName());

            method = new MethodInfo(new ClassInfo("c.b.ui.UI$ViewHandler", false, null,
                null), "<init>", -1, "(int)System.Void", null, null, null, Visibility.NULL, cost1, null, false);
            Assert.AreEqual("c.b.ui.UI$ViewHandler(int)", method.GetFullName());
        }

        [Test]
        public void TestDeconstructParameters()
        {
            MethodInfo method;
                
            method = new MethodInfo(null, null, 0, null, null, null, null,
                Visibility.NULL, cost1, null, false);

            Assert.AreEqual("int", method.DeconstructParameters("I"));
            Assert.AreEqual("double[][][]", method.DeconstructParameters("[[[D"));
            Assert.AreEqual("java.lang.Object", method.DeconstructParameters("Ljava.lang.Object;"));

        }
    }
}
