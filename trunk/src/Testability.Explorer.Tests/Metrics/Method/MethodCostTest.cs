using NUnit.Framework;
using Thinklouder.Testability.Metrics;

namespace Thinklouder.Testability.Tests.Metrics.Method
{
    [TestFixture]
    public class MethodCostTest
    {
        [Test]
        public void TestComputeOverallCost()
        {
            var cost = new MethodCost("a", 0);
            cost.addCostSource(new CyclomaticCost(0, Cost.Cyclomatic(1)));
            cost.addCostSource(new GlobalCost(0, null, Cost.Global(1)));

            var cost3 = new MethodCost("b", 0);
            cost3.addCostSource(new CyclomaticCost(0, Cost.Cyclomatic(1)));
            cost3.addCostSource(new CyclomaticCost(0, Cost.Cyclomatic(1)));
            cost3.addCostSource(new CyclomaticCost(0, Cost.Cyclomatic(1)));

            cost.addCostSource(new MethodInvokationCost(0, cost3,
                Reason.IMPLICIT_STATIC_INIT, Cost.Cyclomatic(3)));

            var costModel = new CostModel(2, 10);
            cost.link();

            Assert.AreEqual((long) 2*(3+1)+10*1, costModel.computeOverall(cost.getTotalCost()));
            Assert.AreEqual(2, cost.getExplicitViolationCosts().Count);
            Assert.AreEqual(1, cost.getImplicitViolationCosts().Count);
        }
    }
}
