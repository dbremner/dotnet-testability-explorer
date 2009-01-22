using C5;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class GetField : StackOperation
    {

        private readonly FieldInfo fieldInfo;

        public GetField(int lineNumber, FieldInfo fieldInfo)
            : base(lineNumber)
        {
            this.fieldInfo = fieldInfo;
        }

        public override string ToString()
        {
            return "get " + (fieldInfo.IsGlobal ? "static " : "") + fieldInfo;
        }

        public override int getOperatorCount()
        {
            return fieldInfo.IsGlobal ? 0 : 1;
        }

        public override IList<Variable> apply(IList<Variable> input)
        {
            Variable instance = fieldInfo.IsGlobal ? null : input[0];
            return list(new LocalField(instance, fieldInfo));
        }
    }
}
