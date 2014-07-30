#define STATS
//#define DISPLAY_FLOW
//#define DISPLAY_CUT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Pseudoflow
{
    public class Pseudoflow
    {
        int highestStrongLabel = 1;

        Graph G;
        Root[] strongRoots;
        Vertex[] AdjacencyList;
        Arc[] arcList;
        int[] labelCount;
        int NumNodes;
        int NumArcs;

#if STATS
        static long numPushes = 0;
        static int numMergers = 0;
        static int numRelabels = 0;
        static int numGaps = 0;
        static long numArcScans = 0;
#endif

#if DISPLAY_CUT
        void DisplayCut(int gap)
        {
            int i;

            Console.WriteLine("c\nc Nodes in source set of min s-t cut:");

            for (i = 0; i < NumNodes; ++i)
            {
                if (AdjacencyList[i].label >= gap)
                {
                    Console.WriteLine("n {0}", AdjacencyList[i].number);
                }
            }
        }
#endif

#if DISPLAY_FLOW
        void DisplayFlow()
        {
            int i;

            Console.WriteLine("c\nc Flow values on each arc:");

            for (i = 0; i < NumArcs; ++i)
            {
                Console.WriteLine("a {0} {1} {2}", arcList[i].from.number,
                arcList[i].to.number, arcList[i].flow);
            }
        }
#endif

        void LiftAll(Vertex rootNode)
        {
            Vertex temp;
            Vertex current = rootNode;

            current.nextScan = current.childList;

            --labelCount[current.label];
            current.label = G.V.Count;

            for (; (current != null); current = current.parent)
            {
                while (current.nextScan != null)
                {
                    temp = current.nextScan;
                    current.nextScan = current.nextScan.next;
                    current = temp;
                    current.nextScan = current.childList;

                    --labelCount[current.label];
                    current.label = G.V.Count;
                }
            }
        }


        #region s,t-Graph

        /// <summary>
        /// Source
        /// </summary>
        Vertex Vsource;
        /// <summary>
        /// Sink
        /// </summary>
        Vertex Vsink;

        int forward = 0;
        int backward = 0;
        private void CreateGraphST(Graph g)
        {
            G = new Graph(g);
            // all existing arcs between normal nodes have infinite capacity.
            for (int i = 0; i < G.A.Count; i++)
            {
                var a = G.A[i];

                // TODO
                //if (((a.from.number + a.to.number) % 2) != 0)
                //{
                //    forward++;
                //}
                //else
                //{
                //    backward++;
                //}
                
                a.capacity = double.PositiveInfinity;
            }

            Vsource = new Vertex(1, 0);
            Vsink = new Vertex(12, 0);

            for (int i = 0; i < G.V.Count; i++)
            {
                var v = G.V[i];

                // positive nodes connect from s
                if (v.weight > 0)
                {
                    AddEdgeFromSource(Vsource, v);
                }
                // negative nodes connect to t
                else if (v.weight < 0)
                {
                    AddEdgeToSink(v, Vsink);
                }
                
            }

            G.V.Insert(0, Vsource);
            G.V.Add(Vsink);

            for (int i = 0; i < G.A.Count; i++)
            {
                var a = G.A[i];
                a.from.numAdjacent++;
                a.to.numAdjacent++;
            }

            for (int i = 0; i < G.V.Count; i++)
            {
                var v = G.V[i];
                v.outOfTree = new Arc[v.numAdjacent];
            }
        }

        public void AddEdgeFromSource(Vertex s, Vertex v)
        {
            var arc = new Arc(s, v);
            arc.capacity = v.weight;
            G.A.Add(arc);
            // TODO 
            //if (((s.number + v.number) % 2) != 0)
            //{
            //    G.A.Insert(forward, arc);
            //    forward++;
            //    //backward++;
            //}
            //else
            //{
            //    G.A.Insert(G.A.Count - backward, arc);
            //    backward++;
            //}
            //s.Outgoing[v.Number] = arc;
            //v.Incoming[s.Number] = arc;
        }

        public void AddEdgeToSink(Vertex v, Vertex t)
        {
            var arc = new Arc(v, t);
            arc.capacity = -v.weight;
            // TODO 
            if (((t.number + v.number) % 2) != 0)
            {
                G.A.Insert(forward, arc);
                forward++;
                //backward++;
            }
            else
            {
                G.A.Insert(G.A.Count - backward, arc);
                backward++;
            }
            //v.Outgoing[t.Number] = arc;
            //t.Incoming[v.Number] = arc;
        }

        #endregion

        public void InitialisePF()
        {
            NumNodes = G.V.Count;
            NumArcs = G.A.Count;
            strongRoots = new Root[G.V.Count];
            AdjacencyList = new Vertex[G.V.Count];
            labelCount = new int[G.V.Count];
            for (int i = 0; i < G.V.Count; i++)
            {
                strongRoots[i] = new Root();
                AdjacencyList[i] = G.V[i];
                labelCount[i] = 0;
                G.V[i].number = i+1;
            }
            arcList = new Arc[G.A.Count];
            for (int i = 0; i < G.A.Count; i++)
            {
                arcList[i] = G.A[i];
                var Vfrom = arcList[i].from;
                var Vto = arcList[i].to;
                var capacity = arcList[i].capacity;
#if FULL_DEBUG
                Console.Write("Arc [{3}] {0}->{1} C:{2}", Vfrom.number, Vto.number, capacity, i);
#endif
                if (!(Vsource == Vto || Vsink == Vfrom || Vto == Vfrom))
                {
                    if (Vsource == Vfrom && Vto == Vsink)
                    {
#if FULL_DEBUG
                        Console.Write("Flow set from capacity ");
#endif
                        arcList[i].flow = capacity;
                    }
                    else if (Vfrom == Vsource)
                    {
#if FULL_DEBUG
                        Console.Write("Adding to Source's out of tree node");
#endif
                        AddOutOfTreeNode(Vfrom, arcList[i]);
                    }
                    else if (Vto == Vsink)
                    {
#if FULL_DEBUG
                        Console.Write("Adding to Sink's out of tree node");
#endif
                        AddOutOfTreeNode(Vto, arcList[i]);
                    }
                    else
                    {
#if FULL_DEBUG
                        Console.Write("Adding to Vfrom's out of tree node");
#endif
                        AddOutOfTreeNode(Vfrom, arcList[i]);
                    }
                }
#if FULL_DEBUG
                Console.WriteLine();
#endif
            }

            for (int i = 0; i < Vsource.numOutOfTree; i++)
            {
                var arc = Vsource.outOfTree[i];
                arc.flow = arc.capacity;
                arc.to.excess += arc.capacity;
#if FULL_DEBUG
                Console.Write("source {0}->{1} = {2}\n", arc.from.number, arc.to.number, arc.capacity);
#endif
            }
            for (int i = 0; i < Vsink.numOutOfTree; i++)
            {
                var arc = Vsink.outOfTree[i];
                arc.flow = arc.capacity;
                arc.from.excess -= arc.capacity;
#if FULL_DEBUG
                Console.Write("{0}->{1} sink = {2}\n", arc.from.number, arc.to.number, arc.capacity);
#endif
            }

            Vsource.excess = 0;
            Vsink.excess = 0;

            for (int i = 0; i < G.V.Count; i++)
            {
                var v = G.V[i];
                if (v.excess > 0)
                {
                    v.label = 1;
                    labelCount[1]++;
                    AddToStrongBucket(v, strongRoots[1]);
                }
            }

            Vsource.label = G.V.Count;
            Vsink.label = 0;
            labelCount[0] = (G.V.Count - 2) - labelCount[1];
#if FULL_DEBUG
            Console.WriteLine("LC0: " + labelCount[0] + " LC1: " + labelCount[1]);
#endif
        }

        void AddToStrongBucket(Vertex newRoot, Root rootBucket)
        {
            if (rootBucket.start != null)
            {
                rootBucket.end.next = newRoot;
                rootBucket.end = newRoot;
                newRoot.next = null;
            }
            else
            {
                rootBucket.start = newRoot;
                rootBucket.end = newRoot;
                newRoot.next = null;
            }
        }

        void Phase1()
        {
            Vertex strongRoot;
            while ((strongRoot = GetHighestStrongRoot()) != null)
            {
                ProcessRoot(strongRoot);
            }
        }

        void CheckOptimality(int gap)
        {
            uint i;
            uint check = 1;
            double mincut = 0;
            double[] excess;

            excess = new double[G.V.Count];

            for (i = 0; i < NumNodes; ++i)
            {
                excess[i] = 0;
            }

            for (i = 0; i < NumArcs; ++i)
            {
#if FULL_DEBUG
                Console.WriteLine(arcList[i].from.number + "->" +
                    arcList[i].to.number + " fromLabel:" + arcList[i].from.label +
                    " toLabel:" + arcList[i].to.label + " gap:" + gap +
                    " flow:" + arcList[i].flow + " before fromExcess:" +
                    excess[arcList[i].from.number - 1] + " toExcess:" +
                    excess[arcList[i].to.number - 1]);
#endif
                if ((arcList[i].from.label >= gap) && (arcList[i].to.label < gap))
                {
                    mincut += arcList[i].capacity;
                }

                if ((arcList[i].flow > arcList[i].capacity) || (arcList[i].flow < 0))
                {
                    check = 0;
                    Console.WriteLine("c Capacity constraint violated on arc ({0}, {1}). Flow = {2}, capacity = {3}",
                        arcList[i].from.number,
                        arcList[i].to.number,
                        arcList[i].flow,
                        arcList[i].capacity);
                }
                excess[arcList[i].from.number - 1] -= arcList[i].flow;
                excess[arcList[i].to.number - 1] += arcList[i].flow;
#if FULL_DEBUG
                Console.Write("{0}->{1} flow:{2} after fromExcess:{3} toExcess:{4}\n",
                    arcList[i].from.number, arcList[i].to.number,
                    arcList[i].flow,
                    excess[arcList[i].from.number - 1],
                    excess[arcList[i].to.number - 1]);
#endif
            }

            for (i = 0; i < NumNodes; i++)
            {
                if ((i != (Vsource.number - 1)) && (i != (Vsink.number - 1)))
                {
                    if (excess[i] != 0)
                    {
                        check = 0;
                        Console.WriteLine("c Flow balance constraint violated in node {0}. Excess = {1}",
                            i + 1,
                            excess[i]);
                    }
                }
            }

            if (check != 0)
            {
                Console.WriteLine("c\nc Solution checks as feasible.");
            }

            check = 1;

            if (excess[Vsink.number - 1] != mincut)
            {
                check = 0;
                Console.WriteLine("c Flow is not optimal - max flow does not equal min cut!\nc");
            }

            if (check != 0)
            {
                Console.WriteLine("c\nc Solution checks as optimal.\nc ");
                Console.WriteLine("s Max Flow            : {0}", mincut);
            }

        }

        void AddOutOfTreeNode(Vertex n, Arc outArc)
        {
            n.outOfTree[n.numOutOfTree] = (outArc);
            n.numOutOfTree++;
        }

        Vertex GetHighestStrongRoot()
        {
            int i;
            Vertex strongRoot;
#if FULL_DEBUG
            Console.WriteLine("GetHighestStrongRoot {0}", highestStrongLabel);
#endif
            for (i = highestStrongLabel; i > 0; --i)
            {
#if FULL_DEBUG
                Console.WriteLine("i:{0} s:{1}", i, (strongRoots[i].start != null) ? strongRoots[i].start.number : 0);
#endif
                if (strongRoots[i].start != null)
                {
                    highestStrongLabel = i;
                    if (labelCount[i - 1] != 0)
                    {
                        strongRoot = strongRoots[i].start;
                        strongRoots[i].start = strongRoot.next;
                        strongRoot.next = null;
#if FULL_DEBUG
                        Console.Write("LC[{0}] has {1} strongRoots. using {2} as strong root\n", i - 1, labelCount[i - 1], strongRoot.number);
#endif
                        return strongRoot;
                    }
#if FULL_DEBUG
                    Console.Write("LC[{0}] has no strongRoots. next {1}\n", i - 1, 
                        strongRoots[i].start != null ? strongRoots[i].start.number : 0);
#endif
                    while (strongRoots[i].start != null)
                    {

#if STATS
                        ++numGaps;
#endif
                        strongRoot = strongRoots[i].start;
#if FULL_DEBUG
                        Console.Write("#Gap {0} lifting {1} as a strongRoot\n",
                            numGaps, strongRoot != null ? strongRoot.number : 0);
#endif
                        strongRoots[i].start = strongRoot.next;
                        LiftAll(strongRoot);
#if FULL_DEBUG
                        Console.Write("next {0}\n",
                            strongRoots[i].start != null ? strongRoots[i].start.number : 0);
#endif
                    }
                }
            }

            if (strongRoots[0].start == null)
            {
#if FULL_DEBUG
                Console.Write("strongRoots[0].start is null\n");
#endif
                return null;
            }

            while (strongRoots[0].start != null)
            {
                strongRoot = strongRoots[0].start;
                strongRoots[0].start = strongRoot.next;
                strongRoot.label = 1;
                --labelCount[0];
                ++labelCount[1];

#if STATS
                ++numRelabels;
#endif
#if FULL_DEBUG
                Console.Write("#Relabels {0} strongRoot {1}\n", numRelabels, strongRoot.number);
#endif
                AddToStrongBucket(strongRoot, strongRoots[strongRoot.label]);
            }

            highestStrongLabel = 1;

            strongRoot = strongRoots[1].start;
            strongRoots[1].start = strongRoot.next;
            strongRoot.next = null;
#if FULL_DEBUG
            Console.Write("using label 1 strongRoot {0}\n", strongRoot.number);
#endif

            return strongRoot;
        }

        void ProcessRoot(Vertex strongRoot)
        {
            Vertex temp;
            Vertex strongNode = strongRoot;
            Vertex weakNode = null;
            Arc outArc;

            strongRoot.nextScan = strongRoot.childList;
#if FULL_DEBUG
            Console.Write("processRoot {0} {1}\n", strongRoot.number, strongRoot.nextScan != null ? strongRoot.nextScan.number : 0);
#endif

            if ((outArc = FindWeakNode(strongRoot, ref weakNode)) != null)
            {
                Merge(weakNode, strongNode, outArc);
                PushExcess(strongRoot);
                return;
            }

            CheckChildren(strongRoot);

            while (strongNode != null)
            {
                while (strongNode.nextScan != null)
                {
                    temp = strongNode.nextScan;
                    strongNode.nextScan = strongNode.nextScan.next;
                    strongNode = temp;
                    strongNode.nextScan = strongNode.childList;

                    if ((outArc = FindWeakNode(strongNode, ref weakNode)) != null)
                    {
                        Merge(weakNode, strongNode, outArc);
                        PushExcess(strongRoot);
                        return;
                    }

                    CheckChildren(strongNode);
                }

                if ((strongNode = strongNode.parent) != null)
                {
                    CheckChildren(strongNode);
                }
            }

            AddToStrongBucket(strongRoot, strongRoots[strongRoot.label]);

            ++highestStrongLabel;
        }

        void CheckChildren(Vertex curNode)
        {
            for (; (curNode.nextScan != null); curNode.nextScan = curNode.nextScan.next)
            {
                if (curNode.nextScan.label == curNode.label)
                {
                    return;
                }

            }

            --labelCount[curNode.label];
            ++curNode.label;
            ++labelCount[curNode.label];

#if STATS
            ++numRelabels;
#endif

            curNode.nextArc = 0;
        }

        Arc FindWeakNode(Vertex strongNode, ref Vertex weakNode)
        {
            int i, size;
            Arc outArc;
            //strongNode.NumOutOfTree = strongNode.OutOfTree.Count;
            size = strongNode.numOutOfTree;
#if FULL_DEBUG
            Console.Write("findWeakNode {0} nextArc:{1} to Size:{2}\n", strongNode.number, strongNode.nextArc, size);
#endif
            for (i = strongNode.nextArc; i < size; ++i)
            {

#if STATS
                ++numArcScans;
#endif
#if FULL_DEBUG
                Console.Write("#ArcScans {0}. [{1}] {2}->{3} L:{4} HL-1:{5}\n", numArcScans, i, strongNode.number,
                    strongNode.outOfTree[i].to.number, strongNode.outOfTree[i].to.label,
                    (highestStrongLabel - 1));
#endif
                if (strongNode.outOfTree[i].to.label == (highestStrongLabel - 1))
                {
                    strongNode.nextArc = i;
                    outArc = strongNode.outOfTree[i];
                    (weakNode) = outArc.to;
                    --strongNode.numOutOfTree;
                    strongNode.outOfTree[i] = strongNode.outOfTree[strongNode.numOutOfTree];
#if FULL_DEBUG
                    Console.Write("to is weak node {1}. Strong NA:{2} #OOT:{3}\n", numArcScans, weakNode.number, strongNode.nextArc, strongNode.numOutOfTree);
#endif
                    return (outArc);
                }
                else if (strongNode.outOfTree[i].from.label == (highestStrongLabel - 1))
                {
                    strongNode.nextArc = i;
                    outArc = strongNode.outOfTree[i];
                    (weakNode) = outArc.from;
                    --strongNode.numOutOfTree;
                    strongNode.outOfTree[i] = strongNode.outOfTree[strongNode.numOutOfTree];
#if FULL_DEBUG
                    Console.Write("from is weak node {1}. Strong NA:{2} #OOT:{3}\n", numArcScans, weakNode.number, strongNode.nextArc, strongNode.numOutOfTree);
#endif
                    return (outArc);
                }
            }

            strongNode.nextArc = strongNode.numOutOfTree;
#if FULL_DEBUG
            Console.Write("#ArcScans {0}. no weak node. Strong NA=#OOT:{1}\n", numArcScans, strongNode.nextArc);
#endif

            return null;
        }

        int AddRelationship(Vertex newParent, Vertex child)
        {
            child.parent = newParent;
            child.next = newParent.childList;
            newParent.childList = child;

            return 0;
        }

        void BreakRelationship(Vertex oldParent, Vertex child)
        {
            Vertex current;

            child.parent = null;

            if (oldParent.childList == child)
            {
                oldParent.childList = child.next;
                child.next = null;
                return;
            }

            for (current = oldParent.childList; (current.next != child); current = current.next) ;

            current.next = child.next;
            child.next = null;
        }

        void Merge(Vertex parent, Vertex child, Arc newArc)
        {
            Arc oldArc;
            Vertex current = child;
            Vertex oldParent;
            Vertex newParent = parent;

#if STATS
            ++numMergers;
#endif

            while (current.parent != null)
            {
                oldArc = current.arcToParent;
                current.arcToParent = newArc;
                oldParent = current.parent;
                BreakRelationship(oldParent, current);
                AddRelationship(newParent, current);
                newParent = current;
                current = oldParent;
                newArc = oldArc;
                newArc.direction = 1 - newArc.direction;
            }

            current.arcToParent = newArc;
            AddRelationship(newParent, current);
        }


        void
        PushUpward(Arc currentArc, Vertex child, Vertex parent, double resCap)
        {
#if STATS
            ++numPushes;
#endif
#if FULL_DEBUG
            Console.Write("U#Pushes {0} ", numPushes);
            Console.Write("{0}->{1} resCap {2}. childExcess {3} ", child.number, parent.number, resCap, child.excess);
#endif
            if (resCap >= child.excess)
            {
                parent.excess += child.excess;
                currentArc.flow += child.excess;
#if FULL_DEBUG
                Console.Write("Residual available to take all excess. Child done. arcflow:{2} parentExcess:{3} (increased by all of child excess) Remaining capacity:{5}\n",
                 child.number, parent.number, currentArc.flow, parent.excess, child.excess,
                 resCap - child.excess);
#endif
                child.excess = 0;
                return;
            }

            currentArc.direction = 0;
            parent.excess += resCap;
            child.excess -= resCap;
            currentArc.flow = currentArc.capacity;
#if FULL_DEBUG
            Console.Write("Not enough residual to take all excess. Flow at max capacity. arcflow:{2} parentExcess:{3} (increased by residual capacity {4}) child excess {5} (decreased by residual capacity)\n",
                child.number, parent.number, currentArc.flow, parent.excess, resCap, child.excess);
#endif
            parent.outOfTree[parent.numOutOfTree] = currentArc;
            ++parent.numOutOfTree;
            BreakRelationship(parent, child);


            AddToStrongBucket(child, strongRoots[child.label]);
        }


        void PushDownward(Arc currentArc, Vertex child, Vertex parent, double flow)
        {
#if STATS
            ++numPushes;
#endif
#if FULL_DEBUG
            Console.Write("D#Pushes %ld ", numPushes);
            Console.Write("{0}->{1} original flow {2}. childExcess {3} ", 
                child.number, parent.number, flow, child.excess);
#endif
            if (flow >= child.excess)
            {
                parent.excess += child.excess;
                currentArc.flow -= child.excess;
#if FULL_DEBUG
                Console.Write("enough flow to take full child excess. Child done. arcflow:{2} (was decreased and) parentExcess:{3} (was increased by all of child excess {4})\n",
                    child.number, parent.number, currentArc.flow, parent.excess, child.excess
                    );
#endif
                child.excess = 0;
                return;
            }

            currentArc.direction = 1;
            child.excess -= flow;
            parent.excess += flow;
            currentArc.flow = 0;
#if FULL_DEBUG
            Console.Write("not enough flow to take full child excess. arcflow:{2} parentExcess:{3} (decreased by Original flow) and childExcess:{5} (was increased by Original Flow)\n",
                child.number, parent.number, currentArc.flow, parent.excess, flow, child.excess);
#endif
            parent.outOfTree[parent.numOutOfTree] = currentArc;
            ++parent.numOutOfTree;
            BreakRelationship(parent, child);

            AddToStrongBucket(child, strongRoots[child.label]);
        }

        void PushExcess(Vertex strongRoot)
        {
            Vertex current, parent;
            Arc arcToParent;
            double prevEx = 1;

            for (current = strongRoot; (current.excess != 0 && current.parent != null); current = parent)
            {
                parent = current.parent;
                prevEx = parent.excess;

                arcToParent = current.arcToParent;

                if (arcToParent.direction != 0)
                {
                    PushUpward(arcToParent, current, parent, (arcToParent.capacity - arcToParent.flow));
                }
                else
                {
                    PushDownward(arcToParent, current, parent, arcToParent.flow);
                }
            }

            if ((current.excess > 0) && (prevEx <= 0))
            {

                AddToStrongBucket(current, strongRoots[current.label]);
            }
        }

        void RecoverFlow(int gap)
        {
            int i, j, iteration = 1;
            Arc tempArc;
            Vertex tempNode;

            for (i = 0; i < AdjacencyList[Vsink.number-1].numOutOfTree; ++i)
            {
                tempArc = AdjacencyList[Vsink.number-1].outOfTree[i];
                if (tempArc.from.excess < 0)
                {
                    if ((tempArc.from.excess + (int)tempArc.flow) < 0)
                    {
                        tempArc.from.excess += (int)tempArc.flow;
                        tempArc.flow = 0;
                    }
                    else
                    {
                        tempArc.flow = (uint)(tempArc.from.excess + (int)tempArc.flow);
                        tempArc.from.excess = 0;
                    }
                }
            }

            for (i = 0; i < AdjacencyList[Vsource.number-1].numOutOfTree; ++i)
            {
                tempArc = AdjacencyList[Vsource.number-1].outOfTree[i];
                AddOutOfTreeNode(tempArc.to, tempArc);
            }

            AdjacencyList[Vsource.number-1].excess = 0;
            AdjacencyList[Vsink.number-1].excess = 0;

            for (i = 0; i < NumNodes; ++i)
            {
                tempNode = AdjacencyList[i];

                if ((i == (Vsource.number -1)) || (i == (Vsink.number - 1)))
                {
                    continue;
                }

                if (tempNode.label >= gap)
                {
                    tempNode.nextArc = 0;
                    if ((tempNode.parent != null) && (tempNode.arcToParent.flow != 0))
                    {
                        AddOutOfTreeNode(tempNode.arcToParent.to, tempNode.arcToParent);
                    }

                    for (j = 0; j < tempNode.numOutOfTree; ++j)
                    {
                        if (tempNode.outOfTree[j].flow == 0)
                        {
                            --tempNode.numOutOfTree;
                            tempNode.outOfTree[j] = tempNode.outOfTree[tempNode.numOutOfTree];
                            --j;
                        }
                    }

                    Sort(tempNode);
                }
            }

            for (i = 0; i < NumNodes; ++i)
            {
                tempNode = AdjacencyList[i];
                while (tempNode.excess > 0)
                {
                    ++iteration;
                    Decompose(tempNode, Vsource.number, ref iteration);
                }
            }
        }

        void Decompose(Vertex excessNode, int source, ref int iteration)
        {
            Vertex current = excessNode;
            Arc tempArc;
            double bottleneck = excessNode.excess;

            for (; (current.number != source) && (current.visited < (iteration));
                        current = tempArc.from)
            {
                current.visited = (iteration);
                tempArc = current.outOfTree[current.nextArc];

                if (tempArc.flow < bottleneck)
                {
                    bottleneck = tempArc.flow;
                }
            }

            if (current.number == source)
            {
                excessNode.excess -= bottleneck;
                current = excessNode;

                while (current.number != source)
                {
                    tempArc = current.outOfTree[current.nextArc];
                    tempArc.flow -= bottleneck;

                    if (tempArc.flow != 0)
                    {
                        Minisort(current);
                    }
                    else
                    {
                        ++current.nextArc;
                    }
                    current = tempArc.from;
                }
                return;
            }

            ++(iteration);

            bottleneck = current.outOfTree[current.nextArc].flow;

            while (current.visited < (iteration))
            {
                current.visited = (iteration);
                tempArc = current.outOfTree[current.nextArc];

                if (tempArc.flow < bottleneck)
                {
                    bottleneck = tempArc.flow;
                }
                current = tempArc.from;
            }

            ++(iteration);

            while (current.visited < (iteration))
            {
                current.visited = (iteration);

                tempArc = current.outOfTree[current.nextArc];
                tempArc.flow -= bottleneck;

                if (tempArc.flow != 0)
                {
                    Minisort(current);
                    current = tempArc.from;
                }
                else
                {
                    ++current.nextArc;
                    current = tempArc.from;
                }
            }
        }

        void Minisort(Vertex current)
        {
            Arc temp = current.outOfTree[current.nextArc];
            int i;
            int size = current.numOutOfTree;
            double tempflow = temp.flow;

            for (i = current.nextArc + 1; ((i < size) && (tempflow < current.outOfTree[i].flow)); ++i)
            {
                current.outOfTree[i - 1] = current.outOfTree[i];
            }
            current.outOfTree[i - 1] = temp;
        }

        void QuickSort(Arc[] arr, int first, int last)
        {
            int i, j, left = first, right = last, mid, pivot;
            double x1, x2, x3, pivotval;
            Arc swap;

            if ((right - left) <= 5)
            {// Bubble sort if 5 elements or less
                for (i = right; (i > left); --i)
                {
                    swap = null;
                    for (j = left; j < i; ++j)
                    {
                        if (arr[j].flow < arr[j + 1].flow)
                        {
                            swap = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = swap;
                        }
                    }

                    if (swap == null)
                    {
                        return;
                    }
                }

                return;
            }

            mid = (first + last) / 2;

            x1 = arr[first].flow;
            x2 = arr[mid].flow;
            x3 = arr[last].flow;

            pivot = mid;

            if (x1 <= x2)
            {
                if (x2 > x3)
                {
                    pivot = left;

                    if (x1 <= x3)
                    {
                        pivot = right;
                    }
                }
            }
            else
            {
                if (x2 <= x3)
                {
                    pivot = right;

                    if (x1 <= x3)
                    {
                        pivot = left;
                    }
                }
            }

            pivotval = arr[pivot].flow;

            swap = arr[first];
            arr[first] = arr[pivot];
            arr[pivot] = swap;

            left = (first + 1);

            while (left < right)
            {
                if (arr[left].flow < pivotval)
                {
                    swap = arr[left];
                    arr[left] = arr[right];
                    arr[right] = swap;
                    --right;
                }
                else
                {
                    ++left;
                }
            }

            swap = arr[first];
            arr[first] = arr[left];
            arr[left] = swap;

            if (first < (left - 1))
            {
                QuickSort(arr, first, (left - 1));
            }

            if ((left + 1) < last)
            {
                QuickSort(arr, (left + 1), last);
            }
        }

        void Sort(Vertex current)
        {
            if (current.numOutOfTree > 1)
            {
                QuickSort(current.outOfTree, 0, (current.numOutOfTree - 1));
            }
        }


        //#region Extension Network

        //private const int IdRoot = -1;

        //Graph Gext;
        ///// <summary>
        ///// Root
        ///// </summary>
        //Vertex Vr;

        //private void CreateGraphExt(Graph Gst)
        //{
        //    Gst = new Graph(Gst);

        //    Vr = new Vertex(IdRoot, 0);

        //    for (int i = 0; i < Gst.V.Count; i++)
        //    {
        //        var v = Gst.V[i];
        //        if (v.Id != IdS && v.Id != IdT)
        //        {
        //            AddInfiniteCapacity(Vr, v);
        //            AddInfiniteCapacity(v, Vr);
        //        }
        //    }

        //    //// excess arcs. this is going to be an arc coming from every Vertex
        //    //for (int i = 0; i < Vs.Incoming.Count; i++)
        //    //{
        //    //    Vr.Incoming.Add(Vs.Incoming[i]);
        //    //}
        //    // this is going to be an arc going to every positively weighted Vertex
        //    for (int i = 0; i < Vs.Outgoing.Count; i++)
        //    {
        //        Vr.Outgoing[(Vs.Outgoing[i]);
        //    }

        //    // this is going to be an arc coming from every negatively weighted Vertex
        //    for (int i = 0; i < Vt.Incoming.Count; i++)
        //    {
        //        Vr.Incoming.Add(Vt.Incoming[i]);
        //    }
        //    //// deficit arcs. this is going to be an arc going to every Vertex
        //    //for (int i = 0; i < Vt.Outgoing.Count; i++)
        //    //{
        //    //    Vr.Outgoing.Add(Vt.Outgoing[i]);
        //    //}
        //}

        //private void AddInfiniteCapacity(Vertex source, Vertex destination)
        //{
        //    var arc = new Arc(source, destination);
        //    arc.Capacity = double.PositiveInfinity;

        //    source.Outgoing[destination.Id] = arc;
        //    destination.Incoming[source.Id] = arc;
        //}

        //#endregion

        public double CutCapacity(List<Arc> cutArcs)
        {
            double cutCapacity = cutArcs.Sum(a => a.capacity);
            return cutCapacity;
        }

        public double TotalFlow(List<Arc> cutArcs)
        {
            double totalFlow = cutArcs.Sum(a => a.flow);
            return totalFlow;
        }

        public double CutResidualCapacity(List<Arc> cutArcs)
        {
            double totalFlow = cutArcs.Sum(a => a.ResidualCapacity());
            return totalFlow;
        }

        //public bool IsResidualPath(List<Arc> cut

        public Pseudoflow(Graph g)
        {

            DateTime readStart, readEnd, initStart, initEnd, solveStart, solveEnd, flowStart, flowEnd;
            readStart = DateTime.Now;
            CreateGraphST(g);
            Console.WriteLine("Finished creating the s,t graph");
            readEnd = DateTime.Now;

            initStart = readEnd;
            InitialisePF();
            initEnd = DateTime.Now;
            Console.WriteLine("Finished initialising pseudoflow");

            solveStart = initEnd;
            Phase1();
            solveEnd = DateTime.Now;
            Console.WriteLine("Finished pseudoflow phase 1");

            int gap = G.V.Count;
            flowStart = solveEnd;
            RecoverFlow(gap);
            flowEnd = DateTime.Now;

            Console.WriteLine("c Number of nodes     : {0}", NumNodes);
            Console.WriteLine("c Number of arcs      : {0}", NumArcs);
            Console.WriteLine("c Time to read        : {0}", (readEnd - readStart));
            Console.WriteLine("c Time to initialize  : {0}", (initEnd - initStart));
            Console.WriteLine("c Time to min cut     : {0}", (solveEnd - initStart));
            Console.WriteLine("c Time to max flow    : {0}", (flowEnd - initStart));
#if STATS
            Console.WriteLine("c Number of arc scans : {0}", numArcScans);
            Console.WriteLine("c Number of mergers   : {0}", numMergers);
            Console.WriteLine("c Number of pushes    : {0}", numPushes);
            Console.WriteLine("c Number of relabels  : {0}", numRelabels);
            Console.WriteLine("c Number of gaps      : {0}", numGaps);
#endif

            CheckOptimality(gap);
#if DISPLAY_CUT
            DisplayCut(gap);
#endif
            DisplayWeight(gap);
#if DISPLAY_FLOW
            DisplayFlow();
#endif
        }
        void DisplayWeight(int gap)
        {
            int i;
            double weight = 0;
            Console.WriteLine("c\nc Nodes in source set of min s-t cut (gap{0}):", gap);

            for (i = 0; i < NumNodes; ++i)
            {
                if (AdjacencyList[i].label >= gap)
                {
                    weight+=AdjacencyList[i].weight;
                    //Console.WriteLine("n {0}", AdjacencyList[i].number);
                }
            }
            Console.WriteLine("w {0}", weight);
        }
    }
}
