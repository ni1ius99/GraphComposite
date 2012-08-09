//-----------------------------------------------------------------------
// <copyright file="SubgraphAttemptedException.cs" company="Fluxtree Technologies LLC.">
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

namespace GraphComposite.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Exception for attempt to add a subgraph to a non-hierarchical graph.
    /// </summary>
    public class SubgraphAttemptedException : GraphException
    {
        /// <summary>
        /// Initializes a new instance of the SubgraphAttemptedException class.
        /// </summary>
        /// <param name="s">Message string.</param>
        public SubgraphAttemptedException(string s)
            : base(s)
        {
        }
    }
}
