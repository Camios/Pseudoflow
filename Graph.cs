using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pseudoflow
{
    public class Graph
    {
        public List<Vertex> V = new List<Vertex>();
        public List<Arc> A = new List<Arc>();
        //public Dictionary<int, Arc> Incoming = new Dictionary<int, Arc>();
        //public Dictionary<int, Arc> Outgoing = new Dictionary<int, Arc>();

        public Graph()
        {
        }

        public Graph(Graph toCopy)
        {
            this.V = new List<Vertex>(toCopy.V.Count);
            var numberToClonedVertex = new Dictionary<int, Vertex>();
            for (int i = 0; i < toCopy.V.Count; i++)
            {
                var clonedVertex = new Vertex(toCopy.V[i]);
                numberToClonedVertex[clonedVertex.number] = clonedVertex;
                this.V.Add(clonedVertex);
            }

            this.A = new List<Arc>(toCopy.A.Count);
            for (int i = 0; i < toCopy.A.Count; i++)
            {
                var arcToCopy = toCopy.A[i];
                Vertex fromVertex;
                Vertex toVertex;
                if (numberToClonedVertex.TryGetValue(arcToCopy.from.number, out fromVertex))
                {

                    if (numberToClonedVertex.TryGetValue(arcToCopy.to.number, out toVertex))
                    {
                        var newArc = new Arc(fromVertex, toVertex)
                        {
                            flow = arcToCopy.flow,
                            direction = arcToCopy.direction,
                            capacity = arcToCopy.capacity,
                        };
                        this.A.Add(newArc);
                    }
                }
            }

            //this.Incoming = new Dictionary<int, Arc>(toCopy.Incoming.Count);
            //foreach (var kvp in toCopy.Incoming)
            //{
            //    Incoming[kvp.Key] = new Arc(kvp.Value);
            //}

            //this.Outgoing = new Dictionary<int, Arc>(toCopy.Outgoing.Count);
            //foreach (var kvp in toCopy.Outgoing)
            //{
            //    Outgoing[kvp.Key] = new Arc(kvp.Value);
            //}
        }

    }
}
