using C5;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class Store : StackOperation
    {
        private readonly Variable variable;

        public Store(int lineNumber, Variable variable)
            : base(lineNumber)
        {
            this.variable = variable;
        }

        public override string ToString()
        {
            return "store " + variable;
        }

        public override int getOperatorCount()
        {
            return ClrType.IsDoubleSlot(variable.Type) ? 2 : 1;
        }

        public override Operation ToOperation(IList<Variable> input)
        {
            return new LocalAssignment(lineNumber, variable, input[0]);
        }

    }
}