using System;
using ASD.Graphs;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace ASD
{
    public class Lab04 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - Szukanie mozliwych do odwiedzenia miast z grafu skierowanego
        /// przy zalozeniu, ze pociagi odjezdzaja co godzine.
        /// </summary>
        /// <param name="graph">Graf skierowany przedstawiający siatke pociagow</param>
        /// <param name="miastoStartowe">Numer miasta z ktorego zaczyna sie podroz pociagiem</param>
        /// <param name="K">Godzina o ktorej musi zakonczyc sie nasza podroz</param>
        /// <returns>Tablica numerow miast ktore mozna odwiedzic. Posortowana rosnaco.</returns>
        public int[] Lab04Stage1(DiGraph graph, int miastoStartowe, int K)
        {
            // TODO
           
            int[] miastaMozliweDoOdwiedzenia = new int[] { miastoStartowe };
            
            int[] time = new int[graph.VertexCount];
            for(int i=0; i<graph.VertexCount;i++)
            {
                time[i] = int.MaxValue;
            }
            time[miastoStartowe] = 8;
            foreach(Edge e in graph.BFS().SearchFrom(miastoStartowe))
            {
                if (time[e.To] > time[e.From]+1 && time[e.From]+1 <=K && time[e.From]!=int.MaxValue)
                {
                    time[e.To] = time[e.From] + 1;
                }
            }
            List<int> list = new List<int>();
            for(int i=0; i<graph.VertexCount;i++)
            {
                if (time[i]!=int.MaxValue)
                {
                    list.Add(i);
                }
            }
            miastaMozliweDoOdwiedzenia = list.ToArray();
            return miastaMozliweDoOdwiedzenia;
        }

        /// <summary>
        /// Etap 2 - Szukanie mozliwych do odwiedzenia miast z grafu skierowanego.
        /// Waga krawedzi oznacza, ze pociag rusza o tej godzinie
        /// </summary>
        /// <param name="graph">Wazony graf skierowany przedstawiający siatke pociagow</param>
        /// <param name="miastoStartowe">Numer miasta z ktorego zaczyna sie podroz pociagiem</param>
        /// <param name="K">Godzina o ktorej musi zakonczyc sie nasza podroz</param>
        /// <returns>Tablica numerow miast ktore mozna odwiedzic. Posortowana rosnaco.</returns>
        public int[] Lab04Stage2(DiGraph<int> graph, int miastoStartowe, int K)
        {
            int []distance = new int[graph.VertexCount];
            SafePriorityQueue<int, int> queue = new SafePriorityQueue<int, int>();
            for (int i=0; i<miastoStartowe; i++)
            {
                distance[i] = int.MaxValue;
                queue.Insert(i, distance[i]);

            }
            distance[miastoStartowe] = 8;
            queue.Insert(miastoStartowe, distance[miastoStartowe]);
            for(int i = miastoStartowe+1; i<distance.Length; i++)
            {
                distance[i] = int.MaxValue;
                queue.Insert(i, distance[i]);
            }
            while(queue.Count > 0)
            {
                int vert = queue.Extract();
                foreach(Edge<int> e in graph.OutEdges(vert))
                {
                    if(e.From == vert && distance[e.To]>e.Weight + 1 &&
                        1 + e.Weight<=K && e.Weight >= distance[e.From])
                    {
                        distance[e.To] = e.Weight + 1;
                        queue.UpdatePriority(e.To, distance[e.To]);
                    }
                }
            }
            List<int> city = new List<int>();   
            for(int i=0; i<distance.Length; i++)
            {
                if (distance[i]<=K)
                {
                    city.Add(i);
                }
            }
            return city.ToArray();
           
        }
    }
}
