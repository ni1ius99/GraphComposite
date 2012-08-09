//-----------------------------------------------------------------------
// <copyright file="GraphCompositeBuilderConfiguration.cs" company="Fluxtree Technologies LLC.">
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
    /// Represents the configuration for a GraphCompositeBuilder
    /// </summary>
    public class GraphCompositeBuilderConfiguration
    {
        /// <summary>
        /// Whether or not the graph can contain subgraphs, or is flat.
        /// </summary>
        private bool isHierarchical = true;

        /// <summary>
        /// Whether or not there can be cycles.
        /// </summary>
        private bool isAcyclic = true;

        /// <summary>
        /// Initializes a new instance of the GraphCompositeBuilderConfiguration class.
        /// </summary>
        public GraphCompositeBuilderConfiguration()
        {
            this.isHierarchical = true;
            this.isAcyclic = true;
        }

        /// <summary>
        /// Initializes a new instance of the GraphCompositeBuilderConfiguration class.
        /// </summary>
        /// <param name="hierarchical">whether or not there can be subgraphs</param>
        /// <param name="acyclic">whether or not there can be cycles</param>
        public GraphCompositeBuilderConfiguration(bool hierarchical, bool acyclic)
        {
            this.isHierarchical = hierarchical;
            this.isAcyclic = acyclic;
        }

        /// <summary>
        /// Gets a value indicating whether or not the graphcomposite is hierarchical, i.e. contains subgraphs.
        /// </summary>
        public bool Hierarchical
        {
            get
            {
                return this.isHierarchical;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the graph can contain cycles.
        /// </summary>
        public bool Acyclic
        {
            get
            {
                return this.isAcyclic;
            }
        }
    }
}
