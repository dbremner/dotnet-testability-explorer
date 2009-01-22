using C5;
using NUnit.Framework;
using Thinklouder.Testability.Metrics;

namespace Thinklouder.Testability.Tests.Metrics
{
    [TestFixture]
    public class ClassCostTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            methodCost1.addCostSource(new CyclomaticCost(0, Cost.Cyclomatic(1)));

            methodCost2.addCostSource(new CyclomaticCost(0, Cost.Cyclomatic(1)));
            methodCost2.addCostSource(new CyclomaticCost(0, Cost.Cyclomatic(1)));

            methodCost0.link();
            methodCost1.link();
            methodCost2.link();

            classCost0 = new ClassCost("FAKE_classInfo0", new ArrayList<MethodCost> {methodCost0});
            classCost1 = new ClassCost("FAKE_classInfo1", new ArrayList<MethodCost> {methodCost1});
            classCost2 = new ClassCost("FAKE_classInfo2", new ArrayList<MethodCost> {methodCost2});
        }

        #endregion

        private readonly MethodCost methodCost0 = new MethodCost("c.g.t.A.method0()V", 0);
        private readonly MethodCost methodCost1 = new MethodCost("c.g.t.A.method1()V", 0);
        private readonly MethodCost methodCost2 = new MethodCost("c.g.t.A.method2()V", 0);

        private ClassCost classCost0;
        private ClassCost classCost1;
        private ClassCost classCost2;

        [Test]
        public void testSumsUpTotalClassCostCorrectly()
        {
            Assert.AreEqual(0, classCost0.getTotalComplexityCost());
            Assert.AreEqual(1, classCost1.getTotalComplexityCost());
            Assert.AreEqual(2, classCost2.getTotalComplexityCost());
        }

        [Test]
        public void testClassCostSortsByDescendingCost()
        {
            IList<ClassCost> classCosts = new ArrayList<ClassCost>();
            classCosts.Add(classCost1);
            classCosts.Add(classCost0);
            classCosts.Add(classCost2);

            //Collections.sort(classCosts, new ClassCost.CostComparator(new CostModel()));
            classCosts.Sort(new ClassCost.CostComparator(new CostModel()));

            Assert.AreEqual(classCost2, classCosts[0]);
            Assert.AreEqual(classCost1, classCosts[1]);
            Assert.AreEqual(classCost0, classCosts[2]);
        }

        [Test]
        public void testGetPackageName()
        {
            var classCost0 = new ClassCost("com.a.b.c.Dee", new ArrayList<MethodCost> { methodCost0 });

            Assert.AreEqual("com.a.b.c", classCost0.getPackageName());

            classCost0 = new ClassCost("Dee", new ArrayList<MethodCost> { methodCost0 });

            Assert.AreEqual("", classCost0.getPackageName());
        }
    }
}