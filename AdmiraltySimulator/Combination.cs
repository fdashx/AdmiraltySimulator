using System;
using System.Collections.Generic;
using System.Linq;

namespace AdmiraltySimulator
{
    public static class Combination
    {
        public static long Coefficient(int n, int k)
        {
            if (k < 0 || k > n) return 0;
            if (k == 0 || k == n) return 1;

            // Optimize using symmetry: Choose(100, 97) is the same as Choose(100, 3)
            if (k > n - k)
            {
                k = n - k;
            }

            long result = 1;

            for (var i = 1; i <= k; i++)
            {
                result = result * n-- / i;
            }

            return result;
        }

        public static IEnumerable<T[]> Combinations<T>(this ICollection<T> elements, int k)
        {
            if (k < 0 || k > elements.Count)
                yield break;

            if (elements.Count == 0)
            {
                yield return Array.Empty<T>();
                yield break;
            }

            var reverseMode = k > elements.Count - elements.Count / 2;
            var reverseK = elements.Count - k;

            var indexes = Enumerable.Range(0, reverseMode ? reverseK : k).ToArray();
            var elementsList = elements.ToArray();

            do
            {
                var comb = new T[k];
                var combIdx = 0;

                if (reverseMode)
                {
                    var currExceptIdx = 0;

                    for (var i = 0; i < elements.Count; i++)
                    {
                        if (currExceptIdx >= indexes.Length || i != indexes[currExceptIdx])
                        {
                            comb[combIdx] = elementsList[i];
                            combIdx++;
                        }
                        else
                        {
                            currExceptIdx++;
                        }
                    }
                }
                else
                {
                    foreach (var idx in indexes)
                    {
                        comb[combIdx] = elementsList[idx];
                        combIdx++;
                    }
                }

                yield return comb;
            } while (NextCombIdx(indexes, elements.Count, (reverseMode ? reverseK : k) - 1));
        }

        private static bool NextCombIdx(int[] indexes, int elementCount, int currIdx)
        {
            if (currIdx == -1)
                return false;

            if (indexes[currIdx] == elementCount - indexes.Length + currIdx)
            {
                if (!NextCombIdx(indexes, elementCount, currIdx - 1))
                    return false;

                for (var j = currIdx; j < indexes.Length; j++)
                    indexes[j] = indexes[j - 1] + 1;
            }
            else
            {
                indexes[currIdx]++;
            }

            return true;
        }
    }
}