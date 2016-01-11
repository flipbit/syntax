using System;
using NUnit.Framework;

namespace Syntax.Layout.Tests
{
    [TestFixture]
    public class ResourceReaderTests
    {
        private ResourceReader reader;

        [SetUp]
        public void SetUp()
        {
            reader = new ResourceReader();
        }

        [Test]
        public void Test()
        {
            var contents = reader.Read(this);

            Assert.AreEqual("- type: Image", contents);
        }
    }
}

