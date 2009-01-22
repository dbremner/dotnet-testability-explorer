using C5;
using System.Text;

namespace Thinklouder.Testability.Metrics
{
    public class LocalVariableState : VariableState
    {
        private readonly IDictionary<Variable, int> lodCount = new HashDictionary<Variable, int>();
        private readonly VariableState globalVariables;

        public LocalVariableState(VariableState globals)
        {
            this.globalVariables = globals;
        }

        public VariableState GetGlobalVariableState()
        {
            return globalVariables;
        }

        public override void SetGlobal(Variable variable)
        {
            if ( variable is LocalField || variable is FieldInfo )
            {
                globalVariables.SetGlobal(variable);
            }
            else
            {
                base.SetGlobal(variable);
            }
        }

        public override void SetInjectable(Variable variable)
        {
            if ( variable is LocalField || variable is FieldInfo )
            {
                globalVariables.SetInjectable(variable);
            }
            else
            {
                base.SetInjectable(variable);
            }
        }

        public override bool IsGlobal(Variable variable)
        {
            return base.IsGlobal(variable) || globalVariables.IsGlobal(variable);
        }

        public override bool IsInjectable(Variable variable)
        {
            return base.IsInjectable(variable) || globalVariables.IsInjectable(variable);
        }

        public int GetLoDCount(Variable variable)
        {
            int count;
            if ( lodCount.Find(variable, out count) ) return count;
            
            var localField = variable as LocalField;
            if ( localField != null ) return GetLoDCount(localField.GetField());
                
            return 0;
        }

        public void SetLoDCount(Variable variable, int newCount)
        {
            int count;
            if(!lodCount.Find(variable, out count))
            {
                count = 0;
            }
            else
            {
                if(count < newCount)
                {
                    count = newCount;
                }
            }
            lodCount[variable] = count;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("GLOBALS:\n");
            sb.Append(globalVariables.ToString());
            sb.Append(base.ToString());
            sb.Append("\nLoD:");

            foreach (var variable in lodCount.Keys)
            {
                sb.Append("\n   ");
                sb.Append(variable);
                sb.Append(": ");
                sb.Append(lodCount[variable]);
            }
            return sb.ToString();
        }
    }
}