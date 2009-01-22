namespace Thinklouder.Testability.Metrics
{
    public class CyclomaticCost : ViolationCost
    {
        public CyclomaticCost(int lineNumber, Cost cyclomaticCost) : base(lineNumber, cyclomaticCost)
        {
        }

        public override string getReason()
        {
            return "cyclomatic complexity";
        }

        public override void link(Cost directCost, Cost dependantCost)
        {
            directCost.AddDependant(getCost());
        }

        public override string getDescription()
        {
            return "Conditional cost";
        }

    }
}