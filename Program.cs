using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pseudoflow
{
    class Program
    {
        static Graph graph = new Graph();

        static void Main(string[] args)
        {

            int columns = 100;
            int rows = 100;
            int count = 1;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; ++j)
                {
                    count = AddNode(count, i, j, columns, 0);
                }
            }

            for (int i = rows - 1; i >= 1; i--)
            {
                for (int j = 0; j < columns; ++j)
                {
                    int from = 1 + ((i + 0) * columns + j);

                    int top = 1 + ((i - 1) * columns + j);
                    int topRight = 1 + ((i - 1) * columns + j + 1);
                    int topLeft = 1 + ((i - 1) * columns + j - 1);
                    //graph.V[from].AddUniqueEdgeOut(graph.V[top]);
                    graph.A.Add(new Arc(graph.V[from-1],graph.V[top-1]));

                    if (1 + ((0) * columns + j + 1) <= columns)
                    {
                        //graph.V[from].AddUniqueEdgeOut(graph.V[topRight]);
                        graph.A.Add(new Arc(graph.V[from-1], graph.V[topRight-1]));
                    }

                    if (1 + (j - 1) > 0)
                    {
                        //graph.V[from].AddUniqueEdgeOut(graph.V[topLeft]);
                        graph.A.Add(new Arc(graph.V[from-1], graph.V[topLeft-1]));
                    }
                }
            }

            //graph.nVertices[(rows - 1) * columns + 5].Number = 23.9;
            //graph.nVertices[(rows - 1) * columns + 9].Number = 6.9;
            ////graph.nVertices[(rows - 3) * columns + 1].Number = 4.0;
            //graph.nVertices[(rows - 2) * columns + 10].Number = -1.0;
            //graph.nVertices[(rows - 1) * columns + 13].Number = 23.9;

            //graph.V[(rows - 1) * columns + 18].weight = 240;
            //graph.V[(rows - 1) * columns + 19].weight = 3000;
            //graph.V[(rows - 1) * columns + 20].weight = 3000;
            //graph.V[(rows - 2) * columns + 21].weight = 3000;
            //graph.V[(rows - 2) * columns + 22].weight = 3000;
            //graph.V[(rows - 8) * columns + 42].weight = 1;
            graph.V[(rows - 1) * columns + 18-1].weight = 240;
            graph.V[(rows - 1) * columns + 19-1].weight = 3000;
            graph.V[(rows - 1) * columns + 20-1].weight = 3000;
            graph.V[(rows - 2) * columns + 21-1].weight = 3000;
            graph.V[(rows - 2) * columns + 22-1].weight = 3000;
            graph.V[(rows - 8) * columns + 42-1].weight = 1;

            //graph.nVertices[(rows - 2) * columns + 17].Number = 24;
            //graph.nVertices[(rows - 2) * columns + 18].Number = 24;
            //graph.nVertices[(rows - 2) * columns + 19].Number = 24;
            //graph.nVertices[(rows - 2) * columns + 20].Number = 24;

            //graph.nVertices[(rows - 3) * columns + 16].Number = 24;
            //graph.nVertices[(rows - 3) * columns + 17].Number = 24;
            //graph.nVertices[(rows - 3) * columns + 18].Number = 24;
            //graph.nVertices[(rows - 3) * columns + 19].Number = 24;
            //graph.nVertices[(rows - 3) * columns + 20].Number = 24;

            //graph.nVertices[(rows - 4) * columns + 15].Number = 24;
            //graph.nVertices[(rows - 4) * columns + 16].Number = 24;
            //graph.nVertices[(rows - 4) * columns + 17].Number = 24;
            //graph.nVertices[(rows - 4) * columns + 18].Number = 24;
            //graph.nVertices[(rows - 4) * columns + 19].Number = 24;
            //graph.nVertices[(rows - 4) * columns + 20].Number = 24;
            //graph.nVertices[(rows - 4) * columns + 21].Number = 24;

            //graph.nVertices[(rows - 4) * columns + 18].Number = 300;
            //graph.nVertices[(rows - 4) * columns + 19].Number = 300;
            //graph.nVertices[(rows - 4) * columns + 20].Number = 300;

            //graph.nVertices[(rows - 5) * columns + 18].Number = 300;
            //graph.nVertices[(rows - 5) * columns + 19].Number = 300;
            //graph.nVertices[(rows - 5) * columns + 20].Number = 300;
            //graph.nVertices[(rows - 1) * columns + 13].Number = 23.9;
            //var a = new Vertex(2, -3);
            //var b = new Vertex(3, 5);
            //var c = new Vertex(4, 4);
            //var d = new Vertex(5, -7);
            //var e = new Vertex(6, -1);
            //var f = new Vertex(7, 1);
            //var g = new Vertex(8, -1);
            //var h = new Vertex(9, 2);
            //var i = new Vertex(10, -5);
            //var j = new Vertex(11, 3);

            //graph.V.Add(a);
            //graph.V.Add(b);
            //graph.V.Add(c);
            //graph.V.Add(d);
            //graph.V.Add(e);
            //graph.V.Add(f);
            //graph.V.Add(g);
            //graph.V.Add(h);
            //graph.V.Add(i);
            //graph.V.Add(j);

            //// arc order based on ((from+to)%2)==0 insert at end
            //// ((from+to)%2)!=0 insert at start
            //graph.A.Add(new Arc(a, b)); // 2 + 3 = 5
            //graph.A.Add(new Arc(c, b)); // 4 + 3 = 7
            //graph.A.Add(new Arc(c, d)); // 4 + 5 = 9
            //graph.A.Add(new Arc(d, e)); // 5 + 6 = 11
            //graph.A.Add(new Arc(d, g)); // 5 + 8 = 13
            //graph.A.Add(new Arc(e, h)); // 6 + 9 = 15
            //graph.A.Add(new Arc(f, c)); // 7 + 4 = 11
            //graph.A.Add(new Arc(f, g)); // 7 + 8 = 15
            //graph.A.Add(new Arc(g, h)); // 8 + 9 = 17
            //graph.A.Add(new Arc(i, f)); // 10 + 7 = 17
            //graph.A.Add(new Arc(i, j)); // 10 + 11 = 18
            //graph.A.Add(new Arc(j, g)); // 11 + 8 = 19
            ////--
            //graph.A.Add(new Arc(j, h)); // 11 + 9 = 20
            //graph.A.Add(new Arc(g, e)); // 8 + 6 = 14
            //graph.A.Add(new Arc(f, j)); // 7 + 11 = 18
            //graph.A.Add(new Arc(f, d)); // 7 + 5 = 12
            //graph.A.Add(new Arc(d, b)); // 5 + 3 = 8
            //graph.A.Add(new Arc(a, c)); // 2 + 4 = 6

            Pseudoflow pf = new Pseudoflow(graph);

            Console.ReadLine();
        }

        private static int AddNode(int count, int i, int j, int x1, int x2)
        {
            if (j < x1 || j > x2)
            {
                Vertex v = new Vertex(count++, -1);
                //graphPad1.Data.Add(v.nId, new Point(j, i));
                //graphPad1.Colors.Add(v.nId, Color.LightCoral);
                graph.V.Add(v);
            }
            return count;
        }

    }
}
