using C5;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class TryCatchRunnable : IRunnable
    {
        private readonly BlockDecomposer block;
        private readonly IList<int> cyclomaticComplexity;
        private readonly Instruction tryStart;
        private readonly Instruction tryEnd;
        private readonly Instruction handlerStart;
        private readonly Instruction handlerEnd;
        private readonly string type;

        public TryCatchRunnable(BlockDecomposer block, IList<int> cyclomaticComplexity, Instruction tryStart, Instruction tryEnd, 
                                Instruction handlerStart, Instruction handlerEnd, TypeReference type)
        {
            this.block = block;
            this.cyclomaticComplexity = cyclomaticComplexity;
            this.tryStart = tryStart;
            this.tryEnd = tryEnd;
            this.handlerStart = handlerStart;
            this.handlerEnd = handlerEnd;
            this.type = type == null ? null : type.FullName;
        }

        public void run()
        {
            if(type!=null)
            {
                cyclomaticComplexity.Add(handlerStart.Offset);
            }
            block.tryCatchBlock(new Label(tryStart.Offset), new Label(tryEnd.Offset), new Label(handlerStart.Offset), type);
        }
    }
}