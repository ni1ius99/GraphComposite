//-----------------------------------------------------------------------
// <copyright file="GraphCompositeBuilder.cs" company="Fluxtree Technologies LLC.">
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
    using GraphComposite.Exceptions;

    /// <summary>
    /// Interface class for GraphComposites. Controls modification of the GraphComposite.
    /// </summary>
    /// <typeparam name="Tkey">Tkey of the GraphComposite.</typeparam>
    /// <typeparam name="Tval">Tval of the GraphComposite.</typeparam>
    public class GraphCompositeBuilder<Tkey, Tval>
    {
        /// <summary>
        /// Configuration of the builder.
        /// </summary>
        private GraphCompositeBuilderConfiguration config = new GraphCompositeBuilderConfiguration();

        /// <summary>
        /// The root GraphComposite.
        /// </summary>
        private GraphComposite<Tkey, Tval> root;

        /// <summary>
        /// A flat collection of the GraphComposites indexed by key.
        /// </summary>
        private Dictionary<Tkey, GraphComposite<Tkey, Tval>> allItems;

        /// <summary>
        /// A flat collection of all the edges.
        /// </summary>
        private List<KeyValuePair<Tkey, Tkey>> allEdges = new List<KeyValuePair<Tkey, Tkey>>();

        /// <summary>
        /// Initializes a new instance of the GraphCompositeBuilder class.
        /// </summary>
        /// <param name="comparer">Comparer for nodes.</param>
        /// <param name="rootKey">Key of the root node.</param>
        /// <param name="rootVal">Value of the root node.</param>
        public GraphCompositeBuilder(IEqualityComparer<Tkey> comparer, Tkey rootKey, Tval rootVal)
        {
            this.allItems = new Dictionary<Tkey, GraphComposite<Tkey, Tval>>(comparer);
            this.root = new GraphComposite<Tkey, Tval>(rootKey, rootVal, true, null);
            this.allItems.Add(rootKey, this.root);
        }

        /// <summary>
        /// Initializes a new instance of the GraphCompositeBuilder class.
        /// </summary>
        /// <param name="cfg">Configuration the builder will use.</param>
        /// <param name="comparer">Comparer for nodes.</param>
        /// <param name="rootKey">Key of the root node.</param>
        /// <param name="rootVal">Value of the root node.</param>
        public GraphCompositeBuilder(GraphCompositeBuilderConfiguration cfg, IEqualityComparer<Tkey> comparer, Tkey rootKey, Tval rootVal)
            : this(comparer, rootKey, rootVal)
        {
            this.config = cfg;
        }

        /// <summary>
        /// Gets the configuration of the builder.
        /// </summary>
        public GraphCompositeBuilderConfiguration Configuration
        {
            get
            {
                return this.config;
            }
        }

        /// <summary>
        /// Try to get the value of the node with given key.
        /// </summary>
        /// <param name="key">Key of the node to retrieve the value of.</param>
        /// <param name="value">Out parameter for the value.</param>
        /// <returns>True if the node is found, otherwise false.</returns>
        public bool TryGetValue(Tkey key, out Tval value)
        {
            GraphComposite<Tkey, Tval> c;
            if (!this.allItems.TryGetValue(key, out c))
            {
                value = default(Tval);
                return false;
            }
            else
            {
                value = c.Value;
                return true;
            }
        }

        /// <summary>
        /// Adds a node to the graph.
        /// </summary>
        /// <param name="containingGraphKey">Key of the graph that the new node goes into.</param>
        /// <param name="thisKey">Key of the new node.</param>
        /// <param name="value">Value in the new node.</param>
        /// <param name="isGraph">If true, put an empty subgraph in the node, otherwise it is a leaf node.</param>
        public void AddNode(Tkey containingGraphKey, Tkey thisKey, Tval value, bool isGraph)
        {
            GraphComposite<Tkey, Tval> containingGraph;
            if (!this.allItems.TryGetValue(containingGraphKey, out containingGraph))
            {
                throw new KeyNotFoundGraphException("containing graph " + containingGraphKey.ToString() + " not found");
            }
            else if (!containingGraph.IsGraph)
            {
                throw new AddToLeafGraphException(containingGraphKey.ToString() + " is not a graph");
            }
            else if (!this.config.Hierarchical)
            {
                if (isGraph)
                {
                    throw new SubgraphAttemptedException("Attempt to add a subgraph to a non-hierarchical graph");
                }
            }

            if (this.allItems.ContainsKey(thisKey))
            {
                throw new AddDuplicateGraphException(thisKey.ToString() + " already a member");
            }
            else
            {
                GraphComposite<Tkey, Tval> c = new GraphComposite<Tkey, Tval>(thisKey, value, isGraph, containingGraph);
                containingGraph.Subgraph.Add(c);
                this.allItems.Add(thisKey, c);
            }
        }

        // TBD - AddSubgraph: adds a subgraph to a node that originally did not have a subgraph.

        // TBD - generate replicant along with original, to avoid having to do a copy later (for large graphs).

        /// <summary>
        /// Adds an edge to the graph.
        /// </summary>
        /// <param name="key1">Key of source node.</param>
        /// <param name="key2">Key of target node.</param>
        public void AddEdge(Tkey key1, Tkey key2)
        {
            GraphComposite<Tkey, Tval> val1, val2;

            if (!this.allItems.TryGetValue(key1, out val1))
            {
                throw new KeyNotFoundGraphException(key1.ToString() + " not found");
            }
            else if (!this.allItems.TryGetValue(key2, out val2))
            {
                throw new KeyNotFoundGraphException(key2.ToString() + " not found");
            }
            else if ((val1 == this.root) || (val2 == this.root))
            {
                throw new AddEdgeToRootGraphException("attempt to connect edge to root");
            }
            else
            {
                if (val1.Outgoing.Contains(val2))
                {
                    throw new AddDuplicateGraphException("duplicate edge attempt from " + key1.ToString() + " to " + key2.ToString());
                }
                else if (val2.Incoming.Contains(val2))
                {
                    throw new AddDuplicateGraphException("duplicate edge attempt from " + key1.ToString() + " to " + key2.ToString());
                }

                if (this.config.Acyclic)
                {
                    // check for cycles
                    Func<GraphComposite<Tkey, Tval>, bool> match =
                        c => { return allItems.Comparer.Equals(c.Key, val1.Key); };
                    if (GraphCompositeTraversal<Tkey, Tval>.FindFirstOnPaths(val2, match) != null)
                    {
                        throw new CycleAttemptedException("edge between " + key1.ToString() + " and " + key2.ToString());
                    }
                }

                val1.Outgoing.Add(val2);
                val2.Incoming.Add(val1);
                this.allEdges.Add(new KeyValuePair<Tkey, Tkey>(key1, key2));
            }
        }

        /// <summary>
        /// Returns a copy of the graph. This is the only way to get the graph owned by the builder.
        /// </summary>
        /// <returns>The copy of the graph.</returns>
        public GraphComposite<Tkey, Tval> GenerateCopy()
        {
            GraphComposite<Tkey, Tval> result = null;
            GraphComposite<Tkey, Tval> currentParent = null;
            GraphComposite<Tkey, Tval> latest = null;
            Dictionary<Tkey, GraphComposite<Tkey, Tval>> allCopiedItems = new Dictionary<Tkey, GraphComposite<Tkey, Tval>>();

            Action<GraphComposite<Tkey, Tval>> onSelf = c =>
            {
                latest = new GraphComposite<Tkey, Tval>(c.Key, c.Value, c.IsGraph, currentParent);
                if (latest.Parent != null)
                {
                    latest.Parent.Subgraph.Add(latest);
                }
                else
                {
                    result = latest; // root
                }

                allCopiedItems.Add(latest.Key, latest);
            };

            Action<GraphComposite<Tkey, Tval>> onGraphEntry = c =>
            {
                currentParent = latest;
            };

            Action<GraphComposite<Tkey, Tval>> onGraphExit = c =>
            {
                currentParent = currentParent.Parent;
            };

            GraphCompositeTraversal<Tkey, Tval>.TraverseSubgraphs(this.root, onSelf, onGraphEntry, onGraphExit);

            foreach (KeyValuePair<Tkey, Tkey> p in this.allEdges)
            {
                allCopiedItems[p.Key].Outgoing.Add(allCopiedItems[p.Value]);
                allCopiedItems[p.Value].Incoming.Add(allCopiedItems[p.Key]);
            }

            return result;
        }
    }
}
