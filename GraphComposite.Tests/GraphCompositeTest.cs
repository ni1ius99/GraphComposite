//-----------------------------------------------------------------------
// <copyright file="GraphCompositeTest.cs" company="Fluxtree Technologies LLC.">
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
    /// GraphComposite unit test
    /// </summary>
    [TestClass]
    public class GraphCompositeTest
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [TestMethod]
        public void GraphCompositeConstructorTest()
        {
            // Construct a GraphComposite.
            GraphComposite<int, string> c = new GraphComposite<int, string>(1, "constructed", true, null);
        }

        /// <summary>
        /// Tests the Add method.
        /// </summary>
        [TestMethod]
        public void GraphCompositeAddTest()
        {
            GraphComposite<int, string> c = new GraphComposite<int, string>(1, "C", true, null);

            // Add a node to the root.
            GraphComposite<int, string> subC = new GraphComposite<int, string>(2, "subC", false, c);
            c.Subgraph.Add(subC);

            Assert.IsTrue(c.Subgraph.Contains(subC));
        }

        /// <summary>
        /// Tests the IsGraph method.
        /// </summary>
        [TestMethod]
        public void GraphCompositeIsGraphTest()
        {
            GraphComposite<int, string> c = new GraphComposite<int, string>(1, "C", true, null);

            GraphComposite<int, string> c2 = new GraphComposite<int, string>(2, "C2", false, null);

            Assert.IsTrue(c.IsGraph);
            Assert.IsFalse(c2.IsGraph);
        }

        /// <summary>
        /// Tests the Key method.
        /// </summary>
        [TestMethod]
        public void GraphCompositeKeyTest()
        {
            GraphComposite<int, string> c = new GraphComposite<int, string>(1, "C", true, null);
            GraphComposite<int, string> c2 = new GraphComposite<int, string>(2, "C2", false, null);
            Assert.AreEqual(c.Key, 1);
            Assert.AreEqual(c2.Key, 2);
        }

        /// <summary>
        /// Tests the Value method.
        /// </summary>
        [TestMethod]
        public void GraphCompositeValueTest()
        {
            GraphComposite<int, string> c = new GraphComposite<int, string>(1, "C", true, null);
            GraphComposite<int, string> c2 = new GraphComposite<int, string>(2, "C2", false, null);

            Assert.AreEqual(c.Value, "C");
            Assert.AreEqual(c2.Value, "C2");
        }

        /// <summary>
        /// Test for the Outoing and Incoming methods.
        /// </summary>
        [TestMethod]
        public void GraphCompositeIncomingOutgoingTest()
        {
            GraphComposite<int, string> c = new GraphComposite<int, string>(1, "C", true, null);
            GraphComposite<int, string> c2 = new GraphComposite<int, string>(2, "C2", false, null);

            c.Outgoing.Add(c2);
            c2.Incoming.Add(c);

            Assert.IsTrue(c.Outgoing.Contains(c2));
            Assert.IsTrue(c2.Incoming.Contains(c));
        }

        /// <summary>
        /// Test for the Subgraph method.
        /// </summary>
        [TestMethod]
        public void GraphCompositeSubGraphTest()
        {
            GraphComposite<int, string> c = new GraphComposite<int, string>(1, "C", true, null);
            GraphComposite<int, string> c2 = new GraphComposite<int, string>(2, "C2", false, null);
            c.Subgraph.Add(c2);

            Assert.ReferenceEquals(c2, c.Subgraph);
        }
    }
}
