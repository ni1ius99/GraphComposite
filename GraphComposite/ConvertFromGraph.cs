//-----------------------------------------------------------------------
// <copyright file="ConvertFromGraph.cs" company="Fluxtree Technologies LLC.">
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
    /// Abstract class for converting from a GraphComposite into another type.
    /// </summary>
    /// <typeparam name="T">Type to convert into.</typeparam>
    /// <typeparam name="Tkey">Tkey parameter of the GraphComposite</typeparam>
    /// <typeparam name="Tval">Tval parameter of the GraphCompsite</typeparam>
    public abstract class ConvertFromGraph<T, Tkey, Tval>
    {
        /// <summary>
        /// Performs the conversion of a GraphComposite into a T.
        /// </summary>
        /// <param name="g">GraphComposite to convert</param>
        /// <returns>The converted instance of type T.</returns>
        public T Convert(GraphComposite<Tkey, Tval> g)
        {
            T result = default(T);
            T currentParent = default(T);

            Action<GraphComposite<Tkey, Tval>> onSelf =
                thisC =>
                {
                    if (currentParent != null)
                    {
                        AddChild(currentParent, MakeInstance(thisC.Key, thisC.Value));
                    }

                    foreach (GraphComposite<Tkey, Tval> g2 in thisC.Outgoing)
                    {
                        OnOutgoingEdge(g2.Key);
                    }

                    foreach (GraphComposite<Tkey, Tval> g2 in thisC.Incoming)
                    {
                        OnIncomingEdge(g2.Key);
                    }
                };

            Action<GraphComposite<Tkey, Tval>> onEntry =
                thisC =>
                {
                    if (currentParent == null)
                    {
                        currentParent = MakeInstance(thisC.Key, thisC.Value);
                        result = currentParent;
                    }
                    else
                    {
                        currentParent = GetChild(currentParent, thisC.Key);
                    }
                };

            Action<GraphComposite<Tkey, Tval>> onExit =
                thisC =>
                {
                    currentParent = GetParent(currentParent);
                };

            GraphCompositeTraversal<Tkey, Tval>.TraverseSubgraphs(g, onSelf, onEntry, onExit);

            return result;
        }

        /// <summary>
        /// Returns an instance of type T given a key and value associated with a graph node.
        /// TBD: this should probably just take a GraphComposite which has the key and value.
        /// </summary>
        /// <param name="key">Key of a node.</param>
        /// <param name="val">Value of the same node.</param>
        /// <returns>The instance of type T.</returns>
        protected abstract T MakeInstance(Tkey key, Tval val);

        /// <summary>
        /// Action to perform on a two instances of type T to make one the parent and the other the child.
        /// </summary>
        /// <param name="parent">Parent instance.</param>
        /// <param name="child">Child instance.</param>
        protected abstract void AddChild(T parent, T child);

        /// <summary>
        /// Returns the parent of a T.
        /// </summary>
        /// <param name="child">Child T to get the parent from.</param>
        /// <returns>Parent of the given child.</returns>
        protected abstract T GetParent(T child);

        /// <summary>
        /// Returns the child of a T.
        /// </summary>
        /// <param name="parent">Parent T to get the child from.</param>
        /// <param name="key">Key to look up the child.</param>
        /// <returns>Child of the given parent.</returns>
        protected abstract T GetChild(T parent, Tkey key);

        /// <summary>
        /// Action to perform on a T corresponding to a node for each incoming edge.
        /// </summary>
        /// <param name="key">Key for node.</param>
        protected abstract void OnIncomingEdge(Tkey key);

        /// <summary>
        /// Action to perform on a T corresponding to a node for each outgoing edge.
        /// </summary>
        /// <param name="key">Key for node.</param>
        protected abstract void OnOutgoingEdge(Tkey key);
    }
}
