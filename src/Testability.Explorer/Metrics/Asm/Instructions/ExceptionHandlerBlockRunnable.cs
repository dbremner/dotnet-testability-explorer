using C5;
using Thinklouder.Testability.Metrics.Method;
using Thinklouder.Testability.Metrics.Method.Op.Stack;

namespace Thinklouder.Testability.Metrics.Asm.Instructions
{
    public class ExceptionHandlerBlockRunnable : IRunnable
    {
        private readonly IDictionary<Label, BlockDecomposer.Frame> frames;
        private readonly IList<Block> exceptionHandlerBlocks;
        private readonly Label handler;
        private readonly string typeName;

        public ExceptionHandlerBlockRunnable(IDictionary<Label, BlockDecomposer.Frame> frames, IList<Block> exceptionHandlerBlocks, Label handler, string typeName)
        {
            this.frames = frames;
            this.exceptionHandlerBlocks = exceptionHandlerBlocks;
            this.handler = handler;
            this.typeName = typeName;
        }

        public void run()
        {
            // TODO, why there will be handler not found? by sunlw
            if (frames.Contains(handler))
            {
                Block handlerBlock = frames[handler].block;
                Type type = string.IsNullOrEmpty(typeName)
                                ? ClrType.FromDescriptor(ClassInfo.GetFullName<System.Exception>())
                                : ClrType.FromDescriptor(typeName);
                handlerBlock.setExceptionHandler(-1, new Constant("?", type));
                exceptionHandlerBlocks.Add(handlerBlock);
            }
        }
    }
}