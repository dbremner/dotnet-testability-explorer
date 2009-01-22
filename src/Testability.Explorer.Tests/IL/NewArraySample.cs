using C5;

namespace Thinklouder.Testability.Tests.IL
{
    public class NewArraySample
    {
        public NewArraySample()
        {
            int[] arr = new int[10];
            IList<int>[] arr1 = new ArrayList<int>[10000000];
            int[][] arr2 = new int[10][];

            double[][][][][][] arr3 = null;
            arr3 = new double[2][][][][][];
        }
    }
}