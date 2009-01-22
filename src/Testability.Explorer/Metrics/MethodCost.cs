using C5;

namespace Thinklouder.Testability.Metrics
{
    public class MethodCost
    {
        private readonly string methodName;
        private readonly int lineNumber;
        private readonly IList<ViolationCost> costSources = new ArrayList<ViolationCost>();
        private readonly Cost directCost = new Cost();
        private readonly Cost dependantCost = new Cost();

        /**
             * @param methodName
             *          name of the method, such as {@code void myMethod()}.
             * @param lineNumber
             *          line number
             */
        public MethodCost(string methodName, int lineNumber)
        {
            this.methodName = methodName;
            this.lineNumber = lineNumber;
        }

        public Cost getTotalCost()
        {
            return new Cost().Add(directCost).Add(dependantCost);
        }

        public string getMethodName()
        {
            return methodName;
        }

        public void addCostSource(ViolationCost costSource)
        {
            costSource.link(directCost, dependantCost);
            costSources.Add(costSource);
        }

        public override string ToString()
        {
            return getMethodName() + toCostsString();
        }

        public string toCostsString()
        {
            return " [" + getTotalCost() + " / " + directCost + "]";
        }

        public int getMethodLineNumber()
        {
            return lineNumber;
        }

        public IList<ViolationCost> getViolationCosts()
        {
            return costSources;
        }

        public IList<ViolationCost> getImplicitViolationCosts()
        {
            return filterViolationCosts(true);
        }
        public IList<ViolationCost> getExplicitViolationCosts()
        {
            return filterViolationCosts(false);
        }
        private IList<ViolationCost> filterViolationCosts(bool isImplicit)
        {
            IList<ViolationCost> result = new ArrayList<ViolationCost>();
            foreach ( ViolationCost cost in getViolationCosts() )
            {
                if ( cost.isImplicit() == isImplicit )
                {
                    result.Add(cost);
                }
            }
            return result;
        }

        public Cost getCost()
        {
            return directCost;
        }

        public IDictionary<string, object> getAttributes()
        {
            IDictionary<string, object> map = getTotalCost().GetAttributes();
            map["line"] = lineNumber;
            map["name"] = methodName;
            return map;
        }

        public void link()
        {
        }

    }
}