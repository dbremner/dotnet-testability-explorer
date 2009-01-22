using C5;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class JMP : StackOperation
    {
        public Block Block { get; set; }

        public JMP(int lineNumber, Block block) : base(lineNumber)
        {
            Block = block;
        }

        public override IList<Variable> apply(IList<Variable> input)
        {
            return list(new Constant("return address", ClrType.Object));
        }

        public override string ToString()
        {
            return "JMP " + Block.getId();
        }
    }
}