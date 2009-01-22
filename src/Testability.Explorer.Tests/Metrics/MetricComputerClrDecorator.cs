using Thinklouder.Testability.Metrics;

namespace Thinklouder.Testability.Tests.Metrics
{
    /**
     * The {@code MetricComputer} needs to be language agnostic as it can work with
     * Java and C++ code. Yet, many tests need more convenient api's, which is what
     * this class provides.
     *
     * @author Jonathan Andrew Wolter
     */
    public class MetricComputerClrDecorator
    {

        private readonly MetricComputer metricComputer;
        private readonly IClassRepository classRepository;

        public MetricComputerClrDecorator(MetricComputer metricComputer,
            IClassRepository classRepository)
        {
            this.metricComputer = metricComputer;
            this.classRepository = classRepository;
        }

        public ClassCost compute(ClassInfo clazz)
        {
            return metricComputer.compute(clazz);
        }

        public MethodCost compute(MethodInfo method)
        {
            return metricComputer.compute(method);
        }

        /** used for testing */
        public MethodCost compute(string clazz, string methodName)
        {
            ClassInfo classInfo = classRepository.GetClass(clazz);
            MethodInfo method = classInfo.GetMethod(methodName);
            return metricComputer.compute(method);
        }

        /** used for testing */
        //public MethodCost compute(System.Type clazz, string method) // TODO, do we use System.Type, by sunlw
        //{
        //    return compute(clazz.Name, method);
        //}

        public MethodCost compute<T>(string method)
        {
            return compute(ClassInfo.GetFullName<T>(), method);
        }

        /** used for testing   */
        public ClassCost compute(string clazz)
        {
            return metricComputer.compute(classRepository.GetClass(clazz));
        }

        /** used for testing   */
        //public ClassCost compute(System.Type clazz)
        //{  // TODO, do we use System.Type, by sunlw
        //    return metricComputer.compute(classRepository.GetClass(clazz.Name));
        //}

        public ClassCost compute<T>()
        {
            return metricComputer.compute(classRepository.GetClass(ClassInfo.GetFullName<T>()));
        }

        public MetricComputer getDecoratedComputer()
        {
            return metricComputer;
        }
    }

}