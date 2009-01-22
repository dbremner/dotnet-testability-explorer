using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Testability.Explorer.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void TestIndexOfCollection()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            bool result = true;
            try
            {
                dic["test"] = "test";
            }
#pragma warning disable 168
            catch (Exception ex)
#pragma warning restore 168
            {
                result = false;
            }
            Assert.IsTrue(result);
        }
    }
}
