using Mono.Cecil;
using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class SetFieldRunnable : IRunnable
    {
        private readonly IClassRepository repository;
        private readonly Instruction instruction;
        private readonly BlockDecomposer block;
        private readonly ClassInfo classInfo;

        public SetFieldRunnable(IClassRepository repository, ClassInfo classInfo, Instruction instruction, BlockDecomposer block)
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
            if (fd == null) return;

            var fieldName = fd.Name;
            var operand = instruction.Operand;

            try
            {
                field = ownerClass.GetField(fieldName);
            }
            catch (FieldNotFoundException e)
            {
                field =
                    new FieldInfo(ownerClass, fieldName, ClrType
                        .FromDescriptor(fd.FieldType.FullName), false, instruction.OpCode == OpCodes.Stsfld, fd.IsPrivate);
            }
            var lineNumber = instruction.Offset;
            block.label(new Label(lineNumber));
            block.addOp(new PutField(lineNumber, field));
        }
    }
}
