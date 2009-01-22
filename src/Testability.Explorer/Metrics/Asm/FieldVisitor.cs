using Mono.Cecil;

namespace Thinklouder.Testability.Metrics.Asm
{
    public class FieldVisitor : NoopVisitor
    {
        //public FieldVisitor(ClassInfo classInfo, string name, string descriptor, string signature, object value, 
        //    bool isFinal, bool isStatic, bool isPrivate)
        //{
        //    Type type = ClrType.FromDescriptor(descriptor);

        //    classInfo.AddField(new FieldInfo(classInfo, name, type, isFinal, isStatic, isPrivate));
        //}

        public FieldVisitor(ClassInfo classInfo, FieldDefinition fieldDefinition)
        {
            //classInfo.AddField();
            //Type type = ClrType.FromName(fieldDefinition.FieldType.FullName);
            Type type = ClrType.FromDescriptor(fieldDefinition.FieldType.FullName);
            classInfo.AddField(new FieldInfo(classInfo, fieldDefinition.Name, type, false, fieldDefinition.IsStatic, fieldDefinition.IsPrivate));
        }
    }
}