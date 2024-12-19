using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Lab02 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - wyznaczenie najtańszej trasy, zgodnie z którą pionek przemieści się z pozycji poczatkowej (0,0) na pozycję docelową
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="moves">tablica z dostępnymi ruchami i ich kosztami (di - o ile zwiększamy numer wiersza, dj - o ile zwiększamy numer kolumnj, cost - koszt ruchu)</param>
        /// <returns>(bool result, int cost, (int, int)[] path) - result ma wartość true jeżeli trasa istnieje, false wpp., cost to minimalny koszt, path to wynikowa trasa</returns>
        public (bool result, int cost, (int i, int j)[] path) Lab02Stage1(int n, int m, ((int di, int dj) step, int cost)[] moves)
        { 
            if(n==1)
            {
                (int, int)[] path_ret = new (int, int)[1];
                path_ret[0] = (0, 0);
                return (true, 0, path_ret);
            }

            int[,] tab = new int[n, m];
            (int i, int j)[,] path_reverse = new (int i, int j)[n, m];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    tab[i, j] = int.MaxValue;
                }
            }
            tab[0, 0] = 0;

            for (int i = 0; i < moves.Length; i++)
            {
                for (int k = 0; k < n; k++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        int pom_k = k + moves[i].step.di;
                        int pom_j = j + moves[i].step.dj;
                        if (tab[k, j] != int.MaxValue && pom_k < n && pom_j < m && tab[pom_k, pom_j] > tab[k, j] + moves[i].cost)
                        {
                            tab[pom_k, pom_j] = tab[k, j] + moves[i].cost;
                            path_reverse[pom_k, pom_j] = (moves[i].step.di, moves[i].step.dj);
                        }
                    }
                }
            }

            int min = int.MaxValue;
            int min_j = int.MaxValue;
            for (int i = 0; i < m; i++)
            {
                if (min > tab[n - 1, i])
                {
                    min = tab[n - 1, i];
                    min_j = i;
                }
            }

            if (min == int.MaxValue)
            {
                return (false, int.MaxValue, null);
            }

            int indx_i = n - 1;
            int indx_j = min_j;
            List<(int i, int j)> path_pom = new List<(int i, int j)>();

            while (true)
            { 
                path_pom.Insert(0, (indx_i, indx_j));
                int indx_pom = indx_i;
                indx_i -= path_reverse[indx_i, indx_j].i;
                indx_j -= path_reverse[indx_pom, indx_j].j;
                if (path_reverse[indx_i, indx_j].i == 0 && path_reverse[indx_pom, indx_j].j == 0)
                {
                    break;
                }
            }
            
            path_pom.Insert(0, (0, 0));
            (int i, int j)[] path = path_pom.ToArray(); 
            return (true, min, path);
        }


        /// <summary>
        /// Etap 2 - wyznaczenie najtańszej trasy, zgodnie z którą pionek przemieści się z pozycji poczatkowej (0,0) na pozycję docelową - dodatkowe założenie, każdy ruch może być wykonany co najwyżej raz
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="moves">tablica z dostępnymi ruchami i ich kosztami (di - o ile zwiększamy numer wiersza, dj - o ile zwiększamy numer kolumnj, cost - koszt ruchu)</param>
        /// <returns>(bool result, int cost, (int, int)[] path) - result ma wartość true jeżeli trasa istnieje, false wpp., cost to minimalny koszt, path to wynikowa trasa</returns>
        public (bool result, int cost, (int i, int j)[] pat) Lab02Stage2(int n, int m, ((int di, int dj) step, int cost)[] moves)
        {
            int[,,] tab = new int[n, m, moves.Length+1];
            (int x, int y, int z)[,,]tab_pom = new (int, int,int)[n,m, moves.Length+1];
            for(int x = 0; x<n; x++)
            {
                for(int y = 0; y<m; y++)
                {
                    for(int z = 0; z<moves.Length+1; z++)
                    {
                        tab[x, y, z] = int.MaxValue;
                    }
                }
            }
            tab[0, 0, 0] = 0;

            for(int z = 1; z<moves.Length+1;z++)
            {
                for(int x = 0; x<n; x++)
                {
                    for(int y = 0;y<m;y++)
                    {
                        tab[x, y, z] = tab[x, y, z - 1];
                        tab_pom[x, y, z] = tab_pom[x, y, z - 1];
                    }
                }

                for(int x = 0; x<n; x++)
                {
                    for(int y = 0; y<m; y++)
                    {
                        int indx_x = x + moves[z-1].step.di;
                        int indx_y = y + moves[z-1].step.dj;
                        if (tab[x,y,z-1]!=int.MaxValue && indx_x<n && indx_y<m && tab[indx_x, indx_y, z-1] > tab[x,y,z-1] + moves[z-1].cost)
                        {
                            tab[indx_x, indx_y, z] = tab[x, y, z-1] + moves[z-1].cost;
                            tab_pom[indx_x, indx_y, z] = (x, y, z-1);
                        }
                    }
                }
            }

            int min = int.MaxValue;
            int min_y = -1;
            for(int i = 0; i<m; i++)
            {
                if (tab[n-1,i,moves.Length]<min)
                {
                    min = tab[n-1, i, moves.Length];
                    min_y = i;
                }
            }

            if (min == int.MaxValue)
            {
                return (false, int.MaxValue, null);
            }

            List<(int x, int y)> list = new List<(int x, int y)>();
            list.Insert(0, (n - 1, min_y));
            int ind_x = n - 1;
            int ind_y = min_y;
            int ind_z = moves.Length;

            while(ind_x>0 || ind_y>0)
            {
                list.Insert(0, (tab_pom[ind_x, ind_y, ind_z].x, tab_pom[ind_x, ind_y, ind_z].y));
                int x_prev = ind_x;
                int y_prev = ind_y;
                ind_x = tab_pom[ind_x, ind_y, ind_z].x;
                ind_y = tab_pom[x_prev, ind_y, ind_z].y;
                ind_z = tab_pom[x_prev, y_prev, ind_z].z;
            }

            (int x, int y)[] path = list.ToArray();
            return (true, min, path);

        }
    }
}