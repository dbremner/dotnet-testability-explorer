using C5;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class PutField : StackOperation
    {

        private readonly FieldInfo fieldInfo;

        public PutField(int lineNumber, FieldInfo fieldInfo)
            : base(lineNumber)
        {
            this.fieldInfo = fieldInfo;
        }

        public override string ToString()
        {
            return "put " + (fieldInfo.IsGlobal ? "static " : "") + fieldInfo;
        }

        public override int getOperatorCount()
        {
            int valueCount = ClrType.IsDoubleSlot(fieldInfo.Type) ? 2 : 1;
            int fieldThis = fieldInfo.IsGlobal ? 0 : 1;
            return valueCount + fieldThis;
            //return 1; // TODO, yes there is only one operator in PutField... by sunlw
        }


        public override Operation ToOperation(IList<Variable> input)
        {
            if (fieldInfo.IsGlobal)
            {
                Variable value = input[0];
                return new FieldAssignment(lineNumber, null, fieldInfo, value);
            }
            else
            {
                Variable instance = input[0];
                Variable value = input[1];
                return new FieldAssignment(lineNumber, instance, fieldInfo, value);
            }
        }
    }
}
