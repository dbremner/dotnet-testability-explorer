namespace Thinklouder.Testability.Tests.IL
{
    public class CastSample
    {
        public class TypeA
        {
            
        }

        public class TypeB : TypeA
        {
            
        }

        public CastSample()
        {
            var a = new TypeA();
            var b = new TypeB();
            TypeA c = new TypeB();

            var d = b is TypeA;
            TypeA e = b;
            var f = b as TypeA;

        }
    }
}