//-----------------------------------------------------------------------
// <copyright file="GraphComposite.cs" company="Fluxtree Technologies LLC.">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a hierarchical graph.
    /// </summary>
    /// <typeparam name="Tkey">Type used for unique identification of nodes.</typeparam>
    /// <typeparam name="Tval">Type used for values at each node.</typeparam>
    public class GraphComposite<Tkey, Tval>
    {
        /// <summary>
        /// Subgraph of the current GraphComposite, if it exists, otherwise it is null.
        /// </summary>
        private List<GraphComposite<Tkey, Tval>> subgraph = null;

        /// <summary>
        /// Parent of this GraphComposite. The value is null if this GraphComposite is the root.
        /// </summary>
        private GraphComposite<Tkey, Tval> parent = null;

        /// <summary>
        /// Key for the GraphComposite.
        /// </summary>
        private Tkey key;

        /// <summary>
        /// Outgoing edges from the GraphComposite.
        /// </summary>
        private List<GraphComposite<Tkey, Tval>> outgoing = new List<GraphComposite<Tkey, Tval>>();

        /// <summary>
        /// Incoming edges to the GraphComposite.
        /// </summary>
        private List<GraphComposite<Tkey, Tval>> incoming = new List<GraphComposite<Tkey, Tval>>();

        /// <summary>
        /// "Value" associated with the GraphComposite.
        /// </summary>
        private Tval value;

        /// <summary>
        /// Initializes a new instance of the GraphComposite class.
        /// </summary>
        /// <param name="key">Value of key. Must be unique.</param>
        /// <param name="value">Value at this node or subgraph.</param>
        /// <param name="isGraph">Sets whether or not this node contains a subgraph.</param>
        /// <param name="parent">Reference to the parent graph. Can be null.</param>
        public GraphComposite(Tkey key, Tval value, bool isGraph, GraphComposite<Tkey, Tval> parent)
        {
            if (isGraph)
            {
                this.subgraph = new List<GraphComposite<Tkey, Tval>>();
            }

            this.parent = parent;
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Prevents a default instance of the GraphComposite class from being created.
        /// </summary>
        private GraphComposite()
        {
        }

        #region properties
        /// <summary>
        /// Gets the list of outgoing edges.
        /// </summary>
        public List<GraphComposite<Tkey, Tval>> Outgoing
        {
            get
            {
                return this.outgoing;
            }
        }

        /// <summary>
        /// Gets the list of incoming edges.
        /// </summary>
        public List<GraphComposite<Tkey, Tval>> Incoming
        {
            get
            {
                return this.incoming;
            }
        }

        /// <summary>
        /// Gets the value at node.
        /// </summary>
        public Tval Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Gets the subgraph of this node. Returns null if there is no subgraph.
        /// </summary>
        public List<GraphComposite<Tkey, Tval>> Subgraph
        {
            get
            {
                return this.subgraph;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this node contains a subgraph or is a leaf.
        /// </summary>
        public bool IsGraph
        {
            get
            {
                return this.subgraph != null;
            }
        }

        /// <summary>
        /// Gets the unique identifier for the node.
        /// </summary>
        public Tkey Key
        {
            get
            {
                return this.key;
            }
        }

        /// <summary>
        /// Gets the reference to the parent graph. Returns null if there is no parent.
        /// </summary>
        public GraphComposite<Tkey, Tval> Parent
        {
            get
            {
                return this.parent;
            }
        }
        #endregion

        /// <summary>
        /// Returns XML representation of the graph. Elements are named after the node keys, and node values are converted into text contents of the elements.
        /// Appends an underscore before all element names, because XML names beginning with a numeric are not allowed.
        /// </summary>
        /// <returns>The XEement representation of the graph.</returns>
        public XElement AsXElement()
        {
            XElement result = null;
            XElement currentParent = null;

            Action<GraphComposite<Tkey, Tval>> onSelf =
                thisC =>
                {
                    if (currentParent != null)
                    {
                        currentParent.Add(new XElement("_" + thisC.Key.ToString(), thisC.Value));
                    }
                    else
                    {
                        currentParent = new XElement("_" + thisC.Key.ToString(), thisC.Value);
                        result = currentParent;
                    }
                };

            Action<GraphComposite<Tkey, Tval>> onEntry =
                thisC =>
                {
                    currentParent = currentParent.Element("_" + thisC.Key.ToString());
                };

            Action<GraphComposite<Tkey, Tval>> onExit =
                thisC =>
                {
                    currentParent = currentParent.Parent;
                };

            GraphCompositeTraversal<Tkey, Tval>.TraverseSubgraphs(this, onSelf, onEntry, onExit);

            return result;
        }
    }
}
