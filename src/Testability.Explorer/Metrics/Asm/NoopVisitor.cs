using Mono.Cecil;

namespace Thinklouder.Testability.Metrics.Asm
{
    // TODO, we might add some NoopImplement of IReflectionVisitor like:
    // NoopTypeVisitor, NoopMethodVisitor, NoopFieldVisitor, 
    public class NoopVisitor : IReflectionVisitor
    {
        public virtual void VisitModuleDefinition(ModuleDefinition module)
        {

        }

        public virtual void VisitTypeDefinitionCollection(TypeDefinitionCollection types)
        {

        }

        public virtual void VisitTypeDefinition(TypeDefinition type)
        {

        }

        public virtual void VisitTypeReferenceCollection(TypeReferenceCollection refs)
        {

        }

        public virtual void VisitTypeReference(TypeReference type)
        {

        }

        public virtual void VisitMemberReferenceCollection(MemberReferenceCollection members)
        {

        }

        public virtual void VisitMemberReference(MemberReference member)
        {

        }

        public virtual void VisitInterfaceCollection(InterfaceCollection interfaces)
        {

        }

        public virtual void VisitInterface(TypeReference interf)
        {

        }

        public virtual void VisitExternTypeCollection(ExternTypeCollection externs)
        {

        }

        public virtual void VisitExternType(TypeReference externType)
        {

        }

        public virtual void VisitOverrideCollection(OverrideCollection meth)
        {

        }

        public virtual void VisitOverride(MethodReference ov)
        {

        }

        public virtual void VisitNestedTypeCollection(NestedTypeCollection nestedTypes)
        {

        }

        public virtual void VisitNestedType(TypeDefinition nestedType)
        {

        }

        public virtual void VisitParameterDefinitionCollection(ParameterDefinitionCollection parameters)
        {

        }

        public virtual void VisitParameterDefinition(ParameterDefinition parameter)
        {

        }

        public virtual void VisitMethodDefinitionCollection(MethodDefinitionCollection methods)
        {

        }

        public virtual void VisitMethodDefinition(MethodDefinition method)
        {

        }

        public virtual void VisitConstructorCollection(ConstructorCollection ctors)
        {

        }

        public virtual void VisitConstructor(MethodDefinition ctor)
        {

        }

        public virtual void VisitPInvokeInfo(PInvokeInfo pinvk)
        {

        }

        public virtual void VisitEventDefinitionCollection(EventDefinitionCollection events)
        {

        }

        public virtual void VisitEventDefinition(EventDefinition evt)
        {

        }

        public virtual void VisitFieldDefinitionCollection(FieldDefinitionCollection fields)
        {

        }

        public virtual void VisitFieldDefinition(FieldDefinition field)
        {

        }

        public virtual void VisitPropertyDefinitionCollection(PropertyDefinitionCollection properties)
        {

        }

        public virtual void VisitPropertyDefinition(PropertyDefinition property)
        {

        }

        public virtual void VisitSecurityDeclarationCollection(SecurityDeclarationCollection secDecls)
        {

        }

        public virtual void VisitSecurityDeclaration(SecurityDeclaration secDecl)
        {

        }

        public virtual void VisitCustomAttributeCollection(CustomAttributeCollection customAttrs)
        {

        }

        public virtual void VisitCustomAttribute(CustomAttribute customAttr)
        {

        }

        public virtual void VisitGenericParameterCollection(GenericParameterCollection genparams)
        {

        }

        public virtual void VisitGenericParameter(GenericParameter genparam)
        {

        }

        public virtual void VisitMarshalSpec(MarshalSpec marshalSpec)
        {

        }

        public virtual void TerminateModuleDefinition(ModuleDefinition module)
        {

        }
    }
}