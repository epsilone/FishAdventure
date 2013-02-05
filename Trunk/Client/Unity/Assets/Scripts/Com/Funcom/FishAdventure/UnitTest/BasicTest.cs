#region Usings

using System;
using System.Collections.Generic;
using NUnit.Framework;


#endregion

namespace Test
{
    [TestFixture]
    public class BasicTest
    {
        #region Setup/Teardown

        [SetUp]
        public void InitSources()
        {
            source01 = new[] {1, 2, 3};
            source02 = new List<int>(source01);
        }

        #endregion

        private int[] source01;
        private List<int> source02;


        [Test]
        public void WorkTest()
        {
            Assert.That(true, Is.EqualTo(true));
        }
		
		[Test]
        public void FailTest()
        {
            Assert.That(true, Is.EqualTo(false), "OMG WTF");
        }
		
		[Test]
        public void FailTest2()
        {
            Assert.That(true, Is.EqualTo(false), "OMG WTF 2");
        }
    }
}
