using Mono.Cecil;
using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class ConvertRunnable : IRunnable
    {
        private readonly BlockDecomposer block;
        private readonly Instruction instruction;
        private readonly Type type;

        public ConvertRunnable(BlockDecomposer block, Instruction instruction, Type type)
        {
            this.block = block;
            this.instruction = instruction;
            this.type = type;
        }

        public void run()
        {
            var lineNumber = instruction.Offset;
            block.label(new Label(lineNumber));
            block.addOp(new Convert(lineNumber, type));
        }
    }
}