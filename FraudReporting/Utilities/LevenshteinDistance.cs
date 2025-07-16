using System.Linq;
using System;

namespace FraudReporting.Utilities
{
    public static class LevenshteinDistance
    {
        public static float GetStringSimilarity(string a, string b)
        {
            if (a == b)
                return 1;

            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return 0;

            if (Math.Abs(a.Length - b.Length) > 5)
                return 0;

            double dist = ComputeLevenshteinDistance(a.ToArray(), b.ToArray());

            return 1f - (float)(dist / Math.Max(a.Length, b.Length));
        }

        private static int ComputeLevenshteinDistance<T>(T[] source, T[] target)
        {
            if (source.GetType() != target.GetType())
                return -1;

            if (source == null)
                return (target == null) ? 0 : target.Length;

            if (target == null)
                return (source == null) ? 0 : source.Length;

            int sourceLength = source.Length;
            int targetLength = target.Length;

            int[,] distance = new int[sourceLength + 1, targetLength + 1];

            for (int i = 0; i <= sourceLength; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetLength; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceLength; i++)
            {
                for (int j = 1; j <= targetLength; j++)
                {
                    int cost = (target[j - 1].Equals(source[i - 1])) ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(
                                     distance[i - 1, j] + 1, //deletion operation
                                     distance[i, j - 1] + 1), //insertion operation
                                     distance[i - 1, j - 1] + cost); //replacement operation
                }
            }

            return distance[sourceLength, targetLength];
        }
    }
}
