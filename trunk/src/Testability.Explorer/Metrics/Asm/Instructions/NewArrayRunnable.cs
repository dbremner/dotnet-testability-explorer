using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class NewArrayRunnable : IRunnable
    {
        private readonly BlockDecomposer block;
        private readonly Instruction instruction;

        public NewArrayRunnable(BlockDecomposer block, Instruction instruction)
        {
            this.block = block;
            this.instruction = instruction;
        }

        public void run()
        {
            var lineNumber = instruction.Offset;
            var type = ClrType.FromDescriptor(instruction.Operand.ToString());
            block.label(new Label(lineNumber));
            block.addOp(new Transform(lineNumber, "newarray", ClrType.Int32, null, type.ToArray()));
        }
    }
}