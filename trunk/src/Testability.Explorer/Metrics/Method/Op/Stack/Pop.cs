namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class Pop : StackOperation
    {
        private int count;

        public Pop(int lineNumber, int count)
            : base(lineNumber)
        {
            this.count = count;
        }

        public override int getOperatorCount()
        {
            return count;
        }

        public override string ToString()
        {
            return "pop" + ( count > 1 ? "" + count : "" );
        }
    }
}