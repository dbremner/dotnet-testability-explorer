namespace Thinklouder.Testability.Metrics
{
    public class Variable
    {
        private int hashCode;
        private string name;

        public Variable(string name, Type type, bool isFinal, bool isGlobal)
        {
            Name = name;
            Type = type;
            IsFinal = isFinal;
            IsGlobal = isGlobal;
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                hashCode = ComputeHashCode();
            }
        }

        public bool IsGlobal { get; private set; }

        public bool IsFinal { get; private set; }

        public Type Type { get; private set; }

        public int ComputeHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + ((string.IsNullOrEmpty(name)) ? 0 : name.GetHashCode());
            result = prime * result + ((Type == null) ? 0 : Type.GetHashCode());
            return result;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public string GetName()
        {
            return name;
        }

        public override string ToString()
        {
            return string.Format("{0}{{{1}}}", name, Type);
            //return name + "{" + type + "}";
        }
    }
}