using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinklouder.Testability.Tests.Metrics
{
    public class CyclomaticComplexityTest
    {
        public class CyclomaticMethods
        {
            public void EmptyMethod_1()
            { }

            public void SimpleMethod_1()
            {
                int i = 0;
                i += 1;
            }

            public void IfMethod_2()
            {
                int a = 0;
                if ( a < 0 )
                {
                    a++;
                }
                else
                {
                    a--;
                }
            }

            public void IfMethodNoElse_2()
            {
                int a = 0;
                if ( a < 0 )
                {
                    a++;
                }
            }

            public void TryCatch_2()
            {
                int a = 0;
                try
                {
                    a++;
#pragma warning disable 168
                }catch(Exception e)
#pragma warning restore 168
                {
                    a++;
                }
            }

            public void TryCatchFinally_2()
            {
                int a = 0;
                try
                {
                    a++;
                }
#pragma warning disable 168
                catch (Exception e)
#pragma warning restore 168
                {
                    a++;
                }
                finally
                {
                    a++;
                }
            }

            public void EmptySwitch_2()
            {
                int a = 0;
                switch (a) 
                {
                    case 0:
                        a = 0;
                        break;
                }
            }

            public void EmptySwitch_5()
            {
                int a = 0;
                switch (a)
                {
                    case 0:
                        a = 0;
                        break;
                    case 1:
                        a = 1;
                        break;
                    case 2:
                        a = 2;
                        break;
                    case 4:
                        a = 4;
                        break;
                    default:
                        break;
                }
            }

            public void SwitchImplementWithLookUp_3()
            {
                int a = 0;
                switch (a)
                {
                    case 0:
                        a = 0;
                        break;
                    case 9999:
                        a = 9999;
                        break;
                    default:
                        a = -1;
                        break;
                }
            }

            public void SwitchWithDefault_2()
            {
                int a = 0;
                switch (a)
                {
                    case 0:
                        a = 0;
                        break;
                    default:
                        break;
                }
            }

            public void testVerifyAllMethodsCyclomaticComplexity()
            {
                
            }
        }
    }
}
