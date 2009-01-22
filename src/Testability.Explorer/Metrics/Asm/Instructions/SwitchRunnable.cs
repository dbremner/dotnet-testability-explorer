using C5;
using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class SwitchRunnable : IRunnable
    {
        private readonly BlockDecomposer block;
        private readonly IList<int> cyclomaticComplexity;
        private readonly Instruction defaultCase;
        private readonly Instruction[] cases;

        public SwitchRunnable(BlockDecomposer block, IList<int> cyclomaticComplexity, Instruction defaultCase, params Instruction[] cases)
        {
            this.block = block;
            this.cyclomaticComplexity = cyclomaticComplexity;
            this.defaultCase = defaultCase;
            this.cases = cases;
        }

        public void run()
        {
            foreach (Instruction ins in cases)
            {
                cyclomaticComplexity.Add(ins.Offset);
            }
            var lineNumber = defaultCase.Offset;
            block.label(new Label(lineNumber));
            block.addOp(new Pop(lineNumber, 1));
            block.tableSwitch(new Label(lineNumber), asLabels(cases));
        }

        public IList<Label> asLabels(params Instruction[] cases)
        {
            var labels = new ArrayList<Label>();
            foreach (Instruction instruction in cases)
            {
                labels.Add(new Label(instruction.Offset));
            }
            return labels;
        }
    }
}
