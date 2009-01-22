using System;
using System.Collections.Generic;
using C5;
using NUnit.Framework;
using Thinklouder.Testability.Metrics.Method.Collection;

namespace Thinklouder.Testability.Tests.Metrics.Method.Collection
{
    [TestFixture]
    public class KeyedMultiStackTest
    {
        [SetUp]
        public void SetUp()
        {
            log = new TreeSet<string>();
            stack = new KeyedMultiStack<string, int>("", new ValueCompactor<int>());
        }

        public class Push : PopClosure<string, int>
        {
            private readonly C5.IList<int> items;

            public Push(params int[] ints)
            {
                items = new ArrayList<int>();
                items.AddAll(ints);
            }

            public override int getSize()
            {
                return 0;
            }

            public override C5.IList<int> pop(string key, C5.IList<int> list)
            {
                return items;
            }
        }

        private class NoopClosure : PopClosure<string, int>
        {
            private int size;

            public NoopClosure(int size)
            {
                this.size = size;
            }

            public override int getSize()
            {
                return size;
            }

            public override C5.IList<int> pop(String key, C5.IList<int> list)
            {
                return new ArrayList<int>();
            }
        }

        private class LoggingClosure : PopClosure<string, int>
        {
            private readonly int size;

            public LoggingClosure(int size)
            {
                this.size = size;
            }

            public override C5.IList<int> pop(string key, C5.IList<int> value)
            {
                log.Add(value.ToString());
                return new ArrayList<int>();
            }

            public override int getSize()
            {
                return size;
            }
        }

        public class InnerPopClosure : PopClosure<string, int>
        {
            public override int getSize()
            {
                return 1;
            }

            public override C5.IList<int> pop(string key, C5.IList<int> list)
            {
                Assert.AreEqual("", key);
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(0, list[0]);
                log.Add(list[0].ToString());
                return new ArrayList<int>();
            }
        }

        static C5.ICollection<string> log;

        static KeyedMultiStack<string, int> stack;

        [Test]
        public void TestBasicOperationsOnSingleDimension()
        {
            log.Clear();
            stack.apply("", new Push(0));
            stack.apply("", new InnerPopClosure());
            //Assert.AreEqual("[0]", log.ToString());
            Assert.AreEqual("{ 0 }", log.ToString());
            stack.assertEmpty();
        }

        [Test]
        public void TestToString()
        {
            stack.apply("", new Push(0));
            Assert.IsNotNull(stack.ToString());
        }

        [Test]
        public void TestPushPushPopOnSplit()
        {
            stack.apply("", new Push(0));
            stack.split("", asList("a", "b"));
            stack.apply("a", new Push(1));
            stack.apply("b", new Push(2));
            stack.apply("a", new LoggingClosure(2));
            stack.apply("b", new LoggingClosure(2));
            //Assert.AreEqual("[[0, 1], [0, 2]]", log.ToString());
            Assert.AreEqual("{ [ 0:0, 1:1 ], [ 0:0, 1:2 ] }", log.ToString());
            stack.assertEmpty();
        }

        [Test]
        public void testPushPushPopOnSplit1()
        {
            stack.apply("", new Push(0));
            stack.split("", asList("c", "d"));
            stack.apply("c", new Push(1));
            stack.apply("d", new Push(2));
            stack.apply("c", new LoggingClosure(2));
            stack.apply("d", new LoggingClosure(2));
            Assert.AreEqual("{ [ 0:0, 1:1 ], [ 0:0, 1:2 ] }", log.ToString());
        }

        [Test]
        public void TestPushSplitPushJoinPOP()
        {
            stack.apply("", new Push(0));
            stack.split("", asList("a", "b"));
            stack.apply("a", new Push(1));
            stack.apply("b", new Push(2));
            stack.join(asList("a", "b"), "c");
            stack.apply("c", new Push(3));
            stack.apply("c", new LoggingClosure(3));
            //Assert.AreEqual("[[0, 1, 3], [0, 2, 3]]", log.ToString());
            Assert.AreEqual("{ [ 0:0, 1:1, 2:3 ], [ 0:0, 1:2, 2:3 ] }", log.ToString());
            stack.assertEmpty();
        }

        [Test]
        public void TestSplitAndJoinShouldCollapsMultipleStacksIfTheyAreOfSameContent()
        {
            stack.apply("", new Push(0));
            stack.split("", asList("a", "b"));
            stack.join(asList("a", "b"), "");
            stack.apply("", new Push(1));
            stack.apply("", new LoggingClosure(2));
            Assert.AreEqual("{ [ 0:0, 1:1 ] }", log.ToString());
        }

        public class InnerPopClosure1 : PopClosure<string, int>
        {
            public override int getSize()
            {
                return 1;
            }
            public override C5.IList<int> pop(string key, C5.IList<int> list)
            {
                stack.apply(key, new Push(list[0] + 10));
                return new ArrayList<int>();
            }
        }

        [Test]
        public void testConcurentPushInPopClosure()
        {
            stack.apply("", new Push(0));
            stack.apply("", new Push(1));
            stack.apply("", new InnerPopClosure1());
            stack.apply("", new LoggingClosure(2));
            //Assert.AreEqual("[[0, 11]]", log.ToString());
            Assert.AreEqual("{ [ 0:0, 1:11 ] }", log.ToString());
        }

        [Test]
        public void TestPopTooMuch()
        {
            try
            {
                stack.apply("", new LoggingClosure(1));
                Assert.Fail();
            }
#pragma warning disable 168
            catch (StackUnderflowException e)
#pragma warning restore 168
            {
            }
        }

        [Test]
        public void TestUnknownKey()
        {
            try
            {
                stack.apply("X", new Push());
                Assert.Fail();
            }
#pragma warning disable 168
            catch (KeyedMultiStack<string, int>.KeyNotFoundException e)
#pragma warning restore 168
            {
            }
        }

        [Test]
        public void TestSplitUnknwonNamespace()
        {
            try
            {
                stack.split("X", asList("A", "B"));
                Assert.Fail();
            }
            catch (KeyedMultiStack<string, int>.KeyNotFoundException e)
            {
            }
        }

        [Test]
        public void TestJoinUnknownNamespace()
        {
            try
            {
                stack.join(asList("B", "C"), "A");
                Assert.Fail();
            }
            catch (KeyedMultiStack<string, int>.KeyNotFoundException e)
            {
            }
        }
        [Test]
        public void TestUnevenJoin()
        {
            stack.split("", asList("a", "b"));
            stack.apply("a", new Push(0));
            try
            {
                stack.join(asList("a", "b"), "c");
                Assert.Fail();
            }
            catch (InvalidOperationException e)
            {
            }
        }
        [Test]
        public void TestJoinThroughSlipt()
        {
            stack.apply("", new Push(0));
            stack.split("", asList("a", "b"));
            stack.apply("a", new Push(1));
            stack.apply("b", new Push(2));
            stack.split("a", asList("join"));
            stack.split("b", asList("join"));
            stack.apply("join", new LoggingClosure(2));
            //Assert.AreEqual("[[0, 1], [0, 2]]", log.ToString());
            Assert.AreEqual("{ [ 0:0, 1:1 ], [ 0:0, 1:2 ] }", log.ToString());

        }
        [Test]
        public void TestParalelPopAndPush()
        {
            stack.apply("", new Push(0));
            stack.apply("", new Push(1));
            stack.split("", asList("a", "b"));
            stack.apply("a", new Push(2));
            stack.apply("b", new Push(3));
            stack.join(asList("a", "b"), "join");
            stack.apply("join", new InnerPopClosure2());
            stack.apply("join", new LoggingClosure(3));
            //Assert.AreEqual("[[0, 3, 4], [0, 5, 6]]", log.ToString());
            Assert.AreEqual("{ [ 0:0, 1:3, 2:4 ], [ 0:0, 1:5, 2:6 ] }", log.ToString());

        }

        public class InnerPopClosure2 : PopClosure<string, int>
        {

            int id = 3;
            public override C5.IList<int> pop(String key, C5.IList<int> list)
            {
                return asList(id++, id++);
            }

            public override int getSize()
            {
                return 2;
            }
        }

        [Test]
        public void TestPathEnsureSize()
        {
            Path<string> path = new Path<string>();
            path.add("A");
            path.add("B");
            path.add("C");
            path.add("D");
            path.add("E");
            path.add("F");
            path.add("G");
            Assert.AreEqual("A :: B :: C :: D :: E :: F :: G", path.ToString());
        }

        [Test]
        public void TestPathHashCode()
        {
            Path<string> p1 = new Path<string>();
            Path<string> p2 = new Path<string>();
            Assert.AreEqual(p1, p2);
            Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
        }

        [Test]
        public void TestPopTooSlowForVeryLargeSets()
        {
            DateTime start = DateTime.Now;//System.currentTimeMillis();
            int counter = 0;
            String[] subKeys = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p" };
            stack.split("", asList(subKeys));
            foreach (String key in subKeys)
            {
                stack.apply(key, new Push(counter++));
            }
            stack.join(asList(subKeys), "L1");

            stack.split("L1", asList(subKeys));
            foreach (String key in subKeys)
            {
                stack.apply(key, new Push(counter++));
            }
            stack.join(asList(subKeys), "L2");

            stack.split("L2", asList(subKeys));
            foreach (String key in subKeys)
            {
                stack.apply(key, new Push(counter++));
            }
            stack.join(asList(subKeys), "L3");

            stack.apply("L3", new NoopClosure(3));
            TimeSpan duration = DateTime.Now - start;
            //Assert.That(duration.Milliseconds < 60, "Duration: " + duration.Milliseconds);  // TODO, improve speed?
            //Assert.That(duration.Milliseconds < 250, "Duration: " + duration.Milliseconds);
            Assert.That(duration.Milliseconds < 250, "Duration: " + duration.Milliseconds);
        }

        public static ArrayList<T> asList<T>(params T[] items)
        {
            var list = new ArrayList<T>();
            for (int i = 0; i < items.Length; i++)
            {
                list.Add(items[i]);
            }
            return list;
        }
    }
}