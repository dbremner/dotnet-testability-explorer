using C5;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class Duplicate : StackOperation
    {
        private readonly int offset;
        public Duplicate(int lineNumber, int offset)
            : base(lineNumber)
        {
            this.offset = offset;
        }

        public override int getOperatorCount()
        {
            return 1 + offset;
        }

        public override IList<Variable> apply(IList<Variable> input)
        {
            var list = new LinkedList<Variable>();
            list.AddAll(input);
            //list.Add(0, );
            list.Insert(0, input[input.Count - 1]);
            return list;
        }

        public override string ToString()
        {
            return "duplicate" + (offset > 0 ? "_X" + offset : "");
        }

    }
}