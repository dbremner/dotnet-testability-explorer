using C5;
using NUnit.Framework;
using Thinklouder.Testability.Metrics;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Tests.Metrics.Method.Op.Stack
{
    [TestFixture]
    public class SwapTest
    {

        [Test]
        public void testOperatorCountIs2()
        {
            Assert.AreEqual(2, new Swap(-1).getOperatorCount());
        }

        [Test]
        public void testApplySwaps()
        {
            Swap swap = new Swap(-1);
            Variable first = new Variable(null, ClrType.Boolean, false, false);
            Variable second = new Variable(null, ClrType.Boolean, false, false);
            IList<Variable> inputs = new ArrayList<Variable>();
            inputs.Add(first);
            inputs.Add(second);
            IList<Variable> output = swap.apply(inputs);
            Assert.AreSame(second, output[0]);
            Assert.AreSame(first, output[1]);
        }

        public void testToString()
        {
            Assert.AreEqual("swap", new Swap(-1).ToString());
        }

    }
}
