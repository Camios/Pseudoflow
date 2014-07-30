using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pseudoflow
{
    public enum ArcValidation
    {
        Valid,
        FlowExceedsCapacity,
        FlowNegative,
        CapacityNegative,
    }

}
