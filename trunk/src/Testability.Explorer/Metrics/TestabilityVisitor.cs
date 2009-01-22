using System;
using System.IO;
using C5;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Metrics
{
    public class TestabilityVisitor
    {
        // TODO: refactor me. The root frame needs to be of different class so that
        // we can remove all of the ifs in Frame
        private readonly IClassRepository classRepository;
        private readonly VariableState globalVariables;
        private readonly Stream err;
        private readonly WhiteList whiteList;

        public TestabilityVisitor(IClassRepository classRepository, VariableState globalVariables, Stream err, WhiteList whiteList)
        {
            this.classRepository = classRepository;
            this.globalVariables = globalVariables;
            this.err = err;
            this.whiteList = whiteList;
        }

        public CostRecordingFrame createFrame(MethodInfo method, int recordingDepth)
        {
            CostRecordingFrame frame = new CostRecordingFrame(err, classRepository, whiteList, globalVariables, method, recordingDepth);
            return frame;
        }

        public class Frame : ParentFrame
        {
            protected readonly ParentFrame parentFrame;
            protected readonly Cost direct = new Cost();
            protected readonly Cost indirect = new Cost();
            protected readonly MethodInfo method;
            protected Variable returnValue;
            protected readonly WhiteList whiteList;
            protected readonly Stream err;
            protected readonly IClassRepository classRepository;
            protected readonly ICollection<MethodInfo> alreadyVisited;

            public Frame()
            { }

            public Frame(Stream err, IClassRepository classRepository,
                ParentFrame parentFrame, WhiteList whiteList,
                VariableState globalVariables, ICollection<MethodInfo> alreadyVisited,
                MethodInfo method)
                : base(globalVariables)
            {
                this.err = err;
                this.method = method;
                this.alreadyVisited = alreadyVisited;
                this.whiteList = whiteList;
                this.parentFrame = parentFrame;
                this.classRepository = classRepository;
                alreadyVisited.Add(method);
            }

            protected virtual void AddCyclomaticCost(int lineNumber)
            {
                direct.AddCyclomaticCost(1);
            }

            public void SetReturnValue(Variable value)
            {
                bool isWorse = variableState.IsGlobal(value)
                    && !variableState.IsGlobal(returnValue);
                if (isWorse)
                {
                    returnValue = value;
                }
            }

            protected void addGlobalCost(int lineNumber, Variable variable)
            {
                direct.AddGlobalCost(1);
            }

            protected void addMethodInvocationCost(int number, MethodInfo to, Cost methodInvocationCost, Reason reason)
            {
                indirect.Add(methodInvocationCost);
            }

            /**
             * If and only if the array is a static, then add it as a Global State Cost
             * for the {@code inMethod}.
             */
            public void assignArray(Variable array, Variable index, Variable value,
                int lineNumber)
            {
                if (variableState.IsGlobal(array))
                {
                    addGlobalCost(lineNumber, array);
                }
            }

            /**
             * The method propagates the global property of a field onto any field it is
             * assigned to. The globality is propagated because global state is
             * transitive (static cling) So any modification on class which is
             * transitively global should also be penalized.
             *
             * <p>
             * Note: <em>final</em> static fields are not added, because they are
             * assumed to be constants, thus this will miss some actual global state.
             * (The justification is that if costs were included for constants it would
             * penalize people for a good practice -- removing magic values from code).
             */
            public void assignField(Variable fieldInstance, FieldInfo field,
                Variable value, int lineNumber)
            {
                assignVariable(field, lineNumber, this, value);
                if (fieldInstance == null || variableState.IsGlobal(fieldInstance))
                {
                    if (!field.IsFinal)
                    {
                        addGlobalCost(lineNumber, field);
                    }
                    variableState.SetGlobal(field);
                }
            }

            public void assignLocal(int lineNumber, Variable destination, Variable source)
            {
                assignVariable(destination, lineNumber, this, source);
            }

            protected void SetInjectable(IList<ParameterInfo> parameters)
            {
                foreach (Variable variable in parameters)
                {
                    variableState.SetInjectable(variable);
                }
            }

            protected void recordNonOveridableMethodCall<T>(Reason reason, int lineNumber,
                MethodInfo toMethod, Variable methodThis,
                IList<T> parameters, Variable returnVariable) where T : Variable
            {
                Frame childFrame = createChildFrame(toMethod);
                childFrame.recordMethodCall<T>(lineNumber, toMethod, methodThis, parameters,
                    returnVariable);
                addMethodInvocationCost(lineNumber, toMethod, childFrame.getTotalCost().copyNoLOD(), reason);
            }

            protected virtual Frame createChildFrame(MethodInfo toMethod)
            {
                return new Frame(err, classRepository, this, whiteList,
                    getGlobalVariables(), alreadyVisited, toMethod);
            }

            private VariableState getGlobalVariables()
            {
                return globalVariableState;
            }

            private void recordMethodCall<T>(int lineNumber, MethodInfo toMethod,
                Variable methodThis, IList<T> parameters,
                Variable returnVariable) where T : Variable
            {
                foreach (int lineNumberWithComplexity in toMethod.LinesOfComplexity)
                {
                    AddCyclomaticCost(lineNumberWithComplexity);
                }
                if (toMethod.isInstance())
                {
                    assignParameter(lineNumber, toMethod.MethodThis, parentFrame,
                        methodThis);
                }
                applyMethodOperations(lineNumber, toMethod, methodThis, parameters,
                    returnVariable);
                assignReturnValue(lineNumber, returnVariable);
            }

            public void recordMethodCall<T>(String clazzName, int lineNumber,
                    String methodName, Variable methodThis, IList<T> parameters,
                    Variable returnVariable) where T : Variable
            {
                try
                {
                    if (whiteList.IsClassWhiteListed(clazzName))
                    {
                        return;
                    }
                    MethodInfo toMethod = classRepository.GetClass(clazzName).GetMethod(
                        methodName);
                    if (alreadyVisited.Contains(toMethod))
                    {
                        // Method already counted, skip (to prevent recursion)
                        incrementLoD(lineNumber, toMethod, methodThis, returnVariable, parentFrame);
                    }
                    else if (toMethod.IsOverridable()
                        && variableState.IsInjectable(methodThis))
                    {
                        // Method can be overridden / injectable
                        recordOverridableMethodCall(lineNumber, toMethod, methodThis,
                            returnVariable);
                    }
                    else
                    {
                        // Method can not be intercepted we have to add the cost
                        // recursively
                        recordNonOveridableMethodCall(Reason.NON_OVERRIDABLE_METHOD_CALL, lineNumber, toMethod,
                          methodThis, parameters, returnVariable);
                    }
                }
                catch (ClassNotFoundException e)
                {
                    // err.println("WARNING: class not found: " + clazzName);
                }
                catch (MethodNotFoundException e)
                {
                    // err.println("WARNING: method not found: " + e.getMethodName() + " in "
                    //    + e.getClassInfo().getName());
                }
            }

            private void recordOverridableMethodCall(int lineNumber, MethodInfo toMethod, Variable methodThis, Variable returnVariable)
            {
                if (returnVariable != null)
                {
                    variableState.SetInjectable(returnVariable);
                    SetReturnValue(returnVariable);
                }
                incrementLoD(lineNumber, toMethod, methodThis, returnVariable, this);
            }

            public void assignReturnValue(int lineNumber, Variable destination)
            {
                assignVariable(destination, lineNumber, this, returnValue);
            }

            protected void applyMethodOperations<T>(int lineNumber, MethodInfo toMethod, Variable methodThis, IList<T> parameters, Variable returnVariable) where T : Variable
            {
                if (parameters.Count != toMethod.Parameters.Count)
                {
                    throw new InvalidOperationException("Argument count does not match method parameter count.");
                }
                int i = 0;
                foreach (T var in parameters)
                {
                    assignParameter(lineNumber, toMethod.Parameters[i++], parentFrame, var);
                }
                returnValue = null;
                foreach (Operation operation in toMethod.Operations)
                {
                    operation.Visit(this);
                }
                incrementLoD(lineNumber, toMethod, methodThis, returnVariable, parentFrame);
            }

            protected virtual void incrementLoD(int lineNumber, MethodInfo toMethod,
                Variable destination, Variable source, ParentFrame destinationFrame)
            {
            }

            public void assignParameter(int lineNumber, Variable destination,
                ParentFrame sourceFrame, Variable source)
            {
                assignVariable(destination, lineNumber, sourceFrame, source);
            }

            public virtual void assignVariable(Variable destination, int lineNumber,
                ParentFrame sourceFrame, Variable source)
            {
                if (sourceFrame.variableState.IsInjectable(source))
                {
                    variableState.SetInjectable(destination);
                }
                if (!destination.IsGlobal && !sourceFrame.variableState.IsGlobal(source)) return;
                variableState.SetGlobal(destination);
                if (source is LocalField && !source.IsFinal)
                {
                    addGlobalCost(lineNumber, source);
                }
            }

            private Cost getTotalCost()
            {
                Cost totalCost = new Cost();
                totalCost.Add(direct);
                totalCost.Add(indirect);
                return totalCost;
            }
        }

        public class ParentFrame
        {
            protected readonly VariableState globalVariableState;
            public readonly LocalVariableState variableState;

            public ParentFrame() { }

            public ParentFrame(VariableState globalVariableState)
            {
                this.globalVariableState = globalVariableState;
                this.variableState = new LocalVariableState(globalVariableState);
            }

            public virtual void addLoDCost(int lineNumber, MethodInfo toMethod, int distance)
            {
            }

            public VariableState GetGlobalVariables()
            {
                return globalVariableState;
            }
        }

        public class CostRecordingFrame : Frame
        {
            private readonly MethodCost methodCost;
            private readonly IDictionary<MethodInfo, MethodCost> methodCosts;
            private readonly int remainingDepth;

            public CostRecordingFrame(Stream err, IClassRepository classRepository,
                ParentFrame parentFrame, WhiteList whitelist,
                VariableState globalVariables, IDictionary<MethodInfo, MethodCost> methodCosts,
                ICollection<MethodInfo> alreadyVisited, MethodInfo method, int remainingDepth) :
                base(err, classRepository, parentFrame, whitelist, globalVariables,
                  alreadyVisited, method)
            {
                this.methodCosts = methodCosts;
                this.remainingDepth = remainingDepth;
                this.methodCost = getMethodCostCache(method);
            }

            public CostRecordingFrame(Stream err, IClassRepository classRepository, WhiteList whitelist, VariableState globalVariables, MethodInfo method, int remainingDepth)
                : this(err, classRepository, new ParentFrame(globalVariables), whitelist,
                    globalVariables, new HashDictionary<MethodInfo, MethodCost>(),
                    new HashSet<MethodInfo>(), method, remainingDepth)
            {

            }

            protected override void AddCyclomaticCost(int lineNumber)
            {
                base.AddCyclomaticCost(lineNumber);
                ViolationCost cost = new CyclomaticCost(lineNumber, Cost.Cyclomatic(1));
                methodCost.addCostSource(cost);
            }

            protected new void addGlobalCost(int lineNumber, Variable variable)
            {
                base.addGlobalCost(lineNumber, variable);
                ViolationCost cost = new GlobalCost(lineNumber, variable, Cost.Global(1));
                methodCost.addCostSource(cost);
            }

            protected new void addLoDCost(int lineNumber, MethodInfo method, int distance)
            {
                base.addLoDCost(lineNumber, method, distance);
                ViolationCost cost = new LoDViolation(lineNumber, method.GetFullName(),
                    Cost.LoD(distance), distance);
                methodCost.addCostSource(cost);
            }

            protected new void addMethodInvocationCost(int lineNumber, MethodInfo to,
                Cost methodInvocationCost, Reason reason)
            {
                base.addMethodInvocationCost(lineNumber, to, methodInvocationCost, reason);
                if (!methodInvocationCost.isEmpty())
                {
                    ViolationCost cost = new MethodInvokationCost(lineNumber,
                        getMethodCostCache(to), reason,
                        methodInvocationCost);
                    methodCost.addCostSource(cost);
                }
            }

            public MethodCost getMethodCost()
            {
                return methodCost;
            }

            protected MethodCost getMethodCostCache(MethodInfo method)
            {
                MethodCost methodCost;
                if (!methodCosts.Find(method, out methodCost))
                {
                    methodCost = new MethodCost(method.GetFullName(), method.StartingLineNumber);
                    methodCosts[method] = methodCost;
                }
                return methodCost;
            }

            /**
             * Implicit costs are added to the {@code from} method's costs when it is
             * assumed that the costs must be incurred in order for the {@code from}
             * method to execute. Example:
             *
             * <pre>
             * void fromMethod() {
             *   this.someObject.toMethod();
             * }
             * </pre>
             * <p>
             * We would add the implicit cost of the toMethod() to the fromMethod().
             * Implicit Costs consist of:
             * <ul>
             * <li>Cost of construction for the someObject field referenced in
             * fromMethod()</li>
             * <li>Static initialization blocks in someObject
             * </ul>
             * <li>The cost of calling all the methods starting with "set" on
             * someObject.</ul>
             * <li>Note that the same implicit costs apply for the class that has the
             * fromMethod. (Meaning a method will always have the implicit costs of the
             * containing class and super-classes at a minimum).</li> </ul>
             *
             * @param implicitMethod
             *          the method that is getting called by {@code from} and
             *          contributes cost transitively.
             * @param reason
             *          the type of implicit cost to record, for giving the user
             *          information about why they have the costs they have.
             * @return
             */
            public void applyImplicitCost(MethodInfo implicitMethod, Reason reason)
            {
                if (implicitMethod.MethodThis != null)
                {
                    variableState.SetInjectable(implicitMethod.MethodThis);
                }
                SetInjectable(implicitMethod.Parameters);
                Constant ret = new Constant("return", ClrType.Object);
                int lineNumber = implicitMethod.StartingLineNumber;
                recordNonOveridableMethodCall(reason, lineNumber, implicitMethod, implicitMethod.MethodThis, implicitMethod.Parameters, ret);
            }

            public MethodCost applyMethodOperations()
            {
                foreach (int lineNumberWithComplexity in method.LinesOfComplexity)
                {
                    addCyclomaticCost(lineNumberWithComplexity);
                }
                if (method.MethodThis != null)
                {
                    variableState.SetInjectable(method.MethodThis);
                }

                SetInjectable(method.Parameters);
                Constant returnVariable = new Constant("rootReturn", ClrType.Object);
                applyMethodOperations(-1, method, method.MethodThis, method
                    .Parameters, returnVariable);
                return methodCost;
            }

            private void addCyclomaticCost(int lineNumberWithComplexity)
            {
                direct.AddCyclomaticCost(1);
            }

            protected override Frame createChildFrame(MethodInfo method)
            {
                if (remainingDepth == 0)
                {
                    return base.createChildFrame(method);
                }
                else
                {
                    return new CostRecordingFrame(err, classRepository, this, whiteList,
                        globalVariableState, methodCosts, alreadyVisited, method,
                        remainingDepth - 1);
                }
            }

            public override void assignVariable(Variable destination, int lineNumber,
                ParentFrame sourceFrame, Variable source)
            {
                base.assignVariable(destination, lineNumber, sourceFrame, source);
                int loDCount = sourceFrame.variableState.GetLoDCount(source);
                variableState.SetLoDCount(destination, loDCount);
            }

            protected override void incrementLoD(int lineNumber, MethodInfo toMethod,
                Variable destination, Variable source, ParentFrame destinationFrame)
            {
                if (source != null)
                {
                    int thisCount = variableState.GetLoDCount(destination);
                    int distance = thisCount + 1;
                    destinationFrame.variableState.SetLoDCount(source, distance);
                    if (distance > 1)
                    {
                        destinationFrame.addLoDCost(lineNumber, toMethod, distance);
                    }
                }
            }

            public ParentFrame getParentFrame()
            {
                return parentFrame;
            }
        }
    }

    // by sunlw
    // in java, enum is a ReferenceType which can self describe itself
    [Flags]
    public enum Reason
    {
        //public static ReasonItem IMPLICIT_CONSTRUCTOR = new ReasonItem("implicit cost from construction", true);
        ////
        //public static ReasonItem IMPLICIT_SETTER = new ReasonItem("implicit cost calling all setters", true);
        ////
        //public static ReasonItem IMPLICIT_STATIC_INIT = new ReasonItem("implicit cost from static initialization", true);
        ////
        //public static ReasonItem NON_OVERRIDABLE_METHOD_CALL = new ReasonItem("cost from calling non-overridable method", false);
        //// TODO(jwolter): be able to tell people why this method could not be overridden:
        //// whether it is static, private or final.
        //// SOMEDAY(jwolter): it would be nice to make static methods worse than others. Because we don't
        //// want to encourage people to subclass for tests.

        IMPLICIT,
        EXPLICIT,
        CONSTRUCTOR,
        SETTER,
        STATIC_INIT,

        IMPLICIT_CONSTRUCTOR = IMPLICIT & CONSTRUCTOR,

        IMPLICIT_SETTER = IMPLICIT & SETTER,

        IMPLICIT_STATIC_INIT = IMPLICIT & STATIC_INIT,

        NON_OVERRIDABLE_METHOD_CALL,

        ALL = IMPLICIT & EXPLICIT & CONSTRUCTOR & SETTER & STATIC_INIT & NON_OVERRIDABLE_METHOD_CALL
    }

    public class ReasonItemFactory
    {
        public static readonly IDictionary<Reason, ReasonItem> reasons;

        static ReasonItemFactory()
        {
            reasons = new HashDictionary<Reason, ReasonItem>
                             {
                                 { Reason.IMPLICIT_CONSTRUCTOR, new ReasonItem("implicit cost from construction", true) }, 
                                 { Reason.IMPLICIT_SETTER, new ReasonItem("implicit cost calling all setters", true) },
                                 { Reason.IMPLICIT_STATIC_INIT,new ReasonItem("implicit cost from static initialization", true) },
                                 { Reason.NON_OVERRIDABLE_METHOD_CALL,new ReasonItem("cost from calling non-overridable method", false) }
                             };
        }

        public static ReasonItem GetReasonItem(Reason reason)
        {
            ReasonItem item;

            if (reasons.Find(reason, out item))
            {
                return item;
            }
            return new ReasonItem(reason.ToString(), false);
        }
    }

    public class ReasonItem
    {
        private readonly string description;
        private readonly bool isImplicit;

        public ReasonItem(string description, bool isImplicit)
        {
            this.description = description;
            this.isImplicit = isImplicit;
        }

        public override string ToString()
        {
            return description;
        }

        public bool IsImplicit()
        {
            return isImplicit;
        }
    }
}