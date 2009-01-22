using System;

namespace Thinklouder.Testability.Tests.IL
{
    public class SwitchSample
    {
        //public SwitchSample()
        //{
        //    for (int i = 0; i < 5; i++)
        //    {
        //        switch (i)
        //        {
        //            case 1:
        //                Console.WriteLine("In case 1");
        //                goto case 3;
        //            case 2:
        //                Console.WriteLine("In case 2");
        //                goto case 1;
        //            case 3:
        //                Console.WriteLine("In case 3");
        //                goto default;
        //            default:
        //                Console.WriteLine("In default");
        //                goto Tests;
        //                break;
        //        }
        //    }

        //    // goto case 1; // ERROR, can't jump into a switch


        //    Tests:
        //    Console.WriteLine("test label");
        //}

        //public void LargeSwitch()
        //{
        //    var param = "a";
        //    switch (param)
        //    {
        //        case "a": break;
        //        case "b": break;
        //        case "c": break;
        //        case "d": break;
        //        case "e": break;
        //        case "f": break;
        //        case "g": break;
        //        case "h": break;
        //        case "i": break;
        //        case "j": break;
        //        case "k": break;
        //    }
        //}

        //public void SmallSwitch()
        //{
        //    var param = "a";
        //    switch (param)
        //    {
        //        case "a": break;
        //        case "b": break;
        //        case "c": break;
        //    }
        //}

        //public void SwitchWith6Cases()
        //{
        //    var param = "a";
        //    switch (param)
        //    {
        //        case "a": break;
        //        case "b": break;
        //        case "c": break;
        //        case "d": break;
        //        case "e": break;
        //        case "f": break;
        //    }
        //}

        public void SwitchWith7Cases()
        {
            var param = "a";
            switch (param)
            {
                case "a": break;
                case "b": break;
                case "c": break;
                case "d": break;
                case "e": break;
                case "f": break;
                case "g": break;
            }
        }

        //public void SwitchWithIntegerCases()
        //{
        //    var param = 1;
        //    switch (param)
        //    {
        //        case 0: break;
        //        case 1: break;
        //        case 2: break;
        //        case 3: break;
        //        case 4: break;
        //    }
        //}

    }
}
