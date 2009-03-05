using System;

namespace Thinklouder.Testability.Metrics.Method.Op.Stack
{
    public class Label : IEquatable<Label>
    {
        internal int label;

        public Label()
        {
            label = -1;
        }

        internal Label(int label)
        {
            this.label = label;
        }

        #region IEquatable<Label> Members

        public bool Equals(Label obj)
        {
            // TODO, added to make test pass... by sunlw
            if (obj.label == label && label != -1) return true;
            if (obj.GetHashCode() == GetHashCode()) return true;
            return false;
        }

        #endregion

        internal int GetLabelValue()
        {
            return label;
        }

        // TODO, is this reasonable to use instruction offset as the hashcode?
        // if so, this will break some testcase.
        public override int GetHashCode()
        {
            if (label == -1) return base.GetHashCode();
            return label;
        }

        public override bool Equals(object obj)
        {
            return ((obj is Label) && Equals((Label) obj));
        }

        //public static bool operator ==(Label a, Label b)
        //{
        //    return a.Equals(b);
        //}

        //public static bool operator !=(Label a, Label b)
        //{
        //    return !( a == b );
        //}

        public override string ToString()
        {
            return "label: " + label;
        }
    }
}