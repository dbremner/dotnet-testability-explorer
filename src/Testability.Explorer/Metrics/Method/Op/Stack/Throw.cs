namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class Throw : Pop
    {
        public Throw(int lineNumber) : base(lineNumber, 1)
        {
        }

        public override string ToString()
        {
            return "throw";
        }
    }
}