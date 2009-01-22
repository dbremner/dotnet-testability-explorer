using C5;
using NUnit.Framework;
using Thinklouder.Testability.Metrics;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Tests.Metrics.Method
{
    [TestFixture]
    public class Stack2TuringTest
    {
        [Test]public void testJSRSingleBlock()
        {
            Block main = new Block("main");
            Block sub = new Block("sub");

            main.addOp(new Load(0, new Variable("this", ClrType.Object, false, false)));
            main.addOp(new JMP(0, sub));
            main.addOp(new PutField(0, new FieldInfo(null, "a", ClrType.Int32, false, false,
                false)));

            sub.addOp(new Load(0, new Constant(1, ClrType.Int32)));
            sub.addOp(new Return(0, ClrType.Void));

            Stack2Turing converter = new Stack2Turing(main);
            IList<Operation> operations = converter.translate();
            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual("null::a{System.Int32} <- 1{System.Int32}", operations[0].ToString());
        }

        [Test]public void testJSRMultiBlock()
        {
            Block main = new Block("main");
            Block sub = new Block("sub");
            Block sub1 = new Block("sub1");
            Block sub2 = new Block("sub2");
            sub.addNextBlock(sub1);
            sub1.addNextBlock(sub2);

            main.addOp(new JMP(0, sub));

            Stack2Turing converter = new Stack2Turing(main);
            converter.translate(); // Assert no exceptions
        }

        [Test]public void testMakeSureThatJsrWhichCallsItselfDoesNotRecurseForever()
        {
            Block main = new Block("main");
            Block sub = new Block("sub");
            main.addOp(new JMP(0, sub));
            main.addOp(new JMP(0, sub));
            sub.addOp(new RetSub(1));
            sub.addNextBlock(main);
            Stack2Turing converter = new Stack2Turing(main);
            converter.translate(); // Assert no exceptions and that we don't get into infinite recursion
        }

        private IList<IList<Variable>> vv(params IList<Variable>[] arrayOflistOfVars)
        {
            var list = new ArrayList<IList<Variable>>();
            list.AddAll(arrayOflistOfVars);
            return list;
        }

        private IList<Variable> v(params Variable[] variables)
        {
            var list = new ArrayList<Variable>();
            list.AddAll(variables);
            return list;
        }

        [Test]public void testCompactionOfPrimitiveConstants()
        {
            Stack2Turing.VariableCompactor compactor = new Stack2Turing.VariableCompactor();
            Constant c1 = new Constant("a", ClrType.Int32);
            Constant c2 = new Constant("a", ClrType.Int32);
            Assert.AreEqual(vv(v(c1)), compactor.compact(vv(v(c1), v(c2))));
        }

        [Test]public void testNonCompactionOfNonPrimitiveConstants()
        {
            Stack2Turing.VariableCompactor compactor = new Stack2Turing.VariableCompactor();
            Constant c1 = new Constant("a", ClrType.Object);
            Constant c2 = new Constant("a", ClrType.FromClr(typeof(string)));
            Assert.AreEqual(vv(v(c1), v(c2)), compactor.compact(vv(v(c1), v(c2))));
        }
    }
}
