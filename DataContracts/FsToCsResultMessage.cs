using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts
{
    /// <summary>
    /// Message object for communicating from F# to C#.
    /// Based on music.
    /// </summary>
    public class FsToCsResultMessage
    {
        public bool IsThisTheRealLife { get; set; }

        public bool IsThisJustFantasy { get; set; }

        public double CaughtInALandSlide { get; set; }

        public IEnumerable<string> NoEscapeFromReality { get; set; }
    }
}
