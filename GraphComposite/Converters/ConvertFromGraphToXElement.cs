//-----------------------------------------------------------------------
// <copyright file="ConvertFromGraphToXElement.cs" company="Fluxtree Technologies LLC.">
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
    /// Converts from GraphComposite to XElement structure. XML Elements are named after their key in string form.
    /// </summary>
    /// <typeparam name="Tkey">Tkey parameter of GraphComposite</typeparam>
    /// <typeparam name="Tval">Tval parameter of GraphComposite</typeparam>
    public class ConvertFromGraphToXElement<Tkey, Tval> : ConvertFromGraph<XElement, Tkey, Tval>
    {
        /// <summary>
        /// Override of ConvertFromGraph.MakeInstance
        /// </summary>
        /// <param name="key">Key of a node.</param>
        /// <param name="val">Value of the same node.</param>
        /// <returns>The new element.</returns>
        protected override XElement MakeInstance(Tkey key, Tval val)
        {
            return new XElement(key.ToString(), val);
        }

        /// <summary>
        /// Override of ConvertFromGraph.AddChild
        /// </summary>
        /// <param name="parent">Parent XElement.</param>
        /// <param name="child">Child XElement.</param>
        protected override void AddChild(XElement parent, XElement child)
        {
            parent.Add(child);
        }

        /// <summary>
        /// Override of ConvertFromGraph.GetParent.
        /// </summary>
        /// <param name="child">child XElement to get parent of.</param>
        /// <returns>child XElement of the parent.</returns>
        protected override XElement GetParent(XElement child)
        {
            return child.Parent;
        }

        /// <summary>
        /// Override of Convert.GetChild.
        /// </summary>
        /// <param name="parent">Parent to get the child of.</param>
        /// <param name="key">Key of the child to get.</param>
        /// <returns>Child of the parent.</returns>
        protected override XElement GetChild(XElement parent, Tkey key)
        {
            return parent.Element(key.ToString());
        }

        /// <summary>
        /// Override of ConvertFromGraph.OnOutgoingEdge.
        /// </summary>
        /// <param name="sourceKey">Key of the node that is the edge source.</param>
        /// <param name="targetKey">Key of the node that is the edge target.</param>
        protected override void OnEdge(Tkey sourceKey, Tkey targetKey)
        {
            return;
        }
    }
}
