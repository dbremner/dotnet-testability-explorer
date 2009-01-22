using C5;

namespace Thinklouder.Testability.Metrics.Method.Op.Turing
{
    public class MethodInvokation : Operation
    {

        private readonly string name;
        private readonly string clazzName;
        private readonly string signature;
        private readonly Variable methodThis;
        private readonly IList<Variable> parameters;
        private readonly Variable returnVariable;

        public MethodInvokation(int lineNumber, string clazz, string name,
            string signature, Variable methodThis, IList<Variable> parameters,
            Variable returnVariable)
            : base(lineNumber)
        {

            this.clazzName = clazz;
            this.name = name;
            this.signature = signature;
            this.methodThis = methodThis;
            this.parameters = parameters;
            this.returnVariable = returnVariable;
        }

        public IList<Variable> getParameters()
        {
            return parameters;
        }

        public string getMethodName()
        {
            return (clazzName == null ? "null" : clazzName.ToString()) + "." + name;
        }

        public string getName()
        {
            return name;
        }

        public string getOwner()
        {
            return clazzName;
        }

        public override string ToString()
        {
            return getMethodName() + signature;
        }

        public override void Visit(TestabilityVisitor.Frame visitor)
        {
            visitor.recordMethodCall(clazzName, LineNumber, name + signature,
                methodThis, parameters, returnVariable);
        }

        public Variable getMethodThis()
        {
            return methodThis;
        }
    }
}
