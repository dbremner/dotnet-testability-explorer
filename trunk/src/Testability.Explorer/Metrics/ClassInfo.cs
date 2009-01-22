using System.Linq;
using SCG = System.Collections.Generic;
using C5;

namespace Thinklouder.Testability.Metrics
{
    public class ClassInfo
    {
        private readonly IDictionary<string, MethodInfo> methods = new TreeDictionary<string, MethodInfo>();
        private readonly IDictionary<string, FieldInfo> fields = new TreeDictionary<string, FieldInfo>();
        
        private readonly string name;
        private readonly bool isInterface;
        private readonly ClassInfo superClass;
        private readonly IList<ClassInfo> interfaces;
        private readonly object typeReference;

        public ClassInfo(string name, bool isInterface, ClassInfo superClass, IList<ClassInfo> interfaces)
        {
            this.name = name;
            this.interfaces = interfaces;
            this.superClass = superClass;
            this.isInterface = isInterface;
        }

        public ClassInfo(string name, bool isInterface, ClassInfo superClass, IList<ClassInfo> interfaces, object typeReference)
        {
            this.name = name;
            this.interfaces = interfaces;
            this.superClass = superClass;
            this.isInterface = isInterface;
            this.typeReference = typeReference;
        }
        
        public IList<ClassInfo> Interfaces
        {
            get { return interfaces; }
        }

        public ClassInfo SuperClass
        {
            get { return superClass; }
        }

        public bool IsInterface
        {
            get { return isInterface; }
        }

        public string Name
        {
            get { return name; }
        }

        public MethodInfo GetMethod(string methodName)
        {
            IList<ClassInfo> superClasses = new ArrayList<ClassInfo> {this};
            while(superClasses.Count>0)
            {
                var clazz = superClasses[0];
                superClasses.Remove(clazz);
                MethodInfo methodInfo;
                if(clazz.methods.Find(methodName, out methodInfo))
                {
                    return methodInfo;
                }
                if ( clazz.superClass != null )
                {
                    superClasses.Add(clazz.superClass);
                }
                superClasses.AddAll(clazz.interfaces);
            }
            throw new MethodNotFoundException(this, methodName);
        }

        public void AddMethod(MethodInfo methodInfo)
        {
            methods.Add(methodInfo.GetNameDescriptor(), methodInfo);    
        }

        public override string ToString()
        {
            return name;
        }

        public FieldInfo GetField(string fieldName)
        {
            ClassInfo clazz = this;
            while(clazz!=null)
            {
                FieldInfo fieldInfo;
                if(clazz.fields.Find(fieldName, out fieldInfo))
                {
                    return fieldInfo;
                }
                clazz = clazz.superClass;
            }
            throw new FieldNotFoundException(this, fieldName);
        }

        public void AddField(FieldInfo fieldInfo)
        {
            fields.Add(fieldInfo.Name, fieldInfo);
        }

        public ICollectionValue<MethodInfo> GetMethods()
        {
            return methods.Values;
        }

        public ICollectionValue<FieldInfo> GetFields()
        {
            return fields.Values;
        }

        public ICollection<ClassInfo> GetInterfaces()
        {
            return interfaces;
        }

        public ICollection<MethodInfo> GetSetters()
        {
            ICollection<MethodInfo> setters = new TreeSet<MethodInfo>();
            if(superClass!=null)
            {
                setters.AddAll(superClass.GetSetters());
            }
            foreach (var method in methods.Values.ToList())
            {
                if(method.IsSetter())
                {
                    setters.Add(method);
                }
            }
            return setters;
        }

        /** When you have multiple constructors you need to know which one to use for marking
         * fields as injectables. The heuristic is that the constructor with most arguments
         * will probably be the constructor best suited for testing as it will give you highest
         * control over your field injection.
         */
        public MethodInfo GetConstructorWithMostNonPrimitiveParameters() {
            // TODO(jwolter): It would seem more accurate a approximation of multiple constructors
            // if we would calculate the cost for all of them, and then add in only the highest,
            // or an average of them.
            MethodInfo constructor = null;
            int currentArgsCount = -1;
            foreach (MethodInfo methodInfo in GetConstructors()) {
              int count = methodInfo.GetNonPrimitiveArgCount();
              if (currentArgsCount < count) {
                constructor = methodInfo;
                currentArgsCount = count;
              }
            }
            return constructor;
        }

        public ICollection<MethodInfo> GetConstructors()
        {
            TreeSet<MethodInfo> constructors = new TreeSet<MethodInfo>();
            foreach (MethodInfo methodInfo in GetMethods()) {
              if (methodInfo.IsConstructor()) {
                constructors.Add(methodInfo);
              }
            }
            return constructors;
          }

        /// <summary>
        /// I might use this format to identify name in the whole system...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetFullName<T>()
        {
            return typeof(T).FullName.Replace("+", "/");
        }

        public object GetTypeReference()
        {
            return typeReference;
        }
    }   
}
