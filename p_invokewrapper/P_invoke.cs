using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;

const string dllname = "Calculator.dll";
test();
Console.WriteLine();
Console.WriteLine(rankToExperienceGeneral(319,260000,400));



[DllImport(dllname)] static extern void test();


/// <summary>
/// Calculates the amount of credits earned at a given rank.
/// </summary>
/// <param name="rank">Desired rank to convert.</param>
/// <returns>The credits obtained after hitting the given rank.</returns>
//  extern "C" DLLEXPORT unsigned long rankToCredits(unsigned long rank)
[DllImport(dllname)] static extern UInt64 rankToCredits(UInt64 rank);

/// <summary>
/// Calculates the rank that achieves the given credits.
/// </summary>
/// <param name="credits">The credits to convert.</param>
/// <returns>The rank that achieves the given rank.</returns>
//  extern "C" DLLEXPORT unsigned long creditsToRank(unsigned long credits)
[DllImport(dllname)] static extern UInt64 creditsToRank(UInt64 credits);

/// <summary>
/// Calculates the sum of all of the credits earned between a range between a given start and end rank.
/// </summary>
/// <param name="startRank">The start rank.</param>
/// <param name="endRank">The end rank.</param>
/// <returns>Returns the sum of all the credits earned in the range defined by the start and end ranks.</returns>
//  extern "C" DLLEXPORT unsigned long rankToCreditsSummation(unsigned long startRank, unsigned long endRank)
[DllImport(dllname)] static extern UInt64 rankToCreditsSummation(UInt64 startRank, UInt64 endRank);

/// <summary>
/// Calculates the rank required to achieve the target experience.
/// </summary>
/// <param name="targetXP">Desired experience to convert.</param>
/// <returns>The rank that will have the given experience.</returns>
//  extern "C" DLLEXPORT unsigned long experienceToRank(unsigned long targetXP)
[DllImport(dllname)] static extern UInt64 experienceToRank(UInt64 targetXP);

/// <summary>
/// Calculatest the experience gained from rank 0 to targetRank.
/// </summary>
/// <param name="targetRank">The target rank.</param>
/// <returns>Returns the experience gained or needed to go from rank 0 to targetRank.</returns>
//  extern "C" DLLEXPORT unsigned long rankToExperienceDefault(unsigned long targetRank)
[DllImport(dllname)] static extern UInt64 rankToExperienceDefault(UInt64 targetRank);

/// <summary>
/// Calculates the experience gained from a startRank to a targetRank
/// </summary>
/// <param name="startRank">The rank to start from.</param>
/// <param name="startRankProgress">The progress within the rank. Needs to be less than (startRank + 1)*1000.</param>
/// <param name="targetRank">The rank to end at.</param>
/// <returns>Returns the experience gained or needed to go from startRank to targetRank.</returns>
//  extern "C" DLLEXPORT unsigned long rankToExperienceGeneral(unsigned long startRank, unsigned long startRankProgress, unsigned long targetRank)
[DllImport(dllname)] static extern UInt64 rankToExperienceGeneral(UInt64 startRank, UInt64 startRankProgress, UInt64 targetRank);


/// <summary>
/// Calculates the amount of credits earned at a given rank.
/// </summary>
/// <param name="rank">Desired rank to convert.</param>
/// <returns>The credits obtained after hitting the given rank.</returns>
//  extern "C" DLLEXPORT unsigned long rankToCredits(unsigned long rank)
[DllImport(dllname)] static extern UInt128 rankToCreditsL(UInt128 rank);


/// <summary>
/// Calculates the rank that achieves the given credits.
/// </summary>
/// <param name="credits">The credits to convert.</param>
/// <returns>The rank that achieves the given rank.</returns>
//  extern "C" DLLEXPORT unsigned long creditsToRank(unsigned long credits)
[DllImport(dllname)] static extern UInt128 creditsToRankL(UInt128 credits);

/// <summary>
/// Calculates the sum of all of the credits earned between a range between a given start and end rank.
/// </summary>
/// <param name="startRank">The start rank.</param>
/// <param name="endRank">The end rank.</param>
/// <returns>Returns the sum of all the credits earned in the range defined by the start and end ranks.</returns>
//  extern "C" DLLEXPORT unsigned long rankToCreditsSummation(unsigned long startRank, unsigned long endRank)
[DllImport(dllname)] static extern UInt128 rankToCreditsSummationL(UInt128 startRank, UInt128 endRank);

/// <summary>
/// Calculates the rank required to achieve the target experience.
/// </summary>
/// <param name="targetXP">Desired experience to convert.</param>
/// <returns>The rank that will have the given experience.</returns>
//  extern "C" DLLEXPORT unsigned long experienceToRank(unsigned long targetXP)
[DllImport(dllname)] static extern UInt128 experienceToRankL(UInt128 targetXP);

/// <summary>
/// Calculatest the experience gained from rank 0 to targetRank.
/// </summary>
/// <param name="targetRank">The target rank.</param>
/// <returns>Returns the experience gained or needed to go from rank 0 to targetRank.</returns>
//  extern "C" DLLEXPORT unsigned long rankToExperienceDefault(unsigned long targetRank)
[DllImport(dllname)] static extern UInt128 rankToExperienceDefaultL(UInt128 targetRank);

/// <summary>
/// Calculates the experience gained from a startRank to a targetRank
/// </summary>
/// <param name="startRank">The rank to start from.</param>
/// <param name="startRankProgress">The progress within the rank. Needs to be less than (startRank + 1)*1000.</param>
/// <param name="targetRank">The rank to end at.</param>
/// <returns>Returns the experience gained or needed to go from startRank to targetRank.</returns>
//  extern "C" DLLEXPORT unsigned long rankToExperienceGeneral(unsigned long startRank, unsigned long startRankProgress, unsigned long targetRank)
[DllImport(dllname)] static extern UInt128 rankToExperienceGeneralL(UInt128 startRank, UInt128 startRankProgress, UInt128 targetRank);