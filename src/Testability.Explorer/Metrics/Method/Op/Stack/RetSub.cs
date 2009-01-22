namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class RetSub : Return
    {
        public RetSub(int lineNumber)
            : base(lineNumber, ClrType.Void)
        {
        }

        public override string ToString()
        {
            return "RETSUB";
        }
    }
}