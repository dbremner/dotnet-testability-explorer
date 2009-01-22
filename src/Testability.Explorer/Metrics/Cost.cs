using System;
using System.Collections.Generic;
using System.Text;
using C5;

namespace Thinklouder.Testability.Metrics
{
    public class Cost : IEquatable<Cost>
    {
        private static readonly string COMPLEXITY_COST_HELP_URL =
            "http://code.google.com/p/testability-explorer/wiki/ComplexityCostExplanation";

        private static readonly string GLOBAL_COST_HELP_URL =
            "http://code.google.com/p/testability-explorer/wiki/GlobalCostExplanation";

        private static readonly string LAW_OF_DEMETER_COST_HELP_URL =
            "http://code.google.com/p/testability-explorer/wiki/LawOfDemeterCostExplanation";

        private static readonly int[] EMPTY = new int[0];

        private int cyclomaticCost;
        private int globalCost;
        private int[] lodDistribution;

        public Cost() : this(0, 0, EMPTY)
        {
            ////new Cost(0, 0, EMPTY);
            //this.cyclomaticCost = 0;
            //this.globalCost = 0;
            //this.lodDistribution = EMPTY;
        }

        public Cost(int cyclomaticCost, int globalCost, int[] lodDistribution)
        {
            this.cyclomaticCost = cyclomaticCost;
            this.lodDistribution = lodDistribution;
            this.globalCost = globalCost;
        }

        public static Cost Global(int count)
        {
            return new Cost(0, count, EMPTY);
        }

        public static Cost LoD(int distance)
        {
            var distribution = new int[distance + 1];
            distribution[distance] = 1;
            return new Cost(0, 0, distribution);
        }

        public static Cost Cyclomatic(int cyclomaticCost)
        {
            return new Cost(cyclomaticCost, 0, EMPTY);
        }

        // TODO, might be ...
        public static Cost LoDDistribution(params int[] counts)
        {
            return new Cost(0, 0, counts);
        }

        public void AddDependant(Cost cost)
        {
            cyclomaticCost += cost.cyclomaticCost;
            globalCost += cost.globalCost;
        }

        public int GetCyclomaticComplexityCost()
        {
            return cyclomaticCost;
        }

        public int GetGlobalCost()
        {
            return globalCost;
        }


        public bool Equals(Cost other)
        {
            return this.GetHashCode() == other.GetHashCode();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            String sep = "";
            if (cyclomaticCost > 0)
            {
                builder.Append(sep);
                builder.Append("CC: " + cyclomaticCost);
                sep = ", ";
            }
            if (globalCost > 0)
            {
                builder.Append(sep);
                builder.Append("GC: " + globalCost);
                sep = ", ";
            }
            int loDSum = GetLoDSum();
            if (loDSum > 0)
            {
                builder.Append(sep);
                builder.Append("LOD: " + loDSum);
                sep = ", ";
            }
            return builder.ToString();
        }

        // TODO(jwolter): Refactor me so the html presentation representation is outside the Cost class.
        // Probably this means configure the urls in the template, and take all the cost types directly
        // in there. Build this string in the templating side of things, rather than in the core Cost
        // side of things here. But more important first is to write up useful html pages explaining the
        // costs.
        public String toHtmlReportString()
        {
            var builder = new StringBuilder();
            String sep = "";
            if (cyclomaticCost > 0)
            {
                builder.Append(sep);
                builder.Append("<a href=\"" + COMPLEXITY_COST_HELP_URL + "\">CC: " + cyclomaticCost + "</a>");
                sep = ", ";
            }
            if (globalCost > 0)
            {
                builder.Append(sep);
                builder.Append("<a href=\"" + GLOBAL_COST_HELP_URL + "\">GC: " + globalCost + "</a>");
                sep = ", ";
            }
            int loDSum = GetLoDSum();
            if (loDSum > 0)
            {
                builder.Append(sep);
                builder.Append("<a href=\"" + LAW_OF_DEMETER_COST_HELP_URL + "\">LOD: " + loDSum + "</a>");
                sep = ", ";
            }
            return builder.ToString();
        }

        public int GetLoDSum()
        {
            int sum = 0;
            foreach (int value in lodDistribution)
            {
                sum += value;
            }
            return sum;
        }

        public Cost Copy()
        {
            return new Cost(cyclomaticCost, globalCost, lodDistribution);
        }

        public Cost CopyNoLOD()
        {
            return new Cost(cyclomaticCost, globalCost, EMPTY);
        }


        public int[] GetLoDDistribution()
        {
            return lodDistribution;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + cyclomaticCost;
            result = prime * result + globalCost;
            result = prime * result + HashCode(lodDistribution);
            return result;
        }

        public Cost Add(Cost cost)
        {
            cyclomaticCost += cost.cyclomaticCost;
            globalCost += cost.globalCost;
            int[] other = cost.lodDistribution;
            int size = Math.Max(lodDistribution.Length, other.Length);
            int[] old = lodDistribution;
            if (lodDistribution.Length < size)
            {
                lodDistribution = new int[size];
            }
            for (int i = 0; i < size; i++)
            {
                int count1 = i < old.Length ? old[i] : 0;
                int count2 = i < other.Length ? other[i] : 0;
                lodDistribution[i] = count1 + count2;
            }
            return this;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            var other = (Cost)obj;
            if (cyclomaticCost != other.cyclomaticCost)
            {
                return false;
            }
            if (globalCost != other.globalCost)
            {
                return false;
            }
            if (Compare(lodDistribution, other.lodDistribution))
            {
                return false;
            }
            return true;
        }

        private static bool Compare(int[] array1, int[] array2)
        {
            if (array1.Length != array2.Length) return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }

            return true;
        }

        private static int HashCode(int[] a)
        {
            if (a == null) return 0;

            int result = 1;
            foreach (int element in a)
            {
                result = 31 * result + element;
            }
            return result;
        }

        public C5.IDictionary<string, object> GetAttributes()
        {
            var atts = new HashDictionary<string, object>
                           {
                               {"cyclomatic", GetCyclomaticComplexityCost()},
                               {"global", GetGlobalCost()},
                               {"lod", GetLoDSum()}
                           };
            return atts;
        }


        public void AddCyclomaticCost(int cyclomaticCost)
        {
            this.cyclomaticCost += cyclomaticCost;
        }

        public void AddGlobalCost(int globalCost)
        {
            this.globalCost += globalCost;
        }

        public void AddLodDistance(int distance)
        {
            Add(Cost.LoD(distance));
        }

        public bool isEmpty()
        {
            return lodDistribution.Length == 0 &&
                   cyclomaticCost == 0 &&
                   globalCost == 0;
        }

        public Cost copyNoLOD()
        {
            return new Cost(cyclomaticCost, globalCost, EMPTY);
        }
    }


    public class CostModel
    {

        /**
         * Increase the weight we give on expensive methods. The ClassCost weighted
         * average will be skewed towards expensive-to-test methods' costs within the
         * class.
         */
        public static readonly double WEIGHT_TO_EMPHASIZE_EXPENSIVE_METHODS = 1.5;
        private static readonly int DEFAULT_CYCLOMATIC_MULTIPLIER = 1;
        private static readonly int DEFAULT_GLOBAL_MULTIPLIER = 10;
        private readonly double cyclomaticMultiplier;
        private readonly double globalMultiplier;

        public CostModel()
            : this(DEFAULT_CYCLOMATIC_MULTIPLIER, DEFAULT_GLOBAL_MULTIPLIER)
        {
        }

        public CostModel(double cyclomaticMultiplier, double globalMultiplier)
        {
            this.cyclomaticMultiplier = cyclomaticMultiplier;
            this.globalMultiplier = globalMultiplier;
        }

        public int computeOverall(Cost cost)
        {
            double sum = 0;
            sum += cyclomaticMultiplier * cost.GetCyclomaticComplexityCost();
            sum += globalMultiplier * cost.GetGlobalCost();
            foreach (int count in cost.GetLoDDistribution())
            {
                sum += count;
            }
            return (int)sum;
        }

        public int computeClass(ClassCost classCost)
        {
            WeightedAverage average = new WeightedAverage(
                WEIGHT_TO_EMPHASIZE_EXPENSIVE_METHODS);
            foreach (MethodCost methodCost in classCost.getMethods())
            {
                average.addValue(computeOverall(methodCost.getTotalCost()));
            }
            return (int)average.getAverage();
        }


    }

    public class ClassCost
    {

        public class CostComparator : IComparer<ClassCost>
        {
            private readonly CostModel costModel;

            public CostComparator(CostModel costModel)
            {
                this.costModel = costModel;
            }

            public int Compare(ClassCost c1, ClassCost c2)
            {
                int diff = (costModel.computeClass(c2) - costModel.computeClass(c1));
                return diff == 0 ? c1.className.CompareTo(c2.className) : diff;
            }


        }

        private readonly C5.IList<MethodCost> methods;
        private readonly String className;

        public ClassCost(String className, C5.IList<MethodCost> methods)
        {
            this.className = className;
            this.methods = methods;
        }

        public MethodCost getMethodCost(String methodName)
        {
            foreach (MethodCost cost in methods)
            {
                if (cost.getMethodName().Equals(methodName))
                {
                    return cost;
                }
            }
            throw new ArgumentException("Method '" + methodName
                + "' does not exist.");
        }

        public override string ToString()
        {
            return className;
        }

        public string getClassName()
        {
            return className;
        }

        public string getPackageName()
        {

            return getClassName().LastIndexOf('.') == -1 ? "" : getClassName().Substring(0, getClassName().LastIndexOf('.'));
        }

        public C5.IList<MethodCost> getMethods()
        {
            return methods;
        }

        // TODO: delete
        public long getTotalComplexityCost()
        {
            long totalCost = 0;
            foreach (MethodCost methodCost in getMethods())
            {
                totalCost += methodCost.getTotalCost().GetCyclomaticComplexityCost();
            }
            return totalCost;
        }

        // TODO: delete
        public long getHighestMethodComplexityCost()
        {
            long cost = 0;
            foreach (MethodCost methodCost in getMethods())
            {
                if (methodCost.getTotalCost().GetCyclomaticComplexityCost() > cost)
                {
                    cost = methodCost.getTotalCost().GetCyclomaticComplexityCost();
                }
            }
            return cost;
        }

        // TODO: delete
        public long getTotalGlobalCost()
        {
            long totalCost = 0;
            foreach (MethodCost methodCost in getMethods())
            {
                totalCost += methodCost.getTotalCost().GetGlobalCost();
            }
            return totalCost;
        }

        // TODO: delete
        public long getHighestMethodGlobalCost()
        {
            long cost = 0;
            foreach (MethodCost methodCost in getMethods())
            {
                if (methodCost.getTotalCost().GetGlobalCost() > cost)
                {
                    cost = methodCost.getTotalCost().GetGlobalCost();
                }
            }
            return cost;
        }

        public C5.IDictionary<string, object> getAttributes()
        {
            var map = new HashDictionary<String, Object>();
            map["class"] = className;
            return map;
        }
    }

    public class WeightedAverage
    {
        public static readonly double WEIGHT = 0.3;
        private readonly double weight;
        private double overallSum = 0;
        private double overallSqr = 0;

        public WeightedAverage()
        {
            new WeightedAverage(WEIGHT);
        }

        public WeightedAverage(double weight)
        {
            this.weight = weight;
        }

        public void addValue(long value)
        {
            overallSqr += Math.Pow(value, weight + 1);
            overallSum += Math.Pow(value, weight);
        }


        public double getAverage()
        {
            double result = overallSqr / overallSum;
            if (result.Equals(double.NaN)) result = 0;
            return result;
        }
    }
}