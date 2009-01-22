using C5;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class Load : StackOperation
    {
        private readonly Variable variable;

        public Load(int lineNumber, Variable variable) : base(lineNumber)
        {
            this.variable = variable;
        }

        public override string ToString()
        {
            string info;
            if(variable==null)
                info = "null";
            else
                info = variable.ToString();
            return "load " + info;
        }

        public override IList<Variable> apply(IList<Variable> input)
        {
            return list(variable);
        }

        public override int getOperatorCount()
        {
            return 0;
        }
    }
}