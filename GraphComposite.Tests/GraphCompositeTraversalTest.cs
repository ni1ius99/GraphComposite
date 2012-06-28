//-----------------------------------------------------------------------
// <copyright file="GraphCompositeTraversalTest.cs" company="Fluxtree Technologies LLC.">
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
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test class for GraphCompositeTraversal class.
    /// </summary>
    [TestClass]
    public class GraphCompositeTraversalTest
    {
        /// <summary>
        /// GraphComposite to use as test data.
        /// </summary>
        private static GraphComposite<int, string> gc;

        /// <summary>
        /// Test context instance.
        /// </summary>
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        /// <summary>
        /// Test data setup.
        /// </summary>
        /// <param name="tc">the TestContext</param>
        [ClassInitialize()]
        public static void TraverSubgraphsClassInitialize(TestContext tc)
        {
            GraphCompositeBuilder<int, string> gcb = new GraphCompositeBuilder<int, string>(EqualityComparer<int>.Default, 0, "root");
            gcb.AddNode(0, 1, "apple", true);
            gcb.AddNode(1, 2, "orange", false);
            gcb.AddNode(0, 3, "kiwi", true);
            gcb.AddNode(3, 4, "potato", false);
            gcb.AddNode(3, 5, "carrot", false);
            gcb.AddEdge(1, 2);
            gcb.AddEdge(3, 4);
            gcb.AddEdge(3, 5);
            gcb.AddEdge(4, 5);
            gc = gcb.GenerateCopy();
        }

        /// <summary>
        /// Test for the TraverseSubgraphs method.
        /// </summary>
        [TestMethod]
        public void TraverseSubgraphsTest()
        {
            int numberOfEdges = 0;
            int numberOfSubgraphs = 0;
            int numberOfNodes = 0;

            GraphCompositeTraversal<int, string>.TraverseSubgraphs(
                gc,
                g => { numberOfNodes++; numberOfEdges += g.Outgoing.Count; },
                g => numberOfSubgraphs++,
                g => { });

            Assert.AreEqual(numberOfNodes, 6);
            Assert.AreEqual(numberOfEdges, 4);
            Assert.AreEqual(numberOfSubgraphs, 3);
        }

        /// <summary>
        /// Test for the PathTraverse method.
        /// </summary>
        [TestMethod]
        public void PathTraverseTest()
        {
            int visits = 0;
            GraphCompositeTraversal<int, string>.PathTraverse(gc, g => visits++);
            Assert.AreEqual(1, visits);
            visits = 0;
            GraphCompositeTraversal<int, string>.PathTraverse(gc.Subgraph[0], g => visits++); // 1 => 2
            Assert.AreEqual(2, visits);
            visits = 0;
            GraphCompositeTraversal<int, string>.PathTraverse(gc.Subgraph[1], g => visits++); // 3 => 4, 4 => 5, 3 => 5
            Assert.AreEqual(3, visits);
        }

        /// <summary>
        /// Test for the FindFirstOnPaths method.
        /// </summary>
        [TestMethod]
        public void FindFirstOnPaths()
        {
            Assert.AreEqual("potato", GraphCompositeTraversal<int, string>.FindFirstOnPaths(gc.Subgraph[1], g => { return g.Key == 4; }).Value);
            Assert.AreEqual("carrot", GraphCompositeTraversal<int, string>.FindFirstOnPaths(gc.Subgraph[1], g => { return g.Key == 5; }).Value);
            Assert.AreEqual(null, GraphCompositeTraversal<int, string>.FindFirstOnPaths(gc.Subgraph[1], g => { return g.Key == 99; }));
        }
    }
}
