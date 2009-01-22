namespace Thinklouder.Testability.Metrics.Method.Op.Turing
{
    public class ReturnOperation : Operation
    {
        private readonly Variable value;

        public ReturnOperation(int lineNumber, Variable value)
            : base(lineNumber)
        {
            this.value = value;
        }

        public override void Visit(TestabilityVisitor.Frame visitor)
        {
            visitor.SetReturnValue(value);
        }

        public override string ToString()
        {
            return "return " + value;
        }
    }
}