using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pseudoflow
{
    public class Vertex
    {
        ///// <summary>
        ///// Alphabetic id (a..)
        ///// </summary>
        //public string Id;
        public int label;
        /// <summary>
        /// Value of vertex.
        /// </summary>
        public double weight;

        /// <summary>
        /// Inflow - Outflow. i.e. too much is coming in.
        /// </summary>
        public double excess;
        public Vertex next;
        public Vertex parent;
        public Vertex childList;
        public Vertex nextScan;
        public int nextArc;
        public int numOutOfTree;
        public Arc[] outOfTree;
        public Arc arcToParent;

        /// <summary>
        /// 1-based index of the vertexes
        /// </summary>
        public int number;
        public int visited;
        public int numAdjacent;
        //public string Name;

        //public Dictionary<int, Arc> Outgoing = new Dictionary<int, Arc>();
        //public Dictionary<int, Arc> Incoming = new Dictionary<int, Arc>();


        public Vertex(int numberIn, double weightIn)
        {
            number = numberIn;
            weight = weightIn;
        }

        public Vertex(Vertex toCopy)
        {
            this.number = toCopy.number;
            this.weight = toCopy.weight;

            //this.Incoming = new Dictionary<int, Arc>(toCopy.Incoming.Count);
            //foreach (var kvp in toCopy.Incoming)
            //{
            //    Incoming[kvp.Key] = new Arc(toCopy.Incoming[kvp.Key]);
            //}
            //for (int i = 0; i < Incoming.Count; i++)
            //{
            //    Incoming[i] = new Arc(toCopy.Incoming[i]);
            //}
            //this.Outgoing = new Dictionary<int, Arc>(toCopy.Outgoing.Count);
            //foreach (var kvp in toCopy.Outgoing)
            //{
            //    Outgoing[kvp.Key] = new Arc(toCopy.Outgoing[kvp.Key]);
            //} 
            //for (int i = 0; i < Outgoing.Count; i++)
            //{
            //    Outgoing[i] = new Arc(toCopy.Outgoing[i]);
            //}

        }
        public override string ToString()
        {
            return number + "=" + weight + " Excess:" + excess;
        }

    }
}
