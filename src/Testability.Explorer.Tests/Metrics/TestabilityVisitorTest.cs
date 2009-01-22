using C5;
using NUnit.Framework;
using Thinklouder.Testability.Metrics;

namespace Thinklouder.Testability.Tests.Metrics
{
    [TestFixture]
    public class TestabilityVisitorTest
    {
        private IList<int> cost1 = new ArrayList<int>();
        Variable instance = new Variable("instance", null, false, false);
        Variable finalInstance = new Variable("instance", null, true, false);
        FieldInfo field = new FieldInfo(null, "field", null, false, false, false);
        FieldInfo finalField = new FieldInfo(null, "field", null, true, false, false);
        FieldInfo finalStaticField = new FieldInfo(null, "field", null, true, true, false);

        private LocalField localField;
        private LocalField localFinalField;
        private LocalField localStaticFinalField;

        Variable dst = new Variable("dst", null, false, false);
        private Variable dstField = new FieldInfo(null, "dstField", null, false, false, false);

        ClassInfo classInfo = new ClassInfo("c.g.t.A", false, null, new ArrayList<ClassInfo>());  // immutable EMPTY_LIST?
        private MethodInfo method;

        private readonly ClrClassRepository repo = new ClrClassRepository();
        private readonly VariableState globalVariables = new VariableState();

        private TestabilityVisitor visitor;
        private TestabilityVisitor.CostRecordingFrame frame;
        private TestabilityVisitor.ParentFrame parentFrame;

        [SetUp]
        public void SetUp()
        {
            localField = new LocalField(instance, field);
            localFinalField = new LocalField(instance, finalField);
            localStaticFinalField = new LocalField(null, finalStaticField);

            method = new MethodInfo(classInfo, "method", 0, "()V", null, null, null, Visibility.NULL, cost1, null, false);
            visitor = new TestabilityVisitor(repo, globalVariables, null, new RegExpWhiteList());
            frame = visitor.createFrame(method, 1);
            parentFrame = frame.getParentFrame();
        }

        [Test]
        public void TestIsInjectable()
        {
            var var = new Variable("", ClrType.FromClr("X"), false, false);
            Assert.IsFalse(globalVariables.IsInjectable(var));
            globalVariables.SetInjectable(var);
            Assert.IsTrue(globalVariables.IsInjectable(var));
        }

        [Test]
        public void TestNoop()
        {
            frame.assignParameter(1, dst, parentFrame, instance);
            Assert.IsFalse(globalVariables.IsGlobal(dst));
            Assert.IsFalse(globalVariables.IsInjectable(dst));
            Assert.AreEqual(0, frame.getMethodCost().getTotalCost().GetGlobalCost());
        }
    }
}