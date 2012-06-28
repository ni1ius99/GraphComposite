//-----------------------------------------------------------------------
// <copyright file="AddToLeafGraphExceptionTest.cs" company="Fluxtree Technologies LLC.">
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test class for AddToLeafGraphException class.
    /// </summary>
    [TestClass]
    public class AddToLeafGraphExceptionTest
    {
        /// <summary>
        /// General test method for AddToLeafGraphException class.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AddToLeafGraphException))]
        public void AddToLeafGraphExceptionThrowTest()
        {
            GraphCompositeBuilder<int, string> gcb = new GraphCompositeBuilder<int, string>(EqualityComparer<int>.Default, 0, "root");
            gcb.AddNode(0, 1, "C1", false);
            gcb.AddNode(1, 2, "C2", false);
        }
    }
}
