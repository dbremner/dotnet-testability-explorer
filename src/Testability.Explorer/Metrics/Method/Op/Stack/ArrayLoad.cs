using System;
using C5;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class ArrayLoad : StackOperation
    {
        private readonly Type type;

        public ArrayLoad(int lineNumber, Type type)
            : base(lineNumber)
        {
            this.type = type;
        }

        public override int getOperatorCount()
        {
            return 2;
        }

        public override IList<Variable> apply(IList<Variable> input)
        {
            if (!input[0].Type.IsObject())
            {
                throw new InvalidOperationException();
            }
            return list(new Constant("?", type));
        }

        public override string ToString()
        {
            return "arrayload:" + type;
        }
    }
}
