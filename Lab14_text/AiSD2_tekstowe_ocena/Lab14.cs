using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labratoria_ASD2_2024
{
    public class Lab14 : MarshalByRefObject
    {
        /// <summary>
        /// Znajduje wszystkie maksymalne palindromy długości przynajmniej 2 w zadanym słowie. Wykorzystuje Algorytm Manachera.
        /// 
        /// Palindromy powinny być zwracane jako lista par (indeks pierwszego znaku, długość palindromu), 
        /// tzn. para (i, d) oznacza, że pod indeksem i znajduje się pierwszy znak d-znakowego palindromu.
        /// 
        /// Kolejność wyników nie ma znaczenia.
        /// 
        /// Można założyć, że w tekście wejściowym nie występują znaki '#' i '$' - można je wykorzystać w roli wartowników
        /// </summary>
        /// <param name="text">Tekst wejściowy</param>
        /// <returns>Tablica znalezionych palindromów</returns>


        public (int startIndex, int length)[] FindPalindromes(string textIn)
        {
            string text = "#" + textIn + "$";
            int[] evenPalindromes = FindEvenPalindromes(text);
            int[] oddPalindromes = FindOddPalindromes(text);

            List<(int, int)> ret = new List<(int, int)>();

            for (int ind = 1; ind <= textIn.Length; ind++)
            { 
                    if (oddPalindromes[ind] != 0)
                    {
                        ret.Add((ind - oddPalindromes[ind] - 1, 2 * oddPalindromes[ind] + 1));
                    }
                    if (evenPalindromes[ind] != 0)
                    {
                        ret.Add((ind - evenPalindromes[ind] - 1, 2 * evenPalindromes[ind]));
                    }
            }
            return ret.ToArray();

        }

        private int[] FindEvenPalindromes(string text)
        {
            int[] evenPalindromes = new int[text.Length - 1];
            int i = 1;
            int r = 0;
            evenPalindromes[0] = 0;
            while (i <= (text.Length-2))
            {
                while (text[i - r - 1] == text[i + r])
                {
                    r++;
                }
                int offset = 1;
                evenPalindromes[i] = r;
                while (offset < r && evenPalindromes[i - offset] != r - offset)
                {
                    evenPalindromes[i + offset] = Math.Min(evenPalindromes[i - offset], r - offset);
                    offset++;
                }
                r = Math.Max(r - offset, 0);
                i += offset;
            }
            return evenPalindromes;
        }
        private int[] FindOddPalindromes(string text)
        {
            int[] oddPalindromes = new int[text.Length - 1];
            int i = 1;
            int r = 0;
            oddPalindromes[0] = 0;
            while (i <= (text.Length - 2))
            {
                while (text[i - r - 1] == text[i + r + 1])
                {
                    r++;
                }
                int offset = 1;
                oddPalindromes[i] = r;
                while (offset < r && oddPalindromes[i - offset] != r - offset)
                {
                    oddPalindromes[i + offset] = Math.Min(oddPalindromes[i - offset], r - offset);
                    offset++;
                }
                r = Math.Max(r - offset, 0);
                i += offset;
            }
            return oddPalindromes;
        }
    }

   
}
