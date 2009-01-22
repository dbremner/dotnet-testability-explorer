using C5;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class Convert : StackOperation
    {
        private readonly Type to;

        public Convert(int lineNumber, Type to)
            : base(lineNumber)
        {
            this.to = to;
        }

        public override int getOperatorCount()
        {
            return ClrType.IsDoubleSlot(to) ? 2 : 1;
        }

        public override IList<Variable> apply(IList<Variable> input)
        {
            Variable variable = input[0];
            return list(new Constant(variable.Name, to));
        }

        public override string ToString()
        {
            return "convert " + " -> " + to;
        }
    }
}