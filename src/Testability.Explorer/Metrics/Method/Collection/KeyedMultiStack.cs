using System;
using SCG=System.Collections.Generic;
using System.Text;
using C5;

namespace Thinklouder.Testability.Metrics.Method.Collection
{
    /**
     * This class acts as a stack externally. The difference is that internally it
     * is implemented with several stacks growing in parallel. The stack starts as a
     * single stack when it is constructed. As it grows it can be split into
     * multiple keys. The keys can then be rejoined to a new key. Any push/pop
     * operations will be performed on one or more internal stacks depending on how
     * the stack was split and rejoined.
     *
     * This is useful when analyzing stack machines (such as Java JVM) which keep
     * variables on the stack but whose execution can split and rejoin on the level
     * of byte-codes.
     *
     *
     * @author mhevery@google.com <Misko Hevery>
     *
     * @param <KEY>
     *            Selector which decides which of the stacks are being pushed /
     *            popped
     * @param <VALUE>
     *            Value on stack.
     */

    public class KeyedMultiStack<KEY, VALUE> // where VALUE : class
    {
        private readonly IDictionary<KEY, ICollection<Entry<VALUE>>> head =
            new HashDictionary<KEY, ICollection<Entry<VALUE>>>();

        private readonly ValueCompactor<VALUE> pathCompactor;
        private Entry<VALUE> root = new Entry<VALUE>();

        /**
         * @param key Initial key for the primordial stack.
         */

        public KeyedMultiStack(KEY key, ValueCompactor<VALUE> pathCompactor)
        {
            this.pathCompactor = pathCompactor;
            init(key);
        }

        public KeyedMultiStack(ValueCompactor<VALUE> pathCompactor)
        {
            this.pathCompactor = pathCompactor;
        }

        public void init(KEY key)
        {
            head.Clear();
            head[key] = set(root);
        }

        private ICollection<Entry<VALUE>> getHead(KEY key)
        {
            if (!head.Contains(key))
            {
                throw new KeyNotFoundException(key);
            }
            return head[key];
        }

        private ICollection<Entry<VALUE>> removeHead(KEY key)
        {
            if (!head.Contains(key))
            {
                throw new KeyNotFoundException(key);
            }
            ICollection<Entry<VALUE>> value = head[key];
            head.Remove(key);
            return value;
        }

        private ICollection<Entry<VALUE>> set(Entry<VALUE> entrie)
        {
            var entries = new HashSet<Entry<VALUE>>();
            entries.Add(entrie);
            return entries;
        }

        /**
         * Pop value from the stack. The closure can be called more then once if there are parallel stacks
         * due to splits.
         *
         * @param key        key as stack selector
         * @param popClosure Closer which will be called once per each virtual stack.
         */

        public void apply(KEY key, PopClosure<KEY, VALUE> popClosure)
        {
            var popSize = popClosure.getSize();
            var headEntries = getHead(key);
            ICollection<Path<VALUE>> paths = fillPopPaths(headEntries, popSize);
            popPaths(key, popSize);
            IList<IList<VALUE>> pushValues = new ArrayList<IList<VALUE>>(paths.Count);
            int pushSize = -1;
            foreach (Path<VALUE> path in paths)
            {
                IList<VALUE> pushSet = popClosure.pop(key, path.asList());
                if (pushSize == -1)
                {
                    pushSize = pushSet.Count;
                }
                else if (pushSize != pushSet.Count)
                {
                    throw new InvalidOperationException(
                        "All push pushes must be of same size.");
                }
                pushValues.Add(pushSet);
            }
            pushValues = pathCompactor.compact(pushValues);
            if (pushSize > 0)
            {
                ICollection<Entry<VALUE>> parent = head[key];
                ICollection<Entry<VALUE>> newHead = new HashSet<Entry<VALUE>>();
                foreach (var values in pushValues)
                {
                    Entry<VALUE> entry = null;
                    foreach (VALUE value in values)
                    {
                        if (entry == null)
                        {
                            entry = new Entry<VALUE>(parent, value);
                        }
                        else
                        {
                            entry = new Entry<VALUE>(set(entry), value);
                        }
                    }
                    newHead.Add(entry);
                }
                head[key] = newHead;
            }
        }

        private void popPaths(KEY key, int size)
        {
            if (size == 0)
            {
                return;
            }
            ICollection<Entry<VALUE>> newEntries = new HashSet<Entry<VALUE>>();
            foreach (var entry in getHead(key))
            {
                newEntries.AddAll(entry.getParents());
            }
            head[key] = newEntries;
            popPaths(key, size - 1);
        }

        private ICollection<Path<VALUE>> fillPopPaths(ICollection<Entry<VALUE>> entries, int size)
        {
            ICollection<Path<VALUE>> paths = new HashSet<Path<VALUE>>();
            if (size == 0)
            {
                paths.Add(new Path<VALUE>());
            }
            else
            {
                foreach (var entry in entries)
                {
                    if (entry.Depth < size - 1)
                    {
                        throw new StackUnderflowException();
                    }
                    ICollection<Path<VALUE>> paths1 = fillPopPaths(entry.getParents(), (size - 1));
                    foreach (var path in  paths1)
                    {
                        path.add(entry.Value);
                        paths.Add(path);
                    }
                }
            }
            return paths;
        }

        /**
         * Split the internal stacks to a new set of stacks
         *
         * @param key     Stack(s) to split.
         * @param subKeys New names for those stacks
         */

        public void split(KEY key, IList<KEY> subKeys)
        {
            ICollection<Entry<VALUE>> entries = removeHead(key);
            foreach (KEY subKey in subKeys)
            {
                if (head.Contains(subKey))
                {
                    ICollection<Entry<VALUE>> existingList = head[subKey];
                    entries.AddAll(existingList);
                }
            }
            assertSameDepth(entries);
            foreach (KEY subKey in subKeys)
            {
                head[subKey] = entries;
            }
        }

        /**
         * Rejoin the stacks. This is not always possible since each stack may now be different, but it
         * will make sure that the new single key will now refer to all of the stacks treating them as
         * one. This will make the pop operation be applied to individual stacks.
         *
         * @param subKeys a list of keys for the old stacks to join.
         * @param newKey  new name for the stack
         */

        public void join(ICollection<KEY> subKeys, KEY newKey)
        {
            ICollection<Entry<VALUE>> newHead = new HashSet<Entry<VALUE>>();
            foreach (KEY key in subKeys)
            {
                ICollection<Entry<VALUE>> entries = getHead(key);
                newHead.AddAll(entries);
            }
            assertSameDepth(newHead);
            foreach (KEY key in subKeys)
            {
                removeHead(key);
            }
            head[newKey] = newHead;
        }

        private void assertSameDepth(ICollection<Entry<VALUE>> entries)
        {
            int expectedDepth = int.MinValue;
            foreach (var entry in entries)
            {
                if (expectedDepth == int.MinValue)
                {
                    expectedDepth = entry.Depth;
                }
                else if (expectedDepth != entry.Depth)
                {
                    throw new InvalidOperationException(
                        "Not all entries are at same depth. Can't join.");
                }
            }
        }

        public void assertEmpty()
        {
            foreach (var entries in head.Values)
            {
                foreach (var entry in entries)
                {
                    if (entry.Depth > -1)
                    {
                        throw new InvalidOperationException("Stack not empty.");
                    }
                }
            }
        }

        public override string ToString()
        {
            return head.ToString();
        }

        #region Nested type: KeyNotFoundException

        public class KeyNotFoundException : SystemException
        {
            //private static readonly long serialVersionUID = 4649233306901482842L;

            public KeyNotFoundException(KEY key)
                : base("Key '" + key + "' not found.")
            {
            }
        }

        #endregion
    }


    public class ValueCompactor<VALUE> // where VALUE : class
    {
        public virtual IList<IList<VALUE>> compact(IList<IList<VALUE>> pushValues)
        {
            return pushValues;
        }
    }

    public class StackUnderflowException : SystemException
    {
        //private static readonly long serialVersionUID = 4649233306901482842L;
    }


    public class Entry<VALUE>
    {
        private readonly int depth;
        private readonly ICollection<Entry<VALUE>> parents;
        private readonly VALUE value;

        public Entry()
        {
            depth = -1;
            parents = null;
            //this.value = null;
        }

        public Entry(ICollection<Entry<VALUE>> parents, VALUE value)
        {
            this.value = value;
            this.parents = parents;
            if (this.parents.Count == 0)
            {
                depth = 0;
            }
            else
            {
                SCG.IEnumerator<Entry<VALUE>> enumerator = this.parents.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    depth = enumerator.Current.depth + 1;
                }
            }
        }

        public VALUE Value
        {
            get { return value; }
        }

        public int Depth
        {
            get { return depth; }
        }

        public override string ToString()
        {
            var buf = new StringBuilder();
            buf.Append("\n");
            toString(buf, "");
            return buf.ToString();
        }

        private void toString(StringBuilder buf, String offset)
        {
            if (depth == -1)
            {
                return;
            }
            buf.Append(offset);
            buf.Append("#");
            buf.Append(depth);
            buf.Append("(");
            buf.Append(value);
            buf.Append(")\n");
            foreach (var parent in parents)
            {
                parent.toString(buf, offset + "  ");
            }
        }

        public ICollection<Entry<VALUE>> getParents()
        {
            if (depth == -1)
            {
                throw new StackUnderflowException();
            }
            return parents;
        }
    }

    public class Path<VALUE>
    {
        private VALUE[] elements = new VALUE[4];
        private int size;

        public void add(VALUE value)
        {
            ensureSize();
            elements[size++] = value;
        }

        private void ensureSize()
        {
            if (elements.Length == size)
            {
                VALUE[] newElements = new VALUE[elements.Length*2];
                //var newElements = new VALUE[elements.Length];
                //System.arraycopy(elements, 0, newElements, 0, elements.length);
                //elements = newElements;
                elements.CopyTo(newElements, 0);
                elements = newElements;
            }
        }

        public IList<VALUE> asList()
        {
            //return Arrays.asList(elements).subList(0, size);
            var list = new ArrayList<VALUE>();
            list.AddAll(elements);
            return list.View(0, size);
            //return list;   // TODO, why return a readonly copy, by sunlw
        }

        public override string ToString()
        {
            var buf = new StringBuilder();
            int count = 0;
            foreach (VALUE value in elements)
            {
                if (count >= size)
                {
                    break;
                }
                if (count > 0)
                {
                    buf.Append(" :: ");
                }
                buf.Append(value);
                count++;
            }
            return buf.ToString();
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            var list = new ArrayList<VALUE>();
            list.AddAll(elements);
            result = prime*result + GetHashCode(list); //list.GetHashCode();
            result = prime*result + size;
            return result;
        }

        /// <summary>
        /// Get HashCode of a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        private int GetHashCode<T>(SCG.IEnumerable<T> items)
        {
            int hashCode = 1;
            SCG.IEnumerator<T> enumerator = items.GetEnumerator();
            do
            {
                T current = enumerator.Current;
                hashCode = 31*hashCode + (current == null ? 0 : current.GetHashCode());
            } while (enumerator.MoveNext());
            return hashCode;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            var other = (Path<VALUE>) obj;
            //if (!Arrays.equals(elements, other.elements)) {
            //  return false;
            //}
            if (elements.Length != other.elements.Length)
            {
                return false;
            }

            for (int i = 0; i < elements.Length; i++)
            {
                if (!(elements[i] == null && other.elements[i] == null) && !elements[i].Equals(other.elements[i]))
                {
                    return false;
                }
            }

            if (size != other.size)
            {
                return false;
            }
            return true;
        }
    }
}