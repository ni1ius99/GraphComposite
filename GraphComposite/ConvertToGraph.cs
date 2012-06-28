//-----------------------------------------------------------------------
// <copyright file="ConvertToGraph.cs" company="Fluxtree Technologies LLC.">
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
    /// Abstract class for converting from any type to a GraphComposite.
    /// </summary>
    /// <typeparam name="T">Type to convert to a GraphComposite.</typeparam>
    /// <typeparam name="Tkey">Key type for the GraphComposite.</typeparam>
    /// <typeparam name="Tval">Value type for the GraphComposite.</typeparam>
    public abstract class ConvertToGraph<T, Tkey, Tval>
    {
        /// <summary>
        /// Internal comparer for comparing key values. Needed to construct the GraphCompositeBuilder.
        /// </summary>
        private IEqualityComparer<Tkey> comparer = null;

        /// <summary>
        /// Initializes a new instance of the ConvertToGraph class.
        /// </summary>
        protected ConvertToGraph()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConvertToGraph class.
        /// </summary>
        /// <param name="comp">Comparer for nodes.</param>
        protected ConvertToGraph(IEqualityComparer<Tkey> comp)
        {
            this.comparer = comp;
        }

        /// <summary>
        /// Performs the conversion from a given instance of type T to a GraphComposite.
        /// </summary>
        /// <param name="source">Source instance.</param>
        /// <returns>GraphComposite representing the source.</returns>
        public GraphComposite<Tkey, Tval> Convert(T source)
        {
            Tkey sourceKey = this.GetKey(source);
            GraphCompositeBuilder<Tkey, Tval> b = new GraphCompositeBuilder<Tkey, Tval>(this.comparer, sourceKey, this.GetValue(source));

            // populate the subgraph hierarchy
            foreach (T el in this.Elements(source))
            {
                this.ProcessNode(b, sourceKey, el);
            }

            // populate the edges
            this.AddEdges(b, source, sourceKey);

            return b.GenerateCopy();
        }

        /// <summary>
        /// This function returns a key for the node representing a given instance of type T.
        /// This function must return the same key if it is called on the same element multiple times.
        /// </summary>
        /// <param name="x">Instance of type T.</param>
        /// <returns>The key representing the given instance.</returns>
        protected abstract Tkey GetKey(T x);

        /// <summary>
        /// This function returns a value for the node representing a given instance of type T.
        /// This function must return the same value if it is called on the same element multiple times.
        /// </summary>
        /// <param name="x">Instance of type T.</param>
        /// <returns>The value.</returns>
        protected abstract Tval GetValue(T x);
        
        /// <summary>
        /// This function returns an enumeration of sub-elements under a given instance of type T.
        /// </summary>
        /// <param name="x">Instance of type T.</param>
        /// <returns>The enumeration.</returns>
        protected abstract IEnumerable<T> Elements(T x);

        /// <summary>
        /// This function returns outgoing edges from a node representing a given instance of type T.
        /// </summary>
        /// <param name="x">Instance of type T.</param>
        /// <returns>The enumeration.</returns>
        protected abstract IEnumerable<T> OutgoingEdges(T x);

        /// <summary>
        /// Helper function for Convert method.
        /// </summary>
        /// <param name="builder">The GraphCompositeBuilder being converted.</param>
        /// <param name="currentParent">The parent of the current node.</param>
        /// <param name="node">The node to process.</param>
        private void ProcessNode(GraphCompositeBuilder<Tkey, Tval> builder, Tkey currentParent, T node)
        {
            List<T> list = new List<T>(this.Elements(node));

            Tkey nodeKey = this.GetKey(node);
            builder.AddNode(currentParent, nodeKey, this.GetValue(node), list.Count > 0);

            foreach (T el in list)
            {
                this.ProcessNode(builder, nodeKey, el);
            }
        }

        /// <summary>
        /// Helper function for Convert method.
        /// </summary>
        /// <param name="builder">GraphCompositeBuilder being used in the conversion process.</param>
        /// <param name="source">Source object of type T that is being converted.</param>
        /// <param name="sourceKey">GraphComposite key of the source element/node.</param>
        private void AddEdges(GraphCompositeBuilder<Tkey, Tval> builder, T source, Tkey sourceKey)
        {
            foreach (T el in this.OutgoingEdges(source))
            {
                builder.AddEdge(sourceKey, this.GetKey(el));
            }

            foreach (T el in this.Elements(source))
            {
                this.AddEdges(builder, el, this.GetKey(el));
            }
        }
    }
}
