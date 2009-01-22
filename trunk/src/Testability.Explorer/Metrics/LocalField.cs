namespace Thinklouder.Testability.Metrics
{
    public class LocalField : Variable
    {
        private readonly Variable instance;
        private readonly FieldInfo fieldInfo;

        public LocalField(Variable instance, FieldInfo fieldInfo):base(fieldInfo.Name, fieldInfo.Type, fieldInfo.IsFinal, fieldInfo.IsGlobal)
        {
            this.instance = instance;
            this.fieldInfo = fieldInfo;

        }

        public override string ToString()
        {
            return fieldInfo.ToString();
        }

        public FieldInfo GetField()
        {
            return fieldInfo;
        }

        public Variable GetInstance()
        {
            return instance;
        }
    }
}