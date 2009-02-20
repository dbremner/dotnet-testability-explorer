using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class BranchRunnable : IRunnable
    {
        private readonly BlockDecomposer block;
        private readonly Instruction instruction;

        public BranchRunnable(BlockDecomposer block, Instruction instruction)
        {
            this.block = block;
            this.instruction = instruction;
        }

        public void run()
        {
            block.label(new Label(instruction.Offset));
            switch (instruction.OpCode.StackBehaviourPop)
            {
                case StackBehaviour.Pop0:
                    block.addOp(new Transform(instruction.Offset, instruction.OpCode.Name, null, null,
                                      null));
                    break;
                case StackBehaviour.Popi:
                    block.addOp(new Transform(instruction.Offset, instruction.OpCode.Name, ClrType.Int32, null,
                                      null));
                    break;
                case StackBehaviour.Pop1_pop1:
                    block.addOp(new Transform(instruction.Offset, instruction.OpCode.Name, ClrType.Int32, ClrType.Int32,
                                      null));
                    break;
            }
            
            if(instruction.OpCode.FlowControl == FlowControl.Branch)
            {
                block.unconditionalGoto(new Label(((Instruction)instruction.Operand).Offset));
            }
            else if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch)
            {
                block.conditionalGoto(new Label(((Instruction)instruction.Operand).Offset));
            }
        }
    }
}