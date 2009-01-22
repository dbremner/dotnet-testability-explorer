using System;
using System.Text;
using C5;
using Thinklouder.Testability.Metrics.Asm;
using Thinklouder.Testability.Metrics.Asm.Instructions;
using Thinklouder.Testability.Metrics.Method.Op.Stack;
using Thinklouder.Testability.Metrics.Method.Op.Turing;

namespace Thinklouder.Testability.Metrics.Method
{
    /**
     * @author misko@google.com <Misko Hevery>
     */
    public class BlockDecomposer
    {

        /**
         * Method is broken down into one frame per bytecode in order to break it down
         * into blocks
         */

        public class Frame
        {

            /**
             * Next frame (ie next bytecode)
             */
            private Frame next;
            /**
             * Operation in this frame
             */
            private readonly StackOperation operation;
            /**
             * Any label associated with this bytecode
             */
            private Label label;
            /**
             * Block which this bytecode ended up assigned to
             */
            public Block block;
            /**
             * True if this is first bytecode of a block (GOTO). If true then new block
             * will start here.
             */
            public bool blockStartsHere;
            /**
             * If true then this will be last instruction in block and new block will
             * start on next frame.
             */
            public bool blockEndsHere;
            /**
             * If not null current block will link with block that this field points to.
             */
            public IList<Label> gotoLabels = new ArrayList<Label>();
            /**
             * This instruction ends execution of block (ie return, throw) If this is
             * true then this and subsequent blocks will not be linked
             */
            public bool terminal;

            public Frame(Frame frame, StackOperation operation, Label label)
            {
                this.operation = operation;
                this.label = label;
                if (frame != null)
                {
                    frame.nextFrame(this);
                }
            }

            public Label Label
            {
                get { return label; }
                set { this.label = value; }
            }

            public Frame Next
            {
                get { return next; }
            }

            public StackOperation Operation
            {
                get { return operation; }
            }

            public Block Block
            {
                get { return block; }
            }

            private void nextFrame(Frame frame)
            {
                next = frame;
            }

            public override string ToString()
            {
                //ByteArrayOutputStream buf = new ByteArrayOutputStream();
                //PrintStream out = new PrintStream(buf, true);
                //out.printf("%-10s %-70s %-15s s=%-5b e=%-5b t=%-5b goto=%s", label, operation,
                //    block == null ? null : block.getId(), blockStartsHere, blockEndsHere,
                //    terminal, gotoLabels);
                //return buf.toString();
                var buf = new StringBuilder();
                buf.AppendFormat("");
                return buf.ToString();
            }
        }

        private readonly IDictionary<Label, Frame> frames = new HashDictionary<Label, Frame>();
        private readonly IDictionary<Label, Block> subroutineBlocks = new HashDictionary<Label, Block>();
        private readonly IList<IRunnable> extraLinkSteps = new ArrayList<IRunnable>();
        private readonly IList<Block> exceptionHandlerBlocks = new ArrayList<Block>();
        private Frame firstFrame;
        private Frame lastFrame;
        private Label lastLabel;
        private Block mainBlock;
        private int counter = 0;

        public void addOp(StackOperation operation)
        {
            lastFrame = new Frame(lastFrame, operation, lastLabel);
            if (operation is Return || operation is Throw)
            {
                lastFrame.terminal = true;
                lastFrame.blockEndsHere = true;
            }
            if (firstFrame == null)
            {
                firstFrame = lastFrame;
            }
            applyLastLabel();
        }

        private void applyLastLabel()
        {
            if (lastLabel != null)
            {
                frames.Add(lastLabel, lastFrame);
                lastFrame.Label = lastLabel;
                lastLabel = null;
            }
        }

        public void label(Label label)
        {
            if (lastLabel == null)
            {
                lastLabel = label;
            }
            else
            {
                throw new InvalidOperationException("Multiple lables per line are not allowed.");
            }
        }

        public void unconditionalGoto(Label label)
        {
            lastFrame.blockEndsHere = true;
            lastFrame.gotoLabels.Add(label);
            lastFrame.terminal = true;
            applyLastLabel();
        }

        public void conditionalGoto(Label label)
        {
            lastFrame.blockEndsHere = true;
            lastFrame.gotoLabels.Add(label);
            applyLastLabel();
        }

        public void jumpSubroutine(Label label, int lineNumber)
        {
            Block subBlock;
            if (!subroutineBlocks.Find(label, out subBlock))
            {
                subBlock = new Block("sub_" + (counter++));
            }
            addOp(new JMP(lineNumber, subBlock));
            subroutineBlocks[label] = subBlock;
            applyLastLabel();
        }

        public void tryCatchBlock(Label tryStart, Label tryEnd, Label handlerStart, string type)
        {
            extraLinkSteps.Add(new ExceptionHandlerBlockRunnable(frames, exceptionHandlerBlocks, handlerStart, type));
        }

        public void tableSwitch(Label dflt, IList<Label> labels)
        {
            lastFrame.terminal = true;
            lastFrame.blockEndsHere = true;
            lastFrame.gotoLabels.AddAll(labels);
            lastFrame.gotoLabels.Add(dflt);
            applyLastLabel();
        }

        public void decomposeIntoBlocks()
        {
            if (firstFrame != null)
            {
                breakIntoBlocks();
                copyToBlocks();
                linkBlocks();
                foreach (IRunnable extraLinkStep in extraLinkSteps)
                {
                    extraLinkStep.run();
                }
                mainBlock = firstFrame.block;
            }
        }

        private void breakIntoBlocks()
        {
            Frame frame = firstFrame;
            while (frame != null)
            {
                foreach (Label label in frame.gotoLabels)
                {
                    frames[label].blockStartsHere = true;
                    frame.blockEndsHere = true;
                }
                if (subroutineBlocks.Contains(frame.Label))
                {
                    frame.blockStartsHere = true;
                }
                frame = frame.Next;
            }
        }

        private void copyToBlocks()
        {
            Frame frame = firstFrame;
            Block block = null;
            String prefix = "block_";
            while (frame != null)
            {
                if (frame.blockStartsHere)
                {
                    block = null;
                }
                Label frameLabel = frame.Label;
                if (subroutineBlocks.Contains(frameLabel))
                {
                    block = subroutineBlocks[frameLabel];
                }
                if (block == null)
                {
                    block = new Block(prefix + (counter++));
                }
                else
                {
                    if (block.getId().StartsWith("sub"))
                    {
                        prefix = "sub_";
                    }
                }
                frame.block = block;
                StackOperation operation = frame.Operation;
                if (operation is RetSub)
                {
                    prefix = "block_";
                }
                frame.block.addOp(operation);
                if (frame.blockEndsHere)
                {
                    block = null;
                }
                frame = frame.Next;
            }
        }

        private void linkBlocks()
        {
            Frame previousFrame = firstFrame;
            Frame thisFrame = previousFrame.Next;
            while (thisFrame != null)
            {
                Block previousBlock = previousFrame.block;
                Block thisBlock = thisFrame.block;
                if (previousBlock != thisBlock && !previousFrame.terminal)
                {
                    previousBlock.addNextBlock(thisFrame.block);
                }
                foreach (Label label in previousFrame.gotoLabels)
                {
                    previousBlock.addNextBlock(frames[label].Block);
                }
                previousFrame = thisFrame;
                thisFrame = thisFrame.Next;
            }
        }

        public IList<Operation> getOperations()
        {
            if (mainBlock == null)
            {
                return new ArrayList<Operation>();
            }
            IList<Operation> operations = new ArrayList<Operation>();
            operations.AddAll(new Stack2Turing(mainBlock).translate());
            foreach (Block exceptionHandlerBlock in exceptionHandlerBlocks)
            {
                operations.AddAll(new Stack2Turing(exceptionHandlerBlock).translate());
            }
            return operations;
        }

        public Block getBlock(Label label)
        {
            return frames[label].block;
        }

        public Block getMainBlock()
        {
            return mainBlock;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            Frame frame = firstFrame;
            while (frame != null)
            {
                buf.Append(frame);
                buf.Append("\n");
                frame = frame.Next;
            }
            if (mainBlock != null)
            {
                frame = firstFrame;
                IList<Block> processed = new ArrayList<Block>();
                while (frame != null)
                {
                    Block block = frame.block;
                    if (!processed.Contains(block))
                    {
                        buf.Append(block);
                        processed.Add(block);
                    }
                    frame = frame.Next;
                }
            }
            return buf.ToString();
        }

        public IList<Block> getExceptionHandlerBlocks()
        {
            return exceptionHandlerBlocks;
        }

    }
}