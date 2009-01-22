namespace Thinklouder.Testability.Metrics
{
    public class GlobalCost : ViolationCost
    {
        private readonly Variable variable;

        public GlobalCost(int lineNumber, Variable variable, Cost globalCost) : base(lineNumber, globalCost)
        {
            this.variable = variable;
        }

        public override string getReason()
        {
            return "dependency on global mutable state";
        }

        public override void  link(Cost directCost, Cost dependantCost)
        {
            directCost.Add(getCost());
        }

        public override string getDescription()
        {
            return variable.GetName() + ":" + variable.GetType();
        }
    }
}