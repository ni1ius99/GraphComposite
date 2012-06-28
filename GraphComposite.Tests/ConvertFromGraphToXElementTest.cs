//-----------------------------------------------------------------------
// <copyright file="ConvertFromGraphToXElementTest.cs" company="Fluxtree Technologies LLC.">
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
    /// Test class for ConvertFromGraphToXElement class.
    /// </summary>
    [TestClass]
    public class ConvertFromGraphToXElementTest
    {
        /// <summary>
        /// Test for the Convert method.
        /// </summary>
        [TestMethod]
        public void ConvertTest()
        {
            ConvertFromGraphToXElement<string, string> converter = new ConvertFromGraphToXElement<string, string>();
            GraphCompositeBuilder<string, string> gc = new GraphCompositeBuilder<string, string>(EqualityComparer<string>.Default, "a", "root");
            gc.AddNode("a", "a1", "apple", true);
            gc.AddNode("a", "a2", "orange", true);
            gc.AddNode("a1", "a11", "pear", true);
            gc.AddNode("a2", "a21", "potato", true);
            gc.AddEdge("a1", "a2");
            gc.AddEdge("a1", "a11");
            gc.AddEdge("a2", "a21");
            gc.AddEdge("a11", "a21");

            XElement result = converter.Convert(gc.GenerateCopy());
            Assert.AreEqual(result.Descendants().Count(), 4);
        }
    }
}
