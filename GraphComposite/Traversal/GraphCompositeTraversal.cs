//-----------------------------------------------------------------------
// <copyright file="GraphCompositeTraversal.cs" company="Fluxtree Technologies LLC.">
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

    /// <summary>
    /// Utility function for processing GraphComposites.
    /// </summary>
    /// <typeparam name="Tkey">Tkey parameter of the GraphComposite</typeparam>
    /// <typeparam name="Tval">Tval parameter of the GraphComposite</typeparam>
    public class GraphCompositeTraversal<Tkey, Tval>
    {
        /// <summary>
        /// Perform actions on a GraphComposite starting at the root node and going down through the subgraphs.
        /// </summary>
        /// <param name="c">GraphComposite to process.</param>
        /// <param name="onSelf">Action to perform at each subgraph.</param>
        /// <param name="onGraphEntry">Action to perform on entry to each subgraph.</param>
        /// <param name="onGraphExit">Action to perform on exit from each subgraph.</param>
        public static void TraverseSubgraphs(
            GraphComposite<Tkey, Tval> c,
            Action<GraphComposite<Tkey, Tval>> onSelf,
            Action<GraphComposite<Tkey, Tval>> onGraphEntry,
            Action<GraphComposite<Tkey, Tval>> onGraphExit)
        {
            {
                if (onSelf != null)
                {
                    onSelf.Invoke(c);
                }

                if (c.IsGraph)
                {
                    if (onGraphEntry != null)
                    {
                        onGraphEntry.Invoke(c);
                    }

                    foreach (GraphComposite<Tkey, Tval> subC in c.Subgraph)
                    {
                        TraverseSubgraphs(subC, onSelf, onGraphEntry, onGraphExit);
                    }

                    if (onGraphExit != null)
                    {
                        onGraphExit.Invoke(c);
                    }
                }
            }
        }

        /// <summary>
        /// Perform actions on a GraphComposite starting at a certain node and going through all paths from that node. Stops before making cycles.
        /// </summary>
        /// <param name="startC">Start node.</param>
        /// <param name="a">Action to perform at each node in paths.</param>
        public static void PathTraverse(
            GraphComposite<Tkey, Tval> startC,
            Action<GraphComposite<Tkey, Tval>> a)
        {
            List<GraphComposite<Tkey, Tval>> l = new List<GraphComposite<Tkey, Tval>>();
            PathTraverse(startC, a, l);
        }

        /// <summary>
        /// Find first node matching some criteria on paths starting from a certain node.
        /// </summary>
        /// <param name="startC">Start node.</param>
        /// <param name="f">Function returning a boolean for whether or not given node matches criteria.</param>
        /// <returns>Returns the first match, or null if there was no match.</returns>
        public static GraphComposite<Tkey, Tval> FindFirstOnPaths(
            GraphComposite<Tkey, Tval> startC,
            Func<GraphComposite<Tkey, Tval>, bool> f)
        {
            if (f.Invoke(startC))
            {
                return startC;
            }
            else
            {
                foreach (GraphComposite<Tkey, Tval> c in startC.Outgoing)
                {
                    GraphComposite<Tkey, Tval> result = FindFirstOnPaths(c, f);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            // if we got here, this vertex and paths from it didn't satisfy
            return null;
        }

        /// <summary>
        /// Helper function for the public PathTraverse method.
        /// </summary>
        /// <param name="startC">Starting GraphComposite in the traversal.</param>
        /// <param name="a">Action to perform on each GraphComposite in the path.</param>
        /// <param name="l">List of GraphComposites that have already been visited in the overall traversal.</param>
        private static void PathTraverse(
            GraphComposite<Tkey, Tval> startC,
            Action<GraphComposite<Tkey, Tval>> a,
            List<GraphComposite<Tkey, Tval>> l)
        {
            // cycle - go back
            if (l.Contains(startC))
            {
                return;
            }
            else
            {
                l.Add(startC);
            }

            a.Invoke(startC);

            foreach (GraphComposite<Tkey, Tval> c in startC.Outgoing)
            {
                PathTraverse(c, a, l);
            }
        }
    }
}
