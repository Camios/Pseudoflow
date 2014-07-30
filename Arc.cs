using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pseudoflow
{
    public class Arc
    {
        public Vertex from;
        public Vertex to;
        public double capacity;
        public double flow;
        public int direction = 1;

        //public Arc(int sourceNumber, int destinationNumber)
        //{
        //    SourceNumber = sourceNumber;
        //    DestinationNumber = destinationNumber;
        //}

        public Arc(Vertex fromIn, Vertex toIn)
        {
            from = fromIn;
            to = toIn;
        }
        public Arc(Arc toCopy)
        {
            from = toCopy.from;
            to = toCopy.to;
            capacity = toCopy.capacity;
            flow = toCopy.flow;
            direction = toCopy.direction;
        }

        public double ResidualCapacity()
        {
            return capacity - flow;
        }

        public double ResidualCapacityBackwards()
        {
            return flow;
        }

        public bool IsResidualArc()
        {
            return flow < capacity;
        }

        public bool IsResidualArcBackwards()
        {
            return flow > 0;
        }
        
        public ArcValidation Validate()
        {
            if (capacity < 0)
            {
                return ArcValidation.CapacityNegative;
            }

            if (flow < 0)
            {
                return ArcValidation.FlowNegative;
            }

            if (flow > capacity)
            {
                return ArcValidation.FlowExceedsCapacity;
            }

            return ArcValidation.Valid;
        }

        public override string ToString()
        {
            if (from != null && to != null)
            {
                return from.number + "->" + to.number + ", C:" + capacity + " F:" + flow + " D:" + direction;
            }
            return string.Empty;
        }
    }

}
