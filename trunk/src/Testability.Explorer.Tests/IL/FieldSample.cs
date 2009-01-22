namespace Thinklouder.Testability.Tests.IL
{
    public class FieldSample
    {
        public int FieldA { get; set; }

        public static int FieldB { get; set; }
    }

    public class FieldClientSample
    {
        public void Test()
        {
            var field = new FieldSample();
            field.FieldA = 1;

            int a = field.FieldA;

            FieldSample.FieldB = 1;

            int b = FieldSample.FieldB;
        }
    }
}