#region

using System;
using C5;
using Thinklouder.Testability.Metrics.Method.Op.Stack;
using SCG=System.Collections.Generic;
using System.Text;

#endregion

namespace Thinklouder.Testability.Metrics.Method
{
    public class Block
    {
        private readonly String id;
        private readonly bool isTerminal = false;
        private readonly IList<Block> nextBlocks = new ArrayList<Block>();
        private readonly IList<StackOperation> operations = new ArrayList<StackOperation>();
        private readonly IList<Block> previousBlocks = new ArrayList<Block>();
        private Constant exception;

        public Block(String id)
        {
            this.id = id;
        }

        public bool IsTerminal
        {
            get { return isTerminal; }
        }

        public void addNextBlock(Block nextBlock)
        {
            if ( !nextBlocks.Contains(nextBlock) && nextBlock != this )
            {
                nextBlocks.Add(nextBlock);
                nextBlock.previousBlocks.Add(this);
            }
        }

        public void addOp(StackOperation operation)
        {
            operations.Add(operation);
        }

        public IList<StackOperation> getOperations()
        {
            return operations;
        }

        public override string ToString()
        {
            var buf = new StringBuilder();
            buf.Append(exception == null ? "Block" : "ExceptionHandlerBlock");
            buf.Append("[");
            buf.Append(id);
            String sep = " -> ";
            foreach ( var next in nextBlocks )
            {
                buf.Append(sep);
                buf.Append(next.id);
                sep = ", ";
            }
            sep = " <- ";
            foreach ( var next in previousBlocks )
            {
                buf.Append(sep);
                buf.Append(next.id);
                sep = ", ";
            }
            buf.Append("]{\n");
            foreach ( var operation in operations )
            {
                buf.Append("  ");
                buf.Append(operation);
                buf.Append("\n");
            }
            buf.Append("}");
            return buf.ToString();
        }

        public IList<Block> getNextBlocks()
        {
            //return new GuardedList<Block>(nextBlocks); // should be a readonly collection
            return nextBlocks;
        }

        public String getId()
        {
            return id;
        }

        public void setExceptionHandler(int lineNumber, Constant exception)
        {
            if ( this.exception == null )
            {
                operations.Insert(0, new Load(lineNumber, exception));
            }
            this.exception = exception;
        }
        
    }
}