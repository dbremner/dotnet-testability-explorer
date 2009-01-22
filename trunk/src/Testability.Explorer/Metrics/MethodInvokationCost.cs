using C5;

namespace Thinklouder.Testability.Metrics
{
    public class MethodInvokationCost : ViolationCost
    {
        private readonly MethodCost methodCost;
        private readonly Reason costSourceType;

        public MethodInvokationCost(int lineNumber, MethodCost methodCost, Reason costSourceType, Cost invocationCost) : base(lineNumber,invocationCost)
        {
            this.methodCost = methodCost;
            this.costSourceType = costSourceType;
        }

        public override string getReason()
        {
            return costSourceType.ToString();
        }

        public override bool isImplicit()
        {
            return (costSourceType & Reason.ALL) == Reason.IMPLICIT;
        }

        public override void link(Cost directCost, Cost dependantCost)
        {
            dependantCost.AddDependant(cost);
        }

        public MethodCost getMethodCost()
        {
            return methodCost;
        }

        public override string getDescription()
        {
            return methodCost.getMethodName();
        }

        public override IDictionary<string, object> getAttributes() {
            IDictionary<string, object> map = base.getAttributes();
            map["method"] = methodCost.getMethodName();
            return map;
        }
    }
}