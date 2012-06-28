//-----------------------------------------------------------------------
// <copyright file="CycleAttemptedException.cs" company="Fluxtree Technologies LLC.">
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
    /// Exception for attempt to add an edge that would create a cycle.
    /// </summary>
    public class CycleAttemptedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CycleAttemptedException class.
        /// </summary>
        /// <param name="s">Message string.</param>
        public CycleAttemptedException(string s)
            : base(s)
        {
        }
    }
}
