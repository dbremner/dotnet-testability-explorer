using System;
using C5;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class ArrayStore : StackOperation
    {
        private readonly Type type;

        public ArrayStore(int lineNumber, Type type)
            : base(lineNumber)
        {
            this.type = type;
        }

        public override int getOperatorCount()
        {
            return 2 + (ClrType.IsDoubleSlot(type) ? 2 : 1);
        }

        public override Operation ToOperation(IList<Variable> input)
        {
            if (!input[0].Type.IsObject())
            {
                throw new InvalidOperationException();
            }
            Variable array = input[0];
            Variable index = input[1];
            Variable value = input[2];
            return new ArrayAssignment(lineNumber, array, index, value);
        }

        public override string ToString()
        {
            return "arraystore";
        }

    }
}