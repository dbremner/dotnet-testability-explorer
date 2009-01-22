using C5;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class Return : Pop
    {

        private readonly Type type;

        public Return(int lineNumber, Type type)
            : base(lineNumber, type == ClrType.Void ? 0 : ( ClrType.IsDoubleSlot(type) ? 2 : 1 ))
        {
            this.type = type;
        }

        public override string ToString()
        {
            return "return " + type;
        }

        public override Operation ToOperation(IList<Variable> input)
        {
            if (type == ClrType.Void)
            {
                return base.ToOperation(input);
            }
            else
            {
                return new ReturnOperation(lineNumber, input[0]);
            }
        }
    }
}