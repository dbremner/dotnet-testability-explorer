using C5;

namespace Thinklouder.Testability.Metrics.Method.Collection
{
    public abstract class PopClosure<KEY,VALUE>
    {
        public abstract int getSize();

        public abstract IList<VALUE> pop(KEY key, IList<VALUE> list);
    }
}
