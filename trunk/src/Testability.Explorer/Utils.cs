using C5;

namespace Thinklouder.Testability
{
    public class Arrays
    {
        public static ArrayList<T> asList<T>(params T[] items)
        {
            var list = new ArrayList<T>();
            for ( int i = 0; i < items.Length; i++ )
            {
                list.Add(items[i]);
            }
            return list;
        }
    }
}
