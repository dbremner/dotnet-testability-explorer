using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class LoadRunnable : IRunnable
    {
        private readonly BlockDecomposer block;
        private readonly Instruction instruction;
        private readonly Variable variable;

        public LoadRunnable(BlockDecomposer block, Instruction instruction, Variable variable)
        {
            this.block = block;
            this.instruction = instruction;
            this.variable = variable;
        }

        public void run()
        {
            var lineNumber = instruction.Offset;
            block.label(new Label(lineNumber));
            block.addOp(new Load(lineNumber, variable));
        }
    }
}