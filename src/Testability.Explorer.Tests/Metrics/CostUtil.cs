namespace Thinklouder.Testability.Tests.Metrics
{
    public class CostUtil
    {
        // This adds a global cost of 1 to everything else in the class. No matter
        // if it is a primitive or Object -- any time you have globaly mutable state
        // (non-final static state), you will have hard to test code.  (And hard to
        // parallelize code).
        private static bool x = false;

        public static bool staticCost0()
        {
            return x;
        }

        public bool instanceCost0()
        {
            return x;
        }

        public static bool staticCost1()
        {
            bool a = x ? false : true;
            return a;
        }

        public bool instanceCost1()
        {
            bool a = x ? false : true;
            return a;
        }

        public static bool staticCost2()
        {
            bool a = x ? false : true;
            bool b = a ? false : true;
            return b;
        }

        public bool instanceCost2()
        {
            bool a = x ? false : true;
            bool b = a ? false : true;
            return b;
        }

        public static bool staticCost3()
        {
            bool a = x ? false : true;
            bool b = a ? false : true;
            bool c = b ? false : true;
            return c;
        }

        public bool instanceCost3()
        {
            bool a = x ? false : true;
            bool b = a ? false : true;
            bool c = b ? false : true;
            return c;
        }

        public static bool staticCost4()
        {
            bool a = x ? false : true;
            bool b = a ? false : true;
            bool c = b ? false : true;
            bool d = c ? false : true;
            return d;
        }

        public bool instanceCost4()
        {
            bool a = x ? false : true;
            bool b = a ? false : true;
            bool c = b ? false : true;
            bool d = c ? false : true;
            return d;
        }
    }
}