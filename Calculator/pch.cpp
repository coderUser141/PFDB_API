// pch.cpp: source file corresponding to the pre-compiled header

#include "pch.h"
#define DLLEXPORT __declspec(dllexport)


// When you are using pre-compiled headers, this source file is necessary for compilation to succeed.

extern "C" DLLEXPORT int test() {
	std::cout << "your mom" << std::endl; return 6;
}

/// <summary>
/// Calculates the amount of credits earned at a given rank.
/// </summary>
/// <param name="rank">Desired rank to convert.</param>
/// <returns>The credits obtained after hitting the given rank.</returns>
extern "C" DLLEXPORT unsigned long long rankToCredits(unsigned long long rank) {
	//cannot be negative, cannot be bigger than unsigned long max
	if (((UINT64_MAX - 201) / 5 ) < rank || rank < 0) return UINT64_MAX; //avoid int overflow
	if (rank == 0)return 0;
	return 200 + 5*rank;
}

/// <summary>
/// Calculates the rank that achieves the given credits.
/// </summary>
/// <param name="credits">The credits to convert.</param>
/// <returns>The rank that achieves the given rank.</returns>
extern "C" DLLEXPORT unsigned long long creditsToRank(unsigned long long credits) {
	//must be a multiple of 5, cannot be negative, cannot be bigger than ulong max
	if (credits < 0 || credits % 5 != 0) return UINT64_MAX;
	return (credits - (unsigned long long)200) / (unsigned long long)5;
}

/// <summary>
/// Calculates the sum of all of the credits earned between a range between a given start and end rank.
/// </summary>
/// <param name="startRank">The start rank.</param>
/// <param name="endRank">The end rank.</param>
/// <returns>Returns the sum of all the credits earned in the range defined by the start and end ranks.</returns>
extern "C" DLLEXPORT unsigned long long rankToCreditsSummation(unsigned long long startRank, unsigned long long endRank) {
	//endRank must be greater than startRank, neither can be negative
	if (endRank < startRank) return UINT64_MAX;
	if (endRank == startRank)return 0;
	//to check if the intermediate calculation will exceed the uint64 limit
	if (startRank * (startRank - 1) > UINT64_MAX - (startRank * 2))return UINT64_MAX;
	if (endRank * (endRank - 1) > UINT64_MAX - (endRank * 2))return UINT64_MAX;
	//see https://www.desmos.com/calculator/4ywrmdkv6p
	unsigned long long startSum = 200 * startRank + 5 * ((startRank * startRank + startRank)/2);
	unsigned long long endSum = 200 * endRank + 5 * ((endRank * endRank + endRank) / 2);

	return endSum - startSum;
}

/// <summary>
/// Calculates the rank required to achieve the target experience.
/// </summary>
/// <param name="targetXP">Desired experience to convert.</param>
/// <returns>The rank that will have the given experience.</returns>
extern "C" DLLEXPORT unsigned long long experienceToRank(unsigned long long targetXP) {
	unsigned long long counter{ 0 }; 
	unsigned long long i{ 0 };
	//not really necessary, but should theoretically help with calculation time
	if (targetXP > 500500000) { i = 1000; counter = 499500000; }
	if (targetXP > 5000050000000) { i = 100000; counter = 4999950000000; }
	for (; i < targetXP; ++i) {
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
extern "C" DLLEXPORT unsigned long long rankToExperienceGeneral(unsigned long long startRank, unsigned long long startRankProgress, unsigned long long targetRank) {
	unsigned long long requirement{ 0 };
	
	unsigned long long startSum = 1000 * ((startRank * startRank + startRank) / 2);
	unsigned long long endSum = 1000 * ((targetRank * targetRank + targetRank) / 2);
	//for (unsigned long long i = 0; i <= targetRank; i++)requirement += i * (ULONG)1000;
	//for (unsigned long long j = 0; j <= startRank; j++)requirement -= j * (ULONG)1000;
	//requirement -= startRankProgress;
	return endSum - startSum - startRankProgress;
}

/// <summary>
/// Calculatest the experience gained from rank 0 to targetRank.
/// </summary>
/// <param name="targetRank">The target rank.</param>
/// <returns>Returns the experience gained or needed to go from rank 0 to targetRank.</returns>
extern "C" DLLEXPORT unsigned long long rankToExperienceDefault(unsigned long long targetRank) {
	return rankToExperienceGeneral(0, 0, targetRank);
}



