using C5;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class CallRunnable : IRunnable
    {
        private readonly Instruction instruction;
        private readonly BlockDecomposer block;
        
        public CallRunnable(Instruction instruction, BlockDecomposer block)
        {
            this.instruction = instruction;
            this.block = block;
        }

        public void run()
        {
            var md = instruction.Operand as MethodReference;

            if (md == null) return;

            var typeName = md.DeclaringType.FullName;
            var methodName = md.Name;

            // TODO, extract method to compile a descriptor from MethodReference::FullName
            string fullName = md.ToString();
            int spaceIndex = fullName.IndexOf(" ");
            int leftBraceIndex = fullName.IndexOf("(");

            var descriptor = fullName.Substring(leftBraceIndex, fullName.Length - leftBraceIndex) +
                fullName.Substring(0, spaceIndex);

            var parameters = new ArrayList<Type>();  // TODO, decompose parameters from descriptor, by sunlw
            var returnType = new Type(md.ReturnType.ReturnType.FullName);

            var lineNumber = instruction.Offset;

            block.label(new Label(lineNumber));
            block.addOp(new Invoke(lineNumber, typeName, methodName, descriptor,
                parameters, instruction.OpCode == OpCodes.Call, returnType));
        }
    }
}
