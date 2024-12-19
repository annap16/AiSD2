using ASD.Graphs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
	public class Lab06 : MarshalByRefObject
	{
		/// <summary>Etap 1</summary>
		/// <param name="n">Liczba kolorów (równa liczbie wierzchołków w c)</param>
		/// <param name="c">Graf opisujący możliwe przejścia między kolorami. Waga to wysiłek.</param>
		/// <param name="g">Graf opisujący drogi w mieście. Waga to kolor drogi.</param>
		/// <param name="target">Wierzchołek docelowy (dom Grzesia).</param>
		/// <param name="start">Wierzchołek startowy (wejście z lasu).</param>
		/// <returns>Pierwszy element pary to informacja, czy rozwiązanie istnieje. Drugi element pary, to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (pierwszy musi być start, ostatni target). W przypadku, gdy nie ma rozwiązania, ma być tablica o długości 0.</returns>
		public (bool possible, int[] path) Stage1(
			int n, DiGraph<int> c, Graph<int> g, int target, int start
		)
		{
			Stack<int> stack = new Stack<int>();
			bool[] prevColor = new bool[g.VertexCount*c.VertexCount];
			int[] prevVert = new int[g.VertexCount * c.VertexCount];
			int pathIdx = -1;
			for(int i=0; i<g.VertexCount*c.VertexCount; i++)
			{
				prevColor[i] = false;
				prevVert[i] = -1;
			}
			bool reachable = false;
			for(int i=0; i<c.VertexCount; i++)
			{
				stack.Push(start + i * g.VertexCount);
			}
			while(stack.Count>0)
			{
				int idx = stack.Pop();
				foreach(Edge<int> e in g.OutEdges(idx%g.VertexCount))
				{
					if(!prevColor[e.To + g.VertexCount*e.Weight] && (c.HasEdge(idx/g.VertexCount, e.Weight) || idx/g.VertexCount ==e.Weight))
					{

                        prevVert[e.To + g.VertexCount * e.Weight] = idx;
                        if (e.To==target)
						{
							reachable = true;
							pathIdx = e.To + g.VertexCount * e.Weight;
							break;
						}
						stack.Push(e.To + g.VertexCount*e.Weight);
						prevColor[e.To + g.VertexCount*e.Weight] = true;
					}
				}
            }
			if(!reachable)
			{
				return (reachable, new int[0]);
			}
			List<int> path = new List<int> ();
			path.Add(target);
			while (prevVert[pathIdx]%g.VertexCount!=start)
			{
				path.Add(prevVert[pathIdx]%g.VertexCount);
				pathIdx = prevVert[pathIdx];


            }
			path.Add(start);
			path.Reverse();
			return(reachable, path.ToArray());
        }

		/// <summary>Drugi etap</summary>
		/// <param name="n">Liczba kolorów (równa liczbie wierzchołków w c)</param>
		/// <param name="c">Graf opisujący możliwe przejścia między kolorami. Waga to wysiłek.</param>
		/// <param name="g">Graf opisujący drogi w mieście. Waga to kolor drogi.</param>
		/// <param name="target">Wierzchołek docelowy (dom Grzesia).</param>
		/// <param name="starts">Wierzchołki startowe (wejścia z lasu).</param>
		/// <returns>Pierwszy element pary to koszt najlepszego rozwiązania lub null, gdy rozwiązanie nie istnieje. Drugi element pary, tak jak w etapie 1, to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (pierwszy musi być start, ostatni target). W przypadku, gdy nie ma rozwiązania, ma być tablica o długości 0.</returns>
		public (int? cost, int[] path) Stage2(
			int n, DiGraph<int> c, Graph<int> g, int target, int[] starts
		)
		{
			int vertCount = g.VertexCount;
			int colorCount = c.VertexCount;
			DiGraph<int> graph = new DiGraph<int>(vertCount * colorCount + 2);
			foreach(Edge<int> e in g.BFS().SearchAll())
			{ 
				graph.AddEdge(e.From + e.Weight * vertCount, e.To + e.Weight * vertCount, 1);
				foreach(Edge<int> e2 in c.OutEdges(e.Weight))
				{
					graph.AddEdge(e.From + e2.From * vertCount, e.To + e2.To * vertCount, e2.Weight + 1);
				}
				for(int l = 0; l<starts.Length; l++)
				{
					graph.AddEdge(vertCount * colorCount, starts[l] + e.Weight * vertCount, 0);
                    graph.AddEdge( target + e.Weight*vertCount, vertCount * colorCount + 1, 0);
                }
			}
			PathsInfo<int> paths = Paths.Dijkstra<int>(graph, vertCount * colorCount);
			int dl = -1 ;
            if (paths.Reachable(vertCount * colorCount, vertCount * colorCount + 1) == true)
            {
				dl = paths.GetDistance(vertCount * colorCount, vertCount * colorCount + 1);
            }
            if (dl == -1)
                return (null, new int[0]);
			int[] minPath = paths.GetPath(vertCount * colorCount, vertCount * colorCount + 1);
			int[] retPath = new int[minPath.Length - 2];
			for(int i=1; i<minPath.Length-1; i++)
			{
				retPath[i-1] = minPath[i] % (vertCount);
			}
            return (dl, retPath);
		}
	}
}
