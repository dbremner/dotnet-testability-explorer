using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class ReturnRunnable : IRunnable
    {
        private readonly BlockDecomposer block;
        private readonly Instruction instruction;

        public ReturnRunnable(BlockDecomposer block, Instruction instruction)
        {
            this.block = block;
            this.instruction = instruction;
        }

        public void run()
        {
            var lineNumber = instruction.Offset;
            block.label(new Label(lineNumber));
            block.addOp(new RetSub(lineNumber));
        }
    }
}
