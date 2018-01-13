using System.Collections.Generic;

namespace AdmiraltySimulator
{
    public static class Combination
    {
        public static List<List<T>> Combinations<T>(this List<T> elements, int k)
        {
            var combs = new List<List<T>>();

            if (k < 0 || k > elements.Count)
                return combs;

            if (elements.Count == 0)
            {
                combs.Add(new List<T>());
                return combs;
            }

            var reverseMode = k > elements.Count - elements.Count / 2;
            var reverseK = elements.Count - k;

            var indexes = new List<int>();

            for (var i = 0; i < (reverseMode ? reverseK : k); i++)
                indexes.Add(i);

            do
            {
                var comb = new List<T>();

                if (reverseMode)
                {
                    var currExceptIdx = 0;

                    for (var i = 0; i < elements.Count; i++)
                        if (currExceptIdx >= indexes.Count || i != indexes[currExceptIdx])
                            comb.Add(elements[i]);
                        else
                            currExceptIdx++;
                }
                else
                {
                    foreach (var idx in indexes)
                        comb.Add(elements[idx]);
                }

                combs.Add(comb);
            } while (NextCombIdx(indexes, elements.Count, (reverseMode ? reverseK : k) - 1));

            return combs;
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