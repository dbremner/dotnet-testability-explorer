using C5;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public abstract class StackOperation
    {
        protected readonly int lineNumber;

        public StackOperation(int lineNumber)
        {
            this.lineNumber = lineNumber;
        }

        /**
         * Stack transformation operation.
         * @param input list of operands which this operations consumes on stack
         * @return a list of operands which get pushed back on stack.
         */

        public virtual IList<Variable> apply(IList<Variable> input)
        {
            return new ArrayList<Variable>();
        }

        public virtual int getOperatorCount()
        {
            return 0;
        }

        /**
         * High level Turing Operations which get produced from the stack operations
         *
         * @param input
         * @return null if no operations; Turing Operation otherwise
         */

        public virtual Operation ToOperation(IList<Variable> input)
        {
            return null;
        }

        protected IList<Variable> list(params Variable[] vars)
        {
            var list = new ArrayList<Variable>(vars.Length);
            foreach ( var variable in vars )
            {
                list.Add(variable);
                if ( ClrType.IsDoubleSlot(variable.Type) )
                {
                    list.Add(variable);
                }
            }
            return list;
        }

        public int getLineNumber()
        {
            return lineNumber;
        }
    }
}