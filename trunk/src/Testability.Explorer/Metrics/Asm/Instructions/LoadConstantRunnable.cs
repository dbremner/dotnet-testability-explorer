using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class LoadConstantRunnable : IRunnable
    {
        private readonly BlockDecomposer block;
        private readonly Instruction instruction;
        private readonly object value;
        private readonly Type type;

        public LoadConstantRunnable(BlockDecomposer block, Instruction instruction, object value, Type type)
        {
            this.block = block;
            this.instruction = instruction;
            this.value = value;
            this.type = type;
        }

        public void run()
        {
            var lineNumber = instruction.Offset;
            block.label(new Label(lineNumber));
            block.addOp(new Load(instruction.Offset, new Constant(value, type)));
        }
    }
}