using System;
using System.Collections.Generic;

namespace Ais.src
{
    static class Generator
    {
        static readonly Random rand = new Random();

        internal static List<char> GetSequence(int len, string[] ranges) {
            List<char> seq = new List<char>();
            double ep = 1.0 / ranges.Length;
            double prob;

            for (int i = 0; i < len; i++) {
                prob = rand.NextDouble();

                for (int j = 0; j < ranges.Length; j++) {
                    if (prob >= j * ep && prob < (j + 1) * ep) {
                        seq.Add(GetRandChar(ranges[j]));

                        break;
                    }
                }
            }

            return seq;
        }

        static char GetRandChar(string range) {
            return (char) rand.Next(range[0], range[2] + 1);
        }
    }
}
