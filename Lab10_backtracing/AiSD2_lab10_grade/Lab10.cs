using System;
using System.Linq;
using System.Collections.Generic;
using ASD.Graphs;

namespace ASD
{
    public class Lab10 : MarshalByRefObject
    {

        /// <param name="labyrinth">Graf reprezentujący labirynt</param>
        /// <param name="startingTorches">Ilość pochodni z jaką startują bohaterowie</param>
        /// <param name="roomTorches">Ilość pochodni w poszczególnych pokojach</param>
        /// <param name="debt>">Ilość złota jaką bohaterowie muszą zebrać</param>
        /// <param name="roomGold">Ilość złota w poszczególnych pokojach</param>
        /// <returns>Informację czy istnieje droga przez labirytn oraz tablicę reprezentującą kolejne wierzchołki na drodze. W przypadku, gdy zwracany jest false, wartość tego pola powinna być null.</returns>
        public (bool routeExists, int[] route) FindEscape(Graph labyrinth, int startingTorches, int[] roomTorches, int debt, int[] roomGold)
        {
            List<int> route = new List<int>();
            route.Add(0);
            bool[] used = new bool[labyrinth.VertexCount];
            used[0] = true;
            for (int i = 1; i < labyrinth.VertexCount; i++) used[i] = false;
            (bool ret, List<int> finalRoute) = FindEscapeRec(labyrinth, startingTorches + roomTorches[0], roomTorches, debt, roomGold[0], roomGold, 0, used, route);
            if (ret)
            {
                return (ret, finalRoute.ToArray());
            }
            return (false, null);
        }

        private (bool, List<int>) FindEscapeRec(Graph labyrinth, int myTorches, int[] roomTorches, int debt, int myMoney, int[] roomGold, int k, bool[] used, List<int> route)
        {
            if (route[k] == labyrinth.VertexCount - 1)
            {
                if (myMoney >= debt)
                {
                    return (true, route);
                }
                else
                    return (false, null);
            }
            if (myTorches <= 0)
            {
                return (false, null);
            }
            foreach (var m in labyrinth.OutNeighbors(route[k]))
            {
                if (!used[m])
                {
                    used[m] = true;
                    myMoney += roomGold[m];
                    myTorches = myTorches - 1 + roomTorches[m];
                    route.Add(m);
                    if (FindEscapeRec(labyrinth, myTorches, roomTorches, debt, myMoney, roomGold, k + 1, used, route).Item1)
                    {
                        return (true, route);
                    }
                    route.Remove(m);
                    myMoney -= roomGold[m];
                    myTorches = myTorches + 1 - roomTorches[m];
                    used[m] = false;
                }
            }
            return (false, null);

        }

        /// <param name="labyrinth">Graf reprezentujący labirynt</param>
        /// <param name="startingTorches">Ilość pochodni z jaką startują bohaterowie</param>
        /// <param name="roomTorches">Ilość pochodni w poszczególnych pokojach</param>
        /// <param name="debt">Ilość złota jaką bohaterowie muszą zebrać</param>
        /// <param name="roomGold">Ilość złota w poszczególnych pokojach</param>
        /// <param name="dragonDelay">Opóźnienie z jakim wystartuje smok</param>
        /// <returns>Informację czy istnieje droga przez labirynt oraz tablicę reprezentującą kolejne wierzchołki na drodze. W przypadku, gdy zwracany jest false, wartość tego pola powinna być null.</returns>

        public (bool routeExists, int[] route) FindEscapeWithHeadstart(Graph labyrinth, int startingTorches, int[] roomTorches, int debt, int[] roomGold, int dragonDelay)
        {
            List<int> route = new List<int>();
            route.Add(0);
            List<int> dragonRoute = new List<int>(route);
            bool[] used = new bool[labyrinth.VertexCount];
            int[] roomTorchesPom = new int[roomTorches.Length];
            roomTorches.CopyTo(roomTorchesPom, 0);
            int[] roomGoldPom = new int[roomGold.Length];
            roomGold.CopyTo(roomGoldPom, 0);
            for (int i = 0; i < labyrinth.VertexCount; i++) used[i] = false;
            startingTorches += roomTorches[0];
            roomTorches[0] = 0;
            int myMoney = roomGold[0];
            roomGold[0] = 0;
            (bool ret, List<int> finalRoute) = FindEscapeWithHeadstartRec(labyrinth, startingTorches, roomTorches,
                debt, myMoney, roomGold, 0, used, route, dragonRoute, dragonDelay, false);
            if (ret)
            {
                return (ret, finalRoute.ToArray());
            }
            return (false, null);
        }

        private (bool, List<int>) FindEscapeWithHeadstartRec(Graph labyrinth, int myTorches, int[] roomTorches, int debt, int myMoney, int[] roomGold, int k, bool[] used, List<int> route, List<int> dragonRoute, int dragonDelay, bool collectPrev)
        {
            if (route[k] == labyrinth.VertexCount - 1)
            {
                if (myMoney >= debt)
                {
                    return (true, route);
                }
                else
                    return (false, null);
            }
            if (myTorches <= 0)
            {
                return (false, null);
            }
            foreach (var m in labyrinth.OutNeighbors(route[k]))
            {

                if (k >= dragonDelay)
                {
                    used[dragonRoute[k - dragonDelay]] = true;
                }
                int prevVert = -1;
                if (route.Count - 2 >= 0)
                    prevVert = route[route.Count - 2];
                if (used[m] || (!collectPrev && prevVert >= 0 && m == prevVert))
                    continue;
                int[] torchesCopy = new int[roomTorches.Length];
                int[] goldCopy = new int[roomGold.Length];
                bool[] usedCopy = new bool[used.Length];
                for (int i = 0; i < roomTorches.Length; i++)
                {
                    torchesCopy[i] = roomTorches[i];
                    goldCopy[i] = roomGold[i];
                    usedCopy[i] = used[i];
                }
                bool collect = false;
                if (roomGold[m] != 0 || roomTorches[m] != 0)
                {
                    collect = true;
                }
                myMoney += roomGold[m];
                roomGold[m] = 0;
                myTorches = myTorches - 1 + roomTorches[m];
                roomTorches[m] = 0;
                route.Add(m);
                bool flag = false;
                List<int> prevDragonRoute = null;
                if (dragonRoute.Contains(m))
                {
                    prevDragonRoute = new List<int>(dragonRoute);
                    int index = dragonRoute.IndexOf(m);
                    if (index + 1 < dragonRoute.Count && index + 1 > 0)
                        dragonRoute.RemoveRange(index + 1, dragonRoute.Count - index - 1);

                    flag = true;
                }
                else
                {
                    dragonRoute.Add(m);
                }

                List<int> dragonRoutePom = new List<int>(dragonRoute);

                if (FindEscapeWithHeadstartRec(labyrinth, myTorches, roomTorches, debt, myMoney, roomGold,
                        k + 1, usedCopy, route, dragonRoutePom, dragonDelay, collect).Item1)
                {
                    return (true, route);
                }
                route.RemoveAt(route.Count - 1);
                if (flag)
                {
                    dragonRoute = new List<int>(prevDragonRoute);
                }
                else
                {
                    dragonRoute.RemoveAt(dragonRoute.Count - 1);
                }

                myMoney -= goldCopy[m];
                roomGold[m] = goldCopy[m];
                myTorches = myTorches + 1 - torchesCopy[m];
                roomTorches[m] = torchesCopy[m];

                if (k - dragonDelay >= 0)
                    used[dragonRoute[k - dragonDelay]] = false;

            }
            return (false, null);
        }
    }
}
