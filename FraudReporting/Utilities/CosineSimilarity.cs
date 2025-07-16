using System.Linq;
using System;

namespace FraudReporting.Utilities
{
    public static class CosineSimilarity
    {
        public static float GetWordSimilarity(string a, string b, string separator = " ", Func<string, float>? weightFunction = null)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return -1;

            if (a == b)
                return 1;

            string[] aArray = a.ToUpper().Split(separator);
            string[] bArray = b.ToUpper().Split(separator);
            string[] dictionary = aArray.Union(bArray).ToArray();

            return ComputeCosineSimilarity(aArray, bArray, dictionary, weightFunction);
        }

        private static float ComputeCosineSimilarity<T>(T[] a, T[] b, T[] dictionary, Func<T, float>? weightFunction = null)
        {
            if (a.GetType() != b.GetType() || a.GetType() != dictionary.GetType() || b.GetType() != dictionary.GetType())
                return -1;

            float[] aVector = new float[dictionary.Length];
            float[] bVector = new float[dictionary.Length];

            float dotProduct = 0;
            float aMagnitude = 0;
            float bMagnitude = 0;

            for (int i = 0; i < dictionary.Length; i++)
            {
                T e = dictionary[i];
                aVector[i] = (a.Contains(e)) ? ((weightFunction != null) ? weightFunction(e) : 1) : 0;
                bVector[i] = (b.Contains(e)) ? ((weightFunction != null) ? weightFunction(e) : 1) : 0;

                dotProduct += aVector[i] * bVector[i];
                aMagnitude += (aVector[i] * aVector[i]);
                bMagnitude += (bVector[i] * bVector[i]);
            }

            aMagnitude = Sqrt(aMagnitude);
            bMagnitude = Sqrt(bMagnitude);

            return dotProduct / (aMagnitude * bMagnitude);
        }

        private static float Sqrt(float x, float? prediction = null, bool lazyExecution = true)
        {
            float thisPrediction = prediction.GetValueOrDefault(x / 2);
            float result = x / thisPrediction;
            float average = (thisPrediction + result) / 2;

            if (lazyExecution)
            {
                average = MathF.Round(average, 2);
                thisPrediction = MathF.Round(thisPrediction, 2);
            }

            if (average == thisPrediction)
                return average;
            else
                return Sqrt(x, average, lazyExecution);
        }
    }
}
