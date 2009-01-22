using Mono.Cecil;

namespace Thinklouder.Testability.Metrics.Asm
{
    public class VisibilityUtil
    {
        public static Visibility ValueOf(MethodDefinition method)
        {
            Visibility visibility;

            if ( method.IsPublic ) visibility = Visibility.PUBLIC;
            else if ( method.IsPrivate ) visibility = Visibility.PRIVATE;
            else if ( method.IsAssembly ) visibility = Visibility.PACKAGE;
            else if ( method.IsFamily ) visibility = Visibility.PROTECTED;
            else visibility = Visibility.NULL;

            return visibility;
        }
    }
}