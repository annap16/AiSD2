using System;
using System.Collections.Generic;
using ASD.Graphs;

namespace ASD
{
    public class Lab08 : MarshalByRefObject
    {
        /// <summary>Etap I: prace przedprojektowe</summary>
        /// <param name="l">Długość działki, którą dysponuje Kameleon Kazik.</param>
        /// <param name="h">Maksymalna wysokość budowli.</param>
        /// <param name="pleasure">Tablica rozmiaru [l,h] zawierająca wartości zadowolenia p(x,y) dla każdych x i y.</param>
        /// <returns>Odpowiedź na pytanie, czy istnieje budowla zadowalająca Kazika.</returns>
        public bool Stage1ExistsBuilding(int l, int h, int[,] pleasure)
        {
            DiGraph<int> G = new DiGraph<int>(l*h+2);

            int count = 0;
            for(int w = 0; w<h; w++)
            {
                for(int k = 0; k<l-w; k++)
                {
                    count+=pleasure[k,w];
                    G.AddEdge(l * h, l * w + k, pleasure[k, w]);
                    G.AddEdge(l * w + k, l * h + 1, 1);
                    if (w - 1 >= 0)
                    {
                        G.AddEdge(l * w + k, l * (w - 1) + k, int.MaxValue);
                    }
                    if (w - 1 >= 0 && k + 1 < l)
                    {
                        G.AddEdge(l * w + k, l * (w - 1) + k + 1, int.MaxValue);
                    }
                }
            }
            var (flowValue,f) = Flows.FordFulkerson(G, l * h, l * h + 1);
            return flowValue < count;
        }

        /// <summary>Etap II: kompletny projekt</summary>
        /// <param name="l">Długość działki, którą dysponuje Kameleon Kazik.</param>
        /// <param name="h">Maksymalna wysokość budowli.</param>
        /// <param name="pleasure">Tablica rozmiaru [l,h] zawierająca wartości zadowolenia p(x,y) dla każdych x i y.</param>
        /// <param name="blockOrder">Argument wyjściowy, w którym należy zwrócić poprawną kolejność ustawienia bloków w znalezionym rozwiązaniu;
        ///     kolejność jest poprawna, gdy przed blokiem (x,y) w tablicy znajdują się bloki (x,y-1) i (x+1,y-1) lub gdy y=0. 
        ///     Ustawiane bloki powinny mieć współrzędne niewychodzące poza granice obszaru budowy (0<=x<l, 0<=y<h).
        ///     W przypadku braku rozwiązania należy zwrócić null.</param>
        /// <returns>Maksymalna wartość zadowolenia z budowli; jeśli nie istnieje budowla zadowalająca Kazika, zależy zwrócić null.</returns>
        public int? Stage2GetOptimalBuilding(int l, int h, int[,] pleasure, out (int x, int y)[] blockOrder)
        {
            DiGraph<int> G = new DiGraph<int>(l * h + 2);
            int count = 0;

            for (int w = 0; w < h; w++)
            {
                for (int k = 0; k < l - w; k++)
                {
                    count += pleasure[k, w];

                    G.AddEdge(l * h, l * w + k, pleasure[k, w]);
                    G.AddEdge(l * w + k, l * h + 1, 1);
                    if (w - 1 >= 0)
                    {
                        G.AddEdge(l * w + k, l * (w - 1) + k, int.MaxValue);
                    }
                    if (w - 1 >= 0 && k + 1 < l)
                    {
                        G.AddEdge(l * w + k, l * (w - 1) + k + 1, int.MaxValue);
                    }
                }
            }
            var (flowValue, f) = Flows.FordFulkerson(G, l * h, l * h + 1);
            blockOrder = new (int, int)[0];
            int content = count - flowValue;
            if(content<=0)
            {
                return null;
            }
            DiGraph<int> residual = Przekroj(G, f);
            int[] visited = new int[h * l + 2];
            foreach(Edge<int> e in residual.DFS().SearchFrom(h*l))
            {
                visited[e.To] = 1;
            }
            List<(int, int)> vert = new List<(int, int)>();
            for (int w=0;w<h;w++)
            {
                for(int k=0;k<l-w;k++)
                {
                    if (visited[w*l+k]==1)
                    {
                        vert.Add((k,w));
                    }
                }
            }
            blockOrder = vert.ToArray();
                return content;
         
        }

        public DiGraph<int> Przekroj(DiGraph<int> graph, DiGraph<int>f)
        {
            DiGraph<int> g = new DiGraph<int>(graph.VertexCount);
            foreach (Edge<int> e in graph.DFS().SearchAll())
            {
                if (f.HasEdge(e.From, e.To) && !f.HasEdge(e.To, e.From))
                {
                    if (e.Weight - f.GetEdgeWeight(e.From, e.To) > 0)
                    {
                        g.AddEdge(e.From, e.To, e.Weight - f.GetEdgeWeight(e.From, e.To));
                        g.AddEdge(e.To, e.From, f.GetEdgeWeight(e.From, e.To));
                    }
                    else
                    {
                        g.AddEdge(e.To, e.From, f.GetEdgeWeight(e.From, e.To));
                    }
                }
                else if (!f.HasEdge(e.From, e.To) && f.HasEdge(e.To, e.From))
                {
                    if (e.Weight - f.GetEdgeWeight(e.To, e.From) > 0)
                    {
                        g.AddEdge(e.To, e.From, e.Weight - f.GetEdgeWeight(e.To, e.From));
                        g.AddEdge(e.From, e.To, f.GetEdgeWeight(e.To, e.From));
                    }
                    else
                    {
                        g.AddEdge(e.From, e.To, f.GetEdgeWeight(e.To, e.From));
                    }
                }
                else if (e.Weight!=0)
                {
                    g.AddEdge(e.From, e.To, e.Weight);
                }

            }
            return g;
        }
    }
}
