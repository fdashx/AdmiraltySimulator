using System.Collections.Generic;

namespace AdmiraltySimulator
{
    public static class Combination
    {
        public static IEnumerable<List<T>> Combinations<T>(this ICollection<T> elements, int k)
        {
            if (k < 0 || k > elements.Count)
                yield break;

            if (elements.Count == 0)
            {
                yield return new List<T>();
                yield break;
            }

            var reverseMode = k > elements.Count - elements.Count / 2;
            var reverseK = elements.Count - k;

            var indexes = new List<int>();

            for (var i = 0; i < (reverseMode ? reverseK : k); i++)
                indexes.Add(i);

            var elementsList = new List<T>(elements);

            do
            {
                var comb = new List<T>();

                if (reverseMode)
                {
                    var currExceptIdx = 0;

                    for (var i = 0; i < elements.Count; i++)
                        if (currExceptIdx >= indexes.Count || i != indexes[currExceptIdx])
                            comb.Add(elementsList[i]);
                        else
                            currExceptIdx++;
                }
                else
                {
                    foreach (var idx in indexes)
                        comb.Add(elementsList[idx]);
                }

                yield return comb;
            } while (NextCombIdx(indexes, elements.Count, (reverseMode ? reverseK : k) - 1));
        }

        private static bool NextCombIdx(IList<int> indexes, int elementCount, int currIdx)
        {
            if (currIdx == -1)
                return false;

            if (indexes[currIdx] == elementCount - indexes.Count + currIdx)
            {
                if (!NextCombIdx(indexes, elementCount, currIdx - 1))
                    return false;

                for (var j = currIdx; j < indexes.Count; j++)
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