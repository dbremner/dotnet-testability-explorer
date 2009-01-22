using C5;
using NUnit.Framework;
using Thinklouder.Testability.Metrics;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Tests.Metrics.Method
{
    [TestFixture]
    public class BlockTest
    {
        private readonly Type Object = ClrType.Object;

        private Variable var(object value)
        {
            return new Constant(value, Object);
        }

        [Test]
        public void testBlockToString()
        {
            var block = new Block("1");
            Assert.AreEqual("Block[1]{\n}", block.ToString());

            block.addOp(new Load(-1, var(1)));
            Assert.AreEqual("Block[1]{\n  load 1{System.Object}\n}", block.ToString());
        }

        [Test]
        public void testGetField()
        {
            var block = new Block("1");
            block.addOp(new GetField(-1, new FieldInfo(null, "src", Object,
                                                       false, true, false)));
            block.addOp(new PutField(-1, new FieldInfo(null, "dst", Object,
                                                       false, true, false)));

            IList<Operation> operations = new Stack2Turing(block).translate();
            Assert.AreEqual("[ 0:null::dst{System.Object} <- null::src{System.Object} ]", operations.ToString());
        }

        [Test]
        public void testVariableAssignment()
        {
            var block = new Block("1");
            block.addOp(new Load(-1, var("this"))); // this
            block.addOp(new Load(-1, var(1)));
            block.addOp(new PutField(-1, new FieldInfo(null, "abc", Object,
                                                       false, false, false)));

            IList<Operation> operations = new Stack2Turing(block).translate();
            Assert.AreEqual("[ 0:null::abc{System.Object} <- 1{System.Object} ]", operations.ToString());
        }

        [Test]
        public void testVariableStaticAssignment()
        {
            var block = new Block("1");
            block.addOp(new Load(-1, var(1)));
            block.addOp(new PutField(-1, new FieldInfo(null, "abc", Object,
                                                       false, true, false)));

            IList<Operation> operations = new Stack2Turing(block).translate();
            Assert.AreEqual("[ 0:null::abc{System.Object} <- 1{System.Object} ]", operations.ToString());
        }

        [Test]
        public void testMethodInvocation()
        {
            Block block = new Block("1");
            block.addOp(new Load(-1, var("methodThis"))); // this
            block.addOp(new GetField(-1, new FieldInfo(null, "p1", Object,
                false, true, false)));
            block.addOp(new GetField(-1, new FieldInfo(null, "p2", Object,
                false, true, false)));
            block.addOp(new Invoke(-1, null, "methodA", "(System.Int32, System.Int32)System.Object",
                new ArrayList<Type>() { ClrType.Int32, ClrType.Int32 }, false, Object));
            block.addOp(new PutField(-1, new FieldInfo(null, "dst", Object,
                false, true, false)));

            IList<Operation> operations = new Stack2Turing(block).translate();
            Assert.AreEqual("[ 0:null.methodA(System.Int32, System.Int32)System.Object, 1:null::dst{System.Object} <- ?{System.Object} ]",
                operations.ToString());
        }

        [Test]
        public void testDiamondBlockArrangment()
        {
            Block root = new Block("root");
            Block branchA = new Block("branchA");
            Block branchB = new Block("branchB");
            Block joined = new Block("joined");
            root.addNextBlock(branchA);
            root.addNextBlock(branchB);
            branchA.addNextBlock(joined);
            branchB.addNextBlock(joined);

            root.addOp(new Load(-1, var("this")));
            root.addOp(new Load(-1, var("root")));
            branchA.addOp(new Load(-1, var("A")));
            branchB.addOp(new Load(-1, var("B")));
            joined.addOp(new Load(-1, var("joined")));
            joined.addOp(new Invoke(-1, null, "m", "(System.Int32, System.Int32, System.Int32)System.Void", 
                new ArrayList<Type>(){ClrType.Int32, ClrType.Int32, ClrType.Int32}, 
                false, ClrType.Void));

            IList<Operation> operations = new Stack2Turing(root).translate();
            Assert.AreEqual(2, operations.Count);
            MethodInvokation mB = (MethodInvokation)operations[0];
            MethodInvokation mA = (MethodInvokation)operations[1];
            // since we use hash order is non-deterministic
            if (mB.getParameters()[1].ToString().StartsWith("A"))
            {
                MethodInvokation temp = mB;
                mB = mA;
                mA = temp;
            }

            Assert.AreEqual("[ 0:root{System.Object}, 1:A{System.Object}, 2:joined{System.Object} ]",
                mA.getParameters().ToString());
            Assert.AreEqual("this{System.Object}", mA.getMethodThis().ToString());

            Assert.AreEqual("[ 0:root{System.Object}, 1:B{System.Object}, 2:joined{System.Object} ]",
                mB.getParameters().ToString());
            Assert.AreEqual("this{System.Object}", mB.getMethodThis().ToString());
        }
    }
}