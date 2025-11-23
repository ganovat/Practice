using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        

        public void AddNumbersTest()
        {
            Calculations calcObject = new Calculations();
            int result = 0;
            int expectedValue = 7;
            Assert.AreEqual(expectedValue, result);
        }
    }
}