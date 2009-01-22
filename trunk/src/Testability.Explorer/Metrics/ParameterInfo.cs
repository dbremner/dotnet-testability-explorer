namespace Thinklouder.Testability.Metrics
{
    public class ParameterInfo : Variable
    {
        public ParameterInfo(string name, Type type) : base(name, type, false, false)
        {
        }

        public override string ToString()
        {
            return GetName();
        }
    }
}
