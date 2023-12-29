using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;

internal class Program
{

    private static ulong pow(ulong x, uint y)
    {
        ulong result = x;
        for (uint i = 1; i < y; ++i)
        {
            result *= x;
        }
        return result;
    }

    const string dllname = "Calculator.dll";
    private static void Main(string[] args)
    {
        write();

    }

        [DllImport(dllname)] static extern void test();
    [DllImport(dllname)] static extern void write();

        /// <summary>
        /// Calculates the amount of credits earned at a given rank.
        /// </summary>
        /// <param name="rank">Desired rank to convert.</param>
        /// <returns>The credits obtained after hitting the given rank.</returns>
        //  extern "C" DLLEXPORT unsigned long rankToCredits(unsigned long rank)
        [DllImport(dllname)] static extern ulong rankToCredits(ulong rank);

        /// <summary>
        /// Calculates the rank that achieves the given credits.
        /// </summary>
        /// <param name="credits">The credits to convert.</param>
        /// <returns>The rank that achieves the given rank.</returns>
        //  extern "C" DLLEXPORT unsigned long creditsToRank(unsigned long credits)
        [DllImport(dllname)] static extern ulong creditsToRank(ulong credits);

        /// <summary>
        /// Calculates the sum of all of the credits earned between a range between a given start and end rank.
        /// </summary>
        /// <param name="startRank">The start rank.</param>
        /// <param name="endRank">The end rank.</param>
        /// <returns>Returns the sum of all the credits earned in the range defined by the start and end ranks.</returns>
        //  extern "C" DLLEXPORT unsigned long rankToCreditsSummation(unsigned long startRank, unsigned long endRank)
        [DllImport(dllname)] static extern ulong rankToCreditsSummation(ulong startRank, ulong endRank);

        /// <summary>
        /// Calculates the rank required to achieve the target experience.
        /// </summary>
        /// <param name="targetXP">Desired experience to convert.</param>
        /// <returns>The rank that will have the given experience.</returns>
        //  extern "C" DLLEXPORT unsigned long experienceToRank(unsigned long targetXP)
        [DllImport(dllname)] static extern ulong experienceToRank(ulong targetXP);

        /// <summary>
        /// Calculatest the experience gained from rank 0 to targetRank.
        /// </summary>
        /// <param name="targetRank">The target rank.</param>
        /// <returns>Returns the experience gained or needed to go from rank 0 to targetRank.</returns>
        //  extern "C" DLLEXPORT unsigned long rankToExperienceDefault(unsigned long targetRank)
        [DllImport(dllname)] static extern ulong rankToExperienceDefault(ulong targetRank);

        /// <summary>
        /// Calculates the experience gained from a startRank to a targetRank
        /// </summary>
        /// <param name="startRank">The rank to start from.</param>
        /// <param name="startRankProgress">The progress within the rank. Needs to be less than (startRank + 1)*1000.</param>
        /// <param name="targetRank">The rank to end at.</param>
        /// <returns>Returns the experience gained or needed to go from startRank to targetRank.</returns>
        //  extern "C" DLLEXPORT unsigned long rankToExperienceGeneral(unsigned long startRank, unsigned long startRankProgress, unsigned long targetRank)
        [DllImport(dllname)] static extern ulong rankToExperienceGeneral(ulong startRank, ulong startRankProgress, ulong targetRank);

    /*

/// <summary>
/// Calculates the amount of credits earned at a given rank.
/// </summary>
/// <param name="rank">Desired rank to convert.</param>
/// <returns>The credits obtained after hitting the given rank.</returns>
UInt128 rankToCreditsL(UInt128 rank) {
	//cannot be negative, cannot be bigger than unsigned long max
	return 200 + 5 * rank;
	//if this result is bigger than ulong max, try to promote to unsigned long long
}

/// <summary>
/// Calculates the rank that achieves the given credits.
/// </summary>
/// <param name="credits">The credits to convert.</param>
/// <returns>The rank that achieves the given rank.</returns>
UInt128 creditsToRankL(UInt128 credits) {
	//must be a multiple of 5, cannot be negative, cannot be bigger than ulong max
	for (UInt128 i{ 0 }; i <= credits; ++i) {
		if (i == credits) return (i - 200) / 5;
	}
}

/// <summary>
/// Calculates the sum of all of the credits earned between a range between a given start and end rank.
/// </summary>
/// <param name="startRank">The start rank.</param>
/// <param name="endRank">The end rank.</param>
/// <returns>Returns the sum of all the credits earned in the range defined by the start and end ranks.</returns>
extern "C" DLLEXPORT unsigned long long rankToCreditsSummationL(unsigned long long startRank, unsigned long long endRank) {
	//endRank must be greater than startRank, neither can be negative
	unsigned long long summation{ 0 };
	for (unsigned long long i{ startRank }; i < endRank; ++i) { //this might be i <= endRank
		summation += creditsToRank(endRank);
		if (i == endRank) {
			return summation;
		}
	}
	return summation;
}

/// <summary>
/// Calculates the rank required to achieve the target experience.
/// </summary>
/// <param name="targetXP">Desired experience to convert.</param>
/// <returns>The rank that will have the given experience.</returns>
extern "C" DLLEXPORT unsigned long long experienceToRankL(unsigned long long targetXP) {
	unsigned long long counter{ 0 };
	for (unsigned long long i = 0; i < targetXP; i++) {
		//rank 1 has 1000xp, rank 2 has rank 1 + 2000 = 3000, rank 3 has rank 2 + rank 1 + 3000 = 6000
		//counter actually has the total xp for each rank here
		counter += i * 1000;
		//checks if the counter is just less than the target xp, but adding one more rank would cause it to go over the target xp
		if (counter + ((i + 1) * 1000) > targetXP && counter <= targetXP) {
			return i;
			break;
		}
	}
}

/// <summary>
/// Calculates the experience gained from a startRank to a targetRank
/// </summary>
/// <param name="startRank">The rank to start from.</param>
/// <param name="startRankProgress">The progress within the rank. Needs to be less than (startRank + 1)*1000.</param>
/// <param name="targetRank">The rank to end at.</param>
/// <returns>Returns the experience gained or needed to go from startRank to targetRank.</returns>
extern "C" DLLEXPORT unsigned long long rankToExperienceGeneralL(unsigned long long startRank, unsigned long long startRankProgress, unsigned long long targetRank) {
	unsigned long long requirement{ 0 };
	for (unsigned long long i = 0; i < targetRank; i++)requirement += i * 1000;
	for (unsigned long long j = 0; j < startRank; j++)requirement -= j * 1000;
	requirement -= startRankProgress;
	return requirement;
}

/// <summary>
/// Calculatest the experience gained from rank 0 to targetRank.
/// </summary>
/// <param name="targetRank">The target rank.</param>
/// <returns>Returns the experience gained or needed to go from rank 0 to targetRank.</returns>
extern "C" DLLEXPORT unsigned long long rankToExperienceDefaultL(unsigned long long targetRank) {
	return rankToExperienceGeneral(0, 0, targetRank);
}
*/
}