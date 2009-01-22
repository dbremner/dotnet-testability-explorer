#region

using System;
using System.Text;
using C5;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

#endregion

namespace Thinklouder.Testability.Metrics
{
    public class MethodInfo : IComparable<MethodInfo>
    {
        public MethodInfo(ClassInfo classInfo, string methodName,
                          int startingLineNumber, string description, Variable methodThis,
                          IList<ParameterInfo> parameters, IList<LocalVariableInfo> localVariables,
                          Visibility visibility, IList<int> linesOfComplexity,
                          IList<Operation> operations, bool isVirtual)
        {
            IsVirtual = isVirtual;
            ClassInfo = classInfo;
            Visibility = visibility;
            Name = methodName;
            StartingLineNumber = startingLineNumber;
            Descriptor = description;
            MethodThis = methodThis;
            Parameters = parameters;
            LocalVariables = localVariables;
            LinesOfComplexity = linesOfComplexity;
            Operations = operations;
        }

        public IList<ParameterInfo> Parameters { get; private set; }

        public IList<LocalVariableInfo> LocalVariables { get; private set; }

        public Visibility Visibility { get; private set; }

        public IList<Operation> Operations { get; private set; }

        public int StartingLineNumber { get; private set; }

        public IList<int> LinesOfComplexity { get; private set; }

        public Variable MethodThis { get; private set; }

        public bool IsVirtual { get; private set; }

        public ClassInfo ClassInfo { get; private set; }

        public string Descriptor { get; private set; }

        public string Name { get; private set; }

        #region IComparable<MethodInfo> Members

        public int CompareTo(MethodInfo other)
        {
            if ( other == null ) return -1;
            return GetFullName().CompareTo(other.GetFullName());
        }

        #endregion

        public bool IsSetter()
        {
            return Visibility != Visibility.PRIVATE && Name.StartsWith("set_");
        }

        public bool IsStatic()
        {
            return MethodThis == null;
        }

        public bool IsConstructor()
        {
            return Visibility != Visibility.PRIVATE && Name.StartsWith("<init>");
        }

        public bool IsOverridable()
        {
            return !IsConstructor() && !IsStatic() && IsVirtual && Visibility != Visibility.PRIVATE;
        }

        public string GetNameDescriptor()
        {
            return Name + Descriptor;
        }

        public string GetFullName()
        {
            //int parametersEnd = Descriptor.IndexOf(')');
            //string returnValue = DeconstructParameters(Descriptor.Substring(parametersEnd + 1)) + " ";
            //string parameters = Descriptor.Substring(1, parametersEnd -1);
            //string methodName = Name;

            int parametersEnd = Descriptor.IndexOf(')');
            string returnValue = Descriptor.Substring(parametersEnd + 1) + " ";
            string parameters = Descriptor.Substring(1, parametersEnd - 1);
            string methodName = Name;
            
            if ( IsStaticConstructor() || IsConstructor() )
            {
                returnValue = "";
                methodName = ClassInfo.Name;
            }
            return returnValue + methodName + "(" + parameters + ")";
        }

        public ICollection<MethodInfo> GetSiblingSetters()
        {
            return ClassInfo.GetSetters();
        }

        public int GetNonPrimitiveArgCount()
        {
            int count = 0;
            foreach (var parameter in Parameters)
            {
                if(parameter.Type.IsObject())
                {
                    count++; 
                }
            }
            return count;
        }

        public bool IsStaticConstructor()
        {
            return Name.Equals("<clinit>");
        }

        // TODO, deprecated method...
        public string DeconstructParameters(string parameters)
        {
            var paramString = new StringBuilder();
            int i = 0;
            string sep = "";
            string arrayRefs = "";
            while (i < parameters.Length)
            {
                switch (parameters[i])
                {
                    case 'B':
                        paramString.Append(sep + "byte" + arrayRefs);
                        break;
                    case 'C':
                        paramString.Append(sep + "char" + arrayRefs);
                        break;
                    case 'D':
                        paramString.Append(sep + "double" + arrayRefs);
                        break;
                    case 'F':
                        paramString.Append(sep + "float" + arrayRefs);
                        break;
                    case 'I':
                        paramString.Append(sep + "int" + arrayRefs);
                        break;
                    case 'J':
                        paramString.Append(sep + "long" + arrayRefs);
                        break;
                    case 'L':
                        // Object becomes L/java/lang/Object; in internal nomenclature
                        // L
                        string internalClassName = parameters.Substring(i + 1, parameters.IndexOf(';', i) - i - 1 );
                        //string className = internalClassName.Replace('/', '.');
                        string className = internalClassName;
                        paramString.Append(sep + className + arrayRefs);
                        i = parameters.IndexOf(';', i);
                        break;
                    case 'S':
                        paramString.Append(sep + "short" + arrayRefs);
                        break;
                    case 'Z':
                        paramString.Append(sep + "bool" + arrayRefs);
                        break;
                    case 'V':
                        paramString.Append("void");
                        break;
                    case '[':
                        arrayRefs += "[]";
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                if ( parameters[i] != '[' )
                {
                    arrayRefs = "";
                    sep = ", ";
                }
                i++;
            }
            return paramString.ToString();
        }

        public override string ToString()
        {
            return GetFullName();
        }

        public bool isInstance()
        {
            return MethodThis != null;
        }
    }
}