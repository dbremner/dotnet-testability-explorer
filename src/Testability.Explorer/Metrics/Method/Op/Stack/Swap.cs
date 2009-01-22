using C5;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
public class Swap : StackOperation {

  public Swap(int lineNumber) : base(lineNumber) {
  }

  public override int getOperatorCount() {
    return 2;
  }

  public override IList<Variable> apply(IList<Variable> input) {
    return list(input[1], input[0]);
  }

  public override string ToString() {
    return "swap";
  }
}

}
