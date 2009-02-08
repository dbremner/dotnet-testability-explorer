using System;
using C5;
using NUnit.Framework;
using Thinklouder.Testability.Metrics;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

//using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Tests.Metrics.Method
{
    [TestFixture]
    public class BlockDecomposerTest
    {
        [Test]
        public void testSimpleLinearMethod()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            Return ret = new Return(0, ClrType.Void);
            decomposer.addOp(ret);
            decomposer.decomposeIntoBlocks();

            Block block = decomposer.getMainBlock();
            Assert.AreEqual(list(ret), block.getOperations());
        }

        [Test]
        public void testUnconditionalBackwardGoto()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            Label label = new Label();
            Load l1 = load(1);
            Load l2 = load(2);
            decomposer.addOp(l1);
            decomposer.label(label);
            decomposer.addOp(l2);
            decomposer.unconditionalGoto(label);
            decomposer.decomposeIntoBlocks();

            Block main = decomposer.getMainBlock();
            Assert.AreEqual(list(l1), main.getOperations());

            Assert.AreEqual(1, main.getNextBlocks().Count);
            Block next = main.getNextBlocks()[0];
            Assert.AreEqual(list(l2), next.getOperations());
        }


        [Test]
        public void testUnconditionalForwardGoto()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            Label label = new Label();
            Load l1 = load(1);
            Load l2 = load(2);
            decomposer.addOp(l1);
            decomposer.unconditionalGoto(label);
            decomposer.label(label);
            decomposer.addOp(l2);
            decomposer.decomposeIntoBlocks();

            Block main = decomposer.getMainBlock();
            Assert.AreEqual(list(l1), main.getOperations());

            Assert.AreEqual(1, main.getNextBlocks().Count);
            Block next = main.getNextBlocks()[0];
            Assert.AreEqual(list(l2), next.getOperations());
        }


        [Test]
        public void testConditionalBackwardGoto()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            Label label = new Label();
            Load l1 = load(1);
            Load l2 = load(2);
            Load l3 = load(3);
            decomposer.addOp(l1);
            decomposer.label(label);
            decomposer.addOp(l2);
            decomposer.conditionalGoto(label);
            decomposer.addOp(l3);
            decomposer.decomposeIntoBlocks();

            Block main = decomposer.getMainBlock();
            Block nextTrue = decomposer.getBlock(label);
            Block nextFlase = nextTrue.getNextBlocks()[0];
            Assert.AreEqual(list(l1), main.getOperations());
            Assert.AreEqual(list(l2), nextTrue.getOperations());
            Assert.AreEqual(list(l3), nextFlase.getOperations());
        }


        [Test]
        public void testConditionalForwardGoto()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            Label label = new Label();
            Load l1 = load(1);
            Load l2 = load(2);
            Load l3 = load(3);
            decomposer.addOp(l1); // main
            decomposer.conditionalGoto(label);
            decomposer.addOp(l2); // nextFalse
            decomposer.label(label);
            decomposer.addOp(l3); // nextTrue
            decomposer.decomposeIntoBlocks();

            Block main = decomposer.getMainBlock();
            Block nextFlase = main.getNextBlocks()[0];
            Block nextTrue = decomposer.getBlock(label);
            Assert.AreEqual(list(l1), main.getOperations());
            Assert.AreEqual(list(l2), nextFlase.getOperations());
            Assert.AreEqual(list(l3), nextTrue.getOperations());
        }



        private IList<T> list<T>(params T[] items)
        {
            return Arrays.asList(items);
        }

        [Test]
        public void testIgnoreExtraLabels()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            Load l1 = load(1);
            Load l2 = load(2);
            decomposer.label(new Label());
            decomposer.addOp(l1);
            decomposer.addOp(l2);
            decomposer.label(new Label());
            decomposer.decomposeIntoBlocks();

            Assert.AreEqual(list(l1, l2), decomposer.getMainBlock().getOperations());
        }

        /**
         * load 1
         * jsr mySub
         * load 2
         * return
         *
         * mySub:
         * load 3
         * return;
         */
        [Test]
        public void testJMP()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            decomposer.addOp(load(1));
            Label sub = new Label();
            decomposer.jumpSubroutine(sub, 0);
            decomposer.addOp(load(2));
            decomposer.addOp(new Return(0, ClrType.Void));
            decomposer.label(sub);
            decomposer.addOp(load(3));
            decomposer.addOp(new RetSub(0));
            decomposer.decomposeIntoBlocks();

            Block mainBlock = decomposer.getMainBlock();
            Assert.AreEqual(0, mainBlock.getNextBlocks().Count);
            IList<StackOperation> operations = mainBlock.getOperations();
            Assert.AreEqual("[ 0:load 1{System.Int32}, 1:JMP sub_0, 2:load 2{System.Int32}, 3:return System.Void ]",
                operations.ToString());
            JMP jsr = (JMP)operations[1];
            Block subBlock = jsr.Block;
            Assert.AreEqual("[ 0:load 3{System.Int32}, 1:RETSUB ]", subBlock.getOperations().ToString());
        }

        private Load load(int value)
        {
            return new Load(0, new Constant(value, ClrType.Int32));
        }

        [Test]
        public void testSwitch()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            Load l1 = load(1);
            Label c1Label = new Label();
            Load c1 = load(2);
            Label defLabel = new Label();
            Load def = load(3);
            decomposer.addOp(l1);
            decomposer.tableSwitch(defLabel, new ArrayList<Label> { c1Label });
            decomposer.label(c1Label);
            decomposer.addOp(c1);
            decomposer.label(defLabel);
            decomposer.addOp(def);
            decomposer.decomposeIntoBlocks();

            Block main = decomposer.getMainBlock();
            Block c1Block = decomposer.getBlock(c1Label);
            Block defBlock = decomposer.getBlock(defLabel);
            Assert.AreEqual(list(l1), main.getOperations());
            Assert.AreEqual(list(c1), c1Block.getOperations());
            Assert.AreEqual(list(def), defBlock.getOperations());
            Assert.AreEqual(list(c1Block, defBlock), main.getNextBlocks());
        }

        [Test]
        public void testTryCatchReturn()
        {
            /*
             * try {        | label:lTry
             *   return 1;  | label:lTryEnd
             * } catch {    | label:lHandle
             *   return 2;
             * }
             */
            Label lTry = new Label();
            Load l1 = load(1);
            Load l2 = load(2);
            Return ret = new Return(1, ClrType.Int32);
            Label lTryEnd = new Label();
            Label lHandle = new Label();

            BlockDecomposer decomposer = new BlockDecomposer();
            decomposer.tryCatchBlock(lTry, lTryEnd, lHandle, null);
            decomposer.label(lTry);
            decomposer.addOp(l1);
            decomposer.label(lTryEnd); // the catch label comes before the last instruction.
            decomposer.addOp(ret);
            decomposer.label(lHandle);
            decomposer.addOp(l2);
            decomposer.addOp(ret);
            decomposer.decomposeIntoBlocks();

            Block tryBlock = decomposer.getBlock(lTry);
            Block handleBlock = decomposer.getBlock(lHandle);
            Assert.AreEqual(list<Block>(), tryBlock.getNextBlocks());
            Assert.AreEqual(list<StackOperation>(l1, ret), tryBlock.getOperations());
            //Assert.AreEqual("[ 0:load ?{java.lang.Throwable}, 1:load 2{System.Int32}, 2:return System.Int32 ]",
            Assert.AreEqual("[ 0:load ?{System.Exception}, 1:load 2{System.Int32}, 2:return System.Int32 ]",
                handleBlock.getOperations().ToString());
        }

        [Test]
        public void testGetExceptionHandlerBlocks()
        {
            /*
             * try {        | label:lTry
             *   a = 1;
             *   return 1;  | label:lTryEnd
             * } catch {    | label:lHandle
             *   a = 2;
             *   return 2;
             * }
             */
            Label lTry = new Label();
            Load l1 = load(1);
            Load l2 = load(2);
            Store store = new Store(-1, new Variable("a", ClrType.Int32, false, false));
            Return ret = new Return(1, ClrType.Int32);
            Label lTryEnd = new Label();
            Label lHandle = new Label();

            BlockDecomposer decomposer = new BlockDecomposer();
            decomposer.tryCatchBlock(lTry, lTryEnd, lHandle, null);
            decomposer.label(lTry);
            decomposer.addOp(l1);
            decomposer.addOp(store);
            decomposer.label(lTryEnd); // the catch label comes before the last instruction.
            decomposer.addOp(l1);
            decomposer.addOp(ret);
            decomposer.label(lHandle);
            decomposer.addOp(l2);
            decomposer.addOp(store);
            decomposer.addOp(l2);
            decomposer.addOp(ret);
            decomposer.decomposeIntoBlocks();

            Block mainBlock = decomposer.getMainBlock();
            Assert.AreEqual(0, mainBlock.getNextBlocks().Count);
            Assert.AreEqual(list<StackOperation>(l1, store, l1, ret), mainBlock.getOperations());

            Block catchBlock = decomposer.getBlock(lHandle);
            Assert.AreEqual(0, catchBlock.getNextBlocks().Count);
            Assert.AreEqual(
                //"[load ?{java.lang.Throwable}, load 2{int}, store a{int}, load 2{int}, return int]",
                "[ 0:load ?{System.Exception}, 1:load 2{System.Int32}, 2:store a{System.Int32}, 3:load 2{System.Int32}, 4:return System.Int32 ]",
                catchBlock.getOperations().ToString("long", null));

            Assert.AreEqual(
                "[ 0:a{System.Int32} <- 1{System.Int32}, 1:return 1{System.Int32}, 2:a{System.Int32} <- 2{System.Int32}, 3:return 2{System.Int32} ]",
                decomposer.getOperations().ToString("long", null));
        }

        /**
         *   public static class TryCatchFinally {
         *   public void method() {
         *     int b = 1;
         *     try {
         *       b = 2;
         *       return;
         *     } catch (RuntimeException e) {
         *       b = 3;
         *     } finally {
         *       b = 4;
         *     }
         *     b = 5;
         *   }
         *  }
         *
         *
         *  public void method();
         *  Code:
         *   0:   iconst_1
         *   1:   istore_1
         *   2:   iconst_2
         *   3:   istore_1
         *   4:   goto    20
         *   7:   astore_2
         *   8:   iconst_3
         *   9:   istore_1
         *   10:  iconst_4
         *   11:  istore_1
         *   12:  goto    22
         *   15:  astore_3
         *   16:  iconst_4
         *   17:  istore_1
         *   18:  aload_3
         *   19:  athrow
         *   20:  iconst_4
         *   21:  istore_1
         *   22:  iconst_5
         *   23:  istore_1
         *   24:  return
         *  Exception table:
         *   from   to  target type
         *     2     4     7   Class java/lang/RuntimeException
         *     2    10    15   any
         */
        [Test]
        public void testTryCatchFinally()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            Label tryStart = new Label();
            Label tryEnd = new Label();
            Label runtimeHandler = new Label();
            Label catchEnd = new Label();
            Label finallyHandler = new Label();
            Label l20 = new Label();
            Label l22 = new Label();
            Variable b = new LocalVariableInfo("b", ClrType.Int32);
            Variable e = new LocalVariableInfo("e", ClrType.Object);
            Variable any = new LocalVariableInfo("any", ClrType.Object);
            decomposer.tryCatchBlock(tryStart, tryEnd, runtimeHandler, "java/lang/RuntimeException");
            decomposer.tryCatchBlock(tryStart, catchEnd, finallyHandler, null);

            /* 0*/
            decomposer.addOp(new Load(0, new Constant(1, ClrType.Int32)));
            /* 1*/
            decomposer.addOp(new Store(1, b));
            decomposer.label(tryStart);
            /* 2*/
            decomposer.addOp(new Load(2, new Constant(2, ClrType.Int32)));
            decomposer.label(tryEnd);
            /* 3*/
            decomposer.addOp(new Store(3, b));
            /* 4*/
            decomposer.unconditionalGoto(l20);
            decomposer.label(runtimeHandler);
            /* 7*/
            decomposer.addOp(new Store(7, e));
            /* 8*/
            decomposer.addOp(new Load(8, new Constant(3, ClrType.Int32)));
            /* 9*/
            decomposer.addOp(new Store(9, b));
            decomposer.label(catchEnd);
            /*10*/
            decomposer.addOp(new Load(10, new Constant(4, ClrType.Int32)));
            /*11*/
            decomposer.addOp(new Store(11, b));
            /*12*/
            decomposer.unconditionalGoto(l22);
            decomposer.label(finallyHandler);
            /*15*/
            decomposer.addOp(new Store(15, any));
            /*16*/
            decomposer.addOp(new Load(16, new Constant(4, ClrType.Int32)));
            /*17*/
            decomposer.addOp(new Store(17, b));
            /*18*/
            decomposer.addOp(new Load(18, any));
            /*19*/
            decomposer.addOp(new Throw(19));
            decomposer.label(l20);
            /*20*/
            decomposer.addOp(new Load(20, new Constant(4, ClrType.Int32)));
            /*21*/
            decomposer.addOp(new Store(21, b));
            decomposer.label(l22);
            /*22*/
            decomposer.addOp(new Load(22, new Constant(4, ClrType.Int32)));
            /*23*/
            decomposer.addOp(new Store(23, b));
            /*24*/
            decomposer.addOp(new Return(24, ClrType.Void));

            decomposer.decomposeIntoBlocks();

            Assert.AreEqual("[ 0:load 1{System.Int32}, 1:store b{System.Int32}, 2:load 2{System.Int32}, 3:store b{System.Int32} ]",
                decomposer.getBlock(tryStart).getOperations().ToString());
        }

        [Test]
        public void testMethodWithNothing()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            decomposer.decomposeIntoBlocks();
            IList<Operation> operations = decomposer.getOperations();
            Assert.AreEqual(0, operations.Count);
        }

        [Test]
        public void testTwoJsrSameLabel()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            Label label = new Label();
            decomposer.jumpSubroutine(label, 0);
            decomposer.jumpSubroutine(label, 0);
            decomposer.decomposeIntoBlocks();

            Block main = decomposer.getMainBlock();
            Assert.AreEqual("[ 0:JMP sub_0, 1:JMP sub_0 ]", main.getOperations().ToString());
        }

        [Test]
        public void testMultipleHandlersSingleExceptionLoad()
        {
            BlockDecomposer decomposer = new BlockDecomposer();
            Label start = new Label();
            Label end = new Label();
            Label handler = new Label();
            decomposer.tryCatchBlock(start, end, handler, null);
            decomposer.tryCatchBlock(start, end, handler, null);
            decomposer.label(start);
            decomposer.addOp(load(1));
            decomposer.label(end);
            decomposer.addOp(load(2));
            decomposer.label(handler);
            decomposer.addOp(load(3));
            decomposer.decomposeIntoBlocks();

            Assert.AreEqual(
                //"[load ?{java.lang.Throwable}, load 1{int}, load 2{int}, load 3{int}]",
                "[ 0:load ?{System.Exception}, 1:load 1{System.Int32}, 2:load 2{System.Int32}, 3:load 3{System.Int32} ]",
                decomposer.getMainBlock().getOperations().ToString("long", null));
        }

        public class TestClass
        {
            int value;
            int method()
            {
                value = 0;
                try
                {
                    value = 1;
                    return -1;
                }
                catch (NullReferenceException e)
                {
                    value = 2;
                    return -2;
                }
                finally
                {
                    value = 3;
                }

            }
        }

        [Test]
        public void testExperiment()
        {
            var repo = new ClrClassRepository();
            repo.GetClass(ClassInfo.GetFullName<TestClass>());
        }


        public class TestPutField
        {
            ClrClassRepository repo;
            public TestPutField()
            {
                repo = new ClrClassRepository();
            }
        }

        [Test]
        public void testInit()
        {
            ClrClassRepository repo = new ClrClassRepository();
            ClassInfo clazz = repo.GetClass(ClassInfo.GetFullName<TestPutField>());
            ICollectionValue<MethodInfo> methods = clazz.GetMethods();
            foreach (MethodInfo method in methods)
            {
                IList<Operation> ops = method.Operations;
                IList<Operation> ops1 = ops;
            }

            //MethodInfo method = clazz.getMethod("<init>V");
            //List<Operation> ops = method.getOperations();
        }

        [Test]
        public void testClassInfoCtor()
        {
            /*
        decomposer.addOp(new Load(0, new LocalVariableInfo("this", JavaType.fromClass(BlockDecomposerTest.TestPutField.class))));
        decomposer.addOp(new Load(1, new ParameterInfo("param_1", JavaType.fromClass(BlockDecomposerTest.class))));
        decomposer.addOp(new PutField(2, new FieldInfo(null, "this$0", JavaType.fromClass(BlockDecomposerTest.TestPutField.class), false, false, false)));
        decomposer.addOp(new Load(3, new LocalVariableInfo("this", JavaType.fromClass(BlockDecomposerTest.TestPutField.class))));
        decomposer.addOp(new Invoke(4, "java.lang.Object", "<init>", "()V", new ArrayList<Type>(), false, JavaType.VOID));
        decomposer.addOp(new Load(5, new LocalVariableInfo("this", JavaType.fromClass(BlockDecomposerTest.TestPutField.class))));
        decomposer.addOp(new Load(6, new Constant("new", JavaType.fromClass(com.google.test.metric.JavaClassRepository.class))));
        decomposer.addOp(new Duplicate(448, 0));
        decomposer.addOp(new Invoke(8, "com.google.test.metric.JavaClassRepository", "<init>", "()V", new ArrayList<Type>(), false, JavaType.VOID));
        decomposer.addOp(new PutField(9, new FieldInfo(null, "repo", JavaType.fromClass(BlockDecomposerTest.TestPutField.class), false, false, false)));
        decomposer.addOp(new Return(449, JavaType.VOID));

             */


            BlockDecomposer decomposer = new BlockDecomposer();


            //block.addOp(new Load(1, new Constant(null, ClrType.FromDescriptor("Thinklouder.Testability.Metrics.ClrClassRepository"))));
            ////block.addOp(new PutField(2, new FieldInfo(null, "repo", ClrType.FromDescriptor("Thinklouder.Testability.Metrics.ClrClassRepository"), false, false, false)));
            //block.addOp(new Invoke(3, "System.Object", ".ctor", "()System.Void", new ArrayList<Thinklouder.Testability.Metrics.Type>(), false, ClrType.Void));
            //block.addOp(new RetSub(4));

            decomposer.addOp(new Load(0, new LocalVariableInfo("this", ClrType.FromClr<BlockDecomposerTest.TestPutField>())));
            decomposer.addOp(new Load(1, new ParameterInfo("param_1", ClrType.FromClr<BlockDecomposerTest>())));
            decomposer.addOp(new PutField(2, new FieldInfo(null, "this$0", ClrType.FromClr<BlockDecomposerTest.TestPutField>(), false, false, false)));
            decomposer.addOp(new Load(3, new LocalVariableInfo("this", ClrType.FromClr<BlockDecomposerTest.TestPutField>())));
            decomposer.addOp(new Invoke(4, "System.Object", ".ctor", "()System.Void", new ArrayList<Thinklouder.Testability.Metrics.Type>(), false, ClrType.Void));
            decomposer.addOp(new Load(5, new LocalVariableInfo("this", ClrType.FromClr<BlockDecomposerTest.TestPutField>())));
            decomposer.addOp(new Load(6, new Constant("new", ClrType.FromClr<ClrClassRepository>())));
            decomposer.addOp(new Duplicate(7, 0));
            decomposer.addOp(new Invoke(8, "ClrClassRepository", ".ctor", "()System.Void", new ArrayList<Thinklouder.Testability.Metrics.Type>(), false, ClrType.Void));
            decomposer.addOp(new PutField(9, new FieldInfo(null, "repo", ClrType.FromClr<BlockDecomposerTest.TestPutField>(), false, false, false)));
            decomposer.addOp(new Return(10, ClrType.Void));


            decomposer.decomposeIntoBlocks();
            IList<Operation> ops = decomposer.getOperations();
            //Console.WriteLine(ops.ToString());
        }
    }
}