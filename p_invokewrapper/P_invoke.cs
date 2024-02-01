using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;

namespace PFDB
{
    namespace pinvoke { 
        public class PINVOKE
        {

            const string dllname = "Calculator.dll";
            public static void Main(string[] args)
            {
                write();
                Console.WriteLine(rankToCreditsSummation(0, 2));
                Console.WriteLine(returnString());

                Console.WriteLine(experienceToRank(rankToExperienceDefault(213) + rankToExperienceDefault(173)));

                Console.WriteLine(rankToCreditsSummation(231, 324));

            }

            [DllImport(dllname)] static extern void test();
            [DllImport(dllname)] static extern void write();
            [DllImport(dllname)]
            [return: MarshalAs(UnmanagedType.BStr)]
            public static extern string returnString();

            /// <summary>Calculates the amount of credits earned at a given rank.</summary>
            /// <param name="rank">Desired rank to convert.</param>
            /// <returns>The credits obtained after hitting the given rank.</returns>
            //  extern "C" DLLEXPORT unsigned long rankToCredits(unsigned long rank)
            [DllImport(dllname)] public static extern ulong rankToCredits(ulong rank);

            /// <summary>Calculates the rank that achieves the given credits.</summary>
            /// <param name="credits">The credits to convert.</param>
            /// <returns>The rank that achieves the given rank.</returns>
            //  extern "C" DLLEXPORT unsigned long creditsToRank(unsigned long credits)
            [DllImport(dllname)] static extern ulong creditsToRank(ulong credits);

            /// <summary>Calculates the sum of all of the credits earned between a range between a given start and end rank.</summary>
            /// <param name="startRank">The start rank.</param>
            /// <param name="endRank">The end rank.</param>
            /// <returns>Returns the sum of all the credits earned in the range defined by the start and end ranks.</returns>
            //  extern "C" DLLEXPORT unsigned long rankToCreditsSummation(unsigned long startRank, unsigned long endRank)
            [DllImport(dllname)] static extern ulong rankToCreditsSummation(ulong startRank, ulong endRank);

            /// <summary>Calculates the rank required to achieve the target experience.</summary>
            /// <param name="targetXP">Desired experience to convert.</param>
            /// <returns>The rank that will have the given experience.</returns>
            //  extern "C" DLLEXPORT unsigned long experienceToRank(unsigned long targetXP)
            [DllImport(dllname)] public static extern ulong experienceToRank(ulong targetXP);

            /// <summary>Calculates the experience gained from rank 0 to targetRank.</summary>
            /// <param name="targetRank">The target rank.</param>
            /// <returns>Returns the experience gained or needed to go from rank 0 to targetRank.</returns>
            //  extern "C" DLLEXPORT unsigned long rankToExperienceDefault(unsigned long targetRank)
            [DllImport(dllname)] public static extern ulong rankToExperienceDefault(ulong targetRank);

            /// <summary>Calculates the experience gained from a startRank to a targetRank</summary>
            /// <param name="startRank">The rank to start from.</param>
            /// <param name="startRankProgress">The progress within the rank. Needs to be less than (startRank + 1)*1000.</param>
            /// <param name="targetRank">The rank to end at.</param>
            /// <returns>Returns the experience gained or needed to go from startRank to targetRank.</returns>
            //  extern "C" DLLEXPORT unsigned long rankToExperienceGeneral(unsigned long startRank, unsigned long startRankProgress, unsigned long targetRank)
            [DllImport(dllname)] public static extern ulong rankToExperienceGeneral(ulong startRank, ulong startRankProgress, ulong targetRank);

        }
    }
}