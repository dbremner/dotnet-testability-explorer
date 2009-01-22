namespace Thinklouder.Testability.Metrics.Method.Op.Turing
{
    public class LocalAssignment : Operation
    {
        private readonly Variable value;
        private readonly Variable variable;

        public LocalAssignment(int lineNumber, Variable dst, Variable value)
            : base(lineNumber)
        {
            this.value = value;
            this.variable = dst;
        }

        public Variable getVariable()
        {
            return variable;
        }

        public Variable getValue()
        {
            return value;
        }

        public override string ToString()
        {
            return variable + " <- " + value;
        }

        public override void Visit(TestabilityVisitor.Frame visitor)
        {
            visitor.assignLocal(LineNumber, variable, value);
        }
    }
}