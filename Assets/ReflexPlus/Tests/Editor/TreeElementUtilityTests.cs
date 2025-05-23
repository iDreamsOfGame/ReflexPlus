using System;
using System.Collections.Generic;
using NUnit.Framework;
using ReflexPlusEditor.DebuggingWindow;

namespace ReflexPlusEditor.Tests
{
    internal class TreeElementUtilityTests
    {
        private class TestElement : TreeElement
        {
            public TestElement(string name, int depth)
            {
                Name = name;
                Depth = depth;
            }
        }

        [Test]
        public void TestTreeToListWorks()
        {
            // Arrange
            var root = new TestElement("root", -1)
            {
                Children = new List<TreeElement>
                {
                    new TestElement("A", 0),
                    new TestElement("B", 0),
                    new TestElement("C", 0)
                }
            };

            root.Children[1].Children = new List<TreeElement> { new TestElement("Bchild", 1) };

            root.Children[1].Children[0].Children = new List<TreeElement> { new TestElement("Bchildchild", 2) };

            // Test
            var result = new List<TestElement>();
            TreeElementUtility.TreeToList(root, result);

            // Assert
            string[] namesInCorrectOrder = { "root", "A", "B", "Bchild", "Bchildchild", "C" };
            Assert.That(namesInCorrectOrder.Length, Is.EqualTo(result.Count), "Result count is not match");
            for (var i = 0; i < namesInCorrectOrder.Length; ++i)
            {
                Assert.That(namesInCorrectOrder[i], Is.EqualTo(result[i].Name));
            }

            TreeElementUtility.ValidateDepthValues(result);
        }

        [Test]
        public void TestListToTreeWorks()
        {
            // Arrange
            var list = new List<TestElement>
            {
                new("root", -1),
                new("A", 0),
                new("B", 0),
                new("Bchild", 1),
                new("Bchildchild", 2),
                new("C", 0)
            };

            // Test
            var root = TreeElementUtility.ListToTree(list);

            // Assert
            Assert.That(root.Name, Is.EqualTo("root"));
            Assert.That(root.Children.Count, Is.EqualTo(3));
            Assert.That(root.Children[2].Name, Is.EqualTo("C"));
            Assert.That(root.Children[1].Children[0].Children[0].Name, Is.EqualTo("Bchildchild"));
        }

        [Test]
        public void TestListToTreeThrowsExceptionIfRootIsInvalidDepth()
        {
            // Arrange
            var list = new List<TestElement>
            {
                new("root", 0),
                new("A", 1),
                new("B", 1),
                new("Bchild", 2)
            };

            // Test
            var hasCatchedException = false;
            try
            {
                TreeElementUtility.ListToTree(list);
            }
            catch (Exception)
            {
                hasCatchedException = true;
            }

            // Assert
            Assert.That(hasCatchedException, Is.True, "We require the root.depth to be -1, here it is: " + list[0].Depth);
        }
    }
}