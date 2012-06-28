//-----------------------------------------------------------------------
// <copyright file="ConvertFromXElementToGraph.cs" company="Fluxtree Technologies LLC.">
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
namespace GraphComposite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Converts from any XElement into a GaphComposite.
    /// The keys are of type int, and assigned incrementally to nodes as they are added.
    /// The values are of type string, and correspond to the name of the element.
    /// </summary>
    public class ConvertFromXElementToGraph : ConvertToGraph<XElement, int, string>
    {
        /// <summary>
        /// Current key. This is the next key that will be given to a new node as the graph is built.
        /// </summary>
        private int currentKey = 1;

        /// <summary>
        /// Dictionary of elements that have already been assigned keys (indexed by HashCode) and their keys.
        /// </summary>
        private Dictionary<int, int> keyMapping = new Dictionary<int, int>();

        /// <summary>
        /// Initializes a new instance of the ConvertFromXElementToGraph class.
        /// </summary>
        /// <param name="comp">Comparer for nodes.</param>
        public ConvertFromXElementToGraph(IEqualityComparer<int> comp) : base(comp)
        {
        }

        /// <summary>
        /// Prevents a default instance of the ConvertFromXElementToGraph class from being created.
        /// </summary>
        private ConvertFromXElementToGraph()
        {
        }

        /// <summary>
        /// Override for ConvertToGraph.Key.
        /// </summary>
        /// <param name="x">XElement to make or return the key for.</param>
        /// <returns>Key. An integer that is incremented each time a new node is created.</returns>
        protected override int GetKey(XElement x)
        {
            int existingKey = default(int);
            if (this.keyMapping.TryGetValue(x.GetHashCode(), out existingKey))
            {
                return existingKey;
            }
            else
            {
                this.keyMapping.Add(x.GetHashCode(), this.currentKey);
                return this.currentKey++;
            }
        }

        /// <summary>
        /// Override of ConvertToGraph.Value.
        /// </summary>
        /// <param name="x">XElement to make the value from.</param>
        /// <returns>Returns the name of the XElement.</returns>
        protected override string GetValue(XElement x)
        {
            return x.Name.ToString();
        }

        /// <summary>
        /// Override of ConvertToGraph.Elements.
        /// </summary>
        /// <param name="x">XElement to get the elements from.</param>
        /// <returns>The child elements of the XElement.</returns>
        protected override IEnumerable<XElement> Elements(XElement x)
        {
            return x.Elements();
        }

        /// <summary>
        /// Override of ConvertToGraph.OutgoingEdges.
        /// </summary>
        /// <param name="x">XElement to get the target nodes of.</param>
        /// <returns>List of XElements that are targets of the outgoing edges.</returns>
        protected override IEnumerable<XElement> OutgoingEdges(XElement x)
        {
            return new List<XElement>();
        }
    }
}
