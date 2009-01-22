using System;
using C5;
using Mono.Cecil;

namespace Thinklouder.Testability.Metrics.Asm
{
    public class ClassInfoVisitor : NoopVisitor
    {
        public IClassRepository Repository { get; private set; }
        private ClassInfo classInfo;

        public ClassInfoVisitor(IClassRepository repository)
        {
            Repository = repository;
        }

        public override void VisitTypeDefinition(TypeDefinition type)
        {
            ClassInfo superClass = null;

            // TODO, we need to support reference to other module... by sunlw
            if (type.BaseType.Scope.Name.Equals(Repository.ScopeName))
            {
                // TODO, base class might be handled here...
                superClass = type.BaseType == null ? null : Repository.GetClass(type.BaseType.FullName);
            }

            IList<ClassInfo> interfaces = new ArrayList<ClassInfo>();
            foreach (TypeDefinition interfaze in type.Interfaces)
            {
                interfaces.Add(Repository.GetClass(interfaze.FullName));
            }
            classInfo = new ClassInfo(type.FullName, type.IsInterface, superClass, interfaces, type as TypeReference);  // TODO, we might remove the dependency to the TypeReference...by sunlw
            Repository.AddClass(classInfo);
        }

        public override void VisitMethodDefinition(MethodDefinition method)
        {
            // TODO, pass 

            /*
            boolean isStatic = ( access & Opcodes.ACC_STATIC ) == Opcodes.ACC_STATIC;
            boolean isFinal = ( access & Opcodes.ACC_FINAL ) == Opcodes.ACC_FINAL;
            return new MethodVisitorBuilder(repository, classInfo, name, desc, signature,
                exceptions, isStatic, isFinal, JavaVisibility.valueFromJavaBytecode(access));
            */

            //if (!method.HasBody || method.IsCompilerControlled) return;
            if (!method.HasBody) return;

            string name = method.Name;
            //var parameters = method.
            MethodReference md = method;
            ParameterDefinitionCollection paras = md.Parameters;
            TypeReference returnType = md.ReturnType.ReturnType;

            string descriptor = "(";
            descriptor += "";

            foreach (ParameterDefinition para in paras)
            {
                var type = ((Mono.Cecil.ParameterReference)(para)).ParameterType;

                descriptor += type.ToString();
                descriptor += ",";
            }
            if (paras.Count > 0) descriptor = descriptor.Substring(0, descriptor.Length - 1);

            descriptor += ")";
            descriptor += returnType.ToString();

            //#if DEBUG
            //            Console.WriteLine(descriptor);
            //#endif

            method.Body.Accept(new MethodVisitor(Repository, classInfo, method.Name, descriptor, null,
                null, method.IsStatic, method.IsFinal, VisibilityUtil.ValueOf(method)));
        }

        public override void VisitMethodDefinitionCollection(MethodDefinitionCollection methods)
        {
            foreach (MethodDefinition methodDefinition in methods)
            {
                VisitMethodDefinition(methodDefinition);
            }
        }

        public override void VisitFieldDefinition(FieldDefinition field)
        {
            //field.Accept(new FieldVisitor(classInfo, field.Name, field.Constant, field.FieldType, field.IsStatic, field.IsPrivate));
            field.Accept(new FieldVisitor(classInfo, field));  // TODO, remove the dependency on FieldDefinition
        }

        public override void VisitFieldDefinitionCollection(FieldDefinitionCollection fields)
        {
            foreach (FieldDefinition fieldDefinition in fields)
            {
                VisitFieldDefinition(fieldDefinition);
            }
        }

        public override void VisitPropertyDefinition(PropertyDefinition property)
        {
            if (property.SetMethod != null) { VisitMethodDefinition(property.SetMethod); }
            if (property.GetMethod != null) { VisitMethodDefinition(property.GetMethod); }
        }

        public override void VisitPropertyDefinitionCollection(PropertyDefinitionCollection properties)
        {
            foreach (PropertyDefinition property in properties)
            {
                VisitPropertyDefinition(property);
            }
        }

        public override void VisitConstructor(MethodDefinition ctor)
        {
            VisitMethodDefinition(ctor);
        }

        public override void VisitConstructorCollection(ConstructorCollection ctors)
        {
            foreach (MethodDefinition ctor in ctors)
            {
                VisitConstructor(ctor);
            }
        }

        public override void VisitGenericParameter(GenericParameter genparam)
        {

        }

        public override void VisitGenericParameterCollection(GenericParameterCollection genparams)
        {

        }

        public ClassInfo GetClassInfo()
        {
            return classInfo;
        }
    }
}
