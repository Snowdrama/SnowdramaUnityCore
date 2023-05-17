using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Core
{
    public class Noise : MonoBehaviour
    {
        /// <summary>
        /// A C# implementatiion based on the example hash function
        /// in Squirrel Eiserloh's GDC Talk 
        /// https://www.youtube.com/watch?v=LWFzPP8ZbdU
        /// 
        /// Not cryptographically safe!
        /// 
        /// if It sucks or doesn't produce good results it's 100% my implementation
        /// and not their algorithm
        /// </summary>
        /// <param name="position">Some positional value in the noise array</param>
        /// <param name="seed">The noise functions seed</param>
        /// <returns></returns>
        public static uint Squirrel3(int position, uint seed = 69420)
        {
            const uint NOISE_1 = 0xB5297A4D;
            const uint NOISE_2 = 0x68E31DA4;
            const uint NOISE_3 = 0x1B56C4E9;

            uint mangled = (uint)position;
            mangled *= NOISE_1;
            mangled += seed;
            mangled ^= mangled >> 8;
            mangled *= NOISE_2;
            mangled ^= mangled << 8;
            mangled *= NOISE_3;
            mangled ^= mangled >> 8;
            return mangled;
        }

        public static int Squirrel3Range(int min, int max, int position, uint seed = 69420)
        {
            uint value = Squirrel3(position, seed);

            uint difference = (uint)Mathf.Abs(min - max);

            uint mod = value % difference;

            return (int)mod + min;
        }

        public static float NormalizedSquirrel3(int position, uint seed = 69420)
        {
            uint value = Squirrel3(position, seed);
            //do the calculation in double precision
            double calc = (double)value / uint.MaxValue;
            //cast to float
            return (float)calc;
        }

        public static uint Squirrel3_2D(int x, int y, uint seed = 69420)
        {
            uint xValue = Squirrel3(x, seed);
            uint yValue = Squirrel3(y, seed);

            //??? no clue how good a value this returns
            return xValue ^ yValue;
        }

        public static uint Squirrel3_2D(int x, int y, int z, uint seed = 69420)
        {
            uint xValue = Squirrel3(x, seed);
            uint yValue = Squirrel3(y, seed);
            uint zValue = Squirrel3(z, seed);

            //??? no clue how good a value this returns
            return xValue ^ yValue ^ zValue;
        }



        /// <summary>
        /// A modified 64 bit version of the Squirrel3 hash function from:
        /// Squirrel Eiserloh's GDC Talk 
        /// https://www.youtube.com/watch?v=LWFzPP8ZbdU
        /// 
        /// Not cryptographically safe!
        /// 
        /// Totally untested on how random it actually is, how performant it is
        /// or how quality the random is.
        /// 
        /// This function is intended to be a joke... unless it works...
        /// </summary>
        /// <param name="position">Some positional value in the noise array</param>
        /// <param name="seed">The noise functions seed</param>
        /// <returns></returns>
        public static long Squirrel64(int position, long seed = 69420717580085)
        {
            const long NOISE_1 = 0xB5297A4D;
            const long NOISE_2 = 0x68E31DA4;
            const long NOISE_3 = 0x1B56C4E9;

            long mangled = (long)position;
            mangled *= NOISE_1;
            mangled += seed;
            mangled ^= mangled >> 16;
            mangled *= NOISE_2;
            mangled ^= mangled << 16;
            mangled *= NOISE_3;
            mangled ^= mangled >> 16;
            return mangled;
        }
    }

}