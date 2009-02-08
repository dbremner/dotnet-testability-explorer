using System;
using System.IO;
using C5;

namespace Thinklouder.Testability.Metrics
{
    public class MetricComputer
    {
        private readonly IClassRepository classRepository;
        private readonly Stream err;
        private readonly int recordingDepth;
        private readonly WhiteList whitelist;

        public MetricComputer(IClassRepository classRepository, Stream err,
                              WhiteList whitelist, int recordingDepth)
        {
            this.classRepository = classRepository;
            this.err = err;
            this.whitelist = whitelist;
            this.recordingDepth = recordingDepth;
        }

        public ClassCost compute(String name)
        {
            return compute(classRepository.GetClass(name));
        }

        /**
           * Computing the ClassCost for a ClassInfo involves tallying up all the MethodCosts contained
           * in the class. Then an overall cost is calculated, based on the {@code CostModel} the metric
           * computer is using.
           *
           * @param clazz to compute the metric for.
           * @return classCost
           */
        public ClassCost compute(ClassInfo clazz)
        {
            IList<MethodCost> methods = new ArrayList<MethodCost>();
            foreach (MethodInfo method in clazz.GetMethods())
            {
                methods.Add(compute(method));
            }
            return new ClassCost(clazz.Name, methods);
        }

        /**
           * Computing the MethodCost for a MethodInfo involves tallying up:
           * <ul><li>The cost in any static initialization blocks of the class which holds the method.</li>
           * <li>The cost of constructing the object.</li>
           * <li>Recognizing injectability through setter methods. This in most cases improves the score
           * unless you have lots of code in your setter.</li>
           * <li>The field costs</li>
           * <li>Lastly the costs are added up for all of the lines in this method. This includes the
           * transitive non-mockable/interceptable closure of all the costs of the methods that are called.</li>
           * <li></li></ul>
           *
           * @param method to compute the cost for.
           * @return MethodCost for this method, including the accumulated costs the methods it calls. This
           * MethodCost is guaranteed to have already been linked (sealed for adding additional costs).
           */
        public MethodCost compute(MethodInfo method)
        {
            var visitor = new TestabilityVisitor(classRepository, new VariableState(), err, whitelist);
            TestabilityVisitor.CostRecordingFrame frame = visitor.createFrame(method, recordingDepth);
            addStaticInitializationCost(method, frame);
            if (!method.IsStatic() && !method.IsConstructor())
            {
                addConstructorCost(method, frame);
                addSetterInjection(method, frame);
            }
            addFieldCost(method, frame);
            return frame.applyMethodOperations();
        }


        /** Goes through all methods and adds an implicit cost for those beginning with "set" (assuming
            * to test the {@code baseMethod}'s class, you need to be able to call the setters for initialization.  */
        private void addSetterInjection(MethodInfo baseMethod, TestabilityVisitor.CostRecordingFrame frame)
        {
            foreach (MethodInfo setter in baseMethod.GetSiblingSetters())
            {
                frame.applyImplicitCost(setter, Reason.IMPLICIT_SETTER);
            }
        }

        /** Adds an implicit cost to all non-static methods for calling the constructor. (Because to test
           * any instance method, you must be able to instantiate the class.) Also marks parameters
           * injectable for the constructor with the most non-primitive parameters. */
        private void addConstructorCost(MethodInfo method, TestabilityVisitor.CostRecordingFrame frame)
        {
            MethodInfo constructor = method.ClassInfo.GetConstructorWithMostNonPrimitiveParameters();
            if (constructor != null)
            {
                frame.applyImplicitCost(constructor, Reason.IMPLICIT_CONSTRUCTOR);
            }
        }

        /** Doesn't really add the field costs (there are none), but marks non-private fields as injectable. */
        private void addFieldCost(MethodInfo method,
                                  TestabilityVisitor.Frame frame)
        {
            foreach (FieldInfo field in method.ClassInfo.GetFields())
            {
                if (!field.IsPrivate())
                {
                    frame.GetGlobalVariables().SetInjectable(field);
                }
            }
        }

        /** Includes the cost of all static initialization blocks, as well as static field assignments. */
        private void addStaticInitializationCost(MethodInfo baseMethod, TestabilityVisitor.CostRecordingFrame frame)
        {
            if (baseMethod.IsStaticConstructor())
            {
                return;
            }
            foreach (var method in baseMethod.ClassInfo.GetMethods())
            {
                //if (method.Name.StartsWith("<clinit>"))
                if (method.Name.StartsWith(".cctor"))
                {
                    // TODO, different way to represent constructor method, by sunlw
                    frame.applyImplicitCost(method, Reason.IMPLICIT_STATIC_INIT);
                }
            }
        }
    }
}