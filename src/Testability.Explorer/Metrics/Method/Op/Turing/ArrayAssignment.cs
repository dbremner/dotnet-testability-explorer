namespace Thinklouder.Testability.Metrics.Method.Op.Turing
{
    public class ArrayAssignment : Operation
    {

        private readonly Variable array;
        private readonly Variable index;
        private readonly Variable value;

        public ArrayAssignment(int lineNumber, Variable array, Variable index, Variable value)
            : base(lineNumber)
        {
            this.array = array;
            this.index = index;
            this.value = value;
        }

        public override void Visit(TestabilityVisitor.Frame visistor)
        {
            visistor.assignArray(array, index, value, LineNumber);
        }

        public override string ToString()
        {
            return array + "[" + index + "] <- " + value;
        }

    }
}
