using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinklouder.Testability.Metrics.Method.Op.Turing
{
public class FieldAssignment : Operation {

  private readonly Variable fieldInstance;
  private readonly FieldInfo field;
  private readonly Variable value;

  public FieldAssignment(int lineNumber, Variable fieldInstance,
      FieldInfo field, Variable value) : base(lineNumber){
    this.fieldInstance = fieldInstance;
    this.field = field;
    this.value = value;
  }

  public override void Visit(TestabilityVisitor.Frame visitor) {
    visitor.assignField(fieldInstance, field, value, LineNumber);
  }

  public override string ToString() {
    return field + " <- " + value;
  }
}
}
