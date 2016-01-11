using System;
using NUnit.Framework;
using Syntax.Layout.Components;

namespace Syntax.Layout.Tests
{
    [TestFixture]
    public class ComponentParserTests
    {
        ComponentParser parser;
        ResourceReader reader;

        [SetUp]
        public void SetUp()
        {
            parser = new ComponentParser();
            reader = new ResourceReader();
        }

        [Test]
        public void TestParseLabel()
        {
            var layout = reader.Read("Label");

            var components = parser.Parse(layout);

            Assert.AreEqual(1, components.Count);
            Assert.AreEqual(typeof(Label), components[0].GetType());

            var label = (Label) components[0];

            Assert.AreEqual("TestLabel", label.Id);
            Assert.AreEqual("Hello World", label.Text);
        }
    }
}

