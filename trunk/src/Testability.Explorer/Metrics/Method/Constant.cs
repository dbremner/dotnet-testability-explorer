namespace Thinklouder.Testability.Metrics.Method
{
    public class Constant : Variable
    {
        public Constant(object value, Type type)
            : base(value==null?"null":value.ToString(), type, false, false)
        {
        }
    }
}
