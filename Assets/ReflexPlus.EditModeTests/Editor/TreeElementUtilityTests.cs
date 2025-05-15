using System;
using System.Collections.Generic;
using NUnit.Framework;
using ReflexPlusEditor.DebuggingWindow;

namespace ReflexPlus.EditModeTests
{
    public class TreeElementUtilityTests
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
            Assert.AreEqual(namesInCorrectOrder.Length, result.Count, "Result count is not match");
            for (var i = 0; i < namesInCorrectOrder.Length; ++i)
            {
                Assert.AreEqual(namesInCorrectOrder[i], result[i].Name);
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
            Assert.AreEqual("root", root.Name);
            Assert.AreEqual(3, root.Children.Count);
            Assert.AreEqual("C", root.Children[2].Name);
            Assert.AreEqual("Bchildchild", root.Children[1].Children[0].Children[0].Name);
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
            Assert.IsTrue(hasCatchedException, "We require the root.depth to be -1, here it is: " + list[0].Depth);
        }
    }
}