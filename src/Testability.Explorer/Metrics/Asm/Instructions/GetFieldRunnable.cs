using Mono.Cecil;
using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class GetFieldRunnable : IRunnable
    {
        private IClassRepository repository;
        private Instruction instruction;
        private BlockDecomposer block;
        private ClassInfo classInfo;

        public GetFieldRunnable(IClassRepository repository, ClassInfo classInfo, Instruction instruction, BlockDecomposer block)
        {
            this.repository = repository;
            this.classInfo = classInfo;
            this.instruction = instruction;
            this.block = block;
        }

        public void run()
        {
            FieldInfo field = null;
            ClassInfo ownerClass = repository.GetClass(classInfo.Name);
            var fd = instruction.Operand as FieldDefinition;
            if(fd==null) return;

            var fieldName = fd.Name;
            var operand = instruction.Operand;

            try
            {
                field = ownerClass.GetField(fieldName);
            }
            catch (FieldNotFoundException e)
            {
                field =
                    //new FieldInfo(ownerClass, "FAKE:" + fieldName, ClrType
                    new FieldInfo(ownerClass, fieldName, ClrType.FromDescriptor(fd.FieldType.FullName), 
                        false, instruction.OpCode == OpCodes.Ldsfld || instruction.OpCode == OpCodes.Ldsflda, fd.IsPrivate);
            }

            var lineNumber = instruction.Offset;
            block.label(new Label(lineNumber));
            block.addOp(new GetField(instruction.Offset, field));
        }
    }

    
}