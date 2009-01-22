namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class MonitorEnter : StackOperation
    {

        public MonitorEnter(int lineNumber)
            : base(lineNumber)
        {
        }

        public override int getOperatorCount()
        {
            return 1;
        }

        public override string ToString()
        {
            return "monitor enter";
        }
    }
}