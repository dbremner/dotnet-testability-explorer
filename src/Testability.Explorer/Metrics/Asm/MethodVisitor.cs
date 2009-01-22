#region

using System;
using C5;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Thinklouder.Testability.Metrics.Asm.Instructions;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;
using Convert=System.Convert;

#endregion

namespace Thinklouder.Testability.Metrics.Asm
{
    public class MethodVisitor : ICodeVisitor
    {
        private readonly LocalVariableInfo methodThis;
        private readonly IDictionary<int, Variable> slots = new HashDictionary<int, Variable>();
        private readonly IList<LocalVariableInfo> localVariables = new ArrayList<LocalVariableInfo>();
        private readonly IList<ParameterInfo> parameters = new ArrayList<ParameterInfo>();
        private readonly IList<int> cyclomaticComplexity = new ArrayList<int>();
        private readonly IList<IRunnable> recorder = new ArrayList<IRunnable>();
        private readonly BlockDecomposer block = new BlockDecomposer();

        // TODO, refactoring 
        public MethodVisitor(IClassRepository repository, ClassInfo classInfo,
                                    string name, string descriptor, string signature, string[] exceptions,
                                    bool isStatic, bool isFinal, Visibility visibility)
        {
            Repository = repository;
            ClassInfo = classInfo;
            Name = name;
            Descriptor = descriptor;
            IsFinal = isFinal;
            Visibility = visibility;

            int slot = 0;
            if ( !isStatic )
            {
                Type thisType = ClrType.FromClr(classInfo.Name);
                methodThis = new LocalVariableInfo("this", thisType);
                slots.Add(slot++, methodThis);
                localVariables.Add(methodThis);
            }

            int rightBraceIndex = descriptor.IndexOf(')');
            string parameterString = descriptor.Substring(1, rightBraceIndex - 1);

            if (parameterString.Length > 0)
            {
                string[] parameters1 = parameterString.Split(',');

                foreach (string parameter in parameters1)
                {
                    Type parameterType = new Type(parameter);
                    ParameterInfo parameterInfo = new ParameterInfo("param_" + slot, parameterType);
                    parameters.Add(parameterInfo);
                    slots.Add(slot++, parameterInfo);
                    if (ClrType.IsDoubleSlot(parameterType))
                    {
                        slot++;
                    }
                }
            }
        }

        public IClassRepository Repository { get; private set; }
        public ClassInfo ClassInfo { get; private set; }
        public string Name { get; private set; }
        public string Descriptor { get; private set; }
        public bool IsFinal { get; private set; }
        public Visibility Visibility { get; private set; }

        #region ICodeVisitor Members

        public void VisitMethodBody(MethodBody body)
        {
            VisitVariableDefinitionCollection(body.Variables);
            //VisitInstructionCollection(body.Instructions);
        }

        public void VisitInstructionCollection(InstructionCollection instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                //recorder.Add(new LabelRunnable(block, instruction.Offset));
                VisitInstruction(instruction);
            }
        }

        public void VisitInstruction(Instruction instruction)
        {
            //Console.WriteLine(instruction.OpCode.Name + " " + instruction.Operand);

            #region "CallRunnable"
            if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
            {
                recorder.Add(new CallRunnable(instruction, block));
            }
            #endregion

            #region "XFieldRunnable"
            if (instruction.OpCode == OpCodes.Stsfld)
            {
                recorder.Add(new SetFieldRunnable(Repository, ClassInfo, instruction, block));
            }
            else if (instruction.OpCode == OpCodes.Stfld)
            {
                recorder.Add(new SetFieldRunnable(Repository, ClassInfo, instruction, block));
            }
            else if (instruction.OpCode == OpCodes.Ldsfld)
            {
                recorder.Add(new GetFieldRunnable(Repository, ClassInfo, instruction, block));
            }
            else if (instruction.OpCode == OpCodes.Ldfld)
            {
                recorder.Add(new GetFieldRunnable(Repository, ClassInfo, instruction, block));
            }
            #endregion

            #region "SwitchRunnable"
            if (instruction.OpCode == OpCodes.Switch)
            {
                var instructions = instruction.Operand as Instruction[];
                recorder.Add(new SwitchRunnable(block, cyclomaticComplexity, instruction.Next, instructions));
            }
            #endregion

            #region "Visit Branching Instructions - visitJumpInsn in asm
            if (instruction.OpCode == OpCodes.Br_S)
            {
                
            }
            #endregion

            #region "visitIntInsn"
            if(instruction.OpCode == OpCodes.Newarr)
            {
                recorder.Add(new NewArrayRunnable(block, instruction));
            }
            #endregion

            #region "visitTypeInsn"
            if(instruction.OpCode == OpCodes.Newobj)
            {
                recorder.Add(new NewRunnable(block, instruction));
            }
            #endregion

            #region "visitLdcInsn - Load constant"
            if (instruction.OpCode == OpCodes.Ldnull)
            {
                recorder.Add(new LoadConstantRunnable(block, instruction, null, ClrType.Object));
            }
            //else if(instruction.OpCode.Name.StartsWith("ldc"))
            else if(instruction.OpCode == OpCodes.Ldc_I4_S)
            {
                recorder.Add(new LoadConstantRunnable(block, instruction,  instruction.Operand, ClrType.Int32));
            }
            else if (instruction.OpCode == OpCodes.Ldc_I4_0 ||
                instruction.OpCode == OpCodes.Ldc_I4_1 ||
                instruction.OpCode == OpCodes.Ldc_I4_2 ||
                instruction.OpCode == OpCodes.Ldc_I4_3)
            {
                int constant = instruction.OpCode.Value - OpCodes.Ldc_I4_0.Value;
                recorder.Add(new LoadConstantRunnable(block, instruction, constant, ClrType.Int32));          
            }
            else if (instruction.OpCode == OpCodes.Ldc_I4_4 ||
                instruction.OpCode == OpCodes.Ldc_I4_5 ||
                instruction.OpCode == OpCodes.Ldc_I4_6 ||
                instruction.OpCode == OpCodes.Ldc_I4_7 ||
                instruction.OpCode == OpCodes.Ldc_I4_8)
            {
                int constant = instruction.OpCode.Value - OpCodes.Ldc_I4_4.Value + 4;  // start from 1A
                recorder.Add(new LoadConstantRunnable(block, instruction, constant, ClrType.Int32));  
            }
            else if (instruction.OpCode == OpCodes.Ldc_I4_M1)
            {
                recorder.Add(new LoadConstantRunnable(block, instruction, -1, ClrType.Int32));  
            }
            else if (instruction.OpCode == OpCodes.Ldc_I8)
            {
                recorder.Add(new LoadConstantRunnable(block, instruction, instruction.Operand, ClrType.Int64));
            }
            else if (instruction.OpCode == OpCodes.Ldc_R4)
            {
                recorder.Add(new LoadConstantRunnable(block, instruction, instruction.Operand, ClrType.Single));
            }
            else if (instruction.OpCode == OpCodes.Ldc_R8)
            {
                recorder.Add(new LoadConstantRunnable(block, instruction, instruction.Operand, ClrType.Double));
            }
            #endregion

            #region "visitInsn - Convert"
            if(instruction.OpCode == OpCodes.Conv_I1)
            {
                convertTo(instruction, ClrType.Int32);
            }
            else if (instruction.OpCode == OpCodes.Conv_I2)
            {
                convertTo(instruction, ClrType.Int32);
            }
            else if (instruction.OpCode == OpCodes.Conv_I4)
            {
                convertTo(instruction, ClrType.Int32);
            }
            else if (instruction.OpCode == OpCodes.Conv_I8)
            {
                convertTo(instruction, ClrType.Int64);
            }
            else if (instruction.OpCode == OpCodes.Conv_R4)
            {
                convertTo(instruction, ClrType.Single);
            }
            else if (instruction.OpCode == OpCodes.Conv_R8)
            {
                convertTo(instruction, ClrType.Double);
            }
            else if (instruction.OpCode == OpCodes.Conv_U1)
            {
                convertTo(instruction, ClrType.Int32);
            }
            else if (instruction.OpCode == OpCodes.Conv_U2)
            {
                convertTo(instruction, ClrType.Int32);
            }
            else if (instruction.OpCode == OpCodes.Conv_U4)
            {
                convertTo(instruction, ClrType.Int32);
            }
            else if (instruction.OpCode == OpCodes.Conv_U8)
            {
                convertTo(instruction, ClrType.Int64);
            }
            else if (instruction.OpCode == OpCodes.Conv_I)
            {
                // TODO, how to deal with native int? by sunlw
            }
            else if (instruction.OpCode == OpCodes.Conv_U)
            {
                // TODO, how to deal with native unsigned int? by sunlw
            }
            else if (instruction.OpCode == OpCodes.Conv_R_Un)
            {
                // TODO, how to deal with unsigned int to float point? by sunlw
            }
            #endregion

            #region "visitVarInsn Load local var (TODO)"
            #endregion

            #region "visitVarInsn Ecma-335 3.63"
            if (instruction.OpCode == OpCodes.Stloc ||
                instruction.OpCode == OpCodes.Stloc_S)
            {
                var vr = instruction.Operand as VariableReference;
                store(instruction, vr.Index);
            }
            else if (instruction.OpCode == OpCodes.Stloc_0 ||
                instruction.OpCode == OpCodes.Stloc_1 ||
                instruction.OpCode == OpCodes.Stloc_2 ||
                instruction.OpCode == OpCodes.Stloc_3)
            {
                int varIndex = instruction.OpCode.Value - OpCodes.Stloc_0.Value;
                store(instruction, varIndex);
            }
            #endregion

            #region "visitVarInsn ret"
            if (instruction.OpCode == OpCodes.Ret)
            {
                recorder.Add(new ReturnRunnable(block, instruction));
            }
            #endregion
        }

        private void convertTo(Instruction instruction, Type type)
        {
            recorder.Add(new ConvertRunnable(block, instruction, type));
        }

        private void store(Instruction instruction, int varIndex)
        {
            recorder.Add(new StoreRunnable(block, instruction, variable(varIndex)));
        }

        private Variable variable(int varIndex)
        {
            return slots[varIndex];
        }

        // TODO, do we need to implement like in java... by sunlw
        //private Variable variable(int varIndex, Type type)
        //{
        //    Variable resultVariable;
        //    if (!slots.Find(varIndex, out resultVariable) || resultVariable == null)
        //    {
        //        LocalVariableInfo localVar = new LocalVariableInfo("local_" + varIndex, type);
        //        //slots.put(varIndex, localVar);
        //        slots[varIndex] = localVar;
        //        localVariables.Add(localVar);
        //        resultVariable = localVar;
        //    }
        //    Type varType = resultVariable.Type;
        //    if (!varType.Equals(type) && (type.IsPrimitive() || varType.IsPrimitive()))
        //    {
        //        // Apparently the compiler reuses local variables and it is possible
        //        // that the types change. So if types change we have to drop
        //        // the variable and try again.
        //        //slots.put(varIndex, null);
        //        slots[varIndex] = null;
        //        return variable(varIndex, type);
        //    }
        //    return resultVariable;
        //}

        public void VisitExceptionHandlerCollection(ExceptionHandlerCollection seh)
        {
            foreach (ExceptionHandler handler in seh)
            {
                VisitExceptionHandler(handler);
            }
        }
         
        public void VisitExceptionHandler(ExceptionHandler eh)
        {
            recorder.Add(new TryCatchRunnable(block, cyclomaticComplexity, eh.TryStart, eh.TryEnd, eh.HandlerStart, eh.HandlerEnd,
                                              eh.CatchType));
        }

        public void VisitVariableDefinitionCollection(VariableDefinitionCollection variables)
        {
            foreach (VariableDefinition definition in variables)
            {
                VisitVariableDefinition(definition);
            }
        }

        public void VisitVariableDefinition(VariableDefinition variable)
        {
            LocalVariableInfo localVar = new LocalVariableInfo("local_" + variable.Index, ClrType.FromDescriptor(variable.VariableType.FullName));
            slots[variable.Index] = localVar;
            localVariables.Add(localVar);
        }

        public void VisitScopeCollection(ScopeCollection scopes)
        {
#warning: NotImplementedException
        }

        public void VisitScope(Scope scope)
        {
            throw new NotImplementedException();
        }

        public void TerminateMethodBody(MethodBody body)
        {
            foreach (IRunnable runnable in recorder)
            {
                runnable.run();
            }
            block.decomposeIntoBlocks();

            // TODO, set startingLineNumber
            Console.WriteLine("******\r\nBefore add MethodInfo:" + Name + "{"+ClassInfo+"}");
            MethodInfo methodInfo = new MethodInfo(ClassInfo, Name, -1,
                Descriptor, methodThis, parameters, localVariables, Visibility,
                cyclomaticComplexity, block.getOperations(), IsFinal);
            ClassInfo.AddMethod(methodInfo);
        }

        #endregion
    }
}