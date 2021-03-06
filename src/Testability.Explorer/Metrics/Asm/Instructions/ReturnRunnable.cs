using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class ReturnRunnable : IRunnable
    {
        private readonly BlockDecomposer block;
        private readonly Instruction instruction;
        private readonly Type type;

        public ReturnRunnable(BlockDecomposer block, Instruction instruction, Type type)
        {
            this.block = block;
            this.instruction = instruction;
            this.type = type;
        }

        public void run()
        {
            block.addOp(new Return(instruction.Offset, type));
        }
    }
}