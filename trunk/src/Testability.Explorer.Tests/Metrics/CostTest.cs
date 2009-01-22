using NUnit.Framework;
using Thinklouder.Testability.Metrics;

namespace Thinklouder.Testability.Tests.Metrics
{
    [TestFixture]
    public class CostTest
    {
        [Test]
        public void TestAddingZeroIsZero()
        {
            Cost none = new Cost();
            none.Add(new Cost());

            var a = new Cost();
            var b = new Cost();
            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(0, new Cost().GetLoDSum());
        }

        [Test]
        public void TestAddOfGreaterSize()
        {
            Cost lod = Cost.LoD(0);
            lod.Add(Cost.LoD(1));
            Assert.IsTrue(Cost.LoDDistribution(1, 1).Equals(lod));
            
            Assert.AreEqual(2, lod.GetLoDSum());
        }

        [Test]
        public void TestAddOfSameSize()
        {
            var lod = Cost.LoD(0);
            lod.Add(Cost.LoD(0));
            Assert.IsTrue(Cost.LoDDistribution(2).Equals(lod));
            Assert.AreEqual(2, lod.GetLoDSum());
        }

        [Test]
        public void TestAddOfSmallerSize()
        {
            var lod = Cost.LoD(1);
            lod.Add(Cost.LoD(0));
            Assert.IsTrue(Cost.LoDDistribution(1, 1).Equals(lod));
            Assert.AreEqual(2, lod.GetLoDSum());
        }
    }
}
