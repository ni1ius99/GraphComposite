//-----------------------------------------------------------------------
// <copyright file="GraphCompositeBuilderTest.cs" company="Fluxtree Technologies LLC.">
// This is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
//-----------------------------------------------------------------------
namespace GraphComposite.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using GraphComposite;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// GraphCompositeBuilder unit test.
    /// </summary>
    [TestClass]
    public class GraphCompositeBuilderTest
    {
        /// <summary>
        /// Tests the three-argument constructor.
        /// </summary>
        [TestMethod]
        public void GraphCompositeBuilder3ArgConstructorTest()
        {
            GraphCompositeBuilder<int, string> gcb = new GraphCompositeBuilder<int, string>(EqualityComparer<int>.Default, 0, "root");
        }

        /// <summary>
        /// Tests the four-argument constructor and also the Configuration property.
        /// </summary>
        [TestMethod]
        public void GraphCompositeBuilder4ArgConstructorTest()
        {
            GraphCompositeBuilder<int, string> gcb = new GraphCompositeBuilder<int, string>(
                new GraphCompositeBuilderConfiguration(false, true),
                EqualityComparer<int>.Default,
                0,
                "root");

            Assert.IsFalse(gcb.Configuration.Hierarchical);
            Assert.IsTrue(gcb.Configuration.Acyclic);
        }

        /// <summary>
        /// Tests the TryGetValue method.
        /// </summary>
        [TestMethod]
        public void TryGetValueTest()
        {
            GraphCompositeBuilder<int, string> gcb = new GraphCompositeBuilder<int, string>(EqualityComparer<int>.Default, 0, "root");
            gcb.AddNode(0, 1, "subG", true);
            string value = null;

            Assert.AreEqual(gcb.TryGetValue(0, out value), true);
            Assert.AreEqual(value, "root");
            Assert.AreEqual(gcb.TryGetValue(1, out value), true);
            Assert.AreEqual(value, "subG");
            value = null;
            Assert.AreEqual(gcb.TryGetValue(2, out value), false);
            Assert.IsNull(value);
        }

        /// <summary>
        /// Tests the AddNode method.
        /// </summary>
        [TestMethod]
        public void AddNodeTest()
        {
            GraphCompositeBuilder<int, string> gcb = new GraphCompositeBuilder<int, string>(EqualityComparer<int>.Default, 0, "root");
            gcb.AddNode(0, 1, "subG", true);
            gcb.AddNode(1, 2, "subG2", false);
            gcb.AddNode(1, 3, "G3", false);
            GraphComposite<int, string> gc = gcb.GenerateCopy();

            Assert.AreEqual(gc.Subgraph.Count, 1);
            Assert.AreEqual(gc.Subgraph[0].Subgraph.Count, 2);
        }

        /// <summary>
        /// Test for the AddEdge method.
        /// </summary>
        [TestMethod]
        public void AddEdgeTest()
        {
            GraphCompositeBuilder<int, string> gcb = new GraphCompositeBuilder<int, string>(EqualityComparer<int>.Default, 0, "root");
            gcb.AddNode(0, 1, "C1", false);
            gcb.AddNode(0, 2, "C2", false);
            gcb.AddEdge(1, 2);
            GraphComposite<int, string> gc = gcb.GenerateCopy();
            Assert.AreNotEqual(gc.Subgraph.First(x => x.Key == 1).Outgoing[0], null);
            Assert.AreNotEqual(gc.Subgraph.First(x => x.Key == 2).Incoming[0], null);
            Assert.AreEqual(gc.Subgraph.First(x => x.Key == 1).Outgoing.Count, 1);
            Assert.AreEqual(gc.Subgraph.First(x => x.Key == 2).Incoming.Count, 1);
            Assert.AreEqual(gc.Subgraph.First(x => x.Key == 1).Incoming.Count, 0);
            Assert.AreEqual(gc.Subgraph.First(x => x.Key == 2).Outgoing.Count, 0);
        }

        /// <summary>
        /// Test for the GenerateCopy method.
        /// </summary>
        [TestMethod]
        public void GenerateCopyTest()
        {
            GraphCompositeBuilder<int, string> gcb = new GraphCompositeBuilder<int, string>(EqualityComparer<int>.Default, 0, "root");
            gcb.AddNode(0, 1, "C1", true);
            gcb.AddNode(0, 2, "C2", false);
            gcb.AddNode(1, 3, "C1a", true);
            gcb.AddNode(1, 4, "C3", false);
            gcb.AddNode(3, 5, "C1a1", false);
            gcb.AddEdge(1, 2);
            gcb.AddEdge(1, 3);
            gcb.AddEdge(4, 5);

            GraphComposite<int, string> gc = gcb.GenerateCopy();

            Assert.AreEqual(gc.Subgraph.Count, 2);
            GraphComposite<int, string> c1 = gc.Subgraph.First(x => x.Key == 1);
            GraphComposite<int, string> c2 = gc.Subgraph.First(x => x.Key == 2);
            Assert.AreEqual(c1.Subgraph.Count, 2);
            Assert.AreEqual(c2.Subgraph, null);
            GraphComposite<int, string> c1a = c1.Subgraph.First(x => x.Key == 3);
            GraphComposite<int, string> c3 = c1.Subgraph.First(x => x.Key == 4);
            Assert.AreEqual(c1a.Subgraph.Count, 1);
            Assert.AreEqual(c3.Subgraph, null);
            GraphComposite<int, string> seeOneA1 = c1a.Subgraph.First(x => x.Key == 5);
            Assert.AreEqual(c1.Outgoing.Count, 2);
            Assert.ReferenceEquals(c1.Outgoing.First(x => x.Key == 2), c2);
            Assert.ReferenceEquals(c1.Outgoing.First(x => x.Key == 3), c3);
            Assert.AreEqual(c3.Outgoing.Count, 1);
            Assert.ReferenceEquals(c3.Outgoing.First(x => x.Key == 5), seeOneA1);
            Assert.AreEqual(c2.Incoming.Count, 1);
            Assert.ReferenceEquals(c2.Incoming[0], c1);
            Assert.AreEqual(c1a.Incoming.Count, 1);
            Assert.ReferenceEquals(c1a.Incoming[0], c1);
            Assert.AreEqual(seeOneA1.Incoming.Count, 1);
            Assert.ReferenceEquals(seeOneA1.Incoming[0], c1);
        }
    }
}
