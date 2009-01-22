using System;
using Mono.Cecil;

namespace Thinklouder.Testability.Metrics
{
    public class Type
    {
        public int Array { get; private set; }
        public string Name { get; private set; }
        public bool isPrimitive;

        public Type() : this(0, null, false)
        {
        }

        public Type(string name) : this(0, name, false)
        {
        }

        public Type(int array, string name, bool isPrimitive)
        {
            Array = array;
            Name = name;
            this.isPrimitive = isPrimitive;
        }

        public override string ToString()
        {
            return Name;
        }

        public Type ToArray()
        {
            return new Type(Array + 1, Name + "[]", isPrimitive);
        }

        public bool IsPrimitive()
        {
            return isPrimitive;
        }

        public bool IsObject()
        {
            return !IsPrimitive();
        }

        public bool IsArray()
        {
            return Array > 0;
        }

        public override int GetHashCode()
        {
            var prime = 31;
            var result = 1;
            result = prime * result + Array;
            result = prime*result + ((Name == null) ? 0 : Name.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if ( this == obj ) return true;
            if ( obj == null ) return false;
            var other = (Type)obj;
            return Name.Equals(other.Name);
        }
    }

}