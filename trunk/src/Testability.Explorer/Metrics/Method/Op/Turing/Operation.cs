namespace Thinklouder.Testability.Metrics.Method.Op.Turing
{
    public abstract class Operation
    {
        private readonly int lineNumber;

        protected Operation(int lineNumber)
        {
            this.lineNumber = lineNumber;
        }

        public int LineNumber{ get { return lineNumber; } }

        public abstract void Visit(TestabilityVisitor.Frame visitor);
    }
}