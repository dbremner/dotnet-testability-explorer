using System;
using C5;
using Thinklouder.Testability.Metrics.Method.Collection;
using Thinklouder.Testability.Metrics.Method.Op.Stack;
using Thinklouder.Testability.Metrics.Method.Op.Turing;
using SCG = System.Collections.Generic;

namespace Thinklouder.Testability.Metrics.Method
{
    public class Stack2Turing
    {
        public static readonly object EMPTY = new object();

        private readonly IList<Operation> operations = new ArrayList<Operation>();
        private readonly ValueCompactor<Variable> pathCompactor = new VariableCompactor();
        private readonly Block rootBlock;
        public KeyedMultiStack<Block, Variable> stack;

        public Stack2Turing(Block block)
        {
            stack = new KeyedMultiStack<Block, Variable>(pathCompactor);
            rootBlock = block;
        }

        public IList<Operation> translate()
        {
            stack.init(rootBlock);
            translate(rootBlock, new ArrayList<Block>());
            return operations;
        }

        private Block translate(Block block, IList<Block> parentProcessed)
        {
            IList<Block> blocks = new ArrayList<Block>();
            IList<Block> processed = parentProcessed;
            blocks.Add(block);
            while (blocks.Count != 0)
            {
                // TODO, debug print by sunlw
                Console.WriteLine("==============\r\n while translate(Block block, IList<Block> parentProcessed): \r\n" + block);

                block = blocks.RemoveAt(0);
                processed.Add(block);
                foreach (StackOperation operation in block.getOperations())
                {
                    translateStackOperation(block, operation);
                    if (operation is JMP)
                    {
                        var jmp = (JMP) operation;
                        Block jmpBlock = jmp.Block;
                        stack.split(block, new ArrayList<Block> {jmpBlock});
                        Block terminalBlock = translate(jmpBlock, processed);
                        stack.join(new ArrayList<Block> {terminalBlock}, block);
                        processed.Add(jmpBlock);
                    }
                }
                //List<Block> nextBlocks = new ArrayList<Block>(block.getNextBlocks());
                IList<Block> nextBlocks = block.getNextBlocks(); // why new an ArrayList?

                nextBlocks.RemoveAll(processed);// Don't visit already visited
                //processed.ForEach(item=>nextBlocks.Remove(item)); 

                //foreach (Block block1 in processed)
                //{
                //    nextBlocks.Remove(block1);
                //}
                if (nextBlocks.Count > 0)
                {
                    stack.split(block, nextBlocks);
                }
                blocks.AddAll(nextBlocks);
                //processed.ForEach(item=>blocks.Remove(item));
                blocks.RemoveAll(processed);
            }
            // It appears that when exceptions are involved a method might have
            // paths where stacks are not emptied. So we can't assert this.
            // Verdict is still out.
            // stack.assertEmpty();
            return block;
        }


        private void translateStackOperation(Block block, StackOperation operation)
        {
            var pop = new Pop(operation, operations);
            stack.apply(block, pop);
        }

        protected void assertValid(IList<Variable> variables)
        {
            SCG.IEnumerator<Variable> iter = variables.GetEnumerator();
            while (iter.MoveNext())
            {
                Variable variable = iter.Current;
                if (ClrType.IsDoubleSlot(variable.Type))
                {
                    Variable varNext = iter.MoveNext() ? iter.Current : null;
                    if (variable != varNext)
                    {
                        throw new InvalidOperationException("Variable list '" + variables
                                                            + "' contanins variable '" + variable
                                                            + "' which is a double but the next "
                                                            + "variable in the list is not a duplicate.");
                    }
                }
            }
        }

        #region Nested type: Pop

        private class Pop : PopClosure<Block, Variable>
        {
            public Pop(StackOperation operation, IList<Operation> operations)
            {
                Operation = operation;
                Operations = operations;
            }

            public StackOperation Operation { get; private set; }

            public IList<Operation> Operations { get; private set; }

            public override int getSize()
            {
                return Operation.getOperatorCount();
            }

            public override IList<Variable> pop(Block key, IList<Variable> input)
            {
                IList<Variable> variables = Operation.apply(input);
                // For performance reasons the line is commented out.
                // assertValid(variables);
                Operation turingOp = Operation.ToOperation(input);
                if (turingOp != null)
                {
                    Operations.Add(turingOp);
                }
                return variables;
            }
        }

        #endregion

        #region Nested type: VariableCompactor

        public class VariableCompactor : ValueCompactor<Variable>
        {
            public override IList<IList<Variable>> compact(IList<IList<Variable>> pushValues)
            {
                if (pushValues.Count < 2)
                {
                    return pushValues;
                }
                IList<IList<Variable>> compacted = new ArrayList<IList<Variable>>();
                ICollection<Object> equivalent = new HashSet<Object>();
                SCG.IEnumerator<IList<Variable>> iter;

                for (iter = pushValues.GetEnumerator(); iter.MoveNext();)
                {
                    IList<Variable> values = iter.Current;
                    Object key = computeKey(values);
                    if (equivalent.Add(key))
                    {
                        compacted.Add(values);
                    }
                }
                return compacted;
            }

            private Object computeKey(C5.IList<Variable> variables)
            {
                int size = variables.Count;
                if (size == 0)
                {
                    return EMPTY;
                }
                else if (size == 1)
                {
                    return computeKey(variables[0]);
                }
                else
                {
                    IList<Object> keys = new ArrayList<Object>();
                    foreach (Variable variable in variables)
                    {
                        keys.Add(computeKey(variable));
                    }
                    return keys;
                }
            }

            private Object computeKey(Variable variable)
            {
                Object key;
                if (variable is Constant)
                {
                    var constant = (Constant) variable;
                    key = constant.Type;
                }
                else
                {
                    key = variable;
                }
                return key;
            }
        }

        #endregion
    }
}