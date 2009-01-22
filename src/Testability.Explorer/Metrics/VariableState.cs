using System.Collections.Generic;
using System.Text;

namespace Thinklouder.Testability.Metrics
{
    public class VariableState
    {
        private readonly ICollection<Variable> injectables = new HashSet<Variable>();
        private readonly ICollection<Variable> globals = new HashSet<Variable>();

        public virtual bool IsGlobal(Variable variable)
        {
            if ( variable == null ) return false;
            if ( variable.IsGlobal ) return true;
            if ( globals.Contains(variable) ) return true;
            
            var field = variable as LocalField;
            if ( field != null )
            {
                return globals.Contains(field.GetInstance()) || globals.Contains(field.GetField());
            }
            return false;
        }

        public virtual bool IsInjectable(Variable variable)
        {
            if ( variable == null ) return false;
            if ( injectables.Contains(variable) )
            {
                return true;
            }
            
            var field = variable as LocalField;
            if ( field != null )
            {
                return injectables.Contains(field.GetField());
            }
            return false;
        }

        public virtual void SetGlobal(Variable variable)
        {
            globals.Add(variable);
        }

        public virtual void SetInjectable(Variable variable)
        {
            injectables.Add(variable);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.Append("Injectables:");
            foreach(var variable in injectables)
            {
                builder.AppendLine();
                builder.Append("   ");
                builder.Append(variable);
            }
            builder.AppendLine();
            builder.Append("Globals:");
            foreach (var variable in globals)
            {
                builder.AppendLine();
                builder.Append("   ");
                builder.Append(variable);
            }
            return builder.ToString();
        }
    }
}