using C5;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class Transform : StackOperation
    {
        private readonly string operation;
        private readonly Type op1;
        private readonly Type op2;
        private readonly Type result;

        public Transform(int lineNumber, string operation, Type op1, Type op2,
            Type result)
            : base(lineNumber)
        {
            this.operation = operation;
            this.op1 = op1;
            this.op2 = op2;
            this.result = result;
        }

        public override int getOperatorCount()
        {
            return size(op1) + size(op2);
        }

        private int size(Type op)
        {
            return op == null ? 0 : ClrType.IsDoubleSlot(op) ? 2 : 1;
        }

        public override IList<Variable> apply(IList<Variable> input)
        {
            if (result == null)
            {
                return base.apply(input);
            }
            else
            {
                return list(new Constant("?", result));
            }
        }

        public override string ToString()
        {
            string sep = " ";
            string buf = operation;
            if (op1 != null)
            {
                buf += sep + op1;
                sep = ", ";
            }
            if (op2 != null)
            {
                buf += sep + op2;
            }
            if (result != null)
            {
                buf += " -> " + result;
            }
            return buf;
        }

    }

}
