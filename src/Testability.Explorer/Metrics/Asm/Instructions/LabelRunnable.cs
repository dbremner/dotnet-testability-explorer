using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class LabelRunnable : IRunnable
    {
        private BlockDecomposer block;
        private int offset;

        // TODO, if a offset is enough?
        public LabelRunnable(BlockDecomposer block, int offset)
        {
            this.block = block;
            this.offset = offset;
        }

        public void run()
        {
            Label label = new Label(offset);
            block.label(label);
        }
    }
}