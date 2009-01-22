using C5;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class Invoke : StackOperation
    {

        private readonly string clazz;
        private readonly string name;
        private readonly string signature;
        private readonly bool isStatic;
        private readonly Type returnType;
        private readonly IList<Type> parameters;
        private readonly Variable returnValue;

        public Invoke(int lineNumber, string clazz, string name, string signature,
                      IList<Type> parameters, bool isStatic, Type returnType)
            : base(lineNumber)
        {

            this.clazz = clazz;
            this.name = name;
            this.signature = signature;
            this.parameters = parameters;
            this.isStatic = isStatic;
            this.returnType = returnType;
            this.returnValue = new Constant("?", returnType);
        }

        public override int getOperatorCount()
        {
            int count = isStatic ? 0 : 1;
            foreach (Type type in parameters)
            {
                count += ClrType.IsDoubleSlot(type) ? 2 : 1;
            }
            return count;
        }

        public override IList<Variable> apply(IList<Variable> input)
        {
            if (returnType == ClrType.Void)
            {
                return new ArrayList<Variable>();
            }
            else
            {
                return list(returnValue);
            }
        }

        public override Operation ToOperation(IList<Variable> input)
        {
            IList<Variable> parameters = removeDuplicateSlots(input);
            Variable methodThis = isStatic ? null : parameters.RemoveAt(0);
            return new MethodInvokation(lineNumber, clazz, name, signature,
                                        methodThis, parameters, returnValue);
        }

        private IList<Variable> removeDuplicateSlots(IList<Variable> input)
        {
            IList<Variable> parameters = new ArrayList<Variable>();
            bool skip = false;
            foreach (Variable variable in input)
            {
                if (skip)
                {
                    skip = false;
                    continue;
                }
                if (ClrType.IsDoubleSlot(variable.Type))
                {
                    skip = true;
                }
                parameters.Add(variable);
            }
            return parameters;
        }

        public override string ToString()
        {
            var operat = isStatic ? "call " : "callvirt ";
            var className = clazz == null ? "null" : clazz.ToString();
            var ret = returnType == null ? "" : " : " + returnType;
            return operat + className + "::" + name + signature + ret;
        }

    }
}