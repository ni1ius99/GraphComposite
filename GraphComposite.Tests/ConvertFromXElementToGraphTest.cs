//-----------------------------------------------------------------------
// <copyright file="ConvertFromXElementToGraphTest.cs" company="Fluxtree Technologies LLC.">
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
    /// Test class for ConvertFromXElementToGraph class.
    /// </summary>
    [TestClass]
    public class ConvertFromXElementToGraphTest
    {
        /// <summary>
        /// Test for the Convert method.
        /// </summary>
        [TestMethod]
        public void ConvertTest()
        {
            ConvertFromXElementToGraph conv = new ConvertFromXElementToGraph(EqualityComparer<int>.Default);
            XElement el = new XElement("TheRoot");
            el.Add(new XElement("apple"));
            XElement triangle = new XElement("triangle");
            triangle.Add(new XElement("shoe"));
            triangle.Add(new XElement("boot"));
            el.Add(triangle);
            GraphComposite<int, string> gc = conv.Convert(el);
            Assert.AreEqual(gc.IsGraph, true);
            Assert.AreEqual(gc.Key, 1);
            Assert.AreEqual(gc.Value, "TheRoot");
            Assert.AreEqual(gc.Subgraph.Count, 2);
            Assert.AreEqual(gc.Subgraph[0].IsGraph, false);
            Assert.AreEqual(gc.Subgraph[0].Value, "apple");
            Assert.AreEqual(gc.Subgraph[1].IsGraph, true);
            Assert.AreEqual(gc.Subgraph[1].Value, "triangle");
            Assert.AreEqual(gc.Subgraph[1].Subgraph.Count, 2);
        }
    }
}
