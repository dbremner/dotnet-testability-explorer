using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class NewRunnable : IRunnable
    {
        private readonly BlockDecomposer block;
        private readonly Instruction instruction;

        public NewRunnable(BlockDecomposer block, Instruction instruction)
        {
            this.block = block;
            this.instruction = instruction;
        }

        public void run()
        {
            Type type = ClrType.FromDescriptor(((Mono.Cecil.MemberReference) (instruction.Operand)).DeclaringType.FullName);
            var lineNumber = instruction.Offset;
            block.label(new Label(lineNumber));
            block.addOp(new Load(lineNumber, new Constant("newobj", type)));
        }
    }
}