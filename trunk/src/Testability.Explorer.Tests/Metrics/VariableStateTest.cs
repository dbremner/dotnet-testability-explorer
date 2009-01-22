#region

using NUnit.Framework;
using Thinklouder.Testability.Metrics;

#endregion

namespace Thinklouder.Testability.Tests.Metrics
{
    [TestFixture]
    public class VariableStateTest
    {
        private VariableState state;
        private Variable instance;
        private FieldInfo field;
        private Variable localField;

        [SetUp]
        public void Setup()
        {
            state = new VariableState();
            instance = new Variable("var", null, false, false);
            field = new FieldInfo(null, "field", null, false, false, false);
            localField = new LocalField(instance, field);
        }

        [Test]
        public void TestGlobals()
        {
            state.SetGlobal(instance);
            Assert.IsTrue(state.IsGlobal(instance));
            Assert.IsFalse(state.IsGlobal(null));
        }

        [Test]
        public void TestToString()
        {
            state.SetGlobal(instance);
            state.SetInjectable(field);
            string text = state.ToString();
            Assert.IsTrue(text.Contains("var"), text);
            Assert.IsTrue(text.Contains("field"), text);
        }
        [Test]
        public void TestLocalFieldIsGlobalIfInstanceIsGlobal()
        {
            state.SetGlobal(instance);
            Assert.IsTrue(state.IsGlobal(localField));
        }
        [Test]
        public void TestLocalFieldIsGlobalIfFieldIsGlobal()
        {
            state.SetGlobal(field);
            Assert.IsTrue(state.IsGlobal(localField));
        }
        [Test]
        public void TestLocalFieldIsInjectableIfInstanceIsInjectable()
        {
            state.SetInjectable(field);
            Assert.IsTrue(state.IsInjectable(localField));
        }
    }
}