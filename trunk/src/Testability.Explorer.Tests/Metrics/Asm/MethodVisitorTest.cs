using NUnit.Framework;
using Thinklouder.Testability.Metrics;
using Thinklouder.Testability.Metrics.Asm;

namespace Thinklouder.Testability.Tests.Metrics.Asm
{
    [TestFixture]
    public class MethodVisitorTest
    {
        [Test]
        public void testSwap()
        {
            //ClassInfo classInfo = new ClassInfo("TestClass", false, null, null);
            //MethodVisitor builder = new MethodVisitor(null , classInfo , "test",
            //    "()V", null, null, true, false, Visibility.PUBLIC);
            //builder.visitInsn(Opcodes.ICONST_0);
            //builder.visitInsn(Opcodes.ICONST_0);
            //builder.visitInsn(Opcodes.SWAP);
            //builder.visitEnd();
        }

        [Test]
        public void testNoop()
        {
            //ClassInfo classInfo = new ClassInfo("TestClass", false, null, null);
            //MethodVisitor builder = new MethodVisitor(null , classInfo , "test",
            //    "()V", null, null, true, false, Visibility.PUBLIC);
            //builder.visitInsn(Opcodes.NOP);
            //builder.visitEnd();
        }
    }
}
