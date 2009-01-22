using System;
using C5;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Thinklouder.Testability.Metrics.Asm
{
    public class ModuleInfoVisitor : NoopVisitor
    {
        private readonly string className;
        public IClassRepository Repository { get; private set; }
        private static readonly IList<ClassInfo> classInfos = new ArrayList<ClassInfo>();
        private ClassInfo classInfo;

        public ModuleInfoVisitor(IClassRepository repository, string className)
        {
            this.className = className;
            Repository = repository;
        }

        public override void VisitTypeDefinition(Mono.Cecil.TypeDefinition type)
        {
            var visitor = new ClassInfoVisitor(Repository);
            type.Accept(visitor);
            classInfo = visitor.GetClassInfo();
        }

        public override void VisitTypeDefinitionCollection(Mono.Cecil.TypeDefinitionCollection types)
        {

//            foreach ( Mono.Cecil.TypeDefinition type in types )
//            {
////#if DEBUG
////                Debug.Print(type.FullName);
////#endif
//                //if (!type.Name.Equals("<Module>") && type.Name.StartsWith("ClassInfoTest"))
//                //if (!type.Name.Equals("<Module>") && !type.Name.StartsWith("ClassInfoTest") && !type.Name.StartsWith("BlockTest") && !type.Name.StartsWith("CostTest") && !type.Name.StartsWith("MetricComputerClrDecorator") && !type.Name.StartsWith("MethodInfoTest") && !type.Name.StartsWith("MethodCostTest"))
//                //if (!type.Name.Equals("<Module>") && (type.Name.StartsWith("StaticFieldSample") || (type.Name.StartsWith("LoadClass")|| (type.Name.StartsWith("PopClass")))))
//                if(!type.Name.Equals("<Module>") && type.Name.Equals(this.className))
//                    VisitTypeDefinition(type);
//            }
            if(types.Contains(className))
            {
                VisitTypeDefinition(types[className]);
            }

        }

        // deprecated... by sunlw
        public IList<ClassInfo> GetClassInfos()
        {
            return classInfos;
        }

        public ClassInfo GetClassInfo()
        {
            return classInfo;
        }
    }
}
