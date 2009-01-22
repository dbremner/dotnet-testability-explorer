namespace Thinklouder.Testability.Tests.IL
{
    public class OpIncrementSample
    {
        public int Increment()
        {
            var a = 0;
            a++;
            var b = a + 4;
            return b;
        }
    }
}