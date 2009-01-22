using C5;

namespace Thinklouder.Testability.Metrics
{
    public abstract class ViolationCost
    {

        private readonly int lineNumber;
        protected readonly Cost cost;

        /**
         * @param lineNumber
         *          that the {@code methodCost} was called on for the class that
         *          contains this cost.
         * @param cost
         *          the cost of the method getting called from this {@code LineNumber}
         */
        public ViolationCost(int lineNumber, Cost cost)
        {
            this.lineNumber = lineNumber;
            this.cost = cost;
        }

        public int getLineNumber()
        {
            return lineNumber;
        }

        public abstract string getReason();


        public override string ToString()
        {
            return "Line " + lineNumber + ": " + getDescription() + " (" + getReason() + ")";
        }

        // TODO: (misko) get rid of this method
        public abstract void link(Cost directCost, Cost dependantCost);

        public Cost getCost()
        {
            return cost;
        }

        public abstract string getDescription();

        public virtual IDictionary<string, object> getAttributes()
        {
            IDictionary<string, object> atts = cost.GetAttributes();

            atts["reason"] = getReason();
            atts["line"] = getLineNumber();
            return atts;
        }

        public virtual bool isImplicit()
        {
            return false;
        }
    }
}