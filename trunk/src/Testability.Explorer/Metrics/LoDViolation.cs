namespace Thinklouder.Testability.Metrics
{
    public class LoDViolation : ViolationCost
    {
        private readonly string methodName;
        private readonly int distance;

        public LoDViolation(int lineNumber, string methodName, Cost lod, int distance) : base(lineNumber, lod)
        {
            this.methodName = methodName;
            this.distance = distance;
        }

        public override string getReason()
        {
            return "cost from breaking the Law of Demeter";
        }

        public override void link(Cost directCost, Cost dependantCost)
        {
            directCost.Add(getCost());
        }

        public override string getDescription()
        {
            return methodName + "[distance=" + distance + "]";
        }
    }
}