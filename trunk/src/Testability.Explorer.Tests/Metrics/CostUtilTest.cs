using NUnit.Framework;
using Thinklouder.Testability.Metrics;

namespace Thinklouder.Testability.Tests.Metrics
{
    [TestFixture]
    public class CostUtilTest
    {
        private MetricComputerClrDecorator decoratedComputer;
        private readonly IClassRepository repo = new ClrClassRepository();

        [SetUp]
        protected void SetUp()
        {
            MetricComputer toDecorate = new MetricComputerBuilder().withClassRepository(repo).build();
            decoratedComputer = new MetricComputerClrDecorator(toDecorate, repo);
        }

        [Test]
        public void testInstanceCost0()
        {
            Assert.AreEqual(1, totalGlobalCost("instanceCost0()System.Boolean"));
            Assert.AreEqual(0, cyclomaticCost("instanceCost0()System.Boolean"));
            Assert.AreEqual(0, globalCost("instanceCost0()System.Boolean"));
            Assert.AreEqual(0, totalComplexityCost("instanceCost0()System.Boolean"));
        }

        [Test]
        public void testStaticCost0()
        {
            Assert.AreEqual(0, cyclomaticCost("staticCost0()System.Boolean"));
            Assert.AreEqual(0, globalCost("staticCost0()System.Boolean"));
            Assert.AreEqual(0, totalComplexityCost("staticCost0()System.Boolean"));
            Assert.AreEqual(1, totalGlobalCost("staticCost0()System.Boolean"));
        }

        [Test]
        public void testInstanceCost1()
        {
            Assert.AreEqual(1, totalGlobalCost("instanceCost1()System.Boolean"));
            Assert.AreEqual(1, cyclomaticCost("instanceCost1()System.Boolean"));
            Assert.AreEqual(0, globalCost("instanceCost1()System.Boolean"));
            Assert.AreEqual(1, totalComplexityCost("instanceCost1()System.Boolean"));
        }

        [Test]
        public void testStaticCost1()
        {
            Assert.AreEqual(1, cyclomaticCost("staticCost1()System.Boolean"));
            Assert.AreEqual(0, globalCost("staticCost1()System.Boolean"));
            Assert.AreEqual(1, totalComplexityCost("staticCost1()System.Boolean"));
            Assert.AreEqual(1, totalGlobalCost("staticCost1()System.Boolean"));
        }

        [Test]
        public void testInstanceCost2()
        {
            Assert.AreEqual(2, cyclomaticCost("instanceCost2()System.Boolean"));
            Assert.AreEqual(0, globalCost("instanceCost2()System.Boolean"));
            Assert.AreEqual(2, totalComplexityCost("instanceCost2()System.Boolean"));
            Assert.AreEqual(1, totalGlobalCost("instanceCost2()System.Boolean"));
        }

        [Test]
        public void testStatcCost2()
        {
            Assert.AreEqual(2, cyclomaticCost("staticCost2()System.Boolean"));
            Assert.AreEqual(0, globalCost("staticCost2()System.Boolean"));
            Assert.AreEqual(2, totalComplexityCost("staticCost2()System.Boolean"));
            Assert.AreEqual(1, totalGlobalCost("staticCost2()System.Boolean"));
        }

        [Test]
        public void testInstanceCost3()
        {
            Assert.AreEqual(3, cyclomaticCost("instanceCost3()System.Boolean"));
            Assert.AreEqual(0, globalCost("instanceCost3()System.Boolean"));
            Assert.AreEqual(3, totalComplexityCost("instanceCost3()System.Boolean"));
            Assert.AreEqual(1, totalGlobalCost("instanceCost3()System.Boolean"));
        }

        [Test]
        public void testStaticCost3()
        {
            Assert.AreEqual(3, cyclomaticCost("staticCost3()System.Boolean"));
            Assert.AreEqual(0, globalCost("staticCost3()System.Boolean"));
            Assert.AreEqual(3, totalComplexityCost("staticCost3()System.Boolean"));
            Assert.AreEqual(1, totalGlobalCost("staticCost3()System.Boolean"));
        }

        [Test]
        public void testInstanceCost4()
        {
            Assert.AreEqual(4, cyclomaticCost("instanceCost4()System.Boolean"));
            Assert.AreEqual(0, globalCost("instanceCost4()System.Boolean"));
            Assert.AreEqual(4, totalComplexityCost("instanceCost4()System.Boolean"));
            Assert.AreEqual(1, totalGlobalCost("instanceCost4()System.Boolean"));
        }

        [Test]
        public void testStaticCost4()
        {
            Assert.AreEqual(4, cyclomaticCost("staticCost4()System.Boolean"));
            Assert.AreEqual(0, globalCost("staticCost4()System.Boolean"));
            Assert.AreEqual(4, totalComplexityCost("staticCost4()System.Boolean"));
            Assert.AreEqual(1, totalGlobalCost("staticCost4()System.Boolean"));
        }

        [Test]
        public void testCostUtilClassCost()
        {
            ClassCost classCost = decoratedComputer.compute<CostUtil>();
            Assert.AreEqual(4, classCost.getHighestMethodComplexityCost());
            Assert.AreEqual(1, classCost.getHighestMethodGlobalCost());
            Assert.AreEqual(20, classCost.getTotalComplexityCost());
            Assert.AreEqual(12, classCost.getTotalGlobalCost());
        }

        private int totalComplexityCost(string method)
        {
            return methodCostFor(method).getTotalCost().GetCyclomaticComplexityCost();
        }

        private int cyclomaticCost(string method)
        {
            return methodCostFor(method).getCost().GetCyclomaticComplexityCost();
        }

        private int globalCost(string method)
        {
            return methodCostFor(method).getCost().GetGlobalCost();
        }

        private int totalGlobalCost(string method)
        {
            return methodCostFor(method).getTotalCost().GetGlobalCost();
        }

        private MethodCost methodCostFor(string method)
        {
            MethodCost cost = decoratedComputer.compute<CostUtil>(method);
            return cost;
        }
    }
}