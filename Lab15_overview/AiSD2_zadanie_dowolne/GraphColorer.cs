using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD2
{
    public class GraphColorer : MarshalByRefObject
    {
        /// <summary>
        /// Metoda znajduje kolorowanie zadanego grafu g używające najmniejsze możliwej liczby kolorów.
        /// </summary>
        /// <param name="g">Graf (nieskierowany)</param>
        /// <returns>Liczba użytych kolorów i kolorowanie (coloring[i] to kolor wierzchołka i). Kolory mogą być dowolnymi liczbami całkowitymi.</returns>

        int minChromaicNumberFound;
        int[] coloringFound;
        int maxDegree;
        int[] numberOfColorsUnavailable;
        int[] numberOfUncoloredNeighbors;
        Dictionary<int, int>[] colorsPerVertex;

        public (int numberOfColors, int[] coloring) FindBestColoring(Graph g)
        {
            // Initializing variables
            minChromaicNumberFound = int.MaxValue;
            numberOfColorsUnavailable = new int[g.VertexCount];
            numberOfUncoloredNeighbors = new int[g.VertexCount];
            colorsPerVertex = new Dictionary<int, int>[g.VertexCount];
            int[] coloring = new int[g.VertexCount];
            List<int> vertList = new List<int>();
            for (int i=0; i<g.VertexCount; i++)
            {
                colorsPerVertex[i] = new Dictionary<int, int>();
                for (int j=0; j<g.VertexCount; j++)
                    colorsPerVertex[i].Add(j, 0);
                numberOfUncoloredNeighbors[i] = g.Degree(i);
            }
            for(int i=0; i<coloring.Length;i++)
            {
                coloring[i] = -1;
            }
            maxDegree = GetMaxDegree(g);
            
            // Calling recursive method
            FindBestColoringRec(g, coloring, 0, vertList, 0);

            // Coloring verticies described in clue nr 2
            for(int j=0; j<coloringFound.Length; j++)
            {
                if (coloringFound[j]==-10)
                { 
                for (int i=0; i<minChromaicNumberFound;i++)
                {
                        if (colorsPerVertex[j][i]==0)
                        {
                            coloringFound[j] = i;
                            break;
                        }
                    }
                }
            }

            return (minChromaicNumberFound, coloringFound);
        }
       
        // Function returning max degree in given graph
        private int GetMaxDegree(Graph g)
        {
            int maxDegree = g.Degree(0);
            for(int i=1; i<g.VertexCount;i++)
            {
                if(g.Degree(i)>maxDegree)
                {
                    maxDegree=g.Degree(i);
                }
            }
            return maxDegree;
        }

        // Choosing vertex based on number of colored neighbors and colors available from vertex
        private int SelectVertex(Graph G, int[] coloring, int availableColors)
        {
            int indxCandidate = -1;
            int minNumberOfColorsAvailable = int.MaxValue;
            int maxNumberOfUncoloredNeighbors = -int.MaxValue;
            for (int i = 0; i < G.VertexCount; i++)
            {
                if (coloring[i] == -1)
                {
                    if (availableColors - numberOfColorsUnavailable[i] < minNumberOfColorsAvailable || (availableColors - numberOfColorsUnavailable[i] == minNumberOfColorsAvailable && numberOfUncoloredNeighbors[i] > maxNumberOfUncoloredNeighbors))
                    {
                        indxCandidate = i;
                        minNumberOfColorsAvailable = availableColors - numberOfColorsUnavailable[i];
                        maxNumberOfUncoloredNeighbors = numberOfUncoloredNeighbors[i];
                    }
                }
            }
            return indxCandidate;
        }

        // Updating information about colored neighbors, used colors for verticies
        private void UpdateTables(Graph G, int vert, bool ifColored, int color)
        {
            foreach(int neighbor in G.OutNeighbors(vert))
            {
                if(ifColored)
                {
                    numberOfUncoloredNeighbors[neighbor]--;
                    if (colorsPerVertex[neighbor][color] == 0)
                    {
                        numberOfColorsUnavailable[neighbor]++;
                    }
                    colorsPerVertex[neighbor][color]++;
                }
                else
                {
                    numberOfUncoloredNeighbors[neighbor]++;
                    colorsPerVertex[neighbor][color]--;
                    if (colorsPerVertex[neighbor][color] == 0)
                    {
                        numberOfColorsUnavailable[neighbor]--;
                    }
                }
            }
        }
        public void FindBestColoringRec(Graph G, int[] coloring, int availableColors, List<int> vertList, int coloredVert)
        {
            // Returning from recursion if all verticies in graph are colored or we won't be able to find better chromatic number
            if (coloredVert == G.VertexCount)
            {
                if (availableColors<minChromaicNumberFound)
                {
                    coloringFound = coloring.ToArray();
                    minChromaicNumberFound = availableColors;
                }
                return;
            }
            if (availableColors >= minChromaicNumberFound && availableColors>maxDegree)
                return;
            int selectedVert = SelectVertex(G, coloring, availableColors);

            // Choosing verticies to be colored later (verticies described in clue nr 2)
            if (numberOfColorsUnavailable[selectedVert] > G.Degree(selectedVert) && !vertList.Contains(selectedVert))
            {
                vertList.Add(selectedVert);
                coloring[selectedVert] = -10;
                coloredVert++;
                FindBestColoringRec(G, coloring, availableColors, vertList, coloredVert);
                vertList.Remove(selectedVert);
                coloring[selectedVert] = -1;
                coloredVert--;
            }

            // Trying to color the selected vertex with one of the available colors
            for (int i = 0; i < availableColors; i++)
            {
                if (availableColors >= minChromaicNumberFound)
                    return;
                if (colorsPerVertex[selectedVert][i]==0)
                {
                    coloring[selectedVert] = i;
                    coloredVert++;
                    UpdateTables(G, selectedVert, true, coloring[selectedVert]);
                    FindBestColoringRec(G, coloring, availableColors, vertList, coloredVert);
                    UpdateTables(G, selectedVert, false, coloring[selectedVert]);
                    coloring[selectedVert] = -1;
                    coloredVert--;    
                }
            }

            // If coloring with one of the available colors is not possible, adding new one
            if (availableColors + 1 < minChromaicNumberFound && coloring[selectedVert]!=-10)
            {
                availableColors++;
                coloring[selectedVert] = availableColors - 1;
                coloredVert++;
                UpdateTables(G, selectedVert, true, coloring[selectedVert]);
                FindBestColoringRec(G, coloring, availableColors, vertList, coloredVert);
                UpdateTables(G, selectedVert, false, coloring[selectedVert]);
                coloring[selectedVert] = -1;
            }

            return;
        }

        
    }
}